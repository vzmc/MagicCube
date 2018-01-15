using UnityEngine;
using System.Collections;

public class TitleLowpolyGlow : MonoBehaviour
{
    public float glowIntensity = 1.0f;
    public float speed = 0.5f;
    private Renderer ren;
    private Material mat;
    private float phase = 0;
    public Texture2D[] emissionPatterns;
    // Use this for initialization
    void Start()
    {
        ren = GetComponent<Renderer>();
        mat = ren.material;
    }

    // Update is called once per frame
    void Update()
    {
        phase += Time.deltaTime * speed;
        while (phase > 1.0f)
        {
            phase -= 1.0f;
            mat.SetTexture("_EmissionMap", emissionPatterns[Random.Range(0, emissionPatterns.Length)]);
            //Swap the textures here!!
        }
        while (phase < 0) phase += 1.0f;
        float instIntensity = (-Mathf.Cos(Mathf.PI * 2 * phase) + 1) * glowIntensity / 2;
        mat.SetColor("_EmissionColor", Color.white * instIntensity);
    }
}
