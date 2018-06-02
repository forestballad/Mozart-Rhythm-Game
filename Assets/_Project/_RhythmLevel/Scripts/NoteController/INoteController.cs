using System;
using UnityEngine;

public class INoteController : MonoBehaviour
{
	public virtual void Init(float timestamp, string inputNoteType, float lifespan)
	{
		throw new NotImplementedException();
	}

	public virtual RhythmLevelController.NoteResultEventArgs GetHit(string hitType, float timeDiff)
	{
		throw new NotImplementedException();
	}

	public virtual float MissTimestamp
	{
		get { throw new NotImplementedException(); }
	}

	public virtual float ActiveTimestamp
	{
		get { throw new NotImplementedException(); }
	}
}
