using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEditor : MonoBehaviour {
    public AudioSource MusicPlayer;

    public class RawNoteRecord
    {
        public List<float> TimestampList;
        public List<string> NoteTypeList;

        public RawNoteRecord()
        {
            TimestampList = new List<float>();
            NoteTypeList = new List<string>();
        }
    }

    class EditableNote
    {
        public int m_index;
        public bool m_IsActive;
        public float m_Timestamp;
        public string m_NoteType;
        public float m_Lifespan;
        public GameObject m_GameObject;

        public EditableNote(int index, float TimeStamp, string NoteType, float LifeSpan)
        {
            m_index = index;
            m_IsActive = true;
            m_Timestamp = TimeStamp;
            m_NoteType = NoteType;
            m_Lifespan = 2f;
            m_GameObject = null;
        }

    }
    List<EditableNote> ConstructedNote = new List<EditableNote>();

    public Transform SpawnLoc;
    public Transform VanishLoc;

    public GameObject NoteContainer;
    public GameObject NotePrefab;
    public GameObject NoteEditWindow;

    public GameObject TimeControllerButtons;
    public GameObject DataControllerButtons;

    int m_currentEditingIndex = -1;

    enum State
    {
        Idle , Recording, Editing
    }
    State EditorState;

    // Use this for initialization
    void Start () {
        NoteEditWindow.SetActive(false);
        MusicPlayer.time = 0;
        EditorState = State.Idle;
        GameObject.Find("CurrentEditorState").GetComponent<Text>().text = "Editor State: Idle";
    }

    // Update is called once per frame
    void Update () {
        GameObject.Find("CurrentTime").GetComponent<Text>().text = "Current Time: " + MusicPlayer.time;
        if (EditorState == State.Editing)
        {
            SycnNoteLocation();
        }

        if (EditorState == State.Recording)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.B))
            {
                string noteType = "";
                if (Input.GetKeyDown(KeyCode.A))
                {
                    noteType = "0";
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    noteType = "1";
                }
                else
                {
                    noteType = "2";
                }

                EditableNote newNote = new EditableNote(ConstructedNote.Count, MusicPlayer.time, noteType, 2f);
                ConstructedNote.Add(newNote);
                GameObject.Find("CurrentTotalNoteNum").GetComponent<Text>().text = "Total Note: " + ConstructedNote.Count;
            }
            if (MusicPlayer.time >= 120)
            {
                StopRecording();
            }
        }
    }

    public void SetMusicTime(float TargetTime)
    {
        if (TargetTime >= 0 && TargetTime <= MusicPlayer.clip.length)
        {
            MusicPlayer.time = TargetTime;
        }
    }

    public void SetMusicTimeByManualEnter()
    {
        float convertedTime = float.Parse(GameObject.Find("SecInputFieldText").GetComponent<Text>().text);
        SetMusicTime(convertedTime);
    }

    public void ForwardMusic(float sec)
    {
        float newTime = MusicPlayer.time + sec;
        SetMusicTime(newTime);
    }

    public void BackwardMusic(float sec)
    {
        float newTime = MusicPlayer.time - sec;
        SetMusicTime(newTime);
    }

    public void PlayMusic()
    {
        MusicPlayer.Play();
    }

    public void PauseMusic()
    {
        MusicPlayer.Pause();
    }

    public void SycnNoteLocation()
    {
        for (int i = 0; i < ConstructedNote.Count; i++)
        {
            float dist = Vector3.Distance(SpawnLoc.position, VanishLoc.position) * (ConstructedNote[i].m_Timestamp - MusicPlayer.time) / ConstructedNote[i].m_Lifespan;
            Vector3 NoteLoc = new Vector3(VanishLoc.position.x + dist, VanishLoc.position.y, VanishLoc.position.z);
            ConstructedNote[i].m_GameObject.transform.position = NoteLoc;
        }
    }

    public void OpenNoteEditWindow(int index)
    {
        PauseMusic();
        NoteEditWindow.SetActive(true);
        GameObject.Find("IndexText").GetComponent<Text>().text = "Index: " + index;
        GameObject.Find("TimestampText").GetComponent<Text>().text = "Timestamp: " + ConstructedNote[index].m_Timestamp;
        string NoteType = "";
        switch (ConstructedNote[index].m_NoteType)
        {
            case "0":
                NoteType = "Left Note";
                GameObject.Find("NoteTypeDropdown").GetComponent<Dropdown>().value = 0;
                break;
            case "1":
                NoteType = "Right Note";
                GameObject.Find("NoteTypeDropdown").GetComponent<Dropdown>().value = 1;
                break;
            case "2":
                NoteType = "Both Note";
                GameObject.Find("NoteTypeDropdown").GetComponent<Dropdown>().value = 2;
                break;
            default:
                break;
        }
        GameObject.Find("TypeText").GetComponent<Text>().text = "Type: " + NoteType;
        GameObject.Find("LifespanText").GetComponent<Text>().text = "Lifespan: " + ConstructedNote[index].m_Lifespan;
        m_currentEditingIndex = index;
    }

    public void CloseNoteEditWindow()
    {
        GameObject.Find("TimestampInputField").GetComponent<InputField>().text = "";
        NoteEditWindow.SetActive(false);
    }

    public void CloseNoteEditWindowWithSavedChanges()
    {
        float newTimeStamp = ConstructedNote[m_currentEditingIndex].m_Timestamp;
        if (GameObject.Find("TimestampInputFieldText").GetComponent<Text>().text != "")
        {
            newTimeStamp = float.Parse(GameObject.Find("TimestampInputFieldText").GetComponent<Text>().text);
        }
        string newNoteType = GameObject.Find("NoteTypeDropdown").GetComponent<Dropdown>().value.ToString();
        float newLifespan = ConstructedNote[m_currentEditingIndex].m_Lifespan;
        if (GameObject.Find("LifespanInputFieldText").GetComponent<Text>().text != "")
        {
            newLifespan = float.Parse(GameObject.Find("LifespanInputFieldText").GetComponent<Text>().text);
        }
        ConstructedNote[m_currentEditingIndex].m_Timestamp = newTimeStamp;
        ConstructedNote[m_currentEditingIndex].m_NoteType = newNoteType;
        ConstructedNote[m_currentEditingIndex].m_Lifespan = newLifespan;
        ConstructedNote[m_currentEditingIndex].m_GameObject.GetComponent<EditableNoteController>().Init(newNoteType,m_currentEditingIndex);

        Debug.Log("For Note index " +  m_currentEditingIndex.ToString() +": new Timestamp will be " + newTimeStamp.ToString()+" , new NoteType will be " +newNoteType.ToString()+" , and new Lifespan will be " +newLifespan.ToString());
        CloseNoteEditWindow();
    }

    public void ConstructNoteFromFile()
    {
        MusicPlayer.Stop();
        foreach (Transform item in NoteContainer.transform)
        {
            Destroy(item.gameObject);
        }

        string FilePath = "";
        #if UNITY_EDITOR
        FilePath = UnityEditor.EditorUtility.OpenFilePanel("Open Note Record File"
                                        , Application.streamingAssetsPath
                                        , "json");
        #endif
        System.IO.StreamReader reader = new System.IO.StreamReader(FilePath);
        string rawJson = reader.ReadLine();
        RawNoteRecord savedNotes = JsonUtility.FromJson<RawNoteRecord>(rawJson);

        int noteNum = savedNotes.TimestampList.Count;

        ConstructedNote.Clear();

        for (int i = 0; i < noteNum; i++)
        {
            EditableNote newNote = new EditableNote(i, savedNotes.TimestampList[i], savedNotes.NoteTypeList[i], 2f);
            if (savedNotes.TimestampList[i] > 90)
            {
                newNote.m_Lifespan = 1.5f;
            }
            GameObject newNoteGameObject = Instantiate(NotePrefab, NoteContainer.transform);
            newNoteGameObject.GetComponent<EditableNoteController>().Init(newNote.m_NoteType, i);
            newNote.m_GameObject = newNoteGameObject;
            ConstructedNote.Add(newNote);
        }
        GameObject.Find("CurrentTotalNoteNum").GetComponent<Text>().text = "Total Note: " + ConstructedNote.Count;
        EditorState = State.Editing;
        GameObject.Find("CurrentEditorState").GetComponent<Text>().text = "Editor State: Editing";
    }

    public void SaveCurrentConstructedNote()
    {
        PauseMusic();
        RawNoteRecord ConstructedRaw = new RawNoteRecord();
        for (int i = 0; i < ConstructedNote.Count; i++)
        {
            ConstructedRaw.TimestampList.Add(ConstructedNote[i].m_Timestamp);
            ConstructedRaw.NoteTypeList.Add(ConstructedNote[i].m_NoteType);
        }
        string NewJson = JsonUtility.ToJson(ConstructedRaw);

        string FilePath = "";
        #if UNITY_EDITOR

        FilePath = UnityEditor.EditorUtility.SaveFilePanel("Save as file",
            Application.streamingAssetsPath,
            "Note Record.json",
            "json");
        #endif
        Debug.Log(FilePath);

        if (FilePath != "")
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(FilePath,false);
            writer.WriteLine(NewJson);
            writer.Close();
        }
    }

    public void NormalizeNote()
    {
        float Normalized = (Mathf.Round(ConstructedNote[m_currentEditingIndex].m_Timestamp / (0.375f / 4))) * (0.375f / 4);
        GameObject.Find("TimestampInputField").GetComponent<InputField>().text = Normalized.ToString();
    }

    public void StartRecording()
    {
        foreach (Transform item in NoteContainer.transform)
        {
            Destroy(item.gameObject);
        }
        MusicPlayer.Stop();
        foreach (Transform item in TimeControllerButtons.transform)
        {
            if (item.GetComponent<Button>() != null)
            {
                item.GetComponent<Button>().interactable = false;
            }
            if (item.GetComponent<InputField>()!= null)
            {
                item.GetComponent<InputField>().interactable = false;
            }
        }
        foreach (Transform item in DataControllerButtons.transform)
        {
            if (item.GetComponent<Button>() != null)
            {
                item.GetComponent<Button>().interactable = false;
            }
            if (item.GetComponent<InputField>() != null)
            {
                item.GetComponent<InputField>().interactable = false;
            }
        }
        EditorState = State.Recording;
        GameObject.Find("CurrentEditorState").GetComponent<Text>().text = "Editor State: Recording";
        GameObject.Find("RecordButton").GetComponent<Button>().interactable = false;
        GameObject.Find("StopRecordButton").GetComponent<Button>().interactable = true;
        MusicPlayer.Play();
    }

    public void StopRecording()
    {
        foreach (Transform item in TimeControllerButtons.transform)
        {
            if (item.GetComponent<Button>() != null)
            {
                item.GetComponent<Button>().interactable = true;
            }
            if (item.GetComponent<InputField>() != null)
            {
                item.GetComponent<InputField>().interactable = true;
            }
        }
        foreach (Transform item in DataControllerButtons.transform)
        {
            if (item.GetComponent<Button>() != null)
            {
                item.GetComponent<Button>().interactable = true;
            }
            if (item.GetComponent<InputField>() != null)
            {
                item.GetComponent<InputField>().interactable = true;
            }
        }
        EditorState = State.Idle;
        GameObject.Find("CurrentEditorState").GetComponent<Text>().text = "Editor State: Idle";
        MusicPlayer.Stop();
        GameObject.Find("StopRecordButton").GetComponent<Button>().interactable = false;
        GameObject.Find("RecordButton").GetComponent<Button>().interactable = true;

    }
}
