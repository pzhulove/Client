using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Tenmove.Runtime.Unity;


namespace Tenmove.Editor.Unity
{
    public class PostprocessSettingCreateMenu
    {
        [MenuItem("Assets/Create/后处理/创建Bloom配置")]
        public static void CreateRadialBlurSetting()
        {
            if (Selection.objects == null || Selection.objects.Length < 1 || AssetDatabase.GetAssetPath(Selection.objects[0]) == string.Empty)
            {
                UnityEngine.Debug.LogError("请选择要处理的文件夹");
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.objects[0]);
            string absolutePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + path;
            // 不是个path
            if (!System.IO.Directory.Exists(absolutePath))
            {
                path = path.Substring(0, path.LastIndexOf("/"));
            }

            var a = ScriptableObject.CreateInstance<Bloom>();
            AssetDatabase.CreateAsset(a, path + "/BloomSetting.asset");
            AssetDatabase.SaveAssets();
        }
    }
}

