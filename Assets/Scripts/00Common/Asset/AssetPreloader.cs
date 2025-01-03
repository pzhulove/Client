using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 策划表资源 里面如果关联其他资源 可以在代理中实现资源抽取 并添加到预加载列表中
/// 还有 场景资源
/// </summary>
/// <param name="file"></param>
public delegate void ResExtrator(string file);

public class CResPreloader : MonoSingleton<CResPreloader>
{
    public static void DefaultResExtractor(string file)
    {
        CResPreloader.instance.AddRes(file);
    }

	public enum PreloadTag
	{
		NONE = 0,
		HELL = 1
	}

	public enum ResType
	{
		OBJECT = 0,
		RES = 1
	}

    protected class CResDesc
    {
        public string m_FullPath = null;
        public System.Type m_Type;
        public bool m_MustExist = false;
		public int num = 1;
		public int extData = 0;
		public List<string> extDataList = null;
		public PreloadTag tag = PreloadTag.NONE; 
		public ResType resType = ResType.OBJECT;
		public System.Type type = null;
    }

	protected class PriorityKey
	{
		public string key;
		public PreloadTag tag = PreloadTag.NONE;
	}

#if !LOGIC_SERVER
	public void SetTag(PreloadTag tag)
	{
		curTag = tag;
	}

	public void RemovePriorityKeys(PreloadTag tag)
	{
		priorityKeys.RemoveAll(x=>{
			if (x.tag == tag)
				return true;
			return false;
		});

		priorityGameObjectKeys.Clear();
		for(int i=0; i<priorityKeys.Count; ++i)
		{
			priorityGameObjectKeys.Add(priorityKeys[i].key);
		}
	}

	public void Clear(bool allclear=false)
    {
        m_ResPreloadTbl.Clear();
		if (allclear)
		{
			priorityGameObjectKeys.Clear();
			priorityKeys.Clear();
		}
			
        m_StatePreloading = false;
        m_Percentage = 0;
        m_ProcessCount = 0;
    }

	public void AddRes(string resFullPath, bool mustExist = false, int num = 1, ResExtrator extractor = null, int extData = 0, List<string> extDataList = null, ResType resType = ResType.OBJECT, System.Type t = null)
    {
        if (m_StatePreloading)
        {
            Logger.LogWarning("Preloading has begun, you can not add resource during procedural!");
            return;
        }

		bool merge = true;

        if (string.IsNullOrEmpty(resFullPath))
            return;

        if (null != extractor)
            extractor(resFullPath);
        else
        {
			bool needAdd = true;
			if (merge)
			{
				var findItem = m_ResPreloadTbl.Find(x =>{
					return x.m_FullPath == resFullPath;
				});
				if (findItem != null)
				{
					needAdd = false;
					findItem.num += num;
					if (findItem.extData == 0 && extData > 0)
						findItem.extDataList = extDataList;
				}
			}

			if (needAdd){
				CResDesc resDesc = new CResDesc();
				resDesc.m_FullPath = resFullPath;
				resDesc.m_MustExist = mustExist;
				resDesc.m_Type = _GetResType(PathUtil.GetExtension(resFullPath));
				resDesc.num = num;
				resDesc.extData = extData;
				resDesc.extDataList = extDataList;
				resDesc.tag = curTag;
				resDesc.resType = resType;
				resDesc.type = t;
				m_ResPreloadTbl.Add(resDesc);
			}
        }
    }

    public int DoPreLoad()
    {
        m_StatePreloading = true;

        AssetLoader assetLoader = AssetLoader.GetInstance();
        if (null == assetLoader)
        {
            Logger.LogAsset( "Get AssetLoader has failed!");
            return 0;
        }
        
        for(int i = 0; i < m_ResPreloadTbl.Count; ++ i)
        {
            CResDesc curRes = m_ResPreloadTbl[i];
            assetLoader.LoadRes(curRes.m_FullPath, curRes.m_Type, curRes.m_MustExist);
        }

        return 100;
    }

	public int DoPreLoadAsync(bool savePriority=false, bool useSync=false)
    {
        m_StatePreloading = true;

        AssetLoader assetLoader = AssetLoader.GetInstance();
        if (null == assetLoader)
        {
            Logger.LogAsset( "Get AssetLoader has failed!");
            return 0;
        }

		var audioClipType = typeof(AudioClip);
			
        long elapsedTime = 0;
        for (int i = m_ProcessCount; i < m_ResPreloadTbl.Count; ++i)
        {
            CResDesc curRes = m_ResPreloadTbl[i];
            long delta = System.DateTime.Now.Ticks;
            // 
            //             if (curRes.m_FullPath.Contains("prefab") || curRes.m_Type == typeof(GameObject))
            //             {
            //                 GameObject go = CGameObjectPool.instance.GetGameObject(curRes.m_FullPath, enResourceType.BattleScene);
            //                 CGameObjectPool.instance.RecycleGameObject(go);
            //             }
            //             else
            //             {
            //                 AssetInst newAsset = assetLoader.LoadRes(curRes.m_FullPath, curRes.m_Type, curRes.m_MustExist);
            //                 newAsset.Release();
            //             }

			//Logger.LogErrorFormat("preload res:{0} num:{1}", curRes.m_FullPath, curRes.num);
			int num = curRes.num;
            //num = 1;

            if (curRes.resType == ResType.RES/*curRes.m_FullPath.StartsWith("Sound")*/)
            {
                var res = AssetLoader.instance.LoadRes(curRes.m_FullPath, curRes.type);
                if (res.obj != null && curRes.type == audioClipType)
                    AudioManager.instance.AddPreloadSound(res.obj);
            }

            else
            {
                //string path = Tenmove.Runtime.Utility.Path.ChangeExtension( curRes.m_FullPath.Replace("_ModelData",null),".prefab");
                string path = curRes.m_FullPath.Replace("_ModelData", null);
                CGameObjectPool.instance.PrepareGameObject(path, enResourceType.BattleScene, num);
            }

			PreloadAnimation(curRes);

			if (!useSync)
			{
				elapsedTime += System.DateTime.Now.Ticks - delta;
				if(elapsedTime > TIME_SLICE_TICKES)
				{
					m_ProcessCount = i;
					Logger.LogFormat("Step preload return {0}%.",(int)((float)m_ProcessCount / m_ResPreloadTbl.Count * 100.0f));
					return (int)((float)m_ProcessCount / m_ResPreloadTbl.Count);
				}
			}
        }

		if (savePriority)
		{
			for(int i=0; i<m_ResPreloadTbl.Count; ++i)
			{
				PriorityKey pk = new PriorityKey();
				pk.key = CFileManager.EraseExtension(m_ResPreloadTbl[i].m_FullPath);
				pk.tag = m_ResPreloadTbl[i].tag;
				priorityKeys.Add(pk);

				priorityGameObjectKeys.Add(pk.key);
			}
		}
			
		bool writeFile = true;

		if (writeFile)
		{
			List<string> contents = new List<string>();
			contents.Add("---------------------------------");
			for(int i=0; i<m_ResPreloadTbl.Count; ++i)
			{
				contents.Add(m_ResPreloadTbl[i].m_FullPath);
				if (m_ResPreloadTbl[i].extDataList != null)
				{
					for(int j=0; j<m_ResPreloadTbl[i].extDataList.Count; ++j)
						contents.Add("--" + m_ResPreloadTbl[i].extDataList[j]);		
				}
			}
				
			ExceptionManager.GetInstance().PrintPreloadRes(contents);

			contents.Clear();
			var cachedID = GameClient.PreloadManager.cachedResID;
			contents.Add("RES ID---------------------------------");
			for(int i=0; i<cachedID.Count; ++i)
				contents.Add(cachedID[i].ToString());
			ExceptionManager.GetInstance().PrintPreloadRes(contents);
		}
        m_ResPreloadTbl.Clear();

        return 100;
    }

	private void PreloadAnimation(CResDesc desc)
	{
		if (desc != null && desc.extData > 0 && desc.extDataList != null)
		{
			var obj = CGameObjectPool.instance.GetGameObject(desc.m_FullPath, enResourceType.BattleScene,(uint)GameObjectPoolFlag.None);
			if (obj != null)
			{
				var comAnimProxy = obj.GetComponent<GeAnimDescProxy>();
				if (comAnimProxy != null)
				{
/*					if (desc.extData == 2)
					{
						var tmpArray = desc.extDataList.ToArray();
						for(int i=0; i<tmpArray.Length; ++i)
							Logger.LogErrorFormat("preload attach animation:{0}", tmpArray[i]);
					}*/
					comAnimProxy.PreloadAction(desc.extDataList.ToArray());
					//desc.extDataList = null;
				}

				CGameObjectPool.instance.RecycleGameObject(obj);
			}
		}
	}

    public int GetLoadPercentage()
    {
        return m_Percentage;
    }

    protected void _LoadRes(string resFullPath)
    {
    }

    protected IEnumerator _LoadResAsync()
    {
        yield return null;
    }

    protected System.Type _GetResType(string fullPath)
    {
        string ext = PathUtil.GetExtension(fullPath);
        if (string.Equals(ext, ".prefab", System.StringComparison.OrdinalIgnoreCase))
            return typeof(GameObject);
        else if (string.Equals(ext, ".asset", System.StringComparison.OrdinalIgnoreCase))
            return typeof(ScriptableObject);
        else
            return typeof(UnityEngine.Object);
    }

#else
	public void SetTag(PreloadTag tag){}
	public void RemovePriorityKeys(PreloadTag tag){}
	public void Clear(bool allclear=false){}
	public void AddRes(string resFullPath, bool mustExist = false, int num = 1, ResExtrator extractor = null, int extData = 0, List<string> extDataList = null, ResType resType = ResType.OBJECT, System.Type t = null){}
	public int DoPreLoad(){return 0;}
	public int DoPreLoadAsync(bool savePriority=false, bool useSync=false){return 0;}
	public int GetLoadPercentage(){return 0;}
	private void PreloadAnimation(CResDesc desc){}
	protected void _LoadRes(string resFullPath){}
	protected IEnumerator _LoadResAsync(){return null;}
	protected System.Type _GetResType(string fullPath){return typeof(int);}
#endif


    #region 成员
    
	protected PreloadTag curTag = PreloadTag.NONE;
    protected bool m_StatePreloading = false;
    protected int m_Percentage = 0;
    protected int m_ProcessCount = 0;
    protected List<CResDesc> m_ResPreloadTbl = new List<CResDesc>();
	public List<string> priorityGameObjectKeys = new List<string>();
	protected List<PriorityKey> priorityKeys = new List<PriorityKey>();

    protected readonly long TIME_SLICE_TICKES = 10000000/*160000*/; /// 16毫秒作为一个装载的时间片

    #endregion


}
