using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

using GSSA;

public class RankingManager : MonoBehaviour
{
    [SerializeField]
    private Text logText;   //ロードログ
    [SerializeField]
    private Text playerLogText; //当プレイヤーに関するログ
    public Button yesButton, noButton;  //削除かどうかのボタン
    [SerializeField]
    private List<Text> rankList;//ランクテキストのリスト
    [SerializeField]
    private List<Text> childScore, childDate;   //スコアと日付テキストのリスト

    //1215何　追加
    [SerializeField]
    private List<Text> personalRecord;  //５位以下の時個人スコアを表示するテキストのリスト

    public Button resetButton, backButton;

    private string noPersonalRecord = "No Personal Record Found.";

    private bool isPlayerTop5 = false;

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

            if (Debug.isDebugBuild)
                Debug.Log(isInternetActive);

            LoadRanking_Local();
        }
        else
        {
            if (connect.connectResult)
            {
                isInternetActive = true;

                if (Debug.isDebugBuild)
                    Debug.Log(isInternetActive);

                StartCoroutine(LoadRanking_Global());
            }
            else
            {
                isInternetActive = false;

                if (Debug.isDebugBuild)
                    Debug.Log(isInternetActive);

                LoadRanking_Local();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// ネット上のランク資料を読み取って表示
    /// </summary>
    private IEnumerator LoadRanking_Global()
    {
        //スコア表示処理が終わる前、リセットボタンを無効化

        //応急処置->リセットボタンを無効化をなしに
        resetButton.interactable = false;
        backButton.interactable = false;

        logText.text = "now loading";


        //*****ローカルとグローバルのスコアを照合、アップデート*****
        //以前アップしたスコア記録があるかをチェック
        var hiScoreCheck = new SpreadSheetQuery();
        yield return hiScoreCheck.Where("id", "=", SpreadSheetSetting.Instance.UniqueID).FindAsync();   //"id"を検索条件に入れることで、すでにスコアが登録されているかチェック

        //既にハイスコアは登録されている
        if (hiScoreCheck.Count > 0)
        {
            var so = hiScoreCheck.Result.First();

            //今持つローカルスコアが大きいだったら
            if (int.Parse(so["hiscore"].ToString()) < PlayerPrefs.GetInt("score", 0))
            {
                //globalに上書き保存
                so["hiscore"] = PlayerPrefs.GetInt("score", 0);
                so["date"] = PlayerPrefs.GetString("date"); //24時間制
                yield return so.SaveAsync();
            }
            else //ローカルが小さいだったら
            {
                //localに上書き
                PlayerPrefs.SetInt("score", int.Parse(so["hiscore"].ToString()));
                PlayerPrefs.SetString("date", so["date"].ToString());
            }
        }
        else
        {
            //0以外の場合
            if (PlayerPrefs.GetInt("score", 0) != 0)
            {
                //登録されていなかったので、新規としてidにUniqueIDを入れて次の更新処理に備えたデータで保存する
                var so = new SpreadSheetObject();
                so["id"] = SpreadSheetSetting.Instance.UniqueID;
                so["hiscore"] = PlayerPrefs.GetInt("score", 0);
                so["date"] = PlayerPrefs.GetString("date"); //24時間制
                yield return so.SaveAsync();
            }
        }


        //*****TOP5を取得*****
        //まずTop5の取得
        var topRankQuery = new SpreadSheetQuery();
        //ハイスコアを降順（大きい順）にして、Limit5にすることで、TOP5を取得
        yield return topRankQuery.OrderByDescending("hiscore").Limit(5).FindAsync();

        //取得できたデータ表示
        var dispRank = 0;
        logText.text = "";
        foreach (var so in topRankQuery.Result)
        {
            string dtString = so["date"] as string;
            if (dtString == "") continue;
            rankList[dispRank].text = (dispRank + 1).ToString();
            childScore[dispRank].text = so["hiscore"].ToString();

            DateTime dt = DateTime.Parse(dtString); // 2017/12/22 Holmes
                                                    //TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
                                                    //childDate[dispRank].text = dt.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
#if(UNITY_ANDROID)
            childDate[dispRank].text = dt.AddHours(9).ToString("yyyy/MM/dd HH:mm");//強制UTC+9にする
#else
            childDate[dispRank].text = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc, TimeZoneInfo.Local).ToString("yyyy/MM/dd HH:mm");
#endif

            //自分のスコアを赤くするために、idでチェック
            if (so["id"] as string == SpreadSheetSetting.Instance.UniqueID)
            {
                isPlayerTop5 = true;
                rankList[dispRank].color = Color.red;
                childScore[dispRank].color = Color.red;
                childDate[dispRank].color = Color.red;
                playerLogText.text = "";
            }

            dispRank++;
        }


        //*****個人のスコアの順位をチェック*****
        //自分がTOP5じゃない時、別でスコア表示
        if (!isPlayerTop5)
        {
            //すでにスコアが登録されているかチェック
            var playerRecord = new SpreadSheetQuery();
            yield return playerRecord.Where("id", "=", SpreadSheetSetting.Instance.UniqueID).FindAsync();   //"id"を検索条件に入れることで、すでにスコアが登録されているかチェック

            //既に個別IDは登録されている
            if (playerRecord.Count > 0)
            {
                //IDがあっても記録日時がない場合は無視
                foreach (var player in playerRecord.Result)
                {
                    string dtString = player["date"] as string;
                    if (dtString == "")
                    {
                        playerLogText.text = noPersonalRecord;
                        resetButton.interactable = true;
                        yield break;
                    }
                }

                //個人スコアの表示処理（TOP5以外の場合）
                //まずプレイヤーの順位を取得
                var playerRankingQuery = new SpreadSheetQuery();
                yield return playerRankingQuery.Where("hiscore", ">", PlayerPrefs.GetInt("score", 0)).CountAsync();  //自分よりスコアが高いプレイヤーが何人いるか

                var rank = playerRankingQuery.Count + 1;    //自分のスコアのランク取得

                //取得できたデータをうまく整形しつつ表示
                foreach (var player in playerRecord.Result)
                {
                    string dtString = player["date"] as string;
                    if (dtString == "") continue;

                    playerLogText.text = "";
                    personalRecord[0].text = rank.ToString();
                    personalRecord[1].text = player["hiscore"].ToString();
                    DateTime dt = DateTime.Parse(dtString);
                    personalRecord[2].text = dt.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
                }
            }
            else //IDすらない
            {
                //個人スコアがないと見なす
                playerLogText.text = noPersonalRecord;

            }
        }

        backButton.interactable = true;
        resetButton.interactable = true;

    }

    /// <summary>
    /// リセットボタン処理
    /// </summary>
    public void ResetPushed()
    {
        //ネットあり
        if (isInternetActive)
        {
            //個人スコアがない場合説明文を出す
            if (playerLogText.text == noPersonalRecord ||
               playerLogText.text == "Record Reset." ||
               playerLogText.text == "No Record to Delete.")
            {
                playerLogText.text = "No Record to Delete.";
            }
            else //個人スコアがある場合
            {
                //遷移ボタンを無効化
                resetButton.interactable = false;
                backButton.interactable = false;

                //個人スコア表示消し、削除確認
                foreach (var p in personalRecord)
                {
                    p.gameObject.SetActive(false);
                }
                playerLogText.text = "Delete Your Record?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
        }
        else //ネットなし
        {
            //個人スコアがない場合説明文を出す
            if (playerLogText.text == noPersonalRecord ||
               playerLogText.text == "Record Reset." ||
               playerLogText.text == "No Record to Delete.")
            {
                playerLogText.text = "No Record to Delete.";
            }
            else //個人スコアがある場合
            {
                //遷移ボタンを無効化
                resetButton.interactable = false;
                backButton.interactable = false;

                //個人スコア表示消し、削除確認
                foreach (var p in personalRecord)
                {
                    p.gameObject.SetActive(false);
                }
                playerLogText.text = "Delete Your Record?";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
        }

    }

    public void YesButtonPushed()
    {
        if (isInternetActive)
        {
            StartCoroutine(DeletePersonalRecord_Global());
        }
        else
        {
            DeletePersonalRecord_Local();
        }
    }

    public void NoButtonPushed()
    {
        //個人スコア表示に戻る
        foreach (var p in personalRecord)
        {
            p.gameObject.SetActive(true);
        }
        playerLogText.text = "";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        resetButton.interactable = true;
        backButton.interactable = true;
    }

    /// <summary>
    /// 個人記録をリセット
    /// </summary>
    private IEnumerator DeletePersonalRecord_Global()
    {
        playerLogText.text = "Deleting...";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        //自分のスコアを取り出し
        var playerRecordOnline = new SpreadSheetQuery();
        yield return playerRecordOnline.Where("id", "=", SpreadSheetSetting.Instance.UniqueID).FindAsync();

        //オンラインリセット
        var record = playerRecordOnline.Result.First();
        record["date"] = "";
        record["hiscore"] = "";
        record["id"] = "";
        yield return record.SaveAsync();

        //ローカルリセット
        PlayerPrefs.SetInt("score", 0);
        playerLogText.text = "Record Reset.";

        //上位５名内のスコアをリセットする時ランキングをリロード
        if (isPlayerTop5)
        {
            ResetRankingText();
            StartCoroutine(LoadRanking_Global());
        }

        //ボタン復帰
        resetButton.interactable = true;
        backButton.interactable = true;
    }


    private void ResetRankingText()
    {
        foreach (var r in rankList)
        {
            r.text = "";
            r.color = Color.black;
        }

        foreach (var s in childScore)
        {
            s.text = "";
            s.color = Color.black;
        }

        foreach (var d in childDate)
        {
            d.text = "";
            d.color = Color.black;
        }
    }

    /// <summary>
    /// ネット接続なし
    /// ローカルスコア
    /// </summary>
    private void LoadRanking_Local()
    {
        //スコア表示処理が終わる前、リセットボタンを無効化
        resetButton.interactable = false;
        backButton.interactable = false;

        logText.text = "No Internet Service.\n" + "No OnlineRanking Information.";

        //スコア記録あるか
        if (PlayerPrefs.GetInt("score") == 0)
        {
            playerLogText.text = noPersonalRecord;
        }
        else
        {
            playerLogText.text = "";

            personalRecord[0].text = "LocalScore";
            personalRecord[1].text = PlayerPrefs.GetInt("score").ToString();

            if (PlayerPrefs.GetString("date") == null)
            {
                personalRecord[2].text = "No date record found.";
            }
            else
            {
                DateTime dt = DateTime.Parse(PlayerPrefs.GetString("date"));
#if(UNITY_ANDROID)
                personalRecord[2].text = dt.AddHours(9).ToString("yyyy/MM/dd HH:mm");//強制UTC+9にする
#else
                personalRecord[2].text = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc, TimeZoneInfo.Local).ToString("yyyy/MM/dd HH:mm");
#endif
            }
        }

        backButton.interactable = true;
        resetButton.interactable = true;
    }
    public void DeletePersonalRecord_Local()
    {
        //ローカルリセット
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetString("date", "");

        LoadRanking_Local();
    }

}
