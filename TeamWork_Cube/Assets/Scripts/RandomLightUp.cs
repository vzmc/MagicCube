using UnityEngine;
using System.Collections;

public class RandomLightUp : MonoBehaviour
{
    public enum LightUpState
    {
        Waiting,
        Onset,
        Release
    }
    public float onsetSpeed = 1.0f;
    public float releaseSpeed = 1.0f;
    public float minDelayTime = 0.0f;
    public float maxDelayTime = 10.0f;
    MeshRenderer meshRenderer;
    Material material;
    Texture2D baseTexture;

    LightUpState lightUpState;
    float currentBrightness = 0;
    float currentWaitTime = 0;
    float currentDelayLength = 0;

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        baseTexture = (Texture2D)material.mainTexture;
        RandomizeWaitTime();
        //Debug.Log(material.HasProperty("_Color"));
    }

    // Update is called once per frame
    void Update()
    {
        switch (lightUpState)
        {
            case (LightUpState.Waiting):
                currentWaitTime += Time.deltaTime;
                if(currentWaitTime > currentDelayLength)
                {
                    currentWaitTime = 0;
                    lightUpState = LightUpState.Onset;
                }
                break;
            case (LightUpState.Onset):
                currentBrightness += Time.deltaTime * onsetSpeed;
                if (currentBrightness > 1)
                {
                    currentBrightness = 1;
                    lightUpState = LightUpState.Release;
                }
                break;
            case (LightUpState.Release):
                currentBrightness -= Time.deltaTime * releaseSpeed;
                if (currentBrightness < 0)
                {
                    currentBrightness = 0;
                    RandomizeWaitTime();
                    lightUpState = LightUpState.Waiting;
                }
                break;
        }
        if (currentBrightness > 0)
        {
            if (!meshRenderer.enabled) meshRenderer.enabled = true;
            material.SetColor("_Color", Color.Lerp(Color.black, Color.white, currentBrightness));
        }
        else meshRenderer.enabled = false;
    }

    void RandomizeWaitTime()
    {
        currentDelayLength = Random.Range(minDelayTime, maxDelayTime);
    }
}
