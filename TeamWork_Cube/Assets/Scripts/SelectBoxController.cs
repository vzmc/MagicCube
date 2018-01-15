using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBoxController : MonoBehaviour
{
    private Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
    }

    public void Start()
    {
        
    }

    public void SetRotationAndPosition(float maxLevel, Axis axis, int level)
    {
        float centerPos = (maxLevel-1) / 2;
        switch(axis)
        {
            case Axis.X:
                {
                    myTransform.localScale = new Vector3(1.05f, maxLevel, maxLevel);
                    myTransform.position = new Vector3(level, centerPos, centerPos);
                    break;
                }
            case Axis.Y:
                {
                    myTransform.localScale = new Vector3(maxLevel, 1.05f, maxLevel);
                    myTransform.position = new Vector3(centerPos, level, centerPos);
                    break;
                }
            case Axis.Z:
                {
                    myTransform.localScale = new Vector3(maxLevel, maxLevel, 1.05f);
                    myTransform.position = new Vector3(centerPos, centerPos, level);
                    break;
                }
        }
    }
}
