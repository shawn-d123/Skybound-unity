using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemytestkey : MonoBehaviour
{
    public baseEnemy enemyToDamage;   // Drag your enemy into this slot in the Inspector
    public int damageAmount = 30;     // Amount of damage to apply

    void Update()
    {
        // Press H to simulate hitting the enemy
        if (Input.GetKeyDown(KeyCode.H) && enemyToDamage != null)
        {
            enemyToDamage.dealDamage(damageAmount);
            Debug.Log("🔨 Test Damage Applied: " + damageAmount);
        }
    }
}
