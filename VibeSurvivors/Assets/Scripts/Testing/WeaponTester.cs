using UnityEngine;

/// <summary>
/// Weapon sistemini test etmek için basit bir script
/// Artýk kullanýlmýyor - CharacterTester kullanýn
/// </summary>
[System.Obsolete("Use CharacterTester instead")]
public class WeaponTester : MonoBehaviour
{
    private void Start()
    {
        Debug.LogWarning("?? WeaponTester is obsolete! Use CharacterTester instead.");
        Debug.LogWarning("Weapons are no longer MonoBehaviour - they are created at runtime.");
    }
}
