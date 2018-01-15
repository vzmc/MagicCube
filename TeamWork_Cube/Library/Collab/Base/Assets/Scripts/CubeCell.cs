using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeMode
{
    Normal,
    Bomb,
    LineClearHorizontal,
    LineClearVertical
}

public class CubeCell : MonoBehaviour
{
    [SerializeField]private CubeMode cubeMode = CubeMode.Normal;
    public Texture bombCubeMark;
    public Texture horizontalLineMark;
    public Texture verticalLineMark;
    private Transform myTransform;
    private Renderer myRenderer;
    private GameManager gameManager;
    private Transform selectBox;
    private Animator animator;
    public GameObject ExplosionEffectPrefab;

    const float specialChance = 0.02f; //特殊ブロック（爆弾ブロックなどとして生成される確率）

    public CubeMode CubeMode
    {
        get { return cubeMode; }
        set
        {
            cubeMode = value;
            SwitchTexture(value);
        }
    }

    public void SwitchTexture(CubeMode mode)
    {
        Texture output;
        switch (mode)
        {
            case CubeMode.Bomb:
                output = bombCubeMark;
                break;
            case CubeMode.LineClearHorizontal:
                output = horizontalLineMark;
                break;
            case CubeMode.LineClearVertical:
                output = verticalLineMark;
                break;
            default:
                output = null;
                break;
        }
        myRenderer.material.mainTexture = output;
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
        GameManager.OnLevelUp += OnLevelUp;
        SwitchTexture(cubeMode);
    }

    private void OnDestroy()
    {
        GameManager.OnLevelUp -= OnLevelUp;
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
        SetMaterial(gameManager.Cube_Materials[Random.Range(0, gameManager.Cube_Materials.Length)]);
        if (Random.Range(0f, 1f) < specialChance)
        {
            CubeMode = CubeMode.Bomb;
        }
        else CubeMode = CubeMode.Normal;
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
        CleanUpLocalRotation();
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
        animator.SetTrigger("ChangeColor");
        if (CubeMode == CubeMode.Bomb)
        {
            GameObject xpfx = Instantiate(ExplosionEffectPrefab, transform.position, transform.rotation);
            Destroy(xpfx, 2.0f);
        }
        //to do
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
        for(int i = 0; i < amount; i++)
        {
            if(Random.Range(0f, 1f) < specialChance)
            {
                CubeMode = CubeMode.Bomb;
            }
        }
    }
}
