using UnityEngine;
using System.Collections;
using Network;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Reflection;
using ActivityLimitTime;
using Protocol;

namespace  GameClient
{
    public delegate IEnumerator loadingCoroutine(IASyncOperation op);
    public delegate bool AddCoroutine(loadingCoroutine coroutine,string name = "",float weight = 1.0f);

    public enum EClientSystem
    {
        Login = 0,
        Town,
        Battle,
    }

    public class SystemContent
    {
        public delegate void OnStart();
        public OnStart onStart;
    }

    public class ClientSystemManager : Singleton<ClientSystemManager>,IClientFrameManager, IClientSystemFrameStack
    {
        protected DictionaryView<string, IClientSystem> _clientSystems = new DictionaryView<string, IClientSystem>();
        public IClientSystem CurrentSystem { get; set; }
        public IClientSystem TargetSystem { get; set; }
        public float SwitchProgress { get; private set; }
        public string SwitchDescription { get; private set; }

        public bool bIsInPkWaitingRoom { get; set; }

        public DelayCaller delayCaller = new DelayCaller();

        public const string kInterfaceName = "GameClient.IClientFrame";
        public const string kGlobalTag = "GlobalFrame";

        public static bool sRemoveRefOnClose = false;

        // 获取当前系统的接口，以后尽量全部统一使用这个接口,
        // 不要再直接这么使用：“ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown”,
        // 特别是在切系统的过程中，用上述方法对CurrentSystem进行判断时，CurrentSystem尚为被赋值为目标系统,
        // CurrentSystem是在切系统完成的最后才被赋值的 by wangbo 2018.01.26, 06:12
        public IClientSystem GetCurrentSystem()
        {
            if (TargetSystem != null)
            {
                return TargetSystem;
            }

            if (CurrentSystem != null)
            {
                return CurrentSystem;
            }

            return null;
        }

        #region Audio
        protected void _InitializeAudio()
        {
            AudioManager.instance.Init();

            SoundConfig bgConfig = SystemConfigManager.GetInstance().SystemConfigData.SoundConfig;
            SoundConfig seConfig = SystemConfigManager.GetInstance().SystemConfigData.MusicConfig;
            SoundConfig envBgConfig = SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig;

            AudioManager.instance.SetVolume(AudioType.AudioStream, (float)bgConfig.Volume);
            AudioManager.instance.SetVolume(AudioType.AudioEffect, (float)seConfig.Volume);
            AudioManager.instance.SetVolume(AudioType.AudioVoice, (float)seConfig.Volume);
            AudioManager.instance.SetVolume(AudioType.AudioEnvironment, (float)envBgConfig.Volume);
            NpcVoiceCachedManager.instance.SetVolume((float)seConfig.Volume);

            AudioManager.instance.SetMute(AudioType.AudioStream, bgConfig.Mute);
            AudioManager.instance.SetMute(AudioType.AudioEffect, seConfig.Mute);
            AudioManager.instance.SetMute(AudioType.AudioVoice, seConfig.Mute);
            AudioManager.instance.SetMute(AudioType.AudioEnvironment, seConfig.Mute);
        }
        #endregion

        #region Frame

        #region UI

        protected void _InitializeUI()
        {
            _InitializeUIRoot();
            _InitializeUICamera();
            _Initialize2DUIRoot();
            _Initialize3DUIRoot();
        }

        #region UIRoot
        protected GameObject _layerRoot;
        
        protected void _InitializeUIRoot()
        {
            _layerRoot = Utility.FindGameObject("UIRoot", false);
            if (_layerRoot != null)
            {
                GameObject.Destroy(_layerRoot);
                _layerRoot = null;
            }

            _layerRoot = AssetLoader.instance.LoadResAsGameObject("Base/UI/Prefabs/Root/UIRoot");
            _layerRoot.SetActive(true);
            _layerRoot.name = "UIRoot";
            _layerRoot.transform.SetAsFirstSibling();

            GameObject.DontDestroyOnLoad(_layerRoot);
        }
        #endregion

        #region UICamera
        protected Camera _uiCamera;
        public Camera UICamera
        {
            get
            {
                return _uiCamera;
            }
        }

        protected void _InitializeUICamera()
        {
            var camera = Utility.FindGameObject(_layerRoot, "UICamera");
            if (camera != null)
            {
                _uiCamera = camera.GetComponent<Camera>();
            }
        }
        #endregion

        #region Layer2DRoot
        protected GameObject[] _layer2DRoots = new GameObject[0];
        protected GameObject[] LayerRoots
        {
            get { return _layer2DRoots; }
        }

        GameObject m_objSceneUILayer = null;

        public GameObject GetLayer(FrameLayer layer)
        {
            if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
            {
                return _layer2DRoots[(int)layer];
            }

            return null;
        }

        public GameObject SceneUILayer
        {
            get
            {
                return m_objSceneUILayer;
            }
        }

        public GameObject BackgroundLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.Background];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        public GameObject BottomLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.Bottom];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        public GameObject MiddleLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.Middle];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        public GameObject HorseLampLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.HorseLamp];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        public GameObject BelowMiddle
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.BelowMiddle];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        public GameObject HighLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.High];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }


        public GameObject TopLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.Top];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        public GameObject TopMostLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.TopMost];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        public GameObject TopMoreMostLayer
        {
            get
            {
                if (_layer2DRoots.Length == (int)FrameLayer.LayerMax)
                {
                    return _layer2DRoots[(int)FrameLayer.TopMoreMost];
                }
                else
                {
                    Logger.LogError("ClientSystem._layerRoots not initialized!!");
                    return null;
                }
            }
        }

        protected Canvas _canvas;
        public Canvas UI2DCanvas
        {
            get
            {
                return _canvas;
            }
        }

        protected void _Initialize2DUIRoot()
        {
            _layer2DRoots = new GameObject[(int)FrameLayer.LayerMax];

            GameObject canvasRoot = Utility.FindGameObject(_layerRoot, "UI2DRoot");
            //if (canvasRoot != null)
            //{
            //    GameObject.Destroy(canvasRoot);
            //    canvasRoot = null;
            //}
            //canvasRoot = AssetLoader.instance.CreateGameObject("UI/Prefabs/UI2DRoot");
            //canvasRoot.name = "UI2DRoot";
            //canvasRoot.transform.SetAsFirstSibling();
            //GameObject.DontDestroyOnLoad(canvasRoot);

            _canvas = canvasRoot.GetComponent<Canvas>();

            string[] names = new string[]
             {
                   "Background",
                   "Bottom",
                   "BelowMiddle",
                   "Middle",
                   "HorseLamp",
                   "High",
                   "Top",
                   "TopMost",
                   "TopMoreMost"
             };
            for (int i = 0; i < (int)FrameLayer.LayerMax; ++i)
            {
                _layer2DRoots[i] = Utility.FindGameObject(canvasRoot, names[i]);

                if (_layer2DRoots[i] == null)
                {
                    GameObject obj = new GameObject
                    {
                        name = names[i]
                    };

                    RectTransform rectTransfor = obj.AddComponent<RectTransform>();

                    rectTransfor.anchorMax = Vector2.one;
                    rectTransfor.anchorMin = Vector2.zero;

                    rectTransfor.offsetMax = Vector2.zero;
                    rectTransfor.offsetMin = Vector2.zero;

                    _layer2DRoots[i] = obj;
                    Utility.AttachTo(obj, canvasRoot);
                }
            }

            m_objSceneUILayer = Utility.FindGameObject(canvasRoot, "SceneUI");
        }

        #endregion

        #region Layer3DRoot
        protected GameObject _layer3DRoot;
        protected void _Initialize3DUIRoot()
        {
            _layer3DRoot = Utility.FindGameObject(_layerRoot, "UI3DRoot", true);
            //if (_layer3DRoot != null)
            //{
            //    GameObject.Destroy(_layer3DRoot);
            //    _layer3DRoot = null;
            //}
            //_layer3DRoot = AssetLoader.instance.CreateGameObject("UI/Prefabs/UI3DRoot");
            //_layer3DRoot.name = "UI3DRoot";
            //GameObject.DontDestroyOnLoad(_layer3DRoot);
        }

        public GameObject Layer3DRoot
        {
            get { return _layer3DRoot; }
        }

		public void Clear3DUIRoot()
		{
			for(int i=0; i<_layer3DRoot.transform.childCount; ++i)
			{
				var go = _layer3DRoot.transform.GetChild(i).gameObject;
				if (go != null)
				{
					GameObject.Destroy(go);
				}
			}

		}

        #endregion

        #endregion

        protected DictionaryView<string, IClientFrame> _frameDics = new DictionaryView<string, IClientFrame>();
        protected List<IClientFrame> _activeFrames = new List<IClientFrame>();

        protected class ClientFrameGroup
        {
            public ClientFrameGroup(string name)
            {
                mGroupTag = name;
                mClientFrames = new List<string>();
            }

            protected string mGroupTag;

            protected string mHiddenTag = "";

            /// <summary>
            /// 存ClientFrame的名字
            /// </summary>
            protected List<string> mClientFrames;

            public string GroupTag()
            {
                return mGroupTag;
            }

            public string GetHiddentGroup()
            {
                return mHiddenTag;
            }

            public bool IsGroupShow()
            {
                return mHiddenTag.Length <= 0;
            }

            public void SetHiddenGroup(string tag)
            {
                mHiddenTag = tag;
            }

            public List<string> ClientFrames()
            {
                return mClientFrames;
            }
        }
        protected DictionaryView<string, ClientFrameGroup> _clientGroups = new DictionaryView<string, ClientFrameGroup>();

        private bool _checkIsValidType(Type type)
        {
            if (type == null)
            {
                Logger.LogError("type is nil");
                return false;
            }

            if (type.IsClass && type.GetInterface(kInterfaceName) != null)
            {
                return true;
            }

            Logger.LogErrorFormat("not valid type with name {0}", type.Name);
            return false;
        }

        private IClientFrame _getFrameByName(string name)
        {
            IClientFrame frame = null;

            if (!_frameDics.TryGetValue(name, out frame))
            {
                Logger.LogWarningFormat("Frame {0} Can not Find", name);
            }

            return frame;

        }

        private IClientFrame _getFrameByType(Type type)
        {
            if (!_checkIsValidType(type))
            {
                return null;
            }

            return _getFrameByName(type.Name);
        }

        #region OpenFrame

		public IClientFrame OpenFrame(string luaFrameName)
		{
			//IClientFrame frame = XLuaManager.GetInstance().luaEnv.Global.Get<IClientFrame>(luaFrameName);

			GameObject root = null;
			if (root == null)
			{
				root = LayerRoots[(int)FrameLayer.Top];
			}
				
			string frameName = luaFrameName;
			IClientFrame frame = null;
			// TODO 这里传进来的名字如果和已有的ClientFrame重名，那么就容易引起问题
			if (!_frameDics.TryGetValue(frameName, out frame))
			{
		//		frame = XLuaManager.GetInstance().luaEnv.Global.Get<IClientFrame>(luaFrameName);
				frame.SetManager(this);
				frame.SetFrameName(frameName);
				_frameDics.Add(frameName, frame);
			}

			frame.Open(root, null);
			frame.SetGlobal(false);

			return frame;
		}
        
        public IClientFrame OpenGlobalFrame<T>(FrameLayer layer,object userData = null) where T : class, IClientFrame
        {
            IClientFrame frame = OpenFrame(typeof(T), layer,userData);
            frame.SetGlobal(true);
            return frame;
        }

        public IClientFrame OpenFrame<T>(GameObject root, object userData = null, string name = "") where T : class, IClientFrame
        {
            return OpenFrame(typeof(T), root, userData, name);
        }

        public IClientFrame OpenFrame(GameObject root, Type type, object userData = null, string name = "")
        {
            return OpenFrame(type, root, userData, name);
        }

        public IClientFrame OpenFrame<T>(FrameLayer layer = FrameLayer.Middle, object userData = null, string name = "") where T : class, IClientFrame
        {
            return OpenFrame(typeof(T), layer, userData, name);
        }

        public IClientFrame OpenFrame(Type type, FrameLayer layer = FrameLayer.Middle, object userData = null, string name = "")
        {
            return OpenFrame(type, LayerRoots[(int)layer], userData, name, layer);
        }

        /// <summary>
        /// 打开界面的基础函数，所有其他接口最终都要调用这个接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="root"></param>
        /// <param name="userData"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        public IClientFrame OpenFrame(Type type, GameObject root, object userData = null, string name = "", FrameLayer layer = FrameLayer.Invalid)
        {
            if (!_checkIsValidType(type))
            {
                return null;
            }

            if(LayerRoots == null)
            {
                Logger.LogErrorFormat("OpenFrame时LayerRoots == null,type = {0},name = {1}", type, name);
                return null;
            }

            if (root == null)
            {
                root = LayerRoots[(int)FrameLayer.Middle];
            }

            if(root == null)
            {
                Logger.LogErrorFormat("root被LayerRoots赋值后依然为null,LayerRoots[(int)FrameLayer.Middle] = {0}", LayerRoots[(int)FrameLayer.Middle]);
                return null;
            }

            IClientFrame frame;
            string frameName = name;
            if (frameName == "")
            {
                frameName = type.Name;
            }

            if (frameName == "SmithShopFrame")
            {
                if (PkWaitingRoom.bBeginSeekPlayer)
                {
                    SystemNotifyManager.SystemNotify(4004);
                    return null;
                }
            }

            // TODO 这里传进来的名字如果和已有的ClientFrame重名，那么就容易引起问题
            if (!_frameDics.TryGetValue(frameName, out frame))
            {
                frame = Activator.CreateInstance(type) as IClientFrame;
                frame.SetManager(this);
                frame.SetFrameName(frameName);
                _frameDics.Add(frameName, frame);

                Logger.LogProcessFormat("[frame] 实例化界面对象 {0}", type);
            }

            if (frame == null)
            {
                Logger.LogErrorFormat("OpenFrame时frame为null的唯一情况就是从_frameDics取出来的值本身就是null,frameName = {0}", frameName);
                return null;
            }

            frame.Open(root, userData, layer);
            frame.SetGlobal(false);

            Logger.LogProcessFormat("[frame] 打开界面 {0}, {1}", frameName, type);

            // TODO 如果是用ClientFrame的Close去关闭，那么这个_activeFrame中还有残留
            if (!_activeFrames.Contains(frame))
            {
                _activeFrames.Add(frame);
                _addMutexFrames(frame);
            }
            else
            {
                Logger.LogWarningFormat("already contain the frame {0}, with type {1}", frameName, type);
            }

            UIEvent uiEvent = new UIEvent
            {
                EventID = EUIEventID.FrameOpen,
                Param1 = frameName,
                Param2 = type
            };

            GlobalEventSystem.GetInstance().SendUIEvent(uiEvent);

            return frame;
        }
        #endregion

        #region MutexFrame
        private void _addMutexFrames(IClientFrame frame)
        {
            string groupTag = frame.GetGroupTag();
            if (groupTag.Length > 0)
            {
                if (!_clientGroups.ContainsKey(groupTag))
                {
                    _clientGroups.Add(groupTag, new ClientFrameGroup(groupTag));
                }

                List<string> groupFrames = _clientGroups[groupTag].ClientFrames();
                
                Logger.LogProcessFormat("[mutex] 打开界面 {0}, 标签 {1}, 已有数目 {2}", frame.GetFrameName(), groupTag, groupFrames.Count);

                if (!_clientGroups[groupTag].IsGroupShow())
                {
                    Logger.LogProcessFormat("[mutex] 该互斥组{0}已经隐藏，隐藏界面{1}", groupTag, frame.GetFrameName());
                    frame.Show(false);
                }
                else if (groupFrames.Count <= 0)
                {
                    _updateMutextFrames(groupTag, true);
                }

                groupFrames.Add(frame.GetFrameName());
            }
        }

        private void _removeMutexFrames(IClientFrame frame)
        {
            string tag = frame.GetGroupTag();

            Logger.LogProcessFormat("[mutex] 开始 关闭界面 {0}, 标签 {1}", frame.GetFrameName(), tag);

            if (tag.Length > 0)
            {
                if (_clientGroups.ContainsKey(tag))
                {
                    List<string> groupFrame = _clientGroups[tag].ClientFrames();
                    groupFrame.Remove(frame.GetFrameName());

                    Logger.LogProcessFormat("[mutex] 关闭界面 {0}, 标签 {1}, 剩余数目 {2}", frame.GetFrameName(), tag, groupFrame.Count);

                    if (groupFrame.Count <= 0)
                    {
                        _updateMutextFrames(tag, false);
						_clientGroups.Remove (tag);
                    }
                }
                else
                {
                    Logger.LogErrorFormat("can't find the groupTag {0} with frame name {1} ", tag, frame.GetFrameName());
                }
            }
        }

        private void _showGroupFrame(ClientFrameGroup group, bool isShow)
        {
            List<string> groupFrames = group.ClientFrames();

            for (int i = 0; i < groupFrames.Count; ++i)
            {
                string frameName = groupFrames[i];

                IClientFrame frame = _getFrameByName(frameName);

                if(frame.NeedMutex() == false)
                {
                    continue;
                }
                
                Logger.LogProcessFormat("[mutex] 组{0}, 界面{1}, 操作 {2}", group.GroupTag(), frameName, isShow ? "显示" : "隐藏");

                if (null != frame)
                {
                    frame.Show(isShow);
                }
            }

            if (group.GroupTag() == "system")
            {
                ClientSystem sys = CurrentSystem as ClientSystem;
                if (null != sys)
                {
                    Logger.LogProcessFormat("[mutex] 组{0}, ClientSystem界面, 操作 {1}", group.GroupTag(), isShow ? "显示" : "隐藏");
                    sys.ShowMainFrame(isShow);
                }
            }
        }

        private void _updateMutextFrames(string tag, bool isShow)
        {
            var it = _clientGroups.GetEnumerator();

            Logger.LogProcessFormat("[mutex] 更新互斥组{0}, 一共有{1}个组", tag, _clientGroups.Count);

            while (it.MoveNext())
            {
                if (it.Current.Key != tag)
                {
                    ClientFrameGroup group = it.Current.Value;
                    string hiddenTag = group.GetHiddentGroup();

                    if (isShow && hiddenTag.Length <= 0)
                    {
                        Logger.LogProcessFormat("[mutex] 打开组{0}, 隐藏组{1}", tag, it.Current.Key);

                        group.SetHiddenGroup(tag);
                        _showGroupFrame(group, !isShow);
                    }
                    else if (!isShow && hiddenTag == tag)
                    {
                        Logger.LogProcessFormat("[mutex] 关闭组{0}, 显示组{1}", tag, it.Current.Key);
                        group.SetHiddenGroup("");
                        _showGroupFrame(group, !isShow);
                    }
                }
            }
        }

        private void _clearMutextFrameHiddenTag()
        {
            _clientGroups.Clear();
            _clientGroups.Add("system", new ClientFrameGroup("system"));
        }

        private ClientFrameGroup _findClientGroupByFrameName(string clientframename)
        {
            var iter = _clientGroups.GetEnumerator();

            while (iter.MoveNext())
            {
                List<string> list = iter.Current.Value.ClientFrames();

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] == clientframename)
                    {
                        return iter.Current.Value;
                    }
                }
            }

            return null;
        }

        private List<string> _gatherAllMutexFrame()
        {
            List<string>     keepFrame   = new List<string>();

            var iter = _clientGroups.GetEnumerator();
            while (iter.MoveNext())
            {
                keepFrame.AddRange(iter.Current.Value.ClientFrames());
            }

            return keepFrame;
        }

        private List<string> _forceShowGroup(ClientFrameGroup group)
        {
            if (null == group || group.IsGroupShow())
            {
                return _gatherAllMutexFrame();
            }

            string           hiddentag   = group.GetHiddentGroup();
            ClientFrameGroup hiddengroup = _clientGroups[hiddentag];
            Stack<string>    allgroups   = new Stack<string>();

            while (null != hiddengroup)
            {
                allgroups.Push(hiddengroup.GroupTag());

                if (_clientGroups.ContainsKey(hiddengroup.GetHiddentGroup()))
                {
                    hiddengroup = _clientGroups[hiddengroup.GetHiddentGroup()];
                }
                else 
                {
                    break;
                }
            }

            while (allgroups.Count > 0)
            {
                string tag = allgroups.Pop();

                Logger.LogProcessFormat("[mutex-clear] 关闭组 {0}", tag);

                if (_clientGroups.ContainsKey(tag))
                {
                    List<string> frames = _clientGroups[tag].ClientFrames();

                    for (int i = 0; i < frames.Count; ++i)
                    {
                        IClientFrame frame = _getFrameByName(frames[i]);
                        if (null != frame)
                        {
                            Logger.LogProcessFormat("[mutex-clear] 关闭互斥界面 {0}", frame.GetFrameName());
                            _closeFrame(frame);
                        }
                    }
                }
            }

            return _gatherAllMutexFrame();
        }

        private void _forceClearFrameExcept(List<string> framenames)
        {
            List<IClientFrame> frames = new List<IClientFrame>();
            string name               = "";
            bool   flag               = false;

            for (int i = 0; i < _activeFrames.Count; ++i)
            {
                name = _activeFrames[i].GetFrameName();
                flag = true;

                for (int j = 0; j < framenames.Count; ++j)
                {
                    if (name == framenames[j])
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    frames.Add(_activeFrames[i]);
                }
            }

            for (int i = 0; i < frames.Count; ++i)
            {
                Logger.LogProcessFormat("[mutex-clear] 关闭其他界面 {0}", frames[i].GetFrameName());
                _closeFrame(frames[i]);
            }

            frames.Clear();
        }

        public bool IsMainPrefabTop()
        {
            ClientFrameGroup group = _clientGroups["system"];

            if (group != null)
            {
                List<string> groupFrames = group.ClientFrames();

                for (int i = 0; i < groupFrames.Count; ++i)
                {
                    string framename = groupFrames[i];
                    IClientFrame fg = _activeFrames.Find(x=>{ return x.GetFrameName() == framename; });

                    if (null == fg || fg.IsHidden())
                    {
                        return false;
                    }
                }

                if (_activeFrames.Count > groupFrames.Count)
                {
                    return false;
                }

                return true;
            }

            return false;
        }


        public void ForceClearFrame(string framename)
        {
            ClientFrameGroup group = _findClientGroupByFrameName(framename);
            if (group == null)
            {
                group = _clientGroups["system"];
            }

            Logger.LogProcessFormat("[mutex-clear] 强制显示 {0}, 组标识 {1}", framename, group.GroupTag());

            List<string> frames = _forceShowGroup(group);
            frames.Add(framename);

            _forceClearFrameExcept(frames);
        }

        #endregion

#region FullScreen
        private int mFullScreenDirtyCount = 0;
        private bool mFullScreenDirtyFlag = false;
        private List<IClientFrame> mFullScreenFrames = new List<IClientFrame>();
        private List<IClientFrame> mCacheFullScreenFramesToClose = new List<IClientFrame>();

        public void NotifyFrameIsOpen(IClientFrame frame)
        {
            if (null == frame)
            {
                Logger.LogError("[ClientFrameManager] 传入frame为空");
                return ;
            }

            mFullScreenFrames.Add(frame);
            mFullScreenDirtyFlag = true;
        }

        public void NotifyFrameIsClose(IClientFrame frame)
        {
            if (null == frame)
            {
                Logger.LogError("[ClientFrameManager] 传入frame为空");
                return;
            }

            mFullScreenFrames.Remove(frame);
            mFullScreenDirtyFlag = true;
        }

        private void _updateFullScreen(float delta)
        {
            if (mFullScreenDirtyFlag)
            {
                bool enabled = mFullScreenFrames.Count <= 0;

                Logger.LogProcessFormat("[ClientFrameManager] 全屏界面计数 {0}, {1}", mFullScreenFrames.Count, enabled);

                GameFrameWork.instance.SetMainCamera(enabled);

                _tryCloseNeedCloseFullScreenFrames();

                if (CurrentSystem is ClientSystemTown)
                {
                    var townFrame = GetFrame(typeof(ClientSystemTownFrame));
                    if (townFrame != null)
                    {
                        townFrame.Show(enabled);
                    }
                }

                mFullScreenDirtyFlag = false;
            }
        }

        private void _tryCloseNeedCloseFullScreenFrames()
        {
            mCacheFullScreenFramesToClose.Clear();

            for (int i = 0; i < mFullScreenFrames.Count - 1; ++i)
            {
                if (mFullScreenFrames[i].IsFullScreenFrameNeedBeClose())
                {
                    mCacheFullScreenFramesToClose.Add(mFullScreenFrames[i]);
                }
            }

            Logger.LogProcessFormat("[ClientFrameManager] 全面界面需要被关闭 {0}", mCacheFullScreenFramesToClose.Count);

            for (int i = 0; i < mCacheFullScreenFramesToClose.Count; ++i)
            {
                CloseFrame(mCacheFullScreenFramesToClose[i]);
            }

            mCacheFullScreenFramesToClose.Clear();
        }

        private void _clearFullScreenStatus()
        {
            Logger.LogProcessFormat("[ClientFrameManager] 清楚 全屏界面计数");
            mFullScreenFrames.Clear();
            mCacheFullScreenFramesToClose.Clear();
            mFullScreenDirtyFlag = false;
            GameFrameWork.instance.SetMainCamera(true);
        }
#endregion

        #region CloseFrame

        /// <summary>
        /// 关闭界面的基本函数，所有的关闭界面最终都要调用到这里
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="bImmediately"></param>
        private void _closeFrame(IClientFrame frame, bool bImmediately = false)
        {
            if (frame == null || !frame.IsOpen())
            {
                Logger.LogFormat("Frame {0} Is not Opened Can not Close", frame.GetType().Name);
                return;
            }

            Logger.LogProcessFormat("[frame] 关闭ClientFrame {0}", frame.GetFrameName());
            //这里不要添加处理逻辑
            frame.Close(bImmediately);
        }

        public void OnFrameClose(IClientFrame frame,bool removeRef)
        {
            if(frame == null)
            {
                Logger.LogProcessFormat("[frame] 移除ClientFrame失败");

                return;
            }

            Logger.LogProcessFormat("[frame] 成功移除ClientFrame {0}", frame.GetFrameName());

            _activeFrames.Remove(frame);
            if(removeRef)
                _frameDics.Remove(frame.GetFrameName());

            _removeMutexFrames(frame);
        }

        public void CloseFrameByType(Type type, bool bImmediately = false)
        {
            IClientFrame frame = _getFrameByType(type);
            _closeFrame(frame, bImmediately);
        }

        public void CloseFrame<T>(T frame = null, bool bImmediately = false) where T : class, IClientFrame
        {
            Type t = typeof(T);
            if (null == frame)
            {
                frame = _getFrameByType(t) as T;

                if (null == frame)
                {
                    return;
                }
            }

            _closeFrame(frame, bImmediately);
        }

        public void CloseFrame(Type frameType, bool bImmediately = false)
        {
            var frame = _getFrameByType(frameType);
            if (frame != null)
            {
                _closeFrame(frame, bImmediately);
            }
        }


        public void CloseFrame(string FrameName)
        {
            IClientFrame frame = _getFrameByName(FrameName);
            _closeFrame(frame);
        }
        #endregion

        #region Query
        public IClientFrame GetFrame(Type type)
        {
            return _getFrameByType(type);
        }

		public IClientFrame GetFrame(string name)
		{
			return _getFrameByName(name);
		}

        public bool HasActiveFrame(string type)
        {
            for(int i = 0; i < _activeFrames.Count; ++i)
            {
                if(null != _activeFrames[i] && _activeFrames[i].GetType().Name == type)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFrameOpen(Type type)
        {
            var frame = _getFrameByType(type);
            
            if (frame == null)
            {
                //Logger.LogError("frame is nil");
                return false;
            }

            return frame.IsOpen();
        }

        public bool IsFrameOpen<T>(T frame = null) where T : class, IClientFrame
        {
            Type t = typeof(T);
            if (frame == null)
            {
                IClientFrame iframe = frame as IClientFrame;
                if (false == _frameDics.TryGetValue(t.Name, out iframe))
                {
                    return false;
                }
                frame = iframe as T;
            }

            return frame.IsOpen();
        }

        public bool IsFrameOpen(string FrameName)
        {
            IClientFrame iframe;

            if (false == _frameDics.TryGetValue(FrameName, out iframe))
            {
                return false;
            }

            return iframe.IsOpen();
        }

        public bool IsFrameHidden(Type type)
        {
            var frame = _getFrameByType(type);
            if (frame == null)
            {
                return true;
            }

            return frame.IsHidden();
        }
        #endregion

        private void _closeAllActiveFrame(bool closeGlobal,bool removeRef)
        {
            Logger.LogProcessFormat("[frame] 关闭打开的 {0} 个界面", _activeFrames.Count);

            List<IClientFrame> closePools = GamePool.ListPool<IClientFrame>.Get();

            for (int i = 0; i < _activeFrames.Count; ++i)
            {
                ClientFrame frame = _activeFrames[i] as ClientFrame;
                if (closeGlobal || null == frame || !frame.IsGlobal())
                {
                    closePools.Add(frame);
                }
            }

            for (int i = 0; i < closePools.Count; ++i)
            {
                IClientFrame frame = closePools[i];
                if (null != frame)
                {
                    frame.Close(true);
                }
            }

            GamePool.ListPool<IClientFrame>.Release(closePools);

            _clearMutextFrameHiddenTag();
            _clearFullScreenStatus();
        }

        private void _clearAllGameObjects(bool closeGlobal)
        {
            for (int i = 0; i < _layer2DRoots.Length; ++i)
            {
                GameObject root = _layer2DRoots[i];
                int j = 0;
                while (root.transform.childCount > j)
                {
                    GameObject obj = root.transform.GetChild(j).gameObject;
                    if (closeGlobal || !obj.CompareTag(kGlobalTag))
                    {
                        GameObject.DestroyImmediate(obj);
                    }
                    else
                    {
                        j++;
                    }
                }
            }
        }

        public void CloseAllFrames( bool closeGlobal = false,bool removeRef = false)
        {
            _closeAllActiveFrame(closeGlobal, removeRef);
            _clearAllGameObjects(closeGlobal);
        }

        public void UpdateAllFrames(float timeElapsed)
        {
            for (int i = 0; i < _activeFrames.Count; ++i)
            {
                var frame = _activeFrames[i];
                if (frame != null && frame.IsNeedUpdate())
                {
                    frame.Update(timeElapsed);
                }
            }
        }

        public void CloseFrames()
        {
            var data = _frameDics.GetEnumerator();

            var framelist = GamePool.ListPool<IClientFrame>.Get();
            while(data.MoveNext())
            {
                IClientFrame frame = data.Current.Value;

                if (frame.IsNeedClearWhenChangeScene())
                {
                    framelist.Add(frame);
                    //_closeFrame(frame);
                }
            }

            for(int i = 0; i < framelist.Count; ++i)
            {
                var current = framelist[i];
                _closeFrame(current);
            }

            GamePool.ListPool<IClientFrame>.Release(framelist);
        }

        public void ShowFrame(Type targetFrame, Type currentFrame, bool isShow)
        {
            var frame = _getFrameByType(targetFrame);
            if (frame == null)
            {
                return;
            }

            if (!_checkIsValidType(currentFrame))
            {
                return;
            }

            Logger.LogProcessFormat("[ClientFrame] {0} 让 {1} 变成了 {2} ", currentFrame, targetFrame, isShow);
            frame.Show(isShow, currentFrame);
        }

        public void ShowAllFrame(Type currentFrame, bool isShow)
        {
            if (!_checkIsValidType(currentFrame))
            {
                return;
            }

            var iter = _frameDics.GetEnumerator();
            while (iter.MoveNext())
            {
                var item = iter.Current;
                var type = item.Value.GetType();

                if (currentFrame != type)
                {
                    ShowFrame(type, currentFrame, isShow);
                }
            }
        }

        public DictionaryView<string, IClientFrame> GetAllFrames()
        {
            return _frameDics;
        }
        #endregion

        #region FrameStack
        
        private List<IClientFrameStackCmd> mClientFrameStack = new List<IClientFrameStackCmd>();

        public void Push2FrameStack(IClientFrameStackCmd cmd)
        {
            mClientFrameStack.Add(cmd);
        }

        public void ClearFrameStack()
        {
            mClientFrameStack.Clear();
        }

        private IEnumerator _popAllFrameInStack()
        {
            for (int i = mClientFrameStack.Count - 1; i >= 0; i--)
            {
                IClientFrameStackCmd cmd = mClientFrameStack[i];

                if (cmd.Do())
                {
                    yield return Yielders.EndOfFrame;
                }
                else
                {
                    break;
                }
            }

            ClearFrameStack();
        }
        #endregion

        public sealed override void Init()
        {
            _InitializeAudio();
            _InitializeUI();

#if CUDLR_SERVER || UNITY_EDITOR
//#if UNITY_EDITOR
            //只在编辑器模式下使用
            //_InitializeCUDLRServer();
#endif

            _InitializeDebugReportRoot();

            _clearMutextFrameHiddenTag();

            BindNetEvents();
        }

        public void Update(float timeElapsed)
        {
            UpdateSwitchSystemLoadingProcess();

            if (CurrentSystem != null)
            {
                CurrentSystem.Update(timeElapsed);
            }
            if (TargetSystem != null)
            {
                TargetSystem.Update(timeElapsed);
            }
            WaitNetMessageManager.GetInstance().Update(timeElapsed);
            PlayerDataManager.GetInstance().Update(timeElapsed);

            UpdateAllFrames(timeElapsed);
            MissionManager.GetInstance().Update();
            ChatManager.GetInstance().Update();
            SystemNotifyManager.GetInstance().OnUpdate(timeElapsed);
            AnnouncementManager.GetInstance().OnUpdate(timeElapsed);
            TeamDataManager.GetInstance().OnUpdate(timeElapsed);
            RedPointDataManager.GetInstance().Update(timeElapsed);
			ActivityDungeonDataManager.GetInstance().Update(timeElapsed);
            PetDataManager.GetInstance().OnUpdate(timeElapsed);
            AdsPush.LoginPushManager.GetInstance().OnUpdate(timeElapsed);
            ActivityLimitTimeCombineManager.GetInstance().OnUpdate(timeElapsed);
#if ROBOT_TEST
            AutoFightRunTime.GetInstance().Update(timeElapsed);
#endif

            if(delayCaller != null)
                delayCaller.Update((int)(timeElapsed * GlobalLogic.VALUE_1000));

            if (null != mManager)
            {
                mManager.UpdateEnumerators();
            }

            //AsyncLoadTaskManager.instance.Update(timeElapsed);
            _updateFullScreen(timeElapsed);
        }

        private void _onChangeClear()
        {
            AudioFxManager.instance.Stop();
            ScriptPool.ClearAll();

        }

        public void TryCloseAllFrames()
        {
			/// 添加
			CloseAllFrames(false, true);

			List<string> removeLst = new List<string>();
			DictionaryView<string, IClientFrame>.Enumerator it = _frameDics.GetEnumerator();
			while(it.MoveNext())
			{
				var cur = it.Current;
				if (!cur.Value.IsOpen())
					removeLst.Add(cur.Key);
			}

			removeLst.RemoveAll(x => {
				_frameDics.Remove(x);
				return true;
			});
		}

        protected SwitchSystemFinishedEvent m_switchFinished = new SwitchSystemFinishedEvent();
        public SwitchSystemFinishedEvent OnSwitchSystemFinished
        {
            get
            {
                return m_switchFinished;
            }
        }

        protected SwitchSystemBeginEvent m_switchBegin = new SwitchSystemBeginEvent();
        public SwitchSystemBeginEvent OnSwitchSystemBegin
        {
            get
            {
                return m_switchBegin;
            }
        }

        public class SwitchSystemFinishedEvent : UnityEvent { }

        public class SwitchSystemBeginEvent : UnityEvent { }

        Type m_ePreSystemType = null;
        public Type PreSystemType
        {
            get
            {
                return m_ePreSystemType;
            }
        }

        private List<IEnumerator> m_switchSystemLoadingExitCoroutines   = new List<IEnumerator>();
        private List<IEnumerator> m_switchSystemLoadingEnterCoroutines  = new List<IEnumerator>();
        private bool              m_switchSystemLoadingCoroutinesLocked;
        private bool              m_bSwitchSystem;
        private SystemAsyncOperation switchSystemLoadingOP = new SystemAsyncOperation();

        public bool isSwitchSystemLoading
        {
            get 
            {
                return m_switchSystemLoadingCoroutinesLocked;
            }
        }

        void  BeginSwitchSystemLoading()
        {
            switchSystemLoadingOP.ReInit();
            SwitchProgress = 0.0f;
            m_bSwitchSystem = true;
            InputManager.isForceLock = true;
        }

        public string GetSwitchSystemInfo()
        {
            return switchSystemLoadingOP.GetProgressInfo();
        }

        void  EndSwitchSystemLoading()
        {
            SwitchProgress = 1.0f;
            m_bSwitchSystem = false;
            InputManager.isForceLock = false;
        }

        void UpdateSwitchSystemLoadingProcess()
        {
            if(m_bSwitchSystem && m_switchSystemLoadingCoroutinesLocked)
            {
#if UNITY_EDITOR
                SwitchProgress = switchSystemLoadingOP.Progress * 0.9f;
#else
				SwitchProgress = switchSystemLoadingOP.Progress;
#endif
            }
        }

        private bool AddExitCoroutine(loadingCoroutine coroutine,string name = "",float weight = 1.0f)
        {
            //防止在加载协程中使用
            if(m_switchSystemLoadingCoroutinesLocked)
            {
                return false;
            }

            if(coroutine == null)
            {
                return false;
            }

            switchSystemLoadingOP.AddTask(name,weight);
            //保证有序插入，确保有序执行
            m_switchSystemLoadingExitCoroutines.Add(coroutine(switchSystemLoadingOP));
            return true;
        }

        private bool AddEnterCoroutine(loadingCoroutine coroutine,string name = "",float weight = 1.0f)
        {
            //防止在加载协程中使用
            if(m_switchSystemLoadingCoroutinesLocked)
            {
                return false;
            }

            if(coroutine == null)
            {
                return false;
            }

            switchSystemLoadingOP.AddTask(name,weight);
            //保证有序插入，确保有序执行
            m_switchSystemLoadingEnterCoroutines.Add(coroutine(switchSystemLoadingOP));
            return true;
        }

        //public void 

        public void InitSystem<T>(params object[] userData) where T : class, IClientSystem
        {
            if(CurrentSystem != null)
            {
                Logger.LogError("初始化系统初始系统必须为空");
            }
            
            Type t = typeof(T);
            IClientSystem targetClientSystem = null;
            _clientSystems.TryGetValue(t.Name, out targetClientSystem);
            CurrentSystem = targetClientSystem;

            if (CurrentSystem == null)
            {
                CurrentSystem = Activator.CreateInstance<T>() as IClientSystem;
                ClientSystem system = CurrentSystem as ClientSystem;
                system.SystemManager = this;
                system.SetName(t.Name);
                _clientSystems.Add(t.Name, CurrentSystem);
            }

            (CurrentSystem as ClientSystem).OnEnterSystem();
        }

        // userData 为扩展参数，可能会需要一些额外的参数，来设置系统的初始状态
        // 用到再加
        public void SwitchSystem<T>(SystemContent systemContent = null, object userData = null, bool isAllowWwitchSameSystem = false/* , int targetSceneid = -1 仅对城镇有效*/) where T : class, IClientSystem
        {
            //不支持系统切换
			if (!isAllowWwitchSameSystem && null != CurrentSystem && CurrentSystem.GetType() == typeof(T)) 
			{
				//Logger.LogErrorFormat("[系统切换] 无法支持从 {0} -> {0}", typeof(T).Name);
				return;
			}

            //正在切换系统
            if (null != TargetSystem)
            {
                //Logger.LogErrorFormat("[系统切换] 上一个系统正在切换 {0}", null != TargetSystem ? TargetSystem.GetType().Name : "[invalid]");
                return;
            }

            Type t = typeof(T);
            IClientSystem targetClientSystem = null;
            _clientSystems.TryGetValue(t.Name, out targetClientSystem);
           
            TargetSystem = targetClientSystem;

            if (TargetSystem == null)
            {
                TargetSystem = Activator.CreateInstance<T>() as IClientSystem;
                ClientSystem system = TargetSystem as ClientSystem;
                system.SystemManager = this;
                system.SetName(t.Name);
                _clientSystems.Add(t.Name, TargetSystem);
            }

            _onChangeClear();

            Logger.LogFormat("current system: {0}", CurrentSystem == null ? "null" : CurrentSystem.GetName());
            Logger.LogFormat("target system: {0}", TargetSystem == null ? "null" : TargetSystem.GetName());

            //直接进战斗的测试模式
            if (Global.Settings.startSystem == EClientSystem.Battle)
            {
                GameClient.PlayerBaseData.GetInstance().Level = (ushort)Global.Settings.TestLevel;
            }

            m_ePreSystemType = CurrentSystem == null ? null : CurrentSystem.GetType();

            
            if (isAllowWwitchSameSystem || CurrentSystem != TargetSystem)
            {
                if (null != TargetSystem)
                {
                    TargetSystem.BeforeEnter();
                }

                /*
                   ThreeStepProcess _3step =  new ThreeStepProcess(
                   "SwitchSystemAsync",
                   mManager, 
                   _switchSystemProcess(),
                   _switchSystemCommonStart(),
                   _switchSystemCommonEnd());

                   _3step.SetErrorProcessHandle(_switchSystemErrorHandle);

                   yield return _3step;
                   */
                GameFrameWork.instance.StartCoroutine(_SwitchSystemCoroutine(systemContent));
            }
        }

        public void QuitToLoginSystem(int id)
        {
            if (m_beQuitToLogin == false)
            {
                m_beQuitToLogin = true;

                SystemNotifyManager.SystemNotifyOkCancel(id, _QuitToLoginImpl,()=>
                {
                    m_beQuitToLogin = false;
                }, FrameLayer.TopMost);
            }
        }

        bool m_beQuitToLogin = false;
        public void QuitToLogin(string message)
        {
            //if (m_beQuitToLogin == false)
            //{
            //    //m_beQuitToLogin = true;

            //    //SystemNotifyManager.SysNotifyMsgBoxOK(message, _QuitToLoginImpl);
            //}
        }

        private void _forceQuitTargetSystem()
        {
            if (null != TargetSystem)
            {
                ClientSystem sys = TargetSystem as ClientSystem;
                if (null != sys)
                {
                    sys.OnExitSystem();
                }

                CurrentSystem = TargetSystem;
                TargetSystem = null;
                m_switchSystemLoadingCoroutinesLocked = false;
            }
        }

        // 清空所有数据，重新登录
        public void _QuitToLoginImpl()
        {
            m_beQuitToLogin = false;
            delayCaller.Clear();
            ClientApplication.playerinfo.state = PlayerState.LOGOUT;

            // 当前是Version的话就不切
            if (CurrentSystem is ClientSystemVersion)
            {
                SystemSwitchEventManager.GetInstance().TriggerEvent(SystemEventType.SYSTEM_EVENT_ON_SWITCH_FAILED);
                return;
            }

            // dd; 这里的代码临时处理成这样 - -
            if (CurrentSystem is ClientSystemLogin)
            {
                ClientSystemManager.instance.CloseFrame<CreateRoleFrame>();
                ClientSystemManager.instance.CloseFrame<SelectRoleFrame>();
                ClientSystemManager.instance.CloseFrame<ServerWaitQueueUp>();
                ClientSystemManager.instance.CloseFrame<ServerListFrame>();

                ClientReconnectManager.instance.Clear();

                NetManager.Instance().Disconnect(ServerType.GATE_SERVER);
                NetManager.Instance().Disconnect(ServerType.RELAY_SERVER);
                SystemSwitchEventManager.GetInstance().TriggerEvent(SystemEventType.SYSTEM_EVENT_ON_SWITCH_FAILED);
                return;
            }

            //GameStatisticManager.GetInstance().DoStatistic("[switch system] quit to login");


            _forceQuitTargetSystem();

            ClientReconnectManager.instance.Clear();

            // 确保所有数据都被清空了
            // 如果存在不受ClientSystem控制的对象，请确保对象状态自制
            ComTalk.ForceDestroy();
            VoiceSDK.SDKVoiceManager.GetInstance().LeaveVoiceSDK(true);
            UIEventSystem.GetInstance().Clear();
            GlobalEventSystem.GetInstance().Clear();
            InvokeMethod.Exit();
            Logger.LogErrorFormat("===================[开始停止所有协程]=======================");

            GameFrameWork.instance.StopAllCoroutines();

            if (null != mManager)
            {
                mManager.ClearAllEnumerators();
            }

            Logger.LogErrorFormat("===================[结束停止所有协程]=======================");

            // 清空所有数据
            PlayerDataManager.GetInstance().ClearAll();
            SystemNotifyManager.Clear();
            ChapterChange.UnInit();
#if !LOGIC_SERVER
            BeActionFrameMgr.Clear(true);
            SkillFileListCache.Clear(true);
#endif
            
            ClearFrameStack();

            if (CurrentSystem is ClientSystemLogin)
            {
                _closeAllActiveFrame(true,true);

                // 断网
                NetManager.Instance().Disconnect(ServerType.GATE_SERVER);
                NetManager.Instance().Disconnect(ServerType.RELAY_SERVER);

                // 这里不清空网络的注册消息
            }
            else 
            {
                // 关闭所有界面
                CloseAllFrames(true,true);

                // 断网
                NetManager.Instance().Disconnect(ServerType.GATE_SERVER);
                NetManager.Instance().Disconnect(ServerType.RELAY_SERVER);

                // 清空消息缓存和注册的处理函数
                NetProcess.Instance().Clear();
            }
           
            _frameDics.Clear();
            //_activeFrames.Clear();

            PluginManager.GetInstance().BuglySceneInfo = string.Empty;

            //SwitchSystem<ClientSystemLogin>();
            SwitchSystem<ClientSystemVersion>();
        }

        public void _QuitToSelectRoleImpl()
        {
            delayCaller.Clear();
            _forceQuitTargetSystem();

            ClientReconnectManager.instance.Clear();
           
            // 清掉一些特殊的obj
            ComTalk.ForceDestroy();

            // 清空与角色有关的数据
            PlayerDataManager.GetInstance().ClearAll();
            NewbieGuideManager.GetInstance().Reset();
            AnnouncementManager.GetInstance().Clear(); 
            SystemNotifyManager.Clear();
            ChapterChange.UnInit();
#if !LOGIC_SERVER
            BeActionFrameMgr.Clear(true);
            SkillFileListCache.Clear(true);
#endif

            // 清空事件
            //UIEventSystem.GetInstance().Clear();
            //GlobalEventSystem.GetInstance().Clear();

            InvokeMethod.Exit();
            Logger.LogErrorFormat("===================[开始停止所有协程]=======================");

            GameFrameWork.instance.StopAllCoroutines();

            if (null != mManager)
            {
                mManager.ClearAllEnumerators();
            }

            Logger.LogErrorFormat("===================[结束停止所有协程]=======================");

            ClearFrameStack();

            // 关闭所有界面
            CloseAllFrames(true, true);

            // 清空消息缓存和注册的处理函数
            //NetProcess.Instance().Clear();

            _frameDics.Clear();

            PluginManager.GetInstance().BuglySceneInfo = string.Empty;

            // 最后切换系统    
            SwitchSystem<ClientSystemLogin>();
        }

        private IEnumeratorManager mManager = new EnumeratorProcessManager();
        public IEnumeratorManager enumeratorManager
        {
            get 
            {
                return mManager;
            }
        }

        //protected IEnumerator _switchSystemCommonStart()
        //{
        //    InputManager.isForceLock = true;
        //    SwitchProgress = 0.0f;
        //    yield break;
        //}

        //protected IEnumerator _switchSystemCommonEnd()
        //{
        //    InputManager.isForceLock = false;
        //    yield break;
        //}

        //protected IEnumerator _switchSystemErrorHandle(eEnumError errorType, string msg)
        //{
        //    CurrentSystem = null;
        //    QuitToLogin(TR.Value("net_connect_timeout"));
        //    yield break;
        //}

        public static IEnumerator _PreloadRes(IASyncOperation op)
        {

			HGProfiler.BeginProfiler("5---preload all res");
			//var startTime = Time.realtimeSinceStartup;

            int preloadPercentage = 0;
			while (preloadPercentage < 100)
			{
				preloadPercentage = CResPreloader.instance.DoPreLoadAsync(true);
                op.SetProgress(0.5f + preloadPercentage * 0.01f * 0.2f);
                yield return Yielders.EndOfFrame;
			}

			//var duration = Time.realtimeSinceStartup - startTime;
			//Logger.LogErrorFormat("preload hell duration:{0}", duration);
        }

        IEnumerator _SwitchSystemCoroutine(SystemContent systemContent)
        {
            ITMStopWatch stopWatch01 = TMBattleAssetLoadRecord.instance.CreateStopWatch("SwitchSystemCoroutine");
            //开始切换场景，正在loading，向服务器发送loading的情况
            _isLoading = true;
            SendSceneNotifyLoadingInfoBySwitchSystem();

            //打开Loading界面
            //临时测试代码
            bool bPKLoading = _TryOpenPkLoadingFrame(CurrentSystem, TargetSystem);
            if(bPKLoading)    yield return _ShowPKLoadingFrame();
            BeginSwitchSystemLoading();
            m_switchBegin.Invoke();
            IClientFrame loadingFrame = _OpenGlobalLoadingFrame(CurrentSystem, TargetSystem);

            yield return new WaitClientFrameOpen(loadingFrame);

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine 打开Loading界面", stopWatch01);

            if (null != loadingFrame)
            {
                yield return loadingFrame.LoadingOpenPost();
            }

#if UNITY_EDITOR
            Logger.LogProcessFormat("[当前系统状态 ] 1 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif

            //收集协程
            m_switchSystemLoadingExitCoroutines.Clear();
            m_switchSystemLoadingEnterCoroutines.Clear();
            if(CurrentSystem != null) CurrentSystem.GetExitCoroutine(AddExitCoroutine);
            if (TargetSystem != null) TargetSystem.GetEnterCoroutine(AddEnterCoroutine);
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine收集协程完成", stopWatch01);
            //AddEnterCoroutine(_PreloadRes);

            m_switchSystemLoadingCoroutinesLocked = true;
            int exitCoroutinesCount = m_switchSystemLoadingExitCoroutines.Count;
            //退出协程
            for(int i = 0; i < m_switchSystemLoadingExitCoroutines.Count; ++i)
            {
#if UNITY_EDITOR
                Logger.LogProcessFormat("[当前系统状态 ] 2 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif
                switchSystemLoadingOP.BeginTask(i);
                yield return m_switchSystemLoadingExitCoroutines[i];

#if UNITY_EDITOR
                Logger.LogProcessFormat("[当前系统状态 ] 3 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif
                switchSystemLoadingOP.FinishTask(i);
            }
            m_switchSystemLoadingExitCoroutines.Clear();

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine 退出协程完成", stopWatch01);

            //退出系统
            if (CurrentSystem != null) (CurrentSystem as ClientSystem).OnExitSystem();

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine 退出上一个系统完成", stopWatch01);

#if UNITY_EDITOR
            Logger.LogProcessFormat("[当前系统状态 ] 4 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif
            //尝试关闭所有界面
            TryCloseAllFrames();

            ComTalk.ForceDestroy();
#if TEST_SILLFILE_LOAD
            CResPreloader.instance.Clear(false);
#else            
            CResPreloader.instance.Clear(true);
#endif
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine 关闭界面和聊天", stopWatch01);
            //进入协程
            for (int i = 0; i < m_switchSystemLoadingEnterCoroutines.Count; ++i)
            {
#if UNITY_EDITOR
                Logger.LogProcessFormat("[当前系统状态 ] 5 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif
                switchSystemLoadingOP.BeginTask(i+exitCoroutinesCount);
                yield return m_switchSystemLoadingEnterCoroutines[i];
#if UNITY_EDITOR
                Logger.LogProcessFormat("[当前系统状态 ] 6 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif
                switchSystemLoadingOP.FinishTask(i+exitCoroutinesCount);
            }
            m_switchSystemLoadingEnterCoroutines.Clear();

            /// 资源预加载

            //CResPreloader.instance.Clear(true);

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine 战斗加载完成", stopWatch01);

#if UNITY_EDITOR
            yield return new WaitForSeconds(LeanTween.instance.loadWaitTime);
#endif

            //正式切换系统
            CurrentSystem = TargetSystem;
            TargetSystem = null;
            // 系统成功切换了，应该重置这个变量，
            m_beQuitToLogin = false;

#if UNITY_EDITOR
            Logger.LogProcessFormat("[当前系统状态 ] 7 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif

            m_switchSystemLoadingCoroutinesLocked = false;
          
            //临时代码
            //if(bPKLoading) yield return _EnterPK(); 

            //关闭Loading界面
            SwitchProgress = 1.0f;
            
            
            // 进入系统
            if(CurrentSystem != null) (CurrentSystem as ClientSystem).OnEnterSystem();

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine 进入系统完成", stopWatch01);

            try
            {
                // 切换完成的回调
                m_switchFinished.Invoke();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
            EndSwitchSystemLoading();
            
            yield return new WaitClientFrameClose(loadingFrame);

            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine Loading界面关闭", stopWatch01);

            //??
            GameFrameWork.instance.StartCoroutine(_popAllFrameInStack());

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SystemChanged);
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.SystemChanged);

            //正式启动游戏
            if (CurrentSystem != null) (CurrentSystem as ClientSystem).OnStartSystem(systemContent);

#if UNITY_EDITOR
            Logger.LogProcessFormat("[当前系统状态 ] 8 {0}, {1}", (CurrentSystem as ClientSystem).state, (CurrentSystem as ClientSystem).GetName());
#endif
            TMBattleAssetLoadRecord.instance.SaveAndRefreshTag("SwitchSystemCoroutine 系统切换完成", stopWatch01);
            //系统切换完成，loading结束，向服务器发送Loading结束的消息
            //延迟一下
            yield return Yielders.GetWaitForSeconds(0.5f);
            _isLoading = false;
            SendSceneNotifyLoadingInfoBySwitchSystem();
            SyncMainPlayerBaseDataBySwitchSystemFinished();

            if(CurrentSystem != null && CurrentSystem is ClientSystemTown)
            {
#if ROBOT_TEST
                AutoFightRunTime.GetInstance().OnReturnToTown();
#endif
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PK3V3CrossButton, (byte)Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus());
            }

            // 测试代码注释
            //             if(GlobalNetMessage.ReceiveEnterDungeonMsgWhenQuitDungeon)
            //             {
            //                 GlobalNetMessage.ReceiveEnterDungeonMsgWhenQuitDungeon = false;
            // 
            //                 SceneDungeonStartReq req = new SceneDungeonStartReq();
            //                 req.dungeonId = GlobalNetMessage.NeedEnterDungeonIdWhenQuitDungeon;
            // 
            //                 NetManager netMgr = NetManager.Instance();
            //                 netMgr.SendCommand(ServerType.GATE_SERVER, req);
            // 
            //                 GlobalNetMessage.NeedEnterDungeonIdWhenQuitDungeon = 0;
            //             }
            TMBattleAssetLoadRecord.instance.CloseAndSaveWatch(stopWatch01);
        }

        bool _TryOpenPkLoadingFrame(IClientSystem current, IClientSystem target)
        {
            if (target == null)
            {
                return false;
            }

            Type targetType = target.GetType();
            Type loginType = typeof(ClientSystemLogin);
            Type townType = typeof(ClientSystemTown);
            Type battleType = typeof(ClientSystemBattle);
            Type gameBattleType = typeof(ClientSystemGameBattle);
            if (current == null)
            {
                return false;
            }
            else
            {
                Type currType = current.GetType();
                if ((currType == townType || currType == gameBattleType) && targetType == battleType)
                {
                    //if (BattleSceneMain.GetInstance().GetSceneType() == EBattleSceneType.TeamDungeon || BattleMain.instance == null)
                    if (BattleMain.instance == null)
                    {
                        return false;
                    }
                    switch (BattleMain.battleType)
                    {
                        case BattleType.MutiPlayer:
                        case BattleType.GuildPVP:
                        case BattleType.MoneyRewardsPVP:
                        case BattleType.ChijiPVP:
                            return true;
                     
                    }
                }
            }

            return false;  
        }

        IEnumerator  _ShowPKLoadingFrame()
        {
            PkLoadingFrame pkloading = OpenGlobalFrame<PkLoadingFrame>(FrameLayer.Top) as PkLoadingFrame;
            yield return pkloading.LoadStartLoading();
        }

        IEnumerator _EnterPK()
        {
            PkLoadingFrame pkloading = _getFrameByType(typeof(PkLoadingFrame)) as PkLoadingFrame;
            if(pkloading != null)
            {
                yield return Yielders.GetWaitForSeconds(2.0f);
                //pkloading.LoginRaceServer();
            }
            yield break;
        }

        IEnumerator _EnterPK2()
        {
            yield return Yielders.GetWaitForSeconds(2.0f);
            yield return Yielders.EndOfFrame;
            yield return Yielders.GetWaitForSeconds(10000.0f);
        }

        IClientFrame _OpenGlobalLoadingFrame(IClientSystem current, IClientSystem target)
        {
            Logger.LogProcessFormat("[switchsystem] 打开LoadingFrame");

            if (target == null)
            {
                return null;
            }

            Type targetType = target.GetType();
            Type loginType = typeof(ClientSystemLogin);
            Type townType = typeof(ClientSystemTown);
            Type battleType = typeof(ClientSystemBattle);
            Type gameBattleType = typeof(ClientSystemGameBattle);
            if (current == null)
            {
                if (targetType == battleType)
                {
                    return OpenGlobalFrame<DungeonLoadingFrame>(FrameLayer.Top);
                }

                return OpenGlobalFrame<SplashLoadingFrame>(FrameLayer.Top);
            }
            else
            {
                Type currType = current.GetType();
                if (currType == loginType && targetType == townType)
                {
                    return OpenGlobalFrame<LoadingFrame>(FrameLayer.Top);
                }
                else if (currType == loginType && targetType == battleType)
                {
                    return OpenGlobalFrame<DungeonLoadingFrame>(FrameLayer.Top,PlayerBaseData.GetInstance().JobTableID);
                }
                else if (((currType == townType && targetType == battleType) ||
                           (currType == townType && targetType == gameBattleType)) 
                           || (currType==battleType && targetType== battleType) ||
                           (currType == gameBattleType && targetType == battleType))
                {
                    //if (BattleSceneMain.GetInstance().GetSceneType() == EBattleSceneType.TeamDungeon || BattleMain.instance == null)
                    if (BattleMain.instance == null)
                    {
                        return OpenGlobalFrame<DungeonLoadingFrame>(FrameLayer.Top);
                    }
                    switch (BattleMain.battleType)
                    {
                        case BattleType.MutiPlayer:
                        case BattleType.ChijiPVP:
                            return GetFrame(typeof(PkLoadingFrame));
                        //return OpenGlobalFrame<PkLoadingFrame>(FrameLayer.Top);
                        case BattleType.GuildPVP:
                        case BattleType.MoneyRewardsPVP:
                            return GetFrame(typeof(PkLoadingFrame));
                        case BattleType.Single:
                            return OpenGlobalFrame<LoadingFrame>(FrameLayer.Top);
                        case BattleType.NewbieGuide:
                            return OpenGlobalFrame<DungeonLoadingFrame>(FrameLayer.Top);
                        case BattleType.Dungeon:
                        case BattleType.FinalTestBattle:
                        case BattleType.DeadTown:
                        case BattleType.Mou:
                        case BattleType.North:
                        case BattleType.ChampionMatch:
                            return OpenGlobalFrame<DungeonLoadingFrame>(FrameLayer.Top);
                        case BattleType.PVP3V3Battle:
                        case BattleType.ScufflePVP:
                            return OpenGlobalFrame<Dungeon3v3LoadingFrame>(FrameLayer.Top);
                        case BattleType.GuildPVE:
                            return OpenGlobalFrame<DungeonLoadingFrame>(FrameLayer.Top);
                        default:
                            return OpenGlobalFrame<DungeonLoadingFrame>(FrameLayer.Top);
                    }
                }
                else if (currType == townType && targetType == loginType)
                {
                    return OpenGlobalFrame<SplashLoadingFrame>(FrameLayer.Top);
                }
                else if ((currType == battleType && targetType == townType) || 
                        (currType == gameBattleType && targetType == townType) || //吃鸡场景到主城
                        (currType == battleType && targetType == gameBattleType)) //吃鸡pk场景到吃鸡场景
                {
                    if(BattleMain.instance != null && BattleMain.battleType == BattleType.NewbieGuide)
                    {
                       return OpenGlobalFrame<LoadingFrame>(FrameLayer.Top);     
                    }
                    else 
                    return OpenGlobalFrame<LoadingFrame>(FrameLayer.Top,targetType);
                }
            }

            return OpenGlobalFrame<SplashLoadingFrame>(FrameLayer.Top);
        }

        [Conditional("WORD_DEBUG"), Conditional("LOG_DIALOG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNING"), Conditional("LOG_NORMAL")]
        private void _InitializeCUDLRServer()
        {
            var server = Utility.FindGameObject("CUDLRServer", false);
            if (server == null)
            {
                server = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/CUDLR/CUDLRServer");
                server.name = "CUDLRServer";
                server.transform.SetAsLastSibling();
                server.SetActive(true);
                GameObject.DontDestroyOnLoad(server);
            }
        }

        private const string kDebugReportRootName = "__DebugReportRoot";

        [Conditional("DEBUG_REPORT_ROOT")]
        private void _InitializeDebugReportRoot()
        {
            GameObject debutReportRoot = Utility.FindGameObject(kDebugReportRootName, false);

            if (debutReportRoot == null)
            {
                debutReportRoot = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/DebugReport/DebugReport");
                debutReportRoot.name = kDebugReportRootName;
                debutReportRoot.transform.SetAsLastSibling();
                GameObject.DontDestroyOnLoad(debutReportRoot);
            }
        }

#region UI Effect
        public GameObject PlayUIEffect(FrameLayer layer, string prefab, float time = 0.0f)
        {
            GameObject obj = AssetLoader.instance.LoadResAsGameObject(prefab);
            if (obj != null)
            {
                obj.transform.SetParent(LayerRoots[(int)layer].transform, false);
            }

            if (obj != null && time != 0.0f)
            {
                DestroyDelay delay = obj.GetComponent<DestroyDelay>();
                if (null == delay)
                {
                    delay = obj.AddComponent<DestroyDelay>();
                }

                delay.Delay = time;
            }

            return obj;
        }
#endregion

#region ClientSystemNetEvents
        //场景是否正在loading
        private bool _isLoading = false;
        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneQueryLoadingInfo.MsgID, NetSceneQueryLoadingInfo);
        }

        public sealed override void UnInit()
        {
            base.UnInit();
            UnBindNetEvents();
        }

        private void UnBindNetEvents()
        {
            NetProcess.RemoveMsgHandler(SceneQueryLoadingInfo.MsgID, NetSceneQueryLoadingInfo);
        }
        /// <summary>
        /// 服务器向客户端请求loading的情况
        /// </summary>
        /// <param name="msgData"></param>
        private void NetSceneQueryLoadingInfo(MsgDATA msgData)
        {
            if (msgData == null)
            {
                Logger.LogErrorFormat("NetSceneQueryLoadingInfo ==> msgData is null");
                return;
            }

            //SendSceneNotifyLoadingInfo();
            if (_isLoading == true)
            {
                SendSceneNotifyLoadingInfo();
            }
            else
            {

                var systemTown = CurrentSystem as ClientSystemTown;
                if (systemTown == null)
                {
                    SendSceneNotifyLoadingInfo();
                    return;
                }

                if (systemTown.GetTownSceneSwitchState() == true)
                {
                    SendSceneNotifyLoadingInfoByTownSwitchScene(true);
                    return;
                }

                SendSceneNotifyLoadingInfo();
                
            }
        }

        //为服务器同步当前系统切换的情况
        private void SendSceneNotifyLoadingInfoBySwitchSystem()
        {
            SendSceneNotifyLoadingInfo();
        }

        //主城中场景的切换：从内城切换到外城
        public void SendSceneNotifyLoadingInfoByTownSwitchScene(bool isTownSwitchScene)
        {
            var flag = 0;
            if (isTownSwitchScene)
            {
                flag = 1;
            }

            var req = new SceneNotifyLoadingInfo
            {
                isLoading = (byte) flag,
            };

            if (NetManager.Instance() != null)
            {
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
        }

        //系统之间的切换：从战斗场景切换到主城
        private void SendSceneNotifyLoadingInfo()
        {
            //目标系统不为null，说明还没有切换完成
            var flag = 0;      //默认是0,没有loading,1正在loading
            if (_isLoading == true)
            {
                flag = 1;
            }

            var req = new SceneNotifyLoadingInfo
            {
                isLoading = (byte) flag,
            };

            if (NetManager.Instance() != null)
            {
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
        }

        //场景切换完成的时候
        //应服务器要求，在切换到主城系统的时候（从其他系统：如登录，战斗），同步主角的附魔值
        //发送场景切换完成消息
        private void SyncMainPlayerBaseDataBySwitchSystemFinished()
        {
            if(CurrentSystem == null)
                return;

            var systemTown = CurrentSystem as ClientSystemTown;
            if(systemTown == null)
                return;

            //从其他系统进入到主城系统，发送消息
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SceneChangedLoadingFinish);

            if (systemTown.MainPlayer == null)
                return;
            //魔法值同步
            systemTown.MainPlayer.SyncResistMagicValue();

        }

#endregion


    }
}
