using UnityEngine;
using UnityEditor;
using System.Collections;

public class AssetProxyCooker : Editor
{
    [MenuItem("[TM工具集]/ArtTools/Pre-process Asset Proxy")]
    static public void PrecookAllAssetProxy()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });

        int cnt = 0;
        float total = str.Length;
        foreach (var guid in str)
        {
            EditorUtility.DisplayProgressBar("烘焙资源代理", "正在烘焙第" + cnt + "个资源...", ((cnt++) / total));
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                AssetProxy proxy = temp.GetComponent<AssetProxy>();

                if (null == proxy)
                    proxy = temp.AddComponent<AssetProxy>();
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/Cook Asset Proxy")]
    static public void GenerateAnimDescProxy()
    {
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        for (int i = 0; i < selection.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("烘焙资源代理", "正在烘焙第" + i + "个资源...", ((float)i / selection.Length));
            GameObject data = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == data)
                continue;

            string path = AssetDatabase.GetAssetPath(data);
            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                AssetProxy proxy = temp.GetComponent<AssetProxy>();
                if (null == proxy)
                    proxy = temp.AddComponent<AssetProxy>();
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();
    }



}
