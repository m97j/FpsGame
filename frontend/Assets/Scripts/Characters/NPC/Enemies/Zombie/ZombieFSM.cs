using UnityEngine;

public class ZombieFSM : FSMAgent
{
    private Animator anim;
    private ZombieController controller;

    public override void Initialize()
    {
        base.Initialize(); // stateMachine √ ±‚»≠

        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<ZombieController>();

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

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        Tick(); // FSMAgent.Tick() °Ê stateMachine.Tick()
    }
}
