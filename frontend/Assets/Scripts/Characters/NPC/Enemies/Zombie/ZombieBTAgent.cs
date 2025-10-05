using UnityEngine;

[RequireComponent(typeof(ZombieController))]
[RequireComponent(typeof(ZombieFSM))]
[RequireComponent(typeof(BehaviorTree))]
[RequireComponent(typeof(PerceptionSystem))]
public class ZombieAgent : BTAgent   // BTAgent 상속
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

        // === BT 트리 구성 ===
        root = new Selector(
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
                    return controller.IsTargetInAttackRange() ? BTNode.State.Success : BTNode.State.Failure;
                }),
                new ActionNode(() =>
                {
                    fsm.ChangeState<ZombieAttackState>();
                    return BTNode.State.Success;
                })
            ),
            // 4. 플레이어 탐지됨 → 추적 (PerceptionSystem 활용)
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
            // 5. 기본 Idle
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
        // Perception 결과를 Blackboard에 기록
        blackboard.Set("HasTarget", perception.HasTarget());
        blackboard.Set("Target", perception.detectedTarget);

        // BT 실행
        Tick();
    }
}
