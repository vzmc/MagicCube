using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour
{

    public float gizmoSize = 0.3f;
    public Color gizmoColor = Color.yellow;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
}

//参考 http://portaltan.hatenablog.com/entry/2016/04/15/134129