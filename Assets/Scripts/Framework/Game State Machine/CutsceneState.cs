using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneState : IGameState
{
	public void StateStart()
	{
	}

	public void StateUpdate ()
	{

	}

	public void StateFixedUpdate ()
	{

	}

	public void StateStop()
	{

	}

	public GameState GetState()
	{
		return GameState.Cutscene;
	}
}
