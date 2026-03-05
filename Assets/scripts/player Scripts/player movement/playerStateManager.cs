using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateManager : MonoBehaviour
{
    // This enum holds all the movement states for player
    public enum PlayerMovementState
    {
        Idle,      
        Walking,   
        Sprinting, 
        Jumping,   
        Crouching, 
        Gliding,   
        Dashing,   
        Falling   
    }
}

