using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour {
    public Transform HitLoc;
    int m_TutorialStage;
    public List<GameObject> Instructions;
    bool m_SomeLock;

	// Use this for initialization
	void Start () {
        m_TutorialStage = 0;
        m_SomeLock = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!m_SomeLock && m_TutorialStage == 0 && Input.GetKeyDown(KeyCode.A) && (Vector2.Distance(GameObject.Find("TutorialNote").transform.position, HitLoc.transform.position) < 0.3))
        {
            m_SomeLock = true;
            NextTutorialStage();
        }
        else if (!m_SomeLock && m_TutorialStage == 1 && Input.GetKeyDown(KeyCode.L) && (Vector2.Distance(GameObject.Find("TutorialNote").transform.position, HitLoc.transform.position) < 0.3))
        {
            m_SomeLock = true;
            NextTutorialStage();
        }
        if (!m_SomeLock && m_TutorialStage == 2 && Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.L) && (Vector2.Distance(GameObject.Find("TutorialNote").transform.position, HitLoc.transform.position) < 0.3))
        {
            IEnumerator coroutine = LoadRhythmLevel();
            StartCoroutine(coroutine);
        }
    }

    public void NextTutorialStage()
    {
        Instructions[m_TutorialStage].SetActive(false);
        m_TutorialStage++;
        GameObject.Find("GrayscaleCar").GetComponent<Animator>().SetInteger("Stage", m_TutorialStage);
        IEnumerator coroutine = UnlockAfterTwoSec();
        StartCoroutine(coroutine);

    }

    IEnumerator LoadRhythmLevel()
    {
        Instructions[m_TutorialStage].SetActive(false);
        m_TutorialStage++;
        GameObject.Find("GrayscaleCar").GetComponent<Animator>().SetInteger("Stage", m_TutorialStage);
        yield return new WaitForSeconds(2);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }


    IEnumerator UnlockAfterTwoSec()
    {
        yield return new WaitForSeconds(2);
        m_SomeLock = false;
        Instructions[m_TutorialStage].SetActive(true);
    }
}
