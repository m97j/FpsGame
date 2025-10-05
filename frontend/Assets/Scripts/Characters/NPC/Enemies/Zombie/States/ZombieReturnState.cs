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
        // 체력이 낮으면 후퇴 행동 수행
        if (controller.IsLowHp())
        {
            // 커버가 있으면 커버 쪽으로 이동
            if (controller.perception != null && controller.perception.FindNearestCover() != null)
            {
                controller.MoveToCover();
            }
            else
            {
                // 커버가 없으면 단순히 뒤로 이동
                controller.MoveBackward();
            }
        }
        else
        {
            // 체력이 회복되면 Idle 상태로 전환
            sm.ChangeState<ZombieIdleState>();
        }
    }

    public void Exit() { }
}
