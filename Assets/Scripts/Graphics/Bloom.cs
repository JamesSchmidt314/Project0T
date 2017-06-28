using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class Bloom : PostEffectsBase 
{
	private float bloomIntensity = 0.3f;

	private float threshold = 0.25f;
	private float blurSize = 1.0f;
	private int blurIterations = 1;

	private Shader fastBloomShader;
	private Material fastBloomMaterial = null;

	void Awake()
	{
		//enabled = (PlayerDataManager.instance != null && PlayerDataManager.instance.saveFile != null && PlayerDataManager.instance.saveFile.settingsData.bloom);
		if (Application.isPlaying)
			enabled = (PlayerDataManager.instance != null && PlayerDataManager.instance.saveFile != null && PlayerDataManager.instance.saveFile.settingsData.bloom);
		else
			enabled = true;
	}

	public override bool CheckResources()
	{
		CheckSupport (false);
		if (fastBloomShader == null)
			fastBloomShader = Shader.Find ("Hidden/FastBloom");
		fastBloomMaterial = CheckShaderAndCreateMaterial (fastBloomShader, fastBloomMaterial);

		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}

	void OnDisable()
	{
		if (fastBloomMaterial)
			DestroyImmediate (fastBloomMaterial);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (CheckResources () == false) 
		{
			Graphics.Blit (source, destination);
			return;
		}

		int divider = 4;
		float widthMod = 0.5f;

		fastBloomMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod, 0.0f, threshold, bloomIntensity));
		source.filterMode = FilterMode.Bilinear;

		var rtW = source.width / divider;
		var rtH = source.height / divider;

		RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
		rt.filterMode = FilterMode.Bilinear;
		Graphics.Blit (source, rt, fastBloomMaterial, 1);

		var passOffs = 0;

		for (int i = 0; i < blurIterations; i++) 
		{
			fastBloomMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod + (i * 1.0f), 0.0f, threshold, bloomIntensity));

			RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, fastBloomMaterial, 2 + passOffs);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;

			rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
			Graphics.Blit (rt, rt2, fastBloomMaterial, 3 + passOffs);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
		}

		fastBloomMaterial.SetTexture ("_Bloom", rt);
		Graphics.Blit (source, destination, fastBloomMaterial, 0);
		RenderTexture.ReleaseTemporary (rt);
	}
}
