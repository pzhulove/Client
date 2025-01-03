using System;
using System.IO;
using UnityEngine;
using UnityEditor;


public class FXBSkinImporter : AssetPostprocessor
{
    bool _importSuccess = false;
    string _removeAssetPath = "";
    string _validAssetDirectory = "";
    string _suitPart = "";
    string _dicName = "";

    bool _FBXFile(string assetPath)
    {
        return assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase);
    }
    

    bool _IsHeroModel(string assetName)
    {
        return assetName.Contains("Hero_", System.StringComparison.OrdinalIgnoreCase);
    }

    bool _IsActorModel(string assetPath)
    {
        return assetPath.StartsWith("Assets/Resources/Actor/");
    }

    string _GetActorModelDirectoryName(string assetName)
    {
        string dicName = assetName;
        int skinIdx = dicName.LastIndexOf("_skin", StringComparison.OrdinalIgnoreCase);
        if(0 <= skinIdx && skinIdx< dicName.Length)
            dicName = dicName.Remove(skinIdx, "_skin".Length);

        int lowIdx = dicName.LastIndexOf("_Low", StringComparison.OrdinalIgnoreCase);
        if (0 <= lowIdx && lowIdx < dicName.Length)
            dicName = dicName.Remove(lowIdx, "_Low".Length);

        int headIdx = dicName.LastIndexOf("_Head", StringComparison.OrdinalIgnoreCase);
        if (0 <= headIdx && headIdx < dicName.Length)
        {
            dicName = dicName.Remove(headIdx, "_Head".Length);
            _suitPart = "Head";
        }

        int bodyIdx = dicName.LastIndexOf("_Body", StringComparison.OrdinalIgnoreCase);
        if (0 <= bodyIdx && bodyIdx < dicName.Length)
        {
            dicName = dicName.Remove(bodyIdx, "_Body".Length);
            _suitPart = "Body";
        }

        int pantIdx = dicName.LastIndexOf("_Pant", StringComparison.OrdinalIgnoreCase);
        if (0 <= pantIdx && pantIdx < dicName.Length)
        {
            dicName = dicName.Remove(pantIdx, "_Pant".Length);
            _suitPart = "Pant";
        }

        string jobName =  _GetActorJobName(dicName);
        if (string.Format("Hero_{0}", jobName) == dicName)
            dicName = string.Format("{0}_{1}", dicName, jobName);

        return dicName;
    }

    string _GetActorJobName(string dicName)
    {
        string jobName = dicName;
        int heroIdx = jobName.IndexOf("Hero_", StringComparison.OrdinalIgnoreCase);
        if (0 <= heroIdx && heroIdx < jobName.Length)
            jobName = jobName.Remove(heroIdx, "Hero_".Length);

        int underlineIdx = jobName.IndexOf('_');
        if (0 <= underlineIdx && underlineIdx < jobName.Length)
            jobName = jobName.Substring(0,underlineIdx);

        return jobName;
    }

    void OnPostprocessModel(GameObject g)
    {
        ModelImporter importer = (ModelImporter)assetImporter;
       
        string assetPath = importer.assetPath;
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        if (_FBXFile(assetPath) && _IsNeedTrimMaterial(assetPath))
        {

            //if (_importSuccess)
            //{
            /// Material defMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Shader/DefaultPBR.mat");
            /// if (null == defMaterial)
            /// {
            ///     defMaterial = new Material(Shader.Find("HeroGo/PBR/Surface/General"));
            ///     AssetDatabase.CreateAsset(defMaterial, "Assets/Resources/Shader/DefaultPBR.mat");
            ///     EditorUtility.SetDirty(defMaterial);
            /// }

            Renderer[] ar = g.GetComponentsInChildren<Renderer>();
            if (null != ar)
            {
                for (int i = 0, icnt = ar.Length; i < icnt; ++i)
                {
                    Renderer curRend = ar[i];
                    if (null == curRend)
                        continue;

                    curRend.sharedMaterials = new Material[0];
                }
            }

            /// ModelImporter importer = (ModelImporter)assetImporter;
            /// 
            /// Material mat = null;
            /// if (!string.IsNullOrEmpty(_suitPart))
            /// {
            ///     string dstMaterialPath = string.Format("{0}/{1}/Materials/M_{2}_{3}.mat", _validAssetDirectory, _suitPart, _dicName, _suitPart);
            ///     if (!File.Exists(dstMaterialPath))
            ///     {
            ///         string dstDirPath = Path.GetDirectoryName(dstMaterialPath);
            ///         if (!Directory.Exists(dstDirPath))
            ///             Directory.CreateDirectory(dstDirPath);
            /// 
            ///         mat = new Material(Shader.Find("HeroGo/PBR/Surface/General"));
            ///         AssetDatabase.CreateAsset(mat, dstMaterialPath);
            ///         EditorUtility.SetDirty(mat);
            ///         AssetDatabase.SaveAssets();
            ///     }
            /// }
            /// 
            /// Renderer[] ar = g.GetComponentsInChildren<Renderer>();
            /// if(null != ar)
            /// {
            ///     for(int i = 0,icnt = ar.Length;i<icnt;++i)
            ///     {
            ///         Renderer curRend = ar[i];
            ///         if (null == curRend)
            ///             continue;
            /// 
            ///         Material[] am = curRend.sharedMaterials;
            ///         if(null != am)
            ///         {
            ///             for (int j = 0, jcnt = am.Length; j < jcnt; ++j)
            ///             {
            ///                 Material curMat = am[j];
            ///                 if (null == curMat)
            ///                     continue;
            /// 
            ///                 am[j] = mat;
            ///             }
            ///         }
            ///     }
            /// }
            //}
        }
    }

    /// void OnPreprocessModel()
    /// {
    ///     ModelImporter importer = (ModelImporter)assetImporter;
    ///     importer.importMaterials = false;
    /// 
    ///     _importSuccess = false;
    ///     string assetPath = importer.assetPath;
    ///     string assetName = Path.GetFileNameWithoutExtension(assetPath);
    ///     if (_FBXFile(assetPath) && _IsHeroModel(assetName))
    ///     {
    ///         _dicName = _GetActorModelDirectoryName(assetName);
    ///         string jobName = _GetActorJobName(_dicName);
    /// 
    ///         _validAssetDirectory = string.Format("Assets/Resources/Actor/Hero_{0}/{1}", jobName, _dicName);
    ///         string validAssetPath = string.Format("{0}/Skins/{1}{2}", _validAssetDirectory, assetName,Path.GetExtension(assetPath));
    ///         if(!validAssetPath.Equals(assetPath))
    ///         {
    ///             EditorUtility.DisplayDialog("FBX asset import check failed", string.Format("asset '{0}' path or name is invalid, correct path:'{1}'!",assetPath, validAssetPath),"OK");
    ///             _removeAssetPath = assetPath;
    ///             
    ///             // string validPath = Path.GetDirectoryName(validAssetPath);
    ///             // if (!Directory.Exists(validPath))
    ///             //     Directory.CreateDirectory(validPath);
    ///             // if(!File.Exists(validAssetPath))
    ///             //     File.Move(assetPath, validAssetPath);
    ///             // else
    ///             //     File.Move(assetPath, validAssetPath + ".bak");
    /// 
    ///         }
    ///     }
    /// 
    ///     _importSuccess = true;
    /// }


    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {


        /// ModelImporter modelImp = (ModelImporter)assetImporter;
        /// string path = assetPath.ToLower();
        /// if (path.EndsWith(".fbx"))
        /// {
        ///     Renderer[] renderComs = model.GetComponentsInChildren<Renderer>();
        ///     for (int i = 0; i < renderComs.Length; i++)
        ///     {
        ///         renderComs[i].sharedMaterial = null;
        /// 
        ///         if (renderComs[i].sharedMaterials != null)
        ///         {
        ///             renderComs[i].sharedMaterials = new Material[0];
        ///         }
        ///     }
        /// }
    }

    static string[] needTrimAssetList = new string[]
    {
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Fightergirl/",
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_002/",
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_003/",
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_004/",
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_006/",
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_Fightergirl_Shizhuang_022/",
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_FighterGirl_Tiankong_01/",
        "Assets/Resources/Actor/Hero_Fightergirl/Hero_FighterGirl_Tiankong_01_a/",
        "Assets/Resources/Actor/Hero_Fightergirl/Wing/",

        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Gungirl/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Manyou/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_002/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_003/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_003_a/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_004/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_006/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_Gungirl_Shizhuang_022/",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_GunGirl_Tiankong_01",
        "Assets/Resources/Actor/Hero_Gungirl/Hero_GunGirl_Tiankong_01_a",
        "Assets/Resources/Actor/Hero_Gungirl/Wing",

        "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Gunman/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_002/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_003/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_003_a/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_004/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_006/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_Gunman_Shizhuang_022/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_GunMan_Tiankong_01/",
        "Assets/Resources/Actor/Hero_Gunman/Hero_GunMan_Tiankong_01_a/",
        "Assets/Resources/Actor/Hero_Gunman/Wing/",

        "Assets/Resources/Actor/Hero_MageGirl/Hero_MageGirl_Magegirl/",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_002",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_003",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_003_a",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_004",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_006",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_Magegirl_Shizhuang_022",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_MageGirl_Tiankong_01",
        "Assets/Resources/Actor/Hero_MageGirl/Hero_MageGirl_Tiankong_01_a",
        "Assets/Resources/Actor/Hero_MageGirl/Wing",

        "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Swordsman/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_002/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_003/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_003_a/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_004/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_006/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_Swordsman_Shizhuang_022/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_SwordsMan_Tiankong_01/",
        "Assets/Resources/Actor/Hero_Swordsman/Hero_SwordsMan_Tiankong_01_a/",
        "Assets/Resources/Actor/Hero_Swordsman/Wing",
    };

    private static bool _IsNeedTrimMaterial(string assetPath)
    {
        /// for(int i = 0,icnt = needTrimAssetList.Length;i<icnt;++i)
        /// {
        ///     if (assetPath.StartsWith(needTrimAssetList[i]))
        ///     {
        ///         return true;
        ///     }
        /// }
        /// 
        /// return false;
        /// 

        return assetPath.StartsWith("Assets/Resources/Actor/Hero_") || assetPath.StartsWith("Assets/Resources/Actor/Pet_",System.StringComparison.OrdinalIgnoreCase) || assetPath.StartsWith("Assets/Resources/Actor/Weapon");
    }
}
