using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : IGameState 
{
	public static IGameState lastKnownState;
	bool paused;

	public MenuState(bool _paused = false)
	{
		paused = _paused;
	}

	public void StateStart () 
	{
		if (paused)
		{
			Time.timeScale = 0;
			GUIFormatter.instance.pausedMenu.SetActive (true);
		}
	}

	public void StateUpdate () 
	{
		if (Input.GetButtonDown ("Submit")) 
		{
			GameManager.instance.SwitchState (lastKnownState);
		}
	}

	public void StateFixedUpdate()
	{

	}

	public void StateStop()
	{
		if (paused)
		{
			GUIFormatter.instance.pausedMenu.SetActive (false);
			Time.timeScale = GameManager.originalTimeScale;
		}
	}

	public GameState GetState () 
	{
		return GameState.Menu;
	}
}
