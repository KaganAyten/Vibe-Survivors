using UnityEngine;

/// <summary>
/// Oyuncu karakteri - Character s?n?f?ndan türetilmi?
/// PlayerMovementController ile birlikte çal???r
/// </summary>
public class Player : Character
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Oyun ba?lay?nca otomatik sald?r?ya ba?la
        if (currentWeapon != null)
        {
            StartAttacking();
            Debug.Log("??? Auto-attack started!");
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        // Oyuncuya özel ba?lang?ç de?erleri
        movementSpeed = 5f;
        baseHealth = 100f;
        attackSpeed = 1f;
        baseDamage = 10f;
        collectRange = 3f;

        Debug.Log("?? Player initialized!");
    }

    protected override void OnStatUpdated(StatType statType, float value)
    {
        Debug.Log($"?? Player stat updated: {statType} by {value}");

        // Attack speed güncellenirse silah?n animasyon süresini güncelle
        if (statType == StatType.AttackSpeed)
        {
            UpdateWeaponAnimationSpeed();
        }
    }

    protected override void OnAttackPerformed()
    {
        Debug.Log("?? Player attacked!");
    }

    protected override void OnDamageTaken(float damage)
    {
        Debug.Log($"?? Player took {damage} damage!");
    }

    protected override void OnHealed(float amount)
    {
        Debug.Log($"?? Player healed {amount}!");
    }

    protected override void OnDeath()
    {
        Debug.Log("?? Player died! Game Over!");

        // Hareket ve sald?r?y? durdur
        PlayerMovementController movementController = GetComponent<PlayerMovementController>();
        if (movementController != null)
        {
            movementController.enabled = false;
        }
    }

    protected override void OnWeaponEquipped(Weapon weapon)
    {
        Debug.Log($"?? Player equipped {weapon.name}!");

        // Yeni silah tak?l?nca animasyon süresini ayarla
        UpdateWeaponAnimationSpeed();

        // Otomatik sald?r?y? ba?lat
        if (!isAttacking)
        {
            StartAttacking();
        }
    }

    /// <summary>
    /// Silah?n animasyon süresini attack speed'e göre ayarlar
    /// Animasyon süresi = 1 / (attackSpeed * 2)
    /// </summary>
    private void UpdateWeaponAnimationSpeed()
    {
        if (currentWeapon == null)
            return;

        // Sword tipindeki silahlar için özel ayar
        Sword sword = currentWeapon as Sword;
        if (sword != null)
        {
            // Attack speed = 10 ? 1/10 = 0.1 saniye sald?r? aral???
            // Animasyon süresi = 0.1 / 2 = 0.05 saniye
            float animationDuration = (1f / attackSpeed) / 2f;
            sword.SetSwingDuration(animationDuration);

            Debug.Log($"? Weapon animation speed updated: {animationDuration:F3}s (Attack Speed: {attackSpeed})");
        }
    }

    // Oyuncuya özel metodlar
    public void AddXP(float xp)
    {
        currentXP += xp;
        Debug.Log($"? Gained {xp} XP! Total: {currentXP}");

        CheckLevelUp();
    }

    public void AddGold(int gold)
    {
        currentGold += gold;
        Debug.Log($"?? Gained {gold} gold! Total: {currentGold}");
    }

    private void CheckLevelUp()
    {
        // Basit level up sistemi
        float xpRequired = currentLevel * 100f;

        if (currentXP >= xpRequired)
        {
            currentLevel++;
            currentXP -= xpRequired;
            OnLevelUp();
        }
    }

    private void OnLevelUp()
    {
        Debug.Log($"?? Level Up! Now level {currentLevel}!");

        // Level up bonuslar?
        UpdateStat(StatType.BaseHealth, 10f, true);
        UpdateStat(StatType.BaseDamage, 2f, true);

        // Can? doldur
        currentHealth = baseHealth;
    }
}
