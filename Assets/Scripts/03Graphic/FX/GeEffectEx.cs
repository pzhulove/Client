using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HGBase;
using Spine.Unity;
public partial class GeEffectEx
{
    bool scaled = false;
    float scaledValue = 0f;
	bool touchGround = false;
	float defaultTimeLen = 0f;
	public bool useCube = false;

	public static Dictionary<string, float> defaultTimeMap = new Dictionary<string, float>();
    bool createdInBackMode = false;

#if !LOGIC_SERVER
    private GraphicBackController mGBController = null;
    public bool IsCreatedInBackMode { get { return createdInBackMode; } }
    GraphicBackController GBController
    {
        get
        {
            if (mGBController == null)
            {
                mGBController = EffectGraphicBack.Acquire();
            }
            return mGBController;
        }
    }
    public float GetDefaultTimeLen()
	{
        /*if (!defaultTimeMap.ContainsKey(m_Name))
		{
			float time = _getDefaultTimeLen();
			defaultTimeMap[m_Name] = time;
		}

		return defaultTimeMap[m_Name];*/

        if (defaultTimeLen > 0)
            return defaultTimeLen;
        else
            return _getDefaultTimeLen();
	}

	public static void ClearDefaultTimeMap()
	{
		defaultTimeMap.Clear();
	}

    public GeEffectEx()
    {

    }

	public void Reset()
	{
		m_PlaySpeed = 1.0f;
		m_ElapsedTimeMS = 0;
		m_TimeLenMS = 0;
		m_TimeType = EffectTimeType.SYNC_ANIMATION;

		if (m_EffectSpace.m_RootNode != null)
		{
			m_EffectSpace.m_RootNode.transform.SetParent(null, false);
			m_EffectSpace.m_RootNode.transform.position = Vector3.zero;
			m_EffectSpace.m_RootNode.transform.rotation = Quaternion.identity;
			m_EffectSpace.m_RootNode.transform.localScale = new Vector3(1f,1f,1f);
		}

		if (m_EffectSpace.m_EffectNode != null)
		{
			m_EffectSpace.m_EffectNode.transform.SetParent(null, false);
			m_EffectSpace.m_EffectNode.transform.position = Vector3.zero;
			m_EffectSpace.m_EffectNode.transform.rotation = Quaternion.identity;
			m_EffectSpace.m_EffectNode.transform.localScale = new Vector3(1f,1f,1f);
		}

		if (m_EffectSpace.m_GameObject != null)
		{
			var script = m_EffectSpace.m_GameObject.GetComponent<CPooledGameObjectScript>();
			if (script != null)
			{
				//script.gameObject.transform.SetParent(null, true);
				script.gameObject.transform.position = Vector3.zero;
				script.gameObject.transform.rotation = Quaternion.identity;
				script.gameObject.transform.localScale = script.m_defaultScale;
			}
		}

        if(m_SpineAnimations != null)
        {
            for(int i = 0; i < m_SpineAnimations.Count; ++i)
            {
                m_SpineAnimations[i].ReWind();
            }
        }
	}
    //追帧模式下需要构建一些初始数据，用于保证逻辑能够正常运行
    public bool InitInBackMode(string strResName, EffectsFrames info, float time, Vector3 initPos, bool bFaceLeft, GeEntity owner, string attachNode, bool useCube = false)
    {
        this.useCube = useCube;
        m_Name = Utility.FormatString(strResName);
        m_EffectFrame = info;
        if (m_EffectFrame != null)
            m_TimeType = m_EffectFrame.timetype;
        if (useCube)
        {
            strResName = "Effects/DummyEffect/DummyEffect";
        }
        if (!AssetLoader.IsResExist(strResName, typeof(GameObject), true))
        {
            Logger.LogErrorFormat("InitInBackMode GeEffect resource {0} is not exist!", strResName);
            return false;
        }
        string fileName = System.IO.Path.GetFileNameWithoutExtension(strResName);
        m_EffectSpace.m_RootNode = new GameObject(fileName + "_EffRoot");
        m_EffectSpace.m_EffectNode = new GameObject(fileName + "_TMNode");
        if (time > 0)
            m_TimeLenMS = (uint)(time * 1000);
        else
            m_TimeLenMS = (uint)(defaultTimeLen * 1000);

        if (m_TimeLenMS <= 0 && m_EffectFrame != null && m_EffectFrame.time > 0)
        {
            m_TimeLenMS = (uint)(m_EffectFrame.time * 1000);
        }
        uint curFrame = 0;
        if (FrameSync.instance != null)
        {
            curFrame = FrameSync.instance.curFrame;
        }
        var cmd = CreateEffectGBCommand.Acquire();
        cmd.strResName = strResName;
        cmd.info = info;
        cmd.time = time;
        cmd.initPos = initPos;
        cmd.bFaceLeft = bFaceLeft;
        cmd.useCube = useCube;
        cmd.owner = owner;
        cmd.attachNode = attachNode;
        cmd._this = this;
        cmd.frame = curFrame;
        cmd.timeStamp = FrameSync.GetTicksNow();
        createdInBackMode = true;
        GBController.RecordCmd((int)GeEffectBackCmdType.Create, cmd);
        return true;
    }
    public bool Init(string strResName, EffectsFrames info, float time,Vector3 initPos,bool bFaceLeft,GameObject parentObj = null, bool useCube=false)
    {
		this.useCube = useCube;
        m_Name = Utility.FormatString(strResName);
        m_EffectFrame = info;
		if (m_EffectFrame != null)
			m_TimeType = m_EffectFrame.timetype;
        createdInBackMode = false;
        if (FrameSync.instance.IsInChasingMode)
        {
            if (useCube)
            {
                strResName = "Effects/DummyEffect/DummyEffect";
            }
            if (!AssetLoader.IsResExist(strResName, typeof(GameObject), true))
            {
                Logger.LogErrorFormat("Init GeEffect backMode resource {0} is not exist!", strResName);
                return false;
            }
            string fileName = System.IO.Path.GetFileNameWithoutExtension(strResName);
            m_EffectSpace.m_RootNode = new GameObject(fileName + "_EffRoot");
            m_EffectSpace.m_EffectNode = new GameObject(fileName + "_TMNode");
            if (time > 0)
                m_TimeLenMS = (uint)(time * 1000);
            else
                m_TimeLenMS = 99999999u;

            if (m_TimeLenMS <= 0 && m_EffectFrame != null && m_EffectFrame.time > 0)
            {
                m_TimeLenMS = (uint)(m_EffectFrame.time * 1000);
            }
            uint curFrame = 0;
            if (FrameSync.instance != null)
            {
                curFrame = FrameSync.instance.curFrame;
            }
            CreateEffectGBCommand cmd = CreateEffectGBCommand.Acquire();
            cmd.strResName = strResName;
            cmd.info = info;
            cmd.time = time;
            cmd.initPos = initPos;
            cmd.bFaceLeft = bFaceLeft;
            cmd.useCube = useCube;
            cmd.parentObj = parentObj;
            cmd._this = this;
            cmd.frame = curFrame;
            cmd.timeStamp = FrameSync.GetTicksNow();
            createdInBackMode = true;
            GBController.RecordCmd((int)GeEffectBackCmdType.Create, cmd);
            return true;
        }
        this.m_EffectSpace.m_ParentNode = parentObj;

        if(string.IsNullOrEmpty(strResName) || "-" == strResName)
        {
            m_EffectSpace.m_GameObject = new GameObject("DummyEffect");
            return true;
        }

		if (
			#if DEBUG_REPORT_ROOT
			DebugSettings.instance.DummyEffect || 
			#endif
			useCube)
            strResName = "Effects/DummyEffect/DummyEffect";

        m_EffectSpace.m_GameObject = CGameObjectPool.instance.GetGameObject(strResName, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
        if(null != m_EffectSpace.m_GameObject)
        {
            m_EffectSpace.m_GameObject.tag = "EffectModel";

			var proxy = m_EffectSpace.m_GameObject.GetComponent<GeEffectProxy>();
			if (proxy == null)
			{
			#if UNITY_EDITOR				
				Logger.LogErrorFormat("{0} 没有刷GeEffectProxy!!!!!!", m_EffectSpace.m_GameObject.name);
			#endif
				proxy = m_EffectSpace.m_GameObject.AddComponent<GeEffectProxy>();
				proxy.DoCookData(true);
			}

            if((proxy.m_SpineAnimations != null && proxy.m_SpineAnimations.Length > 0) || 
                (proxy.m_SpineAnimationsUI != null && proxy.m_SpineAnimationsUI.Length > 0))
            {
                m_SpineAnimations = new List<ISkeletonAnimation>();
                if(proxy.m_SpineAnimations != null)
                {
                    for(int i = 0; i < proxy.m_SpineAnimations.Length; ++i)
                    {
                        m_SpineAnimations.Add(proxy.m_SpineAnimations[i]);
                    }
                }

                if(proxy.m_SpineAnimationsUI != null)
                {
                    for(int i = 0; i < proxy.m_SpineAnimationsUI.Length; ++i)
                    {
                        m_SpineAnimations.Add(proxy.m_SpineAnimationsUI[i]);
                    }
                }
            }
			m_AudioProxy = proxy.m_AudioProxy;
			m_Animations = proxy.m_Animations;
			m_ParticleSys = proxy.m_ParticleSys;
			m_Trails = proxy.m_Trails;
			m_SeqFrames = proxy.m_SeqFrames;
			// m_ParticleEmitter = proxy.m_ParticleEmitter;
			// m_ParticleAnimator = proxy.m_ParticleAnimator;
			m_Animators = proxy.m_Animators;
			defaultTimeLen = proxy.defaultTimeLen;
            m_LockLoop = proxy.m_LockLoop;
/*
            if ( Battle.GeUtility.CheckArrayItems(m_Animations) == false)
            {
                Logger.LogErrorFormat("Effect {0} m_Animations Contain empty item,Pls Check!",strResName);
            }

            if( Battle.GeUtility.CheckArrayItems(m_ParticleSys) == false)
            {
                Logger.LogErrorFormat("Effect {0} m_ParticleSys Contain empty item,Pls Check!",strResName);
            }

            if( Battle.GeUtility.CheckArrayItems(m_Trails) == false)
            {
                Logger.LogErrorFormat("Effect {0} m_Trails Contain empty item,Pls Check!",strResName);
            }

            if( Battle.GeUtility.CheckArrayItems(m_SeqFrames) == false)
            {
                Logger.LogErrorFormat("Effect {0} m_SeqFrames Contain empty item,Pls Check!",strResName);
            }
*/
            // if( Battle.GeUtility.CheckArrayItems(m_ParticleEmitter) == false)
            // {
            //     Logger.LogErrorFormat("Effect {0} m_ParticleEmitter Contain empty item,Pls Check!",strResName);
            // }

            // if( Battle.GeUtility.CheckArrayItems(m_ParticleAnimator) == false)
            // {
            //     Logger.LogErrorFormat("Effect {0} m_ParticleAnimator Contain empty item,Pls Check!",strResName);
            // }
/*
            if( Battle.GeUtility.CheckArrayItems(m_Animators) == false)
            {
                Logger.LogErrorFormat("Effect {0} m_Animators Contain empty item,Pls Check!",strResName);
            }
*/            
            /*
			DestroyDelay[] destroyDelay = m_EffectSpace.m_GameObject.GetComponentsInChildren<DestroyDelay>();
			if(null != destroyDelay && destroyDelay.Length > 0)
			{
				Logger.LogErrorFormat("{0}还有DestroyDelay!!!!", m_Name);
				for (int i = 0; i < destroyDelay.Length; ++i)
				{
					destroyDelay[i].enabled = false;
					UnityEngine.Object.Destroy(destroyDelay[i].gameObject);
				}
			}*/

            /*            m_AudioProxy = m_EffectSpace.m_GameObject.GetComponent<EffectAudioProxy>();

                        m_Animations = m_EffectSpace.m_GameObject.GetComponentsInChildren<Animation>(true);
                        m_ParticleSys = m_EffectSpace.m_GameObject.GetComponentsInChildren<ParticleSystem>(true);


                        m_Trails = m_EffectSpace.m_GameObject.GetComponentsInChildren<TrailRenderer>(true);
                        m_SeqFrames = m_EffectSpace.m_GameObject.GetComponentsInChildren<FrameMaterials>(true);

                        m_ParticleEmitter = m_EffectSpace.m_GameObject.GetComponentsInChildren<ParticleEmitter>(true);
                        m_ParticleAnimator = m_EffectSpace.m_GameObject.GetComponentsInChildren<ParticleAnimator>(true);
                        m_Animators = m_EffectSpace.m_GameObject.GetComponentsInChildren<Animator>(true);*/


            if (m_ParticleSys == null)
            {
                Logger.LogErrorFormat("GeEffect is null path {0}", strResName);
            }
            else
            {
                m_ParticleOrient.Resize(m_ParticleSys.Length);
                for (int i = 0; i < m_ParticleSys.Length; ++i)
                {
                    if (m_ParticleSys[i] == null)
                    {
                        //Logger.LogErrorFormat("GeEffect m_ParticleSys is null path {0} index {1} ", strResName, i);
                        continue;
                    }
                    if (m_ParticleSys[i].transform == null)
                    {
                        //Logger.LogErrorFormat("GeEffect m_ParticleSys transform is null path {0} index {1} ", strResName, i);
                        continue;
                    }
                    m_ParticleOrient[i] = m_ParticleSys[i].transform.localEulerAngles;
                }
            }

            m_EffectSpace.m_RootNode = new GameObject(m_EffectSpace.m_GameObject.name + "_EffRoot");
            if(m_EffectSpace.m_RootNode)
            {
                m_EffectSpace.m_RootNode.tag = "EffectModel";
                if (!this.m_EffectSpace.m_ParentNode)
                    this.m_EffectSpace.m_RootNode.transform.position = initPos;
                else
                    m_EffectSpace.m_RootNode.transform.SetParent(this.m_EffectSpace.m_ParentNode.transform, false);

                m_EffectSpace.m_EffectNode = new GameObject(m_EffectSpace.m_GameObject.name + "_TMNode");
                if(m_EffectSpace.m_EffectNode)
                {
                    m_EffectSpace.m_EffectNode.transform.SetParent(m_EffectSpace.m_RootNode.transform, false);
                    m_EffectSpace.m_GameObject.transform.SetParent(m_EffectSpace.m_EffectNode.transform, false);


                    if (m_EffectSpace.m_EffectNode.transform.localScale != m_EffectFrame.localScale) { 
                        _ScaleEffect(m_EffectFrame.localScale.x);
                        scaled = true;
                        scaledValue = m_EffectFrame.localScale.x;
                    }

                    m_EffectSpace.m_EffectNode.transform.localRotation = m_EffectFrame.localRotation;
                    m_EffectSpace.m_EffectNode.transform.localScale = m_EffectFrame.localScale;

                    this.m_EffectSpace.m_EffectNode.transform.localPosition = m_EffectFrame.localPosition;

                    if (time > 0)
                        m_TimeLenMS = (uint)(time * 1000);
                    else
						m_TimeLenMS = (uint)(defaultTimeLen * 1000);

					if (m_TimeLenMS <= 0 && m_EffectFrame != null && m_EffectFrame.time > 0)
					{
						m_TimeLenMS = (uint)(m_EffectFrame.time * 1000);
					}

                    m_StateEnd = false;
                    m_StatePause = true;
                    m_StateRemoved = false;

                    _SetFacing(bFaceLeft);

					MeshRenderer[] amr = proxy.m_MeshRenderer;//m_EffectSpace.m_RootNode.GetComponentsInChildren<MeshRenderer>();
                    if(null != amr)
                    {
                        for (int i = 0, amrCnt = amr.Length;i < amrCnt;++i)
                        {
                            if (null != amr[i])
                                amr[i].gameObject.tag = "EffectModel";
                        }
                    }
                    GeMeshRenderManager.instance.AddMeshObject(m_EffectSpace.m_EffectNode);
                    return true;
                }
            }
        }

        return false;
    }

	public bool OnReuse(EffectsFrames info, float time, Vector3 initPos, bool bFaceLeft, GameObject parentObj = null, bool useCube=false)
    {
		try{
            if (this.useCube && useCube)
                return true;

			this.useCube = useCube;
			if(this.useCube)
				return false;

			Reset();

			m_EffectFrame = info;

			if (m_EffectFrame != null)
				m_TimeType = m_EffectFrame.timetype;

			this.m_EffectSpace.m_ParentNode = parentObj;

            //Logger.LogErrorFormat("effect name:{0}", m_Name);

            if (m_EffectSpace.m_RootNode != null)
            {
                if (!this.m_EffectSpace.m_ParentNode)
                    this.m_EffectSpace.m_RootNode.transform.position = initPos;
                else
                    m_EffectSpace.m_RootNode.transform.SetParent(this.m_EffectSpace.m_ParentNode.transform, false);
            }

			if (m_EffectSpace.m_EffectNode != null && m_EffectSpace.m_GameObject != null)
			{
				m_EffectSpace.m_EffectNode.transform.SetParent(m_EffectSpace.m_RootNode.transform, false);
				m_EffectSpace.m_GameObject.transform.SetParent(m_EffectSpace.m_EffectNode.transform, false);

				if (m_EffectSpace.m_EffectNode.transform.localScale != m_EffectFrame.localScale)
				{
					_ScaleEffect(m_EffectFrame.localScale.x);
					scaled = true;
					scaledValue = m_EffectFrame.localScale.x;
				}

				m_EffectSpace.m_EffectNode.transform.localRotation = m_EffectFrame.localRotation;
				m_EffectSpace.m_EffectNode.transform.localScale = m_EffectFrame.localScale;

				this.m_EffectSpace.m_EffectNode.transform.localPosition = m_EffectFrame.localPosition;

				if (time > 0)
					m_TimeLenMS = (uint)(time * 1000);
				else
					m_TimeLenMS = (uint)(defaultTimeLen * 1000);

				if (m_TimeLenMS <= 0 && m_EffectFrame != null && m_EffectFrame.time > 0)
				{
					m_TimeLenMS = (uint)(m_EffectFrame.time * 1000);
				}

				m_StateEnd = false;
				m_StatePause = true;
				m_StateRemoved = false;

				_SetFacing(bFaceLeft);

				GeMeshRenderManager.instance.AddMeshObject(m_EffectSpace.m_EffectNode);
			}
		}
		catch(System.Exception e)
		{
			#if UNITY_EDITOR
			Logger.LogErrorFormat("onuse {0} has error:{1} ", m_Name, e.ToString());
			#endif

            return false;
		}

        return true;
    }

    public void OnRecycle()
    {
        touchGround = false;
        if (mGBController != null)
        {
            mGBController.Release();
            mGBController = null;
        }
        if (m_EffectSpace.m_RootNode != null)
        {
            if (m_EffectSpace.m_EffectNode != null)
            {
                if (null != m_EffectSpace.m_GameObject)
                {
                    Resume();
                    _Stop();
                    if (scaled)
                    {
						scaled = false;
                        _ScaleEffect(1.0f / scaledValue);
                    }

                    _SetFacing(m_EffectSpace.m_bFaceLeft);

                    for (int i = 0; i < m_ParticleSys.Length; ++i)
                        if (m_ParticleSys[i] != null)
                            m_ParticleSys[i].transform.localEulerAngles = m_ParticleOrient[i];


                }
            }
        }
    }

    public void Deinit()
    {
		// touchGround = false;
        // if (m_EffectSpace.m_RootNode != null)
        // { 
        //     if (m_EffectSpace.m_EffectNode != null)
        //     {
        //         if (null != m_EffectSpace.m_GameObject)
        //         {
        //             Resume();
        //             _Stop();
        //             if (scaled)
        //             {
        //                 _ScaleEffect(1.0f / scaledValue);
        //             }
        //             
        //             _SetFacing(m_EffectSpace.m_bFaceLeft);
        // 
        //             for (int i = 0; i < m_ParticleSys.Length; ++i)
        //                 if (m_ParticleSys[i] != null)
        //                     m_ParticleSys[i].transform.localEulerAngles = m_ParticleOrient[i];
        // 
        //             m_EffectSpace.m_GameObject.transform.SetParent(null, false);
        //             Singleton<CGameObjectPool>.instance.RecycleGameObject(m_EffectSpace.m_GameObject);
        //             m_EffectSpace.m_GameObject = null;
        //         }
        //         else
        //         {
        //             string msg = StringHelper.Formater.AppendFormat("m_GameObject == null on Destroy!({0})", m_Name).ToString();
        //             //Debug.Assert(m_EffectSpace.m_GameObject, msg);
        //         }
        // 
        //         GameObject.Destroy(m_EffectSpace.m_EffectNode);
        //         m_EffectSpace.m_EffectNode = null;
        //     }
        // 
        //     GameObject.Destroy(m_EffectSpace.m_RootNode);
        //     m_EffectSpace.m_RootNode = null;
        // }
        // m_EffectSpace.m_ParentNode = null;

        OnRecycle();

        if (null != m_EffectSpace.m_GameObject)
        {
            m_EffectSpace.m_GameObject.transform.SetParent(null, false);
            Singleton<CGameObjectPool>.instance.RecycleGameObject(m_EffectSpace.m_GameObject);
            m_EffectSpace.m_GameObject = null;
        }

        if(null != m_EffectSpace.m_EffectNode)
        {
            GameObject.Destroy(m_EffectSpace.m_EffectNode);
            m_EffectSpace.m_EffectNode = null;
        }

        if (null != m_EffectSpace.m_RootNode)
        {
            GameObject.Destroy(m_EffectSpace.m_RootNode);
            m_EffectSpace.m_RootNode = null;
        }

        m_EffectSpace.m_ParentNode = null;
    }

    public void Play(bool bIsLoop)
    {
        //Logger.LogErrorFormat("Play {0} 1", GetEffectName());

        if (createdInBackMode)
        {
            var cmd = PlayAnimationEffectGBCommand.Acquire();
            cmd._this = this;
            cmd.isLoop = bIsLoop;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEffectBackCmdType.Play_Animation, cmd);
            return;
        }

        if (m_EffectSpace.m_GameObject == null)
            return;

		//Logger.LogErrorFormat("Play {0} 2", GetEffectName());

        m_IsLoop = bIsLoop;
        m_StateEnd = false;
        m_StatePause = false;

        if (null != m_AudioProxy)
            m_AudioProxy.TriggerAudio(EffectAudioProxy.AudioTigger.OnPlay);

        if (null != m_ParticleSys)
        {
            ParticleSystem ps = null;
            for(int i = 0; i < m_ParticleSys.Length; ++ i)
            {
                ps = m_ParticleSys[i];
                if(ps)
                {
                    if (!m_LockLoop)
                    {
                        ps.loop = bIsLoop;
                    }

                    ps.Clear();
                    ps.Play();
                }
            }
        }

        if (null != m_Animations)
        {
            Animation ani = null;
            for (int i = 0; i < m_Animations.Length; ++i)
            {
                ani = m_Animations[i];
                if(ani)
                {
                    if (!m_LockLoop)
                        ani.wrapMode = bIsLoop ? WrapMode.Loop : WrapMode.Once;
                    ani.Stop();
                    ani.Rewind();
                    ani.Play();
                }
            }
        }

        if (m_Animators != null)
        {
            Animator ant = null;
            for (int i = 0; i < m_Animators.Length; ++i)
            {
                ant = m_Animators[i];
                if (ant != null)
                {
                    if (ant.runtimeAnimatorController == null)
                        continue;

                    AnimationClip[] curClip = ant.runtimeAnimatorController.animationClips;

                    if (!m_LockLoop)
                    {
                        for (int j = 0; j < curClip.Length; ++j)
                            curClip[j].wrapMode = bIsLoop ? WrapMode.Loop : WrapMode.Once;
                    }

                    //ant.playbackTime = 0.0f;
                    //ant.SetTime(0);
                    int hashname = ant.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    ant.Play(hashname,-1,0);
                }
            }
        }

        if(m_SeqFrames != null)
        {
            FrameMaterials fm = null;
            for (int i = 0; i < m_SeqFrames.Length; ++i)
            {
                fm = m_SeqFrames[i];
                if (fm != null)
                    fm.Start();
            }
        }

        if(m_SpineAnimations != null)
        {
            for(int i = 0; i < m_SpineAnimations.Count; ++i)
            {
                m_SpineAnimations[i].ReWind();
            }
        }
    }

	public void SetLayer(int layer)
	{
        if (createdInBackMode)
        {
            var cmd = LayerEffectGBCommand.Acquire();
            cmd._this = this;
            cmd.layer = layer;
            GBController.RecordCmd((int)GeEffectBackCmdType.Layer, cmd);
            return;
        }

        if (GetRootNode() == null) return;
        Renderer[] amr = GetRootNode().GetComponentsInChildren<Renderer>();
		for(int i =0,icnt = amr.Length;i < icnt;++i)
		{
            if(amr[i] == null) continue;

			amr[i].gameObject.layer = layer;
		}
	}

    public void SetPosition(Vector3 Pos)
    {
		//if (this.m_EffectSpace.m_ParentNode != null)
		{
			if (this.m_EffectSpace.m_RootNode != null)
				this.m_EffectSpace.m_RootNode.transform.position = Pos;
		}
        if (IsCreatedInBackMode)
        {
            var cmd = PositionEffectGBCommand.Acquire();
            cmd.pos = Pos;
            cmd._this = this;
            GBController.RecordCmd((int)GeEffectBackCmdType.Position, cmd);
        }
    }

	public Vector3 GetPosition()
	{
		//if (null != this.m_EffectSpace.m_ParentNode)
		{
			if (this.m_EffectSpace.m_RootNode != null)
				return this.m_EffectSpace.m_RootNode.transform.position;
		}

		return Vector3.zero;
	}

	public void SetLocalPosition(Vector3 pos)
	{
		if (m_EffectSpace != null && m_EffectSpace.m_RootNode != null)
			this.m_EffectSpace.m_RootNode.transform.localPosition = pos;
        if (IsCreatedInBackMode)
        {
            var cmd = LocalPositionEffectGBCommand.Acquire();
            cmd.pos = pos;
            cmd._this = this;
            GBController.RecordCmd((int)GeEffectBackCmdType.Local_Position, cmd);
        }
    }

    public void SetLocalRotation(Quaternion rotation)
    {
        if (m_EffectSpace != null && m_EffectSpace.m_RootNode != null)
            this.m_EffectSpace.m_RootNode.transform.localRotation = rotation;
        if (IsCreatedInBackMode)
        {
            var cmd = RotationEffectGBCommand.Acquire();
            cmd.rot = rotation;
            cmd._this = this;
            GBController.RecordCmd((int)GeEffectBackCmdType.Rotation, cmd);
        }
    }

	public Vector3 GetLocalPosition()
	{
		if (m_EffectSpace != null && m_EffectSpace.m_RootNode != null)
			return this.m_EffectSpace.m_RootNode.transform.localPosition;

		return Vector3.zero;
	}

    public void SetSpeed(float speed)
    {
        m_PlaySpeed = speed;
        if (createdInBackMode)
        {
            var cmd = SpeedEffectGBCommand.Acquire();
            cmd._this = this;
            cmd.fSpeed = speed;
            GBController.RecordCmd((int)GeEffectBackCmdType.Speed, cmd);
            return;
        }
        if (null != m_ParticleSys)
        {
            ParticleSystem ps = null;
            for (int i = 0; i < m_ParticleSys.Length; ++i)
            {
                ps = m_ParticleSys[i];
                
                if(ps == null)
                {
                    continue;
                }

                ps.playbackSpeed = speed;
            }
        }

        if (null != m_Animations)
        {
            Animation ani = null;
            for (int i = 0; i < m_Animations.Length; ++i)
            {
                ani = m_Animations[i];

                if(ani == null)
                {
                    continue;
                }

                foreach (AnimationState ans in ani)
                {
                    if(ans != null)
                    {
                        ans.speed = speed;
                    }
                }
            }
        }

        if (m_Animators != null)
        {
            for (int i = 0; i < m_Animators.Length; ++i)
            {
                if(m_Animators[i] != null)
                {
                    m_Animators[i].speed = speed;     
                }
               
            }
        }
    }
    protected void _SetFacing(bool faceLeft)
    {
        m_EffectSpace.m_bFaceLeft = faceLeft;

        if(null == m_EffectSpace.m_ParentNode)
        {
            Vector3 tempScale = m_EffectSpace.m_RootNode.transform.localScale;
            tempScale.x = faceLeft ? -tempScale.x : tempScale.x;
            m_EffectSpace.m_RootNode.transform.localScale = tempScale;
        }
        // Vector3 tempPos = m_EffectSpace.m_EffectNode.transform.localPosition;
        // tempPos.x = faceLeft ? -tempPos.x : tempPos.x;
        // m_EffectSpace.m_EffectNode.transform.localPosition = tempPos;
        // 
        // Vector3 tempRot = m_EffectSpace.m_EffectNode.transform.localScale;
        // tempRot.x = faceLeft ? -tempRot.x : tempRot.x;
        // m_EffectSpace.m_EffectNode.transform.localScale = tempRot;

        /// 特效缩放特殊处理
        if (false && m_EffectSpace.m_bFaceLeft)
        {
            if (null != m_ParticleSys)
            {
                ParticleSystem ps = null;
                for (int i = 0; i < m_ParticleSys.Length; ++i)
                {
                    ps = m_ParticleSys[i];

                    if(ps == null) continue;

                    ps.startRotation = Mathf.PI - ps.startRotation;
                        
                    while (ps.startRotation > Mathf.PI * 2)
                        ps.startRotation -= Mathf.PI * 2;
                    while(ps.startRotation < 0)
                        ps.startRotation += Mathf.PI * 2;

                    Vector3 reflection = Vector3.Reflect(ps.transform.forward, Vector3.left);
                    Quaternion rotation = Quaternion.LookRotation(reflection);
                    ps.transform.rotation = rotation;

                    //ps.transform.LookAt(ps.transform.position + reflection);

                    //// Vector3 reflection = Vector3.Reflect(ps.transform.localToWorldMatrix.MultiplyVector(ps.transform.forward), Vector3.right);
                    //// //Quaternion rotation = Quaternion.LookRotation(ps.transform.worldToLocalMatrix.MultiplyVector(reflection));
                    //// //ps.transform.rotation = rotation;
                    //// ps.transform.LookAt(ps.transform.position + ps.transform.worldToLocalMatrix.MultiplyVector(reflection));
                }
            }
        }
    }

    public void Pause()
    {
        if (m_StatePause)
            return;

        if (createdInBackMode)
        {
            var cmd = PauseEffectAnimationGBCommand.Acquire();
            cmd._this = this;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEffectBackCmdType.Pause_Animation, cmd);
            m_StatePause = true;
            return;
        }

        if (null == m_EffectSpace.m_GameObject)
            return;

        if (m_Animators != null)
        {
            m_AnimatorSpeed.Resize(m_Animators.Length);
            for (int i = 0; i < m_Animators.Length; ++i)
            {
                Animator ani = m_Animators[i];
                m_AnimatorSpeed[i] = 0.0f;
                if (ani != null)
                {
                    m_AnimatorSpeed[i] = ani.speed;
                    ani.speed = 0.0f;
                }
            }
        }

        if (null != m_Animations)
        {
            Animation ani = null;
            for (int i = 0; i < m_Animations.Length; ++i)
            {
                ani = m_Animations[i];

                if(ani == null) continue;

                foreach (AnimationState ans in ani)
                    ans.speed = 0.0f;
            }
        }

        if (null != m_ParticleSys)
        {
            for (int i = 0; i < m_ParticleSys.Length; ++i)
            {
                if(m_ParticleSys[i] == null) continue;
                m_ParticleSys[i].Pause();
            }
        }

        m_StatePause = true;
    }

    public void Resume()
    {
        if (!m_StatePause)
            return;

        if (createdInBackMode)
        {
            var cmd = ResumeEffectAnimationGBCommand.Acquire();
            cmd._this = this;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEffectBackCmdType.Resume_Animation, cmd);
            m_StatePause = false;
            return;
        }
        if (null == m_EffectSpace.m_GameObject)
            return;

        if (m_Animators != null)
        {
            m_AnimatorSpeed.Resize(m_Animators.Length);
            for (int i = 0; i < m_Animators.Length; ++i)
            {
                Animator ani = m_Animators[i];
                if (ani != null)
                {
                    ani.speed = m_AnimatorSpeed[i];
                }
            }
        }

        if (null != m_Animations)
        {
            Animation ani = null;
            for (int i = 0; i < m_Animations.Length; ++i)
            {
                ani = m_Animations[i];
                if (null == ani) continue;

                foreach (AnimationState ans in ani)
                    ans.speed = m_PlaySpeed;
            }
        }

        if (null != m_ParticleSys)
        {
            for (int i = 0; i < m_ParticleSys.Length; ++i)
            {
                if(m_ParticleSys[i] == null) continue;
                m_ParticleSys[i].Play();
            }
        }

        m_StatePause = false;
    }

    public void Update(int delta)
    {
        m_ElapsedTimeMS += (uint)(delta * m_PlaySpeed);
        if (m_ElapsedTimeMS > m_TimeLenMS/* && !m_IsLoop*/)
        {
            m_StateEnd = true;
        }
    }

    public string GetEffectName()
    {
        return m_Name;
    }
    public EffectTimeType GetTimeType()
    {
        //return m_EffectFrame.timetype;
		return m_TimeType;
    }

	public float GetTimeLen()
	{
		return m_TimeLenMS;
	}

	public void SetTimeLen(int d)
	{
		m_TimeLenMS = (uint)d;
        if (createdInBackMode)
        {
            var cmd = TimeLenEffectGBComand.Acquire();
            cmd._this = this;
            cmd.timeLen = d;
            GBController.RecordCmd((int)GeEffectBackCmdType.Time_Len, cmd);
        }
    }

    public void ResetElapsedTime()
    {
        m_ElapsedTimeMS = 0;
        if (createdInBackMode)
        {
            var cmd = ResetElapseTimeEffectGBCommand.Acquire();
            cmd._this = this;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEffectBackCmdType.Reset_Elapse_Time, cmd);
        }
    }

	public void SetTouchGround(bool flag)
	{
		touchGround = flag;
	}

	public bool IsAlwaysTouchGround()
	{
		return touchGround;
	}

    private float _getDefaultTimeLen()
    {
		float defTimeLen = 0.0f;
		defTimeLen = GeEffectProxy.GetDefaultTimeLen(m_ParticleSys, m_Animations, m_SeqFrames, m_Animators);

		/*
        float defTimeLen = 0.0f;

        if (null != m_ParticleSys)
        {
            ParticleSystem ps = null;
            float timeLen = 0.0f;
            for (int i = 0; i < m_ParticleSys.Length; ++i)
            {
                ps = m_ParticleSys[i];
                timeLen = ps.duration + ps.startDelay + ps.startLifetime;
                if (timeLen > defTimeLen)
                    defTimeLen = timeLen;
            }
        }

        if (null != m_Animations)
        {
            Animation ani = null;
            for (int i = 0; i < m_Animations.Length; ++i)
            {
                ani = m_Animations[i];
                if (ani.clip != null && defTimeLen < ani.clip.length)
                    defTimeLen = ani.clip.length;
            }
        }

        if (null != m_SeqFrames)
        {
            FrameMaterials fm = null;
            for (int i = 0; i < m_SeqFrames.Length; ++i)
            {
                fm = m_SeqFrames[i];
                if (fm != null && defTimeLen < fm.Duration())
                    defTimeLen = fm.Duration();
            }
        }

        if (m_Animators != null)
        {
            Animator anr = null;
            for (int i = 0; i < m_Animators.Length; ++i)
            {
                anr = m_Animators[i];
                if(null == anr) continue;
                
                anr.Rebind();
                if (anr.runtimeAnimatorController == null)
                    continue;

                AnimatorClipInfo[] clipinfo = anr.GetCurrentAnimatorClipInfo(0);
                for (int j = 0; j < clipinfo.Length; ++j)
                {
                    if (clipinfo[j].clip != null && defTimeLen < clipinfo[j].clip.length)
                    {
                        defTimeLen = clipinfo[j].clip.length;
                    }
                }
            }
        }
*/
        return defTimeLen > 0.0f ? defTimeLen : defaultTimeLen;
    }

    public bool IsDead()
    {
        return m_StateEnd || m_StateRemoved;
    }

    public void Remove()
    {
        m_StateRemoved = true;
        if (createdInBackMode)
        {
            GBController.RecordCmd((int)GeEffectBackCmdType.Destroy, null);
        }
    }

    public void DoBackToFront()
    {
        GBController.FlipToFront();
        if (m_ElapsedTimeMS >= m_TimeLenMS)
        {
            m_StateEnd = true;
        }
        if (m_StateEnd)
        {
            Deinit();
        }
    }

    public void SetScale(float scale)
	{
		scaled = true;
		scaledValue = scale;
		_ScaleEffect(scale);
		var localScale = new Vector3(scale, scale, scale);
        if(m_EffectSpace.m_EffectNode != null)
		    m_EffectSpace.m_EffectNode.transform.localScale = localScale;
        else
            Logger.LogErrorFormat("SetScale effectName {0} invalid", m_Name);
        if (IsCreatedInBackMode)
        {
            var cmd = LocalScaleEffectGBCommand.Acquire();
            cmd.scale = scale;
            cmd.vecScale = localScale;
            cmd._this = this;
            GBController.RecordCmd((int)GeEffectBackCmdType.Scale_Effect, cmd);
        }
    }

	public void SetScale(float sx, float sy, float sz)
	{
		var localScale = new Vector3(sx, sy, sz);
        if (m_EffectSpace.m_EffectNode != null)
            m_EffectSpace.m_EffectNode.transform.localScale = localScale;
        else
            Logger.LogErrorFormat("SetScale effectName {0} invalid", m_Name);
        if (IsCreatedInBackMode)
        {
            var cmd = ScaleEffectGBCommand.Acquire();
            cmd.scale = localScale;
            cmd._this = this;
            GBController.RecordCmd((int)GeEffectBackCmdType.Scale, cmd);
        }
    }

#else

public float GetDefaultTimeLen(){return 0.0f;}
public static void ClearDefaultTimeMap(){}
public GeEffectEx(){}
public void Reset(){}
public bool Init(string strResName, EffectsFrames info, float time,Vector3 initPos,bool bFaceLeft,GameObject parentObj = null, bool useCube=false){return false;}
public bool OnReuse(EffectsFrames info, float time, Vector3 initPos, bool bFaceLeft, GameObject parentObj = null, bool useCube=false){return false;}
public void OnRecycle(){}
public GameObject GetRootNode() { return null; }
public void Deinit(){}
public void Play(bool bIsLoop){}
public void SetLayer(int layer){}
public void SetPosition(Vector3 Pos){}
public Vector3 GetPosition(){return Vector3.zero;}
public void SetLocalPosition(Vector3 pos){}
public Vector3 GetLocalPosition(){return Vector3.zero;}
public void SetSpeed(float speed){}
protected void _SetFacing(bool faceLeft){}
public void Pause(){}
public void Resume(){}
public void Update(int delta){}
public string GetEffectName(){return "";}
public EffectTimeType GetTimeType(){return EffectTimeType.BUFF;}
public float GetTimeLen(){return 0.0f;}
public void SetTimeLen(int d){}
public void ResetElapsedTime(){}
public void SetTouchGround(bool flag){}
public bool IsAlwaysTouchGround(){return false;}
public bool IsDead(){return false;}
public void Remove(){}
public void SetScale(float scale){}
public void SetScale(float sx, float sy, float sz){}
public void SetVisible(bool flag) {}
public bool isVisible() { return false; }
#endif

#if !LOGIC_SERVER
    #region 私有方法

    enum Axis
    {
        X,
        Y,
        Z,
    }
    void _MirrorRotationAxis(Axis axis, ref Vector3 eulerAngle)
    {
        switch (axis)
        {
            case Axis.X: eulerAngle.x = 180 - eulerAngle.x; return;
            case Axis.Y: eulerAngle.y = 180 - eulerAngle.y; return;
            case Axis.Z: eulerAngle.z = 180 - eulerAngle.z; return;
        }
    }

    void _Stop()
    {
        if (m_Animators != null)
        {
            Animator ant = null;
            for (int i = 0; i < m_Animators.Length; ++i)
            {
                ant = m_Animators[i];
                if (ant != null)
                    ant.StopPlayback();
            }
        }
    }

    void _ScaleEffect(float scale)
    {
        if (null != m_ParticleSys)
        {
            ParticleSystem ps = null;
            for (int i = 0; i < m_ParticleSys.Length; ++i)
            {
                ps = m_ParticleSys[i];
                
                if(ps == null) continue;
                if (ps.scalingMode == ParticleSystemScalingMode.Hierarchy)
                    continue;

                ps.startSpeed *= scale;
                ps.startSize *= scale;
                ps.gravityModifier *= scale;
            }
        }

        if (m_Trails != null)
        {
            TrailRenderer trail = null;
            for(int i = 0; i < m_Trails.Length; ++ i)
            {
                trail = m_Trails[i];

                if(trail == null) continue;

                trail.startWidth *= scale;
                trail.endWidth *= scale;
            }
        }

        // //apply scaling to emitters
        // if (null != m_ParticleEmitter)
        // {
        //     ParticleEmitter emitter = null;
        //     for (int i = 0;i< m_ParticleEmitter.Length;++ i )
        //     {
        //         emitter = m_ParticleEmitter[i];

        //         if(emitter == null) continue;

        //         emitter.minSize *= scale;
        //         emitter.maxSize *= scale;
        //         emitter.worldVelocity *= scale;
        //         emitter.localVelocity *= scale;
        //         emitter.rndVelocity *= scale;
        //     }
        // }

        // //apply scaling to animators
        // if (m_ParticleAnimator != null)
        // {
        //     ParticleAnimator animator = null;
        //     for(int i = 0; i < m_ParticleAnimator.Length; ++ i)
        //     {
        //         animator = m_ParticleAnimator[i];
                
        //         if(animator == null) continue;

        //         animator.force *= scale;
        //         animator.rndForce *= scale;
        //     }
        // }
    }

	public GameObject GetRootNode()
	{
		return m_EffectSpace.m_RootNode;
	}
	public GameObject GetParentNode()
    {
        return m_EffectSpace.m_ParentNode;
    }
	
	public void SetParentNode(GameObject newParent)
    {
        if (newParent == null)
            return;

        GameObject old = m_EffectSpace.m_ParentNode;
        if (old == null)
            return;

        if(m_EffectSpace.m_RootNode != null)
            m_EffectSpace.m_RootNode.transform.SetParent(newParent.transform, false);

        m_EffectSpace.m_ParentNode = newParent;
    }

	public void SetVisible(bool flag)
	{
		if (useCube)
			return;
        if (createdInBackMode)
        {
            var cmd = VisibleEffectGBCommand.Acquire();
            cmd._this = this;
            cmd.isVisible = flag;
            GBController.RecordCmd((int)GeEffectBackCmdType.Visible, cmd);
            return;
        }
        var root = GetRootNode();
		if (root != null)
			root.CustomActive(flag);
	}

	public bool isVisible()
	{
		var root = GetRootNode();
		if (root != null)
			return root.activeSelf;

		return false;
	}
    #endregion

    #region Private param

    protected string m_Name = ""; /// 特效名

    protected EffectsFrames m_EffectFrame = null;
	protected EffectTimeType m_TimeType = EffectTimeType.SYNC_ANIMATION;
    protected float m_PlaySpeed = 1.0f;

    protected uint m_ElapsedTimeMS = 0;
    protected uint m_TimeLenMS = 0;

    protected Animation[] m_Animations = null;
    protected ParticleSystem[] m_ParticleSys = null;
    protected TrailRenderer[] m_Trails = null;
    protected FrameMaterials[] m_SeqFrames = null;
//    protected ParticleEmitter[] m_ParticleEmitter = null;
//    protected ParticleAnimator[] m_ParticleAnimator = null;
    protected Animator[] m_Animators = null;
    protected bool m_LockLoop = false;

    protected List<ISkeletonAnimation> m_SpineAnimations = null;
    protected HGBase.LazyArray<float> m_AnimatorSpeed = new HGBase.LazyArray<float>(4,0.0f);
    protected HGBase.LazyArray<Vector3> m_ParticleOrient = new HGBase.LazyArray<Vector3>(4, Vector3.zero);

    protected bool m_StatePause = false;
    protected bool m_StateEnd = false;
    protected bool m_StateRemoved = false;
    protected bool m_IsLoop = false;

    protected class GeSpaceDesc
    {
        public GameObject m_ParentNode = null;
        public GameObject m_RootNode = null;
        public GameObject m_EffectNode = null;
        public GameObject m_GameObject = null;
        public Vector3 m_LocalPos = Vector3.zero;
        public Quaternion m_LocalRot = Quaternion.identity;
        public Vector3 m_LocalScale = Vector3.one;
        public bool m_bFaceLeft = false;
    }
    protected GeSpaceDesc m_EffectSpace = new GeSpaceDesc();

    protected EffectAudioProxy m_AudioProxy = null;

    #endregion
#endif
}
