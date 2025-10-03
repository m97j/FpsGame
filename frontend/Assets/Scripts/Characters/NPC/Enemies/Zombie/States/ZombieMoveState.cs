using UnityEngine;

public class ZombieMoveState : IState
{
    private Animator anim;
    private ZombieController controller;
    private StateMachine sm;

    public ZombieMoveState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("IdleToMove");

    public void Tick()
    {
        controller.ChasePlayer();

        if (controller.IsPlayerInAttackRange())
            sm.ChangeState<ZombieAttackState>();
        else if (!controller.IsPlayerDetected())
            sm.ChangeState<ZombieIdleState>();
    }

    public void Exit() { }
}
