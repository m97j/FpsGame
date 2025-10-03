public class Repeater : BTNode
{
    private BTNode child;
    private int repeatCount;
    private int currentCount;

    public Repeater(BTNode child, int repeatCount = -1)
    {
        this.child = child;
        this.repeatCount = repeatCount; // -1�̸� ���� �ݺ�
        this.currentCount = 0;
    }

    public override State Tick()
    {
        if (repeatCount < 0) // ���� �ݺ�
        {
            child.Tick();
            return State.Running;
        }

        if (currentCount < repeatCount)
        {
            var result = child.Tick();
            if (result == State.Success || result == State.Failure)
                currentCount++;
            return State.Running;
        }

        return State.Success;
    }
}
