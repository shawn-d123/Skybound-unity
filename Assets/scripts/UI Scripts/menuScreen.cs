using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuScreen : MonoBehaviour
{
    public GameObject MenuScreen; // refrence to the menu panel 
    public playerController playerController; // rerence to the player controller

    // refrences to player stat text fields
    public TMP_Text levelText;
    public TMP_Text healthText;
    public TMP_Text staminaText;
    public TMP_Text moneyText;
    public TMP_Text attackText;
    public UnityEngine.UI.Slider menuXPBar; // refrence to the XP bar

    private bool isMenuOpen = false; // holds whether menu is open 

    void Start()
    {
        // since different classes can be set, this finds the component and sets it to the playercontroller
        playerController = FindObjectOfType<playerController>();
    }

    void Update()
    {
        // checks if escape is pressed to enable/disable the screen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen == false)
            {
                enableMenu();
            }
            else
            {
                shutMenu();
            }
        }

            setPlayerMenuStats();
    }

    // procedure that sets the values of the menu stats 
    public void setPlayerMenuStats()
    {
        playerController = FindObjectOfType<playerController>();
        // sets the stats to those of the ones in the playerController
        levelText.text = "" + playerController.playerLevel;
        healthText.text = "" + playerController.maxPlayerHealth;
        staminaText.text = "" + playerController.maxPlayerStamina;
        attackText.text = "" + playerController.playerAttack;
        moneyText.text = "" + playerController.playerMoney;

        // sets the XP bars max and current value for the slider
        menuXPBar.maxValue = playerController.levelBoundary;
        menuXPBar.value = playerController.playerXP;
    }

    public void OnSave()
    {
        playerController = FindObjectOfType<playerController>();
        playerController.savePlayerStats();
        Debug.Log("world data saved");
    }

    // procedure that enables the menu panel
    public void enableMenu()
    {
        MenuScreen.SetActive(true);  // displays the menu screen
        Time.timeScale = 0f;         // pauses the in-game time 
        isMenuOpen = true;
    }

    // procedure that disables the menu panel
    public void shutMenu()
    {
        MenuScreen.SetActive(false); // disables the menu screen
        Time.timeScale = 1f;         // resumes the in-game time
        isMenuOpen = false;
    }

    // procedure that quits the menu panel
    public void quitGame()
    {
        Debug.Log("game quit");
        Application.Quit();

    }
}
