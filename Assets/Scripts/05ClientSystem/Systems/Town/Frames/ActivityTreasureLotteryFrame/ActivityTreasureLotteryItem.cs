using DataModel;
using Protocol;
using ProtoTable;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{ 
    namespace ActivityTreasureLottery
    {
        public interface IActivityTreasureLotteryItem
        {
            void OnSelect(bool value);

            //void OnSelect(bool value = true); 这样unity会莫名其妙的认为脚本有错 还不报编译错误。


            void Init<T>(T model, bool isOn) where T : IActivityTreasureLotteryModelBase;
		}

        //public class ActivityTreasureLotteryItem : MonoBehaviour, IActivityTreasureLotteryItem
        public class ActivityTreasureLotteryItem : MonoBehaviour, IActivityTreasureLotteryItem
        {

            #region serialize filed
            [SerializeField]
            GameObject mMainViewInfo;

            [SerializeField]
            GameObject mMyLotteryViewInfo;

            [SerializeField]
            GameObject mHistroyViewRoot;

            [Header("公共部分数据")]
            [SerializeField]
            Text mTextName;

            [SerializeField]
            Transform mComItemRoot;

            [SerializeField]
            Toggle mToggleSelect;

            [Header("夺宝中界面的数据")]
            [SerializeField]
            Slider mSliderProgress;

            [SerializeField]
            Text mTextLeftNum;

            [SerializeField]
            Text mTextLeftGroup;

            [SerializeField]
            GameObject mTipPanel;

            [SerializeField]
            Text mTextTip;

            [Header("我的夺宝界面的数据")]
            [SerializeField]
            Slider mMyLotteryProgress;

            [SerializeField]
            Text mTextMyLotteryLeftNum;

            [SerializeField]
            Text mTextMyLotteryBought;

            [SerializeField]
            Text mTextMyLotteryState;

            [SerializeField]
            GameObject mMyLotterProgressRoot;

            [Header("夺宝记录界面的数据")]
            [SerializeField]
            Text mTextHistoryResult;

            [SerializeField]
            Image mImageWin;


            ComItem mComItem = null;
            #endregion
            IActivityTreasureLotteryModelBase mModel;
            public void Init<T>(T model, bool isOn) where T : IActivityTreasureLotteryModelBase
            {
                if (model is IActivityTreasureLotteryModel)
                {
                    Init(model as IActivityTreasureLotteryModel, isOn);
                }
                else if (model is IActivityTreasureLotteryMyLotteryModel)
                {
                    Init(model as IActivityTreasureLotteryMyLotteryModel, isOn);
                }
                else if (model is IActivityTreasureLotteryHistoryModel)
                {
                    Init(model as IActivityTreasureLotteryHistoryModel, isOn);
                }
            }
			public void OnSelect(bool value)
            {
                mToggleSelect.isOn = value;
            }

            void Update()
            {
                if (mModel is IActivityTreasureLotteryModel)
                {
                    var model = mModel as IActivityTreasureLotteryModel;
                    if (model != null && model.State == GambingItemStatus.GIS_PREPARE)
                    {
                        if (model.TimeRemain <= 0)
                        {
                            mTextLeftGroup.SafeSetText(TR.Value("activity_treasure_lottery_item_loading"));
                            
                        }
                        else
                        {
                            mTextLeftGroup.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_item_prepare"), model.TimeRemainStr, model.LeftGroupNum, model.TotalGroup));
                        }
                    }
                }
            }
            
            /// <summary>
            /// 初始化夺宝中界面数据
            /// </summary>
            /// <param name="model">夺宝中道具数据模型</param>
            void Init(IActivityTreasureLotteryModel model, bool isOn)
            {
                InitBase(model, isOn);
                mMainViewInfo.CustomActive(true);
                mMyLotteryViewInfo.CustomActive(false);
                mHistroyViewRoot.CustomActive(false);
                if (model != null)
                {
	                mTextLeftGroup.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_group_num"), model.LeftGroupNum));
	                mTextLeftNum.SafeSetText(string.Format("{0}/{1}", model.LeftNum, model.TotalNum));
	                mSliderProgress.value = 0;
	                if (model.TotalNum > 0)
	                {
		                mSliderProgress.value = (float)model.LeftNum / model.TotalNum;
	                }

	                mTipPanel.CustomActive(false);
	                switch (model.State)
	                {
		                case GambingItemStatus.GIS_PREPARE:
			                mTextLeftGroup.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_item_prepare"), model.TimeRemainStr, model.LeftGroupNum, model.TotalGroup));
			                break;
		                case GambingItemStatus.GIS_SOLD_OUE:
		                case GambingItemStatus.GIS_LOTTERY:
			                mTipPanel.CustomActive(true);
			                mTextTip.SafeSetText(TR.Value("activity_treasure_lottery_item_sell_out"));
			                break;
	                }
                }
            }

            /// <summary>
            /// 初始化我的夺宝界面数据
            /// </summary>
            /// <param name="model">我的夺宝道具数据模型</param>
            void Init(IActivityTreasureLotteryMyLotteryModel model, bool isOn)
            {
                InitBase(model, isOn);
                mMainViewInfo.CustomActive(false);
                mMyLotteryViewInfo.CustomActive(true);
                mHistroyViewRoot.CustomActive(false);

                if (model == null)
                {
                    return;
                }

                mTextMyLotteryBought.SafeSetText(string.Format(TR.Value("activity_treasure_lottery_bought"), model.BoughtNum));

                switch (model.Status)
                {
                    case GambingMineStatus.GMS_WAIT:
                        mMyLotterProgressRoot.CustomActive(true);
                        if (mMyLotteryProgress != null)
                        {
                            mMyLotteryProgress.value = 0;
                        }

                        if (model.TotalNum > 0)
                        {
                            mMyLotteryProgress.value = (float)model.RestNum / model.TotalNum;
                        }
                        mTextMyLotteryLeftNum.SafeSetText(string.Format("{0}/{1}", model.RestNum, model.TotalNum));
                        mTextMyLotteryState.SafeSetText(TR.Value("activity_treasure_lottery_waiting"));
                        break;
                    case GambingMineStatus.GMS_SUCCESS:
                        mMyLotterProgressRoot.CustomActive(false);
                        mTextMyLotteryState.SafeSetText(TR.Value("activity_treasure_lottery_win"));
                        break;
                    case GambingMineStatus.GMS_FAILE:
                        mMyLotterProgressRoot.CustomActive(false);
                        mTextMyLotteryState.SafeSetText(TR.Value("activity_treasure_lottery_lose"));
                        break;
                }

            }

            /// <summary>
            /// 初始化夺宝记录界面数据
            /// </summary>
            /// <param name="model"></param>
            void Init(IActivityTreasureLotteryHistoryModel model, bool isOn)
            {
                InitBase(model, isOn);
                mMainViewInfo.CustomActive(false);
                mMyLotteryViewInfo.CustomActive(false);
                mHistroyViewRoot.CustomActive(true);
                if (model != null)
                {
                    if (model.IsWin)
                    {
                        mImageWin.CustomActive(true);
                    }
                    else
                    {
                        mImageWin.CustomActive(false);
                    }

                    mTextHistoryResult.SafeSetText(model.IsSellOut ? string.Format(TR.Value("activity_treasure_history_sold_out"), model.TimeSoldOut) : TR.Value("activity_treasure_history_selling"));
                }
            }

            void InitBase(IActivityTreasureLotteryModelBase model, bool isOn)
            {
                mModel = model;
                if (this.mComItem != null)
                {
	                if (mComItem.ItemData.TableID != model.ItemId)
	                {
		                ComItemManager.Destroy(mComItem);
		                this.mComItem = null;
		                mComItem = ComItemManager.Create(mComItemRoot.gameObject);
	                }
				}
                else
                {
	                mComItem = ComItemManager.Create(mComItemRoot.gameObject);
				}


				if (mModel != null)
                {
	                if (mComItem.ItemData == null || mComItem.ItemData.TableID != mModel.ItemId)
	                {
		                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mModel.ItemId);
		                if (mComItem != null)
		                {
			                mComItem.Setup(itemData, ShowItemTip);
		                }
	                }

	                if (mToggleSelect != null)
	                {
		                mToggleSelect.isOn = isOn;
	                }

	                ItemTable itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(mModel.ItemId);
	                if (itemTableData != null)
	                {
		                var qualityInfo = ItemData.GetQualityInfo(itemTableData.Color);
		                if (qualityInfo != null)
		                {
			                mTextName.SafeSetText(string.Format("<color={0}>{1}*{2}</color>", qualityInfo.ColStr, mModel.Name, mModel.ItemNum));
		                }
	                }
	                else
	                {
		                Logger.LogError("未找到的物品id:" + model.ItemId);
	                }
                }
            }
            void ShowItemTip(GameObject go, ItemData itemData)
            {
                if (null != itemData)
                {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                }
            }

            void OnDestroy()
            {
                ComItemManager.Destroy(mComItem);
            }
        }
    }
}