using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mozart/Create GameGonfig ")]
public class GameGonfig : ScriptableObject
{
	[SerializeField]
	private float _noteLifespanMin;
	[SerializeField]
	private float _noteLifespanMax;
	[SerializeField]
	private float _noteSpeedMin;
	[SerializeField]
	private float _noteSpeedMax;
	[SerializeField]
	private float _noteSpeed;

	public float NoteSpeed
	{
		get { return _noteSpeed; }
		set { _noteSpeed = Mathf.Clamp(value, _noteSpeedMin, _noteSpeedMax); }
	}

	public float NoteLifespan
	{
		get { return Mathf.Lerp(_noteLifespanMin, _noteLifespanMax, _noteSpeed / _noteSpeedMax); }
	}
}
