﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetBundlesUtility 
{
	public const string OUTPUT_PATH = "AssetBundles";

	public static string GetPlatformName()
	{
		#if UNITY_EDITOR
		return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
		#else
		return GetPlatformForAssetBundles(Application.platform);
		#endif
	}

	#if UNITY_EDITOR
	public static string GetPlatformForAssetBundles(BuildTarget target)
	{
		switch (target) 
		{
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.iOS:
			return "iOS";
		case BuildTarget.WebGL:
			return "WebGL";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "Windows";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "OSX";
		default:
			return null;
		}
	}
	#endif

	public static string GetPlatformForAssetBundles(RuntimePlatform platform)
	{
		switch (platform) 
		{
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:
			return "iOS";
		case RuntimePlatform.WebGLPlayer:
			return "WebGL";
		case RuntimePlatform.WindowsPlayer:
			return "Windows";
		case RuntimePlatform.OSXPlayer:
			return "OSX";
		default:
			return null;
		}
	}
}