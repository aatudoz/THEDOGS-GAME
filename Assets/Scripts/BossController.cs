using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthBarFill;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.9f;
    [SerializeField] private float detectionRange = 100f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int attackDamage = 1;

    [Header("Boss Dash Attack")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDistance = 8f;
    [SerializeField] private float chargeUpTime = 1.5f;
    [SerializeField] private int dashDamage = 2;

    [Header("Collision")]
    [SerializeField] private LayerMask solidObjectsLayer;

    [Header("PowerUp Drop")]
    [SerializeField] private GameObject[] powerUps;
    [SerializeField] private float dropChance = 0.1f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private int currentHealth;
    private float lastAttackTime;
    private bool isDead = false;

    //boss specific 
    private bool isCharging = false;
    private bool isDashing = false;
    private bool hasUsedFirstDash = false;
    private bool hasUsedSecondDash = false;
    private bool hasUsedThirdDash = false;

    public event Action OnEnemyDeath;

    private UIManager uiManager;



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
        if (isCharging || isDashing)
        {
            return;
        }

        Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y);

        // check dash attack based on hp
        CheckForDashAttack();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // normal attack
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        // chase
        else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
    }

    void CheckForDashAttack()
    {
        //Dash at 40hp
        if (currentHealth <= 25 && !hasUsedFirstDash)
        {
            hasUsedFirstDash = true;
            StartCoroutine(DoDashAttack());
        }
        //Dash at 23hp
        else if (currentHealth <= 15 && !hasUsedSecondDash)
        {
            hasUsedSecondDash = true;
            StartCoroutine(DoDashAttack());
        }
        //Dash at 10hp
        else if (currentHealth <= 6 && !hasUsedThirdDash)
        {
            hasUsedThirdDash = true;
            StartCoroutine(DoDashAttack());
        }
    }

    IEnumerator DoDashAttack()
    {
        if (player == null) yield break;

        isCharging = true;
        rb.linearVelocity = Vector2.zero;
        Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y);

        //remember where player is standing
        Vector2 playerPosition = player.position;
        Vector2 dashDirection = (playerPosition - (Vector2)transform.position).normalized;

        if (dashDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        //charge duration
        Vector3 startPosition = transform.position;
        float chargeTimer = 0f;

        while (chargeTimer < chargeUpTime)
        {
            chargeTimer += Time.deltaTime;
            //shake sprite
            float shakeX = Mathf.Sin(Time.time * 20f) * 0.1f;
            float shakeY = Mathf.Cos(Time.time * 30f) * 0.1f;
            transform.position = startPosition + new Vector3(shakeX, shakeY, 0);

            //flash red and white
            float flash = Mathf.PingPong(Time.time * 10f, 1f);
            spriteRenderer.color = Color.Lerp(Color.white, Color.red, flash);

            yield return null;
        }

        //reset colors
        transform.position = startPosition;
        spriteRenderer.color = Color.white;
        isCharging = false;
        isDashing = true;

        //dash towards where player was
        Vector2 dashStart = transform.position;
        Vector2 dashEnd = dashStart + dashDirection * dashDistance;
        float dashTimer = 0f;
        float dashDuration = dashDistance / dashSpeed;

        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            float progress = dashTimer / dashDuration;

            Vector2 newPosition = Vector2.Lerp(dashStart, dashEnd, progress);
            rb.MovePosition(newPosition);

            // Check if we hit the player
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange * 1.5f)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(dashDamage);
                }
            }

            yield return null;
        }

        // Dash finished
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
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

        Vector2 attackPosition = new Vector2(transform.position.x, transform.position.y);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        DealDamage();

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

        DropPowerUp();

        StartCoroutine(AddScoreWithDelay(0.3f, 150));

        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowFloatingScore(transform.position, 150);
        }

        GetComponent<Collider2D>().enabled = false;

        OnEnemyDeath?.Invoke();

        Destroy(gameObject, 2f);

        Victory();
    }

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

    public void Victory()
    {
        StartCoroutine(VictoryDelay());
    }
    
    private IEnumerator VictoryDelay()
    {
        yield return new WaitForSeconds(2f);
        uiManager.ShowVictoryScreen();
    }
}