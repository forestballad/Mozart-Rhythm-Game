using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericeLevelController : MonoBehaviour {
    public RhythmLevelController h_SpecificLevelController;
    public IchBinExtraordinarStageControl h_StageActor;

    public GameObject PauseGameWindow;

	// Use this for initialization
	void Start () {
        PauseGameWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
	}

    public void ReturnToTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Pause();
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Pause();
        }
    }

    void Pause()
    {
        h_SpecificLevelController.Pause();
        PauseGameWindow.SetActive(true);
    }

    public void UnPause()
    {
        h_SpecificLevelController.UnPause();
        PauseGameWindow.SetActive(false);
    }
}
