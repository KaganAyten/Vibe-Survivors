using UnityEngine;

/// <summary>
/// Ok silahý - Weapon sýnýfýndan türetilmiþ
/// Uzun menzilli, hýzlý cooldown'lu silah
/// </summary>
public class ArrowWeapon : Weapon
{
    public ArrowWeapon(Character owner) : base(owner)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        weaponType = WeaponType.Arrow;
        itemDamage = 5f;
        attackRange = 8f;
        cooldown = 0.5f;
    }

    protected override void Trigger()
    {
        Debug.Log($"?? Arrow triggered! Damage: {itemDamage}, Range: {attackRange}");

        // Menzil içindeki düþmanlara hasar ver
        DealDamageInRange();

        // Visual efekti tetikle (enum ile!)
        PlayVisualEffect();
    }

    protected override void OnDamageDealt(GameObject target)
    {
        Debug.Log($"?? Arrow hit {target.name} for {itemDamage} damage!");

        // Life steal uygula
        if (owner != null)
        {
            owner.ApplyLifeStealFromWeapon(itemDamage);
        }
    }

    protected override void OnStatsUpdated()
    {
        Debug.Log($"?? Arrow stats updated - Damage: {itemDamage}, Range: {attackRange}");
    }
}
