using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResolutionButton : MonoBehaviour
{
    private static string NowResoution;/* デフォルトの解像度を入れる */ //解像度は前回のプレイ時のが引き継がれる？
    Text ResoText;

    void Start()
    {
        ResoText = GameObject.Find("ResolutioText/NowResolutionText").GetComponent<Text>();
        NowResoution = Screen.width.ToString() + "×" + Screen.height.ToString();
    }

    public void Resolution_H()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        Screen.SetResolution(1600, 900, Screen.fullScreen);
        //NowResoution = "1600 × 900";
#else
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
        //NowResoution = "1920 × 1080";
#endif
    }

    public void Resolution_M()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        Screen.SetResolution(1280, 720, Screen.fullScreen);
        //NowResoution = "1280 × 720";
#else
        Screen.SetResolution(1600, 900, Screen.fullScreen);
        //NowResoution = "1600 × 900";
#endif
    }

    public void Resolution_L()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        Screen.SetResolution(704, 480, Screen.fullScreen);
        //全てのAndroidでビデオをプレイできる解析度
#else
        Screen.SetResolution(1280, 720, Screen.fullScreen);
        //NowResoution = "1280 × 720";
#endif
    }

    void Update()
    {
        ResoText.text = Screen.width.ToString() + "×" + Screen.height.ToString();
    }
}
