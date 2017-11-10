using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericeLevelController : MonoBehaviour {
    public RhythmLevelController h_SpecificLevelController;
    public IchBinExtraordinarStageControl h_StageActor;

    public GameObject ResultDisplayWindow;
    public GameObject PauseGameWindow;

	// Use this for initialization
	void Start () {
        StartGame();
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
	}

    public void ReturnToTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (h_SpecificLevelController.CurrentGameState == RhythmLevelController.GameState.playing || h_SpecificLevelController.CurrentGameState == RhythmLevelController.GameState.record)
        {
            h_SpecificLevelController.Pause();
            PauseGameWindow.SetActive(true);
        }
    }

    public void UnPauseGame()
    {
        PauseGameWindow.SetActive(false);
        h_SpecificLevelController.UnPause();
    }

    public void StartGame()
    {
        ResultDisplayWindow.SetActive(false);
        PauseGameWindow.SetActive(false);
        h_SpecificLevelController.PlayLevel();
    }

    public void ReplayGame()
    {
        ResultDisplayWindow.SetActive(false);
        PauseGameWindow.SetActive(false);
        h_SpecificLevelController.PlayLevel();
    }

    public void EndGame()
    {
        ResultDisplayWindow.SetActive(true);
    }

    public void PlayRecord()
    {
        ResultDisplayWindow.SetActive(false);
        h_SpecificLevelController.PlayRecord();
    }
}
