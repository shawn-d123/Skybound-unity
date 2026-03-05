using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 50; // Enemy starting health

    public void dealDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has been defeated!");
        Destroy(gameObject); // Remove enemy from the scene
    }
}