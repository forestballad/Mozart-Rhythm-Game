using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour {
    public ParticleSystem PSPrefab;
    public ParticleSystem PS;
    bool isAlive;
    float lifespan = 0.5f;
    float counter;

    public Transform spawnPoint;
    public Transform vanishPoint;

	// Use this for initialization
	void Start () {
        isAlive = false;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestThing();
        }
		if (isAlive)
        {
            counter += Time.deltaTime;
            Vector3 loc = new Vector3(spawnPoint.position.x + (vanishPoint.position.x - spawnPoint.position.x) / lifespan * counter, spawnPoint.position.y + (vanishPoint.position.y - spawnPoint.position.y) / lifespan * counter);
            if (counter <= lifespan)
            {
                PS.transform.position = loc;
            }
        }
	}

    void TestThing()
    {
        PS = Instantiate(PSPrefab);
        PS.transform.position= new Vector3(spawnPoint.position.x,spawnPoint.position.y);
        isAlive = true;
        counter = 0;
    }
}
