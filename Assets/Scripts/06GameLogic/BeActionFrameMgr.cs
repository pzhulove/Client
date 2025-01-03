using System.Collections.Generic;

#if !LOGIC_SERVER
public interface IActionCache<T>
{
	void Start();
	T GetCached(string path);
	void AddCached(string path, T info);
	void Clear(bool isForce);
#if ROBOT_TEST
	int GetCacheCount();
#endif
}
#endif

#if ROBOT_TEST
public class CacheStatistics
{
	HashSet<string> totalSet = new HashSet<string>();
	public void AddStatistics(string path)
	{
		totalSet.Add(path);
		AddCount++;
	}

	public void GetStatistics(string path)
	{
		totalSet.Add(path);
		GetCount++;
	}
	
	public int AddCount;
	public int GetCount;

	public string GetStatisticsInfo()
	{
		return "Add:" + AddCount + " Get:" + GetCount + " Total:" + totalSet.Count + " Hit Rate:" + GetHitRate() * 100f + "%";
	}

	public int TotalCount()
	{
		return totalSet.Count;
	}

	public void ClearStatistics()
	{
		AddCount = 0;
		GetCount = 0;
		totalSet.Clear();
	}
	
	private float GetHitRate()
	{
		int useCount = AddCount + GetCount;
		if (useCount == 0)
		{
			return 0f;
		}
		else
		{
			return GetCount / (float)useCount;
		}
	}
}
#endif

public class BeActionFrameMgr
{
#if !LOGIC_SERVER
	static BeActionFrameMgr()
	{
		// 代码切换模式——切换缓存类型
		CacheType = CacheTypeEnum.General;
	}
	
	public enum CacheTypeEnum
	{
		None,
		General,	// 进战斗缓存，出战斗清空
		LRU,		// 缓存最近使用的1000个
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
				mActionCache?.Clear(true);
				if (value == CacheTypeEnum.General)
				{
					mActionCache = new GeneralCache();
				}
				else if(value == CacheTypeEnum.LRU)
				{
					mActionCache = new LRUCache();
				}
				else if(value == CacheTypeEnum.CollectLRU)
				{
					mActionCache = new CollectLRUCache();
				}
			}
			cacheType = value;
		}
	}

	/// <summary>
	/// 缓存容量，取关卡最大值-异界1：960+
	/// </summary>
	private readonly static int CACHE_COUNT = 1000;
	private static IActionCache<BDEntityActionInfo> mActionCache;

	
#if ROBOT_TEST
	private class GeneralCache : CacheStatistics, IActionCache<BDEntityActionInfo>
#else
	private class GeneralCache : IActionCache<BDEntityActionInfo>
#endif
	{
		class refInfo
		{
			public refInfo(){}
			public BDEntityActionInfo info;
			public int                refCount;
		}
		private Dictionary<string,refInfo> mEntityActionInfoCache = new Dictionary<string,refInfo>();

		public void Start()
		{
			
		}

		public BDEntityActionInfo GetCached(string path)
		{
			refInfo info = null;
			if(mEntityActionInfoCache.TryGetValue(path,out info))
			{
#if ROBOT_TEST
				GetStatistics(path);
#endif
				info.refCount++;
				return info.info;
			}

			return null;
		}

		public void AddCached(string path, BDEntityActionInfo actInfo)
		{
			if(mEntityActionInfoCache.ContainsKey(path))
			{
				Logger.LogWarningFormat("[Cached ActionInfo Add Error]{0}",path);
				return;
			}
#if ROBOT_TEST
			AddStatistics(path);
#endif
			actInfo.key = path;
			mEntityActionInfoCache.Add(path,new refInfo(){info=actInfo,refCount=1});
		}

		public void Clear(bool isForce)
		{
			mEntityActionInfoCache.Clear();
		}

#if ROBOT_TEST
		public int GetCacheCount()
		{
			return mEntityActionInfoCache.Count;
		}
#endif
	}
	
	/// <summary>
	/// LRUcache,缓存最近使用
	/// </summary>
	
#if ROBOT_TEST
	private class LRUCache : CacheStatistics, IActionCache<BDEntityActionInfo>
#else
	private class LRUCache : IActionCache<BDEntityActionInfo>
#endif
	{
		// 取单局最大（异界1）
		private LRU.LRUCache<BDEntityActionInfo> mActionInfoLRUCache = new LRU.LRUCache<BDEntityActionInfo>(CACHE_COUNT);
		public void Start()
		{
			
		}

		public BDEntityActionInfo GetCached(string path)
		{
			if (mActionInfoLRUCache != null)
			{
				var cache = mActionInfoLRUCache.Get(path);
#if ROBOT_TEST
				if (cache != null)
				{
					GetStatistics(path);
				}
#endif
				return cache;
			}
			
			return null;
		}

		public void AddCached(string path, BDEntityActionInfo actInfo)
		{
			if (mActionInfoLRUCache != null)
			{
#if ROBOT_TEST
				AddStatistics(path);
#endif
				mActionInfoLRUCache.Put(path, actInfo);
			} 
		}
		
		public void Clear(bool isForce)
		{
			if (isForce)
			{
				mActionInfoLRUCache.Clear();
			}
		}

#if ROBOT_TEST
		public void ClearLRU()
		{
			mActionInfoLRUCache.Clear();
		}
		
		public int GetCacheCount()
		{
			return mActionInfoLRUCache.Size;
		}
#endif
	}

	/// <summary>
	/// CollectLRUCache,缓存上一局，在结束战斗时清理上一局缓存
	/// </summary>
#if ROBOT_TEST
	private class CollectLRUCache : CacheStatistics, IActionCache<BDEntityActionInfo>
#else
	private class CollectLRUCache : IActionCache<BDEntityActionInfo>
#endif
	{
		// 取单局最大（异界1 960+）
		private LRU.CollectLRUCache<BDEntityActionInfo> mActionInfoLRUCache = new LRU.CollectLRUCache<BDEntityActionInfo>(CACHE_COUNT);
		public void Start()
		{
			mActionInfoLRUCache.Start();
		}

		public BDEntityActionInfo GetCached(string path)
		{
			if (mActionInfoLRUCache != null)
			{
				var cache = mActionInfoLRUCache.Get(path);
#if ROBOT_TEST
				if (cache != null)
				{
					GetStatistics(path);
				}
#endif
				return cache;
			}
			
			return null;
		}

		public void AddCached(string path, BDEntityActionInfo actInfo)
		{
			if (mActionInfoLRUCache != null)
			{
#if ROBOT_TEST
				AddStatistics(path);
#endif
				mActionInfoLRUCache.Put(path, actInfo);
			} 
		}
		
		public void Clear(bool isForce)
		{
			if (isForce)
			{
				mActionInfoLRUCache.Clear();				
			}
			else
			{
				mActionInfoLRUCache.Collect();
			}
		}

#if ROBOT_TEST
		public int GetCacheCount()
		{
			return mActionInfoLRUCache.Size;
		}
#endif
	}


	
    private static Dictionary<string, UnityEngine.Object> skillObjectCache = new Dictionary<string, UnityEngine.Object>();
    public static UnityEngine.Object GetSkillObjectCache(string res)
	{
		if (skillObjectCache.ContainsKey(res))
			return skillObjectCache[res];

		UnityEngine.Object skillRes = AssetLoader.instance.LoadRes(res, typeof(DSkillData)).obj;
		//DSkillData data = skillRes as DSkillData;
		skillObjectCache.Add(res, skillRes);

		return skillRes;
	}
    
    public static void ClearSkillObjectCache()
	{
		skillObjectCache.Clear();
	}

    public static void Start()
    {
	    mActionCache.Start();
    }


    public static BDEntityActionInfo GetCached(string path)
    {
	    return mActionCache.GetCached(path);

    }

    public static void AddCached(string path,BDEntityActionInfo actInfo)
    {
	    mActionCache.AddCached(path, actInfo);
    }

    /*public static void ReleaseActInfo(BDEntityActionInfo actInfo)
    {
       refInfo info = null;
        if(sEntityActionInfoCache.TryGetValue(actInfo.key,out info))
        {
            info.refCount--;
            if(info.refCount == 0)
            {
                sEntityActionInfoCache.Remove(actInfo.key);
            }
        }
    }*/

    public static void Clear(bool isForce = false)
    {
	    mActionCache.Clear(false);
    }

    
#if ROBOT_TEST
	public static void ClearLRU()
	{
		var lru = mActionCache as LRUCache;
		if(lru != null)
			lru.ClearLRU();
	}
	public static string GetStatisticsInfo()
	{
		var statistics = mActionCache as CacheStatistics;
		if (statistics != null) 
			return statistics.GetStatisticsInfo() + "Cache Size:" + mActionCache.GetCacheCount();

		return string.Empty;
	}

	public static int GetAddCount()
	{
		var statistics = mActionCache as CacheStatistics;
		if (statistics != null)
			return statistics.AddCount;

		return 0;
	}
	
	public static int GetTotalCount()
	{
		var statistics = mActionCache as CacheStatistics;
		if (statistics != null)
			return statistics.TotalCount();

		return 0;
	}
	
	public static void ClearStatistics()
	{
		var statistics = mActionCache as CacheStatistics;
		if (statistics != null) statistics.ClearStatistics();
	}
#endif
	
#else
	class refInfo
	{
		public refInfo(){}
		public BDEntityActionInfo info;
		public int                refCount;
	}
    private Dictionary<string,refInfo> mEntityActionInfoCache = new Dictionary<string,refInfo>();
    public BDEntityActionInfo GetCached(string path)
    {
        refInfo info = null;
        if(mEntityActionInfoCache.TryGetValue(path,out info))
        {
            info.refCount++;
            return info.info;
        }

        return null;
    }

    public void AddCached(string path,BDEntityActionInfo actInfo)
    {
        if(mEntityActionInfoCache.ContainsKey(path))
        {
            Logger.LogWarningFormat("[Cached ActionInfo Add Error]{0}",path);
            return;
        }
        actInfo.key = path;
        mEntityActionInfoCache.Add(path,new refInfo(){info=actInfo,refCount=1});
    }

    public void ReleaseActInfo(BDEntityActionInfo actInfo)
    {
       refInfo info = null;
        if(mEntityActionInfoCache.TryGetValue(actInfo.key,out info))
        {
            info.refCount--;
            if(info.refCount == 0)
            {
                mEntityActionInfoCache.Remove(actInfo.key);
            }
        }
    }

    public void Clear(bool isForce = false)
    {
        mEntityActionInfoCache.Clear();
    }
    
   
#endif

}
