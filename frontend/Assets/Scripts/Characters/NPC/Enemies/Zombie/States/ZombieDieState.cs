using UnityEngine;

public class ZombieDieState : IState
{
    private Animator anim;
    private ZombieController controller;
    private StateMachine sm;

    public ZombieDieState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter()
    {
        anim.SetTrigger("Die");
        UnityEngine.Object.Destroy(controller.gameObject, 2f);
    }

    public void Tick() { }
    public void Exit() { }
}
