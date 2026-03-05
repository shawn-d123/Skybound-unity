using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static playerStateManager;

public class playerAnimationController : MonoBehaviour
{
    // reference to the animator component that controls the player animations
    public Animator playerAnimator;
    public combatInputHandler combatInputHandler;

    // reference to the input handler that provides movement input from the player
    public playerInputHandler InputHandler;

    [SerializeField] float blendSensitivity = 5;


    public void UpdateAnimations(PlayerMovementState state, bool isCrouching, string attackState)
    {
        bool isAiming = combatInputHandler.isAiming;

        // blendMultiplier variables are used to hold the value as it moves towards the target value 
        float blendValueX = Mathf.Lerp(playerAnimator.GetFloat("xInput"), InputHandler.GetMovementInput().x, Time.deltaTime * blendSensitivity);
        float blendValueY = Mathf.Lerp(playerAnimator.GetFloat("yInput"), InputHandler.GetMovementInput().y, Time.deltaTime * blendSensitivity);

        // input values which are used to control the blend tree
        playerAnimator.SetFloat("xInput", blendValueX);
        playerAnimator.SetFloat("yInput", blendValueY);

        // crouch is set to true when crouching, to triiger the crouch animation
        playerAnimator.SetBool("isCrouching", isCrouching);

        // checks if the player is jumping, if so updates the animator
        if (state == PlayerMovementState.Jumping)
        {
            playerAnimator.SetBool("isJumping", true);
        }
        else
        {
            playerAnimator.SetBool("isJumping", false);
        }

        // Check if the player is falling and has no movement input,if so updates the animator
        if (state == PlayerMovementState.Falling)
        {
            if (InputHandler.GetMovementInput().x == 0 && InputHandler.GetMovementInput().y == 0)
            {
                playerAnimator.SetBool("isFalling", true);
            }
            else
            {
                playerAnimator.SetBool("isFalling", false);
            }
        }
        else
        {
            playerAnimator.SetBool("isFalling", false);
        }

        // checks if the player is gliding, if so updates the animator
        if (state == PlayerMovementState.Gliding)
        {
            playerAnimator.SetBool("isGliding", true);
        }
        else
        {
            playerAnimator.SetBool("isGliding", false);
        }

        // checks if the player is dashing, if so updates the animator
        if (state == PlayerMovementState.Dashing)
        {
            playerAnimator.SetBool("isDashing", true);
        }
        else
        {
            playerAnimator.SetBool("isDashing", false);
        }
        if (attackState != "None")
        {
            playerAnimator.SetTrigger(attackState);
        }

        // checks if the player is Sprinting, if so updates the animator
        if (state == PlayerMovementState.Sprinting && !isAiming)
        {
            playerAnimator.SetBool("isSprinting", true);
        }
        else
        {
            playerAnimator.SetBool("isSprinting", false);
        }
    }
}
