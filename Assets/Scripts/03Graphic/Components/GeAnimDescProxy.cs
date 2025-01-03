using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GeAnimDescProxy : MonoBehaviour
{
    [System.NonSerialized]
    public static GeAnimDesc sInvalidAnimDesc = new GeAnimDesc("", ~0, GeAnimClipPlayMode.AnimPlayOnce, 0, null, null);

    // 外部配置好的动画数据，多个武器可以共用一个动画数据，修改动画时不需要修改多个武器
    [SerializeField]
    GeAnimDescData m_ExternAnimDescData;

    [SerializeField]
    GeAnimDesc[] m_AnimDescArray = new GeAnimDesc[0];

    [SerializeField]
    Animation m_Animaion = null;

    [SerializeField]
    string[] m_AnimDataResFile = new string[0];

    public GeAnimDesc[] animDescArray
    {
        set
        {
            if(null != value)
                m_AnimDescArray = value;
        }

        get
        {
            if (m_ExternAnimDescData != null)
                return m_ExternAnimDescData.animDescArray;

            return m_AnimDescArray; 
        }
    }

    public string[] animDataResFile
    {
        set
        {
            if (null != value)
                m_AnimDataResFile = value;
        }

        get { return m_AnimDataResFile; }
    }

    public Animation animInstance
    {
        set
        {
            if (null != value)
                m_Animaion = value;
        }

        get { return m_Animaion; }
    }
    public void GenAnimDesc()
    {
        List<GeAnimDesc> animClipDescList = new List<GeAnimDesc>();
        _ReinitAnimationDesc(ref animClipDescList);
        animDescArray = animClipDescList.ToArray();

        m_Animaion = gameObject.GetComponent<Animation>();
        if (null == m_Animaion)
            m_Animaion = gameObject.AddComponent<Animation>();

        if (null != m_Animaion)
            m_Animaion.cullingType = AnimationCullingType.BasedOnRenderers;
    }

    public DynamicShadow m_DynamicShadow;
    public bool ChangeAction(string name,bool loop,float speed,float offset = 0.0f)
    {
        if (m_DynamicShadow != null)
        {
            m_DynamicShadow.changeAnimation(name);
        }
        if (EngineConfig.useAsyncLoadAnim)
        {
            if (null != m_Animaion)
            {
                GeAnimClipPlayMode playMode = GeAnimClipPlayMode.AnimPlayOnce;
                AnimationState acActionState = m_Animaion[name];
                if (acActionState == null)
                {
                    GeAnimDesc cur = FindAnimDescByName(name);
                    playMode = cur.animPlayMode;

                    if (null == cur.animClipData && !string.IsNullOrEmpty(cur.animClipPath))
                    {
                        AsyncLoadContext context = new AsyncLoadContext();
                        context.m_AnimDesc = cur;
                        context.m_This = this;

                        m_ChangeActionContext.m_AnimName = name;
                        m_ChangeActionContext.m_IsLoop = loop;
                        m_ChangeActionContext.m_Speed = speed;
                        m_ChangeActionContext.m_Offset = offset;
                        m_ChangeActionContext.m_TimeStamp = System.DateTime.Now.Ticks;
                        m_ChangeActionContext.m_IsValid = true;

                        AssetLoader.LoadResAsync(cur.animClipPath, typeof(AnimationClip), m_AssetLoadCallbacks, context,1);
                        return true;
                    }

                    if (null != cur.animClipData && cur.animClipData.legacy == true)
                    {
                        animInstance.AddClip(cur.animClipData, cur.animClipName);
                        acActionState = animInstance[name];
                    }
                }

                if (null != acActionState)
                {
                    m_ChangeActionContext.m_IsValid = false;
                    acActionState.wrapMode = loop ? WrapMode.Loop : WrapMode.ClampForever;

                    if (!loop)
                        acActionState.wrapMode = playMode == GeAnimClipPlayMode.AnimPlayLoop ? WrapMode.Loop : WrapMode.ClampForever;

                    if (acActionState.speed != speed)
                        acActionState.speed = speed;

                    animInstance.Stop();
                    acActionState.time = offset;
                    animInstance.Play(name);

                    return true;
                }
            }

            return false;
        }
        else
        {
            if(null != m_Animaion)
            {
                GeAnimDesc cur = FindAnimDescByName(name);
                GeAnimClipPlayMode playMode = cur.animPlayMode;
 
                AnimationState acActionState = m_Animaion[name];
                if (acActionState == null)
                {
                    if (null == cur.animClipData && !string.IsNullOrEmpty(cur.animClipPath))
                    {
                        cur.animClipData = _LoadAnimtionClip(cur.animClipPath, null);
                    }

                    if (null != cur.animClipData)
                    {
                        if (cur.animClipData.legacy)
                        {
                            animInstance.AddClip(cur.animClipData, cur.animClipName);
                            acActionState = animInstance[name];
                        }
                        else
                        {
                            Logger.LogErrorFormat("加载 Animation Clip [{0}] legacy 选项未勾选!!!!!", cur.animClipPath);
                        }
                    }
#if UNITY_EDITOR
                    else
                    {
                         Logger.LogWarningFormat("Load animation clip [{0}] has failed!", cur.animClipPath);
                    }
 #endif
                }
 
                if (null != acActionState)
                {
                    acActionState.wrapMode = loop ? WrapMode.Loop : WrapMode.ClampForever;
 
                    if (!loop)
                        acActionState.wrapMode = playMode == GeAnimClipPlayMode.AnimPlayLoop ? WrapMode.Loop : WrapMode.ClampForever;
 
                    if (acActionState.speed != speed)
                        acActionState.speed = speed;
 
                    animInstance.Stop();
                    acActionState.time = offset;
                    animInstance.Play(name);
 
                    return true;
                }
            }
 
            return false;
        }
    }


    private bool _ChangeActionImmediate(string name, bool loop, float speed, float offset)
    {
        if (m_DynamicShadow != null)
        {
            m_DynamicShadow.changeAnimation(name);
        }
        if (null != m_Animaion)
        {
            GeAnimClipPlayMode playMode = GeAnimClipPlayMode.AnimPlayOnce;
            AnimationState acActionState = m_Animaion[name];
            if (acActionState == null)
            {
                GeAnimDesc cur = FindAnimDescByName(name);
                playMode = cur.animPlayMode;

                if (null != cur.animClipData && cur.animClipData.legacy == true)
                {
                    animInstance.AddClip(cur.animClipData, cur.animClipName);
                    acActionState = animInstance[name];
                }
#if UNITY_EDITOR
                else
                    Logger.LogWarningFormat("Play animation clip [{0}] has failed!", cur.animClipName);
#endif
            }

            if (null != acActionState)
            {
                acActionState.wrapMode = loop ? WrapMode.Loop : WrapMode.ClampForever;

                if (!loop)
                    acActionState.wrapMode = playMode == GeAnimClipPlayMode.AnimPlayLoop ? WrapMode.Loop : WrapMode.ClampForever;

                if (acActionState.speed != speed)
                    acActionState.speed = speed;

                animInstance.Stop();
                acActionState.time = offset;
                animInstance.Play(name);

                return true;
            }
        }

        return false;
    }

    public void PreloadAction(string[] animList)
    {
        for (int i = 0, icnt = animList.Length; i < icnt; ++i)
            PreloadAction(animList[i]);
    }

    public void PreloadAction(string anim)
    {
        for (int i = 0, icnt = animDescArray.Length; i < icnt; ++i)
        {
			if (animDescArray[i].animClipName.Equals(anim))
                animDescArray[i].animClipData = _LoadAnimtionClip(animDescArray[i].animClipPath, null);
        }
    }

    public AnimationClip _LoadAnimtionClip(string path, string animClip)
    {
        string subResWithPath = null;
        if (string.IsNullOrEmpty(animClip))
            subResWithPath = path;
        else
            subResWithPath = string.Format("{0}:{1}", path, animClip);

        return AssetLoader.instance.LoadRes(subResWithPath, typeof(AnimationClip)).obj as AnimationClip;
    }

    [System.NonSerialized]
    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

    class ChangeActionContext
    {
        public long m_TimeStamp;
        public string m_AnimName;
        public bool m_IsLoop;
        public float m_Speed;
        public float m_Offset;

        public bool m_IsValid;
    }

    [System.NonSerialized]
    ChangeActionContext m_ChangeActionContext = new ChangeActionContext();

    class AsyncLoadContext
    {
        public GeAnimDesc m_AnimDesc;
        public GeAnimDescProxy m_This;
    }

    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
    {
        if (null == userData)
        {
            Tenmove.Runtime.Debugger.LogError("User data can not be null!");
            return;
        }

        AsyncLoadContext asyncLoadContext = userData as AsyncLoadContext;
        if (null == asyncLoadContext)
        {
            Tenmove.Runtime.Debugger.LogError("User data type '{0}' is NOT AsyncLoadContext!");
            return;
        }

        if (null == asset)
        {
            Tenmove.Runtime.Debugger.LogError("Asset '{0}' load error!", assetPath);
            return;
        }

        AnimationClip animClip = asset as AnimationClip;
        if (null == animClip)
        {
            Tenmove.Runtime.Debugger.LogError("Asset '{0}' is nil or type '{1}' error!", assetPath, asset.GetType());
            return;
        }

        asyncLoadContext.m_AnimDesc.animClipData = animClip;
        if (asyncLoadContext.m_This.m_ChangeActionContext.m_IsValid && asyncLoadContext.m_AnimDesc.animClipName == asyncLoadContext.m_This.m_ChangeActionContext.m_AnimName)
        {
            float offset = Tenmove.Runtime.Utility.Time.TicksToSeconds(System.DateTime.Now.Ticks - asyncLoadContext.m_This.m_ChangeActionContext.m_TimeStamp);
            asyncLoadContext.m_This._ChangeActionImmediate(asyncLoadContext.m_This.m_ChangeActionContext.m_AnimName,
                asyncLoadContext.m_This.m_ChangeActionContext.m_IsLoop,
                asyncLoadContext.m_This.m_ChangeActionContext.m_Speed,
                asyncLoadContext.m_This.m_ChangeActionContext.m_Offset + offset);

            asyncLoadContext.m_This.m_ChangeActionContext.m_IsValid = false;
        }
    }

    static void _OnLoadAssetFailure(string assetPath,int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
        //Logger.LogErrorFormat("[GeAvatar]Load game object '{0}' has failed![Error:{1}]", assetPath, errorMessage);
    }

    private void _LoadAnimationClipAsync(GeAnimDesc animDesc)
    {

    }

    protected class AnimClipDesc
    {
        public AnimationClip m_AnimClip = null;
        public string m_AnimClipFile = null;
    }

    protected class AnimFBXDesc
    {
        public Animation m_Anim = null;
        public string m_AnimFile = null;
    }

    protected void _ReinitAnimationDesc(ref List<GeAnimDesc> animClipDescList)
    {
        List<AnimFBXDesc> animList = new List<AnimFBXDesc>();
        List<AnimClipDesc> animClipList = new List<AnimClipDesc>();

        for (int i = 0, icnt = m_AnimDataResFile.Length; i < icnt; ++i)
        {
            string ext = Path.GetExtension(m_AnimDataResFile[i]);
            if (ext.Contains("fbx") || ext.Contains("FBX"))
            {
                GameObject animGO = AssetLoader.instance.LoadResAsGameObject(m_AnimDataResFile[i]);
                if (null == animGO) continue;

                AnimFBXDesc curAnimFBX = new AnimFBXDesc();
                curAnimFBX.m_Anim = animGO.GetComponent<Animation>();
                curAnimFBX.m_AnimFile = m_AnimDataResFile[i];
                animList.Add(curAnimFBX);
            }
            else if(ext.Contains("anim") || ext.Contains("ANIM"))
            {
                AnimationClip animClip = AssetLoader.instance.LoadRes(m_AnimDataResFile[i],typeof(AnimationClip)).obj  as AnimationClip;
                if (null == animClip) continue;

                AnimClipDesc curAnimClipDesc = new AnimClipDesc();
                curAnimClipDesc.m_AnimClip = animClip;
                curAnimClipDesc.m_AnimClipFile = m_AnimDataResFile[i];
                animClipList.Add(curAnimClipDesc);
            }
        }

        for (int i = 0,icnt = animClipList.Count; i < icnt; ++i)
        {
            AnimClipDesc curAnimClipDesc = animClipList[i];

            if (!_HasAnimClipDesc(curAnimClipDesc.m_AnimClip.name, animClipDescList))
                animClipDescList.Add(new GeAnimDesc(curAnimClipDesc.m_AnimClip.name, curAnimClipDesc.m_AnimClip.name.GetHashCode(), curAnimClipDesc.m_AnimClip.wrapMode == WrapMode.Loop ? GeAnimClipPlayMode.AnimPlayLoop : GeAnimClipPlayMode.AnimPlayOnce, curAnimClipDesc.m_AnimClip.length,curAnimClipDesc.m_AnimClip, curAnimClipDesc.m_AnimClipFile));
        }


        Animation[] animInstList = new Animation[animList.Count];
        for(int i = 0,icnt = animInstList.Length;i<icnt;++i)
            animInstList[i] = animList[i].m_Anim;

        if (animList.Count > 0)
        {
            for (int i = 0; i < animList.Count; ++i)
            {
                foreach (AnimationState state in animList[i].m_Anim)
                {
                    if (!_HasAnimClipDesc(state.name, animClipDescList))
                        animClipDescList.Add(new GeAnimDesc(state.name, state.name.GetHashCode(), state.wrapMode == WrapMode.Loop ? GeAnimClipPlayMode.AnimPlayLoop : GeAnimClipPlayMode.AnimPlayOnce, _CalculateAnimTimeLength(animInstList, state.name),state.clip, animList[i].m_AnimFile));
                }
            }
        }
    }

    protected float _CalculateAnimTimeLength(Animation[] anis, string name)
    {
        float timeLen = 0.0f;
        for (int i = 0; i < anis.Length; ++i)
        {
            if (null == anis[i])
                continue;

            AnimationClip curClip = anis[i].GetClip(name);
            if (null != curClip)
                timeLen = Mathf.Max(curClip.length, timeLen);
        }

        return timeLen;
    }

    protected bool _HasAnimClipDesc(string clipName, List<GeAnimDesc> animClipDescLis)
    {
        for (int i = 0; i < animClipDescLis.Count; ++i)
        {
            if (animClipDescLis[i].animClipName.Equals(clipName))
                return true;
        }
        return false;
    }

    public GeAnimDesc FindAnimDescByName(string name)
    {
        if (null != animDescArray)
        {
            for (int i = 0, icnt = animDescArray.Length; i < icnt; ++i)
            {
                if (animDescArray[i].animClipName == name)
                    return animDescArray[i];
            }
        }

        return sInvalidAnimDesc;
    }
}
