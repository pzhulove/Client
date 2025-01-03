using System;
using System.Collections.Generic;
using behaviac;
using FlatBuffers;

namespace BehaviorTreeMechanism
{
    public class BTMechanism : BeMechanism
    {
        private MechanismAgent m_Agent;
        
        public BTMechanism(int mid, int lv) : base(mid, lv)
        {
        }

        public override void OnInit()
        {
            Clear();
            if (!string.IsNullOrEmpty(data.BTPath))
            {
                m_Agent = new MechanismAgent();
                m_Agent.Init(data.BTPath, this);    
            }
            base.OnInit();
        }

        public override void OnReset()
        {
            base.OnReset();
            if (m_Agent != null)
            {
                m_Agent.Clear();
                m_Agent = null;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            if(m_Agent != null)
                m_Agent.DoAction(MechanismAgent.BTM_ActionType.Start);
        }

        public override void OnUpdate(int deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (m_Agent != null)
            {
                m_Agent.Update(deltaTime);
                m_Agent.DoAction(MechanismAgent.BTM_ActionType.Update);
            }
        }

        public override void OnFinish()
        {
            base.OnFinish();
            if(m_Agent != null)
                m_Agent.DoAction(MechanismAgent.BTM_ActionType.Finish);
            
            Clear();
        }

        public int GetArgs(behaviac.BTM_ArgsType arg)
        {
            switch (arg)
            {
                case BTM_ArgsType.ArgA:
                    return TableManager.GetValueFromUnionCell(data.ValueA[0], level);
                case BTM_ArgsType.ArgB:
                    return TableManager.GetValueFromUnionCell(data.ValueB[0], level);
                case BTM_ArgsType.ArgC:
                    return TableManager.GetValueFromUnionCell(data.ValueC[0], level);
                case BTM_ArgsType.ArgD:
                    return TableManager.GetValueFromUnionCell(data.ValueD[0], level);
                case BTM_ArgsType.ArgE:
                    return TableManager.GetValueFromUnionCell(data.ValueE[0], level);
                case BTM_ArgsType.ArgF:
                    return TableManager.GetValueFromUnionCell(data.ValueF[0], level);
                case BTM_ArgsType.ArgG:
                    return TableManager.GetValueFromUnionCell(data.ValueG[0], level);
                case BTM_ArgsType.ArgH:
                    return TableManager.GetValueFromUnionCell(data.ValueH[0], level);
                default:
                    Logger.LogErrorFormat("使用了未实现的类型，请添加");
                    break;
            }

            return 0;
        }

        public List<int> GetArgsArray(behaviac.BTM_ArgsType arg)
        {
            List<int> ret = new List<int>(); 
            switch (arg)
            {
                case BTM_ArgsType.ArgA:
                    for (int i = 0; i < data.ValueALength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
                    }
                    break;
                case BTM_ArgsType.ArgB:
                    for (int i = 0; i < data.ValueBLength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
                    }
                    break;
                case BTM_ArgsType.ArgC:
                    for (int i = 0; i < data.ValueCLength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
                    }
                    break;
                case BTM_ArgsType.ArgD:
                    for (int i = 0; i < data.ValueDLength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
                    }
                    break;
                case BTM_ArgsType.ArgE:
                    for (int i = 0; i < data.ValueELength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueE[i], level));
                    }
                    break;
                case BTM_ArgsType.ArgF:
                    for (int i = 0; i < data.ValueFLength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueF[i], level));
                    }
                    break;
                case BTM_ArgsType.ArgG:
                    for (int i = 0; i < data.ValueGLength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueG[i], level));
                    }
                    break;
                case BTM_ArgsType.ArgH:
                    for (int i = 0; i < data.ValueHLength; i++)
                    {
                        ret.Add(TableManager.GetValueFromUnionCell(data.ValueH[i], level));
                    }
                    break;
                default:
                    Logger.LogErrorFormat("使用了未实现的类型，请添加");
                    break;
            }

            return ret;
        }

        public string GetArgsStr(behaviac.BTM_StrArgsType arg)
        {
            switch (arg)
            {
                case BTM_StrArgsType.StrArgA:
                    return data.StringValueA[0];
                default:
                    Logger.LogErrorFormat("使用了未实现的类型，请添加");
                    break;
            }

            return string.Empty;
        }
        
    public List<string> GetArgsStrArray(behaviac.BTM_StrArgsType arg)
    {
        List<string> ret = new List<string>(); 
        switch (arg)
        {
            case BTM_StrArgsType.StrArgA:
                for (int i = 0; i < data.StringValueALength; i++)
                {
                    ret.Add(data.StringValueA[i]);
                }
                break;
            default:
                Logger.LogErrorFormat("使用了未实现的类型，请添加");
                break;
        }
        return ret;
    }
    
        protected void Clear()
        {
            if (m_Agent != null)
            {
                m_Agent.Clear();
                m_Agent = null;
            }
        }
    }
}
