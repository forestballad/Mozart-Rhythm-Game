using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEditor : MonoBehaviour {
    public AudioSource MusicPlayer;

	// Use this for initialization
	void Start () {
        MusicPlayer.time = 0;
	}
	
	// Update is called once per frame
	void Update () {
        GameObject.Find("CurrentTime").GetComponent<Text>().text = "Current Time: " + MusicPlayer.time;
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

}
