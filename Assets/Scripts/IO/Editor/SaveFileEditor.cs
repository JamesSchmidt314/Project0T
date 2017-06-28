using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveFileEditor : EditorWindow 
{
	Vector2 scrollPos;
	public SaveFile saveFile;

	[MenuItem("PersistentData/Delete Save File")]
	static void DeleteSaveFile()
	{
		PlayerDataManager.DeleteSaveData ();
	}

	[MenuItem("PersistentData/Save File Editor")]
	static void Init()
	{
		EditorWindow.GetWindow (typeof(SaveFileEditor)).Show ();
		//saveFile = PlayerDataManager.GetSaveFile ();
	}

	private void OnGUI()
	{
		if (saveFile != null)
		{
			SerializedObject serializedObject = new SerializedObject (this);
			SerializedProperty serializedProperty = serializedObject.FindProperty ("saveFile");
			string[] keys = new string[saveFile.gamePersistenceValues.Keys.Count];
			int[] values = new int[saveFile.gamePersistenceValues.Values.Count];

			saveFile.gamePersistenceValues.Keys.CopyTo(keys,0);
			saveFile.gamePersistenceValues.Values.CopyTo(values,0);

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			GUI.enabled = false;
			EditorGUILayout.PropertyField (serializedProperty, true);

			for (int i = 0; i < values.Length; i++) 
			{
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.TextField (keys[i]);
				EditorGUILayout.IntField (values[i]);
				EditorGUILayout.EndHorizontal ();
			}
			GUI.enabled = true;
			EditorGUILayout.EndScrollView ();
			serializedObject.ApplyModifiedProperties ();
		}

		if (GUILayout.Button ("Load Save Data"))
			LoadGameData ();
	}

	private void LoadGameData()
	{
		saveFile = PlayerDataManager.GetSaveFile ();
	}
}
