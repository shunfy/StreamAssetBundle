using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class AssetBuild
{
    [MenuItem("Assets/打ab")]
    static void build()
    {
        var obj = Selection.activeObject;
        if (obj != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            string abName = path.Replace(Path.GetExtension(path), ".ab");
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = abName;
            build.assetNames = new string[]{path};
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.Android);

            StreamEncryption.EcryptAssetBundle(Application.streamingAssetsPath + "/" + abName);
        }
    }
}
