using UnityEngine;

public class ZombieReturnState : IState
{
    private Animator anim;
    private ZombieController controller;
    private StateMachine sm;

    public ZombieReturnState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("MoveToIdle");

    public void Tick()
    {
        controller.RunAway();

        if (!controller.IsLowHp())
            sm.ChangeState<ZombieIdleState>();
    }

    public void Exit() { }
}
