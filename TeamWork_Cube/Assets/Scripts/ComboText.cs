using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboText : TextController
{
    public override void SetText(string str)
    {
        base.SetText("Combo: " + str);
    }
}
