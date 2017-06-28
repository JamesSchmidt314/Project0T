using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueDivergence 
{
	public List<RPGCondition> conditions = new List<RPGCondition>();

	public List<int> dialogueConditionIDs = new List<int>();

	public int defaultDivergenceID = -1;
}