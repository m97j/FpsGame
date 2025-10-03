using UnityEngine;

public class HearingSensor : SensorBase
{
    public LayerMask targetMask;

    public override bool Sense(out Transform target)
    {
        target = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, range, targetMask);
        if (hits.Length > 0)
        {
            target = hits[0].transform;
            return true;
        }
        return false;
    }
}
