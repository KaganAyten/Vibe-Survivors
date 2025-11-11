using UnityEngine;

/// <summary>
/// Dü?man temel s?n?f? - Tüm dü?manlar buradan türer
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected float movementSpeed = 3f;
    [SerializeField] protected float damage = 5f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 2f;

    [Header("Rewards")]
    [SerializeField] protected int goldDropAmount = 10;
    [SerializeField] protected float xpDropAmount = 25f;

    [Header("Visual Feedback")]
    [SerializeField] protected Color damageColor = Color.red;
    [SerializeField] protected float damageFlashDuration = 0.1f;

    // Runtime de?erler
    protected float currentHealth;
    protected bool isDead = false;
    protected Renderer enemyRenderer;
    protected Color originalColor;
    protected float lastAttackTime = 0f;

    public bool IsAlive => !isDead && currentHealth > 0f;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float Damage => damage;

    protected virtual void Awake()
    {
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    protected virtual void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Dü?man?n ba?lang?ç de?erlerini ayarlar
    /// </summary>
    public virtual void Initialize()
    {
        currentHealth = maxHealth;
        isDead = false;
        lastAttackTime = -attackCooldown; // ?lk sald?r?y? hemen yapabilsin

        Debug.Log($"?? {gameObject.name} initialized - Health: {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// IDamageable interface implementasyonu
    /// </summary>
    public virtual void TakeDamage(float damageAmount)
    {
        if (isDead)
            return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0f, currentHealth);

        Debug.Log($"?? {gameObject.name} took {damageAmount} damage! Health: {currentHealth}/{maxHealth}");

        OnDamageTaken(damageAmount);

        // Can 0 veya alt?na dü?tü mü?
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Dü?man ölür
    /// </summary>
    protected virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;

        Debug.Log($"?? {gameObject.name} died!");

        // Loot drop
        DropLoot();

        OnDeath();

        // Ölüm animasyonu vs.
        Destroy(gameObject, 0.5f);
    }

    /// <summary>
    /// Loot b?rak?r (XP/Gold)
    /// </summary>
    protected virtual void DropLoot()
    {
        // Oyuncuyu bul ve XP/Gold ver
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.AddXP(xpDropAmount);
            player.AddGold(goldDropAmount);
        }
    }

    /// <summary>
    /// Oyuncuya sald?r? yapabilir mi kontrol eder
    /// </summary>
    protected virtual bool CanAttack()
    {
        return !isDead && Time.time >= lastAttackTime + attackCooldown;
    }

    /// <summary>
    /// Oyuncuya sald?r?r
    /// </summary>
    protected virtual void AttackPlayer(Player player)
    {
        if (!CanAttack() || player == null || !player.IsAlive)
            return;

        lastAttackTime = Time.time;
        player.TakeDamage(damage);

        OnAttackPerformed(player);
    }

    #region Virtual Methods (Override için)

    /// <summary>
    /// Hasar al?nd???nda ça?r?l?r
    /// </summary>
    protected virtual void OnDamageTaken(float damageAmount)
    {
        // Hasar alma efekti
        if (enemyRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(DamageFlash());
        }
    }

    /// <summary>
    /// Ölüm an?nda ça?r?l?r
    /// </summary>
    protected virtual void OnDeath()
    {
        // Override edilebilir - ölüm efektleri vs.
    }

    /// <summary>
    /// Sald?r? yap?ld???nda ça?r?l?r
    /// </summary>
    protected virtual void OnAttackPerformed(Player target)
    {
        // Override edilebilir - sald?r? efektleri vs.
        Debug.Log($"?? {gameObject.name} attacked player!");
    }

    #endregion

    /// <summary>
    /// Hasar alma renk efekti
    /// </summary>
    protected virtual System.Collections.IEnumerator DamageFlash()
    {
        enemyRenderer.material.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        enemyRenderer.material.color = originalColor;
    }

    // Debug için health bar
    protected virtual void OnGUI()
    {
        if (isDead)
            return;

        Vector3 worldPos = transform.position + Vector3.up * 2f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z > 0)
        {
            float healthPercent = currentHealth / maxHealth;
            Rect barRect = new Rect(screenPos.x - 25, Screen.height - screenPos.y - 10, 50, 5);

            GUI.color = Color.black;
            GUI.DrawTexture(barRect, Texture2D.whiteTexture);

            barRect.width *= healthPercent;
            GUI.color = Color.Lerp(Color.red, Color.green, healthPercent);
            GUI.DrawTexture(barRect, Texture2D.whiteTexture);

            GUI.color = Color.white;
        }
    }
}
