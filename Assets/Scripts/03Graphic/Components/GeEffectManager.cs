using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GeEffectType
{
    EffectUnique,   //GLOBAL_ANIMATION
    EffectMultiple, //一般特效
    EffectGlobal,   //用于buff

    EffectMaxTypeNum,
}

public class GeEffectManager
{
    public struct GeEffectDesc
    {
        public GeEffectDesc(GeEffectEx eff)
        {
            effect = eff;
        }

        public GeEffectEx effect;
    }
    public static GeEffectDesc sInvalidEffDesc = new GeEffectDesc(null);

    protected List<GeEffectDesc>[] m_EffectDescList = new List<GeEffectDesc>[(int)GeEffectType.EffectMaxTypeNum];
    protected bool[] m_IsEffectDescListDirty = new bool[(int)GeEffectType.EffectMaxTypeNum];
    protected bool[] m_IsPaused = new bool[(int)GeEffectType.EffectMaxTypeNum];
    protected bool m_Visible = true;
	public bool useCube;


    public GeEffectManager()
    {
        for(int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++ i)
        {
            m_EffectDescList[i] = new List<GeEffectDesc>();
            m_IsEffectDescListDirty[i] = false;
            m_IsPaused[i] = false;
        }
    }
    public void DoBackToFront()
    {
#if !LOGIC_SERVER
        for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
        {
            List<GeEffectDesc> effectList = m_EffectDescList[i];
            List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GeEffectDesc current = enumerator.Current;
                if (!current.effect.IsDead() && current.effect.IsCreatedInBackMode)
                {
                    current.effect.DoBackToFront();
                }
            }
        }
#endif
    }
    public void Deinit()
    {
        for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
        {
            List<GeEffectDesc> effectList = m_EffectDescList[i];
            effectList.RemoveAll
            (
                f =>
                {
                    //f.effect.Deinit();
					GeEffectPool.instance.RecycleEffect(f.effect);
                    f.effect = null;
                    return true;
                }
            );

            m_IsEffectDescListDirty[i] = false;
            m_IsPaused[i] = false;
        }
    }

	public GeEffectEx GetEffectByName(string path)
	{
		for(int i=0; i<(int)GeEffectType.EffectMaxTypeNum; ++i)
		{
			List<GeEffectDesc> effectList = m_EffectDescList[i];
			List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GeEffectDesc current = enumerator.Current;
				if (0 == current.effect.GetEffectName().CompareTo(path))
					return current.effect;
			}
		}

		return null;
	}

	public GeEffectEx AddEffect(DAssetObject effectRes, EffectsFrames info, float timeLength, Vector3 pos,GameObject parentNode,bool faceLeft,bool forceDisplay = false)
    {
        string effetctPath = Utility.FormatString(effectRes.m_AssetPath);
		if (!Utility.IsStringValid(effetctPath))
			return null;

        GeEffectType effectType = GeEffectType.EffectMultiple;
        if (EffectTimeType.GLOBAL_ANIMATION == info.timetype)
            effectType = GeEffectType.EffectUnique;
        else if (EffectTimeType.BUFF == info.timetype)
            effectType = GeEffectType.EffectGlobal;

        int effectListIdx = (int)effectType;
        if (effectListIdx < (int)GeEffectType.EffectMaxTypeNum)
        {
            List<GeEffectDesc> effectList = m_EffectDescList[effectListIdx];
            if (GeEffectType.EffectUnique == effectType)
            {
                List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GeEffectDesc current = enumerator.Current;
                    if (0 == current.effect.GetEffectName().CompareTo(effetctPath))
                        return current.effect;
                }

                timeLength = 1000.0f;
            }

            //GeEffectEx newEffect = new GeEffectEx();
            //if (null != newEffect)
            //{
            //    if (newEffect.Init(effectRes.m_AssetPath, info, timeLength, pos, faceLeft, parentNode))
            //    {
            //        //newEffect.SetFacing(faceLeft);
            //        effectList.Add(new GeEffectDesc(newEffect));
            //        return newEffect;
            //    }
            //    else
            //		Logger.LogWarningFormat("Init effect with path [{0}] has failed !", effectRes.m_AssetPath);
            //
            //    newEffect = null;
            //}
            //else
            //    Logger.LogError("Allocate effect has failed!");

            if (false && Global.Settings.enableEffectLimit && effectList.Count >= Global.Settings.effectLimitCount)
            {
                Logger.LogProcessFormat("[特效] 超过{0}个，不再创建 {1}", Global.Settings.effectLimitCount, effetctPath);
                return null;
            }

			GeEffectEx newEffect = GeEffectPool.instance.CreateEffect(effetctPath, info, timeLength, pos, faceLeft, parentNode, forceDisplay ? false: useCube);
            if(null != newEffect)
            {
                newEffect.SetVisible(m_Visible);
                effectList.Add(new GeEffectDesc(newEffect));
                return newEffect;
            }
            else
            {
#if UNITY_EDITOR
                Logger.LogErrorFormat("Create effect [{0}] from pool has failed!", effetctPath);
#endif
            }    
        }
        else
            Logger.LogWarningFormat("Unknown effect type {0}!", effectListIdx);

        return null;
    }

    public GeEffectEx AddEffectInBackMode(DAssetObject effectRes, EffectsFrames info, float timeLength, Vector3 pos, GeEntity owner, string attachNode, bool faceLeft, bool forceDisplay)
    {
        string effetctPath = Utility.FormatString(effectRes.m_AssetPath);
        if (!Utility.IsStringValid(effetctPath))
            return null;

        GeEffectType effectType = GeEffectType.EffectMultiple;
        if (EffectTimeType.GLOBAL_ANIMATION == info.timetype)
            effectType = GeEffectType.EffectUnique;
        else if (EffectTimeType.BUFF == info.timetype)
            effectType = GeEffectType.EffectGlobal;

        int effectListIdx = (int)effectType;
        if (effectListIdx < (int)GeEffectType.EffectMaxTypeNum)
        {
            List<GeEffectDesc> effectList = m_EffectDescList[effectListIdx];
            if (GeEffectType.EffectUnique == effectType)
            {
                List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GeEffectDesc current = enumerator.Current;
                    if (0 == current.effect.GetEffectName().CompareTo(effetctPath))
                        return current.effect;
                }

                timeLength = 1000.0f;
            }

            if (false && Global.Settings.enableEffectLimit && effectList.Count >= Global.Settings.effectLimitCount)
            {
                Logger.LogProcessFormat("[特效] 超过{0}个，不再创建 {1}", Global.Settings.effectLimitCount, effetctPath);
                return null;
            }

            GeEffectEx newEffect = GeEffectPool.instance.CreateEffectInBackMode(effetctPath, info, timeLength, pos, faceLeft, owner, attachNode, forceDisplay ? false : useCube);
            if (null != newEffect)
            {
                effectList.Add(new GeEffectDesc(newEffect));
                return newEffect;
            }
            else
            {
#if UNITY_EDITOR
                Logger.LogErrorFormat("Create effect [{0}] from pool has failed!", effetctPath);
#endif
            }
        }
        else
            Logger.LogWarningFormat("Unknown effect type {0}!", effectListIdx);

        return null;
    }


    public void RemoveEffect(GeEffectEx effect, GeEffectType effectType)
    {
        int effectListIdx = (int)effectType;
        if (effectListIdx < (int)GeEffectType.EffectMaxTypeNum)
        {
            m_IsEffectDescListDirty[effectListIdx] = true;
            effect.Remove();
        }
        else
            Logger.LogWarningFormat("Unknown effect type {0}!", effectListIdx);
    }

	float lastHight = 0f;
	public void UpdateTouchGround(float hight, int camp = 99)
	{
        if ( lastHight == hight)
        {
        	//lastHight = hight;
        	return;
        }
        lastHight = hight;
        //Utility.IsFloatZero(hight) ||

        for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
		{
			List<GeEffectDesc> effectList = m_EffectDescList[i];
			List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GeEffectDesc current = enumerator.Current;
				if (current.effect.IsAlwaysTouchGround())
				{
					var pos = current.effect.GetPosition();
					pos.y = 0;
					current.effect.SetPosition(pos);
				}
			}
		}
	}

    public void Update(int deltaTime,GeEffectType effectType)
    {
        for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
        {
            if (i != (int)effectType) continue;
            if (m_IsEffectDescListDirty[i])
            {
                _ClearDeadEffect(i);
            }
        }

        if (!m_IsPaused[(int)effectType])
        {
            List<GeEffectDesc> effectList = m_EffectDescList[(int)effectType];
            for(int i = 0; i < effectList.Count;i++)
            {
                GeEffectDesc current = effectList[i];
                current.effect.Update(deltaTime);
                if (current.effect.IsDead())
                {
                    m_IsEffectDescListDirty[(int)effectType] = true;
                }
            }
        }
    }

    protected void _ClearDeadEffect(int effectType)
    {
        int i = effectType;
        if(i < m_EffectDescList.Length && i >= 0)
        {
            m_EffectDescList[i].RemoveAll(
            eff =>
            {
                if (eff.effect != null && eff.effect.IsDead())
                {
                    GeEffectPool.instance.RecycleEffect(eff.effect);
                    //eff.effect.Deinit();
                    return true;
                }
                return false;
            }
            );

            m_IsEffectDescListDirty[i] = false;
        }
    }

    public void Pause(GeEffectType effectType)
    {
        if (effectType < GeEffectType.EffectUnique || effectType >= GeEffectType.EffectMaxTypeNum)
            return;
        m_IsPaused[(int)effectType] = true;

        for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
        {
            if(i != (int)effectType) continue;

            List<GeEffectDesc> effectList = m_EffectDescList[i];
            List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.effect.Pause();
            }
        }
    }

    //设置特效是否可见
    public void SetEffectVisible(bool isVisible)
    {
        if(m_Visible != isVisible)
        {
            m_Visible = isVisible;
            
            for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
            {
                List<GeEffectDesc> effectList = m_EffectDescList[i];
                List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.effect.SetVisible(m_Visible);
                }
            }
        }
    }

    /// <summary>
    /// 通过设置Layer去控制特效的显示与隐藏
    /// </summary>
    public void SetEffectVisibleByLayer(bool isVisable)
    {
        int layer = isVisable ? 0 : 19;
        for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
        {
            List<GeEffectDesc> effectList = m_EffectDescList[i];
            List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.effect.SetLayer(layer);
            }
        }
    }

    public void Resume(GeEffectType effectType)
    {
        m_IsPaused[(int)effectType] = false;
        for (int i = 0; i < (int)GeEffectType.EffectMaxTypeNum; ++i)
        {
            if (i != (int)effectType) continue;

            List<GeEffectDesc> effectList = m_EffectDescList[i];
            List<GeEffectDesc>.Enumerator enumerator = effectList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.effect.Resume();
            }
        }
    }

    public void ClearAll(GeEffectType effectType)
    {
        int effTpyeIdx = (int)effectType;
        if(effTpyeIdx < m_EffectDescList.Length)
        {
            List<GeEffectDesc> effectList = m_EffectDescList[effTpyeIdx];
            effectList.RemoveAll
            (
                f =>
                {
                    //f.effect.Deinit();
					GeEffectPool.instance.RecycleEffect(f.effect);
                    f.effect = null;
                    return true;
                }
            );
        }
    }
}
