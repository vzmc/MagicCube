using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    public AudioClip musicToPlay;
    static MusicPlayer instance;
    AudioSource audioSource;

    private void Awake()
    {

        audioSource = GetComponent<AudioSource>();
        //if (!audioSource.isPlaying || audioSource.clip != musicToPlay)
        //{
        //    PlayMusic();
        //}
        if (instance == null)
        {
            instance = this;
            instance.PlayMusic();
            DontDestroyOnLoad(this);
        }
        else
        {
            if (musicToPlay != null) instance.ChangeMusic(musicToPlay);
            Destroy(gameObject);
        }
    }

    // Use this for initialization
	void Start () {
        //Debug.Log("Start Called.");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeMusic(AudioClip nextMusic)
    {
        if (musicToPlay == nextMusic) return;
        musicToPlay = nextMusic;
        audioSource.Stop();
        PlayMusic();
    }

    public void PlayMusic()
    {
        audioSource.clip = musicToPlay;
        audioSource.Play();
    }
}
