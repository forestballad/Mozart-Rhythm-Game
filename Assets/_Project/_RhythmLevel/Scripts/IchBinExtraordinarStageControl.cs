using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IchBinExtraordinarStageControl : MonoBehaviour {
    public GameObject GameLogic;
    float m_TimeStamp;
    bool m_IsActing;

    public List<GameObject> SimpleAnimatedCharacter;
    enum SAC_instruction
    {
        activate, deactivate
    }
    struct SimpleAnimatedCharacterInstruction
    {
        public int ActionCounter;
        public List<float> KeyTimestamp;
        public List<SAC_instruction> StageInstruction;
    }
    SimpleAnimatedCharacterInstruction SAC_Record;

    public GameObject AmadeDrum;
    struct AmadeDrumInstruction
    {
        public int ActionCounter;
        public List<float> KeyTimestamp;
        public List<int> IntensityInstruction;
    }
    AmadeDrumInstruction AD_Record;

    public GameObject CaciliaHorn;
    struct CaciliaHornInstruction
    {
        public int ActionCounter;
        public List<float> KeyTimestamp;
    }
    CaciliaHornInstruction CH_Record;
    // Todo: polish this acting
    public GameObject HornPrefab;

    // Combo / Score / Note Result would be specifict to stage
    public GameObject NoteResultText;
    public float NRT_DisplayTime = 0.2f;
    float m_NRT_Vanish_Timestamp;
    public GameObject ScoreText;
    public GameObject ComboText;
    int m_Score;
    int m_Combo;
    string m_LastValidHitType;

    public GameObject ArcoAngryBar;
    public GameObject ArcoAngryWeak;
    public GameObject ArcoAngryStrong;

    public GameObject ResultDisplayWindow;

    // Use this for initialization
    void Start () {
        m_IsActing = false;
        Create_SAC_ActionQueue();
        Create_AD_ActionQueue();
        Create_CH_ActionQueue();

    }
	
    public void BeginActing()
    {
        ResultDisplayWindow.SetActive(false);
        m_IsActing = true;
        m_Score = 0;
        m_Combo = 0;
        m_LastValidHitType = "";
    }

    public void EndActing()
    {
        m_IsActing = false;
        ResultDisplayWindow.SetActive(true);
    }

	// Update is called once per frame
	void Update () {
        if (m_IsActing)
        {
            SyncDataWithGameLogic();
            if (SAC_Record.ActionCounter < SAC_Record.KeyTimestamp.Count)
            {
                Do_SAC_Action();
            }
            if (AD_Record.ActionCounter < AD_Record.KeyTimestamp.Count)
            {
                Do_AD_Action();
            }
            if (CH_Record.ActionCounter < CH_Record.KeyTimestamp.Count)
            {
                Do_CH_Action();
            }
            UpdateScoreAndCombo();
        }
	}

    void SyncDataWithGameLogic()
    {
        m_TimeStamp = GameLogic.GetComponent<RhythmLevelController>().GetCurrentTimestamp();
        m_NRT_Vanish_Timestamp = GameLogic.GetComponent<RhythmLevelController>().GetLastValidHitTimestamp() + NRT_DisplayTime;
    }

    void Create_SAC_ActionQueue()
    {
        SAC_Record.ActionCounter = 0;
        SAC_Record.KeyTimestamp = new List<float>() { 3f, 6f, 9f };
        SAC_Record.StageInstruction = new List<SAC_instruction>() { SAC_instruction.activate, SAC_instruction.deactivate, SAC_instruction.activate };
    }

    void Do_SAC_Action()
    {
        if (m_TimeStamp > SAC_Record.KeyTimestamp[SAC_Record.ActionCounter])
        {
            if (SAC_Record.StageInstruction[SAC_Record.ActionCounter] == SAC_instruction.activate)
            {
                foreach (GameObject item in SimpleAnimatedCharacter)
                {
                    item.GetComponent<Animator>().SetBool("IsActive", true);
                }
            }
            else if (SAC_Record.StageInstruction[SAC_Record.ActionCounter] == SAC_instruction.deactivate)
            {
                foreach (GameObject item in SimpleAnimatedCharacter)
                {
                    item.GetComponent<Animator>().SetBool("IsActive", false);
                }
            }
            SAC_Record.ActionCounter++;
        }
    }

    void Create_AD_ActionQueue()
    {
        AD_Record.ActionCounter = 0;
        AD_Record.KeyTimestamp = new List<float>() {0,4.63f,87.88f,99.54f,102.33f };
        AD_Record.IntensityInstruction = new List<int> {0,1,0,1,2 };
    }

    void Do_AD_Action()
    {
        if (m_TimeStamp > AD_Record.KeyTimestamp[AD_Record.ActionCounter])
        {
            AmadeDrum.GetComponent<Animator>().SetInteger("Intensity", AD_Record.IntensityInstruction[AD_Record.ActionCounter]);
            AD_Record.ActionCounter++;
        }
    }

    void Create_CH_ActionQueue()
    {
        CH_Record.ActionCounter = 0;
        CH_Record.KeyTimestamp = new List<float>() { 15.77f, 20.39f, 29.02f, 33.78f, 55.85f, 60.71f, 86f };
    }

    void Do_CH_Action()
    {
        if (m_TimeStamp > CH_Record.KeyTimestamp[CH_Record.ActionCounter])
        {
            GameObject newHorn = Instantiate(HornPrefab);
            newHorn.GetComponent<HornController>().InitHorn(1);
            CH_Record.ActionCounter++;
        }
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

        if (m_Combo >= 50 && ArcoAngryStrong.activeSelf == false)
        {
            ArcoAngryStrong.SetActive(true);
            ArcoAngryWeak.SetActive(false);
        }
        else if (m_Combo < 50 && ArcoAngryStrong.activeSelf == true)
        {
            ArcoAngryStrong.SetActive(false);
            ArcoAngryWeak.SetActive(true);
        }

        ScoreText.GetComponent<Text>().text = "SCORE:" + m_Score.ToString();
        ComboText.GetComponent<Text>().text = "COMBO:" + m_Combo.ToString();

        NoteResultText.GetComponent<Text>().text = m_LastValidHitType + "!";

        Color HRTColor = new Color(0,0,0);
        switch (m_LastValidHitType)
        {
            case "PERFECT":
                HRTColor = new Color(255,255,255);
                break;
            case "GOOD":
                HRTColor = new Color(255f, 0, 0);
                break;
            case "BAD":
                HRTColor = new Color(0, 255f, 0);
                break;
            case "WRONG":
                HRTColor = new Color(0, 0, 255f);
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

    public void NoteFail(string hitType)
    {
        m_LastValidHitType = hitType;
        m_Combo = 0;
    }

    public void NoteSuccess(string hitType, bool halfbaseScore)
    {
        int baseScore = 0;
        if (hitType == "GOOD")
        {
            m_LastValidHitType = "GOOD";
            baseScore = 10;
        }
        else if (hitType == "PERFECT")
        {
            m_LastValidHitType = "PERFECT";
            baseScore = 20;
        }
        m_Combo++;
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

}