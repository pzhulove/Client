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
    /// ��ͨ����Դ������ȷ��ְҵ
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

    // �Ƿ��ȫ������AvatarPartFallback�����Ƿ��ڳ������
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
    /// ��ȡ����AssetBundle����Ϣ
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
    /// ��ȡAvatar fallback��Դ·��
    /// </summary>
    /// <param name="occupation"> ְҵ���� </param>
    /// <param name="channel"> ��λ </param>
    /// <returns> fallback��Դ·�������ResourceĿ¼ </returns>
    public static string GetFallbackAvatar(int occupation, GeAvatarChannel channel, string resName)
    {
        if(!m_Loaded)
        {
            LoadAvatarRes();
        }

        if(m_AvatarFallback != null)
        {
            // û����ʽָ��ְҵ��ͨ����Դ���²�ְҵ
            if(occupation == -1)
            {
                occupation = GetOccupationFromPath(resName);
            }

            if (occupation >= 0)
            {
                occupation = occupation - occupation % 10;       // ת��10�ı�������ְҵ

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
    /// ���һ����Դ������Package�Ƿ��ڱ��أ�APK���л����ȸ���Ŀ¼��
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public static bool IsAssetDependentAvaliable(string assetName)
    {
        if (!m_Loaded)
        {
            LoadAvatarRes();
        }

        // ���Ĭ��Avatar��Դ��С����Դ�б������������Դ�������̡�
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

            // ���APK�����Ƿ����
            if (m_APKABs.Contains(packageName))
            {
            //    Debug.LogErrorFormat("IsAssetDependentAvaliable: {0} in APK", packageName);
                continue;
            }

            // ����ֱ����Ŀ¼�Ƿ����
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