using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;



#if UNITY_EDITOR
public class SDKInterfaceIOS : SDKInterface
{
}
#elif UNITY_IPHONE || UNITY_IOS

public class SDKInterfaceIOS : SDKInterface {

	public override void Init (bool debug)
	{
		base.Init(debug);
		//_CommonInit();
	}

    public static void CommonInit()
    {
        _CommonInit();
    }

	public override void SetNotification (int nid, string content, string title, int hour)
	{
		_SetNotification(nid, content, title, hour);
	}

	public override void SetNotificationWeekly(int nid, string content, string title, int weekday, int hour, int minute)
	{
		int weekdayId = ResetNidWeekly(nid, weekday);
		_SetNotificationWeekly(weekdayId, content, title, weekday, hour, minute);
	}
	public override void SetNotificationWithTsp(int nid, string content, string title, long tsp_unix)
	{
		_SetNotificationWithTsp(nid, content, title, tsp_unix);
	}
	public override void RemoveNotification (int nid)
	{
		for (int i = 1; i <= 7; i++) 
		{
			_RemoveNotification(ResetNidWeekly(nid, i));
		}		
	}

	public override void RemoveAllNotification ()
	{
		_RemoveAllNotification();
	}
	
	private int ResetNidWeekly(int nid, int weekly)
	{
		return nid * 10 + 100000 + weekly;
	}

	public override void ResetBadge()
	{
		_ResetBadge();
	}

	public override void SetScreenBrightness(float value)
	{
		_SetBrightness(value);
	}
	public override float GetScreenBrightness()
	{
		return _GetBrightness();
	}

	public override void Exit()
	{
		ExitIOS();
	}

    public override float GetBatteryLevel()
    {
        return _GetBatteryLevel();
    }

    public override string GetSystemTimeHHMM()
    {
        return base.GetSystemTimeHHMM();
    }

    public override bool RequestAudioAuthorization()
    {
        return _RequestAudioAuthorization();
    }

    public override void SetAudioSessionActive()
    {
        _SetAudioSessionActive();
    }

    public override int TryGetCurrVersionAPI()
    {
        if(_IsIOSSystemVersionMoreThanNine())
		{
			return 1000;
		}else
		{
			return 0;
		}
    }
	
	public override string GetClipboardText()
    {
        return _GetTextFromClipboard();
    }

    public override string GetSystemIMEI()
    {
        return UnityEngine.SystemInfo.deviceUniqueIdentifier;
    }

    public override string GetBuildPlatformId()
    {
        return TR.Value("zymg_plat_id_ios");
    }

	public override string GetOnlineServicePlatformId()
    {
		if(IsIOSOtherChannel())
		{
			return TR.Value("zymg_onlineservice_plat_id_iosother");
		}
        return TR.Value("zymg_onlineservice_plat_id_ios");
    }

	public override string GetOnlineServicePlatformName()
    {
        if(IsIOSOtherChannel())
		{
			return TR.Value("zymg_onlineservice_plat_name_iosother");
		}
        return TR.Value("zymg_onlineservice_plat_name_ios");
    }

    public override void MobileVibrate()
    {
 	    Handheld.Vibrate();
    }

    public override void ScanFile(string path)
    {
        _SavePhoto(path);
    }

    public override void InitAlartText(string title ,string message,string btnText)
    {
        _InitAlartText(title, message, btnText);
    }

    public override bool HasNotch(bool debug = false)
    {
        if (debug)
            return base.HasNotch(debug);
        return _HasNotch();
    }
    public override int[] GetNotchSize(NotchDebugType debug = NotchDebugType.None, ScreenOrientation screenOrientation = ScreenOrientation.LandscapeLeft)
    {
        if (debug != NotchDebugType.None)
            return base.GetNotchSize(debug, screenOrientation);
        return new int[2] { _GetNotchSize(), Screen.height };

    }

    // --- dllimport start ---
    [DllImport("__Internal")]
	private static extern void _SetNotification(int nid, string content, string title, int hour);

	[DllImport("__Internal")]
	private static extern void _SetNotificationWeekly(int nid, string content, string title, int weekday, int hour, int minute);
	[DllImport("__Internal")]
	private static extern void _SetNotificationWithTsp(int nid, string content, string title, long tsp);

	[DllImport("__Internal")]
	private static extern void _RemoveNotification(int nid);

	[DllImport("__Internal")]
	private static extern void _RemoveAllNotification();

	[DllImport("__Internal")]
	private static extern void _CommonInit();

	[DllImport("__Internal")]
	private static extern void _SetBrightness(float value);

	[DllImport("__Internal")]
	private static extern float _GetBrightness();

	[DllImport("__Internal")]
	private static extern void ExitIOS();
	
	[DllImport("__Internal")]
	private static extern void _ResetBadge();

    [DllImport("__Internal")]
    private static extern float _GetBatteryLevel();

    [DllImport("__Internal")]
    private static extern bool _RequestAudioAuthorization();

    [DllImport("__Internal")]
    private static extern void _SetAudioSessionActive();
	
	[DllImport("__Internal")]
    private static extern bool _IsIOSSystemVersionMoreThanNine();
	
	[DllImport("__Internal")]
    private static extern string _GetTextFromClipboard();

    [DllImport("__Internal")]
    private static extern void _SavePhoto(string readAddr);

    [DllImport("__Internal")]
    private static extern void _InitAlartText(string title, string message, string btnText);

    [DllImport("__Internal")]
    private static extern bool _HasNotch();

    [DllImport("__Internal")]
    private static extern int _GetNotchSize();
}

#endif
