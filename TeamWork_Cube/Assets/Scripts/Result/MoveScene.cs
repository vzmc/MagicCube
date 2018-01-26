using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour {

    public Animator fade;

    private GameLoad load;

	// Use this for initialization
	void Start ()
    {
        load = GetComponent<GameLoad>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.Space) && SceneManager.GetActiveScene().name == "PlayScene")
            {
                ToScene("ResultScene");
            }
        }
    }

    public void ToNextScene()
    {
        if(SceneManager.GetActiveScene().buildIndex<3)
        {
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % 3);
        }
    }
    
    public void ToScene(string sceneName)
    {
        //チュートリアルのビデオパネルがフェードした後残ってしまうから
        TutorialImageChange.OnVideo = false;

        StartCoroutine(Fade(sceneName));
    }

    public void ToLoad()
    {
        //チュートリアルのビデオパネルがフェードした後残ってしまうから
        TutorialImageChange.OnVideo = false;

        StartCoroutine(Fade_ToLoad());
    }

    IEnumerator Fade(string sceneName)
    {
        if (fade != null)
        {
            fade.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1.0f);
        }
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade_ToLoad()
    {
        if (fade != null)
        {
            fade.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1.0f);
        }
        load.LoadingStart();
    }
}
