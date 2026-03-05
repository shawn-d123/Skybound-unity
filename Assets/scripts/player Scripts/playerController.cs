using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    // refrences
    public healthBar healthBar; // refrence to the health bar object 
    public staminaBar staminaBar; // refrence to the stamina bar
    public XPBar XPBar; // refrence to the XP bar
    public playerLevelUI playerLevelUI;  // refrence to the level UI


    // health related variables 
    [SerializeField] public float maxPlayerHealth = 500;   // players max health
    [SerializeField] public float playerHealth;            // players current health
    [SerializeField] public float healthRecoverySpeed = 0.5f; // speed at which the player recovers
    [SerializeField] public float recoveryDelay = 2f; // time between the moment damage was recieved and recovery starting
    private Coroutine healthRecoveryCoroutine; // refrence to the corutine 

    // stamina related variables
    [SerializeField] public float maxPlayerStamina = 50;   // players max stamina
    [SerializeField] public float playerStamina;            // players current stamina
    [SerializeField] public float staminaDrainSpeed = 10f; // speed at which the player stamina drains
    [SerializeField] public float staminaRecoverySpeed = 0.5f; // speed at which the player stamina recovers
    [SerializeField] public float staminaRecoveryDelay = 2f; // time between the moment stmain was reduced and recovery starts
    public bool isStaminaInUse;
    private Coroutine staminaRecoveryCoroutine;

    // attack power related variables
    public float playerAttack = 100f;

    // player level realted variables
    public int playerLevel = 1;
    public int playerXP = 0;
    public int levelBoundary = 1000;
    public int levelPoints = 0;
    
    // money related variables 
    public int playerMoney = 100;

    public string playerClass; // holds the class from worldData

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        // healthBar test inputs to deal damage 
        if (Input.GetKeyDown(KeyCode.K))
        {
            takeDamage(10);
        }

        // XP test input to gain XP
        if (Input.GetKeyDown(KeyCode.T))
        {
            increaseXP(5000); // Increases XP
            // debug logs for testing if xp increases
            Debug.Log("XP increased,  XP: " + playerXP + " / " + levelBoundary); 
        }

        // Test input to upgrade max health using level points
        if (Input.GetKeyDown(KeyCode.U))
        {
            increasePlayerStat("maxPlayerHealth");
        }

        // Test input to upgrade max stamina using level points
        if (Input.GetKeyDown(KeyCode.I))
        {
            increasePlayerStat("maxPlayerStamina");
        }

        // Test input to upgrade player attack using level points
        if (Input.GetKeyDown(KeyCode.L))
        {
            increasePlayerStat("PlayerAttack");
        }


        // Test key for adding money
        if (Input.GetKeyDown(KeyCode.Y))
        {
            increaseMoney(20);
        }
        // Test key for deducting money
        if (Input.GetKeyDown(KeyCode.P))
        {
            deductMoney(20);
        }

    }

    // procedure that sets player stats when called 
    public void playerStatsInitialiser()
    {
        // initialize the health bar to full
        playerHealth = maxPlayerHealth;
        healthBar.setMaxHealth(maxPlayerHealth);
        healthBar.setHealth(playerHealth);

        // initialize the stamina bar to full
        playerStamina = maxPlayerStamina;
        staminaBar.setMaxStamina(maxPlayerStamina);
        staminaBar.setStamina(playerStamina);

        // initialize the XP bar to correct amount
        XPBar.setMaxXP(levelBoundary);
        XPBar.setXP(playerXP);

        // initialises the level to the correct amount
        playerLevelUI.setLevel(playerLevel);
    }

    public void savePlayerStats()
    {
        // saves the player data 
        userWorldData data = new userWorldData();
        data.worldName = currentEngagementData.chosenWorldFile.Replace("World_", "").Replace(".json", "");
        data.playerClass = playerClass;
        data.playerLevel = playerLevel;
        data.playerMoney = playerMoney;
        data.maxPlayerHealth = maxPlayerHealth;
        data.maxPlayerStamina = maxPlayerStamina;
        data.playerAttack = playerAttack;

        // Save the data to the same world file
        string path = Application.persistentDataPath + "/Accounts/" + currentEngagementData.chosenWorldFile;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Player stats saved.");
    }

    // Procedure that increases XP when called, takes XP amount as a parameter
    public void increaseXP(int amountToIncrease)
    {
        playerXP += amountToIncrease;
        XPBar.setXP(playerXP); // updates to show the new XP level

        // Checks if player has reached the XP threshold to level up
        if (playerXP >= levelBoundary)
        {
            increaseLevel();
        }
    }

    // Procedure that increases player level when XP boundary is met
    private void increaseLevel()
    {
        playerLevel += 1; 
        levelPoints += 1; // Player earns a level point
        playerXP = 0; // Resets XP for the next level

        // Adjusts the XP required for the next level based on current level
        if (playerLevel <= 5)
        {
            levelBoundary += 1000;
        }
        else if (playerLevel <= 10)
        {
            levelBoundary += 2000;
        }
        else if (playerLevel <= 15)
        {
            levelBoundary += 3000;
        }
        else
        {
            levelBoundary += 5000;
        }
            
        XPBar.setMaxXP(levelBoundary); // Updates to new XP boundary
        XPBar.setXP(playerXP); // resets the XP after leveling up
        playerLevelUI.setLevel(playerLevel); // updates to the new player level

        Debug.Log("level increased, Current Level: " + playerLevel);
    }

    // Procedure that increases player stats using level points
    public void increasePlayerStat(string statToIncrease)
    {
        // Checks if player has available level points to spend
        if (levelPoints < 1)
        {
            Debug.Log("No Level Points Available");
            return;
        }

        levelPoints -= 1; // Deducts a level point when upgrading a stat

        // Checks which stat the player wants to increase and applies the upgrade
        if (statToIncrease == "PlayerAttack")
        {
            playerAttack += 100;
        }

        else if (statToIncrease == "maxPlayerStamina")
        {
            maxPlayerStamina += 50;
            playerStamina = maxPlayerStamina; // sets current stamina to max
            staminaBar.setMaxStamina(maxPlayerStamina); // Update to show new maxStamina
            staminaBar.setStamina(playerStamina); // Update to new playerStamina
        }

        else if (statToIncrease == "maxPlayerHealth")
        {
            maxPlayerHealth += 300;
            playerHealth = maxPlayerHealth; // sets current health to max
            healthBar.setMaxHealth(maxPlayerHealth); // Update to show new maxHealth
            healthBar.setHealth(playerHealth); // Update to new playerHealth
        }


        Debug.Log(statToIncrease + " increased"); // debug log to show the increasd stat 
    }

    // procedure that reduces the player health by taking damage as a parameter 
    public void takeDamage(float damageAmount)
    {
        // stops health going below 0 and triggers gameOver
        if ((playerHealth -= damageAmount) <= 0)
        {
            playerHealth = 0;
            gameOver();
            healthBar.setHealth(playerHealth);

        }
        else
        {
            playerHealth -= damageAmount;  // damage is dealt
            healthBar.setHealth(playerHealth); // calls a procedure that updates the healthbar

        }
        // stops the recovery when player takes damage by pausing the corutine
        if (healthRecoveryCoroutine != null)
        {
            StopCoroutine(healthRecoveryCoroutine);
        }

        healthRecoveryCoroutine = StartCoroutine(healthRecoverer());

    }
    // procedure that checks if  player health is not full, and starts the recovery
    private IEnumerator healthRecoverer()
    {
        yield return new WaitForSeconds(recoveryDelay);

        // loop that calls  a procedure that heals the player
        while (playerHealth < maxPlayerHealth)
        {
            recoverHealth(healthRecoverySpeed * Time.deltaTime);

            yield return null;
        }

        healthRecoveryCoroutine = null; // resets the coroutine 
    }

    // procedure that recovers health when called, by taking heal amount as a parameter
    public void recoverHealth(float amountToRecover)
    {
        playerHealth += amountToRecover;

        // stops playerHealth going beyound max
        if (playerHealth > maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;
        }

        healthBar.setHealth(playerHealth);
    }

    // Procedure that drains stamina when called
    public void drainStamina(float amountToDrain)
    {
        if (playerStamina > 0) // Only drain if stamina is not 0
        {
            playerStamina -= amountToDrain;

            if (playerStamina < 0) // stops stamina from going below 0
            {
                playerStamina = 0; 
            }

            staminaBar.setStamina(playerStamina); // updates to show the current stamina on bar
            isStaminaInUse = true; // set the bool to true to keep draining

            // Stop stamina recovery if stamina is reduced
            if (staminaRecoveryCoroutine != null)
            {
                StopCoroutine(staminaRecoveryCoroutine);
            }

            // Starts stamina regeneration after a delay
            staminaRecoveryCoroutine = StartCoroutine(staminaRecoverer());
        }
    }

    // Procedure that checks if stamina is not full and starts recovery
    private IEnumerator staminaRecoverer()
    {
        yield return new WaitForSeconds(staminaRecoveryDelay);

        while (playerStamina < maxPlayerStamina)
        {
            recoverStamina(staminaRecoverySpeed * Time.deltaTime);
            yield return null;
        }

        staminaRecoveryCoroutine = null;
    }

    // procedure that recovers stamina when called, by taking stamina amount as a parameter
    public void recoverStamina(float amountToRecover)
    {
        playerStamina += amountToRecover;

        if (playerStamina > maxPlayerStamina)
        {
            playerStamina = maxPlayerStamina;
        }

        staminaBar.setStamina(playerStamina);
    }

    // procedure that increases the players money when called
    public void increaseMoney(int amountToIncrease)
    {
        playerMoney += amountToIncrease;
        Debug.Log("Money increased to: " + playerMoney);
    }

    // procedure that reduces the players money when called
    public void deductMoney(int amountToDeduct)
    {
        if (playerMoney >= amountToDeduct)
        {
            playerMoney -= amountToDeduct;
            Debug.Log( "Money decreased to: "  + playerMoney);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    // procedure that is called when player health is 0, handles death
    private void gameOver()
    {
        Debug.Log("Game over");
        savePlayerStats();
        SceneManager.LoadScene("GameOverScreen");
    }
}


