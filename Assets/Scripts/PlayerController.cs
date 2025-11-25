using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    //collision
    public LayerMask solidObjectsLayer;

    //Future stats
    //private int score = 0;
    //private int maxHealth = 100;
    //private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Future: Initialize health
        //currentHealth = maxHealth;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        float moveY = Input.GetAxisRaw("Vertical");   // W/S or Up/Down arrows

        moveInput = new Vector2(moveX, moveY).normalized;

        UpdateAnimations();

        //Future: Add shooting input here
        //if (Input.GetMouseButtonDown(0)) { Shoot(); }
    }

    void FixedUpdate()
    {   
        // //Move the player
        // rb.linearVelocity = moveInput * moveSpeed;
        if (moveInput != Vector2.zero)
    {
        //Predict the next position
        Vector2 nextPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        //Only move if the next position is walkable
        if (isWalkable(nextPos))
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    else
    {
        rb.linearVelocity = Vector2.zero;
    }
    }

    void UpdateAnimations()
    {
        bool isMoving = moveInput.magnitude > 0;
        animator.SetBool("WalkRight", isMoving);

        //Flip sprite based on movement direction
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false; //Face right
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;  //Face left
        }
    }

    //collision
    private bool isWalkable(Vector3 targetPos)
    {
        //Check if the target position collides with anything on the solidObjectsLayer
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == null;
    }

    // Future methods to implement:

    // void Shoot()
    // {
    //     // Shooting logic
    // }

    // public void AddScore(int points)
    // {
    //     score += points;
    // }

    // public void TakeDamage(int damage)
    // {
    //     currentHealth -= damage;
    //     if (currentHealth <= 0) Die();
    // }

    // void Die()
    // {
    //     // Death logic
    // }
}