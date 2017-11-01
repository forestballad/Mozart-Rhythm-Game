using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNoteController : MonoBehaviour {
    public Transform SpawnLoc;
    public Transform VanishLoc;
    float m_LifeSpan = 2f;
    float m_Timestamp = 0;

	// Use this for initialization
	void Start () {
        m_Timestamp = 0;
	}
	
	// Update is called once per frame
	void Update () {
        m_Timestamp += Time.deltaTime;
        transform.position = (SpawnLoc.position + (VanishLoc.position - SpawnLoc.position) * m_Timestamp / m_LifeSpan);
        if (transform.position.x < VanishLoc.position.x)
        {
            transform.position = SpawnLoc.position;
            m_Timestamp = 0;
        }
	}
}
