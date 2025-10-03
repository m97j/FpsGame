using UnityEngine;

[RequireComponent(typeof(ZombieController))]
[RequireComponent(typeof(ZombieFSM))]
[RequireComponent(typeof(BehaviorTree))]
public class ZombieAgent : MonoBehaviour
{
    private ZombieController controller;
    private ZombieFSM fsm;
    private BehaviorTree bt;
    private Blackboard blackboard;

    void Awake()
    {
        controller = GetComponent<ZombieController>();
        fsm = GetComponent<ZombieFSM>();
        bt = GetComponent<BehaviorTree>();
        blackboard = new Blackboard();

        // === BT Ʈ�� ���� ===
        // ��Ʈ: Selector (���ǿ� �´� �ൿ�� ����)
        BTNode root = new Selector(
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
                    return controller.IsPlayerInAttackRange() ? BTNode.State.Success : BTNode.State.Failure;
                }),
                new ActionNode(() =>
                {
                    fsm.ChangeState<ZombieAttackState>();
                    return BTNode.State.Success;
                })
            ),
            // 4. �÷��̾� Ž���� �� ����
            new Sequence(
                new ConditionNode(() =>
                {
                    return controller.IsPlayerDetected() ? BTNode.State.Success : BTNode.State.Failure;
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

        bt.SetRoot(root);
    }
}
