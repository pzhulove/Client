using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class AndroidSoUtility
{
    private static string kSoLibDirName = "corelibs";

    public static string GetCoreLibPath()
    {
        string libdirpath = string.Empty;

#if UNITY_ANDROID
        try 
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject applicationInfo = currentActivity.Call<AndroidJavaObject>("getApplicationInfo");

            //libdirpath = applicationInfo.Get<string>("dataDir");
            Int32 MODE_PRIVATE = currentActivity.GetStatic<Int32>("MODE_PRIVATE");
            AndroidJavaObject libDir = currentActivity.Call<AndroidJavaObject>("getDir", kSoLibDirName, MODE_PRIVATE);
            libdirpath = libDir.Call<string>("getAbsolutePath");
        } 
        catch (Exception e)
        {
            UnityEngine.Debug.LogErrorFormat("[AndroidSoUtility] GetLibDirPath Error {0}", e.ToString());
        }
#endif

        UnityEngine.Debug.LogFormat("[AndroidSoUtility] GetLibDirPath {0}", libdirpath);

        return libdirpath;
    }
}
