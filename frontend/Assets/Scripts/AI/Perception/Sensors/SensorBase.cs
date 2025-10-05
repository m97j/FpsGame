using UnityEngine;

/// <summary>
/// 모든 센서의 공통 기반 클래스
/// - range: 감지 거리
/// - Sense(): 감지 성공 여부와 SensorHit 정보 반환
/// </summary>
public abstract class SensorBase : MonoBehaviour
{
    [Header("Sensor Settings")]
    public float range = 10f;

    /// <summary>
    /// 감지 결과 구조체
    /// </summary>
    public struct SensorHit
    {
        public Transform target;     // 감지된 대상
        public float distance;       // 거리
        public float angle;          // 시야각 기준 각도
        public float confidence;     // 감지 신뢰도 (0~1)
        public string sensorType;    // Vision/Hearing 등
    }

    /// <summary>
    /// 감지 시도
    /// </summary>
    public abstract bool Sense(out SensorHit hit);
}
