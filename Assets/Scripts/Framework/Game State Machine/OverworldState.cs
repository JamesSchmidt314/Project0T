using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldState : IGameState
{
	public static bool inInteractivePosition = false;
	public static RPGEvent currentRPGEvent;

	public void StateStart () 
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void StateUpdate()
	{
		if (inInteractivePosition && currentRPGEvent)
		{
			if (Input.GetButtonDown ("Fire1")) 
			{
				currentRPGEvent.PlayRPGEvent ();
			}
		}

		if (Input.GetButtonDown ("Submit")) 
		{
			GameManager.instance.SwitchState (new MenuState (true));
		}

		CameraController.instance.HandleRotationMovement (Input.GetAxis ("Mouse X"),Input.GetAxis ("Mouse Y"));
	}

	public void StateFixedUpdate()
	{
		Vector3 camForward = Vector3.Scale (Camera.main.transform.forward, new Vector3 (1, 0, 1)).normalized;
		OverworldPlayerCharacter.playerCharacter.RelayInput (Input.GetAxis ("Vertical") * camForward + Input.GetAxis ("Horizontal") * Camera.main.transform.right);

		for (int i = 1; i < PlayerDataManager.instance.overworldPartyMembers.Count; i++)
		{
			PlayerDataManager.instance.overworldPartyMembers [i].FollowPlayer ();
		}
	}

	public void StateStop()
	{
		for (int i = 0; i < OverworldPlayerCharacter.currentCharacters.Count; i++)
		{
			OverworldPlayerCharacter.currentCharacters [i].StopCharacter ();
		}

		Cursor.lockState = CursorLockMode.None;
	}

	public GameState GetState () 
	{
		return GameState.Overworld;
	}
}