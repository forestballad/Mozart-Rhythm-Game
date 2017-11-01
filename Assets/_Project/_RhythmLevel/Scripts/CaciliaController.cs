using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaciliaController : MonoBehaviour {
    public Camera MainCamera;

    public RhythmLevelController h_LevelController;

    public GameObject Horn;

    bool m_IsActing;
    float m_Timestamp;

    void Awake()
    {
        m_IsActing = false;
    }

    List<float> HornTimestamp;
    int HornBlowCount;

    // Use this for initialization
    void Start() {
        HornTimestamp = new List<float>() {15.5f, 20f, 29.0f, 33.5f, 55.5f ,60.5f ,86 };
    }

    // Update is called once per frame
    void Update() {
        if (m_IsActing)
        {
            if ((HornBlowCount < HornTimestamp.Count) && (m_Timestamp >= HornTimestamp[HornBlowCount]))
            {
                Horn.SetActive(true);
                StartCoroutine(DisableHorn());
                HornBlowCount++;
                if (HornBlowCount == HornTimestamp.Count)
                {
                    m_IsActing = false;
                }
            }
            m_Timestamp = h_LevelController.GetCurrentTimestamp();
        }
    }

    public void StartActing()
    {
        m_IsActing = true;
        HornBlowCount = 0;
    }

    IEnumerator DisableHorn()
    {
        yield return new WaitForSeconds(1f);
        Horn.SetActive(false);
    }
}
