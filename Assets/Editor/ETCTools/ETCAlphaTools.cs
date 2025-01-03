using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class ETCAlphaTools
{

    public static float defaultSizeScale = 1;   //the size decrease scale for alphaTexture
    public static Dictionary<string, bool> texturesAlphaDic = new Dictionary<string, bool>();

    #region 贴图尺寸修正
    [MenuItem("Assets/ETCTools/1-贴图尺寸修正")]
    static void SelectTextureModify()
    {
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        int total = os.Length;
        int i = 0;

        List<string> files = new List<string>();

        // 扫描选择的图片
        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (EditorUtility.DisplayCancelableProgressBar("扫描图片", FormatProgressString(i, total, path), ((float)i / total)))
            {
                ETCLogger.I("取消扫描图片");
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            if (!string.IsNullOrEmpty(path) && IsTextureFile(path))   //full name
            {
                if (!CheckTextureSize(path))
                {
                    files.Add(path);
                }
            }
        }

        i = 0;
        total = files.Count;
        // 开始修正图片尺寸
        foreach (string texturePath in files)
        {
            i++;
            if (EditorUtility.DisplayCancelableProgressBar("修正图片尺寸", FormatProgressString(i, total, texturePath), ((float)i / total)))
            {
                ETCLogger.I("取消修正图片尺寸");
                break;
            }

            ExpandTexture(texturePath);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    [MenuItem("[TM工具集]/ETCTools/批处理/1-UI贴图尺寸修正")]
    static void BatchTextureModify()
    {
        string[] fileExts = new string[] { ".png", ".jpg", ".tga" };
        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/UI", ref files, fileExts);
        SearchFile(Application.dataPath + "/Resources/UIFlatten", ref files, fileExts);
        SearchFile(Application.dataPath + "/Resources/UIPacked", ref files, fileExts);

        int total = files.Count;
        int i = 0;

        List<string> texfiles = new List<string>();

        foreach (string texPath in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("扫描图片", FormatProgressString(i, total, texPath), ((float)i / total)))
            {
                ETCLogger.I("取消扫描图片");
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            if (!CheckTextureSize(texPath))
            {
                texfiles.Add(texPath);
            }
        }

        i = 0;
        total = texfiles.Count;

        // 开始修正图片尺寸
        foreach (string texturePath in texfiles)
        {
            i++;
            if (EditorUtility.DisplayCancelableProgressBar("修正图片尺寸", FormatProgressString(i, total, texturePath), ((float)i / total)))
            {
                ETCLogger.I("取消修正图片尺寸");
                break;
            }

            ExpandTexture(texturePath);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void ExpandTexture(string path)
    {
        SetTextureReadable(path);

        Texture2D srcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (srcTex == null)
        {
            ETCLogger.E("加载帖图失败: {0}", path);
            SetTextureReadable(path, false);
            return;
        }

        Texture2D newTex = null;

        int newWidth = GetNearest(srcTex.width);
        int newHeight = GetNearest(srcTex.height);

        if (newWidth <= 0 || newHeight <= 0)
        {
            ETCLogger.E("帖图尺寸出错: {1}:{2} {0}", path, srcTex.width, srcTex.height);
            SetTextureReadable(path, false);
            return;
        }

        string fileExt = Path.GetExtension(path).ToLower();
        TextureFormat texFmt = TextureFormat.RGB24;

        if ((fileExt == ".png" || fileExt == ".tga") && HasAlphaChannel(srcTex))
        {
            texFmt = TextureFormat.RGBA32;
        }

        newTex = new Texture2D(newWidth, newHeight, texFmt, false);

        Color blackColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        for (int i = 0; i < newWidth; i++)
            for (int j = 0; j < newHeight; j++)
            {
                newTex.SetPixel(i, j, blackColor);
            }

        for (int i = 0; i < srcTex.width; ++i)
            for (int j = 0; j < srcTex.height; ++j)
            {
                Color color = srcTex.GetPixel(i, j);
                Color rgbColor = color;
                newTex.SetPixel(i, j, rgbColor);
            }

        newTex.Apply();

        byte[] bytes = null;
        if (fileExt == ".jpg")
        {
            bytes = newTex.EncodeToJPG();
        }
        else if (fileExt == ".png")
        {
            bytes = newTex.EncodeToPNG();
        }
        else
        {
            ETCLogger.E("不支持的格式: {0}", path);
            SetTextureReadable(path, false);
            return;
        }

        File.Delete(path);
        File.WriteAllBytes(path, bytes);

        // AssetDatabase.Refresh();

        string baseName = Path.GetFileNameWithoutExtension(path);

        TextureImporter ti = TextureImporter.GetAtPath(path) as TextureImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Multiple;
        ti.mipmapEnabled = false;
        ti.textureCompression = TextureImporterCompression.Compressed;
        ti.compressionQuality = (int) UnityEditor.TextureCompressionQuality.Normal;
        ti.isReadable = false;

        ArrayList metas = new ArrayList();
        SpriteMetaData meta = new SpriteMetaData();
        meta.name = baseName;
        meta.rect = new Rect(0, 0, srcTex.width, srcTex.height);
        meta.alignment = 0;
        meta.pivot = new Vector2(0.5f, 0.5f);
        metas.Add(meta);

        ti.spritesheet = metas.ToArray(typeof(SpriteMetaData)) as SpriteMetaData[];

        AssetDatabase.ImportAsset(path);

        ETCLogger.I("帖图转换成功: {0}", path);
    }
    #endregion 贴图尺寸修正

    #region 帖图分离Alpha通道
    [MenuItem("Assets/ETCTools/2-分离通道")]
    static void SeperateAllTexturesRGBandAlphaChannel()
    {
        SeperateAllTexturesRGBandAlphaChannelEx(1);
    }

    //[MenuItem("Assets/ETCTools/分离通道-0.5")]
    //static void SeperateAllTexturesRGBandAlphaChannelHalf()
    //{
    //    SeperateAllTexturesRGBandAlphaChannelEx(0.5f);
    //}

    [MenuItem("[TM工具集]/ETCTools/批处理/2-分离Alpha通道")]
    static void BatchSeperateTextureChannel()
    {
        BatchSeperateTextureChannelEx(1);
    }

    //[MenuItem("[TM工具集]/ETCTools/批处理/分离Alpha通道-0.5")]
    //static void BatchSeperateTextureChannelHalf()
    //{
    //    BatchSeperateTextureChannelEx(0.5f);
    //}

    static void BatchSeperateTextureChannelEx(float sizeScale)
    {
        string[] excludePaths = new string[] {
            "UI/Font/",
            "UI/Image/Emotion/",
            "UI/Effect",
            "UIFlatten/Effect",
            "UIPacked/pck_Number.png",
            "UIPacked/p-Number.png"
        };
        string[] fileExts = new string[] { ".png" };

        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/UI", ref files, fileExts, excludePaths);
        SearchFile(Application.dataPath + "/Resources/UIFlatten", ref files, fileExts, excludePaths);
        SearchFile(Application.dataPath + "/Resources/UIPacked", ref files, fileExts, excludePaths);

        int i = 0;
        int total = files.Count;

        List<string> alphaFiles = new List<string>();

        foreach (string texPath in files)
        {
            if(EditorUtility.DisplayCancelableProgressBar("扫描", FormatProgressString(i, total, texPath), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            i++;

            SeperateRGBAandAlphaChannel(texPath, sizeScale);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void SeperateAllTexturesRGBandAlphaChannelEx(float sizeScale)
    {
        // string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        int total = os.Length;
        int i = 0;

        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            if (!string.IsNullOrEmpty(path) && IsTextureFile(path))   //full name
            {
                SeperateRGBAandAlphaChannel(path, sizeScale);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    public static void SeperateRGBAandAlphaChannel(string _texPath, float sizeScale, bool genMaterial = true)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        SetTextureReadable(assetRelativePath, true);
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file
        if (!sourcetex)
        {
            ETCLogger.E("Load Texture Failed : " + assetRelativePath);
            SetTextureReadable(assetRelativePath, false);
            return;
        }
        if (!HasAlphaChannel(sourcetex))
        {
            SetTextureReadable(assetRelativePath, false);
            return;
        }

        if (!Get2Flag(sourcetex.width) || !Get2Flag(sourcetex.height))
        {
            ETCLogger.E("ETC1通道切分: 贴图非2的N次幂 {0}x{1}:{2}", sourcetex.width, sourcetex.height, _texPath);
            SetTextureReadable(assetRelativePath, false);
            return;
        }

        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        Texture2D alphaTex = new Texture2D((int)(sourcetex.width * sizeScale), (int)(sourcetex.height * sizeScale), TextureFormat.RGB24, false);

        for (int i = 0; i < sourcetex.width; ++i)
            for (int j = 0; j < sourcetex.height; ++j)
            {
                Color color = sourcetex.GetPixel(i, j);
                Color rgbColor = color;
                Color alphaColor = color;
                alphaColor.r = color.a;
                alphaColor.g = color.a;
                alphaColor.b = color.a;
                rgbTex.SetPixel(i, j, rgbColor);
                alphaTex.SetPixel((int)(i * sizeScale), (int)(j * sizeScale), alphaColor);
            }

        rgbTex.Apply();
        alphaTex.Apply();
        // AssetDatabase.DeleteAsset(_texPath + ".png");
        byte[] bytes = rgbTex.EncodeToPNG();
        File.Delete(_texPath);
        File.WriteAllBytes(GetRGBTexPath(_texPath), bytes);
        bytes = alphaTex.EncodeToPNG();
        string alphaPath = GetAlphaTexPath(_texPath);
        File.Delete(alphaPath);
        File.WriteAllBytes(alphaPath, bytes);

        SetTextureReadable(_texPath, false);
        ETCLogger.I("Succeed to seperate RGB and Alpha channel for texture : " + assetRelativePath);

        AssetDatabase.Refresh();

        ConfigUITexture(_texPath);
        ConfigUITexture(alphaPath);

        if(genMaterial)
        {
            CreateTextureMaterial(_texPath);
        }
    }

    static void ConfigUITexture(string path)
    {
        TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);

        if (ti.compressionQuality == (int)UnityEditor.TextureCompressionQuality.Normal
            && ti.textureCompression == TextureImporterCompression.Compressed
            && ti.isReadable == false
            && ti.mipmapEnabled == false)
        {
            return;
        }

        ti.isReadable = false;
        ti.textureCompression = TextureImporterCompression.Compressed;
        ti.compressionQuality = (int)UnityEditor.TextureCompressionQuality.Normal;
        ti.mipmapEnabled = false;
        AssetDatabase.ImportAsset(path);
    }
    #endregion 帖图分离Alpha通道

    #region UI贴图压缩选项
    [MenuItem("[TM工具集]/ETCTools/批处理/3-UI贴图压缩选项")]
    static void BatchConfigUITexture()
    {
        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/UI", ref files, new string[] { ".png", ".jpg", ".tga" });
        SearchFile(Application.dataPath + "/Resources/UIFlatten", ref files, new string[] { ".png", ".jpg", ".tga" });
        SearchFile(Application.dataPath + "/Resources/UIPacked", ref files, new string[] { ".png", ".jpg", ".tga" });

        int i = 0;
        int total = files.Count;

        List<string> alphaFiles = new List<string>();

        foreach (string texPath in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, texPath), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            i++;

            ConfigUITexture(texPath);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ETCTools/3-UI贴图压缩选项")]
    static void SelectConfigUITexture()
    {
        // string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        int total = os.Length;
        int i = 0;

        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            if (!string.IsNullOrEmpty(path) && IsTextureFile(path))   //full name
            {
                ConfigUITexture(path);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
    #endregion UI贴图压缩选项

    #region UI预制体替换材质
    [MenuItem("Assets/ETCTools/4-UI预制体替换材质")]
    static void SelectPrefabChangeMaterial()
    {
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        List<string> files = new List<string>();

        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (Path.GetExtension(path) == ".prefab")
            {
                files.Add(path);
            }
        }

        int total = files.Count;
        int i = 0;
        foreach (string path in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("预制体替换材质", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            PrefabChangeMaterial(path);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    [MenuItem("[TM工具集]/ETCTools/批处理/4-UI预制体替换材质")]
    static void BatchPrefabChangeMaterial()
    {
        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/UI", ref files, new string[] { ".prefab" });
        SearchFile(Application.dataPath + "/Resources/UIFlatten", ref files, new string[] { ".prefab" });
        SearchFile(Application.dataPath + "/Resources/UIPacked", ref files, new string[] { ".prefab" });

        int i = 0;
        int total = files.Count;

        foreach (string path in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("预制体替换材质", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            PrefabChangeMaterial(path);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void PrefabChangeMaterial(string prefabPath)
    {
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (null == obj)
        {
            return;
        }

        bool isDirty = false;
        Image img = obj.GetComponent<Image>();
        if(null != img)
        {
            if (PrefabSetImageMaterial(ref img))
            {
                isDirty = true;
            }
        }

        Image[] imgs = obj.GetComponentsInChildren<Image>(true);
        for(int i = 0;null != imgs && i < imgs.Length;i++)
        {
            if(PrefabSetImageMaterial(ref imgs[i]))
            {
                isDirty = true;
            }
        }

        if (isDirty)
        {
            ETCLogger.I("预制体成功替换材质:" + prefabPath);

            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
    }

    static bool PrefabSetImageMaterial(ref Image img)
    {
        if (img.sprite == null)
        {
            return false;
        }

        string spriteTex = AssetDatabase.GetAssetPath(img.sprite);
        string fileExt = Path.GetExtension(spriteTex);
        if (fileExt == null || fileExt.Length <= 0)
        {
            return false;
        }

        string materialPath = spriteTex.Replace(fileExt, "_Material.mat");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            return false;
        }
        img.material = material;

        return true;
    }
    #endregion UI预制体替换材质

    #region ComCommonBind修改
    [MenuItem("Assets/ETCTools/5-ComCommonBind修改")]
    static void SelectPrefabComCommonBind()
    {
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        List<string> files = new List<string>();

        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (Path.GetExtension(path) == ".prefab")
            {
                files.Add(path);
            }
        }

        int total = files.Count;
        int i = 0;
        foreach (string path in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            PrefabComCommonBind(path);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    [MenuItem("[TM工具集]/ETCTools/批处理/5-ComCommonBind修改")]
    static void BatchPrefabComCommonBind()
    {
        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/UI", ref files, new string[] { ".prefab" });
        SearchFile(Application.dataPath + "/Resources/UIFlatten", ref files, new string[] { ".prefab" });
        SearchFile(Application.dataPath + "/Resources/UIPacked", ref files, new string[] { ".prefab" });

        int i = 0;
        int total = files.Count;

        foreach (string path in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            PrefabComCommonBind(path);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void PrefabComCommonBind(string prefabPath)
    {
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (null == obj)
        {
            return;
        }

        bool isDirty = false;

        ComCommonBind component = obj.GetComponent<ComCommonBind>();
        if(null != component)
        {
            for (int i = 0; i < component.reses.Length; i++)
            {
                if (null == component.reses[i].sprite || null == component.reses[i].sprite.texture)
                {
                    continue;
                }

                string texPath = AssetDatabase.GetAssetPath(component.reses[i].sprite.texture);
                string matPath;
                string texExt = Path.GetExtension(texPath);
                if (texExt == null || texExt.Length <= 0)
                {
                    matPath = texPath + "_Material.mat";
                }
                else
                {
                    matPath = texPath.Replace(texExt, "_Material.mat");
                }

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if (null != mat)
                {
                    isDirty = true;
                    component.reses[i].material = mat;
                }
            }
        }

        ComCommonBind[] components = obj.GetComponentsInChildren<ComCommonBind>(true);
        if(components != null && components.Length > 0)
        {
            ETCLogger.I("预制:{0} 包含ComCommonBind", prefabPath);
        }

        foreach (ComCommonBind com in components)
        {
            // isDirty = true;
            for(int i = 0;i < com.reses.Length;i++)
            {
                if(null == com.reses[i].sprite || null == com.reses[i].sprite.texture)
                {
                    continue;
                }

                string texPath = AssetDatabase.GetAssetPath(com.reses[i].sprite.texture);
                string matPath;
                string texExt = Path.GetExtension(texPath);
                if(texExt == null || texExt.Length <= 0)
                {
                    matPath = texPath + "_Material.mat";
                }
                else
                {
                    matPath = texPath.Replace(texExt, "_Material.mat");
                }

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if(null != mat)
                {
                    isDirty = true;
                    com.reses[i].material = mat;
                }
            }
        }

        if (isDirty)
        {
            ETCLogger.I("ComCommonBind添加材质:" + prefabPath);

            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
    }
    #endregion ComCommonBind修改

    #region Actor目录贴图限制256
    [MenuItem("Assets/ETCTools/贴图限制256")]
    static void SelectTextureLimit()
    {
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        List<string> files = new List<string>();

        // 扫描选择的图片
        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (!string.IsNullOrEmpty(path) && IsTextureFile(path))   //full name
            {
                files.Add(path);
            }
        }

        int total = files.Count;
        int i = 0;

        foreach (string texPath in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, texPath), ((float)i / total)))
            {
                break;
            }

            if (!string.IsNullOrEmpty(texPath) && IsTextureFile(texPath))   //full name
            {
                TextureLimit(texPath);
            }

            i++;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    [MenuItem("[TM工具集]/ETCTools/批处理/Actor贴图限制256")]
    static void BatchActorTextureLimit()
    {
        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/Actor", ref files, new string[] {".png", ".jpg", ".tga"});

        int total = files.Count;
        int i = 0;

        foreach (string texPath in files)
        {
            if(EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, texPath), ((float)i / total)))
            {
                break;
            }

            if (!string.IsNullOrEmpty(texPath) && IsTextureFile(texPath))   //full name
            {
                TextureLimit(texPath);
            }

            i++;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void TextureLimit(string path)
    {
        TextureImporter ti = TextureImporter.GetAtPath(path) as TextureImporter;
        if (ti == null)
        {
            return;
        }

        if(ti.maxTextureSize == 256 
            && ti.compressionQuality == (int)UnityEditor.TextureCompressionQuality.Normal 
            && ti.textureCompression == TextureImporterCompression.Compressed 
            && ti.isReadable == false
            && ti.mipmapEnabled == false)
        {
            return;
        }

        TextureImporterSettings tis = new TextureImporterSettings();
        ti.isReadable = false;
        ti.ReadTextureSettings(tis);
        ti.maxTextureSize = 256;
        ti.textureCompression = TextureImporterCompression.Compressed;
        ti.compressionQuality = (int)UnityEditor.TextureCompressionQuality.Normal;
        ti.mipmapEnabled = false;
        AssetDatabase.ImportAsset(path);

    }
    #endregion Actor目录贴图限制256

    #region Actor压缩模型
    [MenuItem("[TM工具集]/ETCTools/批处理/Actor压缩模型")]
    static void BatchModelCompress()
    {
        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/Actor", ref files, new string[] { ".fbx" });

        int total = files.Count;
        int i = 0;

        foreach (string meshPath in files)
        {
            if(EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, meshPath), ((float)i / total)))
            {
                break;
            }

            ModelCompress(meshPath);
            i++;
        }

        EditorUtility.ClearProgressBar();

        AssetDatabase.Refresh();
    }

    static void ModelCompress(string path)
    {
        ModelImporter mi = ModelImporter.GetAtPath(path) as ModelImporter;
        if (mi == null)
        {
            return;
        }

        if(mi.meshCompression == ModelImporterMeshCompression.Medium
            && mi.isReadable == false 
            && mi.importBlendShapes == false)
        {
            return;
        }

        mi.meshCompression = ModelImporterMeshCompression.Medium;
        mi.isReadable = false;
        mi.importBlendShapes = false;
        AssetDatabase.ImportAsset(path);
    }
    #endregion Actor压缩模型

    #region 场景处理
    [MenuItem("[TM工具集]/ETCTools/Scene/扫描场景Shader")]
    static void SearchSceneShader()
    {
        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/Scene", ref files, new string[] { ".mat" });

        Dictionary<string, string> shaders = new Dictionary<string, string>();

        int total = files.Count;
        int i = 0;
        foreach (string path in files)
        {
            i++;
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (null == mat || null == mat.shader)
            {
                continue;
            }

            string shaderPath = AssetDatabase.GetAssetPath(mat.shader);
            if (!shaders.ContainsKey(shaderPath))
            {
                shaders.Add(shaderPath, mat.shader.name);
            }
        }

        foreach (string path in shaders.Keys)
        {
            ETCLogger.I("Shader: {0}", path);
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("[TM工具集]/ETCTools/Scene/1-分离通道")]
    static void BatchSeperateSceneTextureAlpha()
    {
        string[] excludePaths = null;
        string[] fileExts = new string[] { ".png", ".tga" };

        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/Scene", ref files, fileExts, excludePaths);

        int i = 0;
        int total = files.Count;

        List<string> alphaFiles = new List<string>();

        foreach (string texPath in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, texPath), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            i++;

            SeperateSceneAlphaChannel(texPath, false);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ETCTools/1-场景分离通道")]
    static void SelectSeperateSceneTextureAlpha(float sizeScale)
    {
        // string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        int total = os.Length;
        int i = 0;

        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            if (!string.IsNullOrEmpty(path) && IsTextureFile(path))   //full name
            {
                SeperateSceneAlphaChannel(path);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    public static void SeperateSceneAlphaChannel(string _texPath, bool genMaterial = true)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        SetTextureReadable(assetRelativePath, true);
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file
        if (!sourcetex)
        {
            ETCLogger.E("Load Texture Failed : " + assetRelativePath);
            SetTextureReadable(assetRelativePath, false);
            return;
        }
        if (!HasAlphaChannel(sourcetex))
        {
            SetTextureReadable(assetRelativePath, false);
            return;
        }

        if (!Get2Flag(sourcetex.width) || !Get2Flag(sourcetex.height))
        {
            ETCLogger.E("ETC1通道切分: 贴图非2的N次幂 {0}x{1}:{2}", sourcetex.width, sourcetex.height, _texPath);
            SetTextureReadable(assetRelativePath, false);
            return;
        }

        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        Texture2D alphaTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);

        for (int i = 0; i < sourcetex.width; ++i)
            for (int j = 0; j < sourcetex.height; ++j)
            {
                Color color = sourcetex.GetPixel(i, j);
                Color rgbColor = color;
                Color alphaColor = color;
                alphaColor.r = color.a;
                alphaColor.g = color.a;
                alphaColor.b = color.a;
                rgbTex.SetPixel(i, j, rgbColor);
                alphaTex.SetPixel(i, j, alphaColor);
            }

        rgbTex.Apply();
        alphaTex.Apply();
        // AssetDatabase.DeleteAsset(_texPath + ".png");
        byte[] bytes = rgbTex.EncodeToPNG();
        File.Delete(_texPath);
        File.WriteAllBytes(_texPath, bytes);
        bytes = alphaTex.EncodeToPNG();
        string alphaPath = GetAlphaTexPath(_texPath);
        File.Delete(alphaPath);
        File.WriteAllBytes(alphaPath, bytes);

        SetTextureReadable(_texPath, false);
        ETCLogger.I("Succeed to seperate RGB and Alpha channel for texture : " + assetRelativePath);

        AssetDatabase.Refresh();

        ConfigUITexture(_texPath);
        ConfigUITexture(alphaPath);

        if (genMaterial)
        {
            CreateTextureMaterial(_texPath);
        }
    }

    [MenuItem("[TM工具集]/ETCTools/Scene/2-替换材质")]
    static void BatchReplaceSceneMaterial()
    {
        string[] excludePaths = null;
        string[] fileExts = new string[] { ".mat" };

        List<string> files = new List<string>();
        SearchFile(Application.dataPath + "/Resources/Scene", ref files, fileExts, excludePaths);

        int i = 0;
        int total = files.Count;

        List<string> alphaFiles = new List<string>();

        foreach (string matPath in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, matPath), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            i++;

            ReplaceSceneMaterial(matPath);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/ETCTools/2-场景替换材质")]
    static void SelectReplaceSceneMaterial(float sizeScale)
    {
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        if (os == null)
        {
            return;
        }

        List<string> files = new List<string>();

        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (Path.GetExtension(path) == ".mat")
            {
                files.Add(path);
            }
        }

        int total = files.Count;
        int i = 0;
        foreach (string path in files)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", FormatProgressString(i, total, path), (float)i / total))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            i++;

            ReplaceSceneMaterial(path);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void ReplaceSceneMaterial(string path)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if(null == mat)
        {
            ETCLogger.E("材质球加载失败: {0}", path);
            return;
        }

        if(null == mat.shader)
        {
            ETCLogger.E("材质球没有设置Shader: {0}", path);
            return;
        }

        if(mat.shader.name != "HeroGo/General/UnLit/HG_Unlit_Dye_Transparent" && mat.shader.name != "HeroGo/General/UnLit/HG_Unlit_Dye_TransparentETC1")
        {
            return;
        }

        mat.shader = Shader.Find("HeroGo/General/UnLit/HG_Unlit_Dye_TransparentETC1");
        if(null == mat.shader)
        {
            ETCLogger.E("HG_UnLit_Dye_TransparentETC1.shader载入失败");
            return;
        }

        Texture mainTex = mat.GetTexture("_MainTex");
        if(null == mainTex)
        {
            ETCLogger.E("材质未设置_MainTex: {0}", path);
            return;
        }

        string mainTexPath = AssetDatabase.GetAssetPath(mainTex);
        string texExt = Path.GetExtension(mainTexPath);
        string alphaTexPath = mainTexPath + "_Alpha.png";
        if(null != texExt && texExt.Length > 0)
        {
            alphaTexPath = mainTexPath.Replace(texExt, "_Alpha.png");
        }

        Texture alphaTex = AssetDatabase.LoadAssetAtPath<Texture>(alphaTexPath);
        mat.SetTexture("_AlphaTex", alphaTex);

        AssetDatabase.ImportAsset(path);

        ETCLogger.I("材质球替换成功: {0}", path);
    }

    #endregion 场景处理

    #region 清理日志
    [MenuItem("[TM工具集]/ETCTools/清理日志")]
    static void ClearLogger()
    {
        ETCLogger.ClearLogger();
    }
    #endregion 清理日志

    #region 其它
    static int[] npotValues = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };

    static int GetNearest(int v)
    {
        for(int i = 0;i < npotValues.Length;i++)
        {
            if(v <= npotValues[i])
            {
                return npotValues[i];
            }
        }

        return 0;
    }

    static bool CheckTextureSize(string path)
    {
        Texture2D srcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (srcTex == null)
        {
            ETCLogger.E("加载帖图失败: {0}", path);
            return true;
        }

        if (Get2Flag(srcTex.width) && Get2Flag(srcTex.height))
        {
            return true;
        }

        return false;
    }

    static List<string> CheckTextureDeps(string path)
    {
        List<string> dirs = new List<string>();
        SearchFile(Application.dataPath + "/Resources/UI", ref dirs, new string[] { ".prefab" });
        SearchFile(Application.dataPath + "/Resources/UIFlatten", ref dirs, new string[] { ".prefab" });
        SearchFile(Application.dataPath + "/Resources/UIPacked", ref dirs, new string[] { ".prefab" });

        return dirs;
    }

    public static void SearchFile(string dirPath, ref List<string> files, string[] fileExts, string[] excludePaths = null)
    {
        string ext = "";
        string path1 = "";
        bool skip = false;

        foreach (string path in Directory.GetFiles(dirPath))
        {
            path1 = path.Replace("\\", "/");

            // 排除文件
            skip = false;
            if(null != excludePaths)
            {
                foreach (string excludePath in excludePaths)
                {
                    if (path1.IndexOf(excludePath) >= 0)
                    {
                        skip = true;
                        break;
                    }
                }
            }

            if(skip)
            {
                continue;
            }

            ext = Path.GetExtension(path1).ToLower();
            foreach(string fileExt in fileExts)
            {
                if(ext == fileExt)
                {
                    files.Add(path1.Substring(path1.IndexOf("Assets")));
                    break;
                }
            }
        }

        if (Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹  
        {
            foreach (string path in Directory.GetDirectories(dirPath))
            {
                SearchFile(path, ref files, fileExts, excludePaths);
            }
        }
    }

    [MenuItem("[TM工具集]/ETCTools/创建材质球")]
    static void CreateETC1Material()
    {
        // string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object o in os)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (!string.IsNullOrEmpty(path) && IsTextureFile(path))   //full name
            {
                CreateTextureMaterial(path);
            }
        }
        AssetDatabase.Refresh();
    }

    #region inspect material

    static string[] GetMaterialTexturesHavingAlphaChannel(Material _mat)
    {
        List<string> alphatexpaths = new List<string>();
        string[] texpaths = GetMaterialTexturePaths(_mat);
        foreach (string texpath in texpaths)
        {
            if (texturesAlphaDic[texpath])
            {
                alphatexpaths.Add(texpath);
            }
        }

        return alphatexpaths.ToArray();
    }

    static string[] GetMaterialTexturePaths(Material _mat)
    {
        List<string> results = new List<string>();
        Object[] roots = new Object[] { _mat };
        Object[] dependObjs = EditorUtility.CollectDependencies(roots);
        foreach (Object dependObj in dependObjs)
        {
            if (dependObj.GetType() == typeof(Texture2D))
            {
                string texpath = AssetDatabase.GetAssetPath(dependObj.GetInstanceID());
                results.Add(texpath);
            }
        }
        return results.ToArray();
    }

    #endregion

    public static string FormatProgressString(int i, int total, string message)
    {
        return string.Format("({0}/{1}) {2}", i, total, message);
    }

    static void CalculateTexturesAlphaChannelDic()
    {
        string[] paths = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            if (!string.IsNullOrEmpty(path) && IsTextureFile(path))   //full name
            {
                string assetRelativePath = GetRelativeAssetPath(path);
                SetTextureReadable(assetRelativePath);
                Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;
                if (!sourcetex)  //make sure the file is really Texture2D which can be loaded as Texture2D.
                {
                    continue;
                }
                if (HasAlphaChannel(sourcetex))
                {
                    AddValueToDic(assetRelativePath, true);
                }
                else
                {
                    AddValueToDic(assetRelativePath, false);
                }
            }
        }
    }

    static void AddValueToDic(string _key, bool _val)
    {
        if (texturesAlphaDic.ContainsKey(_key))
        {
            texturesAlphaDic[_key] = _val;
        }
        else
        {
            texturesAlphaDic.Add(_key, _val);
        }
    }


    #region process texture

    public static bool Get2Flag(int num)
    {
        if (num < 1) return false;
        return (num & num - 1) == 0;
    }

    public static void CreateTextureMaterial(string _texPath)
    {
        if(_texPath.IndexOf("_Alpha.png") >= 0)
        {
            Debug.Log("ETC1通道切分: 通道贴图不需要生成材质球 " + _texPath);
            return;
        }

        int len = Path.GetExtension(_texPath).Length;
        string tempPath = _texPath.Substring(0, _texPath.Length - len);
        Shader etcShader = Shader.Find("UI/DefaultETC1-Custom");
        Material mat = new Material(etcShader);

        //Texture texRGB = AssetDatabase.LoadAssetAtPath(tempPath + ".png", typeof(Texture)) as Texture;
        Texture texAlpha = AssetDatabase.LoadAssetAtPath(tempPath + "_Alpha.png", typeof(Texture)) as Texture;

        mat.SetTexture("_MainTex", null);
        mat.SetTexture("_AlphaTex", texAlpha);

        string matPath = tempPath + "_Material.mat";

        AssetDatabase.CreateAsset(mat, matPath);
        AssetDatabase.Refresh();
        //List<string> dirs = CheckTextureDeps(_texPath);
        //foreach (string prefabPath in dirs)
        //{
        //    ConvertPrefab(prefabPath, _texPath);
        //}
    }

    /// <summary>
    /// 是否有透明通道
    /// </summary>
    /// <param name="_tex"></param>
    /// <returns></returns>
    static bool HasAlphaChannel(Texture2D _tex)
    {
        for (int i = 0; i < _tex.width; ++i)
            for (int j = 0; j < _tex.height; ++j)
            {
                Color color = _tex.GetPixel(i, j);
                float alpha = color.a;
                if (alpha < 1.0f - 0.001f)
                {
                    return true;
                }
            }
        return false;
    }

    /// <summary>
    /// 设置纹理可读
    /// </summary>
    /// <param name="_relativeAssetPath"></param>
    static void SetTextureReadable(string _relativeAssetPath, bool isReadable = true)
    {
        string postfix = GetFilePostfix(_relativeAssetPath);
        if (postfix == ".dds")    // no need to set .dds file.  Using TextureImporter to .dds file would get casting type error.
        {
            return;
        }

        TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
        ti.isReadable = isReadable;
        ti.textureCompression = TextureImporterCompression.Compressed;
        ti.compressionQuality = (int)UnityEditor.TextureCompressionQuality.Normal;
        ti.mipmapEnabled = false;
        AssetDatabase.ImportAsset(_relativeAssetPath);
    }

    #endregion


    #region string or path helper

    static bool IsTextureFile(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".dds") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
    }

    static string GetRGBTexPath(string _texPath)
    {
        return GetTexPath(_texPath, ".");
    }

    static string GetAlphaTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_Alpha.");
    }

    static string GetTexPath(string _texPath, string _texRole)
    {
        string result = _texPath.Replace(".", _texRole);
        string postfix = GetFilePostfix(_texPath);
        return result.Replace(postfix, ".png");
    }

    static string GetRelativeAssetPath(string _fullPath)
    {
        _fullPath = GetRightFormatPath(_fullPath);
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    static string GetRightFormatPath(string _path)
    {
        return _path.Replace("\\", "/");
    }

    /// <summary>
    /// 获取文件后缀
    /// </summary>
    /// <param name="_filepath"></param>
    /// <returns></returns>
    static string GetFilePostfix(string _filepath)   //including '.' eg ".tga", ".dds"
    {
        string postfix = "";
        int idx = _filepath.LastIndexOf('.');
        if (idx > 0 && idx < _filepath.Length)
            postfix = _filepath.Substring(idx, _filepath.Length - idx);
        return postfix;
    }

    #endregion

    #endregion 其它
}
