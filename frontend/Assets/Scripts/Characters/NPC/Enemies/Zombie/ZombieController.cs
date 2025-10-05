using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour, IDamageable
{
    public NavMeshAgent agent;
    public ZombieFSM fsm;
    [SerializeField] private ZombiePPOAgent agentRL;      // PPOAgent 훅 연결
    public PerceptionSystem perception;  // PerceptionSystem 연동

    [Header("Stats")]
    public int hp = 65;
    public int maxHp = 65;
    public float attackRange = 2f;
    public int lowHpThreshold = 15;
    public int attackPowerLight = 3;
    public int attackPowerHeavy = 6;

    [Header("Attack Cooldowns")]
    public float lightCooldown = 1.0f;
    public float heavyCooldown = 2.5f;
    private float lastAttackTime = -999f;
    private float currentCooldown = 0f;

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (fsm == null) fsm = GetComponent<ZombieFSM>();
        if (agentRL == null) agentRL = GetComponent<ZombiePPOAgent>();
        if (perception == null) perception = GetComponent<PerceptionSystem>();
    }

    // === 조건 함수 ===
    public bool IsLowHp() => hp < lowHpThreshold;
    public bool HasTarget() => perception != null && perception.HasTarget();
    public bool IsTargetInAttackRange()
    {
        if (perception == null || perception.detectedTarget == null) return false;
        return Vector3.Distance(transform.position, perception.detectedTarget.position) < attackRange;
    }

    // === 이동 함수 ===
    public void Idle() { agent.ResetPath(); }

    public void MoveForward()
    {
        Vector3 forward = transform.forward;
        agent.SetDestination(transform.position + forward * 1.5f);
        fsm.ChangeState<ZombieMoveState>();
    }

    public void MoveBackward()
    {
        Vector3 back = -transform.forward;
        agent.SetDestination(transform.position + back * 1.5f);
        fsm.ChangeState<ZombieMoveState>();
    }

    public void Strafe(int dir)
    {
        // dir = -1 (좌), 1 (우)
        Vector3 side = transform.right * dir;
        agent.SetDestination(transform.position + side * 1.5f);
        fsm.ChangeState<ZombieMoveState>();
    }

    public void Turn(int dir)
    {
        transform.Rotate(Vector3.up, dir * 90f * Time.deltaTime);
    }

    // === 전술 힌트 (FSM/AI Steering에 반영) ===
    public void SetTacticChase() { /* 추격 전술 가중치 적용 */ }
    public void SetTacticCircle() { /* 원형 기동 전술 가중치 적용 */ }
    public void SetTacticRetreat() { /* 후퇴 전술 가중치 적용 */ }

    public void MoveToCover()
    {
        Transform cover = perception?.FindNearestCover();
        if (cover != null)
        {
            agent.SetDestination(cover.position);
            fsm.ChangeState<ZombieMoveState>();
            agentRL?.OnEnterCover();
        }
    }

    // === 공격 함수 ===
    public void AttackLight()
    {
        if (Time.time - lastAttackTime < lightCooldown)
        {
            agentRL?.OnCooldownWasted();
            return;
        }

        lastAttackTime = Time.time;
        currentCooldown = lightCooldown;
        fsm.ChangeState<ZombieAttackState>();

        Transform target = perception?.detectedTarget;
        if (target != null && target.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(attackPowerLight);
            agentRL?.OnAttackLightHit();

            // 플랭킹 각도 체크
            float angle = Vector3.Angle(transform.forward, perception.GetTargetDirection());
            if (angle > 60f) agentRL?.OnFlankSuccess();
        }
        else
        {
            agentRL?.OnAttackLightWhiff();
        }
    }

    public void AttackHeavy()
    {
        if (Time.time - lastAttackTime < heavyCooldown)
        {
            agentRL?.OnCooldownWasted();
            return;
        }

        lastAttackTime = Time.time;
        currentCooldown = heavyCooldown;
        fsm.ChangeState<ZombieAttackState>();

        Transform target = perception?.detectedTarget;
        if (target != null && target.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(attackPowerHeavy);
            agentRL?.OnAttackHeavyHit();

            // 플랭킹 각도 체크
            float angle = Vector3.Angle(transform.forward, perception.GetTargetDirection());
            if (angle > 60f) agentRL?.OnFlankSuccess();
        }
        else
        {
            agentRL?.OnAttackHeavyWhiff();
        }
    }

    public float AttackCooldownNorm()
    {
        if (currentCooldown <= 0f) return 0f;
        float elapsed = Time.time - lastAttackTime;
        return Mathf.Clamp01(elapsed / currentCooldown);
    }

    // === 경로 비용 추정 ===
    public float EstimatePathCostToTarget(Transform target)
    {
        if (target == null) return 999f;
        NavMeshPath path = new();
        if (agent.CalculatePath(target.position, path))
        {
            float dist = 0f;
            for (int i = 1; i < path.corners.Length; i++)
                dist += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            return dist;
        }
        return 999f;
    }

    // === IDamageable 구현 ===
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        agentRL?.OnDamageTaken(dmg);

        if (hp > 0)
        {
            fsm.ChangeState<ZombieDamagedState>();
        }
        else
        {
            fsm.ChangeState<ZombieDieState>();
        }
    }

    // === 커버 상태 체크 (FSM에서 호출 가능) ===
    public void CheckCoverStatus()
    {
        Transform cover = perception?.FindNearestCover();
        if (cover != null)
        {
            float dist = Vector3.Distance(transform.position, cover.position);
            if (dist < 2f) agentRL?.OnEnterCover();
            else if (dist > 3f) agentRL?.OnExitCover();
        }
    }
}
