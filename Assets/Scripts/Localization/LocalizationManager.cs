using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : Singleton<LocalizationManager> 
{
	private static Dictionary<string, string> localizedText;
	private static bool b_LocalizationReady = false;
	private const string missingTextString = "Localized Text Not Found";

	public bool localizationReady
	{
		get{ return b_LocalizationReady;}
	}

	public void LoadLocalizedText(string fileName)
	{
		localizedText = new Dictionary<string, string> ();
		Debug.Log ("searching for file " + fileName);
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);

		if (File.Exists (filePath)) {
			string dataAsJson = File.ReadAllText (filePath);
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (dataAsJson);

			for (int i = 0; i < loadedData.items.Length; i++)
				localizedText.Add (loadedData.items [i].key, loadedData.items [i].value);
			Debug.Log (fileName + " successfully loaded for localization. -- " + localizedText.Count + " entries.");
			
		} else
			Debug.LogError ("File not found");

		b_LocalizationReady = true;
	}

	public string GetLocalizedValue(string key)
	{
		string result = missingTextString;
		if (localizedText.ContainsKey (key)) 
		{
			result = localizedText [key];
		}

		return result;
	}
}
