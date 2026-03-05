using System;

[Serializable] // convert data to be used with JSON
public class userWorld
{
    public string worldName;       // holds the name of the world created
    public string playerClass;     // holds the chosen class
    public string worldFileName;   // refrence to a file that holds the world data
}
