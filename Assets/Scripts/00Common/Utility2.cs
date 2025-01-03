using UnityEngine;
using ProtoTable;
using GameClient;
using System.Collections.Generic;
using Protocol;
using System;

public partial class Utility {

	public static string GetWriteablePath()
	{
		string path = null;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			path = GetPersistentDataPath() + "/";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			path = GetPersistentDataPath() + "/";
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			path = Application.dataPath + "/";
		}
		else
		{
			path = Application.dataPath + "/";
		}

		return path;
	}

	public static string GetPersistentDataPath()
	{
		//         if (Application.platform == RuntimePlatform.Android)
		//             return "file://" + Application.persistentDataPath;
		//         else if (Application.platform == RuntimePlatform.IPhonePlayer)
		//             return "file://" + Application.persistentDataPath;
		//         else if (Application.platform == RuntimePlatform.WindowsEditor)
		//             return "file:///" + Application.persistentDataPath;
		//         else
		//             return "file://" + Application.persistentDataPath;

		return Application.persistentDataPath;
	}

	public static string GetItemModulePath(int itemID)
	{
		string path = null;
		
		var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
		if (itemData != null)
		{
			var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(itemData.ResID);

			if(resData != null)
				path = resData.ModelPath;
		}

		return path;
	}

    public static bool CheckTeamEnterGuildDungeon()
    {
        if(GuildDataManager.GetInstance().GetGuildDungeonActivityStatus() != GuildDungeonStatus.GUILD_DUNGEON_START)
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildDungeonNotOpenTip"));
            return false;
        }
        if (!GuildDataManager.GetInstance().HasSelfGuild())
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("enterGuildDungeonLimitTip1"));
            return false;
        }
        if (GuildDataManager.GetInstance().myGuild.nLevel < GuildDataManager.GetGuildDungeonActivityGuildLvLimit())
        {
            SystemNotifyManager.SystemNotify((int)ProtoErrorCode.GUILD_DUNGEON_MIN_LEVEL);
            return false;
        }
        if (PlayerBaseData.GetInstance().Level < GuildDataManager.GetGuildDungeonActivityPlayerLvLimit())
        {
            SystemNotifyManager.SystemNotify((int)ProtoErrorCode.GUILD_DUNGEON_PLAYER_LEVEL_LIMIT);
            return false;
        }
        if (!Utility.CheckSameGuildInTeam())
        {
            return false;
        }
        if (!Utility.CheckMemberLvInTeam())
        {
            return false;
        }        
        return true;
    }
    public static bool CheckTeamEnterDungeonCondition(int iTeamDungeonTableID)
    {
        if (Global.Settings.CloseTeamCondition)
        {
            return true;
        }

        TeamDungeonTable tabledata = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(iTeamDungeonTableID);
        if (tabledata == null)
        {
            return false;
        }

        int NotifyID = 0;
        if(!CheckTeamCondition(iTeamDungeonTableID, ref NotifyID))
        {
            if(NotifyID == 1008)
            {
                SystemNotifyManager.SystemNotify(1108);
            }
            else if(NotifyID == 1107)
            {
                SystemNotifyManager.SystemNotify(1109);
            }
            //活动堕落深渊结束的提示
            else if (NotifyID == 900014)
            {
                SystemNotifyManager.SystemNotify(900014);
            }

            return false;
        }

        return true;
    }    

    public static bool CheckJoinTeamCondition(ulong teamLeaderID, ref int NotifyID)
    {
        if (Global.Settings.CloseTeamCondition)
        {
            return true;
        }

        List<Team> teamDatas = TeamDataManager.GetInstance().GetTeamList();

        if (teamDatas == null || teamDatas.Count < 1)
        {
            return false;
        }

        for (int i = 0; i < teamDatas.Count; ++i)
        {
            Team data = teamDatas[i];

            if (data.leaderInfo.id != teamLeaderID)
            {
                continue;
            }

            // 没有目标的队伍不再判断条件是否符合
            if (TeamDataManager.GetInstance().TeamDungeonID == 1)
            {
                return true;
            }

            if(!CheckTeamCondition((int)data.teamDungeonID, ref NotifyID))
            {
                return false;
            }

            break;
        }

        return true;
    }

    public static bool CheckTeamCondition(int iTeamDungeonTableID, ref int NotifyID)
    {
        if (Global.Settings.CloseTeamCondition)
        {
            return true;
        }

        TeamDungeonTable table = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(iTeamDungeonTableID);
        if (table == null)
        {
            return false;
        }

        // 角色等级
        if (table.MinLevel > PlayerBaseData.GetInstance().Level)
        {
            NotifyID = 1008;
            return false;
        }

        //如果是怪物攻城，只需要判断一下等级就可以了
        if (table.Type == TeamDungeonTable.eType.CityMonster)
            return true;

        DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(table.DungeonID);
        if (dungeonTable == null)
        {
            Logger.LogErrorFormat("===当前地下城Id不存在===地下城名称:{0}===组队地下城表Id:{1}===", table.Name, iTeamDungeonTableID);
            return true;
        }

        if(dungeonTable != null && dungeonTable.SubType == DungeonTable.eSubType.S_GUILD_DUNGEON)
        {
            return true;
        } 
        //如多是堕落深渊 判断活动是否开启 活动开启返回true 未开启返回false
        else if (dungeonTable != null && dungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL || dungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL)
        {
            OpActivityData mData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMIT_TIME_HELL);
            if (mData != null)
            {
                if (mData.state == (int)OpActivityState.OAS_IN)
                {
                    return true;
                }
                else
                {
                    NotifyID = 900014;
                    return false;
                }
            }
            else
            {
                //堕落深渊活动结束
                NotifyID = 900014;
                return false;
            }
        }
        else if (dungeonTable != null && dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY)
        {
            //周常深渊入口，如果前置任务已经结束，则可以显示周常深渊入口，否则不显示
            if (DungeonUtility.GetWeekHellPreTaskState(dungeonTable.ID) == WeekHellPreTaskState.IsFinished)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isOpen;
        int hard;

        // 关卡是否开放
        DungeonTable dungeondata = TableManager.GetInstance().GetTableItem<DungeonTable>(table.DungeonID);
        if(dungeondata == null)
        {
            return false;
        }

        if (!ChapterUtility.GetDungeonCanEnter(table.DungeonID, false, false))
        {
            NotifyID = 1107;
            return false;
        }
        return true;
    }
    public static bool CheckMemberLvInTeam()
    {
        Team myTeam = TeamDataManager.GetInstance().GetMyTeam();
        if (myTeam == null)
        {
            return false;
        }
        for (int i = 0; i < myTeam.members.Length; i++)
        {
            if (myTeam.members[i].id == 0)
            {
                continue;
            }
            if (myTeam.members[i].level < GuildDataManager.GetGuildDungeonActivityPlayerLvLimit())
            {
                string name = myTeam.members[i].name;
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format(TR.Value("enterGuildDungeonLimitTip2"), name));
                return false;
            }
        }
        return true;
    }
    public static bool CheckSameGuildInTeam()
    {
        Team myTeam = TeamDataManager.GetInstance().GetMyTeam();
        if(myTeam == null)
        {
            return false;
        }
        for (int i = 0; i < myTeam.members.Length; i++)
        {
            if(myTeam.members[i].id == 0)
            {
                continue;
            }
            if (!GuildDataManager.GetInstance().IsSameGuild(myTeam.members[i].guildid))
            {
                string name = myTeam.members[i].name;
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format(TR.Value("enterGuildDungeonLimitTip3"), name));
                return false;
            }
        }

        return true;
    } 

    public static bool CheckIsTeamDungeon(int iDungeonID, ref int TeamDungeonTableID)
    {
        if(Global.Settings.CloseTeamCondition)
        {
            return true;
        }

        var TeamDungeonTableData = TableManager.GetInstance().GetTable<TeamDungeonTable>();
        var datas = TeamDungeonTableData.GetEnumerator();

        while(datas.MoveNext())
        {
            TeamDungeonTable data = datas.Current.Value as TeamDungeonTable;

            if(data.DungeonID == iDungeonID)
            {
                TeamDungeonTableID = data.ID;
                return true;
            }
        }

        return false;
    }

    public static List<int> GetTeamDungeonMenuFliterList(ref Dictionary<int, List<int>> SecFliterDict)
    {
        List<int> Fliter = new List<int>();

        List<int> OriList = TableManager.GetInstance().GetTeamDungeonFirstMenuList();

        if (OriList == null)
        {
            return Fliter;
        }

        int iNotifyid = 0;
        for(int i = 0; i < OriList.Count; i++)
        {
            TeamDungeonTable tabledata = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(OriList[i]);
            if (tabledata == null)
            {
               continue;
            }

            // 没有目标的队伍不再判断条件是否符合
            if (tabledata.ID == 1)
            {
                Fliter.Add(tabledata.ID);
                continue;
            }

            ////攻城怪物，直接添加一级菜单和二级菜单
            //if (tabledata.FightType == TeamDungeonTable.eFightType.AttackCityMonster)
            //{
            //    //查找二级菜单
            //    List<int> secondTabsList;
            //    var secondTabsDic = TableManager.GetInstance().GetTeamDungeonSecondMenuDict();
            //    //二级菜单没有找到
            //    if (!secondTabsDic.TryGetValue(tabledata.ID, out secondTabsList))
            //        continue;

            //    if(secondTabsList == null || secondTabsList.Count <= 0)
            //        continue;

            //    //添加一级菜单和二级菜单
            //    Fliter.Add(tabledata.ID);
            //    SecFliterDict.Add(tabledata.ID, secondTabsList);
                
            //    continue;
            //}

            List<int> FliterSecList = new List<int>();
            List<int> OriSecList = new List<int>();

            Dictionary<int, List<int>> OriSecDict = TableManager.GetInstance().GetTeamDungeonSecondMenuDict();
            if(!OriSecDict.TryGetValue(OriList[i], out OriSecList))
            {
                continue;
            }

            bool bHasOpenDungeon = false;
            for(int j = 0; j < OriSecList.Count; j++)
            {
                TeamDungeonTable data = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(OriSecList[j]);
                if (data == null)
                {
                    continue;
                }

                if(false == CheckTeamCondition(OriSecList[j], ref iNotifyid))
                {
                    continue;
                }

                FliterSecList.Add(data.ID);

                bHasOpenDungeon = true;
            }

            if(bHasOpenDungeon)
            {
                Fliter.Add(tabledata.ID);
                SecFliterDict.Add(tabledata.ID, FliterSecList);
            }
        }

        return Fliter;
    }

    public static void CalTeamFirstAndSecondMenuIndex(int iTeamDungeonTableID, ref int iFirstMenuIndex , ref int iSecondMenuIndex)
    {
        if(iTeamDungeonTableID == 1)
        {
            iFirstMenuIndex = 0;
            iSecondMenuIndex = -1;

            return;
        }

        Dictionary<int, List<int>> FliterSecDict = new Dictionary<int, List<int>>();
        GetTeamDungeonMenuFliterList(ref FliterSecDict);

        var emu = FliterSecDict.GetEnumerator();

        int iCount = 1;
        while (emu.MoveNext())
        {
            List<int> data = emu.Current.Value;

            bool bFind = false;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == iTeamDungeonTableID)
                {
                    iSecondMenuIndex = i;

                    bFind = true;
                    break;
                }
            }

            if (bFind)
            {
                iFirstMenuIndex = iCount;
                break;
            }

            iCount++;
        }
    }

    public static void OpenTeamFrame(int iDungeonId)
    {
        int iTeamDungeonTableID = 0;
        if (!CheckIsTeamDungeon(iDungeonId, ref iTeamDungeonTableID))
        {
            return;
        }

        int iNotifyID = 0;
        if(!CheckTeamCondition(iTeamDungeonTableID, ref iNotifyID))
        {
            SystemNotifyManager.SystemNotify(1107);
            return;
        }

        if (TeamDataManager.GetInstance().IsTeamLeader())
        {
            int teamDungeonID = iTeamDungeonTableID;
            TeamDungeonTable table = TableManager.instance.GetTableItem<TeamDungeonTable>(teamDungeonID);
            if (null != table && (TeamDungeonTable.eType.DUNGEON == table.Type
                                  || TeamDungeonTable.eType.CityMonster == table.Type))
            {
                TeamDataManager.GetInstance().ChangeTeamInfo(TeamOptionOperType.Target, teamDungeonID);
            }
        }

        TeamListFrame.TryOpenTeamListOrTeamMyFrame(iTeamDungeonTableID);
        //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle, iTeamDungeonTableID);        
    }

    public static string GetGuildPositionName(int position)
	{
		EGuildDuty pos = (EGuildDuty) GuildDataManager.GetInstance().GetClientDuty((byte)position);

		if (pos <= EGuildDuty.Invalid || pos > EGuildDuty.Leader)
			return "-";

		return TR.Value(pos.GetDescription());
	}


	public static ComItem AddItemIcon(ClientFrame frame, GameObject parent, uint itemID, int itemNum, int strengthLevel=0)
	{
		var item = frame.CreateComItem(parent);
		var itemData = ItemDataManager.CreateItemDataFromTable((int)itemID,100,strengthLevel);
		if (strengthLevel > 0)
			itemData.StrengthenLevel = strengthLevel;
		itemData.Count = itemNum;
		item.Setup(itemData, (GameObject obj, ItemData item1) => { ItemTipManager.GetInstance().ShowTip(item1);});

		return item;
	}

    public static void EnterGuildBattle()
    {
        if (GuildDataManager.GetInstance().HasSelfGuild() == false)
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
            return;
        }
        ClientSystemManager.instance.OpenFrame<GuildMainFrame>(FrameLayer.Middle,EOpenGuildMainFramData.OpenManor);
    }

    public static bool HasNewFunc()
    {
        var funcUnLock = TableManager.GetInstance().GetTable<FunctionUnLock>();
        var emu = funcUnLock.GetEnumerator();

        while (emu.MoveNext())
        {
            FunctionUnLock curItem = emu.Current.Value as FunctionUnLock;

            if (curItem != null && (curItem.bPlayAnim == 1))
            {
                if (curItem.FinishLevel == PlayerBaseData.GetInstance().Level)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static string To10K(ulong nNumber)
    {
        float f = nNumber / 10000.0f;
        int i = (int)(f * 100);
        f = (float)(i * 1.0) / 100;    
        return f.ToString("f2") + "万";
    }
    public static string ToThousandsSeparator(ulong nNumber)
    {
        if (string.IsNullOrEmpty(nNumber.ToString()))
        {
            return "";
        }

        string str = new string(nNumber.ToString().ToCharArray());
        if (str == null)
        {
            return "";
        }

        for (int i = str.Length - 3; i > 0; i -= 3)
        {
            str = str.Insert(i, ",");
        }

        return str;
    }

    public static string GetShowPrice(UInt64 uPrice, bool bUseToMillion = false)
    {
        if(uPrice < 10000)
        {
            return uPrice.ToString();
        }
        else if (uPrice >= 10000 && uPrice < 100000000)
        {
            if(bUseToMillion)
            {
                return string.Format("{0}万", uPrice / 10000.0f);
            }
            else
            {
                return uPrice.ToString();
            }           
        }
        else
        {
            float finalPrice = uPrice / 100000000.0f;
            string sfinalPrice = finalPrice.ToString("F2");

            return string.Format("{0}亿", sfinalPrice);
        }
    }

    public static bool EnterBudo()
    {
        if (!BudoManager.GetInstance().IsOpen || BudoManager.GetInstance().CanParty)
        {
            BoduInfoFrame.Open();
            return false;
        }

        if (BudoManager.GetInstance().CanAcqured)
        {
            BudoResultFrameData data = new BudoResultFrameData();
            data.bOver = true;
            data.bNeedOpenBudoInfo = true;
            BudoResultFrame.Open(data);
            return false;
        }

        BudoManager.GetInstance().GotoPvpBudo();

        return true;
    }

    public static void CalShowUplevelGiftIndex(ActiveManager.ActiveData activeData, ref int iCanReceiveIdx, ref int iUnFinishIdx)
    {
        var acts = activeData.akChildItems;

        if (acts == null)
        {
            return;
        }

        for (int i = 0; i < activeData.akChildItems.Count; i++)
        {
            if (activeData.akChildItems[i].status == (int)TaskStatus.TASK_FINISHED)
            {
                iCanReceiveIdx = i;
                break;
            }
            else if (activeData.akChildItems[i].status < (int)TaskStatus.TASK_FINISHED && iUnFinishIdx == -1)
            {
                iUnFinishIdx = i;
            }
        }
    }

    public static int GetDayOnLineTime()
    {
        int iDayOnLineTime = 0; 
        int iTempleteID = 5000;

        if (ActiveManager.GetInstance().ActiveDictionary.ContainsKey(iTempleteID))
        {
            var activeData = ActiveManager.GetInstance().ActiveDictionary[iTempleteID];
            if (activeData != null)
            {
                ActiveManager.ActiveMainUpdateKey find = null;

                for (int i = 0; i < activeData.updateMainKeys.Count; ++i)
                {
                    if (activeData.updateMainKeys[i].key == "DayOnline")
                    {
                        find = activeData.updateMainKeys[i];
                        break;
                    }
                }

                if (find == null)
                {
                    return iDayOnLineTime;
                }

                int iAccumulatedTime = ActiveManager.GetInstance().GetTemplateUpdateValue(iTempleteID, find.key);
                int iPassedTime = (int)TimeManager.GetInstance().GetServerTime() - (int)find.fRecievedTime;

                iDayOnLineTime = iAccumulatedTime + iPassedTime;
            }
        }

        return iDayOnLineTime;
    }

    public static string GetJobName(int iJobID, int iAwakeState = 0)
    {
        JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(iJobID);
        if (jobData == null)
        {
            return "";
        }

        if (jobData.JobType == 0)
        {
            return jobData.Name;
        }

        if (iAwakeState == 0)
        {
            return jobData.Name;
        }

        if ((iAwakeState - 1) >= 0 && (iAwakeState - 1) < jobData.AwakenJobName.Count)
        {
            return jobData.AwakenJobName[iAwakeState - 1];
        }

        return jobData.Name;
    }

    public static int GetLeftLimitNum(MallItemInfo ItemInfo, ref bool bIsDailyLimit)
    {
        int iLeftLimitNum = 0;

        // 普通商品和礼包的限购次数所使用的协议字段虽然统一了,但是剩余次数的计算方式还是各用各的
        if(ItemInfo.gift == 1)
        {
            if (ItemInfo.limitnum >= 0 && ItemInfo.limitnum < ushort.MaxValue)
            {
                iLeftLimitNum = ItemInfo.limitnum;
                bIsDailyLimit = true;
            }
            else if (ItemInfo.limittotalnum >= 0 && ItemInfo.limittotalnum < ushort.MaxValue)
            {
                iLeftLimitNum = ItemInfo.limittotalnum;
            }
        }
        else
        {
            if(ItemInfo.limitnum > 0 && ItemInfo.limitnum < ushort.MaxValue)
            {
                iLeftLimitNum = ItemInfo.limitnum - CountDataManager.GetInstance().GetCount(ItemInfo.id.ToString());
                bIsDailyLimit = true;
            }
            else if(ItemInfo.limittotalnum > 0 && ItemInfo.limittotalnum < ushort.MaxValue)
            {
                iLeftLimitNum = ItemInfo.limittotalnum - CountDataManager.GetInstance().GetCount(ItemInfo.id.ToString());
            }
        }

        return iLeftLimitNum;
    }

    public static int GetBaseJobID(int JobID)
    {
        var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(JobID);
        if (jobTable != null)
        {
            if (jobTable.JobType==0)
            {
                return JobID;
            }

            if (jobTable.JobType==1)
            {
                return jobTable.prejob;
            }
        }
        return 0;
    }

    //如果本身是小职业，则直接返回；如果是基础职业，则找到第一个小职业
    public static int GetBetterJobId(int jobId)
    {
        var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(jobId);
        if (jobTable == null)
            return 0;

        //小职业
        if (jobTable.JobType == 1)
            return jobId;

        var jobTables = TableManager.GetInstance().GetTable<ProtoTable.JobTable>();
        if (jobTables != null)
        {
            var iter = jobTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var current = iter.Current.Value as ProtoTable.JobTable;
                //专职后的职业
                if (current != null
                    && current.Open == 1
                    && current.JobType == 1
                    && current.prejob == jobId)
                    return current.ID;
            }
        }

        return 0;
    }

#region Get ItemData By Format :  ID_Count
    public static ItemData GetItemDataFromTableWithIdCount(string itemInfo_Id_Count)
    {
        if (string.IsNullOrEmpty(itemInfo_Id_Count))
        {
            return null;
        }
        string[] itemInfo = itemInfo_Id_Count.Split('_');
        if (itemInfo == null || itemInfo.Length != 2)
        {
            Logger.LogErrorFormat("[Utility] - GetItemDataFromTableWithIdCount itemInfo format is error !");
            return null;
        }
        int itemId = 0;
        int itemCount = 0;
        ItemData itemData = null;
        if(int.TryParse(itemInfo[0], out itemId) && int.TryParse(itemInfo[1], out itemCount))
        {
            itemData= ItemDataManager.CreateItemDataFromTable(itemId);
            if (itemData != null)
            {
                itemData.Count = itemCount;
            }
        }
        return itemData;
    }

    public static ItemSimpleData GetItemSimpleDataFromTableWithIdCount(string itemInfo_Id_Count)
    {
        if (string.IsNullOrEmpty(itemInfo_Id_Count))
        {
            return null;
        }
        string[] itemInfo = itemInfo_Id_Count.Split('_');
        if (itemInfo == null || itemInfo.Length != 2)
        {
            Logger.LogErrorFormat("[Utility] - GetItemDataFromTableWithIdCount itemInfo format is error !");
            return null;
        }
        int itemId = 0;
        int itemCount = 0;
        ItemSimpleData itemData = null;
        if (int.TryParse(itemInfo[0], out itemId) && int.TryParse(itemInfo[1], out itemCount))
        {
            string name = ItemDataManager.GetInstance().GetOwnedItemName(itemId);
            if (string.IsNullOrEmpty(name))
            {
                return itemData;
            }
            itemData = new ItemSimpleData(itemId, itemCount);
            itemData.Name = name;
        }
        return itemData;
    }
#endregion
    /// <summary>
    /// 获取 01  02 ... 按顺序的 格式字
    /// 注 不包括 00 
    /// </summary>
    /// <returns></returns>
    public static string GetUnitNumWithHeadZero(int num, bool startZero)
    {
        int i = num;
        if (startZero)
        {
            i += 1;
        }
        string i_str = i.ToString();
        if (i < 10)
        {
            i_str = string.Format("0{0}", i_str);
        }
        return i_str;
    }

    /// <summary>
    /// 判断 根据背包类型获取对应类型背包的空格子是否用完
    /// </summary>
    /// <param name="ePackageType">背包类型</param>
    /// <returns></returns>
    public static bool CheckPackageGridFullByPackageType(EPackageType ePackageType)
    {
        bool bFull = false;

        int ownedPackageTypeItemCount = 0;
        int totalPackageTypeGridCount = 0;
        int packageTypeIndex = (int)ePackageType;

        List<ulong> itemGUIDs = ItemDataManager.GetInstance().GetItemsByPackageType(ePackageType);
        if (itemGUIDs != null)
        {
            ownedPackageTypeItemCount = itemGUIDs.Count;
        }
        List<int> totalPackageGrids = PlayerBaseData.GetInstance().PackTotalSize;
        if (totalPackageGrids != null && totalPackageGrids.Count > packageTypeIndex)
        {
            totalPackageTypeGridCount = totalPackageGrids[packageTypeIndex];
        }

        if (ownedPackageTypeItemCount >= totalPackageTypeGridCount)
        {
            bFull = true;
        }
        return bFull;
    }

    /// <summary>
    /// 根据角色ID获取角色ICON
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public static string GetRoleIconByRoleId(int jobTableId)
    {
        JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(jobTableId);
        if (jobData != null)
        {
            ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
            if (resData != null)
            {
                return resData.IconPath;
            }
        }
        return string.Empty;
    }


    /// <summary>
    /// 通过buff表得到对应的特效路径
    /// </summary>
    /// <param name="buffTable"></param>
    /// /// <param name="type 0表示出生特效 1表示中间特效  2表示结束特效 对应buff表里面的三个特效"></param>
    /// <returns></returns>
    public static string GetEffectPathByBuffTable(BuffTable buffTable,int type)
    {
        if (buffTable == null)
        {
            Logger.LogError("buffTable is null");
            return string.Empty;
        }

        int effectId = 0;
        if (type == 0)
        {
            effectId = buffTable.BirthEffectInfoID;
        }else if (type == 1)
        {
            effectId = buffTable.EffectInfoID; 
        }else if (type == 2)
        {
            effectId = buffTable.EndEffectInfoID;
        }
        else
        {
            Logger.LogError("填写的type要在0和2之间（左右闭区间）");
        }
        if (effectId > 0)
        {
            EffectInfoTable effectInfoTable =
                TableManager.GetInstance().GetTableItem<EffectInfoTable>(effectId);
            if (effectInfoTable == null)
            {
                Logger.LogFormat("buffTable里面填写的EffectInfoId={0}在bufftable里面不存在:",effectId);
                return String.Empty;
            }
            else
            {
                return effectInfoTable.Path;
            }
        }else
        {
            return buffTable.EffectName;
        }
    }
}
