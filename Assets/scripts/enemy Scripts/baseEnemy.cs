using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static enemyStateMachine;

public class baseEnemy : MonoBehaviour
{
    public Vector3 spawnPoint;        // holds he spawn location of the enemy

    // enemy stata related variables
    public float enemyHealth = 300f;          // enemy max health
    public int enemyLevel = 3;             // holds the enemy level
    public int enemyXP = 600;           // holds the XP that will be rewarded to the player

    // enemy range related variables
    public float enemyDetectionRange = 12f;  // holds the range where the enemy detects the player 
    public float closeAttackRange = 2f;  // holds distance for close ranged attack
    public float midAttackRange = 8f;  // holds distance for mid ranged attack

    // enemy timer realated variables
    public float idleTime = 4f;   // holds the amount of time the enmy stays in idle before patroling
    public float timeBetweenAttacks = 2f;   // holds the cooldown between attaks
    public float staggerTime = 1.5f; // holds the time the enemy is staggered 
    public float maximumChaseTime = 240f; // holds the maximum time the enemy can chase the player
    public float globalCooldownEndTime = 0f;  // holds the next time enemy can attack

    // enemy speed related variables
    public float walkPointRange = 10f; // holds the range in which the enemy can patrol
    public float patrolSpeed = 2f;  // holds the speed at which the enemy patrols
    public float chaseSpeed = 3f;  // holds the speed at which the enemy chases
    public float fleeSpeed = 5f;  // holds the enemy flee speed

    // enemy refrences
    public Transform playerPosition;        // refrence to the player position
    public NavMeshAgent enemyNavMeshAgent;    // refrence to the navmesh agent component
    public enemyState currentState;  // refrence to the current enemy state
    public enemyHealthBar enemyHealthBar;     // refrence to the enemy health bar


    public bool hasPlayerInitiatedAttack = false;  // holds if the player has hit the enemy
    public bool isTakingDamage = false;  // holds if the enemy has staggred
    public bool isEnemyInsideHabitat = true;   // holds if enemy is inside the habitat collider
    public bool hasEnemyAttacked = false;  // holds if enemy has attaced to trigger cooldown

    // Patrol related variables
    public Vector3 walkPointPosition;        // holds the position of the walkpoint
    public bool iswalkPointSet = false; // holds if the wal point has been assigned
    public float idleEndTime = 0f;  // holds the time when the enmy has to stop idling

    public float timeSinceChaseStarted = 0f;  // holds the time since the enmy started chasing 

    public string lastAttackType = "";           // holds the attack that has been used 
    public int numberOfConsecutiveSameAttack = 0; 

    // Reference to the state handler 
    protected enemyStateHandler stateHandler;


    protected virtual void Start()
    {
        playerPosition = GameObject.FindWithTag("Player") ?.transform; // finds the player position and assignes it
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        stateHandler = new enemyStateHandler(this);              

        stateHandler.setInitialState(enemyState.idle);

        spawnPoint = transform.position; // sets the enemy position to spawn point 
        
        // assignes all the variables for the enemy health bar
        enemyHealthBar = GetComponentInChildren<enemyHealthBar>();
        enemyHealthBar.setMaxHealth(enemyHealth);
        enemyHealthBar.camera = Camera.main;  
    }

    protected virtual void Update()
    {
       // state handler scipt is called to handel enemy logic
        stateHandler.updateStateMachine();
    }

    // procedure that takes damage and undates the health bar
    public void dealDamage(int damageAmount)
    {
        // deducts the damge from enemy health and updates bar
        enemyHealth -= damageAmount;
        enemyHealthBar.setHealth(enemyHealth);

        // checks if this is the first attack if so starts chase
        if (!hasPlayerInitiatedAttack)
        {
            hasPlayerInitiatedAttack = true;
            currentState = enemyState.chase;
        }

        // checks if enemy heath is 0 if so, updates health bar and switches to death state
        if (enemyHealth <= 0f)
        {
            currentState = enemyState.death;
            enemyHealthBar.setHealth(0);
            stateHandler.handleDeath();
            return; // stops update
        }

        // checks if damge is more than 10% of health if so calls the stagger
        if (damageAmount >= enemyHealth * 0.1f)
        {
            stateHandler.triggerStagger();
        }
    }

    // procedure that resets enemy, called after attack state ends
    public void resetEnemyState()
    {
        iswalkPointSet = false;
        timeSinceChaseStarted = 0f;
        hasPlayerInitiatedAttack = false;
        numberOfConsecutiveSameAttack = 0;
        lastAttackType = "";
        idleEndTime = Time.time + idleTime;
        currentState = enemyState.idle;

    }

    // procedure that detects if the enmy is in the habitat
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HabitatZone"))
        {
            isEnemyInsideHabitat = true;
        }
    }

    // procedure that detcts if the enmy has left the habitat
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HabitatZone"))
        {
            isEnemyInsideHabitat = false;
            Debug.Log("enemy left habitat zone");
        }
    }



    public virtual void useLeafSlash()
    {
        Debug.LogWarning("baseEnemy.useLeafSlash() called – but this enemy hasn’t overridden it.");
    }

    public virtual void useVerdantCyclone()
    {
        Debug.LogWarning("baseEnemy.useVerdantCyclone() called – but this enemy hasn’t overridden it.");
    }


    private void OnDrawGizmosSelected()
    {
        // Close‑range attack
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeAttackRange);

        // Mid‑range attack
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, midAttackRange);

        // Sight radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyDetectionRange);

        // Patrol point (only visible while in the editor)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(walkPointPosition, 0.5f);
    }
}
