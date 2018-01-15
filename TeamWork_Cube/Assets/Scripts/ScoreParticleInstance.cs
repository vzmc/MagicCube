using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreParticleInstance : MonoBehaviour {
    public Transform target;

    private Vector3 direction;
    private Image uiImage;

    private void Awake()
    {
        uiImage = GetComponent<Image>();
    }

    // Use this for initialization
    void Start () {
        //uiImage = GetComponent<Image>();
        direction = Random.onUnitSphere;
        Destroy(gameObject, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null) Destroy(gameObject);
        float timeDelta = Time.unscaledDeltaTime;
        transform.position += direction * timeDelta * 1000;
        Vector3 deltaPos = target.position - transform.position;
        direction = Vector3.Lerp(direction, (target.position - transform.position).normalized, timeDelta * 5);

        if (deltaPos.magnitude < 100) transform.localScale = Vector3.one * deltaPos.magnitude / 100;
        if (deltaPos.magnitude < 10) Destroy(gameObject);
	}

    public void SetColour(Color targetColour)
    {
        uiImage.color = targetColour;
    }
}
