using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveState : IGameState 
{
	public static IGameState lastKnownState;
	public static int currentNextCommandID = 0;
	private static bool unskippable = true;
	public static bool proceed = false;
	public static bool isChoice = false;

	public void StateStart(){}

	public void StateUpdate ()
	{
		if (!isChoice) 
		{
			if (Input.GetButtonDown ("Fire1")) 
			{
				if (!GUIFormatter.instance.isTyping)
				{
					proceed = true;
					if (currentNextCommandID < 0)
						GameManager.instance.SwitchState (lastKnownState);
				}
				else
				{
					if (!unskippable) 
					{
						GUIFormatter.instance.SkipDialogue ();
					}
				}
			}
		}
	}

	public void StateFixedUpdate ()
	{
		
	}

	public void StateStop()
	{
		GUIFormatter.instance.dialogueBox.gameObject.SetActive(false);
	}

	public GameState GetState()
	{
		return GameState.Interaction;
	}

	public static void SetUnskippable(bool value)
	{
		unskippable = value;
	}
}
