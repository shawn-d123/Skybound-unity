using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordHandler : MonoBehaviour
{
    public float damage = 200f;
    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit an object tagged as Enemy
        if (other.CompareTag("Enemy"))
        {
            // First try baseEnemy (for base and boss enemies)
            baseEnemy baseEnemyScript = other.GetComponent<baseEnemy>();
            if (baseEnemyScript != null)
            {
                baseEnemyScript.dealDamage((int)damage); // Cast if needed
                Debug.Log("Hit base enemy and dealt " + damage + " damage.");
                return;
            }

            // Then check if it's a boss enemy (if bosses use separate scripts)
            verdaliaBossEnemy boss = other.GetComponent<verdaliaBossEnemy>();
            if (boss != null)
            {
                boss.takeDamage(damage);
                Debug.Log("Hit boss enemy and dealt " + damage + " damage.");
                return;
            }

            Debug.LogWarning("Enemy tag found, but no valid damageable component on: " + other.name);
        }
    }
}