using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using XUPorterJSON;
using GameClient;
using Tenmove.Runtime.Unity;

public enum GraphicLevel
{
	NORMAL = 0,
	MIDDLE = 1,
	LOW = 2,

    Num,
}
		

public class GeGraphicSetting : Singleton<GeGraphicSetting>
{
	public bool needPromoted = false;
    public bool isModified = false;
    protected readonly int VERSION = 2;
    protected readonly string GE_SETTING_RES = "GraphicSetting.json";

    protected class GeSettingDesc
    {
        public string m_DataKey;
        public string m_DataValue;
    }

    List<GeSettingDesc> m_GeSettingDescList = new List<GeSettingDesc>();

	#if !LOGIC_SERVER

    public override void Init()
    {
        if(!LoadSetting())
        {
            AddSetting("GraphicLevel", 0);
            AddSetting("PlayerDisplayNum", 20);
        }

        int version = 0;
        if(!GetSetting("Version",ref version))
        {
            Debug.Log("### Loading Version succeed!");

            int curAPILv = (int)OSInfo.GetAndroidOSAPILevel();
            Debug.Log("### GetAndroidOSAPILevel value is " + curAPILv.ToString());

            if (((int)AndroidAPILevel.None < curAPILv && curAPILv < (int)AndroidAPILevel.Level_22 ) || OSInfo.GetSysMemorySize() < 1200)
            {
                Debug.Log("### Android system version is not good or memory is less than 1GB!");

                isModified = true;
                SetSetting("GraphicLevel", 2);
                SetSetting("PlayerDisplayNum", 5);
            }
           
            AddSetting("Version", VERSION);
            Debug.Log("### Save setting!");
            SaveSetting();
        }
    }

	public void SetGraphicLevel(GraphicLevel level)
	{
		SetSetting("GraphicLevel", (int)level);

		if (level == GraphicLevel.NORMAL)
		{
			SetSetting("PlayerDisplayNum", 20);
		}
		else if (level == GraphicLevel.MIDDLE)
		{
			SetSetting("PlayerDisplayNum", 10);
		}
		else if (level == GraphicLevel.LOW)
		{
			SetSetting("PlayerDisplayNum", 5);

			DoSetLowLevel();
		}

        PostprocessManager.SetProcessQuality(level);
    }

	public int GetGraphicLevel()
	{
		int graphicLevel = 0;
		GeGraphicSetting.instance.GetSetting("GraphicLevel", ref graphicLevel);

		return graphicLevel;
	}
		
	public void DoSetLowLevel()
	{
		if (BattleMain.instance == null)
		{
		 	ClientSystemTown curTown = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
			if (null != curTown)
			{
				int num = 0;
				GetSetting("PlayerDisplayNum", ref num);
				curTown.OnGraphicSettingChange(num);
			}
				
		}
		else if (BattleMain.instance != null && !BattleMain.IsModePvP(BattleMain.battleType))
		{
			//友军
			var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
			for(int i=0; i<players.Count; ++i)
			{
				if (!players[i].playerActor.isLocalActor)
				{
					var actor = players[i].playerActor;

					actor.m_pkGeActor.GetEffectManager().useCube = true;


					var list = GamePool.ListPool<BeActor>.Get();
					//友军的召唤兽
					actor.CurrentBeScene.GetSummonBySummoner(list, actor);

					for(int j=0; j<list.Count; ++j)
					{
						list[j].m_pkGeActor.SetUseCube(true);
						list[j].m_pkGeActor.SetActorForLowLevel();
					}

					GamePool.ListPool<BeActor>.Release(list);
				}
			}

            //刷新showhitcomponent
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
            if (battleUI != null && battleUI.comShowHit != null)
                battleUI.comShowHit.RefreshGraphicSetting();
		}
	}

	public bool IsHighLevel()
	{
		int graphicLevel = 0;
		GeGraphicSetting.instance.GetSetting("GraphicLevel", ref graphicLevel);
		return graphicLevel == (int)GraphicLevel.NORMAL;
	}

	public bool IsMiddleLevel()
	{
		int graphicLevel = 0;
		GeGraphicSetting.instance.GetSetting("GraphicLevel", ref graphicLevel);
		return graphicLevel == (int)GraphicLevel.MIDDLE;
	}

	public bool IsLowLevel()
	{
		int graphicLevel = 0;
		GeGraphicSetting.instance.GetSetting("GraphicLevel", ref graphicLevel);
		return graphicLevel == (int)GraphicLevel.LOW; 
	}
		
    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    public bool AddSetting(string key,int value)
    {
        string str_value = value.ToString();
        return _AddSetting(key, str_value);
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    public bool AddSetting(string key, float value)
    {
        string str_value = value.ToString();
        return _AddSetting(key, str_value);
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    public bool AddSetting(string key, bool value)
    {
        string str_value = value.ToString();
        return _AddSetting(key, str_value);
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetSetting(string key, int value)
    {
        string str_value = value.ToString();
        return _SetSetting(key, str_value);
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetSetting(string key, float value)
    {
        string str_value = value.ToString();
        return _SetSetting(key, str_value);
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    public bool SetSetting(string key, bool value)
    {
        string str_value = value.ToString();
        return _SetSetting(key, str_value);
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    public bool GetSetting(string key,ref int value)
    {
        string str_value = null;
        if(_GetSetting(key,ref str_value))
        {
            if (int.TryParse(str_value, out value))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    public bool GetSetting(string key, ref float value)
    {
        string str_value = null;
        if (_GetSetting(key, ref str_value))
        {
            if (float.TryParse(str_value, out value))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 不要再高频函数中调用
    /// </summary>
    public bool GetSetting(string key, ref bool value)
    {
        string str_value = null;
        if (_GetSetting(key, ref str_value))
        {
            if (bool.TryParse(str_value, out value))
            {
                Debug.Log("### Load graphic setting " + key);
                return true;
            }
        }

        return false;
    }

    public bool LoadSetting()
    {
        try {
            string jsonData = null;
            if (FileArchiveAccessor.LoadFileInPersistentFileArchive(GE_SETTING_RES, out jsonData))
            {
                Hashtable settingTbl = MiniJSON.jsonDecode(jsonData) as Hashtable;
                IDictionaryEnumerator it = settingTbl.GetEnumerator();
                while (it.MoveNext())
                {
                    string key = it.Key as string;
                    string value = it.Value as string;

                    GeSettingDesc settingDesc = new GeSettingDesc();
                    settingDesc.m_DataKey = key;
                    settingDesc.m_DataValue = value;

                    m_GeSettingDescList.Add(settingDesc);
                }
                
                Debug.Log("### Load graphic setting succeed!");
                return true;
            }
            else
                Logger.LogWarning("Load graphic setting has failed!");

            return false;
        } 
        catch(System.Exception e)
        {
            Logger.LogErrorFormat("LoadSetting error {0}!", e.ToString());
            return false;
        }
    }

    public void SaveSetting()
    {
        Hashtable settingTbl = new Hashtable();
        for (int i = 0, icnt = m_GeSettingDescList.Count; i < icnt; ++i)
        {
            GeSettingDesc curSettingDesc = m_GeSettingDescList[i];
            if (null == curSettingDesc) continue;

            settingTbl.Add(curSettingDesc.m_DataKey,curSettingDesc.m_DataValue);
        }

        string jsonData = MiniJSON.jsonEncode(settingTbl);
        if (!FileArchiveAccessor.SaveFileInPersistentFileArchive(GE_SETTING_RES, jsonData))
            Logger.LogWarning("Save graphic setting has failed!");
    }

    public bool _GetSetting(string key, ref string value)
    {
        GeSettingDesc setting = _GetSettingDesc(key);
        if (null != setting)
        {
            value = setting.m_DataValue;
            return true;
        }

        return false;
    }

    protected bool _AddSetting(string key, string value)
    {
        GeSettingDesc setting = _GetSettingDesc(key, true);
        if (null != setting)
        {
            Logger.LogWarningFormat("Already exist setting with key {0}!", key);
            return false;
        }

        GeSettingDesc newSettingDesc = new GeSettingDesc();
        newSettingDesc.m_DataKey = key.ToLower();
        newSettingDesc.m_DataValue = value;

        m_GeSettingDescList.Add(newSettingDesc);
        return true;
    }

    protected bool _SetSetting(string key, string value)
    {
        GeSettingDesc setting = _GetSettingDesc(key);
        if (null != setting)
        {
            setting.m_DataValue = value;
            return true;
        }

        return false;
    }

    protected GeSettingDesc _GetSettingDesc(string key,bool mute = false)
    {
        for(int i = 0,icnt = m_GeSettingDescList.Count;i<icnt;++i)
        {
            GeSettingDesc curSettingDesc = m_GeSettingDescList[i];
            if(null == curSettingDesc) continue;

            if (key.Equals(curSettingDesc.m_DataKey, System.StringComparison.OrdinalIgnoreCase))
                return curSettingDesc;
        }

        if(!mute)
            Logger.LogWarningFormat("Can not find setting with key {0}!", key);
        return null;
    }

	public void CheckTownFPS()
	{
		if (!needPromoted && ComponentFPS.instance.GetLastAverageFPS() <= ComponentFPS.instance.lowFrameTown && !IsLowLevel())
		{
			/*GameClient.SystemNotifyManager.SysNotifyMsgBoxOkCancel("检查当前设备的帧率较低，是否切换到低画质？",
				() =>
				{
					SetGraphicLevel(GraphicLevel.LOW);
				},
				() =>
				{
				}
			);*/

			if (ClientSystemManager.GetInstance().IsMainPrefabTop())
				needPromoted = true;
		}
	}
		
	public void CheckBattleFPS()
	{
		if (!needPromoted && BattleMain.instance != null &&(ComponentFPS.instance.GetLastAverageFPS() <= ComponentFPS.instance.lowFrameTown && !IsLowLevel()) )
		{
			if (BattleMain.instance.GetDungeonManager() != null && 
				BattleMain.instance.GetDungeonManager().GetBeScene() != null &&
				BattleMain.instance.GetDungeonManager().GetBeScene().state == BeSceneState.onFight)
				needPromoted = true;

			/*GameClient.SystemNotifyManager.SysNotifyMsgBoxOkCancel("检查当前设备的帧率较低，是否切换到低画质？",
				() =>
				{
					SetGraphicLevel(GraphicLevel.LOW);
				},
				() =>
				{
				}
			);*/
		}
	}

	public void CheckComponent(GameObject go)
	{
		if (go == null)
			return;

		var coms = go.GetComponentsInChildren<ComGraphicControl>();
		if (coms != null)
		{
			for(int i=0; i<coms.Length; ++i)
			{
				if (coms[i].controlEnum == ComGraphicControl.GraphicControlEnum.High)
				{
					coms[i].gameObject.CustomActive(false);
				}
				else if (coms[i].controlEnum == ComGraphicControl.GraphicControlEnum.Mid && 
					(GeGraphicSetting.instance.IsLowLevel() || GeGraphicSetting.instance.IsMiddleLevel()))
				{
					coms[i].gameObject.CustomActive(false);
				}
				else if ((coms[i].controlEnum == ComGraphicControl.GraphicControlEnum.Low || coms[i].controlEnum == ComGraphicControl.GraphicControlEnum.VeryLow)
					&& GeGraphicSetting.instance.IsLowLevel())
				{
					coms[i].gameObject.CustomActive(false);
				}
				else
					coms[i].gameObject.CustomActive(true);
			}
		}
	}

	#else

	public void SetGraphicLevel(GraphicLevel level){}
	public int GetGraphicLevel(){return 0;}
	public void DoSetLowLevel(){}
	public bool IsHighLevel(){return false;}
	public bool IsMiddleLevel(){return false;}
	public bool IsLowLevel(){return false;}
	public bool AddSetting(string key,int value){return true;}
	public bool AddSetting(string key, float value){return true;}
	public bool AddSetting(string key, bool value){return true;}
	public bool SetSetting(string key, int value){return true;}
	public bool SetSetting(string key, float value){return true;}
	public bool SetSetting(string key, bool value){return true;}
	public bool GetSetting(string key,ref int value){return true;}
	public bool GetSetting(string key, ref float value){return true;}
	public bool GetSetting(string key, ref bool value){return true;}
	public bool LoadSetting(){return true;}
	public void SaveSetting(){}
	public bool _GetSetting(string key, ref string value){return true;}
	protected bool _AddSetting(string key, string value){return true;}
	protected bool _SetSetting(string key, string value){return true;}
	protected GeSettingDesc _GetSettingDesc(string key,bool mute = false){return null;}
	public void CheckTownFPS(){}
	public void CheckBattleFPS(){}
	public void CheckComponent(GameObject go){}


	#endif
}
