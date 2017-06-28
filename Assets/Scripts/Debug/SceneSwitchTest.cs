using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitchTest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButtonDown ("Fire1"))
		{
			RoomManager.instance.ChangeScene ("Scene01");
		}
	}
}
