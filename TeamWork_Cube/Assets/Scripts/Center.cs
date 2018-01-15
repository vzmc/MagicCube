using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour
{
    private Transform myTransform;
    private List<CubeCell> childrenList;

    public Vector3 LocalPosition
    {
        get
        {
            return myTransform.localPosition;
        }
        set
        {
            myTransform.localPosition = value;
        }
    }

    private void Awake()
    {
        myTransform = transform;
    }

    /// <summary>
    /// 子供Listを追加する
    /// </summary>
    /// <param name="list"></param>
    public void AddChildren(List<CubeCell> list)
    {
        childrenList = list;
        childrenList.ForEach(cell => cell.SetParent(myTransform));
    }

    /// <summary>
    /// 回転のCubeを全てCenterから釈放し、Centerの回転を初期化する
    /// </summary>
    public void ReleaseChildren()
    {
        childrenList.ForEach(cell =>
        {
            cell.SetParent(myTransform.parent);
            cell.CleanUpTransform();
        });
        myTransform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 回転用のCoroutine
    /// </summary>
    /// <param name="rotationAixs">回転軸</param>
    /// <param name="rd">回転方向</param>
    /// <param name="speed">回転速度</param>
    /// <returns></returns>
    public IEnumerator RotateCoroutine(Vector3 rotationAixs, RotationDirection rd, float speed)
    {
        float sum = 0;
        float delta = 0;

        while (sum < 90)
        {
            delta = speed * Time.deltaTime;
            myTransform.Rotate(rotationAixs, (int)rd * delta);
            sum += delta;
            yield return null;
        }

        myTransform.localRotation = Quaternion.Euler(rotationAixs * 90 * (int)rd);
    }
}
