﻿using System;
using UnityEngine;

public class IBEInputHandler : IInputHandler
{
	public override string GetInput()
	{
		string ThisFrameHitType = "-1";

		if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.L)))
		{
			if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.L))
			{
				ThisFrameHitType = "2";
			}
			else if (Input.GetKeyDown(KeyCode.A))
			{
				ThisFrameHitType = "0";
			}
			else if (Input.GetKeyDown(KeyCode.L))
			{
				ThisFrameHitType = "1";
			}
		}

		return ThisFrameHitType;
	}
}
