using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSplashFrame : ClientFrame 
{
    [UIControl("splash")]
    Image splash;

    protected bool isSplashDone = false;
    public bool IsSplashDone
    {
        get { return isSplashDone; }
    }

    private float durationTime = 2f;

    private string[] splashPaths;

    private int splashCount;
    public override string GetPrefabPath()
    {
        return "Base/UI/Prefabs/SplashFrame";
    }

    protected override void _OnOpenFrame()
    {
        isSplashDone = false;
        splashPaths = SDKInterface.Instance.GetSplashResourcePath();
        StartCoroutine(WaitToLoadSplash(durationTime));
    }

    protected override void _OnCloseFrame()
    {
        isSplashDone = true;
    }

    IEnumerator WaitToLoadSplash(float duration)
    {
        if (splashPaths == null)
        {
            Logger.LogError("[StartSplashFrame] - WaitToLoadSplash splashPaths is null");
			this.Close();
            yield break;
        }
        splashCount = splashPaths.Length;
        int index = 0;
        while (splashCount > index)
        {
            SetSplashSprite(splashPaths[index]);
            yield return Yielders.GetWaitForSecondsRealtime(duration);
            index++;
        }
        this.Close();
    }

    private void SetSplashSprite(string resPath)
    {
        if (splash)
        {
            splash.sprite = Resources.Load<Sprite>(resPath);
        }
    }
    private void SetSplashActive(bool isShow)
    {
        if (splash)
        {
            splash.CustomActive(isShow);
        }
    }
}
