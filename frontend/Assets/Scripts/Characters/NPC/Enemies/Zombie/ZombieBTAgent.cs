using UnityEngine;

[RequireComponent(typeof(ZombieController))]
[RequireComponent(typeof(ZombieFSM))]
[RequireComponent(typeof(BehaviorTree))]
[RequireComponent(typeof(PerceptionSystem))]
public class ZombieAgent : BTAgent   // BTAgent ���
{
    private ZombieController controller;
    private ZombieFSM fsm;
    private PerceptionSystem perception;
    private Blackboard blackboard;

    public override void Initialize()
    {
        controller = GetComponent<ZombieController>();
        fsm = GetComponent<ZombieFSM>();
        perception = GetComponent<PerceptionSystem>();
        blackboard = new Blackboard();

        // === BT Ʈ�� ���� ===
        root = new Selector(
            // 1. ��� ����
            new Sequence(
                new ConditionNode(() =>
                {
                    return controller.hp <= 0 ? BTNode.State.Success : BTNode.State.Failure;
                }),
                new ActionNode(() =>
                {
                    fsm.ChangeState<ZombieDieState>();
                    return BTNode.State.Success;
                })
            ),
            // 2. ��ü�� �� ����
            new Sequence(
                new ConditionNode(() =>
                {
                    return controller.IsLowHp() ? BTNode.State.Success : BTNode.State.Failure;
                }),
                new ActionNode(() =>
                {
                    fsm.ChangeState<ZombieReturnState>();
                    return BTNode.State.Success;
                })
            ),
            // 3. ���� ���� �� �� ����
            new Sequence(
                new ConditionNode(() =>
                {
                    return controller.IsTargetInAttackRange() ? BTNode.State.Success : BTNode.State.Failure;
                }),
                new ActionNode(() =>
                {
                    fsm.ChangeState<ZombieAttackState>();
                    return BTNode.State.Success;
                })
            ),
            // 4. �÷��̾� Ž���� �� ���� (PerceptionSystem Ȱ��)
            new Sequence(
                new ConditionNode(() =>
                {
                    return perception.HasTarget() ? BTNode.State.Success : BTNode.State.Failure;
                }),
                new ActionNode(() =>
                {
                    fsm.ChangeState<ZombieMoveState>();
                    return BTNode.State.Success;
                })
            ),
            // 5. �⺻ Idle
            new ActionNode(() =>
            {
                fsm.ChangeState<ZombieIdleState>();
                return BTNode.State.Success;
            })
        );
    }

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        // Perception ����� Blackboard�� ���
        blackboard.Set("HasTarget", perception.HasTarget());
        blackboard.Set("Target", perception.detectedTarget);

        // BT ����
        Tick();
    }
}
