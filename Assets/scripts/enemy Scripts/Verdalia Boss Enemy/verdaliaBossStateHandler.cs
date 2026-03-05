using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static enemyStateMachine;
using UnityEngine.AI;
using UnityEditor.SearchService;

public class verdaliaBossStateHandler : MonoBehaviour
{
    // refrencces to components
    private verdaliaBossEnemy boss; // refrence to the boss enemy script

    // initialises the boss enemy
    public void Initialize(verdaliaBossEnemy reference)
    {
        boss = reference;
    }

    // procedure that sets the boss inital state
    public void setInitialState(bossState state)
    {
        boss.currentState = state;

        if (state == bossState.idle)
        {
            boss.idleEndTime = Time.time + boss.idleDuration;
        }
    }

    public void updateStateMachine()
    {
        // checks if the boss is taking damage, ie; is staggered, exit the stateMachine 
        if (boss.isTakingDamage)
        {
            return;
        }

        // checks if the boss is dead after revivial, exit the statemachine
        if (boss.enemyHealth <= 0f && boss.hasBossRevived)
        {
            return;
        }

        // holds the distance between the boss and the player
        float distanceToPlayer = Vector3.Distance(boss.transform.position, boss.playerTransform.position);

        // checks if boss leaves habitat zone, if so reset and walk back to spawn
        if (boss.isBossInsideHabitat == false)
        {
            boss.resetBossState();
            boss.bossAgent.SetDestination(boss.spawnPoint); // destination is set
            boss.bossAgent.speed = boss.patrolSpeed; // boess moves to destination inside the habitat
            return;
        }

        // Handles current state controls
        if (boss.currentState == bossState.idle)
        {
            handleIdle();
        }
        else if (boss.currentState == bossState.patrol)
        {
            handlePatrol();
        }
        else if (boss.currentState == bossState.stalk)
        {
            handleStalk(distanceToPlayer);
        }
        else if (boss.currentState == bossState.chase)
        {
            handleChase(distanceToPlayer);
        }
        else if (boss.currentState == bossState.attack)
        {
            handleAttack(distanceToPlayer);
        }

        // checks if the player is close enough, and boss is not already stalking or attacking
        if (distanceToPlayer <= boss.playerDetectionRange &&
            boss.currentState != bossState.stalk &&
            boss.currentState != bossState.attack)
        {
            boss.isActiveAgainstPlayer = true;
            boss.currentState = bossState.stalk; // stalk state activated
            boss.bossAgent.SetDestination(boss.transform.position);
            boss.transform.LookAt(boss.playerTransform); // faces the player
            boss.stalkEndTime = Time.time + boss.stalkDuration; // stalk timer started
        }
    }

    // procedure that controls the idle state
    private void handleIdle()
    {
        if (Time.time >= boss.idleEndTime)
        {
            boss.currentState = bossState.patrol;
            boss.patrolPointSet = false;
        }
    }
    
    // procedure that handles the patrol state 
    private void handlePatrol()
    {
        // checks if patrol point is not yet set, if so pick a new point
        if (boss.patrolPointSet == false)
        {
            setPatrolPoint();

            if (boss.patrolPointSet == false)
            {
                return;
            }
        }

        // Moves towards the patrol point, at patrol speed
        boss.bossAgent.speed = boss.patrolSpeed;
        boss.bossAgent.SetDestination(boss.patrolPoint);

        // checks if boss reached patrol point, if so returns to idle state
        if (Vector3.Distance(boss.transform.position, boss.patrolPoint) < 1f)
        {
            boss.patrolPointSet = false;
            boss.idleEndTime = Time.time + boss.idleDuration;
            boss.currentState = bossState.idle;
        }
    }

    // prcedure that decides on patrol points
    private void setPatrolPoint()
    {
        boss.patrolPointSet = false;

        // loop 20 times till a valid point is found 
        for (int i = 0; i < 20; i++)
        {
            // here the enemy randomly choses an x and y coordinate
            float randomX = Random.Range(-boss.patrolRange, boss.patrolRange);
            float randomZ = Random.Range(-boss.patrolRange, boss.patrolRange);

            Vector3 candidatePoint = boss.spawnPoint + new Vector3(randomX, 0f, randomZ);

            // Raycast is sent to that point to check if it is the ground 
            RaycastHit hit;
            if (Physics.Raycast(candidatePoint + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("ground")))
            {
                Collider[] colliders = Physics.OverlapSphere(hit.point, 0.1f);

                // Check if the ground is inside a HabitatZone,  if so sets the patrol point
                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("HabitatZone"))
                    {
                        boss.patrolPoint = hit.point;
                        boss.patrolPointSet = true;
                        return;
                    }
                }
            }
        }
    }

    // procedure that handels the stalk state 
    private void handleStalk(float distanceToPlayer)
    {
        // enemy doesnt move and faces the player 
        boss.bossAgent.SetDestination(boss.transform.position); 
        boss.transform.LookAt(boss.playerTransform);
        boss.isActiveAgainstPlayer = true;

        // checks if stalk time has ended, if so attacks again
        if (Time.time >= boss.stalkEndTime)
        {
            if (distanceToPlayer <= boss.longRange)
            {
                if (boss.isGlobalCooldownOver())
                {
                    boss.currentState = bossState.attack; // switches to attack state
                }
            }
            else
            {
                boss.currentState = bossState.chase; // switches to chase state
            }
        }
    }

    // procedure that handls the chase state
    private void handleChase(float distanceToPlayer)
    {
        // moves the enemy towards the player at chase speed
        boss.bossAgent.speed = boss.chaseSpeed;
        boss.bossAgent.SetDestination(boss.playerTransform.position);

        // checks if player is in attack range, if so switches to sttack state
        if (distanceToPlayer <= boss.longRange)
        {
            if (boss.isGlobalCooldownOver())
            {
                boss.currentState = bossState.attack;
            }
        }
    }

    // procedure that handels the attack state
    private void handleAttack(float distanceToPlayer)
    {
        // best attack, with highst priority is chosen 
        string choseBestAttack = boss.priorityHandler.GetBestAttack();

        // holds if the enemy can use the attack
        bool globalCooldownReady = boss.isGlobalCooldownOver();
        bool attackCooldownReady = boss.priorityHandler.isAttackOffCooldown(choseBestAttack);

        // checks if the enemy can use the attck is not switches to stalk state
        if (!globalCooldownReady || !attackCooldownReady)
        {
            boss.currentState = bossState.stalk;
            return;
        }

        // Check if boss needs to reposition to attack
        float neededDistanceToAttack = getRangeForAttack(choseBestAttack);

        if (distanceToPlayer > neededDistanceToAttack)
        {
            if (boss.isRepositioning == false)
            {
                boss.isRepositioning = true;
                boss.repositionStartTime = Time.time;

                // claculates the position to move to to reposition
                Vector3 direction = (boss.transform.position - boss.playerTransform.position).normalized;
                Vector3 positionToReposition = boss.playerTransform.position + direction * neededDistanceToAttack;

                // moves the enemy to wards the destnation at chase speed
                boss.bossAgent.SetDestination(positionToReposition);
                boss.bossAgent.speed = boss.chaseSpeed;
            }

            // checks if repositioning failed, if so uses the attack in which it is in the range of 
            if (Time.time - boss.repositionStartTime > boss.maxRepositionDuration)
            {
                string rangeBasedAttack = getRangeBasedAttack(distanceToPlayer); // finds the range attack to use

                if (boss.priorityHandler.isAttackOffCooldown(rangeBasedAttack))
                {
                    executeAttack(rangeBasedAttack); // uses the attack
                    boss.startGlobalCooldown();
                }
                else
                {
                    boss.currentState = bossState.stalk; // sets state to stalk
                }

                boss.isRepositioning = false; // ends repositioning 
            }

            return;
        }

        boss.isRepositioning = false;

        // checks is the attack cooldown has ended
        if (boss.priorityHandler.isAttackOffCooldown(choseBestAttack))
        {
            executeAttack(choseBestAttack); // uses the highest priority attack
            boss.startGlobalCooldown(); // cooldown is started
        }
        else
        {
            boss.currentState = bossState.stalk;
        }
    }

    // procedure that retuns the range for the type of attack being used
    private float getRangeForAttack(string attack)
    {
        if (attack == "close")
        {
            return boss.closeRange;
        }
        else if (attack == "area")
        {
            return boss.areaRange;
        }
        else if (attack == "mid")
        {
            return boss.midRange;
        }
        else
        {
            return boss.longRange;
        }
    }

    // procedure that gets the attack based on the range inwhich the player is in
    // used when repositioning to attack fails 
    private string getRangeBasedAttack(float distance)
    {
        if (distance <= boss.closeRange)
        {
            return "close";
        }
        else if (distance <= boss.areaRange)
        {
            return "area";
        }
        else if (distance <= boss.midRange)
        {
            return "mid";
        }
        else
        {
            return "long";
        }
    }

    // procedure that calls the attack procedurefor the parameter that was passed
    private void executeAttack(string attack)
    {
        if (attack == "close")
        {
            boss.useBite();
        }
        else if (attack == "area")
        {
            boss.useShockwaveBlast();
        }
        else if (attack == "mid")
        {
            boss.useHellFire();
        }
        else if (attack == "long")
        {
            boss.useMeteorCrash();
        }
    }
}