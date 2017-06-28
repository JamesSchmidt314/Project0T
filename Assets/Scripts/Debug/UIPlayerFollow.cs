using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerFollow : MonoBehaviour {

	RectTransform rt;
	// Use this for initialization
	void Start ()
	{
		rt = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (OverworldPlayerCharacter.playerCharacter != null)
		{
			Vector2 viewportPoint = Camera.main.WorldToViewportPoint (OverworldPlayerCharacter.playerCharacter.GetMiddlePosition());
			rt.anchorMin = viewportPoint;
			rt.anchorMax = viewportPoint;
		}
	}
}