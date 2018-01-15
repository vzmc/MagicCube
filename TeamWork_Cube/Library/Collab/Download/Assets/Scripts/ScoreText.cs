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
        base.SetText("SCORE: " + score.ToString().PadLeft(8, '0'));
    }
}
