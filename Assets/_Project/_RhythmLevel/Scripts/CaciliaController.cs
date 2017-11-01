using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaciliaController : MonoBehaviour {
    public Camera MainCamera;

    public RhythmLevelController h_LevelController;

    public List<Transform> WayPoints;
    public GameObject LeftHorn;
    public GameObject RightHorn;

    bool m_IsActing;
    float m_Timestamp;
    bool m_FacingRight;

    public int m_WP0, m_WP1;
    public float m_TS0, m_TS1;

    void Awake()
    {
        m_IsActing = false;
    }

    List<float> HornTimestamp;
    int HornMovementCount;
    int HornBlowCount;

    // Use this for initialization
    void Start() {
        HornTimestamp = new List<float>() {0, 15.5f, 20.4f, 29.0f, 33.5f, 55.5f ,60.5f ,86 };
        HornMovementCount = 0;
        HornBlowCount = 1;
        m_WP0 = 0;
        m_WP1 = 0;
        m_TS0 = 0;
        m_TS1 = 0;
        transform.position = WayPoints[m_WP0].position;
    }

    // Update is called once per frame
    void Update() {
        if (m_IsActing)
        {
            if ((HornMovementCount < HornTimestamp.Count-1) && (m_Timestamp >= HornTimestamp[HornMovementCount]))
            {
                m_WP0 = m_WP1;
                m_TS0 = m_TS1;
                m_TS1 = HornTimestamp[HornMovementCount + 1];
                m_WP1 = Random.Range(0,2);

                if (m_WP0 == 0 && m_WP1 == 1 && !m_FacingRight)
                {
                    m_FacingRight = true;
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                else if (m_WP0 == 1 && m_WP1 == 0 && m_FacingRight)
                {
                    m_FacingRight = false;
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                if (m_WP0 == m_WP1)
                {
                    GetComponent<Animator>().SetBool("IsMoving", false);
                }
                else
                {
                    GetComponent<Animator>().SetBool("IsMoving", true);
                }

                HornMovementCount++;
            }
            if ((HornBlowCount < HornTimestamp.Count) && (m_Timestamp >= HornTimestamp[HornBlowCount]))
            {
                Vector3 viewPos = MainCamera.WorldToViewportPoint(transform.position);
                if (viewPos.x > 0.5F)
                {
                    RightHorn.SetActive(true);
                }
                else
                {
                    LeftHorn.SetActive(true);
                }
                StartCoroutine(DisableHorn());
                HornBlowCount++;
                if (HornBlowCount == HornTimestamp.Count)
                {
                    m_IsActing = false;
                    GetComponent<Animator>().SetBool("IsMoving", false);
                }
            }

            m_Timestamp = h_LevelController.GetCurrentTimestamp();

            transform.position = new Vector3((WayPoints[m_WP0].position.x + (WayPoints[m_WP1].position.x - WayPoints[m_WP0].position.x) * (m_Timestamp - m_TS0) / (m_TS1 - m_TS0))
                ,WayPoints[m_WP0].position.y
                ,WayPoints[m_WP0].position.z);
        }
    }

    public void StartActing()
    {
        m_IsActing = true;
        m_FacingRight = true;
    }

    IEnumerator DisableHorn()
    {
        yield return new WaitForSeconds(1f);
        LeftHorn.SetActive(false);
        RightHorn.SetActive(false);
    }
}
