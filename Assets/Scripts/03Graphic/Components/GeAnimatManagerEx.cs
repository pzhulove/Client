using UnityEngine;
using System.Collections.Generic;

using AnimatHandle = System.UInt32;

public partial class GeAnimatManagerEx
{
    public class GeAnimatDescEx
    {
        public GeAnimatDescEx(GameObject o, GeMeshDescProxy[] a)
        {
            animatProxy = a;
            obj = o;
        }

        public GameObject obj;
        public GeMeshDescProxy[] animatProxy;
    }

    public class GeAnimatCacheEx
    {
        public GeAnimatCacheEx(string n,AnimatHandle h,float tl,bool ea,bool nr)
        {
            name = n;
            handle = h;

            timeLen = tl;
            enableAnim = ea;
            needRecover = nr;

            timePos = 0.0f;
            isPlaying = true;
            isFinished = false;
        }

        public void Update(float delta)
        {
            if (isPlaying)
            {
                timePos += delta * 0.001f;
                if (timeLen > 0.0f && timePos > timeLen)
                {
                    isPlaying = false;
                    isFinished = true;
                }
            }
        }

        public string name;
        public AnimatHandle handle;

        public float timeLen;
        public float timePos;
        public bool enableAnim;
        public bool needRecover;

        public bool isPlaying;
        public bool isFinished;
    }

    protected List<GeAnimatDescEx> m_AnimatDescList = new List<GeAnimatDescEx>();
    protected List<GeAnimatCacheEx> m_AnimatStack = new List<GeAnimatCacheEx>();
    public GeAnimatDescEx sInvalidAnimatDesc = new GeAnimatDescEx(null,null);

    protected uint m_CurAnimHandleCnt = 0;
    protected GeSurfParamDesc m_CurParamDesc;

    protected bool m_IsDirty = false;
#if !LOGIC_SERVER
    private GraphicBackController mGBController = null;
    GraphicBackController GBController
    {
        get
        {
            if (mGBController == null)
            {
                mGBController = AnimatGraphicBack.Acquire();
            }
            return mGBController;
        }
    }
#endif
    public GeAnimatManagerEx()
    {
    }

    public bool Init()
    {
        return true;
    }

    public void Deinit()
    {
        ClearAll();
#if !LOGIC_SERVER
        if (mGBController != null)
        {
            mGBController.Release();
            mGBController = null;
        }
#endif

        m_AnimatDescList.RemoveAll(
            f =>
            {
                if (null != f)
                {
                    if (null != f.animatProxy)
                    {
                        for (int i = 0, icnt = f.animatProxy.Length; i < icnt; ++i)
                        {
                            if (null != f.animatProxy[i])
                                f.animatProxy[i].Recover();
                        }
                    }
                }

                return true;
            });
    }

    public void AppendObject(GameObject[] obj)
    {
		#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAnimMaterial)
            return;
		#endif

        if (null == obj)
            return;

        for(int i =0,icnt = obj.Length;i<icnt;++i)
        {
            if(null == obj[i]) continue;

            m_AnimatDescList.Add(new GeAnimatDescEx(obj[i], obj[i].GetComponentsInChildren<GeMeshDescProxy>()));
        }

        //GeAnimatCache top = _GetTopAnimatCache();
        //if (null != top)
        //    top.animatDesc.animat.Apply(top.animatDesc.animat.GetTimeLen(), top.animatDesc.animat.GetElapsedTime(), top.animatDesc.animat.IsAnimate(), top.animatDesc.animat.IsRecover());
    }

    public void RemoveObject(GameObject[] obj)
    {
		#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAnimMaterial)
            return;
		#endif

        if (null == obj)
            return;

        m_AnimatDescList.RemoveAll(
            f =>
            {
                if (null != f)
                {
                    GeMeshDescProxy[] removed = null;
                    if(null == f.obj)
                    {
                        if (null != f.animatProxy)
                            removed = f.animatProxy;
                    }
                    else
                    {
                        for (int i = 0, icnt = obj.Length; i < icnt; ++i)
                        {
                            GameObject curObj = obj[i];
                            if (null == curObj) continue;

                            if (curObj == f.obj)
                            {
                                removed = f.animatProxy;
                                break;
                            }
                        }
                    }

                    if (null != removed)
                    {
                        for (int i = 0, icnt = removed.Length; i < icnt; ++i)
                        {
                            if (null != removed[i])
                                removed[i].Recover();
                        }

                        return true;
                    }
                    else
                    {
                        if (null == f.obj)
                            return true;
                        else
                            return false;
                    }
                }
                else
                    return true;
            });
    }
    public AnimatHandle PushAnimatInBackMode(string animatName, float time, bool enableAnim, bool needRecover)
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAnimMaterial)
            return uint.MaxValue;
#endif

        if (!string.IsNullOrEmpty(animatName))
        {
            AnimatHandle hNewAnimat = _AllocHandle();
#if !LOGIC_SERVER
            var cmd = PushAnimatGBCommand.Acquire();
            cmd.name = animatName;
            cmd.timeLen = time;
            cmd.enableAnim = enableAnim;
            cmd.needRecover = needRecover;
            cmd._this = this;
            cmd.timeStamp = FrameSync.GetTicksNow();
            cmd.handle = hNewAnimat;
            GBController.RecordCmd((int)AnimatBackCmdType.Push, cmd);
#endif
            return hNewAnimat;
        }
        return uint.MaxValue;
    }
    public void RemoveAnimatInBackMode(AnimatHandle handle)
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAnimMaterial)
            return;
#endif
#if !LOGIC_SERVER
        var cmd = RemoveAnimatGBCommand.Acquire();
        cmd.handle = handle;
        GBController.RecordCmd((int)AnimatBackCmdType.Remove, cmd);
#endif
        return;
    }

    public AnimatHandle PushAnimat(string animatName, float time, bool enableAnim, bool needRecover)
    {
		#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAnimMaterial)
            return uint.MaxValue;
		#endif

        if (!string.IsNullOrEmpty(animatName))
        {
            AnimatHandle hNewAnimat = _AllocHandle();
            m_AnimatStack.Add(new GeAnimatCacheEx(animatName, hNewAnimat, time, enableAnim, needRecover));
            _ApplyAnimat(animatName, time, enableAnim);

            return hNewAnimat;
        }

        return uint.MaxValue;
    }

    public void RemoveAnimat(AnimatHandle handle)
    {
		#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAnimMaterial)
            return;
		#endif

        for (int i = 0; i < m_AnimatStack.Count; ++ i)
        {
            GeAnimatCacheEx curAnimatCache = m_AnimatStack[i];
            if(null == curAnimatCache) continue;

            if (handle == curAnimatCache.handle)
            {
                if (1 == m_AnimatStack.Count)
                {
                    _RecoverAnimat();
                    m_AnimatStack.RemoveAt(i);
                }
                else if(i == m_AnimatStack.Count - 1)
                {
                    GeAnimatCacheEx topCache = curAnimatCache;
                    m_AnimatStack.RemoveAt(m_AnimatStack.Count - 1);

                    GeAnimatCacheEx nextCache = null;
                    while (m_AnimatStack.Count > 0)
                    {
                        nextCache = m_AnimatStack[m_AnimatStack.Count - 1];
                        m_AnimatStack.RemoveAt(m_AnimatStack.Count - 1);

                        if (null == nextCache) continue;
                        if (nextCache.isFinished)
                            continue;

                        _ApplyAnimat(nextCache.name, nextCache.timeLen, nextCache.enableAnim);
                        m_AnimatStack.Add(nextCache);
                        break;
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
    }

    public void Resume()
    {
    }

    public void Update(int deltaTime, GameObject obj)
    {
        int count = m_AnimatStack.Count;
        int lastIdx = count - 1;
        for (int i = 0; i < count; ++ i)
        {
            GeAnimatCacheEx cache = m_AnimatStack[i];
            if (null != cache)
            {
                cache.Update(deltaTime);

                if (cache.isFinished)
                    m_IsDirty = true;
                else
                    _UpdateAnimat(cache.timePos, obj.transform.position);
            }
            else
                m_IsDirty = true;
        }
        
        if (m_IsDirty)
            _RemoveDeadAnimat();

        if (m_AnimatStack.Count !=  count)
        {
            GeAnimatCacheEx top = _GetTopAnimatCache();
            if (null != top)
                _ApplyAnimat(top.name, top.timeLen, top.enableAnim);
            else
                _RecoverAnimat();
        }
    }

    protected void _RemoveDeadAnimat()
    {
        m_AnimatStack.RemoveAll(
            cache =>
            {
                if (null != cache)
                    return cache.isFinished;
                else
                    return true;
            }
            );

        m_IsDirty = false;
    }

    public void ClearAll()
    {
#if !LOGIC_SERVER
        if (FrameSync.instance.IsInChasingMode)
        {
            GBController.RecordCmd((int)AnimatBackCmdType.Clear, null);
        }
#endif
        _RecoverAnimat();
        m_AnimatStack.Clear();
    }
    public void DoBackToFront()
    {
#if !LOGIC_SERVER
        GBController.FlipToFront();
#endif
    }
    protected uint _AllocHandle()
    {
        if (m_CurAnimHandleCnt + 1 == uint.MaxValue)
            m_CurAnimHandleCnt = 0;
        return m_CurAnimHandleCnt++;
    }

    protected void _ApplyAnimat(string name,float timeLen,bool enableAnim)
    {
        for (int j = 0, jcnt = m_AnimatDescList.Count; j < jcnt; ++j)
        {
            GeAnimatDescEx curAnimatDesc = m_AnimatDescList[j];
            if (null == curAnimatDesc) continue;

            for (int k = 0, kcnt = curAnimatDesc.animatProxy.Length; k < kcnt; ++k)
            {
                GeMeshDescProxy cur = curAnimatDesc.animatProxy[k];
                if (null == cur) continue;

                cur.Apply(name, timeLen, enableAnim);
            }
        }
    }

    protected void _RecoverAnimat()
    {
        for (int j = 0, jcnt = m_AnimatDescList.Count; j < jcnt; ++j)
        {
            GeAnimatDescEx curAnimatDesc = m_AnimatDescList[j];
            if (null == curAnimatDesc) continue;

            for (int k = 0, kcnt = curAnimatDesc.animatProxy.Length; k < kcnt; ++k)
            {
                GeMeshDescProxy cur = curAnimatDesc.animatProxy[k];
                if (null == cur) continue;

                cur.Recover();
            }
        }
    }

    protected void _UpdateAnimat(float timePos,Vector3 pos)
    {
        m_CurParamDesc.m_ElapsedTime = timePos;
        m_CurParamDesc.m_WorldPos = pos;

        for (int j = 0, jcnt = m_AnimatDescList.Count; j < jcnt; ++j)
        {
            GeAnimatDescEx curAnimatDesc = m_AnimatDescList[j];
            if (null == curAnimatDesc) continue;

            for (int k = 0, kcnt = curAnimatDesc.animatProxy.Length; k < kcnt; ++k)
            {
                GeMeshDescProxy cur = curAnimatDesc.animatProxy[k];
                if (null == cur) continue;

                cur.DoUpdate(m_CurParamDesc);
            }
        }
    }

    protected GeAnimatCacheEx _GetTopAnimatCache()
    {
        for (int i = m_AnimatStack.Count - 1; i >= 0; --i)
        {
            GeAnimatCacheEx curAnimatCache = m_AnimatStack[i];
            if (null == curAnimatCache) continue;

            return curAnimatCache;
        }

        return null;
    }
}

