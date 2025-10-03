using System;
using UnityEngine;

public static class HitEvent
{
    // target: �ǰݵ� ������Ʈ, damage: ������ ��, hitPoint: �ǰ� ��ġ
    public static event Action<GameObject, float, Vector3> OnHit;

    public static void Raise(GameObject target, float damage, Vector3 hitPoint)
    {
        OnHit?.Invoke(target, damage, hitPoint);
    }
}
