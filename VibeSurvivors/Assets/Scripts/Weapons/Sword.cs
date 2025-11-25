using UnityEngine;

/// <summary>
/// Örnek kılıç silahı - Weapon sınıfından türetilmiş
/// </summary>
public class Sword : Weapon
{
    [Header("Sword Effects")]
    [SerializeField] private ParticleSystem attackParticles;
    
    [Header("Particle Scaling")]
    [SerializeField] private float baseAttackRange = 2.5f;
    [SerializeField] private float baseParticleSize = 2f;
    
    private float currentParticleSize;

    private void Awake()
    {
        // Başlangıç particle size'ını sakla
        if (attackParticles != null)
        {
            currentParticleSize = baseParticleSize;
            UpdateParticleSize();
        }
    }

    protected override void OnTrigger()
    {
        if (attackParticles != null)
        {
            attackParticles.Play();
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            UpdateAttackRange(AttackRange + 0.5f);
        }
    }
    /// <summary>
    /// Hasar verildiğinde hit efekti
    /// </summary>
    protected override void OnDamageDealt(GameObject target)
    {
        Debug.Log($"💥 Sword hit {target.name} for {ItemDamage} damage!");
    }

    /// <summary>
    /// Stat güncellendiğinde - Attack range arttıkça particle size da artar
    /// </summary>
    protected override void OnStatsUpdated()
    {
        Debug.Log($"🔼 Sword stats updated - Damage: {ItemDamage}, Range: {AttackRange}");
        UpdateParticleSize();
    }

    /// <summary>
    /// Attack range'a orantılı olarak particle size'ı günceller
    /// Formül: currentParticleSize = baseParticleSize * (currentRange / baseRange)
    /// </summary>
    private void UpdateParticleSize()
    {
        if (attackParticles == null)
            return;

        // Orantılı hesaplama: baseParticleSize (2) ile baseAttackRange (2.5) arasındaki oran
        currentParticleSize = baseParticleSize * (AttackRange / baseAttackRange);

        // Particle System'deki main module'ü güncelle
        ParticleSystem.MainModule mainModule = attackParticles.main;
        mainModule.startSize = currentParticleSize;

        Debug.Log($"🎯 Particle size updated: {currentParticleSize:F2} (Range: {AttackRange})");
    }
}
