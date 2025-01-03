using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ResMaterialChecker : Editor
{
    /// 替换掉所有的AlphaTest材质
    static protected readonly string[] AlphaTestShaderList = new string[]
    {
            "Cutout",
    };

    static protected readonly string SCENE_RES_PATH = "Assets/Resources/Scene";
    static protected readonly string ACTOR_RES_PATH = "Assets/Resources/Actor";

    [MenuItem("[TM工具集]/ArtTools/Replace Cutout Material")]
    static public void ReplaceCutoutMaterial()
    {
        Shader refShader = AssetShaderLoader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_Transparent");
        if(null == refShader)
        {
            Logger.LogError("Can not find reference shader!");
            return;
        }

        _ReplaceMaterialByDirectory(SCENE_RES_PATH, refShader);
        _ReplaceMaterialByDirectory(ACTOR_RES_PATH, refShader);

        AssetDatabase.SaveAssets();
    }

    static protected void _ReplaceMaterialByDirectory(string resDirPath,Shader refShader)
    {
        string[] prefabList = Directory.GetFiles(resDirPath, "*.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < prefabList.Length; ++i)
        {
            GameObject curPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
            if (null != curPrefab)
            {
                _ModifyChildMaterial(curPrefab, refShader);
                EditorUtility.SetDirty(curPrefab);

                Logger.LogFormat("Find prefab [{0}] with cut-out material!", prefabList[i]);
            }

            Editor.DestroyImmediate(curPrefab, false);
        }
    }

    static protected void _ModifyChildMaterial(GameObject go,Shader refShader)
    {
        int childNum = go.transform.childCount;

        List<Renderer> renderList = new List<Renderer>();
        renderList.AddRange(go.GetComponentsInChildren<MeshRenderer>());
        renderList.AddRange(go.GetComponentsInChildren<SkinnedMeshRenderer>());

        for (int j = 0; j < renderList.Count; ++j)
        {
            Material[] asm = renderList[j].sharedMaterials;
            if(null == asm)
                continue;

            for (int k = 0; k < asm.Length; ++k)
            {
                if(null == asm[k])
                    continue;

                bool bShouldReplace = false;
                for (int m = 0; m < AlphaTestShaderList.Length; ++m)
                    bShouldReplace |= asm[k].shader.name.Contains(AlphaTestShaderList[m]);

                if (bShouldReplace)
                    asm[k].shader = refShader;
            }

            renderList[j].sharedMaterials = asm;
        }

        for (int i = 0; i < childNum; ++ i)
        {
            Transform curChild = go.transform.GetChild(i);
            _ModifyChildMaterial(curChild.gameObject,refShader);
        }
    }
}
