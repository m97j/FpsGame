using UnityEngine;

public class FSMAgent : BaseAgent
{
    protected StateMachine stateMachine;

    public override void Initialize()
    {
        stateMachine = new StateMachine();
    }

    public override void Tick()
    {
        stateMachine?.Tick();
    }
}
