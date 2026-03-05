using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static playerStateManager;

public class combatController : MonoBehaviour
{
    // refrences
    public Animator playerAnimator; // Reference to Animator for attack animations
    public playerAnimationController animationController; // Reference to animation controller
    public combatInputHandler combatInputHandler; // Reference to combatInputHandler 
    public movementHandler movementHandler; // Reference to movement handler
    public cameraHandler cameraHandler; // Reference to cameraHandler

    public LayerMask enemyLayer; // Layer that defines what counts as an enemy
    private bool isGrounded;
    private bool isSprinting;

    // equip system related variables
    public bool isMeleeEquiped = false;
    public bool isBowEquiped = false;

    // spawn position variables
    public Transform weaponSpawnPointRight;  // holds a refrence to a object on the right player hand
    public Transform weaponSpawnPointLeft;  // holds a refrence to a object on the left player hand
    public Transform bowSpawnPoint;         // holds a refrence to a object on the left player hand where bows spawn

    // refrences to the object prefabs for combat
    [SerializeField] private GameObject meleeWeaponPrefab;  // refrence to sword
    [SerializeField] private GameObject bowWeaponPrefab; // refrence to bow
    [SerializeField] private GameObject arrowPrefab; // refrence to arrow

    [SerializeField] public float shootSpeed = 40f; // velocity of the arrow when shot

    private GameObject currentEquippedWeapon; // refers to the current item that is equipped

    // melee combat related variables
    public Collider swordCollider; // refrence to the sword collider
    public float meleeAttackCooldown = 0.6f; // attack cooldown 
    private float nextMeleeAttackTime = 0f; // holds the time when the player can attack
    private bool isAttacking = false; // holds if player is attacking 
    private string currentAttack = "None"; // holds if player has used right ot left slash



    // procedure that runs every frame
    private void Update()
    {
        // switches to correct camera continully to keep it rendering
        cameraHandler.SwitchToAimingCamera(combatInputHandler.isAiming); 

        aimShoot();
        isGrounded = movementHandler.IsGrounded();
        isSprinting = movementHandler.playerInputHandler.IsSprinting();

        // test keys to switch between items
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isMeleeEquiped = true;
            equipWeapon(meleeWeaponPrefab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isMeleeEquiped = false;
            isBowEquiped = false;
            unequipWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            isBowEquiped = true;
            equipWeapon(bowWeaponPrefab);
        }

        // checks if player can attack again and if player is not already attacking, if so calls the melee attack
        if (isMeleeEquiped == true && Input.GetMouseButtonDown(0) && Time.time >= nextMeleeAttackTime && !isAttacking)
        {
            useMeleeAttack();
        }
    }

    // procedure that does the melee attack and plays the correct swing animation
    private void useMeleeAttack()
    {
        isAttacking = true;
        nextMeleeAttackTime = Time.time + meleeAttackCooldown;

        if (currentAttack == "None" || currentAttack == "leftSwing")
        {
            currentAttack = "rightSwing";
            playerAnimator.SetTrigger("rightSwing");
        }
        else
        {
            currentAttack = "leftSwing";
            playerAnimator.SetTrigger("leftSwing");
        }

        StartCoroutine(allowToAttackAgain());
    }

    // procedure that handels the cooldown for attacks
    private IEnumerator allowToAttackAgain()
    {
        yield return new WaitForSeconds(meleeAttackCooldown);
        isAttacking = false;
    }

    // procedure that enables the sword collider for hitdetection, called from animation evebt
    public void enableMeleeCollider()
    {
        swordCollider.enabled = true;
    }

    // procedure that disables the sword collider for hitdetection, called from animation event

    public void disableSwordCollider()
    {
        swordCollider.enabled = false;
    }

    // procedure thats responsible for triggering archery animations and setting parameters 
    private void aimShoot()
    {
        if (combatInputHandler.isAiming && isGrounded == true)
        {
            playerAnimator.SetBool("isAiming", combatInputHandler.isAiming);
            playerAnimator.SetBool("isShooting", combatInputHandler.isShooting);
        }
        else 
        {
            playerAnimator.SetBool("isAiming", false);
            playerAnimator.SetBool("isShooting", false);
        }
    }

    // instantiates and fires the arrow 
    public void shootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, bowSpawnPoint.position, bowSpawnPoint.rotation);
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * shootSpeed, ForceMode.Impulse);
    }

    // handles switch between weapons by calling a equipWeaponNew to instatiate the item
    public void equipWeapon(GameObject weapon)
    {
        if (isMeleeEquiped == true)
        {
            equipWeaponNew(weapon, "right");
        }

        if (isBowEquiped == true)
        {
            equipWeaponNew(weapon, "bowPosition");
        }
    }
    //  procedure that instantiates the item in the correct position and hand 
    public void equipWeaponNew(GameObject weapon, string whichHand)
    {
        if (currentEquippedWeapon != null)
        {
            Destroy(currentEquippedWeapon);
        }

        if (whichHand == "left")
        {
            currentEquippedWeapon = Instantiate(weapon, weaponSpawnPointLeft.position, weaponSpawnPointLeft.rotation);
            currentEquippedWeapon.transform.SetParent(weaponSpawnPointLeft);

            // Resets  position and rotation to match the hand
            currentEquippedWeapon.transform.localPosition = Vector3.zero;
            currentEquippedWeapon.transform.localRotation = Quaternion.identity;
        }
        if (whichHand == "right") 
        {
            currentEquippedWeapon = Instantiate(weapon, weaponSpawnPointRight.position, weaponSpawnPointRight.rotation);
            currentEquippedWeapon.transform.SetParent(weaponSpawnPointRight);

            // Resets  position and rotation to match the hand
            currentEquippedWeapon.transform.localPosition = Vector3.zero;
            currentEquippedWeapon.transform.localRotation = Quaternion.identity;

            swordCollider = currentEquippedWeapon.GetComponentInChildren<Collider>();
            swordCollider.enabled = false;

        }
        if (whichHand == "bowPosition")
        {
            currentEquippedWeapon = Instantiate(weapon, bowSpawnPoint.position, bowSpawnPoint.rotation);
            currentEquippedWeapon.transform.SetParent(bowSpawnPoint);

            // Resets  position and rotation to match the hand
            currentEquippedWeapon.transform.localPosition = Vector3.zero;
            currentEquippedWeapon.transform.localRotation = Quaternion.identity;
        }

    }

    // destroys the item instance and sets current equipped weapon to null
    public void unequipWeapon()
    {
        if (currentEquippedWeapon != null)
        {
            Destroy(currentEquippedWeapon);
            currentEquippedWeapon = null;
        }
    }
}


