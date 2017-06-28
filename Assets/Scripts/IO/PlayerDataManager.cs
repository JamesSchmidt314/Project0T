using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
	[SerializeField]private SaveFile s_SaveFile;
	public const string SAVE_PATH = "/saveFile.gd";
	private bool m_SaveFileLoaded = false;

	public List<OverworldPlayerCharacter> overworldPartyMembers = new List<OverworldPlayerCharacter> ();

	public SaveFile saveFile
	{
		get{ return s_SaveFile;}
	}

	public bool saveFileLoaded
	{
		get{ return m_SaveFileLoaded;}
	}

	IEnumerator Start ()
	{
		if (SaveFileExists ())
		{
			LoadGame ();
		} else
			NewGame ();

		m_SaveFileLoaded = true;
		yield return null;
	}

	public void NewGame()
	{
		s_SaveFile = new SaveFile ();
	}

	public void SaveGame()
	{
		PrimeSaveFile ();
		Debug.Log ("Saving data to: " + Application.persistentDataPath + SAVE_PATH);
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + SAVE_PATH);
		bf.Serialize (file, s_SaveFile);
		file.Close ();
	}

	public void LoadGame()
	{
		if (SaveFileExists ()) 
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + SAVE_PATH, FileMode.Open);
			s_SaveFile = (SaveFile)bf.Deserialize (file);
			file.Close ();
		}
		else Debug.Log ("Error: No Save File exists. Please create a new Save File");
	}

	public static void DeleteSaveData()
	{
		if (SaveFileExists ()) 
		{
			File.Delete (Application.persistentDataPath + SAVE_PATH);
			Debug.Log ("Existing Save File Deleted");
		}
		else Debug.Log ("Failed to Delete Save File: No Save File Exists");
	}

	public static bool SaveFileExists()
	{
		return File.Exists (Application.persistentDataPath + SAVE_PATH);
	}

	public static SaveFile GetSaveFile()
	{
		if (SaveFileExists ()) 
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + SAVE_PATH, FileMode.Open);
			SaveFile save = (SaveFile)bf.Deserialize (file);
			file.Close (); 
			return save;
		}
		else return new SaveFile ();
	}

	public void ToggleAA(Toggle toggle)
	{
		saveFile.settingsData.antiAliasing = toggle.isOn;
		Debug.Log ("Anti-Aliasing: " + (saveFile.settingsData.antiAliasing ? "On" : "Off"));
		for (int i = 0; i < Camera.allCamerasCount; i++) 
		{
			if (Camera.allCameras [i].GetComponent<Antialiasing> ())
				Camera.allCameras [i].GetComponent<Antialiasing> ().enabled = saveFile.settingsData.antiAliasing;
		}
	}

	public void ToggleBloom(Toggle toggle)
	{
		saveFile.settingsData.bloom = toggle.isOn;
		Debug.Log ("Bloom: " + (saveFile.settingsData.bloom ? "On" : "Off"));
		for (int i = 0; i < Camera.allCamerasCount; i++) 
		{
			if (Camera.allCameras [i].GetComponent<Bloom> ())
				Camera.allCameras [i].GetComponent<Bloom> ().enabled = saveFile.settingsData.bloom;
		}
	}

	public void ToggleDOF(Toggle toggle)
	{
		saveFile.settingsData.depthOfField = toggle.isOn;
		Debug.Log ("Depth Of Field: " + (saveFile.settingsData.depthOfField ? "On" : "Off"));
		for (int i = 0; i < Camera.allCamerasCount; i++) 
		{
			if (Camera.allCameras [i].GetComponent<DepthOfField> ())
				Camera.allCameras [i].GetComponent<DepthOfField> ().enabled = saveFile.settingsData.depthOfField;
		}
	}

	public void AdjustMusicVolume(Slider slider)
	{
		saveFile.settingsData.musicVolume = slider.value;
		MusicManager.instance.ChangeMusicVolume (saveFile.settingsData.musicVolume);
	}

	public void PrimeSaveFile()
	{
		if (OverworldRoomData.instance)
		{
			saveFile.roomName = OverworldRoomData.instance.roomName;
			saveFile.scenePathName = UnityEngine.SceneManagement.SceneManager.GetSceneAt(1).name;

			for (int i = 0; i < overworldPartyMembers.Count; i++) 
			{
				saveFile.partyMembers [i].lastKnownPositionX = overworldPartyMembers [i].transform.position.x;
				saveFile.partyMembers [i].lastKnownPositionY = overworldPartyMembers [i].transform.position.y;
				saveFile.partyMembers [i].lastKnownPositionZ = overworldPartyMembers [i].transform.position.z;

				saveFile.partyMembers [i].lastKnownDirectionX = overworldPartyMembers [i].transform.TransformDirection (Vector3.forward).x;
				saveFile.partyMembers [i].lastKnownDirectionZ = overworldPartyMembers [i].transform.TransformDirection (Vector3.forward).z;
			}

			Debug.Log ("party and scene data saved to current SaveFile instance.");
		}
	}

	public void InstantiatePartyOverworldCharacters(Transform orientation = null)
	{
		for (int i = 0; i < saveFile.partyMembers.Count; i++) 
		{
			Vector3 startingPosition = (orientation != null) ? orientation.position : 
				new Vector3(saveFile.partyMembers [i].lastKnownPositionX,
								saveFile.partyMembers [i].lastKnownPositionY,
									saveFile.partyMembers [i].lastKnownPositionZ);
			GameObject memberResource = Resources.Load ("PartyMembers/"+ saveFile.partyMembers[i].overworldCharacterPath) as GameObject;
			GameObject member = Instantiate (memberResource, startingPosition, Quaternion.LookRotation(new Vector3(saveFile.partyMembers [i].lastKnownDirectionX,0,saveFile.partyMembers [i].lastKnownDirectionZ))) as GameObject;
			OverworldPlayerCharacter character = member.GetComponent<OverworldPlayerCharacter> ();
			overworldPartyMembers.Add (character);
			if (i == 0)
				character.StartAsPlayer ();
			else
				character.GetComponent<UnityEngine.AI.NavMeshAgent> ().stoppingDistance = (i + 1) * 2;
		}
	}
}
