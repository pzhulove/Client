﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FeedbackGiftActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]private GameObject[] mItemRoots = new GameObject[0];
        [SerializeField]private Text mTextTime;
        [SerializeField]private Text mTextRule;
        [SerializeField]private Text mTextTotalProgrees;
        [SerializeField]private Slider mImageProgresss;
       
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
           
            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                if (mItems.ContainsKey(data.TaskDatas[i].DataId))
                {
                    mItems[data.TaskDatas[i].DataId].UpdateData(data.TaskDatas[i]);
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
            
            for (int i = 0; i < data.TaskDatas.Count; i++)
            {
                _AddItem(go, i, data);
            }

            Destroy(go);
        }

        protected new void _AddItem(GameObject go, int id, ILimitTimeActivityModel data)
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
            if (mImageProgresss != null && model.ParamArray != null && model.ParamArray.Length > 0)
            {
                float finishedValue = 0f;
                if (model.TaskDatas.Count > 0)
                {
                    finishedValue = model.TaskDatas[0].DoneNum;
                }
             
                mImageProgresss.value = finishedValue / model.ParamArray[0];

                mTextTotalProgrees.SafeSetText(string.Format("{0}", finishedValue));
            }
        }
    }
}