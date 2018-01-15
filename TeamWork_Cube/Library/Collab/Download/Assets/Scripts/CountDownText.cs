using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownText : TextController
{
    public string turnMessage;

    public override void SetText(string str)
    {
        //base.SetText("CountDown: " + str);
        if (int.Parse(str) > 0)
        {
            base.SetText("回転まで\nあと " + str + " 回");
            this.gameObject.GetComponent<Text>().color = Color.white;
        }
        else
        {
            base.SetText(turnMessage);
            this.gameObject.GetComponent<Text>().color = Color.red;
        }
    }
}
