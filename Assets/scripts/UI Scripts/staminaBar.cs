using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class staminaBar : MonoBehaviour
{
    public Slider slider; // refrence to the slider component of Stamina Bar
    public TextMeshProUGUI staminaText; // refrence to the Text object (stamina text)

    // procedure that updates the stamina bar to player's current stamina
    public void setStamina(float stamina)
    {
        slider.value = stamina;

        // sets the text for player stamina when called and rounds the number
        staminaText.text = Mathf.RoundToInt(stamina) + " / " + slider.maxValue;
    }

    // procedure that updates the stamina bar to show full stamina
    public void setMaxStamina(float maxStamina)
    {
        slider.maxValue = maxStamina;
        slider.value = maxStamina;

        // resets the player stamina text to max and rounds the number
        staminaText.text = Mathf.RoundToInt(maxStamina) + " / " + maxStamina;
    }
}