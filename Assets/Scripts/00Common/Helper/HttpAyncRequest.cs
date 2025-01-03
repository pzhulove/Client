using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System;

/// <summary>
/// HTTP 异步请求
/// </summary>
public class HTTPAsyncRequest
{
    public enum eState
    {
        None,

        Wait,
        Success,
        Error,
    }

    private class CustomAsyncCallState
    {
        public const int      cBuffSize  = 1024;

        public eState          state     = eState.None;

        public HttpWebRequest  req       = null;
        public HttpWebResponse res       = null;
        public Stream          resstream = null;

        public byte[]          buffer    = new byte[cBuffSize];
        public StringBuilder   resdata   = new StringBuilder(256);

        public long            leftCount = 0;

        public long GetReadSize()
        {
            if (leftCount < cBuffSize)
            {
                return leftCount;
            }
            else 
            {
                return cBuffSize;
            }
        }
    }

    private CustomAsyncCallState mStatus = new CustomAsyncCallState();

    private string mResultString = string.Empty;

    public void SendHttpRequst(string url, int timeout)
    {
        try
        {
            HttpWebRequest req   = (HttpWebRequest)WebRequest.Create(url);


            mStatus.req           = req;
            mStatus.state         = eState.Wait;

            req.Timeout          = timeout;

            req.BeginGetResponse(new AsyncCallback(_getResponseCallback), mStatus);

            Logger.LogProcessFormat("[http] 开始请求数据 {0}, 超时时间 {1}", url, timeout);
        } 
        catch(WebException e)
        {
            Logger.LogErrorFormat("[WebException] msg {0} status {1}", e.Message, e.Status);
            mStatus.state = eState.Error;
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("[WebException] msg {0}", e.ToString());
            mStatus.state = eState.Error;
        }
    }

    private void _getResponseCallback(IAsyncResult ar)
    {
        try
        {
            CustomAsyncCallState status = (CustomAsyncCallState)ar.AsyncState;
            HttpWebRequest       req    = status.req;

            status.res                  = (HttpWebResponse)req.EndGetResponse(ar);

            Stream resstream            = status.res.GetResponseStream();
            status.resstream            = resstream;
            status.leftCount            = status.res.ContentLength; 


            Logger.LogProcessFormat("[http] 得到http响应开始读取返回的流, 总长度{0}", status.leftCount);

            IAsyncResult result         = resstream.BeginRead(status.buffer, 0, CustomAsyncCallState.cBuffSize /*(int)status.GetReadSize()*/, _getReadStreamCallback, status);
            Logger.LogProcessFormat("[http] 异步读取结果 {0}", result);

        }
        catch(WebException e)
        {
            Logger.LogErrorFormat("[WebException] msg {0} status {1}", e.Message, e.Status);
            mStatus.state = eState.Error;
        }
        catch(Exception e)
        {
            mStatus.state = eState.Error;
            Logger.LogErrorFormat("[WebException] msg {0}", e.ToString());
        }

    }

    private void _finishStreamRead(CustomAsyncCallState status)
    {
        if(status.resdata.Length > 1)
        {
            mResultString = status.resdata.ToString();
            Logger.LogProcessFormat("[http] 成功获取到返回数据 {0} {1}", mResultString.Length, mResultString);
        }

        status.resstream.Close();
        status.state = eState.Success;
    }

    private void _getReadStreamCallback(IAsyncResult ar)
    {
        try
        {
            CustomAsyncCallState status = (CustomAsyncCallState)ar.AsyncState;
            Stream stream               = status.resstream;

            int read                    = stream.EndRead(ar);
            // Read the HTML page and then do something with it
            if (read > 0)
            {
                Logger.LogProcessFormat("[http] 读取到 {0} 长度数据数据", read);

                //status.leftCount -= read;
                status.resdata.Append(Encoding.UTF8.GetString(status.buffer, 0, read));

                IAsyncResult result = stream.BeginRead( status.buffer, 0, CustomAsyncCallState.cBuffSize, _getReadStreamCallback, status);
            }
            else
            {
                _finishStreamRead(status);
            }

        }
        catch(WebException e)
        {
            Logger.LogErrorFormat("[WebException] msg {0} status {1}", e.Message, e.Status);
            mStatus.state = eState.Error;
        }
        catch(Exception e)
        {
            Logger.LogErrorFormat("[WebException] msg {0}", e.ToString());
            mStatus.state = eState.Error;
        }
    }

    public eState GetState()
    {
        if (null == mStatus)
        {
            return eState.None;
        }

        return mStatus.state;
    }

    public string GetResString()
    {
        return mResultString;
    }
}
