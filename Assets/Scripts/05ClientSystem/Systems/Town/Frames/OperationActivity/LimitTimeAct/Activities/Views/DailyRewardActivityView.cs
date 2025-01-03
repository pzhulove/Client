using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class DailyLevelRewardData
    {
        public int minLv;
        public int maxLv;
        public List<int> itemIdList = new List<int>();
        public List<int> itemNumList = new List<int>();

        public DailyLevelRewardData(GiftSyncInfo info)
        {
            minLv = info.validLevels[0] < info.validLevels[1] ? (int)info.validLevels[0] : (int)info.validLevels[1];
            maxLv = info.validLevels[0] > info.validLevels[1] ? (int)info.validLevels[0] : (int)info.validLevels[1];
            itemIdList.Add((int)info.itemId);
            itemNumList.Add((int)info.itemNum);
        }

        public void AddItem(GiftSyncInfo info)
        {
            itemIdList.Add((int)info.itemId);
            itemNumList.Add((int)info.itemNum);
        }
    }

    public class DailyRewardActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField] private ComUIListScript mLevelRewardList;
        private List<DailyLevelRewardData> mLvRewardDataList = new List<DailyLevelRewardData>();
        private uint mGiftId;

        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
	        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftPackInfoResp);
            base.Init(model, onItemClick);
            _ShowLevelRewrad(model);
        }

        private void _ShowLevelRewrad(ILimitTimeActivityModel model)
        {
            if (null != mLevelRewardList)
            {
                mLevelRewardList.Initialize();
                mLevelRewardList.onItemVisiable = _OnItemShow;
            }
            var awards = model.TaskDatas;
            if (null != awards && awards.Count > 0 && null != awards[0].AwardDataList && awards[0].AwardDataList.Count > 0)
            {
                mGiftId = awards[0].AwardDataList[0].id;
                //请求礼包信息
                GiftPackDataManager.GetInstance().GetGiftPackItem((int)mGiftId);
            }
        }
        //接受礼包信息
        void _OnGetGiftPackInfoResp(UIEvent param)
        {
	        if (param == null || param.Param1 == null)
	        {
		        Logger.LogError("礼包数据为空");
		        return;
	        }
	        GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;
            if (data.id == mGiftId)
            {
				_InitAwardItemDetails(data.gifts);
            }
        }
        private void _InitAwardItemDetails(GiftSyncInfo[] gifts)
        {
            mLvRewardDataList.Clear();
            List<int> levelList = new List<int>();
            foreach (var info in gifts)
            {
                int minLv = info.validLevels[0] < info.validLevels[1] ? (int)info.validLevels[0] : (int)info.validLevels[1];
                int maxLv = info.validLevels[0] > info.validLevels[1] ? (int)info.validLevels[0] : (int)info.validLevels[1];
                if (!levelList.Contains(minLv))
                {
                    levelList.Add(minLv);
                    var data = new DailyLevelRewardData(info);
                    mLvRewardDataList.Add(data);
                }
                else
                {
                    int index = levelList.IndexOf(minLv);
                    if (mLvRewardDataList.Count > index && index >= 0)
                        mLvRewardDataList[index].AddItem(info);
                    else
                    {
                        Logger.LogError("每日登录活动有问题");
                    }
                }
            }
            mLevelRewardList.SetElementAmount(mLvRewardDataList.Count);
        }

        private void _OnItemShow(ComUIListElementScript item)
        {
            if (null == item)
                return;
            var script = item.GetComponent<DailyRewardLevelItem>();
            if (null == script)
                return;
            script.OnInit(mLvRewardDataList[item.m_index]);
        }

        public override void UpdateData(ILimitTimeActivityModel data)
        {
            base.UpdateData(data);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void Dispose()
        {
	        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftPackInfoResp);
            base.Dispose();
        }
    }
}
