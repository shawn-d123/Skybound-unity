using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatInputHandler : MonoBehaviour
{
    // Reference to Combat Controller to handle attacks
    public combatController combatController; // refrence to the combat controller script

    [SerializeField]public bool isAiming;
    [SerializeField] public bool isShooting;
    
    // called every frame
    void Update()
    {
        handleAimingInput();

        handleShootingInput();
    }


    // procedure that checks if right click is being hels - the player is aiming
    private void handleAimingInput()
    {
        // Detect right mouse button press
        if (Input.GetMouseButtonDown(1))  // Right-click pressed
        {
            if (!isAiming)  // Only change if aiming is not already true
            {
                isAiming = true;
                Debug.Log("isAiming is true");
            }
        }

        // Detect right mouse button release
        else if (Input.GetMouseButtonUp(1))  // Right-click released
        {
            if (isAiming == true)  // Only change if aiming is true
            {
                isAiming = false;
                Debug.Log("isAiming is false");
            }
        }
    }

    // procedure that checks if the plaer has left clicked and aiming
    // if so sets isShooting to True
    public void handleShootingInput()
    {
        if (Input.GetMouseButtonDown(0) && isAiming == true) 
        {
            isShooting = true;
        }
        else
        {
            isShooting = false;
        }
    }

}


