using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCamera : MonoBehaviour {

	[SerializeField] private float skyboxScale = 16;

	Vector3 startingPos;

	void Start () 
	{
		startingPos = transform.position;
	}

	void LateUpdate () 
	{
		if (Camera.main != null) 
		{
			transform.rotation = Camera.main.transform.rotation;
			transform.position = startingPos + new Vector3 (Camera.main.transform.position.x / skyboxScale, Camera.main.transform.position.y / skyboxScale, Camera.main.transform.position.z / skyboxScale);
		}
	}
}
