using UnityEngine;

public class ZombieAttackState : IState
{
    private readonly Animator anim;
    private readonly ZombieController controller;
    private readonly StateMachine sm;

    public ZombieAttackState(Animator anim, ZombieController controller, StateMachine sm)
    {
        this.anim = anim;
        this.controller = controller;
        this.sm = sm;
    }

    public void Enter() => anim.SetTrigger("StartAttack");

    public void Tick()
    {
        // ���� ���� (���⼭�� �⺻������ ����� ���)
        // �ʿ��ϴٸ� �������� Light/Heavy ���� ����
        if (Random.value < 0.7f)
            controller.AttackLight();
        else
            controller.AttackHeavy();

        // ���� ���� ����� �̵� ���·� ��ȯ
        if (!controller.IsTargetInAttackRange())
            sm.ChangeState<ZombieMoveState>();
    }

    public void Exit() { }
}
