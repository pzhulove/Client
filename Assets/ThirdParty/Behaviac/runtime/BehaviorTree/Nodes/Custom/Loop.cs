    using System.Collections.Generic;
    using behaviac;

    public class Loop : BehaviorNode
    {
        public override string GetDisplayName()
        {
            return "循环执行";
        }

        protected override BehaviorTask createTask()
        {
            LoopTask pTask = new LoopTask();
            return pTask;
        }
        
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Array")
                {
                    this.m_array = AgentMeta.ParseProperty(p.value);
                }
            }
        }

        protected virtual List<int> GetArray(Agent pAgent)
        {
            if (this.m_array != null)
            {
                Debug.Check(this.m_array is CInstanceMember<List<int>>);
                var array = ((CInstanceMember<List<int>>)this.m_array).GetValue(pAgent);

                return array;
            }

            return null;
        }

        protected IInstanceMember m_array;
        
        private class LoopTask : SingeChildTask
        {
            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);

                var array = this.GetArray(pAgent);

                this.m_array = array;

                return true;
            }
            
            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);
                AgentBase agent = pAgent as AgentBase;
                Debug.Check(agent != null);
                Debug.Check(this.GetNode() is Loop, "node is not an Loop");

                EBTStatus retStatus = EBTStatus.BT_SUCCESS;
                if (m_array != null && m_array.Count > 0)
                {
                    int cacheArrayIndex = agent.ArrayIndex;
                    int cacheArrayValue = agent.ArrayValue;
                    for (int i = 0; i < m_array.Count; i++)
                    {
                        agent.ArrayIndex = i;
                        agent.ArrayValue = m_array[i];
                        this.m_root.exec(pAgent, childStatus);
                    }
                    agent.ArrayIndex = cacheArrayIndex;
                    agent.ArrayValue = cacheArrayValue;
                }
                return retStatus;
            }
            
            public List<int> GetArray(Agent pAgent)
            {
                Debug.Check(this.GetNode() is Loop);
                Loop loop = (Loop)(this.GetNode());

                return loop != null ? loop.GetArray(pAgent) : null;
            }
            
            protected List<int> m_array;
        }
    }
