using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IchBinExtraordinarStageControl : MonoBehaviour {
    public RhythmLevelController h_SpecificLevelController;
    float m_TimeStamp;
    bool m_IsActing;

    // These are for sprite character animation control
    public List<GameObject> SimpleAnimatedCharacter;
    public GameObject Crowd;
    public GameObject Arco;
    public GameObject AmadeDrum;
    public GameObject MozartGuitar;

    // Cacilia Horn
    public GameObject CaciliaHorn;

    public GameObject StageLight;

    class AnimInstruction
    {
        public int counter;
        public List<float> TimeStamp;
        public List<string> Instruction;
    }
    AnimInstruction StageAnimationInstruction;

    // Combo / Score / Note Result would be specifict to stage
    public GameObject NoteResultText;
    public float NRT_DisplayTime = 0.2f;
    float m_NRT_Vanish_Timestamp;
    string m_LastValidHitType;

    int m_Score;
    public GameObject ScoreText;

    int m_Combo;
    public GameObject ComboText;
    public GameObject ArcoAngryBar;
    public GameObject ArcoAngryEffect;

    // These are for star filling effect
    int m_TotalNoteNum;
    int m_CurrentStar;
    public List<GameObject> StarIcons;

    // Display while playing record
    public GameObject RecText;

    // Game Result Window
    public GameObject ResultDisplayWindow;
    public GameObject CreditWindow;

    public ParticleSystem hitParticleGood;
    public ParticleSystem hitParticlePerfect;
    public ParticleSystem starFillingEffect;

    public GameObject StarFillingSpawnLoc;

    public List<Sprite> Endings;

    public GameObject DebugHitTypeText;
    int DebugTotalHit;
    int DebugPerfectHit;

    // Use this for initialization
    void Start () {
        m_IsActing = false;
        StageAnimationInstruction = new AnimInstruction();
        StageAnimationInstruction.counter = 0;
        StageAnimationInstruction.Instruction = new List<string>() { "AnimMusicBegin", "AnimDrumIn", "AnimVocalIn" ,"AnimMusicPause", "AnimMusicBack","AnimMusicEnd" };
        StageAnimationInstruction.TimeStamp = new List<float>() {0,4.6f,10.4f,87.5f,98,119 };

        h_SpecificLevelController.OnEntryPlaying += OnEntryPlaying;
        h_SpecificLevelController.OnEntryRecord += OnEntryRecord;
        h_SpecificLevelController.OnEntryResult += OnEntryResult;
        h_SpecificLevelController.OnUpdateView += OnUpdateView;
	    h_SpecificLevelController.OnNoteResult += NoteResult;
    }

    private void OnEntryPlaying(object sender, EventArgs e)
    {
        BeginActing(false);
    }

    private void OnEntryRecord(object sender, EventArgs e)
    {
        BeginActing(true);
    }

    private void OnEntryResult(object sender, EventArgs e)
    {
        EndActing();
    }

    private void OnUpdateView(object sender, EventArgs e)
    {
        ManualUpdate();
    }

    private void BeginActing(bool PlayingRecord)
    {
        CaciliaHorn.GetComponent<CaciliaController>().StartActing();
        StageLight.SetActive(false);
        m_IsActing = true;
        m_Score = 0;
        m_Combo = 0;
        m_TotalNoteNum = h_SpecificLevelController.GetTotalNoteNumber();
        m_LastValidHitType = "";
        NoteResultText.GetComponent<Text>().text = "";
        StageAnimationInstruction.counter = 0;
        InitStars();
        if (PlayingRecord)
        {
            RecText.SetActive(true);
        }

        DebugTotalHit = 0;
        DebugPerfectHit = 0;
    }

    public void EndActing()
    {
        CreditWindow.SetActive(false);
        m_IsActing = false;
        GameObject.Find("GameResultScoreText").GetComponent<Text>().text = m_Score.ToString();
        GameObject.Find("GameResultCG").GetComponent<Image>().sprite = Endings[m_CurrentStar];
        RecText.SetActive(false);
    }

	void Update () {
       
	}

    public void ManualUpdate()
    {
        if (m_IsActing)
        {
            SyncDataWithGameLogic();
            if (StageAnimationInstruction.counter < 6 && StageAnimationInstruction.TimeStamp[StageAnimationInstruction.counter] < m_TimeStamp)
            {
                Invoke(StageAnimationInstruction.Instruction[StageAnimationInstruction.counter], 0);
                StageAnimationInstruction.counter++;
            }
            UpdateScoreAndCombo();
        }
    }

    void SyncDataWithGameLogic()
    {
        m_TimeStamp = h_SpecificLevelController.GetMusicTime();
        m_NRT_Vanish_Timestamp = h_SpecificLevelController.GetLastValidHitTimestamp() + NRT_DisplayTime;
    }

    void UpdateScoreAndCombo()
    {
        Vector3 ArcoAngryBarScale = new Vector3(22f/50f*m_Combo, 1f, 1f);
        if (ArcoAngryBarScale.x <= 22)
        {
            ArcoAngryBar.transform.localScale = ArcoAngryBarScale;
        }
        else
        {
            ArcoAngryBar.transform.localScale = new Vector3(22f, 1f, 1f);
        }

        if (m_Combo >= 50 && ArcoAngryEffect.GetComponent<Animator>().GetInteger("AngryLevel") != 1)
        {
            ArcoAngryEffect.GetComponent<Animator>().SetInteger("AngryLevel",1);
        }
        else if (m_Combo < 50 && ArcoAngryEffect.GetComponent<Animator>().GetInteger("AngryLevel") != 0)
        {
            ArcoAngryEffect.GetComponent<Animator>().SetInteger("AngryLevel", 0);
        }

        ScoreText.GetComponent<Text>().text = "SCORE:" + m_Score.ToString();
        ComboText.GetComponent<Text>().text = "COMBO:" + m_Combo.ToString();

        if (m_LastValidHitType != "")
        {
            NoteResultText.GetComponent<Text>().text = m_LastValidHitType + "!";
        }

        Color32 HRTColor = new Color32(255,255,255,255);
        switch (m_LastValidHitType)
        {
            case "PERFECT":
                HRTColor = new Color32(255,231,50,255);
                break;
            case "GOOD":
                HRTColor = new Color32(255, 255, 255, 255);
                break;
            case "BAD":
                HRTColor = new Color32(211, 212, 217,255);
                break;
            case "WRONG":
                HRTColor = new Color32(255, 0, 0,255);
                break;
            default:
                break;
        }
        NoteResultText.GetComponent<Text>().color = HRTColor;

        if (m_TimeStamp > m_NRT_Vanish_Timestamp)
        {
            NoteResultText.SetActive(false);
        }
        else
        {
            NoteResultText.SetActive(true);
        }

        DebugHitTypeText.GetComponent<Text>().text = "Perfect/Total: "+DebugPerfectHit + "/" + DebugTotalHit;
    }

    #region NoteResult

    private void NoteResult(System.Object sender, RhythmLevelController.NoteResultEventArgs e)
    {
        switch (e.resultType)
        {
	        case "PERFECT":
	        case "GOOD":
		        NoteSuccess(sender, e);
		        break;
	        case "BAD":
	        case "MISS":
	        case "WRONG":
		        NoteFail(sender, e);
		        break;
		}
    }

    private void NoteSuccess(System.Object sender, RhythmLevelController.NoteResultEventArgs e)
    {
        string hitType = e.resultType;
        bool halfbaseScore = e.halfBaseScore;
        int baseScore = (10 + GetComboMultiplier()) * 100;
        m_LastValidHitType = hitType;
        m_Combo++;
        if (hitType == "GOOD")
        {
            ParticleSystem ps = Instantiate(hitParticleGood);
            ps.transform.position = GameObject.Find("NoteEndLoc").transform.position;
            ps.Play();
        }
        else if (hitType == "PERFECT")
        {
            baseScore *= 2;

            ParticleSystem ps = Instantiate(hitParticlePerfect);
            ps.transform.position = GameObject.Find("NoteEndLoc").transform.position;
            ps.Play();

            if (m_CurrentStar < 3)
            {
                FillStar();
            }
            DebugPerfectHit++;
        }
        if (halfbaseScore)
        {
            m_Score += baseScore / 2;
        }
        else
        {
            m_Score += baseScore;
        }
        DebugTotalHit++;
    }

    private void NoteFail(System.Object sender, RhythmLevelController.NoteResultEventArgs e)
    {
        string hitType = e.resultType;
        if (hitType == "BAD")
        {
            m_Score += 5 * 100;
        }
        else if (hitType == "WRONG")
        {
            m_Score -= 5 * 100;
        }
        m_LastValidHitType = hitType;
        m_Combo = 0;
        DebugTotalHit++;
    } 

    #endregion

    int GetComboMultiplier()
    {
        if (m_Combo >= 100)
        {
            return 10;
        }
        else if (m_Combo >= 50)
        {
            return 5;
        }
        else if (m_Combo >= 10)
        {
            return 3;
        }
        else
        {
            return 2;
        }
    }

    void InitStars()
    {
        int starHit = Mathf.FloorToInt(m_TotalNoteNum * 0.8f / 3);
        m_CurrentStar = 0;

        for (int i = 0; i < 2; i++)
        {
            StarIcons[0].GetComponent<StarIconController>().Init(starHit);
            StarIcons[1].GetComponent<StarIconController>().Init(starHit);
            StarIcons[2].GetComponent<StarIconController>().Init(starHit);
        }

        for (int i = 0; i < 3; i++)
        {
            StarIcons[i].GetComponent<SpriteRenderer>().color = Color.white;
            StarIcons[i].GetComponent<Animator>().SetBool("IsActive", false);
        }
    }

    void FillStar()
    {
        ParticleSystem ps = Instantiate(starFillingEffect);
        ps.GetComponent<StarFillingParticle>().Init(StarFillingSpawnLoc.transform.position, StarIcons[m_CurrentStar].transform.position);
        ps.Play();
        if (!StarIcons[m_CurrentStar].GetComponent<StarIconController>().ReceiveHit())
        {
            m_CurrentStar++;
        }
    }

    public void OpenCreditWindow()
    {
        CreditWindow.SetActive(true);
    }

    public void CloseCreditWindow()
    {
        CreditWindow.SetActive(false);
    }

    #region Anim

    void AnimMusicBegin()
    {
        foreach (GameObject item in SimpleAnimatedCharacter)
        {
            item.GetComponent<Animator>().SetBool("IsActive", true);
        }
        Arco.SetActive(true);
        Arco.GetComponent<Animator>().SetBool("IsActive", true);
        Arco.GetComponent<Animator>().SetInteger("Intensity", 0);
        Crowd.GetComponent<Animator>().SetBool("IsActive", true);
        Crowd.GetComponent<Animator>().SetInteger("Intensity", 0);
        AmadeDrum.GetComponent<Animator>().SetBool("IsActive", true);
        AmadeDrum.GetComponent<Animator>().SetInteger("Intensity", 0);
        CaciliaHorn.GetComponent<Animator>().SetBool("IsActive", true);
    }

    void AnimDrumIn()
    {
        AmadeDrum.GetComponent<Animator>().SetInteger("Intensity", 1);
        MozartGuitar.GetComponent<Animator>().SetBool("IsActive", true);
        MozartGuitar.GetComponent<Animator>().SetInteger("Intensity", 0);
        StageLight.SetActive(true);
    }

    void AnimVocalIn()
    {
        MozartGuitar.GetComponent<Animator>().SetInteger("Intensity", 1);
    }

    void AnimMusicPause()
    {
        foreach (GameObject item in SimpleAnimatedCharacter)
        {
            item.GetComponent<Animator>().SetBool("IsActive", false);
        }
        Crowd.GetComponent<Animator>().SetBool("IsActive", false);
        Arco.GetComponent<Animator>().SetInteger("Intensity", 1);
        AmadeDrum.GetComponent<Animator>().SetBool("IsActive", false);
        MozartGuitar.GetComponent<Animator>().SetBool("IsActive", false);
        CaciliaHorn.GetComponent<Animator>().SetBool("IsActive", false);
    }

    void AnimMusicBack()
    {
        foreach (GameObject item in SimpleAnimatedCharacter)
        {
            item.GetComponent<Animator>().SetBool("IsActive", true);
        }
        Crowd.GetComponent<Animator>().SetBool("IsActive", true);
        Crowd.GetComponent<Animator>().SetInteger("Intensity", 1);
        Arco.SetActive(false);
        AmadeDrum.GetComponent<Animator>().SetBool("IsActive", true);
        AmadeDrum.GetComponent<Animator>().SetInteger("Intensity", 2);
        MozartGuitar.GetComponent<Animator>().SetBool("IsActive", true);
        CaciliaHorn.GetComponent<Animator>().SetBool("IsActive", true);
    }

    void AnimMusicEnd()
    {
        foreach (GameObject item in SimpleAnimatedCharacter)
        {
            item.GetComponent<Animator>().SetBool("IsActive", false);
        }
        Crowd.GetComponent<Animator>().SetBool("IsActive", false);
        AmadeDrum.GetComponent<Animator>().SetBool("IsActive", false);
        MozartGuitar.GetComponent<Animator>().SetBool("IsActive", false);
        CaciliaHorn.GetComponent<Animator>().SetBool("IsActive", false);
    }

    #endregion
}