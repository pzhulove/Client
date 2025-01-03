using System;
using System.Collections.Generic;
using MemoryWriteReaderAnimation;

public class SkillFileListCache
{
#if !LOGIC_SERVER
	static SkillFileListCache()
	{
		// 代码切换模式——切换缓存类型
		CacheType = CacheTypeEnum.CollectLRU;
	}
	
	public enum CacheTypeEnum
	{
		None,
		General,	// 进战斗缓存，出战斗清空
		LRU,		// 缓存最近使用的160个
		CollectLRU  // 缓存最近这关使用的，在出战斗会将上一次战斗的清理掉
	}

	private static CacheTypeEnum cacheType = CacheTypeEnum.None;
	
	/// <summary>
	/// 开关切换模式接口
	/// </summary>
	public static CacheTypeEnum CacheType
	{
		set
		{
			if (cacheType != value)
			{
				mSkillFileCache?.Clear(true);
				if (value == CacheTypeEnum.General)
				{
					mSkillFileCache = new GeneralCache();
				}
				else if(value == CacheTypeEnum.LRU)
				{
					mSkillFileCache = new LRUCache();
				}
				else if(value == CacheTypeEnum.CollectLRU)
				{
					mSkillFileCache = new CollectLRUCache();
				}
			}
			cacheType = value;
		}
	}
	
	/// <summary>
	/// 缓存容量，取关卡最大值-异界1：150+
	/// </summary>
	private readonly static int CACHE_COUNT = 160;
	public static IActionCache<List<SkillFileName>> mSkillFileCache;
	

	/// <summary>
	/// 原本的cache,1局打完删除
	/// </summary>
#if ROBOT_TEST
	private class GeneralCache : CacheStatistics, IActionCache<List<SkillFileName>>
#else
	private class GeneralCache : IActionCache<List<SkillFileName>>
#endif
	{
		private Dictionary<string, List<SkillFileName> > skillFileListCache = new Dictionary<string, List<SkillFileName> >();

		public void Start()
		{
			
		}

		public List<SkillFileName> GetCached(string path)
		{
			List<SkillFileName> list = null;
			if(skillFileListCache.TryGetValue(path,out list))
			{
#if ROBOT_TEST
				GetStatistics(path);
#endif
				return list;
			}
			else {
				list = BeUtility.GetSkillFileList(path);
				AddCached(path, list);
			}

			return list;
		}

		public void AddCached(string path, List<SkillFileName> list)
		{
			if (skillFileListCache.ContainsKey(path))
				return;
#if ROBOT_TEST
			AddStatistics(path);
#endif
			skillFileListCache.Add(path, list);
		}

		public void Clear(bool isForce)
		{
			skillFileListCache.Clear();
		}

#if ROBOT_TEST
		public int GetCacheCount()
		{
			return skillFileListCache.Count;
		}
#endif
	}
	
	/// <summary>
	/// LRUCache,缓存最近使用
	/// </summary>
#if ROBOT_TEST
	private class LRUCache : CacheStatistics, IActionCache<List<SkillFileName>>
#else
	private class LRUCache : IActionCache<List<SkillFileName>>
#endif
	{
		private LRU.LRUCache<List<SkillFileName>> mPathLRUCache = new LRU.LRUCache<List<SkillFileName>>(CACHE_COUNT);
		public void Start()
		{
			
		}

		public List<SkillFileName> GetCached(string path)
		{
			var cache = mPathLRUCache.Get(path);
			if (cache == null)
			{
				cache = BeUtility.GetSkillFileList(path);
				AddCached(path, cache);
			}
#if ROBOT_TEST
			else
			{
				GetStatistics(path);
			}
#endif
			return cache;
		}
		
		public void AddCached(string path, List<SkillFileName> list)
		{
#if ROBOT_TEST
			AddStatistics(path);
#endif
			mPathLRUCache.Put(path, list);
		}
		
		public void Clear(bool isForce)
		{
			if (isForce)
			{
				mPathLRUCache.Clear();
			}
		}
		
#if ROBOT_TEST
		public void ClearLRU()
		{
			mPathLRUCache.Clear();
		}
		
		public int GetCacheCount()
		{
			return mPathLRUCache.Size;
		}
#endif
	}

	
	/// <summary>
	/// CollectLRUCache,缓存上一局，在结束战斗时清理上一局缓存
	/// </summary>
#if ROBOT_TEST
	private class CollectLRUCache : CacheStatistics, IActionCache<List<SkillFileName>>
#else
	private class CollectLRUCache : IActionCache<List<SkillFileName>>
#endif
	{
		private LRU.CollectLRUCache<List<SkillFileName>> mPathLRUCache = new LRU.CollectLRUCache<List<SkillFileName>>(CACHE_COUNT);
		public void Start()
		{
			mPathLRUCache.Start();
		}

		public List<SkillFileName> GetCached(string path)
		{
			var cache = mPathLRUCache.Get(path);
			if (cache == null)
			{
				cache = BeUtility.GetSkillFileList(path);
				AddCached(path, cache);
			}
#if ROBOT_TEST
			else
			{
				GetStatistics(path);
			}
#endif
			return cache;
		}
		
		public void AddCached(string path, List<SkillFileName> list)
		{
#if ROBOT_TEST
			AddStatistics(path);
#endif
			mPathLRUCache.Put(path, list);
		}
		
		public void Clear(bool isForce)
		{
			if (isForce)
			{
				mPathLRUCache.Clear();				
			}
			else
			{
				mPathLRUCache.Collect();
			}
		}

#if ROBOT_TEST
		public int GetCacheCount()
		{
			return mPathLRUCache.Size;
		}
#endif
	}

	public static void Start()
	{
		mSkillFileCache.Start();
	}
	
	public static List<SkillFileName> GetCached(string path)
	{
		return mSkillFileCache.GetCached(path);
	}

	public static void  AddCache(string path,  List<SkillFileName> list)
	{
		mSkillFileCache.AddCached(path, list);
	}

	/*
	public static List<SkillFileName> GetCachedWithoutNew(string path)
	{
		List<SkillFileName> list = null;
		if(skillFileListCache.TryGetValue(path,out list))
		{
			//Logger.LogErrorFormat("Get from skillFileListCache:{0}", path);
			return list;
		}
		return null;
	}*/
	public static void Clear(bool isForce = false)
	{
		mSkillFileCache.Clear(isForce);
	}
	
#if ROBOT_TEST
	public static string GetStatisticsInfo()
	{
		var statistics = mSkillFileCache as CacheStatistics;
		if (statistics != null) 
			return statistics.GetStatisticsInfo() + "Cache Size:" + mSkillFileCache.GetCacheCount();;

		return string.Empty;
	}
	
	public static void ClearStatistics()
	{
		var statistics = mSkillFileCache as CacheStatistics;
		if (statistics != null) statistics.ClearStatistics();
	}
	
	public static void ClearLRU()
	{
		var lru = mSkillFileCache as LRUCache;
		if(lru != null)
			lru.ClearLRU();
	}
	
	public static int GetTotalCount()
	{
		var statistics = mSkillFileCache as CacheStatistics;
		if (statistics != null)
			return statistics.TotalCount();

		return 0;
	}
	
	public static int GetAddCount()
	{
		var statistics = mSkillFileCache as CacheStatistics;
		if (statistics != null)
			return statistics.AddCount;

		return 0;
	}
#endif
	
#else
    private Dictionary<string, List<SkillFileName> > mSkillFileListCache = new Dictionary<string, List<SkillFileName> >();


	public List<SkillFileName> GetCached(string path)
	{
		List<SkillFileName> list = null;
		if(mSkillFileListCache.TryGetValue(path,out list))
		{
			//Logger.LogErrorFormat("Get from skillFileListCache:{0}", path);
			return list;
		}
		else {
			list = BeUtility.GetSkillFileList(path);
			AddCache(path, list);
		}

		return list;
	}

	public void  AddCache(string path,  List<SkillFileName> list)
	{
		if (mSkillFileListCache.ContainsKey(path))
			return;

		mSkillFileListCache.Add(path, list);
	}

	public List<SkillFileName> GetCachedWithoutNew(string path)
	{
		List<SkillFileName> list = null;
		if(mSkillFileListCache.TryGetValue(path,out list))
		{
			//Logger.LogErrorFormat("Get from skillFileListCache:{0}", path);
			return list;
		}
		return null;
	}
	public void Clear(bool isForce = false)
	{
		mSkillFileListCache.Clear();
	}
#endif
}


public class SkillFileNameList : IBufferObject
{
    public List<SkillFileName> m_SkillFileName;

    public void ReadFrom(MemoryBufferReader memoryReader)
    {
        int num = 0;
        memoryReader.Read(ref num);
        if (num > 0)
        {
            m_SkillFileName = new List<SkillFileName>(num);
            for (int i = 0; i < num; ++i)
            {
                SkillFileName skillFileName = new SkillFileName();

                memoryReader.Read(ref skillFileName.fullPath);
                memoryReader.Read(ref skillFileName.folderName);
                memoryReader.Read(ref skillFileName.lastName);
                memoryReader.Read(ref skillFileName.pvpPath);

                memoryReader.Read(ref skillFileName.isCommon);
                memoryReader.Read(ref skillFileName.isPvp);
                memoryReader.Read(ref skillFileName.weaponType);

                if (skillFileName.pvpPath == "")
                    skillFileName.pvpPath = null;

                m_SkillFileName.Add(skillFileName);
            }
        }
    }

    public void WriteTo(MemoryBufferWriter memoryWriter)
    {
        int num = m_SkillFileName.Count;
        memoryWriter.Write(num);

        for (int i = 0; i < num; ++i)
        {
            SkillFileName skillFileName = m_SkillFileName[i];

            memoryWriter.Write(skillFileName.fullPath);
            memoryWriter.Write(skillFileName.folderName);
            memoryWriter.Write(skillFileName.lastName);
            memoryWriter.Write(skillFileName.pvpPath);

            memoryWriter.Write(skillFileName.isCommon);
            memoryWriter.Write(skillFileName.isPvp);
            memoryWriter.Write(skillFileName.weaponType);
        }
    }
}

public class SkillFileName : IEquatable<SkillFileName>
{
	//public string name;
	public string fullPath;
	public bool isCommon;
	public string folderName;
	public string lastName;
	public bool isPvp;
	public string pvpPath;
    public bool isChiji;
    public string chijiPath;

	public int 	indexForFB;
	public int  pvpIndexForFB;
    public int  chijiIndexForFB;

    public int weaponType;//0��ʾ��������ͨ��


	public SkillFileName()
	{

	}

	public SkillFileName(string filename, string parentPath)
	{
        _initWithNameAndPath(filename, parentPath);
	}

    public override bool Equals(object obj)
    {
        return Equals(obj as SkillFileName);
    }

    public bool Equals(SkillFileName other)
    {
        return other != null &&
               fullPath == other.fullPath &&
               isCommon == other.isCommon &&
               folderName == other.folderName &&
               lastName == other.lastName &&
               isPvp == other.isPvp &&
               pvpPath == other.pvpPath &&
               isChiji == other.isChiji &&
               chijiPath == other.chijiPath &&
               indexForFB == other.indexForFB &&
               pvpIndexForFB == other.pvpIndexForFB &&
               chijiIndexForFB == other.chijiIndexForFB &&
               weaponType == other.weaponType;
    }

    public override int GetHashCode()
    {
        int hashCode = 989479725;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(fullPath);
        hashCode = hashCode * -1521134295 + isCommon.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(folderName);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(lastName);
        hashCode = hashCode * -1521134295 + isPvp.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(pvpPath);
        hashCode = hashCode * -1521134295 + isChiji.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(chijiPath);
        hashCode = hashCode * -1521134295 + indexForFB.GetHashCode();
        hashCode = hashCode * -1521134295 + pvpIndexForFB.GetHashCode();
        hashCode = hashCode * -1521134295 + chijiIndexForFB.GetHashCode();
        hashCode = hashCode * -1521134295 + weaponType.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return base.ToString();
    }

    /*	public SkillFileName(string filename,string parentPath,bool isCommon,string lastName)
        {
            _initWithNameAndPath(filename, parentPath);
            this.isCommon = isCommon;
            this.lastName = lastName;
        }*/

    private void _initWithNameAndPath(string filename, string parentPath)
    {
		fullPath = parentPath + "/" + filename;
		if (filename.Contains("common", System.StringComparison.OrdinalIgnoreCase))
			isCommon = true;

		if (filename.Contains("pvp", System.StringComparison.OrdinalIgnoreCase))
			isPvp= true;

        if (filename.Contains("chiji", System.StringComparison.OrdinalIgnoreCase))
            isChiji = true;

        int tmp = filename.LastIndexOf("-");
        if (tmp != -1)
        {
            int lastIndex = 0;
            for (int i = tmp+1; i < filename.Length; ++i)
            {
                if (filename[i] >= '0' && filename[i] <= '9')
                    continue;
                else
                {
                    lastIndex = i;
                    break;
                }
            }

            string sub = filename.Substring(tmp + 1, lastIndex-tmp-1);
            try
            {
                int wt = Convert.ToInt32(sub);
                weaponType = wt;

                filename = filename.Replace("-" + sub, "");
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("{0} parse weaponType error", filename);
            }
        }


		var tokens = filename.Split('/');
		folderName = tokens[0];
		lastName = tokens[tokens.Length-1];
    }

    public static bool operator ==(SkillFileName left, SkillFileName right)
    {
        return EqualityComparer<SkillFileName>.Default.Equals(left, right);
    }

    public static bool operator !=(SkillFileName left, SkillFileName right)
    {
        return !(left == right);
    }
}
