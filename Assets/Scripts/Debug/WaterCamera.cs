using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCamera : MonoBehaviour {

	public Transform target;

	void Start () 
	{
		
	}

	void FixedUpdate () 
	{
		transform.position = Vector3.Lerp (transform.position,target.position, Time.deltaTime * 5);
	}
}
