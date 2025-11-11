using UnityEngine;

/// <summary>
/// Uzaktan sald?ran iskelet dü?man? - Enemy s?n?f?ndan türetilmi?
/// </summary>
public class Skeleton : Enemy
{
    [Header("Skeleton Specific")]
    [SerializeField] private float shootRange = 15f;
    [SerializeField] private float safeDistance = 5f; // Oyuncudan uzak durma mesafesi
  [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

private Transform playerTransform;

    protected override void Start()
    {
        base.Start();
        
        // ?skelet için özel de?erler
        attackCooldown = 3f; // Daha yava? sald?r?
   
        // Oyuncuyu bul
        Player player = FindObjectOfType<Player>();
 if (player != null)
        {
   playerTransform = player.transform;
   }
    }

    private void Update()
    {
     if (isDead || playerTransform == null)
   return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

   // Oyuncuya her zaman bak
Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
directionToPlayer.y = 0f;
        if (directionToPlayer != Vector3.zero)
        {
     transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        // Oyuncu çok yak?nsa geri çekil
        if (distanceToPlayer < safeDistance)
        {
   Retreat();
  }
  // Sald?r? menzilindeyse ate? et
    else if (distanceToPlayer <= shootRange)
        {
Player player = playerTransform.GetComponent<Player>();
    if (player != null && CanAttack())
    {
       ShootAtPlayer(player);
  }
        }
    }

    /// <summary>
    /// Oyuncudan uzakla??r
    /// </summary>
 private void Retreat()
 {
        Vector3 direction = (transform.position - playerTransform.position).normalized;
        direction.y = 0f;
        transform.position += direction * movementSpeed * 0.5f * Time.deltaTime; // Yar? h?zda geri çekilir
    }

/// <summary>
    /// Oyuncuya ok atar
    /// </summary>
    private void ShootAtPlayer(Player player)
    {
        lastAttackTime = Time.time;
        
  if (projectilePrefab != null && shootPoint != null)
  {
// Projectile f?rlat
     GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
   
   // Projectile'a hasar bilgisi ekle (ileride Projectile script'i eklenebilir)
 // projectile.GetComponent<Projectile>().Initialize(damage, player);
   
         Debug.Log($"?? Skeleton shot an arrow at player!");
     }
        else
        {
  // Projectile yoksa direkt hasar ver
    player.TakeDamage(damage);
        }

     OnAttackPerformed(player);
    }

protected override void OnDamageTaken(float damageAmount)
    {
    base.OnDamageTaken(damageAmount);
  
Debug.Log($"?? Skeleton hurt!");
  }

    protected override void OnDeath()
    {
 base.OnDeath();
        
        Debug.Log($"?? Skeleton defeated!");
    }

    protected override void OnAttackPerformed(Player target)
{
        base.OnAttackPerformed(target);
    }

 // Gizmos
    private void OnDrawGizmosSelected()
    {
        // Sald?r? menzili
   Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
 
 // Güvenli mesafe
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
