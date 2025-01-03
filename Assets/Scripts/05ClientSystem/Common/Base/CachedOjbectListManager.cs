using System.Collections.Generic;

namespace GameClient
{
    public class CachedObjectListManager<TValue> where TValue : CachedObject, new()
    {
        public CachedObjectListManager()
        {
            m_akCachedObject.Clear();
            m_akActivedObject.Clear();
        }

        public TValue Find(System.Predicate<TValue> match)
        {
            return m_akActivedObject.Find(match);
        }

        public void Recycle(System.Predicate<TValue> match)
        {
            var find = m_akActivedObject.Find(match);
            if(find != null)
            {
                RecycleObject(find);
            }
        }

        public void RefreshAllObjects(object[] param)
        {
            for (int i = 0; i < m_akActivedObject.Count; ++i)
            {
                m_akActivedObject[i].OnRefresh(param);
            }
        }

        public TValue Create(object[] param)
        {
            TValue current = null;
            if (m_akCachedObject.Count > 0)
            {
                current = m_akCachedObject[0];
                m_akCachedObject.RemoveAt(0);
                current.OnDecycle(param);
            }
            else
            {
                current = new TValue();
                current.OnCreate(param);
            }
            m_akActivedObject.Add(current);

            return current;
        }

        public void RecycleObject(TValue current)
        {
            if (current != null)
            {
                m_akActivedObject.Remove(current);
                m_akCachedObject.Add(current);
                current.OnRecycle();
            }
        }

        public void DestroyAllObjects()
        {
            for (int i = 0; i < m_akActivedObject.Count; ++i)
            {
                m_akActivedObject[i].OnDestroy();
            }
            m_akActivedObject.Clear();

            for (int i = 0; i < m_akCachedObject.Count; ++i)
            {
                m_akCachedObject[i].OnDestroy();
            }
            m_akCachedObject.Clear();
        }

        public void RecycleAllObject()
        {
            while (m_akActivedObject.Count > 0)
            {
                var activeObject = m_akActivedObject[m_akActivedObject.Count - 1];
                if (activeObject != null)
                {
                    m_akCachedObject.Add(activeObject);
                    activeObject.OnRecycle();
                }
                m_akActivedObject.RemoveAt(m_akActivedObject.Count - 1);
            }
        }

        public void Filter(object[] param = null)
        {
            for (int i = 0; i < m_akActivedObject.Count; ++i)
            {
                if (m_akActivedObject[i].NeedFilter(param))
                {
                    m_akActivedObject[i].Disable();
                }
                else
                {
                    m_akActivedObject[i].Enable();
                }
            }
        }

        public void FilterObject(TValue current, object[] param = null)
        {
            if (current.NeedFilter(param))
            {
                current.Disable();
            }
            else
            {
                current.Enable();
            }
        }

        public void Clear()
        {
            m_akCachedObject.Clear();
            m_akActivedObject.Clear();
        }

        public TValue GetChild(int iIndex)
        {
            if (iIndex >= 0 && iIndex < m_akActivedObject.Count)
            {
                return m_akActivedObject[iIndex];
            }
            return null;
        }

        public int ChildCount
        {
            get
            {
                return m_akActivedObject.Count;
            }
        }

        public List<TValue> ActiveObjects
        {
            get { return m_akActivedObject; }
        }

        List<TValue> m_akCachedObject = new List<TValue>();
        List<TValue> m_akActivedObject = new List<TValue>();
    }
}