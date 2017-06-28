using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSquare
{
	public Transform squareTransform;

	public MeshFilter terrainMeshFilter;

	private float size;

	public float spacing;

	private int width;

	public Vector3 centerPos;

	public Vector3[] vertices;

	public WaterSquare(GameObject waterSquareObj, float size, float spacing)
	{
		this.squareTransform = waterSquareObj.transform;
		this.size = size;
		this.spacing = spacing;

		this.terrainMeshFilter = squareTransform.GetComponent<MeshFilter> ();

		width = (int)(size / spacing);
		width += 1;

		float offset = -((width - 1) * spacing) / 2;

		Vector3 newPos = new Vector3 (offset, squareTransform.position.y, offset);

		squareTransform.position += newPos;

		this.centerPos = waterSquareObj.transform.localPosition;

		float startTime = System.Environment.TickCount;

		//generateMesh();
	}
}
