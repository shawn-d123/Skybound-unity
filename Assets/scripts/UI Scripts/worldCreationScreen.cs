using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class worldCreationScreen : MonoBehaviour
{
    public TMP_InputField worldName; // refrence to the input text box
    public TMP_Dropdown dropdownClassUI; // refrence to the drop down menu
    public GameObject errorPopUp; // refrence to the error pop up

    // procedure that triggers when create world is clicked and saves and loads the world
    public void OnCreateNewWorld()
    {
        // checks if threre is an active account
        if (currentEngagementData.activeAccount == null)
        {
            Debug.LogError("No user is logged in. You must log in first.");
            return;
        }
        else
        {
            string chosenWorldName = worldName.text; // refrence to the input value
            string chosenClass = dropdownClassUI.options[dropdownClassUI.value].text; // refrence to the slected class

            // checks if the player has enetered a world name, if not spawns pop up
            if (string.IsNullOrEmpty(chosenWorldName))
            {
                errorPopUp.SetActive(true); 
                return;
            }
            userAccountHandler userAccounthandler = FindObjectOfType<userAccountHandler>(); // refrence ot the accountHandler script


            userWorld newWorld = new userWorld();
            newWorld.worldName = chosenWorldName;
            newWorld.playerClass = chosenClass;
            newWorld.worldFileName = "World_" + chosenWorldName + ".json";

            // creates a world to the active account
            currentEngagementData.activeAccount.worlds.Add(newWorld);

            // saves the newlt created world to the account
            userAccounthandler.saveUserAccountData(currentEngagementData.activeAccount);

            // sets the world data which is displayed during selection
            userWorldData worldData = new userWorldData();
            worldData.worldName = chosenWorldName;
            worldData.playerClass = chosenClass;
            worldData.playerLevel = 1;
            worldData.maxPlayerHealth = 100;
            worldData.maxPlayerStamina = 50;
            worldData.playerAttack = 100;

            // stores the data to the correct path wich unity uses
            string path = Application.persistentDataPath + "/Accounts/" + newWorld.worldFileName;
            string json = JsonUtility.ToJson(worldData, true);
            File.WriteAllText(path, json);
            Debug.Log("World saved at: " + path);

            // switches scenes to the game scene, later it will switch to the scene that is instantiated
            SceneManager.LoadScene("WorldDemo 1");
        }

    }

    public void OnBack()
    {
        SceneManager.LoadScene("WorldSelectionScreen");
    }
}
 