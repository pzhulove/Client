using UnityEngine;
using System.Collections.Generic;
using System;
using ProtoTable;
using DG.Tweening;
using GameClient;
using UnityEngine.UI;

public enum BeRegionState
{
    None = 0,
    In,
    Out,
    Over,
};

public class BeRegionTarget
{
    protected BeActor mTarget;
    protected BattlePlayer mBattlePlayer;
    protected BeRegionState mState;
    protected BeRegionState mLastState;
    protected bool mRemoved;
    protected SceneRegionTable.eType mType;
    public bool removed
    {
        get
        {
            return mRemoved;
        }

        set
        {
            mRemoved = value;
        }
    }

    public SceneRegionTable.eType type
    {
        get
        {
            return mType;
        }

        set
        {
            mType = value;
        }
    }

    public BattlePlayer battlePlayer
    {
        get
        {
            return mBattlePlayer;
        }

        set
        {
            mBattlePlayer = value;
        }
    }

    public BeActor target
    {
        get
        {
            return mTarget;
        }

        set
        {
            mTarget = value;
        }
    }

    public BeRegionState lastState
    {
        get
        {
            return mLastState;
        }
    }

    public bool isStateChanged
    {
        get
        {
            // TODO 这里这样改有可能出现问题
            if (mLastState == BeRegionState.Over)
            {
                mLastState = mState;
                return false;
            }

            return mLastState != mState;
        }
    }

    public BeRegionState state
    {
        get
        {
            return mState;
        }

        set
        {
            mLastState = mState;
            mState = value;

#if !LOGIC_SERVER
            if (target != null && BattleMain.instance.GetPlayerManager().GetAllPlayers().Count > 1 && type== SceneRegionTable.eType.DOOR)
            {
                switch (mState)
                {
                    case BeRegionState.In:
                        target.ShowTransport(true);
                        break;
                    case BeRegionState.Out:
                    case BeRegionState.None:
                        target.ShowTransport(false);
                        break;

                }
            }
#endif
        }
    }
}


public class BeRegionBase
{
    public delegate bool TriggerRegion(ISceneRegionInfoData info, BeRegionTarget target);
    public delegate List<BattlePlayer> TriggerTarget();

    protected TriggerRegion mTriggerRegion;
    protected TriggerRegion mTriggerRegionOut;
    private TriggerTarget mTriggerTargetList;

    protected List<BeRegionTarget> mTargetList = new List<BeRegionTarget>();

    protected VInt3 mPosition; //逻辑位置
    protected VInt mScale = VInt.one;

    protected bool mActive = false;
    protected bool mIsActiveEffect = false;
    protected bool mCanRemove;

    protected ISceneRegionInfoData mRegionInfo;

    protected BeScene mCurrentBeScene;

#if !SERVER_LOGIC 

    protected GeActorEx mGeActor;

    protected GeEffectEx mActiveEffect;

    protected GameObject mTrailEff;

#endif


    protected SceneRegionTable mRegionTable;

    protected bool mIsBoss;

    private int mResID;

    #region Getter & Setter
    public int resID
    {
        get
        {
            return mResID;
        }
    }

#if !SERVER_LOGIC

    public GeActorEx geActor
    {
        get
        {
            return mGeActor;
        }
    }

#endif


    public VInt3 position
    {
        get
        {
            return mPosition;
        }
    }

    public void PlaySound(int audioID)
    {
        if (currentBeScene != null && currentBeScene.mBattle != null)
            currentBeScene.mBattle.PlaySound(audioID);
    }

    public void SetScale(VInt scale)
    {
        mScale = scale;

#if !SERVER_LOGIC 

        if (mGeActor != null)
        {
            mGeActor.SetScale(scale.scalar);
        }

#endif

    }

    public void SetPosition(VInt3 vec)
    {
        mPosition = vec;

#if !SERVER_LOGIC 

        if (mGeActor != null)
        {
            mGeActor.SetPosition(vec.vector3);
            UpdateGeActorPos(vec.vector3);
        }

#endif

    }

    public bool active
    {
        get
        {
            return mActive;
        }

        set
        {
            if (value != mActive)
            {
                mActive = value;
            }
        }
    }

    public bool activeEffect
    {
        get
        {
            return mIsActiveEffect;
        }

        set
        {
            if (value != mIsActiveEffect)
            {
                mIsActiveEffect = value;

                if (mIsActiveEffect)
                {
                    _unloadActivedEffect();
                    _loadActivedEffect();
                }
                else
                {
                    _unloadActivedEffect();
                }
            }
        }
    }

    public bool canRemove
    {
        get
        {
            return mCanRemove;
        }
    }

    public ISceneRegionInfoData regionInfo
    {
        private set
        {
            mRegionInfo = value;
        }

        get
        {
            return mRegionInfo;
        }
    }

    public TriggerRegion triggerRegion
    {
        set
        {
            mTriggerRegion = value;
        }
    }

    public TriggerRegion triggerRegionOut
    {
        set
        {
            mTriggerRegionOut = value;
        }
    }

    public TriggerTarget triggerTarget
    {
        set
        {
            mTriggerTargetList = value;
        }
    }

    public BeScene currentBeScene
    {
        get
        {
            if (mCurrentBeScene == null)
            {
                Logger.LogError("currentBeScene is nil");
            }
            return mCurrentBeScene;
        }
    }

    public void SetBeScene(BeScene scene)
    {
        mCurrentBeScene = scene;
    }
    #endregion

    #region Load
    private void _loadData(int id)
    {
        mRegionTable = TableManager.GetInstance().GetTableItem<SceneRegionTable>(id);
        if (mRegionTable == null)
        {
            Logger.LogErrorFormat("sceneregion table not contain the item with id {0}", id);
        }
    }
    #endregion

    protected void _loadEffect()
    {
        mResID = mRegionTable.ResID;

        if (mIsBoss && mRegionTable.ReplaceResID > 0)
        {
            mResID = mRegionTable.ReplaceResID;
        }

#if !SERVER_LOGIC 
        mGeActor = currentBeScene.currentGeScene.CreateActor(mResID, 0, 0, true, false, true, false, false);
        if (mGeActor == null)
        {
            Logger.LogErrorFormat("create actor is nil with res id {0} current room:{1}", mResID, BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentScenePath());
        }
#endif

        _onLoadActorFinishCB();
    }

    private void _onLoadActorFinishCB()
    {
        _onLoadActorFinish();
    }

    protected virtual void _onLoadActorFinish()
    {

    }
    protected virtual void _unloadEffect()
    {
#if !SERVER_LOGIC 

        if (mGeActor != null)
        {
            currentBeScene.currentGeScene.DestroyActor(mGeActor);
            mGeActor = null;
        }

#endif

    }

    protected virtual void _loadActivedEffect()
    {
#if !SERVER_LOGIC 

        var node = new DAssetObject();
        node.m_AssetPath = mRegionTable.ActivedEffect;
        EffectsFrames effectFrames = new EffectsFrames();
        effectFrames.attachname = "[actor]Origin";

        mActiveEffect = mGeActor.CreateEffect(node, effectFrames, 0, mPosition.vec3, 1, 1, true);

#endif

    }

    protected virtual void _unloadActivedEffect()
    {
#if !SERVER_LOGIC 

        if (null != mActiveEffect)
        {
            mGeActor.DestroyEffect(mActiveEffect);
            mActiveEffect = null;
        }

#endif

    }

    protected virtual void UpdateGeActorPos(Vector3 pos)
    {

    }

    public void Create(ISceneRegionInfoData info, bool isReplace = false)
    {
        regionInfo = info;

        mIsBoss = isReplace;

        _loadData(regionInfo.GetEntityInfo().GetResid());

        _loadEffect();

        _onCreate();

    }

    public void Remove()
    {
        _unloadActivedEffect();
        _unloadEffect();

        mCurrentBeScene = null;
    }


    public virtual VInt3 GetRegionPos()
    {
        return mPosition;
    }

    protected virtual bool _isInRegion(BeRegionTarget regionTarget)
    {
        if (null == regionTarget || null == regionTarget.target)
        {
            return false;
        }

        var p = regionTarget.target.GetPosition();
        var regionPos = GetRegionPos();

        int offset = 0;

        return _isPointInRegion(p, regionPos, mRegionInfo, offset);

    }

    private bool _isPointInRegion(VInt3 point, VInt3 regionPos, ISceneRegionInfoData regionInfo, int offset = 0)
    {
        if (regionInfo == null)
        {
            return false;
        }

        if (mRegionInfo.GetRegiontype() == DRegionInfo.RegionType.Circle)
        {
            //   var dis = Vector2.Distance(
            //                 new Vector2(regionPos.x, regionPos.z),
            //                 new Vector2(point.x, point.z));

            VInt2 a = new VInt2(regionPos.x, regionPos.y);
            VInt2 b = new VInt2(point.x, point.y);

            int dis = (a - b).magnitude;                      
            return dis <= (VInt.Float2VIntValue(mRegionInfo.GetRadius()) + offset);
        }
        else if (mRegionInfo.GetRegiontype() == DRegionInfo.RegionType.Rectangle)
        {
            VInt2 min = new VInt2(regionPos.x - (VInt.Float2VIntValue(mRegionInfo.GetRect().x) + offset) / 2, regionPos.y - (VInt.Float2VIntValue(mRegionInfo.GetRect().y) + offset) / 2);
            VInt2 max = new VInt2(regionPos.x + (VInt.Float2VIntValue(mRegionInfo.GetRect().x) + offset) / 2, regionPos.y + (VInt.Float2VIntValue(mRegionInfo.GetRect().y) + offset) / 2);

            if (point.x < min.x) { return false; }
            if (point.z < min.y) { return false; }
            if (point.x > max.x) { return false; }
            if (point.z > max.y) { return false; }

            return true;
        }

        return false;
    }


    private bool _isOverRegion(BeRegionTarget regionTarget)
    {
        if (regionTarget == null || regionTarget.target == null)
        {
            return false;
        }

        return regionTarget.target.GetPosition().z > 0;
    }


    protected virtual void _onUpdate(int delta)
    {

    }

    protected virtual bool _canTriggerIn(BeRegionTarget target)
    {
        if (target.isStateChanged && BeRegionState.In == target.state)
        {
            return true;
        }

        return false;
    }

    protected virtual bool _canTriggerOut(BeRegionTarget target)
    {
        if (target.isStateChanged && BeRegionState.Out == target.state)
        {
            return true;
        }

        return false;
    }

    private void _triggerIn(BeRegionTarget target)
    {
        if (null != mTriggerRegion)
        {
            try
            {
                Logger.LogProcessFormat("mTriggerRegion Out -> In");
                mTriggerRegion(mRegionInfo, target);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("mTriggerRegion Out -> In with error {0}", e.ToString());
            }
        }
    }

    private void _triggerOut(BeRegionTarget target)
    {
        if (null != mTriggerRegionOut)
        {
            try
            {
                Logger.LogProcessFormat("mTriggerRegion In -> Out");
                mTriggerRegionOut(mRegionInfo, target);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("mTriggerRegion In -> Out with error {0}", e.ToString());
            }
        }
    }

    // TODO TEMP
    protected virtual void _tryTriggerIn()
    {

    }

    private bool checkCanRemoveRegionTarget(BeRegionTarget x)
    {
        return x.removed;
    }

    private void _updateTargetList()
    {
        if (null != mTriggerTargetList)
        {
            try
            {
                // tag all element removed:true
                //mTargetList.RemoveAll(x =>
                //{
                //    x.removed = true;
                //    return false;
                //});

                var list = mTriggerTargetList.Invoke();

                if (list == null)
                {
                    Logger.LogProcessFormat("target list is nil");
                    return;
                }

                for (int i = 0; i < mTargetList.Count; ++i)
                {
                    mTargetList[i].removed = true;
                }

                for (int i = 0; i < list.Count; ++i)
                {
                    BattlePlayer player = list[i];

                    if (null != player)
                    {
                        bool hasNotFind = true;
                        for (int j = 0; j < mTargetList.Count; ++j)
                        {
                            if (mTargetList[j].battlePlayer.playerInfo.seat == player.playerInfo.seat)
                            {
                                mTargetList[j].battlePlayer = player;
                                mTargetList[j].target = player.playerActor;
                                mTargetList[j].removed = false;
                                hasNotFind = false;
                                break;
                            }
                        }

                        if (hasNotFind)
                        {
                            BeRegionTarget addUnit = new BeRegionTarget()
                            {
                                state = BeRegionState.None,
                                battlePlayer = player,
                                target = player.playerActor,
                                removed = false,
                                type = mRegionTable.Type
                            };


                            mTargetList.Add(addUnit);

                            // init the state
                            if (_isInRegion(addUnit))
                            {
                                addUnit.state = BeRegionState.In;
                                addUnit.state = BeRegionState.In;
                            }
                            else
                            {
                                addUnit.state = BeRegionState.Out;
                                addUnit.state = BeRegionState.Out;
                            }
                        }
                    }
                }

                for (int i = mTargetList.Count - 1; i >= 0; --i)
                {
                    if (checkCanRemoveRegionTarget(mTargetList[i]))
                    {
                        mTargetList.RemoveAt(i);
                    }
                }

                //mTargetList.RemoveAll(checkCanRemoveRegionTarget);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("tirgger target list error with {0}", e.ToString());
            }
        }
    }

    public void Update(int deltaTime)
    {
#if !SERVER_LOGIC

        if (null != mGeActor)
            mGeActor.Update(deltaTime);

#endif


        if (mCanRemove)
        {
            return;
        }

        if (!mActive)
        {
            return;
        }

        _updateTargetList();

        if (null == mTargetList)
        {
            return;
        }

        for (int i = 0; i < mTargetList.Count; ++i)
        {
            var unit = mTargetList[i];

            switch (unit.state)
            {
                case BeRegionState.None:
                    break;
                case BeRegionState.In:
                    {
                        bool inRegion = _isInRegion(unit);
                        if (!inRegion)
                        {
                            unit.state = BeRegionState.Out;
                            _onExitEffect(unit);

                            if (_canTriggerOut(unit))
                            {
                                _triggerOut(unit);
                            }
                        }
                        else if (_isOverRegion(unit))
                        {
                            unit.state = BeRegionState.Over;
                        }

                    }
                    break;
                //over没用了
                case BeRegionState.Over:
                    {
                        if (!_isOverRegion(unit))
                        {
                            unit.state = BeRegionState.In;
                        }
                    }
                    break;
                case BeRegionState.Out:
                    {
                        if (_isInRegion(unit))
                        {
                            unit.state = BeRegionState.In;

                            //这里hack为全部
                            for (int j = 0; j < mTargetList.Count; ++j)
                            {
                                _onEnterEffect(mTargetList[j]);
                            }

                            if (_canTriggerIn(unit))
                            {
                                _triggerIn(unit);

                            }

                            if (mCanRemove)
                            {
                                Remove();
                                _onRemove();
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        _onUpdate(deltaTime);
        _tryTriggerIn();
    }


    protected virtual void _onCreate()
    {
    }

    protected virtual void _onEnterEffect(BeRegionTarget target)
    {
    }

    protected virtual void _onExitEffect(BeRegionTarget target)
    {
    }

    protected virtual void _onRemove()
    {
    }
}

public class BeRegion : BeRegionBase
{
    protected BeRegionTarget mRegionTarget = new BeRegionTarget();

    protected bool mIsFlow;
    protected int mRate = 0;

    private const float kRateOver = 0.1f;
    private GameObject objDesc = null;


    public int rate
    {
        get
        {
            return mRate;
        }
    }


    public BeActor regionTarget
    {
        set
        {
            mRegionTarget.target = value;
            //mFlowTarget = value;
            mIsFlow = true;
        }
    }

    public BeRegion()
    {
        mCanRemove = false;
        mActive = true;
    }

    protected override void _onCreate()
    {
        if (mRegionTable.Type == SceneRegionTable.eType.BUFF)
        {
#if !LOGIC_SERVER
            //var go = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle_Digit/SpritiBuffText");
            var go = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Battle_Digit/SpritiBuffText", enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
            if (go != null)
            {
                Battle.GeUtility.AttachTo(go, mGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));

                var bind = go.GetComponent<ComCommonBind>();
                if (bind != null)
                {
                    Text txtDesc = bind.GetCom<Text>("txtDesc");
                    if (txtDesc != null)
                        txtDesc.text = mRegionTable.Desc;
                }

                objDesc = go;
            }
#endif
        }
    }

    private void _takeEffect(BeRegionTarget target)
    {
        if (null != target && null != target.target)
        {
            for (int i = 0; i < mRegionTable.EffectID.Count; ++i)
            {
                int effid = mRegionTable.EffectID[i];
                target.target.DealEffectFrame(null, effid);
            }
        }
    }

    private void _untakeEffect(BeRegionTarget target)
    {
        if (null != target &&
            null != target.target &&
            null != target.target.buffController)
        {
            for (int i = 0; i < mRegionTable.EffectID.Count; ++i)
            {
                // todo
                var id = mRegionTable.EffectID[i];
                var item = TableManager.instance.GetTableItem<EffectTable>(id);
                if (null != item)
                {
                    var buff = target.target.buffController.HasBuffByID(item.BuffID);
                    if (null != buff)
                    {
                        buff.Finish();
                    }
                }
            }
        }
    }

    protected override void _onEnterEffect(BeRegionTarget target)
    {
        switch (mRegionTable.Type)
        {
            case SceneRegionTable.eType.BUFF:
                _takeEffect(target);
                mCanRemove = true;
                PlaySound(1012);
                break;
            case SceneRegionTable.eType.TRAP:
                _takeEffect(target);
                break;
            case SceneRegionTable.eType.LOOP:
                break;
            case SceneRegionTable.eType.RIDE:
                mCanRemove = true;
                break;
            default:
                break;
        }
    }
    

    protected override void _onExitEffect(BeRegionTarget target)
    {
        switch (mRegionTable.Type)
        {
            case SceneRegionTable.eType.TRAP:
                _untakeEffect(target);
                break;
        }
    }

    protected override void _onRemove()
    {
        if (objDesc != null)
        {
#if !LOGIC_SERVER
            CGameObjectPool.instance.RecycleGameObject(objDesc);
            //GameObject.Destroy(objDesc);
#endif
            objDesc = null;
        }
    }

    private int mDealTime = 0;

    protected override void _onUpdate(int delta)
    {
        if (SceneRegionTable.eType.LOOP == mRegionTable.Type)
        {
            if (mIsFlow && null != mRegionTarget.target)
            {
                if ((int)EntityLifeState.ELS_CANREMOVE == mRegionTarget.target.GetLifeState())
                {
                    mCanRemove = true;
                    Remove();
                    return;
                }
                else
                {
                    var pos = mRegionTarget.target.GetPosition();
                    pos.z = mPosition.z;
                    SetPosition(pos);
                }
            }

            for (int i = 0; i < mTargetList.Count; ++i)
            {
                var target = mTargetList[i];

                if (BeRegionState.In == target.state)
                {
                    mRate += delta / GlobalLogic.VALUE_10;
                    mRate = Mathf.Min(GlobalLogic.VALUE_1000, mRate);

                    //Logger.LogErrorFormat("rate {0}", mRate);

                    if (mRate >= GlobalLogic.VALUE_1000)
                    {
                        mRate = 0;
                        _takeEffect(target);
                    }
                }
                //else if (BeRegionState.Over == target.state)
                else
                {
                    mRate -= delta / GlobalLogic.VALUE_10;
                    mRate = Mathf.Max(0, mRate);
                }

#if !LOGIC_SERVER
                // TODO
                var battleUIPve = GameClient.BattleUIHelper.GetBattleUIComponent<GameClient.BattleUIPve>();
                if (null != battleUIPve)
                {
                    battleUIPve.SetTipsPercent(mRate / (float)(GlobalLogic.VALUE_1000));
                }
#endif
            }
        }
    }
}

public class BeRegionDropItem : BeRegionBase
{
    private List<int> mPickedList = null;
    private Battle.DungeonDropItem mDropItem;
    private GameClient.ItemData mItemTable = null;
    private bool mInit = false;

    //private string mDropItemExtraDesc = string.Empty;

    //private const int kGoldTypeID = 600000001;

    private bool mIsTrailPickedItem = false;
    private bool mHasPickedItem = false;
    private bool m_needChangeParent = false;

#if !SERVER_LOGIC

    //private GameObject objText;
    private GameObject objEffect;
    private GameObject objEffect2;
#endif


#if !SERVER_LOGIC
    private DropTrail trailObj = null;
    private GameObject dropModel = null;


#endif


    public BeRegionDropItem()
    {
        mCanRemove = false;
        mActive = true;
        mIsTrailPickedItem = false;
        mHasPickedItem = false;
    }

    public void SetPickedList(List<int> list)
    {
        mPickedList = list;
    }

    public void SetDropItem(Battle.DungeonDropItem dropItem)
    {
        mDropItem = dropItem;
        // TEST CODE
        //mDropItem.typeId = 165042005;
    }

    private bool IsGold(int type)
    {
        return type == Global.GOLD_ITEM_ID || type == Global.GOLD_ITEM_ID2;
    }

    private bool checkItemType(int itemId, ProtoTable.ItemTable.eType checkType)
    {
        var res = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(itemId);
        if (null == res)
            return false;

        if (res.Type != checkType)
            return false;

        return true;
    }

    private bool tryToPlayHighQualityEquipShowedSound(ProtoTable.ItemTable res)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem.tryToPlayHighQualityEquipShowedSound"))
        {
#endif
#if !SERVER_LOGIC 

        if (null == res)
            return false;

        if (res.Type != ProtoTable.ItemTable.eType.EQUIP)
            return false;

        ItemTable.eColor curColor = res.Color;
        if (curColor == ItemTable.eColor.PINK)
        {
            PlaySound(104);
        }
        else if (curColor == ItemTable.eColor.YELLOW)
        {
            PlaySound(105);
        }
        else
            return false;


#endif

        return true;
#if ENABLE_PROFILER
        }
#endif
    }


    private string GetGoldName(int type)
    {
        var dropTableItem = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(type);
        if (dropTableItem != null)
            return dropTableItem.Name;

        return "";
    }

    protected Vector3 _backUpPos = Vector3.zero;
    protected List<GameObject> _dropComponentList = new List<GameObject>();
    protected string _iconPath = "UIFlatten/Prefabs/BattleUI/DropItem/DropItemIconRoot";
    protected string _namePath = "UIFlatten/Prefabs/BattleUI/DropItem/DropItemNameRoot";
    protected string _descPath = "UIFlatten/Prefabs/BattleUI/DropItem/DropItemSpecailDescRoot";
    protected string _bgPath = "UIFlatten/Prefabs/BattleUI/DropItem/DropItemTextBgRoot";
    protected string _effectPath = "UIFlatten/Prefabs/BattleUI/DropItem/DropItemEffectRoot";

    /// <summary>
    /// 刷新组件位置
    /// </summary>
    protected override void UpdateGeActorPos(Vector3 pos)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem.UpdateGeActorPos"))
        {
#endif
        if (_backUpPos == pos) return;
        for (int i = 0; i < _dropComponentList.Count; i++)
        {
            var component = _dropComponentList[i];
            if (component != null)
                component.transform.localPosition = pos;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    protected override void _onLoadActorFinish()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._onLoadActorFinish"))
        {
#endif
#if !SERVER_LOGIC
        if (mInit) return;

        _dropComponentList.Clear();

        var icon = CreateDropComponent(_iconPath,SceneUINodeType.DropItem_Icon);
        var name = CreateDropComponent(_namePath, SceneUINodeType.DropItem_Name);
        var desc = CreateDropComponent(_descPath, SceneUINodeType.DropItem_SpecialDesc);
        var bg = CreateDropComponent(_bgPath, SceneUINodeType.DropItem_TextBg);

        var effectRoot = CreateDropComponent(_effectPath, SceneUINodeType.DropItem_Effect);
        //因为特效根节点是从池子里面取出来的  所以使用之前需要将原来的特效移除
        Utility.ClearChild(effectRoot);

        var dropTableItem = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(mDropItem.typeId);
        if (null == dropTableItem) return;

        Image iconImage = null;
        if (icon != null && icon.transform.childCount > 0)
            iconImage = icon.transform.GetChild(0).GetComponent<Image>();
        Text nameText = null;
        if (name != null && name.transform.childCount > 0)
            nameText = name.transform.GetChild(0).GetComponent<Text>();
        Text descText = null;
        if (desc != null && desc.transform.childCount > 0)
            descText = desc.transform.GetChild(0).GetComponent<Text>();
        Image bgImage = null;
        if (bg != null && bg.transform.childCount > 0)
            bgImage = bg.transform.GetChild(0).GetComponent<Image>();

        var dropData = GameClient.ItemDataManager.CreateItemDataFromTable(mDropItem.typeId);
        mItemTable = dropData;

        desc.CustomActive(null != mDropItem && mDropItem.isDouble);
        if (nameText != null)
            nameText.text = string.Format("<color={0}>{1}</color>", dropData.GetQualityInfo().ColStr, dropTableItem.Name);

        bool descDisplay = null != mDropItem && mDropItem.isDouble;
        if (descText != null)
            descText.CustomActive(!descDisplay);

        int bgImageWidth = dropTableItem.Name.Length * 20 + 20;
        int bgImageHeight = descDisplay ? 70 : 30;
        if (bgImage != null)
            bgImage.rectTransform.sizeDelta = new Vector2(bgImageWidth, bgImageHeight);

        if (icon != null && icon.transform.childCount > 0) 
        {
            var dropRoot = icon.transform.GetChild(0);
            if(dropRoot != null)
            {
                dropModel = dropRoot.gameObject;
            }
        }

        string modelPath = dropTableItem.ModelPath;
        if (IsGold(mDropItem.typeId))
        {
            if (mDropItem.num >= 100 && mDropItem.num <= 500)
                modelPath = "UI/Image/Icon/Icon_Item/Drop_Gold2.png:Drop_Gold2";
            else if (mDropItem.num > 500)
                modelPath = "UI/Image/Icon/Icon_Item/Drop_Gold3.png:Drop_Gold3";

            GameClient.ClientSystemManager.GetInstance().delayCaller.DelayCall(800, () =>
            {
                PlaySound(2);
            });

        }
        if (!tryToPlayHighQualityEquipShowedSound(dropTableItem))
        {
            GameClient.ClientSystemManager.GetInstance().delayCaller.DelayCall(800, () =>
            {
                PlaySound(3);
            });
        }
        if (iconImage != null)
            ETCImageLoader.LoadSprite(ref iconImage, modelPath);

        string effectName = GetEffectName(dropTableItem.Type, dropTableItem.Color);
        if (effectName != null)
        {
            var eff = CGameObjectPool.instance.GetGameObject(effectName, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
            objEffect2 = eff;
            Battle.GeUtility.AttachTo(eff, effectRoot);
        }
        mInit = true;
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    private GameObject CreateDropComponent(string path, SceneUINodeType type)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem.CreateDropComponent"))
        {
#endif
#if !SERVER_LOGIC
        var obj = CGameObjectPool.instance.GetGameObject(path, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
        _dropComponentList.Add(obj);
        var regionRootNode = mGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        if (regionRootNode != null)
            obj.transform.localPosition = regionRootNode.transform.localPosition;

        if (currentBeScene != null && currentBeScene.currentGeScene != null)
            currentBeScene.currentGeScene.AttachUIRoot(obj, type);

        return obj;
#else
        return null;
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    private string GetEffectName(ItemTable.eType type, ItemTable.eColor color)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem.GetEffectName"))
        {
#endif
#if !SERVER_LOGIC
        string effectName = null;
        //橙色装备
        if (type == ItemTable.eType.EQUIP && color == ItemTable.eColor.YELLOW)
            effectName = "Effects/Scene_effects/Eff_jipinzhuangbei_dimian_guo";
        else if (type == ItemTable.eType.EQUIP && color == ItemTable.eColor.PINK)
            effectName = "Effects/Scene_effects/Eff_jipinzhuangbei_dimian_guo02";
        else if (color == ItemTable.eColor.PINK)
            effectName = "Effects/Scene_effects/Eff_fensezhuangbei_guo";
        else if (color == ItemTable.eColor.YELLOW)
            effectName = "Effects/Scene_effects/Eff_jipinzhuangbei_guo";
        return effectName;
#else
        return null;
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    protected void ClearDropItemComponents()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem.ClearDropItemComponents"))
        {
#endif
#if !SERVER_LOGIC
        for (int i = 0; i < _dropComponentList.Count; i++)
        {
            var obj = _dropComponentList[i];
            if (obj != null)
                CGameObjectPool.instance.RecycleGameObject(obj);
        }
        _dropComponentList.Clear();
#endif
#if ENABLE_PROFILER
        }
#endif
    }

//    protected override void _onLoadActorFinish()
//    {
//#if !SERVER_LOGIC
//        if (!mInit)
//        {

//            //挂到统一的节点上
//            var rootNode = mGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
//            if (rootNode != null)
//            {
//                Vector3 localPos = rootNode.transform.localPosition;
//                if (mCurrentBeScene != null && mCurrentBeScene.currentGeScene != null)
//                    mCurrentBeScene.currentGeScene.AttachUIRoot(rootNode, SceneUINodeType.DropItemIcon);
//                rootNode.transform.localPosition = localPos;
//            }

//            var node = mGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
//            var obj = Utility.FindGameObject(node, "DungeonBox(Clone)/DungeonBox");

//            if (null != obj)
//            {
//                Utility.ClearChild(obj);
//                var dropTableItem = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(mDropItem.typeId);
//                if (null != dropTableItem)
//                {
//                    var dropData = GameClient.ItemDataManager.CreateItemDataFromTable(mDropItem.typeId);
//                    mItemTable = dropData;

//                    //var textName = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/BattleUI/DungeonBoxText", enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
//                    var textName = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/DungeonBoxText");
//                    var bind = textName.GetComponent<ComCommonBind>();

//                    objText = textName;

//                    Battle.GeUtility.AttachTo(textName, obj);

//                    if (null == bind)
//                    {
//                        return;
//                    }


//                    Text dropName = bind.GetCom<Text>("dropName");
//                    Image dropIcon = bind.GetCom<Image>("dropIcon");
//                    GameObject _dropModel = bind.GetGameObject("dropModel");
//                    GameObject uniformTips = bind.GetGameObject("uniformTips");
//                    Text uniformText = bind.GetCom<Text>("uniformText");


//                    //if (null != uniformText && null != mDropItem && mDropItem.isDouble)
//                    //{
//                    //    mDropItemExtraDesc = uniformText.text;
//                    //}

//                    uniformTips.SetActive(null != mDropItem && mDropItem.isDouble);

//                    dropName.text = string.Format("<color={0}>{1}</color>", dropData.GetQualityInfo().ColStr, dropTableItem.Name);
//                    dropModel = _dropModel;

//                    string modelPath = dropTableItem.ModelPath;
//                    if (IsGold(mDropItem.typeId))
//                    {
//                        if (mDropItem.num >= 100 && mDropItem.num <= 500)
//                            modelPath = "UI/Image/Icon/Icon_Item/Drop_Gold2.png:Drop_Gold2";
//                        else if (mDropItem.num > 500)
//                            modelPath = "UI/Image/Icon/Icon_Item/Drop_Gold3.png:Drop_Gold3";

//                        GameClient.ClientSystemManager.GetInstance().delayCaller.DelayCall(800, () =>
//                        {
//                            AudioManager.instance.PlaySound(2);
//                        });

//                    }
//                    if (tryToPlayHighQualityEquipShowedSound(dropTableItem))
//                    { }
//                    else
//                    {
//                        GameClient.ClientSystemManager.GetInstance().delayCaller.DelayCall(800, () =>
//                        {
//                            AudioManager.instance.PlaySound(3);
//                        });
//                    }

//                    // modelImage.sprite = AssetLoader.instance.LoadRes(modelPath, typeof(Sprite)).obj as Sprite;
//                    ETCImageLoader.LoadSprite(ref dropIcon, modelPath);

//                    //var effect = AssetLoader.instance.LoadResAsGameObject("Effects/Common/Sfx/DiaoLuo/Eff_jinbi_shiqu_guo");
//                    //Utility.AttachTo(effect, obj);
//                    //objEffect = effect;

//                    string effectName = null;

//                    //橙色装备
//                    if (dropTableItem.Type == ItemTable.eType.EQUIP && dropTableItem.Color == ItemTable.eColor.YELLOW)
//                    {
//                        effectName = "Effects/Scene_effects/Eff_jipinzhuangbei_dimian_guo";
//                    }
//                    else if (dropTableItem.Type == ItemTable.eType.EQUIP && dropTableItem.Color == ItemTable.eColor.PINK)
//                    {
//                        effectName = "Effects/Scene_effects/Eff_jipinzhuangbei_dimian_guo02";
//                    }
//                    else if (dropTableItem.Color == ItemTable.eColor.PINK)
//                    {
//                        effectName = "Effects/Scene_effects/Eff_fensezhuangbei_guo";
//                    }
//                    else if (dropTableItem.Color == ItemTable.eColor.YELLOW)
//                    {
//                        effectName = "Effects/Scene_effects/Eff_jipinzhuangbei_guo";
//                    }
//                    if (effectName != null)
//                    {
//                        //var eff = AssetLoader.instance.LoadResAsGameObject(effectName);
//                        var eff = CGameObjectPool.instance.GetGameObject(effectName, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
//                        Battle.GeUtility.AttachTo(eff, obj);
//                        objEffect2 = eff;
//                    }
//                }

//                GeMeshRenderManager.GetInstance().AddMeshObject(node);
//                mInit = true;
//            }
//        }

//#endif

//    }

    protected override void _onUpdate(int delta)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._onUpdate"))
        {
#endif
#if !SERVER_LOGIC

        base._onUpdate(delta);

        if (trailObj != null)
        {
            trailObj.UpdatePosition(delta);
            if (trailObj != null)
            {
                SetPosition(new VInt3(trailObj.position));
            }
        }

        _delayRemoveUpdate(delta);
#endif

#if ENABLE_PROFILER
        }
#endif
    }

    protected override void _onCreate()
    {
        mInit = false;
#region TMP CODE
        // dd: tmp code
        {
        }
#endregion


    }

    private int mDelayRemoveTime = 0;
    private enum eDelayRemoveStatus
    {
        eNone,
        eStart,
        eEnd,
    }

    private eDelayRemoveStatus mDelayStatus = eDelayRemoveStatus.eNone;

    private void _delayRemove(int second)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._delayRemove"))
        {
#endif
        if (mDelayStatus == eDelayRemoveStatus.eNone)
        {
            mDelayStatus = eDelayRemoveStatus.eStart;
            mDelayRemoveTime = second;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    private void _delayRemoveUpdate(int delta)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._delayRemoveUpdate"))
        {
#endif
        if (mDelayStatus != eDelayRemoveStatus.eStart)
        {
            return;
        }

        mDelayRemoveTime -= delta;

        if (mDelayRemoveTime <= 0)
        {
            mDelayStatus = eDelayRemoveStatus.eEnd;

            _realRemoveItem();
        }
#if ENABLE_PROFILER
        }
#endif
    }
   
    private void _realRemoveItem(bool isChangeParent = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._realRemoveItem"))
        {
#endif
        mCanRemove = true;
        m_needChangeParent = isChangeParent;
        _unloadActivedEffect();
        _unloadEffect();
        _onRemove();
#if ENABLE_PROFILER
        }
#endif
    }
    public void RemoveAll()
    {
        _realRemoveItem();
    }

    protected override void _unloadEffect()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._unloadEffect"))
        {
#endif
#if !SERVER_LOGIC
        if (m_needChangeParent)
        {
            //if (objText != null)
            //{
            //    objText.transform.SetParent(null);
            //}

            if (objEffect != null)
            {
                objEffect.transform.SetParent(null);
            }
            if (objEffect2 != null)
            {
                objEffect2.transform.SetParent(null);
            }

            ClearDropItemComponents();
        }
        base._unloadEffect();
#endif

#if ENABLE_PROFILER
        }
#endif
    }

    protected override void _onEnterEffect(BeRegionTarget target)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._onEnterEffect"))
        {
#endif
        mHasPickedItem = true;

#if !LOGIC_SERVER
        try
        {
            if (null != target && null != target.target)
            {
                var battleUI = GameClient.BattleUIHelper.GetBattleUIComponent<GameClient.BattleUIDungeonMap>();
                if (null != battleUI)
                {
                    var pos = Vector3.zero;
                    bool isGold = false;
                    if (IsGold(mDropItem.typeId))
                    {
                        isGold = true;
                        //battleUI.dungeonMapCom.AddGlod(mDropItem.num);

                        //!! GetOwner 
                        BeActor actor = target.target.GetOwner() as BeActor;

                        var go = mGeActor.CreateHeadText(GameClient.HitTextType.GET_GOLD, mDropItem.num, true, GetGoldName(mDropItem.typeId));

                        if (go != null && actor != null)
                        {
                            var p = go.transform.position;
                            p.z = actor.GetGePosition().z - 2f;
                            p.y += 0.5f;
                            go.transform.position = p;
                        }

                        currentBeScene.TriggerEventNew(BeEventSceneType.onPickGold, new EventParam()
                        {
                            m_Int = mDropItem.num,
                            m_Vector = position.vector3
                        });
                    }
                    else
                    {
                        //battleUI.dungeonMapCom.AddBox(mDropItem.num);
                    }

                    target.state = BeRegionState.None;

                    var node = mGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                    //var effect = AssetLoader.instance.LoadResAsGameObject("Effects/Common/Sfx/DiaoLuo/Eff_jinbi_tuowei");
                    mTrailEff = CGameObjectPool.instance.GetGameObject(_getPickedEffectPath(), enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
                    Battle.GeUtility.AttachTo(mTrailEff, node);
                    GameObject goActive = Utility.FindThatChild("actor", node);
                    if (goActive)
                    {
                        ClearDropItemComponents();
                        goActive.gameObject.SetActive(false);
                    }

                    if (mIsTrailPickedItem)
                    {
                        var comp = node.AddComponent<TweenPosArc>();
                        var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                        comp.targetPosition = mainPlayer.playerActor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                        comp.onFinish.AddListener(() =>
                        {
                            _realRemoveItem(comp != null ? comp.isActiveAndEnabled :false);
                            if (isGold)
                            {
                                PlaySound(100);
                            }
                            else
                            {
                                PlaySound(4);
                            }
                        });
                    }
                    else
                    {
                        _delayRemove(_getParticleSystemLength(node.transform));

                        if (isGold)
                        {
                            ClearDropItemComponents();
                            goActive.gameObject.SetActive(false);
                        }
                    }
                    
                    // 武器拾取音效(直接播放)
                    if (checkItemType(mDropItem.typeId, ProtoTable.ItemTable.eType.EQUIP))
                    {
                        PlaySound(101);
                    }
                }
            }

            if (null != mItemTable && null != mItemTable.GetQualityInfo() && !IsGold(mDropItem.typeId))
            {
                GameClient.SystemNotifyManager.SystemNotify(6003, (int)mItemTable.TableID, mItemTable.Name, mDropItem.num);
            }
            /*		else if (mDropItem.typeId != kGoldTypeID)
                            {
                                Logger.LogErrorFormat("drop item error with id {0}", mDropItem.typeId);
                            }*/
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat(e.ToString());
        }
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    static int _getParticleSystemLength(Transform transform)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._getParticleSystemLength"))
        {
#endif
        ParticleSystem[] particleSystems = transform.GetComponentsInChildren<ParticleSystem>();

        float maxDuration = 0;

        for (int i = 0; i < particleSystems.Length; ++i)
        {
            ParticleSystem ps = particleSystems[i];

            if (ps.enableEmission)
            {
                if (ps.loop)
                {
                    return int.MaxValue;
                }

                float dunration = 0f;

                if (ps.emissionRate <= 0)
                {
                    dunration = ps.startDelay + ps.startLifetime;
                }
                else
                {
                    dunration = ps.startDelay + ps.duration + ps.startLifetime;
                }
                if (dunration > maxDuration)
                {
                    maxDuration = dunration;
                }
            }
        }

        return (int)(maxDuration * 1000);
#if ENABLE_PROFILER
        }
#endif
    }

    private static string[] kGoundPickedEffects =
    {
        "Effects/Common/Sfx/DiaoLuo/Eff_jinbi_yuandi",
        "Effects/Common/Sfx/DiaoLuo/Eff_putong_yuandi",
        "Effects/Common/Sfx/DiaoLuo/Eff_jinse_yuandi",
        "Effects/Common/Sfx/DiaoLuo/Eff_fense_yuandi",
    };

    private static string[] kTrailPickedEffects =
    {
        "Effects/Common/Sfx/DiaoLuo/Eff_jinbi_tuowei",
        "Effects/Common/Sfx/DiaoLuo/Eff_putong_tuowei",
        "Effects/Common/Sfx/DiaoLuo/Eff_jinse_tuowei",
        "Effects/Common/Sfx/DiaoLuo/Eff_fense_tuowei",
    };

    private string _getPickedEffectPath()
    {
        string[] allEffects = _getPickedEffectArray();

        if (null == allEffects)
        {
            return string.Empty;
        }

        int index = _getPickedEffectIndex();

        if (index < 0 || index >= allEffects.Length)
        {
            return string.Empty;
        }

        return allEffects[index];
    }

    private int _getPickedEffectIndex()
    {
        if (null == mDropItem)
        {
            return -1;
        }

        if (IsGold(mDropItem.typeId))
        {
            return 0;
        }
        var dropTableItem = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(mDropItem.typeId);

        if (null == dropTableItem)
        {
            return -1;
        }

        if (dropTableItem.Type == ItemTable.eType.EQUIP && dropTableItem.Color == ItemTable.eColor.YELLOW)
        {
            return 2;
        }
        else if (dropTableItem.Type == ItemTable.eType.EQUIP && dropTableItem.Color == ItemTable.eColor.PINK)
        {
            return 3;
        }
        else if (dropTableItem.Color == ItemTable.eColor.PINK)
        {
            return 3;
        }
        else if (dropTableItem.Color == ItemTable.eColor.YELLOW)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private string[] _getPickedEffectArray()
    {
        if (mIsTrailPickedItem)
        {
            return kTrailPickedEffects;
        }
        else
        {
            return kGoundPickedEffects;
        }
    }

    private void _playPickedSound()
    {

    }

    public void ForceTriggerEnter(BeActor actor)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem.ForceTriggerEnter"))
        {
#endif
        mIsTrailPickedItem = true;

        if (mHasPickedItem)
        {
            return ;
        }

        for (int i = 0; i < mTargetList.Count; ++i)
        {
            if (mTargetList[i].target == actor)
            {
                try
                {
                    _onEnterEffect(mTargetList[i]);
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat(e.ToString());
                }
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    protected override void _onRemove()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem._onRemove"))
        {
#endif
        base._onRemove();
#if !SERVER_LOGIC

        //if (objText != null)
        //{
        //    //CGameObjectPool.instance.RecycleGameObject(objText);
        //    GameObject.Destroy(objText);
        //    objText = null;
        //}

        if (objEffect != null)
        {
            if (m_needChangeParent)
                CGameObjectPool.instance.RecycleGameObject(objEffect);
            else
                GameObject.Destroy(objEffect);
            objEffect = null;
        }

        if (objEffect2 != null)
        {
            if (m_needChangeParent)
                CGameObjectPool.instance.RecycleGameObject(objEffect2);
            else
                GameObject.Destroy(objEffect2);
            objEffect2 = null;
        }

        if (null != mTrailEff)
        {
            if (m_needChangeParent)
                CGameObjectPool.instance.RecycleGameObject(mTrailEff);
            else
                GameObject.Destroy(mTrailEff);
            mTrailEff = null;
        }
        ClearDropItemComponents();
#endif

#if ENABLE_PROFILER
        }
#endif
    }

    /*
        public void CreateShadow()
        {
            Vector3 scale = Vector3.one;
            Vector4 entityPlane = GeSceneEx.EntityPlane;
            geActor.SetEntityPlane(entityPlane);

            GeSimpleShadowManager.instance.AddShadowObject(geActor.renderObject, entityPlane, scale);
        }*/

    public void StartTrail(VInt3 _orgPos, VInt3 _targetPos)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeRegionDropItem.StartTrail"))
        {
#endif
#if !LOGIC_SERVER
        float hight = 1.0f;//Global.Settings.offset.x;//0.4f;
        float t = 0.5f;//Global.Settings.offset.y;//0.3f;

        Vec3 targetPos = _targetPos.vec3;
        Vec3 orgPos = _orgPos.vec3;

        targetPos.y -= 0.5f;

        float deltX = targetPos.x - orgPos.x;
        float deltY = targetPos.y - orgPos.y;
        float speedx = deltX / (t * 30) * 60 / 3;
        float speedy = deltY / (t * 30) * 60 / 3;
        float speedz = 7.5f;//Global.Settings.offset.z;//7.5f;

        DropTrail trail = new DropTrail();
        trail.currentBeScene = currentBeScene;
        trail.speed = new Vec3(speedx, speedy, speedz);
        trail.acc = new Vec3(0, 0, 35);
        orgPos.z = hight;
        trail.position = orgPos;
        trail.touchGroundDelegate = () =>
        {

            if (null != trailObj)
            {
                SetPosition(new VInt3(trailObj.position));
            }

            trailObj = null;
            if (dropModel != null)
            {
                DG.Tweening.DOTween.Kill(dropModel);
                dropModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        };
        trailObj = trail;
#endif

        //CreateShadow();
#if ENABLE_PROFILER
        }
#endif
    }
}


public class BeRegionTransportDoor : BeRegionBase
{
#if !SERVER_LOGIC
    protected ControlDoorState mControlDoorState;
#endif

    protected ITransportDoorExtraData mTransportDoorData;
    public TransportDoorType doorType = TransportDoorType.None;

    bool m_triggered = false;

    protected bool mIsVisited = false;
    bool transported = false;
    int timerId = -1;
    public bool isEggDoor = false;
    VInt3 regionPos = VInt3.zero;

    public BeRegionTransportDoor()
    {
        mCanRemove = false;
        mActive = false;
    }

    public void Create(ISceneRegionInfoData info, bool isReplace = false, TransportDoorType doorType = TransportDoorType.None)
    {
        base.Create(info, isReplace);
    }

    public void SetVisited(bool isVisited)
    {
        mIsVisited = isVisited;
    }

    public bool IsVisited()
    {
        return mIsVisited;
    }

    public bool IsBoss()
    {
        return mIsBoss;
    }

    public void SetDoorType(TransportDoorType type)
    {
        doorType = type;
        if (mTransportDoorData != null)
        {
            ISceneTransportDoorData ddoor = regionInfo as ISceneTransportDoorData;
            if (null != ddoor)
            {
                regionPos = new VInt3(ddoor.GetRegionInfo().GetEntityInfo().GetPosition());
            }

            regionPos += new VInt3(mTransportDoorData.GetRegionPos(type));
            regionPos.z = 0;
        }

        if (regionInfo != null && regionPos != VInt3.zero)
        {
            regionInfo.SetRegiontype(DRegionInfo.RegionType.Circle);
            regionInfo.SetRadius(1.2f);//Global.Settings.transportDoorRadius;

            //DTransportDoor 
            ISceneTransportDoorData ddoor = regionInfo as ISceneTransportDoorData;
            if (ddoor != null)
            {
                ddoor.SetBirthposition(regionPos);
            }
        }
    }


    protected override void _onUpdate(int delta)
    {
        base._onUpdate(delta);
    }

    public override VInt3 GetRegionPos()
    {
        //regionPos = mControlDoorState.GetRegionPos(doorType);

        if (regionPos != VInt3.zero)
            return regionPos;

        return base.GetRegionPos();
    }

    protected override void _onLoadActorFinish()
    {
#if !SERVER_LOGIC

        if (null == mControlDoorState)
        {
            var node = mGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
            if (null != node)
            {
                mControlDoorState = node.GetComponentInChildren<ControlDoorState>();
                _setDoorState(active);
            }
        }
#endif


        ResTable resTable = TableManager.instance.GetTableItem<ResTable>(resID);
        if (null != resTable)
        {
            string extraDataPath = resTable.ModelPath.Replace(".prefab", "");
            extraDataPath += "_extradata";

            mTransportDoorData = GameClient.DungeonUtility.LoadSceneTransportExtraData(extraDataPath);
        }
        else
        {
            Logger.LogErrorFormat("[TransportDoor] ID {0} 找不到对应模型资源", resID);
        }
    }

    protected override bool _canTriggerIn(BeRegionTarget target)
    {
        return false;
    }

    protected override void _tryTriggerIn()
    {
        bool canTrigger = false;
        bool delay = false;
        // 处理所有人都在触发范围
        //bool stateChanged = false;

        int inRegionCount = 0;
        int allAlivePlayerCount = 0;

        for (int i = 0; i < mTargetList.Count; ++i)
        {
            var unit = mTargetList[i];
            
            if (unit != null && (unit.battlePlayer !=null && unit.battlePlayer.playerActor!=null && !unit.battlePlayer.playerActor.IsDead() && unit.battlePlayer.netState != BattlePlayer.eNetState.Offline))
            {
                allAlivePlayerCount++;
                if (unit.state == BeRegionState.In && unit.isStateChanged)
                    inRegionCount++;
            }        
            /*			if (unit.isStateChanged)
                        {
                            stateChanged = true;
                        }*/
        }

        if (allAlivePlayerCount > 0)
        {
            //所有人都在传送门里
            if (inRegionCount >= allAlivePlayerCount)
            {
                canTrigger = true;
                delay = false;
            }
            else if (inRegionCount >= Global.TEAM_TRANSPORT_NUM)
            {
                canTrigger = true;
                delay = true;
            }
        }


        /*
        for (int i = 0; i < mTargetList.Count; ++i)
        {
            var unit = mTargetList[i];
			if (unit.target != null && !unit.target.IsDead() && unit.state != BeRegionState.In)
            {
                canTrigger = false;
                break;
            }

            if (unit.isStateChanged)
            {
                stateChanged = true;
            }
        }*/

        //var system = GameClient.ClientSystemManager.instance.CurrentSystem as GameClient.ClientSystemBattle;
        if (m_triggered && !canTrigger && currentBeScene.IsShowTransportDoorCount())
        {
            currentBeScene.StopTransportDoorCount(timerId);
            m_triggered = false;
        }

        if (canTrigger && !delay && m_triggered && currentBeScene.IsShowTransportDoorCount())
        {
            currentBeScene.StopTransportDoorCount(timerId);
            m_triggered = false;
        }     
        if (m_triggered == false && canTrigger == true/* && stateChanged*/)
        {
            if (null != mTriggerRegion)
            {
                /*
                try
                {
                    Logger.LogProcessFormat("mTriggerRegion Out -> In");
                    mTriggerRegion(mRegionInfo, mTargetList[0]);
                }
                catch (Exception e)
                {
                    Logger.LogErrorFormat("mTriggerRegion Out -> In with error {0}", e.ToString());
                }*/

                if (!delay)
                    DoTransport();
                else
                {
                    currentBeScene.StartTransportDoorCount(3, () => { DoTransport(); });
                    timerId = currentBeScene.curTransportTimerId;
                }
                //system.StartTransportDoorCount(3, ()=>{DoTransport();});
            }

            m_triggered = true;
        }
    }

    protected void DoTransport()
    {
        if (transported)
            return;
        transported = true;
        //在这里把已经死亡的角色隐藏

        if (mCurrentBeScene == null || mCurrentBeScene.mBattle == null || mCurrentBeScene.mBattle.dungeonPlayerManager == null)
        {
            Logger.LogError("mCurrentBeScene null reference");
            return;
        }
        var players = mCurrentBeScene.mBattle.dungeonPlayerManager.GetAllPlayers();
        if (players == null)
        {
            
            Logger.LogErrorFormat("players null reference {0}", mCurrentBeScene.mBattle.recordServer != null ? mCurrentBeScene.mBattle.recordServer.sessionID:"none");
            return;
        }

        if (players.Count > 1)
        {
            for (int i = 0; i < players.Count; ++i)
            {
                var player = players[i];
                if (player != null && player.playerActor != null && player.playerActor.m_pkGeActor != null && player.playerActor.IsDead())
                    player.playerActor.m_pkGeActor.SetActorVisible(false);
                else
                {
                    if(player == null)
                        Logger.LogErrorFormat("players {0} null reference {1}", i, mCurrentBeScene.mBattle.recordServer != null ? mCurrentBeScene.mBattle.recordServer.sessionID : "none");
                    else if(player.playerActor == null)
                    {
                        Logger.LogErrorFormat("players.playerActor {0} {1} {2} null reference {3}", i, player.GetPlayerName(), player.GetPlayerServerName(), mCurrentBeScene.mBattle.recordServer != null ? mCurrentBeScene.mBattle.recordServer.sessionID : "none");
                    }
                    else if(player.playerActor.m_pkGeActor == null)
                    {
                        Logger.LogErrorFormat("player.playerActor.m_pkGeActor {0} {1} {2} null reference {3}", i, player.playerActor.GetName(), player.playerActor.m_iResID, mCurrentBeScene.mBattle.recordServer != null ? mCurrentBeScene.mBattle.recordServer.sessionID : "none");
                    }
                }
            }
        }
        if (mTriggerRegion == null)
        {
            Logger.LogErrorFormat("mTriggerRegion null reference {0}", mCurrentBeScene.mBattle.recordServer != null ? mCurrentBeScene.mBattle.recordServer.sessionID : "none");
            return;
        }

        bool ret = mTriggerRegion(mRegionInfo, mTargetList[0]);
        if (!ret)
        {
            transported = false;
            m_triggered = false;
            for (int i = 0; i < mTargetList.Count; ++i)
            {
                var unit = mTargetList[i];
                if (unit != null)
                    unit.state = BeRegionState.In;
            }
        }

    }

    protected override void _onCreate()
    {
    }

    private GeEffectEx mEffect = null;

    private void _addActiveEffect()
    {
        _deleteActiveEffect();

#if !SERVER_LOGIC

        var pos = position;
        pos.z += VInt.one.i * 3;
        mEffect = mGeActor.CreateEffect(5, pos.vec3);

#endif

    }

    private void _deleteActiveEffect()
    {
#if !SERVER_LOGIC

        if (null != mEffect)
        {
            mGeActor.DestroyEffect(mEffect);
            mEffect = null;
        }

#endif

    }

    private void _setDoorState(bool isActived)
    {
#if !SERVER_LOGIC

        if (null != mControlDoorState)
        {
            if (isActived)
            {
                _addActiveEffect();
                mControlDoorState.OpenDoor(doorType);
            }
            else
            {
                _deleteActiveEffect();
                mControlDoorState.CloseDoor();
            }
        }
#endif
    }


    protected override void _loadActivedEffect()
    {
        base._loadActivedEffect();
        _setDoorState(true);
    }

    protected override void _unloadActivedEffect()
    {
        base._unloadActivedEffect();
        _setDoorState(false);
    }

    //检查是否在传送门范围内 并且可以传送
    public bool CheckInDoorRange(BeActor actor)
    {
        bool isRange = false;
        for (int i = 0; i < mTargetList.Count; ++i)
        {
            var unit = mTargetList[i];
            if (unit != null && (!unit.battlePlayer.playerActor.IsDead() && unit.battlePlayer.netState != BattlePlayer.eNetState.Offline))
            {
                if (unit.battlePlayer.playerActor == actor && unit.state == BeRegionState.In && unit.isStateChanged)
                {
                    isRange = true;
                }
            }
        }
        return isRange;
    }

    
}
