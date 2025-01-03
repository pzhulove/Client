using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 排队打开队列的 触发结果类型
    /// </summary>
    public enum FollowingOpenTriggerType
    {
        Normal = 0,         //正常打开
        Excepetion,         //异常情况
    }

    public class FollowingOpenTrigger
    {
        public FollowingOpenTriggerType triggerType;
    }

    /// <summary>
    /// 一揽子需要排队打开的 队列顺序
    /// </summary>
    public enum FollowingOpenQueueOrder
    {
        None = 0,

        TryToOpenActiveWelfareFrame,                                //尝试打开福利界面

        TopUpPushFrame_FirstOpenEcheDay,                            //首次登陆关闭福利界面 打开充值推送界面

        FuncUnlock_MainTown_AdventureTeam,                          //功能解锁提示-佣兵团
        FuncUnlock_MainTown_AdventurePassCard,                     //功能解锁提示-冒险通行证

        Reconnect,                                                  //登录重连        

        WarriorRecruitPush,                                         //勇士招募推送

        Count,
    }

    public class FollowingOpenQueueManager : DataManager<FollowingOpenQueueManager>
    {
        public delegate void FollowingOpenHandler(FollowingOpenTrigger trigger);

        private Queue<FollowingOpenQueueOrder> m_FollowingOpenQueue = new Queue<FollowingOpenQueueOrder>();
        private Dictionary<int, FollowingOpenHandler> m_FollowingOpenHandlers = new Dictionary<int, FollowingOpenHandler>();

        FollowingOpenTriggerType m_OpenTriggerType = FollowingOpenTriggerType.Excepetion;
        FollowingOpenTrigger m_OpenTrigger = new FollowingOpenTrigger();

        FollowingOpenQueueOrder m_OpenQueueOrder = FollowingOpenQueueOrder.None;

        public sealed override void Initialize()
        {
            Clear();

            _InitFollowingOpenQueue();
            _RegisterAllExtendMethods();
        }

        public sealed override void Clear()
        {
            if (m_FollowingOpenQueue != null)
            {
                m_FollowingOpenQueue.Clear();
            }

            if (m_FollowingOpenHandlers != null && m_FollowingOpenHandlers.Count > 0)
            {
                for (int i = 0; i < (int)FollowingOpenQueueOrder.Count; i++)
                {
                    FollowingOpenHandler handler;
                    if (!m_FollowingOpenHandlers.TryGetValue(i, out handler))
                    {
                        continue;
                    }
                    if(handler == null)
                    {
                        continue;
                    }
                    var invocations = handler.GetInvocationList();
                    if (invocations == null || invocations.Length <= 0)
                    {
                        continue;
                    }
                    for (int j = 0; j < invocations.Length; j++)
                    {
                        handler -= invocations[j] as FollowingOpenHandler;
                    }
                    handler = null;
                }
                m_FollowingOpenHandlers.Clear();
            }

            m_OpenTriggerType = FollowingOpenTriggerType.Excepetion;
            m_OpenQueueOrder = FollowingOpenQueueOrder.None;
        }

        private void _InitFollowingOpenQueue()
        {           
            int followingFrameCount = (int)FollowingOpenQueueOrder.Count;
            for(int i = 1; i < followingFrameCount; i++)
            {
                m_FollowingOpenQueue.Enqueue((FollowingOpenQueueOrder)i);           
            }
        }

        private bool _IsFollowingOpenQueueEmpty()
        {
            if (m_FollowingOpenQueue == null || m_FollowingOpenQueue.Count == 0)
            {
                return true;
            }
            return false;
        }

        private FollowingOpenQueueOrder _RemoveCurrentFollowingOpenOrder()
        {
            if(_IsFollowingOpenQueueEmpty())
            {
                return FollowingOpenQueueOrder.None;
            }
            return m_FollowingOpenQueue.Dequeue();
        }

        private void _OpenFollowingQueueHandler()
        {
            m_OpenTriggerType = FollowingOpenTriggerType.Excepetion;
            if (m_OpenTrigger == null)
            {
                m_OpenTrigger = new FollowingOpenTrigger();
            }
            m_OpenTrigger.triggerType = m_OpenTriggerType;

            m_OpenQueueOrder = _RemoveCurrentFollowingOpenOrder();

            if (m_FollowingOpenHandlers != null && m_FollowingOpenHandlers.ContainsKey((int)m_OpenQueueOrder))
            {
                var handler = m_FollowingOpenHandlers[(int)m_OpenQueueOrder];
                if (handler != null && m_OpenTrigger != null)
                {
                    handler(m_OpenTrigger);
                }
            }

            //未成功打开流程
            if (m_OpenTrigger != null && m_OpenTrigger.triggerType == FollowingOpenTriggerType.Excepetion)
            {
                StartOpenFollowingQueue();
            }
        }

        /// <summary>
        /// 注册打开队列的具体打开方法
        /// </summary>
        /// <param name="order">打开的类型</param>
        /// <param name="handler">具体方法</param>
        private void _RegisterFollowingOpenQueueHandler(FollowingOpenQueueOrder order, FollowingOpenHandler handler)
        {
            if (m_FollowingOpenHandlers == null)
            {
                return;
            }
            if (m_FollowingOpenHandlers.ContainsKey((int)order))
            {
                m_FollowingOpenHandlers[(int)order] += handler;
            }
            else
            {
                m_FollowingOpenHandlers.Add((int)order, new FollowingOpenHandler(handler));
            }
        }

        /// <summary>
        /// 开始执行队列队首的方法
        /// </summary>
        public void StartOpenFollowingQueue()
        {
            if(_IsFollowingOpenQueueEmpty())
            {
                return;        
            }
            _OpenFollowingQueueHandler();
        }

        /// <summary>
        /// 通知当前打开队列关闭
        /// </summary>
        /// <param name="frameType"></param>
        public void NotifyCurrentOrderClosed()
        {
            //队列序号为None 表示 还没开始打开队列
            if (m_OpenQueueOrder == FollowingOpenQueueOrder.None ||
                m_OpenQueueOrder == FollowingOpenQueueOrder.Count)
            {
                return;
            }
            StartOpenFollowingQueue();
        }


        #region Extend Methods

        //注册全部的扩展方法

        private void _RegisterAllExtendMethods()
        {
            _RegisterFollowingOpenQueueHandler(FollowingOpenQueueOrder.TopUpPushFrame_FirstOpenEcheDay, OpenTopUpPushFrame);
            _RegisterFollowingOpenQueueHandler(FollowingOpenQueueOrder.FuncUnlock_MainTown_AdventureTeam, _PlayAdventureMainTownUnlockAnim);
            _RegisterFollowingOpenQueueHandler(FollowingOpenQueueOrder.FuncUnlock_MainTown_AdventurePassCard, _PlayAdventurePassCardMainTownUnlockAnim);
            _RegisterFollowingOpenQueueHandler(FollowingOpenQueueOrder.TryToOpenActiveWelfareFrame, _TryToOpenActiveWelfareFrame);
            _RegisterFollowingOpenQueueHandler(FollowingOpenQueueOrder.Reconnect, TryOpenReconnectSceneTip);
            _RegisterFollowingOpenQueueHandler(FollowingOpenQueueOrder.WarriorRecruitPush, TryOpenWarriorRecruitPushFrame);      
        }

        /// <summary>
        /// 首次 城镇解锁佣兵团时的解锁飞入动画 
        /// </summary>
        /// <param name="type"></param>
        private void _PlayAdventureMainTownUnlockAnim(FollowingOpenTrigger trigger)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NotifyShowAdventureTeamUnlockAnim, trigger);
        }

        private void _PlayAdventurePassCardMainTownUnlockAnim(FollowingOpenTrigger trigger)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NotifyShowAdventurePassSeasonUnlockAnim, trigger);               
        }
        /// <summary>
        /// 首次登陆关闭福利界面 打开充值推送界面
        /// </summary>
        private void OpenTopUpPushFrame(FollowingOpenTrigger trigger)
        {
            if (ClientSystemManager.GetInstance().PreSystemType != typeof(ClientSystemLogin))
            {
                return;
            }

            if (AdsPush.LoginPushManager.GetInstance().IsFirstLogin())
            {
                var mData = TopUpPushDataManager.GetInstance().GetTopUpPushDataModel();
                if (mData != null && mData.mItems.Count > 0 && TopUpPushDataManager.GetInstance().CheckFirstLoginIsPush())
                {
                    if (!TopUpPushDataManager.GetInstance().LoginTopUpPushIsOpen)
                    {
                        var frame = ClientSystemManager.GetInstance().OpenFrame<TopUpPushFrame>(FrameLayer.Middle);
                        if (frame != null && trigger != null)
                        {
                            trigger.triggerType = FollowingOpenTriggerType.Normal;
                        }
                        TopUpPushDataManager.GetInstance().LoginTopUpPushIsOpen = true;
                    }
                }
            }
        }

        private void _TryToOpenActiveWelfareFrame(FollowingOpenTrigger trigger)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NotifyOpenWelfareFrame, trigger);
        }

        #region ReconnectSceneTip
        private void TryOpenReconnectSceneTip(FollowingOpenTrigger trigger)
        {
            //只有从登录界面计入才判断
            if (ClientSystemManager.GetInstance().PreSystemType != typeof(ClientSystemLogin))
            {
                return;
            }

            //重连进入到团本提示
            if (TeamDuplicationUtility.IsNeedReconnectToTeamDuplicationScene() == false)
                return;

            TeamDuplicationUtility.OpenReconnectToTeamDuplicationSceneTip(OnReconnectToTeamDuplicationSceneAction,
                OnReconnectToDuplicationSceneCancelAction);

            //标志重置
            trigger.triggerType = FollowingOpenTriggerType.Normal;
        }

        private void OnReconnectToTeamDuplicationSceneAction()
        {
            TeamDuplicationUtility.ReconnectToTeamDuplicationScene();
            NotifyCurrentOrderClosed();
        }

        private void OnReconnectToDuplicationSceneCancelAction()
        {
            NotifyCurrentOrderClosed();
        }
        #endregion

        private void TryOpenWarriorRecruitPushFrame(FollowingOpenTrigger trigger)
        {
            //只有从登录界面计入才判断
            if (ClientSystemManager.GetInstance().PreSystemType != typeof(ClientSystemLogin))
            {
                return;
            }

            int targetLv = 0;
            int.TryParse(TR.Value("RecruitmentPush_Lv"), out targetLv);
            if (AdsPush.LoginPushManager.GetInstance().IsFirstLogin())
            {
                if (PlayerBaseData.GetInstance().Level >= targetLv &&
                    AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_COUNTER_HIRE_PUS) <= 0 &&
                    WarriorRecruitDataManager.GetInstance().CheckWarriorRecruitActiveIsOpen())
                {
                    ClientSystemManager.GetInstance().OpenFrame<WarriorRecruitPushFrame>(FrameLayer.Middle);
                    //标志重置
                    trigger.triggerType = FollowingOpenTriggerType.Normal;
                }
            }
        }

        #endregion
    }
}