using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {
    public Sprite LNote;
    public Sprite LNoteHit;
    public Sprite RNote;
    public Sprite RNoteHit;
    public Sprite BNote;
    public Sprite BNoteHit;

    public Sprite hitSprite;
    public Sprite badSprite;

    public float m_Timestamp;
    string m_notetype;
    float m_lifespan;

    float m_currentTimestamp;

    float HitGoodThreshold = 0.1f;
    float HitPerfectThreshold = 0.05f;

    public GameObject vanishPoint;
    public GameObject spawnPoint;

    public GameObject GameLogic;

    Camera MainCamera;

    Vector3 m_SpawnLoc;
    Vector3 m_VanishLoc;


   

    // Use this for initialization

    void Start () {
        vanishPoint = GameObject.Find("NoteEndLoc");
        spawnPoint = GameObject.Find("NoteStartLoc");
        GameLogic = GameObject.Find("GameLogic");

        m_SpawnLoc = spawnPoint.transform.position;
        m_VanishLoc = vanishPoint.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        m_currentTimestamp = GameLogic.GetComponent<RhythmLevelController>().GetCurrentTimestamp();
        transform.position = GetLocByTimetamp();
    }

    public void Init(float timestamp, string inputNoteType, float lifespan)
    {
        m_Timestamp = timestamp;
        m_lifespan = lifespan;
        if (inputNoteType == "0")
        {
            m_notetype = "0";
            GetComponent<SpriteRenderer>().sprite = LNote;
            hitSprite = LNoteHit;
        }
        else if(inputNoteType == "1")
        {
            m_notetype = "1";
            GetComponent<SpriteRenderer>().sprite = RNote;
            hitSprite = RNoteHit;
        }
        else if (inputNoteType == "2")
        {
            m_notetype = "2";
            GetComponent<SpriteRenderer>().sprite = BNote;
            hitSprite = BNoteHit;
        }
    }

    public Vector3 GetLocByTimetamp()
    {
        float dist = Vector3.Distance(m_SpawnLoc, m_VanishLoc) * (m_Timestamp - m_currentTimestamp) / m_lifespan;
        Vector3 NoteLoc = new Vector3(m_VanishLoc.x + dist, m_VanishLoc.y, m_VanishLoc.z);
        return NoteLoc;
    }

    public void GetHit(string hitType, float timeDiff)
    {
        if (hitType == "3")
        {
            BadNotePlayed();
            return;
        }
        if ((hitType == "1" && m_notetype == "0") || (hitType == "0" && m_notetype == "1"))
        {
            WrongNotePlayed();
            return;
        }
        bool halfScore = false;
        if ((hitType == "1" || hitType == "0" )&& m_notetype == "2")
        {
            halfScore = true;
        }
        if (timeDiff > HitGoodThreshold)
        {
            BadNotePlayed();
        }
        else if (timeDiff <= HitPerfectThreshold)
        {
            PerfectNotePlayed(halfScore);
        }
        else
        {
            GoodNotePlayed(halfScore);
        }
    }

    public void PerfectNotePlayed(bool halfbaseScore)
    {
        GetComponent<SpriteRenderer>().sprite = hitSprite;
        GameLogic.GetComponent<IchBinExtraordinarStageControl>().NoteSuccess("PERFECT", halfbaseScore);
    }

    public void GoodNotePlayed(bool halfbaseScore)
    {
        GetComponent<SpriteRenderer>().sprite = hitSprite;
        GameLogic.GetComponent<IchBinExtraordinarStageControl>().NoteSuccess("GOOD", halfbaseScore);
    }

    public void BadNotePlayed()
    {
        GetComponent<SpriteRenderer>().sprite = badSprite;
        GameLogic.GetComponent<IchBinExtraordinarStageControl>().NoteFail("BAD");
    }

    public void WrongNotePlayed()
    {
        GetComponent<SpriteRenderer>().sprite = badSprite;
        GameLogic.GetComponent<IchBinExtraordinarStageControl>().NoteFail("WRONG");
    }
}
