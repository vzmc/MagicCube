using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : TextController
{
    public override void SetText(string str)
    {
        base.SetText("SCORE: " + str);
    }

    public void SetScore(int score)
    {
        if (score <= 99999999)
        {
            base.SetText("SCORE: " + score.ToString().PadLeft(8, '0'));//八桁
        }
        else
        {
            //桁越え防止
            score = 99999999;//9千9百9十9万9千9百9十9点
        }
    }
}
