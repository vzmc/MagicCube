using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchedCountText : TextController
{

    public override void SetText(string str)
    {
        base.SetText("MatchedCount: " + str);
    }
}
