using System;
using System.Collections.Generic;

public abstract class SingletonData
{
	protected static bool _isProcessing_Release = false;
	protected static readonly List<SingletonData> _singletonList = new List<SingletonData>();
	public static SingletonData[] GetSingletonDatas()
	{
		return SingletonData._singletonList.ToArray();
	}
	protected static void PushSingleton(SingletonData _obj)
	{
		if (_obj != null)
		{
			SingletonData._singletonList.Add(_obj);
			//AutoClearChangeSceneAttribute.Regist(_obj);
			_obj.Initialize();
		}
	}
	public static void ReleaseSingletons()
	{
		if (!SingletonData._isProcessing_Release)
		{
			SingletonData._isProcessing_Release = true;
			int count = SingletonData._singletonList.Count;
			for (int i = 0; i < count; i++)
			{
				if (SingletonData._singletonList[i] != null)
				{
					SingletonData._singletonList[i].ReleaseSingleton(false);
				}
			}
			SingletonData._singletonList.Clear();
			SingletonData._isProcessing_Release = false;
		}
	}
	public abstract void Initialize();
	public abstract void ReleaseSingleton(bool _remove_in_list = true);
}
public abstract class SingletonData<T> : SingletonData where T : class, new()
{
	private static T m_instance;
	private bool _initialized;
	public static T Instance
	{
		get
		{
			if (SingletonData<T>.m_instance == null)
			{
				if (SingletonData._isProcessing_Release)
				{
					throw new Exception("Instance(" + typeof(T) + ") already destroyed on application quit.");
				}
				SingletonData<T>.m_instance = Activator.CreateInstance<T>();
				SingletonData.PushSingleton(SingletonData<T>.m_instance as SingletonData);
			}
			return SingletonData<T>.m_instance;
		}
	}
	public static bool IsActiveInstance()
	{
		return SingletonData<T>.m_instance != null;
	}
	public override void Initialize()
	{
		if (!this._initialized)
		{
			this._initialized = true;
			this.AwakeInstance();
			//GameManager.eventChangeScene_Begin += new Action(this.OnClearData);
		}
	}
	public override void ReleaseSingleton(bool _remove_in_list = true)
	{
		if (_remove_in_list)
		{
			SingletonData._singletonList.Remove(this);
		}
		if (this._initialized)
		{
			//GameManager.eventChangeScene_Begin -= new Action(this.OnClearData);
			this.ReleaseInstance();
			//AutoClearChangeSceneAttribute.UnRegist(SingletonData<T>.m_instance);
			this._initialized = false;
		}
		SingletonData<T>.m_instance = (T)((object)null);
	}

	protected abstract void AwakeInstance();
	protected abstract void ReleaseInstance();
	protected virtual void OnClearData()
	{
	}
}

public abstract class SimpleSingleton<T> where T : SimpleSingleton<T>, new()
{
    private static T m_instance;
    private bool _initialized;
    public static T Instance
    {
        get
        {
/*
            if (SimpleSingleton<T>.m_instance == null)
            {
                throw new Exception("Instance(" + typeof(T) + ") Not Created.");
            }*/
            return SimpleSingleton<T>.m_instance;
        }
    }

    public static void CreateInstance()
    {
        if (m_instance != null)
        {
            ReleaseInstance();
        }

        m_instance = Activator.CreateInstance<T>();
        m_instance.OnCreateInstance();
    }

    public static void ReleaseInstance()
    {
        if (m_instance != null)
        {
            m_instance.OnReleaseInstance();
            m_instance = (T)((object)null);
        }
    }

    protected abstract void OnCreateInstance();
    protected abstract void OnReleaseInstance();
}