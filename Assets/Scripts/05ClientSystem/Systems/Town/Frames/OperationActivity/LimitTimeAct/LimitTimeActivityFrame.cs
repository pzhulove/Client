using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ActivityLimitTime;
using Protocol;
using ProtoTable;
using Network;
using System;
///////删除linq

namespace GameClient
{
    public enum LotteryType
    {
        ConsumeLottery = 10002,

        OnlineTimeLottert = 10006,//在线时间转盘
    }

    public class LimitTimeActivityFrame : ClientFrame
    {
        struct ToggleIndex
        {
            public int ParentId;
            public int SubId;

            public ToggleIndex(int parentId, int subId)
            {
                ParentId = parentId;
                SubId = subId;
            }
        }

        //预制体路径
        private const string prefabPath = "UIFlatten/Prefabs/OperateActivity/ActivityCombineFrame";
        // 外部list对应大页签，内部list对应小页签, 存储活动id
        readonly List<List<uint>> mActivityToggleData = new List<List<uint>>();
        //需要把大页签数组下标和大页签分类id做映射
        readonly List<int> mToggleFilterIds = new List<int>();
        //当前选中的大页签下标
        private int mCurrentSelectParentIndex;
        //当前选中的活动id
        private uint mSelectActivityId;

        //frame去创建预制体
        public override string GetPrefabPath()
        {
            return prefabPath;
        }

        //与update相关 但是并没有实现_OnUpdate 可以删除
        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            _InitView();
            _BindEvents();
            //获取商城礼包的数据
            ActivityDataManager.GetInstance().RequestMallGiftData();
            ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq(ActivityLimitTimeFactory.EActivityType.OAT_SUMMER_WEEKLY_VACATION, ActivityLimitTimeFactory.EActivityCounterType.OAT_SUMMER_WEEKLY_VACATION);
            if (userData != null)
            {
                int activityId = (int)userData;
                int parentId = _GetParentId(activityId);
                int subId = _GetSubId(parentId, activityId);
                mView.GoToToggleFromID(parentId, subId);
            }
        }
        public static void OpenLinkFrame(string strParam)
        {
            var tokens = strParam.Split('|');
            int activityId = -1;
            int.TryParse(tokens[0], out activityId);
            if (activityId != -1)
            {
                ClientSystemManager.GetInstance().OpenFrame<LimitTimeActivityFrame>(FrameLayer.Middle, activityId);
            }
        }

        /// <summary>
        /// 通过活动id 直接切换到指定的页面（限时活动页面要打开）
        /// </summary>
        /// <param name="activityId"></param>
        public void OpenFrameByActivityId(uint activityId)
        {
            int parentId = _GetParentId((int)activityId);
            int subId = _GetSubId(parentId, (int)activityId);
            if (mView != null)
            {
                mView.GoToToggleFromID(parentId, subId);
            }

        }

        //关闭界面
        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            //挨个关闭 销毁view的obj
            if (mActivityToggleData != null)
            {
                for (int i = 0; i < mActivityToggleData.Count; ++i)
                {
                    for (int j = 0; j < mActivityToggleData[i].Count; ++j)
                    {
                        var activity = ActivityManager.GetInstance().GetActivity(mActivityToggleData[i][j]);
                        if (activity != null)
                        {
                            activity.Close();
                        }
                    }
                }
                mActivityToggleData.Clear();
            }
            if (mToggleFilterIds != null)
                mToggleFilterIds.Clear();

            _UnBindEvents();
        }

        void _InitView()
        {
            if (mBind != null && mView != null)
            {
                mView.Init(_GetParentToggleData(), _InitSubToggleDatas(), _OnParentToggleValueChanged, _OnSubToggleValueChanged);
                mCurrentSelectParentIndex = 0;
                mView.OnButtonCloseClick = _OnCloseClick;
            }
        }

        void _OnCloseClick()
        {
            Close();
        }

        //选中父页签
        void _OnParentToggleValueChanged(int id, bool value)
        {
            if (value)
            {
                mCurrentSelectParentIndex = id;
            }
        }

        //选中子页签
        void _OnSubToggleValueChanged(int id, bool value)
        {
            if (value && mActivityToggleData != null && mCurrentSelectParentIndex >= 0 && mCurrentSelectParentIndex < mActivityToggleData.Count)
            {
                if (mView != null)
                {
                    ActivityManager.GetInstance().HideActivity(mSelectActivityId);

                    if (id < 0 || id >= mActivityToggleData[mCurrentSelectParentIndex].Count)
                    {
                        return;
                    }

                    ActivityManager.GetInstance().ShowActivity(mActivityToggleData[mCurrentSelectParentIndex][id], mView.FrameRoot);
                    mSelectActivityId = mActivityToggleData[mCurrentSelectParentIndex][id];
                }
            }
        }

        void OnButtonClose()
        {
            Close();
        }

        void _BindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, _OnTaskStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeUpdate, _OnActivityStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeToggleChange, _OnActivityLimitTimeToggleChange);
            //UIEventSystem.这里监听活动内页签跳转，
        }

        void _UnBindEvents() 
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, _OnTaskStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeUpdate, _OnActivityStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeToggleChange, _OnActivityLimitTimeToggleChange);
        }

        //任务状态改变
        void _OnTaskStateChanged(UIEvent uiEvent)
        {
            var data = (LimitTimeActivityTaskUpdateData)uiEvent.Param1;
            if (data == null)
            {
                return;
            }

            var activity = ActivityManager.GetInstance().GetActivity((uint)data.ActivityId);
            if (mToggleFilterIds != null && activity != null && mView != null)
            {
                var id = _GetParentId((int)activity.GetId());
                if (id >= 0 && id < mToggleFilterIds.Count)
                {
                    var subId = _GetSubId(id, (int)activity.GetId());
                    if (subId != -1)
                    {
                        mView.SetSubRedPoint(id, subId, activity.IsHaveRedPoint());
                        mView.SetParentRedPoint(id, ActivityManager.GetInstance().IsFilterHaveRedPoint(mToggleFilterIds[id]));
                    }
                }
            }
        }

        //选中某个活动
        void _OnActivityLimitTimeToggleChange(UIEvent uiEvent)
        {
            int activityId = (int)uiEvent.Param1;
            int parentId = _GetParentId(activityId);
            int subId = _GetSubId(parentId, activityId);
            mView.GoToToggleFromID(parentId, subId);
        }

        //活动状态修改
        void _OnActivityStateChanged(UIEvent uiEvent)
        {
            var activity = (IActivity)uiEvent.Param1;
            //var activity = ActivityManager.GetInstance().GetActivity((uint) activityId);
            if (activity != null && mView != null)
            {
                var activityId = (int)activity.GetId();
                var id = _GetParentId(activityId);
                //如果之前记录过此活动则更新数据或者删除
                if (id != -1)
                {
                    var subId = _GetSubId(id, activityId);
                    //如果活动没了 或者状态已经结束了
                    if (activity.GetState() == OpActivityState.OAS_END)
                    {
                        if (mActivityToggleData != null && id >= 0 && id < mActivityToggleData.Count && subId < mActivityToggleData[id].Count)
                        {
                            mActivityToggleData[id].RemoveAt(subId);
                            mView.RemoveSubToggle((uint)id, (uint)subId);
                            //如果此大页签下没有活动了,清除此大页签
                            if (mActivityToggleData[id].Count == 0)
                            {
                                mActivityToggleData.RemoveAt(id);
                                mToggleFilterIds.RemoveAt(id);
                                mView.RemoveParentToggle(id);
                            }
                        }
                    }
                    else
                    {
                        mView.SetSubRedPoint(id, subId, activity.IsHaveRedPoint());
                        mView.SetParentRedPoint(id, ActivityManager.GetInstance().IsFilterHaveRedPoint(mToggleFilterIds[id]));
                    }
                }
                //如果没有 则是新开启的活动,创建新的页签
                else if (activity.GetState() == OpActivityState.OAS_IN || activity.GetState() == OpActivityState.OAS_PREPARE)
                {
                    var filterId = ActivityDataManager.GetInstance().GetFilterIdByActivityId(activityId);
                    var subId = ActivityManager.GetInstance().GetActivityOrderId(filterId, activityId);
                    var parentId = _GetIdByFilterId(filterId);
                    //如果是-1则是没有对应的大页签,需要创建大页签
                    if (parentId == -1)
                    {
                        parentId = _GetParentInsertPosition(filterId);
                        if (mCurrentSelectParentIndex >= parentId)
                        {
                            mCurrentSelectParentIndex++;
                        }
                        mToggleFilterIds.Insert(parentId, filterId);
                        mActivityToggleData.Insert(parentId, new List<uint>());
                        mActivityToggleData[parentId].Add((uint)activityId);
                        List<ITwoLevelToggleData> subDatas = new List<ITwoLevelToggleData> { };
                        var tabData = ActivityDataManager.GetInstance().GetActivityTabInfo(filterId);
                        if (tabData != null)
                        {
                            ITwoLevelToggleData parentData = new LimitTimeToggleData(tabData.name, activity.IsHaveRedPoint(), null);
                            mView.AddTopToggle(subDatas, parentData, parentId);
                        }
                    }
                    else if (parentId >= 0 && parentId < mActivityToggleData.Count)
                    {
                        mActivityToggleData[parentId].Insert(subId, (uint)activityId);
                    }

                    mView.AddSubToggle((uint)parentId, new LimitTimeToggleData(activity), subId);
                }
            }
        }

        //获取大页签插入的位置
        int _GetParentInsertPosition(int filterId)
        {
            for (int i = 0; i < mToggleFilterIds.Count; ++i)
            {
                if (filterId < mToggleFilterIds[i])
                {
                    return i;
                }
            }

            return mActivityToggleData.Count;
        }

        //根据大页签类型id获取数组的id
        int _GetIdByFilterId(int filterId)
        {
            for (int i = 0; i < mToggleFilterIds.Count; ++i)
            {
                if (mToggleFilterIds[i] == filterId)
                {
                    return i;
                }
            }

            return -1;
        }

        //获取filterId对应的大页签数据
        List<ITwoLevelToggleData> _GetParentToggleData()
        {
            var activities = ActivityManager.GetInstance().GetAllActivities();
            List<ITwoLevelToggleData> list = new List<ITwoLevelToggleData>(activities.Count);
            foreach (var data in activities)
            {
                //这个页签没有活动就不加入
                if (data.Value.Count <= 0)
                {
                    continue;
                }
                ActivityTabInfo tabData = ActivityDataManager.GetInstance().GetActivityTabInfo(data.Key);
                if (tabData == null)
                {
                    Logger.LogError("在活动页签表中找不到id为" + data.Key + "的数据!");
                    continue;
                }

                string name = tabData.name;
                bool isShowRedPoint = false;
                bool isHaveActiveActivity = false;
                for (int i = 0; i < data.Value.Count; ++i)
                {
                    //结束的活动不算入 此处备注有问题
                    if (data.Value[i].GetState() == OpActivityState.OAS_IN || data.Value[i].GetState() == OpActivityState.OAS_PREPARE)
                    {
                        isHaveActiveActivity = true;
                        if (data.Value[i].IsHaveRedPoint())
                        {
                            isShowRedPoint = true;
                            break;
                        }
                    }
                }

                if (!isHaveActiveActivity)
                    continue;

                list.Add(new LimitTimeToggleData(name, isShowRedPoint, null));
            }


            return list;
        }

        //获取大页签下所有小页签数据
        List<List<ITwoLevelToggleData>> _InitSubToggleDatas()
        {
            var activities = ActivityManager.GetInstance().GetAllActivities();
            List<List<ITwoLevelToggleData>> datas = new List<List<ITwoLevelToggleData>>(activities.Count);
            int id = 0;
            foreach (var data in activities)
            {
                if (data.Value.Count <= 0)
                    continue;

                bool isInit = false;
                for (int i = 0; i < data.Value.Count; ++i)
                {
                    //只显示进行中的任务
                    if (data.Value[i].GetState() == OpActivityState.OAS_IN || data.Value[i].GetState() == OpActivityState.OAS_PREPARE)
                    {
                        if (!isInit)
                        {
                            datas.Add(new List<ITwoLevelToggleData>());
                            mActivityToggleData.Add(new List<uint>());
                            mToggleFilterIds.Add(data.Key);
                            isInit = true;
                        }

                        var activityId = (int)data.Value[i].GetId();
                        //获取活动对应的大页签
                        var filterId = ActivityDataManager.GetInstance().GetFilterIdByActivityId(activityId);
                        //获取活动对应的排序位置
                        var subId = ActivityManager.GetInstance().GetActivityOrderId(filterId, activityId);
                        var toggleData = new LimitTimeToggleData(data.Value[i]);
                        if (mActivityToggleData[id].Count == 0)
                        {
                            datas[id].Add(toggleData);
                            mActivityToggleData[id].Add(data.Value[i].GetId());
                        }
                        else
                        {
                            for (int j = 0; j < mActivityToggleData[id].Count; ++j)
                            {
                                var curSubId = ActivityManager.GetInstance().GetActivityOrderId(filterId, (int)mActivityToggleData[id][j]);
                                if (curSubId > subId)
                                {
                                    datas[id].Insert(j, toggleData);
                                    mActivityToggleData[id].Insert(j, data.Value[i].GetId());
                                    break;
                                }

                                if (j == mActivityToggleData[id].Count - 1)
                                {
                                    datas[id].Add(toggleData);
                                    mActivityToggleData[id].Add(data.Value[i].GetId());
                                    break;
                                }
                            }
                        }
                    }
                }

                if (isInit)
                {
                    id++;
                }
            }

            return datas;
        }

        //获取活动的父页签下标
        int _GetParentId(int activityId)
        {
            if (mActivityToggleData != null)
            {
                for (int i = 0; i < mActivityToggleData.Count; ++i)
                {
                    for (int j = 0; j < mActivityToggleData[i].Count; ++j)
                    {
                        if (mActivityToggleData[i][j] == activityId)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        //获取活动的子页签下标
        int _GetSubId(int parentId, int activityId)
        {
            if (parentId >= 0 && mActivityToggleData != null && parentId < mActivityToggleData.Count)
            {
                for (int j = 0; j < mActivityToggleData[parentId].Count; ++j)
                {
                    if (mActivityToggleData[parentId][j] == activityId)
                    {
                        return j;
                    }
                }
            }

            return -1;
        }

        #region ExtraUIBind
        private LimitTimeActivityView mView = null;

        protected override void _bindExUI()
        {
            mView = mBind.GetCom<LimitTimeActivityView>("LimitTimeActivityFrame");
        }

        protected override void _unbindExUI()
        {
            if (mView != null)
            {
                mView.Dispose();
            }
            mView = null;

        }
        #endregion
    }
}
