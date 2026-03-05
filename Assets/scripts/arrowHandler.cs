using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class arrowHandler : MonoBehaviour
{
    public float damage = 50f;
    private float bonusDamage;

    private void Start()
    {
        Destroy(gameObject, 10); // Auto-destroy arrow after 10 seconds
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore habitat zone triggers
        if (other.CompareTag("HabitatZone"))
        {
            return;
        }

        // Stick arrow to the hit object
        transform.SetParent(other.transform);
        Destroy(GetComponent<Rigidbody>());

        // Handle hitting enemies
        if (other.CompareTag("Enemy"))
        {
            float playerAttackStat = 0f;
            playerController playerStats = GameObject.FindWithTag("Player")?.GetComponent<playerController>();
            bonusDamage = playerStats.playerAttack;

            // Check baseEnemy (including inherited classes) using GetComponentInParent
            baseEnemy baseEnemyScript = other.GetComponentInParent<baseEnemy>();
            if (baseEnemyScript != null)
            {
                float totalDamage = damage + bonusDamage;
                baseEnemyScript.dealDamage((int)totalDamage);
                Debug.Log("Hit base enemy and dealt " + totalDamage + " damage.");
                return;
            }

            // Check for boss enemies if using separate class
            verdaliaBossEnemy boss = other.GetComponentInParent<verdaliaBossEnemy>();
            if (boss != null)
            {
                float totalDamage = damage + bonusDamage;
                boss.takeDamage(totalDamage);
                Debug.Log("Hit boss enemy and dealt " + totalDamage + " damage.");
                return;
            }

            Debug.LogWarning("Enemy tag found, but no valid damageable component on: " + other.name);
        }
    }
}