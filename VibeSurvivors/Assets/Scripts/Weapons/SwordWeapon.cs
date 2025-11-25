using UnityEngine;

/// <summary>
/// Kılıç silahı - Weapon sınıfından türetilmiş
/// </summary>
public class SwordWeapon : Weapon
{
    public SwordWeapon(Character owner) : base(owner)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        weaponType = WeaponType.Sword;
        itemDamage = 10f;
        attackRange = 2.5f;
        cooldown = 1f;
    }

    protected override void Trigger()
    {
        Debug.Log($"⚔️ Sword triggered! Damage: {itemDamage}, Range: {attackRange}");

        // Menzil içindeki düşmanlara hasar ver
        DealDamageInRange();

        // Visual efekti tetikle (enum ile!)
        PlayVisualEffect();
    }

    protected override void OnDamageDealt(GameObject target)
    {
        Debug.Log($"💥 Sword hit {target.name} for {itemDamage} damage!");

        // Life steal uygula
        if (owner != null)
        {
            owner.ApplyLifeStealFromWeapon(itemDamage);
        }
    }

    protected override void OnStatsUpdated()
    {
        Debug.Log($"🔼 Sword stats updated - Damage: {itemDamage}, Range: {attackRange}");
    }
}
