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
{
    public class DecoratorFailureUntil : DecoratorCount
    {
        public override string GetDisplayName() { return "返回失败直到"; }
        public DecoratorFailureUntil()
        {
        }

        //~DecoratorFailureUntil()
        //{
        //}

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorFailureUntil))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorFailureUntilTask pTask = new DecoratorFailureUntilTask();

            return pTask;
        }

        ///Returns EBTStatus.BT_FAILURE for the specified number of iterations, then returns EBTStatus.BT_SUCCESS after that
        private class DecoratorFailureUntilTask : DecoratorCountTask
        {
            public DecoratorFailureUntilTask()
            : base()
            {
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            public override void onreset(Agent pAgent)
            {
                this.m_n = 0;
            }

            protected override bool onenter(Agent pAgent)
            {
                //base.onenter(pAgent);

                if (this.m_n == 0)
                {
                    int count = this.GetCount(pAgent);

                    if (count == 0)
                    {
                        return false;
                    }

                    this.m_n = count;
                }
                else
                {
                    Debug.Check(true);
                }

                return true;
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (this.m_n > 0)
                {
                    this.m_n--;

                    if (this.m_n == 0)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }

                    return EBTStatus.BT_FAILURE;
                }

                if (this.m_n == -1)
                {
                    return EBTStatus.BT_FAILURE;
                }

                Debug.Check(this.m_n == 0);

                return EBTStatus.BT_SUCCESS;
            }
        }
    }
}
