using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/// <summary>
/// Perception 기반 고도화된 PPO 좀비 에이전트 (단일 에이전트 최종본 예시)
/// - 다중 분기 discrete 행동(이동/회전/전술 힌트/공격 타입)
/// - 거리/각도/LoS/경로 비용/장애물 기반 Reward shaping
/// - FSM/Controller 연동 훅(공격 성공/실패/피해/타깃 상실)
/// - Inspector 튜닝 가능한 보상/패널티/스케일 파라미터
/// </summary>
[RequireComponent(typeof(ZombieController))]
[RequireComponent(typeof(PerceptionSystem))]
public class ZombiePPOAgent : Agent
{
    private ZombieController controller;
    private PerceptionSystem perception;

    // ===== Reward weights (tunable in Inspector) =====
    [Header("Reward weights")]
    public float rwSeeTarget = 0.01f;         // 타겟 시야 확보/유지
    public float rwApproach = 0.02f;          // 목표에 가까워짐
    public float rwAngleAlign = 0.01f;        // 정면 각도 정렬
    public float rwFlankBonus = 0.02f;        // 측면/후방 각도 확보
    public float rwPathEfficiency = 0.01f;    // NavMesh 경로 비용 감소
    public float rwLoSQuality = 0.01f;        // 시야 품질(가림 없음)

    public float rwAttackLightHit = 0.6f;     // 경공격 성공 보상
    public float rwAttackHeavyHit = 1.1f;     // 강공격 성공 보상

    // ===== Penalties =====
    [Header("Penalties")]
    public float pnIdleLong = -0.006f;        // 의미 없는 정지
    public float pnNoTarget = -0.001f;        // 타겟 상실 상태
    public float pnWallHit = -0.004f;         // 장애물 접근/충돌 위험
    public float pnDeath = -1.0f;             // 사망
    public float pnWhiffLight = -0.02f;       // 경공격 허공
    public float pnWhiffHeavy = -0.04f;       // 강공격 허공
    public float pnWasteCooldown = -0.01f;    // 쿨다운 낭비(옵션)

    // ===== Config =====
    [Header("Config")]
    public float idleThresholdSec = 2.5f;
    public float flankAngleMin = 60f;       // 측면 보너스 시작 각도
    public float probeRayDist = 2.0f;
    public LayerMask obstacleMask;

    // ===== Normalization scales (Inspector 튜닝용) =====
    [Header("Observation scales")]
    public float maxTargetDist = 30f;       // 거리 정규화 상한
    public float maxPathCost = 60f;         // 경로 비용 정규화 상한
    public float maxIdleTime = 5f;          // Idle 시간 정규화 상한

    // Episode tracking
    private Vector3 lastPos;
    private float idleTimer;
    private float lastDist;
    private float lastPathCost;
    private float lastAngleToTarget;

    // Last actions (for observation summarization)
    private int lastMove, lastTurn, lastTactic, lastAttack;

    public override void Initialize()
    {
        controller = GetComponent<ZombieController>();
        perception = GetComponent<PerceptionSystem>();
        lastPos = transform.position;

        lastDist = maxTargetDist;
        lastPathCost = maxPathCost;
        lastAngleToTarget = 180f;
        idleTimer = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // === 타겟 관련 ===
        bool hasTarget = perception.HasTarget();
        sensor.AddObservation(hasTarget ? 1f : 0f);

        if (hasTarget && perception.detectedTarget != null)
        {
            Vector3 dir = perception.GetTargetDirection();
            float dist = perception.GetTargetDistance();
            Vector3 vel = perception.GetTargetVelocity();
            Vector3 acc = perception.GetTargetAcceleration();
            float confidence = perception.GetTargetConfidence();

            // 방향 (XZ 평면)
            sensor.AddObservation(dir.x);
            sensor.AddObservation(dir.z);

            // 거리 (정규화)
            sensor.AddObservation(Mathf.Clamp01(dist / 30f));

            // 속도/가속 (정규화)
            sensor.AddObservation(Mathf.Clamp(vel.x / 10f, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(vel.z / 10f, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(acc.x / 20f, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(acc.z / 20f, -1f, 1f));

            // 감지 신뢰도
            sensor.AddObservation(confidence);

            // 시야각/LoS 여부
            sensor.AddObservation(perception.IsTargetInFOV() ? 1f : 0f);
            sensor.AddObservation(perception.HasLineOfSight() ? 1f : 0f);
        }
        else
        {
            // 타겟 없음 → 기본값
            sensor.AddObservation(0f); // dir.x
            sensor.AddObservation(0f); // dir.z
            sensor.AddObservation(1f); // 거리=최대
            sensor.AddObservation(0f); // vel.x
            sensor.AddObservation(0f); // vel.z
            sensor.AddObservation(0f); // acc.x
            sensor.AddObservation(0f); // acc.z
            sensor.AddObservation(0f); // confidence
            sensor.AddObservation(0f); // FOV
            sensor.AddObservation(0f); // LoS
        }

        // === 커버 관련 ===
        Transform cover = perception.FindNearestCover();
        if (cover != null)
        {
            float coverDist = Vector3.Distance(transform.position, cover.position);
            sensor.AddObservation(Mathf.Clamp01(coverDist / 20f));
        }
        else
        {
            sensor.AddObservation(1f); // 커버 없음 → 최대 거리
        }

        // === 좀비 상태 ===
        sensor.AddObservation(controller.hp / (float)controller.maxHp); // 체력 비율
        sensor.AddObservation(controller.AttackCooldownNorm());        // 공격 쿨다운 [0~1]

        // === Idle 시간 ===
        sensor.AddObservation(Mathf.Clamp01(idleTimer / 5f));

        // === 마지막 행동 요약 ===
        sensor.AddObservation(Norm(lastMove, 0f, 4f));
        sensor.AddObservation(Norm(lastTurn, 0f, 2f));
        sensor.AddObservation(Norm(lastTactic, 0f, 2f));
        sensor.AddObservation(Norm(lastAttack, 0f, 2f));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Branches:
        // Move  : 0=Idle,1=Forward,2=Backward,3=StrafeL,4=StrafeR
        // Turn  : 0=None,1=Left,2=Right
        // Tactic: 0=Chase,1=Circle,2=Retreat
        // Attack: 0=None,1=Light,2=Heavy
        int move = actions.DiscreteActions[0];
        int turn = actions.DiscreteActions[1];
        int tactic = actions.DiscreteActions[2];
        int attack = actions.DiscreteActions[3];

        // === 행동 실행 ===
        switch (move)
        {
            case 0: controller.Idle(); break;
            case 1: controller.MoveForward(); break;
            case 2: controller.MoveBackward(); break;
            case 3: controller.Strafe(-1); break;
            case 4: controller.Strafe(1); break;
        }

        switch (turn)
        {
            case 1: controller.Turn(-1); break;
            case 2: controller.Turn(1); break;
        }

        switch (tactic)
        {
            case 0: controller.SetTacticChase(); break;
            case 1: controller.SetTacticCircle(); break;
            case 2: controller.SetTacticRetreat(); break;
        }

        switch (attack)
        {
            case 1: controller.AttackLight(); break;
            case 2: controller.AttackHeavy(); break;
        }

        // === 보상 설계 ===
        bool hasTarget = perception.HasTarget();
        if (!hasTarget)
        {
            AddReward(pnNoTarget);
        }
        else
        {
            float distNow = perception.GetTargetDistance();
            float angleNow = Vector3.Angle(transform.forward, perception.GetTargetDirection());
            float conf = perception.GetTargetConfidence();

            // 1) 타겟 감지 보상
            AddReward(rwSeeTarget * conf);

            // 2) LoS / FOV 보상
            if (perception.IsTargetInFOV()) AddReward(0.01f);
            if (perception.HasLineOfSight()) AddReward(rwLoSQuality);

            // 3) 접근 보상 (거리 감소)
            if (distNow < lastDist) AddReward(rwApproach);

            // 4) 정면 각도 정렬 보상
            if (angleNow < lastAngleToTarget) AddReward(rwAngleAlign);

            // 5) 플랭킹 보너스
            if (angleNow > flankAngleMin && angleNow < 180f) AddReward(rwFlankBonus);

            // 6) 경로 효율 보상
            float pathCostNow = controller.EstimatePathCostToTarget(perception.detectedTarget);
            if (pathCostNow < lastPathCost) AddReward(rwPathEfficiency);

            // 7) 속도 추적 보상 (상대 속도 차이 줄이면 보상)
            Vector3 relVel = perception.GetTargetVelocity() - controller.agent.velocity;
            if (relVel.magnitude < 1.0f) AddReward(0.01f);

            // 8) 커버 활용 보상
            Transform cover = perception.FindNearestCover();
            if (cover != null)
            {
                float coverDist = Vector3.Distance(transform.position, cover.position);
                if (coverDist < 3f) AddReward(0.02f); // 커버 근처 유지
            }
        }

        // === Idle 패널티 ===
        if (move == 0 && turn == 0 && attack == 0)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleThresholdSec) AddReward(pnIdleLong);
        }
        else idleTimer = 0f;

        // === 장애물 충돌 패널티 ===
        if (RayProbe(transform.position, transform.forward, probeRayDist) > 0.5f)
        {
            AddReward(pnWallHit);
        }

        // === 사망 시 에피소드 종료 ===
        if (controller.hp <= 0)
        {
            AddReward(pnDeath);
            EndEpisode();
        }

        // === 상태 업데이트 ===
        lastMove = move;
        lastTurn = turn;
        lastTactic = tactic;
        lastAttack = attack;
        lastDist = perception.GetTargetDistance();
        lastAngleToTarget = Vector3.Angle(transform.forward, perception.GetTargetDirection());
        lastPathCost = controller.EstimatePathCostToTarget(perception.detectedTarget);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var da = actionsOut.DiscreteActions;
        da[0] = 0; // Move
        da[1] = 0; // Turn
        da[2] = 0; // Tactic
        da[3] = 0; // Attack

        if (Input.GetKey(KeyCode.W)) da[0] = 1;
        if (Input.GetKey(KeyCode.S)) da[0] = 2;
        if (Input.GetKey(KeyCode.A)) { da[0] = 3; da[1] = 1; }
        if (Input.GetKey(KeyCode.D)) { da[0] = 4; da[1] = 2; }

        if (Input.GetKey(KeyCode.Q)) da[2] = 1; // circle
        if (Input.GetKey(KeyCode.E)) da[2] = 2; // retreat

        if (Input.GetKey(KeyCode.Space)) da[3] = 1; // light
        if (Input.GetKey(KeyCode.LeftShift)) da[3] = 2; // heavy
    }

    // ===== Hooks from Controller/FSM (연동 이벤트로 호출) =====
    public void OnAttackLightHit()
    {
        AddReward(rwAttackLightHit);
    }

    public void OnAttackHeavyHit()
    {
        AddReward(rwAttackHeavyHit);
    }

    public void OnAttackLightWhiff()
    {
        AddReward(pnWhiffLight);
    }

    public void OnAttackHeavyWhiff()
    {
        AddReward(pnWhiffHeavy);
    }

    public void OnCooldownWasted()
    {
        AddReward(pnWasteCooldown);
    }

    public void OnDamageTaken(float dmgAmount)
    {
        // 피해량을 0~0.2 범위로 정규화하여 페널티 부여
        AddReward(-Mathf.Clamp(dmgAmount / 10f, 0f, 0.2f));
    }

    public void OnTargetLost()
    {
        AddReward(pnNoTarget * 5f);
    }

    // === 추가 전술 이벤트 훅 ===
    public void OnEnterCover()
    {
        // 커버에 들어가면 소량 보상
        AddReward(0.05f);
    }

    public void OnExitCover()
    {
        // 커버에서 너무 빨리 나오면 약간의 패널티
        AddReward(-0.02f);
    }

    public void OnFlankSuccess()
    {
        // 측면/후방에서 공격 성공 시 보너스
        AddReward(0.1f);
    }

    // ===== Helpers =====
    private float Norm(float v, float min, float max)
    {
        if (max <= min) return 0f;
        return Mathf.Clamp01((v - min) / (max - min));
    }

    /// <summary>
    /// 단일 RayProbe (전방 충돌 여부)
    /// </summary>
    private float RayProbe(Vector3 origin, Vector3 dir, float dist)
    {
        return Physics.Raycast(origin + Vector3.up * 0.5f, dir, dist, obstacleMask) ? 1f : 0f;
    }

    /// <summary>
    /// 다중 RayProbe (좌/우/상/하 방향 포함 평균값)
    /// 좁은 통로나 장애물 밀집도를 감지하는 데 유용
    /// </summary>
    private float MultiRayProbe(Vector3 origin, float dist)
    {
        Vector3[] dirs = new Vector3[]
        {
        transform.forward,
        -transform.forward,
        transform.right,
        -transform.right,
        (transform.forward + transform.right).normalized,
        (transform.forward - transform.right).normalized
        };

        int hitCount = 0;
        foreach (var d in dirs)
        {
            if (Physics.Raycast(origin + Vector3.up * 0.5f, d, dist, obstacleMask))
                hitCount++;
        }
        return (float)hitCount / dirs.Length; // 0~1 사이 값
    }

    /// <summary>
    /// 멀티 레이 기반 LoS 품질 계산
    /// 상체/하체/좌/우 지점에서 Ray를 쏴 평균
    /// </summary>
    private float ComputeLoSQuality(Vector3 from, Vector3 to)
    {
        Vector3[] offsets = new Vector3[]
        {
        Vector3.up * 1.6f, // 눈 높이
        Vector3.up * 0.9f, // 가슴 높이
        Vector3.up * 0.3f, // 허리 높이
        Vector3.up * 1.6f + transform.right * 0.3f, // 오른쪽 어깨
        Vector3.up * 1.6f - transform.right * 0.3f  // 왼쪽 어깨
        };

        int clearCount = 0;
        foreach (var offset in offsets)
        {
            Vector3 origin = from + offset;
            Vector3 targetPos = to + offset;
            Vector3 dir = (targetPos - origin).normalized;
            float dist = Vector3.Distance(origin, targetPos);

            if (!Physics.Raycast(origin, dir, dist, obstacleMask))
                clearCount++;
        }

        return (float)clearCount / offsets.Length; // 0~1 사이 값
    }
}
