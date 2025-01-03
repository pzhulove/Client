

namespace Tenmove.Runtime
{
    public abstract class RecycleBinObject<TRecyclable> : Recyclable where TRecyclable: Recyclable,new()
    {
        static private ITMRecycleBin m_RecycleBin;

        public RecycleBinObject()
        {
        }

        static public TRecyclable Acquire()
        {
            if (null == m_RecycleBin)
                m_RecycleBin = ModuleManager.GetModule<ITMRecycleBin>();

            Debugger.Assert(null != m_RecycleBin, "Recycle bin can not be null!");
            return m_RecycleBin.Acquire<TRecyclable>();
        }

        public void Recycle()
        {
            Debugger.Assert(null != m_RecycleBin, "Recycle bin can not be null!");
            m_RecycleBin.Recycle<TRecyclable>(this as TRecyclable);
        }
    }

    public abstract class RecyclableObject<TRecyclable> : RecycleBinObject<TRecyclable> where TRecyclable : Recyclable, new()
    {
        public sealed override bool IsValid { get { return true; } }
        public sealed override void OnRelease() { }
    }
}

