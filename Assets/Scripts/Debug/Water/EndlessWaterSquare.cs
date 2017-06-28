using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EndlessWaterSquare : MonoBehaviour 
{
	public GameObject boatObj;

	public GameObject waterSqrObj;

	private float squareWidth = 800f;
	private float innerSquareResolution = 5f;
	private float outerSquareResolution = 25f;

	List<WaterSquare> waterSquares = new List<WaterSquare>();

	float secondsSinceStart;
	Vector3 boatPos;
	Vector3 oceanPos;
	bool hasThreadUpdatedWater;

	void Start()
	{
		CreateEndlessSea ();

		secondsSinceStart = Time.time;

		//ThreadPool.QueueUserWorkItem(new WaitCallback(Upda))
	}

	void Update()
	{
		
	}

	void UpdateWaterWithThreadPooling(object state)
	{
		MoveWaterToBoat ();

		for (int i = 0; i < waterSquares.Count; i++)
		{
			//Vector3 centerPos = waterSquares[i].c
		}
	}

	void MoveWaterToBoat()
	{
		float x = innerSquareResolution * (int)Mathf.Round (boatPos.x / innerSquareResolution);
		float z = innerSquareResolution * (int)Mathf.Round (boatPos.z / innerSquareResolution);

		if (oceanPos.x != x || oceanPos.z != z)
			oceanPos = new Vector3 (x, oceanPos.y, z);
	}

	void CreateEndlessSea()
	{
		AddWaterPlane (0f, 0f, 0f, squareWidth, innerSquareResolution);

		for (int x = -1; x <= 1; x++) 
		{
			for (int z = -1; z <= 1; z++) 
			{
				if (x == 0 && z == 0)
					continue;

				float yPos = -0.5f;
				AddWaterPlane (x * squareWidth, z * squareWidth, yPos, squareWidth, outerSquareResolution);
			}
		}
	}

	void AddWaterPlane(float xCoord, float zCoord, float yPos, float squareWidth, float spacing)
	{
		GameObject waterPlane = Instantiate (waterSqrObj, transform.position, transform.rotation) as GameObject;

		waterPlane.SetActive (true);

		Vector3 centerPos = transform.position;

		centerPos.x += xCoord;
		centerPos.y = yPos;
		centerPos.z += zCoord;

		waterPlane.transform.position = centerPos;

		waterPlane.transform.parent = transform;

		WaterSquare newWaterSquare = new WaterSquare (waterPlane, squareWidth, spacing);

		waterSquares.Add (newWaterSquare);

	}
}