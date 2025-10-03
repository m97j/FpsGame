using UnityEngine;

public abstract class BaseAgent : MonoBehaviour
{
    protected bool isActive = true;

    public virtual void Initialize() { }
    public virtual void Tick() { }
    public virtual void Shutdown() { isActive = false; }
}
