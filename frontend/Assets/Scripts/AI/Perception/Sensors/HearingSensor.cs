using UnityEngine;

/// <summary>
/// û�� ��� ����
/// - �ܼ� ���� ���� + �Ÿ� ��� �ŷڵ�
/// - ���� Ÿ�� �� ���� ����� �� ����
/// </summary>
public class HearingSensor : SensorBase
{
    [Header("Hearing Settings")]
    public LayerMask targetMask;

    // === NonAlloc ���� ===
    private readonly Collider[] hitBuffer = new Collider[20]; // �ʿ信 ���� ũ�� ����

    public override bool Sense(out SensorHit hit)
    {
        hit = new SensorHit
        {
            target = null,
            distance = Mathf.Infinity,
            angle = 0f,
            confidence = 0f,
            sensorType = "Hearing"
        };

        // NonAlloc ���� ���
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, range, hitBuffer, targetMask);

        for (int i = 0; i < hitCount; i++)
        {
            var h = hitBuffer[i];
            float dist = Vector3.Distance(transform.position, h.transform.position);

            if (dist < hit.distance)
            {
                float confidence = 1f - Mathf.Clamp01(dist / range);

                hit.target = h.transform;
                hit.distance = dist;
                hit.angle = 0f; // û���� ���� ���ǹ�
                hit.confidence = confidence;
            }
        }

        return hit.target != null;
    }
}
