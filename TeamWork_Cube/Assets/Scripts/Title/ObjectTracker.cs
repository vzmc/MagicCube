using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectTracker : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.transform.position + offset);
    }
}
