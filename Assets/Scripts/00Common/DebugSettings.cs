// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using XUPorterJSON;


public class DebugSettings : Singleton<DebugSettings>
{
    Dictionary<string, bool> m_DebugSettings = new Dictionary<string, bool>();

	bool _disableAnimation;
	bool _DummyEffect;
	bool _disableBugly;
	bool _disableAudio;
	bool _disableGameObjPool;
	bool _disableAnimMaterial;
	bool _disableAI;
	bool _disableHitText;
	bool _disableChatDispaly;
	bool _disableModuleLoad;
	bool _disableSnap;
	bool _disablePreload;

	bool _enableActionFrameCache;
	bool _enableDSkillDataCache;
	bool _enableTestFashionEquip;

	public bool DisableAnimation
	{
		get {
			return _disableAnimation;
		}
	}

	public bool DummyEffect
	{
		get {
			return _DummyEffect; 
		}
	}

	public bool DisableBugly
	{
		get {
			return _disableBugly;
		}
	}

	public bool DisableAudio
	{
		get {
			return _disableAudio;
		}
	}

	public bool DisableGameObjPool
	{
		get {
			return _disableGameObjPool;
		}
	}

	public bool DisableAnimMaterial
	{
		get {
			return _disableAnimMaterial;
		}
	}

	public bool DisableAI
	{
		get {
			return _disableAI;
		}
	}

	public bool DisableHitText
	{
		get {
			return _disableHitText;
		}
	}

	public bool DisableChatDisplay
	{
		get {
			return _disableChatDispaly;
		}
	}

	public bool DisableModuleLoad
	{
		get{
			return _disableModuleLoad;
		}
	}

	public bool DisableSnap
	{
		get{
			return _disableSnap;
		}
	}

	public bool DisablePreload
	{
		get{
			return _disablePreload;
		}
	}

	public bool EnableActionFrameCache
	{
		get {
			return _enableActionFrameCache;
		}
	}

	public bool EnableDSkillDataCache
	{
		get {
			return _enableDSkillDataCache;
		}
	}

	public bool EnableTestFashionEquip
	{
		get {
			return _enableTestFashionEquip;
		}
	}

    public override void Init()
    {
        base.Init();

        _LoadSetting();
    }

    public override void UnInit()
    {
        base.UnInit();
    }

    public bool IsDebugEnable(string setting)
    {
        bool isEnable = false;
        if(m_DebugSettings.TryGetValue(setting,out isEnable))
        {
            return isEnable;
        }

        return false;
    }

    public void SetDebugEnable(string setting,bool enable)
    {
        if (m_DebugSettings.ContainsKey(setting))
        {
            m_DebugSettings[setting] = enable;
        }
		else {
			m_DebugSettings.Add(setting, enable);
		}
		_SaveSetting();

        switch (setting)
        {
            case "DisableAnimation":
                _disableAnimation = enable;
                break;
            case "DummyEffect":
                _DummyEffect = enable;
                break;
            case "DisableBugly":
                _disableBugly = enable;
                break;
            case "DisableAudio":
                _disableAudio = enable;
                break;
            case "DisableGameObjPool":
                _disableGameObjPool = enable;
                break;
            case "DisableAnimMaterial":
                _disableAnimMaterial= enable;
                break;
            case "DisableAI":
                _disableAI = enable;
                break;
            case "DisableHitText":
                _disableHitText = enable;
                break;
			case "DisableChatDisplay":
				_disableChatDispaly = enable;
				break;
			case "DisableModuleLoad":
				_disableModuleLoad = enable;
				break;
			case "DisableSnap":
				_disableSnap = enable;
			break;
			case "DisablePreload":
			_disablePreload = enable;
			break;
		case "EnableActionFrameCache":
			_enableActionFrameCache = enable;
			break;
		case "EnableDSkillDataCache":
			_enableDSkillDataCache = enable;
			break;
		case "EnableTestFashionEquip":
			_enableTestFashionEquip = enable;
			break;
            default:
                Logger.LogErrorFormat("debugSetting找不到{0}", setting);
                break;
        }
    }

    public void AddDebugSetting(string setting,bool defEnabled)
    {
        if (!m_DebugSettings.ContainsKey(setting))
        {
            m_DebugSettings.Add(setting, defEnabled);
        }
    }

    protected void _LoadSetting()
    {
#if !DEBUG_REPORT_ROOT
		return;
#endif

        Hashtable setting = null;
        byte[] fileData = null;
        FileArchiveAccessor.LoadFileInPersistentFileArchive( "DebugSettings.json",out fileData);
        if (null != fileData)
        {
            setting = MiniJSON.jsonDecode(System.Text.UTF8Encoding.Default.GetString(fileData)) as Hashtable;
            if(null != setting)
            {
                m_DebugSettings.Clear();
                IDictionaryEnumerator it = setting.GetEnumerator();
                while (it.MoveNext())
                {
                    bool enable = false;
                    //string strBool = it.Value as string;
					enable = (bool)it.Value;
					if(enable/*bool.TryParse(strBool,out enable)*/)
                    {
						string key = it.Key as string;
                        m_DebugSettings.Add(it.Key as string, enable);
						if(enable)
						{
							switch(key)
							{
							case "DisableAnimation":
								_disableAnimation = true;
								break;
							case "DummyEffect":
								_DummyEffect = true;
								break;
							case "DisableBugly":
								_disableBugly = true;
								break;
							case "DisableAudio":
								_disableAudio = true;
								break;
							case "DisableGameObjPool":
								_disableGameObjPool = true;
								break;
							case "DisableAnimMaterial":
                                _disableAnimMaterial = true;
								break;
							case "DisableAI":
								_disableAI = true;
								break;
							case "DisableHitText":
								_disableHitText = true;
								break;
							case "DisableChatDisplay":
								_disableChatDispaly = true;
								break;
							case "DisableModuleLoad":
								_disableModuleLoad = true;
								break;
							case "DisableSnap":
								_disableSnap = true;
								break;
							case "DisablePreload":
								_disablePreload = true;
								break;
							case "EnableActionFrameCache":
								_enableActionFrameCache = true;
								break;
							case "EnableDSkillDataCache":
								_enableDSkillDataCache = true;
								break;
							case "EnableTestFashionEquip":
								_enableTestFashionEquip = true;
								break;
							default:
								Logger.LogErrorFormat("debugSetting找不到{0}", key);
								break;
							}
						}
                    }
                }
            }
        }
    }

    protected void _SaveSetting()
    {
        Hashtable setting = new Hashtable(m_DebugSettings);
        string str_data = MiniJSON.jsonEncode(setting);
        FileArchiveAccessor.SaveFileInPersistentFileArchive("DebugSettings.json", str_data);
    }
}
