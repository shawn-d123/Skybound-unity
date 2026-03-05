using System.IO;
using UnityEngine;

public class userAccountHandler : MonoBehaviour
{
    private string folderPath;  // Path where we store all the account files

    void Awake()
    {
        // Set up the folder where account files will be saved
        folderPath = Application.persistentDataPath + "/Accounts";

        // Check if the directory exists, if not, create it
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
       // Debug.Log("Account folder path: " + Application.persistentDataPath + "/Accounts");

    }

    // Check if an account already exists for this username
    public bool accountExiststenceCheck(string username)
    {
        string filePath = folderPath + "/" + username + "_account.json";
        return File.Exists(filePath);  // If the file exists, return true
    }

    // Create a new account and save it to a file
    public bool createUserAccount(string username, string password)
    {
        if (accountExiststenceCheck(username) == true)  // If the account already exists, don't create it again
        {
            Debug.Log("Account already exists.");
            return false;
        }

        // Create a new user account and set up the username and password
        userAccountHolder newAccount = new userAccountHolder();
        newAccount.username = username;
        newAccount.password = password;

        // Save the new account to a file
        saveUserAccountData(newAccount);
        return true;  // Successfully created the account
    }

    // Save the account information to a JSON file
    public void saveUserAccountData(userAccountHolder account)
    {
        string filePath = folderPath + "/" + account.username + "_account.json";
        string json = JsonUtility.ToJson(account, true);  // Convert the account to JSON
        File.WriteAllText(filePath, json);  // Save the JSON data to a file
    }

    // Load an account from the file
    public userAccountHolder loadUserAccountData(string username)
    {
        string filePath = folderPath + "/" + username + "_account.json";

        if (File.Exists(filePath))  // If the file exists, load the account data
        {
            string json = File.ReadAllText(filePath);  // Read the file
            userAccountHolder loadedAccount = JsonUtility.FromJson<userAccountHolder>(json);  // Convert JSON back to an object
            return loadedAccount;
        }

        return null;  // If no file exists, return null (account not found)
    }
}
