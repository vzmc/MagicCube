using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キューブの色を増減するクラス
/// </summary>
public class CubeColorChange : MonoBehaviour {

    //
    GameManager gameManager;

    public bool isColorNumChanged = false;
    private List<Material> cube_Materials_List = new List<Material>();

    // Use this for initialization
    void Awake ()
    {
        gameManager = GetComponent<GameManager>();
        cube_Materials_List = gameManager.Cube_Materials_List;
    }

    // Update is called once per frame
    void Update ()
    {
        cube_Materials_List = gameManager.Cube_Materials_List;
    }

    //何1016追加
    /// <summary>
    /// 色を増やす
    /// </summary>
    /// <param name="newMaterial"></param>
    public void IncreaseCubeColor()
    {
        //色が8色だったら増やさない
        if (cube_Materials_List.Count == 8)
        {
            //DEBUG
            Debug.Log("(もう８色あったため発動なし)");
            isColorNumChanged = true;

            return;
        }
        else
        {
            //今使われていない色を格納するリスト
            List<Material> notIncludeMaterials = new List<Material>();

            //今使っている色をチェック
            foreach (var ma in gameManager.Cube_Materials)
            {
                //もし使われていない色があれば
                if (!cube_Materials_List.Contains(ma))
                {
                    //格納
                    notIncludeMaterials.Add(ma);
                }
            }
            //使われていない色が無ければ以降なし
            if (notIncludeMaterials.Count == 0) return;

            //使われていない色の中からランダム選ぶ
            int addColorIndex = UnityEngine.Random.Range(0, notIncludeMaterials.Count);
            //生成する色を追加
            cube_Materials_List.Add(notIncludeMaterials[addColorIndex]);



            isColorNumChanged = true;
        }
    }

    /// <summary>
    /// 色を減らす1110
    /// </summary>
    public void DecreaseCubeColor()
    {
        //色が5色以下だったら減らさない
        if (cube_Materials_List.Count <= 5)
        {
            //DEBUG
            Debug.Log("(5色以下だったため発動なし)");

            isColorNumChanged = true;
            return;
        }
        else
        {
            //消す色をランダム指定
            int deleteColorIndex = UnityEngine.Random.Range(0, cube_Materials_List.Count);
            //指定した色を再度生成されないように生成マテリアルリストから消す
            cube_Materials_List.RemoveAt(deleteColorIndex);

            isColorNumChanged = true;
        }
    }

    //1110
    //色を増減しますか？
    private bool IsColorChangeActive()
    {
        bool isActive = false;

        //色の変動ができる状態なら
        if (!isColorNumChanged)
        {
            isActive = true;
        }

        return isActive;

    }

    /// <summary>
    /// 色増減の行動をランダムにする
    /// </summary>
    public void RandomColorAction()
    {
        int select = UnityEngine.Random.Range(0, 3);

        switch (select)
        {
            case 0:
                if (IsColorChangeActive())
                {
                    Debug.Log("色追加");
                    IncreaseCubeColor();
                    //isColorNumChanged = true;
                }
                break;

            case 1:
                if (IsColorChangeActive())
                {
                    Debug.Log("色減少");
                    DecreaseCubeColor();
                    //isColorNumChanged = true;
                }
                break;

            case 2:
                isColorNumChanged = true;
                //DEBUG
                Debug.Log("色増減無し");
                break;
            default:
                break;
        }
    }



    //↑↑↑↑1016追加分↑↑↑↑↑
}
