using System;

public class ConditionNode : BTNode
{
    private readonly Func<State> condition; // 'readonly' 추가하여 IDE0044 경고 해결

    public ConditionNode(Func<State> condition)
    {
        this.condition = condition;
    }

    public override State Tick()
    {
        return condition != null ? condition() : State.Failure;
    }
}
