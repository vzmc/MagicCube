using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    private Text textComponet;

    private void Start()
    {
        textComponet = GetComponent<Text>();
    }

    public virtual void SetText(string str)
    {
        textComponet.text = str;
    }
}
