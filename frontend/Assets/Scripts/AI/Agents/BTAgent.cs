using UnityEngine;

public class BTAgent : BaseAgent
{
    protected BTNode root;

    public override void Tick()
    {
        if (root != null)
            root.Tick();
    }
}
