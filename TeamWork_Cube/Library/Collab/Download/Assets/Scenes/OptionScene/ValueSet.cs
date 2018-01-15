using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueSet : MonoBehaviour {
    public Scrollbar BGMVolumeBar;
    public Scrollbar SEVolumeBar;
    public Text BGMVolumeText;
    public Text SEVolumeText;
    public string savadata;
    public string savedata_SE;

    public static float SEVolume; //現在のSE音量

    //SEの音量を変えた時に確認として音を出す
    //private bool OnClick;
    //private float BeforVol;
    //public AudioClip SelectSound;//選択SE

    //new AudioSource audio;

    void Start()
    {
        TitleSound.Default = false;
        BGMVolumeBar.value= PlayerPrefs.GetFloat(savadata);
        BGMVolumeText.text = ((int)(BGMVolumeBar.value * 100)).ToString() + "%";

        SEVolumeBar.value = PlayerPrefs.GetFloat(savadata);
        SEVolumeText.text = ((int)(SEVolumeBar.value * 100)).ToString() + "%";

        //audio = GetComponent<AudioSource>();
    }

    public void ValueChange()
    {
        BGMVolumeText.text = ((int)(BGMVolumeBar.value*100)).ToString()+"%";
        PlayerPrefs.SetFloat(savadata, BGMVolumeBar.value);
        SEVolumeText.text = ((int)(SEVolumeBar.value * 100)).ToString() + "%";
        PlayerPrefs.SetFloat(savedata_SE, SEVolumeBar.value);
    }

    void Update()
    {
        SEVolume = SEVolumeBar.value;
    }

    //private void SE_Check_befor()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        BeforVol = SEVolumeBar.value;
    //        OnClick = true;
    //    }
    //}

    //private void SE_Check_after()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        OnClick = false;
    //        if(SEVolumeBar.value != BeforVol)
    //        {
    //            audio.PlayOneShot(SelectSound);
    //        }
    //    }
    //}
}
