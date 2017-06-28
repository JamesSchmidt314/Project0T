using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile 
{
	//localization
	public string localizationFileName = "OTDemo_en.json";
	//game data
	public string roomName = "Scene";
	public string scenePathName = "Scene01";

	public List<InventoryItem> inventoryList;
	public List<PartyMember> partyMembers = new List<PartyMember>();

	//game persistence Values
	public Dictionary<string, int> gamePersistenceValues = new Dictionary<string, int>();

	//graphics settings
	public SettingsData settingsData = new SettingsData();
}
