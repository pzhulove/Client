using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
#endif
using System;
using System.IO;
using System.Reflection;
using UnityEditor.Callbacks;
using AssetBundleTool;

using System.Linq;
using System.Text;

public class UnityCommandArgumentParse
{
    private enum eState
    {
        onNone,
        onStart,
        onEnd,
    }

    public static string GetFuctionArgs(int idx, string def)
    {
        string[] args = GetFuctionArgs();
        if (args.Length > idx)
        {
            return args[idx];
        }

        return def;
    }

    public static string[] GetFuctionArgs()
    {
        eState state = eState.onNone;

        string[] args = System.Environment.GetCommandLineArgs();

        List<string> funcArgs = new List<string>();

        for (int i = 0; i < args.Length; ++i)
        {
            switch (state)
            {
                case eState.onNone:
                    if (args[i] == "-executeMethod")
                    {
                        state = eState.onStart;
                    }
                    break;
                case eState.onStart:
                    {
                        if (!args[i].StartsWith("-"))
                        {
                            funcArgs.Add(args[i]);
                        }
                        else 
                        {
                            state = eState.onEnd;
                            return funcArgs.ToArray();
                        }
                    }
                    break;
            }
        }
        return new string[0];
    }
}


public class BuildPlayer : MonoBehaviour 
{
#if UNITY_IOS
    private static object _getPBXObject(object project, string key)
    {
        Type type = project.GetType();
        bool isContain = (bool)type.InvokeMember("HasEntry",BindingFlags.GetField | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic, null, project, new object[] {key});

        if (isContain)
        {
            FieldInfo info = type.GetField("m_Entries", BindingFlags.NonPublic | BindingFlags.Public);
            return info.GetValue((object)key);
        }

        UnityEngine.Debug.LogErrorFormat("can't find the key {0}", key);

        return null;
    }


    private static FieldInfo _getFiled(object project, string key)
    {
        Type type = project.GetType ();
        return type.GetField(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic);
    }

    private static object _getDict(object dic, string key)
    {
        PropertyInfo fkinfo = dic.GetType().GetProperty("Item", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
        return fkinfo.GetValue(dic, new object[] {key});
    }


	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if (target == BuildTarget.iOS) 
		{
			UnityEngine.Debug.Log ("path " + pathToBuiltProject);

			var pbxpath = PBXProject.GetPBXProjectPath (pathToBuiltProject);
			var pbx = new PBXProject ();

			pbx.ReadFromFile (pbxpath);

            var unityTargetName = PBXProject.GetUnityTargetName ();
			var guid = pbx.TargetGuidByName (unityTargetName);
#if UNITY_2018
            if (!pbx.ContainsFramework(guid, "libz.tbd"))
#else
            if (!pbx.HasFramework ("libz.tbd")) 
#endif
			{
				
				try {
					Type type = pbx.GetType ();
					PropertyInfo proInfo = type.GetProperty ("project", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
					object project = proInfo.GetValue (pbx, null);

                    PropertyInfo info = project.GetType().GetProperty("project", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
                    object pobj = info.GetValue(project, null);

                    FieldInfo filds = _getFiled(pobj, "m_Properties");
					object dict = filds.GetValue(pobj);

					FieldInfo realDic = _getFiled(dict, "m_PrivateValue");
					object fk = realDic.GetValue(dict);

					object attribute = _getDict(fk, "attributes");
					object tarattr = _getDict(attribute, "TargetAttributes");

					object reDict = tarattr.GetType().InvokeMember("CreateDict", BindingFlags.InvokeMethod | BindingFlags.Public, null, tarattr, new object[] {guid});

					reDict.GetType().InvokeMember("SetString", BindingFlags.InvokeMethod | BindingFlags.Public, null, reDict, new object[] {"ProvisioningStyle", "Manual"});
			

				} catch (Exception e) {

				}
				pbx.WriteToFile (pbxpath);
			}
		}

	}


    public static void Test()
    {
        OnPostprocessBuild(BuildTarget.iOS, "DNF");
    }
#endif

#if UNITY_ANDROID
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.Android)
        {
            PostProcessAndroidBuild(pathToBuiltProject);
        }
    }
#endif
    public static void PostProcessAndroidBuild(string pathToBuiltProject)
    {
		UnityEditor.ScriptingImplementation backend = (UnityEditor.ScriptingImplementation)UnityEditor.PlayerSettings.GetScriptingBackend (UnityEditor.BuildTargetGroup.Android);
        if (backend == UnityEditor.ScriptingImplementation.IL2CPP)
        {
            CopyAndroidIL2CPPSymbols(pathToBuiltProject, PlayerSettings.Android.targetArchitectures);
        }
    }

    public static void CopyAndroidIL2CPPSymbols(string pathToBuiltProject, AndroidArchitecture targetDevice)
    {
        string buildName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        FileInfo fileInfo = new FileInfo(pathToBuiltProject);
        string symbolsDir = fileInfo.Directory.Name;
        symbolsDir = symbolsDir + "/"+buildName+"_IL2CPPSymbols";
        string il2cpp_folder = Application.dataPath + "/../Temp/StagingArea/Il2Cpp/il2cppOutput";
        string savename = "source_il2cpp";

        CreateDir(symbolsDir);

        switch (PlayerSettings.Android.targetArchitectures)
        {
            case AndroidArchitecture.All:
                {
                    CopyARMSymbols(symbolsDir);
                    CopyX86Symbols(symbolsDir);
                    break;
                }
            case AndroidArchitecture.ARMv7:
                {
                    CopyARMSymbols(symbolsDir);
                    break;
                }
#if UNITY_2019_3_OR_NEWER
#else
            case AndroidArchitecture.X86:
                {
                    CopyX86Symbols(symbolsDir);
                    break;
                }
#endif
            default:
                break;
        }
        if(Directory.Exists(il2cpp_folder))
        {
            string savepath = symbolsDir + "/" + savename;
            CreateDir(savepath);
            CopyTree(il2cpp_folder,savepath);
        }
    }


    const string libpath = "/../Temp/StagingArea/symbols/";
    const string libFilename = "libil2cpp.so.debug";
    private static void CopyARMSymbols(string symbolsDir)
    {
		string sourcefileARM = Application.dataPath + libpath + "armeabi-v7a/";// + libFilename;
        CreateDir(symbolsDir + "/armeabi-v7a/");
		CopyTree(sourcefileARM, symbolsDir + "/armeabi-v7a/");

        //File.Copy(sourcefileARM, symbolsDir + "/armeabi-v7a/libil2cpp.so.debug");
    }

    private static void CopyX86Symbols(string symbolsDir)
    {
		string sourcefileX86 = Application.dataPath + libpath + "x86/";//libil2cpp.so.debug";
		CopyTree(sourcefileX86, symbolsDir + "/x86/");
        //File.Copy(sourcefileX86, symbolsDir + "/x86/libil2cpp.so.debug");
    }

	private static void CopyTree(string src, string dst)
	{
		string[] allFiles = Directory.GetFiles(src, "*", SearchOption.AllDirectories);

		for (int i = 0; i < allFiles.Length; ++i) {
			string srcPath = allFiles [i];

			string destPath = srcPath.Replace (src, dst);

			if (File.Exists (srcPath)) 
			{
				CreateDir(Path.GetDirectoryName (destPath));

				if (File.Exists (destPath)) 
				{
					File.Delete (destPath);
				}

				File.Copy(srcPath, destPath);
			}
		}
	}


    public static void CreateDir(string path)
    {
        if (Directory.Exists(path))
            return;

        Directory.CreateDirectory(path);
    }


    public static void BuildAllAssets()
    {
        string isIncremental = "false";//UnityCommandArgumentParse.GetFuctionArgs(1, "true");
        BuildPackage.IncrementBuildBundle(isIncremental == "true");
        // AssetBundlePackerStrategyWindow.BuildPackageInterface(isIncremental == "true");

        /// 老的1.0打包
        //AssetPacker.PackAsset_CommandMode();
    }

    public static void BuildOnlyDataAsset()
    {
        AssetPacker.PackDataAsset_CommandMode();
    }

    private static string[] _updateSceneList()
    {
        List<string> list = new List<string>();
        int count = EditorBuildSettings.scenes.Length;
        for (int i = 0; i < count; ++i)
        {
            var scene = EditorBuildSettings.scenes[i];

            if (scene.enabled && File.Exists(scene.path))
            {
                list.Add(scene.path);
                UnityEngine.Debug.LogFormat("add scene with path {0}", scene.path);
            }
            else 
            {
                UnityEngine.Debug.LogErrorFormat("scene not exit with name {0}", scene.path);
            }
        }

        return list.ToArray();
    }

    public static void OpenCSharpProject()
    {
        UnityEditor.EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
    }
    public static void Refresh()
    {
        UnityEngine.Debug.LogFormat("Refresh");
    }
    //[MenuItem("[Build]/[OpenMainSceneAddUWAPrefab]")]
    public static void OpenMainSceneAddUWAPrefab()
    {
        string type = UnityCommandArgumentParse.GetFuctionArgs(1, "None");
        bool isMonoManual = UnityCommandArgumentParse.GetFuctionArgs(2, "false") == "true";
        
        //string[] SCENE_LIST = _updateSceneList();

        string prefabPath = "Assets/UWA/Prefabs/UWA_Launcher.prefab";
        if (!System.IO.File.Exists(prefabPath))
        {
            return;
        }

        // Assets/UWA/Prefabs/UWA_Launcher.prefab
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");


        var res = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (null == res)
        {
            return;
        }

        GameObject obj = GameObject.Instantiate(res);
        if (null == obj)
        {
            return;
        }

        obj = UnityEditor.PrefabUtility.ConnectGameObjectToPrefab(obj, res);

        if (type == "Mono" && isMonoManual)
        {
            Component com = obj.GetComponent("UWA_Launcher");
            if (null != com)
            {
                SerializedObject so = new SerializedObject(com);
                var propty = so.FindProperty("DirectManualMono");
                if (null != propty)
                {
                    propty.boolValue = true;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
                else 
                {
                    UnityEngine.Debug.LogErrorFormat("DirectManualMono not found!");
                }
            }
            else 
            {
                UnityEngine.Debug.LogErrorFormat("UWA_Launcher not found!");
            }
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }


    //[MenuItem("[Build]/[BuildTarget]")]
	public static void BuildTargets()
	{
        string path = UnityCommandArgumentParse.GetFuctionArgs(1, "./DNF");
        string autoConnect = UnityCommandArgumentParse.GetFuctionArgs(2, "false");
        string platform = UnityCommandArgumentParse.GetFuctionArgs(3, "Win");

        path = Path.GetFullPath(path);

        UnityEngine.Debug.LogFormat("[BuildPlayer] Path {0} Target {1}, Platform: {2}", path, Application.platform, platform);

        string[] SCENE_LIST = _updateSceneList();

#if UNITY_IOS
		BuildPipeline.BuildPlayer(SCENE_LIST, path, BuildTarget.iOS, _getBuildOptions(autoConnect == "true"));
#elif UNITY_ANDROID
		PlayerSettings.Android.keyaliasPass = "123456";
		PlayerSettings.Android.keystorePass = "123456";
		BuildPipeline.BuildPlayer(SCENE_LIST, path+".apk", BuildTarget.Android, _getBuildOptions(autoConnect == "true"));
#else
        if (platform.ToLower().Equals("win64"))
        {
            BuildPipeline.BuildPlayer(SCENE_LIST, path+".exe", BuildTarget.StandaloneWindows64, _getBuildOptions(autoConnect == "true"));
        }
        else if ( platform.ToLower().Equals("osxuniversal"))
        {
            BuildPipeline.BuildPlayer(SCENE_LIST, path+".app", BuildTarget.StandaloneOSX, _getBuildOptions(autoConnect == "true"));
        }
#endif
	}

    public static void BuildTargetsWithOnlyScripts()
	{
        string path = UnityCommandArgumentParse.GetFuctionArgs(1, "./DNF");

        path = Path.GetFullPath(path);

        UnityEngine.Debug.LogFormat("[BuildPlayer] Path {0} Target {1}", path, Application.platform);

        string[] SCENE_LIST = _updateSceneList();

        BuildOptions buildOp = _getBuildOptions();

        buildOp |= BuildOptions.AcceptExternalModificationsToPlayer;
        buildOp |= BuildOptions.BuildScriptsOnly;

#if UNITY_IOS
		BuildPipeline.BuildPlayer(SCENE_LIST, path, BuildTarget.iOS, buildOp);
#elif UNITY_ANDROID
		PlayerSettings.Android.keyaliasPass = "123456";
		PlayerSettings.Android.keystorePass = "123456";
		BuildPipeline.BuildPlayer(SCENE_LIST, path, BuildTarget.Android, buildOp);
#elif UNITY_EDITOR_OSX
#elif UNITY_EDITOR_WIN
#else
#endif
	}


    private static BuildOptions _getBuildOptions(bool isAutoconnect = false)
    {
        BuildOptions op = BuildOptions.None;

        if (EditorUserBuildSettings.connectProfiler && isAutoconnect)
        {
            op |= BuildOptions.ConnectWithProfiler;

            UnityEngine.Debug.LogFormat("[BuildPlayer] connnect with profiler");
        }

        if ( EditorUserBuildSettings.development)
        {
            op |= BuildOptions.Development;

            UnityEngine.Debug.LogFormat("[BuildPlayer] development build");
        }

        return op;
    }

    public static void BuildMarcoConfig()
    {
        string marco = UnityCommandArgumentParse.GetFuctionArgs(1, "LOG_ERROR");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, marco);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, marco);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, marco);
    }

    public static void SetBuildDevAndAutoConnectProfiler()
    {
        string isDev = UnityCommandArgumentParse.GetFuctionArgs(1, "true");

        EditorUserBuildSettings.development     = isDev == "true";
        EditorUserBuildSettings.allowDebugging  = isDev == "true";
    }

    public static void SetGlobalSetting()
    {
        string[] args = UnityCommandArgumentParse.GetFuctionArgs();
        int i = 1;
        while (i < args.Length && i < args.Length - 1)
        {
            string filed = args[i];//UnityCommandArgumentParse.GetFuctionArgs(1, "isGuide");
            string isOpen = args[i + 1];//UnityCommandArgumentParse.GetFuctionArgs(2, "true");

            _setglobalSetting(filed, isOpen == "true");

            i += 2;
        }
    }

    public static void SetGlobalChannel()
    {
        string sdkChannel = UnityCommandArgumentParse.GetFuctionArgs(1, SDKChannel.NONE.ToString());
        _setglobalSetting("sdkChannel", sdkChannel);

        File.WriteAllText("Assets/StreamingAssets/sdk.conf", sdkChannel);
    }

    private static void _setglobalSetting(string filed, object v)
    {
        UnityEngine.Debug.LogFormat("GlobalSetting KeyValue {0} {1}", filed, v);
#if !LOGIC_SERVER
        GlobalSetting setting = AssetDatabase.LoadAssetAtPath<GlobalSetting>("Assets/Resources/"+Global.PATH+".asset");

        SerializedObject sobj = new SerializedObject(setting);

        SerializedProperty prop = sobj.FindProperty(filed);

        if (null != prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.String:
                    prop.stringValue = (string)v;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = (bool)v;
                    break;
                case SerializedPropertyType.Integer:
                    prop.intValue = (int)v;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = (float)v;
                    break;
                case SerializedPropertyType.Enum:
                    for (int i = 0; i < prop.enumNames.Length; ++i)
                    {
                        if (prop.enumNames[i] == (string)v)
                        {
                            prop.enumValueIndex = i;
                            break;
                        }
                    }
                    break;
            }
        }

        sobj.ApplyModifiedProperties();

        EditorUtility.SetDirty(setting);
        AssetDatabase.SaveAssets();
#endif
    }

#region 测试中的脚本
    class Conf
    {
        public Conf(Dictionary<string, string> argsPair)
        {
            var t = this.GetType();
            var fields = t.GetFields();
            foreach (var item in fields)
            {
                if (argsPair.TryGetValue(item.Name, out string tmp))
                {
                    if (item.FieldType == typeof(bool))
                    {
                        item.SetValue(this, ConvertPythonBoolean(tmp));
                    }
                }
                else
                {
                    var defaultvalue = item.GetValue(this);
                    Debug.LogWarning($"[警告] 配置{item.Name} 未找到，使用默认值{defaultvalue}");
                }
            }
        }
    }
    class GlobalSettingsConf : Conf
    {
        /// <summary>
        /// （遗留）是否是XYSDK
        /// </summary>
        public bool isUsingXYSDK = true;
        /// <summary>
        /// 是否开启新手引导（jenkins可配置）
        /// </summary>
        public bool isGuide = true;
        /// <summary>
        /// 强制自动战斗（jenkins可配置）
        /// </summary>
        public bool forceUseAutoFight = false;
        /// <summary>
        /// 是否开启热更（jenkins可配置）
        /// </summary>
        public bool enableHotFix = true;
        /// <summary>
        /// （WebGL网页用false）是否从包内加载资源
        /// </summary>
        public bool loadFromPackage = true;
        /// <summary>
        /// debug下显示热更拉取url资源的日志
        /// </summary>
        public bool hotFixUrlDebug = false;
        /// <summary>
        /// 未知
        /// </summary>
        public bool isXYPaySDKDebug = false;
        /// <summary>
        /// 未知
        /// </summary>
        public bool isRecordPVP = false;
        /// <summary>
        /// 全局debug设置 （jenkins可配置）
        /// </summary>
        public bool isDebug = false;
        /// <summary>
        /// 未知
        /// </summary>
        public bool skillHasCooldown = true;
        public GlobalSettingsConf(Dictionary<string, string> argsPair) : base(argsPair)
        {

        }
    }
    class BuildABConf : Conf
    {
        /// <summary>
        /// 转换ScriptData，已经集成入ab流程中
        /// </summary>
        //public bool needPackScriptData = true;

        public BuildABConf(Dictionary<string, string> argsPair) : base(argsPair)
        {

        }
    }
    public static void NewOpenMainSceneAddUWAPrefab()
    {
        var argsPair = ParseCommandLineArgs();

        string type = argsPair["directMode"];
        bool isMonoManual = ConvertPythonBoolean(argsPair["directMonoManual"]);

        string prefabPath = "Assets/UWA/Prefabs/UWA_Launcher.prefab";
        if (!System.IO.File.Exists(prefabPath))
            throw new Exception("!System.IO.File.Exists(prefabPath)");

        // Assets/UWA/Prefabs/UWA_Launcher.prefab
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");

        var res = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (null == res)
            throw new Exception("null == res");

        GameObject obj = GameObject.Instantiate(res);
        if (null == obj)
            throw new Exception("null == obj");

        obj = UnityEditor.PrefabUtility.ConnectGameObjectToPrefab(obj, res);

        if (type == "Mono" && isMonoManual)
        {
            Component com = obj.GetComponent("UWA_Launcher");
            if (null != com)
            {
                SerializedObject so = new SerializedObject(com);
                var propty = so.FindProperty("DirectManualMono");
                if (null != propty)
                {
                    propty.boolValue = true;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
                else
                {
                    throw new Exception("DirectManualMono not found!");
                }
            }
            else
            {
                throw new Exception("UWA_Launcher not found!");
            }
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    public static void NewInitGlobalSettings(Dictionary<string,object> kv)
    {
#if !LOGIC_SERVER
        GlobalSetting setting = AssetDatabase.LoadAssetAtPath<GlobalSetting>("Assets/Resources/"+Global.PATH+".asset");

        SerializedObject sobj = new SerializedObject(setting);

        foreach (var item in kv)
        {
            var filed = item.Key;
            var v = item.Value;
            SerializedProperty prop = sobj.FindProperty(filed);

            if (null != prop)
            {
                switch (prop.propertyType)
                {
                    case SerializedPropertyType.String:
                        prop.stringValue = (string)v;
                        break;
                    case SerializedPropertyType.Boolean:
                        prop.boolValue = (bool)v;
                        break;
                    case SerializedPropertyType.Integer:
                        prop.intValue = (int)v;
                        break;
                    case SerializedPropertyType.Float:
                        prop.floatValue = (float)v;
                        break;
                    case SerializedPropertyType.Enum:
                        for (int i = 0; i < prop.enumNames.Length; ++i)
                        {
                            if (prop.enumNames[i] == (string)v)
                            {
                                prop.enumValueIndex = i;
                                break;
                            }
                        }
                        break;
                }
            }
        }
        sobj.ApplyModifiedProperties();

        EditorUtility.SetDirty(setting);
        AssetDatabase.SaveAssets();
#endif

    }

    public static Dictionary<string,object> Class2Dict(object obj)
    {
        return obj.GetType().GetFields().ToDictionary(q => q.Name, q => q.GetValue(obj));
    }
    private static Dictionary<string,string> ParseCommandLineArgs()
    {
        var rawArgs = System.Environment.GetCommandLineArgs();
        var args = CommandLine.Parse(rawArgs);
        var argsPair = args.ArgPairs;
        return argsPair;
    }
    public static void NewBuildAB()
    {
        var argsPair = ParseCommandLineArgs();

        //EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        //var _ = ConfigAll(argsPair);
        //AssetDatabase.Refresh();
        //var buildABConf = new BuildABConf(argsPair);

        //var globalSettingsConf = new GlobalSettingsConf(argsPair);
        //var globalSettingsConfDict = Class2Dict(globalSettingsConf);
        //NewInitGlobalSettings(globalSettingsConfDict);
        //AssetDatabase.Refresh();

        string isIncremental = "false";
        BuildPackage.IncrementBuildBundle(isIncremental == "true");
    }
    public static void NewBuild()
    {
        var argsPair = ParseCommandLineArgs();
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");

        string outpath = argsPair["outpath"];
        string plat = argsPair["plat"];
        BuildOptions op = ConfigAll(argsPair);
        AssetDatabase.Refresh();
        var globalSettingsConf = new GlobalSettingsConf(argsPair);
        var globalSettingsConfDict = Class2Dict(globalSettingsConf);
        NewInitGlobalSettings(globalSettingsConfDict);
        AssetDatabase.Refresh();


        string[] SCENE_LIST = _updateSceneList();
        BuildTarget target = BuildTarget.NoTarget;
#if UNITY_IOS
        target = BuildTarget.iOS;
#elif UNITY_ANDROID
        PlayerSettings.Android.keyaliasPass = "123456";
        PlayerSettings.Android.keystorePass = "123456";
        target = BuildTarget.Android;
#elif UNITY_STANDALONE
        if (plat.ToLower().Equals("win64"))
        {
            target = BuildTarget.StandaloneWindows64;
        }
        else if (plat.ToLower().Equals("osxuniversal"))
        {
            target = BuildTarget.StandaloneOSX;
        }
#endif
        BuildPipeline.BuildPlayer(SCENE_LIST, outpath, target, op);
    }

    public static BuildOptions ConfigAll(Dictionary<string,string> argsPair)
    {

        bool isUnityDev = ConvertPythonBoolean(argsPair["isUnityDev"]);
        bool isIL2CPP = ConvertPythonBoolean(argsPair["isIL2CPP"]);
        bool isConnectProfiler = ConvertPythonBoolean(argsPair["isConnectProfiler"]);
        string marco = argsPair["marco"];

        BuildOptions op = BuildOptions.None;
        SetUnityDev(isUnityDev, ref op);
        SetConnectProfiler(isConnectProfiler, ref op);
        SetScriptBackEnd(isIL2CPP);
        SetMarco(marco);


        return op;
    }
    private static void SetUnityDev(bool isUnityDev, ref BuildOptions op)
    {
        EditorUserBuildSettings.development = isUnityDev;
        if (isUnityDev)
        {
            op |= BuildOptions.Development;
        }
    }
    private static void SetConnectProfiler(bool isConnectProfiler, ref BuildOptions op)
    {
        EditorUserBuildSettings.connectProfiler = isConnectProfiler;
        if (isConnectProfiler)
        {
            op |= BuildOptions.ConnectWithProfiler;
        }
    }
    private static void SetScriptBackEnd(bool isIL2CPP)
    {
        ScriptingImplementation implementation = ScriptingImplementation.Mono2x;
        if (isIL2CPP)
        {
            implementation = ScriptingImplementation.IL2CPP;
        }
#if UNITY_STANDALONE
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone,implementation);
#elif UNITY_ANDROID
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, implementation);
#elif UNITY_IOS
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS,implementation);
#endif
    }
    private static void SetMarco(string marco)
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, marco);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, marco);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, marco);
    }
    public static bool ConvertPythonBoolean(string pybool)
    {
        if (pybool == "True")
        {
            return true;
        }
        else if (pybool == "False")
        {
            return false;
        }
        throw new Exception("python boolean 非法 val="+ pybool);
    }

#region 命令行解析脚本
    //---------------------------------------------------------------------
    /// <summary>
    /// Contains the parsed command line arguments. This consists of two

    /// lists, one of argument pairs, and one of stand-alone arguments.
    /// </summary>

    public class CommandArgs

    {
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns the dictionary of argument/value pairs.
        /// </summary>
        public Dictionary<string, string> ArgPairs

        {
            get { return mArgPairs; }
        }
        Dictionary<string, string> mArgPairs = new Dictionary<string, string>();
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns the list of stand-alone parameters.
        /// </summary>
        public List<string> Params

        {
            get { return mParams; }
        }
        List<string> mParams = new List<string>();
    }
    //---------------------------------------------------------------------
    /// <summary>
    /// Implements command line parsing

    /// </summary>

    public class CommandLine

    {
        //---------------------------------------------------------------------
        /// <summary>
        /// Parses the passed command line arguments and returns the result

        /// in a CommandArgs object.
        /// </summary>
        /// The command line is assumed to be in the format:
        ///
        /// CMD [param] [[-|--|\]&lt;arg&gt;[[=]&lt;value&gt;]] [param]
        ///
        /// Basically, stand-alone parameters can appear anywhere on the command line.
        /// Arguments are defined as key/value pairs. The argument key must begin

        /// with a '-', '--', or '\'. Between the argument and the value must be at

        /// least one space or a single '='. Extra spaces are ignored. Arguments MAY

        /// be followed by a value or, if no value supplied, the string 'true' is used.
        /// You must enclose argument values in quotes if they contain a space, otherwise

        /// they will not parse correctly.
        ///
        /// Example command lines are:
        ///
        /// cmd first -o outfile.txt --compile second \errors=errors.txt third fourth --test = "the value" fifth

        ///
        /// <param name="args">array of command line arguments</param>
        /// <returns>CommandArgs object containing the parsed command line</returns>
        public static CommandArgs Parse(string[] args)
        {
            char[] kEqual = new char[] { '=' };
            char[] kArgStart = new char[] { '-', '\\' };
            CommandArgs ca = new CommandArgs();
            int ii = -1;
            string token = NextToken(args, ref ii);
            while (token != null)
            {
                if (IsArg(token))
                {
                    string arg = token.TrimStart(kArgStart).TrimEnd(kEqual);
                    string value = null;
                    if (arg.Contains("="))
                    {
                        // arg was specified with an '=' sign, so we need

                        // to split the string into the arg and value, but only

                        // if there is no space between the '=' and the arg and value.
                        string[] r = arg.Split(kEqual, 2);
                        if (r.Length == 2 && r[1] != string.Empty)
                        {
                            arg = r[0];
                            value = r[1];
                        }
                    }
                    while (value == null)
                    {
                        string next = NextToken(args, ref ii);
                        if (next != null)
                        {
                            if (IsArg(next))
                            {
                                // push the token back onto the stack so

                                // it gets picked up on next pass as an Arg

                                ii--;
                                value = "true";
                            }
                            else if (next != "=")
                            {
                                // save the value (trimming any '=' from the start)
                                value = next.TrimStart(kEqual);
                            }
                        }
                        // 结尾
                        else if (next == null && args.Length == ii)
                        {
                            value = "true";
                        }
                        else
                        {
                            throw new Exception("未知错误"+ next);
                        }
                    }
                    // save the pair

                    ca.ArgPairs.Add(arg, value);
                }
                else if (token != string.Empty)
                {
                    // this is a stand-alone parameter.
                    ca.Params.Add(token);
                }
                token = NextToken(args, ref ii);
            }
            return ca;
        }

        public static List<string> OrderList(Dictionary<string,string> args, string key)
        {
            List<string> ls = new List<string>(args[key].Split(','));
            return ls;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns True if the passed string is an argument (starts with

        /// '-', '--', or '\'.)
        /// </summary>
        /// <param name="arg">the string token to test</param>
        /// <returns>true if the passed string is an argument, else false if a parameter</returns>
        static bool IsArg(string arg)
        {
            return (arg.StartsWith("-") || arg.StartsWith("\\"));
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Returns the next string token in the argument list

        /// </summary>
        /// <param name="args">list of string tokens</param>
        /// <param name="ii">index of the current token in the array</param>
        /// <returns>the next string token, or null if no more tokens in array</returns>
        static string NextToken(string[] args, ref int ii)
        {
            ii++; // move to next token

            while (ii < args.Length)
            {
                string cur = args[ii].Trim();
                if (cur != string.Empty)
                {
                    // found valid token

                    return cur;
                }
                ii++;
            }
            // failed to get another token

            return null;
        }
    }

    #endregion
    #endregion

    #region 自动化接口
    public enum PathType
    {
        NotExists,
        File,
        Directory
    }
    public static PathType GetPathType(string path)
    {
        if (Directory.Exists(path))
        {
            return PathType.Directory;
        }
        else if(File.Exists(path))
        {
            return PathType.File;
        }
        return PathType.NotExists;
    }
    public static void Ensure_PathNewest(string path)
    {
        var pt = GetPathType(path);
        switch (pt)
        {
            case PathType.NotExists:
                Directory.CreateDirectory(path);
                break;
            case PathType.File:
                File.Delete(path);
                File.WriteAllText(path, "");
                break;
            case PathType.Directory:
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
                break;
            default:
                break;
        }

    }
    public static string SaveReportData(string methodname,string reportData)
    {
        string reportRoot = Path.Combine(Directory.GetCurrentDirectory(), methodname);
        Ensure_PathNewest(reportRoot);

        File.WriteAllText(Path.Combine(reportRoot, "report.txt"), string.Join("\n", reportData));

        return reportRoot;
    }
    #endregion
    #region 提交监控
    public static void ObserverMethod()
    {
        var argsPair = ParseCommandLineArgs();
        var methodnames = CommandLine.OrderList(argsPair, "methodnames");

        foreach (var item in methodnames)
        {
            var tmp = item.Split('.');
            var tmp2 = new StringBuilder();
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                tmp2.Append(tmp[i]);
            }
            var path = tmp2.ToString();
            var name = tmp[tmp.Length - 1];
            var asm = Assembly.GetExecutingAssembly();
            var type = asm.GetType(path);
            type.InvokeMember(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);

        }
    }
    #endregion
}
