#if UNITY_EDITOR
#define DealDoor
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

[CustomEditor(typeof(MeshToSpriteRenderTool))]
public class MeshToSpriteRenderToolEditor : Editor
{
    private MeshToSpriteRenderTool m_SelfCs;

    private void OnEnable()
    {
        m_SelfCs = target as MeshToSpriteRenderTool;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        m_SelfCs.OddOffset = GUILayout.Toggle(m_SelfCs.OddOffset, m_SelfCs.OddOffset ? "负向偏移" : "正向偏移");
        m_SelfCs.SplitTexFlipX =
            GUILayout.Toggle(m_SelfCs.SplitTexFlipX, m_SelfCs.SplitTexFlipX ? "拆分图片X轴翻转" : "拆分图片X轴正常");
        m_SelfCs.SplitTexFlipY =
            GUILayout.Toggle(m_SelfCs.SplitTexFlipY, m_SelfCs.SplitTexFlipY ? "拆分图片Y轴翻转" : "拆分图片Y轴正常");
        //if (GUILayout.Button("备份当前预制体"))
        //{
        //    _selfCs.BackupCurPrefab();
        //}
        if (GUILayout.Button("开始转换"))
        {
            m_SelfCs.StartChange();
        }
    }

    #region Mesh转SpriteRenderer
    [MenuItem("Assets/场景工具/1_Mesh转SpriteRenderer")]
    private static void _MeshRenderToSpriteRender()
    {
        string[] selectPath = Selection.assetGUIDs;
        if (selectPath.Length <= 0)
            return;
        string findRootPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
        if (!Directory.Exists(findRootPath))
            return;
        string[] allAssetsGuid = _CollectAllResGuid(findRootPath, "t:prefab");
        int corrCount = allAssetsGuid.Length;
        for (int i = 0; i < corrCount; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            bool ignoreOffset = !assetPath.Contains("scenes");
            GameObject insPrefab = Instantiate(prefab);
            MeshToSpriteRenderTool changeTool = insPrefab.AddComponent<MeshToSpriteRenderTool>();
            changeTool.StartChange(ignoreOffset);
            if (changeTool != null)
                DestroyImmediate(changeTool);
            PrefabUtility.SaveAsPrefabAsset(insPrefab, assetPath);
            DestroyImmediate(insPrefab);
        }
    }
    #endregion

    #region 生成场景目录
    [MenuItem("Assets/场景工具/5_生成单一类型场景目录")]
    private static void _GenerateSingleSceneDir()
    {
        string[] selectPath = Selection.assetGUIDs;
        if (selectPath.Length <= 0)
            return;
        string findRootPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
        if (!Directory.Exists(findRootPath))
            return;
        string subType;
        if (findRootPath.Contains("Chapter/Chapter"))
            subType = "Chapter";
        else if (findRootPath.Contains("City/City"))
            subType = "City";
        else if (findRootPath.Contains("Other/Other"))
            subType = "Other";
        else
            return;
        string subPath = subType + "/" + subType;
        string sceneDirPath = findRootPath.Substring(0, findRootPath.LastIndexOf(subPath));
        string num = findRootPath.Substring(findRootPath.LastIndexOf(subType) + subType.Length);
        subPath += num.ToString();
        List<string> allChildDirPath = new List<string>(7)
        {
            sceneDirPath + subPath + "/Chapter" + num.ToString() + "_Dungeon/Scene",
            sceneDirPath + subPath + "/LogicObject",
            sceneDirPath + subPath + "/Material",
            sceneDirPath + subPath + "/Portal",
            sceneDirPath + subPath + "/Prefab",
            sceneDirPath + subPath + "/Texture/Multiple",
            sceneDirPath + subPath + "/Texture/Single",
        };
        _CreateChildDirByPath(allChildDirPath);
        EditorUtility.DisplayDialog("生成指定场景目录", "处理完毕", "完成");
        AssetDatabase.Refresh();
    }
    private static void _CreateChildDirByPath(List<string> allChildDirPath)
    {
        foreach (string dirPath in allChildDirPath)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }
    }
    #endregion

    #region 根据Sprite生成预制体
    private static string sm_NorShaderPath = "HeroGo/General/UnLit/HG_Unlit_Dye_Transparent";
    [MenuItem("Assets/场景工具/10_根据Sprite生成预制体")]
    private static void _GeneratorPrefabBySprite()
    {
        string[] selectIdS = Selection.assetGUIDs;
        if (selectIdS.Length <= 0)
            return;
        List<string> dirPaths = new List<string>();
        List<string> assetPaths = new List<string>();
        // 收集满足条件文件夹和文件
        foreach (string id in selectIdS)
        {
            string selectPath = AssetDatabase.GUIDToAssetPath(id);
            if (Directory.Exists(selectPath) && selectPath.Contains("Resources/Scene"))
            {
                dirPaths.Add(selectPath);
                continue;
            }

            if (File.Exists(selectPath) && selectPath.Contains("Resources/Scene"))
            {
                assetPaths.Add(selectPath);
            }
        }
        // 收集文件夹内所有资源
        int dirCount = dirPaths.Count;
        if (dirCount > 0)
        {
            string[] searchFolder = new string[dirCount];
            for (int i = 0; i < dirCount; i++)
                searchFolder[i] = dirPaths[i];
            string[] allAssetsGuid = AssetDatabase.FindAssets("t:Sprite", searchFolder); // 此处最好去重
            int corrCount = allAssetsGuid.Length;
            for (int i = 0; i < corrCount; i++)
                assetPaths.Add(AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]));
        }
        int assetCount = assetPaths.Count;
        if (assetCount == 0)
            return;

        Shader shader = Shader.Find(sm_NorShaderPath);
        // 处理所有资源
        for (int i = 0; i < assetCount; i++)
        {
            string sprPath = assetPaths[i];
            int slashIndex = sprPath.LastIndexOf('/');
            int pointIndex = sprPath.LastIndexOf('.');
            if (slashIndex + 1 > pointIndex)
                continue;
            // 资源目录 Resources/Scene/Chapter/Chapter[num]/Texture/Single|Multiple/*****.***
            string chapterDirPath;
            List<Sprite> allSpr = new List<Sprite>();
            if (sprPath.Contains("Texture/Single"))
            {
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(sprPath);
                if (spr == null)
                    continue;
                chapterDirPath = sprPath.Substring(0, slashIndex - 15); // 去掉/Texture/Single
                if (!allSpr.Contains(spr))
                    allSpr.Add(spr);
            }
            else if (sprPath.Contains("Texture/Multiple"))
            {
                // 判断是否为Multiple，是的话要处理子级Sprite
                chapterDirPath = sprPath.Substring(0, slashIndex - 17); // 去掉/Texture/Multiple
                Object[] allObj = AssetDatabase.LoadAllAssetsAtPath(sprPath);
                int objCount = allObj.Length;
                for (int j = 0; j < objCount; j++)
                {
                    Sprite spr = allObj[j] as Sprite;
                    if (spr != null && !allSpr.Contains(spr))
                        allSpr.Add(spr);
                }
            }
            else
                continue;

            foreach (Sprite spr in allSpr)
            {
                string sprName = spr.name;
                string subSprName = sprName.Substring(2);
                string prefabName = "P_" + subSprName;
                string matName = "M_" + subSprName;
                string prefabDirPath = chapterDirPath + "/Prefab";
                if (!Directory.Exists(prefabDirPath))
                    Directory.CreateDirectory(prefabDirPath);
                string matDirPath = chapterDirPath + "/Material";
                if (!Directory.Exists(matDirPath))
                    Directory.CreateDirectory(matDirPath);
                // 生成材质
                string matAssetPath = matDirPath + "/" + matName + ".mat";
                Material mat = new Material(shader);
                mat.mainTexture = spr.texture;
                AssetDatabase.CreateAsset(mat, matAssetPath);
                // 生成预制体
                string prefabAssetPath = prefabDirPath + "/" + prefabName + ".prefab";
                GameObject prefab = new GameObject(prefabName);
                prefab.transform.localEulerAngles = new Vector3(20, 0, 0);
                SpriteRenderer sprRender = prefab.AddComponent<SpriteRenderer>();
                sprRender.sprite = spr;
                sprRender.material = mat;
                PrefabUtility.SaveAsPrefabAsset(prefab, prefabAssetPath);
                DestroyImmediate(prefab);
                AssetDatabase.Refresh();
            }
        }
        EditorUtility.DisplayDialog("根据Sprite生成预制体及Material", "处理完毕", "完成");
    }
    #endregion

    #region 修正场景资源名称
    [MenuItem("Assets/场景工具/15_修正场景资源名称")]
    private static void _CorrectSceneResName()
    {
        string[] selectIds = Selection.assetGUIDs;
        if (selectIds.Length <= 0)
            return;
        // 查找满足条件的文件夹
        List<string> dirsPathList = new List<string>();
        List<string> filesPathList = new List<string>();
        foreach (string id in selectIds)
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            if (Directory.Exists(path) && path.Contains("Resources/Scene"))
                dirsPathList.Add(path);
            else if (File.Exists(path) && path.Contains("Resources/Scene"))
                filesPathList.Add(path);
        }
        if (dirsPathList.Count > 0)
        {
            int dirCount = dirsPathList.Count;
            string[] dirsPathAry = new string[dirCount];
            for (int i = 0; i < dirCount; i++)
            {
                string dirPath = dirsPathList[i];
                dirsPathAry[i] = dirPath.Substring(dirPath.IndexOf("Assets", StringComparison.Ordinal));
            }

            string[] allAssetsGuid = AssetDatabase.FindAssets("t:prefab t:texture2d t:material", dirsPathAry);
            int idCount = allAssetsGuid.Length;
            if (filesPathList.Capacity < filesPathList.Count + idCount)
                filesPathList.Capacity = filesPathList.Count + idCount;
            for (int i = 0; i < idCount; i++)
                filesPathList.Add(AssetDatabase.GUIDToAssetPath(allAssetsGuid[i]));
        }

        int fileCount = filesPathList.Count;
        for (int i = 0; i < fileCount; i++)
        {
            string assetPath = filesPathList[i]; // 因图片后缀名不定，此处以前缀为判断条件
            int slashIndex = assetPath.LastIndexOf('/');
            string assetName = assetPath.Substring(slashIndex + 1); // 带后缀名
            if (slashIndex > 5 && assetPath.Substring(slashIndex - 5, 5) == "Scene")
            {
                // 修正场景预制体
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                GameObject insPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (insPrefab == null)
                    continue;
                SpriteRenderer[] allRender = insPrefab.GetComponentsInChildren<SpriteRenderer>();
                bool haveChange = false;
                foreach (SpriteRenderer renderer in allRender)
                {
                    string newName = _ReplaceSceneResName(renderer.name);
                    if (newName == null)
                        continue;
                    haveChange = true;
                    renderer.name = newName;
                }

                if (haveChange)
                {
                    PrefabUtility.SaveAsPrefabAsset(insPrefab, assetPath);
                    AssetDatabase.Refresh();
                }
                DestroyImmediate(insPrefab);
            }
            else if (assetName.StartsWith("P_")) // 修正单个预制体名称
            {
                string newAssetName = _ReplaceSceneResName(assetName);
                if (newAssetName == null)
                    continue;
                AssetDatabase.RenameAsset(assetPath, newAssetName);
            }
            else if (assetName.StartsWith("M_")) // 修正材质名称
            {
                string newAssetName = _ReplaceSceneResName(assetName);
                if (newAssetName == null)
                    continue;
                AssetDatabase.RenameAsset(assetPath, newAssetName);
            }
            else if (assetName.StartsWith("T_")) // 修正图片名称
            {
                string newAssetName = _ReplaceSceneResName(assetName);
                if (newAssetName == null)
                    continue;
                AssetDatabase.RenameAsset(assetPath, newAssetName);
            }
        }
    }
    private static string _ReplaceSceneResName(string name)
    {
        foreach (KeyValuePair<string, string> keyValuePair in sm_OldNameToNewNameMap)
        {
            if (name.Contains(keyValuePair.Key))
                return name.Replace(keyValuePair.Key, keyValuePair.Value);
        }
        return null;
    }
    private static Dictionary<string, string> sm_OldNameToNewNameMap = new Dictionary<string, string> // 此处为优化效率可拆分成不同章节
    {
        { "31ymxc", "ch31fcst"},
        { "32ymsc", "ch32aysm"},
        { "36sh", "ch36zxyt"},
        { "38yszhb", "ch38xhqb"},

        { "Ch6", "ch41hcft"},
    };
    #endregion

    #region 移除无用的场景材质属性
    [MenuItem("Assets/场景工具/18_移除无用的材质属性")]
    private static void _RemoveUnuseMaterialProperty()
    {
        string[] selectPath = Selection.assetGUIDs;
        if (selectPath.Length <= 0)
            return;
        string findDirPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
        if (!Directory.Exists(findDirPath) || !findDirPath.Contains("Resources/Scene"))
        {
            EditorUtility.DisplayDialog("选择有误", "此功能仅限场景目录使用。请重试", "知道了");
            return;
        }

        string[] allMatGuid = _CollectAllResGuid(findDirPath, "t:material");
        int matCount = allMatGuid.Length;
        List<Shader> allShaderList = new List<Shader>();
        List<ShaderPropertyData> allShaderDataList = new List<ShaderPropertyData>();
        for (int i = 0; i < matCount; i++)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(allMatGuid[i]);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
                continue;
            Shader shader = mat.shader;
            int shaderIndex = allShaderList.IndexOf(shader);
            if (shaderIndex < 0)
            {
                shaderIndex = allShaderList.Count;
                allShaderList.Add(shader);
                allShaderDataList.Add(_CollectShaderNormalProperty(shader));
            }
            ShaderPropertyData data = allShaderDataList[shaderIndex];
            _RemoveExtraMatProperty(mat, data);
        }
        allShaderList.Clear();
        allShaderDataList.Clear();
        AssetDatabase.SaveAssets();
        //EditorUtility.DisplayDialog("移除无用的材质属性", "移除完毕", "完成");
    }
    private static ShaderPropertyData _CollectShaderNormalProperty(Shader shader)
    {
        Material mat = new Material(shader);
        SerializedObject so = new SerializedObject(mat);
        SerializedProperty sp = so.GetIterator();
        while (sp.Next(true))
        {
            if (sp.name == "m_SavedProperties")
                break;
        }
        SerializedProperty texEnvsProp = sp.FindPropertyRelative("m_TexEnvs");
        int texArrayCount = texEnvsProp.arraySize;
        SerializedProperty floatProp = sp.FindPropertyRelative("m_Floats");
        int floatArrayCount = floatProp.arraySize;
        SerializedProperty colorProp = sp.FindPropertyRelative("m_Colors");
        int colorArrayCount = colorProp.arraySize;
        ShaderPropertyData shaderData = new ShaderPropertyData(texArrayCount, floatArrayCount, colorArrayCount);
        for (int i = 0; i < texArrayCount; ++i)
        {
            SerializedProperty dataProperty = texEnvsProp.GetArrayElementAtIndex(i);
            SerializedProperty firstProp = dataProperty.FindPropertyRelative("first");
            shaderData.TexPropertyList.Add(firstProp.stringValue);
        }
        for (int i = 0; i < floatArrayCount; ++i)
        {
            SerializedProperty dataProperty = floatProp.GetArrayElementAtIndex(i);
            SerializedProperty firstProp = dataProperty.FindPropertyRelative("first");
            shaderData.FloatPropertyList.Add(firstProp.stringValue);
        }
        for (int i = 0; i < colorArrayCount; ++i)
        {
            SerializedProperty dataProperty = colorProp.GetArrayElementAtIndex(i);
            SerializedProperty firstProp = dataProperty.FindPropertyRelative("first");
            shaderData.ColorPropertyList.Add(firstProp.stringValue);
        }
        return shaderData;
    }
    private static void _RemoveExtraMatProperty(Material mat, ShaderPropertyData shaderData)
    {
        SerializedObject so = new SerializedObject(mat);
        SerializedProperty sp = so.GetIterator();
        while (sp.Next(true))
        {
            if (sp.name == "m_SavedProperties")
                break;
        }
        SerializedProperty texEnvsProp = sp.FindPropertyRelative("m_TexEnvs");
        int texArrayCount = texEnvsProp.arraySize;
        SerializedProperty floatProp = sp.FindPropertyRelative("m_Floats");
        int floatArrayCount = floatProp.arraySize;
        SerializedProperty colorProp = sp.FindPropertyRelative("m_Colors");
        int colorArrayCount = colorProp.arraySize;
        bool haveChanged = false;
        for (int i = 0; i < texArrayCount; ++i)
        {
            SerializedProperty dataProperty = texEnvsProp.GetArrayElementAtIndex(i);
            SerializedProperty firstProp = dataProperty.FindPropertyRelative("first");
            string texName = firstProp.stringValue;
            if (shaderData.TexPropertyList.Contains(texName))
                continue;
            haveChanged = true;
            texEnvsProp.DeleteArrayElementAtIndex(i);
            texArrayCount--;
            i--;
        }
        for (int i = 0; i < floatArrayCount; ++i)
        {
            SerializedProperty dataProperty = floatProp.GetArrayElementAtIndex(i);
            SerializedProperty firstProp = dataProperty.FindPropertyRelative("first");
            string floatName = firstProp.stringValue;
            if (shaderData.FloatPropertyList.Contains(floatName))
                continue;
            haveChanged = true;
            floatProp.DeleteArrayElementAtIndex(i);
            floatArrayCount--;
            i--;
        }
        for (int i = 0; i < colorArrayCount; ++i)
        {
            SerializedProperty dataProperty = colorProp.GetArrayElementAtIndex(i);
            SerializedProperty firstProp = dataProperty.FindPropertyRelative("first");
            string colorName = firstProp.stringValue;
            if (shaderData.ColorPropertyList.Contains(colorName))
                continue;
            haveChanged = true;
            colorProp.DeleteArrayElementAtIndex(i);
            colorArrayCount--;
            i--;
        }
        if (haveChanged)
            so.ApplyModifiedProperties();
    }
    private class ShaderPropertyData
    {
        public List<string> TexPropertyList;
        public List<string> FloatPropertyList;
        public List<string> ColorPropertyList;

        public ShaderPropertyData(int texCount, int floatCount, int colorCount)
        {
            TexPropertyList = new List<string>(texCount);
            FloatPropertyList = new List<string>(floatCount);
            ColorPropertyList = new List<string>(colorCount);
        }
    }
    #endregion

    #region 修正相同的场景材质
    private static bool sm_IsChangePrefab;
    // 修正相同材质【材质除_MainTex相同】
    [MenuItem("Assets/场景工具/20_修正相同的场景材质")]
    private static void CorrectSameMaterial()
    {
        string[] selectPath = Selection.assetGUIDs;
        if (selectPath.Length <= 0)
            return;
        string findDirPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
        if (!Directory.Exists(findDirPath) || !findDirPath.Contains("Resources/Scene"))
        {
            EditorUtility.DisplayDialog("选择有误", "此功能仅限章节级目录使用。示例路径：Assets/Resources/Scene/City/City123。请重试", "知道了");
            return;
        }
        int slashCount = 0;
        foreach (char c in findDirPath)
        {
            if (c == '/')
                slashCount++;
        }
        if (slashCount != 4)
        {  // 提示选择关卡。样式：Assets/Resources/Scene/Chapter/Chapter1。特点是Scene往下下两级的目录
            EditorUtility.DisplayDialog("选择有误", "此功能仅限章节级目录使用。示例路径：Assets/Resources/Scene/City/City123。请重试", "知道了");
            return;
        }
        _RemoveUnuseMaterialProperty();
        string[] allPrefabGuid = _CollectAllResGuid(findDirPath, "t:prefab");
        int prefabCount = allPrefabGuid.Length;
        List<Material> useMats = new List<Material>();
        for (int i = 0; i < prefabCount; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGuid[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject insPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (insPrefab == null)
                continue;
            SpriteRenderer[] allRender = insPrefab.GetComponentsInChildren<SpriteRenderer>();
            sm_IsChangePrefab = false;
            foreach (SpriteRenderer render in allRender)
            {
                Material mat = render.sharedMaterial;
                if (mat == null)
                {
                    // 警告
                    Debug.Log("**********警告*********** " + prefab.name + "->" + render.name + " 没有使用材质");
                    continue;
                }

                int useMatCount = useMats.Count;
                Material sameMat = null;
                for (int j = 0; j < useMatCount; j++)
                {
                    if (_IsSameMaterial(mat, useMats[j]))
                    {
                        sameMat = useMats[j];
                        break;
                    }
                }

                if (sameMat == null)
                    useMats.Add(mat);
                else
                {
                    render.sharedMaterial = sameMat;
                }
            }

            if (sm_IsChangePrefab)
            {
                PrefabUtility.SaveAsPrefabAsset(insPrefab, prefabPath);
                AssetDatabase.Refresh();
            }
            DestroyImmediate(insPrefab);
        }

#if DealDoor
        // 处理传送门使用材质
        string protalDataDirPath = findDirPath.Replace("Resources/Scene", "Resources/Data/SceneData");
        if (Directory.Exists(protalDataDirPath))
        {
            string[] allSceneDataGuid = _CollectAllResGuid(protalDataDirPath, "t:DSceneData");
            int dataCount = allSceneDataGuid.Length;
            for (int i = 0; i < dataCount; i++)
            {
                string sceneDataPath = AssetDatabase.GUIDToAssetPath(allSceneDataGuid[i]);
                DSceneData sceneData = AssetDatabase.LoadAssetAtPath<DSceneData>(sceneDataPath);
                if (sceneData._transportdoor == null)
                    continue;
                int doorCount = sceneData._transportdoor.Length;
                bool isChange = false;
                for (int j = 0; j < doorCount; j++)
                {
                    DTransportDoor doorData = sceneData._transportdoor[j];
                    Material doorMat = doorData.materialAsset.m_AssetObj as Material;
                    if (doorMat == null)
                        continue;
                    int useMatCount = useMats.Count;
                    Material sameMat = null;
                    for (int k = 0; k < useMatCount; k++)
                    {
                        if (_IsSameMaterial(doorMat, useMats[k]))
                        {
                            if (doorMat != useMats[k])
                            {
                                //Debug.Log(sceneData.name + "传送门材质由: + " + doorMat.name + " 修改为: " + useMats[k].name);
                                doorData.materialAsset.Set(useMats[k]);
                                isChange = true;
                            }
                            sameMat = useMats[k];
                            break;
                        }
                    }
                    if (sameMat == null)
                        useMats.Add(doorMat);
                }
                if (isChange)
                {
                    AssetDatabase.SaveAssets();
                    //EditorUtility.SetDirty(sceneData);
                }
            }
        }
#endif

        // 收集未使用的Material
        Dictionary<string, string[]>
            matDirToMatsPathDic = new Dictionary<string, string[]>(); // 此处本为了统一处理所有材质，但场景为了章节独立，允许资源重复，故而字典没啥意义
        string[] useMatsPath = new string[useMats.Count];
        for (int i = 0; i < useMats.Count; i++)
        {
            useMatsPath[i] = AssetDatabase.GetAssetPath(useMats[i]);
        }

        foreach (string matPath in useMatsPath)
        {
            string matDir = matPath.Substring(0, matPath.LastIndexOf('/'));
            string[] allMatPathAtDir;
            if (!matDirToMatsPathDic.TryGetValue(matDir, out allMatPathAtDir))
            {
                string[] allAssetGuid = _CollectAllResGuid(matDir, "t:material");
                int matCount = allAssetGuid.Length;
                allMatPathAtDir = new string[matCount];
                for (int i = 0; i < matCount; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(allAssetGuid[i]);
                    if (HaveUseThisMat(useMatsPath, path))
                        allMatPathAtDir[i] = null;
                    else
                        allMatPathAtDir[i] = path;
                }

                matDirToMatsPathDic.Add(matDir, allMatPathAtDir);
            }
        }

        // 将未使用的Material移动至备份文件夹
        foreach (KeyValuePair<string, string[]> keyValuePair in matDirToMatsPathDic)
        {
            string backupDir = keyValuePair.Key + "/MatBackup";
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
                AssetDatabase.Refresh();
            }
            int moveCount = 0;
            foreach (string matPath in keyValuePair.Value)
            {
                if (matPath == null)
                    continue;
                moveCount++;
                string matName = matPath.Substring(matPath.LastIndexOf('/')); // 名字包含/
                AssetDatabase.MoveAsset(matPath, backupDir + matName);
            }
            if (moveCount == 0)
                Directory.Delete(backupDir);
        }
        AssetDatabase.Refresh();
    }
    private static bool _IsSameMaterial(Material mat1, Material mat2)
    {
        bool isMat1Null = mat1 == null;
        bool isMat2Null = mat2 == null;
        if (isMat1Null && isMat2Null)
            return true;
        if (isMat1Null || isMat2Null)
            return false;
        if (mat1 == mat2)
            return true;
        if (mat1.renderQueue != mat2.renderQueue)
            return false;
        if (mat1.shader != mat2.shader)
            return false;
        SerializedObject so1 = new SerializedObject(mat1);
        SerializedProperty sp1 = so1.GetIterator();
        SerializedObject so2 = new SerializedObject(mat2);
        SerializedProperty sp2 = so2.GetIterator();
        while (sp1.Next(true))
        {
            if (sp1.name == "m_SavedProperties")
                break;
        }

        while (sp2.Next(true))
        {
            if (sp2.name == "m_SavedProperties")
                break;
        }

        SerializedProperty texEnvsProp1 = sp1.FindPropertyRelative("m_TexEnvs");
        SerializedProperty texEnvsProp2 = sp2.FindPropertyRelative("m_TexEnvs");
        int texArrayCount1 = texEnvsProp1.arraySize;
        int texArrayCount2 = texEnvsProp2.arraySize;
        if (texArrayCount1 != texArrayCount2)
            return false;
        for (int i = 0; i < texArrayCount1; ++i)
        {
            SerializedProperty dataProperty1 = texEnvsProp1.GetArrayElementAtIndex(i);
            SerializedProperty firstProp1 = dataProperty1.FindPropertyRelative("first");
            if (firstProp1.stringValue == "_MainTex")
                continue;
            SerializedProperty secondProp1 = dataProperty1.FindPropertyRelative("second");
            SerializedProperty texProp1 = secondProp1.FindPropertyRelative("m_Texture");
            SerializedProperty scaleProp1 = secondProp1.FindPropertyRelative("m_Scale");
            SerializedProperty offsetProp1 = secondProp1.FindPropertyRelative("m_Offset");
            SerializedProperty dataProperty2 = texEnvsProp2.GetArrayElementAtIndex(i);
            SerializedProperty secondProp2 = dataProperty2.FindPropertyRelative("second");
            SerializedProperty texProp2 = secondProp2.FindPropertyRelative("m_Texture");
            SerializedProperty scaleProp2 = secondProp2.FindPropertyRelative("m_Scale");
            SerializedProperty offsetProp2 = secondProp2.FindPropertyRelative("m_Offset");
            if (texProp1.objectReferenceValue != texProp2.objectReferenceValue ||
                scaleProp1.vector2Value != scaleProp2.vector2Value ||
                offsetProp1.vector2Value != offsetProp2.vector2Value)
                return false;
        }

        SerializedProperty floatProp1 = sp1.FindPropertyRelative("m_Floats");
        SerializedProperty floatProp2 = sp2.FindPropertyRelative("m_Floats");
        int floatArrayCount1 = floatProp1.arraySize;
        int floatArrayCount2 = floatProp2.arraySize;
        if (floatArrayCount1 != floatArrayCount2)
            return false;
        for (int i = 0; i < floatArrayCount1; ++i)
        {
            SerializedProperty dataProperty1 = floatProp1.GetArrayElementAtIndex(i);
            SerializedProperty secondProp1 = dataProperty1.FindPropertyRelative("second");
            SerializedProperty dataProperty2 = floatProp2.GetArrayElementAtIndex(i);
            SerializedProperty secondProp2 = dataProperty2.FindPropertyRelative("second");
            if (secondProp1.floatValue != secondProp2.floatValue)
                return false;
        }

        SerializedProperty colorProp1 = sp1.FindPropertyRelative("m_Colors");
        SerializedProperty colorProp2 = sp2.FindPropertyRelative("m_Colors");
        int colorArrayCount1 = colorProp1.arraySize;
        int colorArrayCount2 = colorProp2.arraySize;
        if (colorArrayCount1 != colorArrayCount2)
            return false;
        for (int i = 0; i < colorArrayCount1; ++i)
        {
            SerializedProperty dataProperty1 = colorProp1.GetArrayElementAtIndex(i);
            SerializedProperty secondProp1 = dataProperty1.FindPropertyRelative("second");
            SerializedProperty dataProperty2 = colorProp2.GetArrayElementAtIndex(i);
            SerializedProperty secondProp2 = dataProperty2.FindPropertyRelative("second");
            if (secondProp1.colorValue != secondProp2.colorValue)
                return false;
        }
        sm_IsChangePrefab = true;
        return true;
    }

    private static bool HaveUseThisMat(string[] useMatsPath, string matPath)
    {
        foreach (string path in useMatsPath)
        {
            if (path == matPath)
                return true;
        }
        return false;
    }

    #endregion

    #region 清除未使用的场景资源
    [MenuItem("Assets/场景工具/25_删除未使用场景资源")]
    private static void ClearUnusedSceneRes()
    {
        string[] selectPath = Selection.assetGUIDs;
        if (selectPath.Length <= 0)
            return;
        string findDirPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
        if (!Directory.Exists(findDirPath) || !findDirPath.Contains("Resources/Scene"))
            return;
        int slashCount = 0;
        foreach (char c in findDirPath)
        {
            if (c == '/')
                slashCount++;
        }
        if (slashCount != 4)
        {  // 提示选择关卡。样式：Assets/Resources/Scene/Chapter/Chapter1。特点是Scene往下下两级的目录
            EditorUtility.DisplayDialog("路径有误", "此功能仅限章节级目录使用。示例路径：Assets/Resources/Scene/City/City123。请重试", "知道了");
            return;
        }

        #region 处理场景和传送门预制体
        string[] allPrefabGuid = _CollectAllResGuid(findDirPath, "t:prefab");
        int corrCount = allPrefabGuid.Length;
        string[] allChildPrefabName = null;
        List<string> usedPrefabName = null;
        List<string> usedSprName = new List<string>();
        List<string> usedMatName = new List<string>();
        string childPrefabDirPath = findDirPath + "/Prefab";
        for (int i = 0; i < corrCount; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGuid[i]);
            int slashIndex = prefabPath.LastIndexOf('/');
            string subDir5Name = prefabPath.Substring(slashIndex - 5, 5);
            string subDir6Name = prefabPath.Substring(slashIndex - 6, 6);
            if (subDir5Name != "Scene" && subDir6Name != "Portal")  // 目前处理场景和传送门预制体
                continue;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (allChildPrefabName == null)
            {  // 初次收集所有子级prefab
                allChildPrefabName = _CollectAllResName(childPrefabDirPath, "t:prefab");
                usedPrefabName = new List<string>(allChildPrefabName.Length);
            }
            // 遍历子节点，找到被使用的Sprite、Material、Prefab
            SpriteRenderer[] allRender = prefab.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer render in allRender)
            {
                int spaceIndex = render.name.IndexOf(' ');
                string renderName;
                if (spaceIndex >= 0)
                    renderName = render.name.Substring(0, spaceIndex);
                else
                    renderName = render.name;
                if (!usedPrefabName.Contains(renderName))
                    usedPrefabName.Add(renderName);

                Sprite spr = render.sprite;
                if (spr == null)  // 理论而言不该存在
                    Debug.LogError("场景预制体错误: 预制体:" + prefab.name + "->" + renderName + " Sprite为空", prefab);
                else if (!usedSprName.Contains(spr.name))
                    usedSprName.Add(spr.name);

                Material mat = render.sharedMaterial;
                if (mat == null)  // 理论而言不该存在
                    Debug.LogError("场景预制体错误: 预制体:" + prefab.name + "->" + renderName + " 材质为空", prefab);
                else if (!usedMatName.Contains(mat.name))
                    usedMatName.Add(mat.name);
            }
        }
        if (allChildPrefabName == null)
        {
            Debug.LogError("当前关卡不存在场景预制体，请确认");
            return;
        }
#if DealDoor
        // 处理传送门使用材质
        string protalDataDirPath = findDirPath.Replace("Resources/Scene", "Resources/Data/SceneData");
        if (Directory.Exists(protalDataDirPath))
        {
            string[] allSceneDataGuid = _CollectAllResGuid(protalDataDirPath, "t:DSceneData");
            int dataCount = allSceneDataGuid.Length;
            for (int i = 0; i < dataCount; i++)
            {
                string sceneDataPath = AssetDatabase.GUIDToAssetPath(allSceneDataGuid[i]);
                DSceneData sceneData = AssetDatabase.LoadAssetAtPath<DSceneData>(sceneDataPath);
                if (sceneData._transportdoor == null)
                    continue;
                int doorCount = sceneData._transportdoor.Length;
                for (int j = 0; j < doorCount; j++)
                {
                    DTransportDoor doorData = sceneData._transportdoor[j];
                    Material doorMat = doorData.materialAsset.m_AssetObj as Material;
                    if (doorMat == null)
                        continue;
                    if (!usedMatName.Contains(doorMat.name))
                        usedMatName.Add(doorMat.name);
                }
            }
        }
#endif

        // 确认单一预制体是否被使用
        string backupChildPrefabDirPath = findDirPath + "/Prefab/Backup";
        foreach (string childPrefabName in allChildPrefabName)
        {
            if (!usedPrefabName.Contains(childPrefabName))  // 移动到备份文件夹
            {
                if (!Directory.Exists(backupChildPrefabDirPath))
                {
                    Directory.CreateDirectory(backupChildPrefabDirPath);
                    AssetDatabase.Refresh();
                }
                AssetDatabase.MoveAsset(childPrefabDirPath + "/" + childPrefabName + ".prefab",
                    backupChildPrefabDirPath + "/" + childPrefabName + ".prefab");
                AssetDatabase.Refresh();
            }
        }
        #endregion

        #region 收集所有Sprite
        string singleSprDirPath = findDirPath + "/Texture/Single";
        string multipleSprDirPath = findDirPath + "/Texture/Multiple"; // Multiple类型存在1对多
        string[] allSingleSprGuid = _CollectAllResGuid(singleSprDirPath, "t:Sprite");
        int singleSprCount = allSingleSprGuid.Length;
        string[] allSingleSprName = new string[singleSprCount];
        for (int i = 0; i < singleSprCount; i++)
        {
            string sprPath = AssetDatabase.GUIDToAssetPath(allSingleSprGuid[i]);
            int sprPointIndex = sprPath.LastIndexOf('.');
            int sprSlashIndex = sprPath.LastIndexOf('/');
            allSingleSprName[i] = sprPath.Substring(sprSlashIndex + 1, sprPointIndex - sprSlashIndex - 1);
        }

        string[] allMultipleGuid = _CollectAllResGuid(multipleSprDirPath, "t:Sprite");
        Dictionary<string, List<string>> mulToChildSprDic = new Dictionary<string, List<string>>();
        int multipleSprCount = allMultipleGuid.Length;
        for (int i = 0; i < multipleSprCount; i++)
        {
            string sprPath = AssetDatabase.GUIDToAssetPath(allMultipleGuid[i]);
            int sprPointIndex = sprPath.LastIndexOf('.');
            int sprSlashIndex = sprPath.LastIndexOf('/');
            string mulSprName = sprPath.Substring(sprSlashIndex + 1, sprPointIndex - sprSlashIndex - 1);
            if (mulToChildSprDic.ContainsKey(mulSprName))
                continue;
            List<string> childSprName = new List<string>();
            Object[] allObj = AssetDatabase.LoadAllAssetsAtPath(sprPath);
            foreach (Object obj in allObj)
            {
                Sprite spr = obj as Sprite;
                if (spr != null)
                    childSprName.Add(spr.name);
            }
            if (childSprName.Count > 0)
                mulToChildSprDic.Add(mulSprName, childSprName);
        }
        #endregion

        #region 收集所有Material
        string matDirPath = findDirPath + "/Material";
        string[] allMatName = _CollectAllResName(matDirPath, "t:material");
        #endregion

        #region 将未使用的精灵和材质放到备份文件夹
        string backupMatDirPath = findDirPath + "/Material/Backup";
        foreach (string matName in allMatName)
        {
            if (!usedMatName.Contains(matName))
            {  // 移动到备份文件夹
                if (!Directory.Exists(backupMatDirPath))
                {
                    Directory.CreateDirectory(backupMatDirPath);
                    AssetDatabase.Refresh();
                }

                AssetDatabase.MoveAsset(matDirPath + "/" + matName + ".mat", backupMatDirPath + "/" + matName + ".mat");
                AssetDatabase.Refresh();
            }
        }

        string backupSingleSprDirPath = findDirPath + "/Texture/Single/Backup";
        for (int i = 0; i < singleSprCount; i++)
        {
            string sprName = allSingleSprName[i];
            if (!usedSprName.Contains(sprName))
            {  // 移动至备份文件夹
                if (!Directory.Exists(backupSingleSprDirPath))
                {
                    Directory.CreateDirectory(backupSingleSprDirPath);
                    AssetDatabase.Refresh();
                }

                string sprPath = AssetDatabase.GUIDToAssetPath(allSingleSprGuid[i]);
                string sprBackupPath = sprPath.Replace("Texture/Single", "Texture/Single/Backup");
                AssetDatabase.MoveAsset(sprPath, sprBackupPath);
                AssetDatabase.Refresh();
            }
        }

        string backupMultipleSprDirPath = findDirPath + "/Texture/Multiple/Backup";
        int index = -1;
        foreach (KeyValuePair<string, List<string>> keyValuePair in mulToChildSprDic)
        {
            index++;
            bool isUsed = false;
            foreach (string childSprName in keyValuePair.Value)
            {
                if (usedSprName.Contains(childSprName))
                {
                    isUsed = true;
                    break;
                }
            }

            if (isUsed == false)
            {
                if (!Directory.Exists(backupMultipleSprDirPath))
                {
                    Directory.CreateDirectory(backupMultipleSprDirPath);
                    AssetDatabase.Refresh();
                }

                string sprPath = AssetDatabase.GUIDToAssetPath(allMultipleGuid[index]);
                string sprBackupPath = sprPath.Replace("Texture/Multiple", "Texture/Multiple/Backup");
                AssetDatabase.MoveAsset(sprPath, sprBackupPath);
                AssetDatabase.Refresh();
            }
        }
        #endregion
    }
    private static string[] _CollectAllResName(string findDirPath, string resFindFilter)
    {
        string[] searchFolder = new string[1];
        searchFolder[0] = findDirPath;
        string[] allChildPrefabGuid = AssetDatabase.FindAssets(resFindFilter, searchFolder);
        int childGuidCount = allChildPrefabGuid.Length;
        string[] childPrefabName = new string[childGuidCount];
        for (int j = 0; j < childGuidCount; j++)
        {
            string childPrefabPath = AssetDatabase.GUIDToAssetPath(allChildPrefabGuid[j]);
            int childPointIndex = childPrefabPath.LastIndexOf('.');
            int childSlashIndex = childPrefabPath.LastIndexOf('/');
            childPrefabName[j] = childPrefabPath.Substring(childSlashIndex + 1, childPointIndex - childSlashIndex - 1);
        }
        return childPrefabName;
    }
    private static string[] _CollectAllResGuid(string findDirPath, string resFindFilter)
    {
        string[] searchFolder = new string[1];
        searchFolder[0] = findDirPath;
        return AssetDatabase.FindAssets(resFindFilter, searchFolder);
    }
    #endregion

    //#region 优化场景图集
    //[MenuItem("Assets/场景工具/30_优化场景图集")]
    //private static void PackSprAtlasBySprBreak()
    //{
    //    string[] selectPath = Selection.assetGUIDs;
    //    if (selectPath.Length <= 0)
    //        return;
    //    string findDirPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
    //    if (!Directory.Exists(findDirPath) || !findDirPath.Contains("Resources/Scene"))
    //    {
    //        EditorUtility.DisplayDialog("选择有误", "此功能仅限章节级目录使用。示例路径：Assets/Resources/Scene/City/City123。请重试", "知道了");
    //        return;
    //    }
    //    int slashCount = 0;
    //    foreach (char c in findDirPath)
    //    {
    //        if (c == '/')
    //            slashCount++;
    //    }
    //    if (slashCount != 4)
    //    {
    //        // 提示选择关卡。样式：Assets/Resources/Scene/Chapter/Chapter1。特点是Scene往下下两级的目录
    //        EditorUtility.DisplayDialog("选择有误", "此功能仅限章节级目录使用。示例路径：Assets/Resources/Scene/City/City123。请重试", "知道了");
    //        return;
    //    }

    //    string[] allPrefabGuid = _CollectAllResGuid(findDirPath, "t:prefab");
    //    int prefabCount = allPrefabGuid.Length;
    //    List<List<SprBreakOtherSprData>> allBreakInfo = new List<List<SprBreakOtherSprData>>();
    //    List<Dictionary<Sprite, int>> allSprFrequencyInfo = new List<Dictionary<Sprite, int>>();
    //    for (int i = 0; i < prefabCount; i++)
    //    {
    //        string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGuid[i]);
    //        int lastSlashIndex = prefabPath.LastIndexOf('/');
    //        if (prefabPath.Substring(lastSlashIndex - 5, 5) != "Scene" && prefabPath.Substring(lastSlashIndex - 6, 6) != "Portal")
    //            continue;
    //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    //        GameObject insPrefab = Instantiate(prefab);
    //        AnalysisRenderOrder renderAnalysis = insPrefab.AddComponent<AnalysisRenderOrder>();
    //        renderAnalysis.CalculateRenderOrder();
    //        renderAnalysis.AnalysisSprBreak();
    //        allBreakInfo.Add(renderAnalysis.GetBreakOtherData());
    //        allSprFrequencyInfo.Add(renderAnalysis.GetSprFrequencyDic());
    //        DestroyImmediate(insPrefab);
    //    }

    //    List<Sprite> allBreakSpr = AnalysisRenderOrder.CalculateBreakSprAtlasInfo(allBreakInfo, allSprFrequencyInfo);

    //    //List<Sprite> allNoBreakSpr = new List<Sprite>();
    //    //string[] allSprGuids = _CollectAllResGuid(findDirPath, "t:sprite");
    //    //int sprCount = allSprGuids.Length;
    //    //for (int i = 0; i < sprCount; i++)
    //    //{
    //    //    string sprPath = AssetDatabase.GUIDToAssetPath(allSprGuids[i]);
    //    //    Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(sprPath);
    //    //    if (spr == null)
    //    //        continue;
    //    //    if (!allBreakSpr.Contains(spr))
    //    //        allNoBreakSpr.Add(spr);
    //    //}
    //    //string chapterName = findDirPath.Substring(findDirPath.LastIndexOf('/') + 1);
    //    //UiTools.GeneratorSpriteAtlas(allBreakSpr, findDirPath + "/" + chapterName + "_Break.spriteatlas");
    //    //UiTools.GeneratorSpriteAtlas(allNoBreakSpr, findDirPath + "/" + chapterName + "_NoBreak.spriteatlas");
    //}
    //#endregion

    #region 查找跨章节使用资源
    [MenuItem("Assets/场景工具/35_查找跨章节使用资源")]
    private static void SearchPrefabUseOtherChapterRes()
    {
        string[] selectPath = Selection.assetGUIDs;
        if (selectPath.Length <= 0)
            return;
        string findDirPath = AssetDatabase.GUIDToAssetPath(selectPath[0]);
        if (!findDirPath.Contains("Resources/Scene"))
            return;
        string[] allPrefabGuid = _CollectAllResGuid(findDirPath, "t:prefab");
        int prefabCount = allPrefabGuid.Length;
        string lashChapterName = "";
        StringBuilder sbSpr = new StringBuilder();
        StringBuilder sbMat = new StringBuilder();
        List<string> allChapterPath = new List<string>();
        for (int i = 0; i < prefabCount; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGuid[i]);
            // 示例Assets/Resources/Scene/Chapter/Chapter1/……
            int prefabSlashCount = 0;
            int prefabSlash4Index = 0;
            int prefabSlash5Index = 0;
            int prefabPathLen = prefabPath.Length;
            for (int j = 0; j < prefabPathLen; j++)
            {
                char c = prefabPath[j];
                if (c == '/')
                    prefabSlashCount++;
                if (prefabSlashCount == 4 && prefabSlash4Index == 0)
                    prefabSlash4Index = j;
                if (prefabSlashCount == 5)
                {
                    prefabSlash5Index = j;
                    break;
                }
            }
            string chapterName = prefabPath.Substring(prefabSlash4Index + 1, prefabSlash5Index - prefabSlash4Index - 1);
            if (lashChapterName != chapterName)
            {
                sbSpr.AppendLine(chapterName);
                sbMat.AppendLine(chapterName);
                lashChapterName = chapterName;
                allChapterPath.Add(prefabPath.Substring(0, prefabSlash5Index));
            }

            string chapterDirPath = prefabPath.Substring(0, prefabSlash5Index);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            SpriteRenderer[] allRender = prefab.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sprRender in allRender)
            {
                // 检测所有的Sprite
                if (sprRender.sprite == null)
                    continue;
                string sprPath = AssetDatabase.GetAssetPath(sprRender.sprite);
                if (sprPath.Substring(0, prefabSlash5Index) != chapterDirPath)
                {
                    int sprSlashCount = 0;
                    int sprSlash4Index = 0;
                    int sprSlash5Index = 0;
                    int sprPathLen = sprPath.Length;
                    for (int j = 0; j < sprPathLen; j++)
                    {
                        char c = sprPath[j];
                        if (c == '/')
                            sprSlashCount++;
                        if (sprSlashCount == 4 && sprSlash4Index == 0)
                            sprSlash4Index = j;
                        if (sprSlashCount == 5)
                        {
                            sprSlash5Index = j;
                            break;
                        }
                    }
                    string sprDirName = sprPath.Substring(sprSlash4Index + 1, sprSlash5Index - sprSlash4Index - 1);
                    //Debug.Log(prefab.name + " -> " + sprRender.name, prefab);
                    sbSpr.AppendLine(chapterName + " 下预制体 " + prefab.name + " 中SpriteRenderer-> " + sprRender.name +
                                     " 中Sprite使用了 " + sprDirName + "下资源");
                }
                // 检测所有的Material
                if (sprRender.sharedMaterial == null)
                    continue;
                string matPath = AssetDatabase.GetAssetPath(sprRender.sharedMaterial);
                if (matPath.Length < prefabSlash5Index)
                {
                    sbMat.AppendLine(chapterName + " 下预制体 " + prefab.name + " 中SpriteRenderer-> " + sprRender.name +
                                     " 中material使用了Unity内置材质" + sprRender.sharedMaterial.name);
                    continue;
                }

                if (matPath.Substring(0, prefabSlash5Index) != chapterDirPath)
                {
                    int matSlashCount = 0;
                    int matSlash4Index = 0;
                    int matSlash5Index = 0;
                    int matPathLen = matPath.Length;
                    for (int j = 0; j < matPathLen; j++)
                    {
                        char c = matPath[j];
                        if (c == '/')
                            matSlashCount++;
                        if (matSlashCount == 4 && matSlash4Index == 0)
                            matSlash4Index = j;
                        if (matSlashCount == 5)
                        {
                            matSlash5Index = j;
                            break;
                        }
                    }

                    if (matSlash4Index == 0 || matSlash5Index == 0)
                    {
                        sbMat.AppendLine(prefab.name + " 材质-> " + sprRender.name);
                    }
                    else
                    {
                        string matDirName = matPath.Substring(matSlash4Index + 1, matSlash5Index - matSlash4Index - 1);
                        //Debug.Log(prefab.name + " -> " + matRender.name, prefab);
                        sbMat.AppendLine(chapterName + " 下预制体 " + prefab.name + " 中SpriteRenderer-> " + sprRender.name +
                                         " 中material使用了 " + matDirName + "下资源");
                    }
                }
            }
        }
#if DealDoor
        // 新加入对DSceneData中传送门材质信息处理
        foreach (string chapterPath in allChapterPath)
        {
            string protalDataDirPath = chapterPath.Replace("Resources/Scene", "Resources/Data/SceneData");
            string chapterName = chapterPath.Substring(chapterPath.LastIndexOf('/') + 1);
            int prefabSlash5Index = chapterPath.Length;
            if (Directory.Exists(protalDataDirPath))
            {
                string[] allSceneDataGuid = _CollectAllResGuid(protalDataDirPath, "t:DSceneData");
                int dataCount = allSceneDataGuid.Length;
                for (int i = 0; i < dataCount; i++)
                {
                    string sceneDataPath = AssetDatabase.GUIDToAssetPath(allSceneDataGuid[i]);
                    DSceneData sceneData = AssetDatabase.LoadAssetAtPath<DSceneData>(sceneDataPath);
                    if (sceneData._transportdoor == null)
                        continue;
                    int doorCount = sceneData._transportdoor.Length;
                    for (int j = 0; j < doorCount; j++)
                    {
                        DTransportDoor doorData = sceneData._transportdoor[j];
                        Material doorMat = doorData.materialAsset.m_AssetObj as Material;
                        if (doorMat == null)
                            continue;
                        string matPath = AssetDatabase.GetAssetPath(doorMat);
                        if (matPath.Length < prefabSlash5Index)
                        {
                            sbMat.AppendLine(chapterName + " 下场景数据 " + Path.GetFileName(sceneDataPath) + " 中传送门使用了Unity内置材质" + doorMat.name);
                            continue;
                        }
                        if (matPath.Substring(0, prefabSlash5Index) != chapterPath)
                        {
                            int matSlashCount = 0;
                            int matSlash4Index = 0;
                            int matSlash5Index = 0;
                            int matPathLen = matPath.Length;
                            for (int k = 0; k < matPathLen; k++)
                            {
                                char c = matPath[k];
                                if (c == '/')
                                    matSlashCount++;
                                if (matSlashCount == 4 && matSlash4Index == 0)
                                    matSlash4Index = k;
                                if (matSlashCount == 5)
                                {
                                    matSlash5Index = k;
                                    break;
                                }
                            }
                            if (matSlash4Index == 0 || matSlash5Index == 0)
                            {
                                sbMat.AppendLine(Path.GetFileName(sceneDataPath) + " 传送门材质有问题");
                            }
                            else
                            {
                                string matDirName = matPath.Substring(matSlash4Index + 1, matSlash5Index - matSlash4Index - 1);
                                sbMat.AppendLine(chapterName + " 下场景数据 " + Path.GetFileName(sceneDataPath) + " 中传送门材质 " + doorMat.name + " 使用了 " + matDirName + "下资源");
                            }
                        }
                    }
                }
            }
        }
#endif
        Debug.Log(sbSpr.ToString());
        Debug.Log(sbMat.ToString());
    }
    #endregion
}

public class SceneSprAtlasPacker
{
    private struct atlasStructForScene
    {
        public string AtlasName;
        public bool PackToAtlas;
        public bool AtlasCompress;

        public atlasStructForScene(string atlasName, bool packToAtlas = true, bool atlasCompress = false)
        {
            AtlasName = atlasName;
            PackToAtlas = packToAtlas;
            AtlasCompress = atlasCompress;
        }
    }

    #region 此为针对场景Sprite的处理
    private static string[] sceneSprDirFilter = new string[1] { "Texture/Single" };
    [MenuItem("Assets/场景工具/40_更新选中Sprite所在目录合图")]
    private static void UpdateSelectSpriteAtlasForScene()
    {
        Object[] selection = Selection.GetFiltered(typeof(Texture), SelectionMode.Assets);
        if (selection == null || selection.Length <= 0)
        {
            Debug.LogError("请选择一个纹理对象");
            return;
        }
        foreach (var atlas in selection)
        {
            string assetPath = AssetDatabase.GetAssetPath(atlas);
            FilterSpriteAtlas(Path.GetDirectoryName(assetPath), false);
        }
        AssetDatabase.SaveAssets();
    }
    [MenuItem("Assets/场景工具/45_更新选中目录下所有场景合图")]
    private static void UpdateSelectDirSpriteAtlasForScene()
    {
        try
        {
            string[] allSelectIds = Selection.assetGUIDs;
            int idCount = allSelectIds.Length;
            for (int i = 0; i < idCount; i++)
            {
                string selectDirPath = AssetDatabase.GUIDToAssetPath(allSelectIds[i]);
                if (!Directory.Exists(selectDirPath))
                    continue;
                FilterSpriteAtlas(selectDirPath, true, sceneSprDirFilter);
            }
            AssetDatabase.SaveAssets();
        }
        finally
        {
            EditorUtility.DisplayDialog("更新选中目录下所有场景合图", "更新完毕", "完成");
        }
    }
    [MenuItem("Assets/场景工具/50_更新所有场景合图")]
    public static void UpdateAllSpriteAtlasForSceneGUI()
    {
        try
        {
            string selectDirPath = "Assets/Resources/Scene";
            if (!Directory.Exists(selectDirPath))
                return;
            FilterSpriteAtlas(selectDirPath, true, sceneSprDirFilter);
            AssetDatabase.SaveAssets();
        }
        finally
        {
            EditorUtility.DisplayDialog("更新所有场景合图", "更新完毕", "完成");
        }
    }
    public static void UpdateAllSpriteAtlasForScene()
    {
        try
        {
            string selectDirPath = "Assets/Resources/Scene";
            if (!Directory.Exists(selectDirPath))
                return;
            FilterSpriteAtlas(selectDirPath, true, sceneSprDirFilter);
            AssetDatabase.SaveAssets();
        }
        finally
        {
            Debug.Log("更新所有场景合图完毕");
        }
    }
    #endregion

    private static void FilterSpriteAtlas(string dirPath, bool bRecursive = true, string[] dirPathFilter = null)
    {
        string atlasName = dirPath + "/" + Path.GetFileNameWithoutExtension(dirPath) + ".spriteatlas";
        atlasName = atlasName.Replace("\\", "/");
        if (dirPathFilter != null)
        {
            int i;
            for (i = 0; i < dirPathFilter.Length; i++)
            {
                if (atlasName.Contains(dirPathFilter[i]))
                    break;
            }
            if (i == dirPathFilter.Length)
                return;
        }
        Dictionary<atlasStructForScene, List<Sprite>> altasStruct = new Dictionary<atlasStructForScene, List<Sprite>>();
        atlasName = atlasName.Substring(atlasName.IndexOf("Assets/"));
        string assetRelativePath = Path.GetDirectoryName(atlasName);
        // 获取同目录下所有Sprites
        string[] spriteGUIDNames = AssetDatabase.FindAssets("t:sprite", new string[] { assetRelativePath });

        for (int j = 0; j < spriteGUIDNames.Length; ++j)
        {
            string spriteName = AssetDatabase.GUIDToAssetPath(spriteGUIDNames[j]);

            // 子目录的Sprite，不添加
            if (Path.GetDirectoryName(spriteName) != assetRelativePath)
                continue;

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spriteName);
            string[] assetLabels = AssetDatabase.GetLabels(sprite);
            bool bPack = true;
            bool bCompress = false;
            string PackName = "";
            if (assetLabels.Length != 0)
            {

                foreach (var label in assetLabels)
                {
                    if (bPack)
                        bPack = !label.Contains("Pack:no");
                    if (!bCompress)
                        bCompress = label.Contains("Compress:yes");
                    if (label.Contains("Name:"))
                        PackName = label.Substring((label.IndexOf(":") + 1));
                }
            }
            atlasStructForScene atlasSet = new atlasStructForScene(PackName, bPack, bCompress);


            if (altasStruct.ContainsKey(atlasSet))
            {
                altasStruct[atlasSet].Add(sprite);
            }
            else
            {
                List<Sprite> newSpriteSets = new List<Sprite>();
                newSpriteSets.Add(sprite);
                altasStruct.Add(atlasSet, newSpriteSets);
            }
        }

        foreach (var singleAltas in altasStruct)
        {
            if (singleAltas.Key.PackToAtlas)
                newOrUpdateAltas(dirPath, singleAltas.Key, singleAltas.Value);
        }
        if (bRecursive)
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < subDirs.Length; ++i)
            {
                string dirName = subDirs[i];
                FilterSpriteAtlas(dirName, bRecursive);
            }
        }
    }

    private static void newOrUpdateAltas(string dirPath, atlasStructForScene atlasSet, List<Sprite> sprites)
    {
        string mark = atlasSet.AtlasCompress ? "_compressed" : "";
        string pre = atlasSet.AtlasName == "" ? "" : "_";
        mark = pre + atlasSet.AtlasName + mark;
        string atlasName = dirPath + "/" + Path.GetFileNameWithoutExtension(dirPath) + mark + ".spriteatlas";
        atlasName = atlasName.Replace("\\", "/");
        atlasName = atlasName.Substring(atlasName.IndexOf("Assets/"));

        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasName);
        if (atlas == null)
        {
            var instance = new SpriteAtlas();
            AssetDatabase.CreateAsset(instance, atlasName);

            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasName);
        }

        NormalizeSpriteAtlas(atlas, !atlasSet.AtlasCompress);

        SerializedObject alsObject = new SerializedObject(atlas);
        SerializedProperty packables = alsObject.FindProperty("m_EditorData.packables");
        packables.ClearArray();

        foreach (var spr in sprites)
        {
            packables.InsertArrayElementAtIndex(0);
            SerializedProperty elementProperty = packables.GetArrayElementAtIndex(0);
            elementProperty.objectReferenceValue = spr;
        }

        alsObject.ApplyModifiedProperties();
        SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
    }

    private static void NormalizeSpriteAtlas(SpriteAtlas als, bool bNormalize = true)
    {
        als.SetIncludeInBuild(true);

        SpriteAtlasTextureSettings textureSetting = als.GetTextureSettings();
        textureSetting.filterMode = FilterMode.Bilinear;
        textureSetting.readable = false;
        textureSetting.generateMipMaps = false;
        textureSetting.sRGB = true;
        als.SetTextureSettings(textureSetting);

        SpriteAtlasPackingSettings packingSetting = als.GetPackingSettings();
        packingSetting.enableRotation = true;
        packingSetting.enableTightPacking = true;
        packingSetting.padding = 2;
        als.SetPackingSettings(packingSetting);

        TextureImporterPlatformSettings importSetting = als.GetPlatformSettings("DefaultTexturePlatform");
        if (!bNormalize)
        {
            importSetting.textureCompression = TextureImporterCompression.Compressed;
            importSetting.format = TextureImporterFormat.Automatic;
            importSetting.crunchedCompression = true;
        }
        else
        {
            importSetting.textureCompression = TextureImporterCompression.Uncompressed;
            //importSetting.format = TextureImporterFormat.RGBA32;
            importSetting.format = TextureImporterFormat.ASTC_RGBA_6x6;
            importSetting.overridden = true;
        }
        importSetting.maxTextureSize = 2048;
        als.SetPlatformSettings(importSetting);
    }
}
#endif