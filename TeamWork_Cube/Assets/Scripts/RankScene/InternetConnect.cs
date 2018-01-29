﻿using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.IO;

public class InternetConnect : MonoBehaviour
{
    public bool connectResult;


    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {

                    using (TextReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
    void Awake()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            if (Debug.isDebugBuild)
                Debug.Log("Connection Not Found");

            connectResult = false;
        }
        else
        {
            //success
            if (Debug.isDebugBuild)
                Debug.Log("Connection Found");

            connectResult = true;
        }

        StartCoroutine(WaitSeconds(3f));
    }

    IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
