using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor.VersionControl;
using System.Collections;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using UnityEngine.Events;
using Debug = System.Diagnostics.Debug;

public class FileTools
{
    public static void GetFullName(GameObject obj,ref string fullname)
    {
        if( obj )
        {
            if( obj.transform.parent )
            {
                GetFullName(obj.transform.parent.gameObject, ref fullname);
                fullname += "/";
            }

            fullname += obj.name;
        }
    }

    static void PrintCallBackInfo(UnityEngine.Events.UnityEventBase eventBase)
    {
        if (eventBase == null)
        {
            return;
        }

        BindingFlags flags = BindingFlags.Instance |
                             BindingFlags.Static |
                             BindingFlags.Public |
                             BindingFlags.NonPublic |
                             BindingFlags.DeclaredOnly;

        FieldInfo[] fieldInfoArray1 = typeof(UnityEngine.Events.UnityEventBase).GetFields(flags);
        if (fieldInfoArray1 == null)
        {
            return;
        }

        FieldInfo fieldInfo1 = fieldInfoArray1.ToList().Find((info) => { return info.Name == "m_Calls"; });
        if (fieldInfo1 == null)
        {
            return;
        }

        object calls = fieldInfo1.GetValue(eventBase);
        if (calls == null)
        {
            return;
        }

        FieldInfo[] fieldInfoArray2 = calls.GetType().GetFields(flags);
        if (fieldInfoArray2 == null)
        {
            return;
        }

        UnityAction printRunTimeCallBackInfo = () =>
        {
            FieldInfo fieldInfo2 = fieldInfoArray2.ToList().Find((info) => { return info.Name == "m_RuntimeCalls"; });
            if (fieldInfo2 == null)
            {
                return;
            }

            object runTimeCalls = fieldInfo2.GetValue(calls);
            if (runTimeCalls == null)
            {
                return;
            }

            if (!(runTimeCalls is IList))
            {
                return;
            }

            foreach (object elem in (IList)runTimeCalls)
            {
                if (elem == null)
                {
                    continue;
                }

                Type t = elem.GetType();
                FieldInfo[] fieldInfos = t.GetFields(flags);
                if (fieldInfos == null)
                {
                    continue;
                }

                FieldInfo temp = fieldInfos.ToList().Find((info) => { return info.Name == "Delegate"; });
                if (temp == null)
                {
                    continue;
                }

                object callback = temp.GetValue(elem);
                if (callback == null)
                {
                    continue;
                }

                System.Delegate d = callback as System.Delegate;
                if (d == null)
                {
                    continue;
                }

                Delegate[] callbacks = d.GetInvocationList();
                if (callbacks == null)
                {
                    continue;
                }

                UnityEngine.Debug.LogError("运行时callback+++++++++++++++++++++++++++++");
                foreach (var call in callbacks)
                {
                    if (call == null)
                    {
                        continue;
                    }

                    string info = string.Format("callback info:target = {0},method = {1}", call.Target, call.Method);
                    UnityEngine.Debug.LogError(info);
                }
                UnityEngine.Debug.LogError("运行时callback+++++++++++++++++++++++++++++");
            }
        };
        printRunTimeCallBackInfo.Invoke();


        UnityAction printPersistentCallBackInfo = () =>
        {
            int count = eventBase.GetPersistentEventCount();
            UnityEngine.Debug.LogError("持久性callback+++++++++++++++++++++++++++++");
            for (int i = 0; i < count; i++)
            {
                string method = eventBase.GetPersistentMethodName(i);
                object target = eventBase.GetPersistentTarget(i);

                string info = string.Format("callback info:target = {0},method = {1}", target, method);
                UnityEngine.Debug.LogError(info);
            }
            UnityEngine.Debug.LogError("持久性callback+++++++++++++++++++++++++++++");
        };
        printPersistentCallBackInfo.Invoke();

        return;
    }

    [MenuItem("GameObject/打印控件注册的所有回调函数信息", false, 30)]
    public static void PrintAllCallBackInfos()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length == 0)
        {
            return;            
        }

        GameObject go = selection[0] as GameObject;
        if (go == null)
        {
            return;
        }

        string path = "";
        GetFullName(go, ref path);

        if (go.GetComponent<Button>() != null)
        {
            UnityEngine.Debug.LogErrorFormat("btn path = {0}", path);
            PrintCallBackInfo(go.GetComponent<Button>().onClick);
        }
        else if (go.GetComponent<Toggle>() != null)
        {
            UnityEngine.Debug.LogErrorFormat("toggle path = {0}", path);
            PrintCallBackInfo(go.GetComponent<Toggle>().onValueChanged);
        }
        else if (go.GetComponent<Slider>() != null)
        {
            UnityEngine.Debug.LogErrorFormat("slider path = {0}", path);
            PrintCallBackInfo(go.GetComponent<Slider>().onValueChanged);
        }
    }

    [MenuItem("GameObject/CopyFullName",false,30)]
    public static void CopyGameObjectFullName()
    {

        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            string path = "";
            GetFullName(selection[0] as GameObject,ref path);
            GUIUtility.systemCopyBuffer = path;
        }

    }

    static GameObject ms_CopyRecursion = null;
    [MenuItem("GameObject/CopyRecursion/RectTransform", false, 10)]
    static void CopyComponentRecursion(MenuCommand menuCommand)
    {
        ms_CopyRecursion = menuCommand.context as GameObject;
    }

    [MenuItem("GameObject/PasteRecursion/RectTransform", false, 10)]
    static void PasteComponentRectTransformRecursion(MenuCommand menuCommand)
    {
        if(null != ms_CopyRecursion && null != menuCommand.context as GameObject)
        {
            Utility.CopyRecursion<RectTransform>(ms_CopyRecursion, menuCommand.context as GameObject);
        }
    }

    [MenuItem("GameObject/PasteRecursion/LetterSpacing", false, 10)]
    static void PasteComponentLetterSpacingRecursion(MenuCommand menuCommand)
    {
        if (null != ms_CopyRecursion && null != menuCommand.context as GameObject)
        {
            Utility.CopyRecursion<UnityEngine.UI.Extensions.LetterSpacing>(ms_CopyRecursion, menuCommand.context as GameObject);
        }
    }

    [MenuItem("GameObject/RemoveRecursion/RemoveLetterSpacing", false, 10)]
    static void RemoveLetterSpacing(MenuCommand menuCommand)
    {
        if (null != menuCommand.context as GameObject)
        {
            _RemoveLetterSpacing(menuCommand.context as GameObject, typeof(UnityEngine.UI.Extensions.LetterSpacing));
        }
    }

    static void _RemoveLetterSpacing(GameObject gameObject,System.Type type)
    {
        if(null == gameObject)
        {
            return;
        }

        var com = gameObject.GetComponent(type);
        if (null != com)
        {
            UnityEngine.Object.DestroyImmediate(com);
        }

        for(int i = 0; i < gameObject.transform.childCount; ++i)
        {
            _RemoveLetterSpacing(gameObject.transform.GetChild(i).gameObject,type);
        }
    }

    static bool _GetFramePathRoot(GameObject go,ref string root,ref string current)
    {
        if(go != null && root != null && current != null)
        {
            root = "";
            current = "";
            string path = "";
            GetFullName(go, ref path);
            var pathTokens = path.Split('/');
            //0/1/2/3/Desc
            //UIRoot / UI2DRoot / Top / AdjustResultFrame / Desc
            if (pathTokens.Length > 4)
            {
                path = "";
                for (int i = 4; i < pathTokens.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        path += "/" + pathTokens[i];
                    }
                    else
                    {
                        path = pathTokens[i];
                    }
                }
                current = path;
                root = pathTokens[0] + "/" + pathTokens[1] + "/" + pathTokens[2] + "/" + pathTokens[3];
                return (!string.IsNullOrEmpty(root)) && (!string.IsNullOrEmpty(current));
            }
        }
        return false;
    }

    static string _GetFramePath(GameObject go)
    {
        string path = "";
        GetFullName(go, ref path);
        var pathTokens = path.Split('/');
        //0/1/2/3/Desc
        //UIRoot / UI2DRoot / Top / AdjustResultFrame / Desc
        if (pathTokens.Length > 4)
        {
            path = "";
            for (int i = 4; i < pathTokens.Length; ++i)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    path += "/" + pathTokens[i];
                }
                else
                {
                    path = pathTokens[i];
                }
            }
        }
        return path;
    }

    static bool GetPath(GameObject child,GameObject parent,ref string realPath)
    {
        realPath = "";

        List<string> paths = new List<string>();
        while (child != null && parent != null && child != parent)
        {
            paths.Add(child.name);
            var transform = child.transform.parent;
            child = transform == null ? null : transform.gameObject;
        }
        if (child != parent)
        {
            return false;
        }

        if (paths.Count > 0)
        {
            for (int k = paths.Count - 1; k >= 0; --k)
            {
                realPath += (realPath.Length > 0 ? "/" : "");
                realPath += paths[k];
            }
        }

        return true;
    }

    [MenuItem("GameObject/CreateFrameScript", false, 1)]
    public static void CreateFrameScript()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if(selection.Length <= 0)
        {
            return;
        }

        GameObject goCurrent = selection[0] as GameObject;
        if (goCurrent == null)
        {
            return;
        }

        var script = goCurrent.GetComponent<ClientObjectFactory>();
        if(script == null)
        {
            Logger.LogErrorFormat("can not find ClientObjectFactory components!");
            return;
        }

        var className = goCurrent.name;
        var kBuilderString = StringBuilderCache.Acquire();
        //import module
        kBuilderString.Append("using UnityEngine;\n");
        kBuilderString.Append("using UnityEngine.UI;\n");
        kBuilderString.Append("using System;\n");
        kBuilderString.Append("using System.Collections;\n");
        kBuilderString.Append("using System.Collections.Generic;\n");

        //global annotation
        kBuilderString.Append("\n//|---------------------------------------------------|");
        kBuilderString.Append("\n//|this code is created by generator do not edit it!!!|");
        kBuilderString.Append("\n//|---------------------------------------------------|\n\n");

        //beg namespace
        kBuilderString.Append("namespace GameClient\n{");

        //beg frame class
        kBuilderString.Append("\nclass " + className + " :ClientFrame\n{");

        //declare frame var
        for(int i = 0; i < script.m_akBinderGroups.Length; ++i)
        {
            var binderGroup = script.m_akBinderGroups[i];
            if(binderGroup == null)
            {
                continue;
            }
            //add annotation
            kBuilderString.Append("\n///<summary>");
            kBuilderString.AppendFormat("\n///module tag = {0}",binderGroup.Tag);
            kBuilderString.Append("\n///</summary>");

            for(int j = 0; j < binderGroup.m_akFrameBinders.Length; ++j)
            {
                var frameBinder = binderGroup.m_akFrameBinders[j];
                if(frameBinder == null)
                {
                    continue;
                }

                var attribute = Utility.GetEnumAttribute<CachedObjectBehavior.UIBinder.BinderType, CachedObjectBehavior.UIBinderAttribute>(frameBinder.eBinderType);
                if (attribute == null)
                {
                    continue;
                }
                kBuilderString.Append("\n" + attribute.Type.Name + " " + frameBinder.varName + ";");
            }
        }

        //frame beg Create
        kBuilderString.Append("\npublic void Create(GameObject frame)\n{");

        for (int i = 0; i < script.m_akBinderGroups.Length; ++i)
        {
            var binderGroup = script.m_akBinderGroups[i];
            if (binderGroup == null)
            {
                continue;
            }

            for (int j = 0; j < binderGroup.m_akFrameBinders.Length; ++j)
            {
                var frameBinder = binderGroup.m_akFrameBinders[j];
                if (frameBinder == null)
                {
                    continue;
                }

                string retValue = "";
                if (!GetPath(frameBinder.goLocal, goCurrent, ref retValue))
                {
                    Logger.LogErrorFormat("frameBinder error child={0} of parent={1} can not find!!", frameBinder.goLocal.name,
                        goCurrent.name);
                    continue;
                }

                var attribute = Utility.GetEnumAttribute<CachedObjectBehavior.UIBinder.BinderType, CachedObjectBehavior.UIBinderAttribute>(frameBinder.eBinderType);
                if (attribute == null)
                {
                    continue;
                }

                if(attribute.Type == typeof(GameObject))
                {
                    kBuilderString.AppendFormat("\n{0} = Utility.FindChild(frame,\"{1}\");", frameBinder.varName, retValue);
                }
                else
                {
                    kBuilderString.AppendFormat("\n{0} = Utility.FindComponent<{1}>(frame,\"{2}\");",frameBinder.varName,attribute.Type.Name,retValue);
                }
            }
        }

        //frame end Create
        kBuilderString.Append("\n}");

        //frame beg Destroy
        kBuilderString.Append("\npublic void Destroy()\n{");
        for (int i = 0; i < script.m_akBinderGroups.Length; ++i)
        {
            var binderGroup = script.m_akBinderGroups[i];
            if (binderGroup == null)
            {
                continue;
            }

            for (int j = 0; j < binderGroup.m_akFrameBinders.Length; ++j)
            {
                var frameBinder = binderGroup.m_akFrameBinders[j];
                if (frameBinder == null)
                {
                    continue;
                }

                string retValue = "";
                if (!GetPath(frameBinder.goLocal, goCurrent, ref retValue))
                {
                    Logger.LogErrorFormat("frameBinder error child={0} of parent={1} can not find!!", frameBinder.goLocal.name,
                        goCurrent.name);
                    continue;
                }

                var attribute = Utility.GetEnumAttribute<CachedObjectBehavior.UIBinder.BinderType, CachedObjectBehavior.UIBinderAttribute>(frameBinder.eBinderType);
                if (attribute == null)
                {
                    continue;
                }

                if (attribute.Type == typeof(Button))
                {
                    kBuilderString.AppendFormat("\n{0}.onClick.RemoveAllListeners();", frameBinder.varName);
                }
                else if (attribute.Type == typeof(Toggle))
                {
                    kBuilderString.AppendFormat("\n{0}.onValueChanged.RemoveAllListeners();", frameBinder.varName);
                }
                kBuilderString.AppendFormat("\n{0} = null;", frameBinder.varName);
            }
        }
        //frame end Destroy
        kBuilderString.Append("\n}");

        //frame getprefabpath
        if (!string.IsNullOrEmpty(script.m_kPrefabPath))
        {
            kBuilderString.Append("\npublic override string GetPrefabPath()\n{");
            kBuilderString.AppendFormat("\nreturn\"{0}\";", script.m_kPrefabPath);
            kBuilderString.Append("\n}");
        }

        //frame beg _OnOpenFrame
        kBuilderString.Append("\nprotected override void _OnOpenFrame()\n{");
        kBuilderString.Append("\nbase._OnOpenFrame();");
        kBuilderString.Append("\nCreate(frame);");
        //frame end _OnOpenFrame
        kBuilderString.Append("\n}");

        //frame beg _OnCloseFrame
        kBuilderString.Append("\nprotected override void _OnCloseFrame()\n{");
        kBuilderString.Append("\nDestroy();");
        kBuilderString.Append("\nbase._OnCloseFrame();");
        //frame end _OnCloseFrame
        kBuilderString.Append("\n}");

        //sub class
        for (int i = 0; i < script.m_akObjects.Length; ++i)
        {
            CachedObjectBehavior curObject = script.m_akObjects[i];
            if(curObject == null || curObject.goPrefab == null)
            {
                Logger.LogError("curObject == null || curObject.goPrefab == null !!!");
                continue;
            }

            if(curObject.goParent == null)
            {
                Logger.LogErrorFormat("goParent is null!");
                continue;
            }

            //beg sub class
            kBuilderString.Append("\nprotected class " + curObject.goParent.name + "\n{");

            //declare var 
            for(int j = 0; j < curObject.uiBinders.Length; ++j)
            {
                CachedObjectBehavior.UIBinder binder = curObject.uiBinders[j];
                if(binder == null || binder.goLocal == null)
                {
                    continue;
                }

                if(binder.eBinderType == CachedObjectBehavior.UIBinder.BinderType.BT_INVALID)
                {
                    continue;
                }

                var attribute = Utility.GetEnumAttribute<CachedObjectBehavior.UIBinder.BinderType, CachedObjectBehavior.UIBinderAttribute>(binder.eBinderType);
                if(attribute == null)
                {
                    continue;
                }

                if(string.IsNullOrEmpty(binder.varName))
                {
                    Logger.LogError("varName is nill!!");
                    continue;
                }

                kBuilderString.Append("\npublic " + attribute.Type.Name + " " + binder.varName + ";");
            }

            string retValue = "";
            if (!GetPath(curObject.goParent, goCurrent, ref retValue))
            {
                Logger.LogErrorFormat("{0} is not child of {1}", curObject.goParent.name, goCurrent.name);
                continue;
            }
            kBuilderString.AppendFormat("\nstring strParentPath = \"{0}\";", retValue);

            retValue = "";
            if (!GetPath(curObject.goPrefab, curObject.goParent, ref retValue))
            {
                Logger.LogErrorFormat("{0} is not child of {1}", curObject.goPrefab.name, curObject.goParent.name);
                continue;
            }
            kBuilderString.AppendFormat("\nstring strPrefabPath = \"{0}\";", retValue);

            //beg GetParent function
            kBuilderString.Append("\n\npublic GameObject GetParent(GameObject frame)\n{");
            kBuilderString.Append("\nreturn Utility.FindChild(frame, strParentPath);");
            //end GetParent function
            kBuilderString.Append("\n}");

            //beg GetPrefab function
            kBuilderString.Append("\n\npublic GameObject GetPrefab(GameObject parent)\n{");
            kBuilderString.Append("\nreturn Utility.FindChild(parent, strPrefabPath);");
            //end GetPrefab function
            kBuilderString.Append("\n}");

            //beg create function
            kBuilderString.Append("\n\npublic void Create(GameObject goLocal)\n{");
            kBuilderString.Append("\nif(goLocal == null) \n{\nreturn;\n}\n");
            for (int j = 0; j < curObject.uiBinders.Length; ++j)
            {
                CachedObjectBehavior.UIBinder binder = curObject.uiBinders[j];
                if (binder == null || binder.goLocal == null)
                {
                    continue;
                }

                if (binder.eBinderType == CachedObjectBehavior.UIBinder.BinderType.BT_INVALID)
                {
                    continue;
                }

                var attribute = Utility.GetEnumAttribute<CachedObjectBehavior.UIBinder.BinderType, CachedObjectBehavior.UIBinderAttribute>(binder.eBinderType);
                if (attribute == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(binder.varName))
                {
                    Logger.LogError("varName is nill!!");
                    continue;
                }

                string realPath = "";
                if(!GetPath(binder.goLocal, curObject.goPrefab,ref realPath))
                {
                    Logger.LogErrorFormat("child can not find in prefab curObject = {0}", curObject.goPrefab.name);
                    continue;
                }

                if(string.IsNullOrEmpty(realPath))
                {
                    kBuilderString.AppendFormat("\n{0} = goLocal.GetComponent<{1}>();", binder.varName, attribute.Type.Name);
                }
                else
                {
                    if (attribute.Type != typeof(GameObject))
                    {
                        kBuilderString.AppendFormat("\n{0} = Utility.FindComponent<{1}>(goLocal,\"{2}\");", binder.varName, attribute.Type.Name, realPath);
                    }
                    else
                    {
                        kBuilderString.AppendFormat("\n{0} = Utility.FindChild(goLocal,\"{1}\");", binder.varName, realPath);
                    }
                }
            }
            //end create function
            kBuilderString.Append("\n}");

            //beg destroy function
            kBuilderString.Append("\n\npublic void Destroy(GameObject goLocal)\n{");
            kBuilderString.Append("\nif(goLocal == null) \n{\nreturn;\n}\n");

            for (int j = 0; j < curObject.uiBinders.Length; ++j)
            {
                CachedObjectBehavior.UIBinder binder = curObject.uiBinders[j];
                if (binder == null || binder.goLocal == null)
                {
                    continue;
                }

                if (binder.eBinderType == CachedObjectBehavior.UIBinder.BinderType.BT_INVALID)
                {
                    continue;
                }

                var attribute = Utility.GetEnumAttribute<CachedObjectBehavior.UIBinder.BinderType, CachedObjectBehavior.UIBinderAttribute>(binder.eBinderType);
                if (attribute == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(binder.varName))
                {
                    Logger.LogError("varName is nill!!");
                    continue;
                }

                if(attribute.Type == typeof(Button))
                {
                    kBuilderString.AppendFormat("\n{0}.onClick.RemoveAllListeners();", binder.varName);
                }
                else if (attribute.Type == typeof(Toggle))
                {
                    kBuilderString.AppendFormat("\n{0}.onValueChanged.RemoveAllListeners();", binder.varName);
                }
                kBuilderString.AppendFormat("\n{0} = null;", binder.varName);
            }

            //end destroy function
            kBuilderString.Append("\n\nstrParentPath = null;");
            kBuilderString.Append("\nstrPrefabPath = null;");
            kBuilderString.Append("\n}");

            //end sub class
            kBuilderString.Append("\n}");
        }

        //end frame class
        kBuilderString.Append("\n}");

        //end namespace
        kBuilderString.Append("\n}");

        var dataPath = Application.dataPath + "/Scripts/05ClientSystem/WrappedFrameAutoScript/" + className + ".cs";
        try
        {
            Write(dataPath, kBuilderString.ToString());
            Logger.LogWarning("create file succeed!!");
        }
        catch(System.Exception e)
        {
            Logger.LogError(e.ToString());
        }
        StringBuilderCache.Release(kBuilderString);
        kBuilderString = null;
    }

    public static void Write(string path,string content)
    {
        FileStream fs = new FileStream(path, File.Exists(path) ? System.IO.FileMode.Truncate : System.IO.FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(content);
        sw.Flush();
        sw.Close();
        fs.Close();
    }

    [MenuItem("GameObject/CopyFrame====>/Path(相对于Frame路径)", false, 1)]
    public static void OnCopyFramePath()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            GUIUtility.systemCopyBuffer = _GetFramePath(selection[0] as GameObject);
        }
    }

    [MenuItem("GameObject/CopyFrame====>/TextPath(Utility.FindComponent<Text>(frame,xxx))", false, 1)]
    public static void OnCopyTextPath()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            string comPath = "";
            GameObject goCurrent = selection[0] as GameObject;
            comPath = _GetFramePath(goCurrent);
            Text goLocal = goCurrent.GetComponent<Text>();
            if (goLocal != null)
            {
                var value = string.Format("Utility.FindComponent<Text>(frame, \"{0}\")", comPath);
                if (!string.IsNullOrEmpty(value))
                {
                    GUIUtility.systemCopyBuffer = value;
                }
            }
            else
            {
                Logger.LogErrorFormat("Text component is not exist in this gameobject!");
            }
        }
    }

    [MenuItem("GameObject/CopyFrame====>/ImagePath(Utility.FindComponent<Image>(frame,xxx))", false, 1)]
    public static void OnCopyImagePath()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            string comPath = "";
            GameObject goCurrent = selection[0] as GameObject;
            comPath = _GetFramePath(goCurrent);
            Image goLocal = goCurrent.GetComponent<Image>();
            if (goLocal != null)
            {
                var value = string.Format("Utility.FindComponent<Image>(frame, \"{0}\")", comPath);
                if (!string.IsNullOrEmpty(value))
                {
                    GUIUtility.systemCopyBuffer = value;
                }
            }
            else
            {
                Logger.LogErrorFormat("Image component is not exist in this gameobject!");
            }
        }
    }

    [MenuItem("GameObject/CopyFrame====>/TogglePath(Utility.FindComponent<Toggle>(frame,xxx))", false, 1)]
    public static void OnCopyTogglePath()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            string comPath = "";
            GameObject goCurrent = selection[0] as GameObject;
            comPath = _GetFramePath(goCurrent);
            Toggle goLocal = goCurrent.GetComponent<Toggle>();
            if (goLocal != null)
            {
                var value = string.Format("Utility.FindComponent<Toggle>(frame, \"{0}\")", comPath);
                if (!string.IsNullOrEmpty(value))
                {
                    GUIUtility.systemCopyBuffer = value;
                }
            }
            else
            {
                Logger.LogErrorFormat("Toggle component is not exist in this gameobject!");
            }
        }
    }

    [MenuItem("GameObject/CopyFrame====>/ButtonPath(Utility.FindComponent<Button>(frame,xxx))", false, 1)]
    public static void OnCopyButtonPath()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            string comPath = "";
            GameObject goCurrent = selection[0] as GameObject;
            comPath = _GetFramePath(goCurrent);
            Button goLocal = goCurrent.GetComponent<Button>();
            if (goLocal != null)
            {
                var value = string.Format("Utility.FindComponent<Button>(frame, \"{0}\")", comPath);
                if (!string.IsNullOrEmpty(value))
                {
                    GUIUtility.systemCopyBuffer = value;
                }
            }
            else
            {
                Logger.LogErrorFormat("Text component is not exist in this gameobject!");
            }
        }
    }

    [MenuItem("GameObject/CopyFrame====>/GameObjectPath(Utility.FindChild(frame,xxx))", false, 1)]
    public static void OnCopyGameObjectPath()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length > 0)
        {
            string comPath = "";
            GameObject goCurrent = selection[0] as GameObject;
            comPath = _GetFramePath(goCurrent);
            var value = string.Format("Utility.FindChild(frame, \"{0}\")", comPath);
            if (!string.IsNullOrEmpty(value))
            {
                GUIUtility.systemCopyBuffer = value;
            }
        }
    }

    [MenuItem("GameObject/CopyFrame====>/CodeCreate(Tabs|tab)", false, 1)]
    public static void OnCopyTabsPrefabCode()
    {
        //1 _pp 表示的是Tabs 父结点
        //2 _o_开始表示的是要初始化获得的初始结点
        // _o_tt text
        // _o_go gameobject
        // _o_bt button
        // _o_te toggle
        // _o_i  image
        Logger.LogErrorFormat("function has not complete!");
        return;

        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.TopLevel);
        if (selection.Length <= 0)
        {
            return;
        }

        GameObject frame = selection[0] as GameObject;
        if (frame == null)
        {
            return;
        }

        var framePath = _GetFramePath(frame);
        var tokenPath = framePath.Split(new char[] { '/'});
        if(tokenPath.Length != 4)
        {
            Logger.LogErrorFormat("please select the frame root gameobject !");
            return;
        }

        var currentObject = new CachedObjectData(frame);
        _CreateChildTabs(currentObject);
    }

    public class CachedChildData
    {
        public GameObject child;
        public string path;
    }

    public class CachedObjectData
    {
        public CachedObjectData(GameObject goParent = null)
        {
            this.goParent = goParent;
            this.goSearchParent = goParent;
            pureObjects = null;
            cachedObject = null;
            localPath = "";
            lastPath = "";
        }
        public GameObject goParent;
        public GameObject goSearchParent;
        public List<CachedChildData> pureObjects;
        public List<CachedObjectData> cachedObject;
        public string localPath;
        public string lastPath;

        public GameObject goLocalParent;
        public string findPath;
    }

    static void _CreateChildTabs(CachedObjectData chachedObject)
    {
        if(chachedObject == null || chachedObject.goParent == null)
        {
            return;
        }

        if(chachedObject.pureObjects == null)
            chachedObject.pureObjects = new List<CachedChildData>();
        if (chachedObject.cachedObject == null)
            chachedObject.cachedObject = new List<CachedObjectData>();

        for(int i = 0; i < chachedObject.goSearchParent.transform.childCount; ++i)
        {
            var child = chachedObject.goSearchParent.transform.GetChild(i);
            if(child.name.StartsWith("_PP_") || child.name.StartsWith("_P_"))
            {
                var current = new CachedObjectData(child.gameObject);
                current.goLocalParent = chachedObject.goParent;
                current.findPath = chachedObject.localPath + "/" +  child.name;

                if(current.findPath.StartsWith("/"))
                {
                    current.findPath = current.findPath.Substring(1, current.findPath.Length - 1);
                }
                _CreateChildTabs(current);
                chachedObject.cachedObject.Add(current);
            }
            else
            {
                if (child.name.StartsWith("_O_"))
                {
                    CachedChildData childData = new CachedChildData();
                    childData.child = child.gameObject;
                    childData.path = chachedObject.localPath + "/" + child.name;
                    chachedObject.pureObjects.Add(childData);
                }
                else
                {
                    chachedObject.localPath += "/" + child.name;
                    GameObject goSearchTemplate = chachedObject.goSearchParent;
                    chachedObject.goSearchParent = child.gameObject;
                    _CreateChildTabs(chachedObject);
                    chachedObject.goSearchParent = goSearchTemplate;
                    chachedObject.localPath = chachedObject.localPath.Substring(0, chachedObject.localPath.Length - child.name.Length - 1);
                }
            }
        }
    }

    static public void GetComponetsInChildrenWithHide<T>(GameObject obj,ref List<T> components) where T : Component
	{
		if (obj == null) 
		{
			return;
		}

		T com = obj.GetComponent<T> ();

		if (com != null) 
		{
			components.Add(com);
		}

		for (int i = 0; i < obj.transform.childCount; ++i) 
		{
			var t = obj.transform.GetChild(i);
			GetComponetsInChildrenWithHide<T>(t.gameObject,ref components);
		}
	}

	[MenuItem("[TM工具集]/ArtTools/DestroyAnimatorNull",false,1)]
	public static void DestroyAnimatorNullName()
	{
		UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.Assets);
		if (selection.Length > 0)
		{
			foreach(GameObject obj in selection)  
			{  
				if( PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab )
				{
					continue;
				}

				UnityEngine.Debug.LogWarning("Process Prefab :" + obj.name + "....\n");
				bool bChange = false;
				List<Animator> coms = new List<Animator>();
				GetComponetsInChildrenWithHide<Animator>(obj,ref coms);
				foreach(Animator LAnimator in coms)
				{
					if(LAnimator.avatar == null && LAnimator.runtimeAnimatorController == null)
					{
						UnityEngine.Debug.LogWarning("Del Null Animator in :" + LAnimator.gameObject.name + "....\n");
						UnityEngine.GameObject.DestroyImmediate(LAnimator,true);
					}
				}
			}
		}

	}

	[MenuItem("[TM工具集]/ArtTools/ActorRShadow",false,2)]
	public static void ActorRShadowName()
	{
		UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.Assets);
		if (selection.Length > 0)
		{
			foreach(GameObject obj in selection)  
			{  
				if( PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab )
				{
					continue;
				}

				UnityEngine.Debug.LogWarning("Process Prefab :" + obj.name + "....\n");
				bool bChange = false;
				List<SkinnedMeshRenderer> coms = new List<SkinnedMeshRenderer>();
				GetComponetsInChildrenWithHide<SkinnedMeshRenderer>(obj,ref coms);
				foreach(SkinnedMeshRenderer RShadow in coms)
				{
					if(RShadow.receiveShadows == true)
					{
						UnityEngine.Debug.LogWarning("Del Null receiveShadows in :" + RShadow.gameObject.name + "....\n");
						RShadow.receiveShadows = false;
					}
				}
			}
		}		
	}

	[MenuItem("[TM工具集]/ArtTools/TextPingFangBoldTo")]
	public static void TextPingFangBoldToName()
	{
		UnityEngine.Object[] selection = Selection.GetFiltered (typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.Assets);
		if (selection.Length > 0)
		{
			foreach(GameObject obj in selection)
			{
				if( PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab )
				{
					continue;
				}
				GameObject temp = PrefabUtility.InstantiatePrefab(obj) as GameObject;
				UnityEngine.Debug.LogWarning("Process Prefab :" + temp.name + "....\n");
				bool bChange = false;
				List<Text> coms = new List<Text>();
				GetComponetsInChildrenWithHide<Text>(temp, ref coms);
				Font toChangeFont = (Font)EditorGUIUtility.Load("GameFontYaHei #3.ttf");
				foreach(Text PText in coms)
				{
					if(PText.font.name == "PingFangBold")
					{
						PText.font = toChangeFont;
						if(PText.font.name == "GameFontYaHei #3")
						{
							UnityEngine.Debug.LogWarning("Process Prefab :" + obj.name + "的" + "PingFangBold字体更改成功\n");

						}
					}
				}
				PrefabUtility.ReplacePrefab(temp,obj,ReplacePrefabOptions.ConnectToPrefab);
				GameObject.DestroyImmediate(temp);
			}
		}
	}

    [MenuItem("[TM工具集]/ArtTools/NullPrefabSpricts",false,4)]
	public static void NullPrefabSprictsName()
	{
		GameObject[] go = Selection.gameObjects;
		string log = "";
		foreach (GameObject g in go)
		{
			FindInGO(g, ref log);
		}
	}




	private static void FindInGO(GameObject g, ref string log)
	{
		Component[] components = g.GetComponents<Component>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] == null)
			{
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null)
				{
					s = t.parent.name + "/" + s;
					
					t = t.parent;
				}
				string logStr = s + " has an empty script attached in position: " + i;
				log += logStr + "....\n";
				UnityEngine.Debug.Log(logStr, g);
				UnityEngine.GameObject.DestroyImmediate(components[i],true);
			}
		}
		 
		foreach (Transform childT in g.transform)
		{
			FindInGO(childT.gameObject, ref log);
		}
	}



    private static string ResourceRootPath = "Assets/Resources/";

    static public string GetAssetFullPath(UnityEngine.Object assets)
    {
        return AssetDatabase.GetAssetPath(assets);
    }

    static public string GetAssetPath(UnityEngine.Object assets)
    {
        StringBuilder path = new StringBuilder(GetAssetFullPath(assets));
        path = path.Replace(ResourceRootPath, "");
        return path.ToString();
    }

    static public string GetAssetLoadPath(UnityEngine.Object assets)
    {
        string path = GetAssetPath(assets);
        if(string.IsNullOrEmpty(path))
        {
            return path;
        }

        int index = path.LastIndexOf(".");
        if(index >= 0)
        path = path.Remove(index);
        return path;
    }
    static public bool IsResourcesAsset(UnityEngine.Object assets)
    {
        string path = GetAssetFullPath(assets);
        return path.IndexOf(ResourceRootPath) == 0;
    }

    public static T CreateAsset<T>(string filename) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = "";

        if (Selection.assetGUIDs.Length > 0)
        {
            path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        }

        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + filename + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();

        EditorGUIUtility.PingObject(asset);
        Selection.activeObject = asset;
        return asset;
    }
}