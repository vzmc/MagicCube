using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectButton : MonoBehaviour
{
    private bool SceneSelectEnd;

    public Animator fade;

    private GameLoad load;

    // Use this for initialization
    void Start()
    {
        load = GetComponent<GameLoad>();

        SceneSelectEnd = false;
        TitleManager.OnPanel = true;
    }

    //それぞれのボタン処理
    public void L_Button()
    {
        Select.Flag = false;
        //マルチプレイが出来てから必要
        if (SelectCube.stopRotateY != -90)
        {
            TitleSound.RotateSE_Play = true;
            SelectCube.stopRotateY -= 90;
            TitleManager.backFlag = false;
        }
        Invoke("DontClick", 1.05f);
    }
    public void R_Button()
    {
        Select.Flag = false;
        if (SelectCube.stopRotateY != 90)
        {
            TitleSound.RotateSE_Play = true;
            SelectCube.stopRotateY += 90;
            TitleManager.backFlag = false;
        }
        Invoke("DontClick", 1.05f);
    }
    public void U_Button()
    {
        Select.Flag = false;
        SelectCube.stopRotateX -= 90;
        TitleSound.RotateSE_Play = true;
        Invoke("DontClick", 1.05f);
    }
    public void D_Button()
    {
        Select.Flag = false;
        SelectCube.stopRotateX += 90;
        TitleSound.RotateSE_Play = true;
        Invoke("DontClick", 1.05f);
    }
    public void Play_Button()
    {
        Select.Flag = false;
        TitleManager.OnPanel = true;
        SceneSelectEnd = true;
        fade.SetTrigger("FadeOut");
        TitleSound.SelectSE_Play = true;
        if (SelectCube.multiPlay == false) Invoke("GoToPlay", 1f);
        else
        {
            //マルチプレイゲームシーン用
            SceneManager.LoadScene("Title");//仮
            Debug.Log("マルチプレイのゲームシーンへ");
        }
        Invoke("DontClick", 1.05f);
    }
    public void Tutorial_Button()
    {
        Select.Flag = false;
        TitleManager.OnPanel = true;
        SceneSelectEnd = true;
        fade.SetTrigger("FadeOut");
        TitleSound.SelectSE_Play = true;
        if (SelectCube.multiPlay == false) Invoke("GoToTutorial", 1.5f);
        else
        {
            //マルチプレイチュートリアル用(シングルプレイ用と違いが少なければ同じシーンで作る？)
            SceneManager.LoadScene("Title");//仮
            Debug.Log("マルチプレイのチュートリアル選択へ");
        }
        Invoke("DontClick", 1.05f);
    }
    public void Back_Button()
    {
        Select.Flag = false;
        TitleSound.BackSE_Play = true;
        TitleManager.cube.SetActive(true);
        SelectCube.stopRotateX = 0;
        SelectCube.stopRotateY = 0;
        SelectCube.multiPlay = false;
        TitleManager.GameSelect = false;
        TitleManager.backFlag = true;
        Invoke("DontClick", 1.05f);
    }

    //Invorkで使う
    private void GoToPlay()
    {
        //SceneManager.LoadScene("PlayScene");
        load.LoadingStart();
    }
    private void GoToTutorial()
    {
        SceneManager.LoadScene("TutorialSelect");
    }
    private void DontClick()
    {
        Select.Flag = true;
        TitleManager.backFlag = false;
    }
}
