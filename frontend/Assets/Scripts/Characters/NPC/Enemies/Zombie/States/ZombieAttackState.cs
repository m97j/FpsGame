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
        // 공격 실행 (여기서는 기본적으로 경공격 사용)
        // 필요하다면 랜덤으로 Light/Heavy 선택 가능
        if (Random.value < 0.7f)
            controller.AttackLight();
        else
            controller.AttackHeavy();

        // 공격 범위 벗어나면 이동 상태로 전환
        if (!controller.IsTargetInAttackRange())
            sm.ChangeState<ZombieMoveState>();
    }

    public void Exit() { }
}
