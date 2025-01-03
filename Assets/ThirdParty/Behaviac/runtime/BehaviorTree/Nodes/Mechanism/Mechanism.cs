using System.Collections.Generic;

namespace behaviac
{
    public class Mechanism : BehaviorNode
    {
        public Mechanism()
        {
        }

        public override string GetDisplayName() { return "机制"; }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Mechanism))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }
        
        protected override BehaviorTask createTask()
        {
            MechanismTask pTask = new MechanismTask();

            return pTask;
        }

        private class MechanismTask : CompositeTask
        {
            protected override bool CheckPreconditions(Agent pAgent, bool bIsAlive)
            {
                // Tip: 机制节点。只在Start时才会执行前置附件
                MechanismAgent mAgent = pAgent as MechanismAgent;
                Debug.Check(mAgent != null);
                bool alive = bIsAlive;
                if (mAgent != null)
                {
                    var type = mAgent.CurActionType();
                    if (type != MechanismAgent.BTM_ActionType.Start)
                        alive = true;
                }

                return base.CheckPreconditions(pAgent, alive);
            }
            
             protected override bool onenter(Agent pAgent)
             {
                 this.m_activeChildIndex = CompositeTask.InvalidChildIndex;
                 if (this.m_children.Count != 3)
                     return false;
                 
                 return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus != EBTStatus.BT_INVALID);
                Debug.Check(this.m_children.Count == 3);
                MechanismAgent mAgent = pAgent as MechanismAgent;
                Debug.Check(mAgent != null);
                

                if (mAgent != null)
                {
                    var type = mAgent.CurActionType();
                    if (type == MechanismAgent.BTM_ActionType.Start)
                    {
                        m_activeChildIndex = 0;
                    }
                    else if (type == MechanismAgent.BTM_ActionType.Update)
                    {
                        m_activeChildIndex = 1;
                    }
                    else if (type == MechanismAgent.BTM_ActionType.Finish)
                    {
                        m_activeChildIndex = 2;
                    }
                }

                return m_children[m_activeChildIndex].exec(pAgent);
            }
        }
    }
}