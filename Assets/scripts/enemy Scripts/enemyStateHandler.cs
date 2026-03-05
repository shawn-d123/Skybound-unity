using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static enemyStateMachine;

public class enemyStateHandler
{
    private baseEnemy baseEnemy;            // reference to the baseEnemy script
    public playerController playerController;   // reference to the player controller script

    private int enemyXP = 1000;     // XP rewarded to the player when enemy is defeated

    // constructor method that sets the reference to the base enemy script
    public enemyStateHandler(baseEnemy reference)
    {
        baseEnemy = reference;
    }

    // procedure that sets the enemies initial state
    public void setInitialState(enemyState state)
    {
        baseEnemy.currentState = state; 

        // checks if the enemy state is idle, if so sets the idel end time 
        if (state == enemyState.idle)
        {
            baseEnemy.idleEndTime = Time.time + baseEnemy.idleTime;
        }
    }

    // procedure that constantly updates the enemy state depending on the game context 
    public void updateStateMachine()
    {
        // checks if enemy is dead or staggered, if so this procedure is stoped
        if (baseEnemy.currentState == enemyState.death || baseEnemy.isTakingDamage)
        {
            return; // exits the procedure
        }

        // holds the distance to the player from the enemy
        float distanceToPlayer = Vector3.Distance(baseEnemy.transform.position, baseEnemy.playerPosition.position);

        // checks if enemy is outside the habitat, if so, resets its variables and sets a walkpoit to the spawn
        if (!baseEnemy.isEnemyInsideHabitat)
        {
            baseEnemy.resetEnemyState(); // resets enemy interaction variables
            baseEnemy.enemyNavMeshAgent.SetDestination(baseEnemy.spawnPoint); // returns the enemy back to the habitat
            baseEnemy.enemyNavMeshAgent.speed = baseEnemy.patrolSpeed;
            return;
        }

        // handles the switching of states
        switch (baseEnemy.currentState)
        {
            case enemyState.idle:
                if (Time.time > baseEnemy.idleEndTime)
                {
                    baseEnemy.currentState = enemyState.patrol;
                }
                break;

            case enemyState.patrol:
                handlePatrol(); 
                break;

            case enemyState.chase:
                handleChase(distanceToPlayer);
                break;

            case enemyState.attack:
                handleAttack(distanceToPlayer); 
                break;

            case enemyState.flee:
                handleFlee(); 
                break;
        }
    }

    // procedure to handles patrol state
    private void handlePatrol()
    {
        // checks if a walkpoint is set, if not class the procedure to set the walkpoint
        if (!baseEnemy.iswalkPointSet)
        {
            setWalkPoint(); // procedure that sets walk point
            if (!baseEnemy.iswalkPointSet)
            {
                return; 
            }
        }

        // useses the navmesh component to move the enemy towards the walkpoint, at patrol speed
        baseEnemy.enemyNavMeshAgent.speed = baseEnemy.patrolSpeed;
        baseEnemy.enemyNavMeshAgent.SetDestination(baseEnemy.walkPointPosition);

        // holds the distance to the walkpoint
        float distanceToWalkPoint = Vector3.Distance(baseEnemy.transform.position, baseEnemy.walkPointPosition);

        // check if enemy has reached the patrol point, if so switch to idel state
        if (distanceToWalkPoint < 0.5f)
        {
            baseEnemy.iswalkPointSet = false; // resets the walkpoint flag
            baseEnemy.idleEndTime = Time.time + baseEnemy.idleTime;
            baseEnemy.currentState = enemyState.idle;
        }
    }

    // procedure that sets a walkpoint, by calculating a random x and z coordinate and checking is the point is tag ground
    private void setWalkPoint()
    {
        baseEnemy.iswalkPointSet = false; // reset flag

        // try up to 20 random positions to find a valid walk point
        for (int i = 0; i < 20; i++)
        {
            float randomX = Random.Range(-baseEnemy.walkPointRange, baseEnemy.walkPointRange);
            float randomZ = Random.Range(-baseEnemy.walkPointRange, baseEnemy.walkPointRange);

            Vector3 randomPosition = baseEnemy.spawnPoint + new Vector3(randomX, 0f, randomZ);

            // raycast is shot to detect if thre is a barrior or the layer is not ground
            if (Physics.Raycast(randomPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("ground")))
            {
                // check if the point is still inside the habitat zone
                Collider[] colliders = Physics.OverlapSphere(hit.point, 0.1f);
                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("HabitatZone"))
                    {
                        baseEnemy.walkPointPosition = hit.point;
                        baseEnemy.iswalkPointSet = true;
                        return; 
                    }
                }
            }
        }
    }

    // procedure to handles enmy chase state
    private void handleChase(float distanceToPlayer)
    {
        // uses the navmesh agent component to move the enemy towards the player at chase speed
        baseEnemy.enemyNavMeshAgent.speed = baseEnemy.chaseSpeed;
        baseEnemy.enemyNavMeshAgent.SetDestination(baseEnemy.playerPosition.position);

        // updates the timeSinceChaseStarted 
        baseEnemy.timeSinceChaseStarted += Time.deltaTime;

        // checks if the enemy leaves the habitat or chases too long, if so stops chase and resets the enemy
        if (!baseEnemy.isEnemyInsideHabitat || baseEnemy.timeSinceChaseStarted > baseEnemy.maximumChaseTime)
        {
            baseEnemy.resetEnemyState();
            baseEnemy.enemyNavMeshAgent.SetDestination(baseEnemy.spawnPoint);
            return;
        }

        // checks if player is attack range, if so switches to attack state
        if (distanceToPlayer > baseEnemy.closeAttackRange && distanceToPlayer <= baseEnemy.midAttackRange)
        {
            bool isCycloneAttackAvailable = false;
            // checks if cyclone attack can be used 
            if (Time.time >= ((verdaliaFirstBaseEnemy)baseEnemy).nextCycloneAvailableTime)
            {
                isCycloneAttackAvailable = true;
            }

            bool isglobalCooldownOver = false;
            // checks if global cooldown between attacks is over
            if (Time.time >= baseEnemy.globalCooldownEndTime)
            {
                isCycloneAttackAvailable = true;
            }

            // switches to attack state if both statements are true
            if (isCycloneAttackAvailable && isglobalCooldownOver)
            {
                baseEnemy.currentState = enemyState.attack;
            }
        }

        // checks if player is in close range, switch to attack
        if (distanceToPlayer <= baseEnemy.closeAttackRange)
        {
            baseEnemy.currentState = enemyState.attack;
        }
    }

    // procedure to handles the enmy attack state 
    private void handleAttack(float distanceToPlayer)
    {
        // stops the enemy movement and makes the enemy face the player
        baseEnemy.enemyNavMeshAgent.SetDestination(baseEnemy.transform.position);
        baseEnemy.transform.LookAt(baseEnemy.playerPosition);

        // checks if the enemy has attack, if so exitst the procedure
        if (baseEnemy.hasEnemyAttacked == true)
        {
            return;
        }

        bool isCycloneAttackAvailable = false;
        // checks if cyclone attack can be used 
        if (Time.time >= ((verdaliaFirstBaseEnemy)baseEnemy).nextCycloneAvailableTime)
        {
            isCycloneAttackAvailable = true;
        }

        bool isglobalCooldownOver = false;
        // checks if global cooldown between attacks is over
        if (Time.time >= baseEnemy.globalCooldownEndTime)
        {
            isCycloneAttackAvailable = true;
        }

        // checks if the player is in close range, if so calls the slash attack procedure
        if (distanceToPlayer <= baseEnemy.closeAttackRange)
        {
            baseEnemy.lastAttackType = "closeRange";
            baseEnemy.numberOfConsecutiveSameAttack++;
            baseEnemy.useLeafSlash();
            baseEnemy.globalCooldownEndTime = Time.time + 2f;
            baseEnemy.hasEnemyAttacked = true;
            baseEnemy.StartCoroutine(resetAttack());
            return;
        }

        // checks if the player is in mid range, if so calls the cycone attack procedure
        if (distanceToPlayer <= baseEnemy.midAttackRange && isCycloneAttackAvailable && isglobalCooldownOver)
        {
            baseEnemy.lastAttackType = "midRange";
            baseEnemy.numberOfConsecutiveSameAttack++;
            baseEnemy.useVerdantCyclone();
            baseEnemy.hasEnemyAttacked = true;
            baseEnemy.StartCoroutine(resetAttack());
            return;
        }

        // if we couldn't attack, go back to chasing
        baseEnemy.currentState = enemyState.chase;
    }

    // coroutine that handels the attack cooldown 
    private IEnumerator resetAttack()
    {
        yield return new WaitForSeconds(baseEnemy.timeBetweenAttacks);
        baseEnemy.hasEnemyAttacked = false;
    }

    // procedure that handles fleeing
    private void handleFlee()
    {
        // holds the direction and destination to flee in
        Vector3 fleeDirection = (baseEnemy.transform.position - baseEnemy.playerPosition.position).normalized;
        Vector3 fleeDestination = baseEnemy.transform.position + fleeDirection * 6f;

        // uses the navmesh agent component to move the enemy away from player at flee speed
        baseEnemy.enemyNavMeshAgent.speed = baseEnemy.fleeSpeed;
        baseEnemy.enemyNavMeshAgent.SetDestination(fleeDestination);

        // if enemy leaves habitat while fleeing, reset
        if (!baseEnemy.isEnemyInsideHabitat)
        {
            baseEnemy.resetEnemyState();
        }
    }

    // procedure that makes the enemy stagger
    public void triggerStagger()
    {
        baseEnemy.isTakingDamage = true;
        baseEnemy.enemyNavMeshAgent.isStopped = true;
        baseEnemy.StartCoroutine(staggerRoutine());
    }

    // coroutine that handels the stagger if 1.5 seconds
    private IEnumerator staggerRoutine()
    {
        yield return new WaitForSeconds(baseEnemy.staggerTime);
        baseEnemy.isTakingDamage = false;
        baseEnemy.enemyNavMeshAgent.isStopped = false;
        baseEnemy.currentState = enemyState.chase;
    }

    // procedure that destroys the enemy after death and adds the XP
    public void handleDeath()
    {
        // find the player object and adds the XP 
        playerController = GameObject.FindWithTag("Player")?.GetComponent<playerController>();
        if (playerController != null)
        {
            playerController.increaseXP(enemyXP);
        }

        // destroy the enemy object after 2 seconds
        GameObject.Destroy(baseEnemy.gameObject, 2f);
    }
}