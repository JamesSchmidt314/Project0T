using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
	void StateStart();

	void StateUpdate ();

	void StateFixedUpdate ();

	void StateStop();

	GameState GetState();
}
