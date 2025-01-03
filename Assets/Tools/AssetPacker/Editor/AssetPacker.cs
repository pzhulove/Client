using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;

using XUPorterJSON;
using System.Collections;

using AssetBundleTool;

public class AssetPacker : EditorWindow
{
    public enum EAssetType
    {
        Actor       = 0,
        Shader      = 1,
        Data        = 2,
        Scene       = 3,
        Effect      = 4,
        UI          = 5,
        Sound       = 6,
        All         = 7,

        Mask        = (1 << (int)All) - 1,
    }

    public delegate void PackPolicy_Delegate(EAssetType assetType, string[] assetPath,string[] exclusivePath,int packLv, List<AssetPackageDesc> descList);
    static protected string m_OutputBundlePath = "Assets/StreamingAssets/AssetBundles/";
    static protected string m_StreamingBundlePath = "AssetBundles/";
    static protected string m_TempAssetPath = "Assets/PackAssetTemp/";
    static protected string m_OutputBundleExt = ".pck";

    protected class AssetCheckDesc
    {
        public string m_PackageName = null;
        public List<string> m_SameAssetList = new List<string>();
    }

    static protected List<Dictionary<string, AssetCheckDesc>> m_SameAssetTableList = new List<Dictionary<string, AssetCheckDesc>>();
    public class AssetPackCommand
    {
        public AssetPackCommand(EAssetType eAssetType, string[] assetPath,string[] excluderPath, PackPolicy_Delegate packPolicy,int packLv)
        {
            m_AssetPath = assetPath;
            m_ExcluderPath = excluderPath;
            m_PackPolicy = packPolicy;
            m_PackLevel = packLv;
            m_AssetType = eAssetType;
        }

        public EAssetType m_AssetType;
        public string[] m_AssetPath;
        public string[] m_ExcluderPath;
        public PackPolicy_Delegate m_PackPolicy;
        public int m_PackLevel;
    }

    static public AssetPackCommand[] m_PackCommands = new AssetPackCommand[]
    {
        //临时先增加
        new AssetPackCommand(EAssetType.Actor   ,new string[] { "Assets/Resources/_NEW_RESOURCES/Actor"}, null                                             , PackPolicyActor           , 0),
        new AssetPackCommand(EAssetType.Actor   ,new string[] { "Assets/Resources/_NEW_RESOURCES/Actor"}, null                                             , PackPolicyActorAnimClipEx , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/_NEW_RESOURCES/Data" }, null                                             , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/_NEW_RESOURCES/Effects"},null                                            , PackPolicyEffectEx        , 1),
        //
        
        new AssetPackCommand(EAssetType.Actor   ,new string[] { "Assets/Resources/Actor"}                                 ,new string[] { "Assets/Resources/Actor/Weapon" }                                                                                                                             , PackPolicyActor           , 2),
        new AssetPackCommand(EAssetType.Actor   ,new string[] { "Assets/Resources/Actor"}                                 ,new string[] { "Assets/Resources/Actor/Weapon" }                                                                                                                             , PackPolicyActorAnimClipEx , 2),
        new AssetPackCommand(EAssetType.Actor   ,new string[] { "Assets/Resources/Actor/Weapon" }                         ,null                                                                                                                                                                         , PackPolicyActorWeapon     , 2),
        new AssetPackCommand(EAssetType.Shader  ,new string[] { "Assets/Resources/Shader", "Assets/Resources/Animat"}     ,null                                                                                                                                                                         , PackPolicyShader          , 0),
        
        new AssetPackCommand(EAssetType.Shader  ,new string[] { "Assets/Resources/Environment"}                           ,null                                                                                                                                                                         , PackPolicyEnvironment     , 0),
        
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/AI" }                              ,new string[] { "Assets/Resources/Data/AI/Guanka_apc", "Assets/Resources/Data/AI/Monster_AI" }                                                                                , PackPolicyData            , 1),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/AI/Guanka_apc" }                   ,null                                                                                                                                                                         , PackPolicyData            , 1),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/AI/Monster_AI" }                   ,null                                                                                                                                                                         , PackPolicyData            , 1),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/CommonData" }                      ,null                                                                                                                                                                         , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/NewbieGuide" }                     ,null                                                                                                                                                                         , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/ChapterData" }                     ,null                                                                                                                                                                         , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/DungeonData" }                     ,null                                                                                                                                                                         , PackPolicyData            , 1),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/DungeonMapData" }                  ,null                                                                                                                                                                         , PackPolicyData            , 0),
        //new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/InputData" }                       ,null                                                                                                                                                                         , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SceneData" }                       ,null                                                                                                                                                                         , PackPolicyData            , 1),
        //new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SkillData" }                       ,new string[] {"Assets/Resources/Data/SkillData/Monster"}                                                                                                                     , PackPolicyData            , 1),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/table" }                           ,null                                                                                                                                                                         , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/table_fb" }                           ,null                                                                                                                                                                      , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/Language" }                        ,null                                                                                                                                                                         , PackPolicyData            , 0),
        //new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SkillData/Monster" }               ,null                                                                                                                                                                         , PackPolicyData            , 1),
		new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/Lua" }                             ,null                                                                                                                                                                         , PackPolicyData            , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SkillData" }                       ,null                                                                                                                                                                         , PackPolicyDataInOne       , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SkillData_fb" }                       ,null                                                                                                                                                                         , PackPolicyDataInOne       , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/ExTransportData" }                 ,null                                                                                                                                                                         , PackPolicyDataInOne       , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/ScriptData" }                 ,null                                                                                                                                                                         , PackPolicyDataInOne       , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/ItemSourceInfoTable" }                 ,null                                                                                                                                                                         , PackPolicyDataInOne       , 0),
        new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/ComboData" }                 ,null                                                                                                                                                                         , PackPolicyDataInOne       , 0),
        
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Banding" }      ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Blood" }        ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Clamp" }        ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Element" }      ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/fire" }         ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Floor" }        ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Ice" }          ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Light" }        ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Lightning" }    ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Mesh_textures"} ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Other" }        ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Serial" }       ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Sky" }          ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Smoke" }        ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Spots" }        ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Texture" }      ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects/Common/Textures/Ui" }           ,null                                                                                                                                                                         , PackPolicyEffectCommonTexture, 0),
        // new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects"}                               ,new string[] { "Assets/Resources/Effects/Common/Textures" }                                                                                                                  , PackPolicyEffect      , 0),
        new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects"}                               ,null                                                                                                                                                                         , PackPolicyEffectEx    , 1),

        new AssetPackCommand(EAssetType.Scene   ,new string[] { "Assets/Resources/Scene" }                                ,new string[] { "Assets/Resources/Scene/Town" }                                                                                                                               , PackPolicyScene       , 1),
        new AssetPackCommand(EAssetType.Scene   ,new string[] { "Assets/Resources/Scene/Town" }                           ,null                                                                                                                                                                         , PackPolicyScene       , 1),
        
        // new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI"}                                    ,new string[] { "Assets/Resources/UI/Animation/Anim_chengHao_Formal", "Assets/Resources/UI/Font" }                                                                            , PackPolicyUI          , 0),
        // new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Animation/Anim_chengHao_Formal" }    ,null                                                                                                                                                                         , PackPolicyUITitle     , 1),
        // new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Font" }                              ,null                                                                                                                                                                         , PackPolicyUIFont      , 0),
        // new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIPacked"}                              ,null                                                                                                                                                                         , PackPolicyPackedUI    , 0),
        // new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten" }                            ,null                                                                                                                                                                         , PackPolicyFlattenUI   , 0),
        
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI"}                                    ,new string[] { "Assets/Resources/UI/Animation/Anim_chengHao_Formal", "Assets/Resources/UI/Font" }                                                                                                        , PackPolicyUI              , 2),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Font" }                              ,null                                                                                                                                                                         , PackPolicyUIFont          , 0),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Animation/Anim_chengHao_Formal" }    ,null                                                                                                                                                                         , PackPolicyUITitle         , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI"}                                    ,new string[] { "Assets/Resources/UI/Animation/Anim_chengHao_Formal", "Assets/Resources/UI/Image", "Assets/Resources/UI/Font" }                                                                           , PackPolicyUIImage         , 2),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/Background" }                  ,null                                                                                                                                                                         , PackPolicyUIPackedImage   , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/NewBackground" }               ,null                                                                                                                                                                         , PackPolicyUIPackedImage   , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/Emotion" }                     ,null                                                                                                                                                                         , PackPolicyUIImage         , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/Packed" }                      ,null                                                                                                                                                                         , PackPolicyUIPackedImage   , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/NewPacked" }                   ,null                                                                                                                                                                         , PackPolicyUIPackedImage   , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/NewIcon" }                     ,null                                                                                                                                                                         , PackPolicyUIImage   , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/Icon" }                        ,null                                                                                                                                                                         , PackPolicyUIImage         , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/Portrait" }                    ,null                                                                                                                                                                         , PackPolicyUIImage         , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI/Image/ChenghaoDefault" }             ,null                                                                                                                                                                         , PackPolicyUIImage         , 1),

        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIPacked"}                              ,null                                                                                                                                                                         , PackPolicyUIPackedImage   , 1),

        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten"}                             ,new string[] { "Assets/Resources/UIFlatten/Font", "Assets/Resources/UIFlatten/Animation" }                                                                                   , PackPolicyUI              , 2),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten/Animation"}                   ,null                                                                                                                                                                         , PackPolicyUI              , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten/Font" }                       ,null                                                                                                                                                                         , PackPolicyUIFont          , 0),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten"}                             ,new string[] { "Assets/Resources/UIFlatten/Image", "Assets/Resources/UIFlatten/Font" }                                                                                       , PackPolicyUIImage         , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten/Image/Background" }           ,null                                                                                                                                                                         , PackPolicyUIPackedImage   , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten/Image/Comic" }                ,null                                                                                                                                                                         , PackPolicyUIImage         , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten/Image/Packed" }               ,null                                                                                                                                                                         , PackPolicyUIPackedImage   , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten/Image/Icon" }                 ,null                                                                                                                                                                         , PackPolicyUIImage         , 1),
        new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UIFlatten/Image/Portrait" }             ,null                                                                                                                                                                         , PackPolicyUIImage         , 1),


        new AssetPackCommand(EAssetType.Sound   ,new string[] { "Assets/Resources/Sound"}                                 ,new string[] { "Assets/Resources/Sound/Dungeon" }                                                                                                                            , PackPolicySound       , 1),
        new AssetPackCommand(EAssetType.Sound   ,new string[] { "Assets/Resources/Sound/Dungeon" }                        ,null                                                                                                                                                                         , PackPolicySoundSingle , 0),

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// new AssetPackCommand(EAssetType.Actor   ,new string[] { "Assets/Resources/Actor"}                                                               ,null, PackPolicyAllInOne, 0),
        /// new AssetPackCommand(EAssetType.Shader  ,new string[] { "Assets/Resources/Shader", "Assets/Resources/Animat", "Assets/Resources/Environment" }  ,null, PackPolicyAllInOne, 0),
        /// new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data" }                                                               ,null, PackPolicyAllInOneData, 0),
        /// new AssetPackCommand(EAssetType.Effect  ,new string[] { "Assets/Resources/Effects" }                                                            ,null, PackPolicyAllInOne, 0),
        /// new AssetPackCommand(EAssetType.Scene   ,new string[] { "Assets/Resources/Scene" }                                                              ,null, PackPolicyAllInOne, 0),
        /// new AssetPackCommand(EAssetType.UI      ,new string[] { "Assets/Resources/UI", "Assets/Resources/UIPacked", "Assets/Resources/UIFlatten" }      ,null, PackPolicyAllInOne, 0),
        /// new AssetPackCommand(EAssetType.Sound   ,new string[] { "Assets/Resources/Sound" }                                                              ,null, PackPolicyAllInOne, 0),
    };

    static uint m_PackMask = ~0u;
    Vector2 m_vScrollViewPos = Vector2.zero;
    static List<string> m_vecInfoList = new List<string>();

    static public string[] packExt = new string[] { ".prefab", ".asset", ".ogg", ".tga", ".png", ".json", ".xml", ".shader", ".controller", ".anim", ".mat", ".fbx" };

    class AssetPackageMapDesc
    {
        public string assetKey;
        public string assetPackageGUID;
        public int packageDescIdx;

        static public int Comparison(AssetPackageMapDesc x, AssetPackageMapDesc y)
        {
            return x.assetKey.CompareTo(y.assetKey);
        }
    }

    [MenuItem("[打包工具]/Pack Assets")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        AssetPacker window = (AssetPacker)EditorWindow.GetWindow(typeof(AssetPacker));
        window.titleContent = new GUIContent("资源打包");
        window.Show();
        m_PackMask = ~0u;
    }

    void OnGUI()
    {

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("请选择要打包的资源：");

        GUILayout.BeginVertical();
        ++EditorGUI.indentLevel;

        bool bActor = (m_PackMask & (uint)(1 << (int)EAssetType.Actor)) != 0;
        bActor = EditorGUILayout.Toggle(EAssetType.Actor.ToString(), bActor);

        bool bShader = (m_PackMask & (uint)(1 << (int)EAssetType.Shader)) != 0;
        bShader = EditorGUILayout.Toggle(EAssetType.Shader.ToString(), bShader);

        bool bData = (m_PackMask     & (uint)(1 << (int)EAssetType.Data)) != 0;
        bData = EditorGUILayout.Toggle(EAssetType.Data.ToString(), bData);

        bool bScene = (m_PackMask   & (uint)(1 << (int)EAssetType.Scene)) != 0;
        bScene = EditorGUILayout.Toggle(EAssetType.Scene.ToString(), bScene);

        bool bEffect = (m_PackMask & (uint)(1 << (int)EAssetType.Effect)) != 0;
        bEffect = EditorGUILayout.Toggle(EAssetType.Effect.ToString(), bEffect);

        bool bUI = (m_PackMask         & (uint)(1 << (int)EAssetType.UI)) != 0;
        bUI = EditorGUILayout.Toggle(EAssetType.UI.ToString(), bUI);

        bool bSound = (m_PackMask   & (uint)(1 << (int)EAssetType.Sound)) != 0;
        bSound = EditorGUILayout.Toggle(EAssetType.Sound.ToString(), bSound);

        m_PackMask = bActor ?   (m_PackMask | (1 << (int)EAssetType.Actor)) : (m_PackMask & ~((uint)(1 << (int)EAssetType.Actor)));
        m_PackMask = bShader ?  (m_PackMask | (1 << (int)EAssetType.Shader)) : (m_PackMask & ~((uint)(1 << (int)EAssetType.Shader)));
        m_PackMask = bData ?    (m_PackMask | (1 << (int)EAssetType.Data)) : (m_PackMask & ~((uint)(1 << (int)EAssetType.Data)));
        m_PackMask = bScene ?   (m_PackMask | (1 << (int)EAssetType.Scene)) : (m_PackMask & ~((uint)(1 << (int)EAssetType.Scene)));
        m_PackMask = bEffect ?  (m_PackMask | (1 << (int)EAssetType.Effect)) : (m_PackMask & ~((uint)(1 << (int)EAssetType.Effect)));
        m_PackMask = bUI ?      (m_PackMask | (1 << (int)EAssetType.UI)) : (m_PackMask & ~((uint)(1 << (int)EAssetType.UI)));
        m_PackMask = bSound ?   (m_PackMask | (1 << (int)EAssetType.Sound)) : (m_PackMask & ~((uint)(1 << (int)EAssetType.Sound)));

        m_PackMask &= (int)EAssetType.Mask;
        --EditorGUI.indentLevel;
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        if (GUILayout.Button("执行打包1"))
        {
            Logger.LogError("============1123123");
            BuildPackage.IncrementBuildBundle(false);
        }

        if (GUILayout.Button("执行打包"))
        {
            _ClearPackages();
            _PackAllAsset(EditorUserBuildSettings.activeBuildTarget, m_PackMask);
            _GenerateAssetTreeData(EditorUserBuildSettings.activeBuildTarget);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("清理打包目录"))
        {
            _ClearPackages();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("生成新版依赖文件列表"))
        {
            _GenerateAssetTreeData(EditorUserBuildSettings.activeBuildTarget);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("执行首包打包"))
        {
            _SplitBasePacakge();
            //_PackSpecificAsset(EditorUserBuildSettings.activeBuildTarget);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal("ObjectFieldThumb");
        {
            m_vScrollViewPos = EditorGUILayout.BeginScrollView(m_vScrollViewPos, GUILayout.Height(180));

            for(int i = 0,icnt = m_vecInfoList.Count; i < icnt; ++ i)
            {
                if(!string.IsNullOrEmpty(m_vecInfoList[i]))
                    EditorGUILayout.LabelField(m_vecInfoList[i]);
            }

            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndHorizontal();
    }
    static public void PackAsset_CommandMode()
    {
        _ClearPackages();
        _PackAllAsset(EditorUserBuildSettings.activeBuildTarget, m_PackMask);
        _GenerateAssetTreeData(EditorUserBuildSettings.activeBuildTarget);
    }

    static public void PackDataAsset_CommandMode()
    {
        _ClearPackages();
        _PackAllAsset(EditorUserBuildSettings.activeBuildTarget,(uint)(1 << (int)EAssetType.Data));
        _GenerateAssetTreeData(EditorUserBuildSettings.activeBuildTarget);
    }

    static protected string[] _GatherDependency(string objectPath, string[] excludeExt)
    {
        string objExt = PathUtil.GetExtension(objectPath);
        if (objExt == ".asset" || objExt == ".prefab")
        {
            string[] dependency = AssetDatabase.GetDependencies(objectPath);
            if (null != excludeExt && excludeExt.Length > 0)
            {
                List<string> dependFilter = new List<string>();
                bool bSkip = false;
                for (int i = 0; i < dependency.Length; ++i)
                {
                    bSkip = false;
                    for (int j = 0; j < excludeExt.Length; ++j)
                    {
                        if (excludeExt[j] == PathUtil.GetExtension(dependency[i]))
                        {
                            bSkip = true;
                            break;
                        }
                    }

                    if (bSkip) continue;
                    dependFilter.Add(dependency[i]);
                }

                return dependFilter.ToArray();
            }
            else
                return dependency;
        }

        return null;
    }


    static public void _PackAllAsset(BuildTarget platform,uint mask = 0xff)
    {
        //_ReimportFBXMaterial();
        _BreakDungeonAssetLink();
        _BreakSkillAssetLink();
        _BuildSceneSpriteAltas();

        Hashtable setting = null;
        int patchVer = 0;
        byte[] fileData = CFileManager.ReadFile(Path.Combine("Assets/StreamingAssets", "version.config"));
        if (null != fileData)
        {
            setting = MiniJSON.jsonDecode(System.Text.UTF8Encoding.Default.GetString(fileData)) as Hashtable;
            patchVer = int.Parse(setting["clientShortVersion"].ToString());
        }

        m_vecInfoList.Add("第一步 执行打包策略，收集打包资源...(共" + m_PackCommands.Length.ToString() + "个集合)。");
        List <AssetPackageDesc> assetPackageDesc = new List<AssetPackageDesc>();
        for (int i = 0; i < m_PackCommands.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在执行打包策略" + i + "，收集打包资源...", ((float)i / m_PackCommands.Length));
            AssetPackCommand curCommand = m_PackCommands[i];

            if(((uint)(1<<(int)curCommand.m_AssetType) & mask) == 0) continue;

            if (null != curCommand.m_ExcluderPath)
            {
                for (int j = 0, jcnt = curCommand.m_ExcluderPath.Length; j < jcnt; ++j)
                {
                    string curPath = curCommand.m_ExcluderPath[j];
                    curPath = curPath.Replace('\\', '/');
                    if ('/' != curPath[curPath.Length - 1])
                        curPath += '/';
                    curCommand.m_ExcluderPath[j] = curPath;
                }
            }

            curCommand.m_PackPolicy(curCommand.m_AssetType,curCommand.m_AssetPath,curCommand.m_ExcluderPath,curCommand.m_PackLevel, assetPackageDesc);
        }
        EditorUtility.ClearProgressBar();
        m_vecInfoList.Add("第一步 执行完毕！");

        /// 收集和关联依赖
        Dictionary<string, string> assetInPackMap = new Dictionary<string, string>();
        for (int i = 0; i < assetPackageDesc.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包","正在收集和关联依赖...",((float)i / assetPackageDesc.Count) * 0.5f);
            AssetPackageDesc curPackageDesc = assetPackageDesc[i];
            for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
            {
                if (!assetInPackMap.ContainsKey(curPackageDesc.m_PackageAsset[j].m_AssetPath))
                    assetInPackMap.Add(curPackageDesc.m_PackageAsset[j].m_AssetPath, curPackageDesc.m_PackageName);
                else
                    Logger.LogWarningFormat("Same asset [{0}] has packed into multiple package!", curPackageDesc.m_PackageAsset[j]);
            }
        }

        for (int i = 0; i < assetPackageDesc.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在收集和关联依赖...", ((float)i / assetPackageDesc.Count) * 0.5f + 0.5f);
            AssetPackageDesc curPackageDesc = assetPackageDesc[i];
            for (int j = 0; j < curPackageDesc.m_DependAsset.Count; ++j)
            {
                string curPackage = "";
                if (assetInPackMap.TryGetValue(curPackageDesc.m_DependAsset[j], out curPackage))
                {
                    if (!curPackageDesc.m_DependPackage.Contains(curPackage))
                        curPackageDesc.m_DependPackage.Add(curPackage);
                }
                else
                {
                    Logger.LogWarningFormat("Dependent asset [{0}] does not packed into any package!", curPackageDesc.m_PackageAsset[j]);
                }
            }
        }
        EditorUtility.ClearProgressBar();

        /// 建立BundleAsset资源备份
        if (Directory.Exists(m_TempAssetPath))
            Directory.Delete(m_TempAssetPath, true);

        AssetDatabase.CreateFolder("Assets", "PackAssetTemp");

        for (int i = 0; i < assetPackageDesc.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在以GUID为键值重命名资源...", ((float)i / assetPackageDesc.Count));
            AssetPackageDesc curPackageDesc = assetPackageDesc[i];
        
            if(0 == ((uint)DAssetPackageFlag.UsingGUIDName & curPackageDesc.m_PackageFlag))
                continue;
        
            for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
            {
            
                string srcFile = curPackageDesc.m_PackageAsset[j].m_AssetPath;
                if (File.Exists(srcFile))
                {
                    string dstFile = m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath);
                    //AssetDatabase.CopyAsset(curPackageDesc.m_PackageAsset[j].m_AssetPath, m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath));
                    File.Copy(srcFile, dstFile);
                    string srcMetaFile = curPackageDesc.m_PackageAsset[j].m_AssetPath + ".meta";
                    if (File.Exists(srcMetaFile))
                    {
                        string dstMetaFile = m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath) + ".meta";
                        if (File.Exists(dstMetaFile))
                            File.Delete(dstMetaFile);
            
                        File.Copy(srcMetaFile, dstMetaFile);
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        if (!Directory.Exists(m_OutputBundlePath))
            Directory.CreateDirectory(m_OutputBundlePath);

        /// 检查资源重名
        m_SameAssetTableList.Clear();
        for (int i = 0; i < assetPackageDesc.Count; ++i)
        {
            AssetPackageDesc curPackageDesc = assetPackageDesc[i];
            if (null == curPackageDesc) continue;
            if((curPackageDesc.m_PackageFlag & (uint)DAssetPackageFlag.UsingGUIDName) != 0)
                continue;

            Dictionary<string, AssetCheckDesc> curSameNameTbl = new Dictionary<string, AssetCheckDesc>();
            bool bHasSameAsset = false;
            for (int j = 0, jcnt = curPackageDesc.m_PackageAsset.Count; j < jcnt; ++j)
            {
                string curFilePath = curPackageDesc.m_PackageAsset[j].m_AssetPath;

                if (string.IsNullOrEmpty(curFilePath)) continue;

                string ext = Path.GetExtension(curFilePath);
                if (ext.Equals(".meta", StringComparison.OrdinalIgnoreCase) || ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(curFilePath);

                AssetCheckDesc curAssetCheckDesc = null;
                if (curSameNameTbl.TryGetValue(fileName, out curAssetCheckDesc))
                {
                    if (!curAssetCheckDesc.m_SameAssetList.Contains(curFilePath))
                    {
                        curAssetCheckDesc.m_SameAssetList.Add(curFilePath);
                        bHasSameAsset = true;
                    }
                }
                else
                {
                    curAssetCheckDesc = new AssetCheckDesc();
                    curAssetCheckDesc.m_SameAssetList = new List<string>();
                    curAssetCheckDesc.m_SameAssetList.Add(curFilePath);
                    curAssetCheckDesc.m_PackageName = curPackageDesc.m_PackageName;
                    curSameNameTbl.Add(fileName, curAssetCheckDesc);
                }
            }

            if (bHasSameAsset)
            {
                List<string> keyNeedRemoved = new List<string>();
                Dictionary<string, AssetCheckDesc>.Enumerator it = curSameNameTbl.GetEnumerator();
                while (it.MoveNext())
                {
                    if (it.Current.Value.m_SameAssetList.Count == 1)
                        keyNeedRemoved.Add(it.Current.Key);
                }

                for (int j = 0, jcnt = keyNeedRemoved.Count; j < jcnt; ++j)
                    curSameNameTbl.Remove(keyNeedRemoved[j]);

                m_SameAssetTableList.Add(curSameNameTbl);
            }
        }

        if (m_SameAssetTableList.Count > 0)
        {/// 说明有重复资源
            string mailContent = "";
            for (int i = 0, icnt = m_SameAssetTableList.Count; i < icnt; ++i)
            {
                Dictionary<string, AssetCheckDesc> curSameNameTbl = m_SameAssetTableList[i];
                Dictionary<string, AssetCheckDesc>.Enumerator it = curSameNameTbl.GetEnumerator();

                while (it.MoveNext())
                {
                    string resName = it.Current.Key;
                    AssetCheckDesc resAssetCheckDesc = it.Current.Value;

                    mailContent += string.Format("{0}[Package:{1}] \r\n", resName, resAssetCheckDesc.m_PackageName);
                    for (int j = 0, jcnt = resAssetCheckDesc.m_SameAssetList.Count; j < jcnt; ++j)
                    {
                        mailContent += ("   -   " + resAssetCheckDesc.m_SameAssetList[j]);
                        mailContent += "\r\n";
                    }
                }
            }

            MailSender.SetEmail("simonking200@vip.qq.com", "打包发现重名资源", mailContent);
            MailSender.SetEmail("414759377@qq.com", "打包发现重名资源", mailContent);
            MailSender.SetEmail("262956000@qq.com", "打包发现重名资源", mailContent);
            MailSender.SetEmail("368400127@qq.com", "打包发现重名资源", mailContent);
        }

        List<AssetBundleManifest> packageGroupManifest = new List<AssetBundleManifest>();

        // Actor Shader Data Scene Effect UI Sound
        int[] packageGroupTag = new int[7]{ 0, 0, 0, 0, 0, 0, 0 };
        int GROUP_NUM = 3;
        List<AssetPackageDesc>[] packageGroupList = new List<AssetPackageDesc>[GROUP_NUM];
        for (int i = 0; i < GROUP_NUM; ++i)
            packageGroupList[i] = new List<AssetPackageDesc>();

        for (int i = 0,icnt = assetPackageDesc.Count; i<icnt;++i)
        {
            AssetPackageDesc curDesc = assetPackageDesc[i];
            if (null != curDesc)
            {
                int idx = (int)curDesc.m_AssetType;
                if(idx < packageGroupTag.Length)
                {
                    if(packageGroupTag[idx] < GROUP_NUM)
                        packageGroupList[packageGroupTag[idx]].Add(curDesc);
                }
            }
        }

        AssetDatabase.Refresh();

        List<DAssetPackageDesc> assetPackageDescList = new List<DAssetPackageDesc>();

        for (int g = 0; g < GROUP_NUM; ++g)
        {
            List<AssetPackageDesc> curPackageGroup = packageGroupList[g];
            if(null != curPackageGroup)
            {
                AssetBundleBuild[] bundleBuildGrp = new AssetBundleBuild[curPackageGroup.Count];
                for (int i = 0; i < bundleBuildGrp.Length; ++i)
                {
                    EditorUtility.DisplayProgressBar("正在打包", "正在准备打包...", ((float)i / bundleBuildGrp.Length));
                    AssetPackageDesc curPackageDesc = curPackageGroup[i];

                    bundleBuildGrp[i].assetBundleName = curPackageDesc.m_PackageName;
                    bundleBuildGrp[i].assetBundleVariant = "";

                    bundleBuildGrp[i].assetNames = new string[curPackageDesc.m_PackageAsset.Count];

                    for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
                    {
                        if (0 != ((uint)DAssetPackageFlag.UsingGUIDName & curPackageDesc.m_PackageFlag))
                            bundleBuildGrp[i].assetNames[j] = m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath);
                        else
                            bundleBuildGrp[i].assetNames[j] = curPackageDesc.m_PackageAsset[j].m_AssetPath;
                    }
                }

                EditorUtility.ClearProgressBar();

                AssetBundleManifest assetManifestGrp= BuildPipeline.BuildAssetBundles(m_OutputBundlePath, bundleBuildGrp, BuildAssetBundleOptions.ChunkBasedCompression, platform);
                if (null == assetManifestGrp)
                {
                    Debug.LogError("####### FATAL ERROR: Build asset bundle has failed! ABORT!!! ########");
                    continue;
                }

                for (int i = 0; i < bundleBuildGrp.Length; ++i)
                {
                    DAssetPackageDesc assetPackageDescOut = new DAssetPackageDesc();

                    if (null == bundleBuildGrp[i].assetBundleName)
                        continue;

                    if (curPackageGroup[i].m_IncludeDepend)
                    {
                        string[] depent = assetManifestGrp.GetAllDependencies(bundleBuildGrp[i].assetBundleName);
                        if (null != depent && depent.Length > 0)
                        {
                            assetPackageDescOut.packageAutoDepend = new string[depent.Length];
                            for (int j = 0; j < depent.Length; ++j)
                                assetPackageDescOut.packageAutoDepend[j] = depent[j];
                        }
                    }

                    assetPackageDescOut.packageMD5 = FileUtil.GetFileMD5(m_OutputBundlePath + bundleBuildGrp[i].assetBundleName);
                    assetPackageDescOut.packageName = curPackageGroup[i].m_PackageName.ToLower();
                    assetPackageDescOut.packagePath = m_StreamingBundlePath;
                    assetPackageDescOut.packageKey = assetPackageDescOut.packagePath.ToLower() + assetPackageDescOut.packageName.ToLower();
                    assetPackageDescOut.packageVer = 0;

                    assetPackageDescOut.packageFlag = curPackageGroup[i].m_PackageFlag;
                    assetPackageDescOut.packageDependency = curPackageGroup[i].m_DependPackage.ToArray();

                    List<DPackAssetDesc> assetList = new List<DPackAssetDesc>();
                    for (int j = 0; j < curPackageGroup[i].m_PackageAsset.Count; ++j)
                        assetList.Add(new DPackAssetDesc(PathUtil.EraseExtension(curPackageGroup[i].m_PackageAsset[j].m_AssetPath.ToLower()).Replace("assets/resources/", null).Replace('\\', '/'), curPackageGroup[i].m_PackageAsset[j].m_AssetGUID));

                    assetPackageDescOut.packageAsset = assetList.ToArray();
                    assetPackageDescList.Add(assetPackageDescOut);
                }
            }
        }

        //AssetBundleBuild[] bundleBuild = new AssetBundleBuild[assetPackageDesc.Count];
        //for (int i = 0; i < bundleBuild.Length; ++i)
        //{
        //    EditorUtility.DisplayProgressBar("正在打包", "正在准备打包...", ((float)i / bundleBuild.Length));
        //    AssetPackageDesc curPackageDesc = assetPackageDesc[i];
        //
        //    bundleBuild[i].assetBundleName = curPackageDesc.m_PackageName;
        //    bundleBuild[i].assetBundleVariant = "";
        //
        //    bundleBuild[i].assetNames = new string[curPackageDesc.m_PackageAsset.Count];
        //
        //    for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
        //    {
        //
        //        if (0 != ((uint)DAssetPackageFlag.UsingGUIDName & curPackageDesc.m_PackageFlag))
        //            bundleBuild[i].assetNames[j] = m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath);
        //        else
        //            bundleBuild[i].assetNames[j] = curPackageDesc.m_PackageAsset[j].m_AssetPath;
        //
        //        //string dir = Path.GetDirectoryName(curPackageDesc.m_PackageAsset[j].m_AssetPath);
        //        //string ext = PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath);
        //        //string file = dir + "/" + curPackageDesc.m_PackageAsset[j].m_AssetGUID + ext;
        //        //bundleBuild[i].assetNames[j] = file;
        //    }
        //}
        //
        //EditorUtility.ClearProgressBar();
        //
        //AssetDatabase.Refresh();
        //
        //AssetBundleManifest assetManifest = BuildPipeline.BuildAssetBundles(m_OutputBundlePath, bundleBuild, BuildAssetBundleOptions.ChunkBasedCompression, platform);
        //
        //DAssetPackageDesc[] assetPackageDescListOut = new DAssetPackageDesc[bundleBuild.Length];
        //for (int i = 0; i < bundleBuild.Length; ++i)
        //{
        //    DAssetPackageDesc assetPackageDescOut = new DAssetPackageDesc();
        //
        //    if (null == bundleBuild[i].assetBundleName)
        //        continue;
        //
        //    if(assetPackageDesc[i].m_IncludeDepend)
        //    {
        //        string[] depent = assetManifest.GetAllDependencies(bundleBuild[i].assetBundleName);
        //        if (null != depent && depent.Length > 0)
        //        {
        //            assetPackageDescOut.packageAutoDepend = new string[depent.Length];
        //            for (int j = 0; j < depent.Length; ++j)
        //                assetPackageDescOut.packageAutoDepend[j] = depent[j];
        //        }
        //    }
        //
        //    assetPackageDescOut.packageMD5 = FileUtil.GetFileMD5(m_OutputBundlePath + bundleBuild[i].assetBundleName);
        //    assetPackageDescOut.packageName = assetPackageDesc[i].m_PackageName.ToLower();
        //    assetPackageDescOut.packagePath = m_StreamingBundlePath;
        //    assetPackageDescOut.packageKey = assetPackageDescOut.packagePath.ToLower() + assetPackageDescOut.packageName.ToLower();
        //    assetPackageDescOut.packageVer = 0;
        //    
        //    assetPackageDescOut.packageFlag = assetPackageDesc[i].m_PackageFlag;
        //    assetPackageDescOut.packageDependency = assetPackageDesc[i].m_DependPackage.ToArray();
        //
        //    List<DPackAssetDesc> assetList = new List<DPackAssetDesc>();
        //    for (int j = 0; j < assetPackageDesc[i].m_PackageAsset.Count; ++j)
        //        assetList.Add(new DPackAssetDesc(PathUtil.EraseExtension(assetPackageDesc[i].m_PackageAsset[j].m_AssetPath.ToLower()).Replace("assets/resources/", null).Replace('\\', '/'), assetPackageDesc[i].m_PackageAsset[j].m_AssetGUID));
        //
        //    assetPackageDescOut.packageAsset = assetList.ToArray();
        //    assetPackageDescListOut[i] = assetPackageDescOut;
        //}

        DAssetPackageDesc[] assetPackageDescListOut = assetPackageDescList.ToArray();
        assetPackageDescListOut = _UnifyBundle(assetPackageDescListOut);

        DAssetPackageDependency asset = null;
        // /// 增量依赖
        // DAssetPackageDependency asset = AssetDatabase.LoadAssetAtPath<DAssetPackageDependency>("Assets/Resources/Base/Version/PackageInfo.asset");
        // if(null != asset && asset.packageDescArray.Length > 0)
        //     assetPackageDescListOut = _MergeBundle(asset.packageDescArray, assetPackageDescListOut);
        // /// 增量依赖
        // /// 

        Dictionary <string, int> assetMap = new Dictionary<string, int>();
        for (int i = 0, icnt = assetPackageDescListOut.Length; i < icnt; ++i)
        {
            DAssetPackageDesc curAssetPackageDesc = assetPackageDescListOut[i];

            if (!assetMap.ContainsKey(curAssetPackageDesc.packageName))
                assetMap.Add(curAssetPackageDesc.packageName, i);
            else
                Debug.LogWarningFormat("Same bundle key \"{0}\"!", curAssetPackageDesc.packageName);
        }

        Logger.LogAsset("Initialize asset packages!");
        for (int i = 0; i < assetPackageDescListOut.Length; ++i)
        {
            DAssetPackageDesc curAssetPackageDesc = assetPackageDescListOut[i];
            List<int> assetPackageList = new List<int>();
            for (int j = 0; j < curAssetPackageDesc.packageAutoDepend.Length; ++j)
            {
                int curPackageIdx = 0;
                if (assetMap.TryGetValue(curAssetPackageDesc.packageAutoDepend[j], out curPackageIdx))
                    assetPackageList.Add(curPackageIdx);
            }

            curAssetPackageDesc.packageAutoDependIdx = assetPackageList.ToArray();
        }

        if (!Directory.Exists("Assets/Resources/Base/Version"))
            Directory.CreateDirectory("Assets/Resources/Base/Version");

        if (null == asset)
        {
            asset = ScriptableObject.CreateInstance<DAssetPackageDependency>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/Base/Version/PackageInfo.asset");
        }

        /// 建立反查表
        asset.assetDescPackageMap.Clear();
        for (int i = 0, icnt = assetPackageDescListOut.Length; i < icnt; ++i)
        {
            for (int j = 0, jcnt = assetPackageDescListOut[i].packageAsset.Length; j < jcnt; ++j)
            {
                DPackAssetDesc curPackageAsset = assetPackageDescListOut[i].packageAsset[j];

                DAssetPackageMapDesc assetPackageMapDesc = new DAssetPackageMapDesc();
                assetPackageMapDesc.assetPathKey = curPackageAsset.packageAsset;
                assetPackageMapDesc.assetPackageGUID = curPackageAsset.packageGUID;
                assetPackageMapDesc.packageDescIdx = i;
                asset.assetDescPackageMap.Add(assetPackageMapDesc);
            }
        }
        asset.assetDescPackageMap.Sort(DAssetPackageDependency.Comparison);

        //AssetDatabase.CreateAsset(asset, "Assets/Resources/Base/Version/PackageInfo_" + date + ".asset");
        asset.packageDescArray = assetPackageDescListOut;
        asset.patchVersion = patchVer;

        _DumpPackageInfoAsTxt(assetPackageDescListOut);
        _DumpPackageInfoAsCsv(asset);

        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();

        AssetBundleBuild[] bundlePackageInfoBuild = new AssetBundleBuild[1];
        bundlePackageInfoBuild[0].assetBundleName = "PackageInfo.pak";
        bundlePackageInfoBuild[0].assetNames = new string[1] { "Assets/Resources/Base/Version/PackageInfo.asset" };
        bundlePackageInfoBuild[0].assetBundleVariant = "";

        AssetBundleManifest assetManifestPackageInfo = BuildPipeline.BuildAssetBundles(m_OutputBundlePath, bundlePackageInfoBuild, BuildAssetBundleOptions.None, platform);
        Logger.Log("Asset");
        /// 升级一下版本号文件 Patch字段

        if(null != setting)
        {
            setting["clientShortVersion"] = ++patchVer;

            FileStream streamVer = new FileStream(Path.Combine("Assets/StreamingAssets", "version.config"), FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter swVer = new StreamWriter(streamVer, Encoding.GetEncoding(936));

            swVer.Write(MiniJSON.jsonEncode(setting));

            streamVer.Flush();

            swVer.Close();
            streamVer.Close();
        }

        //_SplitBasePacakge();


        //         for (int i = 0; i < assetPackageDesc.Count; ++i)
        //         {
        //             AssetPackageDesc curPackageDesc = assetPackageDesc[i];
        //             for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
        //             {
        //                 if (File.Exists(curPackageDesc.m_PackageAsset[j].m_AssetPath))
        //                 {
        //                     string dir = Path.GetDirectoryName(curPackageDesc.m_PackageAsset[j].m_AssetPath);
        //                     string ext = PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath);
        //                     string file = dir + "/" + curPackageDesc.m_PackageAsset[j].m_AssetGUID + ext;
        // 
        //                     string res = AssetDatabase.RenameAsset(file, curPackageDesc.m_PackageAsset[j].m_AssetPath);
        //                     if (!string.IsNullOrEmpty(res))
        //                         Logger.LogError(res);
        //                 }
        //             }
        //         }

    }
    class AssetSearchCompare : IComparer<DAssetPackageMapDesc>
    {
        public int Compare(DAssetPackageMapDesc x, DAssetPackageMapDesc y)
        {
            return x.assetPathKey.CompareTo(y.assetPathKey);
        }
    }
    static AssetSearchCompare SEARCH_COMPARE = new AssetSearchCompare();
    static DAssetPackageMapDesc DUMMY_MAPDESC = new DAssetPackageMapDesc();

    static DAssetPackageDesc _GetAssetPackageDesc(DAssetPackageDependency packageDependencyDesc, string resPath)
    {
        if (null != packageDependencyDesc)
        {
            if (null != packageDependencyDesc.assetDescPackageMap)
            {
                DUMMY_MAPDESC.assetPathKey = resPath.ToLower();
                int idx = packageDependencyDesc.assetDescPackageMap.BinarySearch(DUMMY_MAPDESC, SEARCH_COMPARE);

                if (0 <= idx && idx < packageDependencyDesc.assetDescPackageMap.Count)
                {
                    DAssetPackageMapDesc curDesc = packageDependencyDesc.assetDescPackageMap[idx];
                    if (0 <= curDesc.packageDescIdx && curDesc.packageDescIdx < packageDependencyDesc.packageDescArray.Length)
                    {
                        DAssetPackageDesc curPackageDesc = packageDependencyDesc.packageDescArray[curDesc.packageDescIdx];
                        return curPackageDesc;
                    }
                }
            }
        }

        return null;
    }

    static protected void _SplitBasePacakge()
    {
        /// 执行分包
        /// 加载首包资源列表
        FileStream streamR = new FileStream("Assets/StreamingAssets/BaseAssetList.lst", FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(streamR);
        string buf = sr.ReadToEnd();
        sr.Close();
        streamR.Close();

        string[] lines = buf.Split(new char[] { '\n', '\t' });
        List<string> allAssets = new List<string>();
        if (null != lines)
        {
            for (int i = 0, icnt = lines.Length; i < icnt; ++i)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                if ("-" == lines[i]) continue;

                string pathR = lines[i].Replace("\r", null);
                if (!allAssets.Contains(pathR))
                    allAssets.Add(pathR);
            }
        }
        
        DAssetPackageDependency packageDependencyDesc = AssetDatabase.LoadAssetAtPath<DAssetPackageDependency>("Assets/Resources/Base/Version/PackageInfo.asset") as DAssetPackageDependency;
        if (null != packageDependencyDesc)
        {
            List<DAssetPackageDesc> packageListAll = new List<DAssetPackageDesc>();
            for (int i = 0, icnt = allAssets.Count; i < icnt; ++i)
            {
                string ext = Path.GetExtension(allAssets[i]);
                if (ext.Equals(".meta", System.StringComparison.OrdinalIgnoreCase) || ext.Equals(".cs", System.StringComparison.OrdinalIgnoreCase)
                     || ext.Equals(".dll", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                string validPath = Path.ChangeExtension(allAssets[i].Replace("Assets/Resources/", null),null).Replace('\\', '/');
                packageListAll.Add(_GetAssetPackageDesc(packageDependencyDesc, Path.ChangeExtension(validPath, null)));
            }

            List<string> basePackageList = new List<string>();
            for (int i = 0, icnt = packageListAll.Count; i < icnt; ++i)
            {
                DAssetPackageDesc curDesc = packageListAll[i];

                if (null == curDesc) continue;

                string packageFullPath = Path.Combine(curDesc.packagePath, curDesc.packageName).Replace('\\', '/');
                if (!basePackageList.Contains(packageFullPath))
                    basePackageList.Add(packageFullPath);
            }

            if (!Directory.Exists("Assets/StreamingAssets/AssetBundles/Other"))
                Directory.CreateDirectory("Assets/StreamingAssets/AssetBundles/Other");

            List<string> otherPackageList = new List<string>();
            for (int i = 0,icnt = packageDependencyDesc.packageDescArray.Length;i<icnt;++i)
            {
                DAssetPackageDesc cur =packageDependencyDesc.packageDescArray[i];
                if(null == cur ) continue;

                string packageFullPath = Path.Combine(cur.packagePath, cur.packageName).Replace('\\', '/');
                if (!basePackageList.Contains(packageFullPath))
                    otherPackageList.Add(packageFullPath);
            }

            for (int i = 0, icnt = otherPackageList.Count; i < icnt; ++i)
            {
                string name = Path.GetFileName(otherPackageList[i]);
                string newPath = Path.Combine("Assets/StreamingAssets/", Path.Combine(Path.Combine(Path.GetDirectoryName(otherPackageList[i]),"Other"),name));
                File.Move(Path.Combine("Assets/StreamingAssets/",otherPackageList[i]), newPath);
            }
        }
    }


    static protected void _PackSpecificAsset(BuildTarget platform)
    {
        _BreakDungeonAssetLink();
        _BreakSkillAssetLink();
        _BuildSceneSpriteAltas();

        Hashtable setting = null;
        int patchVer = 0;
        byte[] fileData = CFileManager.ReadFile(Path.Combine("Assets/StreamingAssets", "version.config"));
        if (null != fileData)
        {
            setting = MiniJSON.jsonDecode(System.Text.UTF8Encoding.Default.GetString(fileData)) as Hashtable;
            patchVer = int.Parse(setting["clientShortVersion"].ToString());
        }

        FileStream streamR = new FileStream("Assets/StreamingAssets/AllAssetList.txt", FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(streamR);
        string buf = sr.ReadToEnd();
        sr.Close();
        streamR.Close();

        string[] lines = buf.Split(new char[] { '\n', '\t' });
        
        List<string> specificAssetList = new List<string>();
        if (null != lines)
        {
            for (int i = 0, icnt = lines.Length; i < icnt; ++i)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                if ("-" == lines[i]) continue;

                string pathR = lines[i].Replace("\r", null);
                if (!specificAssetList.Contains(pathR))
                    specificAssetList.Add(pathR);
            }
        }

        AssetPackCommand[] packCommands = new AssetPackCommand[]
        {
            new AssetPackCommand(EAssetType.Shader  ,new string[] { "Assets/Resources/Shader", "Assets/Resources/Animat"}     ,null                                                                                                                                                                         , PackPolicyShader          , 0),
            new AssetPackCommand(EAssetType.Shader  ,new string[] { "Assets/Resources/Environment"}                           ,null                                                                                                                                                                         , PackPolicyEnvironment     , 0),

            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/AI" }                              ,new string[] { "Assets/Resources/Data/AI/Guanka_apc", "Assets/Resources/Data/AI/Monster_AI" }                                                                                , PackPolicyData            , 1),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/AI/Guanka_apc" }                   ,null                                                                                                                                                                         , PackPolicyData            , 1),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/AI/Monster_AI" }                   ,null                                                                                                                                                                         , PackPolicyData            , 1),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/CommonData" }                      ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/NewbieGuide" }                     ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/ChapterData" }                     ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/DungeonData" }                     ,null                                                                                                                                                                         , PackPolicyData            , 1),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/DungeonMapData" }                  ,null                                                                                                                                                                         , PackPolicyData            , 0),
            //new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/InputData" }                       ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SceneData" }                       ,null                                                                                                                                                                         , PackPolicyData            , 1),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SkillData" }                       ,new string[] {"Assets/Resources/Data/SkillData/Monster"}                                                                                                                     , PackPolicyData            , 1),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SkillData_fb" }                           ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/table" }                           ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/table_fb" }                           ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/Language" }                        ,null                                                                                                                                                                         , PackPolicyData            , 0),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/SkillData/Monster" }               ,null                                                                                                                                                                         , PackPolicyData            , 1),
            new AssetPackCommand(EAssetType.Data    ,new string[] { "Assets/Resources/Data/Lua" }                             ,null                                                                                                                                                                         , PackPolicyData            , 0),
        };
        
        List<AssetPackageDesc> assetPackageDesc = new List<AssetPackageDesc>();
        for (int i = 0; i < packCommands.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在执行打包策略" + i + "，收集打包资源...", ((float)i / packCommands.Length));
            AssetPackCommand curCommand = packCommands[i];
            if (null != curCommand.m_ExcluderPath)
            {
                for (int j = 0, jcnt = curCommand.m_ExcluderPath.Length; j < jcnt; ++j)
                {
                    string curPath = curCommand.m_ExcluderPath[j];
                    curPath = curPath.Replace('\\', '/');
                    if ('/' != curPath[curPath.Length - 1])
                        curPath += '/';
                    curCommand.m_ExcluderPath[j] = curPath;
                }
            }
        
            curCommand.m_PackPolicy(curCommand.m_AssetType, curCommand.m_AssetPath, curCommand.m_ExcluderPath, curCommand.m_PackLevel, assetPackageDesc);
        }
        EditorUtility.ClearProgressBar();

        AssetPackageDesc specificPackageDesc = new AssetPackageDesc();
        if (specificAssetList.Count > 0)
        {
            for(int i = 0,icnt = specificAssetList.Count;i<icnt;++i)
            {
                string path = specificAssetList[i];
                if (string.IsNullOrEmpty(path)) continue;
                if (path.Contains("/Data/") || path.Contains("/Shader/") || path.Contains("/Environment/") || path.Contains("/Animat/")) continue;

                string ext = Path.GetExtension(path);
                if(ext.Equals(".unity",StringComparison.OrdinalIgnoreCase))
                    continue;

                specificPackageDesc.m_PackageAsset.Add(new PackAssetDesc(path));
            }
        }

        specificPackageDesc.m_AssetType = EAssetType.Actor;
        specificPackageDesc.m_IncludeDepend = true;
        specificPackageDesc.m_DependAsset = new List<string>();
        specificPackageDesc.m_PackageFlag = (uint)DAssetPackageFlag.None;
        specificPackageDesc.m_PackageName = "first_package" + m_OutputBundleExt;

        assetPackageDesc.Add(specificPackageDesc);

        /// 收集和关联依赖
        Dictionary<string, string> assetInPackMap = new Dictionary<string, string>();
        for (int i = 0; i < assetPackageDesc.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在收集和关联依赖...", ((float)i / assetPackageDesc.Count) * 0.5f);
            AssetPackageDesc curPackageDesc = assetPackageDesc[i];
            for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
            {
                if (!assetInPackMap.ContainsKey(curPackageDesc.m_PackageAsset[j].m_AssetPath))
                    assetInPackMap.Add(curPackageDesc.m_PackageAsset[j].m_AssetPath, curPackageDesc.m_PackageName);
                else
                    Logger.LogWarningFormat("Same asset [{0}] has packed into multiple package!", curPackageDesc.m_PackageAsset[j]);
            }
        }

        for (int i = 0; i < assetPackageDesc.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在收集和关联依赖...", ((float)i / assetPackageDesc.Count) * 0.5f + 0.5f);
            AssetPackageDesc curPackageDesc = assetPackageDesc[i];
            for (int j = 0; j < curPackageDesc.m_DependAsset.Count; ++j)
            {
                string curPackage = "";
                if (assetInPackMap.TryGetValue(curPackageDesc.m_DependAsset[j], out curPackage))
                {
                    if (!curPackageDesc.m_DependPackage.Contains(curPackage))
                        curPackageDesc.m_DependPackage.Add(curPackage);
                }
                else
                {
                    Logger.LogWarningFormat("Dependent asset [{0}] does not packed into any package!", curPackageDesc.m_PackageAsset[j]);
                }
            }
        }
        EditorUtility.ClearProgressBar();

        /// 建立BundleAsset资源备份
        if (Directory.Exists(m_TempAssetPath))
            Directory.Delete(m_TempAssetPath, true);

        AssetDatabase.CreateFolder("Assets", "PackAssetTemp");

        for (int i = 0; i < assetPackageDesc.Count; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在以GUID为键值重命名资源...", ((float)i / assetPackageDesc.Count));
            AssetPackageDesc curPackageDesc = assetPackageDesc[i];

            if (0 == ((uint)DAssetPackageFlag.UsingGUIDName & curPackageDesc.m_PackageFlag))
                continue;

            for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
            {

                string srcFile = curPackageDesc.m_PackageAsset[j].m_AssetPath;
                if (File.Exists(srcFile))
                {
                    string dstFile = m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath);
                    //AssetDatabase.CopyAsset(curPackageDesc.m_PackageAsset[j].m_AssetPath, m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath));
                    File.Copy(srcFile, dstFile);
                    string srcMetaFile = curPackageDesc.m_PackageAsset[j].m_AssetPath + ".meta";
                    if (File.Exists(srcMetaFile))
                    {
                        string dstMetaFile = m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath) + ".meta";
                        if (File.Exists(dstMetaFile))
                            File.Delete(dstMetaFile);

                        File.Copy(srcMetaFile, dstMetaFile);
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        if (!Directory.Exists(m_OutputBundlePath))
            Directory.CreateDirectory(m_OutputBundlePath);

        List<AssetBundleManifest> packageGroupManifest = new List<AssetBundleManifest>();

        // Actor Shader Data Scene Effect UI Sound
        int[] packageGroupTag = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
        int GROUP_NUM = 3;
        List<AssetPackageDesc>[] packageGroupList = new List<AssetPackageDesc>[GROUP_NUM];
        for (int i = 0; i < GROUP_NUM; ++i)
            packageGroupList[i] = new List<AssetPackageDesc>();

        for (int i = 0, icnt = assetPackageDesc.Count; i < icnt; ++i)
        {
            AssetPackageDesc curDesc = assetPackageDesc[i];
            if (null != curDesc)
            {
                int idx = (int)curDesc.m_AssetType;
                if (idx < packageGroupTag.Length)
                {
                    if (packageGroupTag[idx] < GROUP_NUM)
                        packageGroupList[packageGroupTag[idx]].Add(curDesc);
                }
            }
        }

        AssetDatabase.Refresh();

        List<DAssetPackageDesc> assetPackageDescList = new List<DAssetPackageDesc>();

        for (int g = 0; g < GROUP_NUM; ++g)
        {
            List<AssetPackageDesc> curPackageGroup = packageGroupList[g];
            if (null != curPackageGroup)
            {
                AssetBundleBuild[] bundleBuildGrp = new AssetBundleBuild[curPackageGroup.Count];
                for (int i = 0; i < bundleBuildGrp.Length; ++i)
                {
                    EditorUtility.DisplayProgressBar("正在打包", "正在准备打包...", ((float)i / bundleBuildGrp.Length));
                    AssetPackageDesc curPackageDesc = curPackageGroup[i];

                    bundleBuildGrp[i].assetBundleName = curPackageDesc.m_PackageName;
                    bundleBuildGrp[i].assetBundleVariant = "";

                    bundleBuildGrp[i].assetNames = new string[curPackageDesc.m_PackageAsset.Count];

                    for (int j = 0; j < curPackageDesc.m_PackageAsset.Count; ++j)
                    {
                        if (0 != ((uint)DAssetPackageFlag.UsingGUIDName & curPackageDesc.m_PackageFlag))
                            bundleBuildGrp[i].assetNames[j] = m_TempAssetPath + curPackageDesc.m_PackageAsset[j].m_AssetGUID + PathUtil.GetExtension(curPackageDesc.m_PackageAsset[j].m_AssetPath);
                        else
                            bundleBuildGrp[i].assetNames[j] = curPackageDesc.m_PackageAsset[j].m_AssetPath;
                    }
                }

                EditorUtility.ClearProgressBar();

                AssetBundleManifest assetManifestGrp = BuildPipeline.BuildAssetBundles(m_OutputBundlePath, bundleBuildGrp, BuildAssetBundleOptions.ChunkBasedCompression, platform);

                for (int i = 0; i < bundleBuildGrp.Length; ++i)
                {
                    DAssetPackageDesc assetPackageDescOut = new DAssetPackageDesc();

                    if (null == bundleBuildGrp[i].assetBundleName)
                        continue;

                    if (curPackageGroup[i].m_IncludeDepend)
                    {
                        string[] depent = assetManifestGrp.GetAllDependencies(bundleBuildGrp[i].assetBundleName);
                        if (null != depent && depent.Length > 0)
                        {
                            assetPackageDescOut.packageAutoDepend = new string[depent.Length];
                            for (int j = 0; j < depent.Length; ++j)
                                assetPackageDescOut.packageAutoDepend[j] = depent[j];
                        }
                    }

                    assetPackageDescOut.packageMD5 = FileUtil.GetFileMD5(m_OutputBundlePath + bundleBuildGrp[i].assetBundleName);
                    assetPackageDescOut.packageName = curPackageGroup[i].m_PackageName.ToLower();
                    assetPackageDescOut.packagePath = m_StreamingBundlePath;
                    assetPackageDescOut.packageKey = assetPackageDescOut.packagePath.ToLower() + assetPackageDescOut.packageName.ToLower();
                    assetPackageDescOut.packageVer = 0;

                    assetPackageDescOut.packageFlag = curPackageGroup[i].m_PackageFlag;
                    assetPackageDescOut.packageDependency = curPackageGroup[i].m_DependPackage.ToArray();

                    List<DPackAssetDesc> assetList = new List<DPackAssetDesc>();
                    for (int j = 0; j < curPackageGroup[i].m_PackageAsset.Count; ++j)
                        assetList.Add(new DPackAssetDesc(PathUtil.EraseExtension(curPackageGroup[i].m_PackageAsset[j].m_AssetPath.ToLower()).Replace("assets/resources/", null).Replace('\\', '/'), curPackageGroup[i].m_PackageAsset[j].m_AssetGUID));

                    assetPackageDescOut.packageAsset = assetList.ToArray();
                    assetPackageDescList.Add(assetPackageDescOut);
                }
            }
        }

        DAssetPackageDesc[] assetPackageDescListOut = assetPackageDescList.ToArray();
        assetPackageDescListOut = _UnifyBundle(assetPackageDescListOut);

        /// 增量依赖
        DAssetPackageDependency asset = AssetDatabase.LoadAssetAtPath<DAssetPackageDependency>("Assets/Resources/Base/Version/PackageInfo.asset");
        if (null != asset && asset.packageDescArray.Length > 0)
            assetPackageDescListOut = _MergeBundle(asset.packageDescArray, assetPackageDescListOut);
        /// 增量依赖
        /// 

        Dictionary<string, int> assetMap = new Dictionary<string, int>();
        for (int i = 0, icnt = assetPackageDescListOut.Length; i < icnt; ++i)
        {
            DAssetPackageDesc curAssetPackageDesc = assetPackageDescListOut[i];

            if (!assetMap.ContainsKey(curAssetPackageDesc.packageName))
                assetMap.Add(curAssetPackageDesc.packageName, i);
            else
                Debug.LogWarningFormat("Same bundle key \"{0}\"!", curAssetPackageDesc.packageName);
        }

        Logger.LogAsset("Initialize asset packages!");
        for (int i = 0; i < assetPackageDescListOut.Length; ++i)
        {
            DAssetPackageDesc curAssetPackageDesc = assetPackageDescListOut[i];
            List<int> assetPackageList = new List<int>();
            for (int j = 0; j < curAssetPackageDesc.packageAutoDepend.Length; ++j)
            {
                int curPackageIdx = 0;
                if (assetMap.TryGetValue(curAssetPackageDesc.packageAutoDepend[j], out curPackageIdx))
                    assetPackageList.Add(curPackageIdx);
            }

            curAssetPackageDesc.packageAutoDependIdx = assetPackageList.ToArray();
        }

        if (!Directory.Exists("Assets/Resources/Base/Version"))
            Directory.CreateDirectory("Assets/Resources/Base/Version");

        if (null == asset)
        {
            asset = ScriptableObject.CreateInstance<DAssetPackageDependency>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/Base/Version/PackageInfo.asset");
        }

        /// 建立反查表
        asset.assetDescPackageMap.Clear();
        for (int i = 0, icnt = assetPackageDescListOut.Length; i < icnt; ++i)
        {
            for (int j = 0, jcnt = assetPackageDescListOut[i].packageAsset.Length; j < jcnt; ++j)
            {
                DPackAssetDesc curPackageAsset = assetPackageDescListOut[i].packageAsset[j];

                DAssetPackageMapDesc assetPackageMapDesc = new DAssetPackageMapDesc();
                assetPackageMapDesc.assetPathKey = curPackageAsset.packageAsset;
                assetPackageMapDesc.assetPackageGUID = curPackageAsset.packageGUID;
                assetPackageMapDesc.packageDescIdx = i;
                asset.assetDescPackageMap.Add(assetPackageMapDesc);
            }
        }
        asset.assetDescPackageMap.Sort(DAssetPackageDependency.Comparison);

        //AssetDatabase.CreateAsset(asset, "Assets/Resources/Base/Version/PackageInfo_" + date + ".asset");
        asset.packageDescArray = assetPackageDescListOut;
        asset.patchVersion = patchVer;

        _DumpPackageInfoAsTxt(assetPackageDescListOut);
        _DumpPackageInfoAsCsv(asset);

        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();

        AssetBundleBuild[] bundlePackageInfoBuild = new AssetBundleBuild[1];
        bundlePackageInfoBuild[0].assetBundleName = "PackageInfo.pak";
        bundlePackageInfoBuild[0].assetNames = new string[1] { "Assets/Resources/Base/Version/PackageInfo.asset" };
        bundlePackageInfoBuild[0].assetBundleVariant = "";

        AssetBundleManifest assetManifestPackageInfo = BuildPipeline.BuildAssetBundles(m_OutputBundlePath, bundlePackageInfoBuild, BuildAssetBundleOptions.None, platform);
    }

    static protected void _DumpPackageInfoAsTxt(DAssetPackageDesc[] packageDescArray)
    {
        string date = DateTime.Now.ToString("yyyy_MM_dd");
        FileStream streamW = new FileStream(m_OutputBundlePath + "PackageInfoDump_" + date + ".txt", FileMode.Create, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));
        for (int i = 0; i < packageDescArray.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在写入打包信息...", ((float)i / packageDescArray.Length));
            sw.WriteLine(packageDescArray[i].packageName + "(" + packageDescArray[i].packageFlag.ToString() + ")");
            for (int j = 0; j < packageDescArray[i].packageAsset.Length; ++j)
                sw.WriteLine("- Asset:" + packageDescArray[i].packageAsset[j].packageAsset + "(" + packageDescArray[i].packageAsset[j].packageGUID + ")");

            for (int j = 0; j < packageDescArray[i].packageDependency.Length; ++j)
                sw.WriteLine("- Dependency:" + packageDescArray[i].packageDependency[j]);

            for (int j = 0; j < packageDescArray[i].packageAutoDepend.Length; ++j)
                sw.WriteLine("- Auto Depend:" + packageDescArray[i].packageAutoDepend[j]);
        }
        streamW.Flush();
        sw.Close();
        streamW.Close();
        EditorUtility.ClearProgressBar();
    }

    static protected DAssetPackageDesc[] _UnifyBundle(DAssetPackageDesc[] originDesc)
    {
        List<DAssetPackageDesc> outPackageDesc = new List<DAssetPackageDesc>();
        for (int i = 0; i < originDesc.Length; ++i)
        {
            bool bNewBundle = true;
            DAssetPackageDesc curDesc = originDesc[i];
            for (int j = 0; j < outPackageDesc.Count; ++j)
            {
                if (outPackageDesc[j].packageName.Equals(curDesc.packageName,StringComparison.OrdinalIgnoreCase))
                {
                    Logger.LogFormat("Two bundle has the same bundle name \"{0}\"", curDesc.packageName);

                    List<string> depends = new List<string>();
                    depends.AddRange(curDesc.packageDependency);
                    for (int k = 0, kcnt = outPackageDesc[j].packageDependency.Length; k < kcnt; ++k)
                    {
                        if (!depends.Contains(outPackageDesc[j].packageDependency[k]))
                            depends.Add(outPackageDesc[j].packageDependency[k]);
                    }
                    outPackageDesc[j].packageDependency = depends.ToArray();

                    List<string> dependAlts = new List<string>();
                    dependAlts.AddRange(curDesc.packageAutoDepend);
                    for (int k = 0, kcnt = outPackageDesc[j].packageAutoDepend.Length; k < kcnt; ++k)
                    {
                        if (!dependAlts.Contains(outPackageDesc[j].packageAutoDepend[k]))
                            dependAlts.Add(outPackageDesc[j].packageAutoDepend[k]);
                    }
                    outPackageDesc[j].packageAutoDepend = dependAlts.ToArray();

                    List<DPackAssetDesc> assets = new List<DPackAssetDesc>();
                    assets.AddRange(curDesc.packageAsset);
                    for (int k = 0, kcnt = outPackageDesc[j].packageAsset.Length; k < kcnt; ++k)
                    {
                        if (!assets.Contains(outPackageDesc[j].packageAsset[k]))
                            assets.Add(outPackageDesc[j].packageAsset[k]);
                    }
                    outPackageDesc[j].packageAsset = assets.ToArray();

                    bNewBundle = false;
                    break;
                }
            }

            if(bNewBundle)
                outPackageDesc.Add(curDesc);
        }

        return outPackageDesc.ToArray();
    }

    static protected DAssetPackageDesc[] _MergeBundle(DAssetPackageDesc[] elderDesc, DAssetPackageDesc[] newerDesc)
    {
        List<DAssetPackageDesc> outPackageDesc = new List<DAssetPackageDesc>();
        outPackageDesc.AddRange(newerDesc);

        for (int i = 0; i < elderDesc.Length; ++i)
        {
            DAssetPackageDesc newDesc = new DAssetPackageDesc();

            DAssetPackageDesc curDesc = elderDesc[i];
            for (int j = 0; j < outPackageDesc.Count; ++j)
            {
                DAssetPackageDesc curNewerDesc = outPackageDesc[j];

                newDesc.packageName = curNewerDesc.packageName;
                newDesc.packagePath = curNewerDesc.packagePath;
                newDesc.packageKey = curNewerDesc.packageKey;
                newDesc.packageMD5 = curNewerDesc.packageMD5;
                newDesc.packageFlag = curNewerDesc.packageFlag;
                newDesc.packageVer = curNewerDesc.packageVer;
                newDesc.assetPackage = null;

                if (curNewerDesc.packageName == curDesc.packageName)
                {
                    Logger.LogFormat("Two bundle has the same bundle name \"{0}\"", curDesc.packageName);

                    List<string> depends = new List<string>();
                    depends.AddRange(curDesc.packageDependency);
                    for (int k = 0,kcnt = curNewerDesc.packageDependency.Length;k<kcnt;++k)
                    {
                        if (!depends.Contains(curNewerDesc.packageDependency[k]))
                            depends.Add(curNewerDesc.packageDependency[k]);
                    }
                    newDesc.packageDependency = depends.ToArray();

                    List<string> dependAlts = new List<string>();
                    dependAlts.AddRange(curDesc.packageAutoDepend);
                    for (int k = 0, kcnt = curNewerDesc.packageAutoDepend.Length; k < kcnt; ++k)
                    {
                        if (!dependAlts.Contains(curNewerDesc.packageAutoDepend[k]))
                            dependAlts.Add(curNewerDesc.packageAutoDepend[k]);
                    }
                    newDesc.packageAutoDepend = dependAlts.ToArray();

                    List<DPackAssetDesc> assets = new List<DPackAssetDesc>();
                    assets.AddRange(curDesc.packageAsset);
                    for (int k = 0, kcnt = curNewerDesc.packageAsset.Length; k < kcnt; ++k)
                    {
                        if (!assets.Contains(curNewerDesc.packageAsset[k]))
                            assets.Add(curNewerDesc.packageAsset[k]);
                    }
                    newDesc.packageAsset = assets.ToArray();

                    outPackageDesc[j] = newDesc;
                    break;
                }
            }

            outPackageDesc.Add(newDesc);
        }

        return outPackageDesc.ToArray();
    }

    public class PackAssetDesc
    {
        public PackAssetDesc(string assetPath)
        {
            m_AssetPath = assetPath.Replace('\\', '/');
            m_AssetGUID = AssetDatabase.AssetPathToGUID(m_AssetPath);
        }

        public string m_AssetPath;
        public string m_AssetGUID;
    }

    public class AssetPackageDesc
    {
        public string m_PackageName;
        public long m_OriginSize = 0;
        public uint m_PackageFlag = (uint)DAssetPackageFlag.None;
        public bool m_IncludeDepend = true;
        public EAssetType m_AssetType = EAssetType.All;
        public List<PackAssetDesc> m_PackageAsset = new List<PackAssetDesc>();
        public List<string> m_DependPackage = new List<string>();
        public List<string> m_DependAsset = new List<string>();
    }
    static protected AssetPackageDesc _PackDirectory(EAssetType assetType, string dstPackDirPath, string packagePrefix, string[] packExtFilter, string[] dependExcluder, uint packageFlag, SearchOption searchOption, bool bIncDepend)
    {
        dstPackDirPath = dstPackDirPath.Replace('\\', '/');

        AssetPackageDesc newPackageDesc = new AssetPackageDesc();

        string[] assetPathList = Directory.GetFiles(dstPackDirPath, "*.*", searchOption);
        for (int i = 0; i < assetPathList.Length; ++i)
        {
            string curPath = assetPathList[i];
            curPath = curPath.Replace('\\', '/');

            for (int j = 0; j < packExtFilter.Length; ++j)
            {
                if (packExtFilter[j] == PathUtil.GetExtension(curPath).ToLower())
                {
                    newPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curPath));
                    newPackageDesc.m_OriginSize += File.ReadAllBytes(curPath).Length;
                    break;
                }
            }

            if (bIncDepend)
            {
                string[] dependency = _GatherDependency(curPath, dependExcluder);
                if (null != dependency)
                {
                    for (int j = 0; j < dependency.Length; ++j)
                    {
                        dependency[j] = dependency[j].Replace('\\', '/');

                        //for(int k = 0,kcnt = packExtFilter.Length;k<kcnt;++k)
                        //    packExtFilter[k]
                        //
                        //if (dependency[j].Contains(dstPackDirPath))
                        //    continue;

                        if (!newPackageDesc.m_DependAsset.Contains(dependency[j]))
                            newPackageDesc.m_DependAsset.Add(dependency[j]);
                    }
                }
            }
        }

        if ('/' == dstPackDirPath[dstPackDirPath.Length - 1])
            dstPackDirPath = dstPackDirPath.Remove(dstPackDirPath.Length - 1);

        string packName = "";
        string[] subPathSplit = dstPackDirPath.Split('/');
        for (int i = 0, icnt = subPathSplit.Length; i < icnt; ++i)
        {
            if (subPathSplit[i] == "Assets" || subPathSplit[i] == "Resources")
                continue;

            packName += subPathSplit[i];
            if (i + 1 < icnt)
                packName += '_';
        }

        newPackageDesc.m_PackageName = packName + m_OutputBundleExt;
        newPackageDesc.m_PackageFlag = packageFlag;
        newPackageDesc.m_IncludeDepend = bIncDepend;
        newPackageDesc.m_AssetType = assetType;
        if (newPackageDesc.m_PackageAsset.Count > 0)
            return newPackageDesc;
        else
            return null;
    }

    static protected void _PackAssetByDirectoryLevel(EAssetType assetType, string assetPath, string packagePrefix, List<AssetPackageDesc> descList, string[] packExtFilter, string[] dependExcluder, string[] directoryExcluder, uint packageFlag, bool bIncDepend, int packLevel, int curLevel)
    {
        assetPath = assetPath.Replace('\\', '/');
        if (!Directory.Exists(assetPath))
            return;

        AssetPackageDesc newPackageDesc = null;
        string[] topDirLevel = Directory.GetDirectories(assetPath, "*.*", SearchOption.TopDirectoryOnly);
        if (null != topDirLevel)
        {
            for (int j = 0; j < topDirLevel.Length; ++j)
            {
                string curPath = topDirLevel[j].Replace('\\', '/');
                if ('/' != curPath[curPath.Length - 1])
                    curPath += '/';

                bool bIsSkip = false;
                if (null != directoryExcluder)
                {
                    for (int k = 0, kcnt = directoryExcluder.Length; k < kcnt; ++k)
                    {
                        if (curPath.Contains(directoryExcluder[k]))
                        {
                            bIsSkip = true;
                            break;
                        }
                    }
                }
                if (bIsSkip)
                    continue;

                if (packLevel == curLevel + 1)
                {
                    newPackageDesc = _PackDirectory(assetType,curPath, packagePrefix, packExtFilter, dependExcluder, packageFlag, SearchOption.AllDirectories, bIncDepend);
                    if (null != newPackageDesc)
                        descList.Add(newPackageDesc);
                }
                else
                {
                    _PackAssetByDirectoryLevel(assetType,curPath, packagePrefix, descList, packExtFilter, dependExcluder, directoryExcluder, packageFlag, bIncDepend, packLevel, curLevel + 1);
                }
            }
        }

        newPackageDesc = _PackDirectory(assetType,assetPath, packagePrefix, packExtFilter, dependExcluder, packageFlag, SearchOption.TopDirectoryOnly, bIncDepend);
        if (null != newPackageDesc)
            descList.Add(newPackageDesc);
    }

    static protected void _PackAssetByFileSize(EAssetType assetType, string dstPackDirPath, string packagePrefix, List<AssetPackageDesc> descList, string[] packExtFilter, string[] dependExcluder, string[] directoryExcluder, uint packageFlag, bool bIncDepend)
    {
        
    }

    static protected void _PackAssetByFileName(EAssetType assetType, string dstPackDirPath, string packagePrefix, List<AssetPackageDesc> descList, string[] packExtFilter, string[] dependExcluder, string[] directoryExcluder, uint packageFlag, bool bIncDepend,int splitSegNum = 37)
    {
        dstPackDirPath = dstPackDirPath.Replace('\\', '/');
        if (!Directory.Exists(dstPackDirPath))
            return;
        string[] assetPathList = Directory.GetFiles(dstPackDirPath, "*.*", SearchOption.AllDirectories);

        if(null != assetPathList && assetPathList.Length > 0)
        {
            AssetPackageDesc[] assetPackageDescArray = new AssetPackageDesc[splitSegNum];

            for (int i = 0; i < assetPackageDescArray.Length; ++i)
            {
                assetPackageDescArray[i] = new AssetPackageDesc();
                AssetPackageDesc curPackageDesc = assetPackageDescArray[i];


                if ('/' == dstPackDirPath[dstPackDirPath.Length - 1])
                    dstPackDirPath = dstPackDirPath.Remove(dstPackDirPath.Length - 1);

                string packName = "";
                string[] subPathSplit = dstPackDirPath.Split('/');
                for (int j = 0, jcnt = subPathSplit.Length; j <jcnt; ++j)
                {
                    if (subPathSplit[j] == "Assets" || subPathSplit[j] == "Resources")
                        continue;

                    packName += subPathSplit[j];

                    if (j + 1 < jcnt)
                        packName += '_';
                }

                curPackageDesc.m_PackageName = packName + "_FileNamePack_" + splitSegNum.ToString() + '_' + i.ToString() + m_OutputBundleExt;
                curPackageDesc.m_PackageFlag = packageFlag;
                curPackageDesc.m_IncludeDepend = bIncDepend;
                curPackageDesc.m_AssetType = assetType;
            }

            for (int i = 0; i < assetPathList.Length; ++i)
            {
                string curPath = assetPathList[i];
                curPath = curPath.Replace('\\', '/');

                string resName = Path.GetFileName(curPath);
                int charCnt = 0;
                for(int j = 0,jcnt = resName.Length;j<jcnt;++j)
                    charCnt += (int)resName[j];
                int idx = charCnt % splitSegNum;

                bool bIsSkip = false;
                if (null != directoryExcluder)
                {
                    for (int j = 0, jcnt = directoryExcluder.Length; j < jcnt; ++j)
                    {
                        if (curPath.Contains(directoryExcluder[j]))
                        {
                            bIsSkip = true;
                            break;
                        }
                    }
                }
                if (bIsSkip)
                    continue;

                AssetPackageDesc dstPackageDesc = assetPackageDescArray[idx];

                for (int j = 0; j < packExtFilter.Length; ++j)
                {
                    if (packExtFilter[j] == PathUtil.GetExtension(curPath).ToLower())
                    {
                        dstPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curPath));
                        dstPackageDesc.m_OriginSize += File.ReadAllBytes(curPath).Length;
                        break;
                    }
                }

                if (dstPackageDesc.m_PackageAsset.Count == 0)
                    continue;

                if (bIncDepend)
                {
                    string[] depend = _GatherDependency(curPath, dependExcluder);
                    if (null != depend)
                    {
                        for (int j = 0; j < depend.Length; ++j)
                        {
                            depend[j] = depend[j].Replace('\\', '/');
                            if (depend[j].Contains(curPath))
                                continue;

                            if (!dstPackageDesc.m_DependAsset.Contains(depend[j]))
                                dstPackageDesc.m_DependAsset.Add(depend[j]);
                        }
                    }
                }
            }

            for (int i = 0; i < assetPackageDescArray.Length; ++i)
            {
                if (assetPackageDescArray[i].m_PackageAsset.Count > 0)
                    descList.Add(assetPackageDescArray[i]);
            }
        }
    }

    static protected void _PackAssetByFileType(EAssetType assetType, string dstPackDirPath, string packagePrefix, List<AssetPackageDesc> descList, string[] packExtFilter, string[] dependExcluder, string[] directoryExcluder, uint packageFlag, bool bIncDepend)
    {
        dstPackDirPath = dstPackDirPath.Replace('\\', '/');
        if (!Directory.Exists(dstPackDirPath))
            return;

        string[] assetPathList = Directory.GetFiles(dstPackDirPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < assetPathList.Length; ++i)
        {
            string curPath = assetPathList[i];
            curPath = curPath.Replace('\\', '/');

            bool bIsSkip = false;
            if(null != directoryExcluder)
            {
                for (int j = 0, jcnt = directoryExcluder.Length; j < jcnt; ++j)
                {
                    if (curPath.Contains(directoryExcluder[j]))
                    {
                        bIsSkip = true;
                        break;
                    }
                }
            }
            if(bIsSkip)
                continue;

            if (PathUtil.GetExtension(curPath).ToLower() == ".meta")
                continue;

            AssetPackageDesc newPackageDesc = new AssetPackageDesc();

            for (int j = 0; j < packExtFilter.Length; ++j)
            {
                if (packExtFilter[j] == PathUtil.GetExtension(curPath).ToLower())
                {
                    newPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curPath));
                    newPackageDesc.m_OriginSize += File.ReadAllBytes(curPath).Length;
                    break;
                }
            }

            if (newPackageDesc.m_PackageAsset.Count == 0)
                continue;

            if (bIncDepend)
            {
                string[] depend = _GatherDependency(curPath, dependExcluder);
                if (null != depend)
                {
                    for (int j = 0; j < depend.Length; ++j)
                    {
                        depend[j] = depend[j].Replace('\\', '/');
                        if (depend[j].Contains(curPath))
                            continue;

                        if (!newPackageDesc.m_DependAsset.Contains(depend[j]))
                            newPackageDesc.m_DependAsset.Add(depend[j]);
                    }
                }
            }

            if ('/' == dstPackDirPath[dstPackDirPath.Length - 1])
                dstPackDirPath = dstPackDirPath.Remove(dstPackDirPath.Length - 1);

            string packName = "";
            string[] subPathSplit = dstPackDirPath.Split('/');
            for (int j = 0, jcnt = subPathSplit.Length; j < jcnt; ++j)
            {
                if(subPathSplit[j] == "Assets" || subPathSplit[j] == "Resources")
                    continue;

                packName += subPathSplit[j];
                packName += '_';
            }
            packName += Path.GetFileNameWithoutExtension(curPath);
            packName += '_';
            packName += Path.GetExtension(curPath).Replace(".",null);

            newPackageDesc.m_PackageName = packName + m_OutputBundleExt;
            newPackageDesc.m_PackageFlag = packageFlag;
            newPackageDesc.m_AssetType = assetType;
            newPackageDesc.m_IncludeDepend = bIncDepend;

            descList.Add(newPackageDesc);
        }
    }

    static protected void _PackAssetByRootDirectory(EAssetType assetType, string dstPackDirPath, string packagePrefix, List<AssetPackageDesc> descList, string[] packExtFilter, string[] dependExcluder, string[] directoryExcluder, uint packageFlag, bool bIncDepend)
    {
        dstPackDirPath = dstPackDirPath.Replace('\\', '/');
        if (!Directory.Exists(dstPackDirPath))
            return;

        AssetPackageDesc newPackageDesc = new AssetPackageDesc();
        string[] assetPathList = Directory.GetFiles(dstPackDirPath, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < assetPathList.Length; ++i)
        {
            string curPath = assetPathList[i];
            curPath = curPath.Replace('\\', '/');

            bool bIsSkip = false;
            if (null != directoryExcluder)
            {
                for (int j = 0, jcnt = directoryExcluder.Length; j < jcnt; ++j)
                {
                    if (curPath.Contains(directoryExcluder[j]))
                    {
                        bIsSkip = true;
                        break;
                    }
                }
            }
            if (bIsSkip)
                continue;


            for (int j = 0; j < packExtFilter.Length; ++j)
            {
                if (packExtFilter[j] == PathUtil.GetExtension(curPath).ToLower())
                {
                    newPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curPath));
                    newPackageDesc.m_OriginSize += File.ReadAllBytes(curPath).Length;
                    break;
                }
            }

            if (newPackageDesc.m_PackageAsset.Count == 0)
                continue;

            if (bIncDepend)
            {
                string[] dependency = _GatherDependency(curPath, dependExcluder);
                if (null != dependency)
                {
                    for (int j = 0; j < dependency.Length; ++j)
                    {
                        dependency[j] = dependency[j].Replace('\\', '/');
                        if (dependency[j].Contains(dstPackDirPath))
                            continue;

                        if (!newPackageDesc.m_DependAsset.Contains(dependency[j]))
                            newPackageDesc.m_DependAsset.Add(dependency[j]);
                    }
                }
            }
        }

        if ('/' == dstPackDirPath[dstPackDirPath.Length - 1])
            dstPackDirPath = dstPackDirPath.Remove(dstPackDirPath.Length - 1);

        string packName = "";
        string[] subPathSplit = dstPackDirPath.Split('/');
        for(int i = 0,icnt = subPathSplit.Length;i<icnt;++i)
        {
            if (subPathSplit[i] == "Assets" || subPathSplit[i] == "Resources")
                continue;

            packName += subPathSplit[i];

            if(i + 1 < icnt)
                packName += '_';
        }

        newPackageDesc.m_PackageName = packName + m_OutputBundleExt;
        newPackageDesc.m_PackageFlag = packageFlag;
        newPackageDesc.m_AssetType = assetType;
        newPackageDesc.m_IncludeDepend = bIncDepend;

        descList.Add(newPackageDesc);
    }

    static public void PackPolicyAllInOne(EAssetType assetType,string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByRootDirectory(assetType,assetPath[i], "", descList, new string[] { ".mat", ".shader", ".png", ".asset", ".json", ".fbx", ".prefab", ".tga", ".anim" ,".jpg", ".ttf", ".xml", ".anim", ".controller", ".fnt", ".fontsettings",".ogg",".wav" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        }
    }

    static public void PackPolicyAllInOneData(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByRootDirectory(assetType,assetPath[i], "", descList, new string[] { ".asset", ".xml", ".json", ".prefab" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.UsingGUIDName, false);
        }
    }

    static public void PackPolicyActor(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        //for (int i = 0; i < assetPath.Length; ++i)
        //{
        //    _PackAssetByDirectoryLevel(assetType,assetPath[i], "actor", descList, new string[] { ".mat" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, 2, 0);
        //}

        for (int i = 0; i < assetPath.Length; ++i)
        {
            //_PackAssetByFileType(assetPath[i], "actor", descList, new string[] { ".asset", ".fbx", ".tga", ".prefab" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
            _PackAssetByDirectoryLevel(assetType,assetPath[i], "actor", descList, new string[] { ".tga", ".asset", ".fbx", ".prefab"}, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, 2, 0);
        }
    }

    static public void PackPolicyActorWeapon(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType,assetPath[i], "actor", descList, new string[] { ".prefab", ".fbx" , ".tga",}, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, packLv, 0);
        }
    }

    static public void PackPolicyActorAnimClip(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0,icnt = assetPath.Length; i < icnt; ++i)
        {
            string dstPackDirPath = assetPath[i].Replace('\\', '/');
            string[] assetPathList = Directory.GetFiles(dstPackDirPath, "*.anim", SearchOption.AllDirectories);
            for (int j = 0;j < assetPathList.Length; ++j)
            {
                string curPath = assetPathList[j];
                curPath = curPath.Replace('\\', '/');

                if (PathUtil.GetExtension(curPath).ToLower() == ".meta")
                    continue;

                AssetPackageDesc newPackageDesc = new AssetPackageDesc();
                newPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curPath));
                newPackageDesc.m_OriginSize += File.ReadAllBytes(curPath).Length;

                if ('/' == dstPackDirPath[dstPackDirPath.Length - 1])
                    dstPackDirPath = dstPackDirPath.Remove(dstPackDirPath.Length - 1);

                string packName = "";
                packName += Path.GetFileNameWithoutExtension(curPath);
                packName += '_';
                packName += AssetDatabase.AssetPathToGUID(curPath);

                newPackageDesc.m_PackageName = packName + m_OutputBundleExt;
                newPackageDesc.m_PackageFlag = (uint)DAssetPackageFlag.None;
                newPackageDesc.m_AssetType = assetType;
                newPackageDesc.m_IncludeDepend = true;

                descList.Add(newPackageDesc);
            }
        }
    }

    static public void PackPolicyActorAnimClipEx(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        Dictionary<string, List<string>> dictAnim = new Dictionary<string, List<string>>();

        for (int i = 0, icnt = assetPath.Length; i < icnt; ++i)
        {
            string dstPackDirPath = assetPath[i].Replace('\\', '/');
            string[] assetPathList = Directory.GetFiles(dstPackDirPath, "*.anim", SearchOption.AllDirectories);
            for (int j = 0; j < assetPathList.Length; ++j)
            {
                string curPath = assetPathList[j];
                curPath = curPath.Replace('\\', '/');

                if (PathUtil.GetExtension(curPath).ToLower() == ".meta")
                    continue;

                List<string> packLst = null;
                string key = Path.GetDirectoryName(curPath);
                if (!dictAnim.TryGetValue(key, out packLst))
                {
                    packLst = new List<string>();
                    dictAnim.Add(key, packLst);
                }

                bool bSameKey = false;
                for(int k=0,kcnt = packLst.Count;k<kcnt;++k)
                {
                    if(packLst[k].Equals(curPath, StringComparison.OrdinalIgnoreCase))
                    {
                        bSameKey = true;
                        break;
                    }
                }

                if (!bSameKey)
                    packLst.Add(curPath);
            }
        }

        Dictionary<string, List<string>>.Enumerator it = dictAnim.GetEnumerator();
        while(it.MoveNext())
        {
            AssetPackageDesc newPackageDesc = new AssetPackageDesc();
            List<string> fileList = it.Current.Value;
            for(int i = 0,icnt = fileList.Count;i<icnt;++i)
            {
                newPackageDesc.m_PackageAsset.Add(new PackAssetDesc(fileList[i]));
                newPackageDesc.m_OriginSize += File.ReadAllBytes(fileList[i]).Length;
            }

            string dstPackDirPath = it.Current.Key.Replace('\\', '/');

            if ('/' == dstPackDirPath[dstPackDirPath.Length - 1])
                dstPackDirPath = dstPackDirPath.Remove(dstPackDirPath.Length - 1);

            string packName = "";
            string[] subPathSplit = dstPackDirPath.Split('/');
            for (int i = 0, icnt = subPathSplit.Length; i < icnt; ++i)
            {
                if (subPathSplit[i] == "Assets" || subPathSplit[i] == "Resources")
                    continue;

                packName += subPathSplit[i];
                if (i + 1 < icnt)
                    packName += '_';
            }

            newPackageDesc.m_PackageName = packName + m_OutputBundleExt;
            newPackageDesc.m_PackageFlag = (uint)DAssetPackageFlag.None;
            newPackageDesc.m_AssetType = assetType;
            newPackageDesc.m_IncludeDepend = true;

            descList.Add(newPackageDesc);
        }
    }

    static public void PackPolicyShader(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        // string[] shaderList = Directory.GetFiles("Assets/Resources/Shader", "*.shader", SearchOption.AllDirectories);
        // ArrayList shaderResList = new ArrayList();
        // 
        // for (int i = 0; i < shaderList.Length; ++i)
        //     shaderResList.Add(shaderList[i].Replace("Assets/Resources/", "").Replace('\\', '/'));
        // 
        // FileStream streamW = new FileStream(Path.Combine("Assets/Resources/Shader", "ShaderList.json"), FileMode.Create, FileAccess.Write, FileShare.Write);
        // StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));
        // 
        // sw.Write(MiniJSON.jsonEncode(shaderResList));
        // 
        // streamW.Flush();
        // 
        // sw.Close();
        // streamW.Close();


        string[] shaderList = Directory.GetFiles("Assets/Resources/Shader", "*.shader", SearchOption.AllDirectories);
        Hashtable shaderResList = new Hashtable();

        for (int i = 0; i < shaderList.Length; ++i)
        {
            Shader shader = AssetDatabase.LoadAssetAtPath(shaderList[i], typeof(Shader)) as Shader;
            if(null != shader)
                shaderResList.Add(shader.name, shaderList[i].Replace("Assets/Resources/", "").Replace('\\', '/'));
        }

        FileStream streamW = new FileStream(Path.Combine("Assets/Resources/Shader", "ShaderList.json"), FileMode.Create, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(streamW/*, Encoding.GetEncoding(936)*/);

        sw.Write(MiniJSON.jsonEncode(shaderResList));
        streamW.Flush();

        sw.Close();
        streamW.Close();


        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByRootDirectory(assetType,assetPath[i], "shader", descList, new string[] { ".shader", ".mat", ".png", ".asset", ".json", ".shadervariants" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        }
    }

    static public void PackPolicyEnvironment(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByFileType(assetType,assetPath[i], "environment", descList, new string[] { ".prefab", ".json", ".xml" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        }
    }

    static public void PackPolicyData(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType,assetPath[i], "data", descList, new string[] { ".bytes", ".asset", ".xml", ".json", ".prefab", ".txt"}, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.UsingGUIDName, false, packLv, 0);
            //_PackAssetByRootDirectory(assetPath[i], "", descList, new string[] { ".asset", ".xml", ".json", ".prefab" }, new string[] { ".cs", ".dll" }, (uint)DAssetPackageFlag.UsingGUIDName, false);
        }
    }

    static public void PackPolicyDataInOne(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByRootDirectory(assetType, assetPath[i], "", descList, new string[] { ".bytes", ".asset", ".xml", ".json", ".prefab", ".txt" }, new string[] { ".cs", ".dll" },exclusivePath, (uint)DAssetPackageFlag.UsingGUIDName, false);
        }
    }


    static public void PackPolicyScene(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        //for (int i = 0; i < assetPath.Length; ++i)
        //{
        //    _PackAssetByFileType(assetType,assetPath[i], "scene", descList, new string[] { ".prefab", ".asset",  ".ogg" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        //}
        //
        //for (int i = 0; i < assetPath.Length; ++i)
        //{
        //    _PackAssetByDirectoryLevel(assetType,assetPath[i], "scene", descList, new string[] { ".fbx"}, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, 2, 0);
        //}
        //
        //for (int i = 0; i < assetPath.Length; ++i)
        //{
        //    _PackAssetByDirectoryLevel(assetType,assetPath[i], "scene", descList, new string[] { ".tga", ".png", }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, 1, 0);
        //}

        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType, assetPath[i], "scene", descList, new string[] { ".prefab", ".asset", ".anim" , ".mat", ".spriteatlas" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, packLv, 0);
        }
    }

    public class ResFirstDesc
    {
        public string m_PrimeResPath = null;
        public List<string> m_SecondaryResPathList = new List<string>();
        public AssetPackageDesc m_AssetPackageDesc = new AssetPackageDesc();
    }

    static public void PackPolicySceneEx(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        string[] assetGuid = null;
        ResFirstDesc nullTexDesc = new ResFirstDesc();
        List <ResFirstDesc> resMap = new List<ResFirstDesc>();
        for (int m = 0, mcnt = assetPath.Length; m < mcnt; ++m)
        {
            assetPath[m] = assetPath[m].Replace('\\', '/');
            assetGuid = AssetDatabase.FindAssets("t:texture", new string[] { assetPath[m] });

            for (int i = 0, icnt = assetGuid.Length; i < icnt; ++i)
            {
                string curResPath = AssetDatabase.GUIDToAssetPath(assetGuid[i]);
                ResFirstDesc newResDesc = new ResFirstDesc();
                newResDesc.m_PrimeResPath = curResPath;

                string packName = "";
                string[] subPathSplit = Path.GetDirectoryName(assetPath[m]).Split('/');
                for (int j = 0, jcnt = subPathSplit.Length; j < jcnt; ++j)
                {
                    if (subPathSplit[j] == "Assets" || subPathSplit[j] == "Resources")
                        continue;

                    packName += subPathSplit[j];
                    packName += '_';
                }

                packName += Path.GetFileNameWithoutExtension(curResPath);
                packName += '_';
                packName += Path.GetExtension(curResPath).Replace(".", null);

                newResDesc.m_AssetPackageDesc.m_PackageName = packName + m_OutputBundleExt;
                newResDesc.m_AssetPackageDesc.m_PackageFlag = (uint)DAssetPackageFlag.None;
                newResDesc.m_AssetPackageDesc.m_AssetType = assetType;
                newResDesc.m_AssetPackageDesc.m_IncludeDepend = true;

                resMap.Add(newResDesc);
            }
        }

        assetGuid = AssetDatabase.FindAssets("t:prefab", assetPath);
        for (int i = 0, icnt = assetGuid.Length; i < icnt; ++i)
        {
            string curResPath = AssetDatabase.GUIDToAssetPath(assetGuid[i]);
            string[] depResPath = _GatherDependency(curResPath, new string[] { ".cs", ".dll" });
            if (null != depResPath)
            {
                bool hasReg = false;
                for (int j = 0, jcnt = depResPath.Length; j < jcnt; ++j)
                {
                    string ext = Path.GetExtension(depResPath[j]);
                    if (ext.Contains("tga") || ext.Contains("TGA") || ext.Contains("png") || ext.Contains("PNG") || ext.Contains("jpg") || ext.Contains("JPG"))
                    {
                        for (int k = 0, kcnt = resMap.Count; k < kcnt; ++k)
                        {
                            if (depResPath[j] == resMap[k].m_PrimeResPath)
                            {
                                if (!resMap[k].m_SecondaryResPathList.Contains(curResPath))
                                    resMap[k].m_SecondaryResPathList.Add(curResPath);
                                hasReg = true;
                                break;
                            }
                        }
                    }

                    if (hasReg)
                        break;
                }

                if(!hasReg)
                {
                    if (!nullTexDesc.m_SecondaryResPathList.Contains(curResPath))
                        nullTexDesc.m_SecondaryResPathList.Add(curResPath);
                }
            }
        }

        if (resMap.Count > 0)
        {
            for (int j = 0, jcnt = nullTexDesc.m_SecondaryResPathList.Count; j < jcnt; ++j)
                resMap[0].m_AssetPackageDesc.m_PackageAsset.Add(new PackAssetDesc(nullTexDesc.m_SecondaryResPathList[j]));
        }

        for (int i = 0, icnt = resMap.Count; i < icnt; ++i)
        {
            ResFirstDesc curResDesc = resMap[i];

            curResDesc.m_AssetPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curResDesc.m_PrimeResPath));
            for(int j = 0,jcnt = curResDesc.m_SecondaryResPathList.Count;j<jcnt;++j)
                curResDesc.m_AssetPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curResDesc.m_SecondaryResPathList[j]));

            descList.Add(curResDesc.m_AssetPackageDesc);
        }
    }


    static public void PackPolicySceneStart(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByFileType(assetType,assetPath[i], "scene", descList, new string[] { ".prefab" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        }
    }

    static public void PackPolicyEffect(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        ////// 第一步打包Effect Texture
        ///for (int i = 0; i < assetPath.Length; ++i)
        ///{
        ///    _PackAssetByFileType(assetPath[i], "effect", descList, new string[] { ".prefab", ".png", ".tga", ".fbx" }, new string[] { ".cs", ".dll" }, null, (uint)DAssetPackageFlag.None, true);
        ///}

        ///// 第二步打包Effect Prefab
        //for (int i = 0; i < assetPath.Length; ++i)
        //{
        //    _PackAssetByDirectoryLevel(assetPath[i], "effect", descList, new string[] {  ".prefab", ".controller", ".anim", ".mat" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true,2,0);
        //}

        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByFileType(assetType,assetPath[i], "effect", descList, new string[] { ".prefab" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        }

        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByFileName(assetType,assetPath[i], "effect", descList, new string[] { ".png", ".tga" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        }
    }

    static public void PackPolicyEffectCommonTexture(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        string[] packExtFilter = new string[] { ".png", ".tga" };
        string[] dependExcluder = new string[] { ".cs", ".dll" };

        List<string> dstAssetLst = new List<string>();
        float totalSize = 0.0f;
        for(int i = 0,icnt = assetPath.Length;i<icnt;++i)
        {
            assetPath[i] = assetPath[i].Replace('\\', '/');
            dstAssetLst.AddRange(Directory.GetFiles(assetPath[i], "*.*", SearchOption.AllDirectories));
        }
        for (int i = 0, icnt = dstAssetLst.Count; i < icnt; ++i)
            totalSize += FileUtil.GetFileBytes(dstAssetLst[i]) / (1024.0f * 1024.0f);
        
        int groupNum = Mathf.CeilToInt(totalSize);
        if (0 == groupNum)
            groupNum = 1;

        if (dstAssetLst.Count > 0)
        {
            AssetPackageDesc[] assetPackageDescArray = new AssetPackageDesc[groupNum];
            for (int i = 0; i < assetPackageDescArray.Length; ++i)
            {
                assetPackageDescArray[i] = new AssetPackageDesc();
                AssetPackageDesc curPackageDesc = assetPackageDescArray[i];

                string packName = "";
                string[] subPathSplit = assetPath[0].Split('/');
                for (int j = 0, jcnt = subPathSplit.Length; j < jcnt; ++j)
                {
                    if (subPathSplit[j] == "Assets" || subPathSplit[j] == "Resources")
                        continue;

                    packName += subPathSplit[j];

                    if (j + 1 < jcnt)
                        packName += '_';
                }

                curPackageDesc.m_PackageName = packName + "_FileNamePack_" + groupNum.ToString() + '_' + i.ToString() + m_OutputBundleExt;
                curPackageDesc.m_PackageFlag = (uint)DAssetPackageFlag.None;
                curPackageDesc.m_AssetType = assetType;
                curPackageDesc.m_IncludeDepend = true;
            }

            for (int i = 0; i < dstAssetLst.Count; ++i)
            {
                string curPath = dstAssetLst[i];
                curPath = curPath.Replace('\\', '/');

                string resName = Path.GetFileName(curPath);
                int charCnt = 0;
                for (int j = 0, jcnt = resName.Length; j < jcnt; ++j)
                    charCnt += (int)resName[j];
                int idx = charCnt % groupNum;

                bool bIsSkip = false;
                if (null != exclusivePath)
                {
                    for (int j = 0, jcnt = exclusivePath.Length; j < jcnt; ++j)
                    {
                        if (curPath.Contains(exclusivePath[j]))
                        {
                            bIsSkip = true;
                            break;
                        }
                    }
                }
                if (bIsSkip)
                    continue;

                AssetPackageDesc dstPackageDesc = assetPackageDescArray[idx];

                for (int j = 0; j < packExtFilter.Length; ++j)
                {
                    if (packExtFilter[j] == PathUtil.GetExtension(curPath).ToLower())
                    {
                        dstPackageDesc.m_PackageAsset.Add(new PackAssetDesc(curPath));
                        dstPackageDesc.m_OriginSize += File.ReadAllBytes(curPath).Length;
                        break;
                    }
                }

                if (dstPackageDesc.m_PackageAsset.Count == 0)
                    continue;

                if (dstPackageDesc.m_IncludeDepend)
                {
                    string[] depend = _GatherDependency(curPath, dependExcluder);
                    if (null != depend)
                    {
                        for (int j = 0; j < depend.Length; ++j)
                        {
                            depend[j] = depend[j].Replace('\\', '/');
                            if (depend[j].Contains(curPath))
                                continue;

                            if (!dstPackageDesc.m_DependAsset.Contains(depend[j]))
                                dstPackageDesc.m_DependAsset.Add(depend[j]);
                        }
                    }
                }
            }

            for (int i = 0; i < assetPackageDescArray.Length; ++i)
            {
                if (assetPackageDescArray[i].m_PackageAsset.Count > 0)
                    descList.Add(assetPackageDescArray[i]);
            }
        }
    }

    static public void PackPolicyEffectEx(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        string[] prefabAll = AssetDatabase.FindAssets("t:texture", assetPath);
        Dictionary<string, List<string>> effResMap = new Dictionary<string, List<string>>();
        for (int i = 0, icnt = prefabAll.Length; i < icnt; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabAll[i]);
            effResMap.Add(path, new List<string>());
        }

        List<string> nullTexture = new List<string>();
        string[] allprefab = AssetDatabase.FindAssets("t:prefab", assetPath);
        for (int i = 0, icnt = allprefab.Length; i < icnt; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(allprefab[i]);
            UnityEngine.GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            string[] deps = AssetDatabase.GetDependencies(path);
            for (int j = 0, jcnt = deps.Length; j < jcnt; ++j)
            {
                string deppath = deps[j];
                string ext = Path.GetExtension(deppath);
                if (ext.Equals(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".tga", System.StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".png", System.StringComparison.OrdinalIgnoreCase))
                {
                    List<string> curList = null;
                    if (effResMap.TryGetValue(deppath, out curList))
                        curList.Add(path);
                    else
                        nullTexture.Add(deppath);
                }
            }
        }

        AssetPackageDesc texturePackageDesc = new AssetPackageDesc();
        texturePackageDesc.m_PackageName = "effect_common_texture" + m_OutputBundleExt;
        texturePackageDesc.m_PackageFlag = (uint)DAssetPackageFlag.None;
        texturePackageDesc.m_AssetType = assetType;
        texturePackageDesc.m_IncludeDepend = true;

        List<string> assetPackRes = new List<string>();
        Dictionary<string, List<string>>.Enumerator it = effResMap.GetEnumerator();
        while (it.MoveNext())
        {
            List<string> prefabLst = it.Current.Value;
            if (prefabLst.Count > 1)
            {
                texturePackageDesc.m_PackageAsset.Add(new PackAssetDesc(it.Current.Key));
                assetPackRes.Add(it.Current.Key);
            }
        }

        for(int i = 0,icnt = nullTexture.Count;i<icnt;++i)
        {
            bool skip = true;
            for (int j = 0, jcnt = assetPath.Length; j < jcnt; ++j)
            {
                if (nullTexture[i].Contains(assetPath[j]))
                {
                    skip = false;
                    break;
                }
            }

            if(skip) continue;

            if (!texturePackageDesc.m_PackageAsset.Contains(new PackAssetDesc(nullTexture[i])))
                texturePackageDesc.m_PackageAsset.Add(new PackAssetDesc(nullTexture[i]));
        }

        string[] dependExcluder = new string[] { ".cs", ".dll" };
        if (texturePackageDesc.m_IncludeDepend)
        {
            for(int i = 0,icnt = assetPackRes.Count;i<icnt;++i)
            {
                string curPath = assetPackRes[i];
                string[] depend = _GatherDependency(curPath, dependExcluder);
                if (null != depend)
                {
                    for (int j = 0; j < depend.Length; ++j)
                    {
                        depend[j] = depend[j].Replace('\\', '/');
                        if (depend[j].Contains(curPath))
                            continue;

                        if (!texturePackageDesc.m_DependAsset.Contains(depend[j]))
                            texturePackageDesc.m_DependAsset.Add(depend[j]);
                    }
                }
            }
        }

        descList.Add(texturePackageDesc);

        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType, assetPath[i], "effect", descList, new string[] { ".prefab" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, packLv, 0);
        }
    }

    static public void PackPolicyUI(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        /// 第一步打包UI Prefab
        for (int i = 0; i < assetPath.Length; ++i)
        {
            //_PackAssetByDirectoryLevel(assetType, assetPath[i], "ui", descList, new string[] { ".prefab" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true, packLv, 0);
            _PackAssetByFileType(assetType,assetPath[i], "ui", descList, new string[] { ".prefab" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true);
        }

        /// 第二步打包UI 动画
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType, assetPath[i], "ui", descList, new string[] { ".controller", ".anim", ".asset"}, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true, packLv, 0);
        }
    }

    static public void PackPolicyUITitle(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType, assetPath[i], "ui", descList, new string[] { ".prefab", ".png", ".anim", ".controller", ".mat" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true, packLv, 0);
        }
    }

    static public void PackPolicyUIPackedImage(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        /// 打包UI Texture
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByFileType(assetType,assetPath[i], "ui", descList, new string[] { ".png", ".tga", ".jpg", ".mat" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true);
        }
    }

    ///
    static public void PackPolicyUIImage(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType, assetPath[i], "ui", descList, new string[] { ".png", ".tga", ".jpg", ".mat" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true, packLv, 0);
        }
    }

    static public void PackPolicyUIFont(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        /// 第三步打包UI Font
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByFileType(assetType, assetPath[i], "ui", descList, new string[] { ".ttf" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true);
        }

        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByDirectoryLevel(assetType, assetPath[i], "ui", descList, new string[] { ".png", ".fnt", ".fontsettings", ".mat" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true, 0, 0);
        }
    }



    static public void PackPolicyTestUI(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByRootDirectory(assetType,assetPath[i], "ui", descList, new string[] { ".png", ".tga", ".jpg", ".ttf", ".prefab", ".anim", ".controller", ".fnt", ".fontsettings" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.LeaveWithAsset, true);
        }
    }

    static public void PackPolicySound(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            //_PackAssetByFileType(assetPath[i], "sound", descList, new string[] { ".ogg", ".wav" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
            _PackAssetByDirectoryLevel(assetType,assetPath[i], "sound", descList, new string[] { ".ogg", ".wav" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true, packLv, 0);
        }
    }

    static public void PackPolicySoundSingle(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByFileType(assetType, assetPath[i], "sound", descList, new string[] { ".ogg", ".wav" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.None, true);
        }
    }

    static public void PackPolicyDataLite(EAssetType assetType, string[] assetPath, string[] exclusivePath, int packLv, List<AssetPackageDesc> descList)
    {
        for (int i = 0; i < assetPath.Length; ++i)
        {
            _PackAssetByRootDirectory(assetType,assetPath[i], "", descList, new string[] { ".asset", ".xml", ".json" }, new string[] { ".cs", ".dll" }, exclusivePath, (uint)DAssetPackageFlag.UsingGUIDName, false);
        }
    }
    //static public void PackPolicyTest(string[] assetPath, List<AssetPackageDesc> descList)
    //{
    //    for (int i = 0; i < assetPath.Length; ++i)
    //    {
    //        _PackAssetByRootDirectory(assetPath[i], "", descList, new string[] { ".png", ".asset" }, new string[] { ".cs", ".dll" }, DAssetPackageFlag.None, true);
    //        //_PackAssetByDirectoryLevel(assetPath[i], "", descList, new string[] { ".png", ".asset" }, new string[] { ".cs", ".dll" }, DAssetPackageFlag.None, true, 1, 0);
    //    }
    //}

    //static public void PackPolicyAll(string[] assetPath, List<AssetPackageDesc> descList)
    //{
    //    for (int i = 0; i < assetPath.Length; ++i)
    //    {
    //        _PackAssetByRootDirectory(assetPath[i], "", descList, new string[] { ".asset", ".shader",
    //            ".ogg" , ".png", ".tga", ".jpg", ".controller", ".anim", ".mat",
    //            ".ttf", ".fnt", ".fontsettings", ".controller",
    //            ".xml", ".json", ".prefab"}, new string[] { ".cs", ".dll" }, DAssetPackageFlag.LeaveWithAsset, true);
    //        //_PackAssetByDirectoryLevel(assetPath[i], "", descList, new string[] { ".png", ".asset" }, new string[] { ".cs", ".dll" }, DAssetPackageFlag.None, true, 1, 0);
    //    }
    //}

    [MenuItem("[打包工具]/清理热更新缓存")]
    static public void ClearPackageCache()
    {
        string dstDictory = Path.Combine(Application.persistentDataPath, "AssetBundles");
        if (Directory.Exists(dstDictory))
            Directory.Delete(dstDictory, true);

        string verFile1 = Path.Combine(Application.persistentDataPath, "version.config");
        if (File.Exists(verFile1))
            File.Delete(verFile1);

        string verFile2 = Path.Combine(Application.persistentDataPath, "version.json");
        if (File.Exists(verFile2))
            File.Delete(verFile2);
    }

    static protected void _ClearPackages()
    {
        string dstDictory = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        if (Directory.Exists(dstDictory))
            Directory.Delete(dstDictory, true);

        if (Directory.Exists(m_TempAssetPath))
            Directory.Delete(m_TempAssetPath, true);
        
        if (File.Exists("Assets/Resources/Base/Version/PackageInfo.asset"))
            File.Delete("Assets/Resources/Base/Version/PackageInfo.asset");
    }


    static void _GenerateAssetTreeData(BuildTarget platform)
    {
        string packageInfoPath = "Assets/StreamingAssets/AssetBundles/packageinfo.pak";
        UnityEngine.AssetBundle abPackageInfo = UnityEngine.AssetBundle.LoadFromFile(packageInfoPath);
        if (null != abPackageInfo)
        {
            abPackageInfo.Unload(true);
            abPackageInfo = UnityEngine.AssetBundle.LoadFromFile(packageInfoPath);
        }

        if (null != abPackageInfo)
        {
            DAssetPackageDependency apInfo = abPackageInfo.LoadAsset<DAssetPackageDependency>("PackageInfo.asset");
            if (null != apInfo)
            {
                Tenmove.Runtime.Unity.TMUnityAssetTreeData assetTreeData = Tenmove.Editor.Unity.AssetTreeDataConverter.Convert(apInfo);
                if (File.Exists("Assets/Resources/Base/Version/AssetTreeData.asset"))
                    File.Delete("Assets/Resources/Base/Version/AssetTreeData.asset");

                AssetDatabase.CreateAsset(assetTreeData, "Assets/Resources/Base/Version/AssetTreeData.asset");
                EditorUtility.SetDirty(assetTreeData);
                AssetDatabase.SaveAssets();
            }
            abPackageInfo.Unload(false);

            string oldPackageFile = Path.Combine(m_OutputBundlePath, "PackageInfo.pak");
            if (File.Exists(oldPackageFile))
                File.Delete(oldPackageFile);

            AssetBundleBuild[] bundlePackageInfoBuild = new AssetBundleBuild[1];
            bundlePackageInfoBuild[0].assetBundleName = "PackageInfo.pak";
            bundlePackageInfoBuild[0].assetNames = new string[2] { "Assets/Resources/Base/Version/PackageInfo.asset", "Assets/Resources/Base/Version/AssetTreeData.asset" };
            bundlePackageInfoBuild[0].assetBundleVariant = "";

            AssetBundleManifest assetManifestPackageInfo = BuildPipeline.BuildAssetBundles(m_OutputBundlePath, bundlePackageInfoBuild, BuildAssetBundleOptions.None, platform);
        }

    }


    //[MenuItem("[打包工具]/导出资源包依赖信息")]
    static public void DumpPackageInfo()
    {
        DAssetPackageDependency asset = AssetDatabase.LoadAssetAtPath<DAssetPackageDependency>("Assets/Resources/Base/Version/PackageInfo.asset");
        if (null != asset)
            _DumpPackageInfoAsCsv(asset);
    }


    static protected void _DumpPackageInfoAsCsv(DAssetPackageDependency assetPackageDependency)
    {
        /// 包体大小
        /// 资源数量
        /// 依赖包数量
        /// 
        DAssetPackageDesc[] packageDescArray = assetPackageDependency.packageDescArray;
        int packageDescArrayLen = packageDescArray.Length;

        List<string>[] packageDescAssetListMap = new List<string>[packageDescArrayLen];
        for (int i = 0,icnt = assetPackageDependency.assetDescPackageMap.Count;i<icnt;++i)
        {
            DAssetPackageMapDesc cur = assetPackageDependency.assetDescPackageMap[i];

            if(0<=cur.packageDescIdx && cur.packageDescIdx < packageDescArrayLen)
            {
                if (null == packageDescAssetListMap[cur.packageDescIdx])
                    packageDescAssetListMap[cur.packageDescIdx] = new List<string>();
                packageDescAssetListMap[cur.packageDescIdx].Add(assetPackageDependency.assetDescPackageMap[i].assetPathKey);
            }
        }

        string date = DateTime.Now.ToString("yyyy_MM_dd");
        FileStream streamW = new FileStream(m_OutputBundlePath + "PackageInfoDump_" + date + ".csv", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        StreamWriter sw = new StreamWriter(streamW, Encoding.GetEncoding(936));

        Dictionary<string,float> resSizeMap = new Dictionary<string, float>();

        resSizeMap["actor"] = 0;
        resSizeMap["scene"] = 0;
        resSizeMap["effect"] = 0;
        resSizeMap["sound"] = 0;
        resSizeMap["ui"] = 0;
        resSizeMap["other"] = 0;
        resSizeMap["total"] = 0;

        sw.WriteLine("    包名    , 包体大小(MB) ,    资源数量    ,    依赖包数量    ,    依赖包详情    ,    资源量详情    ");
        for (int i = 0; i < packageDescArray.Length; ++i)
        {
            EditorUtility.DisplayProgressBar("正在打包", "正在导出打包信息...", ((float)i / packageDescArray.Length));

            string content = "";
            DAssetPackageDesc cur = packageDescArray[i];

            string curBundlePath = Path.Combine("Assets/StreamingAssets", cur.packagePath + cur.packageName);
            float bundleMBytes = FileUtil.GetFileBytes(curBundlePath) / (1024.0f*1024.0f);

            if(cur.packageName.ToLower().Contains("actor_"))
                resSizeMap["actor"] += bundleMBytes;
            else if (cur.packageName.ToLower().Contains("scene_"))
                resSizeMap["scene"] += bundleMBytes;
            else if (cur.packageName.ToLower().Contains("effect_") || cur.packageName.ToLower().Contains("effects_"))
                resSizeMap["effect"] += bundleMBytes;
            else if (cur.packageName.ToLower().Contains("sound_"))
                resSizeMap["sound"] += bundleMBytes;
            else if (cur.packageName.ToLower().Contains("ui_") || cur.packageName.ToLower().Contains("uipacked_") || cur.packageName.ToLower().Contains("uiflatten_"))
                resSizeMap["ui"] += bundleMBytes;
            else
                resSizeMap["other"] += bundleMBytes;

            resSizeMap["total"] += bundleMBytes;

            content += Path.GetFileName(cur.packageName);
            content += ",";

            content += bundleMBytes.ToString();
            content += ",";

            if (null != packageDescAssetListMap[i])
                content += packageDescAssetListMap[i].Count.ToString();
            else
                content += '0';
            content += ",";

            content += cur.packageAutoDepend.Length.ToString();
            content += ",";

            string subContent = "";
            for (int j = 0, jcnt = cur.packageAutoDepend.Length; j < jcnt; ++j)
            {
                subContent += cur.packageAutoDepend[j];
                if(j < jcnt-1)
                    subContent += "  |  ";
            }

            content += subContent;
            content += ",";

            subContent = "";
            if(null != packageDescAssetListMap[i])
            {
                for (int j = 0, jcnt = packageDescAssetListMap[i].Count; j < jcnt; ++j)
                {
                    subContent += packageDescAssetListMap[i][j];
                    if (j < jcnt - 1)
                        subContent += "  |  ";
                }
            }

            content += subContent;
            content += ",";

            sw.WriteLine(content);
        }

        sw.WriteLine("total:" + resSizeMap["total"] + " MB," + "actor:" + resSizeMap["actor"] + " MB," + "scene:" + resSizeMap["scene"] + " MB," + "sound:" + resSizeMap["sound"] + " MB," + "effect:" + resSizeMap["effect"] + " MB," + "ui:" + resSizeMap["ui"] + " MB," + "other:" + resSizeMap["other"] + " MB");

        streamW.Flush();
        sw.Close();
        streamW.Close();
        EditorUtility.ClearProgressBar();
    }

    static void _BreakDungeonAssetLink()
    {
        string[] assetPathList = Directory.GetFiles("Assets/Resources/Data/SceneData", "*.asset", SearchOption.AllDirectories);
        for (int i = 0,icnt = assetPathList.Length;i<icnt;++i)
        {
            string path = assetPathList[i];
            DSceneData data = AssetDatabase.LoadAssetAtPath<DSceneData>(path);
            if (data == null)
            {
                Debug.LogErrorFormat("####### FATAL ERROR: _BreakDungeonAssetLink load file:{0} failed ########", path);
                continue;
            }
            data._prefab = null;
            EditorUtility.SetDirty(data);
        }
        AssetDatabase.SaveAssets();
    }

    static void _BreakSkillAssetLink()
    {
        string[] assetPathList = Directory.GetFiles("Assets/Resources/Data/SkillData", "*.asset", SearchOption.AllDirectories);
        for (int i = 0, icnt = assetPathList.Length; i < icnt; ++i)
        {
            string path = assetPathList[i];
            DSkillData data = AssetDatabase.LoadAssetAtPath<DSkillData>(path);

            if(null == data ) continue;

            data.goHitEffect = null;
            data.goHitEffectAsset.m_AssetObj = null;

            data.goSFXAsset.m_AssetObj = null;
            data.characterPrefab = null;
            data.characterAsset.m_AssetObj = null;

            if(null != data.attachFrames)
            {
                for(int j = 0,jcnt = data.attachFrames.Length;j<jcnt;++j)
                {
                    EntityAttachFrames curFrame = data.attachFrames[j];
                    curFrame.entityPrefab = null;
                    curFrame.entityAsset.m_AssetObj = null;
                }
            }

            if (null != data.effectFrames)
            {
                for (int j = 0, jcnt = data.effectFrames.Length; j < jcnt; ++j)
                {
                    EffectsFrames curFrame = data.effectFrames[j];
                    curFrame.effectGameObjectPrefeb = null;
                    curFrame.effectAsset.m_AssetObj = null;
                }
            }

            if (null != data.entityFrames)
            {
                for (int j = 0, jcnt = data.entityFrames.Length; j < jcnt; ++j)
                {
                    EntityFrames curFrame = data.entityFrames[j];
                    curFrame.entityPrefab = null;
                    curFrame.entityAsset.m_AssetObj = null;
                }
            }

            if (null != data.sfx)
            {
                for (int j = 0, jcnt = data.sfx.Length; j < jcnt; ++j)
                {
                    DSkillSfx curFrame = data.sfx[j];
                    curFrame.soundClip = null;
                    curFrame.soundClipAsset.m_AssetObj = null;
                }
            }

            EditorUtility.SetDirty(data);
        }
        AssetDatabase.SaveAssets();
    }

    static string[] needTrimAssetList = new string[]
    {
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Fightergirl",
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_002",
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_003",
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_004",
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_006",
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_022",
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_FighterGirl_Tiankong_01",
            "Assets/Resources/Actor/Hero_Fightergirl/Hero_FighterGirl_Tiankong_01_a",
            "Assets/Resources/Actor/Hero_Fightergirl/Wing",

            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Gungirl",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Manyou",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_002",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_003",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_003_a",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_004",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_006",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_022",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_GunGirl_Tiankong_01",
            "Assets/Resources/Actor/Hero_Gungirl/Hero_GunGirl_Tiankong_01_a",
            "Assets/Resources/Actor/Hero_Gungirl/Wing",

            "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Gunman",
            "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_002",
            "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_003",
            "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_003_a",
            "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_004",
            "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_006",
            "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_022",
            "Assets/Resources/Actor/Hero_Gunman/Hero_GunMan_Tiankong_01",
            "Assets/Resources/Actor/Hero_Gunman/Hero_GunMan_Tiankong_01_a",
            "Assets/Resources/Actor/Hero_Gunman/Wing",

            "Assets/Resources/Actor/Hero_MageGirl/Hero_MageGirl_Magegirl",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_002",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_003",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_003_a",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_004",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_006",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_022",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_MageGirl_Tiankong_01",
            "Assets/Resources/Actor/Hero_MageGirl/Hero_MageGirl_Tiankong_01_a",
            "Assets/Resources/Actor/Hero_MageGirl/Wing",

            "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Swordsman",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_002",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_003",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_003_a",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_004",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_006",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_022",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_SwordsMan_Tiankong_01",
            "Assets/Resources/Actor/Hero_Swordsman/Hero_SwordsMan_Tiankong_01_a",
            "Assets/Resources/Actor/Hero_Swordsman/Wing",
    };

    static void _ReimportFBXMaterial()
    {
        /// for (int i = 0, icnt = needTrimAssetList.Length; i < icnt; ++i)
        /// {
        ///     string[] assetPathList = Directory.GetFiles(needTrimAssetList[i], "*.fbx", SearchOption.AllDirectories);
        ///     if(null != assetPathList)
        ///     {
        ///         for (int j = 0, jcnt = assetPathList.Length; j < jcnt; ++j)
        ///             AssetDatabase.ImportAsset(assetPathList[j]);
        ///     }
        ///     
        /// }
        /// 

        /// string[] assetPathList = Directory.GetFiles("Assets/Resources/Actor", "*.fbx", SearchOption.AllDirectories);
        /// if (null != assetPathList)
        /// {
        ///     for (int j = 0, jcnt = assetPathList.Length; j < jcnt; ++j)
        ///     {
        ///         string cur = assetPathList[j];
        ///         if (!cur.StartsWith("Assets/Resources/Actor/Hero_"))
        ///             continue;
        /// 
        ///         AssetDatabase.ImportAsset(cur,ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive | ImportAssetOptions.DontDownloadFromCacheServer);
        ///     }
        /// }
        /// 

        string[] assetPathList = new string[]
        {
            "Assets/Resources/Actor",
        };

        if (null != assetPathList)
        {
            for (int j = 0, jcnt = assetPathList.Length; j < jcnt; ++j)
            {
                string assetPath = assetPathList[j];
                if(assetPath.StartsWith("Assets/Resources/Actor/Hero_") || assetPath.StartsWith("Assets/Resources/Actor/Pet_", System.StringComparison.OrdinalIgnoreCase) || assetPath.StartsWith("Assets/Resources/Actor/Weapon"))
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive | ImportAssetOptions.DontDownloadFromCacheServer);
            }
        }
    }

    static void _BuildSceneSpriteAltas()
    {
        SceneSprAtlasPacker.UpdateAllSpriteAtlasForScene();
    }
}
