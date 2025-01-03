using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace GameClient
{
    public class PriorityQueue<T>
    {
        IComparer<T> m_comparer = null;
        T[] m_arrData = null;

        public int Count { get; private set; }
        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    return m_arrData[index];
                }
                else
                {
                    throw new InvalidOperationException(string.Format("优先队列越界 索引：{0}", index));
                }
            }
            set
            {
                if (index >= 0 && index < Count)
                {
                    m_arrData[index] = value;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("优先队列越界 索引：{0}", index));
                }
            }
        }

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.m_comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.m_arrData = new T[capacity];
        }
        public PriorityQueue() : this(64, null) { }
        public PriorityQueue(int capacity) : this(capacity, null) { }
        public PriorityQueue(IComparer<T> comparer) : this(64, comparer) { }

        public T Top()
        {
            if (Count > 0)
            {
                return m_arrData[0];
            }
            throw new InvalidOperationException("优先队列为空");
        }

        public T TakeTop()
        {
            T t = Top();
            m_arrData[0] = m_arrData[--Count];
            if (Count > 0)
            {
                _Sink(0);
            }
            return t;
        }

        public void Add(T v)
        {
            if (Count >= m_arrData.Length)
            {
                Array.Resize(ref m_arrData, Count * 2);
            }
            m_arrData[Count] = v;
            _Floating(Count);
            ++Count;
        }

        public int FindIndex(Predicate<T> match)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (match.Invoke(m_arrData[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Clear()
        {
            Count = 0;
        }

        protected void _Floating(int a_nIdx)
        {
            T temp = m_arrData[a_nIdx];
            int nParentIdx = (a_nIdx + 1) / 2 - 1;
            while (a_nIdx > 0)
            {
                if (m_comparer.Compare(temp, m_arrData[nParentIdx]) <= 0)
                {
                    break;
                }

                m_arrData[a_nIdx] = m_arrData[nParentIdx];

                a_nIdx = nParentIdx;
                nParentIdx = (a_nIdx + 1) / 2 - 1;
            }
            m_arrData[a_nIdx] = temp;
        }

        protected void _Sink(int a_nIdx)
        {
            T temp = m_arrData[a_nIdx];
            int nChildIdx = (a_nIdx + 1) * 2 - 1;
            while (nChildIdx < Count)
            {
                if (nChildIdx < Count - 1 && m_comparer.Compare(m_arrData[nChildIdx], m_arrData[nChildIdx + 1]) <= 0)
                {
                    ++nChildIdx;
                }
                if (m_comparer.Compare(temp, m_arrData[nChildIdx]) >= 0)
                {
                    break;
                }

                m_arrData[a_nIdx] = m_arrData[nChildIdx];
                a_nIdx = nChildIdx;
                nChildIdx = (a_nIdx + 1) * 2 - 1;
            }
            m_arrData[a_nIdx] = temp;
        }
    }
}
