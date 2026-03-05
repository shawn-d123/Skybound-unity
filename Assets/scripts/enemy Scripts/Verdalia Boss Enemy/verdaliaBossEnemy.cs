using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class verdaliaBossEnemy : MonoBehaviour
{
    // Reference to components 
    public playerController playerController; // refrene to the player contoller
    public bossEnemyHealthBar bossEnemyHealthBar; // refrence to the enemy health bar
    public GameObject biteCollider; // refrence to the collider on the enemy mouth
    public Transform playerTransform; // refrence to the player transform
    public NavMeshAgent bossAgent; // refrence to the navmesh agent
    public Animator bossAnimator; // refrence to the animator component 
    public verdaliaBossStateHandler stateHandler; // refrence tot he statehandler
    public verdaliaBossPriorityHandler priorityHandler;
    public GameObject fireBeamPrefab;
    public Transform fireBeamSpawnPoint;
    private GameObject currentFireBeam;
    public GameObject meteorPrefab;

    public bool hasBossRevived = false; // holds if the enemy health has already reached zero
    public int enemyXP = 10000; // XP award to player after enemy is defeated

    // varaibles related to enemy stats 
    public float maxHealth = 40000f;
    public float enemyHealth;
    public int bossLevel = 25;
    public float staggerDamageThreshold = 6000f;
    public bool isTakingDamage = false;

    public float playerHealth = 100f;

    public bossState currentState; // refrence to the state the boss is in


    // patrol related variables
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4f;
    public float patrolRange = 12f;
    public Vector3 spawnPoint;
    public bool patrolPointSet = false;
    public Vector3 patrolPoint;

    // idle related variables
    public float idleDuration = 3f;
    public float idleEndTime = 0f;

    // stalk state related variables
    public float playerDetectionRange = 10f;
    public float stalkDuration = 5f;
    public float stalkEndTime = 0f;

    // boss enemy rangees related variables
    public float closeRange = 4f;
    public float areaRange = 7f;
    public float midRange = 10f;
    public float longRange = 18f;

    // boss enemy coooldown related varibales
    public float closeCooldown = 3f;
    public float areaCooldown = 5f;
    public float midCooldown = 5f;
    public float longCooldown = 7f;
    public float globalCooldown = 3f;
    public float globalCooldownEndTime = 0f;

    // repositioning related variables 
    public bool isRepositioning = false;
    public float repositionStartTime = 0f;
    // max time before enemy stops respositioning and attacks
    public float maxRepositionDuration = 5f; 

    public float staggerDuration = 1.5f;
    public bool isBossInsideHabitat = true; 

    public bool isActiveAgainstPlayer = false;

    // Attack related variablees
    public float shockwaveBlastDamage = 500f;
    private Vector3 meteorTargetLandingPosition;

    void Start()
    {
        // gets the boss navmesh component
        bossAgent = GetComponent<NavMeshAgent>();

        // gets the player components
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerController = playerObject.GetComponent<playerController>();
        }
        else
        {
            Debug.LogError("Player object not found in scene. Make sure the Player has the tag 'Player'.");
        }

        // gets all the boss script components
        priorityHandler = GetComponent<verdaliaBossPriorityHandler>();
        stateHandler = GetComponent<verdaliaBossStateHandler>();

        // Initialize state handler if it exists
        stateHandler.Initialize(this);
        stateHandler.setInitialState(bossState.idle);

        // Record spawn point location
        spawnPoint = transform.position;

        // Set starting health
        enemyHealth = maxHealth;

        // Set up the boss health bar
        bossEnemyHealthBar = GetComponentInChildren<bossEnemyHealthBar>();
        if (bossEnemyHealthBar != null)
        {
            bossEnemyHealthBar.setMaxHealth(enemyHealth);
            bossEnemyHealthBar.camera = Camera.main;
        }
    }

    void Update()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerController = playerObject.GetComponent<playerController>();
            playerHealth = playerController.playerHealth;
        }

        // Ensure state handler always exists
        if (stateHandler == null)
        {
            stateHandler = GetComponent<verdaliaBossStateHandler>();
            if (stateHandler != null)
            {
                stateHandler.Initialize(this);
            }
        }

        // procedure that calls animations to be updated every second
        updateAnimations();

        stateHandler.updateStateMachine();

    }

    // bool that holds if the min time between attacks is finished
    public bool isGlobalCooldownOver()
    {
        // Check if the global cooldown has ended
        if (Time.time >= globalCooldownEndTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // procedure thatstarts the global cooldown
    public void startGlobalCooldown()
    {
        // updates the timer
        globalCooldownEndTime = Time.time + globalCooldown;

        // boss states switched to stalk
        currentState = bossState.stalk;
        stalkEndTime = Time.time + stalkDuration;
        isActiveAgainstPlayer = true;
    }

    // procedure that handles taking damage and revival of enemy
    public void takeDamage(float damageAmount)
    {
        // chcks if enemy is already taken damage or dead, if so exits procedure
        if (isTakingDamage || enemyHealth <= 0f)
        {
            return;
        }

        // deducts the damage
        enemyHealth = enemyHealth - damageAmount;

        // health bar is updated to show the damage 
        bossEnemyHealthBar.setHealth(enemyHealth);
        
        // Check if boss health is zero is so decides what to do
        if (enemyHealth <= 0f)
        {
            // checkes if the enmy has already revived, if not, revives the enemy
            if (hasBossRevived == false)
            {
                // Revive the boss at 25% of original health
                hasBossRevived = true;
                enemyHealth = maxHealth * 0.25f;
                bossEnemyHealthBar.setHealth(enemyHealth); // updates health bar
                return;
            }
            else
            {
                // Boss has already revivded and health reache zero for the second time
                currentState = bossState.idle; // stops the boss
                bossAgent.isStopped = true;
                // updates the health bar to 0
                bossEnemyHealthBar.setHealth(0f);

                //calls the death animation
                bossAnimator.SetTrigger("Death");

                playerController.increaseXP(enemyXP); // grants the player the enmey XP

                Destroy(gameObject, 5f); // destroys the enemy after 5 seconds
                return;
            }
        }

        // checks If enough damage is taken, if so  stagger the boss
        if (damageAmount >= staggerDamageThreshold)
        {
            StartCoroutine(staggerRoutine());
        }
    }

    // coroutune for the stagger delay
    private IEnumerator staggerRoutine()
    {
        isTakingDamage = true;
        bossAgent.isStopped = true;

        yield return new WaitForSeconds(staggerDuration);

        isTakingDamage = false;
        bossAgent.isStopped = false;
    }

    // procedure that resets the boss, is it leaves the habitat or dies
    public void resetBossState()
    {
        patrolPointSet = false;
        currentState = bossState.idle;
        idleEndTime = Time.time + idleDuration;
        priorityHandler.ResetPriorities();
        isActiveAgainstPlayer = false;
    }


    // boss enemy close ranged bite attack
    public void useBite()
    {
        // triggers the bite animation and the attack itself is done through animation events
        bossAnimator.SetTrigger("Bite");
        priorityHandler.setAttackCooldowns("close");

    }

    // procedure that enables the bite collider, called useing anuimation events
    public void enableBiteCollider()
    {
        biteCollider.SetActive(true); // activates the colloder
    }

    // procedure that disables the bite collider, called useing animation events
    public void disableBiteCollider()
    {
        biteCollider.SetActive(false);
    }

    // boss enemy area based attack 
    public void useShockwaveBlast()
    {
        // calls the animation 
        bossAnimator.SetTrigger("ShockwaveBlast");
        priorityHandler.setAttackCooldowns("area");
        // procedure that deals damage if player in range
        shockwaveBlastDamageCheck();
    }

    // procedure that checks if player is in range useing a sphere cast and damges the player 
    public void shockwaveBlastDamageCheck()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, areaRange);

        for (int i = 0; i < hitPlayers.Length; i++)
        {
            Collider hit = hitPlayers[i];

            if (hit.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (distance <= areaRange)
                {
                    playerController player = hit.GetComponent<playerController>();
                    if (player != null)
                    {
                        player.takeDamage(shockwaveBlastDamage);
                    }
                }
            }
        }
    }

    // boss enemy mid ranged attack
    public void useHellFire()
    {
        // triggers the attack animation
        bossAnimator.SetTrigger("HellFire");
        priorityHandler.setAttackCooldowns("mid");
    }

    // procedure that instantiates the fire beam, called useing animation events
    public void startHellFire()
    {
        if (fireBeamPrefab != null && fireBeamSpawnPoint != null)
        {
            // spawns the fire beam in the enemy mouth 
            currentFireBeam = Instantiate(fireBeamPrefab, fireBeamSpawnPoint.position, fireBeamSpawnPoint.rotation);
            currentFireBeam.transform.parent = fireBeamSpawnPoint;
        }
    }

    // procedure that ends the fire beam, called useing animation events
    public void stopHellFire()
    {
        if (currentFireBeam != null)
        {
            Destroy(currentFireBeam);
            currentFireBeam = null;
        }
    }

    // boss long ranged attack
    public void useMeteorCrash()
    {
        // triggers the attack animation
        bossAnimator.SetTrigger("MeteorCrash");
        priorityHandler.setAttackCooldowns("long");
    }

    // procedure that instantiates the meteors, called using animation events
    public void spawnMeteor()
    {
        meteorTargetLandingPosition = playerTransform.position; // target is set to player position

        // metor is spaened in the correct position
        Vector3 spawnPosition = new Vector3(meteorTargetLandingPosition.x, 0f, meteorTargetLandingPosition.z);
        Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
    }

    // procedure that detects if the enemy is in the habitat
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HabitatZone"))
        {
            isBossInsideHabitat = true;
            isActiveAgainstPlayer = false;
        }
    }

    // procedure that checks if the enemy has left the habitat, if so resets the enemy and returns it to the habitat
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HabitatZone"))
        {
            isActiveAgainstPlayer = false;
        }
    }

    // procedure that updates the animations if it is triggered 
    void updateAnimations()
    {
        float speed = bossAgent.velocity.magnitude;

        if (currentState == bossState.patrol && speed > 0.1f)
        {
            bossAnimator.SetBool("isWalking", true);
        }
        else
        {
            bossAnimator.SetBool("isWalking", false);
        }

        if (currentState == bossState.chase && speed > 0.1f)
        {
            bossAnimator.SetBool("isRunning", true);
        }
        else
        {
            bossAnimator.SetBool("isRunning", false);
        }

        if (currentState == bossState.idle || speed <= 0.1f)
        {
            bossAnimator.SetBool("isIdle", true);
        }
        else
        {
            bossAnimator.SetBool("isIdle", false);
        }
    }















    // -------------------------------
    // Debug Gizmos
    // -------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, areaRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, midRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, longRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, 0.5f);
    }
}