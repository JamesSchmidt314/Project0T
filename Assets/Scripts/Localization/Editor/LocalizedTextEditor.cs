using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class LocalizedTextEditor : EditorWindow 
{
	Vector2 scrollPos;
	public LocalizationData localizationData;

	[MenuItem("Window/Localized Text Editor")]
	static void Init()
	{
		EditorWindow.GetWindow (typeof(LocalizedTextEditor)).Show ();
	}

	private void OnGUI()
	{
		if (localizationData != null) 
		{
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("localizationData");
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			EditorGUILayout.PropertyField (serializedProperty, true);
			EditorGUILayout.EndScrollView ();
			serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button ("Save Data"))
				SaveGameData ();

			if (GUILayout.Button ("Sort Data"))
				SortGameData ();
		}

		if (GUILayout.Button ("Load Data"))
			LoadGameData ();

		if (GUILayout.Button ("Create new Data"))
			CreateNewData ();
	}

	private void LoadGameData()
	{
		string filePath = EditorUtility.OpenFilePanel ("Select Localization Data File", Application.streamingAssetsPath, "json");


		if (!string.IsNullOrEmpty (filePath)) 
		{
			string dataAsJson = File.ReadAllText (filePath);
			localizationData = JsonUtility.FromJson<LocalizationData> (dataAsJson);
		}
	}

	private void SaveGameData()
	{
		string filePath = EditorUtility.SaveFilePanel ("Save Localization Data File", Application.streamingAssetsPath, "", "json");

		if (!string.IsNullOrEmpty (filePath)) 
		{
			string dataAsJson = JsonUtility.ToJson (localizationData);
			File.WriteAllText (filePath,dataAsJson);
		}
	}

	private void SortGameData()
	{
		if (localizationData.items.Length > 0) 
		{
			//List<LocalizationItem> data = new List<LocalizationItem> (localizationData.items);
			//data.Sort ();
			localizationData.items = localizationData.items.OrderBy(item => item.key).ToArray();
			//localizationData.items = data.ToArray ();
		}
	}

	private void CreateNewData()
	{
		localizationData = new LocalizationData ();
	}
}