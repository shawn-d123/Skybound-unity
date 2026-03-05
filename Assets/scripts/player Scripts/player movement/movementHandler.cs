using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static playerStateManager;

public class movementHandler : MonoBehaviour
{
    // refrences to components
    public CharacterController characterController;
    public playerInputHandler playerInputHandler;
    public playerAnimationController playerAnimationController;
    public combatInputHandler combatInputHandler;
    public playerController playerController;

    // movement related variables 
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float rotationSmoothTime = 0.1f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float glideGravity = -2f;

    [SerializeField] public float glideStaminaDrainMultiplier = 10f;
    [SerializeField] public float sprintStaminaDrainMultiplier = 5f;
    [SerializeField]public float jumpStaminaCost = 8f;
    [SerializeField] private float groundDetectionRadius = 0.3f;
    [SerializeField] private float groundDetectionHeight = 0.15f;
    public LayerMask groundLayer;

   
    private float angleToRotate; // holds the smooth angle the player needs to rotate
    private Vector3 velocity;
    private bool isCrouching = false;
    private bool isDashing = false;
    private PlayerMovementState currentMovementState; // holds the players current state
    private float nextDashUseTime;
    private float dashCooldownTime = 1.5f;

    void Update()
    {
        HandleDash();

        // checks if play is dashing if so other movement is skipped and dash is used
        if (isDashing == true)
        {
            return;
        }

        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleGlide();
        ApplyGravity();

        // Updates the player animation based on the state the player is in 
        playerAnimationController.UpdateAnimations(currentMovementState, isCrouching, "None");
    }

    // checks if the player is currently in contact with the ground using a SphereCast
    public bool IsGrounded()
    {
        Vector3 sphereCastOrigin = characterController.bounds.center;
        bool grounded = Physics.SphereCast(sphereCastOrigin, groundDetectionRadius, Vector3.down, out _, groundDetectionHeight, groundLayer);
        if (grounded)
        {
            Debug.DrawRay(sphereCastOrigin, Vector3.down * groundDetectionHeight, Color.green);
        }
        else
        {
            Debug.DrawRay(sphereCastOrigin, Vector3.down * groundDetectionHeight, Color.red);
        }
        return grounded;
    }

    // procedure that handels the player speed, rotation to allow basic movement
    void HandleMovement()
    {
        Vector2 input = playerInputHandler.GetMovementInput(); // movement inputs
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y).normalized; // calculates direction to move
        bool isSprinting = playerInputHandler.IsSprinting();
        float moveSpeed;

        // checks if the player is aiming, if so speed is set to crouch
        bool isAiming = combatInputHandler.isAiming;

        if (isAiming == true)
        {
            moveSpeed = crouchSpeed;
        }
        else if (isCrouching == true)
        {
            moveSpeed = crouchSpeed;
        }
        else if (isSprinting == true && playerController.playerStamina > 0)
        {
            moveSpeed = sprintSpeed;
            // continually drains stamina when sprinting
            playerController.drainStamina(sprintStaminaDrainMultiplier * Time.deltaTime);
        }
        else
        {
            moveSpeed = walkSpeed;
        }
        characterController.Move(Vector3.zero);

        // checks if  movement input is recieved, if so starts movement calculations
        if (moveDirection.magnitude >= 0.1f)
        {
            // player movement direction is calculated, and is earped fro smooth transition and moves a specified movement speed
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + playerInputHandler.GetCameraRotation();
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref angleToRotate, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            Vector3 movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(movementDirection.normalized * moveSpeed * Time.deltaTime);
        }

        UpdatePlayerState(input.x, input.y, isSprinting);
    }

    // checks if the player can jump, if so applies an upward force
    void HandleJump()
    {
        // checks if the player is pressing jump and grounded, to perform a jump
        if (playerInputHandler.IsJumping() && IsGrounded() && playerController.playerStamina >= jumpStaminaCost)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            currentMovementState = PlayerMovementState.Jumping;

            playerController.drainStamina(jumpStaminaCost); // drains stamina when jumping
        }
    }

    // procedure that toggles crouch when c is pressed
    void HandleCrouch()
    {
        if (playerInputHandler.IsCrouching())
        {
            if (!isCrouching) 
            {
                isCrouching = true;
                currentMovementState = PlayerMovementState.Crouching;
                characterController.height = 1f; // Reduce player height when crouching
                characterController.center = new Vector3(0, 0.5f, 0); // Adjusts player center to match new height
            }
            else 
            {
                isCrouching = false;
                currentMovementState = PlayerMovementState.Walking;
                characterController.height = 2f; // Reset player to normal height
                characterController.center = new Vector3(0, 1f, 0);
            }
        }
    }

    // procedure that handels dashing
    void HandleDash()
    {
        // dash stamina is drained which is 25% and 10 
        float dashStaminaCost = (playerController.playerStamina * 0.25f) + 10;

        // checks if the player pressed the dash button, is not already dashing,
        // is grounded, and enough time has passed since the last dash
        if (playerInputHandler.IsDashing() && isDashing == false && IsGrounded() && Time.time >= nextDashUseTime
            && playerController.playerStamina >= dashStaminaCost)
        {
            isDashing = true;
            currentMovementState = PlayerMovementState.Dashing;

            // starts cooldown 
            nextDashUseTime = Time.time + dashCooldownTime;

            playerController.drainStamina(dashStaminaCost);

            // Start the coroutine to handle dash movement 
            StartCoroutine(DashCoroutine());
        }
    }

    // corutine that implements a 3 second delay before next dash is available 
    IEnumerator DashCoroutine()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration) 
        {
            // moves the player forward with dash speed
            Vector3 dashDirection = transform.forward * dashSpeed;
            characterController.Move(dashDirection * Time.deltaTime);

            yield return null;
        }

        // dash ends andsate is set to idel
        isDashing = false;
        currentMovementState = PlayerMovementState.Idle;
    }

    // procedure that handels gliding, if conditions are met
    void HandleGlide()
    {
        // checks if the player is in the air and holding jump, if so starts gliding 
        if (IsGrounded() == false && playerInputHandler.IsHoldingJump() && velocity.y < 0 && playerController.playerStamina > 0)
        {
            currentMovementState = PlayerMovementState.Gliding;
            velocity.y = glideGravity;
            // continually drains stamina when gliding 
            playerController.drainStamina(glideStaminaDrainMultiplier * Time.deltaTime);
        }

        else if (!IsGrounded() && velocity.y < 0 && playerController.playerStamina <= 0)
        {

            playerAnimationController.playerAnimator.SetBool("isFalling", true);
            playerAnimationController.playerAnimator.SetBool("isGliding", false);
        }
    }
    
    // procedure that applies gravity to the player to keep it grounded
    void ApplyGravity()
    {
        if (IsGrounded())
        {
            if (velocity.y < 0) // checks if gravity is less than 0 
            {
                velocity.y = -2f; // sets it to -2 
                currentMovementState = PlayerMovementState.Idle;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    // procedure that determines the players state 
    void UpdatePlayerState(float xInput, float yInput, bool isSprinting)
    {
        if (currentMovementState == PlayerMovementState.Jumping)
        {
            return;
        }
        if (currentMovementState == PlayerMovementState.Crouching)
        {
            return;
        }
        if (currentMovementState == PlayerMovementState.Dashing)
        {
            return;
        }
        if (currentMovementState == PlayerMovementState.Gliding)
        {
            return;
        }

        if (IsGrounded() == false && velocity.y < -0.1f)
        {
            currentMovementState = PlayerMovementState.Falling;
        }
        else if (xInput == 0 && yInput == 0)
        {
            currentMovementState = PlayerMovementState.Idle;
        }
        else
        {
            if (isSprinting)
            {
                currentMovementState = PlayerMovementState.Sprinting;
            }
            else
            {
                currentMovementState = PlayerMovementState.Walking;
            }
        }
    }
}
