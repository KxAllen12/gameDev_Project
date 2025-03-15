using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
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
    }

    private void FixedUpdate()
    {
        Move();
        AdjustPlayerFacingDirection();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        // Set movement parameters for animation
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);

        float speed = movement.magnitude;

        // Add dead zone to avoid tiny inputs being registered
        if (speed < 0.1f)
        {
            movement = Vector2.zero;
        }
    }


    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.deltaTime));
    }

    public void AdjustPlayerFacingDirection()
    {
        // No flipping of sprite when moving up or down, only left or right
        if (movement.x < 0)
        {
            mySpriteRenderer.flipX = true;  // Flip to the left when moving left
        }
        else if (movement.x > 0)
        {
            mySpriteRenderer.flipX = false;  // Face right when moving right
        }
    }
}
