using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoTable;

public delegate void PostLoadCommand();
public partial class GeEntity
{
    public enum GeEntityRes
    {
        Action          = 0x01,
        Material        = 0x02,
        EffectUnique    = 0x04,
        EffectMultiple  = 0x08,
        Attach          = 0x10,
        EffectGlobal    = 0x20,/// Buff特效

        All        = Action | Material | EffectUnique | EffectMultiple | Attach | EffectGlobal,
    }

    public enum GeEntityNodeType
    {
        Root,
        Actor,
        Child,
        Charactor,
        Transform,
    }

	protected string m_EntityName = "";

    
    protected DModelData m_ModelData = null;
    protected AssetInst m_ModelDataAsset = null;


    protected bool m_IsRemoved = false;
    protected static readonly byte[] DEFAULT_BLOCK_DATA = new byte[] {1};
    public bool charactorRootIsNull = false;        //charactor节点下的物件是否为空
#if !LOGIC_SERVER
    protected struct GeEntitySpaceDesc
    {
        public GameObject rootNode;
        public GameObject characterNode;
        public GameObject actorNode;
        public GameObject transformNode;
        public GameObject childNode;
		public GameObject actorModel;

        public Vector3 position;
        public Vector3 localScale;
        public Quaternion rotation;
        public float scale;

        public bool faceLeft;
        public bool m_IsEntityHide;
    }

    #region 内部类

    #endregion

   #region 成员变量

    protected GeEntitySpaceDesc m_EntitySpaceDesc;
    //protected GeSuitPartManager m_SuitPartManager = new GeSuitPartManager();
    protected GeAvatar m_Avatar = new GeAvatar(true);
    //protected GeAnimationManager m_Animation = new GeAnimationManager();
    //protected GeAttachManager m_Attachment = new GeAttachManager();
	protected GeModelManager m_ModelManager = null; // 多段变模型身管理
    protected GeAnimatManagerEx m_Material = new GeAnimatManagerEx();
    protected GeEffectManager m_Effect = new GeEffectManager();
    protected GeAfterImageMnanger m_SnapEffect = new GeAfterImageMnanger();
    protected GeSceneEx m_Scene = null;
    protected Vector4 m_EntityPlane = new Vector4(0, 1, 0, 0.03f);

    protected List<PostLoadCommand> m_PostLoadCommand = new List<PostLoadCommand>();
    protected List<PostLoadCommand> m_PostLoadCommandInBackMode = new List<PostLoadCommand>();
    protected bool m_IsAvatarDirty = false;

    protected List<GeAttach> cachedAttachs = new List<GeAttach>();

	protected bool useCube;
    protected bool isBattleActor = true;
    protected bool hasShadow = false;
    protected Vector3 shadowScale = Vector3.one;

    protected bool createdInBackMode = false;
    public bool isCreatedInBackMode { get { return createdInBackMode; } }
#if !LOGIC_SERVER
    protected GeEntityGraphicBack mGBController = null;
    protected GeEntityGraphicBack GBController
    {
        get
        {
            if (mGBController == null)
            {
                mGBController = GeEntityGraphicBack.Acquire();
            }
            return mGBController;
        }
    }

#endif

    #endregion
    public string EntityName { get { return m_EntityName; } }
    #region 原来方法
    public virtual bool Init(string entityName,GameObject entityRoot,GeSceneEx scene, bool isBattleActor = false)
    {
        if (null != entityRoot)
        {
            m_Scene = scene;
            m_EntityName = entityName;
            m_EntitySpaceDesc.m_IsEntityHide = false;
            if (null == m_EntitySpaceDesc.rootNode)
                m_EntitySpaceDesc.rootNode = new GameObject("root");
            Battle.GeUtility.AttachTo(m_EntitySpaceDesc.rootNode, entityRoot);

            if (null == m_EntitySpaceDesc.transformNode)
                m_EntitySpaceDesc.transformNode = new GameObject("Transform");

            if (null == m_EntitySpaceDesc.characterNode)
                m_EntitySpaceDesc.characterNode = new GameObject("character");

            if(null != m_EntitySpaceDesc.transformNode && null != m_EntitySpaceDesc.rootNode)
                m_EntitySpaceDesc.transformNode.transform.SetParent(m_EntitySpaceDesc.rootNode.transform);

            if (null != m_EntitySpaceDesc.characterNode && null != m_EntitySpaceDesc.transformNode)
                m_EntitySpaceDesc.characterNode.transform.SetParent(m_EntitySpaceDesc.transformNode.transform);

            if(isBattleActor)
                m_SnapEffect.Init();

            return true;
        }
        else
            Logger.LogError("[GeActor] actorRoot is nil");

        return false;
    }
    public virtual void Deinit()
    {
        if(null != m_Effect)
        {
            m_Effect.Deinit();
            m_Effect = null;
        }

        if (null != m_Material)
        {
            m_Material.Deinit();
            m_Material = null;
        }

		if (cachedAttachs != null)
		{
			ClearCached();
        }

        if (null != m_EntitySpaceDesc.actorModel)
        {
            //m_EntitySpaceDesc.actorModel.transform.SetParent(null,false);
            if (null == m_Avatar.GetAvatarRoot())
                CGameObjectPool.instance.RecycleGameObject(m_EntitySpaceDesc.actorModel);
            m_EntitySpaceDesc.actorModel = null;
        }

        if (null != m_Avatar)
        {
            m_Avatar.Deinit();
            m_Avatar = null;
        }

        if (null != m_SnapEffect)
        {
            m_SnapEffect.Deinit();
            m_SnapEffect = null;
        }

        if (null != m_EntitySpaceDesc.rootNode)
        {
            GameObject.Destroy(m_EntitySpaceDesc.rootNode);
            m_EntitySpaceDesc.rootNode = null;
        }

        if (null != m_ModelDataAsset)
        {
            m_ModelData = null;
            m_ModelDataAsset = null;
        }

        RemoveEntityUIRoot();

        m_PostLoadCommand.Clear();
        m_Scene = null;
        hasShadow = false;
        shadowScale = Vector3.one;

    }

    public void DestroySnapEffect()
    {
        if (null != m_SnapEffect)
        {
            m_SnapEffect.Deinit();
        }
    }

    public void PushPostLoadCommand(PostLoadCommand cmdCallback)
    {
        if (isCreatedInBackMode)
        {
            m_PostLoadCommandInBackMode.Add(cmdCallback);
            return;
        }
        m_PostLoadCommand.Add(cmdCallback);
    }

    public void ExcutePostLoadCommand()
    {
        for(int i = 0,num = m_PostLoadCommand.Count; i < num; ++i)
        {
            m_PostLoadCommand[i]();
        }
        m_PostLoadCommand.Clear();
    }

    public bool IsAvatarLoadFinished()
    {
        if (FrameSync.instance.IsInChasingMode && createdInBackMode)
        {
            return true;
        }
        if (null != m_Avatar)
        {
            m_Avatar.UpdateAsyncLoading();
            return m_Avatar.IsLoadFinished();
        }

        return true;
    }

    public void Pause(int Mask = (int)GeEntityRes.All, bool hitEffectPause = true)
    {
        //if (0 != (Mask & (int)GeEntityRes.Action))
        //    m_Animation.Pause();

        if (0 != (Mask & (int)GeEntityRes.EffectUnique) && hitEffectPause)
            m_Effect.Pause(GeEffectType.EffectUnique);

        if (0 != (Mask & (int)GeEntityRes.EffectMultiple) && hitEffectPause)
            m_Effect.Pause(GeEffectType.EffectMultiple);

        //if (0 != (Mask & (int)GeEntityRes.Attach))
        //    m_Attachment.Pause();

        if (0 != (Mask & (int)GeEntityRes.Material))
            m_Material.Pause();

        uint mask = 0;
        bool isActionOrAttach = false;
        if (0 != ((int)GeEntityRes.Action & Mask))
        {
            mask |= (uint)GeAvatar.EAvatarRes.Action;
            isActionOrAttach = true;
        }
        if (0 != ((int)GeEntityRes.Attach & Mask))
        {
            mask |= (uint)GeAvatar.EAvatarRes.Attach;
            isActionOrAttach = true;
        }
        if (isActionOrAttach && createdInBackMode)
        {
            var cmd = EntityPauseAniGBCommand.Acquire();
            cmd._this = this;
            cmd.mask = mask;
            cmd.hitEffectPause = hitEffectPause;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEntityBackCmdType.Pause_Animation, cmd);
            return;
        }
        m_Avatar.Pause(mask);

    }
    public void Resume(int Mask = (int)GeEntityRes.All)
    {
        //if (0 != (Mask & (int)GeEntityRes.Action))
        //    m_Animation.Resume();

        if (0 != (Mask & (int)GeEntityRes.EffectUnique))
            m_Effect.Resume(GeEffectType.EffectUnique);

        if (0 != (Mask & (int)GeEntityRes.EffectMultiple))
            m_Effect.Resume(GeEffectType.EffectMultiple);

        //if (0 != (Mask & (int)GeEntityRes.Attach))
        //    m_Attachment.Resume();

        if (0 != (Mask & (int)GeEntityRes.Material))
            m_Material.Resume();

        uint mask = 0;
        bool isActionOrAttach = false;
        uint gbMask = 0;
        if (0 != ((int)GeEntityRes.Action & Mask))
        {
            mask |= (uint)GeAvatar.EAvatarRes.Action;
            gbMask |= (uint)GeAvatar.EAvatarRes.Action;
            isActionOrAttach = true;
        }
        if (0 != ((int)GeEntityRes.Attach & Mask))
        {
            mask |= (uint)GeAvatar.EAvatarRes.Attach;
            gbMask |= (uint)GeAvatar.EAvatarRes.Attach;
            isActionOrAttach = true;
        }
        if (isActionOrAttach && createdInBackMode)
        {
            var cmd = EntityResumeAniGBCommand.Acquire();
            cmd._this = this;
            cmd.mask = gbMask;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEntityBackCmdType.Resume_Animation, cmd);
            return;
        }
        m_Avatar.Resume(mask);
    }

    public virtual  void Update(int deltaTime, int Mask = (int)GeEntityRes.All, float height = 0.0f, int accTime = 0)
    {
        if(null != m_Avatar)
        {
            if (m_Avatar.IsLoadFinished())
            {
                if (m_PostLoadCommand.Count > 0)
                {
                    ExcutePostLoadCommand();
                }

                if (m_IsAvatarDirty)
                {
                    m_Avatar.SetAvatarVisible(true);
                    m_IsAvatarDirty = false;
                }

                uint mask = 0;
                if (0 != ((int)GeEntityRes.Action & Mask))
                    mask |= (uint)GeAvatar.EAvatarRes.Action;
                if (0 != ((int)GeEntityRes.Attach & Mask))
                    mask |= (uint)GeAvatar.EAvatarRes.Attach;

                m_Avatar.Update(deltaTime, mask);
            }
            else
            {
                m_Avatar.UpdateAsyncLoading();
            }


        }

        if (null != m_ModelManager)
            m_ModelManager.Update();

        if (null != m_Effect)
        {
            m_Effect.UpdateTouchGround(height);
            int time = accTime == 0 ? deltaTime : accTime;
            if (0 != (Mask & (int)GeEntityRes.EffectUnique))
                m_Effect.Update(time, GeEffectType.EffectUnique);
            if (0 != (Mask & (int)GeEntityRes.EffectMultiple))
                m_Effect.Update(time, GeEffectType.EffectMultiple);
            if (0 != (Mask & (int)GeEntityRes.EffectGlobal))
                m_Effect.Update(time, GeEffectType.EffectGlobal);
        }

        // if(null != m_Attachment)
        // {
        //     if (0 != (Mask & (int)GeEntityRes.Attach))
        //         m_Attachment.Update();
        // }

        if (null != m_Material)
        {
            if (0 != (Mask & (int)GeEntityRes.Material))
                m_Material.Update(deltaTime, m_EntitySpaceDesc.actorNode);
        }

        if (null != m_SnapEffect)
            m_SnapEffect.Update(deltaTime, m_EntitySpaceDesc.actorNode);
    }

    public void AddSimpleShadow(Vector3 scale,string actorName="")
    {
        if (createdInBackMode)
        {
            var cmd = EntityAddShadowGBCommand.Acquire();
            cmd._this = this;
            cmd.scale = scale;
            GBController.RecordCmd((int)GeEntityBackCmdType.Add_Shadow, cmd);
            return;
        }
        shadowScale = scale;
        hasShadow = true;

        PushPostLoadCommand(() => { _AddShadow(actorName);});
    }

    public void UpdateAttach()
    {
        // TODO:: 拷贝自void Update(int deltaTime, int Mask = (int)GeEntityRes.All, float height=0.0f) 除去特效的更新逻辑
        if (null != m_Avatar)
        {
            if (m_Avatar.IsLoadFinished())
            {
                if (m_PostLoadCommand.Count > 0)
                {
                    ExcutePostLoadCommand();
                }

                if (m_IsAvatarDirty)
                {
                    m_Avatar.SetAvatarVisible(true);
                    m_IsAvatarDirty = false;
                }
            }
            else
            {
                m_Avatar.UpdateAsyncLoading();
            }
        }

        //if (null != m_Attachment)
        //    m_Attachment.Update();

        if (null != m_SnapEffect)
            m_SnapEffect.Update(0, m_EntitySpaceDesc.actorNode);

        m_Avatar.Update(0, (uint)GeAvatar.EAvatarRes.Attach);
    }

    public void Clear(int Mask = (int)GeEntityRes.All)
    {
        if (0 != (Mask & (int)GeEntityRes.EffectUnique))
            m_Effect.ClearAll(GeEffectType.EffectUnique);

        if (0 != (Mask & (int)GeEntityRes.EffectMultiple))
            m_Effect.ClearAll(GeEffectType.EffectMultiple);

        if (0 != (Mask & (int)GeEntityRes.EffectGlobal))
            m_Effect.ClearAll(GeEffectType.EffectGlobal);

        // if (0 != (Mask & (int)GeEntityRes.Attach))
        //     m_Attachment.ClearAll();

        if (0 != (Mask & (int)GeEntityRes.Material))
            m_Material.ClearAll();

        uint mask = 0;
        if (0 != ((int)GeEntityRes.Action & Mask))
            mask |= (uint)GeAvatar.EAvatarRes.Action;
        if (0 != ((int)GeEntityRes.Attach & Mask))
            mask |= (uint)GeAvatar.EAvatarRes.Attach;

        m_Avatar.Clear(mask);
    }

    public void Remove()
    {
        m_IsRemoved = true;
        if (isCreatedInBackMode)
        {
            GBController.RecordCmd((int)GeEntityBackCmdType.Destroy, null);
        }
    }

    public bool CanRemove()
    {
        return m_IsRemoved;
    }
    public virtual void DoBackToFront()
    {
        //先创建主体
        GBController.FlipToFront();
        //再创建主体相关，如动作，挂件
        m_Avatar.DoBackToFront();
        //创建主体挂件动画
        GBController.FlipAttachToFront();
        m_Material.DoBackToFront();
        m_Effect.DoBackToFront();
        if (m_PostLoadCommandInBackMode.Count > 0)
        {
            m_PostLoadCommand.AddRange(m_PostLoadCommandInBackMode);
            m_PostLoadCommandInBackMode.Clear();
        }
    }

    public void SetPosition(Vector3 pos)
    {   
        m_EntitySpaceDesc.position = pos;

        if(m_EntitySpaceDesc.rootNode != null)
        {
            m_EntitySpaceDesc.rootNode.transform.localPosition = pos;
        }

        SetActorUIRootPos(pos);
    }

    public void SetScale(float scale)
    {
        m_EntitySpaceDesc.scale = scale;
        SetScaleV3(scale * Vector3.one);
    }

    public void SetActorPosition(Vector3 pos)
    {
        if (m_EntitySpaceDesc.actorNode == null)
            return;
        m_EntitySpaceDesc.actorNode.transform.localPosition = pos;
    }

    public Vector3 GetActorNodeScale()
    {
        if (m_EntitySpaceDesc.actorNode == null)
            return Vector3.one;
        return  m_EntitySpaceDesc.actorNode.transform.localScale;
    }

    public void SetActorNodeScale(float scale)
    {
        m_EntitySpaceDesc.actorNode.transform.localScale = scale * Vector3.one;
    }

    public void SetActorNodeScale1(Vector3 scale)
    {
        if (m_EntitySpaceDesc.actorNode == null)
            return;
        m_EntitySpaceDesc.actorNode.transform.localScale = scale;
    }

    public void ResetActorNodeScale()
    {
        m_EntitySpaceDesc.actorNode.transform.localScale = Vector3.one;
    }

    public void SetScaleV3(Vector3 scale)
    {
        m_EntitySpaceDesc.localScale = scale;

		if (m_EntitySpaceDesc.rootNode != null)
        {
            //m_EntitySpaceDesc.rootNode.transform.localScale = m_EntitySpaceDesc.localScale;
			m_EntitySpaceDesc.transformNode.transform.localScale = m_EntitySpaceDesc.localScale;
        }
    }

    public void SetRotation(Quaternion rotation)
    {
        m_EntitySpaceDesc.rotation = rotation;

		if (m_EntitySpaceDesc.rootNode != null)
        {
            //m_EntitySpaceDesc.rootNode.transform.localRotation = m_EntitySpaceDesc.rotation;
			m_EntitySpaceDesc.transformNode.transform.localRotation = m_EntitySpaceDesc.rotation;
        }
    }

    public Quaternion GetRotation()
    {
        return m_EntitySpaceDesc.rotation;
    }

    public float GetScale()
    {
        return m_EntitySpaceDesc.scale;
    }

    public Vector3 GetPosition()
    {
       return m_EntitySpaceDesc.position;
    }

    public bool IsFaceLeft()
    {
        return m_EntitySpaceDesc.faceLeft;
    }

    public void SetFaceLeft(bool bFaceLeft)
    {
        m_EntitySpaceDesc.faceLeft = bFaceLeft;

        if (m_EntitySpaceDesc.characterNode)
        {
            Vector3 localScale = m_EntitySpaceDesc.characterNode.transform.localScale;
            if (m_EntitySpaceDesc.faceLeft)
                localScale.x = -Mathf.Abs(localScale.x);
            else
                localScale.x = Mathf.Abs(localScale.x);

            m_EntitySpaceDesc.characterNode.transform.localScale = localScale;
        }
    }

	public GeEffectEx FindEffectById(int effectInfoId)
    {
        var data = TableManager.GetInstance().GetTableItem<EffectInfoTable>(effectInfoId);
        if (data != null)
        {
            m_Effect.GetEffectByName(data.Path);
        }
        return null;
    }

	public GeEffectEx FindEffect(string effectPath)
	{
		return m_Effect.GetEffectByName(effectPath);
	}
	
    public GeEffectEx CreateEffect(int effectInfoId, Vec3 initPos, bool ignoreFacing = false, float fTime = 0, EffectTimeType timeType = EffectTimeType.SYNC_ANIMATION)
    {
        var data = TableManager.GetInstance().GetTableItem<EffectInfoTable>(effectInfoId);
        if (data != null)
        {
            EffectsFrames effectInfo = EffectsFrames.Default;
            effectInfo.localPosition = new Vector3(0, 0, 0);
            effectInfo.attachname = data.Locator;
            effectInfo.timetype = timeType;

            DAssetObject asset;

            asset.m_AssetObj = null;
            asset.m_AssetPath = data.Path;

            var time = data.Duration / 1000f;
            if (fTime > 0)
                time = fTime;
            var scale = data.Scale / 1000f;
            if (data.Scale == 0)
                scale = 1f;
            var speed = 1f;

            var effect = CreateEffect(asset, effectInfo, time, initPos, scale, speed, data.Loop, ignoreFacing);
            return effect;
        }
        return null;
    }

    public GeEffectEx CreateEffect(string effectPath, string locator, float fTime, Vec3 initPos, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false, bool bIgnoreFacing = false, EffectTimeType timeType = EffectTimeType.SYNC_ANIMATION)
    {
        EffectsFrames effectInfo = new EffectsFrames();
        effectInfo.localPosition = new Vector3(0, 0, 0);
        effectInfo.attachname = locator;
        effectInfo.timetype = timeType;

        DAssetObject asset;

        asset.m_AssetObj = null;
        asset.m_AssetPath = effectPath;

        var effect = CreateEffect(asset, effectInfo, fTime, initPos, initScale, fSpeed, isLoop, bIgnoreFacing);
        return effect;
    }


    public GeEffectEx CreateEffect(DAssetObject effectRes, EffectsFrames info, float fTime, Vec3 initPos, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false, bool bIgnoreFacing = false, bool forceDisplay = false)
    {
        string attachNode = info.attachname;
        bool touchGround = false;
        if (attachNode != null && attachNode == "[actor]OrignTouchGround")
        {
            touchGround = true;
            attachNode = "[actor]Orign";
        }

        if (null == attachNode || "" == attachNode)
            attachNode = "None";

        //GeAttachManager.GeAttachNodeDesc attachNodeDesc = m_Attachment.GetAttchNodeDesc(attachNode);
        GameObject goAttachNode = m_Avatar.GetAttachNode(attachNode);
        if (null != effectRes.m_AssetObj || (null != effectRes.m_AssetPath && "" != effectRes.m_AssetPath))
        {
            Vector3 graphicPos = new Vector3(initPos.x, initPos.z, initPos.y);
            GeEffectEx newEffect = FrameSync.instance.IsInChasingMode ?
                  m_Effect.AddEffectInBackMode(effectRes, info, fTime, graphicPos, this, attachNode, bIgnoreFacing ? false : m_EntitySpaceDesc.faceLeft, forceDisplay) 
                  : m_Effect.AddEffect(effectRes, info, fTime, graphicPos, goAttachNode, bIgnoreFacing ? false : m_EntitySpaceDesc.faceLeft, forceDisplay);
            if (null != newEffect)
            {
                bool bLoop = false;
                if (info.timetype == EffectTimeType.GLOBAL_ANIMATION)
                    bLoop = true;
                else
                {
                    if (newEffect.GetDefaultTimeLen() < fTime)
                        bLoop = true;
                }

                newEffect.SetSpeed(fSpeed);
                newEffect.Play(bLoop || isLoop);

                newEffect.SetTouchGround(touchGround);

                return newEffect;
            }
            else
                Logger.LogWarning("Create effect has failed!");
        }
        else
            Logger.LogWarning("Effect resource path can not be null!");

        return null;
    }

    public string GetAttachNodeName(string attachNode, out bool isTouchGround)
    {
        isTouchGround = false;

        if (attachNode != null && attachNode == "[actor]OrignTouchGround")
        {
            isTouchGround = true;
            attachNode = "[actor]Orign";
        }

        if (null == attachNode || "" == attachNode)
            attachNode = "None";

        return attachNode;
    }
    public void DestroyEffect(GeEffectEx effect)
    {
        GeEffectType effType = GeEffectType.EffectMultiple;
        if (EffectTimeType.GLOBAL_ANIMATION == effect.GetTimeType())
            effType = GeEffectType.EffectUnique;
        else if (EffectTimeType.BUFF == effect.GetTimeType())
            effType = GeEffectType.EffectGlobal;
        m_Effect.RemoveEffect(effect, effType);
    }

    /// <summary>
    /// 移除根据特效路径移除特效
    /// </summary>
    public void DestroyEffectByName(string path)
    {
        if (m_Effect == null) return;
        var effect = m_Effect.GetEffectByName(path);
        if (effect == null) return;
        DestroyEffect(effect);
    }

    public GeAttach GetAttachment(string name, string nodeName = null)
	{
		return m_Avatar.GetAttachment(name, nodeName);
	}

	public GameObject GetAttachNode(string name)
	{
        // var node = m_Attachment.GetAttchNodeDesc(name);
        // return node.attachNode;

        if (null != m_Avatar)
            return m_Avatar.GetAttachNode(name);

        return null;
	}

	public void SetAttachmentVisible(string name, bool flag)
	{
		var attach = GetAttachment(name);
		if (attach != null)
			attach.SetVisible(flag);
	}

	public void PlayAttachmentAnimation(string name, string aniName,float fspeed)
	{
		var attach = GetAttachment(name);
		if (attach != null)
		{
            if (createdInBackMode)
            {
                var cmd = EntityAttachmentPlayAniGBCommond.Acquire();
                cmd._this = this;
                cmd.fSpeed = fspeed;
                cmd.aniName = aniName;
                cmd.attachName = name;
                var controller = GBController;
                if (controller != null)
                    controller.RecordAttachmentCmd(name, cmd);
            }
            else
            {
                float speed = m_Avatar.GetCurActionSpeed() * fspeed;
                attach.PlayAction(aniName, speed, m_Avatar.IsActionLoop(aniName), GetCurActionOffset());
            }
        }
	}
    public string GetAttachmentCurActionName(string name, string nodeName)
    {
        var attach = GetAttachment(name, nodeName);
        if (attach != null)
        {
            if (createdInBackMode)
            {
                var cmd = GBController.GetAttachmentCmd(name) as EntityAttachmentPlayAniGBCommond;
                return cmd != null ? cmd.aniName : string.Empty;
            }
            return attach.GetCurActionName();
        }
        return string.Empty;
    }

    public GeAttach GetAttachFromCached(string attachRes, string attachNode)
	{
		for(int i=0; i<cachedAttachs.Count; ++i)
		{
			var attach = cachedAttachs[i];
			if (attach != null && attach.ResPath.Equals(attachRes) && attach.AttachNodeName.Equals(attachNode))
			{
				attach.Root.SetActive(true);
				cachedAttachs.Remove(attach);

				//Logger.LogErrorFormat("GetAttachFromCached attachRes:{0} attachNode:{1}", attachRes, attachNode);

				return attach;
			}
		}

		return null;
	}

	public void PutAttachToCached(GeAttach attach)
	{
		if (attach != null && attach.cached && attach.Root != null && !cachedAttachs.Contains(attach))
		{
			attach.Reset();
			attach.Root.SetActive(false);
			cachedAttachs.Add(attach);

			//Logger.LogErrorFormat("PutAttachToCached attachRes:{0} attachNode:{1}", attach.ResPath, attach.AttachNodeName);
		}
	}

	public void ClearCached()
	{
		if (cachedAttachs == null)
			return;

		for(int i=0; i<cachedAttachs.Count; ++i)
		{
            // if (m_Attachment != null)
            // 	m_Attachment.RemoveAttachment(cachedAttachs[i]);

            if (null != m_Avatar)
                m_Avatar.DestroyAttachment(cachedAttachs[i]);
        }

		cachedAttachs.Clear();
	}

	public GeAttach CreateAttachment(string attachmentName, string attachRes, string attachNode, bool cached=false,bool asyncLoad = false, bool highPriority =false)
    {
		if (useCube)
			return null;
		
		GeAttach attachment = null;
		if (cached)
		{
			attachment = GetAttachFromCached(attachRes, attachNode);
			if (attachment != null)
			{
				return attachment;
			}
		}

        attachment = m_Avatar.CreateAttachment(attachmentName, attachRes, attachNode, cached, asyncLoad, highPriority);

        // attachment = m_Attachment.AddAttachment( attachmentName, attachRes, attachNode,true, asyncLoad);
        // 
        // if (attachment == null)
        // 	return null;
        // 
        // GameObject[] attachObjs = new GameObject[1];
        // attachObjs[0] = attachment.GetAttachModel();
        // //m_Material.AppendObject(attachObjs);
        // 
        // //m_Attachment.RefreshAttachNode(m_SuitPartManager.avatarObject);
        // m_Attachment.RefreshAttachNode(m_EntitySpaceDesc.actorNode);

        if (cached)
			attachment.cached = true;

        return attachment;
    }

    public void DestroyAttachment(GeAttach att)
    {
		if (att != null && att.cached && !att.isCreatedInBackMode)
		{
			PutAttachToCached(att);
			return;
		}
			
        //GameObject[] attachObjs = new GameObject[1];
        //attachObjs[0] = att.GetAttachModel();
        //m_Material.RemoveObject(attachObjs);
        m_Avatar.DestroyAttachment(att);
    }

    public uint ChangeSurface(string animate,float timeLen,bool enableAnim = true,bool needRecover = true)
    {
		if (useCube)
			return int.MaxValue;
        if (createdInBackMode)
        {
            return m_Material.PushAnimatInBackMode(animate, timeLen, enableAnim, needRecover);
        }
        return m_Material.PushAnimat(animate, timeLen,enableAnim, needRecover);
    }

    public void RemoveSurface(uint handle)
    {
		if (useCube)
			return;
		
        if (uint.MaxValue == handle)
            m_Material.ClearAll();
        else
        {
            if (createdInBackMode)
            {
                m_Material.RemoveAnimatInBackMode(handle);
            }
            else
            {
                m_Material.RemoveAnimat(handle);
            }
        }
    }

    protected GameObject[] snapShotCache = null;
    public void CreateSnapshot(Color32 color, float TimeLen, string materialPath = "")
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableSnap)
			return;
#endif
        if (createdInBackMode)
        {
            var cmd = EntityCreateSnapShotGBCommand.Acquire();
            cmd._this = this;
            cmd.color = color;
            cmd.TimeLen = TimeLen;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEntityBackCmdType.Create_SnapShot, cmd);
            return;
        }

        if (m_Avatar.suitPartModel.Length <= 0)
        {
            if(null == snapShotCache)
            {
                GameObject avatarRoot = m_Avatar.GetAvatarRoot();
                if(null != avatarRoot)
                {
                    SkinnedMeshRenderer skinRender = avatarRoot.GetComponentInChildren<SkinnedMeshRenderer>();
                    if(null != skinRender)
                        snapShotCache = new GameObject[1] { skinRender.gameObject};
                }
            }
        }
        GameObject[] snapObj = null;
        if (null != snapShotCache)
            snapObj = snapShotCache;
        else
            snapObj = m_Avatar.suitPartModel;
        m_SnapEffect.CreateSnapshot(snapObj, color, (int)(TimeLen * 1000), materialPath);
    }

    public bool AddAnimPackage(string animPackage)
    {   //
        //if(null != m_Animation)
        //{
        //    if(null != m_Avatar)
        //    {
        //        string packageName = m_Avatar.GetResPath() + "_" + animPackage;
        //        return m_Animation.AddAnimationPack(packageName);
        //    }
        //}

        return false;
    }

    public void ProloadAction(string[] animList)
    {
        m_Avatar.PreloadAction(animList);
    }

    public void ProloadAction(string anim)
    {
        if (createdInBackMode)
        {
            var cmd = EntityPreloadAniGBCommand.Acquire();
            cmd.aniName = anim;
            cmd._this = this;
            GBController.RecordCmd((int)GeEntityBackCmdType.Preload_Animation, cmd);
            return;
        }
        m_Avatar.PreloadAction(anim);
    }

	public bool ChangeAction(string action,float speed,bool loop = false, bool replace=true, bool force=false)
    {
        if (createdInBackMode)
        {
            var actionCmd = GBController.Get((int)GeEntityBackCmdType.Change_Action) as EntityChangeActionGBCommand;
            var stopActionCmd = GBController.Get((int)GeEntityBackCmdType.Stop_Action) as EntityStopAniGBCommand;
            if (stopActionCmd != null && actionCmd != null)
            {
                if (!force && replace && loop && (string.Empty == action) && Mathf.Abs(speed - actionCmd.fSpeed) < 0.01f)
                {
                    return true;
                }
            }
            else if (actionCmd != null)
            {
                if (!force && replace && loop && (actionCmd.strAction == action) && Mathf.Abs(speed - actionCmd.fSpeed) < 0.01f)
                {
                    return true;
                }
            }
            var cmd = EntityChangeActionGBCommand.Acquire();
            cmd.strAction = action;
            cmd.fSpeed = speed;
            cmd.bLoop = loop;
            cmd.bReplace = replace;
            cmd.bForce = force;
            cmd.timeStamp = FrameSync.GetTicksNow();
            cmd._this = this;
            GBController.RecordCmd((int)GeEntityBackCmdType.Change_Action, cmd);
            return true;
        }

        if (!force && replace && loop && (m_Avatar.GetCurActionName() == action) && Mathf.Abs(speed-m_Avatar.GetActionSpeed())<0.01f)
        {
            return true;
        }
        if (force || replace || m_Avatar.GetCurActionName() != action)
            return m_Avatar.ChangeAction(action, speed, loop );

        return false;
    }

    public void StopAction()
    {
        if (createdInBackMode)
        {
            var cmd = EntityStopAniGBCommand.Acquire();
            cmd._this = this;
            cmd.timeStamp = FrameSync.GetTicksNow();
            GBController.RecordCmd((int)GeEntityBackCmdType.Stop_Action, cmd);
            return;
        }
        m_Avatar.StopAction();
    }


	public string GetCurActionName()
	{
        if (createdInBackMode)
        {
            var cmd = GBController.Get((int)GeEntityBackCmdType.Change_Action) as EntityChangeActionGBCommand;
            var stopCmd = GBController.Get((int)GeEntityBackCmdType.Stop_Action) as EntityStopAniGBCommand;
            if (stopCmd != null) return string.Empty;
            return cmd != null ? cmd.strAction : string.Empty;
        }
        if (null != m_Avatar)
			return m_Avatar.GetCurActionName();
		return "";
	}

    public float GetActionTimeLen(string action)
    {
        if (createdInBackMode)
        {
            return 0.0f;
        }
        return m_Avatar.GetActionTimeLen(action);
    }

	public bool IsActionLoop(string action)
	{
        if (createdInBackMode)
        {
            return false;
        }
        return m_Avatar.IsActionLoop(action);
	}

	public float GetCurActionSpeed()
	{
        if (createdInBackMode)
        {
            var cmd = GBController.Get((int)GeEntityBackCmdType.Change_Action) as EntityChangeActionGBCommand;
            return cmd != null ? cmd.fSpeed : 0.0f;
        }
        return m_Avatar.GetCurActionSpeed();
    }
    public float GetCurActionOffset()
    {
        if (createdInBackMode)
        {
            var cmd = GBController.Get((int)GeEntityBackCmdType.Change_Action) as EntityChangeActionGBCommand;
            return cmd != null ? cmd.offset : 0.0f;
        }
        return m_Avatar.GetCurActionOffset();
    }

    public bool GetCurActionLoop()
    {
        return m_Avatar.GetCurActionLoop();
    }

    public GameObject GetEntityNode(GeEntityNodeType nodeType)
    {
        switch(nodeType)
        {
            case GeEntityNodeType.Root:return m_EntitySpaceDesc.rootNode;
            case GeEntityNodeType.Actor:return m_EntitySpaceDesc.actorNode;
            case GeEntityNodeType.Charactor:return m_EntitySpaceDesc.characterNode;
            case GeEntityNodeType.Child: return m_EntitySpaceDesc.childNode;
            case GeEntityNodeType.Transform:return m_EntitySpaceDesc.transformNode;
            default:return null;
        }
    }

    public void SetEntityPlane(Vector4 plane)
    {
        m_EntityPlane = plane;
    }

	public void ChangeAvatar(GeAvatarChannel channel, string modulePath,bool asyncLoad,bool highPriority,int prodid)
	{
        if (isCreatedInBackMode)
        {
            var cmd = EntityChangeAvatarGBCommand.Acquire();
            cmd._this = this;
            var changeInfo = EntityChangeAvatarGBCommand.AllocateChangeAvatarInfo();
            changeInfo.channel = channel;
            changeInfo.modulePath = modulePath;
            changeInfo.asyncLoad = asyncLoad;
            changeInfo.highPriority = highPriority;
            changeInfo.prodId = prodid;
            cmd.changeAvatarGBInfo.Add((int)changeInfo.channel, changeInfo);
            GBController.RecordCmd((int)GeEntityBackCmdType.Change_Avatar, cmd);
            return;
        }
        DAssetObject asset = new DAssetObject(null, modulePath);
        m_IsAvatarDirty = true;
        
        if (isBattleActor)
            m_Material.RemoveObject(m_Avatar.suitPartModel);
        m_Avatar.ChangeAvatarObject(channel, asset, asyncLoad, highPriority, prodid);
        PushPostLoadCommand(() =>
        {
            if (isBattleActor)
                m_Material.AppendObject(m_Avatar.suitPartModel);
        });
    }

    public void SuitAvatar(bool isAsyncLoad = true,bool highPriority = false,int prodid = 0)
    {
        if (isCreatedInBackMode)
        {
            var cmd = EntitySuitAvatarGBCommand.Acquire();
            cmd._this = this;
            cmd.isAsyncLoad = isAsyncLoad;
            cmd.highPriority = highPriority;
            GBController.RecordCmd((int)GeEntityBackCmdType.Suit_Avatar, cmd);
            return;
        }
        if (isBattleActor)
            m_Material.RemoveObject(m_Avatar.suitPartModel);
        m_Avatar.SuitAvatar(isAsyncLoad,highPriority, prodid);
        PushPostLoadCommand(() =>
        {
            m_Avatar.SetAvatarVisible(true);
            if (isBattleActor)
                m_Material.AppendObject(m_Avatar.suitPartModel);
        });
    }

    GameObject[] simpleObj = new GameObject[1];
    protected void _AddShadow(string actorName="")
    {
        if (!hasShadow)
            return;

        _RemoveShadow();
        int level = 0;
        GeGraphicSetting.instance.GetSetting("GraphicLevel", ref level);

        simpleObj[0] = m_Avatar.GetAvatarRoot();

        Transform parent = GetShadowParentNodeAndAttach();

        int prefixIndex = actorName.LastIndexOf('_');
        actorName = actorName.Remove(0, prefixIndex + 1);
        //Clips clips= FramesLookTable.Instance._GetActorInfo(actorName);

        //#if UNITY_ANDROID
        //if (level > 1 || clips == null)
        //{
            if (level < 2)
            GeSimpleShadowManager.instance.AddShadowObject(simpleObj, m_EntityPlane, shadowScale, parent);
        //}
        //else
        //    {
        //        GeRealShadowManager.instance.AddShadowObject(simpleObj, m_EntityPlane, shadowScale, clips, parent);
        //    }
        //#else
        //        if (1 == level)
        //            GeSimpleShadowManager.instance.AddShadowObject(simpleObj, m_EntityPlane, Vector3.one);
        //        else if (0 == level)
        //            GePlaneShadowManager.instance.AddShadowObject(m_Avatar.suitPartModel, m_EntityPlane);
        //#endif
    }

    protected void _RemoveShadow()
    {
        simpleObj[0] = m_Avatar.GetAvatarRoot();
        GeSimpleShadowManager.instance.RemoveShadowObject(simpleObj);
//        GePlaneShadowManager.instance.RemoveShadowObject(m_Avatar.suitPartModel);
    }

    //获取影子预制体
    public GameObject GetShadowObj()
    {
        return GeSimpleShadowManager.instance.GetShadowObj(m_Avatar.GetAvatarRoot());
    }

	public void SetUseCube(bool flag)
	{
		useCube = flag;
		if (m_Effect != null)
			m_Effect.useCube = flag;
        //if (m_Attachment != null)
        //	m_Attachment.useCube = flag;

        m_Avatar.airMode = flag;

	}

	public bool GetUseCube()
	{
		bool flag = false;
		if (m_Effect != null)
			flag = m_Effect.useCube;

		return useCube || flag;
	}

	public GeEffectManager GetEffectManager()
	{
		return m_Effect;
	}

    public int GetActorNodeLayer()
    {
        if (m_EntitySpaceDesc.actorNode == null)
            return 0;
        return m_EntitySpaceDesc.actorNode.layer;
    }

    public void SetActorNodeLayer(int layer)
    {
        if (m_EntitySpaceDesc.actorNode == null)
            return;
        m_EntitySpaceDesc.actorNode.SetLayer(layer);
    }


    #endregion

#else
     public bool IsAvatarLoadFinished(){return true;}
    public void AddSimpleShadow(Vector3 scale, string actorName = ""){}
    public virtual bool Init(string entityName,GameObject entityRoot,GeSceneEx scene){return false;}
	public virtual void Deinit(){}
	public void PushPostLoadCommand(PostLoadCommand cmdCallback){}
	public void ExcutePostLoadCommand(){}
	public void Pause(int Mask = (int)GeEntityRes.All){}
	public void Resume(int Mask = (int)GeEntityRes.All){}
	public virtual void Update(int deltaTime, int Mask = (int)GeEntityRes.All, float height = 0.0f, int accTime = 0){}
	public void UpdateAttach(){}
	public void Clear(int Mask = (int)GeEntityRes.All){}
	public void Remove(){}
	public bool CanRemove(){return true;}
	public void SetPosition(Vector3 pos){}//
	public void SetScale(float scale){}
	public void SetScaleV3(Vector3 scale){}//
	public void SetRotation(Quaternion rotation){}//
	public Quaternion GetRotation(){return Quaternion.identity;}
	public float GetScale(){return 0.0f;}
	public Vector3 GetPosition(){return new Vector3();}//
	public bool IsFaceLeft(){return false;}
	public void SetFaceLeft(bool bFaceLeft){}
	public GeEffectEx FindEffect(string effectPath){return null;}//
	public GeEffectEx CreateEffect(string effectPath, string locator, float fTime, Vec3 initPos, float initScale = 1.0f, float fSpeed = 1.0f, 
		bool isLoop = false, bool faceLeft = false, EffectTimeType timeType=EffectTimeType.SYNC_ANIMATION){return null;}//
	public GeEffectEx CreateEffect(DAssetObject effectRes, EffectsFrames info,float fTime, Vec3 initPos, float initScale = 1.0f,  float fSpeed 
		= 1.0f, bool isLoop = false,bool bIgnoreFacing = false){return null;}
    /*public GameObject GetAttachNode(string name){}*/
    public GeEffectEx CreateEffect(int effectInfoId, Vec3 initPos, bool ignoreFacing = false, float fTime = 0,
        EffectTimeType timeType = EffectTimeType.SYNC_ANIMATION)
    {
        return null;
    }
    public string GetAttachNodeName(string attachNode, out bool isTouchGround){isTouchGround=false; return "";}
	public void DestroyEffect(GeEffectEx effect){}
	public GeAttach GetAttachment(string name){return null;}
	public GameObject GetAttachNode(string name){return null;}
	public void SetAttachmentVisible(string name, bool flag){}
	public void PlayAttachmentAnimation(string name, string aniName){}
    public string GetAttachmentCurActionName(string name){return string.Empty;}
	public GeAttach GetAttachFromCached(string attachRes, string attachNode){return null;}
	public void PutAttachToCached(GeAttach attach){}
	public void ClearCached(){}
	public GeAttach CreateAttachment(string attachmentName, string attachRes, string attachNode, bool cached=false,bool asyncLoad = false){return null;}
	public void DestroyAttachment(GeAttach att){}
	public uint ChangeSurface(string animate,float timeLen,bool enableAnim = true,bool needRecover = true){return 0;}
	public void RemoveSurface(uint handle){}
	public void CreateSnapshot(Color32 color, float TimeLen){}
	public bool AddAnimPackage(string animPackage){return false;}
	public void ProloadAction(string[] animList){}
	public void ProloadAction(string anim){}
	public bool ChangeAction(string action,float speed,bool loop = false, bool replace=true, bool force=false){return false;}
	public void StopAction(){}
    public string GetCurActionName(){return "";}
	public float GetActionTimeLen(string action){return 0f;}
	public GameObject GetEntityNode(GeEntityNodeType nodeType){return null;}
	public void SetEntityPlane(Vector4 plane){}
	public void ChangeAvatar(GeAvatarChannel channel, string modulePath,bool isAsyncLoad,bool highpriority,int prodid){}
	public void SetUseCube(bool flag){}
	public bool GetUseCube(){return false;}
	public GeEffectManager GetEffectManager(){return null;}

    public bool IsActionLoop(string action){ return false;}

    public GeEffectEx FindEffectById(int effectInfoId)
    {
        return null;
    }

    public float GetCurActionSpeed() { return 1.0f; }
    public float GetCurActionOffset() { return 0.0f; }
    public void SuitAvatar(bool isAsyncLoad = true, bool highPriority = false,int prodid = 0) { }
    public void AddShadow(){}
    public void RemoveShadow(){}
    public void ChangeAvatar(GeAvatarChannel channel, string modulePath, bool asyncLoad, bool highPriority) { }
    public GeAttach CreateAttachment(string attachmentName, string attachRes, string attachNode, bool cached = false, bool asyncLoad = false, bool highPriority = false) { return null; }

    //public void SuitAvatar(bool isAsyncLoad = true){}
    //public void AddShadow(){}
    //public void RemoveShadow(){}

#endif

    public byte[] GetBlockData(out int width,out int height)
	{
		if(null != m_ModelData && null != m_ModelData.blockGridChunk.gridBlockData)
		{
			width = m_ModelData.blockGridChunk.gridWidth;
			height = m_ModelData.blockGridChunk.gridHeight;
			return m_ModelData.blockGridChunk.gridBlockData;
		}
		else
		{
			//Logger.LogWarningFormat("Missing block data {0}", m_EntityName);
			width = 1;
			height = 1;
			return DEFAULT_BLOCK_DATA;
		}
	}

}
