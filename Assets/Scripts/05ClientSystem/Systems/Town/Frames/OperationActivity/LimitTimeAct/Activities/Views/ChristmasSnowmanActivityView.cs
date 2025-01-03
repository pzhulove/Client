using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChristmasSnowmanActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private GameObject[] mItemRoots = new GameObject[0];
        [SerializeField]
        private Text mTextTime;
        [SerializeField]
        private Text mTextRule;
        [SerializeField]
        private Text mTextTodayFalling;
        [SerializeField]
        private Text mTextTotalProgrees;
        [SerializeField]
        private Image mImageProgresss;
        [SerializeField]
        private string mItemPrefabPath2 = "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ChristmasSnowmanItem2";

        public override sealed void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mOnItemClick = onItemClick;
            _InitItems(model);
            _UpdateTitleInfo(model);
            _UpdateProgressInfo(model);
        }

        public override sealed void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || mItems == null)
            {
                Logger.LogError("ActivityLimitTimeData data is null");
                return;
            }

            GameObject go = null;
            GameObject go2 = null;
            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                if (mItems.ContainsKey(data.TaskDatas[i].DataId))
                {
                    mItems[data.TaskDatas[i].DataId].UpdateData(data.TaskDatas[i]);
                }
                else
                {
                    if (i == data.TaskDatas.Count - 1)
                    {
                        if (go2 == null)
                        {
                            go2 = AssetLoader.GetInstance().LoadResAsGameObject(mItemPrefabPath2);
                        }

                        _AddItem(go2, i, data);
                    }
                    else
                    {
                        if (go == null)
                        {
                            go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
                        }

                        _AddItem(go, i, data);
                    }
                }
            }

            //遍历删除多余的数据
            List<uint> dataIdList = new List<uint>(mItems.Keys);
            for (int i = 0; i < dataIdList.Count; ++i)
            {
                bool isHave = false;
                for (int j = 0; j < data.TaskDatas.Count; ++j)
                {
                    if (dataIdList[i] == data.TaskDatas[j].DataId)
                    {
                        isHave = true;
                        break;
                    }
                }

                if (!isHave)
                {
                    var item = mItems[dataIdList[i]];
                    mItems.Remove(dataIdList[i]);
                    item.Destroy();
                }
            }

            if (go != null)
            {
                Destroy(go);
            }
            if (go2 != null)
            {
                Destroy(go2);
            }
            
            _UpdateProgressInfo(data);
        }

        protected override sealed void _InitItems(ILimitTimeActivityModel data)
        {
            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
            if (go == null)
            {
                Logger.LogError("加载预制体失败，路径:" + data.ItemPath);
                return;
            }

            if (go.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }

            GameObject go2 = AssetLoader.GetInstance().LoadResAsGameObject(mItemPrefabPath2);
            if (go2 == null)
            {
                Logger.LogError("加载预制体失败，路径:" + mItemPrefabPath2);
                return;
            }

            if (go2.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go2);
                Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + mItemPrefabPath2);
                return;
            }

            for (int i = 0; i < data.TaskDatas.Count; i++)
            {
                if (i == data.TaskDatas.Count - 1)
                {
                    _AddItem(go2, i, data);
                }
                else
                {
                    _AddItem(go, i, data);
                }
            }

            Destroy(go);
            Destroy(go2);
        }
        
        protected new void _AddItem(GameObject go,int id,ILimitTimeActivityModel data)
        {
            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoots[id].transform, false);
            item.GetComponent<IActivityCommonItem>().Init(data.TaskDatas[id].DataId, data.Id, data.TaskDatas[id], mOnItemClick);
            mItems.Add(data.TaskDatas[id].DataId, item.GetComponent<IActivityCommonItem>());
        }

        void _UpdateTitleInfo(ILimitTimeActivityModel model)
        {
            mTextTime.SafeSetText(string.Format("{0}", Function.GetTimeWithoutYearNoZero((int)model.StartTime, (int)model.EndTime)));
            mTextRule.SafeSetText(model.RuleDesc);
        }

        void _UpdateProgressInfo(ILimitTimeActivityModel model)
        {
            if (mImageProgresss != null && model.ParamArray != null && model.ParamArray.Length > 1)
            {
                float finishedValue = 0f;
                for (int i = 0; i < model.TaskDatas.Count; ++i)
                {
                    finishedValue += model.TaskDatas[i].DoneNum;
                }
                mImageProgresss.fillAmount = (model.ParamArray[0] - finishedValue) / model.ParamArray[0];

                string des = finishedValue > 0 ? TR.Value("activity_sheng_dan_xue_ren_total_progress") : TR.Value("activity_sheng_dan_xue_ren_total_progress_Two");

                mTextTotalProgrees.SafeSetText(string.Format(des, finishedValue, model.ParamArray[0]));
            }
            //今日完成度
            if (model.ParamArray != null && model.ParamArray.Length >= 2)
            {
                string countKey = string.Format("{0}{1}", model.Id, CounterKeys.OPACT_MAGPIE_BRIDGE_DAILY_PROGRESS);
                var completeValue = CountDataManager.GetInstance().GetCount(countKey);
                var maxValue = model.ParamArray[1];
                mTextTodayFalling.SafeSetText(string.Format("{0}℃/{1}℃", completeValue, maxValue));
            }
        }
    }
}
