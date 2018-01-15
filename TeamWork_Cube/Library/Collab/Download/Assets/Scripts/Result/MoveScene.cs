using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour {

    public Animator fade;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)&&SceneManager.GetActiveScene().name=="PlayScene")
        {
            ToScene("ResultScene");
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

    IEnumerator Fade(string sceneName)
    {
        if (fade != null)
        {
            fade.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1.0f);
        }
        SceneManager.LoadScene(sceneName);
    }
}
