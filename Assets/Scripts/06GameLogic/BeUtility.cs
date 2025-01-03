using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using GameClient;
using ProtoTable;
using System;
using System.Text;
using System.IO;

public class BeUtility {

    public static int RemoveMonsterLevel(int mid)
    {
        return mid - (mid / GlobalLogic.VALUE_100 % GlobalLogic.VALUE_100 * GlobalLogic.VALUE_100);
    }

	public static void AdjustMonsterDifficulty(ref int ownerID, ref int monsterID)
	{
        int tmpID = monsterID;

        tmpID -= tmpID % 10;
        tmpID += ownerID % 10;

        //替换难度后如果怪物表里有才替换
        var data = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(RemoveMonsterLevel(tmpID));
        if (data != null)
        {
            monsterID = tmpID;
        }
	}

	public static bool IsMonsterIDEqual(int ma, int mb)
	{
		return GetOnlyMonsterID(ma) == GetOnlyMonsterID(mb);
	}

	public static bool IsMonsterIDEqualList(List<int> lists, int ma)
	{
		for(int i=0; i<lists.Count; ++i)
		{
			if (IsMonsterIDEqual(ma, lists[i]))
				return true;
		}

		return false;
	}

	public static int GetOnlyMonsterID(int m)
	{
		//怪物ID扩容
		if (m < GlobalLogic.VALUE_100000)
			return m;
		
		return m/GlobalLogic.VALUE_10000;
	}

	//返回pvp的index
	public static int FindPvP(List<SkillFileName> list, int index)
	{
		var inItem = list[index];
		for(int i=0; i<list.Count; ++i)
		{
			var item = list[i];
			if (i != index && item.isPvp && item.folderName.Contains(inItem.folderName) 
                && item.lastName == inItem.lastName && item.weaponType == inItem.weaponType)
			{
				return i;
			}
		}

		return -1;
	}

    /// <summary>
    /// 返回吃鸡Index
    /// </summary>
    public static int FindChiji(List<SkillFileName> list, int index)
    {
        var inItem = list[index];
        for (int i = 0; i < list.Count; ++i)
        {
            var item = list[i];
            if (i != index && item.isChiji && item.folderName.Contains(inItem.folderName)
                && item.lastName == inItem.lastName && item.weaponType == inItem.weaponType)
            {
                return i;
            }
        }

        return -1;
    }

    public enum ESkillFileNameType
    {
        None,
        Json,
        Dir,
        Binary,
    }

    public static List<SkillFileName> GetSkillFileList(string path)
    {
        ESkillFileNameType type = ESkillFileNameType.Json;
#if UNITY_EDITOR
        type = ESkillFileNameType.Dir;
#elif UNITY_ANDROID || UNITY_IOS
        type = ESkillFileNameType.Binary;
#else
        type = ESkillFileNameType.Binary;
#endif

        return GetSkillFileList(path, type);
    }

    public static List<SkillFileName> GetSkillFileList(string path, ESkillFileNameType type)
    {
        List<SkillFileName> finalList = null;

        switch (type)
        {
            case ESkillFileNameType.None:
                break;
            case ESkillFileNameType.Json:
                finalList = _LoadFromJson(path);
                _GetSkillFileList(finalList);
                break;
            case ESkillFileNameType.Dir:
                finalList = _LoadFromDir(path);
                _GetSkillFileList(finalList);
                break;
            case ESkillFileNameType.Binary:
                finalList = _LoadFromBinary(path);
                break;
            default:
                break;
        }

        return finalList;
    }

    private static List<SkillFileName> _LoadFromBinary(string path)
    {
        int startIndex = 0;
        int length = 0;
        byte[] packBinary = SingletonData<PackScriptData>.Instance.GetPackBinary(path, ref startIndex, ref length);
        if (packBinary != null)
        {
            SkillFileNameList skillFiles = new SkillFileNameList();
            MemoryWriteReaderAnimation.TMemoryBufferReaderWriter<SkillFileNameList> memReader = new MemoryWriteReaderAnimation.TMemoryBufferReaderWriter<SkillFileNameList>(skillFiles);
            memReader.DeSerializeFrom(packBinary, startIndex, length);

            return skillFiles.m_SkillFileName;
        }

        return null;
    }


    private static string skPrefix = "Assets/Resources/";
    private static List<SkillFileName> _LoadFromDir(string path)
    {
        string relativePath = string.Empty;
        int prefixLen = skPrefix.Length;

        if (!path.StartsWith(skPrefix))
        {
            relativePath = path;
            path = skPrefix + path;
        }
        else
        {
            relativePath = path.Substring(prefixLen);
        }

        var allFiles = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
        List<SkillFileName> skillFileNames = new List<SkillFileName>(allFiles.Length);
        for (int i = 0; i < allFiles.Length; ++i)
        {
            var curFile = allFiles[i].Substring(prefixLen);
            curFile = curFile.Replace("\\", "/");
            curFile = curFile.Replace(".asset", "");
            curFile = curFile.Substring(relativePath.Length);
            if (curFile.StartsWith("/"))
            {
                curFile = curFile.Substring(1);
            }

            skillFileNames.Add(new SkillFileName(curFile, relativePath));
        }

        return skillFileNames;
    }

    private static List<SkillFileName> _LoadFromJson(string path)
    {
        string folderName = Utility.GetPathLastName(path);
        string fileListName = path + "/" + folderName + "_FileList";
        UnityEngine.Object obj = AssetLoader.instance.LoadRes(fileListName).obj;
        if (obj == null)
            return null;

        string content = System.Text.ASCIIEncoding.Default.GetString((obj as TextAsset).bytes);
        ArrayList list = XUPorterJSON.MiniJSON.jsonDecode(content) as ArrayList;

        List<SkillFileName> finalList = new List<SkillFileName>();

        for (int i = 0; i < list.Count; ++i)
        {
            var skillfn = new SkillFileName((string)list[i], path);

            finalList.Add(skillfn);
        }

        return finalList;
    }

    private static void _GetSkillFileList(List<SkillFileName> finalList)
    { 
        if (null == finalList)
        {
            return;
        }
		
        //下面这个函数主要是为了查找所有PVE技能配置文件对应的PVP路径
        for (int i = 0; i < finalList.Count; ++i)
        {
            //Pvp文件不作处理
            if (finalList[i].isPvp)
                continue;

            int index = FindPvP(finalList, i);
            if (index > -1)
                finalList[i].pvpPath = finalList[index].fullPath;
        }

        //下面这个函数主要是为了查找所有PVE技能配置文件对应的Chiji路径
        for (int i = 0; i < finalList.Count; ++i)
        {
            //Chiji文件不作处理
            if (finalList[i].isChiji)
                continue;

            int index = FindChiji(finalList, i);
            if (index > -1)
                finalList[i].chijiPath = finalList[index].fullPath;
        }
	}

	public static string GetStrengthenEffectName(string resName)
	{
		string path = resName;//resName.ToLower();

		if (path.Contains(Global.WEAPON_SWORD_NAME, System.StringComparison.OrdinalIgnoreCase))
			return Global.STRENGTH_SWORD_NAME;
		if (path.Contains(Global.WEAPON_GUN_NAME, System.StringComparison.OrdinalIgnoreCase))
			return Global.STRENGTH_GUN_NAME;
		if (path.Contains(Global.WEAPON_MAGE_NAME, System.StringComparison.OrdinalIgnoreCase))
			return Global.STRENGTH_MAGE_NAME;
        if (path.Contains(Global.WEAPON_FIGHTER_NAME, System.StringComparison.OrdinalIgnoreCase))
            return Global.STRENGTH_FIGHTER_NAME;
        if (path.Contains(Global.WEAPON_LIANDAO_NAME, System.StringComparison.OrdinalIgnoreCase) ||
            path.Contains(Global.WEAPON_ZHANFU_NAME, System.StringComparison.OrdinalIgnoreCase) ||
            path.Contains(Global.WEAPON_SHIZIJIA_NAME, System.StringComparison.OrdinalIgnoreCase))
            return Global.STRENGTH_COMMON_NAME;
        if (path.Contains(Global.WEAPON_NIANZHU_NAME, System.StringComparison.OrdinalIgnoreCase))
            return Global.STRENGTH_NIANZHU_NAME;
         
        return Global.STRENGTH_NAME;
	}

	public static IBeEventHandle ShowWin(BeActor actor, IBeEventHandle handler)
	{
		
		if (actor.sgGetCurrentState() == (int)ActionState.AS_IDLE)
			actor.Locomote(new BeStateData((int)ActionState.AS_WIN), true);
		else {
			if (handler == null)
			{
				handler = actor.RegisterEventNew(BeEventType.onStateChange, (GameClient.BeEvent.BeEventParam param) =>{
					var state = (ActionState)param.m_Int;
					if (state == ActionState.AS_IDLE)
					{
						actor.delayCaller.DelayCall(30, ()=>{
							actor.Locomote(new BeStateData((int)ActionState.AS_WIN), true);	
						});

						if (handler != null)
						{
							handler.Remove();
							handler = null;
						}
					}
				});
			}
		}

		return handler;
	}

	public static List<RaceEquip> GetEquips(int[] equipsData)
	{
		List<RaceEquip> equips = new List<RaceEquip>();
		for(int i=0; i<equipsData.Length; ++i)
		{
			RaceEquip raceEquip = new RaceEquip();
			raceEquip.id = (uint)equipsData[i];

			var data = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(equipsData[i]);
			if (data != null)
			{
				var attributeData = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(data.EquipPropID);
				if (attributeData != null)
				{
					raceEquip.phyAtk = (uint)attributeData.Atk;
					raceEquip.magAtk = (uint)attributeData.MagicAtk;
					raceEquip.phydef = (uint)attributeData.Def;
					raceEquip.magdef = (uint)attributeData.MagicDef;
					raceEquip.stamina = (uint)attributeData.Stamina;
					raceEquip.strenth = (uint)attributeData.Strenth;
					raceEquip.intellect = (uint)attributeData.Intellect;
					raceEquip.spirit = (uint)attributeData.Spirit;
				}
			}
			equips.Add(raceEquip);
		}

		return equips;
	}


    public static bool AddComboSkill(BeAIManager aiManager, AIInputData data, BeActor actor)
    {
        bool isComboSkill = false;
        int skillID = 0;
        bool randomChangeDirection = false;
        if (data.inputs.Count > 0)
        {
            randomChangeDirection = data.inputs[0].randomChangeDirection;
            skillID = data.inputs[0].skillID;
            isComboSkill = actor.IsComboSkill(data.inputs[0].skillID);
        }
        if (isComboSkill && skillID != 0)
        {
            data.inputs.Clear();

            var actionInfo = actor.GetActionInfoBySkillID(skillID);
            if (actionInfo == null) return false;
            data.AddInput(skillID, 0, 0, 0, false);

            do
            {
                if (actionInfo != null && actionInfo.comboSkillID != 0)
                {
                    int delayTime = actionInfo.comboStartFrame * 33;
                    data.AddInput(actionInfo.comboSkillID, delayTime, 0, 0, randomChangeDirection);
                    actionInfo = actor.GetActionInfoBySkillID(actionInfo.comboSkillID);
                }
                else
                    break;


            } while (true);
            aiManager.aiInputData = data;
            return true;
        }
        return false;
    }

    private static BeActor mMainPlayerActor = null;
    private static bool mIsPlayerActorDataDirty = false;

    public static void SetPlayerActorDataDirty()
    {
        mIsPlayerActorDataDirty = true;
    }

    public static BeActor GetMainPlayerActor(bool ispvp = false,List<ItemProperty> equipedEquipments = null, SkillSystemSourceType skillSourceType = SkillSystemSourceType.None)
    {
        if (mMainPlayerActor != null && !mIsPlayerActorDataDirty)
        {
            return mMainPlayerActor;
        }

        //mIsPlayerActorDataDirty = false;
        if (equipedEquipments == null)
            equipedEquipments = PlayerBaseData.GetInstance().GetEquipedEquipments();

        var jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
        mMainPlayerActor = new BeActor(0, 1, 0);

        mMainPlayerActor.InitData(
            PlayerBaseData.GetInstance().Level,
            jobData.FightID,
            SkillDataManager.GetInstance().GetSkillInfo(ispvp, skillSourceType),
            "town",
            PlayerBaseData.GetInstance().JobTableID,
            equipedEquipments,
            PlayerBaseData.GetInstance().buffList,
            0,
            PlayerBaseData.GetInstance().GetRankBuff(),
            PlayerBaseData.GetInstance().GetPetData(ispvp),
            null,
            null,
            false,
            false,
            ispvp,
            skillSourceType
        );

        if(skillSourceType == SkillSystemSourceType.Chiji)
        {
            mMainPlayerActor.GetEntityData().AdjustHPForPvP(
                PlayerBaseData.GetInstance().Level,
                PlayerBaseData.GetInstance().Level,
                PlayerBaseData.GetInstance().JobTableID, 0, (int)Protocol.RaceType.ChiJi);
        }

        return mMainPlayerActor;
    }

    /// <summary>
    /// 获取本地玩家的抗魔值  to do
    /// </summary>
    /// <returns></returns>
    public static int GetMainActorResist()
    {
        int resist = 0;
        var equipedEquipments = PlayerBaseData.GetInstance().GetEquipedEquipments();
        for (int i = 0; i < equipedEquipments.Count; i++)
        {
            var equip = equipedEquipments[i];
            if (equip != null)
            {
                resist += equip.resistMagic;
            }
        }

        var buffList = PlayerBaseData.GetInstance().buffList;
        for (int i = 0; i < buffList.Count; i++)
        {
            var buff = buffList[i];
            if (buff == null)
            {
                continue;
            }
            var buffTable = TableManager.instance.GetTableItem<BuffTable>(buff.id);
            if (buffTable == null)
            {
                continue;
            }
            resist += TableManager.GetValueFromUnionCell(buffTable.ResistMagic, 1);
        }
        return resist;
    }

    public static DisplayAttribute GetMainPlayerActorAttribute(bool ispvp=false, bool isChiji=false)
    {
        SkillSystemSourceType skillSourceType = SkillSystemSourceType.None;
        if (isChiji)
        {
            skillSourceType = SkillSystemSourceType.Chiji;
        }

        BeActor actor = GetMainPlayerActor(ispvp, null, skillSourceType);
        BeEntityData data = actor.GetEntityData();
        DisplayAttribute attribute = BeEntityData.GetActorAttributeForDisplay(data);
        return attribute;
    }

    public static DisplayAttribute GetMainPlayerActorAttribute(BeEntityData entityData, bool ispvp = false, bool isChiji = false)
    {
        DisplayAttribute attribute = BeEntityData.GetActorAttributeForDisplay(entityData);
        return attribute;
    }

    //获取主玩家通过Buff增加的抗魔值
    public static int GetMainPlayerResistAddByBuff()
    {
        int resistAdd = 0;
        BeActor actor = GetMainPlayerActor();
        if (actor == null)
            return resistAdd;
        List<BeBuff> buffList = GamePool.ListPool<BeBuff>.Get();
        buffList = actor.buffController.GetBuffList();
        for (int i = 0; i < buffList.Count; i++)
        {
            BeBuff buff = buffList[i];
            if (buff != null)
            {
                int resistMagic = TableManager.GetValueFromUnionCell(buff.buffData.ResistMagic,buff.level);
                if(resistMagic>0)
                    resistAdd += resistMagic;
            }
        }
        GamePool.ListPool<BeBuff>.Release(buffList);
        return resistAdd;
    }
	public static BeActor GetPlayerActorByRaceInfo(RacePlayerInfo racePlayer)
	{
		var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(racePlayer.occupation);
        
		BeActor actor = new BeActor(0, 1, 0);
        if (jobData == null)
        {
            Logger.LogErrorFormat("职业表没有此职业，ID：｛0｝", racePlayer.occupation);
            return actor;
        }
        actor.InitData(
			racePlayer.level,
			jobData.FightID,
			BattlePlayer.GetSkillInfo(racePlayer),
			"town",
			racePlayer.occupation,
			BattlePlayer.GetEquips(racePlayer,false),
			BattlePlayer.GetBuffList(racePlayer),
			BattlePlayer.GetWeaponStrengthenLevel(racePlayer),
			BattlePlayer.GetRankBuff(racePlayer),
			BattlePlayer.GetPetData(racePlayer,false)
		);

		return actor;
	}

	public static DisplayAttribute GetPlayerActorAttributeByRaceInfo(RacePlayerInfo racePlayer)
	{
		var actor = GetPlayerActorByRaceInfo(racePlayer);
		BeEntityData data = actor.GetEntityData();
		DisplayAttribute attribute = BeEntityData.GetActorAttributeForDisplay(data);
		return attribute;
	}

	public static void SendGM(string str)
	{
		var req = new SceneChat();
		req.channel = 1;
		req.targetId = 0;
		req.voiceKey = "";
		req.word = str;

		Network.NetManager.instance.SendCommand(Network.ServerType.GATE_SERVER, req);
	}

	public static void AddBuffFromSkill(int skillID, int level, List<BuffInfoData> list,bool isPvp = false)
	{
		//int skillID = petTableData.Skills[data.skillIndex];
		if (skillID > 0)
		{
			var skillData = TableManager.GetInstance ().GetTableItem<ProtoTable.SkillTable> (skillID);
			if (skillData == null)
				return;
				
			//添加BuffInfo区分Pve和Pvp
            FlatBufferArray<int> buffInfoList = skillData.BuffInfoIDs;
            if (isPvp)
                buffInfoList = skillData.BuffInfoIDsPVP;

			for(int j=0; j< buffInfoList.Count; ++j)
			{
				var infoID = buffInfoList [j];
				if (infoID <= 0)
					continue;

				BuffInfoData infoData = new BuffInfoData (infoID, level);
				infoData.level = level;

				var buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(infoData.buffID);
                if (buffData == null)
                    continue;

				if (buffData.Type >= 19)//局外buff
					continue;

				list.Add (infoData);
			}
		}
	}


    public static void CopyVectors(int[] fromVec, int[] toVec)
    {
        for(int i=0; i<fromVec.Length && i<toVec.Length; ++i)
        {
            toVec[i] = toVec[i];
        }
    }

    private static int FindAbnormalIndex(string arName)
    {
        for(int i=0; i<Global.ABNORMAL_COUNT; ++i)
        {
            if (Global.AbnormalNames[i] == arName)
                return i;
        }

        return -1;
    }

    public static int[] ParseAbnormalResistString(IList<string> strs)
    {
        int[] rets = new int[Global.ABNORMAL_COUNT];
        for(int i=0; i<strs.Count; ++i)
        {
            try
            {
                var tokens = strs[i].Split(':');
                string arName = tokens[0];
                int value = System.Convert.ToInt32(tokens[1]);

                int arIndex = FindAbnormalIndex(arName);
                if (arIndex == -1)
                {
                    var error = string.Format("ParseAbnormalResistString找不到异常状态{0}", arName);
                    Logger.LogErrorFormat(error);
#if UNITY_EDITOR
                    SystemNotifyManager.SysNotifyMsgBoxOK(error);
#endif
                }
                else
                {
                    if (value != 0)
                        rets[arIndex] = value;
                }
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("ParseAbnormalResistString:{0}", e.ToString()));
#endif
                Logger.LogErrorFormat("ParseAbnormalResistString:{0}", e.ToString());
            }
            

        }

        return rets;
    }

    public static void UseItemInBattle(int itemid, int skillid, int num=1)
    {
#if !LOGIC_SERVER
        ItemData itemData = ItemDataManager.GetInstance().GetItemByTableID(itemid);
        if (itemData != null)
        {

            SceneUseItem msg = new SceneUseItem();
            msg.uid = itemData.GUID;
            msg.param1 = (uint)Mathf.Max(1, num);
            msg.param2 = (uint)skillid;

            Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, msg);
        }
#endif
    }

    public static void ChangeWeaponInBattle(ulong uid,ulong sideUid)
    {
#if !LOGIC_SERVER
        
        if (uid > 0)
        {
            SceneUseItem msg = new SceneUseItem();
            msg.uid = uid;
            ItemData itemData = ItemDataManager.GetInstance().GetItem(uid);
            if (itemData != null && itemData.EquipWearSlotType == EEquipWearSlotType.EquipWeapon)
            {
                msg.param1 = ItemDataManager.iSwitchSecondWeaponId;
            }
            else if (itemData != null && itemData.EquipWearSlotType == EEquipWearSlotType.SecondEquipWeapon)
            {
                msg.param1 = ItemDataManager.iSwitchWeaponId;
            }

            Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, msg);         
        }
#endif
    }

    //判断状态列表中是否包含某个状态
    public static bool HaveBuffState(IList<int> stateDataList, BeBuffStateType type)
    {
        for (int i = 0; i < stateDataList.Count; ++i)
        {
            var state = stateDataList[i];
            if (state < 1)
                continue;

            BeBuffStateType bs = (BeBuffStateType)(1 << state);
            if (bs == type)
                return true;
        }
        return false;
    }

	/// <summary>
    /// 根据装备槽位获取强化等级
    /// </summary>
    /// <returns></returns>
    public static int GetEquipsStrengthBySlot(BeActor actor, int equipWearSlot)
    {
        List<ItemProperty> equipList = actor.attribute.itemProperties;
        if (equipList == null || equipList.Count <= 0)
            return -1;
        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList[i].grid != equipWearSlot || IsFashionWear(equipList[i].itemID))
                continue;
            return equipList[i].strengthen;
        }
        return -1;
    }

    /// <summary>
    /// 是否为穿戴时装
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static bool IsFashionWear(int itemId)
    {
        var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemId);
        if (itemData == null)
            return false;
        if ((int)itemData.SubType >= 11 && (int)itemData.SubType <= 16)
            return true;
        return false;
    }

    //判断当前帧是否有帧标签
    public static bool CheckHaveTag(int tag,int flag)
    {
        int temp = (int)tag & flag;
        return temp != 0;
    }

    /// <summary>
    /// 执行复活逻辑
    /// </summary>
    public static void PlayerReborn(BeActor actor)
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null)
            return;
        if (BattleMain.instance.GetDungeonManager() == null)
            return;
        if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager() == null)
            return;
        int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
        if (!DungeonUtility.CanReborn(id, true))
        {
            SystemNotifyManager.SystemNotify(1098);
            return;
        }
        var dungeonTable = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table;
        if (dungeonTable != null)
        {
            int dungeonRebornCount = dungeonTable.RebornCount;
            bool isBattleRebornCountOk = true;
            var battle = BattleMain.instance.GetBattle();
            if (battle != null && battle.IsRebornCountLimit())
            {
                isBattleRebornCountOk = battle.GetLeftRebornCount() > 0;
            }
            if (isBattleRebornCountOk)
            {
                if (dungeonRebornCount > 0 && actor != null)
                {
                    int actorDungeonCount = actor.dungeonRebornCount;
                    if (actorDungeonCount >= dungeonRebornCount)
                    {
                        return;
                    }
                }
            }
            else
                return;
        }
        byte mainPlayerSeat = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerInfo.seat;
        DungeonUtility.StartRebornProcess(mainPlayerSeat, mainPlayerSeat, id);
#endif
    }

    public static bool CheckVipAutoDrug()
    {
#if !LOGIC_SERVER
        int vipLevel = PlayerBaseData.GetInstance().VipLevel;
        if (vipLevel == 0 || !CheckVipFuncOpen(SettingManager.vipDrugTableId))
            return false;
        if (CheckNotSet(SettingManager.STR_VIPDRUG))
            return GetDefaultData(SettingManager.vipDrugTableId);
        string roleId = PlayerBaseData.GetInstance().RoleID.ToString();
        return SettingManager.GetInstance().GetVipSettingData(SettingManager.STR_VIPDRUG, roleId);
#else
        return false;
#endif
    }

    public static bool CheckVipAutoUseDrug()
    {
#if !LOGIC_SERVER
        int vipLevel = PlayerBaseData.GetInstance().VipLevel;
        if (vipLevel == 0 || !CheckVipFuncOpen(SettingManager.vipPreferTableId))
            return false;
        if (CheckNotSet(SettingManager.STR_VIPPREFER))
            return GetDefaultData(SettingManager.vipPreferTableId);
        string roleId = PlayerBaseData.GetInstance().RoleID.ToString();
        return SettingManager.GetInstance().GetVipSettingData(SettingManager.STR_VIPPREFER, roleId);
#else
        return false;
#endif
    }

    public static bool CheckVipAutoReborn()
    {
#if !LOGIC_SERVER
        int vipLevel = PlayerBaseData.GetInstance().VipLevel;
        if (vipLevel == 0 || !CheckVipFuncOpen(SettingManager.vipRebornTableId))
            return false;
        if (CheckNotSet(SettingManager.STR_VIPREBORN))
            return GetDefaultData(SettingManager.vipRebornTableId);
        string roleId = PlayerBaseData.GetInstance().RoleID.ToString();
        return SettingManager.GetInstance().GetVipSettingData(SettingManager.STR_VIPREBORN, roleId);
#else
        return false;
#endif
    }

    public static bool CheckVipAutoUseCrystalSkill()
    {
#if !LOGIC_SERVER
        int vipLevel = PlayerBaseData.GetInstance().VipLevel;
        if (vipLevel == 0 || !CheckVipFuncOpen(SettingManager.vipUseCrystalTableId))
            return true;
        if (CheckNotSet(SettingManager.STR_VIPCRYSTAL))
            return GetDefaultData(SettingManager.vipUseCrystalTableId);
        string roleId = PlayerBaseData.GetInstance().RoleID.ToString();
        return SettingManager.GetInstance().GetVipSettingData(SettingManager.STR_VIPCRYSTAL, roleId);
#else
        return true;
#endif
    }

    protected static bool CheckNotSet(string key)
    {
        string roleId = PlayerBaseData.GetInstance().RoleID.ToString();
        string realyKey = string.Format("{0}{1}", key, roleId);
        return PlayerLocalSetting.GetValue(realyKey) == null;
    }

    public static bool GetDefaultData(int tableId)
    {
        ProtoTable.SwitchClientFunctionTable switchClientTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SwitchClientFunctionTable>(tableId);
        return switchClientTableData.ValueB == 1 ? true : false;
    }

    public static bool CheckVipFuncOpen(int tableId)
    {
        return TableManager.instance.GetTableItem<ProtoTable.SwitchClientFunctionTable>(tableId).Open;
    }

    //获取关卡抗魔值
    public static int GetDungeonMagicValue(BeEntity owner)
    {
        if (owner == null
           || owner.CurrentBeScene == null
           || owner.CurrentBeBattle.dungeonManager == null
           || owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager() == null)
            return 0;
        int dungeonId = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().id.dungeonID;
        if (dungeonId == -1)
            return 0;
        return DungeonUtility.GetDungeonResistMagicValueById(dungeonId);
    }


    //重置摄像机
    public static void ResetCamera()
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null || BattleMain.instance.GetDungeonManager() == null)
            return;
        var geScene = BattleMain.instance.GetDungeonManager().GetGeScene();
        if (geScene == null)
            return;
        var cameraCtrl = geScene.GetCamera().GetController();
        if (cameraCtrl != null)
        {
            cameraCtrl.ResetCamera();
            cameraCtrl.SetPause(false);
            float off = -1 * cameraCtrl.GetOffset();
            cameraCtrl.MoveCamera(off, 0.01f);
        }
#endif
    }

    //装换装备宝珠相关信息
    public static PrecBead[] SwitchPrecBead(RacePrecBead[] racePreBead)
    {
        if (racePreBead == null)
            return null;
        PrecBead[] precBead = new PrecBead[racePreBead.Length];
        for(int i = 0; i < racePreBead.Length; i++)
        {
            if (racePreBead[i] == null)
                continue;
            precBead[i] = new PrecBead();
            int beadId = (int)racePreBead[i].preciousBeadId;
            precBead[i].preciousBeadId = beadId;
            precBead[i].randomBuffId = (int)racePreBead[i].buffId;
        }
        return precBead;
    }
    public static List<InscriptionHoleData> SwitchInscriptHoleData(UInt32[] ids)
    {
        List<InscriptionHoleData> result = new List<InscriptionHoleData>();
        for (int i = 0; i < ids.Length; i++)
        {
            result.Add(new InscriptionHoleData
            {
                InscriptionId = (int)ids[i]
            });
        }
        return result;
    }

    //获取职业表对应的附加怪物模型路径
    public static string GetAttachModelPath(int jobId)
    {
        JobTable jobData = TableManager.instance.GetTableItem<JobTable>(jobId);
        if (jobData == null)
            return null;
        ResTable resData = TableManager.instance.GetTableItem<ResTable>(jobData.AttachMonsterResID);
        if (resData == null)
            return null;
        return resData.ModelPath;
    }

    //获取当前职业等级下 技能所能提升到的最大等级
    public static int GetSkillTopLevelByRoleLevel(int skillid, int RoleLevel)
    {
        SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillid);
        if (skillData == null)
        {
            return 0;
        }

        if (RoleLevel < skillData.LevelLimit)
        {
            return 0;
        }

        if (skillData.LevelLimitAmend <= 0)
        {
            return 0;
        }

        int lv = (RoleLevel - skillData.LevelLimit) / skillData.LevelLimitAmend + 1;

        if(lv > skillData.TopLevelLimit)
        {
            lv = skillData.TopLevelLimit;
        }

        return lv;
    }

    //设置决斗场机器人技能等级
    public static Dictionary<int, int> GetPKRobotSkillLevel(Dictionary<int,int> skillInfo,int roleLevle)
    {
        if (skillInfo == null)
            return null;
        Dictionary<int, int> skillInfoNew = new Dictionary<int, int>(); 
        Dictionary<int, int>.Enumerator it = skillInfo.GetEnumerator();
        while (it.MoveNext())
        {
            int skillId = (int)it.Current.Key;
            int value = (int)it.Current.Value;
            int skillLevel = GetSkillTopLevelByRoleLevel(skillId, roleLevle);
            if (skillLevel > 0)
                skillInfoNew.Add(skillId, skillLevel);
            else
                skillInfoNew.Add(skillId, value);
        }
        return skillInfoNew;
    }

    //获取装备强化系数表中相关系数 拿到值以后要除以100
    public static int GetEquipStrModByStrength(int iLevel, EquipStrMod strMod)
    {
        EquipStrModTable equipStrMode = TableManager.instance.GetTableItem<EquipStrModTable>(2);
        if (iLevel < 1 || iLevel > 20)
        {
            return 0;
        }

        int count = 0;
        int mod = 0;

        switch (strMod)
        {
            case EquipStrMod.WpStrenthMod:
                count = equipStrMode.WpStrenthMod.Count;
                mod =  equipStrMode.WpStrenthMod[iLevel - 1];
                break;
            case EquipStrMod.ArmStrenthMod:
                count = equipStrMode.ArmStrenthMod.Count;
                mod = equipStrMode.ArmStrenthMod[iLevel - 1];
                break;
            case EquipStrMod.JewStrenthMod:
                count = equipStrMode.JewStrenthMod.Count;
                mod = equipStrMode.JewStrenthMod[iLevel - 1];
                break;
        }

        if (iLevel - 1 < 0 || iLevel > count)
        {
            return 0;
        }

        return mod;
    }

    //获取装备强化系数表中相关系数  拿到值以后要除以100
    public static int GetEquipStrModByColor(int color, EquipStrMod strMod)
    {
        EquipStrModTable equipStrMode = TableManager.instance.GetTableItem<EquipStrModTable>(2);
        if (color < (int)ItemTable.eColor.WHITE || color > (int)ItemTable.eColor.YELLOW)
        {
            return 0;
        }

        int count = 0;
        int mod = 0;

        switch (strMod)
        {
            case EquipStrMod.WpColorQaMod:
                count = equipStrMode.WpColorQaMod.Count;
                mod = equipStrMode.WpColorQaMod[color - 1];
                break;
            case EquipStrMod.WpColorQbMod:
                count = equipStrMode.WpColorQbMod.Count;
                mod = equipStrMode.WpColorQbMod[color - 1];
                break;
            case EquipStrMod.ArmColorQaMod:
                count = equipStrMode.ArmColorQaMod.Count;
                mod = equipStrMode.ArmColorQaMod[color - 1];
                break;
            case EquipStrMod.ArmColorQbMod:
                count = equipStrMode.ArmColorQbMod.Count;
                mod = equipStrMode.ArmColorQbMod[color - 1];
                break;
            case EquipStrMod.JewColorQaMod:
                count = equipStrMode.JewColorQaMod.Count;
                mod = equipStrMode.JewColorQaMod[color - 1];
                break;
            case EquipStrMod.JewColorQbMod:
                count = equipStrMode.JewColorQbMod.Count;
                mod = equipStrMode.JewColorQbMod[color - 1];
                break;
        }

        if (color - 1 < 0 || color > count)
        {
            return 0;
        }

        return mod;
    }

    //装备是否是首饰
    public static bool IsJewelry(ProtoTable.ItemTable.eSubType eSubType)
    {
        switch (eSubType)
        {
            case ItemTable.eSubType.RING:
            case ItemTable.eSubType.NECKLASE:
            case ItemTable.eSubType.BRACELET:
                return true;
        }

        return false;
    }

    //装备是否是防具
    public static bool IsArmy(ProtoTable.ItemTable.eSubType eSubType)
    {
        switch (eSubType)
        {
            case ItemTable.eSubType.HEAD:
            case ItemTable.eSubType.CHEST:
            case ItemTable.eSubType.BELT:
            case ItemTable.eSubType.LEG:
            case ItemTable.eSubType.BOOT:
                return true;
        }

        return false;
    }

    //装备是否是武器
    public static bool IsWeapon(ProtoTable.ItemTable.eSubType eSubType)
    {
        return eSubType == ItemTable.eSubType.WEAPON;
    }
    
    //获取本地角色 验证服务器不能用
    public static BeActor GetLocalActor()
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null)
            return null;
        if (BattleMain.instance.GetPlayerManager() == null)
            return null;
        if (BattleMain.instance.GetPlayerManager().GetMainPlayer() == null)
            return null;
        return BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
#else
    return null;
#endif
    }

    //获取当前的地下城名称 验证服务器不能用
    public static string GetDungeonName()
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null)
            return null;
        if (BattleMain.instance.GetDungeonManager() == null)
            return null;
        if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager() == null)
            return null;
        if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table == null)
            return null;
        return BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.Name;
#else
        return null;
#endif
    }

    //获取所有的友方玩家 包含自己
    public static void GetAllFriendPlayers(BeActor owner,List<BeActor> list)
    {
        if (owner == null
            || owner.CurrentBeBattle == null
            || owner.CurrentBeBattle.dungeonPlayerManager == null)
            return;
        List<BattlePlayer> playerList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if (playerList == null)
            return;
        for (int i = 0; i < playerList.Count; i++)
        {
            BeActor actor = playerList[i].playerActor;
            if (actor != null && !list.Contains(actor) && actor.m_iCamp == owner.m_iCamp)
                list.Add(actor);
        }
    }
    public static void GetAllEnemyPlayers(BeActor owner, List<BeActor> list)
    {
        if (owner == null
            || owner.CurrentBeBattle == null
            || owner.CurrentBeBattle.dungeonPlayerManager == null)
            return;
        List<BattlePlayer> playerList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if (playerList == null)
            return;
        for (int i = 0; i < playerList.Count; i++)
        {
            BeActor actor = playerList[i].playerActor;
            if (actor != null && !list.Contains(actor) && actor.m_iCamp != owner.m_iCamp)
                list.Add(actor);
        }
    }

    private static bool IsNewFashion(int itemID)
    {
        if (itemID <= 0)
            return false;

        var data = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
        if (data != null)
        {
            var modleData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(data.ResID);
            if (modleData != null && modleData.newFashion)
                return true;
        }

        return false;
    }

    //有没有新头饰，返回下标，没有是-1
    private static int HasNewHeadWearOrHead(UInt32[] equipItemIds, EFashionWearSlotType slotType)
    {
        for (int i = 0; i < equipItemIds.Length; ++i)
        {
            int itemID = (int)equipItemIds[i];
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
            if (data == null)
                continue;
            var tmpSlotType = (EFashionWearSlotType) (data.SubType - 10);
            if (tmpSlotType == slotType && IsNewFashion(itemID))
                return i;
        }

        return -1;
    }


    public static void DealWithFashion(UInt32[] equipItemIds)
    {
        int newHeadWearIndex = HasNewHeadWearOrHead(equipItemIds, EFashionWearSlotType.Chest);
        int newHeadIndex = HasNewHeadWearOrHead(equipItemIds, EFashionWearSlotType.Head);

        //有新头饰但是没有新头，就不穿新头饰
        if (newHeadWearIndex != -1 && newHeadIndex == -1)
        {
            equipItemIds[newHeadWearIndex] = 0;
        }
    }

    public static UInt32[] CopyVector(UInt32[] equipItemIds)
    {
        if (equipItemIds == null)
            return null;

        var copied = new UInt32[equipItemIds.Length];
        for(int i=0; i<equipItemIds.Length; ++i)
            copied[i] = equipItemIds[i];

        return copied;
    }
    public static void CancelCurrentSkill(BeActor monster)
    {
        var curSkill = monster.GetCurrentSkill();
        if(curSkill != null)
        {
            monster.CancelSkill(curSkill.skillID);
            monster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        }
    }
    public static void ForceMonsterUseSkill(int monsterid, int skillId,BeActor owner)
    {
        if(owner != null)
        {
            List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindActorById2(monsters, monsterid);
            for (int i = 0; i < monsters.Count; ++i)
            {
                var skill = monsters[i].GetSkill(skillId);
                if (skill != null)
                {
                    skill.ResetCoolDown();
                    monsters[i].UseSkill(skillId, true);
                }
            }
            GamePool.ListPool<BeActor>.Release(monsters);
        }
    }
    public static void DoMonsterDeadById(int monsterid,BeActor owner)
    {
        if(owner != null)
        {
            List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindActorById2(monsters, monsterid);
            for (int i = 0; i < monsters.Count; ++i)
            {
                var sidetick = monsters[i];
                if (sidetick == null || sidetick.IsDead())
                    continue;
                sidetick.DoDead(true);
            }
            GamePool.ListPool<BeActor>.Release(monsters);
        }
    }
    //获取普攻或者combo技能的源技能ID
    public static int GetComboSkillId(BeActor actor, int skillId)
    {
        int sourceId = skillId;
        if (actor == null)
            return sourceId;
        BeSkill skill = actor.GetSkill(sourceId);
        if (skill == null)
            return sourceId;
        if (skill.comboSkillSourceID != 0)
            sourceId = skill.comboSkillSourceID;
        if (skill.skillData.IsAttackCombo == 1)
            sourceId = actor.GetEntityData().normalAttackID;
        return sourceId;
    }
    public static bool IsRaidBattle()
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null)
            return false;
        else
            return BattleMain.battleType == BattleType.RaidPVE;
#else
        return false;
#endif
    }


    /// <summary>
    /// 是否需要分担伤害(如果身上有圣光守护的罩子 并且此次伤害是物理伤害 则伤害由圣光守护去承担 返回False)
    /// </summary>
    public static bool NeedShareBySGSH(int hurtId, BeActor actor)
    {
        if (actor == null || actor.IsDead())
            return true;
        EffectTable hurtData = TableManager.instance.GetTableItem<EffectTable>(hurtId);
        if (hurtData == null)
            return true;
        List<BeMechanism> mechanismList = actor.MechanismList;
        bool haveMechanism = false;
        if (mechanismList != null)
        {
            for (int i = 0; i < mechanismList.Count; i++)
            {
                var mechanism = mechanismList[i] as Mechanism2017;
                if (mechanism != null)
                {
                    haveMechanism = true;
                    break;
                }
            }
        }

        if (hurtData.DamageType == EffectTable.eDamageType.PHYSIC && haveMechanism)
            return false;
        return true;
    }
    private static StringBuilder s_CachedStringBuilder = null;
    public static string Format(string format, object arg0)
    {
        if (format == null)
        {
            Logger.LogErrorFormat("format is ivalid");
            return null;
        }
        CheckCachedStringBuilder();
        s_CachedStringBuilder.Length = 0;
        s_CachedStringBuilder.AppendFormat(format, arg0);
        return s_CachedStringBuilder.ToString();
    }
    public static string Format(string format, object arg0, object arg1)
    {
        if (format == null)
        {
            Logger.LogErrorFormat("format is ivalid");
            return null;
        }
        CheckCachedStringBuilder();
        s_CachedStringBuilder.Length = 0;
        s_CachedStringBuilder.AppendFormat(format, arg0, arg1);
        return s_CachedStringBuilder.ToString();
    }
    public static string Format(string format, params object[] arg0)
    {
        if (format == null)
        {
            Logger.LogErrorFormat("format is ivalid");
            return null;
        }
        CheckCachedStringBuilder();
        s_CachedStringBuilder.Length = 0;
        s_CachedStringBuilder.AppendFormat(format, arg0);
        return s_CachedStringBuilder.ToString();
    }
    private static void CheckCachedStringBuilder()
    {
        if (s_CachedStringBuilder == null)
        {
            s_CachedStringBuilder = new StringBuilder(1024);
        }
    }

    /// <summary>
    /// 异常状态下 保存日志
    /// </summary>
    public static void SaveBattleRecord(BaseBattle battle)
    {
#if LOGIC_SERVER
        if (battle == null)
            return;
        if (battle.recordServer == null)
            return;
        if (battle.recordServer.HaveSaveRecordBattle)
            return;
        battle.recordServer.HaveSaveRecordBattle = true;
        battle.recordServer.EndRecordInBattleOnError();
#endif
    }
    
    /// <summary>
    /// 保存数据到本地文件 以末尾添加的方式
    /// </summary>
    public static void SaveDataToFile(string path, string str)
    {
        FileStream fs = null;
        string filePath = path;
        //将待写的入数据从字符串转换为字节数组  
        Encoding encoder = Encoding.UTF8;
        byte[] bytes = encoder.GetBytes(str);
        try
        {
            fs = File.OpenWrite(filePath);
            //设定书写的开始位置为文件的末尾  
            fs.Position = fs.Length;
            //将待写入内容追加到文件末尾  
            fs.Write(bytes, 0, bytes.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine("文件打开失败{0}", ex.ToString());
        }
        finally
        {
            if (null != fs)
            {
                fs.Close();
            }
        }
    }
    /// <summary>
    /// 检查地下城是否是堕落深渊或者堕落深渊（无消耗）
    /// </summary>
    /// <returns></returns>
    public static bool CheckDungeonIsLimitTimeHell()
    {
        if (BattleMain.instance != null && BattleMain.instance.GetDungeonManager() != null &&
            BattleMain.instance.GetDungeonManager().GetDungeonDataManager() != null &&
            BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table != null &&
           (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL ||
            BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL))
        {
            return true;
        }

        return false;
    }

    public static bool IsActorUseCanChargeSkill(BeActor actor)
    {
        if (actor != null && actor.IsCastingSkill())
        {
            var skillData = actor.GetSkillActionInfo(actor.GetCurSkillID());
            if (skillData != null && skillData.useCharge)
            {
                return true;
            }
        }
        return false;
    }
    
    //获取玩家数量
    public static int GetPlayerCount()
    {
        if (BattleMain.instance == null || BattleMain.instance.GetPlayerManager() == null)
            return 0;
        if (BattleMain.IsModePvP(BattleMain.battleType))
            return 0;
        return BattleMain.instance.GetPlayerManager().GetPlayerCount();
    }


    public static uint GetByte(UInt32 data, int start, int end)
    {
        uint ret = data;
        ret = ret << (32 - end);
        ret = ret >> start + (32 - end);
        return ret;
    }

    public static void ResetIntArray(int[] array)
    {
        if (array == null)
            return;
        
        for(int i=0; i<array.Length; ++i)
            array[i] = 0;
    }
    
    /// <summary>
    /// 寻找目标优先级
    /// </summary>
    public enum TargetPriority
    {
        None = 0,
        Normal,     //普通
        Elite,      //精英
        Boss,       //Boss
        Player,     //玩家PK模式用
    }
    /// <summary>
    /// 获取目标的优先级
    /// </summary>
    public static TargetPriority GetActorPriority(BeActor actor)
    {
        if (!actor.IsMonster())
            return TargetPriority.Player;
        if (actor.IsBoss())
            return TargetPriority.Boss;
        if (actor.GetEntityData().monsterData.Type == ProtoTable.UnitTable.eType.ELITE || actor.GetEntityData().monsterData.Type == ProtoTable.UnitTable.eType.HELL) 
            return TargetPriority.Elite;
        return TargetPriority.Normal;
    }

    /// <summary>
    /// 转职体验和转职试炼入口
    /// </summary>
    /// <param name="occuId">职业ID</param>
    /// <param name="isExpBattle">是否转职试炼</param>
    public static void StartChangeOccuBattle(int occuId, bool isExpBattle = false)
    {
        //if (BattleDataManager.GetInstance().mChangOccuBattleBackMapId > 0)
        {
            //return;
        }

        var jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(occuId);
        if (jobTable == null || jobTable.Open == 0 || jobTable.prejob == 0 || jobTable.ID % 10 == 0)
        {
            return;
        }

        BattleFactory.PrepareChangeOccuBattle((byte)occuId);
        BattleMain.OpenBattle(BattleType.ChangeOccu, eDungeonMode.LocalFrame, 150, "1000");
        ClientSystemManager.GetInstance().SwitchSystem<ClientSystemBattle>();

        //GameSystemTown town = Tenmove.Runtime.Client.Unity.Environment.Instance.CurrentGameSystem as GameSystemTown;
        //if (town != null)
        //{
        //    var baseTown = Client.Unity.Environment.Instance.Town;
        //    if (baseTown != null)
        //    {
        //        BattleDataManager.Instance.mChangOccuBattleBackMapId = (int)baseTown.CurrnetSceneID;
        //    }
        //    var client = BattleFactory.CreateBattleClient(
        //            Game.eDungeonMode.LocalFrame, isExpBattle ? Global.GLOBAL_CHANGE_OCCU_EXP_BATTLE_ID : Global.GLOBAL_CHANGE_OCCU_BATTLE_ID,
        //            null, new Game.TMDemoPlayers(Client.Unity.PlayerBaseData.Instance.Name, occuId, Global.Settings.mChangeOccuBattlePlayerLevel),
        //            TMDungeonCache.instance);

        //    ITMProgressAccessor progress = town.EnterSystem<GameSystemBattle>(false, client);
        //    LoadingParam param = new LoadingParam();
        //    param.progress = progress;
        //    param.mapType = ProtoTable.LoadingTable.eType.COMMON;
        //    param.num = 0;
        //    UI.UIManager.Instance.OpenFrame<LoadingFrame>(new UI.UIParam<LoadingParam>(param), UI.UIGroupLayer.Top);
        //}
    }
}
