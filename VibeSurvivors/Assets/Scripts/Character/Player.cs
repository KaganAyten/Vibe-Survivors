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

        // Otomatik sald?r?y? ba?lat
        if (!isAttacking)
        {
            StartAttacking();
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
