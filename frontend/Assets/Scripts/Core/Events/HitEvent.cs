using System;
using UnityEngine;

public static class HitEvent
{
    // target: 피격된 오브젝트, damage: 데미지 값, hitPoint: 피격 위치
    public static event Action<GameObject, float, Vector3> OnHit;

    public static void Raise(GameObject target, float damage, Vector3 hitPoint)
    {
        OnHit?.Invoke(target, damage, hitPoint);
    }
}
