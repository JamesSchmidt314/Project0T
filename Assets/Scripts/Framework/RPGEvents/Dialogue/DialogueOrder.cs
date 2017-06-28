using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOrder 
{
	public DialogueOrderType orderType;
	public DialogueStatement statement;
	public DialogueDivergence divergence;
	public DialogueOption option;

	public DialogueOrder(DialogueOrderType _type)
	{
		orderType = _type;
		switch (orderType)
		{
		case DialogueOrderType.Statement:
			statement = new DialogueStatement ();
			break;
		case DialogueOrderType.Divergence:
			divergence = new DialogueDivergence ();
			break;
		case DialogueOrderType.Option:
			option = new DialogueOption ();
			break;
		}
	}
}