using System;

public class ConditionNode : BTNode
{
    private readonly Func<State> condition; // 'readonly' �߰��Ͽ� IDE0044 ��� �ذ�

    public ConditionNode(Func<State> condition)
    {
        this.condition = condition;
    }

    public override State Tick()
    {
        return condition != null ? condition() : State.Failure;
    }
}
