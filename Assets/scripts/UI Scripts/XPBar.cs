using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class XPBar : MonoBehaviour
{
    public Slider slider; // refrence to the slider component of Xp Bar
    public TextMeshProUGUI XPtext; // refrence to the Text object (XP text)

    // Procedure that updates the XP bar and text continually
    public void setXP(int currentXP)
    {
        slider.value = currentXP;

        // Display XP as whole numbers
        XPtext.text = "XP: " + currentXP + " / " + Mathf.RoundToInt(slider.maxValue);
    }
    public void setMaxXP(int maxXP)
    {
        slider.maxValue = maxXP;
        slider.value = 0; // Start at 0 XP

        // Display XP as whole numbers
        XPtext.text = "XP: 0 / " + maxXP;
    }
}
