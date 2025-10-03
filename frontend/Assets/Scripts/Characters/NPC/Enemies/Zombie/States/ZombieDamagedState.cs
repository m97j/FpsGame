using UnityEngine;

public class ZombieDamagedState : IState
{
    private Animator anim;
    private ZombieController controller;
    private StateMachine sm;

    public ZombieDamagedState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("Damaged");

    public void Tick()
    {
        if (controller.hp <= 0)
            sm.ChangeState<ZombieDieState>();
        else
            sm.ChangeState<ZombieIdleState>();
    }

    public void Exit() { }
}
