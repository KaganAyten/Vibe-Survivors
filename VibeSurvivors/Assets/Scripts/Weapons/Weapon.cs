using UnityEngine;

/// <summary>
/// Tüm silahlar için temel soyut sınıf - MonoBehaviour değil!
// Her silah kendi cooldown'unu yönetir ve owner karaktere referans tutar
/// </summary>
[System.Serializable]
public class Weapon
{
    [Header("Weapon Info")]
    [SerializeField] protected WeaponType weaponType = WeaponType.None;
    
    [Header("Weapon Stats")]
    [SerializeField] protected float itemDamage = 10f;
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float cooldown = 1f; // Silahın kendi cooldown'u (attack speed'den bağımsız)

    // Runtime values
    [SerializeField] protected float currentCooldown = 0f;
    protected Character owner; // Silahın sahibi (serialize edilmez)

    /// <summary>
    /// Silahın türü
    /// </summary>
    public WeaponType WeaponType => weaponType;

    /// <summary>
    /// Silahın mevcut hasar değeri
    /// </summary>
    public float ItemDamage => itemDamage;

    /// <summary>
    /// Silahın mevcut menzil değeri
    /// </summary>
    public float AttackRange => attackRange;

    /// <summary>
    /// Silahın cooldown süresi
    /// </summary>
    public float Cooldown => cooldown;

    /// <summary>
    /// Silahın sahibi
    /// </summary>
    public Character Owner => owner;

    /// <summary>
    /// Silahın tetiklenmeye hazır olup olmadığı
    /// </summary>
    public bool IsReady => currentCooldown <= 0f;

    /// <summary>
    /// Weapon constructor - Her weapon oluşturulduğunda çağrılır
    /// </summary>
    /// <param name="owner">Silahın sahibi olan karakter</param>
    public Weapon(Character owner)
    {
        this.owner = owner;
        Initialize();
    }

    /// <summary>
    /// Silahın başlangıç ayarları - Override edilebilir
    /// </summary>
    protected virtual void Initialize()
    {
        currentCooldown = 0f;
    }

    /// <summary>
    /// Her frame cooldown'u günceller - Character'dan çağrılır
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime</param>
    public void UpdateCooldownTimer(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= deltaTime;
        }
    }

    /// <summary>
    /// Silahı tetikler - cooldown kontrolü ile
    /// </summary>
    public void TryTrigger()
    {
        if (!IsReady || owner == null || owner.IsDead)
            return;

        // Tetikle
        Trigger();

        // Cooldown'u başlat
        currentCooldown = cooldown;
    }

    /// <summary>
    /// Silahın asıl tetikleme mantığı - Her weapon kendi şekilde override eder
    /// </summary>
    protected virtual void Trigger() { }

    /// <summary>
    /// Silahın hasar değerini günceller
    /// </summary>
    /// <param name="newDamage">Yeni hasar değeri</param>
    public void UpdateItemDamage(float newDamage)
    {
        itemDamage = Mathf.Max(0f, newDamage);
        OnStatsUpdated();
    }

    /// <summary>
    /// Silahın menzil değerini günceller
    /// </summary>
    /// <param name="newRange">Yeni menzil değeri</param>
    public void UpdateAttackRange(float newRange)
    {
        attackRange = Mathf.Max(0f, newRange);
        OnStatsUpdated();
    }

    /// <summary>
    /// Silahın cooldown değerini günceller
    /// </summary>
    /// <param name="newCooldown">Yeni cooldown değeri</param>
    public void SetCooldown(float newCooldown)
    {
        cooldown = Mathf.Max(0.1f, newCooldown);
        OnStatsUpdated();
    }

    /// <summary>
    /// Visual efekti tetikler - Particle System'i Play() yapar, instantiate etmez!
    /// </summary>
    protected virtual void PlayVisualEffect()
    {
        if (owner != null)
        {
            // Enum ile visual controller'a haber ver
            WeaponVisualController.OnWeaponTriggered?.Invoke(weaponType, owner.transform.position, attackRange);
        }
    }

    /// <summary>
    /// Stat'lar güncellendiğinde çağrılır - override edilebilir
    /// </summary>
    protected virtual void OnStatsUpdated()
    {
        // Override edilebilir
    }

    /// <summary>
    /// Hasar verme mantığı - menzil içindeki IDamageable nesnelere hasar verir
    /// </summary>
    protected virtual void DealDamageInRange()
    {
        if (owner == null)
            return;

        // Menzil içindeki tüm collider'ları bul
        Collider[] hitColliders = Physics.OverlapSphere(owner.transform.position, attackRange);

        foreach (Collider hitCollider in hitColliders)
        {
            // IDamageable interface'i olan nesneleri kontrol et
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();

            if (damageable != null && damageable.IsAlive)
            {
                // Hasar ver
                damageable.TakeDamage(itemDamage);

                // Hasar verme efekti
                OnDamageDealt(hitCollider.gameObject);
            }
        }
    }

    /// <summary>
    /// Hasar verildiğinde çağrılır - override edilebilir
    /// </summary>
    protected virtual void OnDamageDealt(GameObject target)
    {
        // Override edilebilir
    }
}
