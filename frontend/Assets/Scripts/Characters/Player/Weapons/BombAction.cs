using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;
    public int attackPower = 10;
    public float explosionRadius = 5f;

    private static readonly Collider[] overlapResults = new Collider[10]; // Pre-allocated array for OverlapSphereNonAlloc

    private void OnCollisionEnter(Collision collision)
    {
        // 폭발 범위 내의 모든 콜라이더 탐색 (OverlapSphereNonAlloc 사용)
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, overlapResults, 1 << 10);
        for (int i = 0; i < hitCount; i++)
        {
            // EnemyFSM 직접 참조 대신 IDamageable 인터페이스 사용
            if (overlapResults[i].TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(attackPower);
            }
        }

        // 폭발 이펙트 생성
        GameObject eff = Instantiate(bombEffect);
        eff.transform.position = transform.position;

        Destroy(gameObject);
    }
}
