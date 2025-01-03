using behaviac;
using GameClient;

namespace BehaviorTreeMechanism
{
    /// <summary>
    /// 事件注册器 2种
    /// </summary>
    public interface IBTEventTrigger
    {
        void RegisterEvent(MechanismAgent agent, MechanismAgent.EventCallback cb);
        void Clear();
    }


    public class BTEventTrigger : IBTEventTrigger
    {
        private MechanismAgent.EventCallback m_EventCallBack;
        private BTEventDataParser m_Parser;
        private BeEventType m_EventType;
        private IBeEventHandle m_Handle;
        private BeEntity m_Entity;

        public BTEventTrigger(BeEntity entity, BeEventType eventType, BTEventDataParser parser)
        {
            m_Entity = entity;
            m_EventType = eventType;
            m_Parser = parser;
        }

        public void RegisterEvent(MechanismAgent agent, MechanismAgent.EventCallback cb)
        {
            if (agent == null)
                return;
            if (m_Entity == null)
                return;

            m_EventCallBack = cb;
            m_Handle = m_Entity.RegisterEventNew(m_EventType, OnTrigger);
        }

        private void OnTrigger(BeEvent.BeEventParam param)
        {
            if (m_EventCallBack != null)
            {
                m_Parser.Full(param);
                m_EventCallBack(param.m_SenderId, m_Parser);
            }
        }

        public void Clear()
        {
            if (m_Handle != null)
            {
                m_Handle.Remove();
                m_Handle = null;
            }
        }
    }

    public class BTSceneEventTrigger : IBTEventTrigger
    {
        private MechanismAgent.EventCallback m_EventCallBack;
        private BTEventDataParser m_Parser;
        private BeEventSceneType m_EventType;
        private BeEvent.BeEventHandleNew m_Handle;
        private BeScene m_Scene;

        public BTSceneEventTrigger(BeEventSceneType eventType, BTEventDataParser parser)
        {
            m_EventType = eventType;
            m_Parser = parser;
        }

        public void RegisterEvent(MechanismAgent agent, MechanismAgent.EventCallback cb)
        {
            if (agent == null)
                return;

            m_Scene = agent.GetBeScene();
            if (m_Scene == null)
                return;

            m_EventCallBack = cb;
            m_Handle = m_Scene.RegisterEventNew(m_EventType, OnTrigger);
        }

        private void OnTrigger(BeEvent.BeEventParam param)
        {
            if (m_EventCallBack != null)
            {
                m_Parser.Full(param);
                m_EventCallBack(0, m_Parser);
            }
        }

        public void Clear()
        {
            if (m_Handle != null)
            {
                if (m_Scene != null)
                {
                    m_Scene.RemoveEventNew(m_Handle);
                }

                m_Handle = null;
            }
        }
    }
}