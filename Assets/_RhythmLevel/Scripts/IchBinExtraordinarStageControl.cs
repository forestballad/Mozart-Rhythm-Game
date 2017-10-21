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

    // Use this for initialization
    void Start () {
        m_IsActing = false;
        Create_SAC_ActionQueue();
        Create_AD_ActionQueue();
        Create_CH_ActionQueue();
    }
	
    public void BeginActing()
    {
        m_IsActing = true;
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
        m_Combo = GameLogic.GetComponent<RhythmLevelController>().GetCurrentCombo();
        m_Score = GameLogic.GetComponent<RhythmLevelController>().GetCurrentScore();
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
        ScoreText.GetComponent<Text>().text = "SCORE:" + m_Score.ToString();
        ComboText.GetComponent<Text>().text = "COMBO:" + m_Combo.ToString();
        NoteResultText.GetComponent<Text>().text = GameLogic.GetComponent<RhythmLevelController>().GetLastValidHitType() + "!";
        if (m_TimeStamp > m_NRT_Vanish_Timestamp)
        {
            NoteResultText.SetActive(false);
        }
        else
        {
            NoteResultText.SetActive(true);
        }
    }
}