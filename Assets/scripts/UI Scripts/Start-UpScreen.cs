using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class StartUPScreen : MonoBehaviour
{
    // procedure that loads the log in screen, called when button is pressed
    public void loadLoginScreen()
    {
        SceneManager.LoadScene("LoginScreen");
    }

    // procedure that loads the sign up screen, called when button is pressed
    public void loadSignupScreen()
    {
        SceneManager.LoadScene("SignUpScreen");
    }

    // procedure that quits the game, called when button is pressed
    public void quitGame()
    {
        Application.Quit(); // exits the game 
        Debug.Log("Game Quit");
    }

}
