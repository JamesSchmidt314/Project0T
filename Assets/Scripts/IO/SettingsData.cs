using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData 
{
	public float musicVolume = 1.0f;
	public bool antiAliasing = true;
	public bool bloom = true;
	public bool depthOfField = true;
	public ShadowResolution shadowResolution = ShadowResolution.VeryHigh;
}
