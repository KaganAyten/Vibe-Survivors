using UnityEngine;

/// <summary>
/// Basit zombi dü?man? - Enemy s?n?f?ndan türetilmi?
/// </summary>
public class Zombie : Enemy
{
    [Header("Zombie Specific")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float stopDistance = 1.5f;

    private Transform playerTransform;
    private bool isChasing = false;

    protected override void Start()
    {
        base.Start();
        
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

        // Oyuncu tespit menzilinde mi?
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;

      // Durma mesafesinden uzaktaysa yakla?
            if (distanceToPlayer > stopDistance)
   {
           ChasePlayer();
          }
  else
      {
     // Sald?r? menzilindeyse sald?r
       Player player = playerTransform.GetComponent<Player>();
       if (player != null)
                {
          AttackPlayer(player);
    }
  }
        }
        else
        {
 isChasing = false;
     }
    }

    /// <summary>
    /// Oyuncuya do?ru hareket eder
    /// </summary>
    private void ChasePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0f; // Y ekseninde hareket etmesin

        transform.position += direction * movementSpeed * Time.deltaTime;
        
  // Oyuncuya bak
        if (direction != Vector3.zero)
   {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    protected override void OnDamageTaken(float damageAmount)
    {
        base.OnDamageTaken(damageAmount);
        
        // Zombi özel hasar efekti
        Debug.Log($"?? Zombie hurt!");
    }

    protected override void OnDeath()
  {
    base.OnDeath();
        
 Debug.Log($"?? Zombie defeated!");
        
        // Zombi ölüm animasyonu/efekti
    }

    protected override void OnAttackPerformed(Player target)
    {
        base.OnAttackPerformed(target);
        
        Debug.Log($"?? Zombie bit the player for {damage} damage!");
  }

    // Gizmos ile tespit menzilini göster
    private void OnDrawGizmosSelected()
  {
   // Tespit menzili
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
      
      // Sald?r? menzili
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
