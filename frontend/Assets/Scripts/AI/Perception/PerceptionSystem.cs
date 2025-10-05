using UnityEngine;
using System.Collections.Generic;

public class PerceptionSystem : MonoBehaviour
{
    private readonly List<SensorBase> sensors = new();

    [Header("Detected Targets")]
    public Transform detectedTarget;                        // 현재 선택된 타겟
    public List<SensorBase.SensorHit> detectedHits = new(); // 모든 센서 감지 결과

    private Vector3 lastTargetPos;
    private Vector3 targetVelocity;
    private Vector3 targetAcceleration;

    [Header("Perception Settings")]
    public string coverTag = "Cover";  // 커버 포인트 태그

    void Awake()
    {
        sensors.AddRange(GetComponentsInChildren<SensorBase>());
    }

    void Update()
    {
        detectedTarget = null;
        detectedHits.Clear();

        // 모든 센서에서 감지 시도
        foreach (var sensor in sensors)
        {
            if (sensor.Sense(out SensorBase.SensorHit hit))
            {
                detectedHits.Add(hit);
            }
        }

        // 가장 신뢰도 높은 타겟 선택
        float bestScore = 0f;
        foreach (var hit in detectedHits)
        {
            if (hit.confidence > bestScore)
            {
                bestScore = hit.confidence;
                detectedTarget = hit.target;
            }
        }

        // 속도/가속 추정
        if (detectedTarget != null)
        {
            Vector3 currentPos = detectedTarget.position;
            Vector3 vel = (currentPos - lastTargetPos) / Mathf.Max(Time.deltaTime, 0.0001f);
            targetAcceleration = (vel - targetVelocity) / Mathf.Max(Time.deltaTime, 0.0001f);
            targetVelocity = vel;
            lastTargetPos = currentPos;
        }
        else
        {
            targetVelocity = Vector3.zero;
            targetAcceleration = Vector3.zero;
        }
    }

    // === 공개 API ===
    public bool HasTarget() => detectedTarget != null;

    public Vector3 GetTargetDirection()
    {
        if (detectedTarget == null) return Vector3.zero;
        return (detectedTarget.position - transform.position).normalized;
    }

    public float GetTargetDistance()
    {
        if (detectedTarget == null) return 999f;
        return Vector3.Distance(transform.position, detectedTarget.position);
    }

    public Vector3 GetTargetVelocity() => targetVelocity;
    public Vector3 GetTargetAcceleration() => targetAcceleration;

    /// <summary>
    /// 현재 선택된 타겟의 감지 신뢰도 (0~1)
    /// </summary>
    public float GetTargetConfidence()
    {
        foreach (var hit in detectedHits)
        {
            if (hit.target == detectedTarget)
                return hit.confidence;
        }
        return 0f;
    }

    /// <summary>
    /// 가장 가까운 커버 포인트 반환
    /// </summary>
    public Transform FindNearestCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag(coverTag);
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var c in covers)
        {
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = c.transform;
            }
        }
        return nearest;
    }

    /// <summary>
    /// 현재 타겟이 시야각 내에 있는지 여부
    /// </summary>
    public bool IsTargetInFOV(float fov = 120f)
    {
        if (detectedTarget == null) return false;
        Vector3 dir = GetTargetDirection();
        return Vector3.Angle(transform.forward, dir) < fov * 0.5f;
    }

    /// <summary>
    /// 현재 타겟과의 라인오브사이트(가림 여부) 체크
    /// </summary>
    public bool HasLineOfSight(LayerMask? maskOverride = null)
    {
        if (detectedTarget == null) return false;

        Vector3 origin = transform.position + Vector3.up * 1.6f; // 눈 높이
        Vector3 targetPos = detectedTarget.position + Vector3.up * 1.6f;
        Vector3 dir = (targetPos - origin).normalized;
        float dist = Vector3.Distance(origin, targetPos);

        LayerMask mask = maskOverride ?? Physics.DefaultRaycastLayers;
        return !Physics.Raycast(origin, dir, dist, mask);
    }

}
