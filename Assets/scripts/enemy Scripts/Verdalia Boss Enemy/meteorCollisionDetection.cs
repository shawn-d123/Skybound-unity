using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meteorCollisionDetection : MonoBehaviour
{
    public float damageAmount = 500f;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to get the player controller
            playerController player = other.GetComponent<playerController>();

                player.takeDamage(damageAmount);
                Debug.Log("Meteor particle hit the player and dealt damage!");
        }
    }
}
