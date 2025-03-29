using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }
    [SerializeField] private float moveSpeed = 1f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private Camera mainCamera;

    private bool facingLeft = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        // Set movement parameters for animation
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);

        // Add dead zone to avoid tiny inputs being registered
        if (movement.magnitude < 0.1f)
        {
            movement = Vector2.zero;
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.deltaTime));
    }

    private void UpdateFacingDirection()
    {
        // Priority system: Mouse position overrides movement direction
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // Only consider left/right (ignore Y-axis difference)
        if (mouseWorldPosition.x < transform.position.x)
        {
            mySpriteRenderer.flipX = true;  // Face left
            facingLeft = true;
        }
        else if (mouseWorldPosition.x > transform.position.x)
        {
            mySpriteRenderer.flipX = false; // Face right
            facingLeft = false;
        }

        // Fallback to movement direction if mouse isn't moving much
        if (Vector2.Distance(mouseScreenPosition, mainCamera.WorldToScreenPoint(transform.position)) < 10f)
        {
            if (movement.x < -0.1f)
            {
                mySpriteRenderer.flipX = true;
            }
            else if (movement.x > 0.1f)
            {
                mySpriteRenderer.flipX = false;
            }
        }
    }
}