using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartyMember 
{
	public string overworldCharacterPath = "Gregg";
	public PartyMemberData memberData;
	//position
	public float lastKnownPositionX;
	public float lastKnownPositionY;
	public float lastKnownPositionZ;
	//orientation (rotation)
	public float lastKnownDirectionX;
	public float lastKnownDirectionZ;

	public PartyMember(string oCharacterPath, PartyMemberData data)
	{
		overworldCharacterPath = oCharacterPath;
		memberData = data;
	}
}