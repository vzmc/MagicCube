using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : TextController
{
    private int defaltFontSize = 30;
    private bool isReverse = false;



    public void SetText(int maxLevel, int nowLevel, int expToNextLevel)
    {
        if (nowLevel == maxLevel)
        {
            base.SetText("Level: MAX!!" + "\nExp.to Next Level: 0");
        }
        else
        {
            base.SetText("Level: " + nowLevel + "\nExp.to Next Level: " + expToNextLevel);
        }
    }

}
