using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class OnLineGiftGivingActivityView : MonoBehaviour, IActivityView
    {
        [SerializeField] protected RectTransform mOnLineGiftItemRoot = null;
        [SerializeField] protected RectTransform mNumberDayItemRoot = null;
        [SerializeField] protected ActiveUpdate mActiveUpdate;
        [SerializeField] protected Text mNumberDay;
        [SerializeField] private Text mActivityTime;
        [SerializeField] private Text mActivityRule;

        protected readonly Dictionary<uint, IActivityCommonItem> mItems = new Dictionary<uint, IActivityCommonItem>();

        protected ActivityItemBase.OnActivityItemClick<int> mOnItemClick;

        protected ILimitTimeActivityModel mModel;

        private int totalonLineTime = 0;
        private int totalDay = 0;
        private int curDay = 0;

        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            mModel = model;
            mOnItemClick = onItemClick;

            mActivityTime.SafeSetText(string.Format("{0}~{1}", Function._TransTimeStampToStr(model.StartTime), Function._TransTimeStampToStr(model.EndTime)));
            mActivityRule.SafeSetText(model.RuleDesc.Replace('|', '\n'));

            if (mModel.ParamArray.Length >= 2)
            {
                totalonLineTime = (int)mModel.ParamArray[0];
                totalDay = (int)mModel.ParamArray[1];
            }

            InitItems(model);
          
            if (mActiveUpdate != null)
            {
                mActiveUpdate.SetTotlaNum(totalonLineTime);
            }
        }

        public void Show()
        {

        }

        public void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || mItems == null)
            {
                Logger.LogError("ActivityLimitTimeData data is null");
                return;
            }

            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                var taskData = data.TaskDatas[i];
                if (taskData == null)
                {
                    continue;
                }
                
                if (mItems.ContainsKey(taskData.DataId))
                {
                    mItems[taskData.DataId].UpdateData(taskData);
                }

                if (taskData.ParamNums2[0] == 2)
                {
                    curDay = (int)taskData.DoneNum;
                }
            }

            if (mNumberDay != null)
            {
                mNumberDay.text = string.Format("({0}/{1})", curDay, totalDay);
            }
        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            foreach (var item in mItems.Values)
            {
                item.Dispose();
            }

            mItems.Clear();
            mOnItemClick = null;
            mModel = null;
        }

        public void Hide()
        {
            
        }

        private void InitItems(ILimitTimeActivityModel data)
        {
            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
            if (go == null)
            {
                Logger.LogErrorFormat("加载预制体失败，路径:" + data.ItemPath);
                return;
            }

            if (go.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogErrorFormat("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }

            mItems.Clear();

            List<ILimitTimeActivityTaskDataModel> onlineRewardList = new List<ILimitTimeActivityTaskDataModel>();
            List<ILimitTimeActivityTaskDataModel> numberDayRewardList = new List<ILimitTimeActivityTaskDataModel>();

            for (int i = 0; i < data.TaskDatas.Count; i++)
            {
                var taskData = data.TaskDatas[i];
                if (taskData == null)
                {
                    continue;
                }

                if (taskData.ParamNums2[0] == 1)
                {
                    onlineRewardList.Add(taskData);
                }
                else
                {
                    numberDayRewardList.Add(taskData);
                }
            }

            for (int i = 0; i < onlineRewardList.Count; i++)
            {
                var taskData = onlineRewardList[i];
                if (taskData == null)
                {
                    continue;
                }

                bool isShow = i == onlineRewardList.Count - 1 ? false : true;

                _AddItem(go, taskData, data, mOnLineGiftItemRoot, isShow);
            }

            for (int i = 0; i < numberDayRewardList.Count; i++)
            {
                var taskData = numberDayRewardList[i];
                if (taskData == null)
                {
                    continue;
                }

                curDay = (int)taskData.DoneNum;

                bool isShow = i == numberDayRewardList.Count - 1 ? false : true;

                _AddItem(go, taskData, data, mNumberDayItemRoot, isShow);
            }

            if (mNumberDay != null)
            {
                mNumberDay.text = string.Format("({0}/{1})", curDay, totalDay);
            }

            Destroy(go);
        }

        protected void _AddItem(GameObject go, ILimitTimeActivityTaskDataModel taskData, ILimitTimeActivityModel data, RectTransform goParent,bool bIsShowArrow)
        {
            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(goParent, false);
            item.GetComponent<IActivityCommonItem>().Init(taskData.DataId, data.Id, taskData, mOnItemClick);
            (item.GetComponent<IActivityCommonItem>() as OnLineGiftGivingItem).OnSetArrowIsShow(bIsShowArrow);
            mItems.Add(taskData.DataId, item.GetComponent<IActivityCommonItem>());
        }
    }
}