using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static enemyStateMachine;

public class verdaliaFirstBaseEnemy : baseEnemy
{

    public GameObject cyclonePrefab;            // refrence fot the cyclone prefab
    public Transform cycloneSpawnPoint;         // refrence to the spawn position
    public float cycloneForce = 10f;            // speed of the cyclone 
    private GameObject inGameCycloneInstance;  // refrence to the instantiated cycolne 
    public float cycloneCooldown = 3f;          // holds the cooldown for cyclone attack
    public float nextCycloneAvailableTime = 0f; // holds the next time cyclone attack can be used
    public Collider slashCollider;              // refrence to the slash collider
    public Animator enemyAnimator;              // reference to Animator component
    private bool hasRoared = false;             // holds if the enemy has roared (visual effect)

    protected override void Start()
    {
        base.Start();

        slashCollider.enabled = false; // diables the slash collider 
    }

    // overrident update method from the base enemy script 
    protected override void Update()
    {
        // calls all the processes in the base script update procedure and also overries by adding more processes
        base.Update();

        float currentSpeed = enemyNavMeshAgent.velocity.magnitude; // sets the current speed

        // assigns the correct animation depending on the enemy state
        if (currentState == enemyState.idle)
        {
            enemyAnimator.SetBool("isIdle", true);
            enemyAnimator.SetBool("isWalking", false);
            enemyAnimator.SetBool("isRunning", false);
        }
        else if (currentState == enemyState.patrol)
        {
            enemyAnimator.SetBool("isIdle", false);
            enemyAnimator.SetBool("isWalking", currentSpeed > 0.1f);
            enemyAnimator.SetBool("isRunning", false);
        }
        else if (currentState == enemyState.chase || currentState == enemyState.flee)
        {
            enemyAnimator.SetBool("isIdle", false);
            enemyAnimator.SetBool("isWalking", false);
            enemyAnimator.SetBool("isRunning", currentSpeed > 0.1f);
        }
        else
        {
            enemyAnimator.SetBool("isWalking", false);
            enemyAnimator.SetBool("isRunning", false);
            enemyAnimator.SetBool("isIdle", false);
        }

        // checks if the enemy has been damaged for the first time, and calls the roar animation
        if (hasPlayerInitiatedAttack && !hasRoared)
        {
            enemyAnimator.SetTrigger("roar");
            hasRoared = true;
        }

        // checks if enemy state is death, if so triggers the death animation
        if (currentState == enemyState.death)
        {
            enemyAnimator.SetTrigger("death");
        }
    }

    // procedure that triggers the slash animation
    public override void useLeafSlash()
    {
        Debug.Log("Leaf Slash triggered.");
        enemyAnimator.SetTrigger("slash");
    }

    // procedure that enables the slash collider which then does hit detection and damages the player
    // this procedure is called useing animation events
    public void enableSlashCollider()
    {
        if (slashCollider != null)
        {
            slashCollider.enabled = true;
        }
    }

    // procedure that disables the slash collider 
    // this procedure is called useing animation events
    public void disableSlashCollider()
    {
        if (slashCollider != null)
        {
            slashCollider.enabled = false;
        }
    }

    // procedure that triggers the cyclone animation 
    public override void useVerdantCyclone()
    {
        // checks if cyclone can be used,  if not exits the procedure
        if (Time.time < nextCycloneAvailableTime)
        {
            return;
        }
        enemyAnimator.SetTrigger("cyclone"); // triggers the cyclone animation
        nextCycloneAvailableTime = Time.time + cycloneCooldown; // starts the cooldown 
    }

    // procedure that spawns the cyclone prefab, called through animation events
    public void spawnCyclone()
    {
        // instantiates the prefab in the spawn position 
        inGameCycloneInstance = Instantiate(cyclonePrefab, cycloneSpawnPoint.position, cycloneSpawnPoint.rotation);
    }

    // procedure that launches the cyclone prefabb also called through animation events
    public void launchCyclone()
    { 
        Rigidbody rigidbody = inGameCycloneInstance.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            Vector3 direction = cycloneSpawnPoint.forward; // stores the forward direction of the cyclone
            rigidbody.AddForce(direction * cycloneForce, ForceMode.Impulse);// adds forward force to the cyclone
        }
    }
}
