using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadRhythmLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

}
