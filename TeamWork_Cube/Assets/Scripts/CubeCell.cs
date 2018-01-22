using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeMode
{
    Normal,
    Bomb,
    ColourClear,
    LineClear
}

public class CubeCell : MonoBehaviour
{
    [SerializeField]
    private CubeMode cubeMode = CubeMode.Normal;

    public Texture bombCubeMark;
    public Texture colourClearMark;
    //public Texture horizontalLineMark;
    //public Texture verticalLineMark;
    public Texture lineClearMark;
    [SerializeField]
    //private Texture nulltexture;
    private Transform myTransform;
    private Renderer myRenderer;
    private GameManager gameManager;
    private Transform selectBox;
    private Animator animator;
    private bool isVanishing; //10月20日　ホームズによる追加
    public GameObject ExplosionEffectPrefab;
    public GameObject LineClearEffectPrefab;
    public GameObject ColourClearEffectPrefab;

    const float specialChance = 0.02f; //特殊ブロック（爆弾ブロックなどとして生成される確率）
    const float lineClearChance = 0.008f; //線消しブロックとして生成される確率（爆弾ブロックの確率より優先される）
    const float colourClearChance = 0.002f; //同色消しブロックとして生成される確率（爆弾ブロック、線消しブロックの確率より優先される）

    public static List<CubeCell> allCubes = new List<CubeCell>();

    private CubeColor color;

    private Vector2 moveDireciton;
    private Vector2 normal;

    private bool isMoving;

    public delegate void CubeMatchEventHandler(CubeCell matchedCube);
    public static event CubeMatchEventHandler OnCubeMatch;

    public CubeMode CubeMode
    {
        get { return cubeMode; }
        set
        {
            cubeMode = value;
            SwitchTexture(value);
        }
    }

    public Vector3 LocalPosition
    {
        get
        {
            return myTransform.localPosition;
        }
    }

    public Transform MyTransform
    {
        get
        {
            return myTransform;
        }
    }

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }


    public void StartMove(Vector3 dir, float moveTime)
    {
        
        StartCoroutine(Move(dir, moveTime));
    }

    private IEnumerator Move(Vector3 dir, float moveTime)
    {
        isMoving = true;
        float time = 0;
        float rate = time / moveTime;
        SendMessage("OnMovement", SendMessageOptions.DontRequireReceiver);

        Vector3 start = transform.localPosition;
        Vector3 end = start + dir;

        while(rate < 1)
        {
            transform.localPosition = Vector3.Lerp(start, end, rate);
            time += Time.deltaTime;
            rate = time / moveTime;
            yield return null;
        }

        transform.localPosition = end;

        CleanUpTransform();
        isMoving = false;
    }

    public void SwitchTexture(CubeMode mode)
    {
        Texture output;
        switch (mode)
        {
            case CubeMode.Bomb:
                output = bombCubeMark;
                break;
            case CubeMode.ColourClear:
                output = colourClearMark;
                break;
            case CubeMode.LineClear:
                output = lineClearMark;
                break;
            default:
                output = null;
                break;
        }
        SetMaterial(gameManager.Cube_Materials[(int)color]);
        //Texture2D baseTex = //(Texture2D)myRenderer.material.mainTexture;
        //    new Texture2D(myRenderer.material.mainTexture.width,
        //    myRenderer.material.mainTexture.height);

        
        myRenderer.material.mainTexture = output;
    }



    private void Awake()
    {
        myTransform = transform;
        myRenderer = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        selectBox = myTransform.GetChild(0);
    }

    //private void Start()
    //{
    //    gameManager = GameManager.Instance;
    //    RandomSetMaterial();
    //}

    void Start()
    {
        allCubes.Add(this);
        GameManager.OnLevelUp += OnLevelUp;
        SwitchTexture(cubeMode);
    }

    private void OnDestroy()
    {
        GameManager.OnLevelUp -= OnLevelUp;
        allCubes.Remove(this);
    }

    public void Init()
    {
        gameManager = GameManager.Instance;
        RandomSetMaterial();
    }

    /// <summary>
    /// ランダムカラー
    /// </summary>
    public void RandomSetMaterial()
    {
        //1016何　　Cube_Materials＞＞Cube_Materials_List
        color = gameManager.ColourFromLocalIndex( Random.Range(0, gameManager.Cube_Materials_List.Count));
        SetMaterial(gameManager.Cube_Materials[(int)color]);
        //以上
        //float randomRoll = Random.Range(0f, 1f);
        //if(randomRoll < colourClearChance)
        //{
        //    CubeMode = CubeMode.ColourClear;
        //}
        //else if (randomRoll < lineClearChance)
        //{
        //    CubeMode = CubeMode.LineClear;
        //}
        //else if (randomRoll < specialChance)
        //{
        //    CubeMode = CubeMode.Bomb;
        //}
        /*else*/ CubeMode = CubeMode.Normal;
    }

    /// <summary>
    /// マテリアルを設定
    /// </summary>
    /// <param name="m"></param>
    private void SetMaterial(Material m)
    {
        myRenderer.material = m;
    }

    public Material GetMaterial()
    {
        return myRenderer.material;
    }

    /// <summary>
    /// Transformの誤差を消す
    /// </summary>
    public void CleanUpTransform()
    {
        CleanUpLocalPosition();
        //CleanUpLocalRotation();
    }

    /// <summary>
    /// 位置データの誤差を消す
    /// </summary>
    private void CleanUpLocalPosition()
    {
        Vector3 localPosition = myTransform.localPosition;
        localPosition.x = Mathf.Round(localPosition.x);
        localPosition.y = Mathf.Round(localPosition.y);
        localPosition.z = Mathf.Round(localPosition.z);
        myTransform.localPosition = localPosition;
    }

    /// <summary>
    /// 回転データを初期化
    /// </summary>
    private void CleanUpLocalRotation()
    {
        myTransform.localRotation = myTransform.parent.localRotation;
        //Vector3 localEulers = myTransform.localEulerAngles;
        //for(int i = 0; i < 3; i++)
        //{
        //    localEulers[i] /= 90;
        //    localEulers[i] = Mathf.Round(localEulers[i]);
        //    localEulers[i] *= 90;
        //}
    }

    /// <summary>
    /// 選択状態の設置
    /// </summary>
    /// <param name="isSelected">選択されたか？</param>
    public void SetSelectState(bool isSelected)
    {
        selectBox.gameObject.SetActive(isSelected);
    }

    /// <summary>
    /// マッチ後の動作(消えるー＞色変更ー＞現れるのアニメション、点数加算操作、エフェクト演出など。。。)
    /// </summary>
    public void StartVanishAction()
    {
        isVanishing = true;
        //1113 リストが二つあっての色と数字が不一致するバッグを解消
        int colorIndex = 0;
        for (int i = 0; i < gameManager.Cube_Materials.Length; i++)
        {
            //if(gameManager.Cube_Materials[i] == gameManager.Cube_Materials_List[(int)color])
            //{
            //    colorIndex = i;
            //}
            if(i == (int)color)
            {
                colorIndex = i;
            }
        }
        OnCubeMatch?.Invoke(this);
        GameManager.Instance.AddColorCount((CubeColor)colorIndex, 1);
        animator.SetTrigger("ChangeColor");
        if (CubeMode == CubeMode.Bomb)
        {
            GameObject xpfx = Instantiate(ExplosionEffectPrefab, transform.position, transform.rotation);
            Destroy(xpfx, 2.0f);
        }
        else if(CubeMode == CubeMode.LineClear)
        {
            GameObject xpfx = Instantiate(LineClearEffectPrefab, transform.position, transform.rotation);
            Destroy(xpfx, 2.0f);
        }
        else if(CubeMode == CubeMode.ColourClear)
        {
            GameObject xpfx = Instantiate(ColourClearEffectPrefab, transform.position, transform.rotation);
            Destroy(xpfx, 2.0f);
        }
        //to do
    }

    public void OnRespawnEvent()
    {
        isVanishing = false;
    }

    /// <summary>
    /// 色変更のアニメションを起動する
    /// </summary>
    private void StartChangeColorAnimation()
    {
        animator.SetTrigger("ChangeColor");
    }

    /// <summary>
    /// 親を指定する
    /// </summary>
    /// <param name="parent">親</param>
    public void SetParent(Transform parent)
    {
        myTransform.SetParent(parent);
    }

    void OnLevelUp(int amount)
    {
        //if (isVanishing)
        //{
        //    return;
        //}
        //for(int i = 0; i < amount; i++)
        //{
        //    float randomRoll = Random.Range(0f, 1f);
        //    if(randomRoll < colourClearChance)
        //    {
        //        CubeMode = CubeMode.ColourClear;
        //    }
        //    else if (randomRoll < lineClearChance)
        //    {
        //        CubeMode = CubeMode.LineClear;
        //    }
        //    else if(randomRoll < specialChance)
        //    {
        //        CubeMode = CubeMode.Bomb;
        //    }
        //}
    }

    //（10月20日、ホームズによる追加）
    /// <summary>
    /// 入力したキューブセルと同じ色のキューブセルを全てゲットする。
    /// </summary>
    /// <param name="test">チェックするキューブ</param>
    /// <returns></returns>
    public static List<CubeCell> GetSameColourCubes(CubeCell test)
    {
        List<CubeCell> output = new List<CubeCell>();
        foreach(CubeCell cc in allCubes)
        {
            if(cc.color == test.color)
            {
                output.Add(cc);
            }
        }
        return output;
    }

    // 新関数（ホームズ　1月22日）
    /// <summary>
    /// 存在中の特殊ブロックの数
    /// </summary>
    /// <returns>今ある特殊ブロックの数</returns>
    public static int numSpecialItems()
    {
        int output = 0;
        foreach(CubeCell cc in allCubes)
        {
            if (cc.cubeMode != CubeMode.Normal) output++;
        }
        return output;
    }

    // 新関数（ホームズ　1月22日）
    /// <summary>
    /// らんだむに選ぶ一つの無地ブロックをランダムの特殊ブロックにする
    /// </summary>
    public static void SetRandomPowerup()
    {
        Debug.Log("Setting Random Powerup.");
        if (numSpecialItems() >= 5 || //特殊ブロックを最大5つにする
            numSpecialItems() >= allCubes.Count) return; //無地キューブがないと以下の処理を行わない
        CubeCell selectedCube = allCubes[Random.Range(0, allCubes.Count - 1)];
        int triesLeft = allCubes.Count * 10; //無限ループを防ぐため
        while (selectedCube.cubeMode != CubeMode.Normal || selectedCube.isVanishing)
        {
            selectedCube = allCubes[Random.Range(0, allCubes.Count - 1)];
            triesLeft--;
            if (triesLeft <= 0)
            {
                Debug.Log("特殊道具が入っていないキューブは見つかりませんでした。");
            }
        }
        selectedCube.SetAsSpecialBlock();
    }

    /// <summary>
    /// このブロックを特殊ブロックにする
    /// </summary>
    public void SetAsSpecialBlock()
    {
        float randomRoll = Random.Range(0, specialChance);
        if (randomRoll < colourClearChance)
        {
            CubeMode = CubeMode.ColourClear;
        }
        else if (randomRoll < lineClearChance)
        {
            CubeMode = CubeMode.LineClear;
        }
        else
        {
            CubeMode = CubeMode.Bomb;
        }
        Debug.Log("Random Block Added.");
    }

    //private void OnMouseDown()
    //{
    //    Debug.Log(gameObject.name + ": I can smell your mouse, player!");
    //}
}
