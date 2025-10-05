using UnityEngine;

/// <summary>
/// ��� ������ ���� ��� Ŭ����
/// - range: ���� �Ÿ�
/// - Sense(): ���� ���� ���ο� SensorHit ���� ��ȯ
/// </summary>
public abstract class SensorBase : MonoBehaviour
{
    [Header("Sensor Settings")]
    public float range = 10f;

    /// <summary>
    /// ���� ��� ����ü
    /// </summary>
    public struct SensorHit
    {
        public Transform target;     // ������ ���
        public float distance;       // �Ÿ�
        public float angle;          // �þ߰� ���� ����
        public float confidence;     // ���� �ŷڵ� (0~1)
        public string sensorType;    // Vision/Hearing ��
    }

    /// <summary>
    /// ���� �õ�
    /// </summary>
    public abstract bool Sense(out SensorHit hit);
}
