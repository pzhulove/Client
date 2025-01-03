using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;
using System;
using Scripts.UI;

namespace GameClient
{
    public class SecondPayFrame : ClientFrame
    {
        protected const string payRewardItemResPath = "UIFlatten/Prefabs/Vip/PayRewardItem";

        protected const string EffUI_shouchong_guang_Path = "UI/UIEffects/Skill_UI_Shouchong/Prefab/Skill_UI_Shouchong_guang01";
        protected const string EffUI_shouchong_anniu_Path = "UI/UIEffects/Skill_UI_Shouchong/Prefab/Skill_UI_Shouchong_anniu";

        enum PayReturnSrcollState
        {
            Head_Left_Most,
            Middle_Left,
            Middle_Right,
            Tail_Right_Most,
        }

        PayReturnSrcollState mPayReturnScrollState = PayReturnSrcollState.Middle_Right;

        int payRewardLevelShowNum = 3;
        int totalRewardLevelNum = 0;
        int payRewardLevelIndex = -1;

        bool flag = false;
        int index_flag = -1;
        int index_unfinish = -1;
        int index_canget = -1;
        int NowID = -1;

        bool hasSecondPay = false;
        bool bGotReward = false;

        string mToGetRewardText = "";
        string mNotGetRewardText = "";
        string mGotRewardText = "";

        GameObject effect_guang_go = null;
        GameObject effect_goPayBtn_go = null;

        //UI Component
        List<PayRewardItem> payRewardItems = new List<PayRewardItem>();
        List<PayRewardLevelTab> payRewardLevelTabs = new List<PayRewardLevelTab>();

        //Data
        Dictionary<uint, int> itemDataDic = new Dictionary<uint, int>();                    //当前档位充值回馈的奖励集合  道具id + 数目
        List<AwardItemData> awardItemDataList = new List<AwardItemData>();                  //将 当前档位充值回馈的奖励集合 里的数据重组为 数据结构（道具id + 数目） 易读
        List<ActiveManager.ActivityData> items = new List<ActiveManager.ActivityData>();    //全部档位充值回馈的奖励集合 （只是活动表中的奖励和）
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Vip/SecondPayFrame";
        }
        protected override void _OnOpenFrame()
        {
            if (userData != null)
            {
                hasSecondPay = (bool)userData;
            }

            //PayManager.GetInstance().InitPayReturnDisplayTable();

            BindEvent();
            InitTRDesc();
            InitEffectRoot();
            UpdateFrame();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SecondPayFrameOpen);
        }
        protected override void _OnCloseFrame()
        {
            //PayManager.GetInstance().ClearPayReturnDisplayTable();

            UnBindEvent();
            ClearData();
            ClearEffectRoot();
        }

        private void BindEvent()
        {
            NetProcess.AddMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPayRewardReceived, OnGotPayReward);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WelfareFrameClose, OnUpdateData);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMallFrameClosed, OnUpdateData);
        }

        private void UnBindEvent()
        {
            NetProcess.RemoveMsgHandler(SceneNotifyActiveTaskStatus.MsgID, OnRecvSceneNotifyActiveTaskStatus);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPayRewardReceived, OnGotPayReward);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WelfareFrameClose, OnUpdateData);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMallFrameClosed, OnUpdateData);
        }

        void InitEffectRoot()
        {
            if (effect_guang_go == null)
            {
                effect_guang_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_guang_Path);
                Utility.AttachTo(effect_guang_go, mEffectRoot_Backlight);
            }
            if (effect_goPayBtn_go == null)
            {
                effect_goPayBtn_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_anniu_Path);
                Utility.AttachTo(effect_goPayBtn_go, mEffectRoot_GoPayBtn);
            }
        }

        void ClearEffectRoot()
        {
            if (effect_guang_go)
            {
                GameObject.Destroy(effect_guang_go);
                effect_guang_go = null;
            }
            if (effect_goPayBtn_go)
            {
                GameObject.Destroy(effect_goPayBtn_go);
                effect_goPayBtn_go = null;
            }
        }

        private void InitTRDesc()
        {
            mToGetRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_toget");
            mNotGetRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_notget");
            mGotRewardText = TR.Value("vip_month_card_first_buy_next_pay_return_got");
        }
        public void OnUpdateData(UIEvent iEvent)
        {
            UpdateMainContent();
        }
		
        private void UpdateFrame()
        {
            UpdateMainContent();
            //UpdatePayRewardLevelTabs();
        }

        private void UpdateMainContent()
        {
            items = PayManager.GetInstance().GetConsumeItems(true);
            if (items == null)
            {
                return;
            }
            if (mHavePay != null)
            {
                mHavePay.text = string.Format("已充值：{0}元", PayManager.GetInstance().GetCurrentRolePayMoney());
            }
            UpdateIndex();

            if (index_canget != -1)
            {
                SetGetBtnShow(true);
                SetGoBtnShow(false);
                //rewardItemDataList = ActiveManager.GetInstance().GetActiveAwards(items[index_canget].ID);
            }
            else if (index_canget == -1 && index_unfinish != -1)
            {
                SetGetBtnShow(false);
                SetGoBtnShow(true);
                //rewardItemDataList = ActiveManager.GetInstance().GetActiveAwards(items[index_unfinish].ID);
            }

            if (index_canget != -1)
            {
                index_flag = index_canget;
            }
            else if (index_canget == -1 && index_unfinish != -1)
            {
                index_flag = index_unfinish;
            }

            UpdateItem();
        }

        /// <summary>
        /// 计算现在界面取第几个充值数据
        /// </summary>
        protected void UpdateIndex()
        {
            totalRewardLevelNum = items.Count;
            //Logger.LogError("totalRewardLevelNum = " + totalRewardLevelNum);

            bool flag2 = false;
            for (int i = 0; i < items.Count && !flag2; i++)
            {
                if (items[i].status == PayManager.STATUS_TASK_CANGET)
                {
                    index_canget = i;
                    flag2 = true;
                }
            }
            if (index_canget != -1)
            {
                return;
            }
            for (int i = 0; i < items.Count && !flag; ++i)
            {
                if (items[i].status == PayManager.STATUS_TASK_UNFINISH)
                {
                    flag = true;
                    index_unfinish = i;
                }
                //if (items[i].status == PayManager.STATUS_TASK_CANGET)
                //{
                //    index_canget = i;
                //}
            }
            if (payRewardLevelIndex != -1 && index_canget == -1)
            {
                index_unfinish = payRewardLevelIndex;
            }
        }

        void UpdateItem(bool bForceScorll = false)
        {
            int payActivityId = -1;
            ActiveManager.ActivityData selectPayActData = null;

            //Start to Refresh Scorll View
            //是否需要手动强制刷新数据 而不是根据当前充值档位 定位
            if (bForceScorll)
            {
                int fIndex = -1;
                switch (mPayReturnScrollState)
                {
                    case PayReturnSrcollState.Head_Left_Most:
                    case PayReturnSrcollState.Tail_Right_Most:
                        fIndex = index_flag;
                        break;
                    case PayReturnSrcollState.Middle_Left:
                        fIndex = index_flag - 1;
                        if (fIndex < 1)
                        {
                            fIndex = 1;
                        }
                        break;
                    case PayReturnSrcollState.Middle_Right:
                        fIndex = index_flag + 1;
                        if (fIndex > totalRewardLevelNum)
                        {
                            fIndex = totalRewardLevelNum - 1;
                        }
                        break;
                }
                index_flag = fIndex;
            }

            //从vip1 开始 是因为累充界面不包括首充了
            if (index_flag < 1)
            {
                index_flag = 1;
            }
            else if (index_flag >= totalRewardLevelNum)
            {
                index_flag = totalRewardLevelNum - 1;
            }

            if (itemDataDic != null)
            {
                itemDataDic.Clear();
            }
            if (awardItemDataList != null)
            {
                awardItemDataList.Clear();
            }
            itemDataDic = PayManager.GetInstance().GetAwardItems(items[index_flag]);

            if (itemDataDic != null)
            {
                var itemdataEnum = itemDataDic.GetEnumerator();
                while (itemdataEnum.MoveNext())
                {
                    AwardItemData award = new AwardItemData();
                    award.ID = (int)itemdataEnum.Current.Key;
                    award.Num = itemdataEnum.Current.Value;
                    if (award != null && !awardItemDataList.Contains(award))
                    {
                        awardItemDataList.Add(award);
                    }
                }
                //if (awardItemDataList != null)
                //{
                //    awardItemDataList.Sort((x, y) => x.Num.CompareTo(y.Num));
                //}
            }

            selectPayActData = items[index_flag];
            payActivityId = items[index_flag].ID;

            //if (mRMBValue)
            //{
            //    mRMBValue.text = items[index_flag].activeItem.Desc;
            //}

            if (mPayRMB)
            {
                //mPayRMB.SetRMBNum(items[index_flag].activeItem.Desc);
                mPayRMB.SetRMBNum_Ex(items[index_flag].activeItem.Desc);
            }

            //Logger.LogError("index_flag = " + index_flag);

            if (totalRewardLevelNum < index_flag + payRewardLevelShowNum)
            {
                payRewardLevelShowNum = totalRewardLevelNum - index_flag;
                //Logger.LogError("payRewardLevelShowNum = " + payRewardLevelShowNum);
            }

            UpdatePayRewardItems(payActivityId);

            if (selectPayActData.status == PayManager.STATUS_TASK_CANGET)
            {
                SetGetBtnStatus(true);
                SetGetBtnText(mToGetRewardText);
                SetHavePayShow(true);
            }
            else if (selectPayActData.status < PayManager.STATUS_TASK_CANGET)
            {
                SetGoBtnShow(true);
                SetGetBtnShow(false);
                SetHavePayShow(true);
            }
            else if (selectPayActData.status > PayManager.STATUS_TASK_CANGET)
            {
                SetGetBtnStatus(false); 
                SetGetBtnText(mGotRewardText);
                SetHavePayShow(false);
            }

            //刷新左右滚动按钮状态
            if (index_flag == 1)
            {
                mPayReturnScrollState = PayReturnSrcollState.Head_Left_Most;
                SetPayReturnLeftBtnActive(false);
                SetPayReturnRightBtnActive(true);
            }
            else if (index_flag == totalRewardLevelNum - 1)
            {
                mPayReturnScrollState = PayReturnSrcollState.Tail_Right_Most;
                SetPayReturnLeftBtnActive(true);
                SetPayReturnRightBtnActive(false);
            }
            else
            {
                SetPayReturnLeftBtnActive(true);
                SetPayReturnRightBtnActive(true);
            }
        }

        private void UpdatePayRewardItems(int selectPayActId)
        {
            //ClearPayRewardItems();

            if (mScrollUIListContentGrid)
            {
                mScrollUIListContentGrid.enabled = false;
            }

            if (awardItemDataList == null)
            {
                Logger.LogError("ItemdataList data is null");
            }
            if (mScrollUIList == null)
            {
                Logger.LogError("mScrollUIList obj is null");
                return;
            }

            //if (mScrollUIList.m_scrollRect != null)
            //{
            //    mScrollUIList.m_scrollRect.enabled = false;
            //}

            if (mScrollUIList.IsInitialised() == false)
            {
                mScrollUIList.Initialize();
                mScrollUIList.onBindItem = (GameObject go) =>
                {
                    PayRewardItem payItem = null;
                    if (go)
                    {
                        payItem = go.GetComponent<PayRewardItem>();
                    }
                    return payItem;
                };
            }
            mScrollUIList.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }
                int iIndex = var1.m_index;
                if (iIndex >= 0 && iIndex < awardItemDataList.Count)
                {
                    ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(awardItemDataList[iIndex].ID);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("Can find !!! Please Check item data id {0} !!!", awardItemDataList[iIndex].ID);
                        return;
                    }
                    itemDetailData.Count = awardItemDataList[iIndex].Num;
                    PayRewardItem payItem = var1.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(this, itemDetailData, true , false);
                        payItem.RefreshView();
                        if (payRewardItems != null && !payRewardItems.Contains(payItem))
                        {
                            payRewardItems.Add(payItem);
                        }
                    }
                }
            };
            //mScrollUIList.m_autoCenteredElements = true;
            mScrollUIList.SetElementAmount(awardItemDataList.Count);
            //mScrollUIList.ResetContentPosition();

            if (mSpecialItem && selectPayActId > 0)
            {
                int specialItemDataId = PayManager.GetInstance().GetPayReturnSpecialResID(selectPayActId, awardItemDataList);
                ItemData detailData = ItemDataManager.CreateItemDataFromTable(specialItemDataId);
                if (detailData == null)
                {
                    Logger.LogErrorFormat("Can find !!! Please Check item data id {0} !!!", specialItemDataId);
                    return;
                }
                mSpecialItem.Initialize(this, detailData, false);
                mSpecialItem.RefreshView(false, false);
                string specialItemIconPath = PayManager.GetInstance().GetPayReturnSpecialResPath(selectPayActId, awardItemDataList);
                mSpecialItem.SetItemIcon(specialItemIconPath);
                mSpecialItem.onPayItemClick = () =>
                {
                    ItemTipManager.GetInstance().ShowTip(detailData);
                };
            }

            if (mScrollUIListContentGrid)
            {
                mScrollUIListContentGrid.enabled = true;
            }
        }

        private void UpdatePayRewardLevelTabs()
        {
            if (index_canget != -1 || hasSecondPay || payRewardLevelShowNum <= 1)
            {
                if (mPayTabs)
                {
                    mPayTabs.CustomActive(false);
                }
                return;
            }
            if (mPayTabs)
            {
                mPayTabs.CustomActive(true);
            }

           // ClearPayRewardLevelTabs();

            if (items == null)
            {
                Logger.LogError("items data is null");
                return;
            }
            if (mTabScrollUIList == null)
            {
                Logger.LogError("mTabScrollUIList list is null");
                return;
            }

            mTabScrollUIList.Initialize();
            mTabScrollUIList.onBindItem = (GameObject go) =>
            {
                PayRewardLevelTab payLevelTab = null;
                if (go)
                {
                    payLevelTab = go.GetComponent<PayRewardLevelTab>();
                }
                return payLevelTab;
            };
            mTabScrollUIList.onItemVisiable = (var2) =>
            {
                if (var2 == null)
                {
                    return;
                }
                int iIndex = var2.m_index;
                int addedIndex = iIndex + index_flag;
                if (iIndex >= 0 && addedIndex < items.Count)
                {
                    PayRewardLevelTab payLevelTab = var2.gameObjectBindScript as PayRewardLevelTab;
                    if (payLevelTab == null)
                    {
                        return;
                    }
                    payLevelTab.Initialize();
                    payLevelTab.SetPayRewardLevelIndex(addedIndex);
                    payLevelTab.SetTabText(items[addedIndex].activeItem.Desc);
                    payLevelTab.onPayRewardLevelTabChanged = () =>
                    {
                        payRewardLevelIndex = addedIndex;
                        ClearData(false);
                        UpdateMainContent();
                    };
                    if (iIndex == 0)
                    {
                        payLevelTab.SetTabActive(true);
                    }
                    if (payRewardLevelTabs != null && !payRewardLevelTabs.Contains(payLevelTab))
                    {
                        payRewardLevelTabs.Add(payLevelTab);
                    }
                }
            };
            mTabScrollUIList.SetElementAmount(payRewardLevelShowNum);
        }

        private void ClearPayRewardItems()
        {
            if (payRewardItems != null)
            {
                for (int i = 0; i < payRewardItems.Count; i++)
                {
                    payRewardItems[i].Clear();
                }
                payRewardItems.Clear();
            }

            if (mSpecialItem != null)
            {
                mSpecialItem.Clear();
            }
        }

        private void ClearPayRewardLevelTabs()
        {
            if (payRewardLevelTabs != null)
            {
                for (int i = 0; i < payRewardLevelTabs.Count; i++)
                {
                    payRewardLevelTabs[i].Clear();
                }
                payRewardLevelTabs.Clear();
            }
        }

        private void ClearData(bool isClearWithTabs = true)
        {
            hasSecondPay = false;
            bGotReward = false;

            index_flag = -1;
            index_unfinish = -1;
            index_canget = -1;
            NowID = -1;
            flag = false;

            if (itemDataDic != null)
            {
                itemDataDic.Clear();
            }
            if (awardItemDataList != null)
            {
                awardItemDataList.Clear();
            }
            if (items != null)
            {
                items.Clear();
            }
            payRewardLevelShowNum = 3;
            totalRewardLevelNum = 0;

            ClearPayRewardItems();

            if (isClearWithTabs)
            {
                ClearPayRewardLevelTabs();
                //重置充值额度 跳转序号
                payRewardLevelIndex = -1;
            }

            //重置状态
            mPayReturnScrollState = PayReturnSrcollState.Middle_Right;
        }

        void SetGetBtnShow(bool bShow)
        {
            if (mBtnGet)
            {
                mBtnGet.CustomActive(bShow);
            }
        }

        void SetGoBtnShow(bool bShow)
        {
            if (mBtnGo)
            {
                mBtnGo.CustomActive(bShow);
            }

            if (mEffectRoot_GoPayBtn && bShow)
            {
                mEffectRoot_GoPayBtn.CustomActive(true);
            }
        }

        void SetHavePayShow(bool bShow)
        {
            if (mHavePay)
            {
                mHavePay.CustomActive(bShow);
            }
        }

        void SetGetBtnStatus(bool active)
        {
            SetGoBtnShow(false);
            SetGetBtnShow(true);
            SetAlreadyGetBtn(active);

            if (mBtnGetGray)
            {
                mBtnGetGray.enabled = !active;
            }
            if (mBtnGet)
            {
                mBtnGet.interactable = active;
            }

            if (mBtnGetPayCD && active)
            {
                mBtnGetPayCD.StopCountDown();
            }

            if (mEffectRoot_GoPayBtn)
            {
                mEffectRoot_GoPayBtn.CustomActive(active);
            }
        }

        void SetAlreadyGetBtn(bool show)
        {
            if (mGetTextImg)
            {
                mGetTextImg.CustomActive(show);
            }
            if (mAlreadyGetImg)
            {
                mAlreadyGetImg.CustomActive(!show);
            }
        }

        void SetGetBtnText(string desc)
        {
            if (mBtnGetText)
            {
                mBtnGetText.text = desc;
            }
        }

        bool BeGetBtnCDOver()
        {
            if (mBtnGetPayCD != null)
            {
                return mBtnGetPayCD.bCDOver;
            }
            return true;
        }

        void SetPayReturnLeftBtnActive(bool bActive)
        {
            if (mPayReturnLeftBtn)
            {
                //mPayReturnLeftBtn.gameObject.CustomActive(bActive);
                var gray = mPayReturnLeftBtn.gameObject.GetComponent<UIGray>();
                if (gray == null)
                {
                    gray = mPayReturnLeftBtn.gameObject.AddComponent<UIGray>();
                }
                gray.enabled = !bActive;
                mPayReturnLeftBtn.interactable = bActive;

                mPayReturnLeftBtn.gameObject.CustomActive(bActive);
            }

            if (mEffectRoot_LeftBtn)
            {
                mEffectRoot_LeftBtn.CustomActive(bActive);
            }
        }

        void SetPayReturnRightBtnActive(bool bActive)
        {
            if (mPayReturnRightBtn)
            {
               // mPayReturnRightBtn.gameObject.CustomActive(bActive);
                var gray = mPayReturnRightBtn.gameObject.GetComponent<UIGray>();
                if (gray == null)
                {
                    gray = mPayReturnRightBtn.gameObject.AddComponent<UIGray>();
                }
                gray.enabled = !bActive;
                mPayReturnRightBtn.interactable = bActive;

                mPayReturnRightBtn.gameObject.CustomActive(bActive);
            }

            if (mEffectRoot_RightBtn)
            {
                mEffectRoot_RightBtn.CustomActive(bActive);
            }
        }

        #region EventCallback

        void OnRecvSceneNotifyActiveTaskStatus(MsgDATA data)
        {
            SceneNotifyActiveTaskStatus kRecv = new SceneNotifyActiveTaskStatus();
            kRecv.decode(data.bytes);
            if (kRecv.taskId == NowID && kRecv.status == (byte)Protocol.TaskStatus.TASK_OVER)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdatePayText);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPayRewardReceived);
            }
        }

        void OnGotPayReward(UIEvent uiEvent)
        {
            if (PayManager.GetInstance().HasConsumePay() || mPayReturnScrollState != PayReturnSrcollState.Tail_Right_Most)
            {
                ClearData();
                bGotReward = true;
                UpdateFrame();
                //if (mBtnGet)
                //{
                //    if (mBtnGet.gameObject.activeSelf == false)
                //    {
                //        ClientSystemManager.GetInstance().CloseFrame<SecondPayFrame>();
                //    }
                //}
            }
            else
            {
                ClientSystemManager.GetInstance().CloseFrame<SecondPayFrame>();
            }
        }

        [UIEventHandle("btnClose")]
        void OnClickClose()
        {
            ClientSystemManager.instance.CloseFrame<SecondPayFrame>();
        }

        //[UIControl("Content/Icon{0}", typeof(RectTransform), 0)]
        //RectTransform[] Icon = new RectTransform[5];

        void OnShowTipsFromJob(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            if (ItemDetailData == null)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }
        void OnShowTips(int itemID)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemID);
            if (ItemDetailData == null)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        #endregion

		#region ExtraUIBind
		private Button mBtnGo = null;
		private UIGray mBtnGetGray = null;
		private Text mBtnGetText = null;
		private PayButtonCountDown mBtnGetPayCD = null;
		private Button mBtnGet = null;
		private Text mRMBValue = null;
		private Text mTotalValue = null;
		private Text mHavePay = null;
		private PayRewardItem mSpecialItem = null;
		private ComUIListScript mScrollUIList = null;
		private ComUIListScript mTabScrollUIList = null;
		private GameObject mPayTabs = null;
		private Button mGotoMoneyplan = null;
		private Button mGotoMonthCard = null;
		private Button mPayReturnRightBtn = null;
		private Button mPayReturnLeftBtn = null;
		private PayRMBItem mPayRMB = null;
		private GridLayoutGroup mScrollUIListContentGrid = null;
		private GameObject mEffectRoot_Backlight = null;
		private GameObject mEffectRoot_LeftBtn = null;
		private GameObject mEffectRoot_RightBtn = null;
		private GameObject mEffectRoot_GoPayBtn = null;

		private GameObject mGetTextImg = null;
		private GameObject mAlreadyGetImg = null;

		
		protected override void _bindExUI()
		{
			mBtnGo = mBind.GetCom<Button>("btnGo");
			if (null != mBtnGo)
			{
				mBtnGo.onClick.AddListener(_onBtnGoButtonClick);
			}
			mBtnGetGray = mBind.GetCom<UIGray>("btnGetGray");
			mBtnGetText = mBind.GetCom<Text>("btnGetText");
			mBtnGetPayCD = mBind.GetCom<PayButtonCountDown>("btnGetPayCD");
			mBtnGet = mBind.GetCom<Button>("btnGet");
			if (null != mBtnGet)
			{
				mBtnGet.onClick.AddListener(_onBtnGetButtonClick);
			}
			mRMBValue = mBind.GetCom<Text>("RMBValue");
			mTotalValue = mBind.GetCom<Text>("TotalValue");
			mHavePay = mBind.GetCom<Text>("HavePay");
			mSpecialItem = mBind.GetCom<PayRewardItem>("SpecialItem");
			mScrollUIList = mBind.GetCom<ComUIListScript>("ScrollUIList");
			mTabScrollUIList = mBind.GetCom<ComUIListScript>("TabScrollUIList");
			mPayTabs = mBind.GetGameObject("PayTabs");
			mGotoMoneyplan = mBind.GetCom<Button>("gotoMoneyplan");
			if (null != mGotoMoneyplan)
			{
				mGotoMoneyplan.onClick.AddListener(_onGotoMoneyplanButtonClick);
			}
			mGotoMonthCard = mBind.GetCom<Button>("gotoMonthCard");
			if (null != mGotoMonthCard)
			{
				mGotoMonthCard.onClick.AddListener(_onGotoMonthCardButtonClick);
			}
			mPayReturnRightBtn = mBind.GetCom<Button>("payReturnRightBtn");
			if (null != mPayReturnRightBtn)
			{
				mPayReturnRightBtn.onClick.AddListener(_onPayReturnRightBtnButtonClick);
			}
			mPayReturnLeftBtn = mBind.GetCom<Button>("payReturnLeftBtn");
			if (null != mPayReturnLeftBtn)
			{
				mPayReturnLeftBtn.onClick.AddListener(_onPayReturnLeftBtnButtonClick);
			}
			mPayRMB = mBind.GetCom<PayRMBItem>("PayRMB");
			mScrollUIListContentGrid = mBind.GetCom<GridLayoutGroup>("ScrollUIListContentGrid");
			mEffectRoot_Backlight = mBind.GetGameObject("EffectRoot_Backlight");
			mEffectRoot_LeftBtn = mBind.GetGameObject("EffectRoot_LeftBtn");
			mEffectRoot_RightBtn = mBind.GetGameObject("EffectRoot_RightBtn");
			mEffectRoot_GoPayBtn = mBind.GetGameObject("EffectRoot_GoPayBtn");

            mGetTextImg = mBind.GetGameObject("GetTextImg");
            mAlreadyGetImg = mBind.GetGameObject("AlreadyGetImg");
		}
		
		protected override void _unbindExUI()
		{
			if (null != mBtnGo)
			{
				mBtnGo.onClick.RemoveListener(_onBtnGoButtonClick);
			}
			mBtnGo = null;
			mBtnGetGray = null;
			mBtnGetText = null;
			mBtnGetPayCD = null;
			if (null != mBtnGet)
			{
				mBtnGet.onClick.RemoveListener(_onBtnGetButtonClick);
			}
			mBtnGet = null;
			mRMBValue = null;
			mTotalValue = null;
			mHavePay = null;
			mSpecialItem = null;
			mScrollUIList = null;
			mTabScrollUIList = null;
			mPayTabs = null;
			if (null != mGotoMoneyplan)
			{
				mGotoMoneyplan.onClick.RemoveListener(_onGotoMoneyplanButtonClick);
			}
			mGotoMoneyplan = null;
			if (null != mGotoMonthCard)
			{
				mGotoMonthCard.onClick.RemoveListener(_onGotoMonthCardButtonClick);
			}
			mGotoMonthCard = null;
			if (null != mPayReturnRightBtn)
			{
				mPayReturnRightBtn.onClick.RemoveListener(_onPayReturnRightBtnButtonClick);
			}
			mPayReturnRightBtn = null;
			if (null != mPayReturnLeftBtn)
			{
				mPayReturnLeftBtn.onClick.RemoveListener(_onPayReturnLeftBtnButtonClick);
			}
			mPayReturnLeftBtn = null;
			mPayRMB = null;
			mScrollUIListContentGrid = null;
			mEffectRoot_Backlight = null;
			mEffectRoot_LeftBtn = null;
			mEffectRoot_RightBtn = null;
			mEffectRoot_GoPayBtn = null;

            mGetTextImg = null;
            mAlreadyGetImg = null;
    }
		#endregion

        #region Callback
        private void _onBtnGoButtonClick()
        {
            /* put your code in here */
            OnClickClose();
            if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MallNewFrame>();
            }
            var mallNewFrame = ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.ReChargeMall }) as MallNewFrame;
        }
        private void _onBtnGetButtonClick()
        {
            /* put your code in here */
            if (items == null || items.Count <= index_flag)
            {
                return;
            }
            if (index_flag < 0)
            {
                Logger.LogError("try to click get reward btn , but get index is less than zero !");
                return;
            }

            NowID = items[index_flag].ID;
            PayManager.GetInstance().GetRewards(NowID);

            if (mBtnGetPayCD)
            {
                mBtnGetPayCD.StartCountDown();
                mBtnGetPayCD.onCDOverHandler = () => {
                    if(bGotReward)
                    {
                        return;
                    }
                    SetGetBtnStatus(true);
                };
                SetGetBtnStatus(mBtnGetPayCD.bCDOver);
            }
        }

        private void _onGotoMoneyplanButtonClick()
        {
            /* put your code in here */
            const int iConfigID = 9380;
            const int iFinancialPlanID = 8600;
            string frameName = typeof(ActiveChargeFrame).Name + iConfigID.ToString();
            if (ClientSystemManager.GetInstance().IsFrameOpen(frameName))
            {
                var frame = ClientSystemManager.GetInstance().GetFrame(frameName) as ActiveChargeFrame;
                frame.Close(true);
            }
            ActiveManager.GetInstance().OpenActiveFrame(iConfigID, iFinancialPlanID);
        }
        private void _onGotoMonthCardButtonClick()
        {
            /* put your code in here */
            //OnClickClose();

            if (ClientSystemManager.GetInstance().IsFrameOpen<MallNewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MallNewFrame>();
            }
            //var mallFrame = ClientSystemManager.instance.OpenFrame<MallFrame>(FrameLayer.Middle, new OutComeData() { MainTab = MallType.Recharge }) as MallFrame;
            var mallFrame = ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.ReChargeMall}) as MallNewFrame;
        }
        private void _onPayReturnRightBtnButtonClick()
        {
            if (mPayReturnScrollState == PayReturnSrcollState.Tail_Right_Most)
            {
                return;
            }
            mPayReturnScrollState = PayReturnSrcollState.Middle_Right;
            UpdateItem(true);
        }
        private void _onPayReturnLeftBtnButtonClick()
        {
            if (mPayReturnScrollState == PayReturnSrcollState.Head_Left_Most)
            {
                return;
            }
            mPayReturnScrollState = PayReturnSrcollState.Middle_Left;
            UpdateItem(true);
        }

        #endregion
    }

}

