using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int attackDamage = 1;

    [Header("Collision")]
    [SerializeField] private LayerMask solidObjectsLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private int currentHealth;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // In attack range!!
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        // Chase player if enemy cant attack
        else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 nextPos = rb.position + direction * moveSpeed * Time.deltaTime;

        if (IsWalkable(nextPos))
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Flip sprite
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetFloat("speed", rb.linearVelocity.magnitude);
    }

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("attackTrigger");
        lastAttackTime = Time.time;

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    // Call this from animation event at the middle of attack animation
    public void DealDamage()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("hurtTrigger");
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("deathTrigger");

        // Disabling enemy collider so turkey can walk through
        GetComponent<Collider2D>().enabled = false;

        // Destroy after death animation
        Destroy(gameObject, 2f);
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}