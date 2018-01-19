using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialImageChange : MonoBehaviour
{
    public Text showText;

    public static int videoNum; //17/11/27 熊木
    private int beforVideoNum;

    private float loadTime;
    private bool loadFlag;

    public GameObject mouseIcon, fingerIcon; //1218 何

    public GameObject VideoPanel;
    public static bool OnVideo = false; //フェード中に表示させない

    public static bool loadTextFlag = false; //動画を再生してるか
    private bool buttonTouch = false; 
    public Text LoadText;
    GameObject CannotClickPanel;

    GameObject canNotVideoButton; //動画を再生できない時に出すボタン
    private bool canNotVideo;
    private bool firstButtonClick;

    // Use this for initialization
    void Start ()
    {
        CannotClickPanel = GameObject.Find("CannotClickPanel");
        CannotClickPanel.SetActive(false);

        canNotVideoButton = GameObject.Find("CanNotVideoButton");
        canNotVideoButton.SetActive(false);

        if (Application.platform == RuntimePlatform.Android)
        {
            mouseIcon.SetActive(false);
        }
        else
        {
            fingerIcon.SetActive(false);
        }

        LoadText.enabled = false;

        showText.text = "左のアイコンを押せば\n説明を見れます";

        videoNum = 0;
        beforVideoNum = videoNum;

        StartCoroutine(MovieOn());

        canNotVideo = false;
        firstButtonClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        //動画が4秒以上ロードしても再生されない時テキスト表示
        if (loadFlag == true && MoviePlayer.mPlayer.isPrepared == false)
        {
            loadTime += Time.deltaTime;
            if (loadTime > 4f)
            {
                LoadText.text = "この端末では\n再生できません";
                canNotVideoButton.SetActive(true);
                if(firstButtonClick == false) showText.text = "右下のボタンを押してから\nもう一度アイコンを\n押してください";
            }
        }
        else
        {
            loadTime = 0;
            LoadText.text = "Now Loading...";
        }

        //ロードテキストの表示
        if (loadTextFlag == false && buttonTouch == true) //再生されていない&ボタンが押されている
        {
            LoadText.enabled = true;
        }
        else
        {
            LoadText.enabled = false;
        }

        //フェード中にパネルを表示しないようにする
        if (OnVideo == true)
        {
            VideoPanel.SetActive(true);
        }
        else
        {
            VideoPanel.SetActive(false);
            LoadText.enabled = false;
            canNotVideoButton.SetActive(false);
        }
    }

    /****PCバージョン操作*******************************/
    public void L_D_Click()
    {
        showText.text = "任意のブロックを\n左ボタンでドラッグすれば\n同じ面の中で動かせます";
        videoNum = 1;
        if (videoNum != beforVideoNum)
        {
            loadFlag = true;
        }
        beforVideoNum = videoNum;
        buttonTouch = true;
    }

    public void R_D_Click()
    {
        showText.text = "任意の位置で\n右ボタンでドラッグすれば\n視点を変えます";
        videoNum = 2;
        if (videoNum != beforVideoNum)
        {
            loadFlag = true;
        }
        beforVideoNum = videoNum;
        buttonTouch = true;
    }

    public void Scroll()
    {
        showText.text = "スクロールすれば\nキューブを\n拡大/縮小できます";
        videoNum = 3;
        if (videoNum != beforVideoNum)
        {
            loadFlag = true;
        }
        beforVideoNum = videoNum;
        buttonTouch = true;
    }

    /****アイテム***************************************/
    public void Bomb()
    {
        showText.text = "ボム";
        if (canNotVideo == false)
        {
            videoNum = 4;
            if (videoNum != beforVideoNum)
            {
                loadFlag = true;
            }
            beforVideoNum = videoNum;
            buttonTouch = true;
        }
        else if (canNotVideo == true)
        {
            Handheld.PlayFullScreenMovie("BombVideo.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
        }
    }

    public void AllColorBomb()
    {
        showText.text = "同色全消しボム";
        if (canNotVideo == false)
        {
            videoNum = 5;
            if (videoNum != beforVideoNum)
            {
                loadFlag = true;
            }
            buttonTouch = true;
            beforVideoNum = videoNum;
        }
        else if (canNotVideo == true)
        {
            Handheld.PlayFullScreenMovie("DeleteVideo.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
        }
    }
    public void Cross()
    {
        showText.text = "十字消し";
        if (canNotVideo == false)
        {
            videoNum = 6;

            //メモリ不足対策
            Resources.UnloadUnusedAssets();

            if (videoNum != beforVideoNum)
            {
                loadFlag = true;
            }
            buttonTouch = true;
            beforVideoNum = videoNum;

        }
        else if (canNotVideo == true)
        {
            Handheld.PlayFullScreenMovie("CrossVideo.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
        }
    }

    /****Androidバージョン操作**************************/
    public void Touch()
    {
        showText.text = "任意のブロックを\nドラッグすれば\n同じ面の中で動かせます";
        if (canNotVideo == false)
        {
            videoNum = 1;
            if (videoNum != beforVideoNum)
            {
                loadFlag = true;
            }
            buttonTouch = true;
            beforVideoNum = videoNum;
        }
        else if (canNotVideo == true)
        {
            Handheld.PlayFullScreenMovie("SlideVideo.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
        }
    }

    public void Touch_Rotate()
    {
        showText.text = "ブロック以外のところを\nスワイプすれば\n回転できます";
        if (canNotVideo == false)
        {
            videoNum = 2;

            //メモリ不足対策
            Resources.UnloadUnusedAssets();

            if (videoNum != beforVideoNum)
            {
                loadFlag = true;
            }
            buttonTouch = true;
            beforVideoNum = videoNum;

        }
        else if (canNotVideo == true)
        {
            Handheld.PlayFullScreenMovie("Rotation_android.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
        }
    }

    public void CanNotVideoButton()
    {
        canNotVideo = true;
        firstButtonClick = true;
    }

    IEnumerator MovieOn()
    {
        yield return new WaitForSeconds(1.0f);
        OnVideo = true;
    }
}
