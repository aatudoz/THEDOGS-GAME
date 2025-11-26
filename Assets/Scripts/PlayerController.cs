using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 14f;
    [SerializeField] private float friction = 10f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Transform gunTransform;
    private Transform gunRight;
    private Transform gunLeft;

    //collision
    public LayerMask solidObjectsLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        gunTransform = transform.Find("Gun");
        gunRight = transform.Find("GunRight");
        gunLeft = transform.Find("GunLeft");
    }

    void Update()
    {
        //Flip based on mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePos.x < transform.position.x)
        {
            //facing left
            spriteRenderer.flipX = true;

            if (gunTransform != null && gunLeft != null)
                gunTransform.localPosition = gunLeft.localPosition;
        }
        else
        {
            //facing right
            spriteRenderer.flipX = false;

            if (gunTransform != null && gunRight != null)
                gunTransform.localPosition = gunRight.localPosition;
        }

        //WASD input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (moveInput != Vector2.zero)
        {
            Vector2 nextPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

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
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private bool isWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == null;
    }
}
