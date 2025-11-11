using UnityEngine;

/// <summary>
/// Weapon sistemini test etmek için basit bir script
/// Space tu?una bas?nca silah? tetikler
/// </summary>
public class WeaponTester : MonoBehaviour
{
    [Header("Weapon Reference")]
    [SerializeField] private Weapon weapon;

    [Header("Test Controls")]
 [SerializeField] private KeyCode triggerKey = KeyCode.Space;
    [SerializeField] private KeyCode increaseDamageKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode increaseRangeKey = KeyCode.Alpha2;

    private void Update()
    {
      if (weapon == null)
        {
Debug.LogWarning("Weapon reference is missing!");
        return;
        }

   // Space ile silah? tetikle
        if (Input.GetKeyDown(triggerKey))
        {
   weapon.Trigger();
 }

        // 1 tu?u ile hasar? art?r
        if (Input.GetKeyDown(increaseDamageKey))
  {
       float newDamage = weapon.ItemDamage + 5f;
            weapon.UpdateItemDamage(newDamage);
        }

   // 2 tu?u ile menzili art?r
        if (Input.GetKeyDown(increaseRangeKey))
{
        float newRange = weapon.AttackRange + 1f;
 weapon.UpdateAttackRange(newRange);
        }
    }

    private void OnGUI()
{
     if (weapon == null)
      return;

      GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Box("=== Weapon Tester ===");
   GUILayout.Label($"Damage: {weapon.ItemDamage}");
   GUILayout.Label($"Range: {weapon.AttackRange}");
        GUILayout.Space(10);
        GUILayout.Label($"[{triggerKey}] - Trigger Weapon");
        GUILayout.Label($"[{increaseDamageKey}] - Increase Damage");
        GUILayout.Label($"[{increaseRangeKey}] - Increase Range");
   GUILayout.EndArea();
    }
}
