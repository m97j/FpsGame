using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour, IDamageable
{
    public Transform player;
    public NavMeshAgent agent;
    public ZombieFSM fsm;

    public int hp = 65;
    public int maxHp = 15;
    public float detectRange = 10f;
    public float attackRange = 2f;
    public int lowHpThreshold = 5;
    public int attackPower = 3;

    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (fsm == null) fsm = GetComponent<ZombieFSM>();
    }

    // === 조건 함수 ===
    public bool IsLowHp() => hp < lowHpThreshold;
    public bool IsPlayerDetected() => Vector3.Distance(transform.position, player.position) < detectRange;
    public bool IsPlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;

    // === 행동 함수 ===
    public void RunAway()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + dir * 5f);
    }

    public void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    public void AttackPlayer()
    {
        Debug.Log("좀비 공격 실행!");

        // 공격 대상이 IDamageable이면 데미지 전달
        if (player != null && player.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(attackPower);
        }
    }

    public void Idle() { }

    // === IDamageable 구현 ===
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp > 0)
        {
            // FSM에 Damaged 상태 전환 요청
            fsm.ChangeState<ZombieDamagedState>();
        }
        else
        {
            fsm.ChangeState<ZombieDieState>();
        }
    }

}
