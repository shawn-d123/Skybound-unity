using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInputHandler : MonoBehaviour
{
    // procedure that returns input values
    public Vector2 GetMovementInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        return new Vector2(horizontalInput, verticalInput);
    }

    // Checks if the sprint is currently being held, if so isSprinting is true
    public bool IsSprinting()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        return isSprinting;
    }

    // Checks if the space is currently being held, if so jump is triggered
    public bool IsJumping()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        return jumpPressed;
    }

    // Checks if the jump key is currently being held down
    public bool IsHoldingJump()
    {
        bool jumpHeld = Input.GetKey(KeyCode.Space);
        return jumpHeld;
    }

    // Checks if the c has just been pressed
    public bool IsCrouching()
    {
        bool crouchPressed = Input.GetKeyDown(KeyCode.C);
        return crouchPressed;
    }

    // Checks if the dash key E has just been pressed
    public bool IsDashing()
    {
        bool dashPressed = Input.GetKeyDown(KeyCode.E);
        return dashPressed;
    }

    // procedure that accesses the camera transform and returns its rotation
    public float GetCameraRotation()
    {
        float cameraRotation = Camera.main.transform.eulerAngles.y;
        return cameraRotation;
    }
}
