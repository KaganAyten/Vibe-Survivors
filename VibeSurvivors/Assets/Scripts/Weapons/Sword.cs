using UnityEngine;

/// <summary>
/// Örnek kılıç silahı - Weapon sınıfından türetilmiş
/// </summary>
public class Sword : Weapon
{
    [Header("Sword Animation")]
    [SerializeField] private Transform weaponObject;
    [SerializeField] private float swingDuration = 0.3f; // Animasyon süresi (saniye)

    private bool isSwinging = false;
    private float currentSwingTime = 0f;

    // Rotasyon değerleri
    private readonly Vector3 startRotation = new Vector3(90f, 0f, 90f);   // Başlangıç: x=90, y=0, z=90
    private readonly Vector3 endRotation = new Vector3(90f, 0f, -90f);     // Bitiş: x=90, y=0, z=-90

    private void Awake()
    {
        // Başlangıç rotasyonunu ayarla
        if (weaponObject != null)
        {
            weaponObject.localRotation = Quaternion.Euler(startRotation);
        }
    }

    private void Update()
    {
        if (isSwinging)
        {
            AnimateSwing();
        }
    }

    protected override void OnTrigger()
    {
        if (weaponObject != null && !isSwinging)
        {
            isSwinging = true;
            currentSwingTime = 0f;
        }
    }

    private void AnimateSwing()
    {
        currentSwingTime += Time.deltaTime;
        float progress = currentSwingTime / swingDuration;

        if (progress <= 1f)
        {
            // Başlangıçtan (90, 0, 90) bitişe (90, 0, -90) yumuşak geçiş
            Vector3 currentRotation = Vector3.Lerp(startRotation, endRotation, progress);
            weaponObject.localRotation = Quaternion.Euler(currentRotation);
        }
        else
        {
            // Animasyon bitti - başa dön
            weaponObject.localRotation = Quaternion.Euler(startRotation);
            isSwinging = false;
        }
    }

    /// <summary>
    /// Animasyon süresini dışarıdan ayarlamak için
    /// </summary>
    /// <param name="duration">Yeni animasyon süresi</param>
    public void SetSwingDuration(float duration)
    {
        swingDuration = Mathf.Max(0.05f, duration); // Minimum 0.05 saniye
    }

    /// <summary>
    /// Hasar verildiğinde hit efekti
    /// </summary>
    protected override void OnDamageDealt(GameObject target)
    {
        Debug.Log($"💥 Sword hit {target.name} for {ItemDamage} damage!");
    }

    /// <summary>
    /// Stat güncellendiğinde
    /// </summary>
    protected override void OnStatsUpdated()
    {
        Debug.Log($"🔼 Sword stats updated - Damage: {ItemDamage}, Range: {AttackRange}");
    }

    // Kılıcı başlangıç pozisyonuna resetle (opsiyonel)
    public void ResetWeaponRotation()
    {
        if (weaponObject != null)
        {
            weaponObject.localRotation = Quaternion.Euler(startRotation);
            isSwinging = false;
        }
    }
}
