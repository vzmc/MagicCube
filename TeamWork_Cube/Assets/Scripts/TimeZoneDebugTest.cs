using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeZoneDebugTest : MonoBehaviour {
    Text text;
    TimeZoneInfo jpTimeZone;
    TimeZoneInfo waTimeZone;
    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();
        //text.text = TimeZoneInfo.ConvertTime(dt,TimeZoneInfo.Utc, TimeZoneInfo.Local).ToString("yyyy/MM/dd HH:mm");
        jpTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        waTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Australia Standard Time");

        string t = TimeZoneInfo.Local.Id;
        Debug.Log(t);
    }

    // Update is called once per frame
    void Update () {
        
        text.text = DateTime.UtcNow + " (" + TimeZoneInfo.Utc.ToString() + ")" + ",\n" +
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jpTimeZone) + " (" + jpTimeZone.ToString() + "),\n" +
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, waTimeZone) + " (" + waTimeZone.ToString() + ")";
    }
}
