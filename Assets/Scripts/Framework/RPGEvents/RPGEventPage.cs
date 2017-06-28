using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class RPGEventPage 
{
	public bool reset = false;
	public List<RPGCondition> conditions = new List<RPGCondition> ();
	public List<RPGEventCommand> commands = new List<RPGEventCommand> ();
}

[Serializable]
public class RPGEventCommand
{
	public RPGEventType eventType;
	public bool show;
	public UnityEvent command;
	public KeyInput keyInput;
	public Enemy[] enemies;
	public DialogueOrder order;
	public int nextCommand = -1;

	[Serializable]
	public class KeyInput
	{
		public string key;
		public KeyInputType keyInputType;
		public int value;
	}

	public RPGEventCommand(RPGEventType _eventType)
	{
		eventType = _eventType;
	}

	public RPGEventCommand(RPGEventType _eventType, DialogueOrderType _DialogueType)
	{
		eventType = _eventType;
		order = new DialogueOrder (_DialogueType);
	}
}

[Serializable]
public class RPGCondition
{
	public string keyName;
	public int value;
	public ValidType validType;

	public enum ValidType
	{
		EqualTo = 0,
		GreaterOrEqualTo = 1,
		LessOrEqualTo = 2,
		GreaterThan = 3,
		LessThan = 4,
	}
}

public enum KeyInputType
{
	Increment = 0,
	Decrement = 1,
	SolidValue = 2
}

public enum RPGTrigger
{
	OnStart = 0,
	OnEnable = 1,
	AfterLevelTransition = 2,
	OnEventTrigger = 3,
	OnPlayerTrigger = 4,
	OnInteract = 5,
	OnLevelTransitionStart = 6,
	OnRoomExit = 7,
	Other = 8,
}

public enum RPGEventType
{
	Generic = 0,
	keyInput = 1,
	BattleInitialize = 2,
	Dialogue = 3,
}