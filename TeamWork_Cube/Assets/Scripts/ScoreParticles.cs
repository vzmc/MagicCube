using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreParticles : MonoBehaviour {
    public ScoreParticleInstance ParticlePrefab;
    public GameObject ScoreGauge;
    Canvas currentCanvas;
	// Use this for initialization
	void Start () {
        currentCanvas = GetComponentInParent<Canvas>();
        CubeCell.OnCubeMatch += OnCubeMatch;
	}

    void OnDestroy()
    {
        CubeCell.OnCubeMatch -= OnCubeMatch;
    }

    void OnCubeMatch(CubeCell matchedCube)
    {
        Vector3 CubePositionOnScreen;
        CubePositionOnScreen = Camera.main.WorldToScreenPoint(matchedCube.transform.position);
        ParticlePrefab.target = ScoreGauge.transform;
        //ParticlePrefab.GetComponent<Renderer>().material.color = matchedCube.GetMaterial().color;
        //ParticlePrefab.SetColour(matchedCube.GetMaterial().color);
        for (int i = 0; i < Random.Range(1, 10); i++)
        {
            ScoreParticleInstance particleInstance = Instantiate(ParticlePrefab, CubePositionOnScreen, Quaternion.identity, transform.parent);
            particleInstance.SetColour(matchedCube.GetMaterial().color);
            Destroy(particleInstance.gameObject, 1);
        }
        //particleInstance.target = ScoreGauge.transform;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
