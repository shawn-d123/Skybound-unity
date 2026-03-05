using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.XR;

// script that is called when the world is initialised after being selected
public class worldInitialiser : MonoBehaviour
{
    public Transform spawnPoint; // position where player will spawn 
    public playerController playerController; // refrence to player controller script

    void Start()
    {
        // holds the path to where the data will be saved and retrived
        string filePath = Application.persistentDataPath + "/Accounts/" + currentEngagementData.chosenWorldFile;

        // reads the file data in filepath
        string json = File.ReadAllText(filePath);

        // sets a variable of type worldData to hold all retrived data
        userWorldData loadedData = JsonUtility.FromJson<userWorldData>(json);

        GameObject playerPrefab = null; // sets player prefab to null to remove any existing instance

        // checks what world class is stored and loads the corresponding prefab
        if (loadedData.playerClass == "ASSASSIN")
        {
            playerPrefab = Resources.Load<GameObject>("Assassin");
        }
        else if (loadedData.playerClass == "MAGE")
        {
            playerPrefab = Resources.Load<GameObject>("Mage New");
        }

        if (playerPrefab == null)
        {
            Debug.LogError("player prefab doesnt match the input");
            return;
        }

        playerController = FindObjectOfType<playerController>();

        // spawns the player at the spawn point location
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        // refrence to the playercontroller component
        playerController playercontroller = player.GetComponent<playerController>();

        // sets the refrences dynamically instead of doing it in the inspector, to stop have the player access all scripts
        playercontroller.healthBar = FindObjectOfType<healthBar>();
        playercontroller.staminaBar = FindObjectOfType<staminaBar>();
        playercontroller.XPBar = FindObjectOfType<XPBar>();
        playercontroller.playerLevelUI = FindObjectOfType<playerLevelUI>();

        // player world stats are set to player controller
        playercontroller.playerLevel = loadedData.playerLevel;
        playercontroller.playerMoney = loadedData.playerMoney;
        playercontroller.maxPlayerHealth = loadedData.maxPlayerHealth;
        playercontroller.maxPlayerStamina = loadedData.maxPlayerStamina;
        playercontroller.playerAttack = loadedData.playerAttack;
        playercontroller.playerClass = loadedData.playerClass;

        // the loaded player stats are now set in the inspector
        playercontroller.playerHealth = playercontroller.maxPlayerHealth;
        playercontroller.playerStamina = playercontroller.maxPlayerStamina;

        Debug.Log("player spawned");

        // set player UI stats for health bar, stamina bar, etc
        playercontroller.playerStatsInitialiser();
    }
}
