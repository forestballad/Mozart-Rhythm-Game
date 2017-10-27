using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IchBinExtraordinarStageControl : MonoBehaviour {
    public GameObject GameLogic;
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

    public GameObject PauseWindow;

    public ParticleSystem hitParticleGood;
    public ParticleSystem hitParticlePerfect;
    public ParticleSystem starFillingEffect;

    public GameObject StarFillingSpawnLoc;

    // Use this for initialization
    void Start () {
        m_IsActing = false;
        StageAnimationInstruction = new AnimInstruction();
        StageAnimationInstruction.counter = 0;
        StageAnimationInstruction.Instruction = new List<string>() { "AnimMusicBegin", "AnimDrumIn", "AnimVocalIn" ,"AnimMusicPause", "AnimMusicBack","AnimMusicEnd" };
        StageAnimationInstruction.TimeStamp = new List<float>() {0,4.6f,10.4f,87.5f,98,119 };
    }

    public void BeginActing(bool PlayingRecord)
    {
        StageLight.SetActive(false);
        ResultDisplayWindow.SetActive(false);
        m_IsActing = true;
        m_Score = 0;
        m_Combo = 0;
        m_TotalNoteNum = gameObject.GetComponent<RhythmLevelController>().GetTotalNoteNumber();
        m_LastValidHitType = "";
        NoteResultText.GetComponent<Text>().text = "";
        StageAnimationInstruction.counter = 0;
        InitStars();
        if (PlayingRecord)
        {
            RecText.SetActive(true);
        }
    }

    public void EndActing()
    {
        m_IsActing = false;
        ResultDisplayWindow.SetActive(true);
        GameObject.Find("GameResultScoreText").GetComponent<Text>().text = m_Score.ToString();
        CreditWindow.SetActive(false);
        RecText.SetActive(false);
    }

	void Update () {
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
        m_TimeStamp = GameLogic.GetComponent<RhythmLevelController>().GetCurrentTimestamp();
        m_NRT_Vanish_Timestamp = GameLogic.GetComponent<RhythmLevelController>().GetLastValidHitTimestamp() + NRT_DisplayTime;
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
    }

    public void NoteSuccess(string hitType, bool halfbaseScore)
    {
        int baseScore = 0;
        m_Combo++;
        if (hitType == "GOOD")
        {
            m_LastValidHitType = "GOOD";
            baseScore = 10;

            ParticleSystem ps = Instantiate(hitParticleGood);
            ps.transform.position = GameObject.Find("NoteEndLoc").transform.position;
            ps.Play();
        }
        else if (hitType == "PERFECT")
        {
            m_LastValidHitType = "PERFECT";

            
            baseScore = 20;
            if (m_CurrentStar < 3)
            {
                FillStar();
            }

            ParticleSystem ps = Instantiate(hitParticlePerfect);
            ps.transform.position = GameObject.Find("NoteEndLoc").transform.position;
            ps.Play();

           
        }
        int finalScore = baseScore + m_Combo * getComboMultiplier();
        if (halfbaseScore)
        {
            m_Score += finalScore / 2;
        }
        else
        {
            m_Score += finalScore;
        }
    }

    public void NoteFail(string hitType)
    {
        m_LastValidHitType = hitType;
        m_Combo = 0;
    }

    int getComboMultiplier()
    {
        if (m_Combo >= 100)
        {
            return 5;
        }
        else if (m_Combo >= 50)
        {
            return 3;
        }
        else if (m_Combo >= 10)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    void InitStars()
    {
        int starHit = m_TotalNoteNum / 3;
        m_CurrentStar = 0;
        StarIcons[0].GetComponent<StarIconController>().Init(starHit);
        StarIcons[1].GetComponent<StarIconController>().Init(starHit);
        StarIcons[2].GetComponent<StarIconController>().Init(m_TotalNoteNum - starHit * 2);

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

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Debug.Log("pause here");
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("Pause here?");
        }
    }
}