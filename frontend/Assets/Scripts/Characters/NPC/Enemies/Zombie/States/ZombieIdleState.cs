using UnityEngine;

public class ZombieIdleState : IState
{
    private readonly Animator anim;
    private readonly ZombieController controller;
    private readonly StateMachine sm;

    public ZombieIdleState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("MoveToIdle");

    public void Tick()
    {
        // 체력이 낮으면 후퇴 상태로 전환
        if (controller.IsLowHp())
        {
            sm.ChangeState<ZombieReturnState>();
        }
        // 타겟이 감지되면 이동 상태로 전환
        else if (controller.HasTarget())
        {
            sm.ChangeState<ZombieMoveState>();
        }
    }

    public void Exit() { }
}
