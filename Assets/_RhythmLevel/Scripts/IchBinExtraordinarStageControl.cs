using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Use this for initialization
    void Start () {
        m_IsActing = false;
        Create_SAC_ActionQueue();
    }
	
    public void BeginActing()
    {
        m_IsActing = true;
    }

	// Update is called once per frame
	void Update () {
        if (m_IsActing)
        {
            m_TimeStamp = GameLogic.GetComponent<RhythmLevelController>().GetCurrentTimestamp();
            if (SAC_Record.ActionCounter < SAC_Record.KeyTimestamp.Count)
            {
                Do_SAC_Action();
            }
        }
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


}
