using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    [SerializeField]
    private Transform AimTarget;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if(_transform.position != AimTarget.position)
        {
            _transform.position = AimTarget.position;
        }
    }
}
