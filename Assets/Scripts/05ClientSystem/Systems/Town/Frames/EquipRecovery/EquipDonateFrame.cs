using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using Protocol;
using EItemQuality = ProtoTable.ItemTable.eColor;

namespace GameClient
{
    class EquipDonateFrame : ClientFrame
    {
        int curSelSubType = 0;//当前选择的subType下标
        int curSelColor = 0;//当前选择的color下标
        int minPrice = 0;
        int maxPrice = 0;
        List<ulong> donateList = new List<ulong>();//已经选中的捐赠链表
        List<ulong> displayList = new List<ulong>();//经过筛选过后的需要展现的道具链表
        List<int> levelList = new List<int>();//装备等级的链表
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipRecovery/EquipDonateFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _InitData();
            _InitEquipItemScrollListBind();
            
            _InitUI();
            _RegisterUIEvent();
        }
        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _ClearUI();
            _UnRegisterUIEvent();
        }
        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipSubmitSuccess, _OnEquipSubmitSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewFrameClosed, _OnEquipDonatePackageUpdate);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipSubmitSuccess, _OnEquipSubmitSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewFrameClosed, _OnEquipDonatePackageUpdate);
        }
        void _InitEquipItemScrollListBind()
        {
            mContent.Initialize();

            mContent.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdatePackageItemScrollListBind(item);
                }
            };

            mContent.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }
                var mAdd = combind.GetCom<Button>("add");
                if(mAdd != null)
                {
                    mAdd.onClick.RemoveAllListeners();
                }
                var mHaveAdd = combind.GetCom<Button>("haveAdd");
                if (mHaveAdd != null)
                {
                    mHaveAdd.onClick.RemoveAllListeners();
                }
            };
        }
        void _UpdatePackageItemScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= displayList.Count)
            {
                return;
            }
            ulong guid = displayList[item.m_index];
            ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData == null)
            {
                return;
            }

            
            var mItemRoot = mBind.GetGameObject("itemRoot");
            var mItemName = mBind.GetCom<Text>("itemName");
            var mCount = mBind.GetCom<Text>("count");
            var mAdd = mBind.GetCom<Button>("add");
            var mHaveAdd = mBind.GetCom<Button>("haveAdd");

            bool haveElement = donateList.Contains(guid);
            mAdd.CustomActive(!haveElement);
            mHaveAdd.CustomActive(haveElement);

            
            int tempMinPrice = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData , true);
            int tempMaxPrice = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData , false);
            mAdd.onClick.AddListener(()=>
            {
                if(!_CannotSelect())
                {
                    mAdd.CustomActive(false);
                    mHaveAdd.CustomActive(true);
                    donateList.Add(guid);
                    maxPrice += tempMaxPrice;
                    minPrice += tempMinPrice;
                    _UpdateExpectPrice();
                    _UpdateWeekCount();
                }
                else
                {
                    SystemNotifyManager.SystemNotify(9075);
                }
            });
            
            mHaveAdd.onClick.AddListener(()=>
            {
                mAdd.CustomActive(true);
                mHaveAdd.CustomActive(false);
                donateList.Remove(guid);
                maxPrice -= tempMaxPrice;
                minPrice -= tempMinPrice;
                _UpdateExpectPrice();
                _UpdateWeekCount();
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
            mCount.text = EquipRecoveryDataManager.GetInstance()._GetEquipPrice(itemData);



        }
        void _OnShowTips(ItemData result)
        {
            if(result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        
        

        void _InitData()
        {
            donateList.Clear();
            displayList.Clear();
            levelList.Clear();
        }
        void _InitUI()
        {
            ItemDataManager.GetInstance().ArrangeItems(EPackageType.Equip);
            _InitLevelList();
            _InitPackage();
            _UpdateWeekCount();
            _UpdateNowScore();

        }

        void _InitLevelList()
        {
            levelList.Add(0);//传入0代表全部等级都ok
            var EquipRecoveryPriceTableData = TableManager.GetInstance().GetTable<EquipRecoveryPriceTable>();
            var enumerator = EquipRecoveryPriceTableData.GetEnumerator();
            while(enumerator.MoveNext())
            {
                levelList.Add(enumerator.Current.Key);
            }
        }
        void _InitPackage()
        {
            _UpdatePackage();
        }
        
        void _ClearData()
        {
            curSelSubType = 0;
            curSelColor = 0;
            minPrice = 0;
            maxPrice = 0;
            donateList.Clear();
            displayList.Clear();
        }
        void _ClearUI()
        {

        }

        void _PlayEffect()
        {

        }

        #region 事件
        void _OnEquipSubmitSuccess(UIEvent uiEvent)
        {
            
            _PlayEffect();
            minPrice = 0;
            maxPrice = 0;
            donateList.Clear();
            _UpdateExpectPrice();
            _UpdatePackage();
            
            _UpdateWeekCount();
            _UpdateNowScore();
            frameMgr.CloseFrame(this);
        }

        void _OnCountValueChange(UIEvent uievent)
        {
            _UpdateWeekCount();
            _UpdateNowScore();
        }

        void _OnEquipDonatePackageUpdate(UIEvent uiEvent)
        {
            _UpdatePackage();

            if (donateList == null || donateList.Count <= 0)
                return;

            //查找捐赠的物品是否还存在
            for (var i = donateList.Count - 1; i >= 0; i--)
            {
                var selectedId = donateList[i];
                bool isFind = false;
                for (var j = 0; j < displayList.Count; j++)
                {
                    if (selectedId == displayList[j])
                    {
                        isFind = true;
                        break;
                    }
                }

                if (isFind == false)
                    donateList.RemoveAt(i);
            }

        }
        #endregion

        #region 刷新函数
        void _RefreshItemListCount()
        {
            mContent.SetElementAmount(displayList.Count);
        }
        void _UpdatePackage()
        {
            displayList.Clear();
            List<ulong> tempdisplayList = new List<ulong>();
            tempdisplayList = EquipRecoveryDataManager.GetInstance().GetItemForType(levelList[curSelSubType], (ItemTable.eColor)curSelColor,levelList[1]);
            for(int i=0;i<tempdisplayList.Count;i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(tempdisplayList[i]);
                if(itemData == null)
                {
                    continue;
                }
                var itemTableItem = TableManager.GetInstance().GetTableItem<ItemTable>(itemData.TableID);
                if(itemTableItem == null)
                {
                    continue;
                }

                //在未启用的装备方案中
                if(itemData.IsItemInUnUsedEquipPlan == true)
                    continue;

                if(itemTableItem.Color2 != 3)
                {
                    //3是玫红色，不参与装备回收
                    if(!itemData.bLocked)
                    {
                        displayList.Add(tempdisplayList[i]);
                    }                    
                }
                
            }
            _RefreshItemListCount();
            mTips.CustomActive(displayList.Count == 0);
        }

        void _UpdateExpectPrice()
        {
            mExpectScore.text = string.Format("{0}-{1}", minPrice, maxPrice);
        }

        void _UpdateWeekCount()
        {
            SystemValueTable jarWeekNumData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_TIMES);
            if(jarWeekNumData != null)
            {
                int maxjarCount = jarWeekNumData.Value;
                mDonateCount.text = (CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_WEEK_COUNT) + donateList.Count).ToString() + "/" + maxjarCount + "次";
            }
            
        }

        bool _CannotSelect()
        {
            SystemValueTable jarWeekNumData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_RECO_TIMES);
            if(jarWeekNumData != null)
            {
                int maxjarCount = jarWeekNumData.Value;
                return CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_WEEK_COUNT) + donateList.Count == maxjarCount;
            }
            return false;
        }
        void _UpdateNowScore()
        {
            int nowScore = CountDataManager.GetInstance().GetCount(CounterKeys.EQUIP_RECOVERY_REWARD_SCORE);
            mHaveScore.text = nowScore.ToString();
        }


        #endregion

		#region ExtraUIBind
		private Dropdown mQualitySelect = null;
		private Dropdown mSubTypeSelect = null;
		private ComUIListScript mContent = null;
		private ComUIListElementScript mRecord = null;
		private Text mHaveScore = null;
		private Text mExpectScore = null;
		private Text mDonateCount = null;
		private Button mSubmit = null;
		private GameObject mContentRoot = null;
		private Button mOpenPriceTable = null;
		private Button mClose = null;
		private Text mTips = null;
		private Button mAuction = null;
		
		protected override void _bindExUI()
		{
			mQualitySelect = mBind.GetCom<Dropdown>("QualitySelect");
			if (null != mQualitySelect)
			{
				mQualitySelect.onValueChanged.AddListener(_onQualitySelectDropdownValueChange);
			}
			mSubTypeSelect = mBind.GetCom<Dropdown>("SubTypeSelect");
			if (null != mSubTypeSelect)
			{
				mSubTypeSelect.onValueChanged.AddListener(_onSubTypeSelectDropdownValueChange);
			}
			mContent = mBind.GetCom<ComUIListScript>("Content");
			mRecord = mBind.GetCom<ComUIListElementScript>("Record");
			mHaveScore = mBind.GetCom<Text>("HaveScore");
			mExpectScore = mBind.GetCom<Text>("ExpectScore");
			mDonateCount = mBind.GetCom<Text>("DonateCount");
			mSubmit = mBind.GetCom<Button>("Submit");
			if (null != mSubmit)
			{
				mSubmit.onClick.AddListener(_onSubmitButtonClick);
			}
			mContentRoot = mBind.GetGameObject("ContentRoot");
			mOpenPriceTable = mBind.GetCom<Button>("openPriceTable");
			if (null != mOpenPriceTable)
			{
				mOpenPriceTable.onClick.AddListener(_onOpenPriceTableButtonClick);
			}
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
			mTips = mBind.GetCom<Text>("tips");
			mAuction = mBind.GetCom<Button>("auction");
			if (null != mAuction)
			{
				mAuction.onClick.AddListener(_onAuctionButtonClick);
			}
		}
		
		protected override void _unbindExUI()
		{
			if (null != mQualitySelect)
			{
			mQualitySelect.onValueChanged.RemoveListener(_onQualitySelectDropdownValueChange);
			}
			mQualitySelect = null;
			if (null != mSubTypeSelect)
			{
			mSubTypeSelect.onValueChanged.RemoveListener(_onSubTypeSelectDropdownValueChange);
			}
			mSubTypeSelect = null;
			mContent = null;
			mRecord = null;
			mHaveScore = null;
			mExpectScore = null;
			mDonateCount = null;
			if (null != mSubmit)
			{
				mSubmit.onClick.RemoveListener(_onSubmitButtonClick);
			}
			mSubmit = null;
			mContentRoot = null;
			if (null != mOpenPriceTable)
			{
				mOpenPriceTable.onClick.RemoveListener(_onOpenPriceTableButtonClick);
			}
			mOpenPriceTable = null;
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
			mTips = null;
			if (null != mAuction)
			{
				mAuction.onClick.RemoveListener(_onAuctionButtonClick);
			}
			mAuction = null;
		}
		#endregion

        #region Callback
        private void _onQualitySelectDropdownValueChange(int index)
        {
            switch(index)
            {
                case (0):
                    curSelColor = 0;
                    break;
                case (1):
                    curSelColor = 2;
                    break;
                case (2):
                    curSelColor = 3;
                    break;
                case (3):
                    curSelColor = 5;
                    break;
            }
            minPrice = 0;
            maxPrice = 0;
            _UpdateExpectPrice();
            donateList.Clear();
            _UpdatePackage();
            _UpdateWeekCount();
        }
        private void _onSubTypeSelectDropdownValueChange(int index)
        {
            if(index >= levelList.Count)
            {
                return;
            }
            curSelSubType = index;
            minPrice = 0;
            maxPrice = 0;
            _UpdateExpectPrice();
            donateList.Clear();
            _UpdatePackage();
            _UpdateWeekCount();
        }
        private void _onSubmitButtonClick()
        {
            if(SecurityLockDataManager.GetInstance().CheckSecurityLock(() => 
            {
                for (int i = 0; i < donateList.Count; i++)
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(donateList[i]);
                    if (itemData != null && itemData.Quality >= EItemQuality.PURPLE)
                    {
                        return true;
                    }
                }

                return false;
            }))
            {
                return;
            }

            if (donateList.Count == 0)
            {
                return;
            }

            string notifyCont = string.Empty;

            bool isSelectedRedEquipment = false;//是否选择了红字装备
            for (int i = 0; i < donateList.Count; i++)
            {
                var ItemData = ItemDataManager.GetInstance().GetItem(donateList[i]);
                if (ItemData == null)
                {
                    continue; 
                }

                if (ItemData.EquipType != EEquipType.ET_REDMARK)
                {
                    continue;
                }

                isSelectedRedEquipment = true;
                break;
            }

            if (isSelectedRedEquipment)
            {
                notifyCont = TR.Value("selected_equipment_has_red_equip_desc","是否确认捐献?");
            }
            else
            {
                notifyCont = TR.Value("equip_submit_tip");
            }

            
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
            {
                EquipRecoveryDataManager.GetInstance()._SubmitEquip(donateList);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipSubmitScore, minPrice, maxPrice);
            });
            
        }
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onOpenPriceTableButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.GetInstance().OpenFrame<EquipPriceFrame>();
        }

        private void _onAuctionButtonClick()
        {
            frameMgr.OpenFrame<AuctionNewFrame>(FrameLayer.Middle);
        }
        #endregion
    }

}
