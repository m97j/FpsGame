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

    // === ���� �Լ� ===
    public bool IsLowHp() => hp < lowHpThreshold;
    public bool IsPlayerDetected() => Vector3.Distance(transform.position, player.position) < detectRange;
    public bool IsPlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;

    // === �ൿ �Լ� ===
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
        Debug.Log("���� ���� ����!");

        // ���� ����� IDamageable�̸� ������ ����
        if (player != null && player.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(attackPower);
        }
    }

    public void Idle() { }

    // === IDamageable ���� ===
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp > 0)
        {
            // FSM�� Damaged ���� ��ȯ ��û
            fsm.ChangeState<ZombieDamagedState>();
        }
        else
        {
            fsm.ChangeState<ZombieDieState>();
        }
    }

}
