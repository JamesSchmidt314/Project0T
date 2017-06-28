using UnityEngine;
using UnityEditor;
using System.Collections;

public class AssetBundlesMenuItems
{
	[MenuItem("Assets/AssetBundles/Build AssetBundles")]
	static public void BuildAssetBundles()
	{
		AssetBundlesBuildScript.BuildAssetBundles ();
	}
}
