using System.Collections.Generic;
///////É¾³ýlinq

namespace GameClient
{
    public class CachedObjectDicManager<Tkey, TValue> where TValue : CachedObject, new()
    {
        public CachedObjectDicManager()
        {

        }

        public TValue Create(Tkey key, object[] param)
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

            m_akActivedObject.Add(key, current);

            return current;
        }

        public bool HasObject(Tkey key)
        {
            return m_akActivedObject.Count > 0 && m_akActivedObject.ContainsKey(key);
        }

        public TValue RefreshObject(Tkey key, object[] param = null)
        {
            TValue current = null;
            if (m_akActivedObject.TryGetValue(key, out current))
            {
                current.OnRefresh(param);
            }
            return current;
        }

        public void RefreshAllObjects(object[] param = null)
        {
            var enumerator = m_akActivedObject.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TValue current = enumerator.Current.Value as TValue;
                current.OnRefresh(param);
            }
        }

        public void RecycleObjectByFilter(System.Predicate<TValue> match)
        {
            var keys = m_akActivedObject.Keys.ToList();
            var values = m_akActivedObject.Values.ToList();
            for (int j = 0; j < values.Count; ++j)
            {
                if(match(values[j]))
                {
                    RecycleObject(keys[j]);
                }
            }
            keys.Clear();
            values.Clear();
        }

        public List<TValue> GetObjectListByFilter(object[] param = null)
        {
            List<TValue> outValue = new List<TValue>();
            var objects = m_akActivedObject.Values.ToList();
            for (int i = 0; i < objects.Count; ++i)
            {
                if (!objects[i].NeedFilter(param))
                {
                    outValue.Add(objects[i]);
                }
            }
            return outValue;
        }

        public bool HasObject(object[] param)
        {
            var objects = m_akActivedObject.Values.ToList();
            for (int i = 0; i < objects.Count; ++i)
            {
                if (!objects[i].NeedFilter(param))
                {
                    return true;
                }
            }
            return false;
        }

        public TValue GetObject(Tkey key)
        {
            TValue current = null;
            m_akActivedObject.TryGetValue(key, out current);
            return current;
        }

        public void FilterObject(Tkey key, object[] param = null)
        {
            TValue current = null;
            if (m_akActivedObject.TryGetValue(key, out current))
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
        }

        public void DestroyObject(Tkey key)
        {
            TValue current = null;
            if (m_akActivedObject.TryGetValue(key, out current))
            {
                m_akActivedObject.Remove(key);
                current.OnDestroy();
            }
        }

        public void DestroyAllObjects()
        {
            var values = m_akActivedObject.Values.ToList();
            for (int i = 0; i < values.Count; ++i)
            {
                values[i].OnDestroy();
            }
            m_akActivedObject.Clear();

            for (int i = 0; i < m_akCachedObject.Count; ++i)
            {
                m_akCachedObject[i].OnDestroy();
            }
            m_akCachedObject.Clear();
        }

        public TValue RebuildOrCreate(Tkey kRecycled, Tkey key, object[] param)
        {
            if (!m_akActivedObject.ContainsKey(kRecycled))
            {
                //Logger.LogErrorFormat("rebuild failed call create!");
                return Create(key, param);
            }
            TValue current = m_akActivedObject[kRecycled];
            m_akActivedObject.Remove(kRecycled);
            m_akActivedObject.Add(key, current);
            current.OnCreate(param);
            return current;
        }

        public void RecycleObject(Tkey key)
        {
            TValue current = null;
            if (m_akActivedObject.TryGetValue(key, out current))
            {
                m_akActivedObject.Remove(key);
                m_akCachedObject.Add(current);
                current.OnRecycle();
            }
        }

        public void RecycleAllObject()
        {
            var values = m_akActivedObject.Values.ToList();
            for (int i = 0; i < values.Count; ++i)
            {
                m_akCachedObject.Add(values[i]);
                values[i].OnRecycle();
            }
            m_akActivedObject.Clear();
        }

        public void Filter(object[] param)
        {
            var objects = m_akActivedObject.Values.ToList();
            for (int i = 0; i < objects.Count; ++i)
            {
                if (objects[i].NeedFilter(param))
                {
                    objects[i].Disable();
                }
                else
                {
                    objects[i].Enable();
                }
            }
        }

        public Dictionary<Tkey, TValue> ActiveObjects
        {
            get { return m_akActivedObject; }
        }

        public void Clear()
        {
            m_akActivedObject.Clear();
            m_akCachedObject.Clear();
        }

        Dictionary<Tkey, TValue> m_akActivedObject = new Dictionary<Tkey, TValue>();
        List<TValue> m_akCachedObject = new List<TValue>();
    }
}