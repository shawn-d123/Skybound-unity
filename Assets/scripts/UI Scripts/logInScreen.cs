using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class logInScreen : MonoBehaviour
{
    public userAccountHandler userAccountHandler; // refrence to the account handler script

    public TMP_InputField Username; // refrence to the username input 
    public TMP_InputField Password; // refrence to the password input

    public GameObject errorPopUp; // refrence to the error pop up game object

    // procedure that is callled when Next is clicked, here validation is carried
    public void OnNext()
    {
        string username = Username.text;
        string password = Password.text;

        // log in validation for inputs is handled here
        if (logInValidator(username, password) == true)
        {
            // checks if the account already exists 
            userAccountHolder enteredAccount = userAccountHandler.loadUserAccountData(username);

            // does a presence check, if true then error is sent out
            if (enteredAccount == null)
            {
                errorPopUp.SetActive(true);  // enables the error popup
                return;
            }

            if (enteredAccount.password != password)
            {
                errorPopUp.SetActive(true);  // enables the error popup
                return;
            }
            else // next scene is loaded 
            {
                currentEngagementData.activeAccount = enteredAccount;
                errorPopUp.SetActive(false);
                Debug.Log("Valid Inputs");
                SceneManager.LoadScene("WorldSelectionScreen");
            }
        }
        else
        {
            errorPopUp.SetActive(true); // if log in fails the error is poped up
        }
    }

    // procedure called when back is clicked, loads the start up screen
    public void OnBack()
    {
        SceneManager.LoadScene("Start-Up Screen");
    }

    // procedure thar validates the user inputs
    public bool logInValidator(string username, string password)
    {
        // checks if username has been entered
        if (username == "")
        {
            return false;
        }

        // check if password has been entered 
        if (password == "")
        {
            return false;
        }

        else
        {
            return true; 
        }

    }
}

