using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ExpBar : MonoBehaviour
{

    private Transform myTransform;

    private int totalExp;

    [SerializeField]
    private float upValue;

    private int currentLevel;

    private int currentExpBarMinValue;
    private int currentExpBarMaxValue;

    private void Awake()
    {
        myTransform = transform;
    }

    // Use this for initialization
    void Start()
    {
        //スタート時にレベル1のvalueに変更
        currentLevel = GameManager.Instance.NowLevel;
        ChangeBarValue();
    }

    // Update is called once per frame
    void Update()
    {
        SetExp();
    }

    private void SetExp()
    {
        totalExp = GameManager.Instance.MatchedCount;
        //今現在の値
        float currentValue = myTransform.GetComponent<Slider>().value;

        //徐々に数値が上がる仕様に変更（上がる数値はエディタ上で変更可能）
        if (totalExp > currentValue)
        {
            myTransform.GetComponent<Slider>().value += upValue;
        }

        //スライダー上の値がmaxになったらbarのvalue変更する
        if (currentValue.Equals(myTransform.GetComponent<Slider>().maxValue))
        {
            currentLevel++;
            ChangeBarValue();
        }
    }

    private void ChangeBarValue()
    {
        //レベルアップスキップしないように変更
        currentExpBarMinValue = GameManager.Instance.Level_Exp_Dic[currentLevel];
        currentExpBarMaxValue = GameManager.Instance.Level_Exp_Dic[currentLevel + 1];

        myTransform.GetComponent<Slider>().minValue = currentExpBarMinValue;
        myTransform.GetComponent<Slider>().maxValue = currentExpBarMaxValue;
    }
}
