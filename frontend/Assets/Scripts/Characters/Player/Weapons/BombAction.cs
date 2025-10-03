using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;
    public int attackPower = 10;
    public float explosionRadius = 5f;

    private static readonly Collider[] overlapResults = new Collider[10]; // Pre-allocated array for OverlapSphereNonAlloc

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ���� ���� ��� �ݶ��̴� Ž�� (OverlapSphereNonAlloc ���)
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, overlapResults, 1 << 10);
        for (int i = 0; i < hitCount; i++)
        {
            // EnemyFSM ���� ���� ��� IDamageable �������̽� ���
            if (overlapResults[i].TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(attackPower);
            }
        }

        // ���� ����Ʈ ����
        GameObject eff = Instantiate(bombEffect);
        eff.transform.position = transform.position;

        Destroy(gameObject);
    }
}
