using UnityEngine;

public class ZombieFSM : MonoBehaviour
{
    private StateMachine stateMachine;
    private Animator anim;
    private ZombieController controller;

    void Awake()
    {
        stateMachine = new StateMachine();
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<ZombieController>();

        // 상태 등록
        stateMachine.AddState(new ZombieIdleState(anim, controller, stateMachine));
        stateMachine.AddState(new ZombieMoveState(anim, controller, stateMachine));
        stateMachine.AddState(new ZombieAttackState(anim, controller, stateMachine));
        stateMachine.AddState(new ZombieReturnState(anim, controller, stateMachine));
        stateMachine.AddState(new ZombieDamagedState(anim, controller, stateMachine));
        stateMachine.AddState(new ZombieDieState(anim, controller, stateMachine));
    }

    public void ChangeState<T>() where T : IState
    {
        stateMachine.ChangeState<T>();
    }

    void Update()
    {
        stateMachine.Tick();
    }
}
