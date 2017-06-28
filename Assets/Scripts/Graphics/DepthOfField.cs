using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DepthOfField : PostEffectsBase 
{
	public bool visualizeFocus = false;
	public float focalLength = 10.0f;
	public float focalSize = 0.05f;
	public float aperature = 0.3f;
	public Transform focalTransform = null;
	public float maxBlurSize = 2.0f;
	//use high resolution, with discBlur, high blur sample count, no near blur
	public float foregroundOverlap = 1.0f;

	public Shader dofHdrShader;
	private Material dofHdrMaterial;

	private float focalDistance01 = 10.0f;
	private ComputeBuffer cbDrawArgs;
	private ComputeBuffer cbPoints;
	private float internalBlurWidth = 1.0f;

	private Camera cachedCamera;

	void Awake()
	{
		if (Application.isPlaying)
			enabled = (PlayerDataManager.instance != null && PlayerDataManager.instance.saveFile != null && PlayerDataManager.instance.saveFile.settingsData.depthOfField);
		else
			enabled = true;
	}

	public override bool CheckResources()
	{
		CheckSupport (true);
		if (dofHdrShader == null)
			dofHdrShader = Shader.Find ("Hidden/DepthOfField");
		dofHdrMaterial = CheckShaderAndCreateMaterial (dofHdrShader, dofHdrMaterial);

		if (!isSupported)
			ReportAutoDisable ();

		return isSupported;
	}

	void OnEnable()
	{
		cachedCamera = GetComponent<Camera> ();
		cachedCamera.depthTextureMode |= DepthTextureMode.Depth;
	}

	void OnDisable()
	{
		ReleaseComputeResources ();

		if (dofHdrMaterial)
			DestroyImmediate (dofHdrMaterial);
		dofHdrMaterial = null;
	}

	void ReleaseComputeResources()
	{
		if (cbDrawArgs != null)
			cbDrawArgs.Release ();
		cbDrawArgs = null;
		if (cbPoints != null)
			cbPoints.Release ();
		cbPoints = null;
	}

	void CreateComputeResources()
	{
		if (cbDrawArgs == null)
		{
			cbDrawArgs = new ComputeBuffer (1, 16, ComputeBufferType.IndirectArguments);
			int[] args = new int[4];
			args [0] = 0;
			args [1] = 1;
			args [2] = 2;
			args [3] = 0;
			cbDrawArgs.SetData (args);
		}
		if (cbPoints == null)
			cbPoints = new ComputeBuffer (90000, 12 + 16, ComputeBufferType.Append);
	}

	float FocalDistance01(float worldDist)
	{
		return cachedCamera.WorldToViewportPoint ((worldDist - cachedCamera.nearClipPlane)
		* cachedCamera.transform.forward + cachedCamera.transform.position).z /
		(cachedCamera.farClipPlane - cachedCamera.nearClipPlane);
	}

	private void WriteCoc(RenderTexture fromTo, bool fgDilate)
	{
		dofHdrMaterial.SetTexture ("_FgOverlap", null);

		//if (fg)
		fromTo.MarkRestoreExpected();
		Graphics.Blit (fromTo, fromTo, dofHdrMaterial, 0);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources ()) 
		{
			Graphics.Blit (source, destination);
			return;
		}

		aperature = (aperature < 0.0f) ? 0.0f : aperature;
		maxBlurSize = (maxBlurSize < 0.1f) ? 0.1f : maxBlurSize;
		focalSize = Mathf.Clamp (focalSize, 0.0f, 2.0f);
		internalBlurWidth = Mathf.Max (maxBlurSize, 0.0f);
		focalDistance01 = (focalTransform) ? (cachedCamera.WorldToViewportPoint (focalTransform.position)).z / (cachedCamera.farClipPlane) : FocalDistance01 (focalLength);
		dofHdrMaterial.SetVector ("_CurveParams", new Vector4 (1.0f, focalSize, (1.0f / (1.0f - aperature) - 1.0f), focalDistance01));
		RenderTexture rtLow = null;
		RenderTexture rtLow2 = null;
		RenderTexture rtSuperLow1 = null;
		RenderTexture rtSuperLow2 = null;
		float fgBlurDist = internalBlurWidth * foregroundOverlap;

		if (visualizeFocus) 
		{
			WriteCoc (source, true);
			Graphics.Blit (source, destination, dofHdrMaterial, 16);
		} 
		else 
		{
			source.filterMode = FilterMode.Bilinear;

			internalBlurWidth *= 2.0f;

			WriteCoc(source, true);

			rtLow = RenderTexture.GetTemporary (source.width >> 1, source.height >> 1,0,source.format);
			rtLow2 = RenderTexture.GetTemporary (source.width >> 1, source.height >> 1,0,source.format);

			int blurPass = 17;
			dofHdrMaterial.SetVector ("_Offsets", new Vector4 (0.0f, internalBlurWidth, 0.025f, internalBlurWidth));
			Graphics.Blit (source, destination, dofHdrMaterial, blurPass);

			if (rtLow)
				RenderTexture.ReleaseTemporary (rtLow);
			if (rtLow2)
				RenderTexture.ReleaseTemporary (rtLow2);
		}
	//	if (aperature < 0.0f)
		//	aperature = 0.0f;

		//if (maxBlurSize < 0.1f)
		//	maxBlurSize = 0.1f;
	}
}
