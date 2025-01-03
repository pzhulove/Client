using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Generic;

namespace behaviac
{
    public class WaitforMultipleSignal : BehaviorNode
    {
        public override string GetDisplayName() { return "多信号等待"; }
        protected IInstanceMember m_time;
        protected IInstanceMember mTriggeredSignalIndex;
        protected string mTriggeredSignalIndexParamName = string.Empty;

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Time")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_time = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_time = AgentMeta.ParseMethod(p.value);
                    }
                }
                else if (p.name == "Opl")
                {
                    this.mTriggeredSignalIndex = AgentMeta.ParseProperty(p.value);
                }
            }
        }

        protected override BehaviorTask createTask()
        {
            return new WaitforMultipleSignalTask();
        }

        protected virtual double GetTime(Agent pAgent)
        {
            double time = 0;

            if (this.m_time != null)
            {
                if (this.m_time is CInstanceMember<double>)
                {
                    time = ((CInstanceMember<double>)this.m_time).GetValue(pAgent);
                }
                else if (this.m_time is CInstanceMember<float>)
                {
                    time = ((CInstanceMember<float>)this.m_time).GetValue(pAgent);
                }
                else if (this.m_time is CInstanceMember<int>)
                {
                    time = ((CInstanceMember<int>)this.m_time).GetValue(pAgent);
                }
            }

            return time;
        }

        public class WaitforMultipleSignalTask : CompositeTask
        {
            private double m_start = 0;
            private double m_time = 0;

            public WaitforMultipleSignalTask()
            {

            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is WaitforMultipleSignalTask);
                WaitforMultipleSignalTask ttask = (WaitforMultipleSignalTask)target;

                ttask.m_start = this.m_start;
                ttask.m_time = this.m_time;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID startId = new CSerializationID("start");
                node.setAttr(startId, this.m_start);

                CSerializationID timeId = new CSerializationID("time");
                node.setAttr(timeId, this.m_time);
            }

            private double GetTime(Agent pAgent)
            {
                WaitforMultipleSignal pWaitNode = this.GetNode() as WaitforMultipleSignal;

                return pWaitNode != null ? pWaitNode.GetTime(pAgent) : 0;
            }

            protected long _getCurrentAgentTime(Agent pAgent)
            {
                long t = 0;
                if ((pAgent as BTAgent) != null)
                {
                    t = (pAgent as BTAgent).CurrentTime;
                }
                else if ((pAgent as LevelAgent) != null)
                {
                    t = (pAgent as LevelAgent).CurrentTime;
                }

                return t;
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_start = _getCurrentAgentTime(pAgent);
                this.m_time = this.GetTime(pAgent);

                var node = (WaitforMultipleSignal)GetNode();
                if (node.mTriggeredSignalIndex == null && node.mTriggeredSignalIndexParamName.Length > 0)
                {
                    node.mTriggeredSignalIndex = AgentMeta.ParseProperty(node.mTriggeredSignalIndexParamName);
                }

                return true;
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                if (childStatus == EBTStatus.BT_RUNNING)
                {
                    var node = (WaitforMultipleSignal)GetNode();

                    if (m_time >= 0 && _getCurrentAgentTime(pAgent) - m_start >= m_time)
                    {
                        if (node.mTriggeredSignalIndex != null)
                        {
                            node.mTriggeredSignalIndex.SetValue(pAgent, AgentMeta.ParseProperty("const int 0"));
                        }
                        return EBTStatus.BT_SUCCESS;
                    }

                    for (int i = 0; i < m_children.Count; i++)
                    {
                        var child = m_children[i];

                        var status = child.GetStatus();
                        //if (status == EBTStatus.BT_INVALID || status == EBTStatus.BT_RUNNING)
                        {
                            status = child.exec(pAgent);
                        }
                        if (status == EBTStatus.BT_SUCCESS)
                        {
                            if (node.mTriggeredSignalIndex != null)
                            {
                                node.mTriggeredSignalIndex.SetValue(pAgent, AgentMeta.ParseProperty("const int " + (i + 1).ToString()));
                            }
                            return status;
                        }
                    }
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}
