using System.Collections.Generic;

public class Sequence : BTNode
{
    private List<BTNode> children = new List<BTNode>();

    public Sequence(params BTNode[] nodes)
    {
        children.AddRange(nodes);
    }

    public override State Tick()
    {
        foreach (var child in children)
        {
            var result = child.Tick();
            if (result == State.Failure) return State.Failure;
            if (result == State.Running) return State.Running;
        }
        return State.Success;
    }
}
