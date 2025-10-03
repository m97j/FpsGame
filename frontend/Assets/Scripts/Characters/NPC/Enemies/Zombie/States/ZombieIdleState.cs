using UnityEngine;

public class ZombieIdleState : IState
{
    private Animator anim;
    private ZombieController controller;
    private StateMachine sm;

    public ZombieIdleState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("MoveToIdle");

    public void Tick()
    {
        if (controller.IsLowHp())
            sm.ChangeState<ZombieReturnState>();
        else if (controller.IsPlayerDetected())
            sm.ChangeState<ZombieMoveState>();
    }

    public void Exit() { }
}
