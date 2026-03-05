using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class gameOver : MonoBehaviour
{
    // refrences to all the game over screen stats text boxes
    public TMP_Text worldNameText;
    public TMP_Text playerClassText;
    public TMP_Text playerLevelText;
    public TMP_Text playerMoneyText;

    void Start()
    {
        // refers to the default unity file path
        string filePath = Application.persistentDataPath + "/Accounts/" + currentEngagementData.chosenWorldFile;

        // holds the active world name fetched from currentEngafgementData 
        string worldName = currentEngagementData.chosenWorldFile.Replace("World_", "").Replace(".json", "");

        // reads the JSON data and adds it to the loadedData variable to refrence from
        string json = File.ReadAllText(filePath);
        userWorldData loadedData = JsonUtility.FromJson<userWorldData>(json);

        // stats text box updated from the loaded data
        worldNameText.text = "" + worldName;
        playerClassText.text = "" + loadedData.playerClass;

        // stats text boxes updated from the playerController script
        playerLevelText.text = "" + loadedData.playerLevel;
        playerMoneyText.text = "" + loadedData.playerMoney;
    }

    // procedure that quits the game when called, when quit button clicked
    public void quitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }

    // procedure that resets the game by reloading the scene, called when respawn is clicked
    public void Respawn()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("WorldDemo 1");
    }

}
