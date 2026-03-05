using System;

[Serializable] // convert data to be used with JSON
public class userWorldData
{
    public string worldName;
    public string playerClass;
    public int playerLevel;
    public int playerMoney;        
    public float maxPlayerHealth;    
    public float maxPlayerStamina;   
    public float playerAttack;
}