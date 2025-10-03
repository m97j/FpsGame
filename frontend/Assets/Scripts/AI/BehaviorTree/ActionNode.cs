using System;

public class ActionNode : BTNode
{
    private readonly Func<State> action; // �ʵ带 �б� �������� ����

    public ActionNode(Func<State> action)
    {
        this.action = action;
    }

    public override State Tick()
    {
        return action != null ? action() : State.Failure;
    }
}
