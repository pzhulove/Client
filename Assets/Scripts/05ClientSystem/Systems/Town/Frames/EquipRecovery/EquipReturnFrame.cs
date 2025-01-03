using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;

namespace GameClient
{
    class EquipReturnFrame : ClientFrame
    {
        float lastTime = 0;//用于刷新时间
        float curTime = 1;
        ulong updateTime = 0;
        bool haveGetTime = false;
        bool canUseReturnBtn = true;

        int nowScore = 0;
        int canReturnScore = 0;
        List<ulong> returnEquipList = new List<ulong>(); 
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipRecovery/EquipReturnFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _InitData();
            _RegisterUIEvent();
        }
        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipRedeemSuccess, _OnEquipRedeemSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipRecoveryUpdateTime, _OnEquipRecoveryUpdateTime);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipRecivertDeleteItem, _OnUpdateDisplayList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipReturnFail, _OnEquipReturnFail);
            
        }
        
        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipRedeemSuccess, _OnEquipRedeemSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipRecoveryUpdateTime, _OnEquipRecoveryUpdateTime);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipRecivertDeleteItem, _OnUpdateDisplayList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipReturnFail, _OnEquipReturnFail);
        }

        void _InitData()
        {
            returnEquipList.Clear();
            _SendTimeReq();
            _InitReturnItemScrollListBind();
            _UpdateScore();
            
            _UpdateEquipList();
            canUseReturnBtn = true;
        }

        void _ClearData()
        {
            lastTime = 0;
            curTime = 1;
            updateTime = 0;
            haveGetTime = false;
            canUseReturnBtn = true;
            nowScore = 0;
            canReturnScore = 0;
            returnEquipList.Clear();
        }

        /// <summary>
        /// 刷新现在拥有的分数和还可赎回使用的分数
        /// </summary>
        void _UpdateScore()
        {
            nowScore = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_REWARD_SCORE);
            mHaveCount.text = nowScore.ToString();
            canReturnScore = _GetReturnScore();
            mCanRedeemCount.text = canReturnScore.ToString();
            _UpdateEquipList();
        }

        /// <summary>
        /// 刷新显示的剩余时间
        /// </summary>
        void _UpdateTime(ulong time)
        {
            haveGetTime = true;
            updateTime = time;
        }

        int _GetReturnScore()
        {
            var jarScoreList = EquipRecoveryDataManager.GetInstance().jarKeyList;
            int previousScore = 0;
            for (int i=0;i<jarScoreList.Count;i++)
            {
                if(EquipRecoveryDataManager.GetInstance()._GetJarState(i+1) == RewardJarStatic.HaveOpen)
                {
                    previousScore = jarScoreList[i];
                }
            }
            return nowScore - previousScore;
        }

        void _UpdateEquipList()
        {
            _RefreshItemListCount();
        }
        void _RefreshItemListCount()
        {
            returnEquipList.Clear();
            List<ulong> tempdisplayList = new List<ulong>();
            tempdisplayList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.EquipRecovery);
            for (int i = 0; i < tempdisplayList.Count; i++)
            {
                returnEquipList.Add(tempdisplayList[i]);
            }
            mTips.CustomActive(returnEquipList.Count == 0);
            mEquipUIList.SetElementAmount(returnEquipList.Count);
        }

        void _SendTimeReq()
        {
            EquipRecoveryDataManager.GetInstance()._SendReturnTimeReq();
        }

        void _InitReturnItemScrollListBind()
        {
            mEquipUIList.Initialize();

            mEquipUIList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateItemScrollListBind(item);
                }
            };

            mEquipUIList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }
                var mRedeem = combind.GetCom<Button>("redeem");
                if (mRedeem != null)
                {
                    mRedeem.onClick.RemoveAllListeners();
                }
            };
        }

        void _UpdateItemScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= returnEquipList.Count)
            {
                return;
            }
            ulong guid = returnEquipList[item.m_index];
            ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData == null)
            {
                return;
            }


            var mItemRoot = mBind.GetGameObject("itemRoot");
            var mItemName = mBind.GetCom<Text>("itemName");
            var mCount = mBind.GetCom<Text>("count");
            var mRedeem = mBind.GetCom<Button>("redeem");
            var mCannotRedeem = mBind.GetGameObject("cannotRedeem");

            float goldMoney = 0;
            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemData.TableID);
            SystemValueTable goldCountParameter = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_SHUHUI_PRICERATION);
            if (itemTableData != null && goldCountParameter != null)
            {
                goldMoney = itemTableData.RecommendPrice * goldCountParameter.Value * 0.01f;
            }
            mRedeem.onClick.RemoveAllListeners();
            mRedeem.onClick.AddListener(() =>
            {
                //mRedeem.CustomActive(false);
                //mCannotRedeem.CustomActive(true);
                if(canUseReturnBtn)
                {
                    
                    float goldCount = 0;
                    int goldID = 0;
                    SystemValueTable goldCountData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_SHUHUI_HBNUM);
                    if (goldCountData != null)
                    {
                        goldCount = goldCountData.Value;
                        if (goldCount < goldMoney)
                        {
                            goldCount = goldMoney;
                        }
                    }
                    SystemValueTable goldIDData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_SHUHUI_HBTYID);
                    if (goldIDData != null)
                    {
                        goldID = goldIDData.Value;
                    }
                    string notifyCont = string.Format(TR.Value("equip_return_tip"), goldCount);
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
                    {
                        CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

                        costInfo.nMoneyID = goldID;

                        ItemTipManager.GetInstance().CloseAll();

                        costInfo.nCount = (int)goldCount;

                        CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                        {
                            canUseReturnBtn = false;
                            EquipRecoveryDataManager.GetInstance()._RedeemEquip(guid);
                        });
                    });
                }
                
            });
            


            //图标
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            if (null == ItemDetailData)
            {

            }
            else
            {
                ComItem comitem = mItemRoot.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    comitem = CreateComItem(mItemRoot);
                }
                int result = itemData.TableID;
                comitem.Setup(itemData, (GameObject Obj, ItemData sitem) => { _OnShowTips(itemData); });
            }
            //名字
            mItemName.text = itemData.Name;

            //价格
            mCount.text = itemData.RecoScore.ToString();

            bool canReturn = (itemData.RecoScore <= canReturnScore);

            mRedeem.CustomActive(canReturn);
            mCannotRedeem.CustomActive(!canReturn);

        }
        void _OnShowTips(ItemData result)
        {
            if(result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        void _OnEquipRedeemSuccess(UIEvent eventID)
        {
            _UpdateEquipList();
            canUseReturnBtn = true;
        }

        void _OnCountValueChange(UIEvent eventID)
        {
            _UpdateScore();
        }

        void _OnEquipRecoveryUpdateTime(UIEvent eventID)
        {
            _UpdateTime((ulong)eventID.Param1);
        }

        void _OnUpdateDisplayList(UIEvent eventID)
        {
            _RefreshItemListCount();
        }

        void _OnEquipReturnFail(UIEvent eventID)
        {
            canUseReturnBtn = true;
        }
        public override bool IsNeedUpdate()
        {
            return true;
        }
        protected override void _OnUpdate(float _LastTime)
        {
            if(haveGetTime)
            {
                mTime.CustomActive(true);
                curTime = Time.time;
                if (curTime - lastTime >= 1)
                {
                    int serverTime = (int)TimeManager.GetInstance().GetServerTime();
                    int actLastTime = (int)(updateTime/1000);
                    if (actLastTime - serverTime > 0)
                    {
                        mTime.text = Function.SetShowTimeHour((int)(updateTime/1000));
                        lastTime = curTime;
                    }
                    else
                    {
                        _SendTimeReq();
                    }
                }
            }
            else
            {
                mTime.CustomActive(false);
            }
        }

        #region ExtraUIBind
        private ComUIListElementScript mEquipItem = null;
        private Text mCanRedeemCount = null;
        private Text mHaveCount = null;
        private ComUIListScript mEquipUIList = null;
        private Button mClose = null;
        private Text mTime = null;
        private GameObject mTips = null;
        protected override void _bindExUI()
        {
            mEquipItem = mBind.GetCom<ComUIListElementScript>("equipItem");
            mCanRedeemCount = mBind.GetCom<Text>("canRedeemCount");
            mHaveCount = mBind.GetCom<Text>("haveCount");
            mEquipUIList = mBind.GetCom<ComUIListScript>("equipUIList");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mTime = mBind.GetCom<Text>("time");
            mTips = mBind.GetGameObject("tips");
        }

        protected override void _unbindExUI()
        {
            mEquipItem = null;
            mCanRedeemCount = null;
            mHaveCount = null;
            mEquipUIList = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mTime = null;
            mTips = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}