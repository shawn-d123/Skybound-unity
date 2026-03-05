using System;
using System.Collections.Generic;

// serializable is used to convert the data to JSON
[Serializable]
// user account holds the account and a rerence to all the world data
public class userAccountHolder
{
    public string username;  // holds the username for every account created
    public string password;  // holds the password for every account created
    public List<userWorld> worlds = new List<userWorld>(); // holds all the worlds that have been created
}

// world class holds the data of every world made
[Serializable]
public class World
{
    public string worldName;  // holds the name of the world
    public string playerClass; // holds the chosen player class
    public string worldFileName;   // will hold the refrence to anther file for worlds
}
