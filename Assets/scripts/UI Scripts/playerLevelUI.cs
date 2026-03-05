using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class playerLevelUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    // updates the level text to show current level
    public void setLevel(int level)
    {
        levelText.text = level.ToString();
    }

}
