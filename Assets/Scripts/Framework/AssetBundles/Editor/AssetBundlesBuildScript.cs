using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetBundlesBuildScript 
{
	public static void BuildAssetBundles()
	{
		string outputPath = Path.Combine (AssetBundlesUtility.OUTPUT_PATH, AssetBundlesUtility.GetPlatformName ());
		if (!Directory.Exists (outputPath))
			Directory.CreateDirectory (outputPath);

		BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
	}

	public static void BuildPlayer()
	{
		var outputPath = EditorUtility.SaveFolderPanel ("Choose Location of the Built Game", "", "");
		if (outputPath.Length == 0)
			return;

		string[] scenes = GetScenesFromBuildSettings ();
		if (scenes.Length == 0)
		{
			Debug.Log ("Nothing to Build");
			return;
		}

		string targetName = GetBuildTargetName (EditorUserBuildSettings.activeBuildTarget);
		if (targetName == null)
			return;

		AssetBundlesBuildScript.BuildAssetBundles ();
		AssetDatabase.Refresh ();

		BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
		BuildPipeline.BuildPlayer (scenes, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
	}

	public static void BuildStandalonePlayer()
	{
		var outputPath = EditorUtility.SaveFolderPanel ("Choose Location of the Built Game", "", "");
		if (outputPath.Length == 0)
			return;

		string[] scenes = GetScenesFromBuildSettings ();
		if (scenes.Length == 0)
		{
			Debug.Log ("Nothing to Build");
			return;
		}

		string targetName = GetBuildTargetName (EditorUserBuildSettings.activeBuildTarget);
		if (targetName == null)
			return;

		AssetBundlesBuildScript.BuildAssetBundles ();
		AssetBundlesBuildScript.CopyAssetBundlesTo (Path.Combine (Application.streamingAssetsPath, AssetBundlesUtility.OUTPUT_PATH));
		AssetDatabase.Refresh ();

		BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
		BuildPipeline.BuildPlayer (scenes, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
	}

	public static string GetBuildTargetName(BuildTarget target)
	{
		switch (target)
		{
		case BuildTarget.Android:
			return "/test.apk";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "/test.exe";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "/test.app";
		case BuildTarget.WebGL:
			return "";
		default:
			Debug.Log ("Target not implemented.");
			return null;
		}
	}

	static void CopyAssetBundlesTo(string outputPath)
	{
		FileUtil.DeleteFileOrDirectory (Application.streamingAssetsPath);
		Directory.CreateDirectory (outputPath);

		string outputFolder = AssetBundlesUtility.GetPlatformName ();

		var source = Path.Combine (Path.Combine (System.Environment.CurrentDirectory, AssetBundlesUtility.OUTPUT_PATH), outputFolder);
		if (!System.IO.Directory.Exists (source))
			Debug.Log ("No assetBundle output folder, try to build the assetBundles first.");

		var destination = System.IO.Path.Combine (outputPath, outputFolder);
		if (System.IO.Directory.Exists (destination))
			FileUtil.DeleteFileOrDirectory (destination);

		FileUtil.CopyFileOrDirectory (source, destination);
	}

	static string[] GetScenesFromBuildSettings()
	{
		List<string> scenes = new List<string> ();
		for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) 
		{
			if (EditorBuildSettings.scenes[i].enabled)
				scenes.Add (EditorBuildSettings.scenes [i].path);
		}

		return scenes.ToArray ();
	}
}