using System;
using System.Collections.Generic;
using ActivityLimitTime;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public interface IActivity : IDisposable
    {
        void Init(uint activityId);//初始化 

        void Show(Transform root);//显示界面

        void Hide();//隐藏

        void Close();//关闭界面

        void UpdateData();//更新活动数据

        void UpdateTask(int taskId);//更新任务数据

        bool IsHaveRedPoint();//是否有红点

        uint GetId();//获取活动id

        string GetName();//获取活动名(显示在页签上)

        OpActivityState GetState();//获取活动状态
    }

    public class LimitTimeToggleData : ITwoLevelToggleData
    {
        public string Name { get; private set; }
        public string SelectName { get; private set; }
        public bool IsShowRedPoint { get; private set; }
        public IActivity Activity { get; private set; }

        public LimitTimeToggleData(IActivity activity)
        {
            if (activity != null)
            {
                Name = activity.GetName();
                IsShowRedPoint = activity.IsHaveRedPoint();
                Activity = activity;
                SelectName = string.Format(TR.Value("activity_tab_selected"), Name);
            }
        }

        public LimitTimeToggleData(string name, bool isShowRedPoint, IActivity activity)
        {
            Name = name;
            IsShowRedPoint = isShowRedPoint;
            Activity = activity;
            SelectName = string.Format(TR.Value("activity_tab_selected"), name);
        }
    }

    //此类只用于LimitTimeActivityFrame 不储存实际数据 只负责管理frame内部的活动
    //数据都取自ActivityDataManager 由ActivityDataManager通知ActivityManager数据更新（页签更新，新填活动，活动关闭）
    //ActivityManager更新完后通知IActivity内部更新
    public sealed class ActivityManager : DataManager<ActivityManager>
    {
        //需要所有活动的id都是唯一的。key是活动id IActivity是活动页面相关（此处只包含限时活动界面的活动 其他界面的活动不在此处）
        readonly Dictionary<uint, IActivity> mActivities = new Dictionary<uint, IActivity>();
        //所有活动的二级管理，key是大页签id
        readonly Dictionary<int, List<IActivity>> mActivitiesByFilterId = new Dictionary<int, List<IActivity>>();

        VanityBuffBonusModel vanityBBModel = null;
      
        public VanityBuffBonusModel GetVanityBuffBonusModel()
        {
            return vanityBBModel;
        }

        //获取活动排序 filterId为大页签id activityId为活动id
        public int GetActivityOrderId(int filterId, int activityId)
        {
            int id = 0;
	        var tabList = ActivityDataManager.GetInstance().GetActivityTabInfo(filterId);
	        if (tabList != null)
	        {
		        for (int i = 0; i < tabList.actIds.Length; ++i)
		        {
			        if (!mActivities.ContainsKey(tabList.actIds[i]) || mActivities[tabList.actIds[i]].GetState() != OpActivityState.OAS_IN)
			        {
				        continue;
			        }

			        if (tabList.actIds[i] == activityId)
			        {
				        break;
			        }

			        id++;
		        }

		        return id;
	        }

            return 0;
        }

        public IActivity GetActivity(uint id)
        {
            if (mActivities.ContainsKey(id))
            {
                return mActivities[id];
            }

            return null;
        }

        //获取大页签对应的活动
        public Dictionary<int, List<IActivity>> GetAllActivities()
        {
            return mActivitiesByFilterId;
        }

        //是否有活动
        public bool IsHaveAnyActivity()
        {
            //活动功能没开放
            if (!Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.ActivityLimitTime))
            {
                return false;
            }

            foreach (var activity in mActivities.Values)
            {
                if (activity.GetState() == OpActivityState.OAS_IN)
                {
                    return true;
                }
            }

            return false;
        }

        //是否开启活动
        public bool IsActivityOpen(int id)
        {
            if(mActivities.ContainsKey((uint)id))
            {
                if (mActivities[(uint)id].GetState() != OpActivityState.OAS_IN)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        //获取是否显示红点
        public bool IsHaveAnyRedPoint()
        {
            foreach (var activity in mActivities.Values)
            {
                if (activity.GetState() == OpActivityState.OAS_IN && activity.IsHaveRedPoint())
                {
                    return true;
                }
            }

            return false;
        }

        //判断 大页签 下有无红点活动 filterId为大页签id
        public bool IsFilterHaveRedPoint(int filterId)
        {
            if (mActivitiesByFilterId.ContainsKey(filterId))
            {
                foreach (var activity in mActivitiesByFilterId[filterId])
                {
                    if (activity.GetState() == OpActivityState.OAS_IN && activity.IsHaveRedPoint())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //获取某活动是否显示红点
        public bool IsHaveRedPoint(uint activityId)
        {
            if (mActivities.ContainsKey(activityId))
            {
                return mActivities[activityId].IsHaveRedPoint() && mActivities[activityId].GetState() == OpActivityState.OAS_IN;
            }

            return false;
        }

        //展示activityview
        public void ShowActivity(uint activityId, Transform root)
        {
            if (!mActivities.ContainsKey(activityId))
            {
                _AddActivity(activityId);
            }

            if (mActivities.ContainsKey(activityId))
            {
                mActivities[activityId].Show(root);
            }
        }
        //隐藏actity
        public void HideActivity(uint activityId)
        {
            if (mActivities.ContainsKey(activityId))
            {
                mActivities[activityId].Hide();
            }
        }

        public override void Initialize()
        {
            _BindEvents();
        }

        public override void Clear()
        {
            _UnBindEvents();
            if (mActivities != null)
            {
                foreach (var activity in this.mActivities.Values)
                {
                    activity.Dispose();
                }

                mActivities.Clear();
            }

            if (mActivitiesByFilterId != null)
            {
                mActivitiesByFilterId.Clear();
            }

            vanityBBModel = null;
        }

        //请求限时活动的商城礼包（为啥不直接请求ActivityDataManager还要加个中间层..）
        public void RequestGiftDatas()
        {
	        ActivityDataManager.GetInstance().RequestMallGiftData();
        }

        void _BindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnActivityUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate, _OnActivityTaskUpdate);
	        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityTabsInfoUpdate, _OnTabsInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VanityBonusBuffPos, _onVanityBonusBuffPos);
        }

		void _UnBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnActivityUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskDataUpdate, _OnActivityTaskUpdate);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityTabsInfoUpdate, _OnTabsInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VanityBonusBuffPos, _onVanityBonusBuffPos);
        }

        //buff加成
        private void _onVanityBonusBuffPos(UIEvent ui)
        {
            Vector3 pos = (Vector3)ui.Param1;
            BuffTable table = (BuffTable)ui.Param2;

            vanityBBModel = new VanityBuffBonusModel();
            vanityBBModel.pos = pos;
            vanityBBModel.iconPath = table.Icon;
            vanityBBModel.des = table.Name;
        }

        //活动页签更新 重新刷一遍mActivitiesByFilterId
        void _OnTabsInfoUpdate(UIEvent uiEvent)
	    {
            //获取到页签表数据 页签表包含所有页签 所以不需要直接清除mActivitiesByFilterId 清除内部即可
		    var tabDatas = ActivityDataManager.GetInstance().GetTabInfos();
		    foreach (var data in tabDatas)
		    {
			    var tabData = data.Value as ActivityTabInfo;
			    if (tabData != null && !mActivitiesByFilterId.ContainsKey(data.Key))
			    {
				    mActivitiesByFilterId.Add(data.Key, new List<IActivity>());
			    }

			    mActivitiesByFilterId[data.Key].Clear();

				for (int i = 0; i < tabData.actIds.Length; ++i)
			    {
				    if (mActivities.ContainsKey(tabData.actIds[i]))
				    {
                        if (mActivitiesByFilterId[data.Key].Contains(mActivities[tabData.actIds[i]]))
                        {
                            continue;
                        }

                        //特殊处理，飞升礼包页签只有满足所有角色等级有一个为60级的才显示
                        if (!CheckFlyingGiftPackActivityTabIsShow(tabData.actIds[i]))
                        {
                            continue;
                        }
                        //特殊处理，消费返利页签当前角色大于40级的才显示
                        if (!CheckConsumeRebateActivityTabIsShow(tabData.actIds[i]))
                        {
                            continue;
                        }

                        //特殊处理，春节红包雨页签当前角色大于等于40级才显示
                        if (!CheckSpringFestivalRedEnvelopeRainActivityTabIsShow(tabData.actIds[i]))
                        {
                            continue;
                        }

                        mActivitiesByFilterId[data.Key].Add(mActivities[tabData.actIds[i]]);
				    }
			    }
		    }
	    }

		//活动更新
		void _OnActivityUpdate(UIEvent uiEvent)
        {
            uint activityId = (uint) uiEvent.Param1;
            IActivity activity = null;
            if (mActivities.ContainsKey(activityId))
            {
                activity = mActivities[activityId];
                mActivities[activityId].UpdateData();
                //活动结束 需要删除
                if (mActivities[activityId].GetState() == OpActivityState.OAS_END)
                {
                    mActivities[activityId].Close();
                    mActivities.Remove(activityId);
                }
            }
            else
            {
                //（如果add了一个活动结束的活动可能会出问题？
                _AddActivity(activityId);
                if (mActivities.ContainsKey(activityId))
                    activity = mActivities[activityId];
            }

            if (activity != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, activity);
            }
        }

        //活动任务更新
        void _OnActivityTaskUpdate(UIEvent uiEvent)
        {
            var data = (LimitTimeActivityTaskUpdateData) uiEvent.Param1;
            if (data == null)
            {
                return;
            }

            if (mActivities.ContainsKey((uint)data.ActivityId))
            {
                mActivities[(uint)data.ActivityId].UpdateTask(data.TaskId);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeTaskUpdate, data);
            }
        }

        //添加活动界面 在获取每个活动数据的时候都会add进来 活动数据都是根据ActivityDataManager获取
        void _AddActivity(uint activityId)
        {
            var activity = ActivityLimitTimeFactory.Create(activityId);
            if (activity != null)
            {
                mActivities.Add(activityId, activity);
                int filterId = ActivityDataManager.GetInstance().GetFilterIdByActivityId((int)activityId);
                if (filterId == -1)
                {
                    Logger.LogWarning("活动页签表找到对应的活动:" + activityId);
                    return;
                }

                //特殊处理，飞升礼包页签只有满足所有角色等级有一个为60级的才显示
                if (!CheckFlyingGiftPackActivityTabIsShow(activityId))
                {
                    return;
                }
                //特殊处理，消费返利页签当前角色等级大于40的才显示
                if (!CheckConsumeRebateActivityTabIsShow(activityId))
                {
                    return;
                }
                //特殊处理，春节红包雨页签当前角色等级大于40的才显示
                if (!CheckSpringFestivalRedEnvelopeRainActivityTabIsShow(activityId))
                {
                    return;
                }
                
                mActivitiesByFilterId[filterId].Add(activity);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, mActivities[activityId]);
            }
        }

        /// <summary>
        /// 检查飞升礼包页签是否显示
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool CheckFlyingGiftPackActivityTabIsShow(uint activityId)
        {
            int soarActivityId = 0;
            int.TryParse(TR.Value("soaring_activity_id"), out soarActivityId);

            if (activityId == soarActivityId)
            {
                return ClientApplication.playerinfo.CheckAllRoleLevelIsContainsParamLevel(60);
            }

            return true;
        }

        /// <summary>
        /// 消费返利活动页签是否显示
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool CheckConsumeRebateActivityTabIsShow(uint activityId)
        {
            if (activityId == 1540)
            {
                int limitLv = 0;
                int.TryParse(TR.Value("ConsumeRebateLimitPlayerGrade"), out limitLv);
                return PlayerBaseData.GetInstance().Level>= limitLv;
            }

            return true;
        }

        /// <summary>
        /// 春节红包雨页签是否显示
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool CheckSpringFestivalRedEnvelopeRainActivityTabIsShow(uint activityId)
        {
            if (activityId == 1585)
            {
                int limitLv = 0;
                int.TryParse(TR.Value("SpringFestivalRedEnvelopeRainLimitPlayerGrade"), out limitLv);

                return PlayerBaseData.GetInstance().Level >= limitLv;
            }

            return true;
        }

        /// <summary>
        /// 特殊处理，飞升礼包活动更新
        /// </summary>
        /// <param name="activityId"></param>
        public void UpdateFlyingGiftPackActivity(uint activityId)
        {
            if (mActivities.ContainsKey(activityId))
            {
                var activity = mActivities[activityId];

                int filterId = ActivityDataManager.GetInstance().GetFilterIdByActivityId((int)activityId);
                if (filterId == -1)
                {
                    Logger.LogWarning("活动页签表找到对应的活动:" + activityId);
                    return;
                }
                
                //特殊处理，飞升礼包页签只有满足所有角色等级有一个为60级的才显示
                if (!CheckFlyingGiftPackActivityTabIsShow(activityId))
                {
                    return;
                }

                mActivitiesByFilterId[filterId].Add(activity);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, mActivities[activityId]);
            }
        }

        /// <summary>
        /// 特殊处理，消费返利活动更新
        /// </summary>
        /// <param name="activityId"></param>
        public void UpdateConsumeRebateActivity(uint activityId)
        {
            if (mActivities.ContainsKey(activityId))
            {
                var activity = mActivities[activityId];

                int filterId = ActivityDataManager.GetInstance().GetFilterIdByActivityId((int)activityId);
                if (filterId == -1)
                {
                    Logger.LogWarning("活动页签表找到对应的活动:" + activityId);
                    return;
                }

                //特殊处理，充值返利活动当前角色大于40级的才显示
                if (!CheckConsumeRebateActivityTabIsShow(activityId))
                {
                    return;
                }

                mActivitiesByFilterId[filterId].Add(activity);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, mActivities[activityId]);
            }
        }

        /// <summary>
        /// 特殊处理，春节红包雨活动更新
        /// </summary>
        /// <param name="activityId"></param>
        public void UpdateSpringFestivalRedEnvelopeRainActivity(uint activityId)
        {
            if (mActivities.ContainsKey(activityId))
            {
                var activity = mActivities[activityId];

                int filterId = ActivityDataManager.GetInstance().GetFilterIdByActivityId((int)activityId);
                if (filterId == -1)
                {
                    Logger.LogWarning("活动页签表找到对应的活动:" + activityId);
                    return;
                }

                //特殊处理，春节红包雨活动当前角色大于40级的才显示
                if (!CheckSpringFestivalRedEnvelopeRainActivityTabIsShow(activityId))
                {
                    return;
                }

                mActivitiesByFilterId[filterId].Add(activity);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeUpdate, mActivities[activityId]);
            }
        }

        /// <summary>
        /// 获取虚空加成活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool GetVanityBonusActivityIsShow()
        {
            uint vanityBonusId = ActivityDataManager.GetInstance().GetActivityVanityBonusActivityId();
            IActivity activity = ActivityManager.GetInstance().GetActivity(vanityBonusId);
            if (activity == null)
            {
                return false;
            }

            if (activity.GetState() == OpActivityState.OAS_IN)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 混沌加成混沌是否开启
        /// </summary>
        /// <returns></returns>
        public bool GetChaosAdditionActivityIsShow()
        {
            uint chaosAdditionID = ActivityDataManager.GetInstance().GetActivityChaosAdditionID();
            IActivity activity = ActivityManager.GetInstance().GetActivity(chaosAdditionID);
            if (activity == null)
            {
                return false;
            }

            if (activity.GetState() == OpActivityState.OAS_IN)
            {
                return true;
            }
            return false;
        }

       
    }
}