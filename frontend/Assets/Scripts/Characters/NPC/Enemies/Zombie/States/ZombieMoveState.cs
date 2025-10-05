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
        // Ÿ���� ������ ����
        if (controller.HasTarget() && controller.perception.detectedTarget != null)
        {
            controller.agent.SetDestination(controller.perception.detectedTarget.position);
        }

        // ���� ���� �ȿ� ������ ���� ���·� ��ȯ
        if (controller.IsTargetInAttackRange())
        {
            sm.ChangeState<ZombieAttackState>();
        }
        // Ÿ���� ������ Idle ���·� ��ȯ
        else if (!controller.HasTarget())
        {
            sm.ChangeState<ZombieIdleState>();
        }
    }

    public void Exit() { }
}
