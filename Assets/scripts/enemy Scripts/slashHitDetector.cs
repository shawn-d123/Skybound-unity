using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slashHitDetector : MonoBehaviour
{
    public float damageAmount = 50f; // holds the damage that the attack does

    // procedure that detects if the collider did hit, if so damages the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();
            if (player != null)
            {
                player.takeDamage(damageAmount);
            }
        }
    }
}
