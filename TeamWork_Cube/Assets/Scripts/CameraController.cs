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

    //[SerializeField]
    //private UnityStandardAssets.CrossPlatformInput.Joystick joystick;
    //(スワイプ回転があるからJoystickはもう要りません。

    public KeyCode RotateResetKey;

    private Transform pivotTransform;
    private Transform cameraTransform;
    private Transform myTransform;
    private Camera myCamera; // 2017/12/15 Holmes

    private Vector3 lastMousePos;

    //以下はHolmesによる追加（2017/12/15~18）
    private int horizontalRotationStep = 0; //12月22日にホームズにより削除。
    private bool viewFromBelow = false;
    private Vector3 horizontalTargetEulerRotation = Vector3.zero;
    private Vector3 pivotTargetEulerRotation = new Vector3(0, 30, 0);

    //以下はホームズによる追加です
    public void AdjustTargetRotation(float horizontalDegrees, float verticalDegrees, float intensity = 1.0f)
    {
        horizontalTargetEulerRotation.y += horizontalDegrees * intensity * Time.deltaTime;
        pivotTargetEulerRotation.x -= verticalDegrees * intensity * Time.deltaTime;
    }
    //以上

    private void CalculateTargetRotationFromStep()
    {
        horizontalTargetEulerRotation = new Vector3(0, 45 + 90 * horizontalRotationStep, 0);
        float verticalAngle = 30;
        if (viewFromBelow) verticalAngle = -verticalAngle;
        pivotTargetEulerRotation = new Vector3(verticalAngle, 0, 0);
    }

    public void HorizontalStep(int amount)
    {
        Debug.Log("Horizontal step " + amount);
        horizontalRotationStep += amount;
        while (horizontalRotationStep >= 4) horizontalRotationStep -= 4;
        while (horizontalRotationStep < 0) horizontalRotationStep += 4;
    }

    public void VerticalLookMode(bool willViewFromBelow)
    {
        if (willViewFromBelow) Debug.Log("Viewing From Below");
        else Debug.Log("Viewing From Above");
        viewFromBelow = willViewFromBelow;
    }

    private void Update()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        CalculateTargetRotationFromStep();
        Quaternion targetQuat = Quaternion.Euler(horizontalTargetEulerRotation);
        Quaternion pivotQuat = Quaternion.Euler(pivotTargetEulerRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetQuat, Time.deltaTime * 10);
        //transform.rotation = targetQuat;
        pivotTransform.localRotation = Quaternion.Lerp(pivotTransform.localRotation, pivotQuat, Time.deltaTime * 10);
#endif
    }
    //以上

    private void Awake()
    {
        myTransform = transform;
        pivotTransform = myTransform.GetChild(0);
        cameraTransform = pivotTransform.GetChild(0);
        myCamera = cameraTransform.GetComponent<Camera>(); // 2017/12/15 Holmes
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
    //public void GetInput()
    //{
    //    //if(joystick.GetHoldFlag())
    //    //{
    //    //    transform.Rotate(Vector3.up, joystick.GetVector().x * Time.deltaTime * rotateSpeed);
    //    //    pivotTransform.Rotate(Vector3.right, -joystick.GetVector().y * Time.deltaTime * rotateSpeed);

    //    //}
    //}

    public void GetInput(float mouseX, float mouseY)
    {
        transform.Rotate(Vector3.up, mouseX * rotateSpeed);
        //pivotTransform.Rotate(Vector3.right, -mouseY * rotateSpeed);
        pivotTransform.Rotate(Vector3.right, -mouseY * rotateSpeed);
        
        Vector3 ea = pivotTransform.localEulerAngles;
        Debug.Log(ea.x);
        if (ea.x > 180) ea.x -= 360;
        if (ea.x > 75) ea.x = 75;
        if (ea.x < -75) ea.x = -75;
        ea.y = 0;
        ea.z = 0;
        pivotTransform.localEulerAngles = ea;
        
    }

    public void AdjustDistance(float distance)
    {
        //cameraTransform.Translate(0, 0, distance * zoomSpeed);
        myCamera.fieldOfView -= distance * zoomSpeed; // 2017/12/15 Holmes
        float minFov = 15, maxFov = 45;
        if (myCamera.fieldOfView < minFov) myCamera.fieldOfView = minFov;
        if (myCamera.fieldOfView > maxFov) myCamera.fieldOfView = maxFov;
    }
}
