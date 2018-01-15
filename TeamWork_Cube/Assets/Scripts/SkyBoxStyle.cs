using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkyBoxStyle : MonoBehaviour {
    
    [SerializeField]
    private Material skyBox;

    private Color skyBoxColor;
    [SerializeField]
    private float duration = 2.0f;
    void Start () {
        StartCoroutine(skyBox_Run());
        //skyBoxColor = skyBox.GetColorArray("_TintColor");
        StartCoroutine(skyBox_Color());
    }
	
    IEnumerator skyBox_Run()
    {
        float rotate = 0;
        while (true)
        {
            rotate = rotate+ Time.deltaTime >=360?0:rotate+Time.deltaTime;
            
            skyBox.SetFloat("_Rotation", rotate);
           
            yield return null;
        }
    }

	IEnumerator skyBox_Color()
    {
        while (true) {
            float t = 0;
            Color startColor = skyBox.GetColor("_Tint");
            Color endColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1);
            while (t<1)
            {
                t += Time.deltaTime*0.5f;
                skyBox.SetColor("_Tint", Color.Lerp(startColor, endColor, t));
                //Debug.Log(skyBox.GetColor("_Tint"));
                yield return null;
            }
            yield return new WaitForSeconds(3);
        }
    }
}
