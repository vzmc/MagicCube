using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSelectIndicator : MonoBehaviour {
    public GameObject panelPrefab;

    private int cubeLevel;
	// Use this for initialization
	void Start () {
        cubeLevel = GameManager.Instance.MagicCubeLevel;
        transform.position = Vector3.one * (cubeLevel / 2 - 0.5f);
        for(int x = 0; x < cubeLevel; x++)
        {
            for(int y = 0; y < cubeLevel; y++)
            {
                // Add the panels.
                GameObject panelInstance = Instantiate(panelPrefab, transform);
                panelInstance.transform.Translate(new Vector3((-cubeLevel / 2) + 0.5f + x, cubeLevel / 2, (-cubeLevel / 2) + 0.5f + y));
                panelInstance.transform.Rotate(new Vector3(-90, 0, 0));
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
