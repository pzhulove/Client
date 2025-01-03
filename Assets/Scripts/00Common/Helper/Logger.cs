using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

public class LoggerModelAttribute : Attribute
{
    public LoggerModelAttribute(string name)
    {
        mName = name;
    }

    public string mName;
}

public class Logger
{
    public static bool enable = false;

    private class LoggerUnit
    {
        private static string sMethmodName;
        private static string sFileName;
        private static string sClassName;
        private static string sModelName;
        private static int sLineNumber;

        private static bool sInited = false;
        private static Dictionary<string, string> mTypeModeDict = new Dictionary<string, string>();
        //private static List<Regex> mRegexList = new List<Regex>();

        private const int kMaxRegexCount = 4;
        private static string[] sFilterStr = Global.Settings.loggerFilter;

        public static void _loadRegex()
        {
            //mRegexList.Clear();

            //try
            //{
            //    for (int i = 0; i < kMaxRegexCount; i++)
            //    {
            //        mRegexList.Add(new Regex(@sFilterStr[i]));
            //    }
            //}
            //catch (Exception e)
            //{
            //    UnityEngine.Debug.LogError(e.ToString());
            //}
        }

        private static void _loadAttribute()
        {
            mTypeModeDict.Clear();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var typeName = type.ToString();
                    {
                        var attrs = type.GetCustomAttributes(typeof(LoggerModelAttribute), true) as LoggerModelAttribute[];
                        if (attrs.Length > 0)
                        {
                            foreach (var item in attrs)
                            {
                                if (!mTypeModeDict.ContainsKey(typeName))
                                {
                                    mTypeModeDict.Add(typeName, item.mName);
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Init()
        {
            _loadAttribute();
            _loadRegex();
        }

        private static bool _regexMatch()
        {
#if UNITY_STANDALONE
            //int len = mRegexList.Count;

            //try
            //{
            //    if (len > 0 && !mRegexList[0].IsMatch(sMethmodName))
            //    {
            //        return false;
            //    }

            //    if (len > 1 && !mRegexList[1].IsMatch(sFileName))
            //    {
            //        return false;
            //    }

            //    if (len > 2 && !mRegexList[2].IsMatch(sClassName))
            //    {
            //        return false;
            //    }

            //    if (len > 3 && !mRegexList[3].IsMatch(sModelName))
            //    {
            //        return false;
            //    }
            //}
            //catch (Exception e)
            //{
            //    UnityEngine.Debug.LogError(e.ToString());
            //    return true;
            //}
#endif
            return true;
        }

        private static string _formatTag()
        {
//#if UNITY_EDITOR
            //return string.Format("[{0} {1}({2})]", sMethmodName, sFileName, sLineNumber);
            return string.Format("[{0}] [{1} {2}({3})]", System.DateTime.Now.ToString("yy/MM/dd hh:mm:ss"), sMethmodName, sFileName, sLineNumber);
            //return string.Format("[{0}][<color=green>{1}</color> <color=#B0B0B0FF>{2}</color>(<color=orange>{3}</color>)]", System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), sMethmodName, sFileName, sLineNumber);
//#else
            //return string.Format("[{0} {1}({2})]", sMethmodName, sFileName, sLineNumber);
//#endif
        }

        private static string _formatModel()
        {
#if UNITY_EDITOR
            //return string.Format("[<color=#B0B0B0FF>{0}</color> <color=orange>{1}</color>]", sClassName, sModelName);
            return string.Format("[{0} {1}]", sClassName, sModelName);
#else
            return string.Format("[{0} {1}]", sClassName, sModelName);
#endif
        }

        public static bool GetTag(ref string output, bool withAllStack = false, int offset = 2)
        {
            StackTrace stackInfo = new StackTrace(true);
            StackFrame frame;

            if (stackInfo.FrameCount < offset + 1)
            {
                return false;
            }

            bool first = true;
            for (int i = offset; i < stackInfo.FrameCount; i++)
            {

                frame = stackInfo.GetFrame(i);

                sFileName = System.IO.Path.GetFileName(frame.GetFileName());

                if (sFileName == null)
                {
                    sFileName = frame.GetFileName();
                }

                sMethmodName = frame.GetMethod().Name;
                sLineNumber = frame.GetFileLineNumber();

                if (!withAllStack)
                {
                    sClassName = frame.GetMethod().DeclaringType.ToString();
                    sModelName = sClassName;

                    if (mTypeModeDict.ContainsKey(sClassName))
                    {
                        sModelName = mTypeModeDict[sClassName];
                    }
                    else
                    {
                        sModelName = "_other";
                    }

                    if (!_regexMatch())
                    {
                        return false;
                    }

                    output = string.Format("{0} {1}", _formatTag(), _formatModel());
                    break;
                }
                else
                {
                    if (first)
                    {
                        output = _formatTag();
                        first = false;
                    }
                    else
                    {
                        output = string.Format("{0}\n{1}", output, _formatTag());
                    }
                }
            }

            return true;
        }
    }

    internal static void Log(string v1, float v2)
    {
        throw new NotImplementedException();
    }

    private static string[] sColorList =
    {
        "#C7301CFF",
        "#EAB101FF",
        "#EAB101FF",
        "#B0B0B0FF",
        "#EAB101FF",
    };

    [Conditional("WORK_DEBUG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void Init()
    {
        //关闭LoggerUnit
        //LoggerUnit.Init();
    }

	[Conditional("LOG_DIALOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_IOS"), Conditional("LOG_ERROR")]
    public static void DisplayLog(string info, UnityEngine.Events.UnityAction onOKCallBack = null)
    {
        MessageBox(info, onOKCallBack); 
    }

    [Conditional("UNITY_EDITOR")]
    public static void EditorLogWarning(string str, params object[] args)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogWarningFormat(_formatString(LogType.Log, str, args));  
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, _formatString(LogType.Log, str, args));
#endif
        }
    }

    [Conditional("LOG_DIALOG")]
    public static void MessageBox(string info, UnityEngine.Events.UnityAction onOKCallBack = null)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var root = Utility.FindGameObject("AlertBoxCanvas", false);
        if (root == null)
        {
            root = new GameObject("AlertBoxCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            root.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            root.transform.SetAsLastSibling();
        }
        //var goDebugObj = AssetLoader.instance.CreateObject("UI/Prefabs/CommonMsgBoxOK") as GameObject;
        GameObject goDebugObj = AssetLoader.GetInstance().LoadResAsGameObject("Base/UI/Prefabs/BaseMsgBoxOK");
        Utility.AttachTo(goDebugObj, root);

        if (onOKCallBack != null)
        {
            var button = Utility.FindComponent<UnityEngine.UI.Button>(goDebugObj, "loading/Panel/button", false);
            button.onClick.AddListener(onOKCallBack);
        }

        goDebugObj.GetComponent<AlertBox>().SetMessage(info);
        goDebugObj = null;
    }

	[Conditional("WORK_DEBUG"), Conditional("LOG_DIALOG"), Conditional("LOG_ERROR")]
    public static void ShowDailog(string str, params object[] args)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag, true, 3))
        {
            var info = string.Format(str, args).TrimEnd();
            DisplayLog(string.Format("{0}\n{1}", info, tag));
        }
    }

    private static string _formatString(LogType type, string str, params object[] args)
    {
        //string colorstr = sColorList[(int)type];

        string var = string.Format(str, args);
        return var + "\n";

        //var = var.Replace("\n", string.Format("</color>\n<color={0}>", colorstr));
        //return string.Format("<color={0}>{1}</color>\n", colorstr, var.Trim()); 
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_NORMAL")]
    public static void Log(string str)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(LogType.Log, str, new object[] { }));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, new object[] { })));
#endif
        }
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_NORMAL")]
    public static void LogFormat(string str, params object[] args)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(LogType.Log, str, args));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, args)));
#endif
        }
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogWarning(string str)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(LogType.Warning, str, new object[] { }));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, new object[] { })));
#endif
        }
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogWarningFormat(string str, params object[] args)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(LogType.Warning, str, args));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, args)));
#endif
        }
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_PROCESS")]
    public static void LogProcessFormat(string str, params object[] args)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(LogType.Warning, str, args));
#else
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format(str, args));
#endif
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogErrorCode(uint error)
    {
        var tag = "";
        var str = Utility.ProtocolErrorString(error);

        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(LogType.Error, str));
#else
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("{0} {1}", tag, _formatString(LogType.Error, str)));
#endif
        }

        ShowDailog(str, new object[] { });
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogError(string str)
    {
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        var tag = "";
        tag = string.Format("[{0}]:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), str);
        UnityEngine.Debug.LogError(tag);
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, str);
#endif
        //ShowDailog(str, new object[] { });
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogErrorFormat(string str, params object[] args)
    {
        var tag = "";
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        tag = string.Format("[{0}]:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), str);
        UnityEngine.Debug.LogErrorFormat(tag, args);
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format(str, args));
#endif
    }


    [Conditional("LOG_ASSET")]
    public static void LogAsset(string str )
    {
        var tag = "";
        LogType type = LogType.Warning;
        //if (LoggerUnit.GetTag(ref tag))
        {
            switch(type)
            {
                case LogType.Log:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(type, str));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag, _formatString(type, str)));
#endif
                    break;
                case LogType.Warning:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(type, str));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag, _formatString(type, str)));
#endif
                    break;
                case LogType.Error:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(type, str));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("{0} {1}", tag, _formatString(type, str)));
#endif
                    break;
            }
        }
    }

    [Conditional("LOG_ASSET")]
    public static void LogAssetFormat(string str, params object[] args)
    {
        var tag = "";
        LogType type = LogType.Warning;
        //if (LoggerUnit.GetTag(ref tag))
        {
            switch (type)
            {
                case LogType.Log:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(type, str, args));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag, _formatString(type, str, args)));
#endif
                    break;
                case LogType.Warning:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(type, str, args));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag, _formatString(type, str, args)));
#endif
                    break;
                case LogType.Error:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(type, str, args));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("{0} {1}", tag, _formatString(type, str, args)));
#endif
                    break;
            }
        }
    }

    [Conditional("LOG_PROFILE")]
    public static void LogProfile(string str)
    {
        var tag = "";
        LogType type = LogType.Warning;
        //if (LoggerUnit.GetTag(ref tag))
        {
            switch (type)
            {
                case LogType.Log:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(type, str));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag, _formatString(type, str)));
#endif
                    break;
                case LogType.Warning:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(type, str));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag, _formatString(type, str)));
#endif
                    break;
                case LogType.Error:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(type, str));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("{0} {1}", tag, _formatString(type, str)));
#endif
                    break;
            }
        }
    }

    [Conditional("LOG_PROFILE")]
    public static void LogProfileFormat(string str, params object[] args)
    {
        var tag = "";
        LogType type = LogType.Warning;
        //if (LoggerUnit.GetTag(ref tag))
        {
            switch (type)
            {
                case LogType.Log:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(type, str, args));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag, _formatString(type, str, args)));
#endif
                    break;
                case LogType.Warning:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(type, str, args));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag, _formatString(type, str, args)));
#endif
                    break;
                case LogType.Error:
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
                    UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(type, str, args));
#else
                    LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("{0} {1}", tag, _formatString(type, str, args)));
#endif
                    break;
            }
        }
    }

    [Conditional("WORK_DEBUG")]
    public static void Break()
    {

    }

	[Conditional("WORK_DEBUG")]
    public static void LogForNet(string str, params object[] args)
    {
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogWarningFormat(str, args);
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format(str, args));
#endif

    }

    [Conditional("WORK_DEBUG")]
    public static void LogForAI(string str, params object[] args)
    {
        if (!BeAIManager.logerror)
            return;

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogErrorFormat(str, args);
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format(str, args));
#endif
    }

	public static void LogForReplay(string str,  params object[] args)
	{
		if (!Global.Settings.isLogRecord)
			return;


#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogErrorFormat(str, args);
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format(str, args));
#endif
	}
}

