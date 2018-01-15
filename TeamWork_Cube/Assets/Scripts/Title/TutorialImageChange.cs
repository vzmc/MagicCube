using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialImageChange : MonoBehaviour
{
    public Text showText;

    public static int videoNum; //17/11/27 熊木
    private int beforVideoNum;

    public GameObject mouseIcon, fingerIcon; //1218 何

    public GameObject VideoPanel;
    public static bool OnVideo = false; //フェード中に表示させない

    public static bool loadTextFlag = false; //動画を再生してるか
    private bool buttonTouch = false; 
    public GameObject LoadText;
    GameObject CannotClickPanel;

    // Use this for initialization
    void Start ()
    {
        CannotClickPanel = GameObject.Find("CannotClickPanel");
        CannotClickPanel.SetActive(false);

        if (Application.platform == RuntimePlatform.Android)
        {
            mouseIcon.SetActive(false);
        }
        else
        {
            fingerIcon.SetActive(false);
        }

        LoadText.SetActive(false);

        showText.text = "左のアイコンを押せば\n説明を見れます";

        videoNum = 0;
        beforVideoNum = videoNum;

        StartCoroutine(MovieOn());
    }

    // Update is called once per frame
    void Update()
    {
        if (OnVideo == true)
        {
            VideoPanel.SetActive(true);
        }
        else
        {
            VideoPanel.SetActive(false);
        }

        if(loadTextFlag == false && buttonTouch == true) //再生されていない&ボタンが押されている
        {
            LoadText.SetActive(true);
        }
        else
        {
            LoadText.SetActive(false);
        }
    }

    /****PCバージョン操作*******************************/
    public void L_D_Click()
    {
        showText.text = "任意のブロックを\n左ボタンでドラッグすれば\n同じ面の中で動かせます";
        videoNum = 1;
        buttonTouch = true;
    }

    public void R_D_Click()
    {
        showText.text = "任意の位置で\n右ボタンでドラッグすれば\n視点を変えます";
        videoNum = 2;
        buttonTouch = true;
    }

    public void Scroll()
    {
        showText.text = "スクロールすれば\nキューブを\n拡大/縮小できます";
        videoNum = 3;
        buttonTouch = true;
    }

    /****アイテム***************************************/
    public void Bomb()
    {
        showText.text = "ボム";
        videoNum = 4;

        beforVideoNum = videoNum;
        buttonTouch = true;
    }

    public void AllColorBomb()
    {
        showText.text = "同色全消しボム";
        videoNum = 5;

        buttonTouch = true;
        beforVideoNum = videoNum;
    }
    public void Cross()
    {
        showText.text = "十字消し";
        videoNum = 6;

        //メモリ不足対策
        Resources.UnloadUnusedAssets();

        buttonTouch = true;
        beforVideoNum = videoNum;
    }

    /****Androidバージョン操作**************************/
    public void Touch()
    {
        showText.text = "任意のブロックを\nドラッグすれば\n同じ面の中で動かせます";
        videoNum = 1;

        buttonTouch = true;
        beforVideoNum = videoNum;
    }

    public void Touch_Rotate()
    {
        showText.text = "ブロック以外のところを\nスワイプすれば\n回転できます";
        videoNum = 2;

        //メモリ不足対策
        Resources.UnloadUnusedAssets();

        buttonTouch = true;
        beforVideoNum = videoNum;
    }

    IEnumerator MovieOn()
    {
        yield return new WaitForSeconds(1.0f);
        OnVideo = true;
    }
}
