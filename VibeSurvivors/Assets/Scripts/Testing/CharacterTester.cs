using UnityEngine;

/// <summary>
/// Character sistemini test etmek için
/// </summary>
public class CharacterTester : MonoBehaviour
{
    [Header("Character Reference")]
    [SerializeField] private Player player;

    [Header("Test Controls")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode takeDamageKey = KeyCode.K;
    [SerializeField] private KeyCode healKey = KeyCode.H;
    [SerializeField] private KeyCode addXPKey = KeyCode.X;
    [SerializeField] private KeyCode addGoldKey = KeyCode.G;

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing!");
            return;
        }

        // Mouse0 ile saldýrýyý baþlat/durdur
        if (Input.GetKeyDown(attackKey))
        {
            player.StartAttacking();
            Debug.Log("?? Started attacking!");
        }

        if (Input.GetKeyUp(attackKey))
        {
            player.StopAttacking();
            Debug.Log("??? Stopped attacking!");
        }

        // K tuþu ile hasar al
        if (Input.GetKeyDown(takeDamageKey))
        {
            player.TakeDamage(20f);
        }

        // H tuþu ile can yenile
        if (Input.GetKeyDown(healKey))
        {
            player.Heal(30f);
        }

        // X tuþu ile XP ekle
        if (Input.GetKeyDown(addXPKey))
        {
            player.AddXP(50f);
        }

        // G tuþu ile gold ekle
        if (Input.GetKeyDown(addGoldKey))
        {
            player.AddGold(25);
        }

        // Weapon ekleme tuþlarý
        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.AddWeapon(new SwordWeapon(player));
            Debug.Log("?? Sword added!");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            player.AddWeapon(new ArrowWeapon(player));
            Debug.Log("?? Arrow added!");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.AddWeapon(new HammerWeapon(player));
            Debug.Log("?? Hammer added!");
        }

        // Stat güncelleme testleri
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.UpdateStat(StatType.MovementSpeed, 1f, true);
            Debug.Log("?? Movement Speed increased!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.UpdateStat(StatType.AttackSpeed, 0.2f, true);
            Debug.Log("? Attack Speed increased!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.UpdateStat(StatType.BaseDamage, 5f, true);
            Debug.Log("?? Base Damage increased!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            player.UpdateStat(StatType.LifeSteal, 0.1f, true);
            Debug.Log("?? Life Steal increased!");
        }
    }

    private void OnGUI()
    {
        if (player == null)
            return;

        GUILayout.BeginArea(new Rect(10, 10, 400, 550));
        GUILayout.Box("=== Character System Tester ===");

        // Stats
        GUILayout.Label($"?? Health: {player.CurrentHealth:F0}/{player.BaseHealth:F0}");
        GUILayout.Label($"?? Damage: {player.TotalDamage:F1}");
        GUILayout.Label($"? Attack Speed: {player.AttackSpeed:F2}");
        GUILayout.Label($"?? Movement Speed: {player.MovementSpeed:F1}");
        GUILayout.Label($"?? Life Steal: {(player.LifeSteal * 100):F0}%");
        GUILayout.Label($"?? Level: {player.CurrentLevel}");
        GUILayout.Label($"? XP: {player.CurrentXP:F0}");
        GUILayout.Label($"?? Gold: {player.CurrentGold}");
        GUILayout.Label($"??? Weapons: {player.WeaponCount}");

        GUILayout.Space(5);

        // Weapon List - ENUM ile göster!
        if (player.WeaponCount > 0)
        {
            GUILayout.Label("=== Active Weapons ===");
            int index = 1;
            foreach (var weapon in player.Weapons)
            {
                string readyStatus = weapon.IsReady ? "?" : "?";
                GUILayout.Label($"{index}. {weapon.WeaponType} {readyStatus} - DMG: {weapon.ItemDamage:F1} | RNG: {weapon.AttackRange:F1} | CD: {weapon.Cooldown:F1}s");
                index++;
            }
        }

        GUILayout.Space(10);

        // Controls
        GUILayout.Label("=== Controls ===");
        GUILayout.Label($"[{attackKey}] - Attack (Hold)");
        GUILayout.Label($"[{takeDamageKey}] - Take Damage");
        GUILayout.Label($"[{healKey}] - Heal");
        GUILayout.Label($"[{addXPKey}] - Add XP");
        GUILayout.Label($"[{addGoldKey}] - Add Gold");
        
        GUILayout.Space(5);
        GUILayout.Label("=== Add Weapons (Runtime) ===");
        GUILayout.Label("[Q] - Add Sword (Medium)");
        GUILayout.Label("[W] - Add Arrow (Fast/Long)");
        GUILayout.Label("[E] - Add Hammer (Slow/Strong)");
        
        GUILayout.Space(5);
        GUILayout.Label("=== Stat Upgrades ===");
        GUILayout.Label("[1] - Increase Movement Speed");
        GUILayout.Label("[2] - Increase Attack Speed");
        GUILayout.Label("[3] - Increase Damage");
        GUILayout.Label("[4] - Increase Life Steal");

        GUILayout.EndArea();
    }
}
