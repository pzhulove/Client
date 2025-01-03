using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Reflection;
using System;
using System.IO;
public class TextureProcessorData : BaseAssetProcessorData
{
    #region ImporterSetting
    public TextureImporterType textureImporterType;
    public TextureImporterShape textureImporterShape;
    //2D 
    //Default
    public bool sRGBTexture;
    public TextureImporterAlphaSource textureImporterAlphaSource;
    public bool AlphaIsTransparency;
    //Normal
    public bool CreateFromGrayscale;
    public float Bumpiness;
    public TextureImporterNormalFilter textureImporterNormalFilter;

    //Sprite
    public SpriteImportMode spriteImportMode;
    public string spritePackingTag;
    public int pixelsPerUnit;
    public SpriteMeshType spriteMeshType;
    public int ExtrudeEdges;
    public bool generatePhysicsShape;

    //SingleChannel
#if UNITY_2017_1_OR_NEWER
    public TextureImporterSingleChannelComponent singleChannel;
#endif
    //Cubemap
    public TextureImporterGenerateCubemap cubeMapGenerateType;
    public TextureImporterCubemapConvolution cubeMapConvolutionType;
    public bool fixupEdgeSeams;

    //Advanced
    public TextureImporterNPOTScale textureImporterNPOTScale;
    public bool ReadWriteEnabled;
    public bool GenerateMipMaps;
    public bool BorderMipMaps;
    public TextureImporterMipFilter textureImporterMipFilter;
    public bool MipMapsPreserveCoverage;
    public bool FadeoutMipMaps;
    public int FadeOutMipMapStart;
    public int FadeOutMipMapEnd;

    //Uniform
    public TextureWrapMode wrapMode;
    public FilterMode filterMode;
    public int anisoLevel;

    //Platform Setting
    //default
    public int defaultMaxTextureSize;
#if UNITY_2017_1_OR_NEWER
    public TextureResizeAlgorithm defaultTextureResizeAlgorithm;
#endif
    public TextureImporterCompression defaultTextureImporterCompression;
    public bool defaultCrunchCompression;
    public int defaultCompressionQuality;
    //Android
    public bool overrideForAndoird;
    public int androidMaxTextureSize;
#if UNITY_2017_1_OR_NEWER
    public TextureResizeAlgorithm androidTextureResizeAlgorithm;
#endif
    public TextureImporterCompression androidTextureImporterCompression;
    public TextureImporterFormat androidTextureImporterFormat;
    public int androidTextureCompressionQuality;
    public bool androidUsingETC1;
#if UNITY_2017_1_OR_NEWER
    public AndroidETC2FallbackOverride androidETC2FallbackOverride;
#endif

    //iOS
    public bool overrideForiOS;
    public int iOSMaxTextureSize;
#if UNITY_2017_1_OR_NEWER
    public TextureResizeAlgorithm iOSTextureResizeAlgorithm;
#endif
    public TextureImporterCompression iOSTextureImporterCompression;
    public TextureImporterFormat iOSTextureImporterFormat;
    public int iOSTextureCompressionQuality;

    #endregion

    #region ChangeSetCheck
    public bool changeTextureImporterType = false;
    public bool changeTextureImporterShape = false;
    public bool changesRGBTexture = false;
    public bool changeTextureImporterAlphaSource = false;
    public bool changeAlphaIsTransparency = false;
    public bool changeCreateFromGrayscale = false;
    public bool changeBumpiness = false;
    public bool changeTextureImporterNormalFilter = false;
    public bool changeSpriteImportMode = false;
    public bool changeSpritePackingTag = false;
    public bool changePixelsPerUnit = false;
    public bool changeSpriteMeshType = false;
    public bool changeExtrudeEdges = false;
    public bool changeGeneratePhysicsShape = false;
    public bool changeSingleChannel = false;
    public bool changeCubemapGenerateType = false;
    public bool changeCubemapConvolutionType = false;
    public bool changeFixupEdgeSeams = false;
    public bool changeTextureImporterNPOTScale = false;
    public bool changeReadWriteEnabled = false;
    public bool changeGenerateMipMaps = false;
    public bool changeBorderMipMaps = false;
    public bool changeTextureImporterMipFilter = false;
    public bool changeMipMapsPreserveCoverage = false;
    public bool changeFadeoutMipMaps = false;
    public bool changeFadeOutMipMapStart = false;
    public bool changeFadeOutMipMapEnd = false;
    public bool changeWrapMode = false;
    public bool changeFilterMode = false;
    public bool changeAnisoLevel = false;
    //platform setting 
    //default
    public bool changeDefaultMaxTextureSize = false;
    public bool changeDefaultTextureResizeAlgorithm = false;
    public bool changeDefaultTextureImporterCompression = false;
    public bool changeDefaultCrunchCompression = false;
    public bool changeDefaultCompressionQuality = false;
    //Android
    public bool changeOverrideForAndroid = false;
    public bool changeAndroidMaxTextureSize = false;
    public bool changeAndroidTextureResizeAlgorithm = false;
    public bool changeAndroidTextureImporterCompression = false;
    public bool changeAndroidTextureImporterFormat = false;
    public bool changeAndroidCompressionQuality = false;
    public bool changeAndroidUsingETC1 = false;
    public bool changeAndroidETC2FallbakcOverride = false;
    //iOS
    public bool changeOverrideForiOS = false;
    public bool changeiOSMaxTextureSize = false;
    public bool changeiOSTextureResizeAlgorithm = false;
    public bool changeiOSTextureImporterCompression = false;
    public bool changeiOSTextureImporterFormat = false;
    public bool changeiOSCompressionQuality = false;
    #endregion


    private bool showPlatformSetting = false;
    private bool showAdvanced = false;

    private Dictionary<int, string> m_AndroidTexFormatDictionary;
    private Dictionary<int, string> m_iOSTexFormatDictionary;

    public TextureProcessorData()
    {
        assetType = typeof(Texture);

        textureImporterType = TextureImporterType.Default;
        textureImporterShape = TextureImporterShape.Texture2D;
        sRGBTexture = true;
        textureImporterAlphaSource = TextureImporterAlphaSource.FromInput;
        AlphaIsTransparency = false;
        CreateFromGrayscale = false;
        Bumpiness = 0.25f;
        textureImporterNormalFilter = TextureImporterNormalFilter.Standard;
        spriteImportMode = SpriteImportMode.Single;
        spritePackingTag = string.Empty;
        pixelsPerUnit = 100;
        spriteMeshType = SpriteMeshType.Tight;
        ExtrudeEdges = 1;
        generatePhysicsShape = true;

#if UNITY_2017_1_OR_NEWER
        singleChannel = TextureImporterSingleChannelComponent.Alpha;
#endif
        cubeMapGenerateType = TextureImporterGenerateCubemap.AutoCubemap;
        cubeMapConvolutionType = TextureImporterCubemapConvolution.None;
        fixupEdgeSeams = false;
        textureImporterNPOTScale = TextureImporterNPOTScale.None;
        ReadWriteEnabled = false;
        GenerateMipMaps = true;
        BorderMipMaps = false;
        textureImporterMipFilter = TextureImporterMipFilter.BoxFilter;
        MipMapsPreserveCoverage = false;
        FadeoutMipMaps = false;
        FadeOutMipMapStart = 1;
        FadeOutMipMapEnd = 3;
        wrapMode = TextureWrapMode.Repeat;
        filterMode = FilterMode.Bilinear;
        anisoLevel = 1;
        //Platform Setting
        //Default
        defaultMaxTextureSize = 2048;
#if UNITY_2017_1_OR_NEWER
        defaultTextureResizeAlgorithm = TextureResizeAlgorithm.Mitchell;
#endif
        defaultTextureImporterCompression = TextureImporterCompression.Compressed;
        defaultCrunchCompression = false;
        defaultCompressionQuality = 50;
        //Android
        overrideForAndoird = false;
        androidMaxTextureSize = 2048;
#if UNITY_2017_1_OR_NEWER
        androidTextureResizeAlgorithm = TextureResizeAlgorithm.Mitchell;
#endif
        androidTextureImporterCompression = TextureImporterCompression.Compressed;
        androidTextureImporterFormat = TextureImporterFormat.ETC_RGB4;
        androidTextureCompressionQuality = 50;
        androidUsingETC1 = false;
#if UNITY_2017_1_OR_NEWER
        androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;
#endif
        //iOS
        overrideForiOS = false;
        iOSMaxTextureSize = 2048;
#if UNITY_2017_1_OR_NEWER
        iOSTextureResizeAlgorithm = TextureResizeAlgorithm.Mitchell;
#endif
        iOSTextureImporterCompression = TextureImporterCompression.Compressed;
        iOSTextureImporterFormat = TextureImporterFormat.PVRTC_RGB4;
        iOSTextureCompressionQuality = 50;

        //Not Data
        m_AndroidTexFormatDictionary = new Dictionary<int, string>();
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.ETC_RGB4,"RGB Compressed ETC 4 bits");
#if UNITY_2017_1_OR_NEWER
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.ETC_RGB4Crunched,"RGB Crunched ETC");
#endif
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.ETC2_RGB4,"RGB Compressed ETC2 4 bits");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,"RGB + 1-bit Alpha Compressed ETC2 4bits");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.ETC2_RGBA8,"RGBA Compressed ETC2 8bits");
#if UNITY_2017_1_OR_NEWER
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.ETC2_RGBA8Crunched,"RGBA Crunched ETC2");
#endif
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.RGB16,"RGB 16 bit");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.RGB24,"RGB 24 bit");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.Alpha8,"Alpha 8");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.RGBA16,"RGBA 16 bit");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.RGBA32,"RGBA 32 bit");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.RGBAHalf,"RGBA Half");
        m_AndroidTexFormatDictionary.Add((int)TextureImporterFormat.ASTC_RGBA_6x6, "RGBA Compressed ASTC 6x6 block");


        m_iOSTexFormatDictionary = new Dictionary<int, string>();
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.PVRTC_RGB4, "RGB Compressed PVRTC 4 bits");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.PVRTC_RGB2, "RGB Compressed PVRTC 2 bits");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.PVRTC_RGBA2, "RGBA Compressed PVRTC 2 bits");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.PVRTC_RGBA4, "RGBA Compressed PVRTC 4 bits");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.RGB16, "RGB 16 bit");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.RGB24, "RGB 24 bit");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.Alpha8, "Alpha 8");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.RGBA16, "RGBA 16 bit");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.RGBA32, "RGBA 32 bit");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.RGBAHalf, "RGBA Half");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.ASTC_RGBA_4x4, "RGBA Compressed ASTC 4x4 block");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.ASTC_RGBA_5x5, "RGBA Compressed ASTC 5x5 block");
        m_iOSTexFormatDictionary.Add((int)TextureImporterFormat.ASTC_RGBA_6x6, "RGBA Compressed ASTC 6x6 block");
    }

    private string RawName2DisplayName(string rawName)
    {
        switch(rawName)
        {
            case "Bump":
                return "Normal map";
            case "Image":
                return "Default";
            case "GUI":
                return "Editor GUI and legacy GUI";
            case "Texture2D":
                return "2D";
            case "TextureCube":
                return "Cube";
            case "FromInput":
                return "Input Texture Alpha";
            case "FromGrayScale":
                return "From Gray Scale";
            case "Compressed":
                return "Normal Quality";
            case "CompressedHQ":
                return "High Quality";
            case "CompressedLQ":
                return "Low Quality";
            case "Uncompressed":
                return "None";
            case "Standard":
                return "Sharp";
            case "Sobel":
                return "Smooth";
            case "AutoCubemap":
                return "Auto";
            case "Spheremap":
                return "Mirrored Ball(Spheremap)";
            case "Cylindrical":
                return "Latitude-Longitude Layout(Cylindrical)";
            case "FullCubemap":
                return "6 Frames Layout(Cubic Environment)";
            case "Diffuse":
                return "Diffuse(Irradiance)";
            case "Specular":
                return "Specular(Glossy Reflection)";
        }
        return rawName;
    }

    public override void DisplayAndChangeData()
    {
        //图片类型
        EditorGUILayout.BeginHorizontal();
        {
            changeTextureImporterType = EditorGUILayout.Toggle(changeTextureImporterType, GUILayout.Width(10));
            EditorGUI.BeginDisabledGroup(changeTextureImporterType == false);

            GUILayout.Label("Texture Type");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(RawName2DisplayName(textureImporterType.ToString()), GUILayout.Width(200)))
            {
                GenericMenu textureTypeMenu = new GenericMenu();
                textureTypeMenu.AddItem(new GUIContent("Default"), textureImporterType == TextureImporterType.Default,
                                        () => { textureImporterType = TextureImporterType.Default; });
                textureTypeMenu.AddItem(new GUIContent("Normal map"), textureImporterType == TextureImporterType.NormalMap,
                                        () => { textureImporterType = TextureImporterType.NormalMap; });
                textureTypeMenu.AddItem(new GUIContent("Editor GUI and legacy GUI"), textureImporterType == TextureImporterType.GUI,
                                        () => { textureImporterType = TextureImporterType.GUI; });
                textureTypeMenu.AddItem(new GUIContent("Sprite"), textureImporterType == TextureImporterType.Sprite,
                                        () => { textureImporterType = TextureImporterType.Sprite; });
                textureTypeMenu.AddItem(new GUIContent("Cursor"), textureImporterType == TextureImporterType.Cursor,
                                        () => { textureImporterType = TextureImporterType.Cursor; });
                textureTypeMenu.AddItem(new GUIContent("Cookie"), textureImporterType == TextureImporterType.Cookie,
                                        () => { textureImporterType = TextureImporterType.Cookie; });
                textureTypeMenu.AddItem(new GUIContent("Lightmap"), textureImporterType == TextureImporterType.Lightmap,
                                        () => { textureImporterType = TextureImporterType.Lightmap; });
                textureTypeMenu.AddItem(new GUIContent("Single Channel"), textureImporterType == TextureImporterType.SingleChannel,
                                        () => { textureImporterType = TextureImporterType.SingleChannel; });
                textureTypeMenu.ShowAsContext();
            }

            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();

        //Wrap Mode
        EditorGUILayout.BeginHorizontal();
        {
            changeWrapMode = EditorGUILayout.Toggle(changeWrapMode, GUILayout.Width(10));
            EditorGUI.BeginDisabledGroup(changeWrapMode == false);

            GUILayout.Label("Wrap Mode");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(wrapMode.ToString(), GUILayout.Width(200)))
            {
                GenericMenu wrapModeMenu = new GenericMenu();
                wrapModeMenu.AddItem(new GUIContent("Clamp"), wrapMode == TextureWrapMode.Clamp,
                                    () => { wrapMode = TextureWrapMode.Clamp; });
#if UNITY_2017_1_OR_NEWER
                wrapModeMenu.AddItem(new GUIContent("Mirror"), wrapMode == TextureWrapMode.Mirror,
                                    () => { wrapMode = TextureWrapMode.Mirror; });
                wrapModeMenu.AddItem(new GUIContent("MirrorOnce"), wrapMode == TextureWrapMode.MirrorOnce,
                                    () => { wrapMode = TextureWrapMode.MirrorOnce; });
#endif
                wrapModeMenu.AddItem(new GUIContent("Repeat"), wrapMode == TextureWrapMode.Repeat,
                                    () => { wrapMode = TextureWrapMode.Repeat; });
                wrapModeMenu.ShowAsContext();
            }

            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();

        //Filter Mode
        EditorGUILayout.BeginHorizontal();
        {
            changeFilterMode = EditorGUILayout.Toggle(changeFilterMode, GUILayout.Width(10));
            EditorGUI.BeginDisabledGroup(changeFilterMode == false);

            GUILayout.Label("Filter Mode");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(filterMode.ToString(), GUILayout.Width(200)))
            {
                GenericMenu filterModeMenu = new GenericMenu();
                filterModeMenu.AddItem(new GUIContent("Bilinear"), filterMode == FilterMode.Bilinear,
                                        () => { filterMode = FilterMode.Bilinear; });
                filterModeMenu.AddItem(new GUIContent("Point"), filterMode == FilterMode.Point,
                                        () => { filterMode = FilterMode.Point; });
                filterModeMenu.AddItem(new GUIContent("Trilinear"), filterMode == FilterMode.Trilinear,
                                        () => { filterMode = FilterMode.Trilinear; });
                filterModeMenu.ShowAsContext();
            }

            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();

        //Aniso Level
        DisplayIntInfo("Aniso Level", ref anisoLevel, 4, ref changeAnisoLevel);

        EditorGUILayout.LabelField("MipMap", EditorStyles.boldLabel);
        //Generate Mipmaps
        DisplayBoolInfo("Generate MipMaps", ref GenerateMipMaps, 30, ref changeGenerateMipMaps);

        //Border Mipmaps
        DisplayBoolInfo("Border Mip Maps",ref BorderMipMaps,30,ref changeBorderMipMaps);


        //ImporterMipFilter
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Space(30);
            changeTextureImporterMipFilter = EditorGUILayout.Toggle(changeTextureImporterMipFilter,GUILayout.Width(10));
            EditorGUI.BeginDisabledGroup(changeTextureImporterMipFilter == false);

            GUILayout.Label("Mip Map Filtering");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button(textureImporterMipFilter.ToString(),GUILayout.Width(200)))
            {
                GenericMenu mipFilterMenu = new GenericMenu();
                mipFilterMenu.AddItem(new GUIContent("Box"), textureImporterMipFilter == TextureImporterMipFilter.BoxFilter,
                                        () => { textureImporterMipFilter = TextureImporterMipFilter.BoxFilter; });
                mipFilterMenu.AddItem(new GUIContent("KaiserFilter"), textureImporterMipFilter == TextureImporterMipFilter.KaiserFilter,
                                        () => { textureImporterMipFilter = TextureImporterMipFilter.KaiserFilter; });
                mipFilterMenu.ShowAsContext();
            }

            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndHorizontal();

        //MipMap Preserve Coverage
        DisplayBoolInfo("Mip Maps Preserve Coverage", ref MipMapsPreserveCoverage, 30, ref changeMipMapsPreserveCoverage);

        //Fade out Mip Maps
        DisplayBoolInfo("Fadeout Mip Maps", ref FadeoutMipMaps, 30, ref changeFadeoutMipMaps);

        //Fade out Mip Maps Start
        DisplayIntInfo("Fadeout MipMaps Start", ref FadeOutMipMapStart, 50, ref changeFadeOutMipMapStart);


        //Fade out Mip Maps End
        DisplayIntInfo("Fadeout MipMaps End", ref FadeOutMipMapEnd, 50, ref changeFadeOutMipMapEnd);


        #region PlatformSetting
        showPlatformSetting = EditorGUILayout.Foldout(showPlatformSetting, "PlatformSetting");
        if (showPlatformSetting)
        {
            //Default PlatformSetting
            //Max Texture Size
            EditorGUILayout.LabelField("Default", EditorStyles.boldLabel);
            DisplayIntInfo("Max Size", ref defaultMaxTextureSize, 30, ref changeDefaultMaxTextureSize);

#if UNITY_2017_1_OR_NEWER
            //Resize Algorithm
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(30);
                changeDefaultTextureResizeAlgorithm = EditorGUILayout.Toggle(changeDefaultTextureResizeAlgorithm, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeDefaultTextureResizeAlgorithm == false);

                GUILayout.Label("Resize Algorithm");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(defaultTextureResizeAlgorithm.ToString(), GUILayout.Width(200)))
                {
                    GenericMenu resizeAlgorithmMenu = new GenericMenu();
                    resizeAlgorithmMenu.AddItem(new GUIContent("Mitchell"), defaultTextureResizeAlgorithm == TextureResizeAlgorithm.Mitchell,
                                                () => { defaultTextureResizeAlgorithm = TextureResizeAlgorithm.Mitchell; });
                    resizeAlgorithmMenu.AddItem(new GUIContent("Bilinear"), defaultTextureResizeAlgorithm == TextureResizeAlgorithm.Bilinear,
                                                () => { defaultTextureResizeAlgorithm = TextureResizeAlgorithm.Bilinear; });
                    resizeAlgorithmMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
#endif

            //Compression
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(30);
                changeDefaultTextureImporterCompression = EditorGUILayout.Toggle(changeDefaultTextureImporterCompression, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeDefaultTextureImporterCompression == false);

                GUILayout.Label("Compression");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(RawName2DisplayName(defaultTextureImporterCompression.ToString()), GUILayout.Width(200)))
                {
                    GenericMenu compressionMenu = new GenericMenu();
                    compressionMenu.AddItem(new GUIContent("Normal Quality"), defaultTextureImporterCompression == TextureImporterCompression.Compressed,
                                            () => { defaultTextureImporterCompression = TextureImporterCompression.Compressed; });
                    compressionMenu.AddItem(new GUIContent("High Quality"), defaultTextureImporterCompression == TextureImporterCompression.CompressedHQ,
                                            () => { defaultTextureImporterCompression = TextureImporterCompression.CompressedHQ; });
                    compressionMenu.AddItem(new GUIContent("Low Quality"), defaultTextureImporterCompression == TextureImporterCompression.CompressedLQ,
                                            () => { defaultTextureImporterCompression = TextureImporterCompression.CompressedLQ; });
                    compressionMenu.AddItem(new GUIContent("None"), defaultTextureImporterCompression == TextureImporterCompression.Uncompressed,
                                            () => { defaultTextureImporterCompression = TextureImporterCompression.Uncompressed; });
                    compressionMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            //Use Crunch Compression
            DisplayBoolInfo("Use Crunch Compression", ref defaultCrunchCompression, 30, ref changeDefaultCrunchCompression);

            //Compression Quality
            DisplayIntInfo("Compression Quality", ref defaultCompressionQuality, 30, ref changeDefaultCompressionQuality);

            //Android
            /*EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Android", EditorStyles.boldLabel);

            //Override For Android
            DisplayBoolInfo("Override For Android", ref overrideForAndoird, 30);
            changeOverrideForAndroid = overrideForAndoird;
            EditorGUILayout.EndHorizontal();*/

            DisplayHeaderBoolInfo("Android", ref overrideForAndoird, ref changeOverrideForAndroid);

            if (changeOverrideForAndroid && overrideForAndoird)
            {
                //Android MaxTextureSize
                DisplayIntInfo("MaxTextureSize", ref androidMaxTextureSize, 30, ref changeAndroidMaxTextureSize);

#if UNITY_2017_1_OR_NEWER
                //Android Texture Resize Algorithm
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    changeAndroidTextureResizeAlgorithm = EditorGUILayout.Toggle(changeAndroidTextureResizeAlgorithm, GUILayout.Width(10));
                    EditorGUI.BeginDisabledGroup(changeAndroidTextureResizeAlgorithm == false);

                    GUILayout.Label("Resize Algorithm");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(androidTextureResizeAlgorithm.ToString(), GUILayout.Width(200)))
                    {
                        GenericMenu androidResizeAlgorithmMenu = new GenericMenu();
                        androidResizeAlgorithmMenu.AddItem(new GUIContent("Mitchell"), androidTextureResizeAlgorithm == TextureResizeAlgorithm.Mitchell,
                                                    () => { androidTextureResizeAlgorithm = TextureResizeAlgorithm.Mitchell; });
                        androidResizeAlgorithmMenu.AddItem(new GUIContent("Bilinear"), androidTextureResizeAlgorithm == TextureResizeAlgorithm.Bilinear,
                                                    () => { androidTextureResizeAlgorithm = TextureResizeAlgorithm.Bilinear; });
                        androidResizeAlgorithmMenu.ShowAsContext();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
#endif

                //Android Texture Compression
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    changeAndroidTextureImporterCompression = EditorGUILayout.Toggle(changeAndroidTextureImporterCompression, GUILayout.Width(10));
                    EditorGUI.BeginDisabledGroup(changeAndroidTextureImporterCompression == false);

                    GUILayout.Label("Compression");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(RawName2DisplayName(androidTextureImporterCompression.ToString()), GUILayout.Width(200)))
                    {
                        GenericMenu androidCompressionMenu = new GenericMenu();
                        androidCompressionMenu.AddItem(new GUIContent("Normal Quality"), androidTextureImporterCompression == TextureImporterCompression.Compressed,
                                                () => { androidTextureImporterCompression = TextureImporterCompression.Compressed; });
                        androidCompressionMenu.AddItem(new GUIContent("High Quality"), androidTextureImporterCompression == TextureImporterCompression.CompressedHQ,
                                                () => { androidTextureImporterCompression = TextureImporterCompression.CompressedHQ; });
                        androidCompressionMenu.AddItem(new GUIContent("Low Quality"), androidTextureImporterCompression == TextureImporterCompression.CompressedLQ,
                                                () => { androidTextureImporterCompression = TextureImporterCompression.CompressedLQ; });
                        androidCompressionMenu.AddItem(new GUIContent("None"), androidTextureImporterCompression == TextureImporterCompression.Uncompressed,
                                                () => { androidTextureImporterCompression = TextureImporterCompression.Uncompressed; });
                        androidCompressionMenu.ShowAsContext();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                //Android Texture Format
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    changeAndroidTextureImporterFormat = EditorGUILayout.Toggle(changeAndroidTextureImporterFormat, GUILayout.Width(10));
                    EditorGUI.BeginDisabledGroup(changeAndroidTextureImporterFormat == false);

                    GUILayout.Label("Format");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(m_AndroidTexFormatDictionary[(int)androidTextureImporterFormat], GUILayout.Width(200)))
                    {
                        GenericMenu androidTextureFormatMenu = new GenericMenu();
                        foreach (var item in m_AndroidTexFormatDictionary)
                        {
                            androidTextureFormatMenu.AddItem(new GUIContent(item.Value), androidTextureImporterFormat == (TextureImporterFormat)item.Key,
                                                        () => { androidTextureImporterFormat = (TextureImporterFormat)item.Key; });
                        }
                        androidTextureFormatMenu.ShowAsContext();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                //Android Texture Compression Quality
                DisplayIntInfo("Compression Quality", ref androidTextureCompressionQuality, 30, ref changeAndroidCompressionQuality);

                //Android Using ETC1
                DisplayBoolInfo("Compress using ETC1(split alpha channel)", ref androidUsingETC1, 30, ref changeAndroidUsingETC1);


#if UNITY_2017_1_OR_NEWER
                //AndroidETC2FallbackOverride
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    changeAndroidETC2FallbakcOverride = EditorGUILayout.Toggle(changeAndroidETC2FallbakcOverride, GUILayout.Width(10));
                    EditorGUI.BeginDisabledGroup(changeAndroidETC2FallbakcOverride == false);

                    GUILayout.Label("Override ETC2 fallback");
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button(RawName2DisplayName(androidETC2FallbackOverride.ToString()),GUILayout.Width(200)))
                    {
                        GenericMenu androidETC2FallbackMenu = new GenericMenu();
                        androidETC2FallbackMenu.AddItem(new GUIContent("Use build setting"), androidETC2FallbackOverride == AndroidETC2FallbackOverride.UseBuildSettings,
                                                        () => { androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings; });
                        androidETC2FallbackMenu.AddItem(new GUIContent("32 - bit"), androidETC2FallbackOverride == AndroidETC2FallbackOverride.Quality32Bit,
                                                       () => { androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit; });
                        androidETC2FallbackMenu.AddItem(new GUIContent("16 - bit"), androidETC2FallbackOverride == AndroidETC2FallbackOverride.Quality16Bit,
                                                       () => { androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality16Bit; });
                        androidETC2FallbackMenu.AddItem(new GUIContent("32 - bit(half resolution)"), androidETC2FallbackOverride == AndroidETC2FallbackOverride.Quality32BitDownscaled,
                                                       () => { androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32BitDownscaled; });
                        androidETC2FallbackMenu.ShowAsContext();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
#endif
            }

            //iOS
            /*EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("iOS", EditorStyles.boldLabel);

            //Override For iOS
            DisplayBoolInfo("Override For iOS", ref overrideForiOS, 30);
            changeOverrideForiOS = overrideForiOS;
            EditorGUILayout.EndHorizontal();*/

            DisplayHeaderBoolInfo("iOS", ref overrideForiOS, ref changeOverrideForiOS);

            if (overrideForiOS)
            {
                //iOS MaxTextureSize
                DisplayIntInfo("MaxTextureSize", ref iOSMaxTextureSize, 30, ref changeiOSMaxTextureSize);


    #if UNITY_2017_1_OR_NEWER
                //iOS Texture Resize Algorithm
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    changeiOSTextureResizeAlgorithm = EditorGUILayout.Toggle(changeiOSTextureResizeAlgorithm, GUILayout.Width(10));
                    EditorGUI.BeginDisabledGroup(changeiOSTextureResizeAlgorithm == false);

                    GUILayout.Label("Resize Algorithm");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(iOSTextureResizeAlgorithm.ToString(), GUILayout.Width(200)))
                    {
                        GenericMenu iOSResizeAlgorithmMenu = new GenericMenu();
                        iOSResizeAlgorithmMenu.AddItem(new GUIContent("Mitchell"), iOSTextureResizeAlgorithm == TextureResizeAlgorithm.Mitchell,
                                                    () => { iOSTextureResizeAlgorithm = TextureResizeAlgorithm.Mitchell; });
                        iOSResizeAlgorithmMenu.AddItem(new GUIContent("Bilinear"), iOSTextureResizeAlgorithm == TextureResizeAlgorithm.Bilinear,
                                                    () => { iOSTextureResizeAlgorithm = TextureResizeAlgorithm.Bilinear; });
                        iOSResizeAlgorithmMenu.ShowAsContext();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();
    #endif

                //iOS Texture Compression
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    changeiOSTextureImporterCompression = EditorGUILayout.Toggle(changeiOSTextureImporterCompression, GUILayout.Width(10));
                    EditorGUI.BeginDisabledGroup(changeiOSTextureImporterCompression == false);

                    GUILayout.Label("Compression");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(RawName2DisplayName(iOSTextureImporterCompression.ToString()), GUILayout.Width(200)))
                    {
                        GenericMenu iOSCompressionMenu = new GenericMenu();
                        iOSCompressionMenu.AddItem(new GUIContent("Normal Quality"), iOSTextureImporterCompression == TextureImporterCompression.Compressed,
                                                () => { iOSTextureImporterCompression = TextureImporterCompression.Compressed; });
                        iOSCompressionMenu.AddItem(new GUIContent("High Quality"), iOSTextureImporterCompression == TextureImporterCompression.CompressedHQ,
                                                () => { iOSTextureImporterCompression = TextureImporterCompression.CompressedHQ; });
                        iOSCompressionMenu.AddItem(new GUIContent("Low Quality"), iOSTextureImporterCompression == TextureImporterCompression.CompressedLQ,
                                                () => { iOSTextureImporterCompression = TextureImporterCompression.CompressedLQ; });
                        iOSCompressionMenu.AddItem(new GUIContent("None"), iOSTextureImporterCompression == TextureImporterCompression.Uncompressed,
                                                () => { iOSTextureImporterCompression = TextureImporterCompression.Uncompressed; });
                        iOSCompressionMenu.ShowAsContext();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                //iOS Texture Format
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    changeiOSTextureImporterFormat = EditorGUILayout.Toggle(changeiOSTextureImporterFormat, GUILayout.Width(10));
                    EditorGUI.BeginDisabledGroup(changeiOSTextureImporterFormat == false);

                    GUILayout.Label("Format");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(m_iOSTexFormatDictionary[(int)iOSTextureImporterFormat], GUILayout.Width(200)))
                    {
                        GenericMenu iOSTextureFormatMenu = new GenericMenu();
                        foreach (var item in m_iOSTexFormatDictionary)
                        {
                            iOSTextureFormatMenu.AddItem(new GUIContent(item.Value), iOSTextureImporterFormat == (TextureImporterFormat)item.Key,
                                                        () => { iOSTextureImporterFormat = (TextureImporterFormat)item.Key; });
                        }
                        iOSTextureFormatMenu.ShowAsContext();
                    }

                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndHorizontal();

                //iOS Texture Compression Quality
                DisplayIntInfo("Compression Quality", ref iOSTextureCompressionQuality, 30, ref changeiOSCompressionQuality);
            }
        }

        #endregion

        #region Advanced
        //Advanced
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced");
        if (showAdvanced)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            EditorGUILayout.BeginVertical();

            //图片形状
            EditorGUILayout.BeginHorizontal();
            {
                changeTextureImporterShape = EditorGUILayout.Toggle(changeTextureImporterShape, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeTextureImporterShape == false);

                GUILayout.Label("Texture Shape");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(RawName2DisplayName(textureImporterShape.ToString()), GUILayout.Width(200)))
                {
                    GenericMenu textureShapeMenu = new GenericMenu();
                    textureShapeMenu.AddItem(new GUIContent("2D"), textureImporterShape == TextureImporterShape.Texture2D,
                                            () => { textureImporterShape = TextureImporterShape.Texture2D; });
                    textureShapeMenu.AddItem(new GUIContent("Cube"), textureImporterShape == TextureImporterShape.TextureCube,
                                            () => { textureImporterShape = TextureImporterShape.TextureCube; });
                    textureShapeMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            //sRGB
            DisplayBoolInfo("sRGB(Color Texture)", ref sRGBTexture, 4, ref changesRGBTexture);

            //Alpha Source
            EditorGUILayout.BeginHorizontal();
            {
                changeTextureImporterAlphaSource = EditorGUILayout.Toggle(changeTextureImporterAlphaSource, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeTextureImporterAlphaSource == false);

                GUILayout.Label("Alpha Source");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(RawName2DisplayName(textureImporterAlphaSource.ToString()), GUILayout.Width(200)))
                {
                    GenericMenu alphaSourceMenu = new GenericMenu();
                    alphaSourceMenu.AddItem(new GUIContent("None"), textureImporterAlphaSource == TextureImporterAlphaSource.None,
                                            () => { textureImporterAlphaSource = TextureImporterAlphaSource.None; });
                    alphaSourceMenu.AddItem(new GUIContent("Input Texture Alpha"), textureImporterAlphaSource == TextureImporterAlphaSource.FromInput,
                                            () => { textureImporterAlphaSource = TextureImporterAlphaSource.FromInput; });
                    alphaSourceMenu.AddItem(new GUIContent("From Gray Scale"), textureImporterAlphaSource == TextureImporterAlphaSource.FromGrayScale,
                                            () => { textureImporterAlphaSource = TextureImporterAlphaSource.FromGrayScale; });
                    alphaSourceMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            //AlphaIsTransparency
            DisplayBoolInfo("Alpha Is Transparency", ref AlphaIsTransparency, 4, ref changeAlphaIsTransparency);


            EditorGUILayout.LabelField("Normal", EditorStyles.boldLabel);
            //CreateFromScale
            DisplayBoolInfo("Create from Grayscale", ref CreateFromGrayscale, 20, ref changeCreateFromGrayscale);

            //Bumpiness
            DisplayFloatInfo("Normal Bumpiness", ref Bumpiness, 20, ref changeBumpiness);


            //Normal Filter
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeTextureImporterNormalFilter = EditorGUILayout.Toggle(changeTextureImporterNormalFilter, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeTextureImporterNormalFilter == false);

                GUILayout.Label("Normal Filtering");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(RawName2DisplayName(textureImporterNormalFilter.ToString()), GUILayout.Width(200)))
                {
                    GenericMenu normalFilterMenu = new GenericMenu();
                    normalFilterMenu.AddItem(new GUIContent("Sharp"), textureImporterNormalFilter == TextureImporterNormalFilter.Standard,
                                            () => { textureImporterNormalFilter = TextureImporterNormalFilter.Standard; });
                    normalFilterMenu.AddItem(new GUIContent("Smooth"), textureImporterNormalFilter == TextureImporterNormalFilter.Sobel,
                                            () => { textureImporterNormalFilter = TextureImporterNormalFilter.Sobel; });
                    normalFilterMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Sprite", EditorStyles.boldLabel);
            //Sprite Import Mode
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeSpriteImportMode = EditorGUILayout.Toggle(changeSpriteImportMode, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeSpriteImportMode == false);

                GUILayout.Label("Sprite Mode");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(spriteImportMode.ToString(), GUILayout.Width(200)))
                {
                    GenericMenu spriteImportModeMenu = new GenericMenu();
                    spriteImportModeMenu.AddItem(new GUIContent("Single"), spriteImportMode == SpriteImportMode.Single,
                                                () => { spriteImportMode = SpriteImportMode.Single; });
                    spriteImportModeMenu.AddItem(new GUIContent("Multiple"), spriteImportMode == SpriteImportMode.Multiple,
                                                () => { spriteImportMode = SpriteImportMode.Multiple; });
                    spriteImportModeMenu.AddItem(new GUIContent("Polygon"), spriteImportMode == SpriteImportMode.Polygon,
                                                () => { spriteImportMode = SpriteImportMode.Polygon; });
                    spriteImportModeMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
            
            //Sprite Packing Tag
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeSpritePackingTag = EditorGUILayout.Toggle(changeSpritePackingTag, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeSpritePackingTag == false);

                GUILayout.Label("Packing Tag");
                GUILayout.FlexibleSpace();
                spritePackingTag = GUILayout.TextField(spritePackingTag, GUILayout.Width(200));

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            //Pixels Per Unit
            DisplayIntInfo("Pixels Per Unit", ref pixelsPerUnit, 20, ref changePixelsPerUnit);

            //Sprite Mesh Type
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeSpriteMeshType = EditorGUILayout.Toggle(changeSpriteMeshType, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeSpriteMeshType == false);

                GUILayout.Label("Mesh Type");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(spriteMeshType.ToString(), GUILayout.Width(200)))
                {
                    GenericMenu spriteMeshTypeMenu = new GenericMenu();
                    spriteMeshTypeMenu.AddItem(new GUIContent("Tight"), spriteMeshType == SpriteMeshType.Tight,
                                                () => { spriteMeshType = SpriteMeshType.Tight; });
                    spriteMeshTypeMenu.AddItem(new GUIContent("Full Rect"), spriteMeshType == SpriteMeshType.FullRect,
                                () => { spriteMeshType = SpriteMeshType.FullRect; });
                    spriteMeshTypeMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            //ExtrudeEdges
            DisplayIntInfo("Extrude Edges", ref ExtrudeEdges, 20, ref changeExtrudeEdges);

            //Generate Physics Shape
            DisplayBoolInfo("Generate Physics Shape", ref generatePhysicsShape, 20, ref changeGeneratePhysicsShape);

#if UNITY_2017_1_OR_NEWER
            EditorGUILayout.LabelField("Single Channel", EditorStyles.boldLabel);
            //Single Channel
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeSingleChannel = EditorGUILayout.Toggle(changeSingleChannel, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeSingleChannel == false);

                GUILayout.Label("Channel");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(singleChannel.ToString(), GUILayout.Width(200)))
                {
                    GenericMenu singleChannelMenu = new GenericMenu();
                    singleChannelMenu.AddItem(new GUIContent("Alpha"), singleChannel == TextureImporterSingleChannelComponent.Alpha,
                                                () => { singleChannel = TextureImporterSingleChannelComponent.Alpha; });
                    singleChannelMenu.AddItem(new GUIContent("Red"), singleChannel == TextureImporterSingleChannelComponent.Red,
                                                () => { singleChannel = TextureImporterSingleChannelComponent.Red; });
                    singleChannelMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
#endif

            EditorGUILayout.LabelField("Cubemap", EditorStyles.boldLabel);
            //CubemapGenerateType
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeCubemapGenerateType = EditorGUILayout.Toggle(changeCubemapGenerateType, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeCubemapGenerateType == false);

                GUILayout.Label("Mapping");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(RawName2DisplayName(cubeMapGenerateType.ToString()), GUILayout.Width(200)))
                {
                    GenericMenu cubeMapGenerateTypeMenu = new GenericMenu();
                    cubeMapGenerateTypeMenu.AddItem(new GUIContent("Auto"), cubeMapGenerateType == TextureImporterGenerateCubemap.AutoCubemap,
                                                    () => { cubeMapGenerateType = TextureImporterGenerateCubemap.AutoCubemap; });
                    cubeMapGenerateTypeMenu.AddItem(new GUIContent("Mirrored Ball(Spheremap)"), cubeMapGenerateType == TextureImporterGenerateCubemap.Spheremap,
                                                () => { cubeMapGenerateType = TextureImporterGenerateCubemap.Spheremap; });
                    cubeMapGenerateTypeMenu.AddItem(new GUIContent("Latitude-Longitude Layout(Cylindrical)"), cubeMapGenerateType == TextureImporterGenerateCubemap.Cylindrical,
                                                () => { cubeMapGenerateType = TextureImporterGenerateCubemap.Cylindrical; });
                    cubeMapGenerateTypeMenu.AddItem(new GUIContent("6 Frames Layout(Cubic Environment)"), cubeMapGenerateType == TextureImporterGenerateCubemap.FullCubemap,
                                                () => { cubeMapGenerateType = TextureImporterGenerateCubemap.FullCubemap; });
                    cubeMapGenerateTypeMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeCubemapConvolutionType = EditorGUILayout.Toggle(changeCubemapConvolutionType, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeCubemapConvolutionType == false);

                GUILayout.Label("Convolution Type");
                GUILayout.FlexibleSpace();
                if(GUILayout.Button(RawName2DisplayName(cubeMapConvolutionType.ToString()),GUILayout.Width(200)))
                {
                    GenericMenu cubeMapConvolutionTypeMenu = new GenericMenu();
                    cubeMapConvolutionTypeMenu.AddItem(new GUIContent("None"), cubeMapConvolutionType == TextureImporterCubemapConvolution.None,
                                                        () => { cubeMapConvolutionType = TextureImporterCubemapConvolution.None; });
                    cubeMapConvolutionTypeMenu.AddItem(new GUIContent("Specular(Glossy Reflection)"), cubeMapConvolutionType == TextureImporterCubemapConvolution.Specular,
                                            () => { cubeMapConvolutionType = TextureImporterCubemapConvolution.Specular; });
                    cubeMapConvolutionTypeMenu.AddItem(new GUIContent("Diffuse(Irradiance)"), cubeMapConvolutionType == TextureImporterCubemapConvolution.Diffuse,
                                            () => { cubeMapConvolutionType = TextureImporterCubemapConvolution.Diffuse; });

                    cubeMapConvolutionTypeMenu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            //Fix Up Edge Seams
            DisplayBoolInfo("Fixup Edge Seams", ref fixupEdgeSeams, 20, ref changeFixupEdgeSeams);

            EditorGUILayout.LabelField("Other", EditorStyles.boldLabel);
            //Texture Importer Non Power of 2
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                changeTextureImporterNPOTScale = EditorGUILayout.Toggle(changeTextureImporterNPOTScale, GUILayout.Width(10));
                EditorGUI.BeginDisabledGroup(changeTextureImporterNPOTScale == false);

                GUILayout.Label("Non Power of 2");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(textureImporterNPOTScale.ToString(), GUILayout.Width(200)))
                {
                    GenericMenu nonPowerof2Menu = new GenericMenu();
                    nonPowerof2Menu.AddItem(new GUIContent("None"), textureImporterNPOTScale == TextureImporterNPOTScale.None,
                                            () => { textureImporterNPOTScale = TextureImporterNPOTScale.None; });
                    nonPowerof2Menu.AddItem(new GUIContent("ToLarger"), textureImporterNPOTScale == TextureImporterNPOTScale.ToLarger,
                                            () => { textureImporterNPOTScale = TextureImporterNPOTScale.ToLarger; });
                    nonPowerof2Menu.AddItem(new GUIContent("ToNearest"), textureImporterNPOTScale == TextureImporterNPOTScale.ToNearest,
                                            () => { textureImporterNPOTScale = TextureImporterNPOTScale.ToNearest; });
                    nonPowerof2Menu.AddItem(new GUIContent("ToSmaller"), textureImporterNPOTScale == TextureImporterNPOTScale.ToSmaller,
                                            () => { textureImporterNPOTScale = TextureImporterNPOTScale.ToSmaller; });
                    nonPowerof2Menu.ShowAsContext();
                }

                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            //Read Write Enabled
            DisplayBoolInfo("Read/Write Enabled", ref ReadWriteEnabled, 20, ref changeReadWriteEnabled);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
#endregion
    }


    public override bool LoadFromXML(XmlNode node)
    {
        System.Type thisType = this.GetType();
        FieldInfo[] fields = thisType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        Dictionary<string, FieldInfo> fieldInfoDic = new Dictionary<string, FieldInfo>();
        foreach (var field in fields)
        {
            if (IsBaseField(field.Name))
                continue;

            fieldInfoDic.Add(field.Name, field);
        }


        foreach (XmlNode assetNode in node.ChildNodes)
        {
            string fieldName = (assetNode as XmlElement).GetAttribute("key");
            string value = (assetNode as XmlElement).GetAttribute("value");

            FieldInfo field;
            if(fieldInfoDic.TryGetValue(fieldName, out field))
            {
                //field.SetValue(this, );
                Type fieldType = field.FieldType;
                if (fieldType.IsEnum)
                {
                   // string typeName = (assetNode as XmlElement).GetAttribute("type");
                    field.SetValue(this, Enum.Parse(fieldType, value));
                }
                else if(fieldType == typeof(int) || fieldType == typeof(uint))
                {
                    field.SetValue(this, int.Parse(value));
                }
                else if (fieldType == typeof(string))
                {
                    field.SetValue(this, value);
                }
                else if (fieldType == typeof(bool))
                {
                    field.SetValue(this, bool.Parse(value));
                }
                else if (fieldType == typeof(float))
                {
                    field.SetValue(this, float.Parse(value));
                }
                else
                {
                    Debug.LogErrorFormat("Unknown Filed in TextureProcessorData, {0}, {1}", fieldType.Name, fieldName);
                }
            }
        }

        base.LoadFromXML(node);

        return true;
    }

    public override void SaveToXML(XmlDocument xml, XmlNode node)
    {
        base.SaveToXML(xml, node);

        System.Type thisType = this.GetType();
        FieldInfo[] fields = thisType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach(var field in fields)
        {
            if (IsBaseField(field.Name))
                continue;

            XmlElement assetNode = xml.CreateElement("property");

            assetNode.SetAttribute("key", field.Name);
            assetNode.SetAttribute("value", field.GetValue(this).ToString());

            if(field.GetType().IsEnum)
            {
               // assetNode.SetAttribute("type", field.GetType().Name);
            }

            node.AppendChild(assetNode);
        }      
    }

    //获取导入图片的宽高
    (int, int) GetTextureImporterSize(TextureImporter importer)
    {
        if (importer != null)
        {
            object[] args = new object[2];
            MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(importer, args);
            return ((int)args[0], (int)args[1]);
        }
        return (0, 0);
    }

    //贴图不存在、meta文件不存在、图片尺寸发生修改需要重新导入
    bool IsFirstImport(TextureImporter importer, string assetPath)
    {
        (int width, int height) = GetTextureImporterSize(importer);
        Texture tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        var metaFilePath = AssetDatabase.GetAssetPathFromTextMetaFilePath(assetPath) + ".meta";
    //    Debug.LogError(metaFilePath);
        bool hasMeta = File.Exists(metaFilePath);
        return tex == null || !hasMeta || (tex.width != width && tex.height != height);
    }

    static bool showTextureSizeWarning = true;

    public override void OnImportAsset(AssetImporter assetImporter, string assetPath)
    {
        TextureImporter importer = assetImporter as TextureImporter;
        if (importer != null)
        {
            // if (!IsFirstImport(importer, assetPath))
            //     return;

            //Texture Importer Settings
            TextureImporterSettings setting = new TextureImporterSettings();
            importer.ReadTextureSettings(setting);
            if (this.changeTextureImporterType)
            {
                setting.textureType = this.textureImporterType;

                //如果是normal的话特殊处理下
                if (setting.textureType == TextureImporterType.Default && 
                    importer.assetPath.Contains("Actor") && 
                    (importer.assetPath.ToLower().EndsWith("_no.tga") || importer.assetPath.ToLower().EndsWith("_n.tga")))
                {
                    setting.textureType = TextureImporterType.NormalMap;
                }
            }
                

            if (this.changeTextureImporterShape)
                setting.textureShape = this.textureImporterShape;

            if (this.changesRGBTexture)
                setting.sRGBTexture = this.sRGBTexture;

            if (this.changeTextureImporterAlphaSource)
                setting.alphaSource = this.textureImporterAlphaSource;

            if (this.changeAlphaIsTransparency)
                setting.alphaIsTransparency = this.AlphaIsTransparency;

            if (this.changeBumpiness)
                setting.heightmapScale = this.Bumpiness;

            if (this.changePixelsPerUnit)
                setting.spritePixelsPerUnit = this.pixelsPerUnit;

            if (this.changeSpriteMeshType)
                setting.spriteMeshType = this.spriteMeshType;

            if (this.changeExtrudeEdges)
                setting.spriteExtrude = (uint)this.ExtrudeEdges;
#if UNITY_2017_1_OR_NEWER
            if (this.changeGeneratePhysicsShape)
                setting.spriteGenerateFallbackPhysicsShape = this.generatePhysicsShape;

            if (this.changeSingleChannel)
                setting.singleChannelComponent = this.singleChannel;
#endif
            if (this.changeCubemapGenerateType)
                setting.generateCubemap = this.cubeMapGenerateType;

            if (this.changeCubemapConvolutionType)
                setting.cubemapConvolution = this.cubeMapConvolutionType;

            if (this.changeFixupEdgeSeams)
                setting.seamlessCubemap = this.fixupEdgeSeams;

            if (this.changeTextureImporterNPOTScale)
                setting.npotScale = this.textureImporterNPOTScale;
            //read write not found
            if (this.changeReadWriteEnabled)
                setting.readable = this.ReadWriteEnabled;

            if (this.changeGenerateMipMaps)
                setting.mipmapEnabled = this.GenerateMipMaps;

            if (this.changeBorderMipMaps)
                setting.borderMipmap = this.BorderMipMaps;

            if (this.changeTextureImporterMipFilter)
                setting.mipmapFilter = this.textureImporterMipFilter;
#if UNITY_2017_1_OR_NEWER
            if(this.changeMipMapsPreserveCoverage)
                setting.mipMapsPreserveCoverage = this.MipMapsPreserveCoverage;
#endif
            if (this.changeFadeoutMipMaps)
                setting.fadeOut = this.FadeoutMipMaps;

            if (this.changeFadeOutMipMapStart)
                setting.mipmapFadeDistanceStart = this.FadeOutMipMapStart;

            if (this.changeFadeOutMipMapEnd)
                setting.mipmapFadeDistanceEnd = this.FadeOutMipMapEnd;

            if (this.changeWrapMode)
                setting.wrapMode = this.wrapMode;

            if (this.changeFilterMode)
                setting.filterMode = this.filterMode;

            if (this.changeAnisoLevel)
                setting.aniso = this.anisoLevel;

            importer.SetTextureSettings(setting);

            //Importer
            if (this.changeCreateFromGrayscale)
                importer.convertToNormalmap = this.CreateFromGrayscale;

            if (this.changeTextureImporterNormalFilter)
                importer.normalmapFilter = this.textureImporterNormalFilter;

            if (this.changeSpriteImportMode)
                importer.spriteImportMode = this.spriteImportMode;

            if (this.changeSpritePackingTag)
                importer.spritePackingTag = this.spritePackingTag;

            if (this.changeDefaultMaxTextureSize)
                importer.maxTextureSize = this.defaultMaxTextureSize;
            //importer.resizealgorithm not found

            if (this.changeDefaultTextureImporterCompression)
                importer.textureCompression = this.defaultTextureImporterCompression;

            if (this.changeDefaultCrunchCompression)
                importer.crunchedCompression = this.defaultCrunchCompression;

            if (this.changeDefaultCompressionQuality)
                importer.compressionQuality = this.defaultCompressionQuality;

            if (this.changeOverrideForAndroid)
            {
                if (overrideForAndoird)
                {
                    // android iOS not override
                    var androidTextureSetting = importer.GetPlatformTextureSettings("Android");
                    if (androidTextureSetting == null)
                    {
                        androidTextureSetting = new TextureImporterPlatformSettings();
                        androidTextureSetting.name = "Android";
                    }

                    if (this.changeOverrideForAndroid)
                        androidTextureSetting.overridden = this.overrideForAndoird;

                    if (this.changeAndroidMaxTextureSize)
                        androidTextureSetting.maxTextureSize = this.androidMaxTextureSize;

#if UNITY_2017_1_OR_NEWER
                    if (this.changeAndroidTextureResizeAlgorithm)
                        androidTextureSetting.resizeAlgorithm = this.androidTextureResizeAlgorithm;
#endif

                    if (this.changeAndroidTextureImporterCompression)
                        androidTextureSetting.textureCompression = this.androidTextureImporterCompression;

                    if (this.changeAndroidTextureImporterFormat)
                        androidTextureSetting.format = this.androidTextureImporterFormat;

                    if (this.changeAndroidCompressionQuality)
                        androidTextureSetting.compressionQuality = this.androidTextureCompressionQuality;

                    if (this.changeAndroidUsingETC1)
                        androidTextureSetting.allowsAlphaSplitting = this.androidUsingETC1;

#if UNITY_2017_1_OR_NEWER
                    if (this.changeAndroidETC2FallbakcOverride)
                        androidTextureSetting.androidETC2FallbackOverride = this.androidETC2FallbackOverride;
#endif

                    importer.SetPlatformTextureSettings(androidTextureSetting);
                }
                else // 不允许Override
                {
                    importer.ClearPlatformTextureSettings("Android");
                }
            }

            if (this.changeOverrideForiOS)
            {
                if (overrideForiOS)
                {
                    var iPhoneTextureSetting = importer.GetPlatformTextureSettings("iPhone");
                    if (iPhoneTextureSetting == null)
                    {
                        iPhoneTextureSetting = new TextureImporterPlatformSettings();
                        iPhoneTextureSetting.name = "iPhone";
                    }

                    if (this.changeOverrideForiOS)
                        iPhoneTextureSetting.overridden = this.overrideForiOS;

                    if (this.changeiOSMaxTextureSize)
                        iPhoneTextureSetting.maxTextureSize = this.iOSMaxTextureSize;

#if UNITY_2017_1_OR_NEWER
                    if (this.changeiOSTextureResizeAlgorithm)
                        iPhoneTextureSetting.resizeAlgorithm = this.iOSTextureResizeAlgorithm;
#endif

                    if (this.changeiOSTextureImporterCompression)
                        iPhoneTextureSetting.textureCompression = this.iOSTextureImporterCompression;

                    if (this.changeiOSTextureImporterFormat)
                        iPhoneTextureSetting.format = this.iOSTextureImporterFormat;

                    if (this.changeiOSCompressionQuality)
                        iPhoneTextureSetting.compressionQuality = this.iOSTextureCompressionQuality;

                    importer.SetPlatformTextureSettings(iPhoneTextureSetting);
                }
                else
                {
                    importer.ClearPlatformTextureSettings("iPhone");
                }
            }

            
            if (assetPath.Contains("NewBackground"))
            {
                (int width, int height) = GetTextureImporterSize(importer);
                if (width % 4 != 0 || height % 4 != 0)
                {
                    var newWidth1 = width%4==0? width: ((int)(width/4)+1) *4;
                    var newHeight1 = height%4==0?height:((int)(height/4)+1) *4;

                    var newWidth2 = width%4==0? width: ((int)(width/4)) *4;
                    var newHeight2 = height%4==0?height:((int)(height/4)) *4;

                    var str = string.Format("new1({0},{1}) new2({2},{3})", newWidth1, newHeight1, newWidth2,newHeight2);

                    var newWidth = Mathf.Abs(newWidth1-width)<=Mathf.Abs(newWidth2-width)? newWidth1 : newWidth2;
                    var newHeight = Mathf.Abs(newHeight1-height) <= Mathf.Abs(newHeight2-height)?newHeight1: newHeight2;

                    var errorStr = string.Format("{0}({1},{2})的图片长度没有满足被4整除的条件，请修改成({3},{4})！", assetPath, width, height, newWidth, newHeight);

                    if (showTextureSizeWarning)
                    {
                        bool ret = EditorUtility.DisplayDialog("[图片长度导入检查]", errorStr, "好的", "不再提示");
                        if (!ret)
                        {
                            showTextureSizeWarning = false;
                        }
                    }

                    Debug.LogErrorFormat("[图片长度导入检查]:{0}", errorStr);
                }
            }
             
        }
    }
}
