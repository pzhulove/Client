using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameClient;


public partial class GeAttach
{
    public GameObject root = null;
    public GeAvatar geAvatar = null;
    protected GeAttachPhaseProxy attachPhaseProxy = null;

    protected string     name = "";
    protected string     attachNodeName = "";
    protected GameObject attachNode;
    //protected Animation  animation;
    protected GeAnimationManager animation;
    //protected int        resID;
    protected bool       isCreate = false;
    protected bool       needDestroy = false;
    protected string     currentActionName = "";
    protected string     attachResPath = "";
	public bool 		 cached = false;
	protected bool 		 needDestroyRoot = true;

    protected string lastPhaseName = "";
    protected int lastPhaseLv = 0;
    protected int curlayer = 9;
    protected bool m_Visible = true;
    protected GeActorEx.DisplayMode m_DisplayMode = GeActorEx.DisplayMode.Normal;

    #region �첽����

    protected int dstLayer = 9;
    protected bool layerdirty = false;
    protected bool pauseAnimationDirty = false;

    protected bool actionDirty = true;
    protected string actionName = null;
    protected bool actionLoop = false;
    protected float actionSpeed = 1.0f;
    protected float actionOffset = 0.0f;
    private bool createdInBackMode = false;
    public bool isCreatedInBackMode { get { return createdInBackMode; } }
#if !LOGIC_SERVER
    private AttachGraphicBack mGBController = null;
    AttachGraphicBack GBController
    {
        get
        {
            if (mGBController == null)
            {
                mGBController = AttachGraphicBack.Acquire();
            }
            return mGBController;
        }
    }
#endif

    protected uint asyncReqHandle = CGameObjectPool.INVILID_HANDLE;
    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
    {
        if (null == userData)
        {
            Tenmove.Runtime.Debugger.LogError("User data can not be null!");
            return;
        }

        GeAttach _this = userData as GeAttach;
        if (null == _this)
        {
            Tenmove.Runtime.Debugger.LogError("User data type '{0}' is NOT GeAttach!");
            return;
        }

        GameObject go = asset as GameObject;
        if (null == go)
        {
            Tenmove.Runtime.Debugger.LogError("Asset type '{0}' error!", asset.GetType());
            return;
        }
        if (AssetLoader.INVILID_HANDLE != _this.asyncReqHandle)
        {
            if ((uint)grpID == _this.asyncReqHandle)
            {
                _this._CreateImm(go, _this.attachNode, _this.attachNodeName);
                _this.asyncReqHandle = AssetLoader.INVILID_HANDLE;
                return;
            }
        }
        
        GameObject.Destroy(go);
    }

    static void _OnLoadAssetFailure(string assetPath, int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
        Logger.LogErrorFormat("[GeAttach]Load game object '{0}' has failed![Error:{1}]", assetPath, errorMessage);
    }

    public void UpdateAsync()
    {
        if (EngineConfig.useTMEngine)
            return;

            if (_IsLoading())
        {
            if(CGameObjectPool.instance.IsRequestDone( asyncReqHandle ))
            {
                GameObject attachObj = CGameObjectPool.instance.ExtractAsset(asyncReqHandle) as GameObject;
                if (null != attachObj)
                {
                    _CreateImm(attachObj, attachNode, attachNodeName);
                }

                asyncReqHandle = CGameObjectPool.INVILID_HANDLE;
            }
        }
    }
    
    #endregion

    public 	GeAttach(GameObject giveRoot)
	{
		root = giveRoot;
		needDestroyRoot = false;
    }

    public GameObject Root
    {
        get { return root; }
    }    
 
    public GeAttach(string name)
    {
        this.name = name;
    }

    public string Name
    {
        get { return name; } 
    }
    public void ResetAttachNodeInBackMode(GameObject parentNode)
    {
#if !LOGIC_SERVER
        if (GBController != null)
        {
            GBController.parentNode = parentNode;
        }
#endif
    }
    public void Destroy()
    {
        needDestroy = true;
        if (_IsLoading())
        {
            if (EngineConfig.useTMEngine)
                CGameObjectPool.AbortAcquireRequest(asyncReqHandle);
            else
                CGameObjectPool.instance.AbortRequest(asyncReqHandle);
            asyncReqHandle = CGameObjectPool.INVILID_HANDLE;
        }
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            GBController.RecordCmd((int)GeAttachBackCmdType.Destroy, null);
            createdInBackMode = false;
        }
#endif
    }

    public bool NeedDestroy
    {
        get { return needDestroy; }
    }

    public bool IsCreate
    {
        get { return isCreate;  }
    }

	public string ResPath
	{
		get {return attachResPath;}
	}

	public string AttachNodeName
	{
		get {return attachNodeName;}
	}

	public GeAnimationManager AnimationManager
	{
		get {
			return animation;
		}
	}


	public void Reset()
	{
        if (null != animation)
            animation.Stop();
		currentActionName = "";
        //Debug.Log("Cur animation offset:" + animation.GetCurActionOffset());
    }

    public void Create(string attachRes, GameObject parent, string attachName, bool copyInPool = true,bool asyncLoad = true,bool highPriority = false)
    {
        needDestroy = false;
        if (isCreate)
        {
            if(attachName == attachNodeName && parent == attachNode && attachResPath == attachRes)
            {
                return;
            }
            else
            {
                DestroyImm();
            }
        }
#if !LOGIC_SERVER
        if (FrameSync.instance.IsInChasingMode)
        {
            var cmd = AttachCreateGBCommand.Acquire();
            cmd.attachRes = attachRes;
            cmd.parent = parent;
            cmd.attachName = attachName;
            cmd.copyInPool = copyInPool;
            cmd.asyncLoad = asyncLoad;
            cmd.highPriority = highPriority;
            cmd.attach = this;
            attachNodeName = attachName;
            createdInBackMode = true;
            GBController.RecordCmd((int)GeAttachBackCmdType.Create, cmd);
            return;
        }
#endif
        createdInBackMode = false;

        uint flag = copyInPool ? ((uint)GameObjectPoolFlag.ReserveLast | (uint)GameObjectPoolFlag.HideAfterLoad) : (uint)GameObjectPoolFlag.HideAfterLoad;
        flag |= highPriority ? (uint)GameObjectPoolFlag.HighPriority : 0;
        if (asyncLoad)
        {
            if (EngineConfig.useTMEngine)
            {
                if (AssetLoader.INVILID_HANDLE != asyncReqHandle)
                    //AssetLoader.AbortLoadRequest(asyncReqHandle);
                    CGameObjectPool.AbortAcquireRequest(asyncReqHandle);

                //asyncReqHandle = (uint)AssetLoader.LoadResAsGameObjectAsync(attachRes, m_AssetLoadCallbacks, this, flag, 0xcdcdc313);
                asyncReqHandle = (uint)CGameObjectPool.GetGameObjectAsync(attachRes, m_AssetLoadCallbacks, this, flag, 0xcdcdc313);
            }
            else
            {
                if (CGameObjectPool.INVILID_HANDLE != asyncReqHandle)
                    CGameObjectPool.instance.AbortRequest(asyncReqHandle);

                asyncReqHandle = CGameObjectPool.instance.GetGameObjectAsync(attachRes, enResourceType.BattleScene, flag, 0xcdcdc313);
            }

            attachNode = parent;
            attachNodeName = attachName;
        }
        else
        {
            GameObject attachObj = CGameObjectPool.instance.GetGameObject(attachRes, enResourceType.BattleScene, flag);
            if (attachObj == null)
            {
                Logger.LogWarningFormat("Create attachment gameobject [AttachRes:{0}] has failed![AttachNode:{1}]", attachRes, attachNode);
                return;
            }

            _CreateImm(attachObj, parent, attachName);
        }

        attachResPath = attachRes;
		//Logger.LogErrorFormat("create attach!!!! name:{0} attachName:{1} res:{2}", attachName, attachName, attachRes);
    }
    public void DeInit()
    {
        DestroyImm();
        Recycle();
    }
    public void DestroyImm()
    {
        if (_IsLoading())
        {
            if (EngineConfig.useTMEngine)
                //AssetLoader.AbortLoadRequest(asyncReqHandle);
                CGameObjectPool.AbortAcquireRequest(asyncReqHandle);
            else if (CGameObjectPool.instance != null)
                CGameObjectPool.instance.AbortRequest(asyncReqHandle);

            asyncReqHandle = CGameObjectPool.INVILID_HANDLE;
        }

        if (null != animation)
            animation.Deinit();
        _ClearPhase();

		if(root && needDestroyRoot)
        {
            //Logger.LogErrorFormat("destroy attach!!!! name:{0} attachName:{1} res:{2}", name, attachNodeName, ResPath);
            root.transform.SetParent(null,false);
            root.transform.position = Vector3.zero;
            root.transform.eulerAngles = Vector3.zero;
            root.transform.localPosition = Vector3.zero;
            root.transform.localEulerAngles = Vector3.zero;

            if (9 != curlayer)
                SetLayer(9);

            if (EngineConfig.useTMEngine)
                CGameObjectPool.RecycleGameObjectEx(root);
            else if (CGameObjectPool.instance != null)
                CGameObjectPool.instance.RecycleGameObject(root);
            root = null;
            attachNode = null;
            attachNodeName = "";
        }

        isCreate = false;
        needDestroy = false;
        m_ClearPhase = false;
        m_PhaseDirty = false;
        layerdirty = false;
        curlayer = 9;
        attachPhaseProxy = null;
        m_DisplayMode = GeActorEx.DisplayMode.Normal;
        m_Visible = true;
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            GBController.RecordCmd((int)GeAttachBackCmdType.Destroy, null);
            createdInBackMode = false;
        }
#endif
    }
    public void Recycle()
    {
#if !LOGIC_SERVER
        if (mGBController != null)
        {
            mGBController.Release();
            mGBController = null;
        }
#endif
    }
    protected void _CreateImm(GameObject go,GameObject parent,string AttachName)
    {
        // todo: 修改显隐
        go.SetActive(true);
        root = go;
        SetVisible(m_Visible);

        if ( go == null || parent == null )
        {
            if(parent == null && go != null)
            {
                Debug.LogErrorFormat("##### Attach:{0}' can not find parent!(AttachName:{1})", go.name, AttachName);
            }
                
            Destroy();
            return;
        }

        //animation   = root.GetComponent<Animation>();
        if (null == animation)
            animation = new GeAnimationManager();
        animation.Init(root);
        animation.PlayAction("Anim_Idle01",1,true);
        attachNode  = parent;
        attachNodeName = AttachName;

        if (parent != null)
        {
            root.transform.SetParent(attachNode.transform, false);
        }

        if(layerdirty)
        {
            SetLayer(dstLayer);
            layerdirty = false;
        }

        if(!m_ClearPhase)
        {
            if(m_PhaseDirty)
            {
                _ChangePhase(m_PhaseCommand.phaseEffect, m_PhaseCommand.phaseIdx, m_PhaseCommand.forceAdditive);
                m_PhaseDirty = false;
            }
        }
        else
        {
            _ClearPhase();
            m_ClearPhase = false;
            m_PhaseDirty = false;
        }
        
        if(actionDirty)
        {
            actionDirty = false;
            float adjust = 0.0f;

            if (actionName != null)
            {
                if (null != geAvatar)
                    adjust = geAvatar.GetCurActionOffset();
                animation.PlayAction(actionName, actionSpeed, actionLoop, actionOffset + adjust);
            }
            else if (null != geAvatar)
            {
                animation.PlayAction(geAvatar.GetCurActionName(), geAvatar.GetCurActionSpeed(), geAvatar.GetCurActionLoop(), actionOffset + geAvatar.GetCurActionOffset());
            }

            geAvatar = null;
        }

        isCreate = true;
    }

    bool _IsLoading()
    {
        return CGameObjectPool.INVILID_HANDLE != asyncReqHandle;
    }

	public bool PlayAction(string name,float speed, bool loop=false,float offset = 0.0f)
    {
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var cmd = AttachPlayActionGBCommand.Acquire();
            cmd.name = name;
            cmd.speed = speed;
            cmd.loop = loop;
            cmd.offset = offset;
            cmd.timeStamp = FrameSync.GetTicksNow();
            cmd.attach = this;
            GBController.RecordCmd((int)GeAttachBackCmdType.Play_Action, cmd);
            return true;
        }
#endif
        if (_IsLoading())
        {
            actionDirty = true;
            actionName = name;
            actionLoop = loop;
            actionSpeed = speed;
            actionOffset = offset;
            return true;
        }
        else
        {
            //Debug.LogFormat("name:{0}   speed:{1}   loop:{2}",name,speed,loop);
            if (animation != null)
            {
                actionDirty = false;
                return animation.PlayAction(name, speed, loop, offset);
            }
            return false;
        }
    }
    public void StopAction()
    {
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var cmd = AttachStopActionGBCommand.Acquire();
            cmd.timeStamp = FrameSync.GetTicksNow();
            cmd.attach = this;
            GBController.RecordCmd((int)GeAttachBackCmdType.Stop_Action, cmd);
            return;
        }
#endif
        if (animation != null)
            animation.Stop();
    }
    public void PauseAnimation()
    {
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var cmd = AttachPauseAniGBCommand.Acquire();
            cmd.attach = this;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeAttachBackCmdType.Pause_Animation, cmd);
            return;
        }
#endif
        if (AnimationManager != null)
        {
            AnimationManager.Pause();
        }
        else
        {
            if (_IsLoading())
            {
                pauseAnimationDirty = true;
            }
        }
    }
    public void ResumeAnimation()
    {
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var cmd = AttachResumeAniGBCommand.Acquire();
            cmd.attach = this;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeAttachBackCmdType.Resume_Animation, cmd);
            return;
        }
#endif
        if (AnimationManager != null)
        {
            AnimationManager.Resume();
        }
        else
        {
            if (_IsLoading())
            {
                pauseAnimationDirty = false;
            }
        }
    }
    public string GetCurActionName()
    {
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var stopCmd = GBController.Get((int)GeAttachBackCmdType.Stop_Action);
            var playAnimCmd = GBController.Get((int)GeAttachBackCmdType.Play_Action) as AttachPlayActionGBCommand;
            if (stopCmd != null) return "";
            return playAnimCmd != null ? playAnimCmd.name : "";
        }
#endif
        if (null != animation)
            return animation.GetCurActionName();

        return "";
    }

    public void SetBindingPose()
	{
		//�ɵ�����������
		/*
		if (null != animation)
		{
            GeAnimDesc animDesc = animation.GetAnimClipDesc("Anim_Orign");
            if (null == animDesc.animClipData)
                animation.PlayAction("Anim_Orign", 1, false);

            if (null != animDesc.animClipData)
            {
                animDesc.animClipData.wrapMode = WrapMode.Loop;
                animDesc.animClipData.SampleAnimation(root, 0);
                animation.PlayAction("Anim_Orign", 1, false);
            }

        }*/
	}

    public GameObject GetAttachModel()
    {
        return root;
    }

	public void SetVisible(bool visible)
	{
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var cmd = AttachVisibleGBCommand.Acquire();
            cmd.isVisible = visible;
            cmd.attach = this;
            GBController.RecordCmd((int)GeAttachBackCmdType.Visible_Enable, cmd);
            return;
        }
#endif
        m_Visible = visible;

        if (root != null)
            root.SetActive(m_Visible & (m_DisplayMode == GeActorEx.DisplayMode.Normal));
    }

    public void SetDisplayMode(GeActorEx.DisplayMode displayMode)
    {
        if(m_DisplayMode != displayMode)
        {
            m_DisplayMode = displayMode;
            SetVisible(m_Visible);
        }
    }
 
    public void SetLayer(int layer)
    {
        if (layer == curlayer)
            return;

#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var cmd = AttachLayerGBCommand.Acquire();
            cmd.layer = layer;
            cmd.attach = this;
            GBController.RecordCmd((int)GeAttachBackCmdType.Layer_Set, cmd);
            return;
        }
#endif
        if (null == root)
        {
            if (_IsLoading())
            {
                layerdirty = true;
                dstLayer = layer;
            }
            else
                return;
        }
        else
        {
            Renderer[] amr = null;
            if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.GeAttachSetLayerBug))
                amr = root.GetComponentsInChildren<Renderer>(true);
            else
                amr = root.GetComponentsInChildren<Renderer>();
            
            for (int i = 0, icnt = amr.Length; i < icnt; ++i)
                amr[i].gameObject.layer = layer;

            root.layer = layer;
            curlayer = layer;
            layerdirty = false;
            dstLayer = layer;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    protected class PhaseMatSurfDesc
    {
        public PhaseMatSurfDesc(Material[] origMat, Renderer mr)
        {
            m_MeshRenderer = mr;
            m_OriginMatList = origMat;
        }
    
        public Renderer m_MeshRenderer = null;
        public Material[] m_OriginMatList = null;
    }
    protected List<PhaseMatSurfDesc> m_PhaseMatSurfDescList = new List<PhaseMatSurfDesc>();
    GameObject m_EffRoot = null;

    protected struct PhaseCommandDesc
    {
        public string phaseEffect;
        public int phaseIdx;
        public bool forceAdditive;
    }

    PhaseCommandDesc m_PhaseCommand = new PhaseCommandDesc();
    bool m_PhaseDirty = false;
    bool m_ClearPhase = false;
    public void DoBackToFront()
    {
#if !LOGIC_SERVER
        GBController.FlipToFront();
#endif
    }
    public void ChangePhase(string phaseEffect, int phaseIdx, bool forceAddtive = false)
    {
        if (lastPhaseName.Equals(phaseEffect) && phaseIdx == lastPhaseLv)
            return;
#if !LOGIC_SERVER
        if (createdInBackMode)
        {
            var cmd = AttachChangePhaseGBCommand.Acquire();
            cmd.phaseEffect = phaseEffect;
            cmd.phaseIdx = phaseIdx;
            cmd.forceAddtive = forceAddtive;
            cmd.attach = this;
            GBController.RecordCmd((int)GeAttachBackCmdType.Change_Phase, cmd);
            return;
        }
#endif

        if (_IsLoading())
        {
            m_PhaseCommand.phaseEffect = phaseEffect;
            m_PhaseCommand.phaseIdx = phaseIdx;
            m_PhaseCommand.forceAdditive = forceAddtive;
            m_ClearPhase = false;
            m_PhaseDirty = true;
        }
        else
            _ChangePhase(phaseEffect, phaseIdx, forceAddtive);
    }

    public void _FindChildren(string name, GameObject parent,ref List<GameObject> childList, bool includeInactive = false)
    {
        if (null == parent)
            return;

        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);
        for (int i = 0,icnt = children.Length;i<icnt;++i)
        {
            Transform curChild = children[i];
            if (curChild.name.TrimEnd() == name)
                childList.Add(curChild.gameObject);
        }
    }

    protected void _ChangePhase(string phaseEffect,int phaseIdx,bool forceAddtive = false)
    {
        if (null == root)
            return;

        Logger.LogWarningFormat("Change phase effect [{0}] stage index [{1}]", phaseEffect, phaseIdx);

        lastPhaseName = phaseEffect;
        lastPhaseLv = phaseIdx;

        if (attachPhaseProxy == null)
            attachPhaseProxy = root.GetComponent<GeAttachPhaseProxy>();

        if (attachPhaseProxy != null )
            attachPhaseProxy.ChangePhase(phaseEffect, phaseIdx, forceAddtive, curlayer);
    }
    
    protected void _ClearPhase()
    {
        if (root == null)
            return;

        if (attachPhaseProxy == null)
            attachPhaseProxy = root.GetComponent<GeAttachPhaseProxy>();

        if (attachPhaseProxy != null)
            attachPhaseProxy.ClearPhase();

        lastPhaseLv = 0;
    }

    protected string GetGlowTexPath()
    {
        string glow = attachResPath;
        glow = Path.GetDirectoryName(glow);
        int idx = glow.IndexOf("Prefab", System.StringComparison.CurrentCultureIgnoreCase);
        glow = glow.Substring(0, idx);

        string texName = Path.GetFileNameWithoutExtension(attachResPath);
        if (texName.StartsWith("P_"))
            texName = texName.Replace("P_", "T_");
        else if (texName.StartsWith("p_"))
            texName = texName.Replace("p_", "T_");
        else
            texName = "T_" + texName.Substring(2);
        texName = texName.Substring(0, texName.Length - 2) + "_glow";
        return glow + "Textures/" + texName;
    }


//    public virtual void ChangePhase(string phaseEffect, int phaseIdx) { }
//    protected virtual void _ClearPhase() { }
}
