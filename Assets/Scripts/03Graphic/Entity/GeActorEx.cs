using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoTable;

using GameClient;

/// <summary>
/// 临时引用的程序集
/// </summary>
/// 
using UnityEngine.UI;
using Protocol;

public delegate void PosLoadGeActorEx(GeActorEx geActor);


/*
root//这层设坐标，旋转，缩放
  -character //这层改镜像
    -actor   //这层改类似抖动效果
  -other //这里可以挂血条，buff效果，特效等会随角色移动的东西

    
*/


public class GeActorEx : GeEntity
{
    private const string kGeActorExTag = "GeActorEx_";

    public struct GeActorDesc
    {
        public string name;
        public int resID;
        public string resName;
        public string resPath;
        public string modelDataRes;
        public string portraitIconRes;
    }

    public GeActorDesc m_ActorDesc;


    static readonly GeAvatarChannel[] avatarChanTbl = new GeAvatarChannel[]
    {
        GeAvatarChannel.Head,       /// eModelHead
        GeAvatarChannel.UpperPart,  /// eModelUpperPart
        GeAvatarChannel.LowerPart,  /// eModelLowerPart
        GeAvatarChannel.Bracelet,   /// eModelShoulder
        GeAvatarChannel.Headwear,   /// eModelShoulder
        GeAvatarChannel.Wings,      /// eModelWings
        GeAvatarChannel.WholeBody,  /// eModelWholeBody
    };
    protected enum GeEntityState
    {
        Invalid,
        Loaded,
        Inited,
        Removed,
    }
    protected GeEntityState m_EntityState = GeEntityState.Invalid;

    /// <summary>
    /// Actor的显示模式
    /// </summary>
    public enum DisplayMode
    {
        Normal,
        Simple
    }
    protected DisplayMode m_DisplayMode = DisplayMode.Normal;

    public BeEntity entity = null;
    public Dictionary<int, string> skillData = new Dictionary<int, string>();

#if !LOGIC_SERVER
    private Dictionary<int, string> m_FashionsData;
    public Dictionary<int, string> FashionsData => m_FashionsData;

    protected GameObject[] m_ActorPartes = null;
    protected Bounds m_BoundingBox;
    protected bool m_IsHighLight = false;


    private uint mPostLoadTownNPCHandle = uint.MaxValue;

    private PosLoadGeActorEx mPostLoadTownNPC = null;
    public PosLoadGeActorEx mPostLoadInfoBar = null;


    #region 杂项
    /// <summary>
    ///  下面的数据都是临时放在GeActor中的正常情况下是不应该在这个模块中出现的
    ///  
    /// </summary>
    /// 

    public GameObject goHPBar = null;
    public IHPBar mCurHpBar = null;

    /// <summary>
    /// 当前血条的ID
    /// 
    /// 由HPBarManager分配
    /// </summary>
    public int mCurrentHpBarId = HPBarManager.kInvalidHpBarId;

    public int mCurrentStateBarId = StateBarManager.kInvalidStateBarId;

    //#region AsyncLoad
    //protected List<uint> mCachedAsyncHandles = new List<uint>();

    //protected uint _addAsyncLoadGameObject(string path, PostLoadGameObject load, uint condition = uint.MaxValue)
    //{
    //    uint handle = AsyncLoadTaskManager.instance.AddAsyncLoadGameObject(kGeActorExTag, path, load, condition);
    //    mCachedAsyncHandles.Add(handle);

    //    return handle;
    //}

    //public uint _addAsyncLoadGameObject(string path, enResourceType restype, bool reserveLast, PostLoadGameObject load, uint condition = uint.MaxValue)
    //{
    //    uint handle = AsyncLoadTaskManager.instance.AddAsyncLoadGameObject(kGeActorExTag, path, restype, reserveLast, load, condition);
    //    mCachedAsyncHandles.Add(handle);
    //    return handle;
    //}

    //public void _clearAsyncLoadGameObject()
    //{
    //    for (int i = 0; i < mCachedAsyncHandles.Count; ++i)
    //    {
    //        AsyncLoadTaskManager.instance.RemoveAsyncLoadGameObjectByHandle(mCachedAsyncHandles[i]);
    //    }
    //    mCachedAsyncHandles.Clear();
    //}
    //#endregion

    public IHPBar mCurHpHeadBar = null;

    public GameObject goHPPKBar = null;
    public CPKHPBar comHPPKBar = null;
    public GameObject goHPBarHUD = null;
    public GameObject goInfoBar = null;
    public GameObject goTransportInfo = null;
    public GameObject goInfoBarBottom = null;
    public GameObject goSpellBar = null;
    public GameObject goFootInfo = null;
    public GameObject goDialog = null;
    public GameObject goShowFindPath = null;
    DialogScript dialog = null;
    public TitleComponent titleComponent = null;
    NpcArrowComponent npcArrowComponent = null;
    NpcDialogComponent dialogComponent = null;
    NpcVoiceComponent voiceComponent = null;

    public bool isSyncHPMP = true;
    public NpcDialogComponent NpcDialogComponent
    {
        get
        {
            return dialogComponent;
        }
    }
    public NpcVoiceComponent NpcVoiceComponent
    {
        get
        {
            return voiceComponent;
        }
    }
    public CHPBar cSpellBar = null;
    public CStateBar cStateBar = null;
    //public BehaviorTree tree;

    public ComTags cTagsBar = null;

    public Helper_DrawBox m_drawBox = null;

    private Sprite entityHeadIcon = null;
    private AssetInst entityHeadIconAsset = null;
    private Material entityHeadIconMaterial = null;

    public HPBarManager hpBarManager = null;
    public StateBarManager stateBarManager = null;

    public string stringLevelPath = "Bottom/Lv";

    public Dictionary<string, int> headTextCount = new Dictionary<string, int>();

    public bool showTransport = false;

    public GameObject objOverhead = null;
    public GameObject objBuffOrigin = null;
    public Vector3 mBuffOriginPosition = Vector3.zero;
    public Vector3 mBuffOriginLocalPosition = Vector3.zero;
    public Vector3 bodyLocalPosition = Vector3.zero;
    public Vector3 originLocalPosition = Vector3.zero;
    private Vector3 overheadLocalPosition = Vector3.zero;
    static Vector3 defaultBuffOriginPosition = new Vector3(0.0f, 2.0f, 0.0f);

    public ComCommonBind slideArrowBind = null;     //滑动箭头对用的组件
    public ComCommonBind forwardBackArrowBind = null;     //滑动箭头对用的组件(前后)

    private string[,] kHeadDialogPaths = new string[2, 2]
    {
        //useSkill                                                  , useSkill
        //true                                                      , false
        { "UIFlatten/Prefabs/DialogParent/DialogParent_battle_skill", "UIFlatten/Prefabs/DialogParent/DialogParent"        },  // bUseLink = true
        { "UIFlatten/Prefabs/DialogParent/DialogParent_battle_skill", "UIFlatten/Prefabs/DialogParent/DialogParent_battle" },  // bUseLink = false
    };
    public Vector3 buffOriginPosition
    {
        get
        {
            if (isCreatedInBackMode)
            {
                return defaultBuffOriginPosition;
            }
            return mBuffOriginPosition;
        }
        set
        {
            mBuffOriginPosition = value;
        }
    }
    public Vector3 buffOriginLocalPosition
    {
        get
        {
            if (isCreatedInBackMode)
            {
                return defaultBuffOriginPosition;
            }
            return mBuffOriginLocalPosition;
        }
        set
        {
            mBuffOriginLocalPosition = value;
        }
    }

    private enum DialogType
    {
        None = -1,
        Normal = 0, //普通
        Explode,    //爆炸效果
        Link,       //链接
        Count,
    }

    private GameObject[] mDialogs = new GameObject[(int)DialogType.Count];
    private DialogScript[] mDialogScripts = new DialogScript[(int)DialogType.Count];

    private string[] mDialogPaths = new string[] { 
        "UIFlatten/Prefabs/DialogParent/DialogParent_battle" ,
        "UIFlatten/Prefabs/DialogParent/DialogParent_battle_skill",
        "UIFlatten/Prefabs/DialogParent/DialogParent"
    };

    protected class MatSurfRenderDesc
    {
        public MatSurfRenderDesc(Material[] origMat,Renderer mr)
        {
            m_MeshRenderer = mr;
            m_OriginMatList = origMat;
        }

        public Renderer m_MeshRenderer = null;
        public Material[] m_OriginMatList = null;
    }
    protected class MatSurfObjDesc
    {
        public MatSurfObjDesc(MatSurfRenderDesc[] meshRendDesc)
        {
            m_MatMeshRendDescList = meshRendDesc;
        }

        public MatSurfRenderDesc[] m_MatMeshRendDescList = null;
    }

    protected Material m_SurfMaterial = null;
    protected List<MatSurfObjDesc> m_MatSurfObjDescList = new List<MatSurfObjDesc>();

    private List<Renderer> m_EmissiveRenderers;
    private MaterialPropertyBlock m_EmissiveBlock;
    private bool m_EnableEmissiveColor;
    private Color m_LastEmissiveColor = Color.clear;
    private Color m_DestEmissiveColor = Color.clear;
    private float m_EmissiveTimer = 0f;
    private float m_EmissiveDuration = 0f;

    public List<string> hpBarBuffEffectNameList = new List<string>();
    public string curHpBarBuffName = string.Empty;
	protected static Dictionary<string, Vector3> cachedOverhead = new Dictionary<string, Vector3>();
    #region 私有

    private string _getHeadDialogPath(bool bUseLink, bool useSkill)
    {
        int fstIdx = bUseLink ? 0 : 1;
        int sndIdx = useSkill ? 0 : 1;
        return kHeadDialogPaths[fstIdx, sndIdx];
    }
    private bool _isAvatatrPrefabLoadFinish()
    {
        return null != m_EntitySpaceDesc.actorModel;
    }

    private bool _isPlayerInfoHeadLoadFinish()
    {
        return null != goInfoBar;
    }
    private void _onAddNpcArrowComponent(GameObject gameobject)
    {
        if (null == gameobject)
        {
            return;
        }

        npcArrowComponent = gameobject.GetComponent<NpcArrowComponent>();
        if (npcArrowComponent == null)
        {
            npcArrowComponent = gameobject.AddComponent<NpcArrowComponent>();
            npcArrowComponent.DeActive();
        }
        Battle.GeUtility.AttachTo(gameobject, m_EntitySpaceDesc.characterNode);
    }

    void moveBuffsEffects(GameObject fr, GameObject to)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.moveBuffsEffects"))
        {
#endif
        BeActor buffOwer = entity as BeActor;
        if (buffOwer == null)
            return;

        List<BeBuff> buffList = buffOwer.buffController.GetBuffList();
        int count = buffList.Count;
        if (count < 1)
            return;

        GeEffectEx curEffectEx = null;
        GameObject parentNode = null;
        GameObject parentNodeNew = null;
        for (int i = 0; i < count; i++)
        {
            curEffectEx = buffList[i].GetEffectEx();
            if (curEffectEx == null)
                continue;

            parentNode = curEffectEx.GetParentNode();
            if (parentNode == null)
                continue;

            parentNodeNew = m_Avatar.GetAttchNodeDescWithRareName(parentNode.name);
            if (parentNodeNew == null)
                continue;

            curEffectEx.SetParentNode(parentNodeNew);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    void PreLoadModelsFromSkill(BDEntityActionFrameData data)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.PreLoadModelsFromSkill"))
        {
#endif
        if (data == null)
            return;

        List<BDEventBase> events = data.pEvents;

        int count = events.Count;
        if (count < 1)
            return;

        BDEventBase curEvent = null;
        BDSkillFrameEffect curSkillEvent = null;
        for (int i = 0; i < count; i++)
        {
            curEvent = events[i];
            curSkillEvent = curEvent as BDSkillFrameEffect;
            if (curSkillEvent == null)
                continue;

            ProtoTable.EffectTable effectData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(curSkillEvent.effectID);
            if (effectData == null)
                continue;

            ProtoTable.BuffTable buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(effectData.BuffID);
            if (buffData == null)
                continue;

            if (buffData.MechanismID.Count < 1)
                continue;

            ProtoTable.MechanismTable mechanismData = TableManager.GetInstance().GetTableItem<ProtoTable.MechanismTable>(buffData.MechanismID[0]);
            if (mechanismData == null)
                continue;

            if (mechanismData.Index != 27) // TODO:: 临时
                continue;

            var entityData = entity.GetEntityData();

            int monsterId = Mechanism27.GetMonsterId(entityData.monsterID, mechanismData);
            if (monsterId == 0)
                break;

            ProtoTable.UnitTable unitData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monsterId);
            if (unitData == null)
                break;

            PreChangeModel(unitData.Mode);
        }
#if ENABLE_PROFILER
        }
#endif
    }
    private bool _initActorDescWithResTable(ProtoTable.ResTable resData)
    {
        if (null == resData)
        {
            return false;
        }

        m_ActorDesc.resID = resData.ID;
        m_ActorDesc.resPath = resData.ModelPath;
        m_ActorDesc.resName = Path.GetFileNameWithoutExtension(m_ActorDesc.resPath);
        m_ActorDesc.portraitIconRes = resData.IconPath;

        return true;
    }
    private void _onCreateNPCAsync(GameObject actorModel)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onCreateNPCAsync"))
        {
#endif
        if (_onActorModelPrefabLoadFinish(actorModel))
        {
            if (null != actorModel)
            {
                actorModel.CustomActive(true);
            }

            m_EntityState = GeEntityState.Loaded;
            _onPostActorModelPrefabLoadFinish(actorModel);
            if (null != mPostLoadTownNPC)
            {
                mPostLoadTownNPC(this);
            }
        }
        else
        {
            Logger.LogErrorFormat("[GeActorEx] Actor is nil with path {0}", m_ActorDesc.resPath);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    private bool _onActorModelPrefabLoadFinish(GameObject actorModel)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onActorModelPrefabLoadFinish"))
        {
#endif
        if (null == actorModel)
        {
            Logger.LogErrorFormat("[GeActorEx] Actor is nil with path {0}", m_ActorDesc.resPath);
            return false;
        }

        m_ActorPartes = new GameObject[] { actorModel };
        Battle.GeUtility.AttachTo(actorModel, m_EntitySpaceDesc.actorNode);
        m_EntitySpaceDesc.actorModel = actorModel;

        Renderer rnd = actorModel.GetComponent<Renderer>();
        if (null != rnd)
        {
            m_BoundingBox.min = Vector3.Min(rnd.bounds.min, m_BoundingBox.min);
            m_BoundingBox.max = Vector3.Min(rnd.bounds.max, m_BoundingBox.max);
        }

        GeGraphicSetting.instance.CheckComponent(actorModel);

        return true;
#if ENABLE_PROFILER
        }
#endif
    }
    private void _onPostActorModelPrefabLoadFinish(GameObject actorModel)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onPostActorModelPrefabLoadFinish"))
        {
#endif
       // TODO HACKER
        actorModel.layer = 9;
        if (isBattleActor)
            m_Material.AppendObject(m_ActorPartes);
        //m_Animation.Init(m_EntitySpaceDesc.actorNode);
        //
        //m_Attachment.RefreshAttachNode(m_EntitySpaceDesc.actorNode);

        m_EntitySpaceDesc.childNode = m_EntitySpaceDesc.actorNode;

        // icon
        if (m_ActorDesc.portraitIconRes != "-" && m_ActorDesc.portraitIconRes.Length >= 2)
        {
            entityHeadIconAsset = AssetLoader.instance.LoadRes(m_ActorDesc.portraitIconRes, typeof(Sprite));
            entityHeadIcon = entityHeadIconAsset.obj as Sprite;

            entityHeadIconMaterial = ETCImageLoader.LoadMaterialFromSpritePath(m_ActorDesc.portraitIconRes);
        }

        if (null != m_ModelData && null != m_EntitySpaceDesc.transformNode)
            m_EntitySpaceDesc.transformNode.transform.localScale = m_ModelData.modelScale;

        if (null != m_Scene)
            SetDyeColor(m_Scene.GetObjectDyeColor(), m_ActorPartes);

        InitAttachPoint();
        _RefleshUIComponent();

        m_EntityState = GeEntityState.Inited;
        
        AssetGabageCollectorHelper.instance.AddGCPurgeTick(AssetGCTickType.SceneActor);
        m_IsAvatarDirty = true;
#if ENABLE_PROFILER
        }
#endif
    }
    #endregion
#region NpcDialogComponet
    private class NpcDialogComponentData
    {
        public System.Int32 iDialogID { get; private set; }
        public NpcDialogComponent.IdBelong2 eIdBelong2 { get; private set; }

        public NpcDialogComponentData(System.Int32 iDialogID, NpcDialogComponent.IdBelong2 eIdBelong2)
        {
            this.iDialogID = iDialogID;
            this.eIdBelong2 = eIdBelong2;
        }
    }

    private NpcDialogComponentData mDialogComponentData = null;


    private void _onAddComponetDialog(GameObject gameobject)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onAddComponetDialog"))
        {
#endif
        if (null == gameobject)
        {
            return;
        }

        if (null == mDialogComponentData)
        {
            Logger.LogProcessFormat("[GeActorEx] DialogCompoentData is nil");
            return;
        }

        gameobject.name = "DialogParent";
        Battle.GeUtility.AttachTo(gameobject, m_EntitySpaceDesc.rootNode);

        GameObject goPopUpDialog = gameobject.transform.Find("PopUpDialog").gameObject;
        if (goPopUpDialog != null)
        {
            dialogComponent = goPopUpDialog.GetComponent<NpcDialogComponent>();
            if (dialogComponent == null)
            {
                dialogComponent = goPopUpDialog.AddComponent<NpcDialogComponent>();
            }

            if (dialogComponent != null && null != mDialogComponentData)
            {
                Vector3 worldPos = Vector3.zero;

                GameObject objRoot = GetEntityNode(GeEntity.GeEntityNodeType.Root);
                GameObject objOverhead = Utility.FindThatChild("OverHead", objRoot,true);

                if (objOverhead != null)
                {
                    worldPos = objOverhead.transform.position;
                    worldPos.y += 0.45f;
                    //worldPos.x = 0;
                    //worldPos.z = 0;
                }
                else
                {
                    Logger.LogError("[GeActorEx] ObjOverhead is null");
                }

                dialogComponent.Initialize
                (
                    worldPos,
                    mDialogComponentData.iDialogID,
                    mDialogComponentData.eIdBelong2
                );
            }
        }

        mDialogComponentData = null;
#if ENABLE_PROFILER
        }
#endif
    }

    #endregion
    #region NPCBoxCollect

    private class NPCBoxCollectData
    {
        public BeTownNPCData townData { get; private set; }

        public NPCBoxCollectData(BeTownNPCData data)
        {
            townData = data;
        }
    }

    private NPCBoxCollectData mNPCBoxCollectData = null;
    private void _onAddNPCBoxCollider()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onAddNPCBoxCollider"))
        {
#endif
        if (null == mNPCBoxCollectData)
        {
            return;
        }

        GameObject objActor = GetEntityNode(GeEntity.GeEntityNodeType.Actor);
        if (objActor != null)
        {
            if (objActor != null && objActor.GetComponent<BoxCollider>() != null)
            {
                RaycastObject comRaycastObject = objActor.GetComponent<RaycastObject>();
                if (comRaycastObject == null)
                {
                    comRaycastObject = objActor.AddComponent<RaycastObject>();
                    comRaycastObject.Initialize(mNPCBoxCollectData.townData.NpcID, RaycastObject.RaycastObjectType.ROT_NPC, mNPCBoxCollectData.townData);
                }
                else
                {
                    Logger.LogError("[GeAcotrEx] comRayCastObject is nil");
                }
            }
        }
        else
        {
            Logger.LogError("[GeAcotrEx] objActor is nil");
        }
#if ENABLE_PROFILER
        }
#endif
    }

    #endregion
#region NpcInteractionData

    private class NpcInteractionData
    {
        public UInt64 Guid { get; set; }
        public int npcID { get; private set; }
        public NpcInteractionData(int npcID,UInt64 guid = 0)
        {
            this.npcID = npcID;
            this.Guid = guid;
        }
    }

    private NpcInteractionData mNpcInteractionData = null;
    private void _onAddNPCInteraction()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onAddNPCInteraction"))
        {
#endif
        if (null == goInfoBar)
        {
            Logger.LogProcessFormat("[GeActorEx] goInfoBar is nil");
            return;
        }

        if (null == mNpcInteractionData)
        {
            Logger.LogProcessFormat("[GeActorEx] NpcInteractionData is nil");
            return;
        }

        NpcInteraction comNpcInteraction = goInfoBar.GetComponent<NpcInteraction>();
        if (comNpcInteraction == null)
        {
            comNpcInteraction = goInfoBar.AddComponent<NpcInteraction>();
        }

        comNpcInteraction.Initialize(mNpcInteractionData.npcID, mNpcInteractionData.Guid);

        mNpcInteractionData = null;
#if ENABLE_PROFILER
        }
#endif
    }
    #endregion
    #region PlayerInfoData
    Text m_kLevelText;
    public class PlayerInfoBarData
    {
        public PlayerInfoBarData(string name, PlayerInfoColor infoColor, ushort RoleLevel, string namecolors, float nameLocalPosY)
        {
            this.name = name;
            this.infoColor = infoColor;
            this.RoleLevel = RoleLevel;
            this.namecolors = namecolors;
            this.NameLocalPosY = nameLocalPosY;
        }

        public string name { get; private set; }
        public PlayerInfoColor infoColor { get; private set; }
        public ushort RoleLevel { get; private set; }
        public string namecolors { get; private set; }
        public float NameLocalPosY { get; private set; }
    }

    private PlayerInfoBarData mPlayerInfoBarData = null;
    
    public PlayerInfoBarData CurPlayerInfoBarData
    {
        get { return mPlayerInfoBarData; }
    }
    
    //private uint mPlayerInfoBarDataHandle = uint.MaxValue;
    private void _onCreateInfoBar(GameObject gameobject)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onCreateInfoBar"))
        {
#endif
        if (null == gameobject)
        {
            return;
        }

        if (null == mPlayerInfoBarData)
        {
            Logger.LogProcessFormat("[GeActorEx] PlayerInfoBarData is nil");
            CGameObjectPool.RecycleGameObjectEx(gameobject);
            return;
        }
        string goName = gameobject.name;
        goInfoBar = gameobject;
        goInfoBar.name = "PlayerInfo_Head";
        goInfoBar.CustomActive(true);

        goInfoBar.transform.localPosition = new Vector3(0, 2.215332f,0);
        goInfoBar.transform.localEulerAngles = new Vector3(20,0,0);
        goInfoBar.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        //goInfoBar.transform.SetParent(m_EntitySpaceDesc.rootNode.transform, false);
        //Battle.GeUtility.AttachTo(goInfoBar, m_EntitySpaceDesc.rootNode);
        AttachToRootNode(goInfoBar, SceneUINodeType.ActorRoot_InfoHead);

        ComCommonBind bind = goInfoBar.GetComponent<ComCommonBind>();
        if (bind != null)
        {
            goInfoBarBottom = bind.GetGameObject("goInfoBarBottom");
            if(goInfoBarBottom != null)
            { 
                goInfoBarBottom.CustomActive(true);
            }
            else
            {
                Logger.LogErrorFormat("_onCreateInfoBar has not goInfoBarBottom {0}", goName);
            }

            GameObject goExchangeShop = bind.GetGameObject("ExchangeShop");
            if (goExchangeShop != null)
            {
                goExchangeShop.CustomActive(false);
            }

            //goInfoBarBottom = Utility.FindChild(goInfoBar, "Bottom");

            _updateChapterHeadInfoBarPosition();

            // 名字
            Text textComp = bind.GetCom<Text>("textComp");
            //Text textComp = Utility.FindChild(goInfoBar, "Bottom/Name").GetComponent<Text>();
            if (textComp != null)
            {
                textComp.text = mPlayerInfoBarData.name;

                if (mPlayerInfoBarData.namecolors == null)
                {
                    textComp.supportRichText = false;
                    //CPlayerInfo info = goInfoBar.GetComponent<CPlayerInfo>();
                    CPlayerInfo info = bind.GetCom<CPlayerInfo>("info");
                    if (info != null)
                    {
                        textComp.color = info.GetColor(mPlayerInfoBarData.infoColor);
                    }
                }
                else
                {
                    textComp.supportRichText = true;
                    textComp.text = string.Format("<color={0}>{1}</color>", mPlayerInfoBarData.namecolors, mPlayerInfoBarData.name);
                }
            }

            // 等级
            m_kLevelText = bind.GetCom<Text>("levelText");
            UpdateLevel(mPlayerInfoBarData.RoleLevel);

            goTransportInfo = bind.GetGameObject("transport");
        }
        else
        {
            Logger.LogErrorFormat("_onCreateInfoBar has not bind {0}", goName);
        }
        //goTransportInfo = Utility.FindChild(goInfoBar, "transport");

        GeMeshRenderManager.GetInstance().AddMeshObject(goInfoBar);

        _onPostLoadChapterHeadInfoRoot();

        //调整位置，针对攻城怪物中的名字位置
        SetGoInfoBarLocalPosition(goInfoBar.transform.localPosition);
#if ENABLE_PROFILER
        }
#endif
    }
    private void _onCreateInfoBarCallBack(GameObject gameobject)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onCreateInfoBarCallBack"))
        {
#endif
        //if (~0u == mPlayerInfoBarDataHandle)
        //{
        //    CGameObjectPool.RecycleGameObjectEx(gameobject);
        //    return;
        //}
        if (CanRemove())
        {
            CGameObjectPool.RecycleGameObjectEx(gameobject);
            return;
        }
        _onCreateInfoBar(gameobject);
        if (mPostLoadInfoBar != null)
        {
            mPostLoadInfoBar(this);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 设置goInfoBar的局部坐标
    /// </summary>
    /// <param name="localPos"></param>
    private void SetGoInfoBarLocalPosition(Vector3 localPos)
    {
        //如果默认的局部坐标的Y不为0，使用配置的Y值；其他情况使用默认的数值
        if (mPlayerInfoBarData == null)
            return;
        if (mPlayerInfoBarData.NameLocalPosY > 0)
        {
            var goInfoBarLocalPos = goInfoBar.transform.localPosition;
            goInfoBar.transform.localPosition = new Vector3(
                goInfoBarLocalPos.x,
                mPlayerInfoBarData.NameLocalPosY,
                goInfoBarLocalPos.z);

        }
        else
        {
            goInfoBar.transform.localPosition = localPos;
        }
    }

    private void _updateChapterHeadInfoBarPosition()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._updateChapterHeadInfoBarPosition"))
        {
#endif
        if (null == goInfoBar)
        {
            return;
        }

        Vector3 localPos = buffOriginLocalPosition;
        if (entity != null &&
            entity.GetEntityData() != null &&
            entity.GetEntityData().isMonster &&
            entity.GetEntityData().buffOriginHeight > 0f)
        {
            localPos.y = entity.GetEntityData().buffOriginHeight;
        }
        else
        {
            localPos.y -= 0.25f;
        }

        //避免怪物使用Cube创建时头顶名字显示在脚底或者场景下方的BUG
        if (useCube)
        {
            localPos.y = 1.5f;
        }

        goInfoBar.transform.localPosition = localPos;
#if ENABLE_PROFILER
        }
#endif
    }

    private void _onPostLoadChapterHeadInfoRoot()
    {
        _onAddTittleComponent();
    }

    #endregion
#region TittleComponent
    public class TittleComponentData
    {
        public System.Int32 iTittleID { get; private set; }
        public string name { get; private set; }

        public byte guildDuty { get; private set; }
        public string bangName { get; private set; }
        public System.Int32 iRoleLv { get; private set; }
        public int a_nPKRank { get; private set; }
        public PlayerInfoColor color { get; private set; }
        public int iVipLevel { get; private set; }
        public string adventTeamName { get; private set; }
        public PlayerWearedTitleInfo playerWearedTitleInfo { get; private set; }//头衔

        public int guildEmblemLv { get; private set; }
        public TittleComponentData(
            System.Int32 iTittleID,
            string name,
            byte guildDuty,
            string bangName,
            System.Int32 iRoleLv,
            int a_nPKRank,
            PlayerInfoColor color,
            string adventTeamName,
            PlayerWearedTitleInfo playerWearedTitleInfo,
            int guileEmblemLv,
            int iVipLevel = 0)
        {
            this.iTittleID = iTittleID;
            this.name = name;
            this.guildDuty = guildDuty;
            this.bangName = bangName;
            this.iRoleLv = iRoleLv;
            this.a_nPKRank = a_nPKRank;
            this.color = color;
            this.iVipLevel = iVipLevel;
            this.adventTeamName = adventTeamName;
            this.playerWearedTitleInfo = playerWearedTitleInfo;
            this.guildEmblemLv = guileEmblemLv;
        }
    }

    private TittleComponentData mTittleComponentData = null;
    public TittleComponentData CurTittleComponentData
    {
        get
        {
            return mTittleComponentData;
        }
    }
    
    private void _onAddTittleComponent()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._onAddTittleComponent"))
        {
#endif
        if (m_EntitySpaceDesc.rootNode == null)
        {
            Logger.LogError("call create infobar first!");
            return;
        }

        if (goInfoBar == null)
        {
            Logger.LogError("call create infobar first!");
            return;
        }

        if (null == mTittleComponentData)
        {
            Logger.LogProcessFormat("[GeActorEx] mTittleComponentData is nil");
            return;
        }

        CPlayerInfo comCPlayerInfo = goInfoBar.GetComponent<CPlayerInfo>();
        if (comCPlayerInfo == null)
        {
            Logger.LogError("can not find CPlayerInfo!");
            return;
        }

        var nameColor = comCPlayerInfo.GetColor(mTittleComponentData.color);

        if (titleComponent == null)
        {
            titleComponent = TitleComponent.Create(goInfoBar);
            if (titleComponent == null)
            {
                return;
            }
            goInfoBarBottom.CustomActive(false);
        }

        titleComponent.SetTitleData(
            mTittleComponentData.iTittleID,
            mTittleComponentData.a_nPKRank,
            mTittleComponentData.iVipLevel,
            mTittleComponentData.guildDuty,
            mTittleComponentData.bangName,
            mTittleComponentData.iRoleLv,
            mTittleComponentData.name,
            mTittleComponentData.adventTeamName,
            mTittleComponentData.playerWearedTitleInfo,
            mTittleComponentData.guildEmblemLv,
            nameColor);
        //titleComponent.SetNameColor(nameColor);
#if ENABLE_PROFILER
        }
#endif
    }
    #endregion
    #region DungeonBar
    public ComDungeonBarRoot mBarsRoot;


    private List<IDungeonCharactorBar> mBars = new List<IDungeonCharactorBar>();
    private const float BAR_ROOT_HEIGHT = 0.78f;
    private const float HEAD_DIALOG_HEIGHT = 0.45f;
    private const float HP_BAR_HEIGHT = 0.15f;
    private const float SPELL_BAR_HEIGHT = 0.2f;

    private void _createBarRoot()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx._createBarRoot"))
        {
#endif
        if (null == mBarsRoot)
        {
            var bar = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonBarRoot");
            if (null != bar)
            {
                //Battle.GeUtility.AttachTo(bar, m_EntitySpaceDesc.rootNode);
                AttachToRootNode(bar, SceneUINodeType.ActorRoot_DungeonBar);

                Vector3 localPos = GetOverHeadPosition();
				if (entity.GetEntityData()!=null && entity.GetEntityData().overHeadHeight > 0f)
					localPos.y = entity.GetEntityData().overHeadHeight;
				localPos.y += BAR_ROOT_HEIGHT;
				bar.transform.localPosition = localPos;

                mBarsRoot = bar.GetComponent<ComDungeonBarRoot>();
                
                mBarsRoot.dRoot.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -75 * localPos.y, 0);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }
private string _getBarPath(eDungeonCharactorBar type)
    {
        switch (type)
        {
            case eDungeonCharactorBar.Sing:
            case eDungeonCharactorBar.Buff:
            case eDungeonCharactorBar.Continue:
            case eDungeonCharactorBar.Progress:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorHeadSn";
            case eDungeonCharactorBar.Power:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorHeadPn";
                //return "UI/Prefabs/Chapter/DungeonBar/DungeonCharactorBar";
            case eDungeonCharactorBar.Loop:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorBar";
				//return "UI/Prefabs/Chapter/DungeonBar/DungeonCharactorHeadBoss";
            case eDungeonCharactorBar.DunFuCD:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonDunFuBar";
            case eDungeonCharactorBar.RayDrop:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorRayBar";
            case eDungeonCharactorBar.Fire:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorHeadFire";
            case eDungeonCharactorBar.MonsterEnergyBar:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorMonsterEnergy";
            default:
                break;
        }

        return "";
    }

    private IDungeonCharactorBar _attachBarRoot(eDungeonCharactorBar type, GameObject go)
    {
        if (null == go)
        {
            return null;
        }
        if (null != mBarsRoot)
        {
            switch (type)
            {
                case eDungeonCharactorBar.Sing:
                case eDungeonCharactorBar.Power:
				case eDungeonCharactorBar.Continue:
				case eDungeonCharactorBar.Buff:
                case eDungeonCharactorBar.DunFuCD:
                case eDungeonCharactorBar.Progress:
                    Utility.AttachTo(go, mBarsRoot.vRoot);
                    return go.GetComponent<ComDungeonCharactorHeadBar>();
                case eDungeonCharactorBar.Fire:
                    Utility.AttachTo(go, mBarsRoot.dRoot);
                    return go.GetComponent<ComDungeonCharactorHeadFireBar>();
                case eDungeonCharactorBar.Loop:
                case eDungeonCharactorBar.RayDrop:
                case eDungeonCharactorBar.MonsterEnergyBar:
                    Utility.AttachTo(go, mBarsRoot.hRoot);
                    return go.GetComponent<ComDungeonCharactorBar>();
                default:
                    break;
            }
        }

        return null;
    }

    private ComDungeonComboTips mTips = null;
private void _stopBarEffect(eDungeonCharactorBar type)
    {
        GameObject root = null;
        switch (type)
        {
            case eDungeonCharactorBar.Sing:
            case eDungeonCharactorBar.Power:
			case eDungeonCharactorBar.Continue:
			case eDungeonCharactorBar.Buff:
            case eDungeonCharactorBar.Progress:
                root = mBarsRoot.vRoot;
                break;
            case eDungeonCharactorBar.Fire:
                root = mBarsRoot.dRoot;
                break;
            case eDungeonCharactorBar.Loop:
            case eDungeonCharactorBar.RayDrop:
            case eDungeonCharactorBar.MonsterEnergyBar:
                root = mBarsRoot.hRoot;
                break;
            default:
                break;
        }

        if (null != root)
        {
            var bar = _GetBarByType(type);
            if (null != bar)
            {

                GeEffectEx effect = CreateEffect(
                      "Effects/Hero_Jixieshi/EZ-8Zibaozhe/Perfab/Eff_Ez-8Zibaozhe_guang",
                      "",
                      0,
                      new Vec3(0, 0, 0));

				if (effect != null)
				{
					Battle.GeUtility.AttachTo(effect.GetRootNode(), root);
					effect.GetRootNode().transform.position = bar.GetGameObject().transform.position;
				}
            }
        }
    }
public Bounds boundingBox
    {
        get
        {
            if (null != m_Avatar)
                return m_Avatar.boundingBox;

            return m_BoundingBox;
        }
    }
    public GameObject[] renderObject
    {
        get
        {
            return m_ActorPartes;
        }
    }

    //创建各种读条的父节点
    public void CreateBarRoot()
    {
        _createBarRoot();
    }
    #endregion

    #endregion

    public static void ClearStatic()
	{
		cachedOverhead.Clear();
	}

    public Vector3 GetOverHeadPosition()
    {
        if (overheadLocalPosition == Vector3.zero)
            _RefreshOverHeadPostion();

        return overheadLocalPosition;
    }

    private void _RefreshOverHeadPostion()
    {
        //objOverhead = Utility.FindThatChild("OverHead", m_EntitySpaceDesc.actorNode);
        //if (objOverhead != null)
        //{
        //    if (cachedOverhead.ContainsKey(GetResPath()))
        //        overHeadPosition = cachedOverhead[GetResPath()];
        //    else
        //    {
        //        overHeadPosition = objOverhead.transform.position;
        //        cachedOverhead.Add(GetResPath(), overHeadPosition);
        //    }

        //    overheadLocalPosition = m_EntitySpaceDesc.rootNode.transform.InverseTransformVector(overHeadPosition);
        //    overheadLocalPosition.x = 0f;
        //    overheadLocalPosition.z = 0f;
        //}
        //else
        {
            overheadLocalPosition = buffOriginLocalPosition;
            overheadLocalPosition.y -= 0.6f;
        }
    }

    public void InitAttachPoint()
    {
		var objOrigin = Utility.FindThatChild("Orign", m_EntitySpaceDesc.actorNode);
		if (objOrigin != null)
		{
			originLocalPosition = m_EntitySpaceDesc.rootNode.transform.InverseTransformVector(objOrigin.transform.position);
			originLocalPosition.x = 0f;
			originLocalPosition.z = 0f;
		}
			

		objBuffOrigin = Utility.FindThatChild("OrignBuff", m_EntitySpaceDesc.actorNode);
		if (objBuffOrigin != null)
		{
			buffOriginPosition = objBuffOrigin.transform.position;
			buffOriginLocalPosition = m_EntitySpaceDesc.rootNode.transform.InverseTransformPoint(buffOriginPosition);
			mBuffOriginLocalPosition.x = 0f;
            mBuffOriginLocalPosition.z = 0f;
		}
		else {
			buffOriginLocalPosition = originLocalPosition;
            mBuffOriginLocalPosition.y += 1.5f;
		}

        overheadLocalPosition = Vector3.zero;

		var objBody = Utility.FindThatChild("Body", m_EntitySpaceDesc.actorNode);
		if (objBody != null)
		{
			bodyLocalPosition = m_EntitySpaceDesc.rootNode.transform.InverseTransformVector(objBody.transform.position);
		}
		else {
			bodyLocalPosition = originLocalPosition;
			bodyLocalPosition.y += 1f;
		}
    }

    public GameObject AttachRelativeToLocator(string resPath, GameObject parent, string locator, Vector3 offset)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.AttachRelativeToLocator"))
        {
#endif
        if(parent == null)
            return null;
        
        GameObject go = AssetLoader.instance.LoadResAsGameObject(resPath);
        if (go == null)
            return null;
        
        Battle.GeUtility.AttachTo(go, parent);
       
        GameObject objOverhead = Utility.FindThatChild("OverHead", m_EntitySpaceDesc.actorNode);
        if (objOverhead)
        {
            Vector3 localPos = objOverhead.transform.position;
            localPos += offset;
            localPos = parent.transform.InverseTransformPoint(localPos);
            go.transform.localPosition = localPos;
        }
        else
        {
            var localPos = new Vector3(0, 1.7f, 0);
            if (goInfoBar != null)
            {
                localPos = goInfoBar.transform.localPosition;
            }
            
            localPos += offset;
            go.transform.localPosition = localPos;
        }

        return go;
#if ENABLE_PROFILER
        }
#endif
    }


    public void ShowFindPath(bool show = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.ShowFindPath"))
        {
#endif
        if (goShowFindPath == null)
        {
            string EffectPath = "";

            var tableData = TableManager.GetInstance().GetTableItem<EffectInfoTable>(1035);
            if (tableData != null)
            {
                EffectPath = tableData.Path;
            }
            else
            {
                Logger.LogError("自动寻路任务头顶特效路径为空，特效信息表没填id=1035");
            }

            float offsetY = titleComponent != null && titleComponent.TitleID != 0 ? 1.1f : 0.8f;
            Vector3 offset = new Vector3(0, offsetY, 0);
            goShowFindPath = AttachRelativeToLocator(
                EffectPath,
                m_EntitySpaceDesc.rootNode,
                "OverHead",
                offset
            );

            GeMeshRenderManager.GetInstance().AddMeshObject(goShowFindPath);
        }

        if (null == goShowFindPath)
        {
            return;
        }

        if (goShowFindPath.activeSelf != show)
            goShowFindPath.SetActive(show);
#if ENABLE_PROFILER
        }
#endif
    }

    public void ShowHeadDialog(string text, bool hide = false,bool bLink = false,bool bUseLink = false, bool useSkill=false,float fLifeTime = 3.50f, bool isPet=false, int dialogType = -1)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.ShowHeadDialog"))
        {
#endif
            var type = _GetDialogType(bUseLink, useSkill);
            if(dialogType != -1)
            {
                type = _GetDialogType(dialogType);
            }
            var dialog = _GetDialogObj(type);
            
            if (dialog == null && hide)
                return;

            if (useCube)
                return;

            if (dialog == null)
            {
                _CreateDialog(bUseLink, useSkill, dialogType);
            }

            _SetDialog(text, hide, bLink, bUseLink, useSkill, fLifeTime, dialogType);
#if ENABLE_PROFILER
        }
#endif
    }

    //之前冒泡默认大多数是爆炸类型，所以默认用爆炸类型
    private DialogType _GetDialogType(int type)
    {
        DialogType dialogType = DialogType.None;
        switch (type)
        {
            case 0:
                dialogType =  DialogType.Explode;
                break;
            case 1:
                dialogType =  DialogType.Normal;
                break;
        }
        return dialogType;
    }

    private GameObject _CreateDialog(bool bUseLink = false, bool useSkill = false, int dialogType = -1)
    {
        var type = _GetDialogType(bUseLink, useSkill);
        if (dialogType != -1)
        {
            type = _GetDialogType(dialogType);
        }

        var path = _GetDialogPath(type);

        var dialog = AssetLoader.instance.LoadResAsGameObject(path);
        if(dialog == null)
        {
            return dialog;
        }
        Battle.GeUtility.AttachTo(dialog, m_EntitySpaceDesc.rootNode);

        Vector3 localPos = GetOverHeadPosition();
        if (entity != null && entity.GetEntityData() != null && entity.GetEntityData().overHeadHeight > 0f)
        {
            localPos.y = entity.GetEntityData().overHeadHeight;
        }

        localPos.y += HEAD_DIALOG_HEIGHT;
        dialog.transform.localPosition = localPos;

        var dialogScript = dialog.AddComponent<DialogScript>();
        dialogScript.Initialize(bUseLink);
        mDialogScripts[(int)type] = dialogScript;

        GeMeshRenderManager.GetInstance().AddMeshObject(dialog);

        return dialog;
    }

    private void _SetDialog(string text, bool hide, bool bLink, bool bUseLink, bool useSkill, float fLifeTime, int dialogType = -1)
    {
        var type = _GetDialogType(bUseLink, useSkill);
        if (dialogType != -1)
        {
            type = _GetDialogType(dialogType);
        }
        var dialogScript = _GetDialogScript(type);
        if (dialogScript != null)
        {
            if (hide)
                dialogScript.DoHide();
            else
            {
                _HideAllDialog();
                dialogScript.ShowText(text, bLink, fLifeTime);
            }
        }
    }

    private void _HideAllDialog()
    {
        for(int i = 0;i < mDialogScripts.Length; i++)
        {
            if(mDialogScripts[i] != null)
            {
                mDialogScripts[i].DoHide();
            }
        }
    }

    private DialogType _GetDialogType(bool bUseLink = false, bool useSkill = false)
    {
        //useSkill                                                  , useSkill
        //true                                                      , false
        //{ "UIFlatten/Prefabs/DialogParent/DialogParent_battle_skill", "UIFlatten/Prefabs/DialogParent/DialogParent"        },  // bUseLink = true
        //{ "UIFlatten/Prefabs/DialogParent/DialogParent_battle_skill", "UIFlatten/Prefabs/DialogParent/DialogParent_battle" },  // bUseLink = false
        if (useSkill)
        {
            return DialogType.Explode;
        }
        if(bUseLink)
        {
            return DialogType.Link;
        }
        return DialogType.Normal; 
    }
    
    private GameObject _GetDialogObj(DialogType type)
    {
        if ((int)type < 0)
        {
            return null;
        }
        
        if((int)type >= (int)DialogType.Count || (int)type >= mDialogs.Length)
        {
            return null;
        }
        return mDialogs[(int)type];
    }

    private DialogScript _GetDialogScript(DialogType type)
    {
        if ((int)type < 0)
        {
            return null;
        }
        if ((int)type >= (int)DialogType.Count || (int)type >= mDialogScripts.Length)
        {
            return null;
        }
        return mDialogScripts[(int)type];
    }

    private string _GetDialogPath(DialogType type)
    {
        if ((int)type < 0)
        {
            return string.Empty;
        }
        
        if ((int)type >= (int)DialogType.Count || (int)type >= mDialogPaths.Length)
        {
            return string.Empty;
        }
        return mDialogPaths[(int)type];
    }

    public void CreateFootIndicator(string effect = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateFootIndicator"))
        {
#endif
        string path = "UIFlatten/Prefabs/PlayerInfo/PlayerInfo_Foot";

        if (effect != null)
        {
            path = effect;
        }

        if(goFootInfo != null)
        {
            GameObject.Destroy(goFootInfo);
        }
            
        goFootInfo = AssetLoader.instance.LoadResAsGameObject(path);

        if(goFootInfo == null)
        {
            return;
        }

        if (mGoFootInfo != null)
        {
            GameObject.Destroy(mGoFootInfo);
            mGoFootInfo = null;
        }
            //Battle.GeUtility.AttachTo(goFootInfo, m_EntitySpaceDesc.rootNode);
            AttachToRootNode(goFootInfo, SceneUINodeType.ActorRoot_FootEffect);

        var pos = originLocalPosition;
        pos.y += 0.1f;
        goFootInfo.transform.localPosition = pos;

        GeMeshRenderManager.GetInstance().AddMeshObject(goFootInfo);
        //RefreshUIRenderOrder();
#if ENABLE_PROFILER
        }
#endif
    }
    GameObject mGoFootInfo = null;
    private void SetFontIndircatorInBackGround(bool isSet)
    {
        if (!isSet)
        {
            if (mGoFootInfo != null)
            {
                GameObject.Destroy(mGoFootInfo);
                mGoFootInfo = null;
            }
            return;
        }

        if (goFootInfo != null)
        {

            return;
        }
        string path = "Effects/Hero_Zhaohuanshi/Bingnaisi/Prefab/Eff_Zhaohuanbingnaisi_zhaohuan_02";
        if (mGoFootInfo != null)
        {
            GameObject.Destroy(mGoFootInfo);
        }
        mGoFootInfo = AssetLoader.instance.LoadResAsGameObject(path);
        if (mGoFootInfo == null)
        {
            return;
        }
        Battle.GeUtility.AttachTo(mGoFootInfo, m_EntitySpaceDesc.rootNode);
        var pos = originLocalPosition;
        pos.y += 0.1f;
        mGoFootInfo.transform.localPosition = pos;
        GeMeshRenderManager.GetInstance().AddMeshObject(mGoFootInfo);
    }
    public void SetFootIndicatorTouchGround(float y)
    {
        if (goFootInfo == null)
            return;

        Vector3 pos = goFootInfo.transform.localPosition;
        pos.y = -y;
        pos.y += 0.02f;
        goFootInfo.transform.localPosition = pos;
    }

    public void SetFootIndicatorVisible(bool flag)
    {
        if (goFootInfo == null)
            return;
        goFootInfo.SetActive(flag);
    }

    public void SetHeadInfoVisible(bool flag)
    {
        if (goInfoBar == null)
            return;

        goInfoBar.SetActive(flag);
    }

    public void CreateMonsterLoop()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateMonsterLoop"))
        {
#endif
        goFootInfo = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/BOSS/Prefab/Eff_BOSS_GH");
        if (goFootInfo != null)
        {
            Battle.GeUtility.AttachTo(goFootInfo, m_EntitySpaceDesc.rootNode);
        }
#if ENABLE_PROFILER
        }
#endif
    }

	public void CreateMonsterInfoBar(string name, PlayerInfoColor infoColor)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateMonsterInfoBar"))
        {
#endif
        string path = "UIFlatten/Prefabs/PlayerInfo/MonsterInfo_Head";
        if (goInfoBar == null)
        {
            goInfoBar = CGameObjectPool.instance.GetGameObject(path,enResourceType.BattleScene,(uint)GameObjectPoolFlag.ReserveLast);
            //Battle.GeUtility.AttachTo(goInfoBar, m_EntitySpaceDesc.rootNode);
            AttachToRootNode(goInfoBar, SceneUINodeType.ActorRoot_InfoHead);

            _updateChapterHeadInfoBarPosition();

            GeMeshRenderManager.GetInstance().AddMeshObject(goInfoBar);
        }

        if (null == goInfoBar)
        {
            return ;
        }

        GameObject textCompGo = Utility.FindChild(goInfoBar, "Name");

        if (null == textCompGo)
        {
            return ;
        }

        Text textComp = textCompGo.GetComponent<Text>();
        if (null == textComp)
        {
            return ;
        }

        textComp.text = name;

        CPlayerInfo info = goInfoBar.GetComponent<CPlayerInfo>();

        if (null == info)
        {
            return ;
        }

        textComp.color = info.GetColor(infoColor);
#if ENABLE_PROFILER
        }
#endif
    }

	public void SetTaskMonster(string name)
	{
		if (goInfoBar == null)
		{
			CreateMonsterInfoBar(name, PlayerInfoColor.TOWN_OTHER_PLAYER);
		}
		if (goInfoBar != null){
			GameObject goTaskIndicator = Utility.FindChild(goInfoBar, "Name/task_indicate");
			if (goTaskIndicator != null)
				goTaskIndicator.SetActive(true);
		}
	}

	/*
	  SPECIAL_ATTACK
		BUFF_TEXT,		//buff文字
		GET_EXP,
		GET_GOLD
	*/
	public GameObject CreateHeadText(HitTextType type, object arg=null, bool noattach=false, object arg2=null)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateHeadText"))
        {
#endif
		string path = "";

		string key = null;

		if (type == HitTextType.SPECIAL_ATTACK)
		{
			path = "UIFlatten/Prefabs/Battle_Digit/PlayerInfo_SpecialAttack";
            if (arg2 != null)
            {
                string newPath = arg2 as string;
                if (newPath != null)
                    path = newPath;
            }
                
			string param = arg as string;
			key = param;
		}
		else if (type == HitTextType.SKILL_CANNOTUSE)
		{
			path = "UIFlatten/Prefabs/Battle_Digit/PlayerInfo_SpecialAttack";
			string param = arg as string;
			key = param;
		}
		else if (type == HitTextType.BUFF_TEXT)
		{
			path = "UIFlatten/Prefabs/Battle_Digit/PlayerInfo_BuffName";
			key = "BUFF_TEXT";
		}
		else if (type == HitTextType.GET_EXP)
		{
			path = "UIFlatten/Prefabs/Battle_Digit/PlayerInfo_GetExp";
			key = "GET_EXP";
		}
		else if (type == HitTextType.GET_GOLD)
		{
			path = "UIFlatten/Prefabs/Battle_Digit/PlayerInfo_GetGold";
			key = "GET_GOLD";
		}


		if ((type == HitTextType.SPECIAL_ATTACK || type == HitTextType.SKILL_CANNOTUSE) && headTextCount.ContainsKey(key) && headTextCount[key] > 0)
		{
				return null;
		}

		if (!headTextCount.ContainsKey(key))
			headTextCount.Add(key, 0);

		headTextCount[key]++;


		float offset = (headTextCount[key] - 1) * 0.3f;

		string tmpKey = null;
		if (type == HitTextType.SPECIAL_ATTACK || type == HitTextType.SKILL_CANNOTUSE)
		{
			tmpKey = (type == HitTextType.SPECIAL_ATTACK)?"SPECIAL_ATTACK":"SKILL_CANNOTUSE";
			if (!headTextCount.ContainsKey(tmpKey))
				headTextCount.Add(tmpKey, 0);
			headTextCount[tmpKey]++;
			offset = (headTextCount[tmpKey] - 1) * 0.4f;
		}

		if (entity != null)
		{
			entity.delayCaller.DelayCall(1000, ()=>{
				headTextCount[key]--;
				if (tmpKey != null)
					headTextCount[tmpKey]--;
			});
		}

		GameObject go = null;
		if (path.Length > 1)
		{
			go = CGameObjectPool.instance.GetGameObject(path, enResourceType.BattleScene,(uint)GameObjectPoolFlag.ReserveLast);
			if (go != null)
			{
				ClientSystemManager.GetInstance().delayCaller.DelayCall(1300, ()=>{
					CGameObjectPool.instance.RecycleGameObject(go);
				}, 0, 0, true);
			}
		}

		if (go == null)
			return go;
			
		if ((type == HitTextType.SPECIAL_ATTACK || 
			type == HitTextType.BUFF_TEXT || 
			type == HitTextType.SKILL_CANNOTUSE) && go != null)
		{
			string param = arg as string;
			Sprite tmpSprite = AssetLoader.instance.LoadRes(param, typeof(Sprite)).obj as Sprite;

			if (path != null && tmpSprite != null)
			{
				GameObject objText = Utility.FindGameObject(go, "root/text", false);
				if (objText != null)
				{
					Image image = objText.GetComponent<Image>();
                    // image.sprite = tmpSprite;
                    ETCImageLoader.LoadSprite(ref image, param);
					image.SetNativeSize();

                    // TODO::PlayerInfo_SpecialAttack Fade组件问题临时处理.
                    if (type == HitTextType.SPECIAL_ATTACK || type == HitTextType.SKILL_CANNOTUSE)
                    {
                        var color = image.color;
                        color.a = 1.0f;
                        image.color = color;
                    }
                }
			}
				
		}
		else if (type == HitTextType.GET_EXP || type == HitTextType.GET_GOLD)
		{
			int param = (int)arg;
			GameObject objText = Utility.FindThatChild("Text", go); 
			if (objText != null)
			{
				Text text = objText.GetComponent<Text>();
				text.text = "+" + param;
			}

			if (type == HitTextType.GET_GOLD)
			{
				string n = arg2 as string;
				if (n != null)
				{
					GameObject objText2 = Utility.FindThatChild("Text (1)", go); 
					if (objText2 != null)
					{
						Text text = objText2.GetComponent<Text>();
						text.text = n;
					}
				}
			}
		}


		if (noattach)
		{
			go.transform.position = m_EntitySpaceDesc.rootNode.transform.position;

			return go;
		}
			
		Battle.GeUtility.AttachTo(go, m_EntitySpaceDesc.rootNode);

		if (type == HitTextType.SKILL_CANNOTUSE)
		{
			var pos = bodyLocalPosition;
			pos.z = -1f;
			go.transform.localPosition = pos;
		}
		else {
			if (objBuffOrigin != null)
			{
				Vector3 localPos = buffOriginPosition;
				localPos.y += offset;
				localPos.x = 0f;
				localPos.z = -0.1f;
				localPos = m_EntitySpaceDesc.rootNode.transform.InverseTransformVector(localPos);
				go.transform.localPosition = localPos;
			}
			else if (objOverhead != null)
			{
				Vector3 localPos = Vector3.zero;
				localPos.y += 0.35f;
				localPos.x = 0f;
				localPos.z = -0.1f;
				localPos = m_EntitySpaceDesc.rootNode.transform.InverseTransformVector(localPos);
				go.transform.localPosition = localPos;
			}
			else
			{
				var localPos = go.transform.localPosition;
				localPos.y += 2.0f;
				go.transform.localPosition = localPos;
			}
		}

		return go;
#if ENABLE_PROFILER
        }
#endif
	}



    public void CreateInfoBar(string name, PlayerInfoColor infoColor, ushort RoleLevel, string namecolors = null,float nameLocalPosY = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateInfoBar"))
        {
#endif
        mPlayerInfoBarData = new PlayerInfoBarData( name, infoColor, RoleLevel, namecolors,nameLocalPosY);
        var gameObject = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/PlayerInfo/PlayerInfo_Head");
        _onCreateInfoBarCallBack(gameObject);
        //mPlayerInfoBarDataHandle = _addAsyncLoadGameObject("UIFlatten/Prefabs/PlayerInfo/PlayerInfo_Head", enResourceType.UIPrefab, true, _onCreateInfoBarCallBack, mPostLoadTownNPCHandle);
        
#if ENABLE_PROFILER
        }
#endif
    }

   

    public void ShowTranport(bool flag)
    {
        if (showTransport == flag)
            return;

        showTransport = flag;

        if (goTransportInfo != null)
            goTransportInfo.CustomActive(showTransport);
    }

    public void CreatePropertyRiseEffect(string content)
    {
        string path = "UIFlatten/Prefabs/CommonSystemNotify/CommonFloatingEffectByPos";

        GameObject riseEffect = AssetLoader.instance.LoadResAsGameObject(path);
        Utility.AttachTo(riseEffect, goInfoBar);

        GameObject textContentObj = Utility.FindChild(riseEffect, "Text");
        if (null != textContentObj)
        {
            Text textContent = textContentObj.GetComponent<Text>();
            if (textContent != null)
            {
                textContent.text = content;
            }
        }
       
        GeMeshRenderManager.GetInstance().AddMeshObject(riseEffect);
    }

    public void UpdateLevel(int iLevel)
    {
        if (m_kLevelText != null)
        {
            if (iLevel > 0)
            {
                m_kLevelText.text = "Lv." + iLevel.ToString();
                m_kLevelText.CustomActive(true);
            }
            else
            {
                m_kLevelText.text = "";
                m_kLevelText.CustomActive(false);
            }
        }
    }

    public void UpdateDialogComponet(float delta)
    {
        if(dialogComponent != null)
        {
            dialogComponent.Tick(delta);
        }
    }

    public void UpdateNpcInteraction(float delta)
    {
        if (null != goInfoBar)
        {
            NpcInteraction comNpcInteraction = goInfoBar.GetComponent<NpcInteraction>();

            if (comNpcInteraction != null)
            {
                comNpcInteraction.Tick();
            }
        }
    }

    public void UpdateVoiceComponent(float delta)
    {
        if (voiceComponent != null)
        {
            voiceComponent.Tick(delta);
        }
    }

    public void AddVoiceListener(NpcVoiceComponent.SoundEffectType eSoundEffect)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.AddVoiceListener"))
        {
#endif
        if (voiceComponent != null)
        {
            voiceComponent.PlaySound(eSoundEffect);
        }
#if ENABLE_PROFILER
        }
#endif
    }



    

    public void AddComponentDialog(System.Int32 iDialogID = 2006, NpcDialogComponent.IdBelong2 eIdBelong2 = NpcDialogComponent.IdBelong2.IdBelong2_NpcTable)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.AddComponentDialog"))
        {
#endif
        if (m_EntitySpaceDesc.rootNode != null && dialogComponent == null)
        {
            Transform goParent = m_EntitySpaceDesc.rootNode.transform.Find("DialogParent");
            if (goParent == null)
            {
                string path = "UIFlatten/Prefabs/DialogParent/DialogParent";
                GameObject goComponetDialog = CGameObjectPool.instance.GetGameObject(path, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);

                mDialogComponentData = new NpcDialogComponentData(iDialogID, eIdBelong2);
                _onAddComponetDialog(goComponetDialog);
                //_addAsyncLoadGameObject(path, _onAddComponetDialog, mPlayerInfoBarDataHandle);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void AddNPCFunction(string name, PlayerInfoColor infoColor, ushort RoleLevel, string namecolors = null,float nameLocalPosY = 0.0f)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.AddNPCFunction"))
        {
#endif
        mPlayerInfoBarData = new PlayerInfoBarData(name ?? "", infoColor, RoleLevel, namecolors, nameLocalPosY);
        GameObject goHeadInfo = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/PlayerInfo/PlayerInfo_Head", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);

        _onCreateInfoBar(goHeadInfo);
        _onAddNPCInteraction();
        _onAddTittleComponent();
        //goHeadInfo.transform.localPosition = new Vector3(0, 2.0f, 0);
#if ENABLE_PROFILER
        }
#endif
    }

    public void _RefleshUIComponent()
    {
        if(null != goInfoBar)
        {
            Vector3 localPos = buffOriginLocalPosition;
            localPos.y -= 0.25f;
            SetGoInfoBarLocalPosition(localPos);
        }
        if (entity == null) return;
        //entity.delayCaller.DelayCall(50, () =>
        //{
            if (mBarsRoot != null && entity != null && entity.GetEntityData() != null)
            {
                _RefreshOverHeadPostion();
                var localPos = overheadLocalPosition;
                if (entity.GetEntityData() != null && entity.GetEntityData().overHeadHeight > 0f)
                    localPos.y = entity.GetEntityData().overHeadHeight;
                localPos.y += 0.78f;
                mBarsRoot.gameObject.transform.localPosition = localPos;
            }
        //});
    }

    public void AddNPCBoxCollider(BeTownNPCData townData)
    {
        mNPCBoxCollectData = new NPCBoxCollectData(townData);
        //GameFrameWork.instance.StartCoroutine(_onAddNPCBoxColliderIter());
    }

    //lprivate IEnumerator _onAddNPCBoxColliderIter()
    //{
    //    yield return _waitPlayerInfoHeadLoadFinish();

    //    _onAddNPCBoxCollider();
    //}



    

    public void AddNpcInteraction(int npcID,UInt64 guid = 0)
    {
        mNpcInteractionData = new NpcInteractionData(npcID, guid);
        //GameFrameWork.instance.StartCoroutine(_onAddNPCInteractionIter());
    }

    //private IEnumerator _onAddNPCInteractionIter()
    //{
    //    yield return _waitPlayerInfoHeadLoadFinish();

    //    _onAddNPCInteraction();
    //}

   


    

    public void AddTittleComponent(System.Int32 iTittleID,
        string name,
        byte guildDuty,
        string bangName,
        System.Int32 iRoleLv,
        int a_nPKRank,
        PlayerInfoColor color,
        string adventTeamName = "",
        PlayerWearedTitleInfo   playerWearedTitleInfo=null,
        int guileLv = 0,
        int iVipLevel = 0)
    {
        mTittleComponentData = new TittleComponentData(iTittleID, name, guildDuty, bangName, iRoleLv, a_nPKRank, color, adventTeamName, playerWearedTitleInfo, guileLv,iVipLevel);
        //GameFrameWork.instance.StartCoroutine(_onAddTittleComponentIter());
    }

    //private IEnumerator _onAddTittleComponentIter()
    //{
    //    //yield return _waitPlayerInfoHeadLoadFinish();
    //    _onAddTittleComponent();
    //    yield return null;
    //}

    

    public void OnLevelChanged(System.Int32 iRoleLv)
    {
        if (titleComponent != null)
        {
            titleComponent.Level = iRoleLv;
        }
    }

    public void OnTittleChanged(System.Int32 iTittle)
    {
        if (titleComponent != null)
        {
            titleComponent.TitleID = iTittle;
        }
    }

    public void OnGuildNameChanged(string name)
    {
        if (titleComponent != null)
        {
            titleComponent.GangName = name;
        }
    }

    public void OnGuildPostChanged(byte duty)
    {
        if (titleComponent != null)
        {
            titleComponent.GangDuty = duty;
        }
    }

    public void UpdatePkRank(int a_nPKRank, int a_nStar)
    {
        if (titleComponent != null)
        {
            titleComponent.PKRank = a_nPKRank;
            // TODO 最强王者星级显示

        }
    }

    public void UpdateName(string a_strName)
    {
        if (titleComponent != null)
        {
            titleComponent.Name = a_strName;
        }
    }

    public void UpdateAdventTeamName(string a_strName)
    {
        if(titleComponent != null)
        {
            titleComponent.AdventTeamName = a_strName;
        }
    }

    public void UpdateTitleName(Protocol.PlayerWearedTitleInfo playerWearedTitleInfoe)
    {
        if(titleComponent != null)
        {
            titleComponent.PlayerWearedTitleInfo = playerWearedTitleInfoe;
        }
    }

    public void UpdateGuidLv(int guildLv)
    {
        if(titleComponent != null)
        {
            titleComponent.GuildEmblemLv = guildLv;
        }
    }

    public void AddNpcVoiceComponent(System.Int32 iNpcID)
    {
        if (m_EntitySpaceDesc.rootNode != null && voiceComponent == null)
        {
            Transform goParent = m_EntitySpaceDesc.rootNode.transform.Find("VoiceParent");
            if (goParent == null)
            {
                GameObject goVoiceParent = new GameObject("VoiceParent");
                Battle.GeUtility.AttachTo(goVoiceParent, m_EntitySpaceDesc.rootNode);
                goParent = goVoiceParent.transform;
            }

            if (goParent != null)
            {
                voiceComponent = goParent.GetComponent<NpcVoiceComponent>();
                if (voiceComponent == null)
                {
                    voiceComponent = goParent.gameObject.AddComponent<NpcVoiceComponent>();
                }

                if (voiceComponent != null)
                {
                    voiceComponent.Initialize(iNpcID);
                }
            }
        }
    }

    public void AddNpcArrowComponent()
    {
        if (m_EntitySpaceDesc.characterNode != null && npcArrowComponent == null)
        {
            var goArrow = Utility.FindChild(m_EntitySpaceDesc.characterNode, "ArrowComponent");
            if (goArrow == null)
            {
                var gameObject = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/TownUI/NpcWaitArrow");
                _onAddNpcArrowComponent(gameObject);
                //_addAsyncLoadGameObject("UIFlatten/Prefabs/TownUI/NpcWaitArrow", _onAddNpcArrowComponent, mPostLoadTownNPCHandle);
                //GameFrameWork.instance.StartCoroutine(_asyncLoadGameObject("UIFlatten/Prefabs/TownUI/NpcWaitArrow", _onAddNpcArrowComponent));
            }
        }
    }



    public void ActiveArrow()
    {
        if (npcArrowComponent != null)
        {
            npcArrowComponent.Active();
        }
    }

    public void DeActiveArrow()
    {
        if (npcArrowComponent != null)
        {
            npcArrowComponent.DeActive();
        }
    }

    public void UpdateInfoBarLevel(ushort RoleLevel, bool force = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.UpdateInfoBarLevel"))
        {
#endif
            if (goInfoBar != null)
            {
                GameObject goInfoBarLv = Utility.FindChild(goInfoBar, stringLevelPath);
                Text Lv = null;

                if (null != goInfoBarLv)
                {
                    Lv = goInfoBarLv.GetComponent<Text>();
                }

                if (null == Lv)
                {
                    return ;
                }

                string lvlString = "";
                if (Lv != null && RoleLevel > 0)
                {
                    lvlString = "Lv." + RoleLevel.ToString();
                }

                if (lvlString != Lv.text || force)
                {
                    Lv.text = lvlString;

                    int effectInfoId = 3;
                    var pos = GetPosition();
                    CreateEffect(effectInfoId, new Vec3(pos.x, pos.z, pos.y), true);

				    AudioManager.instance.PlaySound(8);
                    ResetHPBar();
                }

                UpdateHPBar(RoleLevel);
            }
#if ENABLE_PROFILER
        }
#endif
    }

    public void PlayAwakeEffect()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.PlayAwakeEffect"))
        {
#endif
            int effectInfoId = 3;
            var pos = GetPosition();
            CreateEffect(effectInfoId, new Vec3(pos.x, pos.z, pos.y), true);

            AudioManager.instance.PlaySound(8);
#if ENABLE_PROFILER
        }
#endif
    }

    public void UpdateInfoBarPKPoints(uint pkPoints)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.UpdateInfoBarPKPoints"))
        {
#endif
        if (goInfoBar != null)
        {
			var go = Utility.FindChild(goInfoBar, "pkGrades");
			if (go == null)
				return;
			
			Image Grade = go.GetComponent<Image>();
            if (Grade != null && pkPoints > 0)
            {
                int RemainPoints = 0;
                int TotalPoints = 0;
                int pkIndex = 0;
                bool bMaxLv = false;

                string PkPath = Utility.GetPathByPkPoints(pkPoints, ref RemainPoints, ref TotalPoints, ref pkIndex, ref bMaxLv);

                if (PkPath != "" && PkPath != "-" && PkPath != "0")
                {
                    // Grade.sprite = AssetLoader.instance.LoadRes(PkPath, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref Grade, PkPath);
                    //Grade.sprite = Sprite.Create(Icon, new Rect(0, 0, Icon.width, Icon.height), new Vector2(0.5f, 0.5f));
                }
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

	public void CreateOverHeadHpBar(eHpBarType type, bool enemy=true, bool isShow = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateOverHeadHpBar"))
        {
#endif
        string path = "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonHpBar";
        if (type != eHpBarType.Player)
		{
			if (entity != null)
			{
				if (/*entity.m_iCamp <= 0*/!enemy)
				{
					path = "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonMonsterHpBar_Green";
					isShow = true;
				}
				else
					path = "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonMonsterHpBar";
			}
		}
            
        //goHPBar = AssetLoader.instance.LoadResAsGameObject(path);
        goHPBar = CGameObjectPool.instance.GetGameObject(path,enResourceType.BattleScene,(uint)GameObjectPoolFlag.ReserveLast);
        //Battle.GeUtility.AttachTo(goHPBar, m_EntitySpaceDesc.rootNode);
        AttachToRootNode(goHPBar, SceneUINodeType.ActorRoot_DungeonHpBar);

        _updateHPHeadBarPostion();

        var com = goHPBar.GetComponent<ComCharactorHeadHPBar>();

        mCurHpHeadBar = com;
		if (!enemy)
		{
			com.mType = eHpBarType.player_Monster; 
			isShow = true;
		}
		else 
		{
			com.mType = type;
			isShow = false;
		}
			
        hpBarManager.AddHpBar(com, isShow);

        var data = entity.GetEntityData();
        if (data != null)
        {
            com.Init(data.battleData.maxHp, data.battleData.maxMp);
        }
			
        GeMeshRenderManager.GetInstance().AddMeshObject(goHPBar);
#if ENABLE_PROFILER
        }
#endif
    }

    private void _updateHPHeadBarPostion()
    {
        if (null == goHPBar)
        {
            return ;
        }

		Vector3 localPos = GetOverHeadPosition();
		if (entity.GetEntityData()!=null && entity.GetEntityData().overHeadHeight > 0f)
			localPos.y = entity.GetEntityData().overHeadHeight;
		
		localPos.y += HP_BAR_HEIGHT;
        //避免怪物使用Cube创建时头顶名字显示在脚底或者场景下方的BUG
        if (localPos.y <= 0)
        {
            localPos.y = 2.0f;
        }

        //避免怪物使用Cube创建时头顶名字显示在脚底或者场景下方的BUG
        if (useCube)
        {
            localPos.y = 2.0f;
        }
		goHPBar.transform.localPosition = localPos;
    }

    public void CreateSpellBar()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateSpellBar"))
        {
#endif
        if (goSpellBar == null)
        {
            string path = "UI/Prefabs/HPBar_Head";
            goSpellBar = AssetLoader.instance.LoadResAsGameObject(path);
            Battle.GeUtility.AttachTo(goSpellBar, m_EntitySpaceDesc.rootNode);


			Vector3 localPos = GetOverHeadPosition();
			if (entity.GetEntityData()!=null && entity.GetEntityData().overHeadHeight > 0f)
				localPos.y = entity.GetEntityData().overHeadHeight;
			
			localPos.y += SPELL_BAR_HEIGHT;
			goSpellBar.transform.localPosition = localPos;

            cSpellBar = goSpellBar.GetComponent<CHPBar>();
            goSpellBar.SetActive(false);
        }

        goSpellBar.SetActive(true);

        GeMeshRenderManager.GetInstance().AddMeshObject(goSpellBar);
#if ENABLE_PROFILER
        }
#endif
    }

    public void SetSpellBar(float progress)
    {
        if (goSpellBar != null && cSpellBar != null)
        {
            progress = Mathf.Clamp(progress, 0, 1f);
            cSpellBar.SetPercent(progress);
        }
    }

    public void StopSpellBar()
    {
        if (goSpellBar != null)
        {
            goSpellBar.SetActive(false);
        }
    }

    public void CreateStateBar(string text, int duration)
    {
        if (stateBarManager != null)
        {
            stateBarManager.CreateStateBar();
            mCurrentStateBarId = stateBarManager.AddStateBar(text, duration);
            if (this == stateBarManager.currentActor)
            {
                stateBarManager.SetBarActive(true);
            }
        }
    }

    public void RemoveStateBar()
    {
        if (stateBarManager != null)
        {
            stateBarManager.RemoveStateBar(mCurrentStateBarId);
        }
    }

    //private GameObject _getStateBarRoot()
    //{
    //    GameClient.ClientSystemBattle system = GameClient.ClientSystemManager.GetInstance().TargetSystem as GameClient.ClientSystemBattle;

    //    if (null == system)
    //    {
    //        system = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemBattle;
    //    }

    //    if (null == system)
    //    {
    //        return null;
    //    }

    //    return system.MonsterBossRoot;
    //}


    public void CreateComboTips(int[] skills)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateComboTips"))
        {
#endif
        _createBarRoot();

        if (null != mBarsRoot)
        {
            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonComboTips");
            mTips = go.GetComponent<ComDungeonComboTips>();
            mTips.SetSkills(skills);
            mTips.BindEvent();
            Utility.AttachTo(go, mBarsRoot.hRoot);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void RemoveComboTips()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.RemoveComboTips"))
        {
#endif
        if (null != mTips)
        {
            mTips.UnbindEvent();
            GameObject.Destroy(mTips.gameObject);
            mTips = null;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public IDungeonCharactorBar CreateBar(eDungeonCharactorBar type)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateBar"))
        {
#endif
        _createBarRoot();

        if (mBarsRoot != null)
        {
            var bar = _GetBarByType(type);
            if (null == bar)
            {
                var path = _getBarPath(type);
                var go = AssetLoader.GetInstance().LoadResAsGameObject(path);
                bar = _attachBarRoot(type, go);
                mBars.Add(bar);
            }

            bar.Show(true);

            return bar;
        }

        return null;
#if ENABLE_PROFILER
        }
#endif
    }

    protected IDungeonCharactorBar _GetBarByType(eDungeonCharactorBar type)
    {
        for (int i = 0, icnt = mBars.Count; i < icnt; ++i)
        {
            IDungeonCharactorBar curBar = mBars[i];
            if (curBar.GetBarType() == type)
                return curBar;
        }

        return null;
    }

	public void SetBar(eDungeonCharactorBar type, float rate, bool show=true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.SetBar"))
        {
#endif
        IDungeonCharactorBar bar = _GetBarByType(type);

        if (null != bar)
        {
			bar.Show(show);
            bar.SetRate(rate);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void SetCdTime(eDungeonCharactorBar type, float cdTime, bool show = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.SetCdTime"))
        {
#endif

        IDungeonCharactorBar bar = _GetBarByType(type);
        if (null != bar)
        {
            bar.Show(show);
            bar.SetCdText(cdTime);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    

    public void StopBar(eDungeonCharactorBar type, bool iscancel)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.StopBar"))
        {
#endif

        IDungeonCharactorBar bar = _GetBarByType(type);

        if (null != bar)
        {
            bar.Show(false);

            if (!iscancel)
            {
                _stopBarEffect(type);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    // TODO use the IHPBar
	public void CreatePKHPBar(CPKHPBar.PKBarType type, string name, PlayerInfoColor color)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreatePKHPBar"))
        {
#endif
        GameClient.ClientSystemBattle system = GameClient.ClientSystemManager.GetInstance().TargetSystem as GameClient.ClientSystemBattle;

        if (system == null)
        {
            system = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemBattle;
        }
        string buffGroupPath = "";
        if (null != system)
        {
            string path     = "";
            GameObject root = null;

            switch (type)
            {
                case CPKHPBar.PKBarType.Right:
                    buffGroupPath = "UIFlatten/Prefabs/Battle/Bars/Charactor/BuffIconContentRight";
                    if (BattleMain.battleType==BattleType.ScufflePVP)
                    {
                        path = "UIFlatten/Prefabs/Battle/Bars/Charactor/PK/PlayerRightScufflePKHPBar";
                        root = system.Pvp3v3RightHpBarRoot;
                    }
                    else
                    {
                        path = "UIFlatten/Prefabs/Battle/Bars/Charactor/PK/PlayerRightPKHPBar";
                        root = system.PlayerPKRightRoot;
                    }
                    break;
                case CPKHPBar.PKBarType.Left:
                    buffGroupPath = "UIFlatten/Prefabs/Battle/Bars/Charactor/BuffIconContentLeft";
                    if (BattleMain.battleType == BattleType.ScufflePVP)
                    {
                        path = "UIFlatten/Prefabs/Battle/Bars/Charactor/PK/PlayerLeftScufflePKHPBar";
                        root = system.Pvp3v3LeftHpBarRoot;
                    }
                    else
                    {
                        path = "UIFlatten/Prefabs/Battle/Bars/Charactor/PK/PlayerLeftPKHPBar";
                        root = system.PlayerPKLeftRoot;
                    }
                    break;
            }

            GameObject obj = AssetLoader.instance.LoadResAsGameObject(path);
            Utility.AttachTo(obj, root);

            var com = obj.GetComponent<CPKHPBar>();
            com.type = type;
            com.SetHPPercent(1.0f);
            com.SetMPPercent(1.0f);
            entity.delayCaller.DelayCall(10, ()=>{
                com.SetHPValue(entity.GetEntityData().GetHP(), entity.GetEntityData().GetMaxHP());	
            });
            com.action = () =>
            {               
                BeActor actor = entity as BeActor;                             
                if (actor != null)
                {
                    if(BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor.IsDead())
                     actor.CurrentBeScene.currentGeScene.AttachCameraTo(actor.m_pkGeActor);
                }
            };
            com.SetNameText(name, "");
            com.SetIcon(entityHeadIcon, entityHeadIconMaterial);
            var mainPlayer = entity as BeActor;
            if (mainPlayer != null &&
                BattleMain.instance != null &&
                BattleMain.instance.GetPlayerManager() != null &&
                BattleMain.instance.GetPlayerManager().GetMainPlayer() != null &&
                BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor == mainPlayer)  //策划需求 pvp只显示左侧己方玩家buff
            {
                var mBind = obj.GetComponent<ComCommonBind>();
                if (mBind != null)
                {
                    var buffRoot = mBind.GetGameObject("buffRoot");
                    if (buffRoot != null)
                    {
                        var buffIconGroup = AssetLoader.instance.LoadResAsGameObject(buffGroupPath);
                        Utility.AttachTo(buffIconGroup, buffRoot);
                        var buffMono = buffIconGroup.GetComponent<ComDungeonBuffGroup>();
                        if (buffMono != null)
                        {
                                buffMono.InitBattleBuff(entity as BeActor, BattleMain.instance.GetPlayerManager().GetMainPlayer().GetPlayerSeat());
                        }
                    }
                }
            }
            if (goInfoBar != null)
            {
                //CPlayerInfo comCPlayerInfo = goInfoBar.GetComponent<CPlayerInfo>();
                //com.nameText.color = comCPlayerInfo.GetColor(color);
            }
            goHPPKBar = obj;
            comHPPKBar = com;
            GeMeshRenderManager.GetInstance().AddMeshObject(goHPPKBar);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void CreateHPBarCharactor(int seat = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateHPBarCharactor"))
        {
#endif
        GameClient.ClientSystemBattle system = GameClient.ClientSystemManager.GetInstance().TargetSystem as GameClient.ClientSystemBattle;

        if (system == null)
        {
            system = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemBattle;
        }

        if (system != null)
        {
            var data = entity.GetEntityData();
            if (data != null)
            {
                //GameObject goHPBarHUD;
                // TODO
                // 这里的UI挂点在System的主UI上
                // 在ClientSystem调用OnDestry的回调的时候，主UI对应的GameObject已经被销毁
                // 如果在OnDestroy里面尝试销毁动态创建出来的资源，可能会爆NullReference的错误
                if (seat == ClientApplication.playerinfo.seat)
                {
                    goHPBarHUD = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle/Bars/Charactor/PlayerSelfHPBar");
                    Utility.AttachTo(goHPBarHUD, system.PlayerSelfInfoRoot);

                    goHPBarHUD.transform.SetAsFirstSibling();
                }
                else
                {
                    goHPBarHUD = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle/Bars/Charactor/PlayerOtherHPBar");
                    Utility.AttachTo(goHPBarHUD, system.PlayerOtherInfoRoot);

                    goHPBarHUD.transform.SetAsLastSibling();
                }

                PlayerLabelInfo playerLabelInfo = null;
                if (entity != null && entity.CurrentBeBattle != null && entity.CurrentBeBattle.dungeonPlayerManager != null)
                {
                    var battlePlayer = entity.CurrentBeBattle.dungeonPlayerManager.GetPlayerBySeat((byte)seat);
                    if (battlePlayer != null && battlePlayer.playerInfo != null)
                    {
                        playerLabelInfo = battlePlayer.playerInfo.playerLabelInfo;
                    }
                }


                var com = goHPBarHUD.GetComponent<ComCharactorHPBar>();
                com.Init(data.battleData.maxHp, data.battleData.maxMp,1, data.GetResistMagic());
                com.SetIcon(entityHeadIcon, entityHeadIconMaterial);
                com.SetName(data.name, data.level);
                com.SetHidden(false);
                com.SetSeat((byte)seat);
                com.SetHeadPortraitFrame(playerLabelInfo);
                var mBind = goHPBarHUD.GetComponent<ComCommonBind>();
                if(mBind != null)
                {
                    var buffRoot = mBind.GetGameObject("buffRoot");
                    if(buffRoot != null)
                    {
                        var buffIconGroup = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle/Bars/Charactor/BuffIconContentLeft");
                        Utility.AttachTo(buffIconGroup, buffRoot);
                        var buffMono = buffIconGroup.GetComponent<ComDungeonBuffGroup>();
                        if(buffMono != null)
                        {
                                buffMono.InitBattleBuff(entity as BeActor, (byte)seat);
                        }
                    }
                }
                com.InitResistMagic(data.GetResistMagic(), entity as BeActor);

                mCurHpBar = com;
                hpBarManager.AddHpBar(mCurHpBar, true);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

	public void RemoveHPBarMonster()
	{
		mCurHpBar = null;
	}

    public void CreateHPBarMonster(ProtoTable.UnitTable.eType type, string name, Color nameColor, int singleBarValue = -1, bool enemy = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateHPBarMonster"))
        {
#endif
        eHpBarType hpBarType = eHpBarType.Monster;
        switch (type)
        {
            case ProtoTable.UnitTable.eType.BOSS:
                hpBarType = eHpBarType.Boss;
                break;
            case ProtoTable.UnitTable.eType.ELITE:
                hpBarType = eHpBarType.Elite;
                break;
            default:
                hpBarType = eHpBarType.Monster;
                break;
        }

        CreateOverHeadHpBar(hpBarType, enemy);

        if (enemy)
        {
            if (!BattleMain.IsModePvP(BattleMain.battleType) && BattleMain.battleType != BattleType.TrainingSkillCombo)
            {
                mCurrentHpBarId = hpBarManager.AddHpBar(entity, hpBarType, name, singleBarValue, entityHeadIcon, entityHeadIconMaterial);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void SetDebugDrawData(BDEntityActionFrameData data, float scale = 1.0f, float zDimScale=1.0f)
    {

		if (m_drawBox == null && m_EntitySpaceDesc.characterNode != null)
        {
            m_drawBox = m_EntitySpaceDesc.characterNode.AddComponent<Helper_DrawBox>();
        }

        //data等于空也设，表示不画了
        if (m_drawBox != null)
        {
            m_drawBox.SetFrameData(data, scale, zDimScale);
        }
    }

    public void ResetHPBar()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.ResetHPBar"))
        {
#endif
        if (entity != null)
        {
            //if (entity.m_iCamp == 0)
            {
                var barCnt = -1;

                var data = entity.GetEntityData();

                if (null != data)
                {
                    if (data.type == (int)ProtoTable.UnitTable.eType.BOSS)
                    {
                        if (null != entity.CurrentBeScene)
                        {
                            barCnt = entity.CurrentBeScene.singleBloodBarCount;
                        }
                    }
                }

                if (HPBarManager.kInvalidHpBarId != mCurrentHpBarId)
                {
                    hpBarManager.ResetHpBar(mCurrentHpBarId);
                }

                if (mCurHpBar != null)
                {
                    mCurHpBar.Init(entity.attribute.battleData.maxHp, entity.attribute.battleData.maxMp, barCnt, entity.attribute.GetResistMagic());
                }

                if (mCurHpHeadBar != null)
                {
                    mCurHpHeadBar.Init(entity.attribute.battleData.maxHp, entity.attribute.battleData.maxMp, barCnt);
                }
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }
		
    public void UpdateHPBar(int level)
    {
        if (entity != null)
        {
            if (entity.m_iCamp == 0)
            {
                if (mCurHpBar != null)
                {
                    mCurHpBar.SetLevel(level);
                }
            }
        }
    }

    public void SetHPDamage(int value, HitTextType type = HitTextType.NORMAL)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.SetHPDamage"))
        {
#endif
        if (mCurHpBar != null)
        {
            hpBarManager.ShowHPBar(mCurHpBar, value, type);
        }

        if (mCurrentHpBarId != HPBarManager.kInvalidHpBarId)
        {
            stateBarManager.currentActor = this;
            hpBarManager.ShowHPBar(mCurrentHpBarId, value, type);
        }

        if (mCurHpHeadBar != null)
        {
            hpBarManager.ShowHPBar(mCurHpHeadBar, value, type);
        }

        if (mCurrentStateBarId != StateBarManager.kInvalidStateBarId)
        {
            stateBarManager.SetBarActive(true);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void SetHPValue(float percent)
    {
        if (!isSyncHPMP)
            return;

        if (comHPPKBar != null)
        {
			comHPPKBar.SetHPPercent(percent);
			if (entity != null)
			{
				comHPPKBar.SetHPValue(entity.GetEntityData().GetHP(), entity.GetEntityData().GetMaxHP());
			}
        }
    }

	public void SetMpValue(float percent)
	{
        if (!isSyncHPMP)
            return;

        if (mCurHpBar != null)
		{
			hpBarManager.ShowMPBar(mCurHpBar, percent);
		}

		if (goHPPKBar != null)
		{
			comHPPKBar.SetMPPercent(percent);
		}
	}

	public void SyncHPBar()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.SyncHPBar"))
        {
#endif
        if (!isSyncHPMP)
            return;

        if (mCurHpBar != null && entity != null && entity.GetEntityData()!= null)
		{
			mCurHpBar.SetHP(entity.GetEntityData().GetHP(), entity.GetEntityData().GetMaxHP());
			mCurHpBar.SetMP(entity.GetEntityData().GetMP(), entity.GetEntityData().GetMaxMP());
		}

		if (goHPPKBar != null && entity != null && entity.GetEntityData()!= null)
		{
			comHPPKBar.SetHPValue(entity.GetEntityData().GetHP(), entity.GetEntityData().GetMaxHP());
		}

        if (HPBarManager.kInvalidHpBarId != mCurrentHpBarId)
        {
            hpBarManager.SyncHPBar(mCurrentHpBarId, entity.GetEntityData().GetHP(), entity.GetEntityData().GetMaxHP());
        }

        //刷新头顶血条
        if (mCurHpHeadBar != null && entity != null && entity.GetEntityData() != null)
        {
            mCurHpHeadBar.SetHP(entity.GetEntityData().GetHP(), entity.GetEntityData().GetMaxHP());
        }
#if ENABLE_PROFILER
        }
#endif
    }

	/// <summary>
	/// 给城镇NPC使用的异步加载角色Prefab的接口
	/// </summary>
	/// <param name="resID"></param>
	/// <param name="entityRoot"></param>
	/// <param name="scene"></param>
	/// <param name="iUnitId"></param>
	/// <param name="needChangeMaterial"></param>
	/// <param name="usePool"></param>
	public void CreateAsyncForTownNPC(int resID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool isBattleActor = true, bool usePool = true, PosLoadGeActorEx load = null)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateAsyncForTownNPC"))
        {
#endif
		if (null == entityRoot)
		{
			Logger.LogError("[GeActorEx] Entity root can not be null!");
			return;
		}

        if (null == scene)
        {
            Logger.LogError("[GeActorEx] Entity scene can not be null!");
            return;
        }

        ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);

        if (_initActorDescWithResTable(resData))
        {
            base.Init(resData.Name, entityRoot, scene, isBattleActor);
            m_EntitySpaceDesc.rootNode.name = resData.ParentName;

            if (null == m_EntitySpaceDesc.actorNode)
                m_EntitySpaceDesc.actorNode = new GameObject("actor");
            Battle.GeUtility.AttachTo(m_EntitySpaceDesc.actorNode, m_EntitySpaceDesc.characterNode);

            mPostLoadTownNPC = load;
            var gameObject = AssetLoader.instance.LoadResAsGameObject(m_ActorDesc.resPath);
            _onCreateNPCAsync(gameObject);
            //mPostLoadTownNPCHandle = _addAsyncLoadGameObject(m_ActorDesc.resPath, enResourceType.BattleScene, false, _onCreateNPCAsync);
        }
        else
        {
            Logger.LogErrorFormat("[GeActorEx] actor model is nil with path {0}", m_ActorDesc.resPath);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void RecreateForProjectile(bool useCube=false)
    {
        if (m_EntitySpaceDesc.actorModel != null && !useCube)
		{
			//如果已经是被屏蔽了，但是现在不要屏蔽了,需要重新加载资源
			/*if (GetUseCube() && !useCube)
			{
				SetUseCube(useCube);

				var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(m_ActorDesc.resID);
				if (resData != null)
				{
					m_ActorDesc.resPath = resData.ModelPath;

					//删掉原来的

					CGameObjectPool.instance.RecycleGameObject(m_EntitySpaceDesc.actorModel);

					m_EntitySpaceDesc.actorModel = CGameObjectPool.instance.GetGameObject(m_ActorDesc.resPath, enResourceType.BattleScene, true);

					m_Animation.Init(m_EntitySpaceDesc.actorNode);

					m_Attachment.RefreshAttachNode(m_EntitySpaceDesc.actorNode);

				}




			}*/



    
			m_EntitySpaceDesc.actorModel.CustomActive(true);
            SetUIVisible(true);
        }
            
    }



    public void ReleaseForProjectile(bool useCube=false)
    {
#if DEBUG_SETTING && !LOGIC_SERVER
        if(Global.Settings.showDebugBox)
            SetDebugDrawData(null);
#endif
        if (null != m_EntitySpaceDesc.actorModel)
        {
            m_EntitySpaceDesc.actorModel.CustomActive(false);
        }

        SetUIVisible(false);
    }

    //     public bool Create(int resID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool needChangeMaterial = true, bool usePool = true, bool useCube = false)
    //     {
    //         if (null == entityRoot)
    //         {
    //             Logger.LogError("Entity root can not be null!");
    //             return false;
    //         }
    //         if (null == scene)
    //         {
    //             Logger.LogError("Entity scene can not be null!");
    //             return false;
    //         }
    // 
    //         var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
    //         if (null != resData)
    //         {
    //             m_ActorDesc.resID = resID;
    //             m_ActorDesc.resPath = resData.ModelPath;
    //             m_ActorDesc.resName = Path.GetFileNameWithoutExtension(m_ActorDesc.resPath);
    //             m_ActorDesc.portraitIconRes = resData.IconPath;
    // 			m_ActorDesc.name = resData.ParentName;
    // 
    //             base.Init(resData.Name, entityRoot, scene);
    //             m_EntitySpaceDesc.rootNode.name = resData.ParentName;
    // 
    //             if (null == m_EntitySpaceDesc.actorNode)
    //                 m_EntitySpaceDesc.actorNode = new GameObject("actor");
    // 
    //             GameObject actorModel = null;
    //             string actorPath = "Actor/";
    // 
    // 
    // 			//this.useCube = useCube;
    // 			SetUseCube(useCube);
    // 			#if DEBUG_REPORT_ROOT
    // 			if (DebugSettings.GetInstance().DisableModuleLoad)
    //             {
    //                 m_ActorDesc.resPath = "Actor/Other/Cube222";
    //             }
    // 			else 
    // 			#endif
    // 			if (useCube)
    // 			{
    // 				m_ActorDesc.resPath = "Actor/Other/Cube";
    // 			}
    //             else
    //             {
    //                 if (resData.ModelPath.StartsWith(actorPath) && !resData.ModelPath.Contains("Other"))
    //                 {
    //                     int last = resData.ModelPath.IndexOf("Prefabs");
    //                     string trim = resData.ModelPath.Substring(0, last - 1);
    // 
    // 					if(null != trim)
    // 					{
    // 						string[] splitTbl = trim.Split('/');
    // 						string Name = splitTbl[splitTbl.Length - 1];
    // 
    // 						m_ActorDesc.modelDataRes = trim + "/" + Name;
    // 
    // 						if (Name.Contains("Hero_") || Name.Contains("Monster_"))
    // 						{
    // #if USE_FB
    //                             FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(m_ActorDesc.modelDataRes, Utility.kRawDataExtension)), out m_ModelData);
    // #else
    //                             m_ModelDataAsset = AssetLoader.instance.LoadRes(m_ActorDesc.modelDataRes, false);
    //                             if (null != m_ModelDataAsset)
    //                             {
    //                                 m_ModelData = m_ModelDataAsset.obj as DModelData;
    //                             }
    // #endif
    // 						}
    // 
    // 						if (null != m_ModelData)
    // 						{
    // 							if (m_Avatar.Init(m_ModelData.modelAvatar.m_AssetPath, 9,usePool))
    // 							{
    // 								m_Avatar.GetAvatarRoot().transform.SetParent(m_EntitySpaceDesc.actorNode.transform, true);
    // 
    //                                 //int remapIndex = 0;
    //                                 //for (int i = 0; i < m_ModelData.partsChunk.Length; ++i)
    //                                 //{
    //                                 //	remapIndex = (int)m_ModelData.partsChunk[i].partChannel;
    //                                 //	if (0 <= remapIndex && remapIndex < avatarChanTbl.Length)
    //                                 //		m_Avatar.ChangeAvatarObject(avatarChanTbl[remapIndex], m_ModelData.partsChunk[i].partAsset, AssetLoadConfig.instance.asyncLoad);
    //                                 //	else
    //                                 //		Logger.LogWarningFormat("Unsupported model data channel enumeration[{0}]!", m_ModelData.partsChunk[i].partChannel.ToString());
    //                                 //}
    // 
    //                                 int remapIndex = 0;
    //                                 for (int i = 0; i < m_ModelData.partsChunk.Length; ++i)
    //                                 {
    //                                     remapIndex = (int)m_ModelData.partsChunk[i].partChannel;
    //                                     if (0 <= remapIndex && remapIndex < avatarChanTbl.Length)
    //                                         m_Avatar.AddDefaultAvatar(avatarChanTbl[remapIndex], m_ModelData.partsChunk[i].partAsset);
    //                                     else
    //                                         Logger.LogWarningFormat("Unsupported model data channel enumeration[{0}]!", m_ModelData.partsChunk[i].partChannel.ToString());
    //                                 }
    // 
    //                                 PushPostLoadCommand(() =>
    // 									{
    // 										m_ActorPartes = m_Avatar.suitPartModel;
    // 										m_Material.AppendObject(m_ActorPartes);
    // 									});
    // 								actorModel = m_Avatar.GetAvatarRoot();
    // 							}
    // 							else
    // 							{
    // 								m_ModelDataAsset = null;
    // 								m_ModelData = null;
    // 							}
    // 						}
    // 					}
    // 				}
    // 
    // 				if (resData.ModelPath.Contains("LogicObject") || resData.ModelPath.Contains("Decorator"))
    // 				{
    // 					string unifiedPath = resData.ModelPath.Replace('\\', '/');
    // 					string[] splitTbl = unifiedPath.Split('/');
    // 					string Name = splitTbl[splitTbl.Length - 1];
    // 
    // 					int last = unifiedPath.IndexOf(Name);
    // 					string trimPath = unifiedPath.Substring(0, last);
    // 
    // 					m_ActorDesc.modelDataRes = trimPath + Name + "_ModelData";
    // 
    // #if USE_FB
    //                     FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(m_ActorDesc.modelDataRes, Utility.kRawDataExtension)), out m_ModelData);
    // #else
    //                     m_ModelDataAsset = AssetLoader.instance.LoadRes(m_ActorDesc.modelDataRes, false);
    //                     if (null != m_ModelDataAsset)
    //                         m_ModelData = m_ModelDataAsset.obj as DModelData;
    // #endif
    // 
    // 					if (null != m_ModelData)
    // 					{
    // 						m_Avatar.Init("", 9);
    // 						m_Avatar.GetAvatarRoot().transform.SetParent(m_EntitySpaceDesc.actorNode.transform, true);
    // 
    // 						int remapIndex = 0;
    // 						for (int i = 0; i < m_ModelData.partsChunk.Length; ++i)
    // 						{
    // 							remapIndex = (int)m_ModelData.partsChunk[i].partChannel;
    // 							if (0 <= remapIndex && remapIndex < avatarChanTbl.Length)
    // 								m_Avatar.ChangeAvatarObject(avatarChanTbl[remapIndex], m_ModelData.partsChunk[i].partAsset, AssetLoadConfig.instance.asyncLoad);
    // 							else
    // 								Logger.LogWarningFormat("Unsupported model data channel enumeration[{0}]!", m_ModelData.partsChunk[i].partChannel.ToString());
    // 						}
    // 
    // 						PushPostLoadCommand(() =>
    // 							{
    // 								m_ActorPartes = m_Avatar.suitPartModel;
    // 								m_Material.AppendObject(m_ActorPartes);
    // 							});
    // 						actorModel = m_Avatar.GetAvatarRoot();
    // 					}
    // 				}
    // 			}
    //             
    //             if (null == actorModel)
    //             {
    //                 //actorModel = CGameObjectPool.instance.GetGameObject(m_ActorDesc.resPath, enResourceType.BattleScene, true);
    //                 //if (!_onActorModelPrefabLoadFinish(actorModel))
    //                 //{
    //                 //    Logger.LogErrorFormat("actor model is nil with path {0}", m_ActorDesc.resPath);
    //                 //    return false;
    //                 //}
    // 
    //                 if (m_Avatar.Init(m_ActorDesc.resPath, 9, usePool))
    //                 {
    //                     actorModel = m_Avatar.GetAvatarRoot();
    //                     if (!_onActorModelPrefabLoadFinish(m_Avatar.GetAvatarRoot()))
    //                     {
    //                         Logger.LogErrorFormat("actor model is nil with path {0}", m_ActorDesc.resPath);
    //                         return false;
    //                     }
    //                 }
    // 
    //                 m_ModelManager = new GeModelManager(this, resID, actorModel);
    //             }
    //             m_EntityState = GeEntityState.Loaded;
    // 
    //             _onPostActorModelPrefabLoadFinish(actorModel);
    //             return true;
    //         }
    //         else
    //             Logger.LogWarningFormat("Can not find table item with ID:{0}", resID);
    // 
    //         return false;
    //     }


    public bool Create(int resID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool isBattleActor = true, bool usePool = true, bool useCube = false, bool needBackCreate = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.Create"))
        {
#endif
        if (null == entityRoot)
        {
            Logger.LogError("Entity root can not be null!");
            return false;
        }
        if (null == scene)
        {
            Logger.LogError("Entity scene can not be null!");
            return false;
        }
        
        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
        if (null != resData)
        {
            m_ActorDesc.resID = resID;
            m_ActorDesc.resPath = resData.ModelPath;
            m_ActorDesc.resName = Path.GetFileNameWithoutExtension(m_ActorDesc.resPath);
            m_ActorDesc.portraitIconRes = resData.IconPath;
            m_ActorDesc.name = resData.ParentName;

            base.Init(resData.Name, entityRoot, scene, isBattleActor);
            m_EntitySpaceDesc.rootNode.name = resData.ParentName;

            if (null == m_EntitySpaceDesc.actorNode)
                m_EntitySpaceDesc.actorNode = new GameObject("actor");
            Battle.GeUtility.AttachTo(m_EntitySpaceDesc.actorNode, m_EntitySpaceDesc.characterNode);

            GameObject actorModel = null;

            m_ActorDesc.modelDataRes = resData.ModelPath;
            //this.useCube = useCube;
            SetUseCube(useCube);
            if (FrameSync.instance.IsInChasingMode && needBackCreate)
            {
                if (useCube)
                {
                    m_ActorDesc.resPath = "Actor/Other/Cube";
                }
                if (Path.GetExtension(m_ActorDesc.modelDataRes).Contains("asset"))
                {
#if USE_FB
                FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(m_ActorDesc.modelDataRes, Utility.kRawDataExtension)), out m_ModelData);
#else
                    m_ModelDataAsset = AssetLoader.instance.LoadRes(m_ActorDesc.modelDataRes, false);
                    if (null != m_ModelDataAsset)
                    {
                        m_ModelData = m_ModelDataAsset.obj as DModelData;
                    }
#endif
                }
                if (m_ModelData != null)
                {
                    if (!AssetLoader.IsResExist(m_ModelData.modelAvatar.m_AssetPath, typeof(GameObject), true))
                    {
                        m_ModelDataAsset = null;
                        m_ModelData = null;
                        if (!AssetLoader.IsResExist(m_ActorDesc.resPath, typeof(GameObject), true))
                        {
                            Logger.LogErrorFormat("Create GeActor  resPath is not exist {0} assetPath {1}", m_ActorDesc.resPath, m_ModelData.modelAvatar.m_AssetPath);
                            return false;
                        }
                    }
                }
                else
                {
                    if (null != m_ModelDataAsset && m_ModelDataAsset.obj is GameObject)
                    {
                        GameObject.Destroy(m_ModelDataAsset.obj);
                        m_ModelDataAsset = null;
                        m_ModelData = null;
                    }
                }
                if (!AssetLoader.IsResExist(m_ActorDesc.resPath, typeof(GameObject), true))
                {
                    Logger.LogErrorFormat("Create GeActor  resPath is not exist {0}", m_ActorDesc.resPath);
                    return false;
                }
                var cmd = ActorCreateGBCommand.Acquire();
                cmd.resID = resID;
                cmd.entityRoot = entityRoot;
                cmd.scene = scene;
                cmd.iUnitId = iUnitId;
                cmd.isBattleActor = isBattleActor;
                cmd.usePool = usePool;
                cmd.useCube = useCube;
                cmd.actor = this;
                GBController.RecordCmd((int)GeEntityBackCmdType.Create_Actor, cmd);
                createdInBackMode = true;
                SetFontIndircatorInBackGround(true);
                return true;
            }
            createdInBackMode = false;
            SetFontIndircatorInBackGround(false);
#if DEBUG_REPORT_ROOT
            if (DebugSettings.GetInstance().DisableModuleLoad)
            {
                m_ActorDesc.resPath = "Actor/Other/Cube222";
            }
            else
#endif
                if (useCube)
            {
                m_ActorDesc.resPath = "Actor/Other/Cube";
            }
            else
            {

                if (Path.GetExtension(m_ActorDesc.modelDataRes).Contains("asset"))
                {
#if USE_FB
                    FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(m_ActorDesc.modelDataRes, Utility.kRawDataExtension)), out m_ModelData);
#else
                    m_ModelDataAsset = AssetLoader.instance.LoadRes(m_ActorDesc.modelDataRes, false);
                    if (null != m_ModelDataAsset)
                    {
                        m_ModelData = m_ModelDataAsset.obj as DModelData;
                    }
#endif
                }

                if (null != m_ModelData)
                {
                    if (m_Avatar.Init(m_ModelData.modelAvatar.m_AssetPath, 9, usePool))
                    {
                        m_IsAvatarDirty = true;
                        m_Avatar.GetAvatarRoot().transform.SetParent(m_EntitySpaceDesc.actorNode.transform, true);

                        int remapIndex = 0;
                        for (int i = 0; i < m_ModelData.partsChunk.Length; ++i)
                        {
                            remapIndex = (int)m_ModelData.partsChunk[i].partChannel;
                            if (0 <= remapIndex && remapIndex < avatarChanTbl.Length)
                                m_Avatar.AddDefaultAvatar(avatarChanTbl[remapIndex], m_ModelData.partsChunk[i].partAsset);
                            else
                                Logger.LogWarningFormat("Unsupported model data channel enumeration[{0}]!", m_ModelData.partsChunk[i].partChannel.ToString());
                        }

                        PushPostLoadCommand(() =>
                        {
                            m_ActorPartes = m_Avatar.suitPartModel;                         
                        });
                        actorModel = m_Avatar.GetAvatarRoot();
                    }
                    else
                    {
                        m_ModelDataAsset = null;
                        m_ModelData = null;
                    }
                }
                else
                {
                    if(null != m_ModelDataAsset && m_ModelDataAsset.obj is GameObject)
                    {
                        GameObject.Destroy(m_ModelDataAsset.obj);
                        m_ModelDataAsset = null;
                        m_ModelData = null;
                    }
                }
            }

            if (null == actorModel)
            {
                if (m_Avatar.Init(m_ActorDesc.resPath, 9, usePool))
                {
                    m_IsAvatarDirty = true;
                    actorModel = m_Avatar.GetAvatarRoot();
                    if (!_onActorModelPrefabLoadFinish(m_Avatar.GetAvatarRoot()))
                    {
                        Logger.LogErrorFormat("actor model is nil with path {0}", m_ActorDesc.resPath);
                        return false;
                    }
                }

                if (isBattleActor)
                    m_ModelManager = new GeModelManager(this, resID, actorModel);
            }

            m_EntityState = GeEntityState.Loaded;

            _onPostActorModelPrefabLoadFinish(actorModel);
            return true;
        }
        else
            Logger.LogWarningFormat("Can not find table item with ID:{0}", resID);

        return false;
#if ENABLE_PROFILER
        }
#endif
    }


    public bool CreateAsync(int resID, GameObject entityRoot, GeSceneEx scene, int iUnitId, PosLoadGeActorEx postLoadCallback, bool isBattleActor = true, bool usePool = true, bool useCube = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateAsync"))
        {
#endif
        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
        if (null != resData)
        {
            m_ActorDesc.resID = resID;
            m_ActorDesc.resPath = resData.ModelPath;
            m_ActorDesc.resName = Path.GetFileNameWithoutExtension(m_ActorDesc.resPath);
            m_ActorDesc.portraitIconRes = resData.IconPath;
            m_ActorDesc.name = resData.ParentName;

            base.Init(resData.Name, entityRoot, scene, isBattleActor);
            m_EntitySpaceDesc.rootNode.name = resData.ParentName;

            if (null == m_EntitySpaceDesc.actorNode)
                m_EntitySpaceDesc.actorNode = new GameObject("actor");
            Battle.GeUtility.AttachTo(m_EntitySpaceDesc.actorNode, m_EntitySpaceDesc.characterNode);

            GameObject actorModel = null;
            m_ActorDesc.modelDataRes = resData.ModelPath;

            SetUseCube(useCube);
#if DEBUG_REPORT_ROOT
            if (DebugSettings.GetInstance().DisableModuleLoad)
            {
                m_ActorDesc.resPath = "Actor/Other/Cube222";
            }
            else
#endif
            if (useCube)
            {
                m_ActorDesc.resPath = "Actor/Other/Cube";
            }
            else
            {
                if(Path.GetExtension(m_ActorDesc.modelDataRes).Contains("asset"))
                { 
#if USE_FB
                    FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(m_ActorDesc.modelDataRes, Utility.kRawDataExtension)), out m_ModelData);
#else
                    m_ModelDataAsset = AssetLoader.instance.LoadRes(m_ActorDesc.modelDataRes, false);
                    if (null != m_ModelDataAsset)
                    {
                        m_ModelData = m_ModelDataAsset.obj as DModelData;
                    }
#endif
                }

                if (null != m_ModelData)
                {
                    if (m_Avatar.Init(m_ModelData.modelAvatar.m_AssetPath, 9, usePool))
                    {
                        m_IsAvatarDirty = true;
                        m_Avatar.GetAvatarRoot().transform.SetParent(m_EntitySpaceDesc.actorNode.transform, true);

                        int remapIndex = 0;
                        for (int i = 0; i < m_ModelData.partsChunk.Length; ++i)
                        {
                            remapIndex = (int)m_ModelData.partsChunk[i].partChannel;
                            if (0 <= remapIndex && remapIndex < avatarChanTbl.Length)
                                m_Avatar.AddDefaultAvatar(avatarChanTbl[remapIndex], m_ModelData.partsChunk[i].partAsset);
                            else
                                Logger.LogWarningFormat("Unsupported model data channel enumeration[{0}]!", m_ModelData.partsChunk[i].partChannel.ToString());
                        }

                        PushPostLoadCommand(() =>
                        {
                            m_ActorPartes = m_Avatar.suitPartModel;
                        });
                        actorModel = m_Avatar.GetAvatarRoot();
                    }
                    else
                    {
                        m_ModelDataAsset = null;
                        m_ModelData = null;
                    }
                }
                else
                {
                    if (null != m_ModelDataAsset && m_ModelDataAsset.obj is GameObject)
                    {
                        GameObject.Destroy(m_ModelDataAsset.obj);
                        m_ModelDataAsset = null;
                        m_ModelData = null;
                    }
                }
            }

            if (null == actorModel)
            {
                m_IsAvatarDirty = true;
                m_Avatar.Init(m_ActorDesc.resPath, 9, usePool, true);
            }

            PushPostLoadCommand(() =>
            {
                actorModel = m_Avatar.GetAvatarRoot();
                if (null != actorModel)
                    actorModel.SetActive(true);
                _onActorModelPrefabLoadFinish(actorModel);
                m_ModelManager = new GeModelManager(this, resID, actorModel);

                m_EntityState = GeEntityState.Loaded;
                _onPostActorModelPrefabLoadFinish(actorModel);

                actorModel.SetActive(true);
                ChangeAction("Anim_Idle01",1.0f);

                if (null != postLoadCallback)
                    postLoadCallback(this);
            });

            return true;
        }
        else
            Logger.LogWarningFormat("Can not find table item with ID:{0}", resID);

        return false;
#if ENABLE_PROFILER
        }
#endif
    }

    public void RemoveHPBar()
	{
        if (HPBarManager.kInvalidHpBarId != mCurrentHpBarId)
        {
            hpBarManager.RemoveHPBar(mCurrentHpBarId);

            mCurrentHpBarId = HPBarManager.kInvalidHpBarId;
        }
        if (null != mCurHpHeadBar)
		{
			hpBarManager.RemoveHPBar(mCurHpHeadBar);
            mCurHpHeadBar.Unload();
			mCurHpHeadBar = null;
		}

		if (mCurHpBar != null)
		{
			hpBarManager.RemoveHPBar(mCurHpBar);
            mCurHpBar.Unload();
			mCurHpBar = null;
		}

		if (goHPBar != null)
        {
            CGameObjectPool.instance.RecycleGameObject(goHPBar);
            //GameObject.Destroy(goHPBar);
			goHPBar = null;
		}
	}

    public void DestroySelfHPBar()
    {
        if (goHPBarHUD != null)
        {
            //goHPBarHUD.SetActive(false);
            GameObject.Destroy(goHPBarHUD);
            goHPBarHUD = null;
		}
	}

    public void Destroy()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.Destroy"))
        {
#endif
        //HideActor(false);
        _ClearMaterial();
        _ClearEmissiveRenderer();

        RemoveHPBar();

		if (goHPBarHUD != null)
		{
			//goHPBarHUD.SetActive(false);
			GameObject.Destroy(goHPBarHUD);
			goHPBarHUD = null;
		}
        if (mGoFootInfo != null)
        {
            GameObject.Destroy(mGoFootInfo);
            mGoFootInfo = null;
        }

        if (goHPPKBar != null)
        {
            GameObject.Destroy(goHPPKBar);
            goHPPKBar = null;
        }

        if(null != m_kLevelText)
        {
            GameObject.Destroy(m_kLevelText);
            m_kLevelText = null;
        }

        if (null != entityHeadIconAsset)
        {
            entityHeadIcon = null;
            entityHeadIconAsset = null;
            entityHeadIconMaterial = null;
        }

        if (titleComponent != null)
        {
            titleComponent.OnRecycle();
            titleComponent = null;
        }

        if(goInfoBar != null)
        {
            goInfoBar.transform.SetParent(null);
            goInfoBar.CustomActive(false);
            goInfoBarBottom = null;
            CGameObjectPool.instance.RecycleGameObject(goInfoBar);
            goInfoBar = null;
        }

        mDialogComponentData = null;
        mPlayerInfoBarData = null;
        //mPlayerInfoBarDataHandle = uint.MaxValue;
        mNpcInteractionData = null;
        mTittleComponentData = null;
        mNPCBoxCollectData = null;
        if (null != mPostLoadTownNPC)
        {
            if (null != m_EntitySpaceDesc.actorModel)
            {
                m_EntitySpaceDesc.actorModel.CustomActive(false);
            }
        }
        mPostLoadInfoBar = null;
        mPostLoadTownNPC = null;
        mPostLoadTownNPCHandle = uint.MaxValue;

        //在模型资源回收以前 重置模型节点的层级 确保从pool里面拿出来用的时候层级正确
        if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer) && m_EntitySpaceDesc.actorModel != null)
        {
            m_EntitySpaceDesc.actorNode.SetLayer(0);
        }

        base.Deinit();
        m_EntityState = GeEntityState.Removed;
        m_EntityState = GeEntityState.Removed;

        //_clearAsyncLoadGameObject();

        AssetGabageCollectorHelper.instance.AddGCPurgeTick(AssetGCTickType.SceneActor);

        // 变身extra模型删除
        if (m_ModelManager != null)
            m_ModelManager.Clear();
        if (slideArrowBind != null)
        {
            GameObject.Destroy(slideArrowBind.gameObject);
            slideArrowBind = null;
        }
        if (forwardBackArrowBind != null)
        {
            GameObject.Destroy(forwardBackArrowBind.gameObject);
            forwardBackArrowBind = null;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public bool SetMaterial(string shaderName)
    {
        return true;
    }

    protected void _ClearMaterial()
    {
        if(null != m_SurfMaterial)
        {
            m_MatSurfObjDescList.RemoveAll(
                e =>
                {
                    for (int i = 0; i < e.m_MatMeshRendDescList.Length; ++i)
                        e.m_MatMeshRendDescList[i].m_MeshRenderer.materials = e.m_MatMeshRendDescList[i].m_OriginMatList;
                    return true;
                });

            UnityEngine.Object.Destroy(m_SurfMaterial);
            m_SurfMaterial = null;
        }
    }



    

    public string GetResPath()
    {
        return m_ActorDesc.resPath;
    }
    public string GetResName()
    {
        return m_ActorDesc.resName;
    }
    public int GetResID()
    {
        return m_ActorDesc.resID;
    }
    public void SetHighLight(bool hightLight)
    {
        if (m_IsHighLight != hightLight)
        {
            if (hightLight)
                ChangeSurface("选中高亮", 0.0f, true);
            else
                ChangeSurface("", 0.0f);

            m_IsHighLight = hightLight;
        }
    }

    public void SetDyeColor(Color dyeColor, GameObject[] modelRoot)
    {
        return;

        if (null == modelRoot)
            return;

        for (int i = 0; i < modelRoot.Length; ++i)
        {
            MeshRenderer[] amr = modelRoot[i].transform.GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < amr.Length; ++j)
            {
                Material[] am = amr[j].materials;
                for (int k = 0; k < am.Length; ++k)
                {
                    if (am[k].HasProperty("_DyeColor"))
                        am[k].SetColor("_DyeColor", dyeColor);
                }
            }
        }
    }

    public void SetEmissiveColor(Color color, float duration = 1f)
    {
        m_EnableEmissiveColor = true;

        if (m_LastEmissiveColor == Color.clear)
            m_LastEmissiveColor = color;
        else
        {
            m_LastEmissiveColor = m_DestEmissiveColor;
        }
        m_DestEmissiveColor = color;
        m_EmissiveDuration = duration;
        m_EmissiveTimer = 0f;

        if (m_EmissiveRenderers == null)
        {
            m_EmissiveRenderers = new List<Renderer>();
            _CollectEmissiveRenderer();
        }
    }

    private void _UpdateEmissiveColor(int delta)
    {
        if (m_EmissiveRenderers != null)
        {
            m_EmissiveTimer += delta / 1000f;

            if (m_EmissiveTimer > m_EmissiveDuration)
                return;

            if (m_EmissiveBlock == null)
                m_EmissiveBlock = new MaterialPropertyBlock();

            for (int i = 0; i < m_EmissiveRenderers.Count; ++i)
            {
                Color color = Color.Lerp(m_LastEmissiveColor, m_DestEmissiveColor, m_EmissiveTimer / m_EmissiveDuration);
                if (m_EmissiveRenderers[i] != null)
                {
                    m_EmissiveRenderers[i].GetPropertyBlock(m_EmissiveBlock);
                    m_EmissiveBlock.SetColor("_TintColor", color);
                    m_EmissiveRenderers[i].SetPropertyBlock(m_EmissiveBlock);
                }
            }
        }
    }

    private void _CollectEmissiveRenderer()
    {
        if (m_EnableEmissiveColor && m_EmissiveRenderers != null)
        {
            m_EmissiveRenderers.Clear();
            for (int i = 0; i < m_ActorPartes.Length; ++i)
            {
                for (int j = 0; j < m_ActorPartes[i].transform.childCount; ++j)
                {
                    Transform child = m_ActorPartes[i].transform.GetChild(j);
                    if (child.CompareTag("EffectModel"))
                    {
                        Renderer renderer = child.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            m_EmissiveRenderers.Add(renderer);
                        }
                    }
                }
            }
        }
    }

    private void _ClearEmissiveRenderer()
    {
        if (m_EmissiveRenderers != null)
        {
            m_EmissiveRenderers.Clear();
            m_EmissiveRenderers = null;
        }
        m_EmissiveBlock = null;
        m_LastEmissiveColor = Color.clear;
        m_DestEmissiveColor = Color.clear;
        m_EmissiveTimer = 0f;
        m_EmissiveDuration = 0f;
    }

    public void SetShadowVisible(GeSceneEx scene, bool visible)
    {
        if (scene != null)
        {
            //             var shadowManager = GePlaneShadowManager.instance;
            //             if (shadowManager == null)
            //                 return;
            if (isCreatedInBackMode)
            {
                var cmd = ActorShadowVisibleGBCommand.Acquire();
                cmd.scene = scene;
                cmd.isVisible = visible;
                cmd.actor = this;
                GBController.RecordCmd((int)GeEntityBackCmdType.Actor_Shadow_Visible, cmd);
                return;
            }
            if (!visible)
            {
                _RemoveShadow();
                //shadowManager.RemoveShadowObject(renderObject);
            }
            else
            {
                _AddShadow();
                //shadowManager.AddShadowObject(renderObject, GeSceneEx.EntityPlane);
            }
        }
    }

	public void SetActorVisible(bool visible)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.SetActorVisible"))
        {
#endif
            if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer))
            {
                //显示与隐藏改用修改layer的方式
                int layer = visible ? 0 : 19;
                m_EntitySpaceDesc.actorNode.SetLayer(layer);
            }
            else
            {
                m_EntitySpaceDesc.actorNode.CustomActive(visible);
            }
            if (goFootInfo != null)
                goFootInfo.CustomActive(visible);
            if (mGoFootInfo != null)
            {
                mGoFootInfo.CustomActive(visible);
            }
            if (goInfoBar != null)
                goInfoBar.CustomActive(visible);
            if (goHPBar != null)
                goHPBar.CustomActive(visible);
            if (goDialog != null)
            {
                if (dialog.IsShow())
                    goDialog.CustomActive(visible);
            }
            for (int i = 0; i < mDialogs.Length; i++)
            {
                if(mDialogs[i] == null)
                {
                    continue;
                }
                if (i < mDialogScripts.Length && mDialogScripts[i].IsShow())
                {
                    mDialogs[i].CustomActive(visible);
                }
            }
#if ENABLE_PROFILER
        }
#endif
    }

	public void SetActorForLowLevel()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.SetActorForLowLevel"))
        {
#endif
        if (null != m_EntitySpaceDesc.actorNode)
        {
            m_EntitySpaceDesc.actorNode.CustomActive(false);
        }

		RemoveHPBar();

		if (goHPBarHUD != null)
		{
			//goHPBarHUD.SetActive(false);
			GameObject.Destroy(goHPBarHUD);
			goHPBarHUD = null;
		}

		if (goHPPKBar != null)
		{
			GameObject.Destroy(goHPPKBar);
			goHPPKBar = null;
		}

		if (null != m_kLevelText)
		{
			GameObject.Destroy(m_kLevelText);
			m_kLevelText = null;
		}

		if (null != entityHeadIconAsset)
		{
			entityHeadIcon = null;
			entityHeadIconAsset = null;
            entityHeadIconMaterial = null;
		}

		if (titleComponent != null)
		{
			titleComponent.OnRecycle();
			titleComponent = null;
		}

		if (goInfoBar != null)
		{
			goInfoBar.transform.SetParent(null);
			goInfoBar.CustomActive(false);
			goInfoBarBottom = null;
			CGameObjectPool.instance.RecycleGameObject(goInfoBar);
            goInfoBar = null;

        }
        if (isCreatedInBackMode)
        {
            var cmd = ActorLowLevelGBCommand.Acquire();
            cmd.actor = this;
            GBController.RecordCmd((int)GeEntityBackCmdType.Actor_LowLevel, cmd);
            return;
        }

        if (null != m_EntitySpaceDesc.actorModel)
		{
			m_EntitySpaceDesc.actorModel.transform.SetParent(null,false);
			CGameObjectPool.instance.RecycleGameObject(m_EntitySpaceDesc.actorModel);
			m_EntitySpaceDesc.actorModel = null;
		}
#if ENABLE_PROFILER
        }
#endif
	}

    public void SetHPBarVisible(bool visible)
    {
        if (goHPBar != null)
            goHPBar.CustomActive(visible);
    }

    /// <summary>
    /// 设置Actor的显示模式，Simple模式只显示一个简单的模型
    /// </summary>
    /// <param name="isSimpleMode"></param>
    public void SetDisplayMode(DisplayMode displayMode)
    {
        if(m_DisplayMode != displayMode)
        {
            m_DisplayMode = displayMode;
            m_Avatar.ChangeDisplayMode(m_DisplayMode);
            switch(displayMode)
            {
                case DisplayMode.Normal:
                    if (m_Effect != null)
                        m_Effect.SetEffectVisible(true);
                    break;
                case DisplayMode.Simple:
                    if (m_Effect != null)
                        m_Effect.SetEffectVisible(false);
                    break;
            }
        }
    }

    public void SetProfessionId(int proId)
    {
        if(m_Avatar != null)
        {
            m_Avatar.SetProfessionalId(proId);
        }
    }
	public void EquipFashions(Dictionary<int, string> fashions,int prodid = 0)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.EquipFashions"))
        {
#endif
		if (fashions != null)
		{
            m_FashionsData = fashions;
			foreach(var fashion in fashions)
			{
				int key = fashion.Key;
				string value = fashion.Value;

                // 吃鸡里的装备要带入外观
				if (key == (int)ProtoTable.ItemTable.eSubType.FASHION_CHEST || key == (int)ProtoTable.ItemTable.eSubType.CHEST)
                {
                    ChangeAvatar(GeAvatarChannel.UpperPart, value, AssetLoadConfig.instance.asyncLoad, false, prodid);
                }
				else if (key == (int)ProtoTable.ItemTable.eSubType.FASHION_HEAD || key == (int)ProtoTable.ItemTable.eSubType.HEAD)
                {
                    ChangeAvatar(GeAvatarChannel.Head, value, AssetLoadConfig.instance.asyncLoad, false, prodid);
                }	
				else if (key == (int)ProtoTable.ItemTable.eSubType.FASHION_LEG || key == (int)ProtoTable.ItemTable.eSubType.BOOT)
                {
                    ChangeAvatar(GeAvatarChannel.LowerPart, value, AssetLoadConfig.instance.asyncLoad, false, prodid);
                }
                else if(key == (int)ProtoTable.ItemTable.eSubType.FASHION_SASH || key == (int)ProtoTable.ItemTable.eSubType.LEG)
                {
                    ChangeAvatar(GeAvatarChannel.Bracelet, value, AssetLoadConfig.instance.asyncLoad, false, prodid);
                }
                else if(key == (int)ProtoTable.ItemTable.eSubType.FASHION_EPAULET || key == (int)ProtoTable.ItemTable.eSubType.BELT)
                {
                    ChangeAvatar(GeAvatarChannel.Headwear, value, AssetLoadConfig.instance.asyncLoad, false, prodid);
                }
                else if (key == (int)ProtoTable.ItemTable.eSubType.FASHION_HAIR)
				{
					
					PlayerBaseData.GetInstance().AvatarEquipWingFromRes(null, value, Global.WING_LOCATOR_NAME, this, false, Global.WING_ATTACH_NAME);
				}

                /*
                //如果是光环战斗里不加载光环，
                else if (key == (int)ProtoTable.ItemTable.eSubType.FASHION_AURAS)
                {
                    PlayerBaseData.GetInstance().AvatarEquipWingFromRes(null, value, Global.HALO_LOCATOR_NAME, this, false, Global.HALO_ATTACH_NAME);
                }*/
					
			}
		}
#if ENABLE_PROFILER
        }
#endif
	}

    public void HackRamp(Texture rampTex)
    {
        if (null == rampTex)
            return;

        int unCount = renderObject.Length;
        for(int i = 0; i < unCount; ++ i)
        {
            SkinnedMeshRenderer[] asmr = renderObject[i].GetComponentsInChildren<SkinnedMeshRenderer>();
            if(null!= asmr)
            {
                for(int j = 0, smrCnt = asmr.Length; j < smrCnt; ++ j)
                {
                    if(null == asmr[j]) continue;

                    Material[] am = asmr[j].materials;
                    if(null != am)
                    {
                        for (int k = 0, mCnt = am.Length; k < mCnt; ++ k)
                        {
                            if (null == am[k]) continue;

                            if (am[k].HasProperty("_Ramp"))
                                am[k].SetTexture("_Ramp", rampTex);

                            //TO DO wangchong Hack LightExposure -> 1.1f 
                            //用于提高角色材质亮度
                            if (am[k].HasProperty("_LightExposure"))
                                am[k].SetFloat("_LightExposure",1.1f);
                        }
                    }
                }
            }
        }
    }

	public void ShowProtectFloat(bool show, float percent=0f)
	{
		if (BattleMain.IsProtectFloat(BattleMain.battleType) == false)
			return;

		if (comHPPKBar != null)
			comHPPKBar.ShowProtectFloat(show, percent);
	}

	public void ShowProtectGround(bool show, float percent=0f)
	{
        if (BattleMain.IsProtectGround(BattleMain.battleType) == false)
            return;

        if (comHPPKBar != null)
			comHPPKBar.ShowProtectGround(show, percent);
	}

	public void ShowProtectStand(bool show, float percent=0f)
	{
        if (BattleMain.IsProtectStand(BattleMain.battleType) == false)
            return;

        if (comHPPKBar != null)
			comHPPKBar.ShowProtectStand(show, percent);
	}

    public void AddKillMark()
    {
        if (null != m_EntitySpaceDesc.rootNode)
        {
            GameObject curKillMark = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/EffectUI/EffUI_KillMark");
            curKillMark.transform.SetParent(m_EntitySpaceDesc.rootNode.transform,false);

            Vector3 localPos = curKillMark.transform.localPosition;
            //GeAttachManager.GeAttachNodeDesc overhead = m_Attachment.GetAttchNodeDesc("[actor]OverHead");
            //if (null != overhead.attachNode)
            //    localPos.y += overhead.attachNode.transform.position.y;
            //curKillMark.transform.localPosition = localPos;

            GameObject overhead = m_Avatar.GetAttachNode("[actor]OverHead");
            if (null != overhead)
                localPos.y += overhead.transform.position.y;
            curKillMark.transform.localPosition = localPos;
        }
    }

    public void HideActor(bool bIsHide)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.HideActor"))
        {
#endif
        //显示与隐藏改用修改layer的方式
        int layer = bIsHide ? 19 : 0;

        m_EntitySpaceDesc.m_IsEntityHide = bIsHide;
        if (null != m_EntitySpaceDesc.rootNode)
        {
            if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer))
            {
                m_EntitySpaceDesc.rootNode.SetLayer(layer);
            }
            else
            {
                m_EntitySpaceDesc.rootNode.SetActive(!bIsHide);
            }

            PushPostLoadCommand(
                () =>
                {
                    if (null != m_EntitySpaceDesc.rootNode)
                    {
                        if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer))
                        {
                            m_EntitySpaceDesc.rootNode.SetLayer(layer);
                        }
                        else
                        {
                            m_EntitySpaceDesc.rootNode.SetActive(!bIsHide);
                        }
                    }   
                }
                );
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void SetActorNodeVisable(bool visible)
    {
        if(BeClientSwitch.FunctionIsOpen(ClientSwitchType.HideObjUseSetlayer))
        {
            //显示与隐藏改用修改layer的方式
            int layer = visible ? 0 : 19;
            m_EntitySpaceDesc.actorNode.SetLayer(layer);
        }
        else
        {
            m_EntitySpaceDesc.actorNode.transform.localPosition = visible ? Vector3.zero : new Vector3(0, -1000, 0);
        }    
    }
    
    public bool IsActorHide()
    {
        return m_EntitySpaceDesc.m_IsEntityHide;
    }

    //#region change model
    public void PreChangeModel(int resID)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.PreChangeModel"))
        {
#endif
            if (createdInBackMode)
            {
                var cmd = ActorChangeModelGBCommand.Acquire();
                cmd.isPreChange = true;
                cmd.resId = resID;
                cmd.actor = this;
                GBController.RecordCmd((int)GeEntityBackCmdType.Change_Model, cmd);
                return;
            }
            if (m_ModelManager == null)
            return;

            m_ModelManager.PreChangeModel(resID);
#if ENABLE_PROFILER
        }
#endif
    }

    public void TryChangeModel(int resID)
    {
        if (createdInBackMode)
        {
            var cmd = ActorChangeModelGBCommand.Acquire();
            cmd.isPreChange = false;
            cmd.resId = resID;
            cmd.actor = this;
            GBController.RecordCmd((int)GeEntityBackCmdType.Change_Model, cmd);
            return;
        }

        if (m_ModelManager == null)
            return;

        m_ModelManager.TryChangeModel(resID);
    }

    // 只能处理怪物
    public bool ChangeModel(GameObject toModel) // modelType:0是复原 1是第一个模型
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.ChangeModel"))
        {
#endif
        if (toModel == null || m_ActorPartes == null || m_ModelManager == null)
            return false;

        GameObject actorNode = m_EntitySpaceDesc.actorNode;
        GameObject frModel = m_EntitySpaceDesc.actorModel;

        // 
        GameObject[] actorPartes = m_ActorPartes;
        m_ActorPartes = new GameObject[] { toModel };
        snapShotCache = m_ActorPartes;
        m_EntitySpaceDesc.actorModel = toModel;

        // 挂点处理
        Transform fr = frModel.transform;
        Transform to = toModel.transform;
        to.localScale = fr.localScale;
        to.localPosition = new Vector3(0,0,0);
        to.position = new Vector3(0, 0, 0);
        to.localRotation = fr.localRotation;

#if !LOGIC_SERVER
        actorNode.transform.DetachChildren(); // 不清理的话 GeAttachMgr 的 _FindSkeletonObject()只会找到第一个加入的武器
#endif
        to.SetParent(actorNode.transform, false);
        //Battle.GeUtility.AttachTo(toModel, actorNode);

        // attach缓存清理 / m_Material / m_Animation
        entity.attachmentproxy.Clear();
        ClearCached();
        m_Avatar.Clear((uint)GeAvatar.EAvatarRes.Attach);

        //m_Attachment.ClearAll();
        m_Avatar.Rebind(toModel);
        m_Avatar.SetSkeletonRoot(toModel);
        frModel.CustomActive(false);
        toModel.CustomActive(true); 
        m_Material.RemoveObject(actorPartes);
        m_Material.AppendObject(m_ActorPartes);

        // BUFF 特效移植
        moveBuffsEffects(frModel, toModel);

        // 清理 frModel
        m_ModelManager.RmoveModel(frModel);

        // 动作回复
        m_Avatar.ReplayAction();

        //收集发光材质
        _CollectEmissiveRenderer();

        _updateChapterHeadInfoBarPosition();
        _updateHeadDialogPosition();
        _updateHPHeadBarPostion();

        return true;
#if ENABLE_PROFILER
        }
#endif
    }
    private void _updateHeadDialogPosition()
    {
        if (goDialog != null)
        {
            Vector3 localPos = GetOverHeadPosition();
            if (entity != null && entity.GetEntityData() != null && entity.GetEntityData().overHeadHeight > 0f)
            {
                localPos.y = entity.GetEntityData().overHeadHeight;
            }

            localPos.y += 0.45f;
            goDialog.transform.localPosition = localPos;
        }
    }
    public sealed override void DoBackToFront()
    {
        var backMode = isCreatedInBackMode;
        base.DoBackToFront();
        if (entity == null) return;
        if (backMode)
        {
            _RefreshOverHeadPostion();
            if (mBarsRoot != null && entity != null && entity.GetEntityData() != null)
            {
                var localPos = overheadLocalPosition;
                if (entity.GetEntityData() != null && entity.GetEntityData().overHeadHeight > 0f)
                    localPos.y = entity.GetEntityData().overHeadHeight;
                localPos.y += 0.78f;
                mBarsRoot.gameObject.transform.localPosition = localPos;
            }
            _updateChapterHeadInfoBarPosition();
            _updateHPHeadBarPostion();
            _updateHeadDialogPosition();
            if (goHPBarHUD != null)
            {
                var com = goHPBarHUD.GetComponent<ComCharactorHPBar>();
                com.SetIcon(entityHeadIcon, entityHeadIconMaterial);
            }
            if (comHPPKBar != null)
            {
                comHPPKBar.SetIcon(entityHeadIcon, entityHeadIconMaterial);
            }
        }
    }
    public override void Update(int deltaTime, int Mask = 63, float height = 0, int accTime = 0)
    {
        base.Update(deltaTime, Mask, height, accTime);
        UpdateBuffName(deltaTime);
        _UpdateEmissiveColor(deltaTime);
    }
    int timer = 0;
    int index = 0;
    private void UpdateBuffName(int deltaTime)
    {
        if (hpBarManager == null) return;
        hpBarManager.ShowBuffName(mCurrentHpBarId, string.Empty);
        if (index >= hpBarBuffEffectNameList.Count)
        {
            index = 0;
        }
        if (hpBarBuffEffectNameList.Count == 0) return;
        curHpBarBuffName = hpBarBuffEffectNameList[index];
        hpBarManager.ShowBuffName(mCurrentHpBarId, curHpBarBuffName);
        timer += deltaTime;
        if (timer > 1000)
        {
            index++;
            timer = 0;
            if (index >= hpBarBuffEffectNameList.Count)
            {
                index = 0;
            }
        }
    }
#else
    public static void ClearStatic(){}
	public void InitAttachPoint(){}
	public GameObject AttachRelativeToLocator(string resPath, GameObject parent, string locator, Vector3 offset){return null;}
	public void ShowFindPath(bool show = true){}
	public void ShowHeadDialog(string text, bool hide = false,bool bLink = false,bool bUseLink = false, bool useSkill=false,float fLifeTime = 3.50f, bool isPet=false){}
	public void CreateFootIndicator(string effect = null){}
    public void SetFontIndircatorInBackGround(){}
	public void SetFootIndicatorTouchGround(float y){}
	public void SetFootIndicatorVisible(bool flag){}
	public void SetHeadInfoVisible(bool flag){}
	public void CreateMonsterLoop(){}
	public void CreateMonsterInfoBar(string name, PlayerInfoColor infoColor){}
	public void SetTaskMonster(string name){}
	public GameObject CreateHeadText(HitTextType type, object arg=null, bool noattach=false, object arg2=null){return null;}
	public void CreateInfoBar(string name, PlayerInfoColor infoColor, ushort RoleLevel, string namecolors = null){}
	public void ShowTranport(bool flag){}
	public void CreatePropertyRiseEffect(string content){}
	public void UpdateLevel(int iLevel){}
	public void UpdateDialogComponet(float delta){}
	public void UpdateNpcInteraction(float delta){}
	public void UpdateVoiceComponent(float delta){}
	public void AddVoiceListener(NpcVoiceComponent.SoundEffectType eSoundEffect){}
	public void AddComponentDialog(System.Int32 iDialogID = 2006, NpcDialogComponent.IdBelong2 eIdBelong2 = NpcDialogComponent.IdBelong2.IdBelong2_NpcTable){}
	public void AddNPCBoxCollider(BeTownNPCData townData){}
	public void AddNpcInteraction(int npcID,UInt64 guid = 0){}

    public void AddNPCFunction(string name, PlayerInfoColor infoColor, ushort RoleLevel, string namecolors = null,float nameLocalPosY = 0.0f) { }
    public void AddTittleComponent(System.Int32 iTittleID,
        string name,
        byte guildDuty,
        string bangName,
        System.Int32 iRoleLv,
        int a_nPKRank,
        PlayerInfoColor color,
        string adventTeamName = "",
        PlayerWearedTitleInfo   playerWearedTitleInfo=null,
        int guileLv = 0,
        int iVipLevel = 0){}
	public void OnLevelChanged(System.Int32 iRoleLv){}
	public void OnTittleChanged(System.Int32 iTittle){}
	public void OnGuildNameChanged(string name){}
	public void OnGuildPostChanged(byte duty){}
	public void UpdatePkRank(int a_nPKRank, int a_nStar){}
	public void UpdateName(string a_strName){}
    public void UpdateAdventTeamName(string a_strName) {}

    public void UpdateTitleName(Protocol.PlayerWearedTitleInfo a_strName) { }

    public void UpdateGuidLv(int guileLv) { }

    public void AddNpcVoiceComponent(System.Int32 iNpcID){}
	public void AddNpcArrowComponent(){}
	public void ActiveArrow(){}
	public void DeActiveArrow(){}
	public void UpdateInfoBarLevel(ushort RoleLevel, bool force = true){}
	public void UpdateInfoBarPKPoints(uint pkPoints){}
	public void CreateOverHeadHpBar(eHpBarType type, bool enemy=true, bool isShow = true){}
	public void CreateSpellBar(){}
	public void SetSpellBar(float progress){}
	public void StopSpellBar(){}
	public void CreateComboTips(int[] skills){}
	public void RemoveComboTips(){}
	public IDungeonCharactorBar CreateBar(eDungeonCharactorBar type){return null;}
	public void SetBar(eDungeonCharactorBar type, float rate, bool show=true){}
	public void StopBar(eDungeonCharactorBar type, bool iscancel){}
	public void CreatePKHPBar(CPKHPBar.PKBarType type, string name, PlayerInfoColor color){}
	public void CreateHPBarCharactor(int seat = 0){}
    public void DestroySelfHPBar() { }
    public void RemoveHPBarMonster(){}
	public void CreateHPBarMonster(ProtoTable.UnitTable.eType type, string name, Color nameColor, int singleBarValue = -1, bool enemy=true){}
	public void SetDebugDrawData(BDEntityActionFrameData data, float scale = 1.0f, float zDimScale=1.0f){}
	public void ResetHPBar(){}
	public void UpdateHPBar(int level){}
	public void SetHPDamage(int value, HitTextType type = HitTextType.NORMAL){}
	public void SetHPValue(float percent){}
	public void SetMpValue(float percent){}
	public void SyncHPBar(){}
	public void CreateAsyncForTownNPC(int resID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool needChangeMaterial = true, bool usePool = true, PosLoadGeActorEx load = null){}
	public void RecreateForProjectile(bool useCube=false){}
	public void ReleaseForProjectile(bool useCube=false){}
    public void DestroyEffectByName(string name) { }

    public bool CreateAsync(int resID, GameObject entityRoot, GeSceneEx scene, int iUnitId, PosLoadGeActorEx postLoadCallback, bool isBattleActor = true, bool usePool = true, bool useCube = false)
    {		
        m_ActorDesc.resID = resID;
		var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
		if (resData != null)
		{
			m_ActorDesc.name = resData.ParentName;
		}

		return false;
    }
	public bool Create(int resID, GameObject entityRoot, GeSceneEx scene, int iUnitId, bool needChangeMaterial = true, bool usePool = true, bool useCube = false,bool needBackCreate = false){
		m_ActorDesc.resID = resID;
		var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
		if (resData != null)
		{
			m_ActorDesc.name = resData.ParentName;
		}

		return false;
	}
    public void SetProfessionId(int prodid){ }
	public void RemoveHPBar(){}
	public void Destroy(){}
	public bool SetMaterial(string shaderName){return false;}
	protected void _ClearMaterial(){}
	public string GetResPath(){return "";}
	public string GetResName(){return "";}
	public void SetHighLight(bool hightLight){}
	public void SetDyeColor(Color dyeColor, GameObject[] modelRoot){}
	public void SetShadowVisible(GeSceneEx scene, bool visible){}
	public void SetActorVisible(bool visible){}
	public void SetActorForLowLevel(){}
	public void SetHPBarVisible(bool visible){}
	public void EquipFashions(Dictionary<int, string> fashions){}
	public void HackRamp(Texture rampTex){}
	public void ShowProtectFloat(bool show, float percent=0f){}
	public void ShowProtectGround(bool show, float percent=0f){}
	public void ShowProtectStand(bool show, float percent=0f){}
	public void AddKillMark(){}
	public void HideActor(bool bIsHide){}
	public bool IsActorHide(){return false;}
	public void PreChangeModel(int resID){}
	public void TryChangeModel(int resID){}
	public bool ChangeModel(GameObject toModel){return true;}
	public void CreateStateBar(string text, int duration){}
	public void RemoveStateBar(){}
#endif


    //#endregion

#if LOGIC_SERVER
    public void LoadOneSkillConfig(string path, BDEntityRes res, BeActionFrameMgr frameMgr, SkillFileListCache fileCache, List<string> configList = null, int tag = 0, List<int> types = null)
#else
    public void LoadOneSkillConfig(string path, BDEntityRes res, List<string> configList = null, int tag = 0, List<int> types = null)
#endif
    {
		var loadedList = GamePool.ListPool<BDEntityActionInfo>.Get();
#if LOGIC_SERVER
        BDEntityActionInfo.SaveLoad(entity.CurrentBeBattle.GetBattleType(), path, configList, false, false, loadedList, null, frameMgr, fileCache, types);
#else
        BDEntityActionInfo.SaveLoad(entity.CurrentBeBattle.GetBattleType(), path, configList, false, false, loadedList, null, types);
#endif

        //处理加载完成后的技能配置文件
        for (int i = 0; i < loadedList.Count; ++i)
		{
			var current = loadedList[i];
            current.weaponTag = tag;

            res.AddActionInfo(current, path);
		}
#if MG_TEST
        if (entity != null)
        {
            BeActor actor = entity as BeActor;
            if (actor != null && (actor.professionID == 40|| actor.professionID == 41 || actor.professionID == 42))
            {
                if (!res.HasAction("Jump_down_loop"))
                {
                    if (actor.IsProcessRecord())
                    {
#if LOGIC_SERVER
                        actor.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Jump_down_loop is null, stack {2}", actor.m_iID, actor.GetInfo(),RecordServer.GetStackTraceModelName());
#else
                        actor.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} Jump_down_loop is null", actor.m_iID, actor.GetInfo());
#endif
                    }
                }
            }
        }
#endif
        GamePool.ListPool<BDEntityActionInfo>.Release(loadedList);
	}

    public void LoadSkillConfig(BDEntityRes res, bool loadCommonSkill = false, List<string> configList = null, int resID = 0, bool loadEx = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.LoadSkillConfig"))
        {
#endif
        if (resID == 0)
            resID = m_ActorDesc.resID;

        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);

        if (resData == null)
        {
            if (resID != 0)
            {
                Logger.LogErrorFormat("模型资源表 中没有ID为 {0} 的项目", resID);
            }
            return;
        }
        BeActionFrameMgr frameMgr = null;
        SkillFileListCache fileCache = null;
        if(entity != null && entity.CurrentBeScene != null)
        {
            frameMgr = entity.CurrentBeScene.ActionFrameMgr;
            fileCache = entity.CurrentBeScene.SkillFileCache;
        }
        for (int i=0; i<resData.ActionConfigPath.Count; ++i)
        //foreach (var path in resData.ActionConfigPath)
        {
            var path = resData.ActionConfigPath[i];
            if (loadEx)
            {
                path += "_EX";
            }
            if (Utility.IsStringValid(path))
            {
#if LOGIC_SERVER
                LoadOneSkillConfig(path, res, frameMgr, fileCache, configList);
#else
                LoadOneSkillConfig(path, res,configList);
#endif
            }
        }

        if (loadCommonSkill)
        {
#if LOGIC_SERVER
            LoadOneSkillConfig("Data/SkillData/Common", res, frameMgr, fileCache);
#else
            LoadOneSkillConfig("Data/SkillData/Common", res);
#endif
        }
#if ENABLE_PROFILER
        }
#endif
    }
    public void LoadWeaponRelatedConfig(BDEntityRes res, int tag, List<string> configList = null, List<int> types = null)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.LoadWeaponRelatedConfig"))
        {
#endif
		if (tag == 0)
			return;

		var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(m_ActorDesc.resID);
		if (resData == null)
		{
			if (m_ActorDesc.resID != 0)
			{
				Logger.LogErrorFormat("模型资源表 中没有ID为 {0} 的项目", m_ActorDesc.resID);
			}
			return;
		}
        BeActionFrameMgr frameMgr = null;
        SkillFileListCache fileCache = null;
        if (entity != null && entity.CurrentBeScene != null)
        {
            frameMgr = entity.CurrentBeScene.ActionFrameMgr;
            fileCache = entity.CurrentBeScene.SkillFileCache;
        }
        for (int i=0; i<resData.ActionConfigPath.Count; ++i)
		{
			string path = resData.ActionConfigPath[i];
			if (Utility.IsStringValid(path))
			{
				string newPath = path+"_"+tag;
                //Logger.LogErrorFormat("LoadWeaponRelatedConfig {0}", newPath);
#if LOGIC_SERVER
                LoadOneSkillConfig(newPath, res, frameMgr, fileCache, configList, tag, types);
#else
                LoadOneSkillConfig(newPath, res,configList, tag, types);
#endif
            }
		}
#if ENABLE_PROFILER
        }
#endif
	}
    public string GetSkillDataNameByID(int ID)
    {
        if (skillData.ContainsKey(ID))
            return skillData[ID];

        Logger.LogWarningFormat("Actor do not have Skill {0}",ID);
        return null;
    }

    #region 滑动方向箭头
    public ComCommonBind GetArrowBind(string path)
    {
        #if !LOGIC_SERVER
        if (slideArrowBind != null)
            return slideArrowBind;
        GameObject obj = AssetLoader.instance.LoadResAsGameObject(path);
        if (obj == null)
            return null;
        GameObject attachNode = GetAttachNode("[actor]Orign");
        if (attachNode == null)
            return null;
        Battle.GeUtility.AttachTo(obj, attachNode);
        slideArrowBind = obj.GetComponent<ComCommonBind>();
        return slideArrowBind;
#else
        return null;
#endif
    }
    
    public ComCommonBind GetForwardBackArrowBind(string path)
    {
#if !LOGIC_SERVER
        if (forwardBackArrowBind != null)
            return forwardBackArrowBind;
        GameObject obj = AssetLoader.instance.LoadResAsGameObject(path);
        if (obj == null)
            return null;
        GameObject attachNode = GetAttachNode("[actor]Orign");
        if (attachNode == null)
            return null;
        Battle.GeUtility.AttachTo(obj, attachNode);
        forwardBackArrowBind = obj.GetComponent<ComCommonBind>();
        return forwardBackArrowBind;
#else
        return null;
#endif
    }
    #endregion

    #region 用特效连接两个角色

    private GeEffectEx chainEffect = null;
    private GameObject chainNode = null;

    /// <summary>
    /// 闯将两个角色之间的特效连线
    /// </summary>
    /// <param name="target">目标角色</param>
    /// <param name="path">特效路径</param>
    public void CreateChainEffect(BeActor target,string path)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.CreateChainEffect"))
        {
#endif
#if !LOGIC_SERVER
        chainEffect = CreateEffect(path, null, 99999, Vec3.zero);
        var goEffect = chainEffect.GetRootNode();

        bool noStrNode = false;
        BeActor actor = entity as BeActor;
        if (actor == null)
            return;
        var go = GetAttachGameObject(actor, "[actor]Body", ref noStrNode);

        Battle.GeUtility.AttachTo(goEffect, go);

        if (noStrNode)
        {
            var effPos = chainEffect.GetPosition();
            effPos.z += VInt.Float2VIntValue(1.5f);
            chainEffect.SetPosition(effPos);
        }

        var bind = goEffect.GetComponentInChildren<ComCommonBind>();
        if (bind != null)
        {
            var com = bind.GetCom<LightningChain>("lcScript");
            if (com != null)
            {
                var origin = target.m_pkGeActor.GetAttachNode("[actor]Orign");
                if (origin != null)
                {
                    if (chainNode == null)
                    {
                        chainNode = new GameObject("Node");
                    }
                    Utility.AttachTo(chainNode, origin);
                    chainNode.transform.localPosition = new Vector3(0, 1, 0);
                    com.target = chainNode;
                    com.ForceUpdate();
                }

            }

            GameObject goNodeA = bind.GetGameObject("goNodeA");
            if (goNodeA != null)
            {
                var comOffset = goNodeA.GetComponent<OffsetChange>();
                if (comOffset == null)
                {
                    comOffset = goNodeA.AddComponent<OffsetChange>();
                    comOffset.LoopCount = 0;
                    comOffset.AStartTime = 0;
                    comOffset.AXSpeed = -5;
                    comOffset.AYSpeed = 0;
                    comOffset.BStartTime = 0;
                    comOffset.BXSpeed = 0;
                    comOffset.BYSpeed = 0;
                }
            }
        }
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 移除两个角色之间的特效连线
    /// </summary>
    public void ClearChainEffect()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]GeActorEx.ClearChainEffect"))
        {
#endif
#if !LOGIC_SERVER
        if (chainEffect != null)
        {
            var obj = chainEffect.GetRootNode();

            if (obj != null)
            {
                var bind = obj.GetComponentInChildren<ComCommonBind>();
                if (bind != null)
                {
                    var com = bind.GetCom<LightningChain>("lcScript");
                    com.target = null;
                    com.SetVertexCount(0);
                }
            }
            DestroyEffect(chainEffect);
        }
        if (chainNode != null)
        {
            GameObject.Destroy(chainNode);
            chainNode = null;
        }
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    protected GameObject GetAttachGameObject(BeActor actor, string nodeName, ref bool noStrNode)
    {
#if !LOGIC_SERVER
        var attachRoot = GetAttachNode(nodeName);
        if (attachRoot == null)
        {
            noStrNode = true;
            attachRoot = actor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        }
        return attachRoot;
#else
        return null;
#endif
    }
    #endregion
}
