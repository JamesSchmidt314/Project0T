using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour 
{
	void LateUpdate () 
	{
		if (Camera.main != null)
			transform.rotation = Quaternion.LookRotation (Camera.main.transform.TransformDirection(Vector3.back));
	}
}
