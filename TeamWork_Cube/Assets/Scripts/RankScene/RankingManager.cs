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

    private bool isPlayerTop5 = false;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadRanking());
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// ネット上のランク資料を読み取って表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadRanking()
    {
        //スコア表示処理が終わる前、リセットボタンを無効化
        resetButton.interactable = false;

        logText.text = "now loading";

        //まずTop5の取得
        var topRankQuery = new SpreadSheetQuery();

        //ハイスコアを降順（大きい順）にして、Limit5にすることで、TOP5を取得
        yield return topRankQuery.OrderByDescending("hiscore").Limit(5).FindAsync();

        //if (topRankQuery.Count == 0) Debug.Log("Oh no! There's nothing there!!!");

        //取得できたデータ表示
        var dispRank = 0;
        logText.text = "";
        foreach (var so in topRankQuery.Result)
        {
            string dtString = so["date"] as string;
            if (dtString == "") continue;
            rankList[dispRank].text = (dispRank + 1).ToString();
            childScore[dispRank].text = so["hiscore"].ToString();
            //Debug.Log(so["date"]);
            DateTime dt = DateTime.Parse(dtString); // 2017/12/22 Holmes
            childDate[dispRank].text = dt.ToLocalTime().ToString("yyyy/MM/dd HH:mm");
            //Debug.Log(childDate.Count);
            //for (int i = 0; i < childDate.Count; i++)
            //{
            //    Debug.Log(i + ": " + childDate[i].text);
            //}

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

        //自分がTOP5じゃない時、別でスコア表示
        if (!isPlayerTop5)
        {
            //すでにスコアが登録されているかチェック
            var playerRecord = new SpreadSheetQuery();
            yield return playerRecord.Where("id", "=", SpreadSheetSetting.Instance.UniqueID).FindAsync();   //"id"を検索条件に入れることで、すでにスコアが登録されているかチェック

            //既にハイスコアは登録されている
            if (playerRecord.Count > 0)
            {
                //個人スコアの表示処理（TOP5以外の場合）
                //まずプレイヤーの順位を取得
                var playerRankingQuery = new SpreadSheetQuery();
                yield return playerRankingQuery.Where("hiscore", ">", PlayerPrefs.GetInt("score", 0)).CountAsync();  //自分よりスコアが高いプレイヤーが何人いるか

                var rank = playerRankingQuery.Count + 1;    //自分のスコアのランク取得
                
                //取得できたデータをうまく整形しつつ表示
                foreach (var so in playerRecord.Result)
                {
                    playerLogText.text = "";
                    personalRecord[0].text = rank.ToString();
                    personalRecord[1].text = so["hiscore"].ToString();
                    personalRecord[2].text = so["date"] as string; ;

                }
            }
            else
            {
                //個人スコアがない
                playerLogText.text = "No Personal Record Yet...";
            }
        }

        resetButton.interactable = true;
    }

    /// <summary>
    /// リセットボタン処理
    /// </summary>
    public void ResetPushed()
    {
        //個人スコアがない場合説明文を出す
        if(playerLogText.text == "No Personal Record Yet..." ||
           playerLogText.text == "Record Reset." ||
           playerLogText.text == "Nothing to Delete.")
        {
            playerLogText.text = "Nothing to Delete.";
        }
        else　//個人スコアがある場合
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

    public void YesButtonPushed()
    {
        StartCoroutine(DeletePersonalRecord());
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
    private IEnumerator DeletePersonalRecord()
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
            yield return StartCoroutine(LoadRanking());
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

    public void ResetJustLocalScore()
    {
        //ローカルリセット
        PlayerPrefs.SetInt("score", 0);
    }

}
