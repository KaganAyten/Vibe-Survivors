using UnityEngine;

/// <summary>
/// Tüm karakterler için temel sınıf (Oyuncu ve düşmanlar için)
/// </summary>
public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] protected float movementSpeed = 5f;
    [SerializeField] protected float baseHealth = 100f;
    [SerializeField] protected float attackSpeed = 1f; // Saniyede kaç saldırı
    [SerializeField] protected float baseDamage = 10f;
    [SerializeField] protected float attackDamage = 0f; // Ekstra hasar
    [SerializeField] protected int projectileCount = 1;
    [SerializeField] protected float collectRange = 2f;
    [SerializeField] protected float lifeSteal = 0f; // 0-1 arası (0.1 = %10)

    [Header("Character Progression")]
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] protected float currentXP = 0f;
    [SerializeField] protected int currentGold = 0;

    [Header("Combat")]
    [SerializeField] protected Weapon currentWeapon;

    // Runtime değerler
    [SerializeField] protected float currentHealth;
    protected bool isDead = false;
    protected bool isAttacking = false;
    protected float attackTimer = 0f; // Saldırı zamanlayıcısı

    #region Properties

    public float MovementSpeed => movementSpeed;
    public float BaseHealth => baseHealth;
    public float CurrentHealth => currentHealth;
    public float AttackSpeed => attackSpeed;
    public float BaseDamage => baseDamage;
    public float AttackDamage => attackDamage;
    public float TotalDamage => baseDamage + attackDamage;
    public int ProjectileCount => projectileCount;
    public float CollectRange => collectRange;
    public float LifeSteal => lifeSteal;
    public int CurrentLevel => currentLevel;
    public float CurrentXP => currentXP;
    public int CurrentGold => currentGold;
    public Weapon CurrentWeapon => currentWeapon;
    public bool IsAlive => !isDead && currentHealth > 0f;
    public bool IsDead => isDead;

    #endregion

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Update()
    {
        // Saldırı sistemi - Update'te çalışır böylece attack speed değişiklikleri anında etkiler
        if (isAttacking && !isDead && currentWeapon != null)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                Attack();
                // Attack Speed = saniyede kaç saldırı
                // Bekleme süresi = 1 / attack speed
                attackTimer = 1f / Mathf.Max(0.1f, attackSpeed);
            }
        }
    }

    /// <summary>
    /// Karakterin başlangıç değerlerini ayarlar
    /// </summary>
    public virtual void Initialize()
    {
        currentHealth = baseHealth;
        currentLevel = 1;
        currentXP = 0f;
        currentGold = 0;
        isDead = false;
        isAttacking = false;
        attackTimer = 0f;

        Debug.Log($"✨ {gameObject.name} initialized - Health: {currentHealth}/{baseHealth}");
    }

    /// <summary>
    /// Karakterin statlarını günceller
    /// </summary>
    /// <param name="statType">Güncellenecek stat türü</param>
    /// <param name="value">Yeni değer (eklenecek veya çarpılacak)</param>
    /// <param name="isAdditive">True = Değer eklenir, False = Değer ile çarpılır</param>
    public virtual void UpdateStat(StatType statType, float value, bool isAdditive = true)
    {
        switch (statType)
        {
            case StatType.MovementSpeed:
                movementSpeed = isAdditive ? movementSpeed + value : movementSpeed * value;
                movementSpeed = Mathf.Max(0.1f, movementSpeed);
                break;

            case StatType.BaseHealth:
                float healthRatio = currentHealth / baseHealth; // Sağlık oranını koru
                baseHealth = isAdditive ? baseHealth + value : baseHealth * value;
                baseHealth = Mathf.Max(1f, baseHealth);
                currentHealth = baseHealth * healthRatio; // Oranı koru
                break;

            case StatType.AttackSpeed:
                attackSpeed = isAdditive ? attackSpeed + value : attackSpeed * value;
                attackSpeed = Mathf.Max(0.1f, attackSpeed);
                break;

            case StatType.BaseDamage:
                baseDamage = isAdditive ? baseDamage + value : baseDamage * value;
                baseDamage = Mathf.Max(0f, baseDamage);
                break;

            case StatType.AttackDamage:
                attackDamage = isAdditive ? attackDamage + value : attackDamage * value;
                break;

            case StatType.ProjectileCount:
                projectileCount = isAdditive ? projectileCount + (int)value : Mathf.RoundToInt(projectileCount * value);
                projectileCount = Mathf.Max(1, projectileCount);
                break;

            case StatType.CollectRange:
                collectRange = isAdditive ? collectRange + value : collectRange * value;
                collectRange = Mathf.Max(0.5f, collectRange);
                break;

            case StatType.LifeSteal:
                lifeSteal = isAdditive ? lifeSteal + value : lifeSteal * value;
                lifeSteal = Mathf.Clamp01(lifeSteal);
                break;
        }

        OnStatUpdated(statType, value);
    }

    /// <summary>
    /// Saldırı başlatır - Attack Speed frekansında silahı tetikler
    /// </summary>
    public virtual void StartAttacking()
    {
        if (isDead || currentWeapon == null)
            return;

        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = 0f; // Hemen saldırsın
            Debug.Log($"🗡️ {gameObject.name} started attacking!");
        }
    }

    /// <summary>
    /// Saldırıyı durdurur
    /// </summary>
    public virtual void StopAttacking()
    {
        isAttacking = false;
        attackTimer = 0f;
        Debug.Log($"🛡️ {gameObject.name} stopped attacking!");
    }

    /// <summary>
    /// Tek bir saldırı yapar
    /// </summary>
    protected virtual void Attack()
    {
        if (currentWeapon == null || isDead)
            return;

        // Silahın hasar değerini güncelle
        float totalDamage = baseDamage + attackDamage;
        currentWeapon.UpdateItemDamage(totalDamage);

        // Silahı tetikle
        currentWeapon.Trigger();

        OnAttackPerformed();
    }

    /// <summary>
    /// Hasar alır (IDamageable benzeri)
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        Debug.Log($"💔 {gameObject.name} took {damage} damage! Health: {currentHealth}/{baseHealth}");

        OnDamageTaken(damage);

        // Can 0 veya altına düştü mü?
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Karakter ölür
    /// </summary>
    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;

        // Saldırıyı durdur
        StopAttacking();

        Debug.Log($"💀 {gameObject.name} died!");

        OnDeath();
    }

    /// <summary>
    /// Can yeniler
    /// </summary>
    public virtual void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, baseHealth); // Max health'i geçmesin

        Debug.Log($"💚 {gameObject.name} healed {amount}! Health: {currentHealth}/{baseHealth}");

        OnHealed(amount);
    }

    /// <summary>
    /// Life steal hesaplar ve uygular
    /// </summary>
    protected virtual void ApplyLifeSteal(float damageDealt)
    {
        if (lifeSteal > 0f)
        {
            float healAmount = damageDealt * lifeSteal;
            Heal(healAmount);
        }
    }

    /// <summary>
    /// Silah değiştirir
    /// </summary>
    public virtual void EquipWeapon(Weapon newWeapon)
    {
        if (newWeapon == null)
            return;

        // Eski silahı devre dışı bırak
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);

        // Silah hasarını güncelle
        currentWeapon.UpdateItemDamage(baseDamage + attackDamage);

        Debug.Log($"⚔️ {gameObject.name} equipped {newWeapon.name}");

        OnWeaponEquipped(newWeapon);
    }

    #region Virtual Methods (Override için)

    /// <summary>
    /// Stat güncellendiğinde çağrılır
    /// </summary>
    protected virtual void OnStatUpdated(StatType statType, float value)
    {
        // Override edilebilir
    }

    /// <summary>
    /// Saldırı yapıldığında çağrılır
    /// </summary>
    protected virtual void OnAttackPerformed()
    {
        // Override edilebilir
    }

    /// <summary>
    /// Hasar alındığında çağrılır
    /// </summary>
    protected virtual void OnDamageTaken(float damage)
    {
        // Override edilebilir
    }

    /// <summary>
    /// Can yenilendiğinde çağrılır
    /// </summary>
    protected virtual void OnHealed(float amount)
    {
        // Override edilebilir
    }

    /// <summary>
    /// Ölüm anında çağrılır
    /// </summary>
    protected virtual void OnDeath()
    {
        // Override edilebilir
    }

    /// <summary>
    /// Silah takıldığında çağrılır
    /// </summary>
    protected virtual void OnWeaponEquipped(Weapon weapon)
    {
        // Override edilebilir
    }

    #endregion

    #region Debug

    protected virtual void OnDrawGizmosSelected()
    {
        // Collect range göster
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectRange);
    }

    #endregion
}
