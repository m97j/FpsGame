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
        // ü���� ������ ���� ���·� ��ȯ
        if (controller.IsLowHp())
        {
            sm.ChangeState<ZombieReturnState>();
        }
        // Ÿ���� �����Ǹ� �̵� ���·� ��ȯ
        else if (controller.HasTarget())
        {
            sm.ChangeState<ZombieMoveState>();
        }
    }

    public void Exit() { }
}
