using System;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace Tenmove.Runtime
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ProcedureAttribute : Attribute
    {
        private bool m_IsLog = false;

        public bool Log
        {
            set { m_IsLog = value; }
            get { return m_IsLog; }
        }

        static public bool NeedLog(Type type)
        {
            if (null != type)
            {
                ProcedureAttribute[] logAttributes = (ProcedureAttribute[])type.GetCustomAttributes(typeof(ProcedureAttribute), false);
                if (null != logAttributes && logAttributes.Length > 0)
                    return logAttributes[0].Log;
            }

            return false;
        }
    }

    public enum DebugLevel
    {
        All,
        Debug,
        Info,
        Warning,
        Error,
        Exception,
        Fatal,
    }

    public class Debugger
    {

        public struct DebugTrace
        {
            public DebugTrace(string fmt, params object[] labels)
            {
#if !LOGIC_SERVER
                _LogInfoFormat(fmt, labels);
#endif
            }
        }

        static int m_DisableLevel =
#if UNITY_EDITOR
#	if RELEASE
            (int)DebugLevel.Fatal;
#	else
            (int)DebugLevel.All;
#	endif
#else
#	if RELEASE//用于打包
            (int)DebugLevel.Fatal;
#	else
            (int)DebugLevel.All;
#endif
#endif

        //static private ITMRemoteConsole sm_RemoteConsole = null;
        //
        //static private ITMRemoteConsole Console
        //{
        //    get
        //    {
        //        if (null == sm_RemoteConsole)
        //            sm_RemoteConsole = ModuleManager.GetModule<ITMRemoteServer>().Console;
        //
        //        return sm_RemoteConsole;
        //    }
        //}

        static public void SetLogLevel(DebugLevel level)
        {
            m_DisableLevel = (int)level;
        }

        public static void TraceStack(params string[] labels)
        {
            string res = string.Empty;
            if (null != labels)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0, icnt = labels.Length; i < icnt; ++i)
                    sb.Append(labels[i]);
                res = sb.ToString();
            }

            new DebugTrace(res);
        }

        static public void LogProcedure<T>(T target, string format, params object[] args)
        {
#if !LOGIC_SERVER
            Type type = typeof(T);
            if (ProcedureAttribute.NeedLog(type))
            {
                string newFmt = string.Format("[DEBUG:{0}] {1}", type, format);
                _LogInfoFormat(newFmt, args);
            }
#endif
        }

        static public void LogInfo(string info)
        {
#if !LOGIC_SERVER
            _LogInfo(info);
#endif
        }

        static public void LogInfo(string format, params object[] args)
        {
#if !LOGIC_SERVER
            _LogInfoFormat(format, args);
#endif
        }

        static public void LogWarning(string warning)
        {
#if !LOGIC_SERVER
            _LogWarning(warning);
#endif
        }

        static public void LogWarning(string format, params object[] args)
        {
#if !LOGIC_SERVER
            _LogWarningFormat(format, args);
#endif
        }

        static public void LogError(string error)
        {
#if !LOGIC_SERVER
            _LogError(error);
#endif
        }

        static public void LogError(string format, params object[] args)
        {
#if !LOGIC_SERVER
            _LogErrorFormat(format, args);
#endif
        }

        static public void LogException(string format, params object[] args)
        {
#if !LOGIC_SERVER
            _LogExceptionFormat(format, args);
#endif
        }

        static public void Assert(bool condition, string message)
        {
#if !LOGIC_SERVER
            _Assert(condition, message);
#endif
        }

        static public void Assert(bool condition, string format, object args0)
        {
#if !LOGIC_SERVER
            _Assert(condition, format, args0);
#endif
        }

        static public void Assert(bool condition, string format, object args0, object args1)
        {
#if !LOGIC_SERVER
            _Assert(condition, format, args0, args1);
#endif
        }

        static public void Assert(bool condition, string format, object args0, object args1, object args2)
        {
#if !LOGIC_SERVER
            _Assert(condition, format, args0, args1, args2);
#endif
        }

        static public void Assert(bool condition, string format, params object[] args)
        {
#if !LOGIC_SERVER
            _Assert(condition, format, args);
#endif
        }

        static public void AssertFailed(string format, object args0)
        {
#if !LOGIC_SERVER
            _AssertFailed(format, args0);
#endif
        }

        static public void AssertFailed(string format, object args0, object args1)
        {
#if !LOGIC_SERVER
            _AssertFailed(format, args0, args1);
#endif
        }

        static public void AssertFailed(string format, object args0, object args1, object args2)
        {
#if !LOGIC_SERVER
            _AssertFailed(format, args0, args1, args2);
#endif
        }

        static public void AssertFailed(string format, params object[] args)
        {
#if !LOGIC_SERVER
            _AssertFailed(format, args);
#endif
        }

        static private void _Assert(bool condition)
        {
#if UNITY_EDITOR && !UNITY_EDITOR_OSX
            Debug.Assert(condition);
#else
            if (!condition)
            {
                throw new EngineException();
            }
#endif
        }

        static private void _AssertFailed(string message)
        {
            Assert(false, message);
        }

        static private void _AssertFailed(string format, params object[] args)
        {
            Assert(false, format, args);
        }

        static private void _AssertFailed(string format, object args0)
        {
            Assert(false, format, args0);
        }

        static private void _AssertFailed(string format, object args0, object args1)
        {
            Assert(false, format, args0, args1);
        }

        static private void _AssertFailed(string format, object args0, object args1, object args2)
        {
            Assert(false, format, args0, args1, args2);
        }

        static private void _Assert(bool condition, string message)
        {
            if (!condition)
            {
                message = string.Format("Tenmove Assert Failed:{0}", message);
                _LogErrorFormat(message);
                throw new EngineException(message);
            }
        }

        static private void _Assert(bool condition, string format, object arg0)
        {
            if (!condition)
            {
                string message = string.Format(format, arg0);
                message = string.Format("Tenmove Assert Failed:{0}", message);
                _LogErrorFormat(message);
                throw new EngineException(message);
            }
        }

        static private void _Assert(bool condition, string format, object arg0, object arg1)
        {
            if (!condition)
            {
                string message = string.Format(format, arg0, arg1);
                message = string.Format("Tenmove Assert Failed:{0}", message);
                _LogErrorFormat(message);
                throw new EngineException(message);
            }
        }

        static private void _Assert(bool condition, string format, object arg0, object arg1, object arg2)
        {
            if (!condition)
            {
                string message = string.Format(format, arg0, arg1, arg2);
                message = string.Format("Tenmove Assert Failed:{0}", message);
                _LogErrorFormat(message);
                throw new EngineException(message);
            }
        }

        /// <summary>
        /// 这里每次有参数的调用至少会导致40 B gc
        /// 需要new object[n]的空间 32 + 8 * n
        /// by wdd
        /// </summary>
        static private void _Assert(bool condition, string format, params object[] args)
        {
            if (!condition)
            {
                string message = string.Format(format, args);
                message = string.Format("Tenmove Assert Failed:{0}", message);
                _LogErrorFormat(message);
                throw new EngineException(message);
            }
        }
		
        static private void _LogInfo(string info)
        {
            if (m_DisableLevel > (int)DebugLevel.Info)
                return;

            //Console.Log("Info", format, args);
#if UNITY_EDITOR
            UnityEngine.Debug.Log(info);
#else
            UnityEngine.Debug.Log(info);
#endif
        }

        static private void _LogInfoFormat(string format, params object[] args)
        {
            if (m_DisableLevel > (int)DebugLevel.Info)
                return;

            //Console.Log("Info", format, args);
#if UNITY_EDITOR
            UnityEngine.Debug.LogFormat(format, args);
#else
            UnityEngine.Debug.LogFormat(format, args);
#endif
        }

        static private void _LogWarning(string warning)
        {
            if (m_DisableLevel > (int)DebugLevel.Warning)
                return;

            //Console.Log("Warning", format, args);
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(warning);
#else
            UnityEngine.Debug.LogWarning(warning);
#endif
        }

        static private void _LogWarningFormat(string format, params object[] args)
        {
            if (m_DisableLevel > (int)DebugLevel.Warning)
                return;

            //Console.Log("Warning", format, args);
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarningFormat(format, args);
#else
            UnityEngine.Debug.LogWarningFormat(format, args);
#endif
        }

        static private void _LogError(string error)
        {
            if (m_DisableLevel > (int)DebugLevel.Error)
                return;

            //Console.Log("Error", format, args);
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(error);
#else
            UnityEngine.Debug.LogError(error);
#endif
        }

        static private void _LogErrorFormat(string format, params object[] args)
        {
            if (m_DisableLevel > (int)DebugLevel.Error)
                return;

            //Console.Log("Error", format, args);
#if UNITY_EDITOR
            UnityEngine.Debug.LogErrorFormat(format, args);
#else
            UnityEngine.Debug.LogErrorFormat(format, args);
#endif
        }

        static private void _LogExceptionFormat(string format, params object[] args)
        {
            if (m_DisableLevel > (int)DebugLevel.Exception)
                return;

            //Console.Log("Exception", format, args);
#if UNITY_EDITOR
            UnityEngine.Debug.LogErrorFormat(format, args);
#else
            UnityEngine.Debug.LogErrorFormat(format, args);
#endif
        }

        static private void _LogDebugFormat(string format, params object[] args)
        {
            if (m_DisableLevel > (int)DebugLevel.Debug)
                return;

#if UNITY_EDITOR
            //UnityEngine.Debug.LogFormat(format, args);
#endif
        }
    }
}
