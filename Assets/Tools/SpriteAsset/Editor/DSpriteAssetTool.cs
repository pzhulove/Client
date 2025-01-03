using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

[InitializeOnLoad]
public class DSpriteAssetTools
{
    public const string kDefaultPath = "Assets/SpriteAsset/allInvalidRef.asset";
    private static DSpriteAsset asset = null;

    public DSpriteAssetTools()
    {
        _load();
    }

    public static void Add(Sprite sprite)
    {
        if (null == asset)
        {
            _load();
        }
        
        if (_isInArray(sprite))
        {
            return ;
        }
       
        asset.references.Add(sprite);
        EditorUtility.SetDirty(asset);
    }

    public static bool Contains(Sprite sprite)
    {
        return _isInArray(sprite);
    }

    private static bool _isInArray(Sprite sprite)
    {
        _load();

        for (int i = 0; i < asset.references.Count; ++i)
        {
            if (sprite == asset.references[i])
            {
                return true;
            }
        }

        return false;
    }

    public static void Clear()
    {
        _reset();
    }

    private static void _load()
    {
        if (null == asset)
        {
            asset = AssetDatabase.LoadAssetAtPath<DSpriteAsset>(kDefaultPath);
        }

        if (null == asset)
        {
            _reset();
        }
    }

    private static void _reset()
    {
        if (File.Exists(kDefaultPath))
        {
            File.Delete(kDefaultPath);
        }
        asset = new DSpriteAsset();
        AssetDatabase.CreateAsset(asset, kDefaultPath);
    }


    static Dictionary<UnityEngine.Object, List<GameObject>> GetDicts<T>(string[] prefabpaths) where T : UnityEngine.Object
    {
        var prefabs = AssetDatabase.FindAssets("t:prefab", prefabpaths);

        Dictionary<UnityEngine.Object, List<GameObject>> coDict = new Dictionary<UnityEngine.Object, List<GameObject>>();

        int cnt = prefabs.Length;
        int ii = 0;

        foreach (var preguid in prefabs)
        {
            ii++;

            var path = AssetDatabase.GUIDToAssetPath(preguid);

            UnityEngine.GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { root });

            foreach (var com in deps)
            {
                T sp = com as T;
                if (null != sp)
                {
                    string objpath = AssetDatabase.GetAssetPath(sp);

                    EditorUtility.DisplayProgressBar(Path.GetFileName(path), Path.GetFileName(objpath), ii*1.0f/cnt);
                    if (!coDict.ContainsKey(sp))
                    {
                        coDict.Add(sp, new List<GameObject>());
                    }

                    coDict[sp].Add(root);
                }
            }
        }

        EditorUtility.ClearProgressBar();

        return coDict;
    }


    //[MenuItem("[TM工具集]/[UI分析]/DSsprite")]
    public static void Create()
    {
        FileTools.CreateAsset<DSpriteAsset>("SpriteAsset");
    }

    //[MenuItem("[TM工具集]/[UI分析]/Dump")]
    public static void DumpAsset()
    {
        string ans = "Name\n";

        for (int i = 0; i < asset.references.Count; ++i)
        {
            ans += FileTools.GetAssetPath(asset.references[i]);
            ans += ":" + asset.references[i].name;
            ans += "\n";
            //ans += ;//AssetDatabase.GUIDToAssetPath(asset.references[i].GetInstanceID());
        }

        File.WriteAllText(kDefaultPath+".csv", ans);
    }


    private static string[] kUIPrefabs       = new string[] {
        "Assets/Resources/UIFlatten/Prefabs"
    };

    private static string[] kInvalidPackagePaths = new string[]
    {
        "Assets/Resources/UIPacked",
        "Assets/Resources/UI/Image/Packed",
        "Assets/Resources/UIFlatten/Image/Packed",
    };
    

    [MenuItem("[TM工具集]/[UI分析]/生成旧合图引用")]
    public static void InvalidGenerateRef()
    {
        var dict = GetDicts<Sprite>(kUIPrefabs);
        string[] texts = AssetDatabase.FindAssets("t:texture", kInvalidPackagePaths);

        for (int i = 0; i < texts.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(texts[i]);
            UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            bool findFlag = false;

            DSpriteAsset spriteAsset = new DSpriteAsset();
            spriteAsset.name = Path.GetFileNameWithoutExtension(path);

            for (int j = 0; j < sprites.Length; ++j)
            {
                if (dict.ContainsKey(sprites[j]))
                {
                    findFlag = true;

                    DSpriteNode node = new DSpriteNode();
                    node.sprite = (Sprite)sprites[j];
                    foreach (var iterItem in dict[sprites[j]])
                    {
                        node.refsPrefabs.Add((GameObject)iterItem);
                    }
                    spriteAsset.refs.Add(node);
                }
            }

            if (findFlag)
            {
                string filename  = Path.GetExtension(path);
                string assetPath = path.Replace(filename, ".asset");

                if (File.Exists(assetPath))
                {
                    File.Delete(assetPath);
                }

                AssetDatabase.CreateAsset(spriteAsset, assetPath);
            }
            else 
            {
                UnityEngine.Debug.LogErrorFormat("合图 {0} 没有用到", path);
            }
        }
    }
    
}
