using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterObject 
{
	public Rigidbody rb;
	public ModifyBoatMesh modifyBoatMesh;

	//debug
	public GameObject underwaterObj;
	private Mesh underWaterMesh;

	public WaterObject(Rigidbody _rb)
	{
		rb = _rb;
		modifyBoatMesh = new ModifyBoatMesh (rb.gameObject);


		//underwaterObj = new GameObject ("underwater object: " + rb.name);
		//underwaterObj.AddComponent<MeshFilter> ();
		//underwaterObj.AddComponent<MeshRenderer> ();
		//underWaterMesh = underwaterObj.GetComponent<MeshFilter> ().mesh;

		//underwaterObj.GetComponent<MeshRenderer> ().material = new Material (Shader.Find("Standard"));
		//underwaterObj.GetComponent<MeshRenderer> ().material.color = Color.red;
		//underwaterObj.GetComponent<MeshRenderer> ().enabled = false;
	}

	public void UpdateObjectData(WaterBody wb)
	{
		modifyBoatMesh.GenerateUnderwaterMesh (wb);

		modifyBoatMesh.DisplayMesh (underWaterMesh, "UnderWater Mesh", modifyBoatMesh.underWaterTriangleData);
	}

	public void FixedUpdateObjectData(float force)
	{
		//underwaterObj.transform.position = rb.transform.position;
		//underwaterObj.transform.rotation = rb.transform.rotation;
		//underwaterObj.transform.localScale = rb.transform.localScale;

		if (modifyBoatMesh.underWaterTriangleData.Count > 0)
			AddUnderwaterForces (force);
	}

	public void DestroyResources()
	{
		GameObject.Destroy (underwaterObj);
	}

	void AddUnderwaterForces(float force)
	{
		List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;

		for (int i = 0; i < underWaterTriangleData.Count; i++) 
		{
			TriangleData triangleData = underWaterTriangleData [i];

			Vector3 buoyancyForce = BuoyancyForce (force, triangleData);

			rb.AddForceAtPosition (buoyancyForce, triangleData.center);

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