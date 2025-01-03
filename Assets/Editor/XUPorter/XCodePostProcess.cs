using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{

#if UNITY_IOS
#if UNITY_EDITOR
	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
#if UNITY_5
		if (target != BuildTarget.iOS) {
#else
        if (target != BuildTarget.iOS) {
#endif
			Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// Create a new project object from build target
		XCProject project = new XCProject( Path.GetFullPath(pathToBuiltProject) );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		string[] files = Directory.GetFiles( Application.dataPath, "*.projmods", SearchOption.AllDirectories );
		foreach( string file in files ) {
			UnityEngine.Debug.Log("ProjMod File: "+file);
			project.ApplyMod( file );
		}

		//TODO disable the bitcode for iOS 9
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "ReleaseForProfiling");
		project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "ReleaseForRunning");

		//TODO implement generic settings as a module option
//		project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");
		string path = Path.GetFullPath (pathToBuiltProject);

		//EditorPlist(path);
		EditorCode(path);

		// Finally save the xcode project
		project.Save();

	}

//	private static void EditorPlist(string filePath)
//	{
//
//		XCPlist list =new XCPlist(filePath);
//
//		//string bundle = "com.hegu.dnf.test";
//
//
//		/*
//		string PlistAdd = @"  
//            <key>CFBundleURLTypes</key>
//            <array>
//            <dict>
//            <key>CFBundleTypeRole</key>
//            <string>Editor</string>
//            <key>CFBundleURLIconFile</key>
//            <string>Icon@2x</string>
//            <key>CFBundleURLName</key>
//            <string>"+bundle+@"</string>
//            <key>CFBundleURLSchemes</key>
//            <array>
//            <string>ww123456</string>
//            </array>
//            </dict>
//            </array>";*/
//
//		string PlistAdd = @"
//			<key>CFBundleURLTypes</key>
//			<array>
//			<dict>
//			<key>CFBundleTypeRole</key>
//			<string>Editor</string>
//			<key>CFBundleURLSchemes</key>
//			<array>
//				<string>com.hegu.dnf.test.alipay</string>
//			</array>
//			</dict>
//			</array>
//<key>LSApplicationQueriesSchemes</key>
//	<array>
//		<string>alipay</string>
//		<string>aliminipayauth</string>
//		<string>alipay://</string>
//		<string>wechat</string>
//		<string>weixin</string>
//		<string>xyzsapp</string>
//		<string>xyzsapp://</string>
//		<string>alipayauth</string>
//	</array>
//<key>NSAppTransportSecurity</key>
//	<dict>
//		<key>NSAllowsArbitraryLoads</key>
//		<true/>
//	</dict>
//";
//
//		//在plist里面增加一行
//		list.AddKey(PlistAdd);
//		//在plist里面替换一行
//		//list.ReplaceKey("<string>com.yusong.${PRODUCT_NAME}</string>","<string>"+bundle+"</string>");
//
//		list.AddPlistItems("");
//
//		//保存
//		list.Save();
//	}

	private static void EditorCode(string filePath)
	{
		//读取UnityAppController.mm文件
		//XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");

		//在指定代码后面增加一行代码
		//UnityAppController.WriteBelow("#include <mach/mach_time.h>\n","#import <XYPlatform/XYPlatform.h>\n");

		//在指定代码中替换一行
		//UnityAppController.Replace("return YES;","return [ShareSDK handleOpenURL:url sourceApplication:sourceApplication annotation:annotation wxDelegate:nil];");

		//在指定代码后面增加一行

		//UnityAppController.WriteBelow("    UnitySendRemoteNotificationError(error);\n}\n\n#endif", "- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url\n{\n    [[XYPlatform defaultPlatform] XYHandleOpenURL:url];\n    return YES;\n}\n\n- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString *,id> *)options\n{\n    [[XYPlatform defaultPlatform] XYHandleOpenURL:url];\n    return YES;\n    \n}");

		//UnityAppController.WriteBelow("ADD_ITEM(annotation);\n\n    #undef ADD_ITEM",
		//	"[[XYPlatform defaultPlatform] XYHandleOpenURL:url];");

		//UnityAppController.WriteBelow("- (void)applicationWillEnterForeground:(UIApplication*)application\n{", 
		//	"[[XYPlatform defaultPlatform] XYAapplicationWillEnterForeground:application];");

		//UnityAppController.WriteBelow(" if (_unityAppReady)\n    {\n        if (UnityIsPaused() && _wasPausedExternal == false)\n        {\n            UnityWillResume();\n            UnityPause(0);\n        }\n        UnitySetPlayerFocus(1);","UnitySetAudioSessionActive(1);\n");
		
        //Unity2018上隐藏iPhone X下面的手势按键，防止进入游戏的误触，和unity5.6有区别
        XClass unityViewController = new XClass (filePath + "/Classes/UI/UnityViewControllerBase+iOS.mm");
		if (unityViewController != null) {
			unityViewController.Replace ("- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures\n{\n    UIRectEdge res = UIRectEdgeNone;\n    if (UnityGetDeferSystemGesturesTopEdge())\n        res |= UIRectEdgeTop;\n    if (UnityGetDeferSystemGesturesBottomEdge())\n        res |= UIRectEdgeBottom;\n    if (UnityGetDeferSystemGesturesLeftEdge())\n        res |= UIRectEdgeLeft;\n    if (UnityGetDeferSystemGesturesRightEdge())\n        res |= UIRectEdgeRight;\n    return res;\n}\n\n- (BOOL)prefersHomeIndicatorAutoHidden\n{\n    return UnityGetHideHomeButton();\n}", 
				"- (BOOL)prefersHomeIndicatorAutoHidden\n{\n    return NO;\n}\n\n// add this\n- (UIRectEdge)preferredScreenEdgesDeferringSystemGestures\n{\n    return UIRectEdgeAll;\n}\n");
		}
	}

#endif
#endif


    public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
