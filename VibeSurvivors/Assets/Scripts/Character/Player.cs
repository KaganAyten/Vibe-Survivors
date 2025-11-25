using UnityEngine;

/// <summary>
/// Oyuncu karakteri - Character sýnýfýndan türetilmiþ
/// PlayerMovementController ile birlikte çalýþýr
/// </summary>
public class Player : Character
{
    [Header("Player Settings")]
    [SerializeField] private bool addStartingWeapon = false; // Inspector'dan kontrol edilebilir

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Sadece starting weapon eklemek istiyorsan ve silah yoksa ekle
        if (addStartingWeapon && WeaponCount == 0)
        {
            AddWeapon(new SwordWeapon(this));
            Debug.Log("?? Starting weapon (Sword) added!");
        }

        // Sadece silah varsa otomatik saldýrýya baþla
        if (WeaponCount > 0)
        {
            StartAttacking();
            Debug.Log("?? Player auto-attack started!");
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        // Oyuncuya özel baþlangýç deðerleri
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

        // Hareket ve saldýrýyý durdur
        PlayerMovementController movementController = GetComponent<PlayerMovementController>();
        if (movementController != null)
        {
            movementController.enabled = false;
        }
    }

    protected override void OnWeaponAdded(Weapon weapon)
    {
        Debug.Log($"?? Player equipped {weapon.WeaponType}! Total: {WeaponCount}");
        
        // Ýlk silah eklendiðinde otomatik saldýrýya baþla
        if (WeaponCount == 1 && !isAttacking)
        {
            StartAttacking();
        }
    }

    protected override void OnWeaponRemoved(Weapon weapon)
    {
        Debug.Log($"??? Player removed {weapon.WeaponType}! Remaining: {WeaponCount}");
        
        // Hiç silah kalmadýysa saldýrýyý durdur
        if (WeaponCount == 0)
        {
            StopAttacking();
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
        Debug.Log($"?? Level Up! Now level {currentLevel} !");

        // Level up bonuslarý
        UpdateStat(StatType.BaseHealth, 10f, true);
        UpdateStat(StatType.BaseDamage, 2f, true);

        // Caný doldur
        currentHealth = baseHealth;
    }
}
