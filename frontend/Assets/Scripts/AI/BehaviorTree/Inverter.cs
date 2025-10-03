public class Inverter : BTNode
{
    private BTNode child;

    public Inverter(BTNode child)
    {
        this.child = child;
    }

    public override State Tick()
    {
        var result = child.Tick();
        if (result == State.Success) return State.Failure;
        if (result == State.Failure) return State.Success;
        return State.Running;
    }
}
