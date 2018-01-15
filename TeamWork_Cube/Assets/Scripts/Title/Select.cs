using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    public static string ObjName;
    public static string frontObjName;
    public static bool Flag;
    public static bool animFlag;

    public float distance = 100f;


    public static bool Test;
    

    // Use this for initialization
    void Start ()
    {
        animFlag = false;
        Flag = false;
        ObjName = null;
        frontObjName = null;

        Test = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        LeftClick();
        RightClick();
        StartCoroutine(FrontRay());
        Invoke("NameDelete", 0.5f);
        
    }

    //SelectCubeでUpdateし続けるため0.5秒で強制的にnullにする
    private void NameDelete()
    {
        ObjName = null;
    }

    private void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Flag == true)
            {
                // クリックしたスクリーン座標をrayに変換
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Rayの当たったオブジェクトの情報を格納する
                RaycastHit hit = new RaycastHit();
                // オブジェクトにrayが当たった時
                if (Physics.Raycast(ray, out hit, distance))
                {
                    // rayが当たったオブジェクトの名前を取得
                    string objectName = hit.collider.gameObject.name;
                    ObjName = objectName;
                }
                if (animFlag == false)
                {
                    animFlag = true;
                }
                
            }
            else
            {
            }
        }
    }

    //android対応(デバックしてない)
    private void tap()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Flag == true)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    RaycastHit hit = new RaycastHit();
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    if (Physics.Raycast(ray))
                    {
                        string objectName = hit.collider.gameObject.name;
                        ObjName = objectName;
                    }
                }
            }
            else
            {
                //回転中
            }
        }
    }

    //オブジェクトを右クリックで戻る（Cube: Single→Start）
    private void RightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (Flag == true)
            {
                // クリックしたスクリーン座標をrayに変換
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Rayの当たったオブジェクトの情報を格納する
                RaycastHit hit = new RaycastHit();
                // オブジェクトにrayが当たった時
                if (Physics.Raycast(ray, out hit, distance))
                {
                    // rayが当たったオブジェクトの名前を取得
                    string objectName = hit.collider.gameObject.name;
                    ObjName = objectName +"can";
                }
            }
        }
    }

   

    //カメラから見えているシーン移動オブジェクトを取得
    IEnumerator FrontRay()
    {
        Ray ray = new Ray(transform.position + new Vector3(0,-0.2f,2), transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            string objectName = hit.collider.gameObject.name;
            frontObjName = objectName;
        }

        yield return new WaitForSeconds(0.5f);
    }
}
