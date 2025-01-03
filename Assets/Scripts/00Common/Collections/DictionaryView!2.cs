using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class DictionaryView<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>> where TValue : class
{
    protected Dictionary<TKey, object> Context;

    public DictionaryView()
    {
        this.Context = new Dictionary<TKey, object>();
    }

    public DictionaryView(int capacity)
    {
        this.Context = new Dictionary<TKey, object>(capacity);
    }

    public void Add(TKey key, TValue value)
    {
        this.Context.Add(key, value);
    }

    public void Clear()
    {
        this.Context.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return this.Context.ContainsKey(key);
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this.Context);
    }

    public bool Remove(TKey key)
    {
        return this.Context.Remove(key);
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        object obj2 = null;
        bool flag = this.Context.TryGetValue(key, out obj2);
        value = (obj2 == null) ? default(TValue) : ((TValue) obj2);
        return flag;
    }

    public int Count
    {
        get
        {
            return this.Context.Count;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            object obj2 = this.Context[key];
            return ((obj2 == null) ? default(TValue) : ((TValue) obj2));
        }
        set
        {
            this.Context[key] = value;
        }
    }

    public Dictionary<TKey, object>.KeyCollection Keys
    {
        get
        {
            return this.Context.Keys;
        }
    }

	public Dictionary<TKey, object>.ValueCollection Values {
		get
		{
			return this.Context.Values;
		}
	}


    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, object> Reference;
        private Dictionary<TKey, object>.Enumerator Iter;
        public Enumerator(Dictionary<TKey, object> InReference)
        {
            this.Reference = InReference;
            this.Iter = this.Reference.GetEnumerator();
        }

        object IEnumerator.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                return new KeyValuePair<TKey, TValue>(this.Iter.Current.Key,  ((this.Iter.Current.Value == null) ? default(TValue) : this.Iter.Current.Value) as TValue );
            }
        }
        public void Reset()
        {
            this.Iter = this.Reference.GetEnumerator();
        }

        public void Dispose()
        {
            this.Iter.Dispose();
            this.Reference = null;
        }

        public bool MoveNext()
        {
            return this.Iter.MoveNext();
        }
    }
}

/**
     * 这货比楼上的更暴力，强行要求Key也是Object
     * 适用于Key不是内置类型的Dictionary替换
    */
public class DictionaryObjectView<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
	protected Dictionary<object, object> Context = null;

	public DictionaryObjectView()
	{
		Context = new Dictionary<object, object>();

		#if UNITY_EDITOR  && !LOGIC_SERVER
		DebugHelper.Assert(
			(typeof(TValue).IsClass || typeof(TValue).IsInterface) &&
			(typeof(TKey).IsClass || typeof(TKey).IsInterface),
			"Performance Warning! DictionaryObjectView.KeyType or ValueType Should Not Contain Value Type!!!!"
		);
		#endif
	}

	public int Count
	{
		get { return Context.Count; }
	}

	public TValue this[TKey key]
	{
		get
		{
			var obj = Context[key];
			return obj != null ? (TValue)obj : default(TValue);
		}
		set
		{
			Context[key] = value;
		}
	}

	public void Add(TKey key, TValue value)
	{
		Context.Add(key, value);
	}

	public void Clear()
	{
		Context.Clear();
	}

	public bool ContainsKey(TKey key)
	{
		return Context.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		return Context.Remove(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		object ResultObject = null;

		bool bResult = Context.TryGetValue(key, out ResultObject);

		value = ResultObject != null ? (TValue)ResultObject : default(TValue);

		return bResult;
	}

	public Dictionary<object, object>.KeyCollection Keys
	{
		get
		{
			return this.Context.Keys;
		}
	}

	public Dictionary<object, object>.ValueCollection Values {
		get
		{
			return this.Context.Values;
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(Context);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
	{
		Dictionary<object, object> Reference;
		Dictionary<object, object>.Enumerator Iter;

		public Enumerator(Dictionary<object, object> InReference)
		{
			Reference = InReference;
			Iter = Reference.GetEnumerator();
		}

		public KeyValuePair<TKey, TValue> Current
		{
			get
			{
				return new KeyValuePair<TKey, TValue>(
					Iter.Current.Key != null ? (TKey)Iter.Current.Key : default(TKey),
					Iter.Current.Value != null ? (TValue)Iter.Current.Value : default(TValue)
				);
			}
		}

		object IEnumerator.Current { get { throw new NotImplementedException(); } }

		public void Reset()
		{
			Iter = Reference.GetEnumerator();
		}

		public void Dispose()
		{
			Iter.Dispose();
			Reference = null;
		}

		public bool MoveNext()
		{
			return Iter.MoveNext();
		}
	}
}