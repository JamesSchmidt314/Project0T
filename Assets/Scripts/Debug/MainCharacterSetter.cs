using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterSetter : MonoBehaviour {

	public OverworldPlayerCharacter character;

	public OverworldPlayerCharacter partyMember;
	
	void Start () 
	{
		GameManager.instance.SwitchState (new OverworldState ());
		OverworldPlayerCharacter player = OverworldPlayerCharacter.Instantiate(character, transform.position, transform.rotation);
		player.SetAsPlayer ();

		//PlayerDataManager.instance.InstantiateCurrentPartyFollowers ();
	}
}