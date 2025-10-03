using UnityEngine;

public class VisionSensor : SensorBase
{
    public float fov = 90f; // �þ߰�
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public override bool Sense(out Transform target)
    {
        target = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, range, targetMask);
        foreach (var hit in hits)
        {
            Vector3 dir = (hit.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dir) < fov * 0.5f)
            {
                // �þ� ���� �ְ�, ��ֹ� ������ üũ
                if (!Physics.Linecast(transform.position, hit.transform.position, obstacleMask))
                {
                    target = hit.transform;
                    return true;
                }
            }
        }
        return false;
    }
}
