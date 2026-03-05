using fireAttackVFXNameSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mageController : MonoBehaviour
{
    // references to scripts
    public playerController playerController; // reference to player controller script
    public Transform magicBallStartPoint; // spawn point of the magicball 
    public GameObject magicBallPrefab; // refrence to the magic ball prefab
    public GameObject mageHealAuraPrefab; // refrence to the mageHeal aura
    public GameObject mageAttackAuraPrefab; // refrence to the attack aura
    public Camera playerCamera; // reference to the player camera
    public Animator playerAnimator; // refrence to the player animator
    public healthBar healthBar; // refrence to the health bar 

    // Heal related ability variables
    public bool isHealing = false;
    public float healCoolDownTime = 10f; // cooldown for heal
    private float nextHealUseTime = 0f; // stores the next time heal is available

    // Ultimate realted ability variables
    public bool isUltimateUnlocked = false;
    public float ultimateCoolDownTime = 45f; // magicball cooldown
    private float nextUltimateUseTime = 0f; // next available ultimate attack time
    public float magicballSpeed = 20f; // magicball speed
    private GameObject inSceneMagicBall; // refrence to the magic ball after it is instantiated

    // Update is called once per frame
    void Update()
    {
        // checks if player level is 10 to allow ultimate
        if (playerController.playerLevel >= 10)
        {
            isUltimateUnlocked = true;
        }

        // checks if Q is entered to heal
        if (Input.GetKeyDown(KeyCode.Q) && playerController.playerHealth < playerController.maxPlayerHealth)
        {
            isHealing = true;
        }

        // checks if player can use heal, if so calls heal ability
        if (Time.time > nextHealUseTime && isHealing == true && playerController.playerHealth < playerController.maxPlayerHealth)
        {
            useHeal(); // calls the heal ability
            nextHealUseTime = Time.time + healCoolDownTime; // sets the next time heal will be available
        }

        // checks if X was entered, if so calls the ultimate attack
        if (Input.GetKeyDown(KeyCode.X) && isUltimateUnlocked == true && Time.time > nextUltimateUseTime)
        {
            useUltimate();
        }
    }

    // procedure that heals the player, by adding 25% of max health to current health
    private void useHeal()
    {
        healthBar = FindObjectOfType<healthBar>();

        float healAmount = playerController.maxPlayerHealth * 0.25f;
        float newHealth = playerController.playerHealth + healAmount;

        // checks if player health is beyound max, if so sets it to max
        if (newHealth > playerController.maxPlayerHealth)
        {
            healthBar.setMaxHealth(playerController.maxPlayerHealth); // sets health bar to max health
            playerController.playerHealth = playerController.maxPlayerHealth; // updates health variable

            // instantiate an aura efffect around the mage
            GameObject aura = Instantiate(mageHealAuraPrefab, transform.position, Quaternion.identity, transform);
            Destroy(aura, 3f); // remove VFX 3 seconds later
        }
        else
        {
            // instantiate an aura efffect around the mage
            GameObject aura = Instantiate(mageHealAuraPrefab, transform.position, Quaternion.identity, transform);
            Destroy(aura, 3f); // remove VFX 3 seconds later
            playerController.playerHealth = newHealth;
        }

        isHealing = false;
    }

    // procedure shoots a magicball in the aiming direction
    private void useUltimate()
    {
        Debug.Log("Magic Ball cast (animation triggered)");

        // starts the spell animation
        playerAnimator.SetTrigger("CastSpell");

        // starts the cooldown
        nextUltimateUseTime = Time.time + ultimateCoolDownTime;
    }

    // spawns the magigball in the player hand, procedure is triggered by an animation event
    public void instantiateMagicball()
    {

        Vector3 spawnPosition = magicBallStartPoint.position;
        Quaternion spawnRotation = magicBallStartPoint.rotation;

        // instantiates the magicball prefab
        inSceneMagicBall = Instantiate(magicBallPrefab, spawnPosition, spawnRotation);
        inSceneMagicBall.transform.SetParent(magicBallStartPoint); // places magic ball in player hands

        fireBallScript fireball = inSceneMagicBall.GetComponent<fireBallScript>();
        if (fireball != null)
        {
            fireball.bonusDamage = playerController.playerLevel * 100;
        }
    }

    // shoots the magic ball, procedure is triggered by an animation event
    public void shootMagicball()
    {
        {
            if (inSceneMagicBall != null)
            {
                inSceneMagicBall.transform.SetParent(null); // unparent before flying

                Vector3 directionToShoot = magicBallStartPoint.forward;

                Rigidbody rigidbody = inSceneMagicBall.GetComponent<Rigidbody>();
                fireBallScript fireball = inSceneMagicBall.GetComponent<fireBallScript>(); // GET fireball script here

                if (rigidbody != null && fireball != null)
                {
                    rigidbody.velocity = directionToShoot * magicballSpeed;
                    fireball.isLaunched = true; 
                }

                Destroy(inSceneMagicBall, 5f); // remove fireball after 5 seconds
                inSceneMagicBall = null; // reset reference
            }
        }
    }
    // adds VFX during ultimate attack, also called by an animation event
    public void instantiateCastingAura()
    {
        GameObject aura = Instantiate(mageAttackAuraPrefab, transform.position, Quaternion.identity, transform); 
        Destroy(aura, 2f); // stops VFX after 2 seonds
    }
}
