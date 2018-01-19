using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowTurnText : TextController
{
    public override void SetText(string str)
    {
        base.SetText("Current Turn: " + str);
    }

    /// <summary>
    /// 残りターン数を表示
    /// </summary>
    /// <param name="nowTurn"></param>
    /// <param name="nowMaxTurn"></param>
    public void SetTextWithLastTurn(int nowTurn, int nowMaxTurn)
    {
        //base.SetText("Current Turn: " + nowTurn + " / " + nowMaxTurn + "\n" +
        //             "Remaining Turns: " + (nowMaxTurn - nowTurn + 1));
        base.SetText("Turns\nLeft " + (nowMaxTurn - nowTurn + 1).ToString("00"));


    }
}
