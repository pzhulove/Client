using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;
using Protocol;
using ProtoTable;
using GameClient;
using Network;
using System.Collections.Generic;

public class ComDrugTipsBar : MonoBehaviour
{
    public int quickBuyPotionID = 200001004;
    public ushort quickBuyPotionCount = 10;
    public float quickBuyPotionUseHPRate = 0.6f;

    public float autoUseHpRate = 0.5f;
    public float autoUseMpRate = 0.5f;

    public float fullRate = 0.25f;
    public bool isRoateWithMiddle = false;

    public ComDrug[] mComDrug;
    //public Image[] mImages;

    public Image icon;
    public int mUnitWeidth = 100;

    private int mSelectComDrugIdx = 0;
    private Coroutine UseDrugCoroutine;
    private object comDrugLock = new object();

    //单例
    private static ComDrugTipsBar _instance;
    public static ComDrugTipsBar instance
    {
        get
        {
            return _instance;
        }
    }

    public enum eState
    {
        onExpand,
        onContract,
    }

    public enum UseState
    {
        idle,
        waitingForResult,
    }

    private eState mState = eState.onContract;
    private UseState useStat = UseState.idle;

    private List<ComDrug> mCacheDrugs = new List<ComDrug>();

    public void Init()
    {
        var localPlayer = BattleMain.instance.GetLocalPlayer();
        if (localPlayer == null)
        {
            return;
        }

        for (int i = 0; i < mComDrug.Length; ++i)
        {
            if (i < localPlayer.playerInfo.potionPos.Length)
            {
                mComDrug[i].SetItemID((int)localPlayer.playerInfo.potionPos[i]);

                var leftCount = localPlayer.UseItemById(mComDrug[i].mItemId, 0);
                mComDrug[i].SetCount(leftCount);
            }

            mCacheDrugs.Add(mComDrug[i]);
        }
        mCacheDrugs.Sort();
        SetDrugColumnStat();
    }

    void Awake()
    {
        _instance = this;
        _bindEvents();
    }

    void OnDestroy()
    {
        _unbindEvents();

        mCacheDrugs.Clear();
        
        if (null != mComDrug)
        {
            for (int i = 0; i < mComDrug.Length; ++i)
            {
                if (null != mComDrug[i])
                {
                    InvokeMethod.RmoveInvokeIntervalCall(mComDrug[i]);
                }
            }
        }
        
        Array.Clear(mComDrug, 0, mComDrug.Length);
        icon = null;
    }

    private void _bindEvents()
    {
        Logger.LogProcessFormat("[PostionSet] 绑定事件");

        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonQuickBuyPotionSuccess, _onQuickBuyPostionSuccess);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonQuickBuyPotionFail, _onQuickBuyPostionFail);
    }

    private void _unbindEvents()
    {
        Logger.LogProcessFormat("[PostionSet] 解绑事件");

        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonQuickBuyPotionSuccess, _onQuickBuyPostionSuccess);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonQuickBuyPotionFail, _onQuickBuyPostionFail);
    }

    public void SetDrugcolumnState()
    {
        mComDrug[1].transform.parent.gameObject.CustomActive(true);
        mComDrug[2].transform.parent.gameObject.CustomActive(true);
        icon.CustomActive(false);
        Invoke("SetDrugColumnStat", 5f);
    }
    public void SetDrugColumnStat()
    {
        mComDrug[1].transform.parent.gameObject.CustomActive(false);
        mComDrug[2].transform.parent.gameObject.CustomActive(false);
        icon.CustomActive(true);
    }
    public void SetRootActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void UseHpDrug(bool isAuto)
    {
        if (_isLocalPlayerCanAutoUseHpDrugs(isAuto))
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            int index = GetCanUseHpDrugIndex(isAuto);
            if (index >= 0 && index < mComDrug.Length)
            {
                if (!_hasLeftCountByItemId(mComDrug[index].mItemId))
                {
                    return;
                }
                if (mComDrug[index].IsCD()) return;
                var data = TableManager.instance.GetTableItem<ItemTable>(mComDrug[index].mItemId);
                if (data != null)
                {
                    if (data.SubType == ItemTable.eSubType.AttributeDrug)
                        return;
                }
                UseDrugsByIdx(index);
            }
        }
    }

    private int GetCanUseHpDrugIndex(bool isAuto)
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        float playerHp = player.playerActor.GetEntityData().GetHPRate().single;
        float mainSlotHp = PlayerBaseData.GetInstance().GetPotionPercent(PlayerBaseData.PotionSlotType.SlotMain)/100.0f;
        float slotExtend1Hp = PlayerBaseData.GetInstance().GetPotionPercent(PlayerBaseData.PotionSlotType.SlotExtend1)/100.0f;
        bool hasMainDrug = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotMain)!=0;
        bool hasSlotExtendDrug = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend1) != 0;
        bool mainSwitchFlag = PlayerBaseData.GetInstance().IsPotionSlotMainSwitchOn();
        bool slot1SwitchFlag = PlayerBaseData.GetInstance().IsPotionSlotMainSwitchOn(PlayerBaseData.potionSlot1SwitchKeyName);

        //if (!isAuto)
        //{
        //    if (slotExtend1Hp >= playerHp && hasSlotExtendDrug)
        //    {
        //        int id = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend1);
        //        if (id > 0)
        //            return (int)PlayerBaseData.PotionSlotType.SlotExtend1;
        //    }
        //}
        //else
        //{
        if (mainSwitchFlag && mainSlotHp >= playerHp && hasMainDrug)
        {

            if (!mComDrug[(int)PlayerBaseData.PotionSlotType.SlotMain].IsCD())
            {
                int id = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotMain);
                if (id > 0)
                    return (int)PlayerBaseData.PotionSlotType.SlotMain;
            }
        }
        if (slot1SwitchFlag && slotExtend1Hp >= playerHp && hasSlotExtendDrug)
        {
            if (!mComDrug[(int)PlayerBaseData.PotionSlotType.SlotExtend1].IsCD())
            {
                int id = PlayerBaseData.GetInstance().GetPotionID(PlayerBaseData.PotionSlotType.SlotExtend1);
                if (id > 0)
                    return (int)PlayerBaseData.PotionSlotType.SlotExtend1;
            }
        }
        //}
        return -1;
    }

    public void UseMPDrug(bool isAuto)
    {
        if (_isLocalPlayerCanAutoUseMpDrugs())
        {
            int index = PlayerBaseData.GetInstance().GetMPPotionIndex();
            if (index >= 0 && index < mComDrug.Length)
            {
                if (!_hasLeftCountByItemId(mComDrug[index].mItemId))
                {
                    return;
                }
                if (mComDrug[index].IsCD()) return;

                var data = TableManager.instance.GetTableItem<ItemTable>(mComDrug[index].mItemId);
                if (data != null)
                {
                    if (data.SubType == ItemTable.eSubType.AttributeDrug)
                        return;
                }
                UseDrugsByIdx(index);
            }
        }
    }

    private int GetLifeHpItem()
    {
        for (int j = 0; j < mComDrug.Length; ++j)
        {
            if (mComDrug[j].mItemId == 200001001 && mComDrug[j].mLeftCount > 0)
            {
                return j;
            }
        }
        return -1;
    }

    //public bool CanUseDefaultDrug()
    //{
    //    return _getDefaultDrugIndex() >= 0;
    //}

    //private int _getDefaultDrugIndex()
    //{
    //    bool canUseHp = _isLocalPlayerCanAutoUseHpDrugs();
    //    bool canUseMp = _isLocalPlayerCanAutoUseMpDrugs();

    //    int idx = -1;
    //    if (-1 == idx && canUseHp)
    //    {
    //        Logger.LogProcessFormat("[PotionSet] 血量到达自动使用hp药");
    //        idx = _getFirstAutoUseDrugIdxByType(ProtoTable.ItemTable.eSubType.Hp);
    //    }

    //    if (-1 == idx && canUseMp)
    //    {
    //        Logger.LogProcessFormat("[PotionSet] 血量到达自动使用mp药");
    //        idx = _getFirstAutoUseDrugIdxByType(ProtoTable.ItemTable.eSubType.Mp);
    //    }

    //    if (-1 == idx && (canUseHp || canUseMp))
    //    {
    //        Logger.LogProcessFormat("[PotionSet] 血量到达自动使用hpmp药");
    //        idx = _getFirstAutoUseDrugIdxByType(ProtoTable.ItemTable.eSubType.HpMp);
    //    }

    //    return idx;
    //}

    private int _getFirstAutoUseDrugIdxByType(ProtoTable.ItemTable.eSubType subType)
    {
        for (int i = 0; i < mCacheDrugs.Count; ++i)
        {
            if (null == mCacheDrugs[i])
            {
                Logger.LogProcessFormat("[PotionSet] 血量到达自动使用药 nil");
                continue;
            }

            if (mCacheDrugs[i].itemSubType != subType)
            {
                Logger.LogProcessFormat("[PotionSet] 血量到达自动使用药 非法类型");
                continue;
            }

            if (mCacheDrugs[i].IsCD())
            {
                Logger.LogProcessFormat("[PotionSet] 血量到达自动使用药 CD");
                continue;
            }

            if (!_hasLeftCountByItemId(mCacheDrugs[i].mItemId))
            {
                Logger.LogProcessFormat("[PotionSet] 血量到达自动使用药 没有剩余");
                continue;
            }


            for (int j = 0; j < mComDrug.Length; ++j)
            {
                if (mComDrug[j] == mCacheDrugs[i])
                {
                    Logger.LogProcessFormat("[PotionSet] 血量到达自动使用药 满足条件");
                    return j;
                }
            }

            break;
        }

        Logger.LogProcessFormat("[PotionSet] 血量到达自动使用药 不满足条件");

        return -1;
    }


    //private void _startCD(float cd)
    //{
    //    // 全部进入CD状态之后进入展开状态会有问题
    //    for (int i = 0; i < mComDrug.Length; ++i)
    //    {
    //        if (_hasLeftCount(i))
    //        {
    //            mComDrug[i].StartCD(cd);
    //        }
    //    }
    //}

    private bool _hasLeftCount(int idx)
    {
        if (idx >= 0 && idx < mComDrug.Length)
        {
            return _hasLeftCountByItemId(mComDrug[idx].mItemId);
        }

        return false;
    }

    private bool _hasLeftCountByItemId(int itemId)
    {
        var localPlayer = BattleMain.instance.GetLocalPlayer();
        return localPlayer.CanUseItemById(itemId, 1);
    }

    private bool _isAllDrugsEmpty()
    {
        for (int i = 0; i < mComDrug.Length; ++i)
        {
            if (_hasLeftCount(i))
            {
                return false;
            }
        }

        return true;
    }

    public void SwitchNext()
    {
    }

    public void OnExpandClick()
    {
    }

    private void _setItemMode(bool isExpand)
    {
        for (int i = 0; i < mComDrug.Length; ++i)
        {
            mComDrug[i].SetMode(isExpand ? ComDrug.eMode.Drag : ComDrug.eMode.Click);
        }
    }


    public void UseDrugsByIdx(int idx)
    {
        if (_isPlayerDead())
        {
            Logger.LogProcessFormat("[PotionSet] 玩家死亡，无法使用药品");
            return;
        }
        if (!FrameSync.instance.isFire)
        {
            Logger.LogProcessFormat("[PotionSet] 无法Fire帧命令");
            return;
        }
        if (_isAllDrugsEmpty())
        {
            Logger.LogProcessFormat("[PotionSet] 进入快速购买逻辑");
            _quickBuyPotionProcess();
        }
        else if (_checkConditionByIndexWithTips(idx))
        {
            Logger.LogProcessFormat("[PotionSet] 进入使用药品逻辑");

            if (idx < 0 || idx >= mComDrug.Length)
            {
                return;
            }
            if (null == mComDrug[idx])
            {
                return;
            }

            if (mComDrug[idx].locked)
            {
                Logger.LogProcessFormat("[PotionSet] UseDrug locked !!!");
                return;
            }

            InvokeMethod.RmoveInvokeIntervalCall(mComDrug[idx]);
            mComDrug[idx].locked = true;
            InvokeMethod.InvokeInterval(mComDrug[idx], 0.250f, 0.0f, 0.0f,
                null,
                null,
                () =>
                {
                    mComDrug[idx].locked = false;
                });

            _useItemIter(idx);
        }
        SetDrugColumnStat();
    }

    private void _quickBuyPotionProcess()
    {
        if (_canQuickBuy(quickBuyPotionID, quickBuyPotionCount))
        {
            Logger.LogProcessFormat("[PotionSet] 开始快速购买逻辑");
            GameFrameWork.instance.StartCoroutine(ChapterBattlePotionSetUtiilty.QuickBuyPostion(quickBuyPotionID, quickBuyPotionCount));
        }
        else
        {
            Logger.LogProcessFormat("[PotionSet] 无法快速购买逻辑");
            SystemNotifyManager.SystemNotify(8513);
        }
    }

    private bool _canQuickBuy(int id, int count)
    {
        QuickBuyTable quicktable = TableManager.instance.GetTableItem<QuickBuyTable>(id);

        if (null == quicktable)
        {
            Logger.LogProcessFormat("[PotionSet] 无法快速购买逻辑");
            return false;
        }

        int cost = quicktable.CostNum * count;
        int costItemID = quicktable.CostItemID;

        ItemTable itemtable = TableManager.instance.GetTableItem<ItemTable>(costItemID);
        if (null == itemtable)
        {
            return false;
        }

        switch (itemtable.SubType)
        {
            case ItemTable.eSubType.POINT:
            case ItemTable.eSubType.BindPOINT:
                {
                    return PlayerBaseData.GetInstance().CanUseTicket((ulong)cost);
                }
                break;
            case ItemTable.eSubType.GOLD:
            case ItemTable.eSubType.BindGOLD:
                {
                    return PlayerBaseData.GetInstance().CanUseGold((ulong)cost);
                }
                break;
        }
        return false;
    }

    private bool _isLocalPlayerCanAutoUseHpDrugs(bool isAuto)
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        if (null == player)
        {
            return false;
        }

        if (_isBattleFinish())
        {
            return false;
        }

        if (!FrameSync.instance.isFire)
        {
            return false;
        }

        if (_isPlayerDead())
            return false;
        float playerRate = player.playerActor.GetEntityData().GetHPRate().single;

        return PlayerBaseData.GetInstance().GetHPPotionPercentMax() / 100.0f > playerRate;
        //if (!isAuto)
        //{
        //    float rate = PlayerBaseData.GetInstance().GetPotionPercent(PlayerBaseData.PotionSlotType.SlotExtend1) / 100.0f;
        //    return rate > playerRate;
        //}
        //else
        //{
        //    float rate = PlayerBaseData.GetInstance().GetHPPotionPercentMax() / 100.0f;
        //    return rate > playerRate;
        //}
    }

    private bool _isLocalPlayerCanAutoUseMpDrugs()
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();

        if (null == player)
        {
            return false;
        }

        if (_isBattleFinish())
        {
            return false;
        }

        if (!FrameSync.instance.isFire)
        {
            return false;
        }

        if (PlayerBaseData.GetInstance().VipLevel <= 0)
        {
            return false;
        }
        autoUseMpRate = PlayerBaseData.GetInstance().GetMPPotionPercentMax() / 100.0f;
        return !_isPlayerDead() && player.playerActor.GetEntityData().GetMPRate().single < autoUseMpRate;
    }

    private bool _isPlayerDead()
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();

        if (null == player || null == player.playerActor)
        {
            return false;
        }

        var data = player.playerActor.GetEntityData();
        if (null == data)
        {
            return false;
        }

        return data.GetHP() <= 0;
    }


    private bool _isBattleFinish()
    {
        if (null == BattleMain.instance)
        {
            return true;
        }

        BeScene scene = BattleMain.instance.Main;

        if (null == scene)
        {
            return true;
        }

        return BeSceneState.onFinish == scene.state;
    }

    private bool _checkConditionByIndexWithTips(int index)
    {
        if (null == BattleMain.instance)
        {
            return false;
        }

        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();

        if (null == player)
        {
            return false;
        }

        var data = player.playerActor.GetEntityData();
        int hp = data.GetHP();
        int maxHP = data.GetMaxHP();
        int mp = data.GetMP();
        int maxMP = data.GetMaxMP();

        int itemId = mComDrug[index].mItemId;
        ProtoTable.ItemTable.eSubType subType = mComDrug[index].itemSubType;

        bool isValid = false;

        switch (subType)
        {
            case ProtoTable.ItemTable.eSubType.Hp:
                isValid = hp < maxHP;
                break;
            case ProtoTable.ItemTable.eSubType.Mp:
                isValid = mp < maxMP;
                break;
            case ProtoTable.ItemTable.eSubType.HpMp:
            case ProtoTable.ItemTable.eSubType.ST_NONE:
                isValid = mp < maxMP || hp < maxHP;
                break;
            case ProtoTable.ItemTable.eSubType.AttributeDrug:
                return true;
        }


        if (!isValid)
        {
            Logger.LogProcessFormat("[PotionSet] 无法使用药品 {0}, {1}", itemId, subType);

            ItemConfigTable table = ChapterBattlePotionSetUtiilty.GetItemConfigTalbeByID(itemId);
            if (null != table)
            {
                SystemNotifyManager.SystemNotify(table.InvalidTipsID);
            }
        }


        return isValid;
    }

    private void _useItemIter(int index)
    {
        ComDrug curItem = mComDrug[index];

        if (null == curItem)
        {
            return;
        }

        Logger.LogProcessFormat("[PotionSet] 获得药品 {0}, {1}, status:{2}", index, curItem.mItemId, useStat);

        if (curItem.IsCD())
        {
            return;
        }

        BattlePlayer localPlayer = BattleMain.instance.GetLocalPlayer();
        if (localPlayer.CanUseItemById(curItem.mItemId, 1))
        {
            if (useStat == UseState.idle)
            {
                _realUseDrug(index);
            }
        }
        else
        {
            Logger.LogProcessFormat("[PotionSet] 无法使用 {0}", curItem.mItemId);

            if (_canQuickBuyByType(curItem.itemSubType))
            {
                Logger.LogProcessFormat("[PotionSet] 无法使用 {0}", curItem.mItemId);

                _quickBuyPotionProcess();
            }
        }
    }

    private bool _canQuickBuyByType(ProtoTable.ItemTable.eSubType subType)
    {
        switch (subType)
        {
            case ProtoTable.ItemTable.eSubType.Hp:
            case ProtoTable.ItemTable.eSubType.Mp:
            case ProtoTable.ItemTable.eSubType.ST_NONE:
                if (_isJustTakePureHpOrPureMpOrEmpty() &&
                    (_isDrugsEmptyByType(ProtoTable.ItemTable.eSubType.Hp) || _isDrugsEmptyByType(ProtoTable.ItemTable.eSubType.Mp))
                   )
                {
                    return true;
                }
                break;
            case ProtoTable.ItemTable.eSubType.HpMp:
                if (_isDrugsEmptyByType(subType))
                {
                    return true;
                }
                break;
        }

        return false;
    }

    private bool _isJustTakePureHpOrPureMpOrEmpty()
    {
        for (int i = 0; i < mComDrug.Length; ++i)
        {
            if (mComDrug[i].itemSubType == ProtoTable.ItemTable.eSubType.HpMp)
            {
                return false;
            }
        }

        return true;
    }

    private bool _isDrugsEmptyByType(ProtoTable.ItemTable.eSubType subType)
    {
        for (int i = 0; i < mComDrug.Length; ++i)
        {
            if (mComDrug[i].itemSubType == subType)
            {
                if (_hasLeftCountByItemId(mComDrug[i].mItemId))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void _onQuickBuyPostionFail(UIEvent ui)
    {
        Logger.LogProcessFormat("[PotionSet] 快速购买失败");
    }

    private void _onQuickBuyPostionSuccess(UIEvent ui)
    {
        Logger.LogProcessFormat("[PotionSet] 快速购买成功");

        int leftCount = BattleMain.instance.GetLocalPlayer().AddItemById(quickBuyPotionID, quickBuyPotionCount);

        int idx = _getQuickBuyPotionIDIndex();

        mComDrug[idx].SetItemID(quickBuyPotionID);
        mComDrug[idx].SetCount(leftCount);

        mCacheDrugs.Sort();

        if (_isPlayerNeedUseDrugAfterQuickBuy())
        {
            UseDrugsByIdx(idx);
        }
    }

    private int _getQuickBuyPotionIDIndex()
    {
        for (int i = 0; i < mComDrug.Length; ++i)
        {
            if (quickBuyPotionID == mComDrug[i].mItemId)
            {
                return i;
            }
        }

        for (int i = 0; i < mComDrug.Length; ++i)
        {
            if (!_hasLeftCountByItemId(mComDrug[i].mItemId))
            {
                return i;
            }
        }

        return 0;
    }

    private bool _isPlayerNeedUseDrugAfterQuickBuy()
    {
        BattlePlayer battlePlayer = BattleMain.instance.GetLocalPlayer();
        if (null == battlePlayer)
        {
            return false;
        }

        BeActor actor = battlePlayer.playerActor;
        if (null == actor)
        {
            return false;
        }

        var data = actor.GetEntityData();

        if (null == data)
        {
            return false;
        }

        return (data.GetHPRate().single < quickBuyPotionUseHPRate);
    }

    private void _realUseDrug(int index)
    {
        ComDrug curItem = mComDrug[index];

        if (null == curItem)
        {
            return;
        }

        Logger.LogProcessFormat("[PotionSet] 尝试使用药品 {0}", curItem.mItemId);

        ItemData itemData = ItemDataManager.GetInstance().GetItemByTableID(curItem.mItemId, false, false);

        if (null != itemData)
        {
            SceneUseItem msg = new SceneUseItem();
            msg.uid = itemData.GUID;

            SceneUseItemRet msgRet = new SceneUseItemRet();

            var battle = BattleMain.instance.GetBattle() as BaseBattle;
            if (battle != null)
            {
                battle.MessageSender.SendMessage(msg, msgRet, ServerType.GATE_SERVER, OnSceneUseItemReceive, new object[] { curItem, itemData }, true, 3500);
            }
        }
        else
        {
            Logger.LogError("[PotionSet] 数据为空");
        }
    }

    private void OnSceneUseItemReceive(MsgDATA msg,object _args)
    {
        SceneUseItemRet msgRet = new SceneUseItemRet();
        msgRet.decode(msg.bytes);

        if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
            return;

        if(_args != null)
        {
            object[] args = (object[])_args;
            if(args.Length > 1)
            {
                ComDrug curItem = args[0] as ComDrug;
                ItemData itemData = args[1] as ItemData;
                if(curItem == null || itemData == null)
                {
                    return;
                }
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                    Logger.LogProcessFormat("[PotionSet] 使用药品 {0} 失败", curItem.mItemId);
                }
                else
                {
                    BattlePlayer localPlayer = BattleMain.instance.GetLocalPlayer();
                    int left = localPlayer.UseItemById(curItem.mItemId, 1);

                    curItem.SetCount(left);
                    curItem.PlayEffect();

                    _startCDByType(curItem.itemSubType, itemData.CD);

                    Logger.LogProcessFormat("[PotionSet] 使用药品 {0}, {1} 成功", curItem.mItemId, left);
                }
                useStat = UseState.idle;
            }
        }
    }

    private void _startCDByType(ProtoTable.ItemTable.eSubType subType, float cdTime)
    {
        for (int i = 0; i < mComDrug.Length; ++i)
        {
            if (mComDrug[i].itemSubType == subType)
            {
                mComDrug[i].StartCD(cdTime);
            }
        }
    }


    public void Update()
    {
        for (int i = 0; i < mComDrug.Length; ++i)
        {
            float rate = mComDrug[i].mBar.fillAmount;
            //mImages[i].fillAmount = rate;
            //mImages[i].fillAmount = _getRate(rate);
            // mImages[i].transform.localRotation = _getRotate(i, rate);

            mComDrug[i].RealUpdate();
        }
    }

    private float _getRate(float rate)
    {
        return rate * fullRate;
    }

    private Quaternion _getRotate(int idx, float rate)
    {
        float angle = fullRate * 360.0f;
        float rotateZ = angle * idx + (isRoateWithMiddle ? Mathf.Clamp01(rate) * angle * 0.5f : -angle * 0.5f);

        return Quaternion.Euler(0, 0, rotateZ);
    }
}
