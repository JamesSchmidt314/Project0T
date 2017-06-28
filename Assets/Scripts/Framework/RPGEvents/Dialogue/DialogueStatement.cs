using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueStatement
{
	public DialogueFlavor statementFlavor;
	public string statementKey;
	public AudioClip optionalVoiceClip;
	public Animator currentlySpeakingActor;
	public bool unskippable = false;
}