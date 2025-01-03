using UnityEngine;
using System.Collections.Generic;


public class AsyncRequestHandleAllocator<T> where T:class
{
    //protected Dictionary<uint, T> m_AsyncRequestHandleMap = new Dictionary<uint, T>();
    protected uint m_CurAudioHandleCnt = 0;
    protected uint m_HandleType = 0;

    protected class AsyncRequestDesc
    {
        public AsyncRequestDesc(uint handle,T request)
        {
            m_Handle = handle;
            m_Request = request;
        }

        public uint m_Handle;
        public T m_Request;
    }
    protected List<AsyncRequestDesc> m_AsyncReqHandleDescList = new List<AsyncRequestDesc>();
    protected AsyncRequestDesc m_AsyncReqHandleDescCache = new AsyncRequestDesc(uint.MaxValue, null);

    public AsyncRequestHandleAllocator(uint type)
    {
        m_HandleType = type;
    }

    public T GetAsyncRequestByHandle(uint handle)
    {
        //         T res = null;
        //         if (m_AsyncRequestHandleMap.TryGetValue(handle, out res))
        //             return res;
        // 
        //         return null;
        if (handle == m_AsyncReqHandleDescCache.m_Handle)
            return m_AsyncReqHandleDescCache.m_Request;

        for (int i = 0,icnt = m_AsyncReqHandleDescList.Count;i<icnt;++i)
        {
            if (handle == m_AsyncReqHandleDescList[i].m_Handle)
            {
                m_AsyncReqHandleDescCache.m_Handle = handle;
                m_AsyncReqHandleDescCache.m_Request = m_AsyncReqHandleDescList[i].m_Request;
                return m_AsyncReqHandleDescList[i].m_Request;
            }
        }

        return null;
    }


    public uint AddAsyncRequest(T asyncRequest)
    {
        //         if (null != asyncRequest)
        //         {
        //             uint handle = _AllocHandle();
        //             m_AsyncRequestHandleMap.Add(handle, asyncRequest);
        //             return handle;
        //         }
        // 
        //         return uint.MaxValue;

        if (null != asyncRequest)
        {
            uint handle = _AllocHandle();
#if UNITY_EDITOR
            if(null != GetAsyncRequestByHandle(handle))
            {
                Logger.LogErrorFormat("Request with handle [{0}] already exist!", handle);
                return uint.MaxValue;
            }
#endif

            m_AsyncReqHandleDescCache.m_Handle = handle;
            m_AsyncReqHandleDescCache.m_Request = asyncRequest;
            m_AsyncReqHandleDescList.Add(new AsyncRequestDesc(handle,asyncRequest));
            return handle;
        }

        return uint.MaxValue;
    }

    public void RemoveAsyncRequest(uint handle)
    {
        //m_AsyncRequestHandleMap.Remove(handle);
        for (int i = 0, icnt = m_AsyncReqHandleDescList.Count; i < icnt; ++i)
        {
            if (handle == m_AsyncReqHandleDescList[i].m_Handle)
            {
                if(handle == m_AsyncReqHandleDescCache.m_Handle)
                {
                    m_AsyncReqHandleDescCache.m_Handle = uint.MaxValue;
                    m_AsyncReqHandleDescCache.m_Request = null;
                }
                m_AsyncReqHandleDescList.RemoveAt(i);
                return;
            }
        }
    }

    //protected


    protected uint _AllocHandle()
    {
        /// 前两位预留
        if (m_CurAudioHandleCnt + 1 >= uint.MaxValue >> 2)
            m_CurAudioHandleCnt = 0;
        return (m_CurAudioHandleCnt++) | ((m_HandleType & 0x03) << 30);
    }
}
