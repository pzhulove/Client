using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GeEffectPool : Singleton<GeEffectPool>, IObjectPool
{
	#region poolInfo

	string poolKey = "GeEffectPool";
	string poolName = "特效池";

	int totalInst = 0;
	int remainInst = 0;

	public string GetPoolName ()
	{
		return poolName;
	}

	public string GetPoolInfo()
	{
		return string.Format ("{0}/{1}", remainInst, totalInst);
	}

	public string GetPoolDetailInfo ()
	{
		return "detailInfo";
	}
    #endregion

#if LOGIC_SERVER
	public GeEffectEx CreateEffect(string effectRes, EffectsFrames info, float time, Vector3 initPos, bool bFaceLeft, GameObject parentObj = null, bool useCube=false){return null;}
		public GeEffectEx CreateEffectInBackMode(string effectRes, EffectsFrames info, float time, Vector3 initPos, bool bFaceLeft, GeEntity owner, string attachNode, bool useCube = false) { return null; }
    public void RecycleEffect(GeEffectEx effect){}
	public void ClearAll(){}
#else

    public class SEffectDesc
    {
        public string m_Key;
        public Queue<GeEffectEx> m_EffectQueue = new Queue<GeEffectEx>();
    }

    List<SEffectDesc> m_EffectPool = new List<SEffectDesc>();
	List<SEffectDesc> useCubeEffectPool = new List<SEffectDesc>();

	GameObject poolRoot;

    private static string m_CubeEffectName = "Effects/DummyEffect/DummyEffect";




    public override void Init()
    {
        base.Init();
		CreateRoot();

		totalInst = 0;
		remainInst = 0;

		CPoolManager.GetInstance ().RegisterPool (poolKey, this);
    }

    public override void UnInit()
    {
        base.UnInit();
    }
    public GeEffectEx CreateEffectInBackMode(string effectRes, EffectsFrames info, float time, Vector3 initPos, bool bFaceLeft, GeEntity owner, string attachNode, bool useCube = false)
    {
        var geEffect = new GeEffectEx();
        if (!geEffect.InitInBackMode(effectRes, info, time, initPos, bFaceLeft, owner, attachNode, useCube))
        {
            Logger.LogWarningFormat("Init effect with path [{0}] has failed !", effectRes);
            geEffect = null;
        }
        else
        {
            totalInst++;
        }
        return geEffect;
    }

    public GeEffectEx CreateEffect(string effectRes, EffectsFrames info, float time, Vector3 initPos, bool bFaceLeft, GameObject parentObj = null, bool useCube=false)
    {
		List<SEffectDesc> pool = m_EffectPool;

        string poolResname = effectRes;

        if (useCube)
        {
            pool = useCubeEffectPool;
            poolResname = m_CubeEffectName;
        }


        GeEffectEx geEffect = null;
		for (int i = 0,icnt = pool.Count;i<icnt;++i)
        {
			SEffectDesc curDesc = pool[i];
            if (curDesc.m_Key.Equals(poolResname, System.StringComparison.OrdinalIgnoreCase))
            {
                Queue<GeEffectEx> queue = curDesc.m_EffectQueue;
                while (queue.Count > 0)
                {
                    geEffect = queue.Dequeue();
                    if(null != geEffect)
                    {
						//Logger.LogErrorFormat("[POOL]get from effect pool:{0}", effectRes);

                        if( geEffect.OnReuse(info, time, initPos, bFaceLeft, parentObj, useCube) )
						{
                        	geEffect.SetVisible(true);

							remainInst--;
                        	return geEffect;
						}
                    }

                    geEffect = null;
                }
            }
        }

        /// 没有创建新的
        geEffect = new GeEffectEx();
		if(!geEffect.Init(effectRes, info, time, initPos, bFaceLeft, parentObj, useCube))
        {
            Logger.LogWarningFormat("Init effect with path [{0}] has failed !", effectRes);
            geEffect = null;
        }
		else {
			totalInst++;
		}
			
		//Logger.LogErrorFormat("[POOL]create new effect:{0}", effectRes);
        return geEffect;
    }

    public void RecycleEffect(GeEffectEx effect)
    {
        if (null == effect)
            return;

		//!!说明已经被释放了
		if(effect.GetRootNode() == null)
			return;
        if (effect.IsCreatedInBackMode)
        {
            effect.Deinit();
            return;
        }
        effect.OnRecycle();
		AttachToRoot(effect);


		remainInst++;


		List<SEffectDesc> pool = m_EffectPool;
        string effectKey = effect.GetEffectName();

        if (effect.useCube)
        {
            pool = useCubeEffectPool;
            effectKey = m_CubeEffectName;
        }


		for (int i = 0, icnt = pool.Count; i < icnt; ++i)
        {
			SEffectDesc curDesc = pool[i];
            if (curDesc.m_Key.Equals(effectKey, System.StringComparison.OrdinalIgnoreCase))
            {
                Queue<GeEffectEx> queue = curDesc.m_EffectQueue;
                queue.Enqueue(effect);
				effect.SetVisible(false);
				//Logger.LogErrorFormat("[POOL]put back effect:{0}", effectKey);

                return;
            }
        }

        SEffectDesc newEffDesc = new SEffectDesc();
        newEffDesc.m_Key = effectKey;
        effect.SetVisible(false);
        newEffDesc.m_EffectQueue.Enqueue(effect);
		pool.Add(newEffDesc);

		//Logger.LogErrorFormat("[POOL]put back effect:{0}", effectKey);
    }

	protected void SubClear(List<SEffectDesc> pool)
	{
		for (int i = 0, icnt = pool.Count; i < icnt; ++i)
		{
			SEffectDesc curDesc = pool[i];
			while(curDesc.m_EffectQueue.Count > 0)
			{
				var effect = curDesc.m_EffectQueue.Dequeue();
				effect.Deinit();
				effect = null;


			}

			//Logger.LogErrorFormat("clear {0} count:{1}", curDesc.m_Key, curDesc.m_EffectQueue.Count);
			curDesc.m_EffectQueue = null;
		}

		pool.Clear();
	}

	public void ClearAll()
	{
		SubClear(m_EffectPool);
		SubClear(useCubeEffectPool);

		if (poolRoot != null)
		{
			GameObject.Destroy(poolRoot);
			poolRoot = null;
		}

		remainInst = 0;
		totalInst = 0;
	}

	void CreateRoot()
	{
		if(null == poolRoot)
			this.poolRoot = new GameObject("GeEffectPool");
	}

	void AttachToRoot(GeEffectEx effect)
	{
		CreateRoot();
		Battle.GeUtility.AttachTo(effect.GetRootNode(), poolRoot);
	}
	#endif
}
