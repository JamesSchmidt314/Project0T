using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPhysics : MonoBehaviour 
{
	public GameObject underWaterObj;

	private ModifyBoatMesh modifyBoatMesh;

	private Mesh underWaterMesh;

	private Rigidbody boatRB;

	private float rhoWater = 1027f;

	void Start()
	{
		boatRB = GetComponent<Rigidbody> ();

		modifyBoatMesh = new ModifyBoatMesh (gameObject);

		underWaterMesh = underWaterObj.GetComponent<MeshFilter> ().mesh;
	}

	void Update()
	{
		//modifyBoatMesh.GenerateUnderwaterMesh ();

		modifyBoatMesh.DisplayMesh (underWaterMesh, "UnderWater Mesh", modifyBoatMesh.underWaterTriangleData);
	}

	void FixedUpdate()
	{
		if (modifyBoatMesh.underWaterTriangleData.Count > 0)
			AddUnderwaterForces ();
	}


	void AddUnderwaterForces()
	{
		List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;

		for (int i = 0; i < underWaterTriangleData.Count; i++) 
		{
			TriangleData triangleData = underWaterTriangleData [i];

			Vector3 buoyancyForce = BuoyancyForce (rhoWater, triangleData);

			boatRB.AddForceAtPosition (buoyancyForce, triangleData.center);

			//debug
			Debug.DrawRay(triangleData.center, triangleData.normal * 4f, Color.white);
			Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -4f, Color.blue);
		}
	}

	private Vector3 BuoyancyForce(float rho, TriangleData triangleData)
	{
		Vector3 buoyancyForce = rho * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal;
		buoyancyForce.x = 0f;
		buoyancyForce.z = 0f;
		return buoyancyForce;
	}
}
