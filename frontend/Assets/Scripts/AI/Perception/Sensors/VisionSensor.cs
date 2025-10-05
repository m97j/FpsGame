using UnityEngine;

/// <summary>
/// �þ� ��� ����
/// - �þ߰�, ��ֹ� ���� üũ
/// - ���� �ŷڵ�: �Ÿ�/���� ������� ���
/// </summary>
public class VisionSensor : SensorBase
{
    [Header("Vision Settings")]
    public float fov = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    // === NonAlloc ���� ===
    private readonly Collider[] hitBuffer = new Collider[20]; // �ʿ信 �°� ũ�� ����

    public override bool Sense(out SensorHit hit)
    {
        hit = new SensorHit
        {
            target = null,
            distance = Mathf.Infinity,
            angle = 180f,
            confidence = 0f,
            sensorType = "Vision"
        };

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, range, hitBuffer, targetMask);

        for (int i = 0; i < hitCount; i++)
        {
            var h = hitBuffer[i];
            Vector3 dir = (h.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);
            float dist = Vector3.Distance(transform.position, h.transform.position);

            if (angle < fov * 0.5f)
            {
                if (!Physics.Linecast(transform.position, h.transform.position, obstacleMask))
                {
                    float distScore = 1f - Mathf.Clamp01(dist / range);
                    float angleScore = 1f - Mathf.Clamp01(angle / (fov * 0.5f));
                    float confidence = (distScore + angleScore) * 0.5f;

                    if (dist < hit.distance)
                    {
                        hit.target = h.transform;
                        hit.distance = dist;
                        hit.angle = angle;
                        hit.confidence = confidence;
                    }
                }
            }
        }

        return hit.target != null;
    }
}

