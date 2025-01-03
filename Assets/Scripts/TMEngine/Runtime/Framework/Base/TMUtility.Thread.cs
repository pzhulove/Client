

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Thread
        {
            static private int m_MainThreadID = ~0;

            static public void SetMainThread()
            {
                m_MainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId; 
            }

            static public bool IsMainThread()
            {
                if (~0 == m_MainThreadID)
                    throw new System.Exception("Main thread ID is not set!");

                return m_MainThreadID == System.Threading.Thread.CurrentThread.ManagedThreadId;
            }
        }
    }
}