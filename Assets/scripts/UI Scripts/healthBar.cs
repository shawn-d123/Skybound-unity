using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class healthBar : MonoBehaviour
{
    public Slider slider; // refrence to the slider component of healthBar
    public TextMeshProUGUI healthText; // refrence to the Text object (health text)
    public Gradient gradient; // refrence to the gradient component of the healthBar
    public Image fill; // refrence to the fill of the health bar

    // procedure that updates the health bar to player's current health
    public void setHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);

        // sets the text for player health when called and rounds the number
        healthText.text = Mathf.RoundToInt(health) + " / " + slider.maxValue; 
    }

    // procedure that updates the health bar to show full health
    public void setMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        fill.color = gradient.Evaluate(1f);

        // resets the player health text to max and rounds the number
        healthText.text = Mathf.RoundToInt(maxHealth) + " / " + maxHealth;
    }
}
