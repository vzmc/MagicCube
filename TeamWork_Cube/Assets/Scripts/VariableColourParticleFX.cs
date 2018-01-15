using UnityEngine;
using System.Collections;

public class VariableColourParticleFX : MonoBehaviour
{
    private ParticleSystem[] ps;

    void Awake()
    {
        ps = GetComponentsInChildren<ParticleSystem>();
        if (ps.Length == 0) ps = new ParticleSystem[] { gameObject.AddComponent<ParticleSystem>() };
    }

    public void TriggerFX(Color colour)
    {
        foreach (var s in ps)
        {
            Color c = colour;
            ParticleSystem.MainModule mm = s.main;
            mm.startColor = c;
            //s.Emit(10);
            s.Play();
        }
    }
}
