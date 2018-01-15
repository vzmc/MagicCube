using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MagicCube全体管理する
/// </summary>
public class MagicCube : MonoBehaviour
{
    [SerializeField]
    private int cubeLevel = 3;                  // 階層
    [SerializeField]
    private GameObject cubeCellPrefab;          // cubeのPrefab
    [SerializeField]
    private int rotateInterval = 5;             // 何ターンことに回転
    [SerializeField]
    private RotationArrowDisplay rotationArrowDisplay;
    [SerializeField]
    private float moveTime = 1;
    [SerializeField]
    private float limitTime = 1;

    [SerializeField]
    private bool isLimitTimeMode = false;

    private int countDown;
    int matchedCount;


    public int MatchedCountNow
    {
        get
        {
            return matchedCount;
        }
    }
    public int CubeLevel
    {
        get
        {
            return cubeLevel;
        }
    }

    public int RotateInterval
    {
        get
        {
            return rotateInterval;
        }
    }

    public bool IsSliding
    {
        get
        {
            return isSliding;
        }
    }

    public bool IsChanging
    {
        get
        {
            return isChanging;
        }
    }

    private GameManager gameManager;

    private List<CubeCell> cellList;            // 全部cubeのインスタンスを収納するList
    private List<CubeCell> surfaceList;         // ある面のCubeを収納するList（回転時、マッチ判定時用）
    private List<CubeCell> circleList;          // あるサークルのCubeを収納するList（サークルマッチ判定時用）
    private List<CubeCell> matchedCubeList;     // 揃ったCubeを収納するList

    private List<CubeCell> chainList;           // あるサークルのCubeを収納するList（サークルマッチ判定時用）

    private Transform myTransform;              // 自分のTransform
    private Center center;                      // 回転中心
    private Catcher catcher;                    // 二つのBlockを交換時用
    private Vector3[] rotationAixs;             // 回転軸の定義
    private SurfaceData[] surfaceData;          // 6面を表す

    public Axis rotateAxis = Axis.X;
    public int rotateDepth = 0;
    public RotationDirection rotateRd = RotationDirection.CCW;
    public int speed = 100;

    private bool isChanging;                    // 回転中か？
    private bool isSliding;                     // スライディング中か？

    private float countDownTimer;
    private bool isCounting;

    private void Awake()
    {
        myTransform = transform;
        center = myTransform.Find("Center").GetComponent<Center>();
        catcher = myTransform.Find("Catcher").GetComponent<Catcher>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        matchedCubeList = new List<CubeCell>();
        chainList = new List<CubeCell>();
        rotationAixs = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };
        surfaceData = new SurfaceData[]
        {
            new SurfaceData(Axis.X, 0), new SurfaceData(Axis.X, cubeLevel - 1),
            new SurfaceData(Axis.Y, 0), new SurfaceData(Axis.Y, cubeLevel - 1),
            new SurfaceData(Axis.Z, 0), new SurfaceData(Axis.Z, cubeLevel - 1),
        };

        RandomRotationParameter();
        SpwanMagicCube(cubeLevel);

        InitTimer();
    }

    private void Update()
    {
        UpdateCountDownTimer();
    }

    private void InitTimer()
    {
        isCounting = false;
        countDownTimer = limitTime;
        gameManager.SetLimitTimeText(limitTime);
        gameManager.SetLimitTimeTextState(false);
    }

    private void UpdateCountDownTimer()
    {
        if(isCounting)
        {
            countDownTimer -= Time.deltaTime;
            countDownTimer = Mathf.Clamp(countDownTimer, 0, limitTime);

            gameManager.SetLimitTimeText(countDownTimer);

            if (countDownTimer <= 0)
            {
                isChanging = true;

                if (isSliding)
                {
                    return;
                }

                //1120何　今のターン＋１
                gameManager.RoundIncrement();
                //1120何　一回足される前のターンを表示
                gameManager.SetNowTurn();

                StartCoroutine(StartCellAction());

                InitTimer();
            }
        }
    }

    public void StartCountDown()
    {
        gameManager.SetLimitTimeTextState(true);
        isCounting = true;

        //1120何 コンボリセット
        gameManager.ResetCombo();
    }

    /// <summary>
    /// マジックチューブを生成
    /// </summary>
    /// <param name="level">階数</param>
    private void SpwanMagicCube(int level)
    {
        cellList = new List<CubeCell>();
        Vector3 tempPos = new Vector3();
        GameObject tempObject;
        CubeCell tempCubeCell;

        for (int i = 0; i < level; i++)
        {
            for (int j = 0; j < level; j++)
            {
                for (int k = 0; k < level; k++)
                {
                    if (0 < i && i < level - 1 && 0 < j && j < level - 1 && 0 < k && k < level - 1)
                    {
                        // 中のCubeを生成する必要がない
                        continue;
                    }
                    tempPos.Set(i, j, k);
                    tempObject = Instantiate(cubeCellPrefab, tempPos, Quaternion.identity, myTransform);
                    tempCubeCell = tempObject.GetComponent<CubeCell>();
                    tempCubeCell.Init();
                    cellList.Add(tempCubeCell);
                }
            }
        }
        MakeSureNoMatch();
        float centerPos = (level - 1) / (float)2;
        tempPos.Set(centerPos, centerPos, centerPos);
        center.LocalPosition = tempPos;
    }

    /// <summary>
    /// 初期状態をマッチしないにする
    /// </summary>
    private void MakeSureNoMatch()
    {
        while (true)
        {
            if (CheckSixSurface(false))
            {
                break;
            }
            else
            {
                matchedCubeList.ForEach(cell => cell.RandomSetMaterial());
            }
        }
    }

    private void RandomRotationParameter()
    {
        rotateAxis = (Axis)UnityEngine.Random.Range(0, 3);
        rotateDepth = UnityEngine.Random.Range(0, cubeLevel);
        rotateRd = UnityEngine.Random.Range(0, 2) == 0 ? RotationDirection.CW : RotationDirection.CCW;

        // アローの位置と回転を設定する 1117無効
        //rotationArrowDisplay.RefreshArrow(rotateAxis, rotateDepth, rotateRd);
    }

    /// <summary>
    /// Blockを指定する(PlayerのInputによる選択、またはAIによる選択)
    /// </summary>
    /// <param name="cell">Block</param>
    /// <param name="normal">ノーマルベクトル</param>
    public void SelectCube(Transform cell, Vector3 normal)
    {
        if (isChanging)
        {
            return;
        }

        IEnumerator change = catcher.AddCellToList(cell.GetComponent<CubeCell>(), normal);

        if (change != null)
        {
            StartCoroutine(ChangeAndRotate(change));
        }
    }

    /// <summary>
    /// 選択解除
    /// </summary>
    public void CancelSelectCube()
    {
        if (isChanging)
        {
            return;
        }
        catcher.RemoveAllCellFormList();
    }

    /// <summary>
    /// 回転用のCoroutine
    /// </summary>
    /// <param name="axis">回転軸</param>
    /// <param name="rd">回転方向</param>
    /// <param name="speed">回転速度</param>
    /// <returns></returns>
    private IEnumerator RotateCoroutine(Axis axis, int depth, RotationDirection rd, float speed)
    {
        //isRotating = true;
        AddCubeToCenter(axis, depth);
        yield return center.RotateCoroutine(rotationAixs[(int)axis], rd, speed);
        ReleaseCubeFromCenter();
        //isRotating = false;
    }

    /// <summary>
    /// 指定する面のCubeを探し出し、surfaceListに入れる
    /// </summary>
    /// <param name="axis">軸</param>
    /// <param name="depth">階層</param>
    /// <param name="surfaceList">結果のCubeを格納するList</param>
    private void FindSurface(Axis axis, int depth, out List<CubeCell> surfaceList)
    {
        switch (axis)
        {
            case Axis.X:
                {
                    surfaceList = cellList.FindAll(cell => Mathf.Approximately(cell.LocalPosition.x, depth));
                    break;
                }
            case Axis.Y:
                {
                    surfaceList = cellList.FindAll(cell => Mathf.Approximately(cell.LocalPosition.y, depth));
                    break;
                }
            case Axis.Z:
                {
                    surfaceList = cellList.FindAll(cell => Mathf.Approximately(cell.LocalPosition.z, depth));
                    break;
                }
            default:
                {
                    surfaceList = null;
                    break;
                }
        }
    }

    /// <summary>
    /// 回転のCubeをCenterの子供にする
    /// </summary>
    /// <param name="axis">回転軸</param>
    /// <param name="depth">軸上の階層</param>
    private void AddCubeToCenter(Axis axis, int depth)
    {
        FindSurface(axis, depth, out surfaceList);
        center.AddChildren(surfaceList);
    }

    /// <summary>
    /// 回転のCubeを全てCenterから釈放し、Centerの回転を初期化する
    /// </summary>
    private void ReleaseCubeFromCenter()
    {
        center.ReleaseChildren();
    }

    /// <summary>
    /// 面内のCubeをソートする
    /// </summary>
    /// <param name="surfaceList">面のList</param>
    private void SortSurfaceList(ref List<CubeCell> surfaceList)
    {
        surfaceList.Sort(SortRule);
    }

    /// <summary>
    /// CubeCellソートのルール
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>ソート結果</returns>
    private int SortRule(CubeCell a, CubeCell b)
    {
        if (a.LocalPosition.x.CompareTo(b.LocalPosition.x) != 0)
        {
            return a.LocalPosition.x.CompareTo(b.LocalPosition.x);
        }

        if (a.LocalPosition.y.CompareTo(b.LocalPosition.y) != 0)
        {
            return a.LocalPosition.y.CompareTo(b.LocalPosition.y);
        }

        if (a.LocalPosition.z.CompareTo(b.LocalPosition.z) != 0)
        {
            return a.LocalPosition.z.CompareTo(b.LocalPosition.z);
        }

        return 0;
    }

    /// <summary>
    /// Block交換行動と回転行動を順番で実行する
    /// </summary>
    /// <param name="change">Block交換行動のコルーチン</param>
    /// <param name="rotate">回転行動のコルーチン</param>
    /// <returns>連結後のコルーチン</returns>
    private IEnumerator ChangeAndRotate(IEnumerator change)//, IEnumerator rotate = null
    {
        isChanging = true;
        //gameManager.ResetCombo(); //11120何　使われていない
        yield return change;

        gameManager.TurnIncrement();//20171103 ホームズ
        if (gameManager.RotateCountDown == 0)
        {
            yield return RotateCoroutine(rotateAxis, rotateDepth, rotateRd, speed);
            yield return StartCellAction();
            RandomRotationParameter();
            gameManager.TurnReload();//20171103 ホームズ
        }
        isChanging = false;
    }

    /// <summary>
    /// Cellのアクションを実行するコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartCellAction()
    {
        while (true)
        {
            if (CheckSixSurface())
            {
                break;
            }

            //何1006追加分
            //int matchedCount = matchedCubeList.Count;
            matchedCount = matchedCubeList.Count; //林1127
            gameManager.MatchedCountAndScoreIncrement(matchedCount);
            gameManager.SetMatchedCountText();
            matchedCubeList.ForEach(cell => cell.StartVanishAction()); //順番を変わった（10月20日ホームズ）
            gameManager.LeveltIncrement();
            
            gameManager.SetLevelText();
            //↑↑↑↑1006追加分↑↑↑↑↑
            gameManager.SetScoreText();
            //↑↑↑↑1110追加分↑↑↑↑↑

            gameManager.SetComboText();
            matchedCubeList.Clear();
            yield return new WaitForSeconds(1.1f);
        }

        isChanging = false;

        //1117何　レベルUPした場合表示ターンが増える
        gameManager.SetNowTurn();

        //1120何　最終ターンかどうかをチェック
        gameManager.CheckIfFinalTurn();
    }

    /// <summary>
    /// 6面をマッチチェックする、マッチがない場合、trueを返す
    /// </summary>
    /// <param name="triggerSpecialBlocks">Falseなら、マッチしても爆弾キューブ、線消しキューブなどの処理を行いません。（10月6日　ホームズによる追加）</param>
    /// <returns>マッチがあるかどうか</returns>
    private bool CheckSixSurface(bool triggerSpecialBlocks = true)
    {
        matchedCubeList.Clear();
        for (int i = 0; i < 6; i++)
        {
            FindSurface(surfaceData[i].axis, surfaceData[i].depth, out surfaceList);
            SortSurfaceList(ref surfaceList);
            FindMatchedCubes(surfaceList, ref matchedCubeList, triggerSpecialBlocks);
        }

        if (matchedCubeList.Count == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 面内のマッチ状況をチェックし、マッチしたCubeを全部matchedCubeListに入れる
    /// </summary>
    /// <param name="surfaceList">面内Cubeを格納するList</param>
    /// <param name="matchedCubeList">マッチしたCubeを格納するList</param>
    private void FindMatchedCubes(List<CubeCell> surfaceList, ref List<CubeCell> matchedCubeList, bool triggerSpecialBlocks = true)
    {
        if (surfaceList == null)// || surfaceList.Count < 36
        {
            Debug.Log("取り出した面内のCube数が間違っている！");
            Debug.Assert(false, "取り出した面内のCube数が間違っている！");
            return;
        }

        for (int i = 0; i < cubeLevel; i++)
        {
            CheckRow(i, surfaceList, ref matchedCubeList);
            CheckCol(i, surfaceList, ref matchedCubeList);
        }

        //以下はホームズによる追加（10月6日）
        // Check the list of matched cubes for bomb blocks or line-clearing blocks
        //マッチされたキューブの中で、爆弾ブロックと線消しブロックを探す
        if (triggerSpecialBlocks)
        {
            bool repeatFlag = true; //10月20日　ホームズによる追加
            while (repeatFlag)//10月20日　ホームズによる追加
            {
                repeatFlag = false;//10月20日　ホームズによる追加
                List<CubeCell> extraCubeList = new List<CubeCell>();
                foreach (CubeCell c in matchedCubeList)
                {
                    //List<CubeCell> specialList = SpecialMatch(c);
                    //foreach(CubeCell s in specialList)
                    //{
                    //    if (!extraCubeList.Contains(s)) {
                    //        extraCubeList.Add(s);
                    //        //SpecialMatch(s).ForEach(p => extraCubeList.Add(p));
                    //    }
                    //}
                    SpecialMatch(c, ref extraCubeList);
                }
                foreach (CubeCell c in extraCubeList)
                {
                    if (!matchedCubeList.Contains(c))
                    {
                        matchedCubeList.Add(c);
                        repeatFlag = true;
                    }
                }
            }
        }

        //以上
    }

    /// <summary>
    /// 爆弾ブロック、線消しブロックの処理（未完成）
    /// </summary>
    /// <param name="c">チェックするキューブセル</param>
    private void SpecialMatch(CubeCell c, ref List<CubeCell> matchedCubeList)
    {
        //List<CubeCell> specialCubeList = new List<CubeCell>();
        switch (c.CubeMode)
        {
            case CubeMode.Bomb:
                //Add all cubes around the bomb to the match list
                //周りのキューブを全てマッチリストに追加する
                Collider[] surroundingCubes = Physics.OverlapBox(c.transform.position, Vector3.one);
                //Debug.DrawRay(transform.position, transform.up, Color.green, 0.5f);
                //Debug.Break();
                foreach (var s in surroundingCubes)
                {
                    CubeCell cell = s.GetComponent<CubeCell>();
                    if (matchedCubeList.Contains(cell)) continue;
                    if (cell != null)
                    {
                        matchedCubeList.Add(cell);
                    }
                }
                break;
            case CubeMode.ColourClear:
                foreach(CubeCell cc in CubeCell.GetSameColourCubes(c))
                {
                    if (matchedCubeList.Contains(cc)) continue;
                    matchedCubeList.Add(cc);
                }
                break;
            case CubeMode.LineClear:
                //TODO: Add all cubes in the same row or column to the Match List
                List<CubeCell> tempList = new List<CubeCell>();
                if(Physics.Raycast(c.transform.position, transform.right, 1.0f) || Physics.Raycast(c.transform.position, -transform.right, 1.0f))
                {
                    foreach (var r in Physics.RaycastAll(c.transform.position, transform.right))
                    {
                        CubeCell cell = r.collider.GetComponent<CubeCell>();
                        if (cell != null && !tempList.Contains(cell))
                            tempList.Add(cell);
                    }
                    foreach (var r in Physics.RaycastAll(c.transform.position, -transform.right))
                    {
                        CubeCell cell = r.collider.GetComponent<CubeCell>();
                        if (cell != null && !tempList.Contains(cell))
                            tempList.Add(cell);
                    }
                }
                if (Physics.Raycast(c.transform.position, transform.up, 1.0f) || Physics.Raycast(c.transform.position, -transform.up, 1.0f))
                {
                    foreach (var r in Physics.RaycastAll(c.transform.position, transform.up))
                    {
                        CubeCell cell = r.collider.GetComponent<CubeCell>();
                        if (cell != null && !tempList.Contains(cell))
                            tempList.Add(cell);
                    }
                    foreach (var r in Physics.RaycastAll(c.transform.position, -transform.up))
                    {
                        CubeCell cell = r.collider.GetComponent<CubeCell>();
                        if (cell != null && !tempList.Contains(cell))
                            tempList.Add(cell);
                    }
                }
                if (Physics.Raycast(c.transform.position, transform.forward, 1.0f) || Physics.Raycast(c.transform.position, -transform.forward, 1.0f))
                {
                    foreach (var r in Physics.RaycastAll(c.transform.position, transform.forward))
                    {
                        CubeCell cell = r.collider.GetComponent<CubeCell>();
                        if (cell != null && !tempList.Contains(cell))
                            tempList.Add(cell);
                    }
                    foreach (var r in Physics.RaycastAll(c.transform.position, -transform.forward))
                    {
                        CubeCell cell = r.collider.GetComponent<CubeCell>();
                        if (cell != null && !tempList.Contains(cell))
                            tempList.Add(cell);
                    }
                }
                foreach(var cc in tempList)
                {
                    if (matchedCubeList.Contains(cc)) continue;
                    matchedCubeList.Add(cc);
                }
                break;
            default:
                break;
        }
        //return specialCubeList;
    }

    /// <summary>
    /// 指定する面の指定の行のCubeのマッチ状況を判定し、マッチしたCubeをmatchedCubeListに追加する
    /// </summary>
    /// <param name="row">指定行</param>
    /// <param name="surfaceList">面内Cubeを格納するList</param>
    /// <param name="matchedCubeList">マッチしたCubeを格納するList</param>
    private void CheckRow(int row, List<CubeCell> surfaceList, ref List<CubeCell> matchedCubeList)
    {
        List<CubeCell> match = new List<CubeCell>();
        int index = 0;

        while (index < cubeLevel)
        {
            CubeCell cell = surfaceList[index + row * cubeLevel];
            Material m = cell.GetMaterial();
            match.Add(cell);

            for (int i = index + 1; i < cubeLevel; i++)
            {
                CubeCell temp = surfaceList[i + row * cubeLevel];
                if (temp.GetMaterial().color == m.color)
                {
                    match.Add(temp);
                }
                else
                {

                    break;
                }
            }

            if (match.Count >= 3)
            {
                int count = 0;          // カウント数を重複しないように
                foreach (var c in match)
                {
                    if (!matchedCubeList.Contains(c))
                    {
                        matchedCubeList.Add(c);
                        count++;
                    }
                }
                if (count > 0)
                {
                    gameManager.ComboIncrement();
                }
                index += match.Count;
            }
            else
            {
                index++;
            }
            match.Clear();
        }
    }

    /// <summary>
    /// 指定する面の指定の列のCubeのマッチ状況を判定し、マッチしたCubeをmatchedCubeListに追加する
    /// </summary>
    /// <param name="col">指定列</param>
    /// <param name="surfaceList">面内Cubeを格納するList</param>
    private void CheckCol(int col, List<CubeCell> surfaceList, ref List<CubeCell> matchedCubeList)
    {
        List<CubeCell> match = new List<CubeCell>();
        int index = 0;

        while (index < cubeLevel)
        {
            CubeCell cell = surfaceList[index * cubeLevel + col];
            Material m = cell.GetMaterial();
            match.Add(cell);

            for (int i = index + 1; i < cubeLevel; i++)
            {
                CubeCell temp = surfaceList[i * cubeLevel + col];
                if (temp.GetMaterial().color == m.color)
                {
                    match.Add(temp);
                }
                else
                {

                    break;
                }
            }

            if (match.Count >= 3)
            {
                int count = 0;          // カウント数を重複しないように
                foreach (var c in match)
                {
                    if (!matchedCubeList.Contains(c))
                    {
                        matchedCubeList.Add(c);
                        count++;
                    }
                }
                if (count > 0)
                {
                    gameManager.ComboIncrement();
                }
                index += match.Count;
            }
            else
            {
                index++;
            }
            match.Clear();
        }
    }


    //====================================================
    // 新たな機能
    //====================================================
    
    /// <summary>
    /// BlockChainを捕獲し、移動させる
    /// </summary>
    /// <param name="start">開始Block</param>
    /// <param name="direction">方向</param>
    /// <param name="normal">法線</param>
    public void MakeBlockChainMove(Transform start, Vector3 direction, Vector3 normal)
    {
        if(!isCounting)
        {
            StartCountDown();
        }

        if(isSliding)
        {
            return;
        }

        isSliding = true;

        chainList.Clear();

        Ray ray = new Ray();
        RaycastHit raycastHit;
        int layer = LayerMask.GetMask("Cell");

        Transform nowTransform = start;
        Vector3 temp;

        CubeCell cube;

        while (true)
        {
            ray.origin = nowTransform.localPosition;
            ray.direction = direction;

            if (Physics.Raycast(ray, out raycastHit, 1, layer))
            {
                cube = nowTransform.GetComponent<CubeCell>();
                chainList.Add(cube);
                cube.StartMove(direction, moveTime);

                if (raycastHit.transform == start)
                {
                    break;
                }
                nowTransform = raycastHit.transform;
            }
            else
            {
                temp = direction;
                direction = -normal;
                normal = temp;
            }
        }

        StartCoroutine(MoveCountDown());
    }

    private IEnumerator MoveCountDown()
    {
        bool isOver = true;
        while(true)
        {
            isOver = true;
            foreach (var cell in chainList)
            {
                if(cell.IsMoving)
                {
                    isOver = false;
                    break;
                }
            }

            if(isOver)
            {
                break;
            }

            yield return null;
        }
        isSliding = false;
        Debug.Log("MoveOver");
    }
}



/// <summary>
/// 回転軸
/// </summary>
public enum Axis
{
    X = 0,
    Y,
    Z,
}

/// <summary>
/// 回転方向
/// </summary>
public enum RotationDirection
{
    CW = -1,    //時計回り
    CCW = 1,    //反時計回り
}

/// <summary>
/// 面を表す構造体
/// </summary>
public struct SurfaceData
{
    public Axis axis;
    public int depth;

    public SurfaceData(Axis a, int d)
    {
        axis = a;
        depth = d;
    }
}