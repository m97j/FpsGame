using UnityEngine;

public class ZombieMoveState : IState
{
    private readonly Animator anim;
    private readonly ZombieController controller;
    private readonly StateMachine sm;

    public ZombieMoveState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("IdleToMove");

    public void Tick()
    {
        // 타겟이 있으면 추적
        if (controller.HasTarget() && controller.perception.detectedTarget != null)
        {
            controller.agent.SetDestination(controller.perception.detectedTarget.position);
        }

        // 공격 범위 안에 들어오면 공격 상태로 전환
        if (controller.IsTargetInAttackRange())
        {
            sm.ChangeState<ZombieAttackState>();
        }
        // 타겟을 잃으면 Idle 상태로 전환
        else if (!controller.HasTarget())
        {
            sm.ChangeState<ZombieIdleState>();
        }
    }

    public void Exit() { }
}
