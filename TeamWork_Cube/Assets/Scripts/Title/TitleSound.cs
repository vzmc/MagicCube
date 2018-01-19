using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//タイトルシーンで常にアクティブなオブジェクトに
public class TitleSound : MonoBehaviour
{
    public static bool RotateSE_Play;     
    public static bool SelectSE_Play;
    public static bool BackSE_Play;  
    
    public static bool Default = true;

    public AudioClip RotateSound;//回転SE
    public AudioClip SelectSound;//選択SE
    public AudioClip BackSouund; //戻るボタンSE

    new AudioSource audio;

    // Use this for initialization
    void Start () {
        RotateSE_Play = false;
        SelectSE_Play = false;
        BackSE_Play = false;
        
        audio = GetComponent<AudioSource>();
        
        //オプションからの音量を受け取る
        //if (Default == false) audio.volume = ValueSet.SEVolume;
    }
	
	// Update is called once per frame
	void Update ()
    {
        PlaySE();
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
    private void PlaySE()
    {
        if (RotateSE_Play == true)
        {
            audio.PlayOneShot(RotateSound);
            RotateSE_Play = false;
        }
        if (SelectSE_Play == true)
        {
            audio.PlayOneShot(SelectSound);
            SelectSE_Play = false;
        }
        if(BackSE_Play == true)
        {
            audio.PlayOneShot(BackSouund);
            BackSE_Play = false;
        }
    }
}
