using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandle : MonoBehaviour
{
    [SerializeField]
    CameraController cameraController;
    [SerializeField]
    MagicCube magicCube;

    public FaceSelectIndicator faceSelectIndicator;
    public GameObject cellCursor;

    Transform selectTransform;
    Vector3 selectNormal;
    Vector3 distance;

    bool isSlider;
    int cellLayer;

    private void Start()
    {
        cellLayer = LayerMask.GetMask("Cell");
    }

    private void Update()
    {
        CameraControl();

        if (Input.GetButtonDown("Fire3"))
        {
            magicCube.CancelSelectCube();
        }

        // 1204何 PAUSE中スライドしない
        if(Time.timeScale != 0)
        {
            SliderHandle();
        }
    }

    private void Init()
    {
        selectTransform = null;
        isSlider = false;
    }

    /// <summary>
    /// カメラコントロール
    /// </summary>
    private void CameraControl()
    {
        if (Input.GetMouseButton(1))
        {
            cameraController.GetInput(Input.GetAxis("MouseX"), Input.GetAxis("MouseY"));
        }
        
        cameraController.AdjustDistance(Input.mouseScrollDelta.y);
    }

    /// <summary>
    /// Slider処理
    /// </summary>
    private void SliderHandle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        if (magicCube.IsChanging)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(ray, out raycastHit, 100.0f, LayerMask.GetMask("Cell")))
            {
                selectTransform = raycastHit.transform;
                selectNormal = raycastHit.normal;
            }
        }
        else if (Input.GetButton("Fire1"))
        {
            if (magicCube.IsSliding)
            {
                return;
            }

            if (selectTransform == null)
            {
                return;
            }

            //Debug.DrawRay(selectTransform.localPosition, selectNormal * 2, Color.cyan);

            if (Physics.Raycast(ray, out raycastHit, 100.0f, LayerMask.GetMask("Cell")))
            {
                //Debug.DrawRay(raycastHit.transform.localPosition, raycastHit.normal * 2, Color.yellow);
                if (selectTransform == raycastHit.transform)
                {
                    return;
                }

                if (selectNormal != raycastHit.normal)
                {
                    return;
                    //selectNormal = raycastHit.normal;
                }

                distance = raycastHit.transform.localPosition - selectTransform.localPosition;

                distance = DirectionHandle(distance);

                if (Vector3.Dot(distance, selectNormal) != 0)
                {
                    return;
                }

                magicCube.MakeBlockChainMove(selectTransform, distance, selectNormal);

                isSlider = true;
            }
            else
            {
                return;
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            //if (!isSlider)
            //{
            //    SelectHandle(Input.mousePosition, cellLayer);
            //}

            Init();
        }

        if (Physics.Raycast(ray, out raycastHit, 100.0f, LayerMask.GetMask("Cell")))
        {
            faceSelectIndicator.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            faceSelectIndicator.gameObject.SetActive(true);
            cellCursor.transform.position = raycastHit.collider.transform.position;
            cellCursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
            cellCursor.SetActive(true);
        }
        else
        {
            faceSelectIndicator.gameObject.SetActive(false);
            cellCursor.SetActive(false);
        }

    }

    private Vector3 DirectionHandle(Vector3 dir)
    {
        Vector3 vx = new Vector3(dir.x, 0, 0);
        Vector3 vy = new Vector3(0, dir.y, 0);
        Vector3 vz = new Vector3(0, 0, dir.z);

        float max = Mathf.Abs(dir.x);
        Vector3 direciton = vx;

        if (Mathf.Abs(dir.y) > max)
        {
            max = Mathf.Abs(dir.y);
            direciton = vy;
        }
        if (Mathf.Abs(dir.z) > max)
        {
            max = Mathf.Abs(dir.z);
            direciton = vz;
        }

        return direciton.normalized;
    }

    /// <summary>
    /// 選択処理
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="layerMask"></param>
    private void SelectHandle(Vector3 mousePosition, LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit raycastHit;

        if (!Physics.Raycast(ray, out raycastHit, 100.0f, layerMask))
        {
            return;
        }

        magicCube.SelectCube(raycastHit.transform, raycastHit.normal);
    }
}