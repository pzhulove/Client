using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    public class ComChapterInfoDrugUnit : ComBaseComponet
    {
#region override
        protected override void Init()
        {
            _updateState(mID);
        }

        protected override void UnInit()
        {

        }
#endregion
        public enum eMarkState
        {
            Marked,
            UnMarked,
        }

        private eMarkState mMarkState = eMarkState.UnMarked;

        public eMarkState markState
        {
            get
            {
                return mMarkState;
            }

            set 
            {
                mMarkState = value;

                Logger.LogProcessFormat("[buffdrug] mark状态 {0}", mMarkState);
                mChecktoggle.isOn = mMarkState == eMarkState.Marked;
            }
        }


        private enum eState
        {
            None,
            /// <summary>
            /// 等待使用
            /// </summary>
            Ready,
            /// <summary>
            /// 等待结果
            /// </summary>
            Wait,
            /// <summary>
            /// 已经使用
            /// </summary>
            Used,
            /// <summary>
            /// 正在CD
            /// </summary>
            CoolDown,
        }

        private int          mID    = 0;
        private eState       mState = eState.None;

        private eState       state 
        {
            get 
            {
                return mState;
            }

            set 
            {
                if (mState != value)
                {
                    Logger.LogProcessFormat("[buffdrug] 状态改变 {0} -> {1}", mState, value);
                    mState = value;
                    _updateBg(value);
                    //_updateCostEx();
                }
            }
        }

        private void _updateBg(eState state)
        {
            if (null != mBind)
            {
                GameObject mask = mBind.GetGameObject("checkmask");

                switch (state)
                {
                    case eState.Ready:
                        mask.CustomActive(false);
                        break;
                    case eState.Used:
                        mask.CustomActive(true);
                        break;
                }
            }
        }

        public void LoadUnit(int id)
        {
            ItemTable item = TableManager.instance.GetTableItem<ItemTable>(id);
            if (null != item)
            {
                mID = id;

                _init(mBind, id, item.Icon);
                _updateState(id);

                _updateCost(id);
            }

        }

        public void UnloadUnit()
        {
            _uninit(mBind);
        }

        public void UpdateCount()
        {
            _updateState(mID);
        }

        public void UpdateCost()
        {
            _updateCost(mID);
            _updateState(mID);
        }

        private string _getName(int id)
        {
            BuffDrugConfigTable table = TableManager.instance.GetTableItem<BuffDrugConfigTable>(id);

            if (null != table)
            {
                return table.Name;
            }

            return "";
        }

        private string _getDescription(int id)
        {
            BuffDrugConfigTable table = TableManager.instance.GetTableItem<BuffDrugConfigTable>(id);

            if (null != table)
            {
                return table.Description;
            }

            return "";
        }

        private bool _isFree2Use(int id)
        {
            BuffDrugConfigTable table = TableManager.instance.GetTableItem<BuffDrugConfigTable>(id);

            if (null != table)
            {
                return table.FreeUseLevel >= PlayerBaseData.GetInstance().Level;
            }

            return false;
        }

        enum eCostType
        {
            Gold,
            Point,
        }

        // TODO 改成统一接口来获取消耗类型
        private eCostType _getCostType(int id)
        {
            QuickBuyTable quicktable = TableManager.instance.GetTableItem<QuickBuyTable>(id);

            if (null == quicktable)
            {
                return eCostType.Gold;
            }

            ItemTable table = TableManager.instance.GetTableItem<ItemTable>(quicktable.CostItemID);

            if (null != table)
            {
                switch (table.SubType)
                {
                    case ItemTable.eSubType.BindGOLD:
                    case ItemTable.eSubType.GOLD:
                        return eCostType.Gold;
                    case ItemTable.eSubType.BindPOINT:
                    case ItemTable.eSubType.POINT:
                        return eCostType.Point;
                }
            }

            return eCostType.Gold;
        }

        private int _getCost(int id)
        {
            QuickBuyTable table = TableManager.instance.GetTableItem<QuickBuyTable>(id);

            if (null != table)
            {
                return table.CostNum;
            }

            return 0;

            //eCostType type = _getCostType(id);

            //switch (type)
            //{
            //    case eCostType.Gold:
            //        return Utility.GetQuickBuyCostCount(ItemTable.eSubType.GOLD);
            //    case eCostType.Point:
            //        return Utility.GetQuickBuyCostCount(ItemTable.eSubType.POINT);
            //}

            //return 0;
        }

        private int _getCnt(int id)
        {
            return ItemDataManager.GetInstance().GetOwnedItemCount(id);
        }

        private void _init(ComCommonBind bind, int id, string iconpath)
        {
            if (null != bind)
            {
                Image icon  = bind.GetCom<Image>("icon");
                Text name   = bind.GetCom<Text>("name");
                Text desc   = bind.GetCom<Text>("desc");
                Button use  = bind.GetCom<Button>("usebutton");

                // icon.sprite = AssetLoader.instance.LoadRes(iconpath, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref icon, iconpath);
                name.text   = _getName(id);
                desc.text   = _getDescription(id);

                //int idx = id;
                //use.onClick.AddListener(()=>
                //{
                //        _onUseDrug(idx);
                //});
            }
        }

        private void _uninit(ComCommonBind bind)
        {
            if (null != bind)
            {
                Button use = bind.GetCom<Button>("usebutton");
                if (null != use)
                {
                    use.onClick.RemoveAllListeners();
                }
            }
        }

        private void _updateState(int id)
        {
            state = _isUsed(id) ? eState.Used : eState.Ready;
            markState = ChapterBuffDrugManager.GetInstance().IsBuffDrugMarked(mID) ? eMarkState.Marked : eMarkState.UnMarked;
        }

        private void _updateCostEx()
        {
            if (null != mBind)
            {
                Text cost = mBind.GetCom<Text>("cost");
                Image costicon = mBind.GetCom<Image>("costicon");
                if (state == eState.Used)
                {
                    if (null != costicon)
                    {
                        costicon.enabled = false;
                    }

                    cost.text = "<color=#418fc5>已使用</color>";
                }
            }
        }

        private void _updateCost(int id)
        {
            ChapterBuffDrugManager.eBuffDrugUseType costType = ChapterBuffDrugManager.GetBuffDrugType(mID);

            mCount.text = ChapterBuffDrugManager.GetBuffDrugCount(mID).ToString();

            switch (costType)
            {
                case ChapterBuffDrugManager.eBuffDrugUseType.FreeUse:
                    mCostRoot.SetActive(true);
                    mCountRoot.CustomActive(false);
                    mCosticon.enabled = false;
                    mCost.text = "<color=#00ff78>免费</color>";
                    break;
                case ChapterBuffDrugManager.eBuffDrugUseType.PackageUse:
                    mCostRoot.SetActive(false);
                    mCountRoot.CustomActive(true);
                    mCosticon.enabled = false;
                    mCost.text = "";
                    break;
                case ChapterBuffDrugManager.eBuffDrugUseType.PayUse:
                case ChapterBuffDrugManager.eBuffDrugUseType.NotEnoughPay2Use:
                    CostItemManager.CostInfo info = ChapterBuffDrugManager.GetBuffDrugCost(mID);
                    mCost.text = string.Format("{0}", info.nCount);
                    mCostRoot.SetActive(true);
                    mCountRoot.CustomActive(false);
                    mCosticon.enabled = true;

                    ItemTable itemData = TableManager.instance.GetTableItem<ItemTable>(info.nMoneyID);
                    if (null != itemData)
                    {
                        switch (itemData.SubType)
                        {
                            case ItemTable.eSubType.BindGOLD:
                            case ItemTable.eSubType.GOLD:
                                // mCosticon.sprite = mBind.GetSprite("gold");
                                mBind.GetSprite("gold", ref mCosticon);
                                break;
                            case ItemTable.eSubType.BindPOINT:
                            case ItemTable.eSubType.POINT:
                                // mCosticon.sprite = mBind.GetSprite("point");
                                mBind.GetSprite("point", ref mCosticon);
                                break;
                        }
                    }
                    break;
                case ChapterBuffDrugManager.eBuffDrugUseType.None:
                    mCostRoot.SetActive(false);
                    mCountRoot.CustomActive(true);
                    mCosticon.enabled = false;
                    mCost.text = "";
                    break;
            }

            //_updateCostEx();
        }

        private bool _canUseItem(int id)
        {
            CostItemManager.CostInfo costInfo = ChapterBuffDrugManager.GetBuffDrugCost(id);
            return CostItemManager.GetInstance().IsEnough2Cost(costInfo);
        }

        private void _onUseDrug(int id)
        {
            if (state == eState.Ready)
            {
                state = eState.Wait;

                if (_isFree2Use(id))
                {
                    Logger.LogProcessFormat("[buffdrug] 免费使用 {0}", id);
                    GameFrameWork.instance.StartCoroutine(_useItem());
                    GameStatisticManager.instance.DoStatDrugUse(id, "free");
                }
                else 
                {
                    int cnt = _getCnt(id);
                    if (cnt > 0)
                    {
                        _usePackageItem(id);
                        GameStatisticManager.instance.DoStatDrugUse(id, "package");
                    }
                    else 
                    {
                        if (_canUseItem(id))
                        {
                            int cost = _getCost(id);
                            eCostType type = _getCostType(id);

                            Logger.LogProcessFormat("[buffdrug] 消耗{0} {1}", type, cost);

                            GameFrameWork.instance.StartCoroutine(_quickBuy(id));

                            GameStatisticManager.instance.DoStatDrugUse(id, "pay&use");
                        }
                        else 
                        {
                            state = eState.Ready;
                        }
                    }
                }
            }
            else
            {
                Logger.LogProcessFormat("[buffdrug] 错误状态 {0}", state);
            }
        }

        private byte _getBuffDrugIdx(int id, int itemid)
        {
            DungeonTable table = TableManager.instance.GetTableItem<DungeonTable>(id);
            if (null != table)
            {
                var drugs = table.BuffDrugConfig;
                for (int i = 0; i < drugs.Count; ++i)
                {
                    if (drugs[i] == itemid)
                    {
                        return (byte)i;
                    }
                }
            }

            return byte.MaxValue;
        }

        private void _usePackageItem(int id)
        {
            Logger.LogProcessFormat("[buffdrug] 消耗道具 {0} 剩余 {1}", id, _getCnt(id));
            ItemData data = ItemDataManager.GetInstance().GetItemByTableID(id);
            ItemDataManager.GetInstance().UseItem(data);
        }

        private IEnumerator _useItem()
        {
            MessageEvents        msg = new MessageEvents();
            SceneQuickUseItemRet res = new SceneQuickUseItemRet();
            SceneQuickUseItemReq req = new SceneQuickUseItemReq();

            int id                   = ChapterBaseFrame.sDungeonID;

            req.dungenid             = (uint)id;
            req.idx                  = _getBuffDrugIdx(id, mID);

            yield return MessageUtility.Wait<SceneQuickUseItemReq, SceneQuickUseItemRet>(ServerType.GATE_SERVER, msg, req, res);

            if (msg.IsAllMessageReceived())
            {
                if (res.code != 0)
                {
                    SystemNotifyManager.SystemNotify((int)res.code);
                }
                else 
                {
                    SystemNotifyManager.SystemNotify(5010);
                }
            }

            _updateState(mID);
        }

        private IEnumerator _quickBuy(int id)
        {
            yield return _useItem();
        }

        private bool _isUsed(int id)
        {
            ItemTable table = TableManager.instance.GetTableItem<ItemTable>(id);

            if (null != table)
            {
                int buffID = table.OnUseBuffId;
                IList<int> buffIDs = table.MutexBuff;

                Battle.DungeonBuff buff = PlayerBaseData.GetInstance().buffList.Find(x=>
                {
                    if (x.id == buffID)
                    {
                        return true;
                    }

                    for (int i = 0; i < buffIDs.Count; ++i)
                    {
                        if (x.id == buffIDs[i])
                        {
                            return true;
                        }
                    }

                    return false;
                });

                return buff != null;
            }

            return false;
        }

#region ExtraUIBind
        private Text mCount = null;
        private Text mName = null;
        private Image mIcon = null;
        private Text mCost = null;
        private Button mUsebutton = null;
        private Image mBg = null;
        private Image mCosticon = null;
        private Text mDesc = null;
        private GameObject mCheckmask = null;
        private Toggle mChecktoggle = null;
        private GameObject mCostRoot = null;
        private GameObject mCountRoot = null;

        protected override void _bindExUI()
        {
            mCount = mBind.GetCom<Text>("count");
            mName = mBind.GetCom<Text>("name");
            mIcon = mBind.GetCom<Image>("icon");
            mCost = mBind.GetCom<Text>("cost");
            mUsebutton = mBind.GetCom<Button>("usebutton");
            mUsebutton.onClick.AddListener(_onUsebuttonButtonClick);
            mBg = mBind.GetCom<Image>("bg");
            mCosticon = mBind.GetCom<Image>("costicon");
            mDesc = mBind.GetCom<Text>("desc");
            mCheckmask = mBind.GetGameObject("checkmask");
            mChecktoggle = mBind.GetCom<Toggle>("checktoggle");
            mChecktoggle.onValueChanged.AddListener(_onChecktoggleToggleValueChange);
            mCostRoot = mBind.GetGameObject("costRoot");
            mCountRoot = mBind.GetGameObject("countRoot");
        }

        protected override void _unbindExUI()
        {
            mCount = null;
            mName = null;
            mIcon = null;
            mCost = null;
            mUsebutton.onClick.RemoveListener(_onUsebuttonButtonClick);
            mUsebutton = null;
            mBg = null;
            mCosticon = null;
            mDesc = null;
            mCheckmask = null;
            mChecktoggle.onValueChanged.RemoveListener(_onChecktoggleToggleValueChange);
            mChecktoggle = null;
            mCostRoot = null;
            mCountRoot = null;
        }
#endregion   

#region Callback
        private void _onUsebuttonButtonClick()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1235);
                return ;
            }

            /* put your code in here */
            switch (markState)
            {
                case eMarkState.Marked:
                    ChapterBuffDrugManager.GetInstance().UnMarkBuffDrug2Use(mID);
                    break;
                case eMarkState.UnMarked:
                    ChapterBuffDrugManager.GetInstance().MarkBuffDrug2Use(mID);
                    break;
            }

            markState = ChapterBuffDrugManager.GetInstance().IsBuffDrugMarked(mID) ? eMarkState.Marked : eMarkState.UnMarked;
        }

        private void _onChecktoggleToggleValueChange(bool changed)
        {
            /* put your code in here */

        }
#endregion
    }
}
