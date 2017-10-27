using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFillingParticle : MonoBehaviour {
    public float m_Life;
    float lifespan = 0.5f;

    Vector3 SpawnLoc;
    Vector3 VanishLoc;

    // Use this for initialization
    void Start () {
        m_Life = 0;
	}

    public void Init(Vector3 spn, Vector3 vns)
    {
        SpawnLoc = spn;
        VanishLoc = vns;
    }

    // Update is called once per frame
    void Update () {
        m_Life += Time.deltaTime;
        Vector3 NewLoc = new Vector3(SpawnLoc.x + (VanishLoc.x - SpawnLoc.x) / lifespan * m_Life, SpawnLoc.y + (VanishLoc.y - SpawnLoc.y) / lifespan * m_Life);
        if (m_Life <= 5f)
        {
            transform.position = NewLoc;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
