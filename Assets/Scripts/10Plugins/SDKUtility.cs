using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace SDKClient
{
    public static class SDKUtility
    {
        public static readonly string SDKDEBUG_HEAD = "###_SDK_###";

        public static bool OPEN_SDK_LOG_WHOLE = false;

        public static string GetDescriptionByName<T>(this T enumItemName)
        {
            FieldInfo fi = enumItemName.GetType().GetField(enumItemName.ToString());
            if (fi == null)
            {
                return enumItemName.ToString();
            }
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return enumItemName.ToString();
            }
        }

        public static void SDKDebugFormat(SDKInterface.DebugType debugType, string logHead, params object[] logContents)
        {
            string logFormat = null;
            if (!OPEN_SDK_LOG_WHOLE)
            {
                logFormat = string.Format("{0}.{1}", SDKDEBUG_HEAD, logHead);
                _ShowSDKLog(debugType, logFormat, logContents);
                return;
            }
            StackTrace st = new StackTrace(true);
            if (st == null || st.GetFrame(1) == null)
            {
                return;
            }
            MethodBase mb = st.GetFrame(1).GetMethod();
            if (mb == null)
            {
                return;
            }
            string methodName = mb.Name;
            string classFullName = mb.DeclaringType.FullName;

            logFormat = string.Format("{0},[{1}.{2}],{3}",SDKDEBUG_HEAD, classFullName, methodName, logHead);
            _ShowSDKLog(debugType, logFormat, logContents);
        }

        private static void _ShowSDKLog(SDKInterface.DebugType debugType, string logFormat, params object[] logContents)
        {
            switch (debugType)
            {
                case SDKInterface.DebugType.NormalMask:
                    Logger.LogFormat(logFormat, logContents);
                    break;
                case SDKInterface.DebugType.NormalNoMask:
                    UnityEngine.Debug.LogFormat(logFormat, logContents);
                    break;
                case SDKInterface.DebugType.WarningMask:
                    Logger.LogWarningFormat(logFormat, logContents);
                    break;
                case SDKInterface.DebugType.WardingNoMask:
                    UnityEngine.Debug.LogWarningFormat(logFormat, logContents);
                    break;
                case SDKInterface.DebugType.ErrorMask:
                    Logger.LogErrorFormat(logFormat, logContents);
                    break;
                case SDKInterface.DebugType.ErrorNoMask:
                    UnityEngine.Debug.LogErrorFormat(logFormat, logContents);
                    break;
            }
        }
    }
}
