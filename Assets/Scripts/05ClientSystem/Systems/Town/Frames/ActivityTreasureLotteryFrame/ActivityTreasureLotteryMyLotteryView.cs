using DataModel;
using Protocol;
using ProtoTable;
using Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {
        /// <summary>
        /// 我的夺宝分页
        /// </summary>
        public class ActivityTreasureLotteryMyLotteryView : ActivityTreasureLotteryActivityViewBase<IActivityTreasureLotteryMyLotteryModel>
        {
            #region serialize field
            [SerializeField]
            private Text mTextItemName = null;

            [SerializeField]
            private Text mTextState;

            [SerializeField]
            private Text mTextRate = null;

            [SerializeField]
            private Text mTextGroupId = null;

            [SerializeField]
            private Text mTextSoldInfo = null;

            [SerializeField]
            private Text mTextBought = null;

            [SerializeField]
            private GameObject mWinnerPanel = null;

            [SerializeField]
            private Text mTextWinnerInfo = null;

            [SerializeField]
            private Transform mComItemRoot;

            [SerializeField]
            private Text mTextEmptyTip = null;

            [SerializeField]
            private GameObject mRightPanel = null;
            #endregion

            ComItem mComItem = null;

            protected override void OnInit()
            {
                base.OnInit();
                if (mDataManager.GetModelAmount<IActivityTreasureLotteryMyLotteryModel>() <= 0)
                {
                    mTextEmptyTip.SafeSetText(TR.Value("activity_treasure_my_lottery_empty_tip"));
                    mTextEmptyTip.gameObject.CustomActive(true);
                    mRightPanel.CustomActive(false);
                }
                else
                {
                    mTextEmptyTip.gameObject.CustomActive(false);
                    mRightPanel.CustomActive(true);
                }
            }

            protected sealed override void OnSelectItem(IActivityTreasureLotteryMyLotteryModel data)
            {
                if (mComItem == null)
                {
                    mComItem = ComItemManager.Create(mComItemRoot.gameObject);
                }
#if !UNITY_EDITOR
                if (data == null)
                {
                    return;
                }
#endif
                if (mTextEmptyTip != null && mTextEmptyTip.gameObject.activeSelf)
                {
                    mTextEmptyTip.gameObject.CustomActive(false);
                    mRightPanel.CustomActive(true);
                }

                ItemTable itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(data.ItemId);
                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.ItemId);
                mComItem.Setup(itemData, ShowItemTip);
                if (itemTableData != null)
                {
                    var qualityInfo = ItemData.GetQualityInfo(itemTableData.Color);
                    if (qualityInfo != null)
                    {
                        mTextItemName.SafeSetText(string.Format("<color={0}>{1}*{2}</color>", qualityInfo.ColStr, itemTableData.Name, data.ItemNum));
                    }
                    else
                    {
                        Logger.LogError(string.Format("找不到匹配的qualityInfo, 道具id为 {0}, ", data.ItemId));
                    }
                }
                else
                {
                    Logger.LogError(string.Format("错误的 itemId:{0} 在道具表中找不到此id", data.ItemId));
                }

                switch (data.Status)
                {
                    case GambingMineStatus.GMS_WAIT:
                        InitOpenState(data);
                        break;
                    case GambingMineStatus.GMS_SUCCESS:
                    case GambingMineStatus.GMS_FAILE:
                        InitCloseState(data);
                        break;
                }
            }

            protected override void OnDispose()
            {
                base.OnDispose();
                ComItemManager.Destroy(mComItem);
                mComItem = null;
            }

            void ShowItemTip(GameObject go, ItemData itemData)
            {
                if (null != itemData)
                {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                }
            }
            void InitOpenState(IActivityTreasureLotteryMyLotteryModel model)
            {
                mTextState.SafeSetText(TR.Value("activity_treasure_my_lottery_state_open"));
                InitBaseInfo(model);
                mWinnerPanel.CustomActive(false);
            }

            void InitCloseState(IActivityTreasureLotteryMyLotteryModel model)
            {
                InitBaseInfo(model);
                mTextState.SafeSetText(TR.Value("activity_treasure_my_lottery_state_close"));
                mWinnerPanel.CustomActive(true);
                if (model != null)
                {
                    //玩家名字 xx货币
                    mTextWinnerInfo.SafeSetText(string.Format(TR.Value("activity_treasure_my_lottery_winner"), model.WinnerName, model.WinnerInvestment, model.CurrencyName));
                }
            }

            void InitBaseInfo(IActivityTreasureLotteryMyLotteryModel model)
            {
                if (model != null)
                {
                    mTextRate.SafeSetText(string.Format(TR.Value("activity_treasure_my_lottery_rate"), model.WinRate));
                    mTextGroupId.SafeSetText(string.Format(TR.Value("activity_treasure_my_lottery_group_id"), model.GroupId));
                    mTextSoldInfo.SafeSetText(string.Format(TR.Value("activity_treasure_my_lottery_sold"), model.TotalNum - model.RestNum, model.TotalNum));
                    //我的投入xx货币(xx份)
                    mTextBought.SafeSetText(string.Format(TR.Value("activity_treasure_my_lottery_bought"), model.MyInvestment, model.CurrencyName, model.BoughtNum));
                }
            }

        }
    }
}