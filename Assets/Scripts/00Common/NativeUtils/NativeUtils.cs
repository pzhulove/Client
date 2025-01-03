using System.Collections;
using System.Runtime.InteropServices;

public class NativeUtils
{
//    public static void OpenUrl(string url) 
//    {
//#if UNITY_ANDROID || UNITY_IPHONE
//		_OpenDownloadUrl(url);
//#endif
//	}
//
//	public static void SetScreenOn(bool state) 
//    {
//#if UNITY_ANDROID || UNITY_IPHONE
//		_SetScreenState(state);
//#endif	
//	}
//
//#if UNITY_ANDROID
//	private static AndroidJavaClass IMBAUtilsClz = null;
//
//	private static AndroidJavaClass getClz() {
//		if(Application.platform == RuntimePlatform.Android && null == IMBAUtilsClz) {
//			IMBAUtilsClz = new AndroidJavaClass("com.dh.imba.utils.DHIMBAUtils");
//		}
//		return IMBAUtilsClz;
//	}
//
//	private static void _OpenDownloadUrl(string url) {
//		if (null == getClz()) {
//			return;
//		}
//		AndroidJavaObject mIMBAUtils = IMBAUtilsClz.CallStatic<AndroidJavaObject>("getInstance");
//		mIMBAUtils.Call("openUrl", url);
//	}
//
//	public static void _SetScreenState(bool state) 
//	{
//        if (null == getClz()) 
//        {
//            return;
//        }
//
//		AndroidJavaObject mIMBAUtils = IMBAUtilsClz.CallStatic<AndroidJavaObject>("getInstance");
//        mIMBAUtils.Call("keepScreenOn", state);
//	}
//
//#elif UNITY_IPHONE
//
//	[DllImport("__Internal")]
//	private static extern void _OpenDownloadUrl(string url);
//
//	[DllImport("__Internal")]
//	private static extern void _SetScreenState(bool state);
//#endif

}
