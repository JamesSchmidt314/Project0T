using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour 
{
	public float moveForce = 3000;
	public float rotateForce = 1000;
	Rigidbody m_Rigidbody;

	void Start () 
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () 
	{
		m_Rigidbody.AddRelativeForce (Camera.main.transform.forward * Input.GetAxis("Vertical") * moveForce);

		m_Rigidbody.AddTorque (Vector3.up * moveForce * Input.GetAxis ("Horizontal"));
	}
}
