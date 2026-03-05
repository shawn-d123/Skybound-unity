using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.IO;


public class worldSelectionScreen : MonoBehaviour
{
    public Transform worldButtonsArea; // refrence to the position of where the buttons will spawn
    public GameObject baseWorldButton; // refrence to the prefab of the button

    private void Start()
    {
        // go back if no user is logged in
        if (currentEngagementData.activeAccount == null)
        {
            SceneManager.LoadScene("Start-Up Screen");
            return;
        }

        // this loop dynamically creates buttons for each world with the world stats 
        foreach (userWorld savedWorld in currentEngagementData.activeAccount.worlds)
        {
            GameObject worldButton = Instantiate(baseWorldButton, worldButtonsArea); // instantiates the button
            worldButton.SetActive(true);

            // world data is loaded fromm default file path and stored in worldData variable 
            string filePath = Application.persistentDataPath + "/Accounts/" + savedWorld.worldFileName;
            string json = File.ReadAllText(filePath);
            userWorldData worldData = JsonUtility.FromJson<userWorldData>(json); // stores the world data 

            // here the name, player level and class of each world is retrived and assigned to the buttons
            worldButton.transform.Find("WorldNameText").GetComponent<TMP_Text>().text = savedWorld.worldName;
            worldButton.transform.Find("PlayerClassText").GetComponent<TMP_Text>().text = "" + savedWorld.playerClass;
            worldButton.transform.Find("PlayerLevelText").GetComponent<TMP_Text>().text = "" + worldData.playerLevel;

            // set what happens when the button is clicked
            Button buttonComponent = worldButton.GetComponent<Button>();
            string fileToLoad = savedWorld.worldFileName;

            // loads the data of the world button that was clicked
            buttonComponent.onClick.AddListener(() =>
            {
                currentEngagementData.chosenWorldFile = fileToLoad;
                SceneManager.LoadScene("WorldDemo 1");
            });
        }
    }
    // procedure that loads the start up screen when back is clicked
    public void OnBack()
    {
        SceneManager.LoadScene("Start-Up Screen");
    }

    // procedure that loads the world creation screen when button is clicked
    public void OnCreateNewWorld()
    {
        SceneManager.LoadScene("WorldCreationScreen");
    }
}

