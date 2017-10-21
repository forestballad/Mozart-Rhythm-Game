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

    public string notetype;
    public ParticleSystem hitParticleGood;
    public ParticleSystem hitParticlePerfect;

    public int NoteOrder;
    public bool CatchHit;
    public bool Hittable;
    float m_Timestamp;
    float m_currentTimestamp;

    public float hitPerfectCheck = 0.4f;

    public GameObject vanishPoint;
    public GameObject spawnPoint;
    public GameObject GameLogic;

    Camera MainCamera;

    Vector3 m_SpawnLoc;
    Vector3 m_VanishLoc;
    public float speed;

    // Use this for initialization

    void Start () {
        vanishPoint = GameObject.Find("NoteEndLoc");
        spawnPoint = GameObject.Find("NoteStartLoc");
        GameLogic = GameObject.Find("GameLogic");
        CatchHit = false;
        Hittable = true;

        GetComponent<SpriteRenderer>().enabled = false;

        m_SpawnLoc = spawnPoint.transform.position;
        m_VanishLoc = vanishPoint.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        m_currentTimestamp = GameLogic.GetComponent<RhythmLevelController>().GetCurrentTimestamp();
        transform.position = GetCurrentLocByTimetamp();

        if (transform.position.x < m_SpawnLoc.x)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }

        if (vanishPoint.transform.position.x - transform.position.x > GameLogic.GetComponent<RhythmLevelController>().hitRangeCheck && !CatchHit)
        {
            Hittable = false;
            GameLogic.GetComponent<RhythmLevelController>().BreakCombo(false);
        }
        if (transform.position.x < -10)
        {
            Destroy(gameObject);
        }
    }

    public void Init(string noteType, float timestamp, float lsp)
    {
        m_Timestamp = timestamp;
        speed = lsp;
        if (noteType == "left")
        {
            notetype = "left";
            GetComponent<SpriteRenderer>().sprite = LNote;
            hitSprite = LNoteHit;

        }
        else if(noteType == "right")
        {
            notetype = "right";
            GetComponent<SpriteRenderer>().sprite = RNote;
            hitSprite = RNoteHit;
        }
        else if (noteType == "both")
        {
            notetype = "both";
            GetComponent<SpriteRenderer>().sprite = BNote;
            hitSprite = BNoteHit;
        }
    }

    public Vector3 GetCurrentLocByTimetamp()
    {
        float dist = Vector3.Distance(m_SpawnLoc, m_VanishLoc) * (m_Timestamp - m_currentTimestamp) / speed;
        Vector3 NoteLoc = new Vector3(m_VanishLoc.x + dist, m_VanishLoc.y, m_VanishLoc.z);
        return NoteLoc;
    }

    public void GetHit(string hitType, float hitRange)
    {
        Hittable = false;
        CatchHit = true;
        if ((hitType == "left" || hitType == "both") && notetype == "left")
        {
            if (hitRange <= hitPerfectCheck) {
                PerfectNotePlayed(false);
            }
            else {
                GoodNotePlayed(false);
            }
        }
        else if ((hitType == "right" || hitType == "both") && notetype == "right")
        {
            if (hitRange <= hitPerfectCheck)
            {
                PerfectNotePlayed(false);
            }
            else
            {
                GoodNotePlayed(false);
            }
        }
        else if (hitType == "right" && notetype == "left")
        {
            WrongNotePlayed();
        }
        else if (hitType == "left" && notetype == "right")
        {
            WrongNotePlayed();
        }
        else if ((hitType == "left" || hitType == "right") && notetype == "both")
        {
            if (hitRange <= hitPerfectCheck)
            {
                PerfectNotePlayed(true);
            }
            else
            {
                GoodNotePlayed(true);
            }
        }
        else if (hitType == "both" && notetype == "both")
        {
            if (hitRange <= hitPerfectCheck)
            {
                PerfectNotePlayed(false);
            }
            else
            {
                GoodNotePlayed(false);
            }
        }
    }

    public void PerfectNotePlayed(bool halfbaseScore)
    {
        GetComponent<SpriteRenderer>().sprite = hitSprite;
        GameLogic.GetComponent<RhythmLevelController>().AddScore("PERFECT", halfbaseScore);

        ParticleSystem ps = Instantiate(hitParticlePerfect);
        ps.transform.position = vanishPoint.transform.position;
        ps.Play();
    }

    public void GoodNotePlayed(bool halfbaseScore)
    {
        GetComponent<SpriteRenderer>().sprite = hitSprite;
        GameLogic.GetComponent<RhythmLevelController>().AddScore("GOOD",halfbaseScore);

        ParticleSystem ps = Instantiate(hitParticleGood);
        ps.transform.position = vanishPoint.transform.position;
        ps.Play();
    }

    public void WrongNotePlayed()
    {
        GameLogic.GetComponent<RhythmLevelController>().BreakCombo(true);
    }
}
