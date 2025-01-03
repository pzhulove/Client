using UnityEngine;
using System.Collections.Generic;

namespace HGBase
{
    public class LazyArray<T>
    {
        public LazyArray(int initSize,T invalid_Value)
        {
            if(0 != initSize)
                m_Array = new T[initSize];

            m_ArrayLen = initSize;
            m_InvalidValue = invalid_Value;
        }

        public void Resize(int newSize)
        {
            if (newSize > m_Array.Length)
            {
                T[] newArray = new T[newSize];
                if(null != newArray)
                {
                    for (int i = 0; i < m_ArrayLen; ++i)
                        newArray[i] = m_Array[i];

                    m_Array = newArray;
                }
            }

            m_ArrayLen = newSize;
        }

        public int Size()
        {
            return m_ArrayLen;
        }

        public void Add(T elem)
        {
            int oldSize = m_ArrayLen;
            Resize(m_ArrayLen + 1);
            m_Array[oldSize] = elem;
        }

        public T At(int index)
        {
            if (0 <= index && index < m_ArrayLen)
                return m_Array[index];

            return INVALID_VALUE;
        }

        public T this[int index]
        {
            get
            {
                if (0 <= index && index < m_ArrayLen)
                    return m_Array[index];

                return INVALID_VALUE;
            }
            set
            {
                if (0 <= index && index < m_ArrayLen)
                    m_Array[index] = value;
            }
        }

        public T INVALID_VALUE { get { return m_InvalidValue; } }
        protected T m_InvalidValue;
        protected T[] m_Array;
        protected int m_ArrayLen;
    }
}
