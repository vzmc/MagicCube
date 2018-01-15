using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 10;
    [SerializeField]
    private float zoomSpeed = 2;
    [SerializeField]
    private Transform aimTarget;

    public KeyCode RotateResetKey;

    private Transform pivotTransform;
    private Transform cameraTransform;
    private Transform myTransform;

    private Vector3 lastMousePos;

    private void Awake()
    {
        myTransform = transform;
        pivotTransform = myTransform.GetChild(0);
        cameraTransform = pivotTransform.GetChild(0);
    }

    void Start()
    {
        if (aimTarget != null)
        {
            myTransform.localPosition = aimTarget.position;
        }
    }

    public void RotateReset()
    {
        if (Input.GetKeyDown(RotateResetKey))
        {
            this.gameObject.transform.rotation = Quaternion.Euler(0, 30, 0);
            pivotTransform.transform.rotation = Quaternion.Euler(30, 30, 0);
        }
    }

    //public void GetInput(bool isButtonDown, bool isButtonHold)
    //{
    //    if (isButtonDown)
    //    {
    //        lastMousePos = Input.mousePosition;
    //    }

    //    if (isButtonHold)
    //    {
    //        transform.Rotate(Vector3.up, (Input.mousePosition.x - lastMousePos.x) * Time.deltaTime * rotateSpeed);
    //        pivotTransform.Rotate(Vector3.right, -(Input.mousePosition.y - lastMousePos.y) * Time.deltaTime * rotateSpeed);

    //        lastMousePos = Input.mousePosition;
    //    }
    //}

    public void GetInput(float mouseX, float mouseY)
    {
        transform.Rotate(Vector3.up, mouseX * rotateSpeed);
        pivotTransform.Rotate(Vector3.right, -mouseY * rotateSpeed);
    }

    public void AdjustDistance(float distance)
    {
        cameraTransform.Translate(0, 0, distance * zoomSpeed);
    }
}
