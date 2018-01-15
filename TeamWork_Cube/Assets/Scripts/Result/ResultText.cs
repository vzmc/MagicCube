using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

using GSSA;

public class ResultText : MonoBehaviour
{
    //ハイスコアのデータ
    //private int score, combo, level, turn;
    //private int[] color = new int[8];

    public Text highscoreText;  //NewScore文字

    public Text comboText;
    public Text turnText;
    public Text[] colorText;

    public Text scoreText;
    public Text levelText;
    public Text hintText;

    //1211何
    public Text compareLogText; //スコア更新ログ
    public Button backButton;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(ScoreSendIterator());
        WriteText();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void WriteText()
    {
        //コンボのスコア
        comboText.text = GameManager.Instance.Combo.ToString() + "コンボ";
        //色のスコア
        for (int i = 0; i < colorText.Length; i++)
        {
            colorText[i].text = GameManager.Instance.ColorCount[i].ToString();// + "個";
        }
        //最大ターン数
        turnText.text = (GameManager.Instance.NowTurn - 1).ToString() + "ターン";
        //スコア
        scoreText.text = GameManager.Instance.Score.ToString().PadLeft(8, '0');
        //レベル
        levelText.text = GameManager.Instance.NowLevel.ToString();
        //ヒント
        hintText.text = RandomHint();
    }

    /// <summary>
    /// ランダムヒント
    /// </summary>
    /// <returns>ヒント文</returns>
    private string RandomHint()
    {
        string rndText = "～HINT～\n";
        int rndHint = UnityEngine.Random.Range(0, 3);

        switch (rndHint)
        {
            case 0:
                rndText += "レベルアップしたらボーナスターンとアイテムがもらえるよ";
                break;
            case 1:
                rndText += "十字消しは端っこで発動した方が効果抜群だよ";
                break;
            case 2:
                rndText += "ブロックを移動する時方向をよく見るんだよ";
                break;
        }

        return rndText;
    }

    /// <summary>
    /// 1211何
    /// 個人ハイスコアをネットにアップデート
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScoreSendIterator()
    {
        //ハイスコア登録処理が終わる前、遷移ボタンを無効化
        backButton.interactable = false;
        var isHiscore = false;
        highscoreText.enabled = false;
        compareLogText.text = "Please Wait...";


        if (GameManager.Instance.Score > PlayerPrefs.GetInt("score", 0))    //所持しているscore(ローカル最高得点)よりも今回のScoreの方が大きい場合
        {
            PlayerPrefs.SetInt("score", GameManager.Instance.Score);    //ローカル得点更新
            isHiscore = true;
            highscoreText.enabled = true;
        }

        if (isHiscore)
        {
            //すでにスコアが登録されているかチェック
            var hiScoreCheck = new SpreadSheetQuery();
            yield return hiScoreCheck.Where("id", "=", SpreadSheetSetting.Instance.UniqueID).FindAsync();   //"id"を検索条件に入れることで、すでにスコアが登録されているかチェック

            //既にハイスコアは登録されている
            if (hiScoreCheck.Count > 0)
            {
                compareLogText.text = "Updating Personal High Score...";

                //登録されている＝hiScoreCheckの戻りリストが更新対象SpreadSheetObjectになるので、そのまま使用する
                var so = hiScoreCheck.Result.First();
                so["hiscore"] = PlayerPrefs.GetInt("score", 0);
                so["date"] = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm"); //24時間制
                yield return so.SaveAsync();
            }
            else
            {
                compareLogText.text = "Creating New Personal High Score Record...";

                //登録されていなかったので、新規としてidにUniqueIDを入れて次の更新処理に備えたデータで保存する
                var so = new SpreadSheetObject();
                so["id"] = SpreadSheetSetting.Instance.UniqueID;
                so["hiscore"] = PlayerPrefs.GetInt("score", 0);
                so["date"] = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm"); //24時間制
                yield return so.SaveAsync();
            }
            compareLogText.text = "High Score Update Complete";
        }
        else
        {
            compareLogText.text = "";
        }

        //ハイスコア登録処理が終わったので、遷移ボタンを有効化
        backButton.interactable = true;
    }
}
