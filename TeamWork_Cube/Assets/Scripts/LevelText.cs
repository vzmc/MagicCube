using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : TextController
{
    private int defaltFontSize = 30;
    private bool isReverse = false;



    public void SetText(string nowLevel, string expToNextLevel)
    {
        base.SetText("Level: " + nowLevel + "\nExp.to Next Level: " + expToNextLevel);
    }

}
