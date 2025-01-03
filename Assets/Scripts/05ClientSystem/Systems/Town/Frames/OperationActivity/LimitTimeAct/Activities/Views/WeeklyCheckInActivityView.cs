using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class WeeklyCheckInActivityView : MonoBehaviour, IActivityView
    {
        [SerializeField] private RectTransform mSignInRoot;
        [SerializeField] private RectTransform mCumulativeSignInRoot;
        [SerializeField] private Text mActivityTime;
        [SerializeField] private Text mActivityRule;
        [SerializeField] private Button mBtnRecommendedDungeons;

        protected readonly Dictionary<uint, IActivityCommonItem> mItems = new Dictionary<uint, IActivityCommonItem>();

        protected ActivityItemBase.OnActivityItemClick<int> mOnItemClick;

        protected ILimitTimeActivityModel mModel;

        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            mModel = model;
            mOnItemClick = onItemClick;

            mActivityTime.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            mActivityRule.SafeSetText(model.RuleDesc.Replace('|', '\n'));
            mBtnRecommendedDungeons.SafeRemoveAllListener();
            mBtnRecommendedDungeons.SafeAddOnClickListener(OnRecommendedDungeonsBtnClick);
            InitItems(model);
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
        
        public void Show()
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

            List<ILimitTimeActivityTaskDataModel> signInTaskDataModelList = new List<ILimitTimeActivityTaskDataModel>();
            List<ILimitTimeActivityTaskDataModel> cumulativeSignInDataModelList = new List<ILimitTimeActivityTaskDataModel>();

            for (int i = 0; i < data.TaskDatas.Count; i++)
            {
                var taskData = data.TaskDatas[i];
                if (taskData == null)
                {
                    continue;
                }

                if (taskData.ParamNums.Count <= 0)
                {
                    continue;
                }

                if (taskData.ParamNums[0] == 0)
                {
                    signInTaskDataModelList.Add(taskData);
                }
                else
                {
                    cumulativeSignInDataModelList.Add(taskData);
                }
            }

            for (int i = 0; i < signInTaskDataModelList.Count; i++)
            {
                var taskData = signInTaskDataModelList[i];
                if (taskData == null)
                {
                    continue;
                }
                
                _AddItem(go, taskData, data, mSignInRoot,i);
            }

            for (int i = 0; i < cumulativeSignInDataModelList.Count; i++)
            {
                var taskData = cumulativeSignInDataModelList[i];
                if (taskData == null)
                {
                    continue;
                }
                
                _AddItem(go, taskData, data, mCumulativeSignInRoot,i);
            }
            Destroy(go);
        }

        protected void _AddItem(GameObject go, ILimitTimeActivityTaskDataModel taskData, ILimitTimeActivityModel data, RectTransform goParent,int index)
        {
            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(goParent, false);
            item.GetComponent<IActivityCommonItem>().Init(taskData.DataId, data.Id, taskData, mOnItemClick);
            item.GetComponent<WeeklyCheckInActivityItem>().SetBackground(index);
            mItems.Add(taskData.DataId, item.GetComponent<IActivityCommonItem>());
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数

            return string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
            //return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }

        public static void OnRecommendedDungeonsBtnClick()
        {
            WeekSignSpringTable weekSignSpringTable = null;
            var enumerator = TableManager.GetInstance().GetTable<WeekSignSpringTable>().GetEnumerator();

            while (enumerator.MoveNext())
            {
                var table = enumerator.Current.Value as WeekSignSpringTable;
                if (table == null)
                {
                    continue;
                }

                if (PlayerBaseData.GetInstance().Level < table.StartLv)
                {
                    continue;
                }

                if (PlayerBaseData.GetInstance().Level > table.EndLv)
                {
                    continue;
                }

                weekSignSpringTable = table;
                break; 
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<RecommendedDungeonsFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RecommendedDungeonsFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<RecommendedDungeonsFrame>(FrameLayer.Middle, weekSignSpringTable);
        }
    }
}
