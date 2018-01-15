using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private bool stopTime = false;
    public GameObject ui;   //表示するUI

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePause(true);
        }
	}

    private void GamePause(bool destoryUI)
    {
        if (!stopTime)
        {
            ui.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            if (destoryUI)
            {
                ui.SetActive(false);
            }
            Time.timeScale = 1;
        }
        stopTime = !stopTime;
    }

    public void YesButtonPressed()
    {
        GamePause(false);
        GetComponent<MoveScene>().ToScene("Title");
    }

    public void NoButtonPressed()
    {
        GamePause(true);
    }

}
