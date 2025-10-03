using System.Collections.Generic;

public class Selector : BTNode
{
    private readonly List<BTNode> children = new();

    public Selector(params BTNode[] nodes)
    {
        children.AddRange(nodes);
    }

    public override State Tick()
    {
        foreach (var child in children)
        {
            var result = child.Tick();
            if (result == State.Success || result == State.Running)
                return result;
        }
        return State.Failure;
    }
}
