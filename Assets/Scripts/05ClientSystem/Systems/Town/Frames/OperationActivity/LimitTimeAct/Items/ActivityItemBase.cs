using UnityEngine;

namespace GameClient
{
    public abstract class ActivityItemBase : MonoBehaviour, IActivityCommonItem
    {
        public delegate void OnActivityItemClick<T>(int id, T t1,ulong t2 = 0);

        protected uint mId;
        protected OnActivityItemClick<int> mOnItemClick;
	    protected uint mActivityId;
        public virtual void Init(uint id, uint activityId, ILimitTimeActivityTaskDataModel data, OnActivityItemClick<int> onItemClick)
        {
            if (data == null)
            {
                Logger.LogError("data is empty");
                return;
            }

	        mActivityId = activityId;
			mId = id;
            mOnItemClick = onItemClick;
            OnInit(data);
            UpdateData(data);
        }

        public virtual void InitFromMode(ILimitTimeActivityModel data, OnActivityItemClick<int> onItemClick)
        {
            //部分活动，是n个task对应一个item就用这个初始化
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public abstract void UpdateData(ILimitTimeActivityTaskDataModel data);

        public virtual void Dispose()
        {
            mOnItemClick = null;
        }

        public virtual void Destroy()
        {
            Dispose();
            Destroy(gameObject);
        }

        protected virtual void _OnItemClick()
        {
            if (mOnItemClick != null)
            {
                mOnItemClick((int)mId, 0,0);
            }
        }
        /// <summary>
        /// 注册活动账号请求的回调
        /// </summary>
        protected virtual void RegisterAccountData(ClientEventSystem.UIEventHandler eventHandler)
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, eventHandler);
        }

        /// <summary>
        ///注销活动账号请求的回调
        /// </summary>
        protected virtual void UnRegisterAccountData(ClientEventSystem.UIEventHandler eventHandler)
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivtiyLimitTimeAccounterNumUpdate, eventHandler);
        }
        /// <summary>
        /// 请求活动账号数据 (先注册请求的回调)
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnRequsetAccountData(ILimitTimeActivityTaskDataModel data)
        {
            if (data == null) return;
            if (data.AccountDailySubmitLimit > 0)
            {
                ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)data.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK);
            }
            if (data.AccountTotalSubmitLimit > 0)
            {
                ActivityDataManager.GetInstance().SendSceneOpActivityGetCounterReq((int)data.DataId, ActivityLimitTimeFactory.EActivityCounterType.OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK);
            }
        }

        
        protected abstract void OnInit(ILimitTimeActivityTaskDataModel data);
    }
}
