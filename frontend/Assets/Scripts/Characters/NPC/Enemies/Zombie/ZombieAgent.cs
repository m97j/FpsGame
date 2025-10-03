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

        // === BT 트리 구성 ===
        // 루트: Selector (조건에 맞는 행동을 선택)
        BTNode root = new Selector(
            // 1. 사망 조건
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
            // 2. 저체력 → 도망
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
            // 3. 공격 범위 내 → 공격
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
            // 4. 플레이어 탐지됨 → 추적
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
            // 5. 기본 Idle
            new ActionNode(() =>
            {
                fsm.ChangeState<ZombieIdleState>();
                return BTNode.State.Success;
            })
        );

        bt.SetRoot(root);
    }
}
