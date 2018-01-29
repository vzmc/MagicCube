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

    //180119
    private float textAlpha;
    private bool flag_flash;


    private bool isInternetActive = false;

    private InternetConnect connect;


    void Awake()
    {
        connect = GetComponent<InternetConnect>();
    }

    // Use this for initialization
    void Start()
    {
        //180122　ネットワーク状態
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isInternetActive = false;
        }
        else
        {
            if (connect.connectResult)
            {
                isInternetActive = true;
            }
            else
            {
                isInternetActive = false;
            }

        }

        textAlpha = 0;

        ScoreUpdate();

        WriteText();
    }

    // Update is called once per frame
    void Update()
    {
        if (highscoreText.enabled)
        {
            FlashText(highscoreText);
        }
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
        int rndHint = UnityEngine.Random.Range(0, 5);

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
            case 3:
                rndText += "アイテムは５つまでなのでどんどん使っちゃおうね";
                break;
            case 4:
                rndText += "３コンボごとにアイテムがもらえるよ";

                break;

        }

        return rndText;
    }

    /// <summary>
    /// 1211何
    /// 個人ハイスコアをネットにアップデート
    /// </summary>
    /// <returns></returns>
    private void ScoreUpdate()
    {
        //ハイスコア登録処理が終わる前、遷移ボタンを無効化
        backButton.interactable = false;
        highscoreText.enabled = false;
        var isHighScore = false;
        compareLogText.text = "Please Wait...";

        //所持しているscore(ローカル最高得点)よりも今回のScoreの方が大きい場合
        if (GameManager.Instance.Score > PlayerPrefs.GetInt("score", 0))
        {   //ローカル得点更新
            PlayerPrefs.SetInt("score", GameManager.Instance.Score);
            PlayerPrefs.SetString("date", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm"));
            isHighScore = true;
            highscoreText.enabled = true;
        }

        //ハイスコアか
        if (isHighScore)
        {
            //ネットあり
            if (isInternetActive)
            {
                StartCoroutine(ScoreSendIterator());
            }
            else //ネットなし
            {
                compareLogText.text = "High Score Saved.";

                //ハイスコア登録処理が終わったので、遷移ボタンを有効化
                backButton.interactable = true;
            }
        }
        else
        {
            compareLogText.text = "";

            //ハイスコア登録処理が終わったので、遷移ボタンを有効化
            backButton.interactable = true;
        }

    }

    private void FlashText(Text target)
    {
        target.color = new Color(target.color.r, target.color.b, target.color.g, textAlpha);
        if (flag_flash)
        {
            textAlpha -= Time.deltaTime;
        }
        else
        {
            textAlpha += Time.deltaTime;
        }

        if (textAlpha <= 0)
        {
            textAlpha = 0;
            flag_flash = false;
        }
        else if (textAlpha >= 1) 
        {
            textAlpha = 1;
            flag_flash = true;
        }
    }

    private IEnumerator ScoreSendIterator()
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
            so["date"] = PlayerPrefs.GetString("date"); //24時間制
            yield return so.SaveAsync();
        }
        else
        {
            compareLogText.text = "Creating New Personal High Score Record...";

            //登録されていなかったので、新規としてidにUniqueIDを入れて次の更新処理に備えたデータで保存する
            var so = new SpreadSheetObject();
            so["id"] = SpreadSheetSetting.Instance.UniqueID;
            so["hiscore"] = PlayerPrefs.GetInt("score", 0);
            so["date"] = PlayerPrefs.GetString("date"); //24時間制
            yield return so.SaveAsync();
        }
        compareLogText.text = "High Score Update Complete";

        //ハイスコア登録処理が終わったので、遷移ボタンを有効化
        backButton.interactable = true;
    }
}
