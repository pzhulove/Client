using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class IntervalConfig
    {
        public object target;
        public int iId;
        public ComFunctionInterval comInterval;
    }

    class ComIntervalGroup : Singleton<ComIntervalGroup>
    {
        List<IntervalConfig> m_akIntervalConfigs = new List<IntervalConfig>();

        public void Register(object target,int iId,ComFunctionInterval comInterval)
        {
            var find = m_akIntervalConfigs.Find(x =>
            {
                return x.target == target &&
                x.iId == iId &&
                x.comInterval == comInterval;
            });

            if(find == null)
            {
                m_akIntervalConfigs.Add(new IntervalConfig
                {
                    target = target,
                    iId = iId,
                    comInterval = comInterval,
                });
            }
        }

        public void UnRegister(object target)
        {
            m_akIntervalConfigs.RemoveAll(x =>
            {
                return x.target == target;
            });
        }

        public void BeginInvoke(object target,int iId,float fLastTime)
        {
            m_akIntervalConfigs.ForEach((x) =>
            {
                if(x.target == target && x.iId == iId && x.comInterval != null)
                {
                    x.comInterval.BeginInvoke(fLastTime);
                }
            });
        }

        public void EnableFunction(object target, int iId)
        {
            m_akIntervalConfigs.ForEach((x) =>
            {
                if (x.target == target && x.iId == iId && x.comInterval != null)
                {
                    x.comInterval.EnableFunction();
                }
            });
        }

        public void DisableFunction(object target, int iId)
        {
            m_akIntervalConfigs.ForEach((x) =>
            {
                if (x.target == target && x.iId == iId && x.comInterval != null)
                {
                    x.comInterval.DisableFunction();
                }
            });
        }
    }
}