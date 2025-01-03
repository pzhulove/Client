using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private List<TKey> keys = new List<TKey>();

    [SerializeField, HideInInspector]
    private List<TValue> values = new List<TValue>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.",
            keys.Count,  values.Count));

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }

    public TKey GetKey(int index)
    {
        return keys[index];
    }

    public TValue GetValue(int index)
    {
        return values[index];
    }
}


[Serializable]
public class AvatarNames
{
    [SerializeField]
    public string[] m_Names = new string[(int)GeAvatarChannel.AvatarRoot + 1];
}

[Serializable]
public class AvatarFallbackDictionary : SerializableDictionary<int, AvatarNames>
{
}

[CreateAssetMenu(fileName = "AvatarFallbackAsset", menuName = "Client/AvatarFallback", order = 0)]
public class GeAvatarFallbackAsset : ScriptableObject 
{
    [SerializeField]
    public AvatarFallbackDictionary avatarDic = new AvatarFallbackDictionary();
}

public static class GeAvatarFallback
{
    private static GeAvatarFallbackAsset m_AvatarFallback;
    //private static FirstPackageFileAsset m_FirstPackageFiles;
    private static bool m_Loaded;
    private static HashSet<string> m_ReadyAssets = new HashSet<string>();
    private static List<string> m_AssetDependentPackages = new List<string>();
    private static string m_LebianABDir;
    private static bool m_ResValid;

    private static HashSet<string> m_APKABs = new HashSet<string>();

    private static Dictionary<string, int> OccupationDic = new Dictionary<string, int>
    {
        {"Swordsman", 10},
        {"Gunman", 20},
        {"MageGirl", 30},
        {"Fightergirl", 40},
        {"Gungirl", 50},
        {"Paladin", 60},
    };

    /// <summary>
    /// 先通过资源名称来确定职业
    /// </summary>
    /// <param name="resName"></param>
    private static int GetOccupationFromPath(string resName)
    {
        var itr = OccupationDic.GetEnumerator();
        while (itr.MoveNext())
        {
            if (resName.Contains(itr.Current.Key))
            {
                return itr.Current.Value;
            }
        }

        return -1;
    }

    // 是否从全局启用AvatarPartFallback，由是否在城镇决定
    public static bool EnableGlobalAvatarPartFallback
    {
        get;
        set;
    }

    public static bool IsAvatarPartFallbackEnabled()
    {
#if USE_SMALLPACKAGE
        return EnableGlobalAvatarPartFallback && SDKInterface.instance.IsSmallPackage() && EngineConfig.enableAvatarPartFallback  && !SDKInterface.instance.IsResourceDownloadFinished();
#else
        return false;
#endif
    }

    /// <summary>
    /// 获取包内AssetBundle包信息
    /// </summary>
    public static bool GetAPKPackageInfo()
    {
#if !UNITY_ANDROID
        return false;
#endif
        m_APKABs.Clear();

        IntPtr zipPtr = IntPtr.Zero;

        zipPtr = LibZip.LibZip.zip_open(Application.dataPath, 1, IntPtr.Zero);
        if (IntPtr.Zero == zipPtr)
        {
            UnityEngine.Debug.LogErrorFormat("[Zip] CompressFiles Open File fail {0}", Application.dataPath);
            return false;
        }

        LibZip.zip_stat m_LibZipState = new LibZip.zip_stat();
        int m_PackageFileCount = LibZip.LibZip.zip_get_num_files(zipPtr);
        for (int i = 0; i < m_PackageFileCount; ++i)
        {
            if (LibZip.LibZip.zip_stat_index(zipPtr, i, 0, ref m_LibZipState) == 0)
            {
                string zipStatName = Marshal.PtrToStringAnsi(m_LibZipState.name);

                m_APKABs.Add(Path.GetFileName(zipStatName));
            }
        }

        LibZip.LibZip.zip_close(zipPtr);
        return true;
    }

    public static void LoadAvatarRes()
    {
        if (m_Loaded)
            return;

        m_Loaded = true;

        AssetInst assetInstance = AssetLoader.instance.LoadRes("Actor/AvatarFallback/AvatarFallbackAsset.asset", typeof(ScriptableObject), false);
        if(assetInstance != null)
        {
            m_AvatarFallback = assetInstance.obj as GeAvatarFallbackAsset;
        }
        else
        {
            Debug.LogErrorFormat("LoadAvatarRes AvatarFallbackAsset failed");
            return;
        }

       /* assetInstance = AssetLoader.instance.LoadRes("Actor/AvatarFallback/FirstPackageFile.asset", typeof(ScriptableObject), false);
        if (assetInstance != null)
        {
            m_FirstPackageFiles = assetInstance.obj as FirstPackageFileAsset;
        }
        else
        {
            Debug.LogErrorFormat("LoadAvatarRes FirstPackageFile failed");
            return;
        }*/

        if(!GetAPKPackageInfo())
        {
            return;
        }

        m_LebianABDir = PluginManager.instance.LeBianGetFullResPath() + "assets/AssetBundles/";
        m_ResValid = true;
    }

    /// <summary>
    /// 获取Avatar fallback资源路径
    /// </summary>
    /// <param name="occupation"> 职业类型 </param>
    /// <param name="channel"> 部位 </param>
    /// <returns> fallback资源路径，相对Resource目录 </returns>
    public static string GetFallbackAvatar(int occupation, GeAvatarChannel channel, string resName)
    {
        if(!m_Loaded)
        {
            LoadAvatarRes();
        }

        if(m_AvatarFallback != null)
        {
            // 没有显式指定职业，通过资源名猜测职业
            if(occupation == -1)
            {
                occupation = GetOccupationFromPath(resName);
            }

            if (occupation >= 0)
            {
                occupation = occupation - occupation % 10;       // 转成10的倍数，大职业

                AvatarNames avatar;
                if (m_AvatarFallback.avatarDic.TryGetValue((int)occupation, out avatar))
                {
                    return avatar.m_Names[(int)channel];
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 检查一个资源依赖的Package是否都在本地（APK包中或者热更新目录）
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public static bool IsAssetDependentAvaliable(string assetName)
    {
        if (!m_Loaded)
        {
            LoadAvatarRes();
        }

        // 如果默认Avatar资源或小包资源列表出错，走正常资源加载流程。
        if (!m_ResValid || m_ReadyAssets.Contains(assetName))
            return true;

        AssetLoader.QurreyResPackage(assetName, m_AssetDependentPackages);

        StringBuilder stringBuilder = StringBuilderCache.Acquire();
        for(int i = 0; i < m_AssetDependentPackages.Count; ++i)
        {
            string packageName = m_AssetDependentPackages[i];
            int pos = packageName.LastIndexOf('/');
            if (pos > 0)
                packageName = packageName.Substring(pos + 1);

            // 检查APK包中是否存在
            if (m_APKABs.Contains(packageName))
            {
            //    Debug.LogErrorFormat("IsAssetDependentAvaliable: {0} in APK", packageName);
                continue;
            }

            // 检查乐变更新目录是否存在
            stringBuilder.Clear();
            stringBuilder.AppendFormat("{0}{1}", m_LebianABDir, m_AssetDependentPackages[i]);
            string packagePath = stringBuilder.ToString();
            if (!File.Exists(packagePath))
            {
                StringBuilderCache.Release(stringBuilder);

            //    Debug.LogErrorFormat("IsAssetDependentAvaliable: {0} not in LebianDir: {1}", m_AssetDependentPackages[i], m_LebianABDir);
                m_AssetDependentPackages.Clear();

                return false;
            }
            else
            {
            //    Debug.LogErrorFormat("IsAssetDependentAvaliable: {0} is in LebianDir", packagePath);
            }
        }

        StringBuilderCache.Release(stringBuilder);
        m_AssetDependentPackages.Clear();
        m_ReadyAssets.Add(assetName);

        return true;
    }
}