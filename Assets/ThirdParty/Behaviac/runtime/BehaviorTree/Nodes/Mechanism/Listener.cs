using System.Collections.Generic;

namespace behaviac
{
    public class Listener : Action
    {
        public override string GetDisplayName() { return "监听"; }
        protected override BehaviorTask createTask()
        {
            ListenerTask pTask = new ListenerTask();

            return pTask;
        }
        
        private class ListenerTask : SingeChildTask
        {
            protected override bool onenter(Agent pAgent)
            {
                if (this.m_root == null)
                    return false;
                 
                return true;
            }
            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);
                MechanismAgent mechanismAgent = pAgent as MechanismAgent;
                Debug.Check(mechanismAgent != null);
                Debug.Check(m_root != null);
                Debug.Check(this.GetNode() is Listener, "node is not an Listener");
                Listener pActionNode = (Listener)(this.GetNode());
                if (mechanismAgent != null) 
                    mechanismAgent.SetEventTree(m_root);
                EBTStatus result = pActionNode.Execute(pAgent, childStatus);
                return result;
            }
        }
    }
}
