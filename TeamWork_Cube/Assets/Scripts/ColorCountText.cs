using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorCountText : MonoBehaviour
{
    private Text textComponet;
    private string[] colornames;
    private string showtext;

    private void Start()
    {
        textComponet = GetComponent<Text>();
        InitColornameArray();
        BuildText();
    }

    private void InitColornameArray()
    {
        colornames = new string[]
        {
            "Red",
            "Green",
            "Blue",
            "Yellow",
            "Purple",
            "Cyan",
            "Orange",
            "Grey"
        };
    }

    public void BuildText()
    {
        showtext = "";
        //1117一時無効
        //for (int i = 0; i < GameManager.Instance.ColorCount.Length; i++)
        //{
        //    AddColorText(i, GameManager.Instance.ColorCount[i]);
        //}
        textComponet.text = showtext;
    }

    private void AddColorText(int colorIndex, int colorCount)
    {
        string str = colornames[colorIndex] + ": " + colorCount;
        showtext += GetColorTagString(str, colorIndex) + "\n";
    }

    private string GetColorTagString(string str, int colorIndex)
    {
        return "<color=" + colornames[colorIndex] + ">" + str + "</color>";
    }
}
