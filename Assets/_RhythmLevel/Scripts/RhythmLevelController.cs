using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RhythmLevelController : MonoBehaviour {
    public enum Note
    {
        left, right, both
    }
    public enum GameState
    {
        idle, recording, playing
    }

    public GameState CurrentGameState;

    public AudioSource MusicPlayer;
    public AudioClip TheSong;

    public List<float> NoteTime;
    public List<Note> NoteType;
    public float timer;
    public int currentNote;

    public GameObject NotePrefab;
    public GameObject NoteStartLoc;
    public GameObject NoteEndLoc;
    public GameObject NoteContainer;

    public float playDelaying;
    public float hitRangeCheck = 0.6f;

    public GameObject NoteResultText;
    public float NoteResultTextDisplayTime = 0.2f;
    float NRTLifeSpan;
    public int score;
    public int combo;
    public GameObject ScoreText;
    public GameObject ComboText;

    public class NoteClass
    {
        public List<float> NoteTime;
        public List<Note> NoteType;
    }

    public List<float> TempRecord = new List<float>();

    void Awake()
    {
        CurrentGameState = GameState.idle;
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (CurrentGameState == GameState.recording)
        {
            #region Recording Note
            timer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.B))
            {
                NoteTime.Add(timer);
                NoteType.Add(Note.both);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                NoteTime.Add(timer);
                NoteType.Add(Note.right);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                NoteTime.Add(timer);
                NoteType.Add(Note.left);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                TempRecord.Add(timer);
            }
            #endregion
        }
        if (CurrentGameState == GameState.playing)
        {
            timer += Time.deltaTime;
            if (timer > NRTLifeSpan)
            {
                NoteResultText.SetActive(false);
                ScoreText.GetComponent<Text>().text = "SCORE:" + score.ToString();
                ComboText.GetComponent<Text>().text = "COMBO:" + combo.ToString();
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.L))
            {
                float timestamp = timer - playDelaying;
                List<GameObject> PotentialAffectedNotes = new List<GameObject>();
                foreach (Transform SingleNote in NoteContainer.transform)
                {
                    if (SingleNote.gameObject.GetComponent<NoteController>().Hittable)
                    {
                        PotentialAffectedNotes.Add(SingleNote.gameObject);
                    }
                }
                if (PotentialAffectedNotes.Count > 0)
                {
                    GameObject closestNote = PotentialAffectedNotes[0];
                    float closetDist = Vector2.Distance(new Vector2(closestNote.transform.position.x, closestNote.transform.position.y), new Vector2(NoteEndLoc.transform.position.x, NoteEndLoc.transform.position.y));
                    for (int i = 1; i < PotentialAffectedNotes.Count; i++)
                    {
                        float thisdist = Vector2.Distance(new Vector2(PotentialAffectedNotes[i].transform.position.x, PotentialAffectedNotes[i].transform.position.y), new Vector2(NoteEndLoc.transform.position.x, NoteEndLoc.transform.position.y));
                        if (thisdist < closetDist)
                        {
                            closestNote = PotentialAffectedNotes[i];
                            closetDist = thisdist;
                        }
                    }
                    if (closetDist <= hitRangeCheck) {
                        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.L))
                        {
                            closestNote.GetComponent<NoteController>().GetHit("both", closetDist);
                        }
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            closestNote.GetComponent<NoteController>().GetHit("left", closetDist);
                        }
                        else if (Input.GetKeyDown(KeyCode.L))
                        {
                            closestNote.GetComponent<NoteController>().GetHit("right", closetDist);
                        }
                    }
                    
                }
            }

            if (!MusicPlayer.isPlaying)
            {
                CurrentGameState = GameState.idle;
            }
            
        }
    }

    void FixedUpdate()
    {
        // Note Spawing is according to the hardcode time/node list.
        #region SpawnNote
        if (CurrentGameState == GameState.playing && currentNote < NoteTime.Count)
        {
            if (timer >= NoteTime[currentNote])
            {
                GameObject newNote = Instantiate(NotePrefab, NoteContainer.transform);
                if (NoteType[currentNote] == Note.left)
                {
                    newNote.GetComponent<NoteController>().Init("left", NoteStartLoc, currentNote, NoteTime[currentNote]);
                }
                else if (NoteType[currentNote] == Note.right)
                {
                    newNote.GetComponent<NoteController>().Init("right", NoteStartLoc, currentNote, NoteTime[currentNote]);
                }
                else if (NoteType[currentNote] == Note.both)
                {
                    newNote.GetComponent<NoteController>().Init("both", NoteStartLoc, currentNote, NoteTime[currentNote]);
                }
                currentNote++;
            }
        }
        #endregion
    }

    public void StartPlayingNote()
    {
        // Getting Node Information
        string hardcodejson = "{\"NoteTime\":[3.2206497192382814,3.9585952758789064,4.697356224060059,7.16276741027832,7.531599998474121,7.900975704193115,8.304034233093262,8.991293907165528,9.362435340881348,9.76483154296875,10.467752456665039,10.802763938903809,11.221982955932618,11.960162162780762,12.329183578491211,12.718740463256836,13.420111656188965,13.82217788696289,14.174277305603028,14.895587921142579,15.266952514648438,15.650158882141114,16.38854217529297,16.75796890258789,17.145980834960939,17.848262786865236,18.219200134277345,18.62163734436035,19.359968185424806,19.74323272705078,20.097688674926759,20.816848754882814,21.590381622314454,22.359949111938478,22.52967643737793,22.7138729095459,23.08147430419922,23.249080657958986,23.43360137939453,23.819236755371095,24.171707153320314,24.557186126708986,25.281435012817384,25.647855758666993,25.999774932861329,26.757532119750978,27.10671615600586,27.509666442871095,28.26432228088379,28.616731643676759,29.00258445739746,29.709455490112306,30.09269142150879,30.4952392578125,31.200117111206056,31.602523803710939,31.99067497253418,32.67591857910156,33.064048767089847,33.4476203918457,34.1855354309082,34.9431037902832,35.66261672973633,35.82961654663086,36.0308837890625,36.43339538574219,36.601131439208987,36.82143020629883,37.20488739013672,37.37295150756836,37.54115295410156,37.70835876464844,37.926239013671878,38.09637451171875,38.278507232666019,38.46320724487305,38.70026397705078,39.03572082519531,39.41917419433594,39.78813171386719,40.15777587890625,40.527488708496097,40.89548873901367,41.26417922973633,41.63322448730469,41.98788833618164,42.35506820678711,42.74091720581055,43.128387451171878,43.293785095214847,43.49529266357422,43.66731643676758,43.88172149658203,44.23339080810547,44.61943054199219,45.00714874267578,45.373992919921878,45.70943069458008,46.079833984375,46.48343276977539,46.8502311706543,47.22129821777344,47.55693054199219,47.959049224853519,48.32597732543945,48.69729232788086,49.064208984375,49.43662643432617,49.81920623779297,49.97280502319336,50.15446090698242,50.52323532104492,50.892921447753909,51.24711990356445,52.03333282470703,52.389156341552737,52.790802001953128,53.5284309387207,53.89542007446289,54.26654052734375,55.00463104248047,55.38790512084961,55.742576599121097,56.494789123535159,56.86381149291992,57.233253479003909,57.9714469909668,58.35948944091797,58.70931625366211,59.48097229003906,59.8667106628418,60.238006591796878,60.94021224975586,61.71410369873047,62.45315933227539,62.6343994140625,62.83552551269531,63.20463943481445,63.55766677856445,63.94340896606445,64.11077117919922,64.29499816894531,64.66696166992188,64.83174133300781,65.00189971923828,65.18399810791016,65.38532257080078,65.7402114868164,66.1074447631836,66.49403381347656,66.89823913574219,67.24723815917969,67.61823272705078,67.98518371582031,68.37091064453125,68.74009704589844,69.12606811523438,69.51327514648438,69.86383819580078,70.23560333251953,70.61980438232422,70.98770141601563,71.35692596435547,71.77873992919922,72.12837219238281,72.53096008300781,72.90070343017578,73.28585052490235,73.6573486328125,74.02392578125,74.39530181884766,74.77874755859375,75.11392974853516,75.5006332397461,75.87130737304688,76.03839111328125,76.22142791748047,76.40782928466797,76.59097290039063,76.75809478759766,76.94474029541016,77.1103286743164,77.29481506347656,77.68338012695313,78.10022735595703,78.8551254272461,79.22603607177735,79.61024475097656,80.33106994628906,80.71668243408203,81.10514831542969,81.84284210205078,82.2266845703125,82.61247253417969,82.796630859375,82.98363494873047,83.36734771728516,83.73611450195313,84.12451171875,84.29081726074219,84.49432373046875,84.84323120117188,85.63174438476563,100.04063415527344,100.409423828125,100.77836608886719,100.9652328491211,101.16661834716797,101.50224304199219,101.6673812866211,101.86875915527344,102.05332946777344,102.23993682861328,102.623779296875,103.0093765258789,103.38099670410156,103.74726104736328,104.09968566894531,104.50209045410156,104.87093353271485,105.27377319335938,105.62606811523438,105.99738311767578,106.36426544189453,106.7665023803711,106.93690490722656,107.11927032470703,107.2867431640625,107.47360229492188,107.65591430664063,107.85723114013672,108.02473449707031,108.25968933105469,108.64534759521485,109.03145599365235,109.36685943603516,109.77165985107422,110.1552963256836,110.54113006591797,110.72593688964844,110.9122085571289,111.27934265136719,111.66619873046875,112.05332946777344,112.21842956542969,112.40270233154297,112.77163696289063,113.1574478149414,113.51138305664063,113.6778793334961,113.89712524414063,114.26702117919922,114.67036437988281,115.01951599121094,115.40673828125,115.75731658935547,116.12651062011719,116.51237487792969,116.6829605102539,116.898193359375,117.2505111694336,117.62153625488281,117.9887466430664,118.17301940917969,118.34100341796875,118.52548217773438,118.7265853881836,119.29920959472656,119.8361587524414,120.21942901611328],\"NoteType\":[1,1,0,2,1,1,0,1,1,0,1,1,0,1,1,0,0,0,1,0,0,1,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,1,0,1,1,0,1,1,0,0,0,1,0,0,1,1,0,1,1,0,1,1,1,0,1,0,1,1,0,1,1,0,1,1,0,0,1,1,0,0,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,0,0,0,1,0,1,1,1,0,1,0,1,1,0,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,1,1,0,1,0,0,1,1,0,0,1,1,1,1,0,1,1,1,0,1,1,1,0,0,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,0,1,1,0,1,2,1,1,0,0,1,0,0,1,1,1,1,0,1,1,1,0,1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,0,1,1,0,1,1,0,1,1,0,1,1,0,1,0,1,1,0,1,1,0,2,2]}";
        string json = PlayerPrefs.GetString("TheSong");
        NoteClass savedNotes = JsonUtility.FromJson<NoteClass>(hardcodejson);
        NoteType = savedNotes.NoteType;
        NoteTime = savedNotes.NoteTime;

        CurrentGameState = GameState.playing;
        timer = 0;
        score = 0;
        combo = 0;

        playDelaying = (NoteStartLoc.transform.position.x - NoteEndLoc.transform.position.x) * Time.deltaTime / NotePrefab.GetComponent<NoteController>().speed;
        MusicPlayer.PlayDelayed(playDelaying);

        GetComponent<IchBinExtraordinarStageControl>().BeginActing();
    }

    public void StartRecordingNote()
    {
        CurrentGameState = GameState.recording;
        timer = 0;
        MusicPlayer.Play();
        NoteTime.Clear();
        NoteType.Clear();
    }

    public void DoneRecording()
    {
        CurrentGameState = GameState.idle;
        MusicPlayer.Stop();
        NoteClass recordedNote = new NoteClass();
        recordedNote.NoteTime = NoteTime;
        recordedNote.NoteType = NoteType;
        string json = JsonUtility.ToJson(recordedNote);

        PlayerPrefs.SetString("TheSong", json);

        string filePath = Application.persistentDataPath + "/NoteRecord.json";
        StreamWriter newFile = new StreamWriter(filePath);
        newFile.WriteLine(json);
        newFile.Close();

        for (int i = 0; i < TempRecord.Count; i++)
        {
            Debug.Log(TempRecord[i]);
        }
    }

    public void BreakCombo()
    {
        combo = 0;
    }

    int getComboMultiplier()
    {
        if (combo >= 100)
        {
            return 5;
        }
        else if (combo >= 50)
        {
            return 3;
        }
        else if (combo >= 10)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    public void AddScore(string hitType, bool halfbaseScore)
    {
        NoteResultText.SetActive(true);
        NoteResultText.GetComponent<Text>().text = hitType + "!";
        NRTLifeSpan = timer + NoteResultTextDisplayTime;
        int baseScore = 0;
        if (hitType == "GOOD")
        {
            baseScore = 10;
        }
        else if (hitType == "PERFECT")
        {
            baseScore = 20;
        }
        combo ++;
        int finalScore = baseScore + combo * getComboMultiplier();
        if (halfbaseScore)
        {
            score += finalScore / 2;
        }
        else
        {
            score += finalScore;
        }
    }

    public float GetCurrentTimestamp()
    {
        return timer;
    }
}
