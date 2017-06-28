using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Antialiasing : PostEffectsBase
{
	public bool showGeneratedNormals = false;
	public float offsetScale = 0.2f;
	public float blurRadius = 18.0f;

	public float edgeThresholdMin = 0.05f;
	public float edgeThreshold = 0.2f;
	public float edgeSharpness = 4.0f;

	//public bool dlaaSharp = false;

	//public Shader ssaaShader;
	//private Material ssaa;
	public Shader shaderFXAAII;
	private Material materialFXAAII;

	void Awake()
	{
		if (Application.isPlaying)
			enabled = (PlayerDataManager.instance != null && PlayerDataManager.instance.saveFile != null && PlayerDataManager.instance.saveFile.settingsData.antiAliasing);
		else
			enabled = true;
	}

	public Material CurrentAAMaterials()
	{
		return materialFXAAII;
	}

	public override bool CheckResources()
	{
		CheckSupport (false);

		materialFXAAII = CreateMaterial (shaderFXAAII, materialFXAAII);
		/*if (!ssaaShader.isSupported) 
		{
			NotSupported ();
			ReportAutoDisable ();
		}*/
		return isSupported;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources ()) 
		{
			Graphics.Blit (source, destination);
			return;
		}

		Graphics.Blit (source, destination, materialFXAAII);
	}
}
