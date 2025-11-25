using UnityEngine;

/// <summary>
/// Çekiç silahý - Weapon sýnýfýndan türetilmiþ
/// Kýsa menzilli, yüksek hasarlý, yavaþ cooldown'lu silah
/// </summary>
public class HammerWeapon : Weapon
{
    public HammerWeapon(Character owner) : base(owner)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        weaponType = WeaponType.Hammer;
        itemDamage = 25f;
        attackRange = 1.5f;
        cooldown = 2f;
    }

    protected override void Trigger()
    {
        Debug.Log($"?? Hammer triggered! Damage: {itemDamage}, Range: {attackRange}");

        // Menzil içindeki düþmanlara hasar ver
        DealDamageInRange();

        // Visual efekti tetikle (enum ile!)
        PlayVisualEffect();
    }

    protected override void OnDamageDealt(GameObject target)
    {
        Debug.Log($"?? Hammer smashed {target.name} for {itemDamage} damage!");

        // Life steal uygula
        if (owner != null)
        {
            owner.ApplyLifeStealFromWeapon(itemDamage);
        }
    }

    protected override void OnStatsUpdated()
    {
        Debug.Log($"?? Hammer stats updated - Damage: {itemDamage}, Range: {attackRange}");
    }
}
