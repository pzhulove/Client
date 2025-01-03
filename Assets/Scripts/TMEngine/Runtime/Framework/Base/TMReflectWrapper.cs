

using System;

namespace Tenmove.Runtime
{
    public abstract class ReflectWrapper<TBase>
    {
        protected readonly TBase m_TargetInstance;
        protected readonly Type m_TargetType;

        public ReflectWrapper(TBase instance)
        {
            Debugger.Assert(null != instance, "Parameter 'instance' can not be null!");

            m_TargetInstance = instance;
            m_TargetType = instance.GetType();
        }
    }
}