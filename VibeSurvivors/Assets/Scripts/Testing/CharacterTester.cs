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

        // Mouse0 ile sald?r?y? ba?lat/durdur
        if (Input.GetKeyDown(attackKey))
{
  player.StartAttacking();
   Debug.Log("??? Started attacking!");
        }
        
        if (Input.GetKeyUp(attackKey))
    {
       player.StopAttacking();
  Debug.Log("??? Stopped attacking!");
  }

        // K tu?u ile hasar al
      if (Input.GetKeyDown(takeDamageKey))
        {
     player.TakeDamage(20f);
        }

        // H tu?u ile can yenile
        if (Input.GetKeyDown(healKey))
  {
    player.Heal(30f);
   }

     // X tu?u ile XP ekle
        if (Input.GetKeyDown(addXPKey))
  {
      player.AddXP(50f);
        }

    // G tu?u ile gold ekle
  if (Input.GetKeyDown(addGoldKey))
     {
       player.AddGold(25);
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

    GUILayout.BeginArea(new Rect(10, 10, 350, 400));
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
        GUILayout.Label($"??? Weapon: {(player.CurrentWeapon != null ? player.CurrentWeapon.name : "None")}");
      
        GUILayout.Space(10);
   
        // Controls
        GUILayout.Label("=== Controls ===");
   GUILayout.Label($"[{attackKey}] - Attack (Hold)");
        GUILayout.Label($"[{takeDamageKey}] - Take Damage");
   GUILayout.Label($"[{healKey}] - Heal");
        GUILayout.Label($"[{addXPKey}] - Add XP");
     GUILayout.Label($"[{addGoldKey}] - Add Gold");
GUILayout.Label("[1] - Increase Movement Speed");
        GUILayout.Label("[2] - Increase Attack Speed");
    GUILayout.Label("[3] - Increase Damage");
        GUILayout.Label("[4] - Increase Life Steal");
        
        GUILayout.EndArea();
    }
}
