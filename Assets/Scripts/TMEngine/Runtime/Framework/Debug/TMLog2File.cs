
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public class Log2File
    {
        private static Log2File sm_Instance = null;
        public static string Path;
        private Stream m_Stream = null;
        private StreamWriter m_Writer = null;

        private class LogItem : RecycleBinObject<LogItem>
        {
            public readonly StringBuilder m_StringBuilder;

            public LogItem()
            {
                m_StringBuilder = new StringBuilder(128);
            }

            public String Data
            {
                get { return m_StringBuilder.ToString(); }
            }

            public void Log(string message)
            {
                m_StringBuilder.Append(message);
            }

            public void Log<T>(string format,T arg)
            {
                m_StringBuilder.AppendFormat(format,arg);
            }

            public void Log<T0,T1>(string format, T0 arg0,T1 arg1)
            {
                m_StringBuilder.AppendFormat(format, arg0, arg1);
            }

            public void Log<T0, T1, T2>(string format, T0 arg0, T1 arg1, T2 arg2)
            {
                m_StringBuilder.AppendFormat(format, arg0, arg1, arg2);
            }

            public void Log<T0, T1, T2, T3>(string format, T0 arg0, T1 arg1, T3 arg2, T3 arg3)
            {
                m_StringBuilder.AppendFormat(format, arg0, arg1, arg2, arg3);
            }

            public void Log(string format,params object[] args)
            {
                m_StringBuilder.AppendFormat(format, args);
            }

            public override void OnRecycle()
            {
                base.OnRecycle();
                m_StringBuilder.Clear();
            }

            public override bool IsValid { get { return true; } }
            public override void OnRelease() { }
        }

        private readonly List<LogItem> m_LogItems;

        public Log2File()
        {
            m_LogItems = new List<LogItem>();

            _Log(" ");
            _Log(" ");
            _Log(string.Format("############################### Log2File Online:{0} ###############################", DateTime.Now.ToString("yyyy-MM-dd")));
            _Log(" ");
            _Log(" ");
        }

        ~Log2File()
        {
        }

        static Log2File Instance
        {
            get
            {
                if (null == sm_Instance)
                {
                    if (!string.IsNullOrEmpty(Path))
                        sm_Instance = new Log2File();
                }
                return sm_Instance;                
            }
        }

        static public void Log(string fmt,params object[] args)
        {
            Log2File log2File = Instance;
            if (null != log2File)
                log2File._Log(string.Format("{0}: {1}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), string.Format(fmt, args)));
        }

        static public void Log(string content)
        {
            Log2File log2File = Instance;
            if (null != log2File)
                log2File._Log(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), content));
        }

        static public void Flush()
        {
            Log2File log2File = Instance;
            if (null != log2File)
                log2File._Flush2File();
        }

        static public void Shutdown()
        {
            if(null != sm_Instance)
                sm_Instance._Close();

            sm_Instance = null;
        }

        private void _Log(string content)
        {
            if (m_LogItems.Count > 128)
                _Flush2File();

            LogItem cur = LogItem.Acquire();
            cur.Log(content);
            m_LogItems.Add(cur);
        }

        private void _Flush2File()
        {
            _Close();

            m_Stream = new System.IO.FileStream(Path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            if (null != m_Stream)
            {
                m_Writer = new StreamWriter(m_Stream);
                for (int i = 0, icnt = m_LogItems.Count; i < icnt; ++i)
                {
                    LogItem cur = m_LogItems[i];
                    m_Writer.WriteLine(cur.Data);
                    cur.Recycle();
                }
                m_LogItems.Clear();
                _Close();
            }
        }

        private void _Close()
        {
            if (null != m_Writer)
            {
                m_Writer.Flush();
                if (null != m_Stream)
                {
                    m_Stream.Close();
                    m_Stream = null;
                }

                m_Writer = null;
            }
        }        
    }
}