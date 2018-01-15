using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSound : MonoBehaviour {
    public AudioClip matchSound;
    public AudioClip respawnSound;
    public AudioClip movementSound;
    public float pitchVariation = 0.1f;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
	}

    void OnMatchEvent()
    {
        PlayMatchSound();
    }

    void OnRespawnEvent()
    {
        PlayRespawnSound();
    }

    void PlayMatchSound()
    {
        RandomizePitch();
        audioSource.clip = matchSound;
        audioSource.Play();
    }

    public void OnMovement()
    {
        audioSource.pitch = 1;
        audioSource.clip = movementSound;
        audioSource.Play();
    }

    void PlayRespawnSound()
    {
        RandomizePitch();
        audioSource.clip = respawnSound;
        audioSource.Play();
    }

    void RandomizePitch()
    {
        float highBoundary = 1 + pitchVariation;
        float lowBoundary = 1 / highBoundary;
        audioSource.pitch = Random.Range(lowBoundary, highBoundary);
    }
}
