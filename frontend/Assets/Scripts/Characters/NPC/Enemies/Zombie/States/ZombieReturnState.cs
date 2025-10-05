using UnityEngine;

public class ZombieReturnState : IState
{
    private readonly Animator anim;
    private readonly ZombieController controller;
    private readonly StateMachine sm;

    public ZombieReturnState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("MoveToIdle");

    public void Tick()
    {
        // ü���� ������ ���� �ൿ ����
        if (controller.IsLowHp())
        {
            // Ŀ���� ������ Ŀ�� ������ �̵�
            if (controller.perception != null && controller.perception.FindNearestCover() != null)
            {
                controller.MoveToCover();
            }
            else
            {
                // Ŀ���� ������ �ܼ��� �ڷ� �̵�
                controller.MoveBackward();
            }
        }
        else
        {
            // ü���� ȸ���Ǹ� Idle ���·� ��ȯ
            sm.ChangeState<ZombieIdleState>();
        }
    }

    public void Exit() { }
}
