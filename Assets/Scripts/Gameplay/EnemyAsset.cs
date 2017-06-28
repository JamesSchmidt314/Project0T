using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class EnemyAsset 
{
	[MenuItem("Assets/Create/Enemy")]
	public static void CreateEnemy()
	{
		Enemy enemy = ScriptableObject.CreateInstance<Enemy> ();

		AssetDatabase.CreateAsset (enemy, "Assets/BattleStateEnemies/Enemy.asset");
		AssetDatabase.SaveAssets ();

		EditorUtility.FocusProjectWindow ();

		Selection.activeObject = enemy;
	}
}