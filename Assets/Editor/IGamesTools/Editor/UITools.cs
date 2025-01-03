using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 所有UI相关工具集合
/// </summary>
public static class UITools
{
    class CreateSpriteAtlasAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var instance = new SpriteAtlas();
            NormalizeSpriteAtlas(instance);
            AssetDatabase.CreateAsset(instance, pathName);
        }
    }

    //[MenuItem("Assets/UI/创建SpriteAtlas")]
    public static void CreateSpriteAtlas()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateSpriteAtlasAsset>(),
                   "Atlas.spriteatlas", null, null);
    }




    //[MenuItem("[TM工具集]/UI相关/规范化所有SpriteAtlas")]
    public static void NormalizeAllSpriteAtlas()
    {
        string[] atlasGUIDNames = AssetDatabase.FindAssets("t:spriteatlas", new string[] { "Assets/Resources/Demo/UI" });
        if (atlasGUIDNames.Length == 0)
            return;

        try
        {
            float fProgress = 0;
            for (int i = 0; i < atlasGUIDNames.Length; ++i)
            {
                string fileName = AssetDatabase.GUIDToAssetPath(atlasGUIDNames[i]);
                EditorUtility.DisplayProgressBar("", "处理" + Path.GetFileName(fileName), fProgress++ / atlasGUIDNames.Length);

                SpriteAtlas als = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(fileName);

                //Debug.LogErrorFormat("{0}: packableCount: {1}, spriteCount: {2}", fileName, als.GetPackables().Length, als.spriteCount);

                NormalizeSpriteAtlas(als);
            }

            AssetDatabase.SaveAssets();
        }
        catch
        {
        }

        EditorUtility.ClearProgressBar();
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
        packingSetting.enableRotation = false;
        packingSetting.enableTightPacking = false;
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
            importSetting.format = TextureImporterFormat.RGBA32;
        }
        importSetting.maxTextureSize = 1024;
        als.SetPlatformSettings(importSetting);


    }

    // 替换所有Image为ImageEx
    // [MenuItem("GameObject/将Image替换为ImageEx", false, 41)]
    public static void ReplaceImage2ImageEx()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.ExcludePrefab);

        if (selection.Length > 0)
        {
            if (selection[0] is GameObject)
            {
                var go = (GameObject)selection[0];

                Image[] images = go.GetComponentsInChildren<Image>();
                if (images != null && images.Length > 0)
                {
                    foreach (Image image in images)
                    {
                        GameObject owner = image.gameObject;

                        Material mat = image.material;
                        Sprite sprite = image.sprite;

                        ImageEx imageEx = new ImageEx();

                        CopyImageProperty(imageEx, image);

                        GameObject.DestroyImmediate(image);

                        ImageEx imageExComponent = owner.AddComponent<ImageEx>();

                        CopyImageProperty(imageExComponent, imageEx);
                        imageExComponent.material = mat;
                        imageExComponent.sprite = sprite;

                        GameObject.DestroyImmediate(imageEx);
                    }
                }
            }
        }
    }

    private static void CopyImageProperty(Image dest, Image src)
    {
        dest.alphaHitTestMinimumThreshold = src.alphaHitTestMinimumThreshold;
        //dest.material = src.material;
        dest.fillClockwise = src.fillClockwise;
        dest.fillAmount = src.fillAmount;
        dest.fillOrigin = src.fillOrigin;
        dest.fillCenter = src.fillCenter;
        dest.preserveAspect = src.preserveAspect;

        dest.type = src.type;
        //    dest.overrideSprite = src.overrideSprite;
        //    dest.sprite = src.sprite;
        dest.fillMethod = src.fillMethod;
    }


    private  struct atlasStruct{
        public string atlasName;
        public bool bPackToAtlas;
        public bool bAtlasCompress;

        public atlasStruct(string atlasName, bool bPackToAtlas=true, bool bAtlasCompress=false)
        {
            this.atlasName = atlasName;
            this.bPackToAtlas = bPackToAtlas;
            this.bAtlasCompress = bAtlasCompress;
        }
    }
    private static void FilterSpriteAtlas(string dirPath, bool bRecursive = true) {
        Dictionary<atlasStruct, List<Sprite>> altasStruct = new Dictionary<atlasStruct, List<Sprite>>();
        string atlasName = dirPath + "/" + Path.GetFileNameWithoutExtension(dirPath) + ".spriteatlas";
        atlasName = atlasName.Replace("\\", "/");
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
            if (assetLabels.Length !=0)
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
            atlasStruct atlasSet = new atlasStruct(PackName, bPack, bCompress);


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

        foreach (var singleAltas in altasStruct) {
            if (singleAltas.Key.bPackToAtlas)
                newOrUpdateAltas(dirPath, singleAltas.Key, singleAltas.Value);
        }
        if (bRecursive)
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < subDirs.Length; ++i)
            {
                string dirName = subDirs[i];
                dirName.Replace("\\", "/");

                FilterSpriteAtlas(dirName, bRecursive);
            }
        }
    }

    private static void newOrUpdateAltas(string dirPath,atlasStruct atlasSet, List<Sprite> sprites)
    {
        string mark = atlasSet.bAtlasCompress?"_compressed":"";
        string pre = atlasSet.atlasName == "" ? "" : "_";
        mark = pre + atlasSet.atlasName + mark;
        string atlasName = dirPath + "/" + Path.GetFileNameWithoutExtension(dirPath)+mark + ".spriteatlas";
        atlasName = atlasName.Replace("\\", "/");
        atlasName = atlasName.Substring(atlasName.IndexOf("Assets/"));

        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasName);
        if (atlas == null)
        {
            var instance = new SpriteAtlas();
            AssetDatabase.CreateAsset(instance, atlasName);

            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasName);
        }
        
        NormalizeSpriteAtlas(atlas,! atlasSet.bAtlasCompress);
       
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

    private static void UpdateSpriteAtlas(string dirPath, bool bRecursive = true)
    {
        string atlasName = dirPath + "/" + Path.GetFileNameWithoutExtension(dirPath) + ".spriteatlas";
        atlasName = atlasName.Replace("\\", "/");
        atlasName = atlasName.Substring(atlasName.IndexOf("Assets/"));

        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasName);
        if (atlas == null)
        {
            var instance = new SpriteAtlas();
            AssetDatabase.CreateAsset(instance, atlasName);

            atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasName);
        }

        NormalizeSpriteAtlas(atlas);

        string assetRelativePath = Path.GetDirectoryName(atlasName);
        // 获取同目录下所有Sprites
        string[] spriteGUIDNames = AssetDatabase.FindAssets("t:sprite", new string[] { assetRelativePath });

        SerializedObject alsObject = new SerializedObject(atlas);
        SerializedProperty packables = alsObject.FindProperty("m_EditorData.packables");
        packables.ClearArray();

        for (int j = 0; j < spriteGUIDNames.Length; ++j)
        {
            string spriteName = AssetDatabase.GUIDToAssetPath(spriteGUIDNames[j]);

            // 子目录的Sprite，不添加
            if (Path.GetDirectoryName(spriteName) != assetRelativePath)
                continue;

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spriteName);

            // 长宽大于1024，不添加
            if (sprite.texture.width > 1024 || sprite.texture.height > 1024)
                continue;

            // 占了1/4以上区域的，不添加
            if (sprite.texture.width * sprite.texture.height > 512 * 512)
                continue;

            // 有UnPackToAltas标签，不添加
            bool needPack = true;
            string[] assetLabels = AssetDatabase.GetLabels(sprite);
            if (assetLabels != null)
            {
                foreach (var label in assetLabels)
                {
                    if (label == "UnPackToAltas")
                    {
                        needPack = false;
                        break;
                    }
                }
            }

            if (needPack)
            {
                packables.InsertArrayElementAtIndex(0);
                SerializedProperty elementProperty = packables.GetArrayElementAtIndex(0);
                elementProperty.objectReferenceValue = sprite;
            }
        }

        alsObject.ApplyModifiedProperties();
        SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);

        if (bRecursive)
        {
            string[] subDirs = Directory.GetDirectories(dirPath);
            for (int i = 0; i < subDirs.Length; ++i)
            {
                string dirName = subDirs[i];
                dirName.Replace("\\", "/");

                UpdateSpriteAtlas(dirName);
            }
        }
    }

    [MenuItem("[TM工具集]/UI相关/更新合图")]
    public static void UpdateSpriteAtlas()
    {
        try
        {
            string[] hetuDirs = Directory.GetDirectories(Application.dataPath + "/Resources/UI/Image/Hetu");

            float fProgress = 0;
            for (int i = 0; i < hetuDirs.Length; ++i)
            {
                string dirName = hetuDirs[i];
                dirName.Replace("\\", "/");

                EditorUtility.DisplayProgressBar("", "处理" + Path.GetFileName(dirName), fProgress++ / hetuDirs.Length);

                //UpdateSpriteAtlas(dirName);
                FilterSpriteAtlas(dirName);
            }

            AssetDatabase.SaveAssets();
        }
        /*
                catch
                {
                    EditorUtility.DisplayDialog("Error", "发生错误", "OK");
                    throw;
                }*/
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    [MenuItem("[TM工具集] / UI相关 / 更新选中纹理对象所在目录合图")]
    public static void UpdateSelectSpriteAtlas()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(Texture), UnityEditor.SelectionMode.Assets);

        if (selection == null || selection.Length <= 0)
        {
            Logger.LogError("请选择一个纹理对象");
            return;
        }

        foreach (var atlas in selection)
        {
            string assetPath = AssetDatabase.GetAssetPath(atlas);
            assetPath.Replace("\\", "/");
            //UpdateSpriteAtlas(Path.GetDirectoryName(assetPath), false);
            FilterSpriteAtlas(Path.GetDirectoryName(assetPath), false);
        }
    }

    //[MenuItem("Assets/拆分SpriteAtlas", false)]
    //private static void SplitSelectedSpriteAtlas()
    //{
    //    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
    //    MySplitSpriteAtlas splitScript = new GameObject("SplitSpriteAtlas").AddComponent<MySplitSpriteAtlas>();
    //    EditorApplication.isPlaying = true;
    //}
    public static int RemoveRepeatGuid(ref string[] allGuid)
    {
        int curIndex = 0;
        for (int i = 0; i < allGuid.Length; i++)
        {
            while (true)
            {
                if (i + 1 >= allGuid.Length)
                    break;
                if (allGuid[i + 1] != allGuid[curIndex])
                {
                    curIndex++;
                    allGuid[curIndex] = allGuid[i + 1];
                    break;
                }
                i++;
            }
        }
        curIndex++;
        return curIndex;
    }
}