/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace behaviac
{    public class Condition2 : ConditionBase
    {
        public override string GetDisplayName() { return "条件2"; }
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Opl")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opl = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_opl = AgentMeta.ParseMethod(p.value);
                    }
                }
                else if (p.name == "Operator")
                {
                    this.m_operator = OperationUtils.ParseOperatorType(p.value);
                }
                else if (p.name == "Opr")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opr = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_opr = AgentMeta.ParseMethod(p.value);
                    }
                }
                else if (p.name == "Time")
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
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Condition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public override bool Evaluate(Agent pAgent)
        {
            if (this.m_opl != null && this.m_opr != null)
            {
                return this.m_opl.Compare(pAgent, this.m_opr, this.m_operator);
            }
            else
            {
                EBTStatus childStatus = EBTStatus.BT_INVALID;
                EBTStatus result = this.update_impl(pAgent, childStatus);
                return result == EBTStatus.BT_SUCCESS;
            }
        }

        protected override BehaviorTask createTask()
        {
            ConditionTask pTask = new ConditionTask();

            return pTask;
        }

        protected virtual int GetIntTime(Agent pAgent)
        {
            int time = 0;

            if (this.m_time != null)
            {
                if (this.m_time is CInstanceMember<int>)
                {
                    time = ((CInstanceMember<int>)this.m_time).GetValue(pAgent);
                }
                else if (this.m_time is CInstanceMember<float>)
                {
                    time =(int) ((CInstanceMember<float>)this.m_time).GetValue(pAgent);
                }
            }

            return time;
        }


        protected IInstanceMember m_opl;
        protected IInstanceMember m_opr;
        protected EOperatorType m_operator = EOperatorType.E_EQUAL;

        protected IInstanceMember m_time;

        private class ConditionTask : ConditionBaseTask
        {
            private long m_intStart = 0;
            private int m_intTime = 0;

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
        

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                ConditionTask ttask = (ConditionTask)target;
                ttask.m_intStart = this.m_intStart;
                ttask.m_intTime = this.m_intTime;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_intStart = _getCurrentAgentTime(pAgent);
                this.m_intTime = this.GetIntTime(pAgent);
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            private int GetIntTime(Agent pAgent)
            {
                Condition2 pWaitNode = this.GetNode() as Condition2;

                return pWaitNode != null ? pWaitNode.GetIntTime(pAgent) : 0;
            }

            protected bool CheckTime(Agent pAgent)
            {
                if (_getCurrentAgentTime(pAgent) - this.m_intStart >= this.m_intTime)
                    return true;
                return false;
            } 
            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);

                Debug.Check(this.GetNode() is Condition);
                Condition2 pConditionNode = (Condition2)(this.GetNode());

                bool ret = pConditionNode.Evaluate(pAgent);

                if (this.m_intTime == 0)
                {
                    return ret ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
                }
                else if (this.m_intTime == -1)
                {
                    return ret ? EBTStatus.BT_SUCCESS : EBTStatus.BT_RUNNING;
                }
                else if (this.m_intTime > 0)
                {
                    if (ret)
                        return EBTStatus.BT_SUCCESS;
                    
                    return CheckTime(pAgent)?EBTStatus.BT_SUCCESS : EBTStatus.BT_RUNNING;
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}
