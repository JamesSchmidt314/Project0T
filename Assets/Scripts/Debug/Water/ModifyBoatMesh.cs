using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyBoatMesh 
{
	private Transform boatTrans;

	Vector3[] boatVertices;

	int[] boatTriangles;

	public Vector3[] boatVerticesGlobal;

	float[] allDistancesToWater;

	public List<TriangleData> underWaterTriangleData = new List<TriangleData> ();

	public ModifyBoatMesh (GameObject boatObj)
	{
		boatTrans = boatObj.transform;

		boatVertices = boatObj.GetComponent<MeshFilter> ().mesh.vertices;
		boatTriangles = boatObj.GetComponent<MeshFilter> ().mesh.triangles;

		boatVerticesGlobal = new Vector3[boatVertices.Length];

		allDistancesToWater = new float[boatVertices.Length];
	}

	public void GenerateUnderwaterMesh(WaterBody wb)
	{
		underWaterTriangleData.Clear ();

		for (int i = 0; i < boatVertices.Length; i++) 
		{
			Vector3 globalPos = boatTrans.TransformPoint (boatVertices [i]);

			boatVerticesGlobal [i] = globalPos;

			allDistancesToWater [i] = wb.DistanceToWater (globalPos, Time.time);
		}

		AddTriangles (wb);
	}

	private void AddTriangles(WaterBody wb)
	{
		List<VertexData> vertexDatas = new List<VertexData> ();

		vertexDatas.Add (new VertexData ());
		vertexDatas.Add (new VertexData ());
		vertexDatas.Add (new VertexData ());

		int i = 0;

		while (i < boatTriangles.Length) 
		{
			for (int x = 0; x < 3; x++)
			{
				vertexDatas [x].distance = allDistancesToWater [boatTriangles [i]];

				vertexDatas [x].index = x;

				vertexDatas [x].globalVertexPos = boatVerticesGlobal [boatTriangles [i]];

				i++;
			}

			if (vertexDatas [0].distance > 0 && vertexDatas [1].distance > 0f && vertexDatas [2].distance > 0f) 
			{
				continue;
			}

			if (vertexDatas [0].distance < 0f && vertexDatas [1].distance < 0f && vertexDatas [2].distance < 0f) {
				Vector3 p1 = vertexDatas [0].globalVertexPos;
				Vector3 p2 = vertexDatas [1].globalVertexPos;
				Vector3 p3 = vertexDatas [2].globalVertexPos;

				underWaterTriangleData.Add (new TriangleData (p1, p2, p3, wb));
			}
			else 
			{
				vertexDatas.Sort ((x, y) => x.distance.CompareTo (y.distance));
				vertexDatas.Reverse ();

				if (vertexDatas [0].distance > 0f && vertexDatas [1].distance < 0f && vertexDatas [2].distance < 0f) 
				{
					AddTrianglesOneAboveWater (vertexDatas, wb);
				}
				else if (vertexDatas [0].distance > 0f && vertexDatas [1].distance > 0f && vertexDatas [2].distance < 0f) 
				{
					AddTrianglesTwoAboveWater (vertexDatas, wb);
				}
			}
		}
	}

	private void AddTrianglesOneAboveWater(List<VertexData> vertexDatas, WaterBody wb)
	{
		Vector3 H = vertexDatas [0].globalVertexPos;

		int M_Index = vertexDatas [0].index - 1;
		if (M_Index < 0)
			M_Index = 2;

		float h_H = vertexDatas [0].distance;
		float h_M = 0f;
		float h_L = 0f;

		Vector3 M = Vector3.zero;
		Vector3 L = Vector3.zero;

		if (vertexDatas [1].index == M_Index) 
		{
			M = vertexDatas [1].globalVertexPos;
			L = vertexDatas [2].globalVertexPos;

			h_M = vertexDatas [1].distance;
			h_L = vertexDatas [2].distance;
		}
		else
		{
			M = vertexDatas [2].globalVertexPos;
			L = vertexDatas [1].globalVertexPos;

			h_M = vertexDatas [2].distance;
			h_L = vertexDatas [1].distance;
		}

		Vector3 MH = H - M;

		float t_M = -h_M / (h_H - h_M);

		Vector3 MI_M = t_M * MH;

		Vector3 I_M = MI_M + M;


		Vector3 LH = H - L;

		float t_L = -h_L / (h_H - h_L);

		Vector3 LI_L = t_L * LH;

		Vector3 I_L = LI_L + L;

		underWaterTriangleData.Add (new TriangleData (M, I_M, I_L, wb));
		underWaterTriangleData.Add (new TriangleData (M, I_L, L, wb));
	}

	private void AddTrianglesTwoAboveWater(List<VertexData> vertexDatas, WaterBody wb)
	{
		Vector3 L = vertexDatas [2].globalVertexPos;

		int H_Index = vertexDatas [2].index + 1;
		if (H_Index > 2)
			H_Index = 0;

		float h_L = vertexDatas [2].distance;
		float h_H = 0f;
		float h_M = 0f;

		Vector3 H = Vector3.zero;
		Vector3 M = Vector3.zero;

		if (vertexDatas [1].index == H_Index) 
		{
			H = vertexDatas [1].globalVertexPos;
			M = vertexDatas [0].globalVertexPos;

			h_H = vertexDatas [1].distance;
			h_M = vertexDatas [0].distance;
		}
		else
		{
			H = vertexDatas [0].globalVertexPos;
			M = vertexDatas [1].globalVertexPos;

			h_H = vertexDatas [0].distance;
			h_M = vertexDatas [1].distance;
		}

		Vector3 LM = H - L;

		float t_M = -h_L / (h_M - h_L);

		Vector3 LJ_M = t_M * LM;

		Vector3 J_M = LJ_M + L;


		Vector3 LH = H - L;

		float t_H = -h_L / (h_H - h_L);

		Vector3 LJ_H = t_H * LH;

		Vector3 J_H = LJ_H + L;

		underWaterTriangleData.Add (new TriangleData (L, J_H, J_M, wb));
	}

	private class VertexData
	{
		public float distance;

		public int index;

		public Vector3 globalVertexPos;
	}

	public void DisplayMesh(Mesh mesh, string name, List<TriangleData> triangleData)
	{
		List<Vector3> vertices = new List<Vector3> ();
		List<int> triangles = new List<int> ();

		for (int i = 0; i < triangleData.Count; i++) 
		{
			Vector3 p1 = boatTrans.InverseTransformPoint (triangleData [i].p1);
			Vector3 p2 = boatTrans.InverseTransformPoint (triangleData [i].p2);
			Vector3 p3 = boatTrans.InverseTransformPoint (triangleData [i].p3);

			vertices.Add (p1);
			triangles.Add (vertices.Count - 1);

			vertices.Add (p2);
			triangles.Add (vertices.Count - 1);

			vertices.Add (p3);
			triangles.Add (vertices.Count - 1);
		}

		//mesh.Clear ();

		//mesh.name = name;
		//mesh.vertices = vertices.ToArray ();
		//mesh.triangles = triangles.ToArray ();

		//mesh.RecalculateBounds ();
	}
}
