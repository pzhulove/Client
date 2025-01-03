using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GlobalSettingFlatBufferConverter : Editor
{

#if !LOGIC_SERVER

    [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert GlobalSetting", false, 0)]
    static public void ConvertAllGlobalSetting()
    {
        List<string> dstAssetPathList = new List<string>();
        List<string> newAssetPathList = new List<string>();
        string[] assetPathList = Directory.GetFiles("Assets/Resources/Base", "*.asset", SearchOption.AllDirectories);
        dstAssetPathList.AddRange(assetPathList);

        for (int i = 0 ,icnt = dstAssetPathList.Count;i<icnt;++i)
        {
            EditorUtility.DisplayProgressBar("转化全局设置资源", "正在转化第" + i + "个资源...", (i / icnt));
            string path = dstAssetPathList[i];
            GlobalSetting data = AssetDatabase.LoadAssetAtPath<GlobalSetting>(path);
            if(null == data ) continue;

            string newPath = Path.ChangeExtension(path, ".bytes");
            try
            {
                FBGlobalSettingSerializer.SaveFBGlobalSetting(newPath, data);
                newAssetPathList.Add(newPath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogErrorFormat(e.ToString());
            }
        }

        for (int i = 0, icnt = newAssetPathList.Count; i < icnt; ++i)
        {
            string newPath = HeroGo.UtilityTools.GetRawResPathOutOfUnity(newAssetPathList[i]);//.Replace("Assets/", "AssetsAlt/");
            if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            if (File.Exists(newPath))
                File.Delete(newPath);
            File.Move(newAssetPathList[i], newPath);
        }

        EditorUtility.ClearProgressBar();
    }
#endif
}
