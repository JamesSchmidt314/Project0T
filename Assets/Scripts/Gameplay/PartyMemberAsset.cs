using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PartyMemberAsset 
{
	[MenuItem("Assets/Create/PartyMember")]
	public static void CreateParyMember()
	{
		PartyMemberObject partyMember = ScriptableObject.CreateInstance<PartyMemberObject> ();

		AssetDatabase.CreateAsset (partyMember, "Assets/Characters/PartyMembers/PartyMember.asset");
		AssetDatabase.SaveAssets ();

		EditorUtility.FocusProjectWindow ();

		Selection.activeObject = partyMember;
	}
}
