using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;
using System;
using Scripts.UI;

namespace GameClient
{
    class VipFrameData
    {
        public int iTab;
    }

	public enum VipTabType
	{
		PAY = 0,
		PAY_RETRN = 1,
		VIP = 2, 
		
		COUNT
	}

    class VipFrame : ClientFrame
    {
        protected const string EffUI_shouchong_jiantou01_Path = "Effects/UI/Prefab/EffUI_shouchong/Prefab/EffUI_shouchong_jiantou01";

        public const string ICON_VIP_RES_PATH_FORMAT = "UI/Image/NewPacked/Vip_Icon.png:Vip_0{0}";
        const string VIP_DES_ELEMENT_PATH = "UIFlatten/Prefabs/Vip/VipElement";

        VipTabType mCurTabIndex = VipTabType.COUNT;
        VipTabType mPreTabIndex = VipTabType.COUNT;
        VipFrameData mFrameData = null;

        #region Pay View

        PayFrame mPayFrameManager;

        #endregion

        #region Pay Return View

        //List<PayConsumeItem4> mConsumeItems = new List<PayConsumeItem4>();    
        bool mPayFrameOpened;
        PayConsumeItem5 mPayReturnItem = null;
        enum PayReturnSrcollState
        {
            Head_Left_Most,
            Middle_Left,
            Middle_Right,
            Tail_Right_Most,
        }

        PayReturnSrcollState mPayReturnScrollState = PayReturnSrcollState.Middle_Right;
        int mPayReturnScorllIndex = -1;

        GameObject effect_left_jiantou_go = null;
        GameObject effect_right_jiantou_go = null;

        #endregion

        #region Pay Priviliege View

        const int MaxItemNum = 6;
        //int mCurShowVipLevel = 0;
        int mPrivilegeNum = 0;
        int mMaxVipLevel = 0;

        List<GameObject> mDesObjList = new List<GameObject>();
        List<ComItem> mItemList = new List<ComItem>();
        List<ItemData> mShowTipItemData = new List<ItemData>();
        List<int> mVipGiftList = new List<int>();

        PayPrivilegeFrameData payPrivilegeFrameData = new PayPrivilegeFrameData();

        PayPrivilegeFrame mPayPrivilegeFrame = null;

        #endregion

        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                int iTab = 0;

                if(int.TryParse(strParam, out iTab))
                {
                    VipTabType vipType = (VipTabType)iTab;
                    ClientSystemManager.GetInstance().CloseFrame<VipFrame>();
                    ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, vipType);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Vip/VipNewFrame";
        }

        protected override void _OnOpenFrame()
        {
            //PayManager.GetInstance().InitPayReturnDisplayTable();

            InitInterface();
            BindUIEvent();

            if (userData != null)
            {
                VipTabType type = (VipTabType)userData;
                SwitchPage(type);
            }
            else
            {
                SwitchPage(VipTabType.VIP);
            }
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VipFrameOpen);
#if APPLE_STORE			
            TryChangeVipTabTitleForIOS();
#endif
        }

        protected override void _OnCloseFrame()
        {
            //PayManager.GetInstance().ClearPayReturnDisplayTable();

            MonthCardBuyResult.bBuyMonthCard = false;

            UnBindUIEvent();
            ClearData();
            mPayFrameManager.Close();
            ClosePayReturn();
            ClosePayPrivilege();
            mFrameData = null;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VipFrameClose);
        }

        void ClearData()
        {
            mMaxVipLevel = 0;
            mPrivilegeNum = 0;
            mCurTabIndex = VipTabType.COUNT;
            mPreTabIndex = VipTabType.COUNT;
            //mCurShowVipLevel = 0;
            mFrameData = null;

            mPayReturnItem = null;

            mPayFrameOpened = false;

            mDesObjList.Clear();
            mItemList.Clear();
            mShowTipItemData.Clear();
            mVipGiftList.Clear();

            //重置状态
            mPayReturnScrollState = PayReturnSrcollState.Middle_Right;
            mPayReturnScorllIndex = -1;


            if (payPrivilegeFrameData != null)
            {
                payPrivilegeFrameData.ClearData();
            }
            mPayPrivilegeFrame = null;

        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPayResultNotify, OnPaySuccess);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, OnCounterChanged);            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdatePayData, OnUpdatePayData);

            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SetPayReturnTab, OnSwitchPayReturnTab);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerVipLvChanged, OnPaySuccess);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VipPrivilegeFrameOpen, OnPrivilegeFrameOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VipPrivilegeFrameClose, OnPrivilegeFrameClose);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPayResultNotify, OnPaySuccess);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, OnCounterChanged);           
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdatePayData, OnUpdatePayData);

            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SetPayReturnTab, OnSwitchPayReturnTab);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerVipLvChanged, OnPaySuccess);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VipPrivilegeFrameOpen, OnPrivilegeFrameOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VipPrivilegeFrameClose, OnPrivilegeFrameClose);
        }

        void InitInterface()
        {
            if (PlayerBaseData.GetInstance().VipLevel == -1)
            {
                Logger.LogError("PlayerBaseData.GetInstance().VipLevel doesn't init!");
            }

            InitData();
            //CreateElementPrefab();
            UpdateShowUpLevelData();
        }

        void InitData()
        {
            Dictionary<int, object> tabledata = TableManager.GetInstance().GetTable<VipPrivilegeTable>();
            if (tabledata != null)
            {
                mPrivilegeNum = tabledata.Count;
            }
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);
            if (SystemValueTableData != null)
            {
                mMaxVipLevel = SystemValueTableData.Value;
            }
            if (payPrivilegeFrameData != null)
            {
                payPrivilegeFrameData.PrivilegeNum = mPrivilegeNum;
                payPrivilegeFrameData.MaxVipLevel = mMaxVipLevel;
                payPrivilegeFrameData.CurShowVipLevel = PlayerBaseData.GetInstance().VipLevel;
            }

            if (PlayerBaseData.GetInstance().VipLevel >= mMaxVipLevel)
            {
                if (mTargetLvRootObj)
                {
                    mTargetLvRootObj.CustomActive(false);
                }
                if (mVipMaxText)
                {
                    mVipMaxText.gameObject.CustomActive(true);
                }

                if (mVipHead2Target)
                {
                    mVipHead2Target.CustomActive(false);
                }
            }

            InitPayFrameManager();
        }

        void InitPayFrameManager()
        {
            if (goPay == null || goPayContent == null)
            {
                Logger.LogError("InitPayFrameManager - out params is null");
                return;
            }
            mPayFrameManager = new PayFrame(goPay, goPayContent);
        }

        void UpdateShowUpLevelData()
        {
            if (payPrivilegeFrameData != null)
            {
                payPrivilegeFrameData.CurShowVipLevel = PlayerBaseData.GetInstance().VipLevel;
            }

            if (mCurVipLvSec)
            {
                mCurVipLvSec.gameObject.CustomActive(true);
            }

            if (mCurVipLv)
            {
                mCurVipLv.gameObject.CustomActive(PlayerBaseData.GetInstance().VipLevel >= 10);
            }

            if (PlayerBaseData.GetInstance().VipLevel < 10)
            {
                ETCImageLoader.LoadSprite(ref mCurVipLvSec, string.Format(ICON_VIP_RES_PATH_FORMAT, PlayerBaseData.GetInstance().VipLevel));
            }
            else
            {
                ETCImageLoader.LoadSprite(ref mCurVipLv, string.Format(ICON_VIP_RES_PATH_FORMAT, PlayerBaseData.GetInstance().VipLevel % 10));

                ETCImageLoader.LoadSprite(ref mCurVipLvSec, string.Format(ICON_VIP_RES_PATH_FORMAT, PlayerBaseData.GetInstance().VipLevel / 10));
            }

            if (PlayerBaseData.GetInstance().VipLevel >= mMaxVipLevel)
            {
                if (mTargetLvRootObj)
                {
                    mTargetLvRootObj.CustomActive(false);
                }
                if (mVipMaxText)
                {
                    mVipMaxText.gameObject.CustomActive(true);
                }

                if (mVipHead2Target)
                {
                    mVipHead2Target.CustomActive(false);
                }
            }
            else
            {
                var VipData = TableManager.GetInstance().GetTableItem<VipTable>(PlayerBaseData.GetInstance().VipLevel);
                if (VipData != null)
                {
                    if (mTargetLvRootObj)
                    {
                        mTargetLvRootObj.CustomActive(true);
                    }
                    if (mRechargeMoneyNum)
                    {
                        mRechargeMoneyNum.text = TR.Value("vip_month_card_first_buy_cost_desc", VipData.TotalRmb - PlayerBaseData.GetInstance().CurVipLvRmb); //"{0}元"
                    }

                    //目标等级
                    if (mVipHead2Target)
                    {
                        mVipHead2Target.CustomActive(true);
                    }
                    int _nextVipLevel = PlayerBaseData.GetInstance().VipLevel + 1;

                    if (mVipLevel1Target)
                    {
                        mVipLevel1Target.gameObject.CustomActive(true);
                    }
                    if (mVipLevel2Target)
                    {
                        mVipLevel2Target.gameObject.CustomActive(_nextVipLevel >= 10);
                    }

                    if (_nextVipLevel < 10)
                    {
                        ETCImageLoader.LoadSprite(ref mVipLevel1Target, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, _nextVipLevel));
                    }
                    else
                    {
                        ETCImageLoader.LoadSprite(ref mVipLevel1Target, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, _nextVipLevel / 10));
                        ETCImageLoader.LoadSprite(ref mVipLevel2Target, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, _nextVipLevel % 10));
                    }

                    //注意顺序 ETC会重新设置 材质球
                    if (mVipHead2TargetGray)
                    {
                        mVipHead2TargetGray.enabled = false;
                        mVipHead2TargetGray.enabled = true;
                    }
                }
            }

            DrawVipLevelExpBar(PlayerBaseData.GetInstance().VipLevel, (ulong)PlayerBaseData.GetInstance().CurVipLvRmb, true);
        }

        void DrawVipLevelExpBar(int iVipLevel, UInt64 CurVipLvExp, bool force = false)
        {
            if (mCostExp)
            {
                mCostExp.SetExp(CurVipLvExp, force, exp =>
                {
                    return TableManager.instance.GetCurVipLevelExp(iVipLevel, exp);
                });
            }
        }

        void ShowPay(bool show)
        {
            if (show)
            {
                if (!mPayFrameManager.isOpened)
                    mPayFrameManager.Open();
                else
                    mPayFrameManager.Show(show);
            }
            else
            {
                mPayFrameManager.Show(show);
            }
        }


        #region Pay Privilege View

        void ShowPayPrivilege()
        {
            if (mPayPrivilegeFrame == null)
            {
                mPayPrivilegeFrame = new PayPrivilegeFrame(payPrivilegeFrameData, mPayPrivilegeContent, this);
                mPayPrivilegeFrame.UpdateView();
            }
            else
            {
                mPayPrivilegeFrame.UpdateView(payPrivilegeFrameData);
            }
        }

        void ClosePayPrivilege()
        {
            if (mPayPrivilegeFrame != null)
            {
                mPayPrivilegeFrame.DestroyView();
            }
        }

        void HidePayPrivilege()
        {
            if (mPayPrivilegeFrame != null)
            {
                mPayPrivilegeFrame.CloseView();
            }
        }

        #endregion

        #region Pay Return View

        void InitEffectRoot()
        {
            //if (effect_left_jiantou_go == null)
            //{
            //    effect_left_jiantou_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_jiantou01_Path);
            //    Utility.AttachTo(effect_left_jiantou_go, mEffectRoot_LeftBtn);
            //}
            //if (effect_right_jiantou_go == null)
            //{
            //    effect_right_jiantou_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_jiantou01_Path);
            //    Utility.AttachTo(effect_right_jiantou_go, mEffectRoot_RightBtn);
            //}
        }

        void ClearEffectRoot()
        {
            if (effect_left_jiantou_go)
            {
                GameObject.Destroy(effect_left_jiantou_go);
                effect_left_jiantou_go = null;
            }
            if (effect_right_jiantou_go)
            {
                GameObject.Destroy(effect_right_jiantou_go);
                effect_right_jiantou_go = null;
            }
        }

        void ShowPayReturn()
        {
            if (mPayFrameOpened)
            {
                RefreshPayReturn();
            }
            else
            {
                mPayFrameOpened = true;
                OpenPayReturn();
            }
        }

        void OpenPayReturn()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnPayResultNotify, OnReceivePayResult);

            SetPayReturnItem();
            InitEffectRoot();
        }

        void SetPayReturnItem(bool bForceScorll = false)
        {
            bool flag = false;
            int index = -1;
            int totalCount = -1;
            string curPay = "", nextPay = "";

            var items = PayManager.GetInstance().GetConsumeItems(true);
            if (items == null)
            {
                Logger.LogError("[Pay Return] - PayManager get consume items is null !");
                return;
            }
            totalCount = items.Count;
            for (int i = 0; i < totalCount; ++i)
            {
                if (!flag && (items[i].status == PayManager.STATUS_TASK_UNFINISH))
                {
                    flag = true;
                    if (items[i].akActivityValues.Count < 2)
                    {
                        curPay = "0"; //"0";
                        nextPay = PayManager.FIRST_PAT_RMB_NUM.ToString(); //"6";
                    }
                    else
                    {
                        curPay = items[i].akActivityValues[0].value;
                        nextPay = items[i].akActivityValues[1].value;
                    }

                    index = i;
                }
            }

            for (int i = 0; i < totalCount; ++i)
            {
                if (items[i].status == PayManager.STATUS_TASK_CANGET)
                {
                    index = i;
                    break;
                }
            }

            //Start to Refresh Scorll View
            //是否需要手动强制刷新数据 而不是根据当前充值档位 定位
            if (bForceScorll)
            {
                int fIndex = -1;
                switch (mPayReturnScrollState)
                {
                    case PayReturnSrcollState.Head_Left_Most:
                    case PayReturnSrcollState.Tail_Right_Most:
                        fIndex = mPayReturnScorllIndex;
                        break;
                    case PayReturnSrcollState.Middle_Left:
                        fIndex = mPayReturnScorllIndex - 1;
                        if (fIndex < 0)
                        {
                            fIndex = 0;
                        }
                        break;
                    case PayReturnSrcollState.Middle_Right:
                        fIndex = mPayReturnScorllIndex + 1;
                        if (fIndex > totalCount)
                        {
                            fIndex = totalCount - 1;
                        }
                        break;
                }
                mPayReturnScorllIndex = fIndex;
            }
            else
            {
                mPayReturnScorllIndex = index;
            }

            //保证显示正常 当领取完所有奖励时
            if (mPayReturnScorllIndex < 0)
            {
                mPayReturnScorllIndex = totalCount - 1;
            }

            if (totalCount > mPayReturnScorllIndex && mPayReturnScorllIndex >= 0)
            {
                //创建和刷新滚动区域状态
                if (mPayReturnItem == null)
                {
                    if (mPayReturnContent)
                    {
                        mPayReturnItem = new PayConsumeItem5(items[mPayReturnScorllIndex], mPayReturnContent, this);
                        mPayReturnScrollState = PayReturnSrcollState.Middle_Right;
                    }
                }
                else
                {
                    //使用新数据刷新状态
                    mPayReturnItem.RefreshView(items[mPayReturnScorllIndex]);
                }
            }

            //刷新左右滚动按钮状态
            if (mPayReturnScorllIndex == 0)
            {
                mPayReturnScrollState = PayReturnSrcollState.Head_Left_Most;
                SetPayReturnLeftBtnActive(false);
                SetPayReturnRightBtnActive(true);
            }
            else if (mPayReturnScorllIndex == totalCount - 1)
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

        void RefreshPayReturn()
        {
            //if(mConsumeItems==null)
            //{
            //    return;
            //}
            //if(mConsumeItems.Count<=0)
            //{
            //    return;
            //}

            //for (int i = 0; i < mConsumeItems.Count; ++i)
            //    mConsumeItems[i].SetStat();

            if (mPayReturnItem == null)
            {
                return;
            }
            //使用当前数据刷新状态
            mPayReturnItem.RefreshView();
        }

        void ClosePayReturn()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnPayResultNotify, OnReceivePayResult);
            //mConsumeItems.Clear();

            if (mPayReturnItem != null)
            {
                mPayReturnItem.Clear();
            }

            ClearEffectRoot();

            mPayFrameOpened = false;
        }

        void SetPayReturnLeftBtnActive(bool bActive)
        {
            if (mPayReturnLeftBtn)
            {
                // mPayReturnLeftBtn.gameObject.CustomActive(bActive);
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

        #endregion

        [UIEventHandle("middle/titleback/Tab{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, 3)]
        void OnSwitchLabel(int iIndex, bool value = true)
        {
            if (iIndex < (int)VipTabType.PAY || !value)
            {
                return;
            }

            SwitchPage((VipTabType)iIndex, false);
        }

        public void SwitchPage(VipTabType index, bool setTab = true)
        {
            if (mCurTabIndex == index)
                return;
            mPreTabIndex = mCurTabIndex;
            mCurTabIndex = index;
            FuncTab[(int)mCurTabIndex].isOn = true;

            for (int i = 0; i < (int)VipTabType.COUNT; ++i)
            {
                goControlTabs[i].CustomActive(false);
            }

            //if (index == VipTabType.VIP)
            //{
            //    goControlTabs[(int)index].CustomActive(false);
            //}
            //else
            //{
            //    goControlTabs[(int)index].CustomActive(true);
            //}

            goControlTabs[(int)index].CustomActive(true);
            //重置支付界面
            ShowPay(false);
            //重置特权界面
            HidePayPrivilege();
            //if (ClientSystemManager.GetInstance().IsFrameOpen<PayPrivilegeFrame>())
            //{
            //    ClientSystemManager.GetInstance().CloseFrame<PayPrivilegeFrame>();
            //}
            switch (mCurTabIndex)
            {
                case VipTabType.PAY:
                    //RechargeText.text = "贵族特权";
                    ShowPay(true);
                    //ShowVipPrivilegeTab(true);
                    break;
                case VipTabType.PAY_RETRN:
                    //RechargeText.text = "充值";
                    ShowPayReturn();
                    //ShowVipPrivilegeTab(true);
                    break;
                case VipTabType.VIP:
                    //RechargeText.text = "充值";
                    //UpdateVipPrivilege();
                    //ShowVipPrivilegeTab(false);
                    //ClientSystemManager.GetInstance().OpenFrame<PayPrivilegeFrame>(FrameLayer.Middle, payPrivilegeFrameData);
                    ShowPayPrivilege();
                    break;
            }
        }

        void ShowVipPrivilegeTab(bool bShow)
        {
            if (FuncTab == null || FuncTab.Length < (int)VipTabType.COUNT)
            {
                return;
            }
            Toggle vipTab = FuncTab[(int)VipTabType.VIP];
            if (vipTab)
            {
                vipTab.isOn = !bShow;
                vipTab.gameObject.CustomActive(bShow);
            }
        }

        //add for ios appstore
        private void TryChangeVipTabTitleForIOS()
        {
            //is ios appstore check !!!!!!!!!!!!!!!!!
            if (mVipFrameBtnImg)
            {
                mVipFrameBtnImg.gameObject.CustomActive(false);
            }
            if (mChecktext)
            {
                mChecktext.CustomActive(true);
            }
            if (mTab3Text)
            {
                mTab3Text.CustomActive(false);
            }
        }

        #region UIEvent Callback

        void OnPaySuccess(UIEvent iEvent)
        {
            UpdateShowUpLevelData();
        }

        void OnCounterChanged(UIEvent iEvent)
        {
            //UpdateVipGiftInfo();
            //UpdateGiftBuyButtonState();
        }

        void OnReceivePayResult(UIEvent uiEvent)
        {
            if (mCurTabIndex == VipTabType.PAY_RETRN)
                return;
            SetPayReturnItem();
            RefreshPayReturn();
        }

        void OnUpdatePayData(UIEvent iEvent)
        {
            RefreshPayReturn();
        }

        void OnSwitchPayReturnTab(UIEvent iEvent)
        {
            SwitchPage(VipTabType.PAY_RETRN);
        }

        void OnPrivilegeFrameOpen(UIEvent iEvent)
        {

        }

        void OnPrivilegeFrameClose(UIEvent iEvent)
        {
            if (mPreTabIndex == VipTabType.COUNT || mPreTabIndex == VipTabType.VIP)
            {
                SwitchPage(VipTabType.PAY);
            }
            else
            {
                SwitchPage(mPreTabIndex);
            }
        }

        #endregion


        #region Public Methods

        public void OpenPayTab()
        {
            SwitchPage(VipTabType.PAY);
        }

        #endregion


        #region Bind Reflect Event Callback

        [MessageHandle(WorldBillingGoodsRes.MsgID)]
        void OnReceivePayItem(MsgDATA msg)
        {
            WorldBillingGoodsRes msgData = new WorldBillingGoodsRes();
            msgData.decode(msg.bytes);
            PayItemData[] datas = new PayItemData[msgData.goods.Length];
            ChargeGoods originItem = msgData.goods[0];
            PayItemData data = new PayItemData(originItem);
            if (mMonthCard)
            {
                mMonthCard.SetView(data);
            }
        }

        [MessageHandle(SceneVipBuyItemRes.MsgID)]
        void OnPrivilegeGiftBuySuccessRes(MsgDATA msg)
        {
            SceneVipBuyItemRes res = new SceneVipBuyItemRes();
            res.decode(msg.bytes);

            if (res.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            SystemNotifyManager.SystemNotify(1201);
        }

        #endregion


        [UIControl("middle/titleback/Tab{0}", typeof(Toggle), 1)]
        protected Toggle[] FuncTab = new Toggle[3];

        [UIObject("middle/pay")]
        GameObject goPay;

        [UIObject("middle/pay/Viewport/Content")]
        GameObject goPayContent;

        #region ExtraUIBind
        GameObject[] goControlTabs = new GameObject[(int)VipTabType.COUNT];

        private GameObject mTabPay = null;
        private GameObject mTabPayReturn = null;
        private GameObject mTabVip = null;
        private Text mTxtCurPay = null;
        private Text mTxtNextpay = null;

        private Image mCurVipLv = null;
        private Image mCurVipLvSec = null;
        private Text mRechargeMoneyNum = null;
        private GameObject mTargetLvRootObj = null;
        private GameObject mObjTestText = null;
        private ComExpBar mCostExp = null;

        private GameObject mPayReturnContent = null;
        private Button mPayReturnLeftBtn = null;
        private Button mPayReturnRightBtn = null;
        private GameObject mEffectRoot_LeftBtn = null;
        private GameObject mEffectRoot_RightBtn = null;
        private Text mVipMaxText = null;
        private Button mVipFrameBtn = null;
        private GameObject mChecktext = null;
        private GameObject mVipFrameBtnImg = null;
        private Button mBtClose = null;
		private GameObject mVipHead2Target = null;
        private UIGray mVipHead2TargetGray = null;
        private Image mVipLevel1Target = null;
        private Image mVipLevel2Target = null;
        //private Text mHasCostRMBNum = null;
        private GameObject mPayPrivilegeContent = null;
        private Text mIOSTips = null;
        private Text mTab3Text = null;
        private ComMonthCard mMonthCard = null;
        private GameObject mPayItemRoot = null;

        protected override void _bindExUI()
        {
            mTabPay = mBind.GetGameObject("tabPay");
            mTabPayReturn = mBind.GetGameObject("tabPayReturn");
            mTabVip = mBind.GetGameObject("tabVip");
            mTxtCurPay = mBind.GetCom<Text>("txtCurPay");
            mTxtNextpay = mBind.GetCom<Text>("txtNextpay");

            mCurVipLv = mBind.GetCom<Image>("CurVipLv");
            mCurVipLvSec = mBind.GetCom<Image>("CurVipLvSec");
            mRechargeMoneyNum = mBind.GetCom<Text>("RechargeMoneyNum");
            mTargetLvRootObj = mBind.GetGameObject("TargetLvRootObj");
            mObjTestText = mBind.GetGameObject("objTestText");
            mCostExp = mBind.GetCom<ComExpBar>("costExp");

            mPayReturnContent = mBind.GetGameObject("payReturnContent");
            mPayReturnLeftBtn = mBind.GetCom<Button>("payReturnLeftBtn");
            if (null != mPayReturnLeftBtn)
            {
                mPayReturnLeftBtn.onClick.AddListener(_onPayReturnLeftBtnButtonClick);
            }
            mPayReturnRightBtn = mBind.GetCom<Button>("payReturnRightBtn");
            if (null != mPayReturnRightBtn)
            {
                mPayReturnRightBtn.onClick.AddListener(_onPayReturnRightBtnButtonClick);
            }
            mEffectRoot_LeftBtn = mBind.GetGameObject("EffectRoot_LeftBtn");
            mEffectRoot_RightBtn = mBind.GetGameObject("EffectRoot_RightBtn");
            mVipMaxText = mBind.GetCom<Text>("vipMaxText");
            mVipFrameBtn = mBind.GetCom<Button>("VipFrameBtn");
            if (null != mVipFrameBtn)
            {
                mVipFrameBtn.onClick.AddListener(_onVipFrameBtnButtonClick);
            }
            mChecktext = mBind.GetGameObject("checktext");
            mVipFrameBtnImg = mBind.GetGameObject("VipFrameBtnImg");
            mBtClose = mBind.GetCom<Button>("btClose");
            if (null != mBtClose)
            {
                mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            }
			mVipHead2Target = mBind.GetGameObject("vipHead2Target");
            mVipHead2TargetGray = mBind.GetCom<UIGray>("vipHead2TargetGray");
            mVipLevel1Target = mBind.GetCom<Image>("vipLevel1Target");
            mVipLevel2Target = mBind.GetCom<Image>("vipLevel2Target");
            //mHasCostRMBNum = mBind.GetCom<Text>("hasCostRMBNum");
            mPayPrivilegeContent = mBind.GetGameObject("PayPrivilegeContent");
            mIOSTips = mBind.GetCom<Text>("IOSTips");
            mTab3Text = mBind.GetCom<Text>("Tab3Text");
            mMonthCard = mBind.GetCom<ComMonthCard>("MonthCard");
            mPayItemRoot = mBind.GetGameObject("PayItemRoot");

            if (mIOSTips)
            {
                mIOSTips.CustomActive(false);
            }
            if (mMonthCard)
            {
                mMonthCard.gameObject.CustomActive(true);
            }
            if (mPayItemRoot)
            {
                mPayItemRoot.CustomActive(true);
            }

            goControlTabs[0] = mTabPay;
            goControlTabs[1] = mTabPayReturn;
            goControlTabs[2] = mTabVip;

#if MG_TEST || MG_TEST2
            if (mMonthCard)
            {
                mMonthCard.gameObject.CustomActive(false);
            }

            if (mPayItemRoot)
            {
                mPayItemRoot.CustomActive(false);
            }

            if (mObjTestText != null)
                mObjTestText.CustomActive(true);
#endif
#if ( UNITY_IPHONE || UNITY_IOS ) && APPLE_STORE
            if (mIOSTips)
            {
                mIOSTips.CustomActive(true);
            }
#endif
        }

        protected override void _unbindExUI()
        {
            mTabPay = null;
            mTabPayReturn = null;
            mTabVip = null;
            mTxtCurPay = null;
            mTxtNextpay = null;

            mCurVipLv = null;
            mCurVipLvSec = null;
            mRechargeMoneyNum = null;
            mTargetLvRootObj = null;
            mObjTestText = null;
            mCostExp = null;

            mPayReturnContent = null;
            if (null != mPayReturnLeftBtn)
            {
                mPayReturnLeftBtn.onClick.RemoveListener(_onPayReturnLeftBtnButtonClick);
            }
            mPayReturnLeftBtn = null;
            if (null != mPayReturnRightBtn)
            {
                mPayReturnRightBtn.onClick.RemoveListener(_onPayReturnRightBtnButtonClick);
            }
            mPayReturnRightBtn = null;
            mEffectRoot_LeftBtn = null;
            mEffectRoot_RightBtn = null;
            mVipMaxText = null;
            if (null != mVipFrameBtn)
            {
                mVipFrameBtn.onClick.RemoveListener(_onVipFrameBtnButtonClick);
            }
            mVipFrameBtn = null;
            mChecktext = null;
            mVipFrameBtnImg = null;
            mBtClose = null;
			mVipHead2Target = null;
            mVipHead2TargetGray = null;
            mVipLevel1Target = null;
            mVipLevel2Target = null;
            //mHasCostRMBNum = null;
            mPayPrivilegeContent = null;
            mIOSTips = null;
            mTab3Text = null;
            mMonthCard = null;
            mPayItemRoot = null;
        }
        #endregion

        #region Callback


        private void _onPayReturnLeftBtnButtonClick()
        {
            if (mPayReturnScrollState == PayReturnSrcollState.Head_Left_Most)
            {
                return;
            }
            mPayReturnScrollState = PayReturnSrcollState.Middle_Left;
            SetPayReturnItem(true);
        }
        private void _onPayReturnRightBtnButtonClick()
        {
            if (mPayReturnScrollState == PayReturnSrcollState.Tail_Right_Most)
            {
                return;
            }
            mPayReturnScrollState = PayReturnSrcollState.Middle_Right;
            SetPayReturnItem(true);
        }

        private void _onVipFrameBtnButtonClick()
        {
            /* put your code in here */
            //if (ClientSystemManager.GetInstance().IsFrameOpen<PayPrivilegeFrame>())
            //{
            //    ClientSystemManager.GetInstance().CloseFrame<PayPrivilegeFrame>();
            //}
            //ClientSystemManager.GetInstance().OpenFrame<PayPrivilegeFrame>(FrameLayer.Middle, payPrivilegeFrameData);
        }

        private void _onBtCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }

        #endregion
    }
}