using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GeAnimClipPlayMode
{
    AnimPlayOnce = 0,
    AnimPlayLoop = 1,
}

[System.Serializable]
public class GeAnimDesc
{
    public GeAnimDesc(string clipName, int clipCRC32, GeAnimClipPlayMode playMode, float time,AnimationClip animClip,string animClipPath)
    {
        animClipName = clipName;
        aniClipCRC32 = clipCRC32;
        animPlayMode = playMode;
        timeLen = time;
        animClipData = animClip;
        this.animClipPath = animClipPath;
    }

    public string animClipName;
    public int aniClipCRC32;
    public GeAnimClipPlayMode animPlayMode;
    public float timeLen;
    public string animClipPath;

    [System.NonSerialized]
    public AnimationClip animClipData;
}

public class GeAnimationManager
{
    public static GeAnimDesc sInvalidAnimDesc = GeAnimDescProxy.sInvalidAnimDesc;
    protected string m_CurAnimClipName = null;
    protected float m_CurSpeed = 1;
    protected bool m_CurLoop = false;

    protected bool m_IsPaused = false;
    protected float m_PauseTimePos = 0.0f;

    protected GeAnimDescProxy m_AnimDescProxy = null;

    public bool Init(GameObject actor)
    {
        if(null != actor)
        {
            m_AnimDescProxy = actor.GetComponentInChildren<GeAnimDescProxy>();
            if (null != m_AnimDescProxy)
            {
                if (null == m_AnimDescProxy.animInstance)
                    m_AnimDescProxy.animInstance = m_AnimDescProxy.gameObject.GetComponent<Animation>();

                if (null == m_AnimDescProxy.animInstance)
                    m_AnimDescProxy.animInstance = actor.GetComponentInChildren<Animation>();

                return true;
            }
        }

        return false;
    }

    public void Deinit()
    {
        if (null != m_AnimDescProxy && null != m_AnimDescProxy.animInstance)
        {
            m_AnimDescProxy.animInstance.Stop();
            if(null != m_AnimDescProxy.animDescArray)
            {
                for(int i = 0,icnt = m_AnimDescProxy.animDescArray.Length;i<icnt;++i)
                {
                    GeAnimDesc curDesc = m_AnimDescProxy.animDescArray[i];
                    if(null == curDesc || null == curDesc.animClipData) continue;

                    AnimationClip animClip = m_AnimDescProxy.animInstance.GetClip(curDesc.animClipName);
                    if(null != animClip)
                        m_AnimDescProxy.animInstance.RemoveClip(animClip);

                    curDesc.animClipData = null;
                }
            }

            m_AnimDescProxy = null;
        }
    }

    public bool HasAnimClipDesc(string clipName)
    {
        if (null != m_AnimDescProxy && null != m_AnimDescProxy.animDescArray)
        {
            for (int i = 0; i < m_AnimDescProxy.animDescArray.Length; ++i)
            {
                if (m_AnimDescProxy.animDescArray[i].animClipName.Equals(clipName))
                    return true;
            }
        }
        return false;
    }

    public GeAnimDesc GetAnimClipDesc(string clipName)
    {
        if (null != m_AnimDescProxy && null != m_AnimDescProxy.animDescArray)
        {
            for (int i = 0; i < m_AnimDescProxy.animDescArray.Length; ++i)
            {
                if (0 == m_AnimDescProxy.animDescArray[i].animClipName.CompareTo(clipName))
                    return m_AnimDescProxy.animDescArray[i];
            }
        }

        return sInvalidAnimDesc;
    }

	public float GetCurrentAnimationSpeed()
	{
        //float speed = 1.0f;
        //if(null != m_AnimDescProxy && null != m_AnimDescProxy.animInstance)
        //{
        //    AnimationState acActionState = m_AnimDescProxy.animInstance[m_CurAnimClipName];
        //    if (acActionState != null)
        //        return acActionState.speed;
        //}
        //
        //return speed;
        return m_CurSpeed;
	}

    public bool IsCurActionLoop()
    {
        return m_CurLoop;
    }

    public void Replay()
    {
        if (m_CurAnimClipName == null)
            return;

        PlayAction(m_CurAnimClipName, m_CurSpeed, m_CurLoop);
    }

    public bool PlayAction(string acActionName, float fSpeed, bool loop = false,float offset = 0.0f)
    {
        //Logger.Log("play animation name:" + acActionName + " ID:" + m_iResID);
        m_CurAnimClipName = acActionName;
        m_CurSpeed = fSpeed;
        m_CurLoop = loop;
        if(null != m_AnimDescProxy)
        {
            //             GeAnimClipPlayMode playMode = GeAnimClipPlayMode.AnimPlayOnce;
            //             AnimationState acActionState = m_AnimDescProxy.animInstance[acActionName];
            // 
            //             //////////////////////////////////////////////////////////////////////////
            //             // if (null != acActionState && null == acActionState.clip)
            //             //     m_AnimDescProxy.animInstance.RemoveClip(acActionName);
            //             // acActionState = m_AnimDescProxy.animInstance[acActionName];
            //             //////////////////////////////////////////////////////////////////////////
            // 
            //             if (acActionState == null)
            //             {
            //                 GeAnimDesc cur = _FindAnimDescByName(acActionName);
            //                 playMode = cur.animPlayMode;
            // 
            //                 if (null == cur.animClipData && !string.IsNullOrEmpty(cur.animClipPath))
            //                     cur.animClipData = _LoadAnimtionClip(cur.animClipPath, null);
            // 
            //                 if (null != cur.animClipData)
            //                 {
            //                     m_AnimDescProxy.animInstance.AddClip(cur.animClipData, cur.animClipName);
            //                     acActionState = m_AnimDescProxy.animInstance[acActionName];
            //                 }
            // #if UNITY_EDITOR
            //                 else
            //                     Logger.LogWarningFormat("Load animation clip [{0}] has failed!", cur.animClipPath);
            // #endif
            //             }
            // 
            //             if (null != acActionState)
            //             {
            //                 acActionState.wrapMode = loop ? WrapMode.Loop : WrapMode.ClampForever;
            // 
            //                 if (!loop)
            //                     acActionState.wrapMode = playMode == GeAnimClipPlayMode.AnimPlayLoop ? WrapMode.Loop : WrapMode.ClampForever;
            // 
            //                 if (acActionState.speed != fSpeed)
            //                 {
            //                     acActionState.speed = fSpeed;
            //                 }
            // 
            //                 m_AnimDescProxy.animInstance.Stop();
            //                 m_AnimDescProxy.animInstance.Play(acActionName);
            // 
            //                 return true;
            //             }


            return m_AnimDescProxy.ChangeAction(m_CurAnimClipName, m_CurLoop, m_CurSpeed,offset);
        }

        ////////////////////////////////////////////////////////////////////////////
        //if(null != m_AnimDescProxy)
        //{
        //    if (null == m_AnimDescProxy.animInstance)
        //        Debug.LogWarning("Animation instance is null!");
        //}
        //else
        //    Debug.LogWarning("Animation Proxy is null!");
        ////////////////////////////////////////////////////////////////////////////

        return false;
    }

    public void PreloadAction(string[] animList)
    {
        for (int i =0,icnt = animList.Length;i<icnt;++i)
        {
            GeAnimDesc cur = _FindAnimDescByName(animList[i]);
            if (null == cur.animClipData && !string.IsNullOrEmpty(cur.animClipPath))
                cur.animClipData = _LoadAnimtionClip(cur.animClipPath, null);
        }
    }

    public void PreloadAction(string anim)
    {
        GeAnimDesc cur = _FindAnimDescByName(anim);
        if (null == cur.animClipData && !string.IsNullOrEmpty(cur.animClipPath))
            cur.animClipData = _LoadAnimtionClip(cur.animClipPath, null);
    }

    public bool IsCurAnimFinished()
    {
        bool bFinished = true;
        if(null != m_AnimDescProxy && null != m_AnimDescProxy.animInstance)
        {
            AnimationState anis = m_AnimDescProxy.animInstance[m_CurAnimClipName];
            if (null != anis)
                bFinished = anis.normalizedTime >= 1.0f;
        }

        return bFinished;
    }

    public void Stop()
    {
        if (null != m_AnimDescProxy && null != m_AnimDescProxy.animInstance)
        {
            if (m_CurAnimClipName == null || m_CurAnimClipName == "")
                return;
            AnimationState anis = m_AnimDescProxy.animInstance[m_CurAnimClipName];
            if (anis != null)
                m_AnimDescProxy.animInstance.Stop();
        }

        m_CurAnimClipName = "";
    }

    public void Pause()
    {
        if (m_IsPaused)
            return;

        if (null != m_AnimDescProxy && null != m_AnimDescProxy.animInstance)
        {
            if (m_CurAnimClipName == null || m_CurAnimClipName == "")
                return;

            AnimationState anis = m_AnimDescProxy.animInstance[m_CurAnimClipName];
            if (null != anis)
            {
                m_PauseTimePos = anis.time;
                m_AnimDescProxy.animInstance.Stop();
            }
        }

        m_IsPaused = true;
    }

    public void Resume()
    {
        if (!m_IsPaused)
            return;

        if (null != m_AnimDescProxy && null != m_AnimDescProxy.animInstance)
        {
            if (m_CurAnimClipName == null || m_CurAnimClipName == "")
                return;

            AnimationState anis = m_AnimDescProxy.animInstance[m_CurAnimClipName];
            if (null != anis)
            {
                anis.time = m_PauseTimePos;
                m_AnimDescProxy.animInstance.Play(m_CurAnimClipName);
            }
        }

        m_IsPaused = false;
    }

    public string GetCurActionName()
    {
        return m_CurAnimClipName;
    }

    public float GetCurActionOffset()
    {
        if (null != m_AnimDescProxy && null != m_AnimDescProxy.animInstance)
        {
            AnimationState acActionState = m_AnimDescProxy.animInstance[m_CurAnimClipName];
            if (null != acActionState)
                return acActionState.time;
        }
        return 0.0f;

        //if (!string.IsNullOrEmpty(m_CurAnimClipName))
        //    return (System.DateTime.Now.Ticks - m_CurActionTimePosMS) * 0.0001f * 0.001f;
        //else
        //    return 0.0f;
    }

    public AnimationClip _LoadAnimtionClip(string path,string animClip)
    {
		#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAnimation)
            return null;
		#endif

        string subResWithPath = null;
        if (string.IsNullOrEmpty(animClip))
            subResWithPath = path;
        else
            subResWithPath = string.Format("{0}:{1}", path, animClip);

        return AssetLoader.instance.LoadRes(subResWithPath, typeof(AnimationClip)).obj as AnimationClip;
    }

    protected GeAnimDesc _FindAnimDescByName(string name)
    {
        if(null != m_AnimDescProxy && null != m_AnimDescProxy.animDescArray)
        {
            for (int i = 0, icnt = m_AnimDescProxy.animDescArray.Length; i < icnt; ++i)
            {
                if (m_AnimDescProxy.animDescArray[i].animClipName == name)
                    return m_AnimDescProxy.animDescArray[i];
            }
        }

        return sInvalidAnimDesc;
    }

    protected float _CalculateAnimTimeLength(string name)
    {
        float timeLen = 0.0f;
        GeAnimDesc curDesc = _FindAnimDescByName(name);
        if (!string.IsNullOrEmpty(curDesc.animClipPath))
        {
            if (null == curDesc.animClipData)
                curDesc.animClipData = _LoadAnimtionClip(curDesc.animClipPath, null);

            if (null != curDesc.animClipData)
                return curDesc.animClipData.length;
        }

        return timeLen;
    }
}
