using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCube : MonoBehaviour
{
    public static float nowRotateX;
    public static float stopRotateX; //Cubeの縦回転用
    public static float stopRotateY; //Cubeの横回転用
    public static bool Select_Sing_or_Rank; //シングルプレイかマルチプレイを選択中か
    public static bool multiPlay; //マルチプレイにするか(trueでマルチプレイに)

    private string ObjName; //Rayで取得したオブジェクトを受け取る

    float speed = 100f;

    public Animator fade;

     Animator anim;

    // Use this for initialization
    void Start()
    {
        //target = GameObject.Find("Cube").transform;
        stopRotateX = transform.localEulerAngles.x;
        stopRotateY = 0; //ゲームシーン等から戻った時用
        Select_Sing_or_Rank = false;

        multiPlay = false;
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        nowRotateX = transform.localEulerAngles.x;

        float step = speed * Time.deltaTime;
        //指定した方向に回転
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(stopRotateX, stopRotateY, 0), step);

        ObjName = Select.ObjName;

        NewMoveScene();

        SelectBool();

        CubeAnim();
    }

    //選択したpanelによって移動するシーンを変える
    private void NewMoveScene()
    {
        switch (ObjName)
        {
            case "front": //start
                stopRotateY = -90;
                TitleSound.RotateSE_Play = true;
                TitleManager.backFlag = false;
                break;
            case "top": //Exit
                TitleSound.SelectSE_Play = true;
                TitleManager.IsEnd = true;
                Invoke("GameEnd", 1.5f);
                break;
            case "base": //オプション
                Select.Flag = false;
                fade.SetTrigger("FadeOut");
                TitleSound.SelectSE_Play = true;
                Invoke("MoveOpt", 1f);
                break;
            case "back": //クレジット
                Select.Flag = false;
                fade.SetTrigger("FadeOut");
                TitleSound.SelectSE_Play = true;
                Invoke("MoveCre", 1f);
                break;
            case "left": //シングルプレイ
                //プレイかチュートリアル選択に
                TitleSound.SelectSE_Play = true;
                TitleManager.NotClick = false;
                TitleManager.GameSelect = true;
                TitleManager.backFlag = false;
                break;
            case "right": //ランキングシーン
                Select.Flag = false;
                fade.SetTrigger("FadeOut");
                TitleSound.SelectSE_Play = true;
                Invoke("MoveRank", 1f);
                break;
            
            case "leftcan": //シングルプレイ　キャンセル
                stopRotateY = 0;
                TitleSound.RotateSE_Play = true;
                Select_Sing_or_Rank = false;
                break;
            case "rightcan": //ランキング　キャンセル
                stopRotateY = 0;
                TitleSound.RotateSE_Play = true;
                Select_Sing_or_Rank = false;
                break;
            default:
                break;
        }
    }

    //Invokeで使用
    private void MoveCre()
    {
        SceneManager.LoadScene("Credit");
    }
    private void MoveOpt()
    {
        SceneManager.LoadScene("OptionScene");
    }
    private void MoveRank()
    {
        SceneManager.LoadScene("RankingScene");
    }
    private void GameEnd()
    {
        Application.Quit();
    }


    //回転した角度でシングル・マルチ選択中か判定
    private void SelectBool()
    {
        if (stopRotateY == -90 || stopRotateY == 90)
        {
            Select_Sing_or_Rank = true;
        }
        else
        {
            Select_Sing_or_Rank = false;
        }
    }

    private void CubeAnim()
    {
        if (Select.animFlag == false)
        {
            anim.SetTrigger("MoveTri");
            Select.animFlag = true;
        }
    }
}