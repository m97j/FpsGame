using UnityEngine;

/// <summary>
/// 청각 기반 센서
/// - 단순 범위 감지 + 거리 기반 신뢰도
/// - 여러 타겟 중 가장 가까운 것 선택
/// </summary>
public class HearingSensor : SensorBase
{
    [Header("Hearing Settings")]
    public LayerMask targetMask;

    // === NonAlloc 버퍼 ===
    private readonly Collider[] hitBuffer = new Collider[20]; // 필요에 따라 크기 조정

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

        // NonAlloc 버전 사용
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
                hit.angle = 0f; // 청각은 각도 무의미
                hit.confidence = confidence;
            }
        }

        return hit.target != null;
    }
}
