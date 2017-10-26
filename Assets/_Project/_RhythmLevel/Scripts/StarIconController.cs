using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarIconController : MonoBehaviour {
    public int m_HitToFill;
    public int m_CurrentHitReceived;
    public Color32 InitColor = new Color32(255,255,255,255);
    public Color32 TargetColor = new Color32(255,231,145,255);
    public Color32 CurrentColor;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(int HitToFill)
    {
        m_CurrentHitReceived = 0;
        m_HitToFill = HitToFill;
    }

    public bool ReceiveHit()
    {
        m_CurrentHitReceived++;
        int newG = (InitColor.g + (TargetColor.g - InitColor.g) / m_HitToFill * m_CurrentHitReceived);
        int newB = (InitColor.b + (TargetColor.b - InitColor.b) / m_HitToFill * m_CurrentHitReceived);
        CurrentColor = new Color32(255, (byte)newG, (byte)newB,255);
        gameObject.GetComponent<SpriteRenderer>().color = CurrentColor;

        if (m_CurrentHitReceived == m_HitToFill)
        {
            GetComponent<Animator>().SetBool("IsActive", true);
            return false;
        }
        else
        {
            return true;
        }
    }
}
