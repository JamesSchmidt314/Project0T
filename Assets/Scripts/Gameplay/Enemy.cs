using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Enemy : ScriptableObject
{
	public bool spareable = false;
	public CharacterData characterData;
	public GameObject model;
	public List<int> spareableConditions = new List<int> ();
	public List<BattleChoice> battleChoices = new List<BattleChoice> ();
	private GameObject m_modelInstance;

	public Enemy(){}

	//copy data from exising enemy
	public Enemy(Enemy enemy)
	{
		spareable = enemy.spareable;
		characterData = enemy.characterData;
		model = enemy.model;
		spareableConditions = enemy.spareableConditions;
		battleChoices = enemy.battleChoices;
	}


	public void TestFunction(int value)
	{
		Debug.Log (value);
	}

	public void SpawnEnemyModelInstance(Vector3 pos, Quaternion rot)
	{
		m_modelInstance = Instantiate (model, pos, rot) as GameObject;
		m_modelInstance.transform.position = pos;
	}
}

[System.Serializable]
public class BattleChoice
{
	public string choiceName = "Choice_Debug";
	public string choiceDescription = "Choice_Description_Debug";
	public List<RPGEventCommand> choiceCommands = new List<RPGEventCommand>();
}

