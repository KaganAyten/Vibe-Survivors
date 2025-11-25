using System.Linq;
using UnityEngine;
using System.Collections.Generic;

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

    [Header("Combat - Weapons")]
    // Weapon listesi - runtime'da silahlar eklenecek - ARTIK INSPECTOR'DA GÖRÜNÜR!
    [SerializeField] protected List<Weapon> weapons = new List<Weapon>();

    // Runtime değerler
    [SerializeField] protected float currentHealth;
    protected bool isDead = false;
    protected bool isAttacking = false;

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
    public IReadOnlyList<Weapon> Weapons => weapons;
    public int WeaponCount => weapons.Count;
    public bool IsAlive => !isDead && currentHealth > 0f;
    public bool IsDead => isDead;

    #endregion

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Update()
    {
        // Weapon cooldown'larını güncelle
        UpdateWeaponCooldowns();

        // Saldırı sistemi - tüm silahları tetikle
        if (isAttacking && !isDead)
        {
            TriggerAllWeapons();
        }
    }

    /// <summary>
    /// Tüm silahların cooldown'larını günceller
    /// </summary>
    private void UpdateWeaponCooldowns()
    {
        foreach (var weapon in weapons)
        {
            weapon.UpdateCooldownTimer(Time.deltaTime);
        }
    }

    /// <summary>
    /// Tüm silahları tetikler (kendi cooldown'larına göre)
    /// </summary>
    private void TriggerAllWeapons()
    {
        foreach (var weapon in weapons)
        {
            // Her silah kendi cooldown'una göre tetiklenir
            weapon.TryTrigger();
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
        weapons.Clear();

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
                // Tüm silahlara yeni hasarı uygula
                UpdateAllWeaponDamage();
                break;

            case StatType.AttackDamage:
                attackDamage = isAdditive ? attackDamage + value : attackDamage * value;
                // Tüm silahlara yeni hasarı uygula
                UpdateAllWeaponDamage();
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
    /// Tüm silahların hasarını günceller
    /// </summary>
    private void UpdateAllWeaponDamage()
    {
        float totalDamage = baseDamage + attackDamage;
        foreach (var weapon in weapons)
        {
            weapon.UpdateItemDamage(totalDamage);
        }
    }

    /// <summary>
    /// Saldırı başlatır - Tüm silahları sürekli tetikler
    /// </summary>
    public virtual void StartAttacking()
    {
        if (isDead)
            return;

        if (!isAttacking)
        {
            isAttacking = true;
            Debug.Log($"🗡️ {gameObject.name} started attacking with {weapons.Count} weapon(s)!");
        }
    }

    /// <summary>
    /// Saldırıyı durdurur
    /// </summary>
    public virtual void StopAttacking()
    {
        isAttacking = false;
        Debug.Log($"🛡️ {gameObject.name} stopped attacking!");
    }

    /// <summary>
    /// Yeni bir silah ekler (Runtime'da)
    /// </summary>
    public virtual void AddWeapon(Weapon newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogWarning("⚠️ Cannot add null weapon!");
            return;
        }
        if (weapons.Contains(weapons.FirstOrDefault(x => x.WeaponType == newWeapon.WeaponType))) return;
        weapons.Add(newWeapon);

        // Silahın hasarını güncelle
        newWeapon.UpdateItemDamage(baseDamage + attackDamage);

        Debug.Log($"⚔️ {gameObject.name} added {newWeapon.WeaponType}! Total weapons: {weapons.Count}");

        OnWeaponAdded(newWeapon);

        // Eğer attacking modundaysa otomatik tetikle
        if (!isAttacking)
        {
            StartAttacking();
        }
    }

    /// <summary>
    /// Silah kaldırır
    /// </summary>
    public virtual void RemoveWeapon(Weapon weapon)
    {
        if (weapons.Remove(weapon))
        {
            Debug.Log($"🗑️ {gameObject.name} removed {weapon.WeaponType}! Remaining: {weapons.Count}");
            OnWeaponRemoved(weapon);
        }
    }

    /// <summary>
    /// Tüm silahları temizler
    /// </summary>
    public virtual void ClearWeapons()
    {
        weapons.Clear();
        Debug.Log($"🗑️ {gameObject.name} cleared all weapons!");
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
    /// Life steal hesaplar ve uygular - Weapon'lardan çağrılır
    /// </summary>
    public virtual void ApplyLifeStealFromWeapon(float damageDealt)
    {
        if (lifeSteal > 0f)
        {
            float healAmount = damageDealt * lifeSteal;
            Heal(healAmount);
        }
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
    /// Silah eklendiğinde çağrılır
    /// </summary>
    protected virtual void OnWeaponAdded(Weapon weapon)
    {
        // Override edilebilir
    }

    /// <summary>
    /// Silah kaldırıldığında çağrılır
    /// </summary>
    protected virtual void OnWeaponRemoved(Weapon weapon)
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

        // Her silahın menzilini göster
        if (weapons != null)
        {
            Gizmos.color = Color.red;
            foreach (var weapon in weapons)
            {
                Gizmos.DrawWireSphere(transform.position, weapon.AttackRange);
            }
        }
    }

    #endregion
}
