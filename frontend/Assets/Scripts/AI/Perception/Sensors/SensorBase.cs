using UnityEngine;

public abstract class SensorBase : MonoBehaviour
{
    public float range = 10f;
    public abstract bool Sense(out Transform target);
}
