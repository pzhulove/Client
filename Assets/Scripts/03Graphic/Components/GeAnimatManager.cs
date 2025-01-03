using UnityEngine;
using System.Collections.Generic;

using AnimatHandle = System.UInt32;

public class GeAnimatManager
{

    public class GeAnimatDesc
    {
        public GeAnimatDesc(GeAnimat a, string n)
        {
            animat = a;
            name = n;
        }

        public string name;
        public GeAnimat animat;
    }

    public class GeAnimatCache
    {
        public GeAnimatCache(AnimatHandle h,GeAnimatDesc a)
        {
            animatDesc = a;
            handle = h;
        }

        public AnimatHandle handle;
        public GeAnimatDesc animatDesc;
    }

    protected List<GeAnimatCache> m_AnimatStack = new List<GeAnimatCache>();

    public GeAnimatDesc sInvalidAnimatDesc = new GeAnimatDesc(null, "");

    protected List<GeAnimatDesc> m_AnimatDescList = new List<GeAnimatDesc>();
    protected uint m_CurAnimHandleCnt = 0;

    protected bool m_IsDirty = false;

    public GeAnimatManager()
    {
    }

    public bool Init()
    {
        return true;
    }

    public void Deinit()
    {
        ClearAll();

        m_AnimatDescList.RemoveAll(
            a =>
            {
                if (null != a && null != a.animat)
                    a.animat.Clear(true);
                return true;
            }
            );
    }

    public void AddEffectMaterial(string animatName, string shaderName, DAnimatParamDesc[] param, GameObject[] meshes)
    {
        GeAnimatDesc curMat = GetEffectMaterialDesc(animatName);
        if (null == curMat.animat)
        {
            GeAnimat newAnimat = new GeAnimat();
            newAnimat.Init(animatName, shaderName, param, meshes);
            m_AnimatDescList.Add(new GeAnimatDesc(newAnimat, animatName));
        }
    }

    public GeAnimatDesc GetEffectMaterialDesc(string animatName)
    {
        for (int i = 0,icnt = m_AnimatDescList.Count; i < icnt; ++i)
        {
            if (0 == m_AnimatDescList[i].name.CompareTo(animatName))
                return m_AnimatDescList[i];
        }

        return sInvalidAnimatDesc;
    }

    public void AppendObject(GameObject[] obj)
    {
        for (int i = 0; i < m_AnimatDescList.Count; ++i)
        {
            m_AnimatDescList[i].animat.AppendObject(obj);
        }

        GeAnimatCache top = _GetTopAnimatCache();
        if(null != top)
            top.animatDesc.animat.Apply(top.animatDesc.animat.GetTimeLen(), top.animatDesc.animat.GetElapsedTime(), top.animatDesc.animat.IsAnimate(), top.animatDesc.animat.IsRecover());
    }

    public void RemoveObject(GameObject[] obj)
    {
        for (int i = 0; i < m_AnimatDescList.Count; ++i)
        {
            m_AnimatDescList[i].animat.RemoveObject(obj);
        }
    }

    public AnimatHandle PushAnimat(string animatName, float time, bool enableAnim, bool recover)
    {
        if (!string.IsNullOrEmpty(animatName))
        {
            GeAnimatDesc curMat = GetEffectMaterialDesc(animatName);
            if (null != curMat.animat)
            {
                AnimatHandle hNewAnimat = _AllocHandle();
                m_AnimatStack.Add(new GeAnimatCache(hNewAnimat, curMat));

                curMat.animat.Apply(time, 0.0f, enableAnim, recover);

                return hNewAnimat;
            }
/*            else
                Logger.LogErrorFormat("Can not find animat named \"{0}\"!", animatName);*/
        }
/*        else
            Logger.LogError("Animat name can not be null!");*/

        return uint.MaxValue;
    }

    public void RemoveAnimat(AnimatHandle handle)
    {
        for(int i = 0; i < m_AnimatStack.Count; ++ i)
        {
            GeAnimatCache curAnimatCache = m_AnimatStack[i];
            if(null == curAnimatCache) continue;

            if (handle == curAnimatCache.handle)
            {
                if (1 == m_AnimatStack.Count)
                {
                    if (null != curAnimatCache.animatDesc && null != curAnimatCache.animatDesc.animat)
                        curAnimatCache.animatDesc.animat.Recover();

                    m_AnimatStack.RemoveAt(i);
                }
                else if(i == m_AnimatStack.Count - 1)
                {
                    GeAnimatCache topCache = curAnimatCache;
                    m_AnimatStack.RemoveAt(m_AnimatStack.Count - 1);

                    GeAnimatCache nextCache = null;
                    while (m_AnimatStack.Count > 0)
                    {
                        nextCache = m_AnimatStack[m_AnimatStack.Count - 1];
                        m_AnimatStack.RemoveAt(m_AnimatStack.Count - 1);

                        if (null == nextCache) continue;
                        if (nextCache.animatDesc.animat.IsFinished())
                            continue;

                        if (null != nextCache.animatDesc && null != nextCache.animatDesc.animat)
                        {
                            GeAnimat nextAnimat = nextCache.animatDesc.animat;
                            nextAnimat.Apply(nextAnimat.GetTimeLen(), nextAnimat.GetElapsedTime(), nextAnimat.IsAnimate(), nextAnimat.IsRecover());
                            m_AnimatStack.Add(nextCache);
                            break;
                        }
                    }
                }
                else
                    m_AnimatStack.RemoveAt(i);
                return;
            }
        }

        Logger.LogWarningFormat("Can not find animat with handle \"{0}\"!", handle);
    }

    public void Pause()
    {
        for (int i = 0; i < m_AnimatStack.Count; ++i)
        {
            if (null != m_AnimatStack[i] && null != m_AnimatStack[i].animatDesc && null != m_AnimatStack[i].animatDesc.animat)
                m_AnimatStack[i].animatDesc.animat.Pause();
        }
    }

    public void Resume()
    {
        for (int i = 0; i < m_AnimatStack.Count; ++i)
        {
            if (null != m_AnimatStack[i] && null != m_AnimatStack[i].animatDesc && null != m_AnimatStack[i].animatDesc.animat)
                m_AnimatStack[i].animatDesc.animat.Resume();
        }
    }

    public void Update(int deltaTime, GameObject obj)
    {
        GeAnimatCache recover = _GetTopAnimatCache();

        int count = m_AnimatStack.Count;
        int lastIdx = count - 1;
        for (int i = 0; i < count; ++ i)
        {
            GeAnimatCache cache = m_AnimatStack[i];
            if (null != cache && null != cache.animatDesc && null != cache.animatDesc.animat)
            {
                cache.animatDesc.animat.Update(i != lastIdx, deltaTime, obj);
                if (cache.animatDesc.animat.IsFinished())
                    m_IsDirty = true;
            }
            else
                m_IsDirty = true;
        }
        
        if (m_IsDirty)
            _RemoveDeadAnimat();

        if (m_AnimatStack.Count !=  count)
        {
            GeAnimatCache top = _GetTopAnimatCache();
            if (null != top)
                top.animatDesc.animat.Apply(top.animatDesc.animat.GetTimeLen(), top.animatDesc.animat.GetElapsedTime(), top.animatDesc.animat.IsAnimate(), top.animatDesc.animat.IsRecover());
            else
                if (null != recover)
                    recover.animatDesc.animat.Recover();
        }
    }

    protected void _RemoveDeadAnimat()
    {
        m_AnimatStack.RemoveAll(
            cache =>
            {
                if (null != cache && null != cache.animatDesc && null != cache.animatDesc.animat)
                    return cache.animatDesc.animat.IsFinished();
                else
                    return true;
            }
            );

        m_IsDirty = false;
    }

    public void ClearAll()
    {
        GeAnimatCache topCache = _GetTopAnimatCache();
        if (null != topCache)
            topCache.animatDesc.animat.Recover();

        m_AnimatStack.Clear();
    }

    protected uint _AllocHandle()
    {
        if (m_CurAnimHandleCnt + 1 == uint.MaxValue)
            m_CurAnimHandleCnt = 0;
        return m_CurAnimHandleCnt++;
    }

    protected GeAnimatCache _GetTopAnimatCache()
    {
        for (int i = m_AnimatStack.Count - 1; i >= 0; --i)
        {
            GeAnimatCache curAnimatCache = m_AnimatStack[i];
            if (null == curAnimatCache) continue;

            if (null != curAnimatCache.animatDesc && null != curAnimatCache.animatDesc.animat)
                return curAnimatCache;
        }

        return null;
    }
}

