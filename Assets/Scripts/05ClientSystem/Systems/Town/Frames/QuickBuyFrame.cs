using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine.EventSystems;

namespace GameClient
{
    public class QuickBuyFrame : ClientFrame
    {
        public enum eState
        {
            None,
            Success,
            Fail,
            Cancel,
        } 

        public enum eType
        {
            None,
            Gold,
            Point,
        }

        public eState state
        {
            private set; get;
        }

        public enum eQuickBuyType
        {
            Default,
            FullScreen,
            BuffDrug,
        }

        public static string _getFrameName(eQuickBuyType type)
        {
            switch (type)
            {
                case eQuickBuyType.BuffDrug:
                    return "QuickBuyFrame_BuffDrug";
                case eQuickBuyType.FullScreen:
                    return "QuickBuyFrame_FullScreen";
            }

            return "QuickBuyFrame_Defaulte";
        }

        public static QuickBuyFrame Open(eQuickBuyType type)
        {
            switch (type)
            {
                case eQuickBuyType.BuffDrug:
                    sPrefabPath = "UIFlatten/Prefabs/Common/ChapterPostionQuickBuy";
                    break;
                case eQuickBuyType.FullScreen:
                    sPrefabPath = "UIFlatten/Prefabs/Common/CommonQuickBuy";
                    break;
            }
            mQuickBuyType = type;

            return ClientSystemManager.instance.OpenFrame<QuickBuyFrame>(FrameLayer.High, null, _getFrameName(type)) as QuickBuyFrame;
        }

        public static bool IsOpen(eQuickBuyType type)
        {
            return ClientSystemManager.instance.IsFrameOpen(_getFrameName(type));
        }

        private static string sPrefabPath = "UIFlatten/Prefabs/Common/CommonQuickBuy";

#region ExtraUIBind
        private Text mTypename = null;
        private Text mCoincount = null;
        private Text mCointype = null;
        private Button mCancel = null;
        private Button mOk = null;
        private Image mIcon = null;
        private Text mBuycount = null;
        private GameObject mScoreMallRoot = null;
        private Text mScoreMallDesc = null;

        protected override void _bindExUI()
        {
            mTypename = mBind.GetCom<Text>("typename");
            mCoincount = mBind.GetCom<Text>("coincount");
            mCointype = mBind.GetCom<Text>("cointype");
            mCancel = mBind.GetCom<Button>("cancel");
            mCancel.onClick.AddListener(_onCancelButtonClick);
            mOk = mBind.GetCom<Button>("ok");
            mOk.onClick.AddListener(_onOkButtonClick);
            mIcon = mBind.GetCom<Image>("icon");
            mBuycount = mBind.GetCom<Text>("buycount");
            mScoreMallRoot = mBind.GetGameObject("scoreMallRoot");
            mScoreMallDesc = mBind.GetCom<Text>("scoreMallDesc");
        }

        protected override void _unbindExUI()
        {
            mTypename = null;
            mCoincount = null;
            mCointype = null;
            mCancel.onClick.RemoveListener(_onCancelButtonClick);
            mCancel = null;
            mOk.onClick.RemoveListener(_onOkButtonClick);
            mOk = null;
            mIcon = null;
            mBuycount = null;
            mScoreMallRoot = null;
            mScoreMallDesc = null;
        }
#endregion      

#region Callback
        private void _onCancelButtonClick()
        {
            /* put your code in here */
            _onCancel();

        }
        private void _onOkButtonClick()
        {
            /* put your code in here */
            _onOK();
        }
#endregion

        private eType mType        = eType.None;
        private int   mCount       = 0;
        private QuickBuyTable mQuicktable = null;

        int multiple = 0;//积分倍数
        int endTime = 0;//积分结束时间
        bool isTimer = false;
        
        byte mRebornSeat = byte.MaxValue;
        static eQuickBuyType mQuickBuyType = eQuickBuyType.Default;
        Coroutine mRebornCoroutine = null;

        public override string GetPrefabPath()
        {
            return sPrefabPath;
        }

        protected override void _OnOpenFrame()
        {
            state = eState.None;
            if(mQuickBuyType == eQuickBuyType.FullScreen)
            {
                _RegisterUIEvent();
            }
        }

        protected override void _OnCloseFrame()
        {
            if (mQuickBuyType == eQuickBuyType.FullScreen)
            {
                _UnRegisterUIEvent();
            }
            mRebornCoroutine = null;//协同程序要释放
        }
        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onBattlePlayerRebornSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonRebornFail, _onBattlePlayerRebornFail);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonRebornSuccess, _onBattlePlayerRebornSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonRebornFail, _onBattlePlayerRebornFail);
        }

        public eType GetMoneyType()
        {
            return mType;
        }

        public void SetRebornPlayerSeat(byte seat)
        {
            mRebornSeat = seat;
        }

        public void SetRebornCoroutine(Coroutine c)
        {
            mRebornCoroutine = c;
        }

        public void SetQuickBuyItem(int id, int buyCount)
        {
            mType = eType.None;

            ItemTable originItemData = TableManager.instance.GetTableItem<ItemTable>(id);
            if (null != originItemData)
            {
                mTypename.text = originItemData.Name;
            }

            mQuicktable = TableManager.instance.GetTableItem<QuickBuyTable>(id);

            int costItemID = mQuicktable.CostItemID;
            if (null != mQuicktable)
            {
                mCount = mQuicktable.CostNum * buyCount;
            }

            ItemTable needItemData = TableManager.instance.GetTableItem<ItemTable>(costItemID);
            if (needItemData != null)
            {
                // mIcon.sprite = AssetLoader.instance.LoadRes(needItemData.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mIcon, needItemData.Icon);
            }

            mCoincount.text = string.Format("{0}", mCount);

            if (null != mBuycount)
            {
                mBuycount.text = buyCount.ToString();
            }

            ItemTable itemtable = TableManager.instance.GetTableItem<ItemTable>(costItemID);
            if (null != itemtable)
            {
                switch (itemtable.SubType)
                {
                    case ItemTable.eSubType.POINT:
                    case ItemTable.eSubType.BindPOINT:
                        {
                            mType = eType.Point;
                            mCointype.text = "点券";
                        }
                        break;
                    case ItemTable.eSubType.GOLD:
                    case ItemTable.eSubType.BindGOLD:
                        {
                            mType = eType.Gold;
                            mCointype.text = "金币";
                        }
                        break;
                }
            }

            multiple = mQuicktable.multiple;
           
            MallItemMultipleIntegralData data = MallNewDataManager.GetInstance().CheckMallItemMultipleIntegral(mQuicktable.ID);
            if (data != null)
            {
                multiple += data.multiple;
                endTime = data.endTime;
            }

            if (endTime > 0)
            {
                isTimer = (endTime - TimeManager.GetInstance().GetServerTime()) > 0;
            }

            if (mScoreMallRoot != null)
            {
                mScoreMallRoot.CustomActive(multiple > 0);
            }

            if (mScoreMallDesc != null)
            {
                if (multiple > 0)
                {
                    int price = MallNewUtility.GetTicketConvertIntergalNumnber(mCount) * multiple;
                    string mContent = string.Empty;
                    if (multiple <= 1)
                    {
                        mContent = TR.Value("mall_fast_buy_intergral_single_multiple_desc", price);
                    }
                    else
                    {
                        mContent = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple, string.Empty);
                    }

                    if (isTimer == true)
                    {
                        mContent = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple, TR.Value("mall_fast_buy_intergral_many_multiple_remain_time_desc", Function.SetShowTimeDay(endTime)));
                    }

                    mScoreMallDesc.text = mContent;
                }
            }
        }

        private void _onOK()
        {
            bool canUse = false;

            switch (mType)
            {
                case eType.Gold:
                    canUse = PlayerBaseData.GetInstance().CanUseGold((ulong)mCount);
                    break;
                case eType.Point:
                    canUse = PlayerBaseData.GetInstance().CanUseTicket((ulong)mCount);
                    break;
            }

            if (mType == eType.Point && SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            if (canUse)
            {
                if (mType == eType.Point)
                {
                    //快速购买判断商城积分（是否等于上限值、是否超出上限值）
                    if (multiple > 0)
                    {
                        string content = string.Empty;
                        //积分商城积分等于上限值
                        if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper() &&
                             MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                        {
                            content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");

                            MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, SetState);
                        }
                        else
                        {
                            int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(mCount) * multiple;

                            int allIntergralScore = (int)PlayerBaseData.GetInstance().IntergralMallTicket + ticketConvertScoreNumber;

                            //购买道具后商城积分超出上限值
                            if (allIntergralScore > MallNewUtility.GetIntergralMallTicketUpper() &&
                                (int)PlayerBaseData.GetInstance().IntergralMallTicket != MallNewUtility.GetIntergralMallTicketUpper() &&
                                MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed == false)
                            {
                                content = TR.Value("mall_buy_intergral_mall_score_exceed_upper_value_desc",
                                                   (int)PlayerBaseData.GetInstance().IntergralMallTicket,
                                                   MallNewUtility.GetIntergralMallTicketUpper(),
                                                   MallNewUtility.GetIntergralMallTicketUpper() - (int)PlayerBaseData.GetInstance().IntergralMallTicket);

                                MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, SetState);
                            }
                            else
                            {//未超出
                                SetState();
                            }
                        }
                    }
                    else
                    {
                        SetState();
                    }
                }
                else
                {
                    SetState();
                }
            }
            else 
            {
                state = eState.Fail;

                switch (mType)
                {
                    case eType.Gold:
                        SystemNotifyManager.SystemNotify(1000014);
                        break;
                    case eType.Point:
                        SystemNotifyManager.SystemNotify(1000027);
                        break;
                }
            }

            frameMgr.CloseFrame(this);
        }

        private void _onCancel()
        {
            state = eState.Cancel;
            frameMgr.CloseFrame(this);
        }

        void SetState()
        {
            state = eState.Success;
        }

        void _onBattlePlayerRebornSuccess(UIEvent ui)
        {
            _stopRebornCoroutine();

            byte target = (byte)ui.Param1;

            if (target == mRebornSeat) 
            {
                ClientSystemManager.instance.CloseFrame(this);
            }
        }

        void _onBattlePlayerRebornFail(UIEvent ui)
        {
            _stopRebornCoroutine();
        }

        void _stopRebornCoroutine()
        {
            if(mRebornCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(mRebornCoroutine);
                mRebornCoroutine = null;
            }
        }
    }
}
