using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;

public class Http
{
    private static ManualResetEvent allDone = new ManualResetEvent(false);
    
    public static void UploadFile(string url, string filename)
    {
        try
        {
            using (FileStream fsRead = new FileStream(StringHelper.BytesToString(filename), FileMode.Open))
            {
                int fsLen = (int)fsRead.Length;
                byte[] byteArray = new byte[fsLen + 1];
                int r = fsRead.Read(byteArray, 0, fsLen);
                fsRead.Close();
                SendPostRequest(url, (byteArray));

                /*byte[] postContent = StringHelper.StringToUTF8Bytes(content);

                //初始化新的webRequst
                //1． 创建httpWebRequest对象
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                //2． 初始化HttpWebRequest对象
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = postContent.Length;
                //3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
                System.IO.Stream newStream = webRequest.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
                newStream.Write(postContent, 0, postContent.Length);
                newStream.Close();

                //4． 读取服务器的返回信息
                webRequest.BeginGetResponse(
                    (IAsyncResult ar) =>
                    {
                    }, null
                );*/
                //HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();11
            }
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("upload {0} log to {1} failed, error:{2}", filename, url, e.ToString());
        }
    }

    private enum eCustomAsyncCall
    {
        None,
        Start,
        WriteRequestStream,
        WaitResponse,
        End,
        Error,
    }

    private class CustomAsyncCall
    {
        public HttpWebRequest       req        = null;
        public byte[]               content    = new byte[0]; 
        public eCustomAsyncCall     state      = eCustomAsyncCall.None;
        public Stream               postStream = null;

        public AsyncCallback        requestCB  = new AsyncCallback(GetRequestStreamCallback);
        public AsyncCallback        uploadCB   = new AsyncCallback(_SendCB);
        public AsyncCallback        responseCB = new AsyncCallback(GetResponseCallback);

		public System.Action<string> timeoutCB = null;
    }

    private const int kMaxCount = 10;
	private const int kTimeOut = 5 * 1000;

    private static int mIdx = 0;

	public static void SendPostRequest(string url, string content, System.Action<string> timeoutCallback = null)
    {
        try
        {
            Logger.LogProcessFormat("[HTTP - SendPostRequest] {0} {1}", url, content);

            byte[] postContent = StringHelper.StringToUTF8Bytes(content);
            if(postContent.Length == 0)
            {
                return;
            }

			SendPostRequest(url, postContent,timeoutCallback);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("send post request to {0} failed, error:{1}", url, e.ToString());
        }
    }

	public static void SendPostRequest(string url, byte[] content,System.Action<string> timeoutCallback = null)
    {
        try
        {
            byte[] postContent = content;
            if (postContent.Length == 0)
            {
                return;
            }

            int length = postContent.Length - 1;

            //初始化新的webRequst
            //1． 创建httpWebRequest对象
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
			webRequest.Timeout = kTimeOut;

            //2． 初始化HttpWebRequest对象
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = length;
            //3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
            //
            //
            //
            
            CustomAsyncCall aysnc = new CustomAsyncCall();

            aysnc.req = webRequest;
            aysnc.content = postContent;
            aysnc.state = eCustomAsyncCall.Start;
			aysnc.timeoutCB = timeoutCallback;	

            webRequest.BeginGetRequestStream(aysnc.requestCB, aysnc);
            mIdx++;

        }
        catch (Exception e)
        {
			if (timeoutCallback != null)
				timeoutCallback (e.ToString());
            Logger.LogErrorFormat("send post request to {0} failed, error:{1}", url, e.ToString());
        }
    }

    private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
    {
        CustomAsyncCall args = (CustomAsyncCall)asynchronousResult.AsyncState;
        try
        {
            if (null == args.req || args.content == null || args.content.Length <= 0)
            {
                args.state = eCustomAsyncCall.Error;
                return;
            }

            byte[] content = args.content;
            HttpWebRequest request = args.req;


            Logger.LogProcessFormat("[HTTP - SendPostRequest] GetRequestStreamCallback");

            //request.ContentLength = content.Length - 1;

            // End the operation
            Stream postStream = request.EndGetRequestStream(asynchronousResult);
            args.postStream = postStream;
            postStream.WriteTimeout = kTimeOut;
            postStream.BeginWrite(content, 0, content.Length - 1, args.uploadCB, args);

            args.state = eCustomAsyncCall.WriteRequestStream;
        }
        catch (Exception e)
        {
			if (args.timeoutCB != null)
			{
				args.timeoutCB (e.ToString());
			}
            //Logger.LogErrorFormat("error {0}", e.ToString());
            args.state = eCustomAsyncCall.Error;
        }
    }

    private static void _SendCB(IAsyncResult asynchronousResult)
    {
        CustomAsyncCall args = (CustomAsyncCall)asynchronousResult.AsyncState;

        try 
        {
            if (null == args || null == args.req || args.postStream == null)
            {
                args.state = eCustomAsyncCall.Error;
                return;
            }

            Logger.LogProcessFormat("[HTTP - SendPostRequest] _SendCB");

            HttpWebRequest request = args.req;

            Stream stream = args.postStream;
            stream.Close();

            args.state = eCustomAsyncCall.WaitResponse;

            request.BeginGetResponse(args.responseCB, args);
        }
        catch (Exception e)
        {
			if (args.timeoutCB != null)
			{
				args.timeoutCB (e.ToString());
			}
            //Logger.LogErrorFormat("error {0}", e.ToString());
            args.state = eCustomAsyncCall.Error;
        }
    }

    private static void GetResponseCallback(IAsyncResult asynchronousResult)
    {
        CustomAsyncCall args = (CustomAsyncCall)asynchronousResult.AsyncState;
        try 
        {
            if (null == args)
            {
                args.state = eCustomAsyncCall.Error;
                return;
            }

            Logger.LogProcessFormat("[HTTP - SendPostRequest] GetResponseCallback");

            args.state = eCustomAsyncCall.End;
            mIdx--;
        }
        catch (Exception e)
        {
			if (args.timeoutCB != null)
			{
				args.timeoutCB (e.ToString());
			}
            //Logger.LogErrorFormat("error {0}", e.ToString());
            args.state = eCustomAsyncCall.Error;
        }
    }
}
