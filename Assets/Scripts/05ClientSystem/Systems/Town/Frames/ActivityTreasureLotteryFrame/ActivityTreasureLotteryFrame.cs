using DataModel;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        /// <summary>
        /// 夺宝活动Frame
        /// </summary>
        public sealed class ActivityTreasureLotteryFrame : ClientFrame
        {
            bool mIsNeedUpdate = false;

            public static void OpenLinkFrame(string value)
            {
                if (ActivityTreasureLotteryDataManager.GetInstance().GetState() == ETreasureLotterState.Close)
                {
                    //提示已经结束
                    SystemNotifyManager.SystemNotify(9062);
                }
                else
                {
                    ClientSystemManager.GetInstance().OpenFrame<ActivityTreasureLotteryFrame>();
                }
            }

            public override string GetPrefabPath()
            {
                return "UIFlatten/Prefabs/ActivityTreasureLottery/ActivityTreasureLotteryFrame";
            }

            protected override void _OnOpenFrame()
            {
                base._OnOpenFrame();
                _OnUpdate(0);
                mIsNeedUpdate = true;
                BindEvents();
                if (userData != null && userData is EActivityTreasureLotteryView)
                {
                    mView.Init(ActivityTreasureLotteryDataManager.GetInstance(), mBind.GetPrefabPath("ItemPrefabPath"), OnChangeSubView, (EActivityTreasureLotteryView)userData);
                }
                else
                {
                    mView.Init(ActivityTreasureLotteryDataManager.GetInstance(), mBind.GetPrefabPath("ItemPrefabPath"), OnChangeSubView);
                }
            }

            protected override void _OnCloseFrame()
            {
                base._OnCloseFrame();
                mIsNeedUpdate = false;
                mView.Dispose();
                UnBindEvents();
            }

            public override bool IsNeedUpdate()
            {
                return mIsNeedUpdate;
            }

            protected override void _OnUpdate(float timeElapsed)
            {
                RefreshData();
            }

            void OnChangeSubView()
            {
                RefreshData(true);
            }

            void RefreshData(bool isImmediately = false)
            {
                switch (mView.CurrentSubView)
                {
                    case EActivityTreasureLotteryView.ActivityView:
                        ActivityTreasureLotteryDataManager.GetInstance().GetLotteryItemList();
                        break;
                    case EActivityTreasureLotteryView.HistoryView:
                        ActivityTreasureLotteryDataManager.GetInstance().GetHistroyItemList();
                        break;
                    case EActivityTreasureLotteryView.MyLotteryView:
                        ActivityTreasureLotteryDataManager.GetInstance().GetMyLotteryItemList();
                        break;
                }
            }

            void BindEvents()
            {
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotteryBuyResp, OnBuyResp);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotterySyncActivity, OnSyncActivity);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotterySyncMyLottery, OnSyncMyLottery);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotterySyncHistory, OnSyncHistory);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotteryShowHistory, OnShowHistory);
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TreasureLotteryShowActivity, OnShowActivity);
            }

            void UnBindEvents()
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotteryBuyResp, OnBuyResp);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotterySyncActivity, OnSyncActivity);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotterySyncMyLottery, OnSyncMyLottery);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotterySyncHistory, OnSyncHistory);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotteryShowHistory, OnShowHistory);
                UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TreasureLotteryShowActivity, OnShowActivity);
            }

            void OnBuyResp(UIEvent data)
            {
                var resp = data.Param1 as PayingGambleRes;
                var currency = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)resp.costCurrencyId);
                switch ((ProtoErrorCode)resp.retCode)
                {
                    case 0:
                        string tip = string.Format(TR.Value("activity_treasure_lottery_buy_success"), resp.investCopies, resp.costCurrencyNum, currency == null ? "" : currency.Name);
                        int id = ActivityTreasureLotteryDataManager.GetInstance().GetItemIdByLotteryId((int)resp.gambingItemId);
                        SystemNotifyManager.SysNotifyFloatingEffect(tip, CommonTipsDesc.eShowMode.SI_QUEUE, id);
                        if (mView != null)
                        {
                            //mView.UpdateData();
                            mView.MainUpdateData();
                            //mView.UpdateMainView();
                        }
                        //RefreshData(true);
                        //SystemNotifyManager.SystemNotify(9063, resp.investCopies, resp.costCurrencyNum, currency == null ? "" : currency.Name);
                        break;
                    //case ProtoErrorCode.ITEM_OFF_SALE:
                    //    SystemNotifyManager.SystemNotify(9059, resp.investCopies);
                    //    break;
                    case ProtoErrorCode.ITEM_NOT_ENOUGH_MONEY:
                        // SystemNotifyManager.SystemNotify(9060, ()=> { BuyCurrency((int)resp.costCurrencyId, resp.costCurrencyNum); }, null, currency == null ? "" : currency.Name);
                        var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
                        {
                            ContentLabel = string.Format(TR.Value("activity_treasure_lock"),currency == null ? "" : currency.Name),
                            IsShowNotify = false,
                            LeftButtonText = TR.Value("activity_treasure_lock_cancel"),
                            RightButtonText = TR.Value("activity_treasure_lock_Ok"),
                            OnRightButtonClickCallBack=()=> { BuyCurrency(); }
                        };
                        SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
                        break;
                    case ProtoErrorCode.ITEM_SOLD_OUT:
                        SystemNotifyManager.SystemNotify(9061);
                        break;
                    case ProtoErrorCode.ITEM_COPIES_NOT_ENOUGH:
                        SystemNotifyManager.SystemNotify(9074);
                        break;
                    case ProtoErrorCode.ITEM_NOT_SELL:
                        SystemNotifyManager.SystemNotify(9073);
                        break;
                    case ProtoErrorCode.ITEM_NO_REASON:
                        break;
                    case ProtoErrorCode.ITEM_GAMBLE_ALL_SOLD_OUT:
                        SystemNotifyManager.SystemNotify(9065);
                        break;
                    case ProtoErrorCode.ITEM_OFF_SALE:
                        break;
                    //case (uint)Protocol.ProtoErrorCode.ITEM_SOLD_OUT:
                    //    SystemNotifyManager.SystemNotify(9060);
                    //    break;
                    //case 2:
                    //    SystemNotifyManager.SystemNotify(9061);
                    //    break;
                }
            }

            void OnSyncActivity(UIEvent data)
            {
                mView.UpdateData();
            }

            void OnSyncMyLottery(UIEvent data)
            {
                mView.UpdateData();
            }
            void OnSyncHistory(UIEvent data)
            {
                mView.UpdateData();
            }

            void OnShowHistory(UIEvent data)
            {
                mView.ShowSubView(EActivityTreasureLotteryView.HistoryView);
            }

            void OnShowActivity(UIEvent data)
            {
                this.mView.ShowSubView(EActivityTreasureLotteryView.ActivityView);
            }

            private void BuyCurrency()
            {
                if(!ClientSystemManager.GetInstance().IsFrameOpen<VipFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.PAY);
                }
            }

            #region ExtraUIBind
            private ActivityTreasureLotteryView mView = null;

            protected override void _bindExUI()
            {
                mView = mBind.GetCom<ActivityTreasureLotteryView>("View");
                mView.OnButtonCloseCallBack = CloseFrame;
                mView.OnButtonBuyCallBack = BuyLottery;
                mView.OnButtonBuyAllCallBack = BuyAllLottery;
            }

            protected override void _unbindExUI()
            {
                mView.OnButtonCloseCallBack = null;
                mView.OnButtonBuyCallBack = null;
                mView.OnButtonBuyAllCallBack = null;
                mView = null;
            }
            #endregion

            bool CheckState(IActivityTreasureLotteryModel model)
            {
                switch (model.State)
                {
                    default:
                        return false;
                    case GambingItemStatus.GIS_OFF_SALE:
                    case GambingItemStatus.GIS_PREPARE:
                        SystemNotifyManager.SystemNotify(9073);
                        return false;
                    case GambingItemStatus.GIS_LOTTERY:
                    case GambingItemStatus.GIS_SOLD_OUE:
                        SystemNotifyManager.SystemNotify(9061);
                        return false;
                    case GambingItemStatus.GIS_SELLING:
                        return true;
                }
            }
            private void CloseFrame()
            {
                Close();
            }
            private void BuyLottery()
            {
                BuyLottery(false, this.mView.BuyCount);
            }

            private void BuyAllLottery()
            {
                BuyLottery(true, this.mView.LeftCount);
            }

            private void BuyLottery(bool isBuyAll, uint count)
            {
                if (mView.SelectId < 0)
                {
                    return;
                }

                var model = ActivityTreasureLotteryDataManager.GetInstance().GetModel<IActivityTreasureLotteryModel>(mView.SelectId);
                if (count <= 0)
                {
                    if (model.LeftNum >= 1)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation("请选择购买数量");
                    }
                }

                if (!CheckState(model))
                {
                    return;
                }

                if (isBuyAll)
                {
                    ActivityTreasureLotteryDataManager.GetInstance().BuyLottery(mView.SelectId, count, true);
                }
                else
                {
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    costInfo.nMoneyID = model.MoneyId;
                    costInfo.nCount = (int)count;

                    CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                    {
                        ActivityTreasureLotteryDataManager.GetInstance().BuyLottery(mView.SelectId, count, false);
                    });
                }
            }

        }
    }
}