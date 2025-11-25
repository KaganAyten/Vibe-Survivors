using UnityEngine;
using System;

/// <summary>
/// Silah görsellerini ve efektlerini yöneten MonoBehaviour sýnýfý
/// Weapon sýnýflarýndan gelen event'leri dinler ve MEVCUT particle system'leri tetikler
/// </summary>
public class WeaponVisualController : MonoBehaviour
{
    [System.Serializable]
    public class WeaponVisualData
    {
        public WeaponType weaponType;
        public ParticleSystem particleSystem; // Sahnede olan, instantiate etmeyeceðimiz
        public float baseParticleSize = 2f;
        public float baseAttackRange = 2.5f;
    }

    [Header("Visual Settings")]
    [SerializeField] private WeaponVisualData[] weaponVisuals;

    // Event - Weapon'lar bu event'i tetikleyecek (ENUM kullanýyor artýk!)
    public static Action<WeaponType, Vector3, float> OnWeaponTriggered;

    private void OnEnable()
    {
        OnWeaponTriggered += PlayWeaponEffect;
    }

    private void OnDisable()
    {
        OnWeaponTriggered -= PlayWeaponEffect;
    }

    /// <summary>
    /// Silah tetiklendiðinde MEVCUT particle system'i tetikler - instantiate etmez!
    /// </summary>
    private void PlayWeaponEffect(WeaponType weaponType, Vector3 position, float attackRange)
    {
        WeaponVisualData visualData = GetVisualData(weaponType);

        if (visualData?.particleSystem == null)
        {
            Debug.LogWarning($"?? Visual data not found for weapon: {weaponType}");
            return;
        }

        // Particle system'in pozisyonunu güncelle
        visualData.particleSystem.transform.position = position;

        // Range'e göre particle size'ý ayarla
        float particleSize = visualData.baseParticleSize * (attackRange / visualData.baseAttackRange);
        ParticleSystem.MainModule mainModule = visualData.particleSystem.main;
        mainModule.startSize = particleSize;

        // Mevcut particle system'i tetikle - instantiate yok!
        visualData.particleSystem.Play();

        Debug.Log($"? {weaponType} effect played at {position} with size {particleSize:F2}");
    }

    /// <summary>
    /// Silah türüne göre visual data'yý getirir
    /// </summary>
    private WeaponVisualData GetVisualData(WeaponType weaponType)
    {
        foreach (var visualData in weaponVisuals)
        {
            if (visualData.weaponType == weaponType)
                return visualData;
        }
        return null;
    }
}
