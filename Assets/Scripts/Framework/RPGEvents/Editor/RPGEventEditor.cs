using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditor.Events;

[CustomEditor(typeof(RPGEvent))]
public class RPGEventEditor : Editor
{
	
	RPGEvent t;
	private SerializedObject serializedObjectTarget;

	void OnEnable()
	{
		t = (RPGEvent)target;
		serializedObjectTarget = new SerializedObject (t);
	}

	public override void OnInspectorGUI()
	{
		Undo.RecordObject (target, target.name);

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("is NPC");
		t.isNPC = EditorGUILayout.Toggle (t.isNPC);
		EditorGUILayout.EndHorizontal ();

		if (t.isNPC) 
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("NPC Animator");
			t.m_NPCAnimator = (Animator)EditorGUILayout.ObjectField (t.m_NPCAnimator, typeof(Animator), true);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Return To Idle Wait Time");
			t.timeBeforeIdleReturn = EditorGUILayout.FloatField (t.timeBeforeIdleReturn);
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Trigger Type");
		t.trigger = (RPGTrigger)EditorGUILayout.EnumPopup (t.trigger);
		EditorGUILayout.EndHorizontal ();

		if (t.trigger == RPGTrigger.OnInteract)
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Triggerable Distance");
			t.triggerableDistance = EditorGUILayout.FloatField (t.triggerableDistance);
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField("Reset After Completion");
		t.eventPage.reset = EditorGUILayout.Toggle (t.eventPage.reset);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginVertical (new GUIStyle ("Box"));

		EditorGUILayout.PrefixLabel ("Conditions");
		for (int i = 0; i < t.eventPage.conditions.Count; i++)
		{
			EditorGUILayout.BeginVertical (new GUIStyle ("Box"));
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Key Name");
			t.eventPage.conditions[i].keyName = EditorGUILayout.TextField(t.eventPage.conditions[i].keyName);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Key Value");
			t.eventPage.conditions[i].value = EditorGUILayout.IntField(t.eventPage.conditions[i].value);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Comparer");
			t.eventPage.conditions[i].validType = (RPGCondition.ValidType)EditorGUILayout.EnumPopup(t.eventPage.conditions[i].validType);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			if (GUILayout.Button ("X", GUILayout.MaxHeight(50)))
			{
				t.eventPage.conditions.RemoveAt (i);
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
		}
		if (GUILayout.Button ("Add New Condition"))
		{
			t.eventPage.conditions.Add (new RPGCondition ());
			serializedObjectTarget.ApplyModifiedProperties();
		}
		EditorGUILayout.EndVertical ();

		EditorGUILayout.BeginVertical (new GUIStyle ("Box"));
		EditorGUILayout.PrefixLabel ("Conditions");
		for (int i = 0; i < t.eventPage.commands.Count; i++)
		{
			/////////////////////
			string[] commandNames = new string[t.eventPage.commands.Count + 1];
			int[] commandIDs = new int[t.eventPage.commands.Count + 1];
			for (int k = 0; k < t.eventPage.commands.Count; k++) 
			{
				if (k < i) 
				{
					commandNames [k] = "Command ID: " + k;
					commandIDs [k] = k;
				} 
				else if (k > i) 
				{
					commandNames [k - 1] = "Command ID: " + k;
					commandIDs [k - 1] = k;
				}
			}
			commandNames [t.eventPage.commands.Count] = "End Event";
			commandIDs [t.eventPage.commands.Count] = -1;
			////////////////////////////

			EditorGUILayout.BeginVertical (new GUIStyle ("Box"));
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical ();
			t.eventPage.commands [i].show = EditorGUILayout.Foldout (t.eventPage.commands [i].show, "Command ID: " + i);
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("↑")) 
			{
				if (i > 0) 
				{
					t.eventPage.commands.Insert (i - 1, t.eventPage.commands[i]);
					t.eventPage.commands.RemoveAt (i + 1);
				}
			}
			if (GUILayout.Button ("↓")) 
			{
				if (i + 1 < t.eventPage.commands.Count) 
				{
					t.eventPage.commands.Insert (i + 2,t.eventPage.commands[i]);
					t.eventPage.commands.RemoveAt (i);
				}
			}
			if (GUILayout.Button ("X")) 
			{
				t.eventPage.commands.RemoveAt (i);
			}
			EditorGUILayout.EndHorizontal ();
			if (i < t.eventPage.commands.Count && t.eventPage.commands [i].show) 
			{
				EditorGUILayout.Space ();
				switch (t.eventPage.commands [i].eventType)
				{
				case RPGEventType.Generic:
					EditorGUILayout.LabelField("Command:");
					SerializedProperty commandEvent = serializedObject.FindProperty ("eventPage").FindPropertyRelative ("commands").GetArrayElementAtIndex (i).FindPropertyRelative("command");
					EditorGUILayout.PropertyField (commandEvent);
					break;
				case RPGEventType.keyInput:
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Key Name:");
					t.eventPage.commands [i].keyInput.key = EditorGUILayout.TextField (t.eventPage.commands [i].keyInput.key);
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField("Key Input Method:");
					t.eventPage.commands [i].keyInput.keyInputType = (KeyInputType)EditorGUILayout.EnumPopup (t.eventPage.commands [i].keyInput.keyInputType);
					EditorGUILayout.EndHorizontal ();

					if (t.eventPage.commands [i].keyInput.keyInputType == KeyInputType.SolidValue) 
					{
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField("Key Value:");
						t.eventPage.commands [i].keyInput.value = EditorGUILayout.IntField (t.eventPage.commands [i].keyInput.value);
						EditorGUILayout.EndHorizontal ();
					}

					break;
				case RPGEventType.BattleInitialize:

					//t.eventPage.commands [i].enemies = EditorGUILayout.ar
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (10);
					EditorGUILayout.PropertyField(serializedObject.FindProperty ("eventPage").FindPropertyRelative ("commands").GetArrayElementAtIndex (i).FindPropertyRelative("enemies"), true);
					//GUILayout.Space (5);
					EditorGUILayout.EndHorizontal ();
					//EditorGUILayout.PropertyField(serializedObject.FindProperty ("eventPage").FindPropertyRelative ("commands").GetArrayElementAtIndex (i).FindPropertyRelative("enemies"), true);
					break;
				case RPGEventType.Dialogue:
					switch (t.eventPage.commands [i].order.orderType)
					{
					case DialogueOrderType.Statement:
						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField("Dialogue Flavor:");
						t.eventPage.commands [i].order.statement.statementFlavor = (DialogueFlavor)EditorGUILayout.ObjectField (t.eventPage.commands [i].order.statement.statementFlavor, typeof(DialogueFlavor), true);
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField("Statement Key:");
						t.eventPage.commands [i].order.statement.statementKey = EditorGUILayout.TextField (t.eventPage.commands [i].order.statement.statementKey);
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField("Unskippable");
						t.eventPage.commands [i].order.statement.unskippable = EditorGUILayout.Toggle (t.eventPage.commands [i].order.statement.unskippable);
						EditorGUILayout.EndHorizontal ();

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField("Currently Speaking Actor");
						t.eventPage.commands [i].order.statement.currentlySpeakingActor = (Animator)EditorGUILayout.ObjectField (t.eventPage.commands [i].order.statement.currentlySpeakingActor, typeof(Animator), true);
						EditorGUILayout.EndHorizontal ();
						break;
					case DialogueOrderType.Divergence:
						for (int j = 0; j < t.eventPage.commands [i].order.divergence.conditions.Count; j++) 
						{
							EditorGUILayout.BeginVertical (new GUIStyle ("Box"));
							EditorGUILayout.BeginHorizontal ();
							if (j == 0)
							{
								EditorGUILayout.LabelField ("If:");
							} 
							else if (j < t.eventPage.commands [i].order.divergence.conditions.Count - 1) 
							{
								EditorGUILayout.LabelField ("Else If:");
							}
							if (GUILayout.Button ("↑")) {
								if (j > 0) {
									t.eventPage.commands [i].order.divergence.conditions.Insert (j - 1, t.eventPage.commands [i].order.divergence.conditions [j]);
									t.eventPage.commands [i].order.divergence.conditions.RemoveAt (j + 1);
									t.eventPage.commands [i].order.divergence.dialogueConditionIDs.Insert (j - 1, t.eventPage.commands [i].order.divergence.dialogueConditionIDs [j]);
									t.eventPage.commands [i].order.divergence.dialogueConditionIDs.RemoveAt (j + 1);
								}
							}
							if (GUILayout.Button ("↓")) {
								if (j + 1 < t.eventPage.commands [i].order.divergence.conditions.Count) {
									t.eventPage.commands [i].order.divergence.conditions.Insert (j + 2, t.eventPage.commands [i].order.divergence.conditions [j]);
									t.eventPage.commands [i].order.divergence.conditions.RemoveAt (j);
									t.eventPage.commands [i].order.divergence.dialogueConditionIDs.Insert (j + 2, t.eventPage.commands [i].order.divergence.dialogueConditionIDs [j]);
									t.eventPage.commands [i].order.divergence.dialogueConditionIDs.RemoveAt (j);
								}
							}
							if (GUILayout.Button ("X")) {
								t.eventPage.commands [i].order.divergence.conditions.RemoveAt (j);
								t.eventPage.commands [i].order.divergence.dialogueConditionIDs.RemoveAt (j);
							}
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.Space ();
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.LabelField ("Key Name");
							t.eventPage.commands [i].order.divergence.conditions [j].keyName = EditorGUILayout.TextField (t.eventPage.commands [i].order.divergence.conditions [j].keyName);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.LabelField ("Key Value");
							t.eventPage.commands [i].order.divergence.conditions [j].value = EditorGUILayout.IntField (t.eventPage.commands [i].order.divergence.conditions [j].value);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.LabelField ("Comparer");
							t.eventPage.commands [i].order.divergence.conditions [j].validType = (RPGCondition.ValidType)EditorGUILayout.EnumPopup (t.eventPage.commands [i].order.divergence.conditions [j].validType);
							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.LabelField ("Then Proceed to:");
							t.eventPage.commands [i].order.divergence.dialogueConditionIDs [j] = EditorGUILayout.IntPopup (t.eventPage.commands [i].order.divergence.dialogueConditionIDs [j], commandNames,commandIDs);
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.EndVertical ();
						}

						if (t.eventPage.commands [i].order.divergence.dialogueConditionIDs.Count > 1) 
						{
							EditorGUILayout.LabelField ("Else:");
						}

						EditorGUILayout.BeginHorizontal ();
						EditorGUILayout.LabelField ("Default direction");
						t.eventPage.commands [i].order.divergence.defaultDivergenceID = EditorGUILayout.IntPopup (t.eventPage.commands [i].order.divergence.defaultDivergenceID, commandNames, commandIDs);
						EditorGUILayout.EndHorizontal ();

						if (GUILayout.Button ("Add New Condition")) {
							t.eventPage.commands [i].order.divergence.conditions.Add (new RPGCondition ());
							t.eventPage.commands [i].order.divergence.dialogueConditionIDs.Add (new int ());
						}
						break;
					case DialogueOrderType.Option:
						for (int j = 0; j < t.eventPage.commands [i].order.option.choiceKeys.Count; j++) 
						{
							EditorGUILayout.Space ();
							EditorGUILayout.BeginHorizontal ();
							EditorGUILayout.BeginHorizontal ();
							t.eventPage.commands [i].order.option.choiceKeys [j] = EditorGUILayout.TextField (t.eventPage.commands [i].order.option.choiceKeys [j]);
							t.eventPage.commands [i].order.option.choiceIDs [j] = EditorGUILayout.IntPopup (t.eventPage.commands [i].order.option.choiceIDs [j], commandNames, commandIDs);

							if (GUILayout.Button ("↑"))
							{
								if (j > 0)
								{
									t.eventPage.commands [i].order.option.choiceKeys.Insert (j - 1, t.eventPage.commands [i].order.option.choiceKeys[j]);
									t.eventPage.commands [i].order.option.choiceKeys.RemoveAt (j + 1);
									t.eventPage.commands [i].order.option.choiceIDs.Insert (j - 1, t.eventPage.commands [i].order.option.choiceIDs [j]);
									t.eventPage.commands [i].order.option.choiceIDs.RemoveAt (j + 1);
								}
							}
							if (GUILayout.Button ("↓")) 
							{
								if (j + 1 < t.eventPage.commands [i].order.divergence.conditions.Count) 
								{
									t.eventPage.commands [i].order.option.choiceKeys.Insert (j + 2,t.eventPage.commands [i].order.option.choiceKeys [j]);
									t.eventPage.commands [i].order.option.choiceKeys.RemoveAt (j);
									t.eventPage.commands [i].order.option.choiceIDs.Insert (j + 2, t.eventPage.commands [i].order.option.choiceIDs [j]);
									t.eventPage.commands [i].order.option.choiceIDs.RemoveAt (j);
								}
							}
							if (GUILayout.Button ("X")) 
							{
								t.eventPage.commands [i].order.option.choiceKeys.RemoveAt (j);
								t.eventPage.commands [i].order.option.choiceIDs.RemoveAt (j);
							}
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.EndHorizontal ();
							EditorGUILayout.Space ();
						}
						if (GUILayout.Button ("Add New Choice"))
						{
							t.eventPage.commands [i].order.option.choiceKeys.Add (string.Empty);
							t.eventPage.commands [i].order.option.choiceIDs.Add (new int ());
						}
						break;
					}
					break;
				}
				if (t.eventPage.commands [i].order.orderType != DialogueOrderType.Divergence && t.eventPage.commands [i].order.orderType != DialogueOrderType.Option) 
				{
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField("Next Command:");
					t.eventPage.commands [i].nextCommand = EditorGUILayout.IntPopup (t.eventPage.commands [i].nextCommand, commandNames, commandIDs);
					EditorGUILayout.EndHorizontal ();
				}
			}

			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical ();
		}
		if (GUILayout.Button ("Add New Generic Command"))
		{
			t.eventPage.commands.Add (new RPGEventCommand (RPGEventType.Generic));
		}
		if (GUILayout.Button ("Add New Key Command"))
		{
			t.eventPage.commands.Add (new RPGEventCommand (RPGEventType.keyInput));
		}
		if (GUILayout.Button ("Add New Battle Command"))
		{
			t.eventPage.commands.Add (new RPGEventCommand (RPGEventType.BattleInitialize));
		}
		if (GUILayout.Button ("Add New Dialogue Statement"))
		{
			t.eventPage.commands.Add (new RPGEventCommand (RPGEventType.Dialogue, DialogueOrderType.Statement));
		}

		if (GUILayout.Button ("Add New Dialogue Divergence"))
		{
			t.eventPage.commands.Add (new RPGEventCommand (RPGEventType.Dialogue, DialogueOrderType.Divergence));
		}

		if (GUILayout.Button ("Add New Dialogue Option"))
		{
			t.eventPage.commands.Add (new RPGEventCommand (RPGEventType.Dialogue, DialogueOrderType.Option));
		}
		EditorGUILayout.EndVertical ();


		//serializedObjectTarget.ApplyModifiedProperties();
		serializedObject.ApplyModifiedProperties();
	}
}