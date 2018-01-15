using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo_Script : MonoBehaviour {

    private Animator[] anims;
  
  
	void Start () {
        anims = GetComponentsInChildren<Animator>();
        StartCoroutine(LogoAnimation());
        Debug.Log(anims.Length);
	}
	
    IEnumerator LogoAnimation()
    {
        while (true)
        {
            //for (int i = 0; i < anims.Length; i++)
            //{
            //    if (0 == Random.Range(0, 5))
            //    {
            //         anims[i].SetTrigger("status");
            //    }

            //}
            anims[Random.Range(0,anims.Length)].SetTrigger("status");
            yield return new WaitForFixedUpdate();
            
        }
    }

	
}
