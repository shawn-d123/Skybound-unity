using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossBiteAttackDetection : MonoBehaviour
{
    public float damageAmount = 200f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent<playerController>();
            if (player != null)
            {
                player.takeDamage(damageAmount);
                Debug.Log("boss bit player and dealt damage.");
            }
        }
    }
}
