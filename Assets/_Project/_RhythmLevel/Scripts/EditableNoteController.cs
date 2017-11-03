using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableNoteController : MonoBehaviour {
    public Sprite LNote;
    public Sprite LNoteHit;
    public Sprite RNote;
    public Sprite RNoteHit;
    public Sprite BNote;
    public Sprite BNoteHit;

    Sprite m_NoteSprite;
    Sprite m_NoteSpriteHit;

    int m_index;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(string NoteType, int index)
    {
        m_index = index;
        switch (NoteType)
        {
            case "0":
                m_NoteSprite = LNote;
                m_NoteSpriteHit = LNoteHit;
                break;
            case "1":
                m_NoteSprite = RNote;
                m_NoteSpriteHit = RNoteHit;
                break;
            case "2":
                m_NoteSprite = BNote;
                m_NoteSpriteHit = BNoteHit;
                break;
        }
        GetComponent<SpriteRenderer>().sprite = m_NoteSprite;
    }

    void OnMouseDown()
    {
        GameObject.Find("NoteEditor").GetComponent<NoteEditor>().OpenNoteEditWindow(m_index);
    }

    public void GetHit()
    {
        GetComponent<SpriteRenderer>().sprite = m_NoteSpriteHit;
    }

    public void ResetHit()
    {
        GetComponent<SpriteRenderer>().sprite = m_NoteSprite;
    }
}
