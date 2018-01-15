using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{

    public static bool NotClick; //シーンの始まりでクリックしたか（オブジェクトを表示させる）
    public static bool GameSelect; //シングルプレイかチュートリアルの選択ボタンの表示するか
    public static bool backFlag; //シーン移動選択ボタンからキューブ操作に戻るか

    private bool start;

    Button Up;
    Button Down;
    Button Left;
    Button Right;

    Button Play;
    Button Tutorial;
    Button Back;
    
    GameObject text2; //Text… 「please Click」
    public static GameObject cube;

    GameObject endText;
    public static bool IsEnd; //ゲームを終了するか
    GameObject panel;　//いい方法が思いつくまで
    public static bool OnPanel; //UIをクリックできないようにするか

    // Use this for initialization
    void Start ()
    {
        Application.targetFrameRate = 30;
        GameSelect = false;
        NotClick = false;
        backFlag = false;
        start = true;

        Up = GameObject.Find("/Canvas/U_Button/").GetComponent<Button>();
        Down = GameObject.Find("/Canvas/D_Button/").GetComponent<Button>();
        Left = GameObject.Find("/Canvas/L_Button/").GetComponent<Button>();
        Right = GameObject.Find("/Canvas/R_Button/").GetComponent<Button>();

        Play = GameObject.Find("/Canvas/Play/").GetComponent<Button>();
        Tutorial = GameObject.Find("/Canvas/Tutorial/").GetComponent<Button>();
        Back = GameObject.Find("/Canvas/BackButton/").GetComponent<Button>();

        text2 = GameObject.Find("Text2");
        cube = GameObject.Find("SelectCube");

        endText = GameObject.Find("EndText");
        endText.SetActive(false);
        IsEnd = false;
        panel = GameObject.Find("Panel");
        OnPanel = true;
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        Button();
        StartGame();
        OffButton();
        
        PanelOn();

        if (IsEnd == true)
        {
            endText.SetActive(true);
        }
    }

    //タイトルのみ表示
    public void StartGame()
    {
        if (NotClick == false || IsEnd == true)
        {
            cube.SetActive(false);
            Left.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
            Right.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
            Up.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
            Down.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
        }
        if (Input.GetMouseButtonDown(0) && NotClick == false && SelectCube.Select_Sing_or_Rank == false ||
            backFlag == true)
        {
            if (backFlag != true) TitleSound.SelectSE_Play = true;
            text2.SetActive(false);
            cube.SetActive(true);

            Up.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
            Down.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
            Left.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
            Right.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);

            StartCoroutine(SelectFlag());
            StartCoroutine(ButtonSet());

            NotClick = true;
        }
    }

    //上下左右のボタンをクリック出来る様にするか決める
    private void Button()
    {
        //         　　　　↓Rotateの誤差があるため　　　　　　　　　　　　　　
        if (SelectCube.nowRotateX <= 3 && SelectCube.nowRotateX >= -3 && NotClick == true && SelectCube.Select_Sing_or_Rank == false)
        {
            Left.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
            Right.GetComponentInChildren<Image>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
            Up.interactable = true;
            Down.interactable = true;
            if (Select.frontObjName == "back") //creditで左右ボタンを選択出来ないようにする
            {
                Right.interactable = false;
                Left.interactable = false;
            }
            else
            {
                Right.interactable = true;
                Left.interactable = true;
            }
        }
        else if (SelectCube.Select_Sing_or_Rank == true) //プレイかランキングを決めているとき
        {
            if (SelectCube.stopRotateY == -90) Left.interactable = false;
            if (SelectCube.stopRotateY == 90) Right.interactable = false;
            Up.interactable = false;
            Down.interactable = false;
        }
        else //Start以外の場所
        {
            if (Select.Flag == true)
            {
                Left.interactable = false;
                Right.interactable = false;
                Up.interactable = true;
                Down.interactable = true;
            }
            else if (Select.Flag == false)
            {
                Left.interactable = false;
                Right.interactable = false;
                Up.interactable = false;
                Down.interactable = false;
            }
        }
    }

    //シーン移動ボタンの表示
    private void OffButton()
    {
        if (GameSelect == false)
        {
            Play.gameObject.SetActive(false);
            Tutorial.gameObject.SetActive(false);
            Back.gameObject.SetActive(false);
        }
        else if (GameSelect == true)
        {
            Play.gameObject.SetActive(true);
            Tutorial.gameObject.SetActive(true);
            Back.gameObject.SetActive(true);
        }

    }

    //Rayを飛ばすのを遅らせるため
    IEnumerator SelectFlag()
    {
        yield return new WaitForSeconds(0.6f);
        Select.Flag = true;
    }

    IEnumerator ButtonSet() //ボタンをクリックさせない
    {
        start = false;
        yield return new WaitForSeconds(1);
        OnPanel = false;
    }

    private void PanelOn()
    {
        if (OnPanel == true || Select.Flag == false)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
