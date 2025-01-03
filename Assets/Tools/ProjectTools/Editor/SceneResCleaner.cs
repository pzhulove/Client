using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SceneResCleaner : Editor
{
    static readonly string SCENE_RESOURCE_DATA_PATH = "Assets/Resources/Data/SceneData";
    static readonly string SCENE_RESOURCE_PATH = "Assets/Resources/Scene";

    static List<string> m_TexListInUse = new List<string>();
    static List<string> m_TexListUnused = new List<string>();

    static List<Object> m_SceneDataList = new List<Object>();


    [MenuItem("[TM工具集]/ArtTools/清理场景资源")]
    static void ClearSceneRes()
    {
        _LoadSceneData();

        List<Object> dependLst = new List<Object>();
        Object[] dependency = EditorUtility.CollectDependencies(m_SceneDataList.ToArray());
        for (int i = 0, icnt = dependency.Length; i < icnt; ++i)
        {
            Object cur = dependency[i];
            if (null == cur) continue;

            if(cur is Texture)
            {
                m_TexListInUse.Add(AssetDatabase.GetAssetPath(cur));
            }
        }

        string[] textAll = AssetDatabase.FindAssets("t:texture", new string[] { SCENE_RESOURCE_PATH });
        int jcnt = m_TexListInUse.Count;
        for (int i = 0, icnt = textAll.Length; i < icnt; ++i)
        {
            string texture = AssetDatabase.GUIDToAssetPath(textAll[i]);
            bool bHasFind = false;
            for(int j = 0;j<jcnt;++j)
            {
                if(m_TexListInUse[j] == texture)
                {
                    bHasFind = true;
                    break;
                }
            }

            if(bHasFind) continue;

            m_TexListUnused.Add(texture);
        }

        if (m_TexListUnused.Count > 0)
            AssetDatabase.ExportPackage(m_TexListUnused.ToArray(), "没有使用的场景贴图备份.unitypackage");

        foreach (var path in m_TexListUnused)
        {
            File.Delete(path);
        }
    }

    static void _LoadSceneData()
    {
        string[] sceneDataList = Directory.GetFiles(SCENE_RESOURCE_DATA_PATH, "*.asset", SearchOption.AllDirectories);
        for (int i = 0, icnt = sceneDataList.Length; i < icnt; ++i)
            sceneDataList[i] = sceneDataList[i].Replace('\\','/');

        for (int i = 0, icnt = sceneDataList.Length; i < icnt; ++i)
        {
            DSceneData curScene = AssetDatabase.LoadAssetAtPath<DSceneData>(sceneDataList[i]);
            if (null != curScene)
                m_SceneDataList.Add(curScene);
        }

        string[] prefabAll = AssetDatabase.FindAssets("t:prefab", new string[] { SCENE_RESOURCE_PATH });
        List<GameObject> resLst = new List<GameObject>();
        for (int i = 0, icnt = prefabAll.Length; i < icnt; ++i)
        {
            UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabAll[i]));
            if(null != root)
                m_SceneDataList.Add(root);
        }
    }

    [MenuItem("Assets/深度清理场景资源")]
    static void ClearSceneResDeep()
    {
        string topLevelPath = "";
        Object[] top = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        if (null != top && null != top[0])
        {
            topLevelPath = Path.GetDirectoryName( AssetDatabase.GetAssetPath(top[0]) );
        }

        string[] selection = AssetDatabase.FindAssets("t:model", new string[] { topLevelPath });
        
        Dictionary< string , List<string> > fbxMap = new Dictionary<string, List<string>>();
        for (int i = 0, icnt = selection.Length; i < icnt; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(selection[i]);
            fbxMap.Add(path, new List<string>());
        }

        string[] allprefab = AssetDatabase.FindAssets("t:prefab", new string[] { topLevelPath });
        for (int i = 0, icnt = allprefab.Length; i < icnt; ++i)
        {
            string guid = allprefab[i];

            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject data = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;

            _RemoveDisabledObject(temp.transform);

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
        }

        for (int i = 0, icnt = allprefab.Length; i < icnt; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(allprefab[i]);
            UnityEngine.GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            string[] deps = AssetDatabase.GetDependencies(path);
            for (int j = 0, jcnt = deps.Length; j < jcnt; ++j)
            {
                string deppath = deps[j];
                List<string> curList = null;
                if (fbxMap.TryGetValue(deppath, out curList))
                    curList.Add(path);
            }
        }



        //         for (int i = 0,icnt = fbxList.Count ; i < icnt ; ++ i)
        //         {
        // 
        //         }
        // 
        //         for (int i = 0, icnt = selection.Length; i < icnt; ++i)
        //         {
        //             EditorUtility.DisplayProgressBar("添加图片控件预加载代理", "正在添加第" + i + "个资源...", ((i++) / icnt));
        //             UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabAll[i]));
        //         }
        // 
        //             // FBX
        //             List <>
        // 
        //         // 外部纹理
        //         for(int i = 0,)
    }


    static void _RemoveDisabledObject(Transform parent)
    {
        if (null == parent)
            return;

        Transform[] children = parent.GetComponentsInChildren<Transform>(true);
        for (int i = 0,icnt = children.Length;i<icnt;++i)
        {
            Transform cur = children[i];
            if(cur == parent) continue;

            if (null == cur)
                continue;

            if (!cur.gameObject.activeSelf)
                DestroyImmediate(cur.gameObject);
        }
    }

}
