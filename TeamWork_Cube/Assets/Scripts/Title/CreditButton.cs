using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditButton : MonoBehaviour {

    public Animator anim;
    public Animator fade;
	// Use this for initialization
	void Start () {
        StartCoroutine("AnimFlag");
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public void OnClick()
    {
        fade.SetTrigger("FadeOut");
        StartCoroutine(MoveScene());
    }

    IEnumerator AnimFlag()
    {
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("Flag");
    }
    IEnumerator MoveScene()
    {
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("Title");
    }
}
