using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catcher : MonoBehaviour
{
    [SerializeField]
    private float moveTime = 1.0f;
    [SerializeField]
    private float rotateTime = 1.0f;

    private Vector3 normal;
    private List<CubeCell> changeList;
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;
        changeList = new List<CubeCell>();
    }

    /// <summary>
    /// ChangeListにCellを追加
    /// </summary>
    /// <param name="cell">選択されたBlock</param>
    /// <param name="n">選択された場所のノーマルベクトル</param>
    public IEnumerator AddCellToList(CubeCell cell, Vector3 n)
    {
        if(changeList.Count >= 2)
        {
            return null;
        }

        if(!normal.Equals(n) && !normal.Equals(Vector3.zero))
        {
            return null;
        }

        if(changeList.Contains(cell))
        {
            cell.SetSelectState(false);
            changeList.Remove(cell);
            normal = Vector3.zero;
            return null;
        }

        normal = n;

        cell.SetSelectState(true);
        changeList.Add(cell);

        if(changeList.Count == 2)
        {
            return Actions();
        }

        return null;
    }

    /// <summary>
    /// Catcherが実行するAcitions
    /// </summary>
    /// <returns></returns>
    private IEnumerator Actions()
    {
        SetPosition();
        CatchCell();
        yield return Move(normal);
        yield return Rotate(normal);
        yield return Move(-normal);
        ReleaseChildren();
        RemoveAllCellFormList();
    }

    /// <summary>
    /// 位置設定
    /// </summary>
    private void SetPosition()
    {
        myTransform.localPosition = (changeList[0].LocalPosition + changeList[1].LocalPosition) / 2;
    }

    /// <summary>
    /// 選択されたCellを全部Cathcherの子にする
    /// </summary>
    private void CatchCell()
    {
        changeList.ForEach(cell => cell.SetParent(myTransform));
    }

    /// <summary>
    /// 移動のコルーチン
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerator Move(Vector3 direction)
    {
        float timer = 0;
        Vector3 startPos = myTransform.localPosition;
        Vector3 endPos = startPos + direction;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            myTransform.localPosition = Vector3.Lerp(startPos, endPos, timer / moveTime);
            yield return null;
        }

        myTransform.localPosition = Vector3.Lerp(startPos, endPos, 1);
    }

    /// <summary>
    /// 回転のコルーチン
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
    private IEnumerator Rotate(Vector3 axis)
    {
        float timer = 0;
        Quaternion startRot = Quaternion.identity;
        Quaternion endRot = Quaternion.Euler(axis * 180);

        while (timer < rotateTime)
        {
            timer += Time.deltaTime;
            myTransform.localRotation = Quaternion.Lerp(startRot, endRot, timer / rotateTime);
            yield return null;
        }

        myTransform.localRotation = Quaternion.Lerp(startRot, endRot, 1);
    }

    /// <summary>
    /// 選択されたCellを全部Cathcherの子から解放する
    /// </summary>
    private void ReleaseChildren()
    {
        changeList.ForEach(cell =>
        {
            cell.SetParent(myTransform.parent);
            cell.CleanUpTransform();
        });
        myTransform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// ChangeListをClean
    /// </summary>
    public void RemoveAllCellFormList()
    {
        changeList.ForEach(cell =>
        {
            cell.SetSelectState(false);
        });

        changeList.Clear();
        normal = Vector3.zero;
    }
}
