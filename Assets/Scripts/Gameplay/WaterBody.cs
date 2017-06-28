using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterBody : MonoBehaviour 
{
	public float floatForce = 100.0f;

	public float waveSize = 0.5f;
	public float waveSpeed = 1.3f;
	public float waveFrequency = 0.2f;

	public float length = 10.0f;
	public float width = 10.0f;

	public float uvTilingScaleX = 1;
	public float uvTilingScaleY = 1;

	public int resX = 2;
	public int resZ = 2;

	private MeshFilter m_Filter;

	//private List<Rigidbody> waterRBs = new List<Rigidbody>();
	//private List<ModifyBoatMesh> boatMeshes = new List<ModifyBoatMesh> ();
	private List<WaterObject> waterObjects = new List<WaterObject>();

	void Start ()
	{
		m_Filter = GetComponent<MeshFilter> ();
		//BuildMesh ();
	}
	

	void Update () 
	{
		Vector3[] verts = m_Filter.mesh.vertices;
		for (int i = 0; i < verts.Length; i++)
		{
			float value1 = waveSize * Mathf.Sin((Time.time * waveSpeed) + (verts[i].x * waveFrequency));
			float value2 = waveSize * Mathf.Sin((Time.time * waveSpeed) + (verts[i].z * waveFrequency));
			verts [i].y = value1 + value2;
		}

		m_Filter.mesh.vertices = verts;

		/*for (int i = 0; i < waterTransforms.Length; i++) 
		{
			Vector3 posOnWater = transform.InverseTransformPoint (waterTransforms [i].position);
			float value = (waveSize * Mathf.Sin ((Time.time * waveSpeed) + (posOnWater.x * waveFrequency))) + (waveSize * Mathf.Sin ((Time.time * waveSpeed) + (posOnWater.z * waveFrequency)));
			waterTransforms [i].position = new Vector3 (waterTransforms [i].position.x, value, waterTransforms [i].position.z);
			waterTransforms [i].rotation = Quaternion.FromToRotation (Vector3.up, new Vector3(waveSize * Mathf.Sin((Time.time * waveSpeed) + ((posOnWater.x - width) * waveFrequency)), 10, waveSize * Mathf.Sin((Time.time * waveSpeed) + ((posOnWater.z - length) * waveFrequency))));
		}*/

		for (int i = 0; i < waterObjects.Count; i++) 
		{
			waterObjects [i].UpdateObjectData (this);
		}
	}

	void FixedUpdate()
	{
		for (int i = 0; i < waterObjects.Count; i++) 
		{
			waterObjects [i].FixedUpdateObjectData (floatForce);
		}
	}

	public void BuildMesh()
	{
		m_Filter = GetComponent<MeshFilter> ();
		m_Filter.mesh = new Mesh ();

		Vector3[] vertices = new Vector3[resX * resZ];

		for (int z = 0; z < resZ; z++) 
		{
			float zPos = ((float)z / (resZ - 1) - 0.5f) * length;
			for (int x = 0; x < resX; x++) 
			{
				float xPos = ((float)x / (resX - 1) - 0.5f) * width;
				vertices [x + z * resX] = new Vector3 (xPos, 0f, zPos);
			}
		}

		Vector3[] normals = new Vector3[vertices.Length];
		for (int n = 0; n < normals.Length; n++) 
		{
			normals [n] = Vector3.up;
		}

		Vector2[] uvs = new Vector2[vertices.Length];
		for (int v = 0; v < resZ; v++) 
		{
			for (int u = 0; u < resX; u++) 
			{
				uvs [u + v * resX] = new Vector2 ((float)u / (resX - 1) * uvTilingScaleX, (float)v / (resZ - 1) * uvTilingScaleY);
			}
		}

		int nbFaces = (resX - 1) * (resZ - 1);
		int[] triangles = new int[nbFaces * 6];
		int t = 0;
		for (int face = 0; face < nbFaces; face++) 
		{
			int i = face % (resX - 1) + (face / (resZ - 1) * resX);

			triangles [t++] = i + resX;
			triangles [t++] = i + 1;
			triangles [t++] = i;

			triangles [t++] = i + resX;
			triangles [t++] = i + resX + 1;
			triangles [t++] = i + 1;
		}

		m_Filter.sharedMesh.vertices = vertices;
		m_Filter.sharedMesh.normals = normals;
		m_Filter.sharedMesh.uv = uvs;
		m_Filter.sharedMesh.triangles = triangles;

		m_Filter.sharedMesh.RecalculateBounds();
		Debug.Log ("total vertices: " + m_Filter.mesh.vertices.Length);
	}
	int closestVertex = -1;

	void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			AddNewWaterObject (other.attachedRigidbody);
			//AddNewWaterObject (other.attachedRigidbody);
			//waterRBs.Add (other.attachedRigidbody);
		}
		//waterObjects.Add (other);
		//if (Physics.SphereCast())
		/*for (int i = 0; i < m_Filter.mesh.vertices.Length; i++) 
		{
			if (closestVertex == -1)
				closestVertex = i;
			if (Vector3.Distance (m_Filter.mesh.vertices [i], other.transform.position) < Vector3.Distance (m_Filter.mesh.vertices [closestVertex], 
				other.transform.position))
			{
				closestVertex = i;
				waterIDs.Add (closestVertex);
			}
		}*/
	}

	void OnTriggerExit(Collider other)
	{
		//if (waterObjects.Contains (wo => wo.gameObj == other.gameObject)) 
		if (waterObjects.Find (wo => wo.rb == other.attachedRigidbody) != null)
		{
			RemoveWaterObject (other.attachedRigidbody);
			//RemoveWaterObject (other.attachedRigidbody);
			//waterRBs.Remove (other.attachedRigidbody);
		}
	}

	private void AddNewWaterObject(Rigidbody rb)
	{
		waterObjects.Add (new WaterObject (rb));
	}

	public float GetWaveYPos(Vector3 position, float timeSinceStart)
	{
		Vector3 posOnWater = transform.InverseTransformPoint (position);
		float value = (waveSize * Mathf.Sin ((Time.time * waveSpeed) + (posOnWater.x * waveFrequency))) + (waveSize * Mathf.Sin ((Time.time * waveSpeed) + (posOnWater.z * waveFrequency)));
		return value;
	}

	public float DistanceToWater(Vector3 position, float timeSinceStart)
	{
		float waterHeight = GetWaveYPos (position, timeSinceStart);

		float distanceToWater = position.y - waterHeight;

		return distanceToWater;
	}

	private void RemoveWaterObject(Rigidbody rb)
	{
		waterObjects.Find (wo => wo.rb == rb).DestroyResources ();
		waterObjects.Remove (waterObjects.Find (wo => wo.rb == rb));
	}
}