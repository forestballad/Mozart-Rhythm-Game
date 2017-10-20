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

    public GameObject vanishPoint;
    public GameObject gameLogic;

    public float speed;
    public string notetype;
    public ParticleSystem hitParticleGood;
    public ParticleSystem hitParticlePerfect;

    public int NoteOrder;
    public bool CatchHit;
    public bool Hittable;
    public float TimeStamp;

    public float hitPerfectCheck = 0.4f;

    // Use this for initialization

    void Start () {
        vanishPoint = GameObject.Find("NoteEndLoc");
        gameLogic = GameObject.Find("GameLogic");
        CatchHit = false;
        Hittable = true;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
        if (vanishPoint.transform.position.x - transform.position.x > gameLogic.GetComponent<RhythmLevelController>().hitRangeCheck && !CatchHit)
        {
            Hittable = false;
            gameLogic.GetComponent<RhythmLevelController>().BreakCombo();
        }
        if (transform.position.x < -10)
        {
            Destroy(gameObject);
        }
    }

    public void Init(string noteType, GameObject loc, int order, float TS)
    {
        transform.position = loc.transform.position;
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
        gameLogic.GetComponent<RhythmLevelController>().AddScore("PERFECT", halfbaseScore);

        ParticleSystem ps = Instantiate(hitParticlePerfect);
        ps.transform.position = vanishPoint.transform.position;
        ps.Play();
    }

    public void GoodNotePlayed(bool halfbaseScore)
    {
        GetComponent<SpriteRenderer>().sprite = hitSprite;
        gameLogic.GetComponent<RhythmLevelController>().AddScore("GOOD",halfbaseScore);

        ParticleSystem ps = Instantiate(hitParticleGood);
        ps.transform.position = vanishPoint.transform.position;
        ps.Play();
    }

    public void WrongNotePlayed()
    {
        gameLogic.GetComponent<RhythmLevelController>().BreakCombo();
    }
}
