using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum CubeColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Cyan,
    Orange,
    Grey
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private Material[] cube_Materials;
    [SerializeField]
    private MagicCube magicCube;
    [SerializeField]
    private int defaltMaxRound = 20;//デフォルト最大ターン数

    //何1110
    [SerializeField]
    private int nowMaxRound;    //現在最大ターン数
    [SerializeField]
    private int increaseRound;  //増えるターン数の量

    [SerializeField]
    NowTurnText nowTurnText;
    [SerializeField]
    CountDownText countDownText;
    [SerializeField]
    ComboText comboText;
    [SerializeField]
    ColorCountText colorCountText;

    //何1006
    [SerializeField]
    MatchedCountText matchedCountText;
    [SerializeField]
    LevelText levelText;

    //何1110
    [SerializeField]
    ScoreText scoreText;
    public int scorePerCube = 50;
    private int totalScore = 0;     //表示するスコア

    private int nowTurn;
    [SerializeField]
    private int currentRound; //（20171103　ホームズ）
    private int rotateCountDown;

    private int maxCombo;
    private int combo;

    private int[] colorCount;

    //何1006
    private int matchedCount = 0;   //今までマッチしたブロックの数
    private int levelCount = 1;
    //レベル・必要ブロック数リスト
    private Dictionary<int, int> level_exp_Dictionary = new Dictionary<int, int>();
    //↑↑↑↑1006追加分↑↑↑↑↑

    //以下はホームズによる追加（10月16日）
    public delegate void LevelUpEventHandler(int level);
    public static event LevelUpEventHandler OnLevelUp;
    //以上

    //何1016
    [SerializeField]
    private List<Material> cube_Materials_List; //今使っている色を格納するリスト
    public int ColorChangePerLevel = 3;          //色を増やすレベル
    private CubeColorChange cubeColorChange;    //キューブの色を増減するクラス
    public int MaxLevel;        //最大レベル
    public int ExpInterval;     //レベル毎上がる経験値差分
    //↑↑↑↑1016追加分↑↑↑↑↑

    //何1110
    private int expForNextLevel;


    /// <summary>
    /// 最大レベル・必要ブロック数リストを作る
    /// </summary>
    /// <param name="maxLevel">最大レベル</param>
    /// <param name="expIncrease">レベル毎に増える経験値差</param>
    /// <returns></returns>
    private Dictionary<int, int> CreateLevelDic(int maxLevel, int expIncrease)
    {
        Dictionary<int, int> levelList = new Dictionary<int, int>();
        
        for(int i = 1; i<= maxLevel; i++)
        {
            if(i == 1)
            {
                levelList.Add(i, (i - 1) * expIncrease);//[1,0]
            }
            else
            {
                levelList.Add(i, (i - 1) * expIncrease + levelList[i - 1]);//[2,10+0][3,20+10]
            }
        }


        return levelList;
    }


    /// <summary>
    /// 現在ターン数上限
    /// </summary>
    public int MaxTrun
    {
        get
        {
            return nowMaxRound;
        }
    }

    /// <summary>
    /// 現在のターン数
    /// </summary>
    public int NowTurn
    {
        get
        {
            //return nowTurn;
            return currentRound;
        }
    }

    public int RotateCountDown
    {
        get
        {
            return rotateCountDown;
        }
    }

    //[SerializeField]
    //private SelectBoxController selectBoxController;

    /// <summary>
    /// 全部のマテリアル
    /// </summary>
    public Material[] Cube_Materials
    {
        get
        {
            return cube_Materials;
        }
    }

    /// <summary>
    /// 1016何
    /// 今使っているのマテリアル
    /// </summary>
    public List<Material> Cube_Materials_List
    {
        get
        {
            return cube_Materials_List;
        }
    }
    //以上

    public int MagicCubeLevel
    {
        get
        {
            return magicCube.CubeLevel;
        }
    }

    public int Combo
    {
        get
        {
            return maxCombo;
        }
    }
    public int[] ColorCount
    {
        get
        {
            return colorCount;
        }
    }

    public int MatchedCount
    {
        get
        {
            return matchedCount;
        }
    }

    public Dictionary<int, int> Level_Exp_Dic
    {
        get
        {
            return level_exp_Dictionary;
        }
    }


    public int NowLevel
    {
        get
        {
            return levelCount;
        }
    }

    public int Score
    {
        get
        {
            return totalScore;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        colorCount = new int[cube_Materials.Length];
        //1016何
        //レベル表生成、テキスト表示
        level_exp_Dictionary = CreateLevelDic(MaxLevel, ExpInterval);

        //1113何
        //最初の六色をランダム決める
        while (cube_Materials_List.Count < 6)//六色未満の場合続く
        {
            //ランダムで色を選出
            int rndIndex = UnityEngine.Random.Range(0, cube_Materials.Length);
            //リストの中にその色があった場合
            if (cube_Materials_List.Contains(cube_Materials[rndIndex]))
            {
                continue;
            }
            //なかった場合
            else
            {
                //リストの追加
                cube_Materials_List.Add(cube_Materials[rndIndex]);
            }

        }

        //1110何
        //デフォルトの最大ターン数を渡す
        nowMaxRound = defaltMaxRound;
        //1113
        cubeColorChange = new CubeColorChange();
    }
    
    private void Start()
    {
        //TurnIncrement();
        RoundIncrement();
        TurnReload();
        SetLevelText();

    }

    public void AddColorCount(CubeColor color, int count)
    {
        colorCount[(int)color] += count;
        colorCountText.BuildText();
    }

    /// <summary>
    /// ターン数が一個増える
    /// </summary>
    public void TurnIncrement()
    {
        /*
        nowTurn++;

        int remainder = nowTurn % magicCube.RotateInterval;

        if(remainder == 0)
        {
            rotateCountDown = 0;
        }
        else
        {
            rotateCountDown = magicCube.RotateInterval - remainder;
        }

        SetNowTurnText(nowTurn.ToString());
        SetCountDownText(rotateCountDown.ToString());

        //Debug.Log("NowTurn: " + nowTurn + " | nowTurn % magicCube.RotateInterval: " + nowTurn % magicCube.RotateInterval);
        if (nowTurn > maxTurn)
        {
            ArriveMaxTurnAction();
        }
        */
        rotateCountDown--;
        SetCountDownText(rotateCountDown.ToString());
    }
    
    /// <summary>
    /// セルあっくしょん実行前のターンす―をリセットする（20171103 ホームズ）
    /// </summary>
    public void TurnReload()
    {
        rotateCountDown = magicCube.RotateInterval;
        SetCountDownText(rotateCountDown.ToString());
    }

    /// <summary>
    /// セルのアクションの実行後、ラウンド数が一戸増える（20171103　ホームズ）
    /// </summary>
    public void RoundIncrement()
    {
        currentRound++;
        SetNowTurnText(currentRound.ToString());

        if (currentRound > nowMaxRound)
        {
            ArriveMaxTurnAction();
        }
    }

    /// <summary>
    /// CountDownのUIを設定
    /// </summary>
    /// <param name="str">CountDownの数字</param>
    private void SetCountDownText(string str)
    {
        countDownText.SetText(str);
    }

    /// <summary>
    /// NowTurnのUIを設定
    /// </summary>
    /// <param name="str">NowTurnの数字</param>
    private void SetNowTurnText(string str)
    {
        //nowTurnText.SetText(str + "/" + nowMaxRound);
        nowTurnText.SetTextWithLastTurn(currentRound, nowMaxRound);
    }

    /// <summary>
    /// ComboのUIを設定
    /// </summary>
    public void SetComboText()
    {
        comboText.SetText(combo.ToString());
    }

    /// <summary>
    /// ScoreのUIを設定
    /// </summary>
    public void SetScoreText()
    {
        totalScore = matchedCount * scorePerCube;
        scoreText.SetScore(totalScore);//仮スコア
    }

    /// <summary>
    /// Combo数が一個増える
    /// </summary>
    public void ComboIncrement()
    {
        combo++;
    }

    /// <summary>
    /// Combo数をリセット
    /// </summary>
    public void ResetCombo()
    {
        if(combo.CompareTo(maxCombo)>0)
        {
            maxCombo = combo;
        }
        combo = 0;
        comboText.SetText(combo.ToString());
    }

    /// <summary>
    /// 最大ターン数に着いたら。。。
    /// </summary>
    private void ArriveMaxTurnAction()
    {
        //SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % 3); //20171103 ホームズ
        transform.GetComponent<MoveScene>().ToScene("ResultScene");//1110何
    }


    //何1006追加分
    /// <summary>
    /// 何1006
    /// MatchedCountのUIを設定
    /// </summary>
    public void SetMatchedCountText()
    {
        matchedCountText.SetText(matchedCount.ToString());
    }

    /// <summary>
    /// 今マッチしたブロックの数を加算
    /// </summary>
    /// <param name="count"></param>
    public void MatchedCountIncrement(int count)
    {
        matchedCount += count;
    }

    /// <summary>
    /// 何1006
    /// LevelTextのUIを設定
    /// </summary>
    public void SetLevelText()
    {
        int nextLevel = 0;

        foreach (var lv in level_exp_Dictionary)
        {
            if (levelCount == lv.Key)
            {
                if (lv.Key < level_exp_Dictionary.Count)
                {
                    nextLevel = level_exp_Dictionary[lv.Key + 1];
                }
                else
                {
                    nextLevel = level_exp_Dictionary[lv.Key];
                }
            }
        }

        expForNextLevel = nextLevel - matchedCount;


        levelText.SetText(levelCount.ToString(), expForNextLevel.ToString());
    }

    /// <summary>
    /// レベルアップのときの処理
    /// </summary>
    public void LeveltIncrement()
    {
        int levelBefore = levelCount; //ホームズ（10月16日）
        foreach (var lv in level_exp_Dictionary)
        {
            if (matchedCount >= lv.Value)
            {
                levelCount = lv.Key;
            }

        }

        //以下はホームズによる追加（10月16日）
        int levelUpAmount = levelCount - levelBefore;
        if (levelUpAmount > 0 && OnLevelUp != null)
        {
            OnLevelUp(levelUpAmount);
            //1110(何)レベルアップ時残りターンが設定した分増える
            nowMaxRound += increaseRound;

            //何1016追加（1113改訂）
            //レベル飛ばす回避（色増減用）
            for (int i = levelBefore; i <= levelCount; i++)
            {

                if (i % ColorChangePerLevel == 0)
                {
                    cubeColorChange.RandomColorAction();
                    break;           
                }

                if (i % ColorChangePerLevel == 1 &&
                    cubeColorChange.isColorNumChanged == true)
                {
                    cubeColorChange.isColorNumChanged = false;
                }

            }
        }
        //以上
    }
    //↑↑↑↑1006追加分↑↑↑↑↑

    //1113何
    //キューブの色を増減するを別のクラスで実行
}
