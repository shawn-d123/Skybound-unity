using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verdaliaBossPriorityHandler : MonoBehaviour
{
    // refrence to components
    public Transform player; // Reference to the player position
    public verdaliaBossEnemy bossEnemy; // Reference to the boss script

    // priority related variables
    public float closePriority = 1f;
    public float areaPriority = 1f;
    public float midPriority = 1f;
    public float longPriority = 1f;

    // cooldown related variables
    public float nextCloseTime = 0f;
    public float nextAreaTime = 0f;
    public float nextMidTime = 0f;
    public float nextLongTime = 0f;

    // time sprent in distance related variables
    public float timeInClose = 0f;
    public float timeInArea = 0f;
    public float timeInMid = 0f;
    public float timeInLong = 0f;

    // total time sprent in distance related variables
    public float totalTimeClose = 0f;
    public float totalTimeArea = 0f;
    public float totalTimeMid = 0f;
    public float totalTimeLong = 0f;

    private string lastAttackUsed = ""; // holds the last attack that was used

    void Start()
    {
        // checks if the boss enemy has the player controller component 
        if (bossEnemy.playerController == null)
        {
            GameObject playerCharacter = GameObject.FindWithTag("Player"); // assigns the player component 

            if (playerCharacter != null)
            {
                bossEnemy.playerController = playerCharacter.GetComponent<playerController>();
            }
        }
    }

    void Update()
    {
        // checks if boss is not engagedd with the player, if so doesnt run the script
        if (bossEnemy.isActiveAgainstPlayer == false)
        {
            return;
        }

        // holds the distance to the player form the enemy
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // checks if the boss enemy has the player controller component 
        if (bossEnemy.playerController == null)
        {
            GameObject playerCharacter = GameObject.FindWithTag("Player");

            if (playerCharacter != null)
            {
                bossEnemy.playerController = playerCharacter.GetComponent<playerController>();
            }
        }

        // procedure that update how long the player spends in each range
        trackTimeSpentInEnemyRanges(distanceToPlayer);

        // procedure that update the attack priorities
        CalculatePriorities(distanceToPlayer);
    }

    // procedure that track time player stays in each distance range
    void trackTimeSpentInEnemyRanges(float distance)
    {
        if (distance <= bossEnemy.closeRange)
        {
            timeInClose += Time.deltaTime;
            totalTimeClose += Time.deltaTime;

            // Reset other ranges
            timeInArea = 0f;
            timeInMid = 0f;
            timeInLong = 0f;
        }
        else if (distance <= bossEnemy.areaRange)
        {
            timeInArea += Time.deltaTime;
            totalTimeArea += Time.deltaTime;

            // Reset other ranges
            timeInClose = 0f;
            timeInMid = 0f;
            timeInLong = 0f;
        }
        else if (distance <= bossEnemy.midRange)
        {
            timeInMid += Time.deltaTime;
            totalTimeMid += Time.deltaTime;

            // Reset other ranges
            timeInClose = 0f;
            timeInArea = 0f;
            timeInLong = 0f;
        }
        else if (distance <= bossEnemy.longRange)
        {
            timeInLong += Time.deltaTime;
            totalTimeLong += Time.deltaTime;

            // Reset other ranges
            timeInClose = 0f;
            timeInArea = 0f;
            timeInMid = 0f;
        }
    }

    // procedure that calculates the attack priorities based on player behaviors
    void CalculatePriorities(float distance)
    {
        // Reset priorities
        closePriority = 1f;
        areaPriority = 1f;
        midPriority = 1f;
        longPriority = 1f;

        // checks which range player is spending time in and increase priority accordingly
        if (timeInClose > 3f)
        {
            closePriority += 1f;
        }
        if (timeInArea > 3f)
        {
            areaPriority += 1f;
        }
        if (timeInMid > 3f)
        {
            midPriority += 1f;
        }
        if (timeInLong > 3f)
        {
            longPriority += 1f;
        }

        // increase priority of attack when player spent the most total time in 
        float maxTime = Mathf.Max(totalTimeClose, totalTimeArea, totalTimeMid, totalTimeLong);

        if (maxTime == totalTimeClose)
        {
            closePriority += 1f;
        }
        else if (maxTime == totalTimeArea)
        {
            areaPriority += 1f;
        }
        else if (maxTime == totalTimeMid)
        {
            midPriority += 1f;
        }
        else if (maxTime == totalTimeLong)
        {
            longPriority += 1f;
        }

        // decrease priority if the attack is on a cooldown
        if (Time.time < nextCloseTime)
        {
            closePriority -= 2f;
        }
        if (Time.time < nextAreaTime)
        {
            areaPriority -= 2f;
        }
        if (Time.time < nextMidTime)
        {
            midPriority -= 2f;
        }
        if (Time.time < nextLongTime)
        {
            longPriority -= 2f;
        }

        // increase priority if the player is in a specific range
        if (distance <= bossEnemy.closeRange)
        {
            closePriority += 2f;
        }
        else if (distance <= bossEnemy.areaRange)
        {
            areaPriority += 2f;
        }
        else if (distance <= bossEnemy.midRange)
        {
            midPriority += 2f;
        }
        else if (distance <= bossEnemy.longRange)
        {
            longPriority += 2f;
        }

        // cheks if boss health is low compared to playe, if so increases priority for long ranged attack
        float healthRatio = bossEnemy.enemyHealth / bossEnemy.maxHealth;
        if (healthRatio <= 0.4f)
        {
            longPriority += 3f;
        }

        // checks if player has much more health, if so prioritize stronger attacks
        if (bossEnemy.playerController != null && bossEnemy.playerController.playerHealth > 0f)
        {
            float playerToBossRatio = bossEnemy.playerController.playerHealth / bossEnemy.enemyHealth;

            if (playerToBossRatio > 1.5f)
            {
                longPriority += 2f;
                midPriority += 1f;
            }
        }

        // decreases the priority of the last used attack 
        if (lastAttackUsed == "close")
        {
            closePriority -= 1f;
        }
        else if (lastAttackUsed == "area")
        {
            areaPriority -= 1f;
        }
        else if (lastAttackUsed == "mid")
        {
            midPriority -= 1f;
        }
        else if (lastAttackUsed == "long")
        {
            longPriority -= 1f;
        }
    }

    // procedure that returns the attack with the highest priority
    public string GetBestAttack()
    {
        float highestPriority = Mathf.Max(closePriority, areaPriority, midPriority, longPriority);

        if (highestPriority == closePriority)
        {
            return "close";
        }
        else if (highestPriority == areaPriority)
        {
            return "area";
        }
        else if (highestPriority == midPriority)
        {
            return "mid";
        }
        else
        {
            return "long";
        }
    }

    // sets the cooldown for each attack
    public void setAttackCooldowns(string attackType)
    {
        lastAttackUsed = attackType;

        if (attackType == "close")
        {
            nextCloseTime = Time.time + bossEnemy.closeCooldown;
        }
        else if (attackType == "area")
        {
            nextAreaTime = Time.time + bossEnemy.areaCooldown;
        }
        else if (attackType == "mid")
        {
            nextMidTime = Time.time + bossEnemy.midCooldown;
        }
        else if (attackType == "long")
        {
            nextLongTime = Time.time + bossEnemy.longCooldown;
        }

        bossEnemy.startGlobalCooldown();
    }

    // procedure that checks if the player is in range to perform a the attack
    public bool isPlayerInRangeForAttack(string attackType)
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (attackType == "close")
        {
            return distance <= bossEnemy.closeRange;
        }
        else if (attackType == "area")
        {
            return distance <= bossEnemy.areaRange;
        }
        else if (attackType == "mid")
        {
            return distance <= bossEnemy.midRange;
        }
        else if (attackType == "long")
        {
            return distance <= bossEnemy.longRange;
        }

        return false;
    }

    // procedure that finds and executes the attack with highest priorty 
    public void findAndExecuteBestAttack()
    {
        string chosenAttack = GetBestAttack();

        if (bossEnemy.isGlobalCooldownOver() == false)
        {
            return;
        }

        if (isPlayerInRangeForAttack(chosenAttack))
        {
            executeAttack(chosenAttack);
        }
        else
        {
            bossEnemy.isRepositioning = true;
            bossEnemy.repositionStartTime = Time.time;

            Debug.Log("Repositioning for " + chosenAttack + " attack.");

            if (chosenAttack == "close")
            {
                bossEnemy.bossAgent.SetDestination(player.position + (-bossEnemy.transform.forward * 2f));
            }
            else if (chosenAttack == "long")
            {
                bossEnemy.bossAgent.SetDestination(player.position + (bossEnemy.transform.forward * 8f));
            }
            else
            {
                bossEnemy.bossAgent.SetDestination(player.position);
            }
        }
    }

    // Check what attack is finished with theit cooldown 
    public bool isAttackOffCooldown(string attackType)
    {
        if (attackType == "close")
        {
            return Time.time >= nextCloseTime;
        }
        else if (attackType == "area")
        {
            return Time.time >= nextAreaTime;
        }
        else if (attackType == "mid")
        {
            return Time.time >= nextMidTime;
        }
        else if (attackType == "long")
        {
            return Time.time >= nextLongTime;
        }

        return false;
    }

    // procedure that calls the attacks from the bossenemy script
    private void executeAttack(string attackType)
    {
        if (attackType == "close")
        {
            bossEnemy.useBite();
        }
        else if (attackType == "area")
        {
            bossEnemy.useShockwaveBlast();
        }
        else if (attackType == "mid")
        {
            bossEnemy.useHellFire();
        }
        else if (attackType == "long")
        {
            bossEnemy.useMeteorCrash();
        }
    }

    // procedure that resets all the prorities, when the enemy disengages
    public void ResetPriorities()
    {
        closePriority = 1f;
        areaPriority = 1f;
        midPriority = 1f;
        longPriority = 1f;

        timeInClose = 0f;
        timeInArea = 0f;
        timeInMid = 0f;
        timeInLong = 0f;

        totalTimeClose = 0f;
        totalTimeArea = 0f;
        totalTimeMid = 0f;
        totalTimeLong = 0f;

        nextCloseTime = 0f;
        nextAreaTime = 0f;
        nextMidTime = 0f;
        nextLongTime = 0f;

        lastAttackUsed = "";
    }
}