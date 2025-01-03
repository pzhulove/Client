using UnityEngine;
using System.Collections;
#if BANGCLE_EVERISK
using Assets.BangBangEveRisk;
#endif

public class SDKCallback : MonoSingleton<SDKCallback> {

	const int TOTAL_COUNTDOWN = 60*2;//二分钟
	const float LOW_SCREEN_BRIGHTNESS = 0.01f;
	float deviceScreenBrightness = 0.5f;
	SimpleTimer2 timer = null;
	bool screenSaverInited = false;
    float durationTimeTemp = 0f;
    float durationTimeTemp2 = 0f;
    ushort roleLevel = 0;

	float checkAcc = 0f;
	bool canRead = false;

	public bool CanRead
	{
		set {
			canRead = value;
		}
		get {
			return canRead;
		}
	}

	//每2分钟统计
	float tempAcc = 0;
	const float STAT_DURATION = 2 * 60;
	bool needCheckTempure;

	public int cpuTempture;
	public int batteryTempture;
	public int frameRate;
	public int batteryLevel;
	public int batteryConsume;
	bool valueUpated;

    float tempBuglyInfoTimer = 0f;
    const float BUGLY_INFO_DURATION = 15f;

    public void Start()
	{
		GameObject.DontDestroyOnLoad(gameObject);

        //SDKInterface.instance.KeepScreenOn(true);
	}

	public override void Init ()
	{
		//Logger.LogErrorFormat("sdkcallback init");
	}

	public void StartScreenSave()
	{
		if (GameClient.SwitchFunctionUtility.IsOpen(5))
		{
			InitScreenBrightnessProtect();
		}
		else
		{
			screenSaverInited = false;
			timer = null;
		}
			
	}
    #region 渠道SDK回调
    public void OnLogin(string param)
	{
		if (param == null)
		{
			Logger.LogErrorFormat("login param is null");
			return;
		}
		Logger.LogProcessFormat("Login param:{0}", param);
		SDKUserInfo userInfo = SDKInterface.Instance.SDKUserInfo;
		if (userInfo == null)
		{
			return;
		}

		var paramArray = param.Split(',');
		if (paramArray != null && paramArray.Length == 2)
		{
			userInfo.openUid = paramArray[0];
			userInfo.token = paramArray[1];
			userInfo.ext = "";
			Logger.LogProcessFormat("[登陆成功] {0} {1}", paramArray[0], paramArray[1]);
			if (SDKInterface.Instance.loginCallbackGame != null)
			{
				SDKInterface.Instance.loginCallbackGame(userInfo);
			}
		}
		else if (paramArray != null && paramArray.Length == 3)
		{
			userInfo.openUid = paramArray[0];
			userInfo.token = paramArray[1];
			userInfo.ext = paramArray[2];
			Logger.LogProcessFormat("[登陆成功] {0} {1} {2}", paramArray[0], paramArray[1], paramArray[2]);
			if (SDKInterface.Instance.loginCallbackGame != null)
			{
				SDKInterface.Instance.loginCallbackGame(userInfo);
			}
		}
		else
		{
			Logger.LogErrorFormat("### [SDK] - login callback to unity , param is wrong: {0} !!!", param);
		}
	}
	public void OnLoginFail(string code)
	{
		if (SDKInterface.Instance.loginFailCallbackGame != null)
		{
			SDKInterface.LoginFailCode errcode = SDKInterface.LoginFailCode.Unkonw;
			try
			{
				errcode = (SDKInterface.LoginFailCode)(System.Enum.Parse(typeof(SDKInterface.LoginFailCode), code));
			}
			catch (System.Exception e)
			{
				Logger.LogError("errcode = " + code);
				Logger.LogError(e.ToString());
			}
			finally
			{
				SDKInterface.Instance.loginFailCallbackGame(errcode);
			}
		}
	}



	public void OnLogout()
	{
		if (SDKInterface.Instance.logoutCallbackGame == null)
		{
			Logger.LogError("logoutCallbackGame == null ");
			return;
		}
		SDKInterface.Instance.logoutCallbackGame();
	}

	public void OnPayResult(string param)
	{
		if (SDKInterface.Instance.payResultCallbackGame == null)
		{
			Logger.LogError("payResultCallbackGame == null "+param);
			return;
		}
		SDKInterface.Instance.payResultCallbackGame(param);
		
	}


    public void OnBindPhoneSucc(string bindPhoneNum)
    {
		if (SDKInterface.Instance.bindPhoneCallbackGame == null)
		{
			Logger.LogError("payResultCallbackGame == null " + bindPhoneNum);
			return;
		}
		SDKInterface.Instance.bindPhoneCallbackGame(bindPhoneNum);
    }
    #endregion

    public void OnKeyBoardShowOn(string param)
    {
        if (SDKInterface.Instance.keyboardShowCallbackGame == null)
        {
			Logger.Log("payResultCallbackGame == null " + param);
			return;
        }
		SDKInterface.Instance.keyboardShowCallbackGame(param);
	}

    public void OnLogRet(string res)
	{
		Logger.LogError("[android res] - "+res);
	}

    public void OnLoadSmallGame()
    {
        string sceneName = SDKInterface.Instance.GetIOSAppstoreSmallGameLoadSceneName();
        if (SDKInterface.Instance.smallGameLoadCallbackGame != null && !string.IsNullOrEmpty(sceneName))
        {
            SDKInterface.Instance.smallGameLoadCallbackGame(sceneName);
        }
    }

	public void Update()
	{
		float timeElapsed = Time.deltaTime;

		if (screenSaverInited)
		{
			if (timer != null)
				timer.UpdateTimer((int)(timeElapsed * 1000));

			//判定是否点屏幕
#if UNITY_EDITOR
			if (Input.GetMouseButtonUp(0))
			{
				RestoreScreenBrightness();
				if (timer != null)
					timer.StartTimer();
			}
#else
				if (Input.touchCount == 1)
				{
					if(Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						RestoreScreenBrightness();
						if (timer != null)
							timer.StartTimer();
					}
				}
#endif
        }

#region Android Impl
        //android call escape / back btn
        SDKInterface.Instance.QuitApplicationOnEsc();
        //record create role info in five minutes to sdk
        //if (SDKInterface.Instance.CanSetCreateRoleInfoInFiveMin())
        //{
        //    durationTimeTemp += timeElapsed;
        //    if (durationTimeTemp >= 300f)
        //    {
        //        SDKInterface.Instance.SetCreateRoleInFiveInfo(ClientApplication.playerinfo.openuid,
        //                                                     GameClient.PlayerBaseData.GetInstance().RoleID.ToString());
        //        durationTimeTemp = 0f;
        //    }
        //}

        durationTimeTemp2 += timeElapsed;
        
        if (durationTimeTemp2 > 300f)
        {
             if(roleLevel < GameClient.PlayerBaseData.GetInstance().Level)
             {
                 roleLevel = GameClient.PlayerBaseData.GetInstance().Level;
                 SDKInterface.Instance.SetRoleUpLevelInfo(ClientApplication.playerinfo.openuid,
                                             GameClient.PlayerBaseData.GetInstance().RoleID.ToString(),
                                             roleLevel.ToString());
                 SDKInterface.Instance.SetCreateRoleInfo(ClientApplication.playerinfo.openuid,
                                        GameClient.PlayerBaseData.GetInstance().RoleID.ToString(),
                                        GameClient.PlayerBaseData.GetInstance().Name.ToString(),
                                        GameClient.PlayerBaseData.GetInstance().Level.ToString(),
                                        ClientApplication.adminServer.name,
                                        //ClientApplication.playerinfo.currentGateServerName,
                                        "角色段位", GameClient.PlayerBaseData.GetInstance().guildName);
             }
            durationTimeTemp2 = 0f;
        }

		UpdateCanReadFlag(timeElapsed);
		
		UpdateCheckTempure(timeElapsed);

#if MG_TEST || MG_TEST2 || MGSPTIYAN
        //_UpdateSetBuglyInfo(timeElapsed);
#endif

#endregion
    }

	void UpdateCanReadFlag(float timeElapsed)
	{
		if (!canRead)
		{
			checkAcc += timeElapsed;
			//每1秒
			if (checkAcc >= 1.0f)
			{
				checkAcc = 0;
				canRead = true;
			}
		}
	}

    void _UpdateSetBuglyInfo(float timeElapsed)
    {
        tempBuglyInfoTimer += timeElapsed;
        if (tempBuglyInfoTimer >= BUGLY_INFO_DURATION)
        {
            PluginManager.GetInstance().SetBuglyVerIdInfo();
            tempBuglyInfoTimer = 0f;
        }
    }

	void OnApplicationFocus( bool focusStatus )
	{
        if (!focusStatus)
        {
            RestoreScreenBrightness();
        }
        else
        {
            RestartBGAudio();
        }
	}

	void OnApplicationPause( bool pauseStatus )
	{
        if (pauseStatus)
        {
            RestoreScreenBrightness();
        }
        else
        {
            RestartBGAudio();
        }
	}
		
	public void InitScreenBrightnessProtect()
	{
		if (screenSaverInited)
			return;
		
		timer = new SimpleTimer2();
		//deviceScreenBrightness = SDKInterface.instance.GetScreenBrightness();
		SaveBrightness();

		timer.timeupCallBack = ()=>{
			SetLowScreenBrightness();
		};

		int countdown = TOTAL_COUNTDOWN;
		var data = TableManager.instance.GetTableItem<ProtoTable.SwitchClientFunctionTable>(5);
		if (data != null)
			countdown = data.ValueA;
		timer.SetCountdown(countdown);
		timer.StartTimer();
		screenSaverInited = true;
	}

	public void SetLowScreenBrightness()
	{
		if (screenSaverInited)
		{
			if (SDKInterface.Instance.GetScreenBrightness() != deviceScreenBrightness)
			{
				SaveBrightness();
			}
			SDKInterface.Instance.SetScreenBrightness(LOW_SCREEN_BRIGHTNESS);
		}
			
	}

	public void RestoreScreenBrightness()
	{
		if (screenSaverInited && SDKInterface.Instance.GetScreenBrightness() != deviceScreenBrightness)
		{
			SaveBrightness();
			
			SDKInterface.Instance.SetScreenBrightness(deviceScreenBrightness);
		}
	}

	void SaveBrightness()
	{
		if (SDKInterface.Instance.GetScreenBrightness() > LOW_SCREEN_BRIGHTNESS)
		{
			deviceScreenBrightness = SDKInterface.Instance.GetScreenBrightness();
		}
	}

    void RestartBGAudio()
    {
        AudioSource[] audioSources = GameObject.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        if (audioSources != null)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                var audioSource = audioSources[i];
                bool loop = audioSource.loop;
                if (loop && audioSource.clip != null)
                {
                    audioSource.Play();
                    //bool sMute = GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute;
                    //audioSource.mute = sMute;
                }
            }
        }
    }

    #if BANGCLE_EVERISK
    void OniOSBangBangEveRiskCallBack(string type)
    {
        if(type == "EMULATOR")
        {
            BangBangEveRisk.ReportEmulator(type);
        }
    }
    #endif

	public void ReportValues()
	{
#if STAT_EXTRA_INFO  	
		if (!valueUpated)
			return;
		
		valueUpated = false;

		if (cpuTempture > 0)
			GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.CPU_TEMP, cpuTempture.ToString());

		if (batteryTempture > 0)
			GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.BATTERY_TEMP, batteryTempture.ToString());

		if (frameRate > 0)
			GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.FRAME, frameRate.ToString());
		
		if (batteryConsume > 0)
			GameStatisticManager.GetInstance().DoStatSmallPackageInfo(GameStatisticManager.ExtraInfo.BATTERY, batteryConsume.ToString());
#endif			
	}
	public void SetNeedCheckTempure(bool flag)
	{
#if STAT_EXTRA_INFO  	
		needCheckTempure = flag;

		if (flag)
		{
			valueUpated = false;
			tempAcc = 0;
			InitRecordValues();
		}
#endif		
	}
	void UpdateCheckTempure(float delta)
	{
#if STAT_EXTRA_INFO  	
		if (!needCheckTempure)
			return;

		tempAcc += delta;
		if (tempAcc >= STAT_DURATION)
		{
			tempAcc = 0f;
			RecordValues();
		}
#endif		
	}

	void InitRecordValues()
	{
#if STAT_EXTRA_INFO  	
		batteryTempture 	= (int)SDKInterface.instance.GetBatteryTemperature();
		cpuTempture 		= (int)SDKInterface.instance.GetCpuTemperature();
		frameRate 			= (int)ComponentFPS.instance.GetLastAverageFPS();
		batteryLevel 		= (int)(SDKInterface.instance.GetBatteryLevel() * 100);
#endif		
	}

	void RecordValues()
	{
#if STAT_EXTRA_INFO  	
		valueUpated = true;

		int batteryV = (int)SDKInterface.instance.GetBatteryTemperature();
		_recordValue(batteryV, ref batteryTempture);

		int cpuV = (int)SDKInterface.instance.GetCpuTemperature();
		_recordValue(cpuV, ref cpuTempture);

		int currentFps = (int)ComponentFPS.instance.GetLastAverageFPS();
		_recordValue(currentFps, ref frameRate);

		int currentBatteryLevel = (int)(SDKInterface.instance.GetBatteryLevel() * 100);
		batteryConsume = batteryLevel - currentBatteryLevel;
		batteryConsume = Mathf.Max(0, batteryConsume);
		batteryLevel = currentBatteryLevel;

		GameStatisticManager.instance.RecordDownloadEndTime();
		//Logger.LogErrorFormat("[RECORD]batteryt:{0} cput:{1} fps:{2} batteryComsume:{3}", batteryV, cpuV, currentFps, batteryConsume);
#endif		
	}

	void _recordValue(int value, ref int var)
	{
#if STAT_EXTRA_INFO  	
		if (value <= 0)
			return;

		if (var <= 0)
			var = value;
		else 
			var = (var + value) / 2;
#endif			
	}


    public void OnAdultCheakRet(string params_JAVA)
    {       
        var p = ParseCallBackParams(params_JAVA);

        if (SDKInterface.Instance.adultCheakcallback != null)
        {
            if (p != null)
            {
                if (p.Length == 3)
                {
                    SDKInterface.Instance.adultCheakcallback(System.Convert.ToInt32(p[0]), System.Convert.ToInt32(p[1]), System.Convert.ToInt32(p[2]));
                }
                else
                {
                    SDKInterface.Instance.adultCheakcallback(0, (int)Protocol.AuthIDType.AUTH_ADULT, 0);
                }
            }
            else
            {
                SDKInterface.Instance.adultCheakcallback(0, (int)Protocol.AuthIDType.AUTH_ADULT, 0);
            }
        }
        else
        {
            SDKInterface.Instance.adultCheakcallback(0, (int)Protocol.AuthIDType.AUTH_ADULT, 0);
        }
    }
    string[] ParseCallBackParams(string params_JAVA)
    {
        return params_JAVA.Split(',');
    }
}
