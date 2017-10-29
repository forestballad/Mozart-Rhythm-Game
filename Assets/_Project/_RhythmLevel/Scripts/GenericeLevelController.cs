using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericeLevelController : MonoBehaviour {
    RhythmLevelController h_SpecificLevelController;
    IchBinExtraordinarStageControl h_StageActor;

    public GameObject PauseGameWindow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReturnToTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            PauseCurrentGame();
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PauseCurrentGame();
        }
    }

    void PauseCurrentGame()
    {
        //h_SpecificLevelController.PauseGame();
        //PauseGameWindow.SetActive(true);
    }
}
