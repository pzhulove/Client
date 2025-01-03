using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

// public class SceneMaterialNormalizer : Editor
// {
//     /// 替换掉所有的AlphaTest材质
//     static protected readonly string SCENE_RES_PATH = "Assets/Resources/Scene";
//     static protected Shader m_RefTransparent = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
//     static protected Shader m_RefOpaque = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Opaque");
// 
//     [MenuItem("Assets/Normalize Material")]
//     static public void ReplaceCutoutMaterial()
//     {
//         if(null == m_RefTransparent)
//         {
//             Logger.LogError("Can not find reference shader!");
//             return;
//         }
//         if (null == m_RefOpaque)
//         {
//             Logger.LogError("Can not find reference shader!");
//             return;
//         }
// 
//         //string[] prefabList = Directory.GetFiles(SCENE_RES_PATH, "*.prefab", SearchOption.TopDirectoryOnly);
//         Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);
// 
//         for (int i = 0; i < selection.Length; ++i)
//         {
//             GameObject curPrefab = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
//             if(null == curPrefab)
//                 continue;
// 
//             if(curPrefab.name.ToLower().Contains("effui_"))
//                 continue;
// 
//             string curPrefabName = curPrefab.name;
//             List<Material> materialList = new List<Material>();
//             List<string> oldMatList = new List<string>();
// 
//             if (null != curPrefab)
//             {
//                 _ModifyChildMaterial(curPrefab, curPrefabName,ref materialList,ref oldMatList);
//                 EditorUtility.SetDirty(curPrefab);
// 
//                 Logger.LogFormat("Find prefab [{0}] with cut-out material!", curPrefab.name);
//             }
// 
//             string subPrefabPath = AssetDatabase.GetAssetPath(curPrefab);
//             if (null != subPrefabPath)
//             {
//                 string[] subPrefabs = Directory.GetFiles(Path.GetDirectoryName(subPrefabPath), "*.prefab", SearchOption.AllDirectories);
//                 for(int j = 0; j < subPrefabs.Length; ++ j)
//                 {
//                     if(subPrefabPath == subPrefabs[j])
//                         continue;
// 
//                     GameObject curSubPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(subPrefabs[j]);
//                     _ModifyChildMaterial(curSubPrefab, curPrefabName, ref materialList, ref oldMatList);
//                 }
//             }
// 
//             Editor.DestroyImmediate(curPrefab, false);
// 
//             for (int j = 0; j < oldMatList.Count; ++j)
//                 AssetDatabase.DeleteAsset(oldMatList[j]);
//         }
// 
//         AssetDatabase.SaveAssets();
//     }
// 
// 
//     static protected Material _AddMaterialToList(string prefabName,Material refMaterial,bool bIsTransparent,ref List<Material> matList,ref List<string> oldMatList)
//     {
//         Texture albedoTex = refMaterial.GetTexture("_MainTex");
//         Vector2 tilling = refMaterial.GetTextureOffset("_MainTex");
//         Vector2 scaling = refMaterial.GetTextureScale("_MainTex");
// 
//         Color dyeColor = Color.white;
//         if(refMaterial.HasProperty("_DyeColor"))
//             dyeColor = refMaterial.GetColor("_DyeColor");
// 
//         int opaqueCnt = 0;
//         int transparentCnt = 0;
// 
//         for (int i = 0; i < matList.Count; ++ i)
//         {
//             Material curMaterial = matList[i];
// 
//             if (curMaterial.name.Contains("transparent"))
//                 ++transparentCnt;
//             else
//                 ++opaqueCnt;
// 
//             if (null == curMaterial)
//             {
//                 Logger.LogError("Material missing!!!!");
//                 continue;
//             }
// 
//             if (curMaterial.GetTexture("_MainTex") == albedoTex &&
//                 curMaterial.GetTextureOffset("_MainTex") == tilling &&
//                 curMaterial.GetTextureScale("_MainTex") == scaling && 
//                 curMaterial.GetColor("_DyeColor") == dyeColor &&
//                 curMaterial.name.Contains("transparent") == bIsTransparent)
//                 return curMaterial;
//         }
// 
//         Material newMat = new Material(refMaterial);
//         if (bIsTransparent)
//             newMat.shader = m_RefTransparent;
//         else
//             newMat.shader = m_RefOpaque;
// 
//         string matName = null;
//         if (prefabName.StartsWith("M_"))
//             matName = prefabName;
//         else if (prefabName.StartsWith("P_"))
//             matName = prefabName.Substring(2);
//         else
//             matName = "M_" + prefabName;
//         matName = matName + (bIsTransparent ? "_transparent" + transparentCnt.ToString("00") : "_opaque" + opaqueCnt.ToString("00"));
//         newMat.name = matName;
// 
//         string oldMatPath = AssetDatabase.GetAssetPath(refMaterial);
//         string refMatPath = Path.Combine(Path.GetDirectoryName(oldMatPath), matName).Replace('\\','/') + ".mat";
//         if(!File.Exists(refMatPath))
//             AssetDatabase.CreateAsset(newMat, refMatPath );
// 
//         Material loadFromFile = AssetDatabase.LoadAssetAtPath<Material>(refMatPath);
//         if(null != loadFromFile)
//             matList.Add(loadFromFile);
// 
//         bool needRemove = true;
//         for (int i = 0; i < matList.Count; ++ i)
//         {
//             if(AssetDatabase.GetAssetPath(matList[i]) == oldMatPath)
//             {
//                 needRemove = false;
//                 break;
//             }
//         }
// 
//         if (!oldMatList.Contains(oldMatPath) && needRemove)
//             oldMatList.Add(oldMatPath);
// 
//         return loadFromFile;
//     }
// 
//     static protected void _ModifyPrefabMaterial(GameObject go, string prefabName, ref List<Material> matList, ref List<string> oldMatList)
//     {
//         List<Renderer> renderList = new List<Renderer>();
//         renderList.AddRange(go.GetComponents<MeshRenderer>());
//         renderList.AddRange(go.GetComponents<SkinnedMeshRenderer>());
// 
//         for (int j = 0; j < renderList.Count; ++j)
//         {
//             Material[] asm = renderList[j].sharedMaterials;
//             if (null == asm)
//                 continue;
// 
//             string curGoName = renderList[j].gameObject.name.ToLower();
//             bool bIsTransparent = !(curGoName.Contains("ground") || curGoName.Contains("background"));
// 
//             for (int k = 0; k < asm.Length; ++k)
//             {
//                 if (null == asm[k])
//                     continue;
// 
//                 Material newMat = _AddMaterialToList(prefabName, asm[k], bIsTransparent, ref matList, ref oldMatList);
//                 if (null != newMat)
//                     asm[k] = newMat;
//             }
// 
//             renderList[j].sharedMaterials = asm;
// 
//             EditorUtility.SetDirty(renderList[j].gameObject);
//         }
//     }
// 
//     static protected void _ModifyChildMaterial(GameObject go,string prefabName,ref List<Material> matList, ref List<string> oldMatList)
//     {
//         int childNum = go.transform.childCount;
// 
//         _ModifyPrefabMaterial(go,prefabName,ref matList,ref oldMatList);
// //         List<Renderer> renderList = new List<Renderer>();
// //         renderList.AddRange(go.GetComponents<MeshRenderer>());
// //         renderList.AddRange(go.GetComponents<SkinnedMeshRenderer>());
// // 
// //         for (int j = 0; j < renderList.Count; ++j)
// //         {
// //             Material[] asm = renderList[j].sharedMaterials;
// //             if(null == asm)
// //                 continue;
// // 
// //             string curGoName = renderList[j].gameObject.name.ToLower();
// //             bool bIsTransparent = !(curGoName.Contains("ground") || curGoName.Contains("background"));
// // 
// //             for (int k = 0; k < asm.Length; ++k)
// //             {
// //                 if(null == asm[k])
// //                     continue;
// // 
// //                 Material newMat = _AddMaterialToList(prefabName, asm[k], bIsTransparent, ref matList, ref oldMatList);
// //                 if (null != newMat)
// //                     asm[k] = newMat;
// //             }
// // 
// //             renderList[j].sharedMaterials = asm;
// // 
// //             EditorUtility.SetDirty(renderList[j].gameObject);
// //         }
// 
//         for (int i = 0; i < childNum; ++ i)
//         {
//             Transform curChild = go.transform.GetChild(i);
//             _ModifyChildMaterial(curChild.gameObject, prefabName,ref matList, ref oldMatList);
//         }
//     }
// }




public class SceneMaterialNormalizer : Editor
{
    /// 替换掉所有的AlphaTest材质
    static protected readonly string SCENE_RES_PATH = "Assets/Resources/Scene";
    static protected Shader m_RefTransparent = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
    static protected Shader m_RefOpaque = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Opaque");

    [MenuItem("Assets/Normalize Material")]
    static public void ReplaceCutoutMaterial()
    {
        if (null == m_RefTransparent)
        {
            Logger.LogError("Can not find reference shader!");
            return;
        }
        if (null == m_RefOpaque)
        {
            Logger.LogError("Can not find reference shader!");
            return;
        }

        //string[] prefabList = Directory.GetFiles(SCENE_RES_PATH, "*.prefab", SearchOption.TopDirectoryOnly);
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets);

        for (int i = 0; i < selection.Length; ++i)
        {
            GameObject curPrefab = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null == curPrefab)
                continue;

            if (curPrefab.name.ToLower().Contains("effui_") || curPrefab.name.ToLower().Contains("eff_"))
                continue;

            string curPrefabName = curPrefab.name;

            if (null != curPrefab)
            {
                _ModifyChildMaterial(curPrefab, curPrefabName);
                EditorUtility.SetDirty(curPrefab);

                Logger.LogFormat("Find prefab [{0}] with cut-out material!", curPrefab.name);
            }

            string subPrefabPath = AssetDatabase.GetAssetPath(curPrefab);
            if (null != subPrefabPath)
            {
                string[] subPrefabs = Directory.GetFiles(Path.GetDirectoryName(subPrefabPath), "*.prefab", SearchOption.AllDirectories);
                for (int j = 0; j < subPrefabs.Length; ++j)
                {
                    if (subPrefabPath == subPrefabs[j])
                        continue;

                    GameObject curSubPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(subPrefabs[j]);
                    _ModifyChildMaterial(curSubPrefab, curPrefabName);
                }
            }

            Editor.DestroyImmediate(curPrefab, false);
        }

        AssetDatabase.SaveAssets();
    }

    static protected void _ModifyPrefabMaterial(GameObject go, string prefabName)
    {
        List<Renderer> renderList = new List<Renderer>();
        renderList.AddRange(go.GetComponents<MeshRenderer>());
        renderList.AddRange(go.GetComponents<SkinnedMeshRenderer>());

        for (int j = 0; j < renderList.Count; ++j)
        {
            Material[] asm = renderList[j].sharedMaterials;
            if (null == asm)
                continue;

            string curGoName = renderList[j].gameObject.name.ToLower();
            bool bIsTransparent = !(curGoName.ToLower().Contains("background") || go.CompareTag("Ground"));

            Material loadFromFile = null;
            if (!bIsTransparent)
            {
                Material newMat = new Material(asm[0]);
                newMat.shader = m_RefOpaque;

                string matName = null;
                if (prefabName.StartsWith("M_"))
                    matName = prefabName;
                else if (prefabName.StartsWith("P_"))
                    matName = prefabName.Substring(2);
                else
                    matName = "M_" + prefabName;
                matName = matName + "_ground";
                newMat.name = matName;

                string oldMatPath = AssetDatabase.GetAssetPath(asm[0]);
                string refMatPath = Path.Combine(Path.GetDirectoryName(oldMatPath), matName).Replace('\\', '/') + ".mat";
                if (!File.Exists(refMatPath))
                    AssetDatabase.CreateAsset(newMat, refMatPath);

                loadFromFile = AssetDatabase.LoadAssetAtPath<Material>(refMatPath);
            }

            for (int k = 0; k < asm.Length; ++k)
            {
                if (null == asm[k])
                    continue;

                if(null != loadFromFile)
                    asm[k] = loadFromFile;
            }

            renderList[j].sharedMaterials = asm;

            EditorUtility.SetDirty(renderList[j].gameObject);
        }
    }

    static protected void _ModifyChildMaterial(GameObject go, string prefabName)
    {
        int childNum = go.transform.childCount;

        _ModifyPrefabMaterial(go, prefabName);

        for (int i = 0; i < childNum; ++i)
        {
            Transform curChild = go.transform.GetChild(i);
            _ModifyChildMaterial(curChild.gameObject, prefabName);
        }
    }
}
