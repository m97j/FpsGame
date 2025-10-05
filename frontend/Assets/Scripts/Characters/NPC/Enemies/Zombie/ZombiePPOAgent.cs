using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

/// <summary>
/// Perception ��� ��ȭ�� PPO ���� ������Ʈ (���� ������Ʈ ������ ����)
/// - ���� �б� discrete �ൿ(�̵�/ȸ��/���� ��Ʈ/���� Ÿ��)
/// - �Ÿ�/����/LoS/��� ���/��ֹ� ��� Reward shaping
/// - FSM/Controller ���� ��(���� ����/����/����/Ÿ�� ���)
/// - Inspector Ʃ�� ������ ����/�г�Ƽ/������ �Ķ����
/// </summary>
[RequireComponent(typeof(ZombieController))]
[RequireComponent(typeof(PerceptionSystem))]
public class ZombiePPOAgent : Agent
{
    private ZombieController controller;
    private PerceptionSystem perception;

    // ===== Reward weights (tunable in Inspector) =====
    [Header("Reward weights")]
    public float rwSeeTarget = 0.01f;         // Ÿ�� �þ� Ȯ��/����
    public float rwApproach = 0.02f;          // ��ǥ�� �������
    public float rwAngleAlign = 0.01f;        // ���� ���� ����
    public float rwFlankBonus = 0.02f;        // ����/�Ĺ� ���� Ȯ��
    public float rwPathEfficiency = 0.01f;    // NavMesh ��� ��� ����
    public float rwLoSQuality = 0.01f;        // �þ� ǰ��(���� ����)

    public float rwAttackLightHit = 0.6f;     // ����� ���� ����
    public float rwAttackHeavyHit = 1.1f;     // ������ ���� ����

    // ===== Penalties =====
    [Header("Penalties")]
    public float pnIdleLong = -0.006f;        // �ǹ� ���� ����
    public float pnNoTarget = -0.001f;        // Ÿ�� ��� ����
    public float pnWallHit = -0.004f;         // ��ֹ� ����/�浹 ����
    public float pnDeath = -1.0f;             // ���
    public float pnWhiffLight = -0.02f;       // ����� ���
    public float pnWhiffHeavy = -0.04f;       // ������ ���
    public float pnWasteCooldown = -0.01f;    // ��ٿ� ����(�ɼ�)

    // ===== Config =====
    [Header("Config")]
    public float idleThresholdSec = 2.5f;
    public float flankAngleMin = 60f;       // ���� ���ʽ� ���� ����
    public float probeRayDist = 2.0f;
    public LayerMask obstacleMask;

    // ===== Normalization scales (Inspector Ʃ�׿�) =====
    [Header("Observation scales")]
    public float maxTargetDist = 30f;       // �Ÿ� ����ȭ ����
    public float maxPathCost = 60f;         // ��� ��� ����ȭ ����
    public float maxIdleTime = 5f;          // Idle �ð� ����ȭ ����

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
        // === Ÿ�� ���� ===
        bool hasTarget = perception.HasTarget();
        sensor.AddObservation(hasTarget ? 1f : 0f);

        if (hasTarget && perception.detectedTarget != null)
        {
            Vector3 dir = perception.GetTargetDirection();
            float dist = perception.GetTargetDistance();
            Vector3 vel = perception.GetTargetVelocity();
            Vector3 acc = perception.GetTargetAcceleration();
            float confidence = perception.GetTargetConfidence();

            // ���� (XZ ���)
            sensor.AddObservation(dir.x);
            sensor.AddObservation(dir.z);

            // �Ÿ� (����ȭ)
            sensor.AddObservation(Mathf.Clamp01(dist / 30f));

            // �ӵ�/���� (����ȭ)
            sensor.AddObservation(Mathf.Clamp(vel.x / 10f, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(vel.z / 10f, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(acc.x / 20f, -1f, 1f));
            sensor.AddObservation(Mathf.Clamp(acc.z / 20f, -1f, 1f));

            // ���� �ŷڵ�
            sensor.AddObservation(confidence);

            // �þ߰�/LoS ����
            sensor.AddObservation(perception.IsTargetInFOV() ? 1f : 0f);
            sensor.AddObservation(perception.HasLineOfSight() ? 1f : 0f);
        }
        else
        {
            // Ÿ�� ���� �� �⺻��
            sensor.AddObservation(0f); // dir.x
            sensor.AddObservation(0f); // dir.z
            sensor.AddObservation(1f); // �Ÿ�=�ִ�
            sensor.AddObservation(0f); // vel.x
            sensor.AddObservation(0f); // vel.z
            sensor.AddObservation(0f); // acc.x
            sensor.AddObservation(0f); // acc.z
            sensor.AddObservation(0f); // confidence
            sensor.AddObservation(0f); // FOV
            sensor.AddObservation(0f); // LoS
        }

        // === Ŀ�� ���� ===
        Transform cover = perception.FindNearestCover();
        if (cover != null)
        {
            float coverDist = Vector3.Distance(transform.position, cover.position);
            sensor.AddObservation(Mathf.Clamp01(coverDist / 20f));
        }
        else
        {
            sensor.AddObservation(1f); // Ŀ�� ���� �� �ִ� �Ÿ�
        }

        // === ���� ���� ===
        sensor.AddObservation(controller.hp / (float)controller.maxHp); // ü�� ����
        sensor.AddObservation(controller.AttackCooldownNorm());        // ���� ��ٿ� [0~1]

        // === Idle �ð� ===
        sensor.AddObservation(Mathf.Clamp01(idleTimer / 5f));

        // === ������ �ൿ ��� ===
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

        // === �ൿ ���� ===
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

        // === ���� ���� ===
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

            // 1) Ÿ�� ���� ����
            AddReward(rwSeeTarget * conf);

            // 2) LoS / FOV ����
            if (perception.IsTargetInFOV()) AddReward(0.01f);
            if (perception.HasLineOfSight()) AddReward(rwLoSQuality);

            // 3) ���� ���� (�Ÿ� ����)
            if (distNow < lastDist) AddReward(rwApproach);

            // 4) ���� ���� ���� ����
            if (angleNow < lastAngleToTarget) AddReward(rwAngleAlign);

            // 5) �÷�ŷ ���ʽ�
            if (angleNow > flankAngleMin && angleNow < 180f) AddReward(rwFlankBonus);

            // 6) ��� ȿ�� ����
            float pathCostNow = controller.EstimatePathCostToTarget(perception.detectedTarget);
            if (pathCostNow < lastPathCost) AddReward(rwPathEfficiency);

            // 7) �ӵ� ���� ���� (��� �ӵ� ���� ���̸� ����)
            Vector3 relVel = perception.GetTargetVelocity() - controller.agent.velocity;
            if (relVel.magnitude < 1.0f) AddReward(0.01f);

            // 8) Ŀ�� Ȱ�� ����
            Transform cover = perception.FindNearestCover();
            if (cover != null)
            {
                float coverDist = Vector3.Distance(transform.position, cover.position);
                if (coverDist < 3f) AddReward(0.02f); // Ŀ�� ��ó ����
            }
        }

        // === Idle �г�Ƽ ===
        if (move == 0 && turn == 0 && attack == 0)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleThresholdSec) AddReward(pnIdleLong);
        }
        else idleTimer = 0f;

        // === ��ֹ� �浹 �г�Ƽ ===
        if (RayProbe(transform.position, transform.forward, probeRayDist) > 0.5f)
        {
            AddReward(pnWallHit);
        }

        // === ��� �� ���Ǽҵ� ���� ===
        if (controller.hp <= 0)
        {
            AddReward(pnDeath);
            EndEpisode();
        }

        // === ���� ������Ʈ ===
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

    // ===== Hooks from Controller/FSM (���� �̺�Ʈ�� ȣ��) =====
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
        // ���ط��� 0~0.2 ������ ����ȭ�Ͽ� ���Ƽ �ο�
        AddReward(-Mathf.Clamp(dmgAmount / 10f, 0f, 0.2f));
    }

    public void OnTargetLost()
    {
        AddReward(pnNoTarget * 5f);
    }

    // === �߰� ���� �̺�Ʈ �� ===
    public void OnEnterCover()
    {
        // Ŀ���� ���� �ҷ� ����
        AddReward(0.05f);
    }

    public void OnExitCover()
    {
        // Ŀ������ �ʹ� ���� ������ �ణ�� �г�Ƽ
        AddReward(-0.02f);
    }

    public void OnFlankSuccess()
    {
        // ����/�Ĺ濡�� ���� ���� �� ���ʽ�
        AddReward(0.1f);
    }

    // ===== Helpers =====
    private float Norm(float v, float min, float max)
    {
        if (max <= min) return 0f;
        return Mathf.Clamp01((v - min) / (max - min));
    }

    /// <summary>
    /// ���� RayProbe (���� �浹 ����)
    /// </summary>
    private float RayProbe(Vector3 origin, Vector3 dir, float dist)
    {
        return Physics.Raycast(origin + Vector3.up * 0.5f, dir, dist, obstacleMask) ? 1f : 0f;
    }

    /// <summary>
    /// ���� RayProbe (��/��/��/�� ���� ���� ��հ�)
    /// ���� ��γ� ��ֹ� �������� �����ϴ� �� ����
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
        return (float)hitCount / dirs.Length; // 0~1 ���� ��
    }

    /// <summary>
    /// ��Ƽ ���� ��� LoS ǰ�� ���
    /// ��ü/��ü/��/�� �������� Ray�� �� ���
    /// </summary>
    private float ComputeLoSQuality(Vector3 from, Vector3 to)
    {
        Vector3[] offsets = new Vector3[]
        {
        Vector3.up * 1.6f, // �� ����
        Vector3.up * 0.9f, // ���� ����
        Vector3.up * 0.3f, // �㸮 ����
        Vector3.up * 1.6f + transform.right * 0.3f, // ������ ���
        Vector3.up * 1.6f - transform.right * 0.3f  // ���� ���
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

        return (float)clearCount / offsets.Length; // 0~1 ���� ��
    }
}
