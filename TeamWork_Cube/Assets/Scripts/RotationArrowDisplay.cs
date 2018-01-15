using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationArrowDisplay : MonoBehaviour
{
    public GameObject ArrowPrefab;
    public MagicCube targetCube;
    public GameManager gameManager;
    int cubeLevel;

    //何　1002
    private Center center;                                              // 回転中心
    public List<GameObject> ArrowPanelList = new List<GameObject>();   // パネル格納リスト

    // Use this for initialization
    void Start()
    {
        center = GameObject.Find("Center").GetComponent<Center>();
        gameObject.transform.position = center.transform.position;

        //アローを生成
        cubeLevel = targetCube.CubeLevel;

        for (int i = 0; i < cubeLevel; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float eulerRot = 90.0f * j;
                GameObject panel = Instantiate(ArrowPrefab, gameObject.transform);
                panel.transform.Rotate(new Vector3(eulerRot, 0, 0));
                panel.transform.Translate(new Vector3(0, -cubeLevel / 2.0f + i + 0.5f, cubeLevel / 2.0f + 0.25f));

                //アローリストに入れる
                ArrowPanelList.Add(panel);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //ChangeArrowColor();
    }

    /// <summary>
    /// アローを生成
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="depth"></param>
    /// <param name="rd"></param>
    public void RefreshArrow(Axis axis, int depth, RotationDirection rd)
    {

        //回転中心の角度をリセット
        gameObject.transform.rotation = center.transform.rotation;

        SetArrowPosition(axis, depth, rd);
    }

    /// <summary>
    /// アローの位置を決める
    /// </summary>
    /// <param name="axis"></param>
    private void SetArrowPosition(Axis axis, int depth, RotationDirection rd)
    {
        switch (axis)
        {
            case Axis.X:
                {
                    gameObject.transform.position = new Vector3(targetCube.rotateDepth, (cubeLevel - 1) / 2f, (cubeLevel - 1) / 2f);
                    break;
                }
            case Axis.Y:
                {
                    gameObject.transform.Rotate(new Vector3(0, 0, 90));
                    gameObject.transform.position = new Vector3((cubeLevel - 1) / 2f, targetCube.rotateDepth, (cubeLevel - 1) / 2f);
                    break;
                }
            case Axis.Z:
                {
                    gameObject.transform.Rotate(new Vector3(0, -90, 0));
                    gameObject.transform.position = new Vector3((cubeLevel - 1) / 2f, (cubeLevel - 1) / 2f, targetCube.rotateDepth);

                    break;
                }
            default:
                {
                    break;
                }
        }

        if (rd == RotationDirection.CW)
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 0));
        }
        else
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 180));

        }
    }


}
