using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;

namespace GameClient
{
    public class PayPrivilegeFrameData
    {
        int mCurShowVipLevel = 0;
        int mPrivilegeNum = 0;
        int mMaxVipLevel = 0;

        public int CurShowVipLevel { get { return mCurShowVipLevel; } set { mCurShowVipLevel = value; } }
        public int PrivilegeNum { get { return mPrivilegeNum; } set { mPrivilegeNum = value; } }
        public int MaxVipLevel { get { return mMaxVipLevel; } set { mMaxVipLevel = value; } }

        public void ClearData()
        {
        }
    }
    
    public class VipDescData
    {
        public string desc;
        public string icon;
        public bool bSpecialDisplay = false;
        public int index = 0;
    }

    public class PayPrivilegeFrame //: ClientFrame
    {
        protected const string EffUI_shouchong_guizu_Path = "Effects/UI/Prefab/EffUI_shouchong/Prefab/EffUI_shouchong_guizu";
        protected const string EffUI_shouchong_jiantou01_Path = "Effects/UI/Prefab/EffUI_shouchong/Prefab/EffUI_shouchong_jiantou01";

        const string UNUSED_VIP_GIFT_ICON_BG = "UI/Image/Packed/p_UI_Vip.png:UI_Vip_Meiyoujiangli";
        const string RES_PAY_PRIVILEGE_VIEW_PATH = "UIFlatten/Prefabs/Vip/PayPrivilegeView";

        PayPrivilegeFrameData frameData = null;

        GameObject root = null;
        GameObject parent = null;
        ClientFrame THIS = null;
        ComCommonBind mBind = null;

        GameObject effect_left_jiantou_go = null;
        GameObject effect_right_jiantou_go = null;
        GameObject effect_guizu_go = null;

        //DATA
        List<VipDescData> mVipDescDataList = new List<VipDescData>();
        List<ItemData> mVipGiftItemList = new List<ItemData>();
        List<int> mVipGiftIdList = new List<int>();
        //VIEW
        List<PayRewardItem> mVipPrivilegeItems = new List<PayRewardItem>();
        List<PayRewardItem> mVipGiftItems = new List<PayRewardItem>();

        enum VipSrcollState
        {
            Head_Left_Most,
            Middle_Left,
            Middle_Right,
            Tail_Right_Most,
        }

        VipSrcollState mVipScrollState = VipSrcollState.Middle_Right;
        int mVipScorllIndex = -1; //手动刷新时用到的vip等级

        //public override string GetPrefabPath()
        //{
        //    return "UIFlatten/Prefabs/Vip/PayPrivilegeFrame";
        //}
        //protected override void _OnOpenFrame()
        //{
        //    if (userData != null)
        //    {
        //        frameData = userData as PayPrivilegeFrameData;
        //    }
        //    else
        //    {
        //        frameData = new PayPrivilegeFrameData();
        //        _InitSelfData();
        //    }
        //    Show();
        //}

        //protected override void _OnCloseFrame()
        //{
        //    Hide();
        //}

        public PayPrivilegeFrame(PayPrivilegeFrameData data, GameObject parent, ClientFrame frame)
        {
            if (data != null)
            {
                frameData = data;
            }
            else
            {
                frameData = new PayPrivilegeFrameData();
                _InitSelfData();
            }

            this.parent = parent;
            this.THIS = frame;

            if (root == null)
            {
                root = AssetLoader.instance.LoadResAsGameObject(RES_PAY_PRIVILEGE_VIEW_PATH);
                if (root != null)
                {
                    Utility.AttachTo(root, parent);
                    mBind = root.GetComponent<ComCommonBind>();
                    _bindExUI();
                }
            }

            BindEvent();
        }

        public void UpdateView(PayPrivilegeFrameData data = null)
        {
            if (data != null)
            {
                frameData = data;
            }

            InitData();
            InitEffectRoot();
            //UpdateVipLevelView();
            UpdateVipPrivilege();

            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VipPrivilegeFrameOpen);
        }

        public void DestroyView()
        {
            CloseView();

            _unbindExUI();
            UnBindEvent();

            if (frameData != null)
            {
                frameData.ClearData();
            }
            frameData = null;

            parent = null;
            THIS = null;
            root = null;
            mBind = null;
        }

        public void CloseView()
        {
            Reset();
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VipPrivilegeFrameClose);
        }

        void InitEffectRoot()
        {
            if (effect_guizu_go == null)
            {
                effect_guizu_go = AssetLoader.GetInstance().LoadResAsGameObject(EffUI_shouchong_guizu_Path);
                Utility.AttachTo(effect_guizu_go, mEffectRoot_Envior);
            }
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
            if (effect_guizu_go)
            {
                GameObject.Destroy(effect_guizu_go);
                effect_guizu_go = null;
            }
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

        void Reset()
        {
            mVipScrollState = VipSrcollState.Middle_Right;
            mVipScorllIndex = -1;

            if (mVipDescDataList != null)
            {
                mVipDescDataList.Clear();
            }

            if (mVipGiftIdList != null)
            {
                mVipGiftIdList.Clear();
            }

            if (mVipGiftItemList != null)
            {
                mVipGiftItemList.Clear();
            }

            if (mVipPrivilegeItems != null)
            {
                //Logger.LogErrorFormat("mVipPrivilegeItems count is {0}", mVipPrivilegeItems.Count);
                for (int i = 0; i < mVipPrivilegeItems.Count; i++)
                {
                    mVipPrivilegeItems[i].Clear();
                }

                mVipPrivilegeItems.Clear();
            }
            if (mVipGiftItems != null)
            {
                //Logger.LogErrorFormat("mVipGiftItems count is {0}", mVipGiftItems.Count);
                for (int i = 0; i < mVipGiftItems.Count; i++)
                {
                    mVipGiftItems[i].Clear();
                }
                mVipGiftItems.Clear();
            }

            ClearEffectRoot();
        }

        void BindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, OnCounterChanged);   
        }
        void UnBindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, OnCounterChanged);       
        }

        void InitData()
        {
            if (frameData == null)
            {
                Logger.LogErrorFormat("{0} frame data is null !","PayPrivilegeFrame");
                return;
            }

            UpdateVipGiftInfo();

            if (mVipGiftIdList != null)
            {
                for (int i = 0; i < mVipGiftIdList.Count; i++)
                {
                    if (mVipGiftIdList[i] == frameData.CurShowVipLevel)
                    {
                        if (mNowRMBBtn)
                        {
                            mNowRMBBtn.interactable = false;
                        }
                        if (mNowRMBBtnGray)
                        {
                            mNowRMBBtnGray.enabled = true;
                        }
                        break;
                    }
                }
            }
        }

        void UpdateVipGiftInfo()
        {
            if (mVipGiftIdList == null)
            {
                return;
            }

            CounterInfo info = CountDataManager.GetInstance().GetCountInfo("vip_gift_buy_bit");

            if (info != null)
            {
                mVipGiftIdList.Clear();

                BitArray bitArray = new BitArray(BitConverter.GetBytes((int)info.value));

                for (int i = 0; i < bitArray.Length; i++)
                {
                    bool bRecvd = bitArray[i];
                    if (bRecvd)
                    {
                        mVipGiftIdList.Add(i);
                    }
                }
            }
        }

        void UpdateVipLevelView()
        {
            if (frameData == null)
            {
                Logger.LogError("Pay Privilege frame data is null");
                return;
            }
            if (frameData.MaxVipLevel <= 0)
            {
                var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);
                if (SystemValueTableData != null)
                {
                    frameData.MaxVipLevel = SystemValueTableData.Value;
                }
            }
            if (frameData.CurShowVipLevel < 0)
            {
                frameData.CurShowVipLevel = PlayerBaseData.GetInstance().VipLevel;
            }

            if (frameData.CurShowVipLevel >= frameData.MaxVipLevel)
            {
                if (mTargetLvRootObj)
                {
                    mTargetLvRootObj.CustomActive(false);
                }
                if (mVipMaxText)
                {
                    mVipMaxText.gameObject.CustomActive(true);
                }
            }

            SetVipZeroShow(frameData.CurShowVipLevel == 0);

            if (mRechargeMoneyNum)
            {
                 string desc_buy_rmb = TR.Value("vip_month_card_first_buy_cost_desc"); //{0}元
                 var VipData = TableManager.GetInstance().GetTableItem<VipTable>(PlayerBaseData.GetInstance().VipLevel);
                 if (VipData != null)
                 {
                     mRechargeMoneyNum.text = string.Format(desc_buy_rmb, VipData.TotalRmb - PlayerBaseData.GetInstance().CurVipLvRmb);//"{0}元"
                 }
            }

            if (mCurVipLvSingle)
            {
                mCurVipLvSingle.gameObject.CustomActive(frameData.CurShowVipLevel < 10);
            }
            if (mCurVipLvSec)
            {
                mCurVipLvSec.gameObject.CustomActive(frameData.CurShowVipLevel >= 10);
            }
            if (mCurVipLv)
            {
                mCurVipLv.gameObject.CustomActive(frameData.CurShowVipLevel >= 10);
            }

            if (mCurVipLvSingle && mCurVipLv)
            {
                if (frameData.CurShowVipLevel < 10)
                {
                    ETCImageLoader.LoadSprite(ref mCurVipLvSingle, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, frameData.CurShowVipLevel));
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref mCurVipLvSec, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, frameData.CurShowVipLevel / 10));
                    ETCImageLoader.LoadSprite(ref mCurVipLv, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, frameData.CurShowVipLevel % 10));
                }
            }

            int _nextVipLevel = frameData.CurShowVipLevel + 1;

            if (mCurVipLvSec2)
            {
                mCurVipLvSec2.gameObject.CustomActive(true);
            }
            if (mCurVipLv2)
            {
                mCurVipLv2.gameObject.CustomActive(_nextVipLevel >= 10);
            }

            if (mCurVipLvSec2 && mCurVipLv2)
            {
                if (_nextVipLevel < 10)
                {
                    ETCImageLoader.LoadSprite(ref mCurVipLvSec2, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, _nextVipLevel));
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref mCurVipLvSec2, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, _nextVipLevel / 10));
                    ETCImageLoader.LoadSprite(ref mCurVipLv2, string.Format(MallPayFrame.ICON_VIP_RES_PATH_FORMAT, _nextVipLevel % 10));
                }
            }

            DrawVipLevelExpBar(PlayerBaseData.GetInstance().VipLevel, (ulong)PlayerBaseData.GetInstance().CurVipLvRmb, true);
        }

        void DrawVipLevelExpBar(int iVipLevel, System.UInt64 CurVipLvExp, bool force = false)
        {
            if (mCostExp)
            {
                mCostExp.SetExp(CurVipLvExp, force, exp =>
                {
                    return TableManager.instance.GetCurVipLevelExp(iVipLevel, exp);
                });
            }
        }

        void UpdateVipPrivilege(bool bForceUpdate=false)
        {
            if (frameData == null)
            {
                Logger.LogError("Pay Privilege frame data is null");
                return;
            }
            int totalVipLevelCount = frameData.MaxVipLevel;

            if (bForceUpdate)
            {
                int fIndex = -1;
                switch (mVipScrollState)
                {
                    case VipSrcollState.Head_Left_Most:
                    case VipSrcollState.Tail_Right_Most:
                        fIndex = mVipScorllIndex;
                        break;
                    case VipSrcollState.Middle_Left:
                        fIndex = mVipScorllIndex - 1;
                        if (fIndex < 0)
                        {
                            fIndex = 0;
                        }
                        break;
                    case VipSrcollState.Middle_Right:
                        fIndex = mVipScorllIndex + 1;
                        if (fIndex > totalVipLevelCount + 1)
                        {
                            fIndex = totalVipLevelCount + 1;
                        }
                        break;
                }
                mVipScorllIndex = fIndex;
            }
            else
            {
                mVipScorllIndex = frameData.CurShowVipLevel;
            }

            if (mVipLevel)
            {
                mVipLevel.text = string.Format(TR.Value("vip_month_card_first_buy_privilege_format"), mVipScorllIndex);
            }
            if (mVipLevel2)
            {
                mVipLevel2.text = string.Format(TR.Value("vip_month_card_first_buy_privilege_format"), mVipScorllIndex);
            }

            SetVipZeroShow( mVipScorllIndex == 0 );

            mVipDescDataList = PayManager.GetInstance().GetPrivilegeDescDataListByVipLevel(mVipScorllIndex);

            if (mVipDescDataList != null)
            {
                mVipDescDataList.Sort((x, y) => x.index.CompareTo(y.index));

                RefreshVipDescView( mVipScorllIndex == 0 );
            }

            VipTable VipData = TableManager.GetInstance().GetTableItem<VipTable>(mVipScorllIndex);
            if (VipData == null)
            {
                Logger.LogErrorFormat("VipTable hasn't [vip level = {0}] data!", mVipScorllIndex);
                return;
            }

            if (VipData.GiftItems == null)
            {
                return;
            }
            bool bBuyBtnShow = true;
            if (VipData.GiftItems.Count > 0)
            {
                if (VipData.GiftItems[0] == "-" || VipData.GiftItems[0] == "")
                {
                    bBuyBtnShow = false;
                }

                if (mVipGiftItemList != null)
                {
                    mVipGiftItemList.Clear();
                }
                for (int i = 0; i < VipData.GiftItems.Count; i++)
                {
                    string[] itemShowdata = VipData.GiftItems[i].Split(new char[] { '_' });
                    if (itemShowdata.Length < 2)
                    {
                        continue;
                    }
                    int giftId = -1;
                    int giftCount = 0;
                    if (int.TryParse(itemShowdata[0], out giftId))
                    {
                        ItemData data = ItemDataManager.CreateItemDataFromTable(giftId);
                        if (data == null)
                        {
                            continue;
                        }
                        if (int.TryParse(itemShowdata[1], out giftCount))
                        {
                            data.Count = giftCount;
                        }

                        if (mVipGiftItemList != null)
                        {
                            mVipGiftItemList.Add(data);
                        }
                    }
                }

                RefreshVipGiftView();

            }else
            {
                 bBuyBtnShow = false;
            }
            if (mNowRMBBtn)
            {
                mNowRMBBtn.gameObject.CustomActive(bBuyBtnShow);
            }

            if (mOriginalRMB)
            {
                mOriginalRMB.text = VipData.GiftPrePrice.ToString();
            }
            if (mNowRMB)
            {
                mNowRMB.text = VipData.GiftDiscountPrice.ToString();
            }
            UpdateGiftBuyButtonState();

            //刷新左右滚动按钮状态
            if (mVipScorllIndex == 0)
            {
                mVipScrollState = VipSrcollState.Head_Left_Most;
                SetPayReturnLeftBtnActive(false);
                SetPayReturnRightBtnActive(true);
            }
            else if (mVipScorllIndex == totalVipLevelCount)
            {
                mVipScrollState = VipSrcollState.Tail_Right_Most;
                SetPayReturnLeftBtnActive(true);
                SetPayReturnRightBtnActive(false);
            }
            else
            {
                SetPayReturnLeftBtnActive(true);
                SetPayReturnRightBtnActive(true);
            }
            if(mPreviewBtn!=null)
            {
                mPreviewBtn.CustomActive(mVipScorllIndex == 2);//VIP等级是2的时候显示预览按钮，否则不显示
            }
        }

        void RefreshVipDescView(bool bHide=false)
        {
            if (mTopMainView)
            {
                mTopMainView.gameObject.CustomActive(!bHide);
            }
            if (mSpecialItem)
            {
                mSpecialItem.gameObject.CustomActive(!bHide);
            }

            if (bHide)
            {
                return;
            }

            if (mVipDescDataList == null)
            {
                return;
            }

            List<VipDescData> tempVipDescDataList = new List<VipDescData>();
            VipDescData specialDescData = null;
            int countVipSpecialNum = 0;
            for (int i = 0; i < mVipDescDataList.Count; i++)
            {
                var descData = mVipDescDataList[i];
                if (descData.bSpecialDisplay)
                {
                    countVipSpecialNum++;
                    specialDescData = descData;
                    //Logger.LogErrorFormat("RefreshVipDescView countVipSpecialNum desc is {0}, index is {1}",specialDescData.desc,specialDescData.index);
                }
                else if (tempVipDescDataList != null && !tempVipDescDataList.Contains(descData))
                {
                    tempVipDescDataList.Add(descData);
                }
            }

            if (countVipSpecialNum > 1)
            {
                Logger.LogErrorFormat("RefreshVipDescView countVipSpecialNum is more than one !!! Please Check VIP Privilege Table");
                return;
            }

            if (countVipSpecialNum == 0)
            {
                Logger.LogErrorFormat("RefreshVipDescView countVipSpecialNum is zero !!! Please Check VIP Privilege Table");
                return;
            }

            if (tempVipDescDataList == null)
            {
                return;
            }

            if (mTopMainView == null)
            {
                return;
            }

            if (mTopMainView.IsInitialised() == false)
            {
                mTopMainView.Initialize();
                mTopMainView.onBindItem = (GameObject go) =>
                {
                    PayRewardItem payItem = null;
                    if (go)
                    {
                        payItem = go.GetComponent<PayRewardItem>();
                    }
                    return payItem;
                };
            }
            mTopMainView.onItemVisiable = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                int iIndex = var.m_index;
                if (iIndex >= 0 && iIndex < tempVipDescDataList.Count)
                {
                    if (tempVipDescDataList[iIndex].bSpecialDisplay)
                    {
                        return;
                    }
                    PayRewardItem payItem = var.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(THIS, null, false);
                        payItem.SetItemIcon(tempVipDescDataList[iIndex].icon);
                        payItem.SetItemName(string.Format("{0}.{1}", iIndex + 1, tempVipDescDataList[iIndex].desc));
                        if (mVipPrivilegeItems != null && !mVipPrivilegeItems.Contains(payItem))
                        {
                            mVipPrivilegeItems.Add(payItem);
                        }
                    }
                }
            };

            mTopMainView.SetElementAmount(tempVipDescDataList.Count);
            mTopMainView.ResetContentPosition();

            if (mSpecialItem != null && specialDescData != null)
            {
                mSpecialItem.Initialize(THIS, null, false);
                mSpecialItem.SetItemIcon(specialDescData.icon);
                mSpecialItem.SetItemName(specialDescData.desc);
            }
        }

        void RefreshVipGiftView()
        {
            if (mVipGiftItemList == null)
            {
                return;
            }
            if (mBottomMainView == null)
            {
                return;
            }
            if (mBottomMainView.IsInitialised() == false)
            {
                mBottomMainView.Initialize();
                mBottomMainView.onBindItem = (GameObject go) =>
                {
                    PayRewardItem payItem = null;
                    if (go)
                    {
                        payItem = go.GetComponent<PayRewardItem>();
                    }
                    return payItem;
                };
            }
            mBottomMainView.onItemVisiable = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                int iIndex = var.m_index;
                if (iIndex >= 0 && iIndex < mVipGiftItemList.Count)
                {
                    PayRewardItem payItem = var.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(THIS, mVipGiftItemList[iIndex], true, false);
                        payItem.RefreshView(true,false);
                        if (mVipGiftItems != null && !mVipGiftItems.Contains(payItem))
                        {
                            mVipGiftItems.Add(payItem);
                        }
                    }
                }
            };
            mBottomMainView.SetElementAmount(mVipGiftItemList.Count);
        }

        void UpdateGiftBuyButtonState()
        {
            bool bFind = false;

            if (mVipScorllIndex < 0)
            {
                mVipScorllIndex = frameData.CurShowVipLevel;
            }

            for (int i = 0; i < mVipGiftIdList.Count; i++)
            {
                var vipTable = TableManager.GetInstance().GetTableItem<VipTable>(mVipScorllIndex);

                if (vipTable == null || mVipGiftIdList[i] == vipTable.GiftID)
                {
                    bFind = true;
                    break;
                }
            }

            if (mNowRMBBtnGray)
            {
                mNowRMBBtnGray.enabled = bFind;
            }
            if (mNowRMBBtn)
            {
                mNowRMBBtn.interactable = !bFind;
            }
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
                //mPayReturnRightBtn.gameObject.CustomActive(bActive);
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

        bool SetVipZeroShow(bool bShow)
        {
            if (mVip_zero)
            {
                mVip_zero.CustomActive(bShow);
                return bShow;
            }
            return false;
        }

        void OnCounterChanged(UIEvent iEvent)
        {
            UpdateVipGiftInfo();
            UpdateGiftBuyButtonState();
        }

        #region Self Data For Show
        /// <summary>
        /// 初始化本地缓存数据（当传入数据为空时）
        /// </summary>
        void _InitSelfData()
        {
            if (frameData == null)
            {
                return;
            }
            Dictionary<int, object> tabledata = TableManager.GetInstance().GetTable<VipPrivilegeTable>();
            if (tabledata != null)
            {
                 frameData.PrivilegeNum = tabledata.Count;
            }
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);
            if (SystemValueTableData != null)
            {
                frameData.MaxVipLevel = SystemValueTableData.Value;
            }
            frameData.CurShowVipLevel = PlayerBaseData.GetInstance().VipLevel;
        }

        #endregion

        #region ExtraUIBind
        private Button mBtnClose = null;
		private Image mCurVipLvSec = null;
		private Image mCurVipLv = null;
		private Image mCurVipLvSingle = null;
		private GameObject mTargetLvRootObj = null;
		private Text mRechargeMoneyNum = null;
		private Image mCurVipLvSec2 = null;
		private Image mCurVipLv2 = null;
		private GameObject mVipMaxText = null;
		private ComExpBar mCostExp = null;
		private Text mVipLevel = null;
        private Text mVipLevel2 = null;
        private PayRewardItem mSpecialItem = null;
		private ComUIListScript mTopMainView = null;
		private GameObject mVip_zero = null;
		private ComUIListScript mBottomMainView = null;
		private Text mOriginalRMB = null;
		private Text mNowRMB = null;
		private Button mNowRMBBtn = null;
		private UIGray mNowRMBBtnGray = null;
		private Button mPayReturnLeftBtn = null;
		private Button mPayReturnRightBtn = null;
		private GameObject mEffectRoot_LeftBtn = null;
		private GameObject mEffectRoot_RightBtn = null;
		private GameObject mEffectRoot_Envior = null;

        private Button mPreviewBtn = null;//预览按钮
		
		private void _bindExUI()
		{
            if (mBind == null)
            {
                Logger.LogError("PayPrivilegeView ComCommonBind is null !!!");
                return;
            }

            //mBtnClose = mBind.GetCom<Button>("BtnClose");
            //if (null != mBtnClose)
            //{
            //    mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
            //}
            //mCurVipLvSec = mBind.GetCom<Image>("CurVipLvSec");
            //mCurVipLv = mBind.GetCom<Image>("CurVipLv");
            //mCurVipLvSingle = mBind.GetCom<Image>("CurVipLvSingle");
            //mTargetLvRootObj = mBind.GetGameObject("TargetLvRootObj");
            //mRechargeMoneyNum = mBind.GetCom<Text>("RechargeMoneyNum");
            //mCurVipLvSec2 = mBind.GetCom<Image>("CurVipLvSec2");
            //mCurVipLv2 = mBind.GetCom<Image>("CurVipLv2");
            //mVipMaxText = mBind.GetGameObject("vipMaxText");
            //mCostExp = mBind.GetCom<ComExpBar>("costExp");
            mVipLevel = mBind.GetCom<Text>("vipLevel");
            mVipLevel2 = mBind.GetCom<Text>("vipLevel2");
			mSpecialItem = mBind.GetCom<PayRewardItem>("specialItem");
			mTopMainView = mBind.GetCom<ComUIListScript>("topMainView");
			mVip_zero = mBind.GetGameObject("vip_zero");
			mBottomMainView = mBind.GetCom<ComUIListScript>("bottomMainView");
			mOriginalRMB = mBind.GetCom<Text>("originalRMB");
			mNowRMB = mBind.GetCom<Text>("nowRMB");
			mNowRMBBtn = mBind.GetCom<Button>("nowRMBBtn");
			if (null != mNowRMBBtn)
			{
				mNowRMBBtn.onClick.AddListener(_onNowRMBBtnButtonClick);
			}
			mNowRMBBtnGray = mBind.GetCom<UIGray>("nowRMBBtnGray");
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
			mEffectRoot_Envior = mBind.GetGameObject("EffectRoot_Envior");

             mPreviewBtn = mBind.GetCom<Button>("PreviewBtn");
            if(mPreviewBtn!=null)
            {
                mPreviewBtn.onClick.AddListener(OnPreviewBtnClick);
            }
		}
		
		private void _unbindExUI()
		{
            //if (null != mBtnClose)
            //{
            //    mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
            //}
            //mBtnClose = null;
            //mCurVipLvSec = null;
            //mCurVipLv = null;
            //mCurVipLvSingle = null;
            //mTargetLvRootObj = null;
            //mRechargeMoneyNum = null;
            //mCurVipLvSec2 = null;
            //mCurVipLv2 = null;
            //mVipMaxText = null;
            //mCostExp = null;
            mVipLevel = null;
            mVipLevel2 = null;
			mSpecialItem = null;
            if (mTopMainView != null)
            {
                mTopMainView.UnInitialize();
                mTopMainView = null;
            }
			mVip_zero = null;
            if (mBottomMainView != null)
            {
                mBottomMainView.UnInitialize();
                mBottomMainView = null;
            }
			mOriginalRMB = null;
			mNowRMB = null;
			if (null != mNowRMBBtn)
			{
				mNowRMBBtn.onClick.RemoveListener(_onNowRMBBtnButtonClick);
			}
			mNowRMBBtn = null;
			mNowRMBBtnGray = null;
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
			mEffectRoot_Envior = null;

            if (mPreviewBtn != null)
            {
                mPreviewBtn.onClick.RemoveListener(OnPreviewBtnClick);
            }
            mPreviewBtn = null;
        }
		#endregion

        #region Callback
        private void _onBtnCloseButtonClick()
        {
            /* put your code in here */
            //this.Close();
        }
        private void _onNowRMBBtnButtonClick()
        {
            /* put your code in here */
            if (PlayerBaseData.GetInstance().VipLevel < mVipScorllIndex)
            {
                SystemNotifyManager.SystemNotify(1084);
                return;
            }

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }
            string notifyCont = TR.Value("vip_month_card_first_buy_privilege_gift");
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCont, () =>
            {
                VipTable VipData = TableManager.GetInstance().GetTableItem<VipTable>(mVipScorllIndex);
                if (VipData == null)
                {
                    return;
                }

                CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
                costInfo.nCount = VipData.GiftDiscountPrice;

                CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                {
                    SceneVipBuyItemReq req = new SceneVipBuyItemReq();
                    req.vipLevel = (byte)mVipScorllIndex;

                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                });
            });
        }
        private void _onPayReturnLeftBtnButtonClick()
        {
            /* put your code in here */
            if (mVipScrollState == VipSrcollState.Head_Left_Most)
            {
                return;
            }

            mVipScrollState = VipSrcollState.Middle_Left;
            UpdateVipPrivilege(true);
        }
        private void _onPayReturnRightBtnButtonClick()
        {
            /* put your code in here */
            if (mVipScrollState == VipSrcollState.Tail_Right_Most)
            {
                return;
            }

            mVipScrollState = VipSrcollState.Middle_Right;
            UpdateVipPrivilege(true);
        }
        /// <summary>
        /// 预览按钮的点击
        /// </summary>
        private void OnPreviewBtnClick()
        {
            if(mVipGiftItemList != null&& mVipGiftItemList.Count>0)
            {
                ItemData itemData = mVipGiftItemList[0];
                if(itemData!=null)
                {
                    ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, itemData.TableID);
                }
              
            }
          
        }
        #endregion
    }
}
