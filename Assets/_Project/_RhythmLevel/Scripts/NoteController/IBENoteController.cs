using System;
using UnityEngine;

public class IBENoteController : INoteController
{
	[SerializeField]
    private Sprite LNote;
	[SerializeField]
	private Sprite LNoteHit;
	[SerializeField]
	private Sprite RNote;
	[SerializeField]
	private Sprite RNoteHit;
	[SerializeField]
	private Sprite BNote;
	[SerializeField]
	private Sprite BNoteHit;

	[SerializeField]
	private Sprite hitSprite;
	[SerializeField]
	private Sprite badSprite;

    private float m_Timestamp;
    string m_notetype;
    float m_lifespan;

    float m_currentTimestamp;

	float hitThreshold = 0.12f;
	float HitGoodThreshold = 0.1f;
    float HitPerfectThreshold = 0.05f;

    public GameObject vanishPoint;
    public GameObject spawnPoint;

    public GameObject GameLogic;

    Vector3 m_SpawnLoc;
    Vector3 m_VanishLoc;

	public override float MissTimestamp
	{
		get { return m_Timestamp + hitThreshold; }
	}

	public override float ActiveTimestamp
	{
		get { return m_Timestamp - hitThreshold; }
	}

    // Use this for initialization

    void Start () {
        vanishPoint = GameObject.Find("NoteEndLoc");
        spawnPoint = GameObject.Find("NoteStartLoc");
        GameLogic = GameObject.Find("GameLogic");

        m_SpawnLoc = spawnPoint.transform.position;
        m_VanishLoc = vanishPoint.transform.position;

        GameLogic.GetComponent<RhythmLevelController>().OnUpdateView += OnUpdateView;
    }

    // Update is called once per frame
    void Update () {
        
    }

    private void OnUpdateView(object sender, EventArgs e)
    {
		m_currentTimestamp = (sender as RhythmLevelController).GetMusicTime();
		transform.position = GetLocByTimetamp();
	}

    public override void Init(float timestamp, string inputNoteType, float lifespan)
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

	private Vector3 GetLocByTimetamp()
    {
        float dist = Vector3.Distance(m_SpawnLoc, m_VanishLoc) * (m_Timestamp - m_currentTimestamp) / m_lifespan;
        Vector3 NoteLoc = new Vector3(m_VanishLoc.x + dist, m_VanishLoc.y, m_VanishLoc.z);
        return NoteLoc;
    }

    public override RhythmLevelController.NoteResultEventArgs GetHit(string hitType, float timeDiff)
    {
        if (hitType == "miss")
        {
            return MissNote();
        }
        if ((hitType == "1" && m_notetype == "0") || (hitType == "0" && m_notetype == "1"))
        {
            return WrongNotePlayed();
        }
        bool halfScore = false;
        if ((hitType == "1" || hitType == "0" )&& m_notetype == "2")
        {
            halfScore = true;
        }
        if (timeDiff > HitGoodThreshold)
        {
            return BadNotePlayed();
        }
        else if (timeDiff <= HitPerfectThreshold)
        {
            return PerfectNotePlayed(halfScore);
        }
        else
        {
            return GoodNotePlayed(halfScore);
        }
    }

    private RhythmLevelController.NoteResultEventArgs PerfectNotePlayed(bool halfbaseScore)
    {
        GetComponent<SpriteRenderer>().sprite = hitSprite;
        var args = new RhythmLevelController.NoteResultEventArgs
        {
            halfBaseScore = halfbaseScore,
            resultType = "PERFECT"
        };
        return args;
    }

	private RhythmLevelController.NoteResultEventArgs GoodNotePlayed(bool halfbaseScore)
    {
        GetComponent<SpriteRenderer>().sprite = hitSprite;
        var args = new RhythmLevelController.NoteResultEventArgs
        {
            halfBaseScore = halfbaseScore,
            resultType = "GOOD"
        };
        return args;
    }

	private RhythmLevelController.NoteResultEventArgs BadNotePlayed()
    {
        GetComponent<SpriteRenderer>().sprite = badSprite;

        var args = new RhythmLevelController.NoteResultEventArgs
        {
            resultType = "BAD"
        };
        return args;
    }

	private RhythmLevelController.NoteResultEventArgs MissNote()
    {
        var args = new RhythmLevelController.NoteResultEventArgs
        {
            resultType = "MISS"
        };
        return args;
    }

	private RhythmLevelController.NoteResultEventArgs WrongNotePlayed()
    {
        GetComponent<SpriteRenderer>().sprite = badSprite;

        var args = new RhythmLevelController.NoteResultEventArgs
        {
            resultType = "WRONG"
        };
        return args;
    }
}
