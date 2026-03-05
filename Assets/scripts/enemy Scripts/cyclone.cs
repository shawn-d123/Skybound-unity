using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cyclone : MonoBehaviour
{
    public float lifetime = 5f;              // Time before auto-destroy
    public float damageAmount = 200f;         // Damage dealt to player
    private bool hasHitPlayer = false;       // Prevent multiple hits

    private void Start()
    {
        // Start coroutine to auto-destroy after some time
        StartCoroutine(autoDestroyAfterTime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHitPlayer) return;

        // Check if the collision was with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Try to get the player script and apply damage
            playerController player = collision.gameObject.GetComponent<playerController>();
            if (player != null)
            {
                player.takeDamage(damageAmount);
                Debug.Log("Cyclone hit player and dealt damage.");
            }

            hasHitPlayer = true;
            Destroy(gameObject); // Destroy cyclone immediately after hitting player
        }
    }

    private IEnumerator autoDestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);

        if (!hasHitPlayer)
        {
            Debug.Log("Cyclone expired without hitting player.");
            Destroy(gameObject);
        }
    }
}

