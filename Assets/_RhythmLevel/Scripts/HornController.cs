using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornController : MonoBehaviour {
    public Sprite HornLSprite;
    public Sprite HornRSprite;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitHorn(float lifespan)
    {
        Destroy(gameObject,lifespan);
    }
}
