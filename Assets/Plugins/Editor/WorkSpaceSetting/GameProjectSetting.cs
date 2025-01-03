using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameProjectSetting
{
    static string[] sSymbolDefines = new string[]
    {
        "WORK_DEBUG",
        "LOG_NORMAL",
        "LOG_WARNNING",
        "LOG_PROCESS",
        "LOG_ERROR",
        "LOG_DIALOG",
        "LOG_ASSET",
        "LOG_PROFILE",
        "DEBUG_REPORT_ROOT",
        "LOG_CREATEASSET",
        "BEHAVIAC_RELEASE",
        "BEHAVIAC_NOT_USE_UNITY",
        "BEHAVIAC_NOT_USE_MONOBEHAVIOUR",
        "DEBUG_SETTING",
        "USE_FB",
        "USE_FB_TABLE",
        "LOGIC_SERVER",
        "SERVER_LOGIC",
        "ZJY_SERVER",
        "LOCAL_RECORD_FILE_REPLAY",
        "NEWADD_COMBIND",
        "LOGIC_SOURCE_DEBUG",
    };

    static string[] sLogicServerDefines = new string[]
    {
        "LOG_ERROR",
        "BEHAVIAC_RELEASE",
        "BEHAVIAC_NOT_USE_UNITY",
        "BEHAVIAC_NOT_USE_MONOBEHAVIOUR",
        "USE_FB",
        "LOGIC_SERVER",
        "SERVER_LOGIC",
        "USE_FB_TABLE",
    };

    static string[] sGameClientRecordReplayDefines = new string[]
      {
        "LOG_ERROR",
        "BEHAVIAC_RELEASE",
        "DEBUG_SETTING",
        "BEHAVIAC_NOT_USE_MONOBEHAVIOUR",
        "LOCAL_RECORD_FILE_REPLAY",
        "USE_FB_TABLE",
   };



    static string[] sGameClientDefines = new string[]
   {
        "LOG_ERROR",
        "BEHAVIAC_RELEASE",
        "DEBUG_SETTING",
        "BEHAVIAC_NOT_USE_MONOBEHAVIOUR",
        "USE_FB_TABLE",
   };

    static string[] szjyGameClientDefines = new string[]
   {
        "LOG_ERROR",
        "BEHAVIAC_RELEASE",
        "BEHAVIAC_NOT_USE_MONOBEHAVIOUR",
        "ZJY_SERVER"
   };

    static string[] sGameClientNewComBindDefines = new string[]
        {
            "NEWADD_COMBIND",
        };

    static GameProjectSetting()
    {
        // Event for when script is reloaded 
        System.AppDomain.CurrentDomain.DomainUnload += System_AppDomain_CurrentDomain_DomainUnload;
    }

    private static bool sIsShowFilter = false;


    static void System_AppDomain_CurrentDomain_DomainUnload(object sender, System.EventArgs e)
    {

        //UpdateSymbolDefines();
        System.Type T = System.Type.GetType("UnityEditor.PreferencesWindow,UnityEditor");

        if (EditorWindow.focusedWindow == null)
            return;

        var window = EditorWindow.GetWindow(T);

        // Only run this when the editor window is visible (cause its what screwed us up)
        if (EditorWindow.focusedWindow.GetType() == T)
        {
            //var window = EditorWindow.GetWindow(T, true, "Unity Preferences");


            if (window == null)
            {
                return;
            }

            if (EditorWindow.focusedWindow == window)
            {
                window.Repaint();
            }
        }
    }

    static bool[] sSymbolDefinesFlag = new bool[0];
    static string sSymbolDefinesString;
    static char[] sSplit = new char[] { ';', ',', ' ' };
    static void UpdateSymbolDefines(string update = null)
    {
        //if (sSymbolDefinesFlag.Length != sSymbolDefines.Length)
        {
            sSymbolDefinesString = update == null ? PlayerSettings.GetScriptingDefineSymbolsForGroup(GetCurrentBuildTargetGroup()) : update;
            sSymbolDefinesFlag = new bool[sSymbolDefines.Length];

            string[] list = sSymbolDefinesString.Split(sSplit, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sSymbolDefines.Length; ++i)
            {
                var current = sSymbolDefines[i];
                sSymbolDefinesFlag[i] = (Array.IndexOf(list, current) >= 0);
            }
        }
    }

    static void SaveSymbolDefines()
    {
        for (int i = 0; i < sSymbolDefines.Length; ++i)
        {
            var current = sSymbolDefines[i];
            EditorPrefs.SetBool(current, sSymbolDefinesFlag[i]);
        }
    }
    static bool bNeedRefreashChanging = false;

    static void SetPredefineSymbols(string[] def)
    {
        string strSymbolDefines = "";
        for (int i = 0; i < def.Length; ++i)
        {
            //if (sSymbolDefinesFlag[i])
            {
                strSymbolDefines += def[i];
                strSymbolDefines += ";";
            }
        }

        if (sSymbolDefinesString != strSymbolDefines)
        {
            sSymbolDefinesString = strSymbolDefines;
            UpdateSymbolDefines(sSymbolDefinesString);
            bNeedRefreashChanging = true;
        }
    }

    static BuildTargetGroup GetCurrentBuildTargetGroup()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        return BuildPipeline.GetBuildTargetGroup(target);
    }
    static void SetSymbolDefines()
    {
        string strSymbolDefines = "";

        for (int i = 0; i < sSymbolDefines.Length; ++i)
        {
            if (sSymbolDefinesFlag[i])
            {
                strSymbolDefines += sSymbolDefines[i];
                strSymbolDefines += ";";
            }
        }

        if (sSymbolDefinesString != strSymbolDefines)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(GetCurrentBuildTargetGroup(), strSymbolDefines);
            sSymbolDefinesString = strSymbolDefines;
        }
    }

    private static string mRecordPath = string.Empty;

    [PreferenceItem("ProjectSetting")]
    static void ProjectSettingPreferences()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.HelpBox("GameProjectSetting.", MessageType.Info);

        if(sSymbolDefinesFlag == null || sSymbolDefinesFlag.Length <= 0)
        {
            UpdateSymbolDefines();
        }
        //bNeedRefreashChanging = false;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("ScriptSysmbolDefine:");
        for (int i = 0; i < sSymbolDefines.Length; ++i)
        {
            sSymbolDefinesFlag[i] = EditorGUILayout.Toggle(sSymbolDefines[i], sSymbolDefinesFlag[i]);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            //bDirty = true;
        }

        EditorGUI.BeginChangeCheck();
        if (!(EditorApplication.isCompiling))
        {
            //mRecordPath = EditorGUILayout.TextField("本地录像地址", mRecordPath);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("GameClient"))
            {
                SetPredefineSymbols(sGameClientDefines);
                bNeedRefreashChanging = true;
            }
            if (GUILayout.Button("ZJYGameClient"))
            {
                SetPredefineSymbols(szjyGameClientDefines);
                bNeedRefreashChanging = true;
            }
            if (GUILayout.Button("LogicServer"))
            {
                SetPredefineSymbols(sLogicServerDefines);
                bNeedRefreashChanging = true;
            }
            
            if (GUILayout.Button("LogicServer .net调试"))
            {
                List<string> array = new List<string>(sLogicServerDefines);
                array.Add("LOGIC_SOURCE_DEBUG");
                SetPredefineSymbols(array.ToArray());
                bNeedRefreashChanging = true;
            }
            
            if (GUILayout.Button("本地录像回放"))
            {
                SetPredefineSymbols(sGameClientRecordReplayDefines);
                bNeedRefreashChanging = true;
            }
            if (GUILayout.Button("NewComBind"))
            {
                string[] defines = new string[sGameClientDefines.Length + sGameClientNewComBindDefines.Length];
                sGameClientDefines.CopyTo(defines, 0);
                sGameClientNewComBindDefines.CopyTo(defines, sGameClientDefines.Length);
                SetPredefineSymbols(defines);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Apply"))
            {
                SetSymbolDefines();
                //bDirty = true;
                bNeedRefreashChanging = true;
            }


            if (GUILayout.Button("Editor Settings"))
            {
                var obj = AssetDatabase.LoadAssetAtPath("Assets/Plugins/Editor/WorkSpaceSetting/GameProjectSetting.cs", typeof(UnityEngine.Object));
                AssetDatabase.OpenAsset(obj, 10);
            }
        }
        else
        {
            EditorGUILayout.HelpBox(">>>>>>Waitting For Script Compiling!!<<<<<<", MessageType.Warning);
        }
        if (EditorGUI.EndChangeCheck())
        {
            //bDirty = true;
        }
        else
        {
            if (bNeedRefreashChanging == true)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(GetCurrentBuildTargetGroup(), sSymbolDefinesString);
                bNeedRefreashChanging = false;
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();


    }
}
