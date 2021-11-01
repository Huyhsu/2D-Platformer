using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    private float movementInputDirection;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;

    private Rigidbody2D _myRigidbody2D;
    private Animator _myAnimator;

    public int amountOfJumps = 1;
    
    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce; 

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;
    
    // Start is called before the first frame update
    void Start()
    {
        _myRigidbody2D = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && _myRigidbody2D.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right,wallCheckDistance,whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if ((isGrounded && _myRigidbody2D.velocity.y<=0) || isWallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        canJump = amountOfJumpsLeft > 0;
    }
    
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(_myRigidbody2D.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
        
    }

    private void UpdateAnimations()
    {
        _myAnimator.SetBool("isWalking",isWalking);
        _myAnimator.SetBool("isGrounded", isGrounded);
        _myAnimator.SetFloat("yVelocity",_myRigidbody2D.velocity.y);
        _myAnimator.SetBool("isWallSliding",isWallSliding);
        
    }
    
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            _myRigidbody2D.velocity = new Vector2(_myRigidbody2D.velocity.x, _myRigidbody2D.velocity.y * variableJumpHeightMultiplier);
        }
    }

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            _myRigidbody2D.velocity = new Vector2(_myRigidbody2D.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
        else if (isWallSliding && movementInputDirection == 0 && canJump) //Wall hop
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            _myRigidbody2D.AddForce(forceToAdd,ForceMode2D.Impulse);
        }
        else if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump) //Wall jump
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            _myRigidbody2D.AddForce(forceToAdd,ForceMode2D.Impulse);  
        }
    }
    
    private void ApplyMovement()
    {
        if (isGrounded)
        {
            _myRigidbody2D.velocity = new Vector2(movementSpeed * movementInputDirection, _myRigidbody2D.velocity.y);
        }
        else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection,0);
            _myRigidbody2D.AddForce(forceToAdd);

            if (Mathf.Abs(_myRigidbody2D.velocity.x) > movementSpeed)
            {
                _myRigidbody2D.velocity = new Vector2(movementSpeed * movementInputDirection, _myRigidbody2D.velocity.y);
            }
        }else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            _myRigidbody2D.velocity = new Vector2(_myRigidbody2D.velocity.x * airDragMultiplier, _myRigidbody2D.velocity.y);
        }
        
        if (isWallSliding)
        {
            if (_myRigidbody2D.velocity.y < -wallSlideSpeed)
            {
                _myRigidbody2D.velocity = new Vector2(_myRigidbody2D.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f,180.0f,0.0f);        
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position,
            new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
