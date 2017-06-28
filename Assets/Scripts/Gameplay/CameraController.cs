using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : Singleton<CameraController>
{
	public float moveSpeed = 4.0f;
	public float turnSpeed = 2.0f;
	public float tiltMax = 75.0f;
	public float tiltMin = 45.0f;
	public OverworldPlayerCharacter target;
	private Transform pivot;
	private Transform cam;

	private float lookAngle;
	private float tiltAngle;
	private Vector3 pivotEulers;
	private Quaternion pivotTargetRot;
	private Quaternion transformTargetRot;

	private float clipMoveTime = 0.02f;
	private float returnTime = 0.4f;
	private float sphereCastRadius = 0.1f;
	private float closestDistance = 0.5f;
	public bool protecting { get; private set; }
	private string dontClipTag = "Player";

	float orginalDist;
	float moveVelocity;
	float currentDist;
	Ray ray = new Ray();
	RaycastHit[] hits;
	RayHitComparer rayhitComparer;

	new void Awake () 
	{
		base.Awake ();
		cam = GetComponentInChildren<Camera> ().transform;
		pivot = transform.Find ("Pivot");
		pivotEulers = pivot.rotation.eulerAngles;
		pivotTargetRot = pivot.localRotation;
		transformTargetRot = transform.localRotation;

		//collision
		orginalDist = cam.localPosition.magnitude;
		currentDist = orginalDist;

		rayhitComparer = new RayHitComparer ();

	}

	void FixedUpdate()
	{
		FollowTarget (Time.deltaTime);



		float targetDist = orginalDist;

		ray.origin = pivot.position + pivot.forward * sphereCastRadius;
		ray.direction = -pivot.forward;

		Collider[] cols = Physics.OverlapSphere (ray.origin, sphereCastRadius);

		bool initialIntersect = false;
		bool hitSomething = false;

		for (int i = 0; i < cols.Length; i++) 
		{
			if ((!cols [i].isTrigger) && !(cols [i].attachedRigidbody != null && cols [i].attachedRigidbody.CompareTag (dontClipTag))) 
			{
				initialIntersect = true;
				break;
			}
		}

		if (initialIntersect)
		{
			ray.origin += pivot.forward * sphereCastRadius;

			hits = Physics.RaycastAll (ray, orginalDist - sphereCastRadius);
		}
		else
		{
			hits = Physics.SphereCastAll (ray, sphereCastRadius, orginalDist + sphereCastRadius);
		}

		Array.Sort (hits, rayhitComparer);

		float nearest = Mathf.Infinity;

		for (int i = 0; i < hits.Length; i++) 
		{
			if (hits [i].distance < nearest && (!hits [i].collider.isTrigger) &&
				!(hits [i].collider.GetComponent<CharacterController>() != null && 
					hits [i].collider.GetComponent<CharacterController>().CompareTag (dontClipTag)))
			{
				nearest = hits [i].distance;
				targetDist = -pivot.InverseTransformPoint (hits [i].point).z;
				hitSomething = true;
			}
		}

		protecting = hitSomething;
		currentDist = Mathf.SmoothDamp (currentDist, targetDist, ref moveVelocity,
			currentDist > targetDist ? clipMoveTime : returnTime);
		currentDist = Mathf.Clamp (currentDist, closestDistance, orginalDist);
		cam.localPosition = -Vector3.forward * (currentDist - 0.5f);
	}

	void FollowTarget(float deltaTime)
	{
		if (target == null)
			return;
		transform.position = Vector3.Lerp (transform.position, target.transform.position, deltaTime * moveSpeed);
	}

	public void SetTarget(OverworldPlayerCharacter newTarget)
	{
		target = newTarget;
		if (cam.GetComponent<DepthOfField> ().enabled)
			cam.GetComponent<DepthOfField> ().focalTransform = target.transform;
	}

	public void SetInstantTarget(OverworldPlayerCharacter newTarget)
	{
		SetTarget (newTarget);
		transform.position = target.transform.position;
	}

	public void HandleRotationMovement(float x, float y)
	{
		if (Time.timeScale < float.Epsilon)
			return;

		lookAngle += x * turnSpeed;

		transformTargetRot = Quaternion.Euler (0f, lookAngle, 0f);

		tiltAngle -= y * turnSpeed;

		tiltAngle = Mathf.Clamp (tiltAngle, -tiltMin, tiltMax);

		pivotTargetRot = Quaternion.Euler (tiltAngle, pivotEulers.y, pivotEulers.z);

		pivot.localRotation = pivotTargetRot;
		transform.localRotation = transformTargetRot;
	}

	public class RayHitComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit) x).distance.CompareTo(((RaycastHit) y).distance);
		}
	}
}
