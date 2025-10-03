using UnityEngine;
using System.Collections.Generic;

public class PerceptionSystem : MonoBehaviour
{
    private List<SensorBase> sensors = new List<SensorBase>();
    public Transform detectedTarget;

    void Awake()
    {
        sensors.AddRange(GetComponentsInChildren<SensorBase>());
    }

    void Update()
    {
        detectedTarget = null;
        foreach (var sensor in sensors)
        {
            if (sensor.Sense(out Transform target))
            {
                detectedTarget = target;
                break;
            }
        }
    }

    public bool HasTarget() => detectedTarget != null;
}
