using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaterBody))]
public class WaterBodyEditor : Editor 
{
	WaterBody t;

	void OnEnable()
	{
		t = (WaterBody)target;
	}

	public override void OnInspectorGUI () 
	{
		//EditorGUILayout.PropertyField(serializedObject.FindProperty("waterTransforms"), true);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("floatForce"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("waveSize"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("waveSpeed"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("waveFrequency"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("length"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("uvTilingScaleX"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("uvTilingScaleY"));

		EditorGUILayout.Space ();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("resX"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("resZ"));

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Build Mesh"))
			t.BuildMesh ();

		serializedObject.ApplyModifiedProperties ();
		EditorUtility.SetDirty (t);
	}
}