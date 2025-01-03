using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using Protocol;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;
using DG.Tweening;

namespace GameClient
{
    public delegate bool UnityBoolAction();

    public class GameFrameWork : MonoSingleton<GameFrameWork>
    {
        public class OnGameFrameWorkLastUpdate : UnityEvent { }
        public OnGameFrameWorkLastUpdate onLastUpdate = new OnGameFrameWorkLastUpdate();            

        #region Global Load

        static private bool bInit = false;
        private bool bBaseModuleInited = false;

        #region Environment
        protected GameObject _environment;
        protected GeCamera _geCamera;

        public GeCamera MainCamera
        {
            get { return _geCamera; }
        }

        static public bool IsGameFrameWorkInited
        {
            get { return bInit; }
        }

        private void Awake()
        {
            Tenmove.Runtime.Utility.Thread.SetMainThread();
#if UNITY_STANDALONE
            Debug.developerConsoleVisible = false;
#endif
        }

        public void SetMainCamera(bool enable)
        {
            if (null == _geCamera)
            {
                Logger.LogError("[Environment] main camera is nil");
                return ;
            }

            _geCamera.enabled = enable;
        }

        public void SetMainCameraRenderTexture(RenderTexture rt)
        {
            if (_geCamera == null) 
                return;

            _geCamera.SetRenderTarget(rt);
        }

        protected void _InitializeEnvironment()
        {
			//GlobalLogic.DebugPrint();
            if (_environment == null)
            {
                GameObject environment = Utility.FindGameObject("Environment", false);
                if (environment != null)
                {
                    GameObject.DestroyImmediate(environment);
                }
                _environment = AssetLoader.instance.LoadResAsGameObject("Environment/Environment");
				if (_environment == null)
				{
					Logger.LogErrorFormat("_environment is null!!!!!!!!!!!!");
				}
                _environment.name = "Environment";
                _environment.transform.SetAsFirstSibling();

#if !RELEASE
                var comFps = _environment.SafeAddComponent<FpsShow>();
                if (comFps != null)
                    comFps.showFps = true;
#endif

                GameObject mainCameraNode = Utility.FindGameObject("Environment/FollowPlayer/Main Camera");
                Camera mainCamera = mainCameraNode.GetComponent<Camera>();

                //强制开启屏幕适配脚本
                if (mainCamera != null)
                {
                    var _gameAspectAdjust = mainCamera.GetComponent<CameraAspectAdjust>();
                    if (_gameAspectAdjust != null)
                    {
                        _gameAspectAdjust.enabled = true;
                    }
                }
                else
                {
                    Logger.LogErrorFormat("Can not find main camera.");
                }

                _geCamera = new GeCamera(mainCameraNode, mainCamera);

                GameObject.DontDestroyOnLoad(_environment);
            }
        }
        #endregion

        /// <summary>
        /// 游戏系统数据初始化（同步）
        /// </summary>
        protected void _gameDataInit()
        {   
            Utility.IterCoroutineImm(_gameDataInitCoroutine(null));
        }

        /// <summary>
        /// 游戏系统数据初始化（异步）
        /// </summary>
        public IEnumerator _gameDataInitCoroutine(UnityAction<int,string> processSet)
        {
            yield return AssetLoader.IsAssetManagerReady();

            if (!bBaseModuleInited)
            {
                yield return InitBaseModuleCoroutine(processSet);
                bBaseModuleInited = true;
            }

            yield return InitLogicModuleCoroutine(processSet);

            // if (null != processSet) processSet(1,"");
            // yield return Yielders.EndOfFrame;
            // 
            // //---------------------------------------------------------------------
            // Logger.Init();                              //Log初始化
            // PlayerLocalSetting.LoadConfig();            //本地数据加载
            // TR.Initialize(TR.EType.CN);                 //本地化表格加载(热更新调整)
            // ClientReconnectManager.instance.Clear();    //初始化重连的类
            // //---------------------------------------------------------------------
            // 
            // if (null != processSet) processSet(10,null); //进度10
            // yield return Yielders.EndOfFrame;
            // 
            // //---------------------------------------------------------------------
            // PluginManager.GetInstance();               //插件管理器初始化 
            // ExceptionManager.GetInstance();            //异常管理器初始化 
            // DOTween.Init(true);                        //DoTween初始化 
            // //---------------------------------------------------------------------
            // 
            // if (null != processSet) processSet(20,null); //进度20
            // yield return Yielders.EndOfFrame;
            // 
            // //---------------------------------------------------------------------                                 
            // TableManager.CreateInstance(false);                     //初始化表格 (热更新调整)
            // yield return TableManager.GetInstance()._InitCoroutine(processSet);   
            // //---------------------------------------------------------------------
            // 
            // //---------------------------------------------------------------------                                           
            // PlayerDataManager.GetInstance().OnApplicationStart();    //玩家数据管理初始化(热更新调整)
            // GePhaseEffect.instance.Init();                           //初始化shader(热更新调整)
            // //ManagerController.Instance().OnEnterGame();            //   
            // MissionManager.GetInstance().AddSystemInvoke();          //任务初始化
            // //---------------------------------------------------------------------
            // 
            // if (null != processSet) processSet(80,null);
            // yield return Yielders.EndOfFrame;
            // 
            // //---------------------------------------------------------------------
            // NewbieGuideManager.instance.Load();                     // 新手引导初始化
            // AssetPackageManager.CreateInstance();                   //
            // LoadingResourceManager.InitLoadingResource();           // 加载初始化资源
            // //---------------------------------------------------------------------
            // 
            // if (null != processSet) processSet(100,null);
            // yield return Yielders.EndOfFrame;
            // 
            // //---------------------------------------------------------------------
            // //GameStatisticManager.GetInstance().DoStatistic("OpenGame", StatType.DEVICE);
            // //---------------------------------------------------------------------
        }

        public IEnumerator InitBaseModuleCoroutine(UnityAction<int, string> processSet)
        {
#if ENABLE_REMOTE_CONFIG
            yield return RemoteConfig.instance.FetchRemoteConfig();
            Debug.Log("### Begin init Bugly!");
            if (RemoteConfig.instance.IsAndroidInBlackList(
                RemoteConfig.instance.GetCurrentOSAndroidAPILevel()))
                PluginManager.InitBugly();
            Debug.Log("### End init Bugly!");
#endif

            if (null != processSet) processSet(1, "");
			yield return Yielders.EndOfFrame;

            //---------------------------------------------------------------------
            Logger.Init();                              //Log初始化
            
            PlayerLocalSetting.LoadConfig();            //本地数据加载
            ClientReconnectManager.instance.Clear();    //初始化重连的类
            //---------------------------------------------------------------------

            if (null != processSet) processSet(10, null); //进度10
            yield return Yielders.EndOfFrame;

            //---------------------------------------------------------------------
            PluginManager.CreateInstance();             //插件管理器初始化 
            ExceptionManager.CreateInstance();          //异常管理器初始化 
            DOTween.Init(true);                         //DoTween初始化 
            //---------------------------------------------------------------------

            if (null != processSet) processSet(20, null); //进度20
            yield return Yielders.EndOfFrame;
        }

        public IEnumerator InitLogicModuleCoroutine(UnityAction<int, string> processSet)
        {
            if (null != processSet) processSet(21, "");
            yield return Yielders.EndOfFrame;

			//Logger.LogErrorFormat("[INIT PROCESS]start AddPackageDependency");

            if(EngineConfig.asyncPackageLoad)
            {
                AssetAsyncLoader.instance.ClearWaitingQueue();
                if (AssetAsyncLoader.instance.IsAsyncInLoading)
                {
                    yield return Yielders.EndOfFrame;
                }
                AssetAsyncLoader.instance.ClearFinishQueue();
            }

            AssetPackageManager.instance.AddPackageDependency();
            AssetGabageCollectorHelper.instance.LoadGCConfig();

            if (SingletonData<PackScriptData>.IsActiveInstance())
            {
                SingletonData<PackScriptData>.Instance.Clear();
            }

            SingletonData<PackScriptData>.Instance.Initialize();

            //Logger.LogErrorFormat("[INIT PROCESS]start _InitializeEnvironment");
            _InitializeEnvironment();

			//Logger.LogErrorFormat("[INIT PROCESS]start TR.Initialize");
            TR.Initialize(TR.EType.CN);                             //本地化表格加载(热更新调整)
            //---------------------------------------------------------------------                                 
            TableManager.CreateInstance(false);                     //初始化表格 (热更新调整)
            yield return TableManager.GetInstance()._InitCoroutine(processSet);


            //---------------------------------------------------------------------

		//	XLuaManager.GetInstance().Load();			//初始化xlua

            //---------------------------------------------------------------------                                           
            PlayerDataManager.GetInstance().OnApplicationStart();    //玩家数据管理初始化(热更新调整)
            GePhaseEffect.CreateInstance(true);                           //初始化shader(热更新调整)
            //ManagerController.Instance().OnEnterGame();            //   
            MissionManager.GetInstance().AddSystemInvoke();          //任务初始化
            TAPNewDataManager.GetInstance().AddSystemInvoke();
            //---------------------------------------------------------------------

            if (null != processSet) processSet(80, null);
            yield return Yielders.EndOfFrame;

            //---------------------------------------------------------------------
            NewbieGuideManager.instance.Load();                     // 新手引导初始化
            LoadingResourceManager.InitLoadingResource();           // 加载初始化资源
            //---------------------------------------------------------------------

            //初始化设置
            SettingFrame.LoadDoublePressConfig();

            if (null != processSet) processSet(100, null);
#if  TEST_SILLFILE_LOAD          
            TestSkillConfigLoad();
#endif            

            yield return Yielders.EndOfFrame;
        }

        public void DeinitLogicModule()
        {
            LoadingResourceManager.DeinitLoadingResource();
            AssetPackageManager.instance.UnInit();
            NewbieGuideManager.instance.Unload();

            MissionManager.GetInstance().RemoveSystemInvoke();
            TAPNewDataManager.GetInstance().RemoveSystemInvoke();

            PlayerDataManager.instance.OnApplicationQuit();
            if(TableManager.bNeedUninit)
                TableManager.instance.UnInit();
        }

        void InitGlobalSetting()
        {
            if (Global.Settings.isBanShuVersion)
            {
                Global.Settings.isGuide = false;
            }
        }

        #endregion

        void Init()
        {
        #if ENABLE_TMPROFILER
            InitUWA();
        #endif

			InitGlobalSetting();
            
            Network.NetManager.Instance();
            ClientSystemManager.Initialize();
            FrameManager.Initialize();
            
            Application.targetFrameRate = 30;


		

            //在这里初始化bugly和exceptionmanager

            if (!DebugSettings.GetInstance().DisableBugly)
            {
#if BUGLY_ENABLE
                PluginManager.InitBugly();
#endif
                ExceptionManager.CreateInstance();
            }
			
            // 设置GC参数
            iOSConfigureLibGC.ConfigureLibGC();

            //保持屏幕常亮
            PluginManager.KeepScreenOn();

#if APPLE_STORE
            //For IOS Appstore Check
            PluginManager.GetInstance().InititalSDK();
#endif
        }

#if ENABLE_TMPROFILER
        private void InitUWA()
        {
            GameObject uwaPrefab = Resources.Load("UWA/Prefabs/UWA_Android") as GameObject;
            if(uwaPrefab != null)
            {
                GameObject uwaInstance = GameObject.Instantiate(uwaPrefab);
                GameObject.DontDestroyOnLoad(uwaInstance);
            }
        }
#endif
        
        protected override void OnApplicationQuit()
        {
            PlayerDataManager.GetInstance().OnApplicationQuit();
            base.OnApplicationQuit();
        }

        public IEnumerator _FadingFrame(UnityAction preAction,UnityAction postAction,UnityBoolAction waitForAction,float fadeIn,float fadeOut)
        {
            FadingFrame frame = ClientSystemManager.GetInstance().OpenFrame<FadingFrame>(FrameLayer.Top) as FadingFrame;
            frame.FadingIn(fadeIn);
            
            while (frame.IsOpened() == false)
            {
                yield return Yielders.EndOfFrame;
            }

            yield return Yielders.EndOfFrame;

            if(preAction != null)
            {
                preAction.Invoke();
            }

            yield return Yielders.EndOfFrame;

            if (waitForAction != null)
            {
                while(waitForAction.Invoke() == true)
                {
                    yield return Yielders.EndOfFrame;
                }
            }

            frame.FadingOut(fadeOut);

            yield return Yielders.EndOfFrame;

            while (frame.IsClosed() == false)
            {
                yield return Yielders.EndOfFrame;
            }

            if(postAction != null)
            {
                postAction.Invoke();
            }
        }

        public UnityEngine.Coroutine OpenFadeFrame(UnityAction preAction,UnityAction postAction,UnityBoolAction waitForAction = null,float fadeIn = 0.4f,float fadeOut = 0.6f)
        {
             return  StartCoroutine(_FadingFrame(preAction,postAction,waitForAction,fadeIn,fadeOut)); 
        }


        private GameObject lastTownName = null;
        public void TownNameShow(string name)
        {
            if (lastTownName != null)
            {
                DestroyImmediate(lastTownName);
                lastTownName = null;
            }
            var obj = ClientSystemManager.instance.PlayUIEffect(GameClient.FrameLayer.Middle, "UIFlatten/Prefabs/TownUI/TownNameShow");
            if (null != obj)
            {
                lastTownName = obj;
                var show = obj.GetComponent<HGTownShow>();
                if (show && show.control)
                {
                    show.control.text = name;
                }
            }
        }

        void Start()
        {
            Input.multiTouchEnabled = true;
            SwithTouchInput(false);
            GameObject.DontDestroyOnLoad(gameObject);

            GameObject engineRoot = GameObject.Find("TMEngine");
            if (null == engineRoot)
            {
                engineRoot = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Base/TMEngine"));
                engineRoot.name = "TMEngine";
            }
            var fps = ComponentFPS.instance;
        }

        static long lastTime;
        static float lastShowTime;
        void Update()
        {
            float now = Time.realtimeSinceStartup;
            if (now - lastShowTime >= 1.0f)
            {
                Network.NetManager.Instance().Log("gameframework start tick...");
                lastShowTime = now;
            }

            if (!bInit)
            {
                Init();

                switch (Global.Settings.startSystem)
                {
                    case EClientSystem.Login:
                        ClientSystemManager.instance.InitSystem<ClientSystemVersion>();
                        break;
                    case EClientSystem.Town:
                        _gameDataInit();
                        ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
                        break;
                    case EClientSystem.Battle:
                        _gameDataInit();
                        ClientSystemManager.instance.SwitchSystem<ClientSystemBattle>();
                        break;
                    default:
                        Logger.LogError("请检查GlobalSetting设置，需要选择一个模式进入！");
                        break;
                }

                bInit = true;
            }


            //Logger.LogProcessFormat("ClientSystemManager Update Begin!");
            float timeElapsed = Time.deltaTime;
            ClientSystemManager.instance.Update(timeElapsed);
            //Logger.LogProcessFormat("ClientSystemManager Update End!");
        }

        void LateUpdate()
        {
            if(onLastUpdate != null)
            {
                onLastUpdate.Invoke();
            }
        }

        public void SwithTouchInput(bool flag = true){}    

        Dictionary<string, BDEntityActionInfo> actionFramesMap = new Dictionary<string, BDEntityActionInfo>();
        string[] jobSkillFiles = new string[]
        {
            "SkillData/Common",
            "SkillData/Fightergirl",
            "SkillData/Gungirl",
            "SkillData/Gunman",
            "SkillData/Mage",
            "SkillData/Paladin",
            "SkillData/Swordman",
        };
        
        public bool IsJobSkillFile(string path)
        {
            bool ret = false;
            foreach(var item in jobSkillFiles)
            {
                if (path.Contains(item))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public BDEntityActionInfo GetGlobalSkillConfig(string filename)
        {
            if (actionFramesMap.ContainsKey(filename))
                return actionFramesMap[filename];
            
            return null;
        }

        public void TestSkillConfigLoad()
        {
#if  TEST_SILLFILE_LOAD      
            var ins = SingletonData<PackScriptData>.Instance; 
            //Logger.LogErrorFormat("TestSkillConfigLoad");


            
            foreach(var currenItem in ins.header_dic_string)
            {
                var path = currenItem.Key;

                var list = SkillFileListCache.GetCached(path);
                if (list == null)
                {
                    Logger.LogErrorFormat("can't get skill file cache:{0}", path);
                    continue;
                }

                for (int i = 0; i < list.Count; ++i)
                {
                    var current = list[i];
                    string filename = (string)current.fullPath;

                    if (!IsJobSkillFile(filename))
                        continue;

                    var skillRes = AssetLoader.instance.LoadRes(filename, typeof(DSkillData)).obj;
                    if (skillRes != null)
                    {
                        DSkillData data = skillRes as DSkillData;
                        var info = new BDEntityActionInfo(filename);
                        info.InitWithDataRes(data);
                        actionFramesMap.Add(filename, info);

                        skillRes = null;
                    }
                }
            }

            //Logger.LogErrorFormat("skill file count:{0}", actionFramesMap.Count);
#endif
        }  

    }

    
}
