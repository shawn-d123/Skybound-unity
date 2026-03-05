using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assassinController : MonoBehaviour
{
    // refrences to scripts
    public Animator animator; // reference to the animator 
    public Transform enemyTransform; // reference to the enemy's position 
    public playerController playerController; // reference to player controller script 
    public Enemy enemy; // reference to the enemy script

    // Ultimate related variables
    public bool isUltimateUnlocked = false; 
    public float ultimateCoolDownTime = 45f; // cooldown for ultimate 
    private float nextUltimateUseTime = 0f; // stores the next time ultimate is available

    // Invisibility related variables
    public bool isInvisible = false; 
    public float invisibilityDuration = 10f; // invisibility duration
    public float invisibilityCooldownTime = 10f; //invisibility cooldown
    private float nextInvisibilityUseTime = 0f; // next time invisibility is available
    public GameObject playerCharacter; // refrence to the player model

    public GameObject stealthAuraPrefab; // refrence to invisibility prefab
    private GameObject instanceOfStealthAura; // holds the in-game aura 

    // Update is called once per frame
    void Update()
    {
        // checks if player level is 10 to allow ultimate
        if (playerController.playerLevel >= 10)
        {
            isUltimateUnlocked = true;
        }

        if (Input.GetKeyDown(KeyCode.Q) && Time.time > nextInvisibilityUseTime && isInvisible == false)
        {
            StartCoroutine(useInvisibility()); // starts invisibility coroutine
            nextInvisibilityUseTime = Time.time + invisibilityCooldownTime; // sets cooldown
        }

        handleUltimateDetection();
    }

    // coroutine that handles invisibility activation and timing
    private IEnumerator useInvisibility()
    {
        // sets player tag to "Invisible" so enemies ignore the player
        gameObject.tag = "Invisible";

        isInvisible = true;

        // instantiates the aura and sets its position and rotation to that of the player
        instanceOfStealthAura = Instantiate(stealthAuraPrefab, transform);
        instanceOfStealthAura.transform.localPosition = Vector3.zero ; 
        instanceOfStealthAura.transform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(invisibilityDuration); // waits until invisibility ends
        
        gameObject.tag = "Player"; // resets player tag hence can be detected

        Destroy(instanceOfStealthAura); // stops the aura effect
        
        isInvisible = false; 

    }

    // procedure that checks if the player is behind the enemy and if ultimate is avaialable, if so calls the ultimate
    private void handleUltimateDetection()
    {
        // checks if ultimate can be used and the player has triggered it
        if (Input.GetKeyDown(KeyCode.X) && isUltimateUnlocked == true && Time.time > nextUltimateUseTime)
        {
            // holds the direction from the enemy to the player
            Vector3 dirFromEnemyToPlayer = (transform.position - enemyTransform.position).normalized;

            // holds the dot product to determine if player is behind the enemy
            float dot = Vector3.Dot(enemyTransform.forward, dirFromEnemyToPlayer);

            float backstabDotOffset = 0.25f;
            float backstabDistance = 1.5f;
            float distanceToEnemy = Vector3.Distance(transform.position, enemyTransform.position);
            // checks if the dot product is low enough to confirm player is behind
            if (dot < -1 + backstabDotOffset && distanceToEnemy < backstabDistance)
            {
                useUltimate(); // calls the ultimate
            }
            else
            {
                return; // stops the check
            }
        }
    }

    // procedure that triggers the backstab attack and deals Damage
    private void useUltimate()
    {
        Debug.Log("ultimate triggered");

        float enemyHealth = enemy.health; 
        float bonusDamage = playerController.playerLevel * 100; // scales damage as player level increases 
        float damageToDeal = (enemyHealth * 0.5f) + bonusDamage; // total backstab damage - which is 50% of enemy health and player level x 100

        animator.SetTrigger("Backstab"); 
        enemy.dealDamage(damageToDeal); // applies damage to the enemy

        nextUltimateUseTime = Time.time + ultimateCoolDownTime; // the cooldown is started
    }

}
