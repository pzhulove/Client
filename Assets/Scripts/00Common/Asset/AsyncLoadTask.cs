using UnityEngine;

public enum eAsyncLoadTaskStatus
{
    onNone,
    onLoading,
    onCondition,
    onPostCall,
    onFinish,
}

public delegate void PostLoadGameObject(GameObject gameObject);
public delegate bool PostLoadCondition();

public class AsyncLoadTask
{
    public uint handle { get; private set; }
    public uint waithandle { get; private set; }

    public bool isPooled { get; private set; }
    public string tag { get; private set; }
    public PostLoadGameObject load { get; private set; }
    public eAsyncLoadTaskStatus status { get; set; }
    public bool isFinish { get { return eAsyncLoadTaskStatus.onFinish == status; } }
    public bool isLoaded
    {
        get
        {
            return eAsyncLoadTaskStatus.onFinish    == status ||
                   eAsyncLoadTaskStatus.onCondition == status;
        }
    }
    public string path { get; private set; }
    private GameObject m_Target = null;

    public void OnObjectLoaded(GameObject go)
    {
        m_Target = go;
    }

    public bool IsObjectLoaded()
    {
        return null != m_Target;
    }

    public GameObject ExtractObject()
    {
        GameObject go = m_Target;
        m_Target = null;
        return go;
    }

    public AsyncLoadTask(string _tag, uint _handle, string _path, uint _waithandle, PostLoadGameObject _load, bool _isPooled)
    {
        tag        = _tag;
        handle 	   = _handle;
        waithandle = _waithandle;
        load       = _load;
        isPooled   = _isPooled;
        status     = eAsyncLoadTaskStatus.onNone;
        path       = _path;
    }
}
