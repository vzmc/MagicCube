using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private bool stopTime = false;

    public GameObject ui;   //表示するUI
    public GameObject mainCube;

    private List<GameObject> objList = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePause();
        }
	}

    public void GamePause()
    {
        if (!stopTime)
        {
            ui.SetActive(true);
            CubeDisplay(false);
            Time.timeScale = 0;
        }
        else
        {
            ui.SetActive(false);
            CubeDisplay(true);
            Time.timeScale = 1;
        }
        
        stopTime = !stopTime;
    }

    public void YesButtonPressed()
    {
        GamePause();
        GetComponent<MoveScene>().ToScene("Title");
    }

    public void NoButtonPressed()
    {
        GamePause();
    }

    private void CubeDisplay(bool isDisplay)
    { 
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("MainOBJinPlayScene");

        objList.AddRange(tempArray);

        if (!isDisplay)
        {
            foreach (var obj in tempArray)
            {
                obj.GetComponent<MeshRenderer>().enabled = false;
            }

        }
        else
        {
            foreach (var obj in tempArray)
            {
                obj.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }



}
