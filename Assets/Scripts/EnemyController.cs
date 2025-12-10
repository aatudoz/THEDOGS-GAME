using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] private Image healthBarFill;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.9f;
    [SerializeField] private float detectionRange = 100f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 9;
    [SerializeField] private int attackDamage = 1;

    [Header("Collision")]
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask ScoreAddSound;

    //powerUps
    [Header("PowerUp Drop")]
    [SerializeField] private GameObject[] powerUps; //prefabs here
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.1f; //10% chance for drop


    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private int currentHealth;
    private float lastAttackTime;
    private bool isDead = false;

    public event Action OnEnemyDeath;
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        if (isDead) return;

        if (player == null) return;

        Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // In attack range!!
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        //Chase player if enemy cant attack
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
        DealDamage();
    }

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
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("hurtTrigger");
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {   
        if (isDead) return;
        
        isDead = true;
        rb.linearVelocity = Vector2.zero;


        animator.SetTrigger("deathTrigger");

        
        //rng powerup drop
        DropPowerUp();


        //scorenlisays
        StartCoroutine(AddScoreWithDelay(0.3f, 150));

       
        //pisteet pamahtaa naytolle kiitos
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowFloatingScore(transform.position, 150);
        }
        

        //Disabling enemy collider so turkey can walk through
        GetComponent<Collider2D>().enabled = false;

        //Calls to WaveManager.cs
        OnEnemyDeath?.Invoke();

        //Destroy after death animation
        Destroy(gameObject, 2f);
    }

    //delay scoreen
    private IEnumerator AddScoreWithDelay(float delay, int scoreAmount)
    {
        yield return new WaitForSeconds(delay);


        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.AddScore(scoreAmount); 
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPosition, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }

    private void DropPowerUp()
    {
        if (powerUps.Length == 0) return;

        float roll = UnityEngine.Random.value;
        if (roll <= dropChance)
        {
            int index = UnityEngine.Random.Range(0, powerUps.Length);
            Instantiate(powerUps[index], transform.position, Quaternion.identity);
        }
    }
}