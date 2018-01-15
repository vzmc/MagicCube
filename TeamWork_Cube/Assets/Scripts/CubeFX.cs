using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFX : MonoBehaviour
{
    //private ParticleSystem[] ps;
    private Renderer rendererInstance;
    //private CubeCell cc;
    public VariableColourParticleFX FXPrefab;

    // Use this for initialization
    void Start()
    {
        rendererInstance = GetComponent<Renderer>();
        //cc = GetComponent<CubeCell>();
        //ps = GetComponentsInChildren<ParticleSystem>();
        //if (ps.Length == 0) ps = new ParticleSystem[] { gameObject.AddComponent<ParticleSystem>() };

    }

    void OnMatchEvent()
    {
        VariableColourParticleFX prefabInstance = Instantiate(FXPrefab, transform.position, transform.rotation);
        prefabInstance.TriggerFX(rendererInstance.material.color);
        //foreach (var s in ps)
        //{
        //    Color c = rendererInstance.material.color;
        //    ParticleSystem.MainModule mm = s.main;
        //    mm.startColor = c;
        //    s.Emit(10);
        //}
    }
}
