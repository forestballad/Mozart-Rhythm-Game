using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RhythmLevelController : MonoBehaviour
{
    public IchBinExtraordinarStageControl StageController;

    public AudioSource MusicPlayer;
    public AudioClip TheSong;

    public TextAsset NoteRecord;

    public enum GameState
    {
        idle, playing, result, record, pause
    }
    public GameState CurrentGameState;
    public GameState ReturnFromPauseState;

    class Note
    {
        public float m_Timestamp;
        public string m_NoteType;
        public float m_Lifespan;
        public bool m_BeenHit;
        public bool m_BeenSpawn;
        public GameObject NoteGameObject;

        public Note(float TimeStamp, string NoteType)
        {
            m_Timestamp = TimeStamp;
            m_NoteType = NoteType;
            m_Lifespan = 2f;
            m_BeenHit = false;
            m_BeenSpawn = false;
            NoteGameObject = null;
        }
    }
    List<Note> ConstructedNote;

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
    RawNoteRecord PlayerInputRecord;
    int InputRecordCounter;

    public GameObject NoteContainer;
    public GameObject NotePrefab;

    public float m_TimeStamp;
    float m_LastValidHit_Timestamp;

    float m_HitThreshold = 0.12f;
    float m_HitCoolDown = 0.05f;

    void Awake()
    {
        CurrentGameState = GameState.idle;
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentGameState == GameState.playing || CurrentGameState == GameState.record)
        {
            m_TimeStamp = MusicPlayer.time;
            GameObject.Find("FPSText").GetComponent<UnityEngine.UI.Text>().text = "FPS: " + 1 / Time.deltaTime;

            #region SpawnNote
            for (int i = 0; i < ConstructedNote.Count; i++)
            {
                if (!ConstructedNote[i].m_BeenSpawn && (ConstructedNote[i].m_Timestamp - ConstructedNote[i].m_Lifespan) <= m_TimeStamp)
                {
                    GameObject newNote = Instantiate(NotePrefab, NoteContainer.transform);
                    newNote.GetComponent<NoteController>().Init(ConstructedNote[i].m_Timestamp, ConstructedNote[i].m_NoteType, ConstructedNote[i].m_Lifespan);
                    ConstructedNote[i].m_BeenSpawn = true;
                    ConstructedNote[i].NoteGameObject = newNote;
                }
            }
            #endregion

            string ThisFrameHitType = "-1";
            float ThisFramHitTimestamp = 0;

            #region CatchHitting
            if (CurrentGameState == GameState.playing && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.L)) && m_TimeStamp >= m_LastValidHit_Timestamp + m_HitCoolDown)
            {
                ThisFramHitTimestamp = m_TimeStamp;
                if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.L))
                {
                    ThisFrameHitType = "2";
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    ThisFrameHitType = "0";
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    ThisFrameHitType = "1";
                }

                PlayerInputRecord.TimestampList.Add(m_TimeStamp);
                PlayerInputRecord.NoteTypeList.Add(ThisFrameHitType);
            }
            #endregion

            #region SimulateHittingFromRecord
            if (CurrentGameState == GameState.record)
            {
                if (InputRecordCounter < PlayerInputRecord.TimestampList.Count && PlayerInputRecord.TimestampList[InputRecordCounter] <= m_TimeStamp)
                {
                    ThisFrameHitType = PlayerInputRecord.NoteTypeList[InputRecordCounter];
                    ThisFramHitTimestamp = PlayerInputRecord.TimestampList[InputRecordCounter];
                    InputRecordCounter++;
                }
            }
            #endregion

            #region SendHitToNote
            if (ThisFrameHitType != "-1")
            {
                int ClosetValidNoteNum = 0;
                while ((ClosetValidNoteNum < ConstructedNote.Count
                    && (!ConstructedNote[ClosetValidNoteNum].m_BeenSpawn
                        || ConstructedNote[ClosetValidNoteNum].m_BeenHit
                        || Mathf.Abs(ConstructedNote[ClosetValidNoteNum].m_Timestamp - ThisFramHitTimestamp) > m_HitThreshold)))
                {
                    ClosetValidNoteNum++;
                }

                if (ClosetValidNoteNum < ConstructedNote.Count)
                {
                    m_LastValidHit_Timestamp = ThisFramHitTimestamp;
                    int SearchStartLoc = ClosetValidNoteNum + 1;
                    for (int i = SearchStartLoc; i < ConstructedNote.Count; i++)
                    {
                        if (ConstructedNote[ClosetValidNoteNum].m_BeenSpawn
                            && !ConstructedNote[ClosetValidNoteNum].m_BeenHit
                            && (Mathf.Abs(ConstructedNote[i].m_Timestamp - ThisFramHitTimestamp) < Mathf.Abs(ConstructedNote[ClosetValidNoteNum].m_Timestamp - ThisFramHitTimestamp)))
                        {
                            ClosetValidNoteNum = i;
                        }
                    }

                    ConstructedNote[ClosetValidNoteNum].m_BeenHit = true;
                    ConstructedNote[ClosetValidNoteNum].NoteGameObject.GetComponent<NoteController>().GetHit(ThisFrameHitType, Mathf.Abs(ConstructedNote[ClosetValidNoteNum].m_Timestamp - m_TimeStamp));
                }
            }


            #endregion

            #region MissNoteBreakCombo
            for (int i = 0; i < ConstructedNote.Count; i++)
            {
                if (ConstructedNote[i].m_BeenSpawn && !ConstructedNote[i].m_BeenHit && (m_TimeStamp > (ConstructedNote[i].m_Timestamp + m_HitThreshold)))
                {
                    ConstructedNote[i].m_BeenHit = true;
                    ConstructedNote[i].NoteGameObject.GetComponent<NoteController>().GetHit("3", Mathf.Abs(ConstructedNote[i].m_Timestamp - m_TimeStamp));
                }
            }

            #endregion

            if (m_TimeStamp >= TheSong.length)
            {
                MusicPlayer.Stop();
                CurrentGameState = GameState.result;
                foreach (Transform item in NoteContainer.transform)
                {
                    Destroy(item.gameObject);
                }
                GetComponent<GenericeLevelController>().EndGame();
                GetComponent<IchBinExtraordinarStageControl>().EndActing();
            }
        }
        GetComponent<IchBinExtraordinarStageControl>().ManualUpdate();

        Component[] NoteControllers;
        NoteControllers = NoteContainer.GetComponentsInChildren<NoteController>();
        foreach (NoteController item in NoteControllers)
        {
            item.ManualUpdate();
        }
    }

    public void PlayLevel()
    {
        ConstructNoteBasedOnRawJson();
        CurrentGameState = GameState.playing;
        MusicPlayer.time = 0;
        m_LastValidHit_Timestamp = 0;
        foreach (Transform item in NoteContainer.transform)
        {
            Destroy(item.gameObject);
        }
        PlayerInputRecord = new RawNoteRecord();
        MusicPlayer.Play();
        m_TimeStamp = 0;
        StageController.BeginActing(false);
    }

    public void PlayRecord()
    {
        ConstructNoteBasedOnRawJson();
        CurrentGameState = GameState.record;
        MusicPlayer.time = 0;
        foreach (Transform item in NoteContainer.transform)
        {
            Destroy(item.gameObject);
        }
        MusicPlayer.Play();
        InputRecordCounter = 0;
        StageController.BeginActing(true);
    }

    void ConstructNoteBasedOnRawJson()
    {
        //string hardcodejson = "{\"TimestampList\":[3.2206497192382814,3.9585952758789064,4.697356224060059,7.16276741027832,7.531599998474121,7.900975704193115,8.304034233093262,8.991293907165528,9.362435340881348,9.76483154296875,10.467752456665039,10.802763938903809,11.221982955932618,11.960162162780762,12.329183578491211,12.718740463256836,13.420111656188965,13.82217788696289,14.174277305603028,14.895587921142579,15.266952514648438,15.650158882141114,16.38854217529297,16.75796890258789,17.145980834960939,17.848262786865236,18.219200134277345,18.62163734436035,19.359968185424806,19.74323272705078,20.097688674926759,20.816848754882814,21.590381622314454,22.359949111938478,22.52967643737793,22.7138729095459,23.08147430419922,23.249080657958986,23.43360137939453,23.819236755371095,24.171707153320314,24.557186126708986,25.281435012817384,25.647855758666993,25.999774932861329,26.757532119750978,27.10671615600586,27.509666442871095,28.26432228088379,28.616731643676759,29.00258445739746,29.709455490112306,30.09269142150879,30.4952392578125,31.200117111206056,31.602523803710939,31.99067497253418,32.67591857910156,33.064048767089847,33.4476203918457,34.1855354309082,34.9431037902832,35.66261672973633,35.82961654663086,36.0308837890625,36.43339538574219,36.601131439208987,36.82143020629883,37.20488739013672,37.37295150756836,37.54115295410156,37.70835876464844,37.926239013671878,38.09637451171875,38.278507232666019,38.46320724487305,38.70026397705078,39.03572082519531,39.41917419433594,39.78813171386719,40.15777587890625,40.527488708496097,40.89548873901367,41.26417922973633,41.63322448730469,41.98788833618164,42.35506820678711,42.74091720581055,43.128387451171878,43.293785095214847,43.49529266357422,43.66731643676758,43.88172149658203,44.23339080810547,44.61943054199219,45.00714874267578,45.373992919921878,45.70943069458008,46.079833984375,46.48343276977539,46.8502311706543,47.22129821777344,47.55693054199219,47.959049224853519,48.32597732543945,48.69729232788086,49.064208984375,49.43662643432617,49.81920623779297,49.97280502319336,50.15446090698242,50.52323532104492,50.892921447753909,51.24711990356445,52.03333282470703,52.389156341552737,52.790802001953128,53.5284309387207,53.89542007446289,54.26654052734375,55.00463104248047,55.38790512084961,55.742576599121097,56.494789123535159,56.86381149291992,57.233253479003909,57.9714469909668,58.35948944091797,58.70931625366211,59.48097229003906,59.8667106628418,60.238006591796878,60.94021224975586,61.71410369873047,62.45315933227539,62.6343994140625,62.83552551269531,63.20463943481445,63.55766677856445,63.94340896606445,64.11077117919922,64.29499816894531,64.66696166992188,64.83174133300781,65.00189971923828,65.18399810791016,65.38532257080078,65.7402114868164,66.1074447631836,66.49403381347656,66.89823913574219,67.24723815917969,67.61823272705078,67.98518371582031,68.37091064453125,68.74009704589844,69.12606811523438,69.51327514648438,69.86383819580078,70.23560333251953,70.61980438232422,70.98770141601563,71.35692596435547,71.77873992919922,72.12837219238281,72.53096008300781,72.90070343017578,73.28585052490235,73.6573486328125,74.02392578125,74.39530181884766,74.77874755859375,75.11392974853516,75.5006332397461,75.87130737304688,76.03839111328125,76.22142791748047,76.40782928466797,76.59097290039063,76.75809478759766,76.94474029541016,77.1103286743164,77.29481506347656,77.68338012695313,78.10022735595703,78.8551254272461,79.22603607177735,79.61024475097656,80.33106994628906,80.71668243408203,81.10514831542969,81.84284210205078,82.2266845703125,82.61247253417969,82.796630859375,82.98363494873047,83.36734771728516,83.73611450195313,84.12451171875,84.29081726074219,84.49432373046875,84.84323120117188,85.63174438476563,100.04063415527344,100.409423828125,100.77836608886719,100.9652328491211,101.16661834716797,101.50224304199219,101.6673812866211,101.86875915527344,102.05332946777344,102.23993682861328,102.623779296875,103.0093765258789,103.38099670410156,103.74726104736328,104.09968566894531,104.50209045410156,104.87093353271485,105.27377319335938,105.62606811523438,105.99738311767578,106.36426544189453,106.7665023803711,106.93690490722656,107.11927032470703,107.2867431640625,107.47360229492188,107.65591430664063,107.85723114013672,108.02473449707031,108.25968933105469,108.64534759521485,109.03145599365235,109.36685943603516,109.77165985107422,110.1552963256836,110.54113006591797,110.72593688964844,110.9122085571289,111.27934265136719,111.66619873046875,112.05332946777344,112.21842956542969,112.40270233154297,112.77163696289063,113.1574478149414,113.51138305664063,113.6778793334961,113.89712524414063,114.26702117919922,114.67036437988281,115.01951599121094,115.40673828125,115.75731658935547,116.12651062011719,116.51237487792969,116.6829605102539,116.898193359375,117.2505111694336,117.62153625488281,117.9887466430664,118.17301940917969,118.34100341796875,118.52548217773438,118.7265853881836,119.29920959472656,119.8361587524414,120.21942901611328],\"NoteTypeList\":[1,1,0,2,1,1,0,1,1,0,1,1,0,1,1,0,0,0,1,0,0,1,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,1,0,1,1,0,1,1,0,0,0,1,0,0,1,1,0,1,1,0,1,1,1,0,1,0,1,1,0,1,1,0,1,1,0,0,1,1,0,0,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,0,0,1,0,1,1,1,0,1,0,1,1,0,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,0,0,1,1,0,0,1,1,1,1,0,1,1,1,0,1,1,1,0,0,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,0,1,1,0,1,2,1,1,0,0,1,0,0,1,1,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,1,0,2,2]}";
        RawNoteRecord savedNotes = JsonUtility.FromJson<RawNoteRecord>(NoteRecord.text);

#if UNITY_EDITOR
        //string FilePath = "";
        //FilePath = UnityEditor.EditorUtility.OpenFilePanel("Open Note Record File"
        //                                    , Application.streamingAssetsPath
        //                                    , "json");
        //StreamReader reader = new StreamReader(FilePath);
        //string rawJson = reader.ReadLine();
        //savedNotes = JsonUtility.FromJson<RawNoteRecord>(rawJson);
#endif

        int noteNum = savedNotes.TimestampList.Count;
        ConstructedNote = new List<Note>();

        for (int i = 0; i < noteNum; i++)
        {
            Note newNote = new Note(savedNotes.TimestampList[i], savedNotes.NoteTypeList[i]);
            if (savedNotes.TimestampList[i] > 90)
            {
                newNote.m_Lifespan = 1.5f;
            }
            ConstructedNote.Add(newNote);
        }
    }

    public float GetCurrentTimestamp()
    {
        return m_TimeStamp;
    }

    public float GetLastValidHitTimestamp()
    {
        return m_LastValidHit_Timestamp;
    }

    public int GetTotalNoteNumber()
    {
        return ConstructedNote.Count;
    }

    public void Pause()
    {
        ReturnFromPauseState = CurrentGameState;
        CurrentGameState = GameState.pause;
        MusicPlayer.Pause();
    }

    public void UnPause()
    {
        CurrentGameState = ReturnFromPauseState;
        MusicPlayer.Play();
    }
}
