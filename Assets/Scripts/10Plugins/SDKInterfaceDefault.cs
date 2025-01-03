using UnityEngine;
using System.Collections;

public class SDKInterfaceDefault : SDKInterface 
{
    public override string GetClipboardText()
    {
        return GUIUtility.systemCopyBuffer;
    }

    public override int TryGetCurrVersionAPI()
    {
        return 1000;
    }

    public override bool NeedSDKBindPhoneOpen()
    {
        return true;
    }

    public override bool IsOppoPlatform()
    {
        return Global.Settings.mainSDKChannel == SDKChannel.OPPO ? true : false;
    }

    public override bool IsVivoPlatForm()
    {
        return Global.Settings.mainSDKChannel == SDKChannel.VIVO ? true : false;
    }

    public override bool IsStartFromGameCenter()
    {
        return Global.Settings.mainSDKChannel == SDKChannel.VIVO || Global.Settings.mainSDKChannel == SDKChannel.OPPO ? true : false;
    }

    public override bool RequestAudioAuthorization()
    {
        return true;
    }

    public override string GetBuildPlatformId()
    {
        return TR.Value("zymg_plat_id_ios");
    }

    public override string GetOnlineServicePlatformId()
    {
        return TR.Value("zymg_onlineservice_plat_id_other");
    }

    public override string GetOnlineServicePlatformName()
    {
        return TR.Value("zymg_onlineservice_plat_name_other");
    }
}

