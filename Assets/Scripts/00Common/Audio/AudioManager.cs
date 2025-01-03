using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using AudioHandle = System.UInt32;

public delegate void OnAudioEndCallback();

public enum AudioType
{
    AudioEffect ,
    AudioStream,
    AudioVoice,
    AudioGuide,
    AudioEnvironment,//环境音

    MaxTypeNum  ,
}

public interface IAudioInst
{
    void Pause();

    void Resume();

    void Stop();

    bool IsEnd();

	int GetLength ();
    void SetPitch(float pitch);

}


#if LOGIC_SERVER

public class AudioManager : Singleton<AudioManager>
{
	public void Clear(){}
	public void PreloadSound(string audioRes){}
	public void PreloadSound(int soundID){}
	public void ClearPreloadSound(){}
	public AudioHandle PlaySound(int soundID){return uint.MaxValue;}
	public AudioHandle PlaySound(ProtoTable.SoundTable data){return uint.MaxValue;}
	public AudioHandle PlaySound(AudioClip audioClip, AudioType type, float volume = 1.0f, bool isLoop = false, GameObject attachObj = null, bool isExculsive = false, bool AsyncLoad = false){return uint.MaxValue;}
	public AudioHandle PlaySound(string audioRes,AudioType type,float volume = 1.0f,bool isLoop = false,GameObject attachObj = null,bool isExculsive = false, bool AsyncLoad = false){return uint.MaxValue;}

	public bool IsMute(AudioType type){return false;}
	public void SetMute(AudioType type, bool isMute){}
	public void SetVolume(AudioType type,float volume){}
	public float GetVolume(AudioType type){ return 0.0f; }
	public void Stop(AudioHandle handle){}
	public void StopAll(AudioType type){}
	public int GetAudioLength(AudioHandle handle){return 0;}
    public void SetPitch(float pitch){}
    public void ClearOnEditor() { }
}

#else
public class AudioManager : MonoSingleton<AudioManager>
{
	protected static int MAX_AUDIOSOURCE_NUM = 8;
    protected class AudioInst : IAudioInst
    {
        public AudioInst()
        {
            m_AudioClip = null;
            m_AudioSource = null;
            m_AttachParent = null;
            m_AudioSrcObj = null;

            m_IsLoop = false;
            m_IsMute = false;
            m_AudioName = null;
            m_OrginVolume = 1;
            m_Volume = 1;
            m_Handle = uint.MaxValue;
            m_PlayFrame = false;

            m_Callback = null;
        }

        public bool Init(GameObject audioRoot)
        {
            m_AudioSrcObj = new GameObject("AudioSource");
            if (null != m_AudioSrcObj)
            {
                if (audioRoot != null)
                    Utility.AttachTo(m_AudioSrcObj, audioRoot);

                m_AudioSource = m_AudioSrcObj.AddComponent<AudioSource>();
                if (null != m_AudioSource)
                {
                    return true;
                }
                else
                    Logger.LogError("Add audio source has failed!");
            }
            else
                Logger.LogError("Create audio source game object has failed!");

            return false;
        }

        public void Deinit()
        {
            Stop();

            if (null != m_AudioSrcObj)
            {
                if(null != m_AudioSource)
                    m_AudioSource = null;
                
                GameObject.Destroy(m_AudioSrcObj);

                m_AudioSrcObj = null;
            }

            m_AttachParent = null;
        }
        
        public void DeinitOnEditor()
        {
            Stop();

            if (null != m_AudioSrcObj)
            {
                if(null != m_AudioSource)
                    m_AudioSource = null;
                
                GameObject.DestroyImmediate(m_AudioSrcObj);

                m_AudioSrcObj = null;
            }

            m_AttachParent = null;
        }

        public void Play(string clipName,AudioHandle handle, AudioClip audioClip,bool isLoop,GameObject attachObj,OnAudioEndCallback endCallback, bool fadeIn=false)
        {
            if (null == m_AudioSrcObj)
            {
                Logger.LogError("Invalid audio source instance!");
                return;
            }

            if (null != attachObj)
            {
                m_AttachParent = attachObj;
                m_AudioSrcObj.transform.SetParent(m_AttachParent.transform, false);
            }
            //else
            //    m_AudioSrcObj.transform.SetParent(instance.gameObject.transform, false);


            Debug.Assert(uint.MaxValue != handle);
            m_Handle = handle;
            m_AudioClip = audioClip;

            m_AudioName = clipName;
            m_IsLoop = isLoop;

            m_AudioSource.loop = m_IsLoop;
            m_AudioSource.clip = audioClip;
            m_AudioSource.Play();



            m_Callback = endCallback;
            m_PlayFrame = true;

            m_PlayTime = 0;
            m_TimeLength = (int)(m_AudioClip.length * 1000);

            if (fadeIn)
            {
                this.m_FadeIn = fadeIn;
                float finalAudio = m_AudioSource.volume;
//                Logger.LogErrorFormat("clipName:{0}", clipName);
                DOTween.To( () => {return 0f;},  (value) => {m_AudioSource.volume = value; }, finalAudio, 3.0f);
            }
            
        }

        public void Pause()
        {
            m_AudioSource.Pause();
        }

        public void Resume()
        {
            m_AudioSource.UnPause();
        }

		public int GetLength ()
		{
            return m_TimeLength;
        }

        public void Update(int ElpasedTIme)
        {
            if (ElpasedTIme > 0)
                m_PlayTime += ElpasedTIme;
        }

        public void OnEnd()
        {
            if(m_PlayTime > m_TimeLength)
            {
                if (null != m_Callback)
                {
                    m_Callback();
                    m_Callback = null;
                }
            }
        }

        public void Stop()
        {
            if (null == m_AudioClip)
                return;

            if (m_FadeIn && m_AudioSource != null)
            {
                var tempVolume = m_AudioSource.volume;
                DOTween.Kill(m_AudioSource.gameObject); 
                DOTween.To( () => {return tempVolume;},  (value) => {m_AudioSource.volume = value; if (value<=0.01f) _Stop();}, 0, 2.0f);
            }
                
            else
                _Stop();
        }
            

            

        protected void _Stop()
        {
            if(null != m_AudioSource)
            {
                m_AudioSource.Stop();
                m_AudioSource.clip = null;
                m_AudioSource.loop = false;
            }

            if(null != m_AudioClip)
                m_AudioClip = null;

            //m_AudioSrcObj.transform.SetParent(instance.gameObject.transform, false);
            m_AttachParent = null;
            m_Handle = uint.MaxValue;
            m_AudioName = "";
            m_IsLoop = false;

            if(null != m_Callback)
            {
                m_Callback();
                m_Callback = null;
            }

            m_TimeLength = 0;
            m_PlayTime = 0;
        }

        public bool IsEnd()
        {
            if(m_PlayFrame)
            {
                m_PlayFrame = false;
                return false;
            }

            if (m_AudioSource == null)
                return true;

            return m_AudioSource.isPlaying == false && false == m_AudioSource.loop;
        }

        public bool isLoop
        {
            get { return m_IsLoop; }
        }

        public bool isMute
        {
            get { return m_IsMute; }
            set
            {
                m_IsMute = value;
                if (null != m_AudioSource)
                    m_AudioSource.mute = m_IsMute;
            }
        }

        public float volume
        {
            get { return m_Volume; }
            set
            {
                m_Volume = value;
                if (null != m_AudioSource)
                    m_AudioSource.volume = m_Volume * m_OrginVolume;
            }
        }
        public void SetPitch(float pitch)
        {
            this.pitch = pitch;
        }
        public float pitch
        {
            get { if (m_AudioSource != null) return m_AudioSource.pitch; return 0.0f; }
            set { if (m_AudioSource != null) m_AudioSource.pitch = value; }
        }

        public string audioClipName
        {
            get { return m_AudioName; }
        }

        public uint handle
        {
            get { return m_Handle; }
        }

        public float orginVolume
        {
            get { return m_OrginVolume; }
            set
            {
                m_OrginVolume = value;
                if (null != m_AudioSource)
                    m_AudioSource.volume = m_Volume * m_OrginVolume;
            }
        }
    
        protected AudioClip m_AudioClip = null;
        protected AudioSource m_AudioSource = null;
        protected GameObject m_AttachParent = null;
        protected GameObject m_AudioSrcObj = null;

        protected bool m_IsLoop = false;
        protected bool m_IsMute = false;
        protected string m_AudioName = null;
        protected float m_OrginVolume = 1;
        protected float m_Volume = 1;
        protected uint m_Handle = uint.MaxValue;
        protected bool m_PlayFrame = false;
        protected OnAudioEndCallback m_Callback = null;
        protected int m_PlayTime = 0;
        protected int m_TimeLength = 0;
        protected bool m_FadeIn = false;
    }

    #region 成员变量

    protected float[] m_AudioVolume = new float[(int)AudioType.MaxTypeNum];

    protected bool[] m_AudioMute = new bool[(uint)AudioType.MaxTypeNum];
    protected List<AudioInst>[] m_AudioDescList = new List<AudioInst>[(uint)AudioType.MaxTypeNum];

    protected uint m_CurAudioHandleCnt = 0;
    protected uint m_FrameCnt = 0;

    protected class AsyncPlayCommand
    {
        //public IAssetInstRequest m_AsyncLoad = null;
        public uint m_AsyncLoad = AssetLoader.INVILID_HANDLE;
        public float m_Volume = 1.0f;
        public bool m_IsLoop = false;
        public GameObject m_AttachObj = null;
        public uint m_AudioType = 0;
        public AudioInst m_AudioInst = null;
        public uint m_AudioHandle = uint.MaxValue;
        public string m_AudioName = null;
        public OnAudioEndCallback endCallback = null;
        public float speed = 0;
    }

    protected List<AsyncPlayCommand> m_AsyncPlayCommandList = new List<AsyncPlayCommand>();
    protected List<AsyncPlayCommand> m_AsyncIdleCommandList = new List<AsyncPlayCommand>();

	protected List<Object> preloadedClips = new List<Object>();

    #endregion

    #region 异步加载
    bool m_UseTMEngine = true;

    Tenmove.Runtime.AssetLoadCallbacks<object> m_AssetLoadCallbacks = new Tenmove.Runtime.AssetLoadCallbacks<object>(_OnLoadAssetSuccess, _OnLoadAssetFailure);

    static void _OnLoadAssetSuccess(string assetPath, object asset, int grpID, float duration, object userData)
    {
        if (null == userData)
        {
            Tenmove.Runtime.Debugger.LogError("User data can not be null!");
            return;
        }

        AudioManager _this = userData as AudioManager;
        if (null == _this)
        {
            Tenmove.Runtime.Debugger.LogError("User data type '{0}' is NOT AudioManager!");
            return;
        }

        AudioClip audioClip = asset as AudioClip;
        if (null == audioClip)
        {
            Tenmove.Runtime.Debugger.LogError("Asset type '{0}' error!", asset.GetType());
            return;
        }

        for (int i = 0, icnt = _this.m_AsyncPlayCommandList.Count; i < icnt; ++i)
        {
            AsyncPlayCommand curPlayCommand = _this.m_AsyncPlayCommandList[i];
            
            if (null != curPlayCommand && (uint)grpID == curPlayCommand.m_AsyncLoad)
            {
                if (null != audioClip)
                {
                    curPlayCommand.m_AudioInst.volume = _this.m_AudioVolume[curPlayCommand.m_AudioType];
                    curPlayCommand.m_AudioInst.orginVolume = curPlayCommand.m_Volume;
                    curPlayCommand.m_AudioInst.Play(curPlayCommand.m_AudioName, curPlayCommand.m_AudioHandle, audioClip, curPlayCommand.m_IsLoop, curPlayCommand.m_AttachObj, curPlayCommand.endCallback);
                    curPlayCommand.m_AudioInst.isMute = _this.m_AudioMute[curPlayCommand.m_AudioType];
                }
                
                curPlayCommand.m_AsyncLoad = AssetLoader.INVILID_HANDLE;
                curPlayCommand.m_AttachObj = null;
                curPlayCommand.m_AudioInst = null;
                curPlayCommand.m_AudioName = null;
                curPlayCommand.m_AudioType = (int)AudioType.MaxTypeNum;
                curPlayCommand.m_IsLoop = false;
                curPlayCommand.m_Volume = 1.0f;

                _this.m_AsyncIdleCommandList.Add(curPlayCommand);
                _this.m_AsyncPlayCommandList.RemoveAt(i);
                break;
            }
        }
    }

    static void _OnLoadAssetFailure(string assetPath,int taskID, Tenmove.Runtime.AssetLoadErrorCode errorCode, string errorMessage, object userData)
    {
        Logger.LogErrorFormat("[AudioManager]Load asset '{0}' has failed![Error:{1}]", assetPath, errorMessage);
    }

    #endregion

    #region 方法

    public override void Init()
    {
        Clear();
        _Reinit();
    }

    bool m_bPreStateVoicePlaying = false;
    void Update()
    {
        ++m_FrameCnt;
        _UpdateAsync();

        if (m_FrameCnt < 5)
            return;

        bool curPlaying = VoiceSDK.SDKVoiceManager.instance.IsVoicePlaying();
        if (curPlaying != m_bPreStateVoicePlaying)
        {
            m_bPreStateVoicePlaying = curPlaying;
            float factor = curPlaying ? 0.3f : 1.0f;

            float volumeSound = factor * (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume;
            SetVolume(AudioType.AudioStream, volumeSound);

            float volumeEffect = factor * (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume;
            SetVolume(AudioType.AudioEffect, volumeEffect);
            SetVolume(AudioType.AudioVoice, volumeEffect);
            SetVolume(AudioType.AudioGuide, volumeEffect);
        }

        for (int i = 0; i < m_AudioDescList.Length; ++ i)
        {
            List<AudioInst> curAudioDescList = m_AudioDescList[i];
            if (null == curAudioDescList) continue;

            for (int j = 0; j < curAudioDescList.Count; ++j)
            {
                AudioInst curAudioInst = curAudioDescList[j];
                if (curAudioInst.IsEnd())
                {
                    curAudioInst.Update(33 * 5);
                    curAudioInst.OnEnd();
                    curAudioInst.Stop();
                }
            }
        }

        m_FrameCnt = 0;
    }

    protected AsyncPlayCommand _AllocateAsyncCommand()
    {
        AsyncPlayCommand asyncPlayCommand = null;
        if (m_AsyncIdleCommandList.Count > 0)
        {
            int lastIdx = m_AsyncIdleCommandList.Count - 1;
            asyncPlayCommand = m_AsyncIdleCommandList[lastIdx];
            m_AsyncIdleCommandList.RemoveAt(lastIdx);
            return asyncPlayCommand;
        }

        return new AsyncPlayCommand();
    }

    protected void _UpdateAsync()
    {
        for(int i = 0,icnt = m_AsyncPlayCommandList.Count;i<icnt;++i)
        {
            AsyncPlayCommand curPlayCommand = m_AsyncPlayCommandList[i];
            
            //if(null != curPlayCommand && null != curPlayCommand.m_AsyncLoad)
            if (null != curPlayCommand && AssetLoader.INVILID_HANDLE != curPlayCommand.m_AsyncLoad)
            {
                //if(!curPlayCommand.m_AsyncLoad.IsDone()) continue;
                if (! AssetLoader.instance.IsRequestDone(curPlayCommand.m_AsyncLoad)) continue;

                //AudioClip audioClip = curPlayCommand.m_AsyncLoad.Extract().obj as AudioClip;
                AudioClip audioClip = AssetLoader.instance.Extract( curPlayCommand.m_AsyncLoad).obj as AudioClip;

                if (null != audioClip)
                {
                    curPlayCommand.m_AudioInst.volume = m_AudioVolume[curPlayCommand.m_AudioType];
                    curPlayCommand.m_AudioInst.orginVolume = curPlayCommand.m_Volume;
                    curPlayCommand.m_AudioInst.SetPitch(curPlayCommand.speed);
                    curPlayCommand.m_AudioInst.Play(curPlayCommand.m_AudioName, curPlayCommand.m_AudioHandle, audioClip, curPlayCommand.m_IsLoop, curPlayCommand.m_AttachObj, curPlayCommand.endCallback);
                    curPlayCommand.m_AudioInst.isMute = m_AudioMute[curPlayCommand.m_AudioType];
                }
                else
#if UNITY_EDITOR  && !LOGIC_SERVER
                    Logger.LogErrorFormat("Load audio clip [{0}] has failed!", curPlayCommand.m_AudioName);
#else
                    Logger.LogWarningFormat("Load audio clip [{0}] has failed!", curPlayCommand.m_AudioName);
#endif

                curPlayCommand.m_AsyncLoad = AssetLoader.INVILID_HANDLE;
            }

            if(null != curPlayCommand)
            {
                curPlayCommand.m_AsyncLoad = AssetLoader.INVILID_HANDLE;
                curPlayCommand.m_AttachObj = null;
                curPlayCommand.m_AudioInst = null;
                curPlayCommand.m_AudioName = null;
                curPlayCommand.m_AudioType = (int)AudioType.MaxTypeNum;
                curPlayCommand.m_IsLoop = false;
                curPlayCommand.m_Volume = 1.0f;

                m_AsyncIdleCommandList.Add(curPlayCommand);
            }

            m_AsyncPlayCommandList.RemoveAt(i);
            break;
        }
    }

    protected override void OnDestroy()
    {
        Clear();
        base.OnDestroy();
    }

    public void Clear()
    {
        for (int i = 0; i < m_AudioDescList.Length; ++i)
        {
            List<AudioInst> curAudioDescList = m_AudioDescList[i];
            if (null == curAudioDescList) continue;

            for (int j = 0; j < curAudioDescList.Count; ++j)
                curAudioDescList[j].Deinit();

            curAudioDescList.Clear();
        }
    }
    
    public void ClearOnEditor()
    {
        for (int i = 0; i < m_AudioDescList.Length; ++i)
        {
            List<AudioInst> curAudioDescList = m_AudioDescList[i];
            if (null == curAudioDescList) continue;

            for (int j = 0; j < curAudioDescList.Count; ++j)
                curAudioDescList[j].DeinitOnEditor();

            curAudioDescList.Clear();
        }
    }

	public void PreloadSound(string audioRes)
	{
/*		//预加载不预加载音效
		if (Global.PRELOAD_MODE == PreloadMode.PART_NO_AUDIO)
			return;*/

		//Logger.LogErrorFormat("preload sound:{0}", audioRes);
/*		AssetLoader.instance.LoadRes(audioRes, typeof(AudioClip));

		//输出预加载音效d
		List<string> list = GamePool.ListPool<string>.Get();
		list.Add(audioRes);
		ExceptionManager.GetInstance().PrintPreloadRes(list);
		GamePool.ListPool<string>.Release(list);*/
		#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAudio)
			return;
		#endif

		if (IsMute(AudioType.AudioEffect))
			return;

		CResPreloader.instance.AddRes(audioRes, false, 1, null, 0, 	null, CResPreloader.ResType.RES, typeof(AudioClip));
	}

	public void PreloadSound(int soundID)
	{
		if (soundID <= 0)
			return;
		
		var data = TableManager.GetInstance().GetTableItem<ProtoTable.SoundTable>(soundID);
		if (data == null || data!=null && data.Path.Count<=0)
			return;

		for(int i=0; i<data.Path.Count; ++i)
		{
			var audioRes = data.Path[i];
			PreloadSound(audioRes);
		}
	}

	public void AddPreloadSound(Object clip)
	{
		if (clip == null)
			return;
		
		preloadedClips.Add(clip);
	}

	public void ClearPreloadSound()
	{
		preloadedClips.Clear();
	}

    public AudioHandle PlaySound(AudioClip audioClip, AudioType type, float volume = 1.0f, bool isLoop = false, GameObject attachObj = null, bool isExculsive = false, bool AsyncLoad = false, OnAudioEndCallback callback = null,float speed = 1)
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAudio)
        {
            if (null != callback)
                callback();
            return uint.MaxValue;
		}
#endif

        if (null != audioClip)
        {
            uint typeIdx = (uint)type;
            if (typeIdx < (uint)AudioType.MaxTypeNum)
            {

                if (type != AudioType.AudioStream && IsMute(type))
                {
                    if (null != callback)
                        callback();
                    return uint.MaxValue;
                }

                string audioName = audioClip.name;
                AudioHandle hNewAudio = uint.MaxValue;
                if (isExculsive)
                {
                    hNewAudio = _CheckExclusive(audioName, type);
                    if (uint.MaxValue != hNewAudio)
                    {
                        if (null != callback)
                            callback();
                        return hNewAudio;
                    }
                }

                hNewAudio = _AllocHandle() | GetTypeHandle(typeIdx);
                Debug.Assert(hNewAudio != uint.MaxValue);

                AudioInst dstAudioInst = _GetAvailableAudioInst(type);
                if (null != dstAudioInst)
                {
                    dstAudioInst.orginVolume = volume;
                    dstAudioInst.volume = m_AudioVolume[(int)type];
                    dstAudioInst.Play(audioName, hNewAudio, audioClip, isLoop, attachObj, callback);
                    dstAudioInst.isMute = m_AudioMute[typeIdx];
                    dstAudioInst.SetPitch(speed);
                    return hNewAudio;
                }
                else
                    Logger.LogWarning("Create audio instance has failed!");
            }
            else
                Logger.LogWarning("Unsupported audio type!");
        }
        else
            Logger.LogWarning("Audio clip can not be null!");

        if (null != callback)
            callback();
        return uint.MaxValue;
    }

    public AudioHandle PlaySound(string audioRes,AudioType type,float volume = 1.0f,bool isLoop = false,GameObject attachObj = null,bool isExculsive = false, bool AsyncLoad = false, OnAudioEndCallback callback = null,float speed =1)
    {
#if DEBUG_REPORT_ROOT
		if (DebugSettings.instance.DisableAudio)
        {
            if (null != callback)
                callback();
            return uint.MaxValue;
        }
#endif

        AsyncLoad = false;

        if (!string.IsNullOrEmpty(audioRes) && Utility.IsStringValid(audioRes))
        {
            string audioName = audioRes;
            //int nameIdx = audioName.LastIndexOf('/');
            //if (0 <= nameIdx && nameIdx < audioName.Length)
            //    audioName = audioName.Substring(nameIdx + 1);

            uint typeIdx = (uint)type;
            if (typeIdx < (uint)AudioType.MaxTypeNum)
            {

                if (type != AudioType.AudioStream && IsMute(type))
                {
                    if (null != callback)
                        callback();
                    return uint.MaxValue;
                }

                AudioHandle hNewAudio = uint.MaxValue;
                if (isExculsive)
                {
                    hNewAudio = _CheckExclusive(audioName, type);
                    if (uint.MaxValue != hNewAudio)
                    {
                        if (null != callback)
                            callback();
                        return hNewAudio;
                    }
                }

                AudioInst dstAudioInst = _GetAvailableAudioInst(type);
                if (null != dstAudioInst)
                {
                    hNewAudio = _AllocHandle() | GetTypeHandle(typeIdx);
                    Debug.Assert(hNewAudio != uint.MaxValue);

                    if (AsyncLoad)
                    {
                        AsyncPlayCommand curCommand = _AllocateAsyncCommand();
                        if(null != curCommand)
                        {
                            //curCommand.m_AsyncLoad = AssetLoader.instance.LoadResAync(audioRes, typeof(AudioClip));
                            curCommand.m_AsyncLoad = AssetLoader.instance.LoadResAync(audioRes, typeof(AudioClip));
                            curCommand.m_AttachObj = attachObj;
                            curCommand.m_AudioHandle = hNewAudio;
                            curCommand.m_AudioInst = dstAudioInst;
                            curCommand.m_AudioName = audioName;
                            curCommand.m_AudioType = typeIdx;
                            curCommand.m_IsLoop = isLoop;
                            curCommand.m_Volume = volume;
                            curCommand.endCallback = callback;
                            curCommand.speed = speed;

                            m_AsyncPlayCommandList.Add(curCommand);

                            return hNewAudio;
                        }
                        else
                            Logger.LogWarningFormat("Allocate audio async-load command has failed!", audioRes);
                    }
                    else
                    {
                        var asset = AssetLoader.instance.LoadRes(audioRes, typeof(AudioClip));
                        if (asset != null && asset.obj != null)
                        {
                            AudioClip newAudioClip = asset.obj as AudioClip;
                            if (null != newAudioClip)
                            {
                                dstAudioInst.volume = m_AudioVolume[typeIdx];
                                dstAudioInst.orginVolume = volume;
                                dstAudioInst.Play(audioName, hNewAudio, newAudioClip, isLoop, attachObj, callback, (type == AudioType.AudioStream || type == AudioType.AudioEnvironment)) ;
                                dstAudioInst.isMute = m_AudioMute[typeIdx];
                                dstAudioInst.SetPitch(speed);
                                return hNewAudio;
                            }
                            else
#if UNITY_EDITOR && !LOGIC_SERVER
                                Logger.LogErrorFormat("Load audio clip [{0}] has failed!", audioRes);
#else
                            Logger.LogWarningFormat("Load audio clip [{0}] has failed!", audioRes);
#endif
                        }
                    }
                }
                else
                    Logger.LogWarning("Create audio instance has failed!");
            }
            else
                Logger.LogWarning("Unsupported audio type!");
        }
        else
            Logger.LogWarning("Audio res name can not be null!");

        if (null != callback)
            callback();
        return uint.MaxValue;
    }

	public AudioHandle PlaySound(int soundID)
	{
        if (soundID == 0)
            return uint.MaxValue;

        var data = TableManager.GetInstance().GetTableItem<ProtoTable.SoundTable>(soundID);

        if (data == null || data != null && data.Path.Count <= 0)
            return uint.MaxValue;

        string soundRes = data.Path[0];
        if (data.IsRandom > 0 && data.Path.Count > 1)
        {
            soundRes = data.Path[Random.Range(0, data.Path.Count)];
        }

        bool isLoop = data.Loop > 0;

        return PlaySound(soundRes, AudioType.AudioEffect, 1, isLoop, null, true);
	}

	public AudioHandle PlaySound(ProtoTable.SoundTable data,float speed = 1,float volume = 1)
	{
        if (data == null || data != null && data.Path.Count <= 0)
            return uint.MaxValue;

        string soundRes = data.Path[0];
        if (data.IsRandom > 0 && data.Path.Count > 1)
        {
            soundRes = data.Path[Random.Range(0, data.Path.Count)];
        }

        return PlaySound(soundRes, AudioType.AudioEffect, volume, false, null, true,false,null, speed);
	}

    public AudioHandle PlayGuideAudio(string audioRes,float volume = 1.0f, OnAudioEndCallback callback = null, bool isLoop = false, GameObject attachObj = null, bool isExculsive = false)
    {
        //Logger.LogError("Volume:" + m_AudioVolume[(int)AudioType.AudioEffect].ToString("0.00"));
        SetVolume(AudioType.AudioEffect, 0.0f);

        return PlaySound(audioRes, AudioType.AudioGuide, volume, isLoop, attachObj, isExculsive,false,()=> 
        {
            SetVolume(AudioType.AudioEffect, (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume);
            //Logger.LogError("On Callback PlayEnd!" + m_AudioVolume[(int)AudioType.AudioEffect].ToString("0.00"));
            if (null != callback)
                callback();
        });
    }
    

    protected void _Reinit()
    {
        for (int i = 0; i < (int)AudioType.MaxTypeNum; ++i)
        {
            m_AudioDescList[i] = new List<AudioInst>();
        }

        m_AudioVolume[(int)AudioType.AudioStream] = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Volume;
        m_AudioVolume[(int)AudioType.AudioEffect] = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume;
        m_AudioVolume[(int)AudioType.AudioVoice] = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Volume;
        m_AudioVolume[(int)AudioType.AudioEnvironment] = (float)GameClient.SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Volume;
        m_AudioVolume[(int)AudioType.AudioGuide] = (float)1.0f;

        m_AudioMute[(int)AudioType.AudioStream] = GameClient.SystemConfigManager.GetInstance().SystemConfigData.SoundConfig.Mute;
        m_AudioMute[(int)AudioType.AudioEffect] = GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Mute;
        m_AudioMute[(int)AudioType.AudioVoice] = GameClient.SystemConfigManager.GetInstance().SystemConfigData.MusicConfig.Mute;
        m_AudioMute[(int)AudioType.AudioEnvironment] = GameClient.SystemConfigManager.GetInstance().SystemConfigData.EnvironmentMusicConfig.Mute;
        m_AudioMute[(int)AudioType.AudioGuide] = false;

    }

    protected void _Clean()
    {

    }

    protected AudioHandle _CheckExclusive(string audioName, AudioType type)
    {
        uint typeIdx = (uint)type;
        if (typeIdx < (uint)AudioType.MaxTypeNum)
        {
            List<AudioInst> curAudioDescList = m_AudioDescList[typeIdx];
            if (null == curAudioDescList) return uint.MaxValue;

            for (int i = 0; i < curAudioDescList.Count; ++i)
            {
                AudioInst curAudioInst = curAudioDescList[i];
                if (!curAudioInst.IsEnd())
                    continue;

                if (curAudioInst.audioClipName == audioName)
                    return curAudioInst.handle;
            }
        }

        return uint.MaxValue;
    }

    protected uint GetTypeHandle(uint typeIdx)
    {
        return (typeIdx % 5) << 29;
    }

    protected uint GetTypeFromHandle(uint handle)
    {
       return  (handle >> 29) % 5;
    }

    protected AudioInst _GetAvailableAudioInst(AudioType type)
    {
        uint typeIdx = (uint)type;
        if (typeIdx < (uint)AudioType.MaxTypeNum)
        {
            AudioHandle hNewAudio = _AllocHandle() | GetTypeHandle(typeIdx);
            Debug.Assert(hNewAudio != uint.MaxValue);

            List<AudioInst> curAudioDescList = m_AudioDescList[typeIdx];
            if (null == curAudioDescList) return null;

            AudioInst dstAudioInst = null;
            for (int i = 0; i < curAudioDescList.Count; ++i)
            {
                AudioInst curAudioDesc = curAudioDescList[i];
                if (curAudioDesc.IsEnd())
                {
                    dstAudioInst = curAudioDesc;
                    break;
                }
            }

			if (null == dstAudioInst && curAudioDescList.Count < MAX_AUDIOSOURCE_NUM)
            {
                AudioInst kNewAudioInst = new AudioInst();
                if (kNewAudioInst.Init(this.gameObject))
                {
                    dstAudioInst = kNewAudioInst;
                    curAudioDescList.Add(kNewAudioInst);
                }
            }

            return dstAudioInst;
        }
        else
            Logger.LogWarning("Unsupported audio type!");

        return null;
    }

    public IAudioInst GetAudioInst(uint hHandle)
    {
        uint tpye = GetTypeFromHandle(hHandle);
        List<AudioInst> curAudioDescList = m_AudioDescList[tpye];
        if (null == curAudioDescList) return null;

        for (int i = 0; i < curAudioDescList.Count; ++ i)
        {
            AudioInst curAudioInst = curAudioDescList[i];
            if (hHandle == curAudioInst.handle)
                return curAudioInst;
        }

        return null;
    }

	public bool IsMute(AudioType type)
	{
		uint typeIdx = (uint)type;

		return m_AudioMute[typeIdx];
	}

    public void SetMute(AudioType type, bool isMute)
    {
        uint typeIdx = (uint)type;
        if (typeIdx < (uint)AudioType.MaxTypeNum)
        {
            m_AudioMute[typeIdx] = isMute;
            List<AudioInst> curAudioDescList = m_AudioDescList[typeIdx];
            if (null == curAudioDescList) return;
            for (int i = 0; i < curAudioDescList.Count; ++i)
                curAudioDescList[i].isMute = isMute;
        }
    }

    public void SetVolume(AudioType type,float volume)
    {
        uint typeIdx = (uint)type;
        if(typeIdx < (uint)AudioType.MaxTypeNum)
        {
            List<AudioInst> curAudioDescList = m_AudioDescList[typeIdx];
            if (null == curAudioDescList) return;
            for (int i = 0; i < curAudioDescList.Count; ++i)
            {
                AudioInst curAudioInst = curAudioDescList[i];
                curAudioInst.volume = volume;
            }

            m_AudioVolume[typeIdx] = volume;
        }
    }

    public float GetVolume(AudioType type)
    {
        uint typeIdx = (uint)type;
        if (typeIdx < (uint)AudioType.MaxTypeNum)
        {
            List<AudioInst> curAudioDescList = m_AudioDescList[typeIdx];
            if (null != curAudioDescList)
            {
                if(curAudioDescList.Count > 0)
                {
                    AudioInst curAudioInst = curAudioDescList[0];
                    return curAudioInst.volume;
                }
            }
        }

        return 1.0f;
    }

    public void Stop(AudioHandle handle)
    {
        if (uint.MaxValue == handle)
            return;

        IAudioInst audioInst = GetAudioInst(handle);
        if (null != audioInst)
            audioInst.Stop();
    }

    public void StopAll(AudioType type)
    {
        uint typeIdx = (uint)type;
        if (typeIdx < (uint)AudioType.MaxTypeNum)
        {
            List<AudioInst> curAudioDescList = m_AudioDescList[typeIdx];
            if (null == curAudioDescList) return;
            for (int i = 0; i < curAudioDescList.Count; ++i)
                curAudioDescList[i].Stop();
        }
    }

	public int GetAudioLength(AudioHandle handle)
	{
		IAudioInst audioInst = GetAudioInst(handle);
		if (null != audioInst)
			return audioInst.GetLength ();

		return 0;
	}

    protected uint _AllocHandle()
    {
        if (m_CurAudioHandleCnt + 1 == uint.MaxValue >> 2)
            m_CurAudioHandleCnt = 0;
        return m_CurAudioHandleCnt++;
    }


#endregion
}
#endif