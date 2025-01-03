using System.Collections.Generic;
using System;

namespace Tenmove.Runtime
{
    [System.Serializable]
    public class LinearMap<K,V> where K: IComparable<K>, IComparable, IEquatable<K>
    {
        [System.Serializable]
        public class KeyValuePair<TKey, TValue> where TKey : IComparable<TKey>, IComparable, IEquatable<TKey>
        {
            private TKey m_Key;
            private TValue m_Value;
            private int m_HashKey;

            public KeyValuePair()
            {
                m_HashKey = ~0;
            }

            public KeyValuePair(TKey key, TValue value)
            {
                m_Key = key;
                m_HashKey = key.GetHashCode();
                m_Value = value;
            }

            public TKey Key
            {
                get { return m_Key; }
                set { m_Key = value; m_HashKey = value.GetHashCode(); }
            }

            public int HashKey
            {
                get { return m_HashKey; }
            }

            public TValue Value
            {
                get { return m_Value; }
                set { m_Value = value; }
            }
        }

        protected class LinearMapComparer : IComparer<KeyValuePair<K, V>>
        {
            public int Compare(KeyValuePair<K, V> x, KeyValuePair<K, V> y)
            {
                /// if (x.HashKey == y.HashKey)
                ///     return x.Key.CompareTo(y.Key);
                /// return 1;
                return x.Key.CompareTo(y.Key);
            }
        }
        protected readonly LinearMapComparer m_Comparer = new LinearMapComparer();
        protected KeyValuePair<K, V> m_TargetItem = new KeyValuePair<K, V>();

        protected List<KeyValuePair<K, V>> m_ObjectMap;
        protected bool m_AutoSorting;
        bool m_IsDirty = false;

        public LinearMap(bool manualSorting)
            : this(0, manualSorting)
        {
        }

        public LinearMap(int capacity,bool manualSorting)
        {
            m_ObjectMap = new List<KeyValuePair<K,V>>(capacity);
            m_AutoSorting = !manualSorting;
        }

        public int Count
        {
            get { return m_ObjectMap.Count; }
        }

        public void Fill(List<KeyValuePair<K, V>> content, bool fixOrder)
        {
            m_ObjectMap = content;
            m_AutoSorting = fixOrder;
        }


        public void Add(K key,V value)
        {
            if (_CheckExist(key))
            {
                Debugger.LogError("There is already exist item with same key '{0}'!", key);
                return;
            }

            m_ObjectMap.Add(new KeyValuePair<K, V>(key, value));
            _MarkDirty();
        }

        public bool TryGetValueAt(int index,out V value)
        {
            if(0 <= index && index < m_ObjectMap.Count)
            {
                value = m_ObjectMap[index].Value;
                return true;
            }

            value = default(V);
            return false;
        }

        public bool TryGetValue(K key,out V value)
        {
            _EnsureOrder(m_AutoSorting);

            m_TargetItem.Key = key;
            int idx = m_ObjectMap.BinarySearch(m_TargetItem, m_Comparer);
            if(0<=idx && idx < m_ObjectMap.Count)
            {
                value = m_ObjectMap[idx].Value;
                return true;
            }

            value = default(V);
            return false;
        }

        public bool ContainsKey(K key)
        {
            _EnsureOrder(m_AutoSorting);

            m_TargetItem.Key = key;
            int idx = m_ObjectMap.BinarySearch(m_TargetItem, m_Comparer);
            return 0 <= idx && idx < m_ObjectMap.Count;
        }

        public bool Remove(K key)
        {
            _EnsureOrder(m_AutoSorting);

            m_TargetItem.Key = key;
            int idx = m_ObjectMap.BinarySearch(m_TargetItem, m_Comparer);
            if (0 <= idx && idx < m_ObjectMap.Count)
            {
                m_ObjectMap.RemoveAt(idx);
                _MarkDirty();

                return true;
            }

            return false;
        }

        public V this[int index]
        {
            get
            {
                return m_ObjectMap[index].Value;
            }
            set
            {
                m_ObjectMap[index].Value = value;
            }
        }

        public void Clear()
        {
            m_ObjectMap.Clear();
            m_IsDirty = false;
        }

        public void Sort()
        {
            if (m_IsDirty)
            {
                m_ObjectMap.Sort(m_Comparer);
                m_IsDirty = false;
            }
        }

        protected void _EnsureOrder(bool sorting)
        {
            if (m_IsDirty)
            {
                if(sorting)
                    m_ObjectMap.Sort(m_Comparer);
                m_IsDirty = false;
            }
        }

        private void _MarkDirty()
        {
            m_IsDirty = true;
        }

        private bool _CheckExist(K key)
        {
            int keyHash = key.GetHashCode();
            for(int i = 0,icnt = m_ObjectMap.Count;i<icnt;++i)
            {
                KeyValuePair<K, V> cur = m_ObjectMap[i];
                if (cur.HashKey == keyHash && cur.Key.Equals(key))
                    return true;
            }
            return false;
        }
    }

}

