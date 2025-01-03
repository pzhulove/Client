using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModelDataFlatBufferConverter : Editor
{
    //[MenuItem("Assets/FB/Convert ModelData")]

#if !LOGIC_SERVER
    static public void ConvertModelData()
    {
        Object[] selection = Selection.GetFiltered(typeof(ScriptableObject), SelectionMode.Assets);

        for (int i = 0; i < selection.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("转化模型资源", "正在转化第" + i + "个资源...", (i / selection.Length));
            DModelData data = selection[i] as DModelData;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == data)
                continue;

            string path = AssetDatabase.GetAssetPath(data);
            string newPath = Path.ChangeExtension(path, ".bytes");

            FBModelDataSerializer.SaveFBModelData(newPath, data);
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("[TM工具集]/[FB转ScriptsObject]/Convert FBModelData", false, 0)]
    static public void ConvertAllModelData()
    {

        List<string> dstAssetPathList = new List<string>();
        List<string> newAssetPathList = new List<string>();
        string[] assetPathList = Directory.GetFiles("Assets/Resources/Actor", "*.asset", SearchOption.AllDirectories);
        dstAssetPathList.AddRange(assetPathList);
        assetPathList = Directory.GetFiles("Assets/Resources/Scene", "*.asset", SearchOption.AllDirectories);
        dstAssetPathList.AddRange(assetPathList);

        for (int i = 0 ,icnt = dstAssetPathList.Count;i<icnt;++i)
        {
            EditorUtility.DisplayProgressBar("转化模型资源", "正在转化第" + i + "个资源...", (i / icnt));
            string path = dstAssetPathList[i];
            DModelData data = AssetDatabase.LoadAssetAtPath<DModelData>(path);
            if(null == data ) continue;

            string newPath = Path.ChangeExtension(path, ".bytes");
            FBModelDataSerializer.SaveFBModelData(newPath, data);
            newAssetPathList.Add(newPath);
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
