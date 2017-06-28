using UnityEngine;
using System.Collections;
using System.IO;

public class AssetLoader 
{

	#if UNITY_EDITOR
	public static AssetBundle dialogueBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(AssetBundlesUtility.OUTPUT_PATH,  AssetBundlesUtility.GetPlatformName()), "dialogue-bundle"));
	#else
	public static AssetBundle dialogueBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "dialogue-bundle"));
	#endif

	#if UNITY_EDITOR
	public static AssetBundle spriteBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(AssetBundlesUtility.OUTPUT_PATH,  AssetBundlesUtility.GetPlatformName()), "sprite-bundle"));
	#else
	public static AssetBundle spriteBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "sprite-bundle"));
	#endif

	/*#if UNITY_EDITOR
	public static AssetBundle audioBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(AssetBundlesUtility.OUTPUT_PATH,  AssetBundlesUtility.GetPlatformName()), "audio-bundle"));
	#else
	public static AssetBundle audioBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "audio-bundle"));
	#endif*/

	#if UNITY_EDITOR
	public static AssetBundle fontBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(AssetBundlesUtility.OUTPUT_PATH,  AssetBundlesUtility.GetPlatformName()), "font-bundle"));
	#else
	public static AssetBundle fontBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "font-bundle"));
	#endif

	/*#if UNITY_EDITOR
	public static AssetBundle characterBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(AssetBundlesUtility.OUTPUT_PATH,  AssetBundlesUtility.GetPlatformName()), "character-bundle"));
	#else
	public static AssetBundle characterBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "character-bundle"));
	#endif*/

	public static bool HasValidAssetBundles()
	{
		int bundlesCount = 0;
		if (dialogueBundle)
			bundlesCount++;
		if (spriteBundle)
			bundlesCount++;
		if (fontBundle)
			bundlesCount++;
		return(bundlesCount == 3);
	}
}
