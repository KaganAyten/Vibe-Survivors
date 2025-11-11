using UnityEngine;

/// <summary>
/// Tüm silahlar için temel sınıf
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] protected float itemDamage = 10f;
    [SerializeField] protected float attackRange = 5f;

    [Header("Debug")]
    [SerializeField] protected bool showDebugGizmos = true;

    /// <summary>
    /// Silahın mevcut hasar değeri
    /// </summary>
    public float ItemDamage => itemDamage;

    /// <summary>
    /// Silahın mevcut menzil değeri
    /// </summary>
    public float AttackRange => attackRange;

    /// <summary>
    /// Silahın tetiklenme fonksiyonu - menzil içindeki IDamageable nesnelere hasar verir
    /// Virtual olduğu için türetilen sınıflarda özelleştirilebilir
    /// </summary>
    public virtual void Trigger()
    {
        // Menzil içindeki tüm collider'ları bul
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider hitCollider in hitColliders)
        {
            // IDamageable interface'i olan nesneleri kontrol et
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();

            if (damageable != null && damageable.IsAlive)
            {
                // Hasar ver
                damageable.TakeDamage(itemDamage);

                // Hasar verme efekti (türetilen sınıflarda override edilebilir)
                OnDamageDealt(hitCollider.gameObject);
            }
        }

        // Tetikleme efekti (türetilen sınıflarda override edilebilir)
        OnTrigger();
    }

    /// <summary>
    /// Silahın hasar değerini günceller
    /// </summary>
    /// <param name="newDamage">Yeni hasar değeri</param>
    public void UpdateItemDamage(float newDamage)
    {
        itemDamage = Mathf.Max(0f, newDamage); // Negatif olamaz
        OnStatsUpdated();
    }

    /// <summary>
    /// Silahın menzil değerini günceller
    /// </summary>
    /// <param name="newRange">Yeni menzil değeri</param>
    public void UpdateAttackRange(float newRange)
    {
        attackRange = Mathf.Max(0f, newRange); // Negatif olamaz
        OnStatsUpdated();
    }

    /// <summary>
    /// Tetikleme sırasında çağrılır - türetilen sınıflarda özelleştirilebilir
    /// Örnek: Ses efekti, parçacık efekti vb.
    /// </summary>
    protected virtual void OnTrigger()
    {
        // Türetilen sınıflarda özelleştirilebilir
    }

    /// <summary>
    /// Hasar verildiğinde çağrılır - türetilen sınıflarda özelleştirilebilir
    /// Örnek: Hit efekti, ses efekti vb.
    /// </summary>
    /// <param name="target">Hasar alan hedef</param>
    protected virtual void OnDamageDealt(GameObject target)
    {
        // Türetilen sınıflarda özelleştirilebilir
    }

    /// <summary>
    /// Stat'lar güncellendiğinde çağrılır - türetilen sınıflarda özelleştirilebilir
    /// Örnek: UI güncelleme, efekt değişikliği vb.
    /// </summary>
    protected virtual void OnStatsUpdated()
    {
        // Türetilen sınıflarda özelleştirilebilir
    }

    /// <summary>
    /// Menzil gösterimi için Gizmos
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        if (showDebugGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
