using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class LimitTimeHaveExtraItemActivityViewCommon : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private Text mDesc = null;
        [SerializeField]
        private Text mCount = null;
        [SerializeField]
        private GameObject mExtraItemRoot = null;
        [SerializeField]
        private RectTransform mNormalItemRoot = null;
        [SerializeField]
        private GameObject mFinished = null;
        [SerializeField]
        private GameObject mUnFinish = null;
        [SerializeField]
        private GameObject mReceive = null;
        [SerializeField]
        private Button mUnFinishBtn = null;
        [SerializeField]
        private Button mReceiveBtn = null;
        [SerializeField]
        private GameObject mRewardListRoot = null;
        [SerializeField]
        Vector2 mRewardSize = new Vector2(100f, 100f);
        [SerializeField]
        Vector2 mNormalItemRoot1 = new Vector2(1183, 246);
        [SerializeField]
        Vector2 mNormalItemRoot2 = new Vector2(1183, 385);
        private uint activityId;
        private List<GameObject> rewardList = new List<GameObject>();
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            activityId = model.Id;
            base.Init(model, onItemClick);
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

            mItems.Clear();
            mExtraItemRoot.CustomActive(false);
            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                if(data.TaskDatas[i].CantAccept == 0)
                {
                    {
                        mExtraItemRoot.CustomActive(true);
                        if(data.TaskDatas[i].State != Protocol.OpActTaskState.OATS_INIT)
                        {
                            _UpdateExtraItem(data.TaskDatas[i]);
                        }
                    }
                }
                else
                {
                    _AddItem(go, i, data);
                }
            }
            if (mExtraItemRoot.activeSelf)
            {
                mNormalItemRoot.sizeDelta = mNormalItemRoot1;
            }
            else
            {
                mNormalItemRoot.sizeDelta = mNormalItemRoot2;
            }
            Destroy(go);
        }
        //protected override sealed void _InitItems(ILimitTimeActivityModel data)
        //{

        //}
        public override void Dispose()
        {
            base.Dispose();
        }
        //private void _OnCountValueChanged(UIEvent uiEvent)
        //{

        //}
        public override void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || mItems == null)
            {
                Logger.LogError("ActivityLimitTimeData data is null");
                return;
            }
            GameObject go = null;
            mExtraItemRoot.CustomActive(false);
            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                if (data.TaskDatas[i].CantAccept == 0)
                {
                    mExtraItemRoot.CustomActive(true);
                    _UpdateExtraItem(data.TaskDatas[i]);
                }
                else
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
            }
            if(mExtraItemRoot.activeSelf)
            {
                mNormalItemRoot.sizeDelta = mNormalItemRoot1;
            }
            else
            {
                mNormalItemRoot.sizeDelta = mNormalItemRoot2;
            }
        }

        private void _UpdateExtraItem(ILimitTimeActivityTaskDataModel data)
        {
            mDesc.text = data.Desc;
            mCount.text = string.Format("已完成{0}/{1}", data.DoneNum, data.TotalNum);
            for(int i = 0;i< rewardList.Count;i++)
            {
                GameObject.Destroy(rewardList[i]);
            }
            for (int i = 0;i< data.AwardDataList.Count;i++)
            {
                var comItem = ComItemManager.Create(mRewardListRoot.gameObject);
                if (comItem != null)
                {
                    ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
                    item.Count = (int)data.AwardDataList[i].num;
                    comItem.Setup(item, Utility.OnItemClicked);
                    (comItem.transform as RectTransform).sizeDelta = mRewardSize;
                    rewardList.Add(comItem.gameObject);
                }
            }
            mFinished.CustomActive(false);
            mUnFinish.CustomActive(false);
            mReceive.CustomActive(false);
            switch (data.State)
            {
                case Protocol.OpActTaskState.OATS_UNFINISH:
                    mUnFinish.CustomActive(true);
                    break;
                case Protocol.OpActTaskState.OATS_FINISHED:
                    mReceive.CustomActive(true);
                    break;
                case Protocol.OpActTaskState.OATS_OVER:
                    mFinished.CustomActive(true);
                    break;
            }
            mUnFinishBtn.onClick.RemoveAllListeners();
            mUnFinishBtn.onClick.AddListener(() =>
            {

            });
            mReceiveBtn.onClick.RemoveAllListeners();
            mReceiveBtn.onClick.AddListener(() =>
            {
                ActivityDataManager.GetInstance().RequestOnTakeActTask(activityId, (uint)data.DataId);
            });
        }
    }
}