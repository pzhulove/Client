using UnityEngine;
using System.Collections;
using System.IO;



public class GeDemoActor : IGeAvatarActor
{
    GeAvatar m_Avatar = null;
    GeAnimationManager m_Animation = null;
    GeAttachManager m_Attachment = null;
    GeEffectManager m_Effect = null;

    protected AssetInst m_ModelDataAsset = null;
    protected DModelData m_ModelData = null;
    protected GeActorState m_ActorState = null;

    protected string m_AvatarPath;
    protected string m_AvatarName;

    GameObject m_ActorRoot = null;

    static readonly GeAvatarChannel[] avatarChanTbl = new GeAvatarChannel[]
    {
        GeAvatarChannel.Head,       /// eModelHead
        GeAvatarChannel.UpperPart,  /// eModelUpperPart
        GeAvatarChannel.LowerPart,  /// eModelLowerPart
        GeAvatarChannel.Bracelet,   /// eModelShoulder
        GeAvatarChannel.Headwear,   /// eModelShoulder
        GeAvatarChannel.Wings,      /// eModelWings
    };

    protected bool m_avatarDirty = false;

    protected void _Init()
    {
        if (null == m_ActorRoot)
            m_ActorRoot = new GameObject("DemoAvatar");

        if (null == m_Avatar)
            m_Avatar = new GeAvatar();
        if (null == m_Animation)
            m_Animation = new GeAnimationManager();
        if (null == m_Attachment)
            m_Attachment = new GeAttachManager();
        if (null == m_Effect)
            m_Effect = new GeEffectManager();
        if (null == m_ActorState)
            m_ActorState = new GeActorState();
    }

    public void Deinit()
    {
        if (null != m_Animation)
        {
            m_Animation.Deinit();
            m_Animation = null;
        }

        if (null != m_Attachment)
        {
            m_Attachment.Deinit();
            m_Attachment = null;
        }

        if (null != m_Effect)
        {
            m_Effect.Deinit();
            m_Effect = null;
        }

        if (null != m_Avatar)
        {
            GameObject avatarNode = m_Avatar.GetAvatarRoot();
            if (avatarNode)
                avatarNode.transform.SetParent(null, true);
            m_Avatar.Deinit();
            m_Avatar = null;
        }

        if (null != m_ModelDataAsset)
        {
            m_ModelDataAsset = null;
            m_ModelData = null;
        }

        if (null != m_ActorState)
        {
            m_ActorState.Deinit();
            m_ActorState = null;
        }

        if (null != m_ActorRoot)
        {
            m_ActorRoot.transform.SetParent(null, true);

            GameObject.Destroy(m_ActorRoot);
            m_ActorRoot = null;
        }
    }

    static public string GetAvatarResPath(string resPath)
    {
        if (!string.IsNullOrEmpty(resPath))
        {
            string actorPath = "Actor/";
            if (resPath.StartsWith(actorPath))
            {
                int last = resPath.IndexOf("Prefabs");
                if (0 <= last && last < resPath.Length)
                {
                    string avatarPath = resPath.Substring(0, last - 1);
                    string[] splitTbl = avatarPath.Split('/');
                    string avatarName = splitTbl[splitTbl.Length - 1];
                    return avatarPath + "/" + avatarName;
                }
            }
        }

        return null;
    }

    public void LoadAvatar(string avatarRes,bool isAsync = false)
    {
#if !LOGIC_SERVER
        Deinit();
        _Init();
        if (!string.IsNullOrEmpty(avatarRes))
        {
            string actorPath = "Actor/";
            m_AvatarPath = "";
            if (avatarRes.StartsWith(actorPath))
            {
                int last = avatarRes.IndexOf("Prefabs");
                if (0 <= last && last < avatarRes.Length)
                {
                    m_AvatarPath = avatarRes.Substring(0, last - 1);

                    string[] splitTbl = m_AvatarPath.Split('/');
                    m_AvatarName = splitTbl[splitTbl.Length - 1];

                    if (m_AvatarName.Contains("Hero_") || m_AvatarName.Contains("Monster_"))
                    {
                        m_ModelDataAsset = AssetLoader.instance.LoadRes(GetModelDataPath(), typeof(ScriptableObject), false);
                        if (null != m_ModelDataAsset)
                        {
                            m_ModelData = m_ModelDataAsset.obj as DModelData;
                            if (m_ModelData)
                            {
                                //string avatarValidPath = m_AvatarPath.Substring(0, m_AvatarPath.Length - m_AvatarName.Length - 1);
                                //if (m_Avatar.Init(avatarValidPath + "/Animations/Avatar",9))
                                if (m_Avatar.Init(m_ModelData.modelAvatar.m_AssetPath, 9))
                                {
                                    m_Avatar.GetAvatarRoot().transform.SetParent(m_ActorRoot.transform, false);
                                    int remapIndex = 0;
                                    for (int i = 0; i < m_ModelData.partsChunk.Length; ++i)
                                    {
                                        remapIndex = (int)m_ModelData.partsChunk[i].partChannel;
                                        if (0 <= remapIndex && remapIndex < avatarChanTbl.Length)
                                            ChangeAvatar(avatarChanTbl[remapIndex], m_ModelData.partsChunk[i].partAsset);
                                        else
                                            Logger.LogWarningFormat("Unsupported model data channel enumeration[{0}]!", m_ModelData.partsChunk[i].partChannel.ToString());
                                    }

                                    m_Attachment.RefreshAttachNode(m_Avatar.GetSkeletonRoot());

                                    m_Animation.Init(m_Avatar.GetAvatarRoot());

                                    //string packageName = m_Avatar.GetResPath() + "_AnimPack00";
                                    //m_Animation.AddAnimationPack(packageName);

                                    m_Animation.PlayAction("Anim_Idle_special", 1);

                                    m_ActorState.Init(this);
                                }
                                else
                                {
                                    ChangeAvatar(GeAvatarChannel.WholeBody, new DAssetObject(avatarRes));
                                }

                                return;
                            }
                            else
                                Logger.LogErrorFormat("Init avatar has failed with resource path {0}", avatarRes);
                        }
                    }
                }
            }

            Logger.LogWarningFormat("Invalid avatar resource path:{0}!", avatarRes);
        }
        else
            Logger.LogWarning("avatarRes can not be null or a null string!");
#endif
    }

    public void ClearAvatar()
    {
        if (null != m_Avatar)
            m_Avatar.ClearAll();

        m_Attachment.ClearAll();
    }

    public void LoadState(string skillDataRes)
    {
        m_ActorState.LoadState(skillDataRes);
    }

    public void ChangeState(string state)
    {
        m_ActorState.ChangeState(state);
    }

    public string GetModelDataPath()
    {
        return m_AvatarPath + "/" + m_AvatarName;
    }

    public void ChangeAvatar(GeAvatarChannel eChannel, string modulePath, bool isAsync = false,bool highPriority = false)
    {
        DAssetObject assert = new DAssetObject(null, modulePath);
        ChangeAvatar(eChannel, assert, isAsync);
    }

    public void ChangeAvatar(GeAvatarChannel eChannel, DAssetObject asset,bool isAsync = false,bool highPriority = false)
    {
        if (null != m_Avatar)
        {
            m_Avatar.ChangeAvatarObject(eChannel, asset, isAsync);

            m_avatarDirty = true;
        }
    }

    public void SuitAvatar(bool isAsync = false,bool highPriority = false)
    {
        if (null != m_Avatar)
        {
            m_Avatar.SuitAvatar(isAsync);
        }
    }

    protected void _OverwriteMaterial()
    {
        GameObject[] partModel = m_Avatar.suitPartModel;


        int idx = -1;
        string dstMatRootPath = m_AvatarPath.Replace('\\','/');
        if ('/' == dstMatRootPath[dstMatRootPath.Length - 1])
            dstMatRootPath.Remove(dstMatRootPath.Length - 1);

        idx = dstMatRootPath.LastIndexOf('/');
        if (0 <= idx && idx < dstMatRootPath.Length)
            dstMatRootPath = dstMatRootPath.Substring(0, idx);

        for (int i = 0,icnt = partModel.Length;i< icnt;++i)
        {
            GameObject curPart = partModel[i];

            string part = "";
            string name = curPart.name;
            if (name.Contains("_Pant"))
                part = "Pant";
            else if (name.Contains("_Head"))
                part = "Head";
            else if (name.Contains("_Body"))
                part = "Body";

            SkinnedMeshRenderer[] asmr = curPart.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int j = 0 ,jcnt = asmr.Length;j<jcnt;++j)
            {
                Material[] am = asmr[j].materials;
                for(int k=0,kcnt = am.Length;k<kcnt;++k)
                {
                    string suitPath = am[k].name;

                    if (('m' == suitPath[0] || 'M' == suitPath[0]) && '_' == suitPath[1])
                        suitPath = suitPath.Substring(2);

                    int idxMat = -1;
                    if (suitPath.Contains("_Pant"))
                        idxMat = suitPath.LastIndexOf("_Pant");
                    else if (suitPath.Contains("_Head"))
                        idxMat = suitPath.LastIndexOf("_Head");
                    else if (suitPath.Contains("_Body"))
                        idxMat = suitPath.LastIndexOf("_Body");
                    else if (suitPath.Contains("_Hair"))
                        idxMat = suitPath.LastIndexOf("_Hair");
                    if (0 <= idxMat && idxMat < suitPath.Length)
                        suitPath = suitPath.Substring(0, idxMat);

                    string path = Path.Combine(Path.Combine(Path.Combine(Path.Combine(dstMatRootPath, suitPath), part), "Material") , am[k].name.Replace(" (Instance)", null) + "_Show" );

                    Material newMat = AssetLoader.instance.LoadRes(path, typeof(Material)).obj as Material;
                    //Material newMat = AssetLoader.instance.LoadRes(m_AvatarPath + part + "Material/" + am[k].name.Replace(" (Instance)",null) + "_Show",typeof(Material)).obj as Material;
                    if(null != newMat)
                        am[k] = newMat;
                }

                asmr[j].materials = am;
            }
        }
    }

	public GeAttach AttachAvatar(string attachmentName, string attachRes, string attachNode, bool needClear = true, bool asyncLoad = true, bool highPriority = false, float fAttHeight = 0.0f)
    {
        if (null != m_Attachment)
        {
            m_Attachment.ClearAttachmentOnNode(attachNode);
            GeAttach att = m_Attachment.AddAttachment(attachmentName, attachRes, attachNode,true,asyncLoad,highPriority);
            if(null != att)
                att.SetLayer(9);

            if (fAttHeight != 0.0f)
            {
                Vector3 pos = att.root.transform.localPosition;
                pos.y = fAttHeight;
                att.root.transform.localPosition = pos;
            }

            return att;
        }

        return null;
    }

    public void RemoveAttach(GeAttach attachment)
    {
        if (null != m_Attachment)
        {
            m_Attachment.RemoveAttachment(attachment);
        }
    }

    public GeAttach GetAttachment(string name)
    {
        if (m_Attachment != null)
        {
            return m_Attachment.GetAttachment(name);
        }

        return null;
    }

    public GameObject GeAttachNode(string name)
    {
        if (m_Attachment != null)
        {
            return m_Attachment.GetAttchNodeDesc(name).attachNode;
        }

        return null;
    }

    public void ChangeAction(string actionName, float speed = 1.0f, bool loop = false)
    {
        if (null != m_Animation)
            m_Animation.PlayAction(actionName, speed, loop);
        if (null != m_Attachment)
            m_Attachment.ChangeAction(actionName, speed);
    }


	public string GetCurActionName()
	{
		if (null != m_Animation)
			m_Animation.GetCurActionName();

		return "";
	}

    public GeEffectEx CreateEffect(string effectRes, string attachNode, float fTime, EffectTimeType timeType, Vector3 localPos, Quaternion localRot, float initScale = 1.0f, float fSpeed = 1.0f, bool isLoop = false)
    {
        EffectsFrames effectInfo = new EffectsFrames();
        effectInfo.localPosition = localPos;
        effectInfo.localRotation = localRot;
        effectInfo.localScale = new Vector3(initScale, initScale, initScale);
        if (null == attachNode || "" == attachNode)
        {
            attachNode = "None";
        }
        effectInfo.attachname = attachNode;

        GeAttachManager.GeAttachNodeDesc attachNodeDesc = m_Attachment.GetAttchNodeDesc(attachNode);
        if (!string.IsNullOrEmpty(effectRes))
        {
            DAssetObject asset;
            asset.m_AssetObj = null;
            asset.m_AssetPath = effectRes;

            GeEffectEx newEffect = m_Effect.AddEffect(asset, effectInfo, fTime,new Vector3(0, 0, 0), attachNodeDesc.attachNode, false);
            if (null != newEffect)
            {
                bool bLoop = false;
                if (timeType == EffectTimeType.GLOBAL_ANIMATION)
                    bLoop = true;
                else
                {
                    if (newEffect.GetDefaultTimeLen() < fTime)
                        bLoop = true;
                }

                newEffect.SetSpeed(fSpeed);
                newEffect.Play(bLoop || isLoop);
                return newEffect;
            }
            else
                Logger.LogWarning("Create effect has failed!");
        }
        else
            Logger.LogWarning("Effect resource path can not be null!");

        return null;
    }

    public void ClearEffect()
    {
        m_Effect.ClearAll(GeEffectType.EffectUnique);
        m_Effect.ClearAll(GeEffectType.EffectMultiple);
        m_Effect.ClearAll(GeEffectType.EffectGlobal);
    }

    public void ChangeLayer(int layer)
    {
        if (null != m_Avatar)
            m_Avatar.ChangeLayer(layer);
    }

    public void OnUpdate(float fDelta)
    {
        if (null != m_Avatar)
        {
            if (!m_Avatar.IsLoadFinished())
            {
                m_Avatar.UpdateAsyncLoading();
            }
            else
            {
                if(m_avatarDirty)
                {
                    m_Avatar.SetAvatarVisible(true);
                    GameObject avatarRoot = m_Avatar.GetAvatarRoot();
                    if (null != avatarRoot)
                    {
                        SkinnedMeshRenderer[] amr = avatarRoot.GetComponentsInChildren<SkinnedMeshRenderer>();
                        if (null != amr)
                        {
                            for (int i = 0, icnt = amr.Length; i < icnt; ++i)
                                amr[i].updateWhenOffscreen = true;
                        }
                    }

                    _OverwriteMaterial();
                    m_avatarDirty = false;
                }
            }
        }

        if(fDelta > 0)
        {
            m_ActorState.Update(fDelta);
            m_Attachment.Update();
            int deltaMS = (int)(fDelta * 1000);

            m_Effect.Update(deltaMS, GeEffectType.EffectGlobal);
            m_Effect.Update(deltaMS, GeEffectType.EffectMultiple);
            m_Effect.Update(deltaMS, GeEffectType.EffectUnique);
        }

    }

    public bool IsCurActionEnd()
    {
        if (null != m_Animation)
            return m_Animation.IsCurAnimFinished();

        return true;
    }

    public GameObject avatarRoot
    {
        get { return m_ActorRoot; }
    }
}
