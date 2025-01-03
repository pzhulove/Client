using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class ReplaceAssetsName : Editor
{
    private static List<string> renameList = new List<string>();

    [MenuItem("[TM工具集]/修正资源名称")]
    public static void CorrectAssetsName()
    {
        _CorrectAssetName("*.prefab", typeof(GameObject));
        _CorrectAssetName("*.mat", typeof(Material));


        BuildPlayer.SaveReportData("ReplaceAssetsName.CorrectAssetsName", string.Join("\n", renameList));
    }

    
    //修正资源名称
    private static void _CorrectAssetName(string filter, Type type)
    {
        string[] assetPaths = Directory.GetFiles("Assets/Resources", filter, SearchOption.AllDirectories);

        for (int i = 0, icnt = assetPaths.Length; i < icnt; ++i)
        {
            string assetPath = assetPaths[i];
            string assetName = Tenmove.Runtime.Utility.Path.GetFileNameWithoutExtension(assetPath);
            UnityEngine.Object assetObject = AssetDatabase.LoadAssetAtPath(assetPath, type);

            if (assetObject.name != assetName)
            {
                //AssetDatabase.RenameAsset(assetPath, assetName);
                Debugger.LogWarning("assetObject.name:{0}, assetName:{1}", assetObject.name, assetName);
                var metadata = File.ReadAllText($"{assetPath}.meta");
                var m = Regex.Match(metadata, @"guid: ([\w\d]*)\s");
                var guid = m.Groups[1].Value;
                var head = guid.Substring(0, 2);
                var cachePath = Path.Combine("Library", "metadata", head, guid);
                var cacheInfoPath = Path.Combine("Library", "metadata", head, $"{guid}.info");

                Tenmove.Runtime.Utility.File.Delete(cachePath);
                Tenmove.Runtime.Utility.File.Delete(cacheInfoPath);

                renameList.Add(Path.Combine(Directory.GetCurrentDirectory(), assetPath));
                renameList.Add(assetObject.name);
                renameList.Add(assetName);


            }
        }
        AssetDatabase.SaveAssets();

        Debugger.LogWarning("{0} is OK", filter);

    }
}
