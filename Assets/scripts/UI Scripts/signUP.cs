using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class signUP : MonoBehaviour
{

    public userAccountHandler userAccountHandler;

    public TMP_InputField Username; // refrence to the username
    public TMP_InputField Password; // refrence to the password

    public GameObject errorPopUp; // refence to the error screen

    // validates the username and password, if correct switches to next scene
    public void OnNext()
    {
        string username = Username.text;
        string password = Password.text;

        // checks if inputs are valid
        if (signUpValidator(username, password) == true)
        {
            // checks if account exists, if so sends alert
            if (userAccountHandler.accountExiststenceCheck(username) == true)
            {
                Debug.Log("Account already exists.");
                errorPopUp.SetActive(true);  // enables the error popup
                return;
            }
            else
            {
                // creates the account, by calling createAccount
                userAccountHandler.createUserAccount(username, password);
                Debug.Log("Valid Inputs, account created");
                errorPopUp.SetActive(false);
                SceneManager.LoadScene("WorldSelectionScreen"); // switches to next scene 
            }
        }
        else // enables the error popup
        {
            errorPopUp.SetActive(true);
        }

    }

    // switches scene back to start up screen
    public void OnBack()
    {
        SceneManager.LoadScene("Start-Up Screen");
    }


    public bool signUpValidator(string username, string password)
    {
        // checks if username has been entered
        if (username == "")
        {
            return false;
        }

        // checks if password has been entered 
        if (password == "")
        {
            return false;
        }

        // Checks if username length is between 6 and 8
        if (username.Length < 6 || username.Length > 8)
        {
            return false;
        }

        // Check if password length is between 6 and 8
        if (password.Length < 6 || password.Length > 8)
        {
            return false;
        }

        else 
        {
            return true;
        }

    }
}
