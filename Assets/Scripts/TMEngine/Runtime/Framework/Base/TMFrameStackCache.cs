
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal class FrameStackCache<T> where T: new()
    {
        private const int CONST_MAX_STACK_DEEP = 4096;

        public delegate void OnAquiredAction(T obj);
        public delegate void OnReleasedAction(T obj);

        private readonly OnAquiredAction m_OnAquiredAction;
        private readonly OnReleasedAction m_OnReleasedAction;

        private readonly Stack<T> m_ObjectFrameStack = new Stack<T>();

        private int m_TotalAllocCount = 0;
        private int m_UsingCount = 0;
        private int m_FreeCount;

        private int m_StackDeepCount = 0;

        public int TotalAllocCount
        {
            get { return m_TotalAllocCount; }
        }

        public int UsingCount
        {
            get { return m_UsingCount; }
        }

        public int FreeCount
        {
            get { return m_FreeCount; }
        }

        public FrameStackCache(OnAquiredAction onAquired, OnReleasedAction onReleased)
        {
            m_OnAquiredAction = onAquired;
            m_OnReleasedAction = onReleased;
        }

        public T Acquire()
        {
            if(m_StackDeepCount < CONST_MAX_STACK_DEEP)
            {
                ++m_StackDeepCount;

                T element;
                if (m_ObjectFrameStack.Count == 0)
                {
                    element = new T();
                    m_TotalAllocCount++;
                }
                else
                    element = m_ObjectFrameStack.Pop();

                if (null != m_OnAquiredAction)
                    m_OnAquiredAction(element);

                return element;
            }

            Debugger.LogError("Allocate stack deep out of max range [{0}], did you acquire and release in different scope or using a terrible recursion?",CONST_MAX_STACK_DEEP);
            return default(T);
        }

        public void Recycle(T element)
        {
            if (m_ObjectFrameStack.Count > 0 && ReferenceEquals(m_ObjectFrameStack.Peek(), element))
                Debugger.LogError("Internal error. Trying to destroy object that is already released to pool.");

            if (0 < m_StackDeepCount)
            {
                --m_StackDeepCount;

                if (null != m_OnReleasedAction)
                    m_OnReleasedAction(element);
                m_ObjectFrameStack.Push(element);

                return;
            }

            Debugger.LogError("Allocate stack deep less than zero, did you acquire and release in different scope or using a terrible recursion?");
        }
    }
}
