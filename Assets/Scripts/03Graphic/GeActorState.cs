using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeActorState
{
    protected class GeFrameAudio
    {
        public string m_AudioClipRes;
    }
    protected class GeFrameEffect
    {
        public string m_EffectResPath;

        public Vector3 m_LocalPosition = Vector3.zero;
        public Quaternion m_LocalRotation = Quaternion.identity;
        public Vector3 m_LocalScale = Vector3.one;
        public int m_Effecttype;
        public string m_AttachNode;
        public float m_EffectTimeLen = 0.0f;
    }

    protected class GeFrameObjAnimDesc
    {
        public int m_StartFrame = 0;
        public string m_AnimName = "";
        public float  m_PlaySpeed = 0;
    }

    protected class GeFrameAttachment
    {
        public string m_AttachResPath;
        public string m_AttachNode;
        public string m_AttachName;
        public int m_StartFrame;
        public int m_EndFrame;

        public Vector3 m_LocalPosition = Vector3.zero;
        public Quaternion m_LocalRotation = Quaternion.identity;
        public Vector3 m_LocalScale = Vector3.one;

        public List<GeFrameObjAnimDesc> m_AnimList = new List<GeFrameObjAnimDesc>();
    }

    protected class GeStateFrame
    {
        public List<GeFrameAttachment> m_AttachFrames = new List<GeFrameAttachment>();
        public List<GeFrameEffect> m_EffectFrames = new List<GeFrameEffect>();
        public List<GeFrameAudio> m_AudioFrames = new List<GeFrameAudio>();

        public List<FrameEvnetCall> m_Events = new List<FrameEvnetCall>();
    }

    public delegate void FrameEvnetCall();

    protected class GeStateDesc
    {
        public string m_ActionName = null;
        public int m_TotalFrameNum = 0;
        public float m_FrameTime = 0.0f;
        public float m_StateTimeLen = 0.0f;
        public float m_PlaybackSpeed = 0.0f;

        public GeStateFrame[] m_StateFrames = null;
    }

    protected Dictionary<string, GeStateDesc> m_StateMap = new Dictionary<string, GeStateDesc>();
    protected uint m_CurStateFrameCnt = 0;
    protected GeStateDesc m_CurState = null;
    protected float m_CurFrameTime = 0;

    protected IGeAvatarActor m_AvatarActor = null;

    public void Init(IGeAvatarActor avatar)
    {
        if (null != avatar)
            m_AvatarActor = avatar;
    }

    public void LoadState(string state)
    {
        UnityEngine.Object skillRes = AssetLoader.instance.LoadRes(state,typeof(DSkillData)).obj;
        DSkillData data = skillRes as DSkillData;

        if (null == data)
            return;

        GeStateDesc newState = _CreateStateByData(data);

        if(null == m_StateMap)
            m_StateMap = new Dictionary<string, GeStateDesc>();

        m_StateMap.Add(data.moveName, newState);
    }

    public void Deinit()
    {

    }

    public void ChangeState(string res)
    {
        GeStateDesc dstState = null;
        if (m_StateMap.TryGetValue(res, out dstState))
        {
            m_CurState = dstState;
            m_CurStateFrameCnt = 0;
            m_CurFrameTime = 0;

            if(null != m_AvatarActor)
                m_AvatarActor.ChangeAction(m_CurState.m_ActionName);
        }
    }

    public void Update(float fDelta)
    {
        if (fDelta > 1.0f)
            fDelta = 1 / 30.0f;

        if (null != m_CurState)
        {
            m_CurFrameTime += fDelta;

            while(m_CurFrameTime > m_CurState.m_FrameTime)
            {
                _UpdateFrame(m_CurStateFrameCnt);
                ++m_CurStateFrameCnt;
                m_CurFrameTime -= m_CurState.m_FrameTime;
            }
        }
    }

    public void _UpdateFrame(uint frameIdx)
    {
        if (null != m_CurState && null != m_AvatarActor)
        {
            if(frameIdx < m_CurState.m_TotalFrameNum)
            {
                GeStateFrame curFrame = m_CurState.m_StateFrames[frameIdx];
                ///if(curFrame.m_AttachFrames.Count > 0)
                ///{
                ///    for(int i = 0, icnt = curFrame.m_AttachFrames.Count; i < icnt; ++ i)
                ///    {
                ///        GeFrameAttachment curAttachFrame = curFrame.m_AttachFrames[i];
                ///        GeAttach attach = m_AvatarActor.AttachAvatar(curAttachFrame.m_AttachName, curAttachFrame.m_AttachResPath, curAttachFrame.m_AttachNode);
                ///
                ///        for (int j = 0, jcnt = curAttachFrame.m_AnimList.Count; j < jcnt; ++j)
                ///        {
                ///            GeFrameObjAnimDesc curAnimDesc = curAttachFrame.m_AnimList[j];
                ///            if (frameIdx >= curAnimDesc.m_StartFrame)
                ///                attach
                ///        }
                ///
                ///        int endFrame = curAttachFrame.m_EndFrame;
                ///        if (endFrame >= m_CurState.m_TotalFrameNum)
                ///            endFrame = m_CurState.m_TotalFrameNum - 1;
                ///
                ///        m_CurState.m_StateFrames[endFrame].m_Events.Add(() => { m_AvatarActor.RemoveAttach(attach); });
                ///    }
                ///}

                if (curFrame.m_EffectFrames.Count > 0)
                {
                    for (int i = 0, icnt = curFrame.m_EffectFrames.Count; i < icnt; ++i)
                    {
                        GeFrameEffect curEffectFrame = curFrame.m_EffectFrames[i];
                        m_AvatarActor.CreateEffect(curEffectFrame.m_EffectResPath, curEffectFrame.m_AttachNode, curEffectFrame.m_EffectTimeLen, (EffectTimeType)curEffectFrame.m_Effecttype, curEffectFrame.m_LocalPosition, curEffectFrame.m_LocalRotation, curEffectFrame.m_LocalScale.x);
                    }
                }

                if (curFrame.m_AudioFrames.Count > 0)
                {
                    for (int i = 0, icnt = curFrame.m_AudioFrames.Count; i < icnt; ++i)
                    {
                        GeFrameAudio  curAudioFrame = curFrame.m_AudioFrames[i];
                        AudioManager.instance.PlaySound(curAudioFrame.m_AudioClipRes,AudioType.AudioEffect);
                    }
                }

                if (curFrame.m_Events.Count > 0)
                {
                    for (int i = 0, icnt = curFrame.m_Events.Count; i < icnt; ++i)
                        curFrame.m_Events[i].Invoke();

                    curFrame.m_Events.Clear();
                }
            }
        }
    }

    protected GeStateDesc _CreateStateByData(DSkillData data)
    {
        GeStateDesc newState = new GeStateDesc();

        newState.m_ActionName = data.animationName;
        newState.m_TotalFrameNum = data.totalFrames;
        newState.m_FrameTime = 1.0f / data.fps;
        
        newState.m_StateTimeLen = (newState.m_TotalFrameNum - 1) * newState.m_FrameTime;
        newState.m_PlaybackSpeed = 1.0f;

        newState.m_StateFrames = new GeStateFrame[newState.m_TotalFrameNum];
        for(int i = 0,icnt = newState.m_StateFrames.Length; i < icnt;++i)
            newState.m_StateFrames[i] = new GeStateFrame();

        /// 特效帧
        if (data.effectFrames != null && data.effectFrames.Length > 0)
        {
            for (int i = 0; i < data.effectFrames.Length; ++i)
            {
                EffectsFrames effectData = data.effectFrames[i];
                int frame = effectData.startFrames;
        
                if (frame >= 0 && frame < newState.m_StateFrames.Length)
                {
                    GeStateFrame stateFrame = newState.m_StateFrames[frame];
                    if (stateFrame != null)
                    {
                        GeFrameEffect newFrameEffect = new GeFrameEffect();

                        newFrameEffect.m_EffectResPath = effectData.effectAsset.m_AssetPath;
                        newFrameEffect.m_AttachNode = effectData.attachname;
                        newFrameEffect.m_EffectTimeLen = effectData.time;
                        newFrameEffect.m_Effecttype = effectData.effecttype;
                        newFrameEffect.m_LocalPosition = effectData.localPosition;
                        newFrameEffect.m_LocalRotation = effectData.localRotation;
                        newFrameEffect.m_LocalScale = effectData.localScale;

                        stateFrame.m_EffectFrames.Add(newFrameEffect);
                    }
                }
            }
        }
        
        //sfx播放
        if (data.sfx != null && data.sfx.Length > 0)
        {
            //foreach (var frameEvent in data.sfx)
			for(int i=0; i<data.sfx.Length; ++i)
            {
				var frameEvent = data.sfx[i];
                int frame = frameEvent.startframe;
                if (frame < newState.m_StateFrames.Length)
                {
                    var stateFrame= newState.m_StateFrames[frame];

                    GeFrameAudio newFrameAudio = new GeFrameAudio();
                    newFrameAudio.m_AudioClipRes = frameEvent.soundClipAsset.m_AssetPath;
                    stateFrame.m_AudioFrames.Add(newFrameAudio);
                }
            }
        }
        
        //挂件
        if (data.attachFrames != null)
        {
            for (int i = 0; i < data.attachFrames.Length; ++i)
            {
                EntityAttachFrames attf = data.attachFrames[i];
        
                if (attf != null)
                {
                    int startFrame = (int)(attf.start / newState.m_FrameTime);
                    if (startFrame >= newState.m_TotalFrameNum)
                        startFrame = newState.m_TotalFrameNum - 1;
                    GeStateFrame stateFrame = newState.m_StateFrames[startFrame];
                    if (stateFrame != null)
                    {

                        GeFrameAttachment newFrameAttach = new GeFrameAttachment();
                        newFrameAttach.m_AttachResPath = attf.entityAsset.m_AssetPath;
                        newFrameAttach.m_AttachNode = attf.attachName;
                        newFrameAttach.m_AttachName = attf.name;
                        newFrameAttach.m_StartFrame = startFrame;
                        newFrameAttach.m_EndFrame = (int)(attf.end / newState.m_FrameTime);
                        if (null != attf.trans)
                        {
                            newFrameAttach.m_LocalPosition = attf.trans.localPosition;
                            newFrameAttach.m_LocalRotation = attf.trans.localRotation;
                            newFrameAttach.m_LocalScale = attf.trans.localScale;
                        }
                        else
                        {
                            newFrameAttach.m_LocalPosition = Vector3.zero;
                            newFrameAttach.m_LocalRotation = Quaternion.identity;
                            newFrameAttach.m_LocalScale = Vector3.one;
                        }

                        for (int j = 0; j < attf.animations.Length; ++j)
                        {
                            GeFrameObjAnimDesc temp = new GeFrameObjAnimDesc();
                            temp.m_AnimName = attf.animations[j].anim;
                            temp.m_StartFrame = (int)(attf.animations[j].start / newState.m_FrameTime);
                            temp.m_PlaySpeed = attf.animations[j].speed;
                            newFrameAttach.m_AnimList.Add(temp);
                        }

                        stateFrame.m_AttachFrames.Add(newFrameAttach);
                    }
                }
            }
        }

        return newState;
    }
}
