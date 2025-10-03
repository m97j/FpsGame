using System;

public class ActionNode : BTNode
{
    private readonly Func<State> action; // 필드를 읽기 전용으로 변경

    public ActionNode(Func<State> action)
    {
        this.action = action;
    }

    public override State Tick()
    {
        return action != null ? action() : State.Failure;
    }
}
