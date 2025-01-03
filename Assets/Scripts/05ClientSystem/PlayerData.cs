using System;
using System.Collections.Generic;
using Protocol;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Network;
using ProtoTable;

namespace GameClient
{
    public enum ActorOccupation
    {
        [Description("战士")]
        SwordMan = 10,

        [Description("飞影")]
        SwordSoulMan = 11,

        [Description("阵魔")]
        Zhengui = 13,

        [Description("明王")]
        AXiuLuo = 14,

        [Description("双剑士")]
        ShuangJianShi = 15,

        [Description("浪人")]
        Langren = 17,

        [Description("法师")]
        MagicMan = 30,

        [Description("枪手")]
        GunMan = 20,

        [Description("苍穹之影")]
        SkyGunMan = 21,

        [Description("武器专家")]
        Wuqizhuanjia = 24,

        [Description("弑神")]
        KuangZhan = 12,

        [Description("爆灭者")]
        QiangPaoShi = 22,

        [Description("指挥官")]
        LinYunZhiYi = 23,

        [Description("龙纹法师")]
        Zhandoufashi = 32,

        [Description("驭灵法师")]
        Zhaohuanshi = 33,
        
        [Description("炼金法师")]
        YaoJiShi = 34,

        [Description("秘术法师")]
        Mishushi = 31,

        [Description("武术师")]
        Gedoujia = 40,

        [Description("念武者")]
        Qigongshi = 41,

        [Description("武斗家")]
        Sanda = 42,

        [Description("女枪手")]
        Gungirl = 50,

        [Description("乱舞者")]
        SkyGungirl = 51,

        [Description("爆烈玫瑰")]
        QiangPaoShiGungirl = 52,

        [Description("神谕者")]
        Paladin = 60,

        [Description("猎魔人")]
        Qumoshi = 61,

        [Description("光明使者")]
        Shengqishi = 62,

        [Description("圣徒")]
        Saint = 63,
    }

    // 纯纯的数据
    public sealed class PlayerBaseData : DataManager<PlayerBaseData>
    {

        private MenuData m_currentMenuData;
        internal MenuData CurrentMenuData
        {
            get
            {
                return m_currentMenuData;
            }

            set
            {
                m_currentMenuData = value;
            }
        }

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.PlayerBaseData;
        }

        bool m_bNotify = true;
        public bool isNotify { get { return m_bNotify; } set { m_bNotify = value; } }

        bool m_bIsWear = false;
        public bool isWear { get { return m_bIsWear; } set { m_bIsWear = value; } }

        bool m_bIsExpand = false;
        public bool IsExpand { get { return m_bIsExpand; } set { m_bIsExpand = value; } }

        private bool m_bIsSelectedPerfectWashingRollTab = false;
        private BeFightBuffManager buffMgr = new BeFightBuffManager();
        public BeFightBuffManager BuffMgr { get{return buffMgr;} }
        public bool IsSelectedPerfectWashingRollTab
        {
            get
            {
                return m_bIsSelectedPerfectWashingRollTab;
            }
            set
            {
                m_bIsSelectedPerfectWashingRollTab = value;
            }
        }

        void InitMainPlayerData()
        {
            // TODO 待整理 把所有的账号信息整合到一起 AccountData
            // 登录的时候把一些基础数据（角色ID）设置到 PlayerData
            RoleInfo roleInfo = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();
            if (roleInfo == null)
            {
                Logger.LogError("_OnSyncPlayerMain ==>> roleInfo is null!");
            }

            RoleID = roleInfo.roleId;
        }
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

        // 是否转职了
        public static bool IsJobChanged()
        {
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobTable == null)
            {
                return false;
            }
            if (jobTable.JobType == 1)
            {
                return true;
            }
            return false;
        }
        public static bool IsWeapon(ProtoTable.ItemTable.eSubType eSubType)
        {
            return eSubType == ItemTable.eSubType.WEAPON;
        }

        public override void Initialize()
        {
            InitMainPlayerData();
            NetProcess.AddMsgHandler(SceneSynSelf.MsgID, _OnSyncSelfObject);
            m_bNotify = true;
            m_bIsWear = false;
            m_bIsExpand = false;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;

            AllGeAvatarCanUseLayers = new int[] { 20, 21, 22, 23, 24, 25, 26, 27, 28 };
            if(UsingLayers != null)
            {
                UsingLayers.Clear();
            }
        }

        public override void Clear()
        {
            NetProcess.RemoveMsgHandler(SceneSynSelf.MsgID, _OnSyncSelfObject);

            AxisScale = 10000.0f;
            bInitializeTownSystem = false;
            RoleID = 0;
            ZoneID = -1;
            Name = "";
            Level = 0;
            Exp = 0;
            CurExp = 0;
            IsFlyUpState = false;
            Sex = 0;
            HP = 0;
            MaxHP = 0;
            chiji_hp = 0;
            chiji_mp = 0;
            Pos = Vector3.zero;
            FaceRight = false;
            MoveSpeedRate = 0f;
            Gold = 0;
            BindGold = 0;
            Ticket = 0;
            BindTicket = 0;
            GoldJarScore = 0;
            MagicJarScore = 0;
            MonthCardLv = 0;
            VipLevel = -1;
            CurVipLvRmb = 0;
            AliveCoin = 0;
            WarriorSoul = 0;
            pkPoints = 0;
            pkMatchScore = 0;
            uiPkCoin = 0;
            fatigue = 0;
            MaxFatigue = 0;
            PackBaseSize = 0;
            FashionPackBaseSize = 0;
            TitlePackBaseSize = 0;
            PackAddSize.Clear();
            PackTotalSize.Clear();
            for (int i = 0; i < (int)EPackageType.Count; i++)
            {
                PackTotalSize.Add(0);
            }
            AccountStorageSize = 50;
            _ChijiScore = 0;
            iTittle = 0;
            _achievementScore = 0;
            _accountAchievementScore = 0;
            _roleAchievementScore = 0;
            isRoleEnterGame = false;
            _dayChargeNum = 0;
            AwakeState = -1;
            bNeedShowAwakeFrame = false;
            NextUnlockFunc.Clear();
            SP = 0;
            SP2 = 0;
            pvpSP2 = 0;
            pvpSP = 0;
            m_jobTableID = Global.Settings.iSingleCharactorID;
            PreChangeJobTableID = 0;
            RoleCreateTime = 0;
            eChangeJobState = ChangeJobState.JobState_Null;
            DeathTowerWipeoutEndTime = 0;
            bIsForceGuide = false;
            bIsWeakGuide = false;
            bIsInitNewbieGuideData = true;
            GuideFinishMission = 0;

            ResistMagicValue = 0;
            avatar = null;              //清空角色的Avatar数据
            isShowFashionWeapon = false;
            //             PkEndData = new SceneMatchPkRaceEnd();
            bPkClickVipCharge = false;
            NewbieGuideSaveBoot = 0;
            NewbieGuideCurSaveOrder = 0;
            NewbieGuideWeakGuideList.Clear();
            AuctionLastRefreshTime = 0;
            AddAuctionFieldsNum = 0;
            GoodTeacherValue = 0;
            TotalEquipScore = 0;

            if (UnlockFuncList != null)
            {
                UnlockFuncList.Clear();
            }

            if (NewUnlockFuncList != null)
            {
                NewUnlockFuncList.Clear();
            }

            if(NewUnlockNotPlayFuncList != null)
            {
                NewUnlockNotPlayFuncList.Clear();
            }

            if(UnlockAreaList != null)
            {
                UnlockAreaList.Clear();
            }

            if(m_activeJobTableIDs != null)
            {
                m_activeJobTableIDs.Clear();
            }
            
            if(buffList != null)
            {
                buffList.Clear();
            }      
            buffList = new List<Battle.DungeonBuff>();

            if(removedBuffList != null)
            {
                removedBuffList.Clear();
            }
            removedBuffList = new List<Battle.DungeonBuff>();

            if(mails != null)
            {
                mails.ClearData();
            }
            mails = new GameMailData();

            BuffMgr.Clear();
            if(PkStatisticsData != null)
            {
                PkStatisticsData.Clear();
            }
            PkStatisticsData = new Dictionary<byte, PkStatistic>();

            if(MallItemData != null)
            {
                MallItemData.Clear();
            }

            guildName = "";
            eGuildDuty = EGuildDuty.Invalid;
            guildDuty = 0;
            guildContribution = 0;
            guildBattleTimes = 0;
            guildBattleScore = 0;
            guildBattleRewardMask = new GuildBattleMaskProperty();

            MissionScore = 0;
            BudoStatus = 0;

            if (null != potionSets)
            {
                potionSets.Clear();
            }
            tmpPostionSets.Clear();

            Announcement = string.Empty;
            getPupil = true;
            m_bNotify = true;
            m_bIsWear = false;
            m_bIsExpand = false;
            appointmentOccu = 0;
            WeaponLeaseTicket = 0;
            IntergralMallTicket = 0;
            gameOptions = 0;
            adventureCoin = 0;
            ChanllengeScore = 0;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;

            AllGeAvatarCanUseLayers = new int[] { 20, 21, 22, 23, 24, 25, 26, 27, 28 };
            if (UsingLayers != null)
            {
                UsingLayers.Clear();
            }

            ChangeJobSelectFrame.changeType = ChangeJobType.ChangeJobMission;
        }

        public override void OnApplicationStart()
        {    
            FileArchiveAccessor.LoadFileInPersistentFileArchive(m_kSavePath, out jsonText);
            if (jsonText == null) // 不存在配置
            {
                FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, "");
                jsonText = "";  
            }
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data != null)
            {
                // 处理默认配置
                if (!data.ContainsKey(PotionSlotType.SlotMain.ToString()))
                {
                    SetPotionPercent(PotionSlotType.SlotMain, defaultPotionPercent, true);
                }
                if (!data.ContainsKey(PotionSlotType.SlotExtend1.ToString()))
                {
                    SetPotionPercent(PotionSlotType.SlotExtend1, defaultPotionPercent, true);
                }
                if (!data.ContainsKey(PotionSlotType.SlotExtend2.ToString()))
                {
                    SetPotionPercent(PotionSlotType.SlotExtend2, defaultPotionPercent, true);
                }
                if (!data.ContainsKey(potionSlotMainSwitchKeyName))
                {
                    SetPotionSlotMainSwitchOn(false, true);
                }
            }
            return;
        }

        public void ClearChijiData()
        {
            SP = 0;
            pvpSP = 0;
        }

        private void OnCountValueChanged(UIEvent uiEvent)
        {
            AppoinmentCoin = (uint)CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_ACTIVITY_COIN_NUM);
        }

        #region base
        public void CheckNameValid(ulong a_guid, string a_strName)
        {
            SceneCheckChangeNameReq msg = new SceneCheckChangeNameReq
            {
                newName = a_strName
            };

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneCheckChangeNameRes>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("player_change_name_ask", a_strName),
                        () =>
                        {
                            ChangeName(a_guid, a_strName);
                            ClientSystemManager.GetInstance().CloseFrame<GuildCommonModifyFrame>();
                        }
                    );
                }
            });
        }

        public void ChangeName(ulong a_guid, string a_strName)
        {
            SceneChangeNameReq msg = new SceneChangeNameReq
            {
                itemUid = a_guid,
                newName = a_strName
            };

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneChangeNameRes>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("player_change_name_success"));
                    //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NameChanged);
                }
            });
        }


        //[EnterGameMessageHandle(SceneSynSelf.MsgID)]
        void _OnInitBaseData(MsgDATA msg)
        {
            _OnSyncSelfObject(msg);
        }

        void _OnSyncSelfObject(MsgDATA msg)
        {
            if (msg == null)
            {
                Logger.LogError("_OnSyncPlayerMain ==>> msg is nil");
                return;
            }

            int pos = 0;
            SceneObject msgSelfObj = SceneObjectDecoder.DecodeSelfSceneObject(msg.bytes, ref pos, msg.bytes.Length);
            for (int j = 0; j < msgSelfObj.dirtyFields.Count; ++j)
            {
                _UpdatePlayerData(msgSelfObj, msgSelfObj.dirtyFields[j]);
            }
        }

        public bool bLevelUpChange;

        private System.Collections.IEnumerator _OpenCHangeJobFinish()
        {
            yield return new WaitForSeconds(0.5f);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayChangeJobEffect);
            int time = Utility.GetClientIntValue(ClientConstValueTable.eKey.CHANGE_JOB_FINISH_DELAY_TIME, 1000);
            yield return new WaitForSeconds(((float)time)/1000f);
            ClientSystemManager.GetInstance().OpenFrame<ChangeJobFinish>(FrameLayer.Middle);
        }

        void _UpdatePlayerData(SceneObject msgData, int dirtyField)
        {
            SceneObjectAttr prop = (SceneObjectAttr)dirtyField;
            switch (prop)
            {
                case SceneObjectAttr.SOA_ZONEID:
                    {
                        ZoneID = (int)msgData.zoneId;
                        break;
                    }
                case SceneObjectAttr.SOA_NAME:
                    {
                        Name = msgData.name;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NameChanged);
                        TitleComponent.OnChangeName(0, PlayerBaseData.GetInstance().Name);
                        break;
                    }
                case SceneObjectAttr.SOA_COUNTER:
                    {
                        var iter = msgData.counterMgr.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            CountDataManager.GetInstance().SetCount(iter.Current.Value.name, iter.Current.Value.value);
                        }

                        AppoinmentCoin = (uint)CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_ACTIVITY_COIN_NUM);

                        RedPointDataManager.GetInstance().NotifyRedPointChanged();
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CounterChanged);
                    }
                    break;
                case SceneObjectAttr.SOA_LEVEL:
                    {
                        bool IsInit = false;
                        if (Level == 0)
                        {
                            IsInit = true;
                        }

                        int iPreLevel = Level;
                        Level = msgData.level;

                        CurExp = 0;
                        Exp = 0;
                        CalRoleTotalExp();

                        ItemDataManager.GetInstance().OnLevelChanged(iPreLevel);

                        if (!IsInit)
                        {
                            SkillDataManager.GetInstance().UpdateNewSkill();
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.LevelChanged);
                        GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.LevelChanged);

                        TitleComponent.OnChangedLv(0, Level);

                        ClientSystemGameBattle systemChiji = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;

                        if (!IsInit && systemChiji == null)
                        {
                            bLevelUpChange = true;
                        }

                        break;
                    }
                case SceneObjectAttr.SOA_EXP:
                    {
                        CurExp = msgData.exp;
                        Exp = 0;

                        CalRoleTotalExp();

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ExpChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_SEX:
                    {
                        Sex = msgData.sex;
                        break;
                    }
                case SceneObjectAttr.SOA_OCCU:
                    {
                        // 是否是转职
                        bool bIsChangeJob = false;

                        // 在吃鸡场景里不需要判断是否是转职
                        ClientSystemGameBattle chijiSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;

                        if (chijiSystem == null)
                        {
                            if(JobTableID != msgData.occu)
                            {
                                bIsChangeJob = true;
                            }
                        }

                        JobTableID = msgData.occu;
                        
                        if (bIsChangeJob)
                        {
                            SkillDataManager.GetInstance().LastSeeSkillLv = 1;
                        }

                        SkillDataManager.GetInstance().InitSkillData(msgData.level);

                        if(ChangeJobSelectFrame.changeType == ChangeJobType.SwitchJob)
                        {
                            GameFrameWork.instance.StartCoroutine(_OpenCHangeJobFinish());
                            // ClientSystemManager.GetInstance().OpenFrame<ChangeJobFinish>(FrameLayer.Middle);
                        }

                        if (bIsChangeJob)
                        {
                            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.JobIDChanged); // 转职
                        }
                        else
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.JobIDReset); // 非转职（吃鸡选择职业，更改职业id）
                        }

                        ItemDataManager.GetInstance().OnJobChanged();

                        break;
                    }
                case SceneObjectAttr.SOA_PRE_OCCU:
                    {
                        PreChangeJobTableID = msgData.preOccu;
                        break;
                    }
                case SceneObjectAttr.SOA_GUILD_BATTLE_LOTTERY_STATUS:
                    {
                        GuildDataManager.GetInstance().SetLotteryState(msgData.guildBattleLotteryStatus);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildLotteryResultRes);

                        break;
                    }
                case SceneObjectAttr.SOA_HP:
                    {
                        HP = (int)msgData.hp;
                        break;
                    }
                case SceneObjectAttr.SOA_MAXHP:
                    {
                        MaxHP = (int)msgData.maxHp;
                        break;
                    }
                case SceneObjectAttr.SOA_CHIJI_HP:
                    {
                        if (ChijiDataManager.GetInstance().CurrentUseDrugId == 401000001 ||
                            ChijiDataManager.GetInstance().CurrentUseDrugId == 401000002)
                        {
                            if (msgData.chijiHp - chiji_hp > 0)
                            {
                                string mContent = string.Empty;
                                
                                var chijiItemTable = TableManager.GetInstance().GetTableItem<ChijiItemTable>(ChijiDataManager.GetInstance().CurrentUseDrugId);
                                if (chijiItemTable != null)
                                {
                                    int percentage = 0;
                                    var chijiBuffTable = TableManager.GetInstance().GetTableItem<ChijiBuffTable>(ChijiDataManager.GetInstance().CurrentUseDrugId);
                                    if (chijiBuffTable != null)
                                    {
                                        percentage = chijiBuffTable.param1 / 10;
                                    }
                                    
                                    mContent = TR.Value("Chiji_UseDrug", chijiItemTable.Name, percentage);
                                }

                                if (!string.IsNullOrEmpty(mContent))
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(mContent);
                                }

                                ChijiDataManager.GetInstance().CurrentUseDrugId = 0;
                            }
                        }
                        chiji_hp = msgData.chijiHp;
                        //Logger.LogErrorFormat("吃鸡HP测试----接收服务器数据，chiji_hp = {0}", chiji_hp);

                        if (chiji_hp <= 0)
                        {
                            ChijiDataManager.GetInstance().DoDead();
                            ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
                            if (systemTown != null && systemTown.MainPlayer != null)
                            {
                                systemTown.MainPlayer.SetDead();
#if MG_TEST || UNITY_EDITOR
                                Logger.LogErrorFormat("吃鸡时序测试----BattleID = {0}, MainPlayer Dead, name = {1} from [SceneObjectAttr.SOA_CHIJI_HP] ", ChijiDataManager.GetInstance().ChijiBattleID, systemTown.MainPlayer.GetPlayerName());
#endif
                            }

                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiPlayerDead);
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiHpChanged);

                        break;
                    }
                case SceneObjectAttr.SOA_CHIJI_MP:
                    {
                        chiji_mp = msgData.chijiMp;
                        //Logger.LogErrorFormat("吃鸡MP测试----接收服务器数据，chiji_mp = {0}", chiji_mp);

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiMpChanged);   
                        break;
                    }
                case SceneObjectAttr.SOA_GOLD:
                    {
                        Gold = msgData.gold;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GoldChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_BINDGOLD:
                    {
                        BindGold = msgData.bindGold;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BindGoldChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_POINT:
                    {
                        Ticket = msgData.point;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TicketChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_BINDPOINT:
                    {
                        BindTicket = msgData.bindPoint;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BindTicketChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_GOLDJAR_POINT:
                    {
                        GoldJarScore = msgData.goldJarPoint;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GoldJarScoreChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_MAGJAR_POINT:
                    {
                        MagicJarScore = msgData.magJarPoint;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MagicJarScoreChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_ALIVE_COIN:
                    {
                        AliveCoin = msgData.aliveCoin;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AliveCoinChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_FATIGUE:
                    {
                        fatigue = msgData.fatigue.fatigue;
                        MaxFatigue = msgData.fatigue.maxFatigue;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FatigueChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_SCENE_POS:
                    {
                        Pos = new Vector3(msgData.pos.x / AxisScale, 0.0f, msgData.pos.y / AxisScale);
                        break;
                    }
                case SceneObjectAttr.SOA_SCENE_DIR:
                    {
                        FaceRight = msgData.dir.faceRight >= 1;
                        break;
                    }
                case SceneObjectAttr.SOA_MOVESPEED:
                    {
                        MoveSpeedRate = msgData.moveSpeed / (float)(GlobalLogic.VALUE_1000);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MoveSpeedChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_DAILY_TASK_SOURCE:
                    {
                        MissionScore = msgData.datilyTaskScore;
                        break;
                    }
                case SceneObjectAttr.SOA_DAILY_TASK_REWARD_MASK:
                    {
                        DailyTaskMaskProperty = msgData.dailyTaskMask;
                        break;
                    }
                case SceneObjectAttr.SOA_ACHIEVEMENT_TASK_REWARD_MASK:
                    {
                        AchievementMaskProperty = msgData.achievementMask;
                        break;
                    }
                case SceneObjectAttr.SOA_WUDAO_STATUS:
                    {
                        BudoStatus = msgData.wudaoStatus;
                        break;
                    }
                case SceneObjectAttr.SOA_BUFFS:
                    {
                        //string str = string.Empty;
                        //str += string.Format("MainRole buffList Count {0} ", msgData.buffList == null ? 999999999: msgData.buffList.Count);
                        //for(int i = 0; msgData.buffList != null && i < msgData.buffList.Count;i++)
                        //{
                        //    var buff = msgData.buffList[i];
                        //    str += string.Format("[uid {0} id {1}] ", buff.id, buff.uid);
                        //}
                        //Logger.LogError(str);
                        ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
                        if (msgData.buffList != null)
                        {
                            var updateBuffList = msgData.buffList;
                            if (systemTown != null)
                            {
                                List<int> removeBuffList = new List<int>();
                                for (int i = 0; i < buffMgr.Count(); i++)
                                {
                                    var buff = buffMgr.Get(i);
                                    bool find = false;
                                    for (int j = 0; j < updateBuffList.Count; j++)
                                    {
                                        if (updateBuffList[j].uid == buff.GUID)
                                        {
                                            find = true;
                                            break;
                                        }
                                    }
                                    if (!find)
                                    {
                                        removeBuffList.Add(i);
                                    }

                                }
                                for (int i = removeBuffList.Count - 1; i >= 0; i--)
                                {
                                    var buff = buffMgr.Get(removeBuffList[i]);
                                    if (buff != null && systemTown != null && systemTown.MainPlayer != null)
                                    {
                                  //      Logger.LogErrorFormat("MainRole RemoveBuff GUID {0} buffID {1}", buff.GUID, buff.BuffID);
                                        if (buff.BuffID == 402000003)
                                        {
                                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PoisonStatChange, false);
                                        }
                                        buff.Finish(systemTown.MainPlayer.GraphicActor);
                                    }
                                    buffMgr.RemoveAt(removeBuffList[i]);
                                }
                            }
                            for (int i = 0; i < updateBuffList.Count; ++i)
                            {
                                var curBuff = updateBuffList[i];
                                var isFind = buffList.Find(buff => { return buff.uid == curBuff.uid; });
                                if (null != isFind)
                                {
                                ///    Logger.LogProcessFormat("[buffdrug] 更新buff时间 {0}", curBuff.id);
                                    // update left time
                                    isFind.lefttime = curBuff.duration;
                                }
                                else
                                {
                                    var data = TableManager.instance.GetTableItem<ProtoTable.BuffTable>(curBuff.id);
                                    if (data != null)
                                    {
                                        curBuff.type = (Battle.DungeonBuff.eBuffDurationType)data.durationType;
                                        curBuff.duration = data.duration;

                                        buffList.Add(curBuff);

                                        Logger.LogProcessFormat("[buffdrug] 添加buff {0}, 类型 {1}, 时长 {2}", curBuff.id, curBuff.type, curBuff.duration);
                                    }
                                }
                                if (systemTown != null)
                                {
                                    var townBuff = buffMgr.GetBuffByGUID(curBuff.uid);
                                    if (townBuff != null)
                                    {
                                        townBuff.Refresh(curBuff.lefttime, curBuff.overlay);
                                        continue;
                                    }
                                    var curTownBuff = buffMgr.AddBuff(curBuff.id, curBuff.uid, 0, curBuff.lefttime, curBuff.overlay);
                                    if (curBuff.id == 402000003)
                                    {
                                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PoisonStatChange, true);
                                    }
                                    if (curTownBuff != null && systemTown != null && systemTown.MainPlayer != null)
                                    {
                                        curTownBuff.Start(systemTown.MainPlayer.GraphicActor);
                                    }
                                }
                            }
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BuffListChanged);

                            //Logger.LogProcessFormat(ObjectDumper.Dump(buffList));
                        }
                        break;
                    }
                case SceneObjectAttr.SOA_SKILLS:
                    {
                        if (msgData.skillMgr != null)
                        {
                            bool bIsInChijiScene = false;
                            ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
                            if (systemTown != null)
                            {
                                bIsInChijiScene = true;
                            }
                            if(!bIsInChijiScene)
                            {
                                SkillDataManager.GetInstance().CurPVESKillPage = (ESkillPage)msgData.skillMgr.currentPage;
                                var isUnlock = SkillDataManager.GetInstance().PVESkillPage2IsUnlock;
                                if (msgData.skillMgr.pageCnt >= 2)
                                {
                                    SkillDataManager.GetInstance().PVESkillPage2IsUnlock = true;
                                }
                                else
                                {
                                    SkillDataManager.GetInstance().PVESkillPage2IsUnlock = false;
                                }
                                //状态变化 解锁了
                                if (isUnlock != SkillDataManager.GetInstance().PVESkillPage2IsUnlock)
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillPlanPageUnlock);
                                SkillDataManager.GetInstance().UpdateSkillData(msgData.skillMgr, SkillConfigType.SKILL_CONFIG_PVE);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillListChanged);
                            }
                           
                        }

                        break;
                    }
                case SceneObjectAttr.SOA_PVP_SKILLS:
                    {
                        if (msgData.pvpSkillMgr != null)
                        {
                            bool bIsInChijiScene = false;
                            ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
                            if (systemTown != null)
                            {
                                bIsInChijiScene = true;
                            }

                            if (bIsInChijiScene)
                            {
                               SkillDataManager.GetInstance().UpdateChijiSkillData(msgData.pvpSkillMgr);
                            }
                            else
                            {
                                SkillDataManager.GetInstance().CurPVPSKillPage = (ESkillPage)msgData.pvpSkillMgr.currentPage;
                                var isUnlock = SkillDataManager.GetInstance().PVPSkillPage2IsUnlock;
                                if (msgData.pvpSkillMgr.pageCnt >= 2)
                                {
                                    SkillDataManager.GetInstance().PVPSkillPage2IsUnlock = true;
                                }
                                else
                                {
                                    SkillDataManager.GetInstance().PVPSkillPage2IsUnlock = false;
                                }
                                //状态变化 解锁了
                                if (isUnlock != SkillDataManager.GetInstance().PVPSkillPage2IsUnlock)
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillPlanPageUnlock);

                                SkillDataManager.GetInstance().UpdateSkillData(msgData.pvpSkillMgr, SkillConfigType.SKILL_CONFIG_PVP);
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillListChanged);
                            }
                        }
                        break;
                    }
                case SceneObjectAttr.SOA_EQUAL_PVP_SKILLS:
                    {
                        if(msgData.equalPvpSkillMgr!=null)
                        {
                            SkillDataManager.GetInstance().UpdateFairDuelSkillData(msgData.equalPvpSkillMgr);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillListChanged);
                        }
                       
                        break;
                    }
                case SceneObjectAttr.SOA_EQUAL_PVP_SKILLBAR:
                    {
                        if(msgData.equalPvpSkillBars!=null)
                        {
                            SkillDataManager.GetInstance().UpdateFairDuelSkillBar(msgData.equalPvpSkillBars);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillBarChanged);
                        }
                        break;
                    }
                case SceneObjectAttr.SOA_EQUAL_PVP_SP:
                    {
                         FairDuelSp = msgData.equalPvpSp;
                         UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SpChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_SKILLBAR:
                    {
                        if (msgData.skillBars.bar.Length <= 0)
                        {
                            break;
                        }

                        SkillDataManager.GetInstance().UpdateSkillBar(msgData.skillBars, SkillConfigType.SKILL_CONFIG_PVE);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillBarChanged);

                        break;
                    }
                case SceneObjectAttr.SOA_PVP_SKILLBAR:
                    {
                        if (msgData.pvpSkillBars.bar.Length <= 0)
                        {
                            break;
                        }

                        bool bIsInChijiScene = false;

                        ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
                        if (systemTown != null)
                        {
                            bIsInChijiScene = true;
                        }

#if MG_TEST
                        string ss = string.Format("测试服吃鸡:收到PvpSkillBar数据,InChijiScene={0}", bIsInChijiScene);

                        if (msgData.pvpSkillBars != null && msgData.pvpSkillBars.bar != null && msgData.pvpSkillBars.bar.Length > 0 && msgData.pvpSkillBars.bar[0].grids != null)
                        {
                            for(int i = 0; i < msgData.pvpSkillBars.bar[0].grids.Length; i++)
                            {
                                if(i < 3)
                                {
                                    ss += "_";
                                    ss += msgData.pvpSkillBars.bar[0].grids[i].id.ToString();
                                }
                            }
                        }

                        ExceptionManager.GetInstance().RecordLog(ss);
                        GameStatisticManager.GetInstance().UploadLocalLogToServer(ss);
#endif

                        if (bIsInChijiScene)
                        {
                            SkillDataManager.GetInstance().UpdateChijiSkillBar(msgData.pvpSkillBars);
                        }
                        else
                        {
                            SkillDataManager.GetInstance().UpdateSkillBar(msgData.pvpSkillBars, SkillConfigType.SKILL_CONFIG_PVP);
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillBarChanged);

                        break;
                    }
                case SceneObjectAttr.SOA_SP:
                    {
                        if (msgData.sp!=null&&msgData.sp.spList!=null&& msgData.sp.spList.Length>=2)
                        {
                            SP = msgData.sp.spList[0];
                            SP2 = msgData.sp.spList[1];
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SpChanged);
                            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.Skill);
                        }
                        break;
                    }
                case SceneObjectAttr.SOA_PVP_SP:
                    {
#if UNITY_EDITOR
                        //Logger.LogErrorFormat("吃鸡时序测试----收到pvp技能sp点, pvpSP = {0}, msgData.pvpSp = {1}", pvpSP, msgData.pvpSp);
#endif
                        if (msgData.pvpSp!=null&&msgData.pvpSp.spList!=null&& msgData.pvpSp.spList.Length >= 2)
                        {
                            pvpSP = msgData.pvpSp.spList[0];
                            pvpSP2 = msgData.pvpSp.spList[1];
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SpChanged);
                            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.Skill);
                        }
                        
                        break;
                    }
                case SceneObjectAttr.SOA_ACHIEVEMENT_SCORE:
                    {
                        RoleAchievemeentScore = (int)msgData.achievementScore;
                        break;
                    }
                case SceneObjectAttr.SOA_ACCOUNT_ACHIEVEMENT_SCORE:
                    {
                        AccountAchievementScore = (int)msgData.accountAchievementScore;
                        break;
                    }
                case SceneObjectAttr.SOA_STATUS:
                    {
                        break;
                    }
                case SceneObjectAttr.SOA_PACKAGE_TYPE:
                {
                    UpdatePackSizeByType(msgData.packageTypeStr);
                    ItemDataManager.GetInstance().NotifyPackageFullState();
                    break;
                }
                case SceneObjectAttr.SOA_PACKSIZE:
                    {
                        PackBaseSize = (int)msgData.packSize;
                        PackTotalSize.Clear();

                        for (int i = 0; i < (int)EPackageType.Count; i++)
                        {
                            //时装 铭文 和称号的基础格子是单独同步，其他类型的格子使用公用的基础格子
                            var curBaseSize = PackBaseSize;
                            if ((EPackageType)i == EPackageType.Fashion || (EPackageType)i == EPackageType.Inscription)
                            {
                                curBaseSize = FashionPackBaseSize;
                            }
                            else if((EPackageType)i == EPackageType.Bxy || (EPackageType)i == EPackageType.Sinan)
                            {
                                curBaseSize = FashionPackBaseSize;
                            }
                            else if ((EPackageType)i == EPackageType.Title)
                            {
                                curBaseSize = TitlePackBaseSize;
                            }
                            PackTotalSize.Add(curBaseSize);

                            if (i < PackAddSize.Count)
                            {
                                PackTotalSize[i] += PackAddSize[i];
                            }
                        }

                        ItemDataManager.GetInstance().NotifyPackageFullState();
                        break;
                    }
                case SceneObjectAttr.SOA_PACKAGE_SIZE_ADDITION:
                    //背包加成，VIP等级增加的格子数量（所有类型的格子）
                    {
                        PackAddSize.Clear();
                        PackTotalSize.Clear();

                        for (int i = 0; i < (int)EPackageType.Count; i++)
                        {
                            //时装 铭文 和称号的基础格子是单独同步，其他类型的格子使用公用的基础格子
                            var curBaseSize = PackBaseSize;
                            if ((EPackageType) i == EPackageType.Fashion || (EPackageType)i == EPackageType.Inscription)
                            {
                                curBaseSize = FashionPackBaseSize;
                            }
                            else if((EPackageType) i == EPackageType.Bxy || (EPackageType)i == EPackageType.Sinan)
                            {
                                curBaseSize = FashionPackBaseSize;
                            }
                            else if ((EPackageType) i == EPackageType.Title)
                            {
                                curBaseSize = TitlePackBaseSize;
                            }
                            PackTotalSize.Add(curBaseSize);

                            if (i < msgData.packSizeAddition.Count)
                            {
                                PackAddSize.Add((int)msgData.packSizeAddition[i]);
                                PackTotalSize[i] += PackAddSize[i];
                            }
                            else
                            {
                                Logger.LogErrorFormat("SOA_PACKAGE_SIZE_ADDITION : PackAddSize.Count <= {0}, EPackageType = {1}", i, (EPackageType)i);
                            }
                        }

                        ItemDataManager.GetInstance().NotifyPackageFullState();

                        break;
                    }
                case SceneObjectAttr.SOA_STORAGESIZE:
                    {
                        //账号仓库的数量
                        AccountStorageSize = msgData.storageSize;
                        break;
                    }
//                 case SceneObjectAttr.SOA_PKVALUE:
//                     {
//                         pkPoints = msgData.pkValue;
//                         TitleComponent.OnPkPointsChanged(0,(int)pkPoints);
//                         UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkPointsChanged);
//                         break;
//                     }
                case SceneObjectAttr.SOA_MATCH_SCORE:
                    {
                        pkMatchScore = msgData.matchScore;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchScoreChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_TITLE:
                    {
                        iTittle = msgData.title;
                        TitleComponent.OnChangedTittle(0, (int)iTittle);
                        break;
                    }
                case SceneObjectAttr.SOA_DAYCHARGENUM:
                    {
                        dayChargeNum = msgData.dayChargeNum;
                        TitleComponent.OnChangedTittle(0, (int)iTittle);
                        break;
                    }
                case SceneObjectAttr.SOA_WARRIOR_SOUL:
                    {
                        WarriorSoul = msgData.warriorSoul;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WarriorSoulChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_PK_COIN:
                    {
                        uiPkCoin = msgData.pkCoin;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkCoinChanged);

                        break;
                    }
                case SceneObjectAttr.SOA_FUNCFLAG:
                    {
                        ClientSystemGameBattle town = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
                        if (town != null)
                        {
                            break;
                        }

                        bool bisInit = false;

                        if (UnlockFuncList.Count <= 0)
                        {
                            bisInit = true;
                        }

                        UnlockFuncList.Clear();
                        UnlockAreaList.Clear();

                        var FunctionUnLockData = TableManager.GetInstance().GetTable<FunctionUnLock>();
                        var enumerator = FunctionUnLockData.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            FunctionUnLock FunctionUnLockItem = enumerator.Current.Value as FunctionUnLock;

                            if (FunctionUnLockItem.IsOpen == false)
                            {
                                continue;
                            }

                            if (!msgData.funcFlag.CheckMask((uint)FunctionUnLockItem.ID))
                            {
                                continue;
                            }

                            //如果是帐号绑定功能 在这里跳过 有专门的帐号解锁通知 和 各自功能的单独的解锁判断 ！！！
                            if (FunctionUnLockItem.BindType == FunctionUnLock.eBindType.BT_AccBind)
                            {
                                continue;
                            }

                            if (FunctionUnLockItem.Type == FunctionUnLock.eType.Area)
                            {
                                UnlockAreaList.Add(FunctionUnLockItem.AreaID);
                            }
                            else if (FunctionUnLockItem.Type == FunctionUnLock.eType.Func)
                            {
                                UnlockFuncList.Add(FunctionUnLockItem.ID);
                            }
                        }

                        //if(bisInit)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateUnlockFunc);
                        }

                        break;
                    }
                case SceneObjectAttr.SOA_FUNCNOTIFY:
                    {
                        FuncMaskProperty data = msgData.funcNotify;
                        if (data == null)
                        {
                            return;
                        }

                        NextUnlockFunc = new List<int>();
                        var FunctionUnLockData = TableManager.GetInstance().GetTable<FunctionUnLock>();
                        var enumerator = FunctionUnLockData.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            FunctionUnLock FunctionUnLockItem = enumerator.Current.Value as FunctionUnLock;

                            if (FunctionUnLockItem.IsOpen == false)
                            {
                                continue;
                            }

                            if (FunctionUnLockItem.ShowNextOpen == 0)
                            {
                                continue;
                            }

                            if (!data.CheckMask((byte)FunctionUnLockItem.ID))
                            {
                                continue;
                            }
                            NextUnlockFunc.Add(FunctionUnLockItem.ID);

                        }
                        
                        NextUnlockFunc.Sort((a, b) =>
                        {
                            var itemA = TableManager.GetInstance().GetTableItem<FunctionUnLock>(a);
                            var itemB = TableManager.GetInstance().GetTableItem<FunctionUnLock>(b);

                            if (itemA != null && itemB != null)
                            {
                                return itemA.FinishLevel - itemB.FinishLevel;
                            }

                            return a - b;
                        });
                        
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NextFuncOpen);

                        break;
                    }
                case SceneObjectAttr.SOA_AWAKEN:
                    {
//                         bool bIsAwake = false;
// 
//                         if (AwakeState > -1 && msgData.awaken > AwakeState)
//                         {
//                             bIsAwake = true;
//                         }

                        AwakeState = msgData.awaken;

//                         if (bIsAwake && !ClientSystemManager.GetInstance().IsFrameOpen<AwakeAwardFrame>())
//                         {
//                             UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AwakeChanged);
//                         }

                        break;
                    }
                case SceneObjectAttr.SOA_VIP:
                    {
                        VipLevel = msgData.vip.level;
                        CurVipLvRmb = (int)msgData.vip.exp;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerVipLvChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_MONTH_CARD_EXPIRE_TIME:
                    {
                        MonthCardLv = msgData.monthCardExpireTime;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnMonthCardUpdate);
                        break;
                    }
                case SceneObjectAttr.SOA_CREATE_TIME:
                    {
                        RoleCreateTime = msgData.createTime;
                        break;
                    }
                case SceneObjectAttr.SOA_NEW_BOOT: // 强引导
                    {
                        NewbieGuideSaveBoot = (int)msgData.newBoot;
                        if (NewbieGuideSaveBoot <= 0)
                        {
                            // 这里做一个容错,如果服务器下发的数据真的因为某种原因小于0，那么会导致强制引导重走一遍
                            NewbieGuideSaveBoot = 100000;
                        }

                        NewbieGuideTable Newbieguidedata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>(NewbieGuideSaveBoot);
                        if (Newbieguidedata == null)
                        {
                            Logger.LogErrorFormat("Receive Server NewbieGuide Save ID error. NewbieGuideSaveBoot = {0}.", NewbieGuideSaveBoot);
                        }
                        else
                        {
                            NewbieGuideCurSaveOrder = Newbieguidedata.Order;
                        }

                        Logger.LogWarningFormat("新手引导存档:{0} = {1}, NewbieGuideCurSaveOrder = {2}", (NewbieGuideTable.eNewbieGuideTask)NewbieGuideSaveBoot, NewbieGuideSaveBoot, NewbieGuideCurSaveOrder);

                        bIsForceGuide = true;
                        IsFlyUpState = false;

                        if (bIsForceGuide && bIsWeakGuide && bIsInitNewbieGuideData)
                        {
                            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.InitNewbieGuideBootData);
                            bIsInitNewbieGuideData = false;
                        }

                        break;
                    }
                case SceneObjectAttr.SOA_BOOT_FLAG: // 弱引导
                    {
                        var tabledata = TableManager.GetInstance().GetTable<NewbieGuideTable>();

                        for (int i = 2; i <= tabledata.Count; i++)
                        {
                            if (!msgData.bootFlag.CheckMask((uint)i))
                            {
                                continue;
                            }

                            bool bFind = false;
                            for (int j = 0; j < NewbieGuideWeakGuideList.Count; j++)
                            {
                                if (NewbieGuideWeakGuideList[j] == i)
                                {
                                    bFind = true;
                                    break;
                                }
                            }

                            if (!bFind)
                            {
                                NewbieGuideWeakGuideList.Add(i);
                                Logger.LogWarningFormat("新手弱引导存档:{0}", (NewbieGuideTable.eNewbieGuideTask)i);
                            }
                        }

                        bIsWeakGuide = true;

                        if (bIsForceGuide && bIsWeakGuide && bIsInitNewbieGuideData)
                        {
                            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.InitNewbieGuideBootData);
                            bIsInitNewbieGuideData = false;
                        }

                        break;
                    }
                case SceneObjectAttr.SOA_TOWER_WIPEOUT_END_TIME:
                    {
                        DeathTowerWipeoutEndTime = msgData.deathTowerWipeoutEndTime;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnDeadTowerWipeoutTimeChange);
                        break;
                    }
                case SceneObjectAttr.SOA_REDPOINT:
                    {
                        RedPointDataManager.GetInstance().UpdateRedPoints(msgData.redPoint);
                        break;
                    }
                case SceneObjectAttr.SOA_ITEMCD:
                    {
                        ItemDataManager.GetInstance().SetupItemCDs(msgData.itemCd);
                        break;
                    }
                case SceneObjectAttr.SOA_GUILD_NAME:
                    {
                        guildName = msgData.guildName;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataGuildUpdated, prop);

                        // TODO 你大爷
                        TitleComponent.OnChangeGuildName(0, PlayerBaseData.GetInstance().guildName);
                        break;
                    }
                case SceneObjectAttr.SOA_GUILD_POST:
                    {
                        eGuildDuty = GuildDataManager.GetInstance().GetClientDuty(msgData.guildPost);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataGuildUpdated, prop);

                        // TODO 你大爷
                        guildDuty = msgData.guildPost;
                        TitleComponent.OnChangeGuildDuty(0, msgData.guildPost);

                        // eGuildDuty为无效值说明被踢出工会了，你大爷
                        if (eGuildDuty == EGuildDuty.Invalid)
                        {
                            //被提出公会了
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildGlobalKickedOut);
                        }
                        else if (eGuildDuty == EGuildDuty.Normal)
                        {
                            //加入公会了
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildGlobalJoined);
                        }

                        break;
                    }
                case SceneObjectAttr.SOA_GUILD_CONTRI:
                    {
                        PlayerBaseData.GetInstance().guildContribution = (int)msgData.guildContri;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataGuildUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_GUILD_BATTLE_NUMBER:
                    {
                        guildBattleTimes = (int)msgData.guildBattleNumber;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataGuildUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_GUILD_BATTLE_SCORE:
                    {
                        guildBattleScore = (int)msgData.guildBattleScore;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataGuildUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_GUILD_BATTLE_REWARD:
                    {
                        guildBattleRewardMask = msgData.guildBattleMask;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataGuildUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_SEASON_ATTR:
                    {
                        SeasonDataManager.GetInstance().seasonAttrID = msgData.seasonAttr;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataSeasonUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_SEASON_LEVEL:
                    {
                        SeasonDataManager.GetInstance().seasonLevel = (int)msgData.seasonLevel;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataSeasonUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_SEASON_STAR:
                    {
                        SeasonDataManager.GetInstance().seasonStar = (int)msgData.seasonStar;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataSeasonUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_SEASON_EXP:
                    {
                        SeasonDataManager.GetInstance().seasonExp = (int)msgData.seasonExp;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataSeasonUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_SEASON_UPLEVEL_RECORD:
                    {
                        SeasonDataManager.GetInstance().seasonUplevelRecords = msgData.seasonUplevelRecord;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataSeasonUpdated, prop);
                        break;
                    }
                case SceneObjectAttr.SOA_AUCTION_REFRESH_TIME:
                    {
                        AuctionLastRefreshTime = msgData.auctionRefreshTime;
                        break;
                    }
                case SceneObjectAttr.SOA_AUCTION_ADDITION_BOOTH:
                    {
                        AddAuctionFieldsNum = (int)msgData.auctionAdditionBooth;
                        break;
                    }
                case SceneObjectAttr.SOA_PET_FOLLOW:
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PetChanged, msgData.followPetDataId);
                        break;
                    }
                case SceneObjectAttr.SOA_POTION_SET:
                    {
                        potionSets = msgData.potionSets;
                        var system = GameClient.ClientSystemManager.instance.CurrentSystem as GameClient.ClientSystemBattle;
                        if (system == null)//保存在城镇里面的配置
                        {
                            tmpPostionSets = potionSets;
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonPotionSetChanged);
                        break;
                    }
                case SceneObjectAttr.SOA_WEAPON_BAR:
                    {
                        SwitchWeaponDataManager.GetInstance().UpdateSideWeapon(msgData.weaponBar);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SwitchEquipSuccess);
                    }
                    break;
                case SceneObjectAttr.SOA_APPOINTMENT_OCCU:
                    {
                        appointmentOccu = msgData.appointmentOccu;
                        break;
                    }
                case SceneObjectAttr.SOA_MONEY_MANAGE_STATUS:
                    {
                        FinancialPlanDataManager.GetInstance().SyncBuyFinancialPlanBoughtStatus(msgData.moneyManageStatus);
                        break;
                    }
                case SceneObjectAttr.SOA_MONEY_MANAGE_REWARD_MASK:
                    {
                        FinancialPlanDataManager.GetInstance().SyncFinancialPlanMaskProperty(msgData.moneyManageRewardMask);
                        break;
                    }
                case SceneObjectAttr.SOA_AVATAR:
                    {
                        if (msgData.avatar == null)
                        {
                            Logger.LogErrorFormat("进城镇收到服务器协议数据: msgData.avatar == null, 协议解析出错");
                        }

                        var isPlayerAvatarNeedChanged = PackageDataManager.GetInstance().IsPlayerAvatarNeedChanged(avatar, msgData.avatar);
                        avatar = msgData.avatar;

#if MG_TEST
                        string ss = "测试服吃鸡:收到AVATAR数据";

                        if(msgData.avatar.equipItemIds != null && msgData.avatar.equipItemIds.Length > 0)
                        {
                            for(int i = 0; i < msgData.avatar.equipItemIds.Length; i++)
                            {
                                ss += "_";
                                ss += msgData.avatar.equipItemIds[i].ToString();
                            }
                        }

                        ExceptionManager.GetInstance().RecordLog(ss);
                        GameStatisticManager.GetInstance().UploadLocalLogToServer(ss);
#endif

                        if (ChijiDataManager.GetInstance().SwitchingPrepareToTown || ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare ||
						   ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene)
                        {
                            break;
                        }

#if MG_TEST
                        ExceptionManager.GetInstance().RecordLog("测试服吃鸡:AvatarChanged");
                        GameStatisticManager.GetInstance().UploadLocalLogToServer("测试服吃鸡:AvatarChanged");
#endif

                        if (avatar != null && isPlayerAvatarNeedChanged)
                        {
                            //Logger.LogErrorFormat("SOA_AVATAR isShoWeapon {0} titleId {1} equips {2}", avatar.isShoWeapon, avatar.titleId, avatar.equipItemIds);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AvatarChanged);
                        }

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateAvatar);

                        break;
                    }
                case SceneObjectAttr.SOA_SCORE_WAR_SCORE:
                    {
                        Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                        pkInfo.nScore = msgData.scoreWarScore;
                        Pk3v3CrossDataManager.GetInstance().PkInfo = pkInfo;
                    }
               
                    break;

                case SceneObjectAttr.SOA_SCORE_WAR_BATTLE_COUNT:
                    {
                        //Logger.LogErrorFormat("pk剩余次数{0}", msgData.scoreWarBattleCount);
                        Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                        pkInfo.nCurPkCount = (int)((uint)(Pk3v3CrossDataManager.MAX_PK_COUNT) - (uint)msgData.scoreWarBattleCount);
                        Pk3v3CrossDataManager.GetInstance().PkInfo = pkInfo;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossPkAwardInfoUpdate);
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3RoomSlotInfoUpdate);
                    }

                    break;
                case SceneObjectAttr.SOA_SCORE_WAR_REWARD_MASK:
                    {
                        Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                        List<uint> arrIDs = pkInfo.arrAwardIDs;
                        arrIDs.Clear();
                        

                        int iSize = TableManager.GetInstance().GetTable<ProtoTable.ScoreWarRewardTable>().Count;
                        for (uint i = 0; i < msgData.scoreWarRewardMask.maskSize && i < iSize; ++i)
                        {
                            int iId = (int)(i + 1);
                            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ScoreWarRewardTable>(iId);
                            if (null == item)
                            {
                                continue;
                            }

                            if (msgData.scoreWarRewardMask.CheckMask((uint)iId))
                            {
                                arrIDs.Add((uint)iId);
                            }
                        }

                        arrIDs.Sort();
                        Pk3v3CrossDataManager.GetInstance().PkInfo = pkInfo;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossPkAwardInfoUpdate);
                    }

                    break;
                case SceneObjectAttr.SOA_SCORE_WAR_WIN_BATTLE_COUNT:
                    {
                        Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                        pkInfo.nWinCount = msgData.scoreWarWinBattleCount;
                        Pk3v3CrossDataManager.GetInstance().PkInfo = pkInfo;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossPkAwardInfoUpdate);
                    }
                    break;
                case SceneObjectAttr.SOA_ACADEMIC_TOTAL_GROWTH_VALUE:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.academicTotalGrowth = msgData.academicTotalGrowthValue;
                    }

                    break;
                case SceneObjectAttr.SOA_MASTER_DAILYTASK_GROWTH:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.masterDailyTaskGrowth = msgData.masterDailyTaskGrowth;
                    }
                    break;
                case SceneObjectAttr.SOA_MASTER_ACADEMICTASK_GROWTH:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.masterAcademicTaskGrowth = msgData.masterAcademicTaskGrowth;
                    }
                    break;
                case SceneObjectAttr.SOA_MASTER_UPLEVEL_GROWTH:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.masterUplevelGrowth = msgData.masterUplevelGrowth;
                    }
                    break;
                case SceneObjectAttr.SOA_MASTER_GIVEEQUIP_GROWTH:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.masterGiveEquipGrowth = msgData.masterGiveEquipGrowth;
                    }
                    break;
                case SceneObjectAttr.SOA_MASTER_GIVEGIFT_GROWTH:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.masterGiveGiftGrowth = msgData.masterGiveGiftGrowth;
                    }
                    break;
                case SceneObjectAttr.SOA_MASTER_TEAMCLEARDUNGON_GROWTH:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.masterTeamClearDungeonGrowth = msgData.masterTeamClearDungeonGrowth;
                    }
                    break;
                case SceneObjectAttr.SOA_MASTER_GOODTEACHER_VALUE:
                    {
                        TAPNewDataManager.GetInstance().myTAPData.goodTeachValue = msgData.goodTeacherValue;
                        GoodTeacherValue = msgData.goodTeacherValue;
                    }
                    break;
                case SceneObjectAttr.SOA_SHOW_FASHION_WEAPON:
                {
                    isShowFashionWeapon = msgData.showFashionWeapon == 1 ? true : false;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnIsShowFashionWeapon);
                    PackageDataManager.GetInstance().ResetSendFashionWeaponReqFlag();
                }
                    break;
                case SceneObjectAttr.SOA_WEAPON_LEASE_TICKETS:
                    WeaponLeaseTicket = msgData.weaponLeaseTickets;
                    break;

                case SceneObjectAttr.SOA_MALL_POINT:
                    IntergralMallTicket = msgData.mallPoint;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerMallPointUpdate);
                    break;

                case SceneObjectAttr.SOA_GAME_SET:
                    SystemConfigManager.GetInstance().ParseGameSet(msgData.gameSets);
                    break;
                case SceneObjectAttr.SOA_HEAD_FRAME:
                    {
                        HeadPortraitFrameDataManager.WearHeadPortraitFrameID = (int)msgData.headFrame;
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.HeadPortraitFrameChange);
                    }
                    break;
                case SceneObjectAttr.SOA_NEW_TITLE_NAME:
                    WearedTitleInfo = msgData.wearedTitleInfo;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TitleNameUpdate);
                    TitleComponent.OnChangeTitleName(0, PlayerBaseData.GetInstance().wearedTitleInfo);
                    break;
                case SceneObjectAttr.SOA_NEW_TITLE_GUID:
                    TitleGuid = msgData.newTitleGuid;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TitleGuidUpdate);
                    break;
                case SceneObjectAttr.SOA_GUILD_EMBLEM:
                    GuildEmblemLv = msgData.guildEmblemLvl;
                    TitleComponent.OnChangeGuileLv(0, PlayerBaseData.GetInstance().guildEmblemLv);
                    break;
                case SceneObjectAttr.SOA_OPPO_VIP_LEVEL:
                    OPPOPrivilegeDataManager.GetInstance().OppOAmberLevel = msgData.oppoVipLevel;
                    break;
                case SceneObjectAttr.SOA_CHIJI_SCORE:
                    ChijiScore = msgData.chijiScore;
                    break;
                case SceneObjectAttr.SOA_TOTAL_EQUIP_SCORE:
                    TotalEquipScore = msgData.equipScore;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateEquipmentScore);
                    break;
                case SceneObjectAttr.SOA_GAMEOPTIONS:
                    gameOptions = msgData.gameOptions;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.UpdateGameOptions);
                    break;
                case SceneObjectAttr.SOA_ADVENTURE_COIN:
                    adventureCoin = msgData.adventureCoin;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnUpdateAdventureCoin);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemCountChanged);
                    if (onMoneyChanged != null)
                    {
                        onMoneyChanged(MoneyBinderType.MBT_OTHER);
                    }
                    break;
                case SceneObjectAttr.SOA_STORAGE_OPEN_NUM:
                    {
                        StorageDataManager.GetInstance().SetAccountStorageOwnerStorageNumber(msgData.storageOpenNum);
                    }
                    break;
                default:
                    {
                        Logger.Log("player prop need update!!");
                        break;
                    }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerDataBaseUpdated, prop);
        }

        public enum MoneyBinderType
        {
            MBT_OTHER = -1,
            MBT_GOLD,//金币
            MBT_BIND_GOLD,//绑金
            MBT_POINT,//点券
            MBT_BIND_POINT,//绑点
            MBT_ALIVE_COIN,//复活币
            MBT_WARRIOR_SOUL,//勇者之魂
            MBT_PKMONSETR_COIN,//斗兽币
            MBT_FIGHT_COIN,//决斗币
            MBT_GUILD_CONTRIBUTION, // 贡献
            MBT_GOLD_JAR_SCORE,//金罐积分
            MBT_Magic_JAR_SCORE,//魔罐积分
            MBT_Appoinment_Coin,//预约硬币
            MBT_GoodTeacher_Value,//良师值
            MBT_WEAPON_LEASE_TICKET,//武器租赁好运符
            MBT_INTERGRALMALL_TICKET,  //积分商城
        }
        public delegate void OnMoneyChanged(MoneyBinderType eMoneyBinderType);
        public OnMoneyChanged onMoneyChanged;

        public PlayerAvatar avatar = null;
        public bool isShowFashionWeapon = false; //是否显示时装武器。如果是0服务器默认不同步，值默认为false

        // TODO 改成整数运算，就不需要这个了
        public float AxisScale = 10000.0f;

        // 城镇是否初始化
        public bool bInitializeTownSystem = false;

        public List<UInt32> potionSets = new List<UInt32>();

        public List<UInt32> tmpPostionSets = new List<UInt32>();

        // 药品槽位类型
        public enum PotionSlotType
        {
            SlotMain, // 主槽位 可放置HP和生命祝福
            SlotExtend1, // 扩展槽位1 可放置HP和生命祝福
            SlotExtend2, // 扩展槽位2 可放置MP和生命祝福
        }
        const int defaultPotionPercent = 50;
        string m_kSavePath = "PotionPercentSet.json";
        string jsonText = null;
        public int GetPotionID(PotionSlotType potionSlotType)
        {
            if(potionSets == null)
            {
                return 0;
            }
            int index = (int)potionSlotType;
            if(index >= potionSets.Count || index < 0)
            {
                return 0;
            }
            return (int)potionSets[index];
        }
        public int GetHPPotionPercentMax()
        {
            if(IsPotionSlotMainSwitchOn() && IsPotionSlotMainSwitchOn(potionSlot1SwitchKeyName))
            {
                return Math.Max(GetPotionPercent(PotionSlotType.SlotMain), GetPotionPercent(PotionSlotType.SlotExtend1));
            }
            else if (IsPotionSlotMainSwitchOn())
            {
                return GetPotionPercent(PotionSlotType.SlotMain);
            }
            else if (IsPotionSlotMainSwitchOn(potionSlot1SwitchKeyName))
            {
                return GetPotionPercent(PotionSlotType.SlotExtend1);
            }
            return -1;
        }
        //public int GetHPPotionIndex(bool isAuto)
        //{
        //    if (IsPotionSlotMainSwitchOn() && isAuto)
        //    {
        //        int id = GetPotionID(PotionSlotType.SlotMain);
        //        if(id>0)
        //          return (int)PotionSlotType.SlotMain;
        //    }
        //    int _id = GetPotionID(PotionSlotType.SlotExtend1);
        //    if(_id>0)
        //      return (int)PotionSlotType.SlotExtend1;
        //    return -1;
        //}
        public int GetMPPotionIndex()
        {
            return (int)PotionSlotType.SlotExtend2;
        }
        public int GetMPPotionPercentMax()
        {
            if (IsPotionSlotMainSwitchOn(potionSlot2SwitchKeyName))
            {
                return GetPotionPercent(PotionSlotType.SlotExtend2);
            }
            return -1;
        }
        public int GetPotionPercent(PotionSlotType potionSlotType)
        {
            if (jsonText == null)
            {
                return 0;
            }
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return 0;
            }      
            string key = potionSlotType.ToString();
            if(data.ContainsKey(key) && data[key].IsInt)
            {
                return (int)data[key];
            }
            return 0;
        }       
        public void SetPotionPercent(PotionSlotType potionSlotType,int value,bool save2File = false)
        {
            if(value < 0)
            {
                value = 0;
            }
            if(value > 100)
            {
                value = 100;
            }
            if(jsonText == null)
            {
                return;
            }
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return;
            }
            data[potionSlotType.ToString()] = value;
            jsonText = data.ToJson();
            if(save2File)
            {
                SavePotionPercentSetsToFile();
            }
            return;
        }
        public const string potionSlotMainSwitchKeyName = "PotionSlotMainSwitch";
        public const string potionSlot1SwitchKeyName = "PotionSlot1Switch";
        public const string potionSlot2SwitchKeyName = "PotionSlot2Switch";
        public bool IsPotionSlotMainSwitchOn(string keyName = potionSlotMainSwitchKeyName)
        {
            if (jsonText == null)
            {
                return false;
            }
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return false;
            }
            string key = keyName;
            if (data.ContainsKey(key) && data[key].IsBoolean)
            {
                return (bool)data[key];
            }
            return false;
        }
        public void SetPotionSlotMainSwitchOn(bool bOn, bool save2File = false, string keyName = potionSlotMainSwitchKeyName)
        {
            if (jsonText == null)
            {
                return;
            }
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(jsonText);
            if (data == null)
            {
                return;
            }
            data[keyName] = bOn;
            jsonText = data.ToJson();
            if (save2File)
            {
                SavePotionPercentSetsToFile();
            }
        }
        public void SavePotionPercentSetsToFile()
        {
            if (jsonText == null)
            {
                return;
            }          
            try
            {            
                if (!string.IsNullOrEmpty(jsonText))
                {
                    FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kSavePath, jsonText);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        public int appointmentOccu = 0;

        // 角色ID
        ulong ulRoleId = 0;
        public ulong RoleID
        {
            get
            {
                return ulRoleId;
            }
            set
            {
                ulong preRoleId = ulRoleId;
                ulRoleId = value;

                if(preRoleId != ulRoleId)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RoleIdChanged,preRoleId,value);
                }
            }
        }

        // 大区ID
        public int ZoneID;

        // 名字
        public string Name;

        //抗魔值
        public int ResistMagicValue;

        // 等级               
        public delegate void OnLevelChanged(int iPreLv, int iCurLv);
        public OnLevelChanged onLevelChanged = null;
        ushort iLevel;                        
        public ushort Level
        {
            get
            {
                return iLevel;
            }
            set
            {
                if(value != iLevel)
                {
                    int iPreLv = iLevel;
                    iLevel = value;
                    if(onLevelChanged != null)
                    {
                        onLevelChanged.Invoke(iPreLv, iLevel);
                        BeUtility.SetPlayerActorDataDirty();
                    }
                }
            }
        }

        public bool IsLevelFull
        {
            get
            {
                var maxLevel = 60;
                var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PLAYER_MAX_LEVEL_LIMIT);
                if(null != systemValue)
                {
                    maxLevel = systemValue.Value;
                }
                return iLevel >= maxLevel;
            }
        }

        // 每日活跃度
        int iActivityValue = 0;
        public int ActivityValue
        {
            get
            {
                return CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_ACTIVITY_VALUE);
            }
        }

        // 任务积分
        uint iMissionScore = 0;
        public delegate void OnMissionScoreChanged(int iValue);
        public OnMissionScoreChanged onMissionScoreChanged;
        public uint MissionScore
        {
            get
            {
                return iMissionScore;
            }
            set
            {
                iMissionScore = value;
                if(onMissionScoreChanged != null)
                {
                    onMissionScoreChanged.Invoke((int)iMissionScore);
                }
            }
        }

        // 武道大会任务状态
        int iBudoStatus = 0;
        public int BudoStatus
        {
            get
            {
                return iBudoStatus;
            }
            set
            {
                iBudoStatus = value;
                BudoManager.GetInstance().BudoStatus = value;
            }
        }

        // 成就奖励字段屏蔽字
        AchievementMaskProperty _achievementMask;
        public AchievementMaskProperty AchievementMaskProperty
        {
            get
            {
                return _achievementMask;
            }
            set
            {
                _achievementMask = value;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementMaskPropertyChanged);
            }
        }

        // 任务奖励屏蔽字
        DailyTaskMaskProperty dailyTaskMaskProperty;
        public DailyTaskMaskProperty DailyTaskMaskProperty
        {
            get
            {
                return dailyTaskMaskProperty;
            }
            set
            {
                dailyTaskMaskProperty = value;
                MissionManager.GetInstance().SetDailyMaskProperty(dailyTaskMaskProperty);
            }
        }

        // 当前等级下的已得到的经验
        public ulong CurExp = 0;
        // 玩家获取的累加总经验                                            
        public ulong Exp = 0;

        // 是否处于使用飞升秘药的升级过程中
        public bool IsFlyUpState = false;

        // 性别                                    
        public int Sex;

        // 新手引导存档
        public int NewbieGuideSaveBoot = 0; // 强引导
        public int NewbieGuideCurSaveOrder = 0; 
        public List<int> NewbieGuideWeakGuideList = new List<int>(); // 弱引导
        private bool bIsForceGuide = false;
        private bool bIsWeakGuide = false;
        public bool bIsInitNewbieGuideData = true;
        public int GuideFinishMission = 0;

        // HP
        public int HP;

        // HP最大值
        public int MaxHP;

        // 吃鸡血量
        private int chiji_hp; // 千分比，分子

        public float Chiji_HP_Percent
        {
            get { return chiji_hp / 1000.0f; }
        }

        // 吃鸡MP
        private int chiji_mp; // 千分比,分子
        public float Chiji_MP_Percent
        {
            get { return chiji_mp / 1000.0f; }
        }

        // 位置
        public Vector3 Pos;

        // 朝向
        public bool FaceRight;

        // 移动速度倍率
        public float MoveSpeedRate;

        // 疲劳值
        public UInt16 fatigue;

        // 疲劳值上限
        public UInt16 MaxFatigue;

        public UInt32 AuctionLastRefreshTime;

        // 拍卖行额外购买栏位
        public int AddAuctionFieldsNum;

        //良师值
        CrypticUlong ulGoodTeacherValue;
        public ulong GoodTeacherValue
        {
            get { return ulGoodTeacherValue; }
            set
            {
                ulGoodTeacherValue = value;
                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_GoodTeacher_Value);
                }
            }
        }


        // 游戏币
        CrypticUlong ulGold;
        public ulong Gold
        {
            get { return ulGold; }
            set
            {
                ulGold = value;
                if(onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_GOLD);
                }
            }
        }

        ulong titleGuid;
        public ulong TitleGuid
        {
            get { return titleGuid; }
            set { titleGuid = value; }
        }

        string titleName;
      
        PlayerWearedTitleInfo wearedTitleInfo;
        public PlayerWearedTitleInfo WearedTitleInfo
        {
            get { return wearedTitleInfo; }
            set { wearedTitleInfo = value; }
        }

        uint guildEmblemLv;
        public uint GuildEmblemLv
        {
            get { return guildEmblemLv;}
            set { guildEmblemLv = value; }
        }

        // 游戏币（绑定）
        CrypticUlong ulBindGold;
        public ulong BindGold
        {
            get { return ulBindGold; }
            set
            {
                ulBindGold = value;
                if(onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_BIND_GOLD);
                }
            }
        }

        //武器租赁好运符
        CrypticUlong weaponLeaseTicket;
        public ulong WeaponLeaseTicket
        {
            get { return weaponLeaseTicket; }
            set
            {
                weaponLeaseTicket = value;
                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_WEAPON_LEASE_TICKET);
                }
            }
        }

        //积分商城积分
        CrypticUlong intergralMallTicket;
        public ulong IntergralMallTicket
        {
            get { return intergralMallTicket; }
            set
            {
                intergralMallTicket = value;
                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_INTERGRALMALL_TICKET);
                }
            }
        }

        private bool _canUse(ulong va, ulong vb, ulong cnt)
        {
            if (va >= cnt || vb >= cnt)
            {
                return true;
            }

            if (va < cnt)
            {
                return (cnt - va) <= vb;
            }
            else 
            {
                return (cnt - vb) <= va;
            }
        }

        public bool CanUseGold(ulong cnt)
        {
            return _canUse(BindGold, Gold, cnt);
        }

        public bool CanUseTicket(ulong cnt)
        {
            return _canUse(Ticket, BindTicket, cnt);
        }

        // 点劵
        CrypticInt32 ulTicket;

        public ulong Ticket
        {
            get { return (ulong)ulTicket.ToInt(); }

            set
            {
                ulTicket = (int)value;

                if(onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_POINT);
                }
            }
        }

        // 点劵（绑定）
        CrypticInt32 ulBindTicket;

        public ulong BindTicket
        {
            get { return (ulong)ulBindTicket.ToInt(); }

            set
            {
                ulBindTicket = (int)value;

                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_BIND_POINT);
                }
            }
        }

        CrypticUlong ulGoldJarScore;
        public ulong GoldJarScore
        {
            get { return ulGoldJarScore; }
            set
            {
                ulGoldJarScore = value;
                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_GOLD_JAR_SCORE);
                }
            }
        }

        CrypticUlong ulMagicJarScore;
        public ulong MagicJarScore
        {
            get { return ulMagicJarScore; }
            set
            {
                ulMagicJarScore = value;
                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_Magic_JAR_SCORE);
                }
            }
        }

        //预约硬币
        CrypticUlong ulAppointmentCoin;
        public ulong AppoinmentCoin
        {
            get { return ulAppointmentCoin; }
            set
            {
                ulAppointmentCoin = value;
                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_Appoinment_Coin);
                }
            }
        }

        public delegate void OnRoleCreateTimeChanged();
        public OnRoleCreateTimeChanged onRoleCreateTimeChanged;
        uint uiRoleCreateTime;
        public uint RoleCreateTime
        {
            get { return uiRoleCreateTime; }
            set
            {
                uiRoleCreateTime = value;
                if(onRoleCreateTimeChanged != null)
                {
                    onRoleCreateTimeChanged();
                }
            }
        }

        // VIP
        public int VipLevel = -1;

        uint iMonthCardLv = 0;
        public uint MonthCardLv
        {
            get
            {
                return iMonthCardLv;
            }
            set
            {
                iMonthCardLv = value;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MonthCardChanged);
            }
        }

        // 当前vip等级下已经充值数目
        public int CurVipLvRmb = 0;

        // 复活币
        CrypticUlong ulAliveCoin;
        public ulong AliveCoin
        {
            get { return ulAliveCoin; }
            set
            {
                ulAliveCoin = value;
                if(onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_ALIVE_COIN);
                }
            }
        }

        // 死亡币
        UInt32 uiWarriorSoul;
        public UInt32 WarriorSoul
        {
            get { return uiWarriorSoul; }
            set
            {
                uiWarriorSoul = value;
                if(onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_WARRIOR_SOUL);
                }
            }
        }

        // pk经验值(段位积分)
        public uint pkPoints;

        // 决斗场积分
        public uint pkMatchScore;

        // 决斗币
        uint pkCoin;
        public UInt32 uiPkCoin
        {
            get
            {
                return pkCoin;
            }
            set
            {
                pkCoin = value;
                if(onMoneyChanged != null)
                {
                    onMoneyChanged.Invoke(MoneyBinderType.MBT_FIGHT_COIN);
                }
            }
        }

        //挑战者积分
        public int ChanllengeScore = 0;
        // pk结算数据
        //public SceneMatchPkRaceEnd PkEndData = new SceneMatchPkRaceEnd();
        public bool bPkClickVipCharge = false;

        // 背包格子数
        public int PackBaseSize; // 背包各个页签下公共的基础格子数，包含了玩家自己解锁的扩展格子数(除去时装和称号)
        public int FashionPackBaseSize; //时装背包的基础格子数
        public int TitlePackBaseSize;       //称号的基础格子数
        public List<int> PackAddSize = new List<int>();// vip特权等各种权益对背包各个页签下单独扩展格子数的加成
        public List<int> PackTotalSize = new List<int>((int)EPackageType.Count); // 每个页签下的总格子数，PackBaseSize + PackSize[i];
        public PlayerBaseData()
        {
            for (int i = 0; i < (int)EPackageType.Count; i++)
            {
                PackTotalSize.Add(0);
            }
        }

        // 仓库格子数
        //账号仓库的格子数
        public int AccountStorageSize = 50;
        //角色仓库每页的格子数，数量固定，每页30个
        public int RoleStorageSize = 30;        

        // 玩家每日累计充值
        uint _dayChargeNum = 0;
        public uint dayChargeNum
        {
            get
            {
                return _dayChargeNum;
            }
            set
            {
                _dayChargeNum = value;

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnDayChargeChanged);
            }
        }

        // 当前成就积分
        int _achievementScore = 0;
        public int AchievementScore
        {
            get
            {
                return _achievementScore;
            }
            set
            {
                int _pre = _achievementScore;
                _achievementScore = value;

                //重新登录 需要角色和帐号 都进行设置后 才判断成就点变换
                if (isRoleEnterGame)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementScoreChanged, _pre, _achievementScore);
                }
            }
        }

        //帐号成就点 （分离）
        int _accountAchievementScore = 0;
        public int AccountAchievementScore
        {
            set
            {
                _accountAchievementScore = value;
                AchievementScore = _accountAchievementScore + _roleAchievementScore;
            }
        }

        //角色成就点 （分离）
        int _roleAchievementScore = 0;
        public int RoleAchievemeentScore
        {
            set
            {               
                _roleAchievementScore = value;
                AchievementScore = _roleAchievementScore + _accountAchievementScore;
            }
        }

        //角色进入游戏
        bool isRoleEnterGame = false;
        public bool IsRoleEnterGame
        {
            set {
                isRoleEnterGame = value;
            }
        }

        //吃鸡积分
        uint _ChijiScore = 0;
        public uint ChijiScore
        {
            get { return _ChijiScore; }
            set { _ChijiScore = value; }
        }

        // 当前玩家称号ID
        public uint iTittle = 0;
        uint totalEquipScore = 0;
        /// <summary>
        /// 全身装备评分
        /// </summary>
        public uint TotalEquipScore
        {
            get { return totalEquipScore; }
            set { totalEquipScore = value; }
        }

        // 觉醒
        public int AwakeState = -1;
        public bool bNeedShowAwakeFrame = false;

        // 解锁功能id
        public List<int> UnlockFuncList = new List<int>();
        public List<int> NewUnlockFuncList = new List<int>();
        public List<int> NewUnlockNotPlayFuncList = new List<int>();
        public List<int> NextUnlockFunc = new List<int>();

        public List<int> UnlockAreaList = new List<int>();

        public uint DeathTowerWipeoutEndTime;

        // 当前职业ID
        int m_jobTableID = (int)Global.Settings.iSingleCharactorID;
        public int JobTableID
        {
            get { return m_jobTableID; }
            set
            {
                m_jobTableID = value;

                m_activeJobTableIDs.Clear();
                int job = m_jobTableID;
                ProtoTable.JobTable table = null;
                if (job > 0)
                {
                    table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(job);
                }
                while (job > 0 && table != null)
                {
                    m_activeJobTableIDs.Add(job);

                    job = table.prejob;
                    table = null;
                    if (job > 0)
                    {
                        table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(job);
                    }
                }
            }
        }
        public int PreChangeJobTableID;

        public ChangeJobState eChangeJobState = ChangeJobState.JobState_Null;

        // 有效的职业ID
        List<int> m_activeJobTableIDs = new List<int>();
        public List<int> ActiveJobTableIDs
        {
            get { return m_activeJobTableIDs; }
        }

        string puiplAnnouncement = string.Empty;
        public string Announcement
        {
            get
            {
                return puiplAnnouncement;
            }
            set
            {
                puiplAnnouncement = value;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAnnouncementChanged);
            }
        }
        bool _getPupilSetting = true;
        public bool getPupil
        {
            get
            {
                return _getPupilSetting;
            }
            set
            {
                _getPupilSetting = value;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGetPupilSettingChanged);
            }
        }

        // guild
        public string guildName = "";
        public EGuildDuty eGuildDuty = EGuildDuty.Invalid;
        public byte guildDuty = 0;
        int ms_nGuildContribution = 0;
        public int guildContribution
        {
            get { return ms_nGuildContribution; }
            set
            {
                ms_nGuildContribution = value;
                if (onMoneyChanged != null)
                {
                    onMoneyChanged(MoneyBinderType.MBT_GUILD_CONTRIBUTION);
                }
            }
        }
        
        /// <summary>
        /// 公会战次数
        /// </summary>
        public int guildBattleTimes;

        /// <summary>
        /// 公会战积分
        /// </summary>
        public int guildBattleScore;

        /// <summary>
        /// 公会战奖励领取掩码
        /// </summary>
        public GuildBattleMaskProperty guildBattleRewardMask;

        public uint gameOptions = 0; // 游戏选项，用bit存储 对应的位类型为SaveOptionMask
        public uint adventureCoin = 0; // 冒险币 冒险者通行证系统使用
        #endregion

        public List<BuffInfoData> GetRankBuff()
		{
			List<BuffInfoData> buffInfos = new List<BuffInfoData>();
			var buffList = SeasonDataManager.GetSeasonAttrBuffIDs(SeasonDataManager.GetInstance().seasonAttrID);
			if (buffList != null)
			{
				for(int i=0; i<buffList.Count; ++i)
				{
					if (buffList[i] > 0) 
					{
                        BuffInfoData buffInfo = new BuffInfoData
                        {
                            buffID = buffList[i]
                        };

                        buffInfos.Add(buffInfo);
					}
				}
			}

			return buffInfos;
		}
		
        public Dictionary<int, string> GetAvatar()
        {
            if (avatar == null)
                return new Dictionary<int, string>(); ;

            return BattlePlayer.GetAvatar(avatar);
            
        }

        public PetData GetPetData(bool isPvp)
		{
			PetData petData = new PetData ();
			List<BuffInfoData> list = new List<BuffInfoData> ();

			var pets = PetDataManager.GetInstance().GetOnUsePetList();
			for(int i=0; i<pets.Count; ++i)
			{
				var pet = pets[i];
				if (pet == null)
					continue;

				var petTableData = TableManager.GetInstance ().GetTableItem<ProtoTable.PetTable> ((int)pet.dataId);
				if (petTableData == null)
					continue;

                //出生技能
                if (petTableData.InnateSkill > 0)
				{
					BeUtility.AddBuffFromSkill(petTableData.InnateSkill, pet.level, list, isPvp);
				}

				if (petTableData.PetType == PetTable.ePetType.PT_ATTACK)
				{
					petData.id = petTableData.MonsterID;
					petData.skillID = petTableData.Skills[pet.skillIndex];
					petData.hunger = pet.hunger;
					petData.level = pet.level;
				}
				//其他宠物
				else
				{
					int skillID = PetDataManager.GetPetSkillIDByJob((int)pet.dataId, PlayerBaseData.GetInstance().JobTableID, (int)pet.skillIndex);

					//int skillID = petTableData.Skills[pet.skillIndex];
					//Logger.LogErrorFormat("pet skill id:{0}", skillID);

					BeUtility.AddBuffFromSkill(skillID, pet.level, list, isPvp);
				}

			}

			petData.buffs = list;

			return petData;
		}


		public bool HasAccompany()
		{
			return false;
		}

        public int GetAwakeSkillID()
        {
            if (BattleMain.IsCanAccompany(BattleMain.battleType))
            {
                var skillInfo = SkillDataManager.GetInstance().GetSkillInfo(BattleMain.IsModePvP(BattleMain.battleType));


                var enumrater = skillInfo.GetEnumerator();
                while(enumrater.MoveNext())
                {
                    int skillID = (int)enumrater.Current.Key;
                    var data = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skillID);
                    //觉醒技能
                    if (data != null && data.SkillCategory == 4 && data.SkillType == SkillTable.eSkillType.ACTIVE)
                        return skillID;
                }
            }
            return 0;
        }

#region item

		public int GetWeaponStrengthenLevel()
		{
			int sl = 0;
			var tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
			if (tmpEquip != null)
			{
				foreach (var uid in tmpEquip)
				{
					ItemData item = ItemDataManager.GetInstance().GetItem(uid);
					if (item != null && item.SubType == (int)EEquipWearSlotType.EquipWeapon && item.EquipWearSlotType == EEquipWearSlotType.EquipWeapon)
					{
						sl = item.StrengthenLevel;
					}
				}
			}

			return sl;
		}

        public List<ItemProperty> GetSideEquipments()
        {
            List<ItemProperty> equipmentsProperty = new List<ItemProperty>();

            //装备的装备
            var tmpEquip = SwitchWeaponDataManager.GetInstance().GetSideWeaponIDList();
            if (tmpEquip != null)
            {
                int length = tmpEquip.Count;
                for (int i = 0; i < length; i++)
                {
                    ItemData item = ItemDataManager.GetInstance().GetItem(tmpEquip[i]);
                    if (item != null)
                    {
                        ItemProperty ip = item.GetBattleProperty();
                        ip.itemID = (int)item.TableID;
                        ip.guid = item.GUID;
                        equipmentsProperty.Add(ip);
                    }
                }
            }
            return equipmentsProperty;
        }
		
		public int GetTitleID()
		{
			int titleID = 0;
			var tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
			if (tmpEquip != null)
			{
				foreach (var uid in tmpEquip)
				{
					ItemData item = ItemDataManager.GetInstance().GetItem(uid);
					if (item != null && item.SubType == (int)EEquipWearSlotType.Equiptitle)
					{
						titleID = (int)item.TableID;
					}
				}
			}

			return titleID;
		}
		
        public List<ItemProperty> GetEquipedEquipments()
        {
            List<ItemProperty> equipmentsProperty = new List<ItemProperty>();

            //装备的装备
            var tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            if (tmpEquip != null)
            {
                foreach (var uid in tmpEquip)
                {
                    ItemData item = ItemDataManager.GetInstance().GetItem(uid);
                    if (item != null)
                    {
                        ItemProperty ip = item.GetBattleProperty();
                        ip.itemID = (int)item.TableID;
                        ip.guid = item.GUID;
                        equipmentsProperty.Add(ip);
                    }
                }
            }


            //装备的时装
            tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
            if (tmpEquip != null)
            {
                foreach (var uid in tmpEquip)
                {
                    ItemData item = ItemDataManager.GetInstance().GetItem(uid);
                    if (item != null)
                    {
                        ItemProperty ip = item.GetBattleProperty();
                        //时装暂时不需要
                        ip.itemID = (int)item.TableID;
                        equipmentsProperty.Add(ip);
                    }
                }
            }


            //TODO 套装的属性

            return equipmentsProperty;
        }

        // 自动设置生命祝福槽位
        bool AutoSetHpMpPotion(uint hpMPPotionID)
        {
            if(hpMPPotionID == 0)
            {
                return false;
            }

            // 只有在战斗场景中获得生命祝福才自动配置
            ClientSystemBattle battleSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemBattle;
            if(battleSystem == null)
            {
                return false;
            }

            bool isFind = false;
            List<uint> ids = tmpPostionSets;
            if (ids != null)
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i] == hpMPPotionID)
                    {
                        isFind = true;
                        ChapterBattlePotionSetUtiilty.Save(i, hpMPPotionID);
                        return true;
                    }
                }
            }

            if (!isFind)
            {
                if (ids != null)
                {
                    for (int i = 0; i < ids.Count; i++)
                    {
                        if (ids[i] == 0)
                        {
                            ChapterBattlePotionSetUtiilty.Save(i, hpMPPotionID);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        void _OnAddNewItem(List<Item> items)
        {
            if(items == null)
            {
                return;
            }
     
            for(int i = 0;i < items.Count;i++)
            {
                if(items[i] == null)
                {
                    continue;
                }

                ProtoTable.ItemTable itemTable = TableManager.instance.GetTableItem<ProtoTable.ItemTable>((int)items[i].tableID);
                if (itemTable == null)
                {
                    continue;
                }

                if (itemTable.SubType != ItemTable.eSubType.HpMp)
                {
                    continue;
                }

                uint hpMPPotionID = items[i].tableID;
                if (AutoSetHpMpPotion(hpMPPotionID))
                {
                    break;
                }
            }           
        }

        #endregion

        public uint SP;//PVE第一套配置方案的技能点
        public uint SP2;//PVE第二套配置方案的技能点

        public uint pvpSP;//PVP第一套配置方案的技能点
        public uint pvpSP2;//PVP第二套配置方案的技能点

        public uint FairDuelSp;//公平竞技场的技能配置方案的技能点

        public List<Battle.DungeonBuff> buffList = new List<Battle.DungeonBuff>()             // 玩家携带buff
        {
            //new Buff() { id = 650001, duration = 2000 },
            //new Buff() { id = 650002, duration = 2000 },
            //new Buff() { id = 660001, duration = 20000 },
            //new Buff() { id = 660002, duration = 10000 },
        };

        public List<Battle.DungeonBuff> removedBuffList = new List<Battle.DungeonBuff>();

#region mail
        public GameMailData mails = new GameMailData();  // 邮件数据
#endregion

#region PkDataStatistics 
        public Dictionary<byte, PkStatistic> PkStatisticsData = new Dictionary<byte, PkStatistic>();  // pk数据统计

        public PkStatistic GetPkStatisticDataByPkType(PkType TypeID)
        {
            byte id = (byte)TypeID;

            PkStatistic data;
            if (PkStatisticsData.TryGetValue(id, out data))
            {
                return data;
            }

            return null;
        }
#endregion

#region Mall
        public Dictionary<MallType, Dictionary<MallTypeTable.eMallType, Dictionary<MallTypeTable.eMallSubType, Dictionary<int, List<MallItemInfo>>>>> MallItemData = new Dictionary<MallType, Dictionary<MallTypeTable.eMallType, Dictionary<MallTypeTable.eMallSubType, Dictionary<int, List<MallItemInfo>>>>>();
#endregion

        //替换时装部件
        //itemId 为0的时候，表示卸掉slotType对应部位的时装
        public void AvatarEquipPart(IGeAvatarActor avatarRenderer, EFashionWearSlotType slotType, int itemId, GeActorEx geActor = null,int prodId = 0,bool highPriority = false)
        {
            if (avatarRenderer == null && geActor == null)
            {
                return;
            }

            EFashionWearSlotType slot = slotType;
            GeAvatarChannel channel = GeAvatarChannel.MaxChannelNum;

            ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);

            // 先做槽位类型转换与其他地方调用同一个接口 by wangbo 2020.03.31
            GetFashionSlotChangedType(ref slot, ref channel, tableData);

            if (channel != GeAvatarChannel.MaxChannelNum)
            {
                if (itemId == 0)
                {
                    if (geActor != null)
                        geActor.ChangeAvatar(channel, null, AssetLoadConfig.instance.asyncLoad,highPriority, prodId);
                    if (avatarRenderer != null)
                        avatarRenderer.ChangeAvatar(channel, null, AssetLoadConfig.instance.asyncLoad, highPriority);
                }
                else
                {
                    if (tableData != null)
                    {
                        int resID = tableData.ResID;
                        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
                        if (resData != null)
                        {
                            if (geActor != null)
                                geActor.ChangeAvatar(channel, resData.ModelPath, AssetLoadConfig.instance.asyncLoad, highPriority, prodId);
                            if (avatarRenderer != null)
                                avatarRenderer.ChangeAvatar(channel, resData.ModelPath, AssetLoadConfig.instance.asyncLoad,highPriority);
                        }
                    }
                }
            }
        }

        //ItemTable EFashionWearSlotType Channel 三种类型之间对应关系
        public void GetFashionSlotChangedType(ref EFashionWearSlotType slot, ref GeAvatarChannel channel, ItemTable itemTable, bool UseTableType = false)
        {
            //itemTable 为null 的时候 EFashionWearSlotType 和 channel一一对应

            if (!UseTableType)
            {
                switch (slot)
                {
                    case EFashionWearSlotType.Head:
                        channel = GeAvatarChannel.Head;
                        break;
                    case EFashionWearSlotType.Waist:
                        channel = GeAvatarChannel.Bracelet;
                        break;
                    case EFashionWearSlotType.UpperBody:
                        channel = GeAvatarChannel.UpperPart;
                        break;
                    case EFashionWearSlotType.LowerBody:
                        channel = GeAvatarChannel.LowerPart;
                        break;
                    case EFashionWearSlotType.Chest:
                        channel = GeAvatarChannel.Headwear;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                slot = EFashionWearSlotType.Invalid;
                channel = GeAvatarChannel.MaxChannelNum;

                if(itemTable != null)
                {
                    ItemTable.eSubType sub = itemTable.SubType;

                    switch (sub)
                    {
                        case ItemTable.eSubType.FASHION_HEAD:
                            {
                                slot = EFashionWearSlotType.Head;
                                channel = GeAvatarChannel.Head;
                            }
                            break;
                        case ItemTable.eSubType.FASHION_SASH:
                            {
                                slot = EFashionWearSlotType.Waist;
                                channel = GeAvatarChannel.Bracelet;
                            }
                            break;
                        case ItemTable.eSubType.FASHION_CHEST:
                            {
                                slot = EFashionWearSlotType.UpperBody;
                                channel = GeAvatarChannel.UpperPart;
                            }
                            break;
                        case ItemTable.eSubType.FASHION_LEG:
                            {
                                slot = EFashionWearSlotType.LowerBody;
                                channel = GeAvatarChannel.LowerPart;
                            }
                            break;
                        case ItemTable.eSubType.FASHION_EPAULET:
                            {
                                slot = EFashionWearSlotType.Chest;
                                channel = GeAvatarChannel.Headwear;
                            }
                            break;
                        case ItemTable.eSubType.FASHION_HAIR:
                            {
                                /// 这个是正常情况，不需要处理
                            }
                            break;
                        default:
                            {
                                Logger.LogErrorFormat("该时装部位外观未处理，对应表格SubType：{0}", sub);
                            }
                            break;
                    }
                }
            }
        }

        public void AvatarEquipWeaponFromRes(IGeAvatarActor avatarRenderer, string weaponPath, string weaponLocatorPath, GeActorEx geActor = null,bool highPriority = false, string defaultWeaponPath=null)
        {
            if (avatarRenderer == null && geActor == null)
                return;

            if (Utility.IsStringValid(weaponPath))
            {
                if (geActor != null)
                {
                    string assetPath = weaponPath;
                    // 如果启用了AvatarFallback，先检查资源是否存在。没有下载当前武器就用默认武器
                    if (GeAvatarFallback.IsAvatarPartFallbackEnabled() && !GeAvatarFallback.IsAssetDependentAvaliable(assetPath) && !string.IsNullOrEmpty(defaultWeaponPath))
                    {
/*
                        assetPath = GeAvatarFallback.GetFallbackAvatar(-1, GeAvatarChannel.Weapon, assetPath);
                        if (string.IsNullOrEmpty(assetPath))
                        {
                            assetPath = weaponPath;
                            //return;
                        }*/

                        assetPath = defaultWeaponPath;
                    }

                    geActor.CreateAttachment(
						Global.WEAPON_ATTACH_NAME,
                        assetPath,
                        weaponLocatorPath,
                        false,
                        true,
                        highPriority
                    );

					geActor.ChangeAction (geActor.GetCurActionName(),1.0f,true);
                }
                else if (avatarRenderer != null)
                {
                    avatarRenderer.AttachAvatar(
						Global.WEAPON_ATTACH_NAME,
                        weaponPath,
                        weaponLocatorPath,
                        false,
                        true,
                        highPriority
                    );
					avatarRenderer.ChangeAction (avatarRenderer.GetCurActionName(),1.0f, true);
                }
            }
        }
        //替换武器，如果有wid为0，会从职业属性表里拿默认的武器
        public void AvatarEquipWeapon(IGeAvatarActor avatarRenderer, int jobID, int wid, GeActorEx geActor = null,bool highPriority = false)
        {
            if (avatarRenderer == null && geActor == null)
                return;

            ProtoTable.JobTable job = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(jobID);
            if (job != null)
            {
                string weaponPath = job.DefaultWeaponPath;
                string locator = job.DefaultWeaponLocator;

                if (wid > 0)
                {
                    var data = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(wid);
                    if (data != null)
                    {
                        int resID = data.ResID;
                        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
                        if (resData != null)
                        {
                            weaponPath = resData.ModelPath;
                        }
                    }
                }

				AvatarEquipWeaponFromRes(avatarRenderer, weaponPath, locator, geActor,highPriority, job.DefaultWeaponPath);
			
/*                //枪手不挂
                if (jobID >= 20 && jobID < 30)
                {

                }
                else
                {
                    
                }*/
            }
        }

        public string GetWeaponResFormID(int wid)
        {
            if (wid > 0)
            {
                var data = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(wid);
                if (data != null)
                {
                    if (data.SubType != ItemTable.eSubType.WEAPON)
                        return null;

                    int resID = data.ResID;
                    var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
                    if (resData != null)
                    {
                        return resData.ModelPath;
                    }
                }
            }

            return null;
        }

        //替换武器,读取角色当前的武器装备
        public void AvatarEquipCurrentWeapon(IGeAvatarActor avatarRenderer, GeActorEx geActor = null,bool highPriority = false)
        {
            if (avatarRenderer == null && geActor == null)
                return;

			int sl = 0;
            int wid = 0;
            ulong uwid = ItemDataManager.GetInstance().GetWearEquipBySlotType(EEquipWearSlotType.EquipWeapon);
            if (uwid > 0)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(uwid);
                if (itemData != null)
                {
                    wid = (int)itemData.TableID;
                    if (itemData.StrengthenLevel > 0)
                        sl = itemData.StrengthenLevel;
                }
            }

            AvatarEquipWeapon(avatarRenderer, PlayerBaseData.GetInstance().JobTableID, wid, geActor);
			if (sl >= 0)
				AvatarShowWeaponStrengthen(avatarRenderer, sl, geActor);
        }

        /// <summary>
        /// 预览整套时装，需要把身上穿戴的时装脱掉，只留翅膀、光环、武器
        /// </summary>
        /// <param name="avatarRenderer"></param>
        /// <param name="geActor"></param>
        public void AvatarEquipFromPreviewCompleteFashion(IGeAvatarActor avatarRenderer, GeActorEx geActor = null)
        {
            if (avatarRenderer == null && geActor == null)
                return;

            var avatar = PlayerBaseData.GetInstance().avatar;
            if (avatar == null)
                return;

            UInt32[] equipItemIds = BeUtility.CopyVector(avatar.equipItemIds);

            for (int i = 0; i < equipItemIds.Length; i++)
            {
                UInt32 itemId = equipItemIds[i];
                var equipedItemData = TableManager.GetInstance().GetTableItem<ItemTable>((int)itemId);
                if (equipedItemData != null)
                {
                    if (equipedItemData.SubType == ItemTable.eSubType.FASHION_HEAD ||
                        equipedItemData.SubType == ItemTable.eSubType.FASHION_SASH ||
                        equipedItemData.SubType == ItemTable.eSubType.FASHION_CHEST ||
                        equipedItemData.SubType == ItemTable.eSubType.FASHION_LEG ||
                        equipedItemData.SubType == ItemTable.eSubType.FASHION_EPAULET)
                    {
                        equipItemIds[i] = 0;
                    }
                }
            }

            PlayerBaseData.GetInstance().AvatarEquipFromItems(avatarRenderer,
              equipItemIds,
              JobTableID,
              (int)(avatar.weaponStrengthen),
              geActor,
              false,
              avatar.isShoWeapon);
        }

        //avatar重置为当前的武器和时装
        public void AvatarEquipFromCurrentEquiped(IGeAvatarActor avatarRenderer, GeActorEx geActor = null,bool highPriority = false)
        {
            if (avatarRenderer == null && geActor == null)
                return;

            var avatar = PlayerBaseData.GetInstance().avatar;
            if (avatar == null)
                return;


            PlayerBaseData.GetInstance().AvatarEquipFromItems(avatarRenderer,
                avatar.equipItemIds,
                JobTableID,
                (int) (avatar.weaponStrengthen),
                geActor,
                false,
                avatar.isShoWeapon);

            /*
            //武器
            AvatarEquipCurrentWeapon(avatarRenderer, geActor,highPriority);

			//翅膀
			AvatarEquipCurrentWing(avatarRenderer, geActor,highPriority);

            //时装
            for (int i = 1; i < (int)EFashionWearSlotType.Max; ++i)
            {
                EFashionWearSlotType slotType = (EFashionWearSlotType)i;
                GeAvatarChannel channel = GeAvatarChannel.MaxChannelNum;
                if (slotType == EFashionWearSlotType.UpperBody)
                    channel = GeAvatarChannel.UpperPart;
                else if (slotType == EFashionWearSlotType.LowerBody)
                    channel = GeAvatarChannel.LowerPart;
                else if (slotType == EFashionWearSlotType.Head)
                    channel = GeAvatarChannel.Head;

                if (channel == GeAvatarChannel.MaxChannelNum)
                    continue;


                int wid = 0;
                ulong uwid = ItemDataManager.GetInstance().GetFashionWearEquipBySlotType((EFashionWearSlotType)i);
                if (uwid > 0)
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(uwid);
                    if(itemData != null)
                        wid = (int)itemData.TableID;
                }

                AvatarEquipPart(avatarRenderer, slotType, wid, geActor, highPriority);
            }*/

//             if (geActor != null)
//                 geActor.SuitAvatar(AssetLoadConfig.instance.asyncLoad, highPriority);
//             if (avatarRenderer != null)
//                 avatarRenderer.SuitAvatar(AssetLoadConfig.instance.asyncLoad, highPriority);
        }

        public void AvatarEquipFromItems(IGeAvatarActor avatarRenderer, 
            UInt32[] equipItemIds, 
            int jobID, 
            int weaponStrength = 0, 
            GeActorEx geActor = null, 
            bool forceAdditive = false, 
            byte isShowFashionWeapon = 0)
        {
            if (avatarRenderer == null && geActor == null)
                return;

            if (equipItemIds == null)
                return;

            bool hasWeapon = false;
            bool hasWing = false;
            bool hasHalo = false;

            var tempEquipItemIds = BeUtility.CopyVector(equipItemIds);

            BeUtility.DealWithFashion(tempEquipItemIds);



            var isEquipedFashionWeapon = PackageDataManager.GetInstance().IsEquipedFashionWeapon(equipItemIds);
            bool isShowFashionWeaponFlag = isShowFashionWeapon == 1 ? true : false;

            List<EFashionWearSlotType> slotEquiped = new List<EFashionWearSlotType>();

            bool bIsInChijiScene = false;

            ClientSystemGameBattle town = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if(town != null)
            {
                bIsInChijiScene = true;
            }

            //weapon & fashion
            for (int i = 0; i < tempEquipItemIds.Length; ++i)
            {
                int itemID = (int)tempEquipItemIds[i];
                if (itemID <= 0)
                    continue;

                var data = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
                if (data != null)
                {
                    if (data.SubType == ItemTable.eSubType.WEAPON)
                    {

                        if (isShowFashionWeaponFlag == false
                            || isEquipedFashionWeapon == false)
                        {
                            hasWeapon = true;
                            AvatarEquipWeapon(avatarRenderer, jobID, itemID, geActor);
                            //武器特效
                            if (weaponStrength >= 0)
                                AvatarShowWeaponStrengthen(avatarRenderer, weaponStrength, geActor, forceAdditive);
                        }
                    }
                    else if (data.SubType == ItemTable.eSubType.FASHION_HAIR)
                    {
                        hasWing = true;
                        AvatarEquipWing(avatarRenderer, itemID, geActor);
                    }
                    else if (data.SubType == ItemTable.eSubType.FASHION_AURAS)
                    {
                        hasHalo = true;
                        AvatarEquipHalo(avatarRenderer, itemID, geActor);
                    }
                    else if (data.SubType.ToString().Contains("FASHION"))
                    {
                        if (data.SubType == ItemTable.eSubType.FASHION_WEAPON)
                        {
                            //显示武器时装
                            if (isShowFashionWeaponFlag == true)
                            {
                                //装备时装武器
                                hasWeapon = true;
                                AvatarEquipWeapon(avatarRenderer, jobID, itemID, geActor);
                                //显示武器装备的等级特效
                                if (weaponStrength >= 0)
                                    AvatarShowWeaponStrengthen(avatarRenderer, weaponStrength, geActor, forceAdditive);
                            }
                        }
                        else
                        {
                            var slotType = (EFashionWearSlotType) (data.SubType - 10);
                            AvatarEquipPart(avatarRenderer, slotType, itemID, geActor,jobID);

                            slotEquiped.Add(slotType);
                        }
                    }
                    else
                    {
                        if(bIsInChijiScene)
                        {
                            EFashionWearSlotType slotType = _GetChijiEquipShowSlot(data);
                            AvatarEquipPart(avatarRenderer, slotType, itemID, geActor,jobID);
                            slotEquiped.Add(slotType);
                        }
                    }
                }
            }

            //没有穿时装的部分加载默认装扮
            EFashionWearSlotType[] fashionSlots = new EFashionWearSlotType[] {
                EFashionWearSlotType.UpperBody,
                EFashionWearSlotType.LowerBody,
                EFashionWearSlotType.Head,
                EFashionWearSlotType.Waist,
                EFashionWearSlotType.Chest};

            for(int i=0; i<fashionSlots.Length; ++i)
            {
               if (!slotEquiped.Contains(fashionSlots[i]))
                    AvatarEquipPart(avatarRenderer, fashionSlots[i], 0, geActor,jobID);
            }

            if (!hasWeapon)
			{
				AvatarEquipWeapon(avatarRenderer, jobID, 0, geActor);
                AvatarShowWeaponStrengthen(avatarRenderer, weaponStrength, geActor, forceAdditive);
            }

            if (!hasWing)
            {
                AvatarEquipWing(avatarRenderer, 0, geActor);
            }

            if (!hasHalo)
            {
                AvatarEquipHalo(avatarRenderer, 0, geActor);
            }

            if (geActor != null)
                geActor.SuitAvatar(AssetLoadConfig.instance.asyncLoad,false,jobID);
            if (avatarRenderer != null)
                avatarRenderer.SuitAvatar(AssetLoadConfig.instance.asyncLoad);
        }

        private EFashionWearSlotType _GetChijiEquipShowSlot(ItemTable data)
        {
            EFashionWearSlotType slotType = EFashionWearSlotType.Max;

            // 就按装备和时装的对应槽位映射，不用管具体部位是否一致 by Wangbo 2019.04.11
            if (data.SubType == ItemTable.eSubType.HEAD)
            {
                slotType = EFashionWearSlotType.Head;
            }
            else if (data.SubType == ItemTable.eSubType.CHEST)
            {
                slotType = EFashionWearSlotType.UpperBody;
            }
            else if (data.SubType == ItemTable.eSubType.BELT)
            {
                slotType = EFashionWearSlotType.Chest;
            }
            else if (data.SubType == ItemTable.eSubType.LEG)
            {
                slotType = EFashionWearSlotType.Waist;
            }
            else if (data.SubType == ItemTable.eSubType.BOOT)
            {
                slotType = EFashionWearSlotType.LowerBody;
            }

            return slotType;
        }

        public void AvatarShowWeaponStrengthen(IGeAvatarActor avatarRenderer, int strengthenLevel=0, GeActorEx geActor = null,bool forceAddtive = false)
		{
			if (avatarRenderer != null && geActor != null)
				return;

			GeAttach weapon = null;

			if (avatarRenderer != null)
			{
				weapon = avatarRenderer.GetAttachment(Global.WEAPON_ATTACH_NAME);

			}
			else if (geActor != null) {
				weapon = geActor.GetAttachment(Global.WEAPON_ATTACH_NAME);
			}

			if (weapon != null)
				weapon.ChangePhase(BeUtility.GetStrengthenEffectName(weapon.ResPath), strengthenLevel, forceAddtive);
		}

        private void CalRoleTotalExp()
        {
            Dictionary<int, object> ExpInfo = TableManager.GetInstance().GetTable<ExpTable>();

            foreach (var expdata in ExpInfo)
            {
                ExpTable data = expdata.Value as ExpTable;

                if (data.ID < Level)
                {
                    Exp += (ulong)data.TotalExp;
                }
                else
                {
                    break;
                }
            }

            Exp += CurExp;
        }

#region 翅膀
		public void AvatarEquipWingFromRes(IGeAvatarActor avatarRenderer, string weaponPath, string weaponLocatorPath, GeActorEx geActor = null,bool highPriority = false, string attachName="")
		{
			if (avatarRenderer == null && geActor == null)
				return;

			if (Utility.IsStringValid(weaponPath))
			{
				if (geActor != null)
				{
                    string assetPath = weaponPath;

                    // 如果启用了AvatarFallback，先检查资源是否存在，不存在不加载翅膀。
                    if (GeAvatarFallback.IsAvatarPartFallbackEnabled() && !GeAvatarFallback.IsAssetDependentAvaliable(assetPath))
                    {
                        return;
                    }
					geActor.CreateAttachment(
                        attachName,
						weaponPath,
						weaponLocatorPath,
                        false,
                        true,
                        highPriority
					);
				}
				else if (avatarRenderer != null)
				{
					avatarRenderer.AttachAvatar(
                        attachName,
						weaponPath,
						weaponLocatorPath,
                        false,
                        true,
                        highPriority
					);
				}
			}
		}
        
        public void SetFootIndocator(IGeAvatarActor avatarRenderer, GeActorEx geActor = null, bool show = true)
        {
            if (avatarRenderer != null)
            {
                var attachment = avatarRenderer.GetAttachment(Global.FOOT_INDICATOR_ATTACH_NAME);
                if (attachment != null)
                {
                    attachment.SetVisible(show);

                }
            }

            if (geActor != null)
            {
                var attachment = geActor.GetAttachment(Global.FOOT_INDICATOR_ATTACH_NAME);
                if (attachment != null)
                {
                    attachment.SetVisible(show);
                }
            }
        }

        public void AvatarEquipHalo(IGeAvatarActor avatarRenderer, int itemID, GeActorEx geActor = null, bool highPriority = false)
        {
            SetFootIndocator(avatarRenderer, geActor, itemID < 0);

            AvatarEquipAttach(avatarRenderer, itemID, geActor, highPriority, Global.HALO_ATTACH_NAME, Global.HALO_LOCATOR_NAME);
        }

		public void AvatarEquipWing(IGeAvatarActor avatarRenderer, int itemID, GeActorEx geActor = null,bool highPriority = false)
		{
            AvatarEquipAttach(avatarRenderer, itemID, geActor, highPriority, Global.WING_ATTACH_NAME, Global.WING_LOCATOR_NAME);
		}

        public void AvatarEquipAttach(IGeAvatarActor avatarRenderer, int itemID, GeActorEx geActor = null, bool highPriority = false, string attachName="", string attachNode = null)
        {
            if (avatarRenderer == null && geActor == null)
                return;

            if (itemID > 0)
            {
                var path = Utility.GetItemModulePath(itemID);
                AvatarEquipWingFromRes(avatarRenderer, path, attachNode, geActor, highPriority, attachName);
            }
            else
            {
                if (geActor != null)
                {
                    var attach = geActor.GetAttachment(attachName);
                    if (attach != null)
                    {
                        geActor.DestroyAttachment(attach);

                    }
                        
                }
                if (avatarRenderer != null)
                {
                    var attach = avatarRenderer.GetAttachment(attachName);
                    if (attach != null)
                    {

                        avatarRenderer.RemoveAttach(attach);
                    }
                        
                }

                if (attachName == Global.HALO_ATTACH_NAME)
                    SetFootIndocator(avatarRenderer, geActor, true);

            }
        }


#endregion

#region  PackageSize

        private void UpdatePackSizeByType(string packageTypeStr)
        {
            //格式："5,100|10,100" 类型为5的道具的基础格子为100；

            if (string.IsNullOrEmpty(packageTypeStr) == true)
                return;

            var typeArray = packageTypeStr.Split('|');
            if (typeArray.Length > 0)
            {
                for (var i = 0; i < typeArray.Length; i++)
                {
                    var curTypeStr = typeArray[i];

                    if (string.IsNullOrEmpty(curTypeStr) == true)
                        continue;

                    var valueArray = curTypeStr.Split(',');
                    if (valueArray.Length == 2)
                    {
                        var firstNumber = int.Parse(valueArray[0]);
                        var secondNumber = int.Parse(valueArray[1]);
                        if ((EPackageType)firstNumber == EPackageType.Fashion || (EPackageType)firstNumber == EPackageType.Inscription)
                        {
                            FashionPackBaseSize = secondNumber;
                        }
                        else if ((EPackageType)firstNumber == EPackageType.Bxy || (EPackageType)firstNumber == EPackageType.Sinan)
                        {
                            FashionPackBaseSize = secondNumber;
                        }
                        else if ((EPackageType)firstNumber == EPackageType.Title)
                        {
                            TitlePackBaseSize = secondNumber;
                        }
                    }
                }
            }

            //时装的基础数量
            var fashionPackIndex = (int)EPackageType.Fashion;
            PackTotalSize[fashionPackIndex] = FashionPackBaseSize;
            if (fashionPackIndex < PackAddSize.Count)
                PackTotalSize[fashionPackIndex] += PackAddSize[fashionPackIndex];

            {
                //铭文的基础数量
                var InscriptionPackIndex = (int)EPackageType.Inscription;
                PackTotalSize[InscriptionPackIndex] = FashionPackBaseSize;
                if (InscriptionPackIndex < PackAddSize.Count)
                    PackTotalSize[InscriptionPackIndex] += PackAddSize[InscriptionPackIndex];
            }

            {
                //辟邪玉的基础数量
                var bxyPackIndex = (int)EPackageType.Bxy;
                PackTotalSize[bxyPackIndex] = FashionPackBaseSize;
                if (bxyPackIndex < PackAddSize.Count)
                    PackTotalSize[bxyPackIndex] += PackAddSize[bxyPackIndex];
            }

            {
                //司南的基础数量
                var sinanPackIndex = (int)EPackageType.Sinan;
                PackTotalSize[sinanPackIndex] = FashionPackBaseSize;
                if (sinanPackIndex < PackAddSize.Count)
                    PackTotalSize[sinanPackIndex] += PackAddSize[sinanPackIndex];
            }

            //背包的基础数量
            var titlePackIndex = (int)EPackageType.Title;
            PackTotalSize[titlePackIndex] = TitlePackBaseSize;
            if (titlePackIndex < PackAddSize.Count)
                PackTotalSize[titlePackIndex] += PackAddSize[titlePackIndex];
        }

#endregion

        #region GeAvatarLayerManager
        private int[] AllGeAvatarCanUseLayers = new int[] { 20, 21, 22, 23, 24, 25, 26, 27, 28 };
        private List<int> UsingLayers = new List<int>();

        public int UseGeAvatarLayer()
        {
            if(AllGeAvatarCanUseLayers != null)
            {
                if(UsingLayers == null)
                {
                    UsingLayers = new List<int>();
                }

                for(int i = 0; i < AllGeAvatarCanUseLayers.Length; i++)
                {
                    int findLayer = UsingLayers.Find(x => { return AllGeAvatarCanUseLayers[i] == x; });
                    
                    if(findLayer <= 0)
                    {
                        UsingLayers.Add(AllGeAvatarCanUseLayers[i]);
                        return AllGeAvatarCanUseLayers[i];
                    }
                }

                Logger.LogErrorFormat("AllGeAvatarCanUseLayers所有层级都在使用中,没有多余的layer可供使用，请扩充AllGeAvatarCanUseLayers的层级数目，或检查当前场景下是否是GeAvatar因不正常原因导致创建过多");
                return -1;
            }
            else
            {
                Logger.LogErrorFormat("AllGeAvatarCanUseLayers == null，检查初始化问题");
                return -1;
            }
        }

        public void ReleaseGeAvatarLayer(int layer)
        {
            if(UsingLayers != null)
            {
                int index = UsingLayers.FindIndex(x => { return layer == x; });

                if(index >= 0)
                {
                    UsingLayers.RemoveAt(index);
                }
            }
        }

        public bool IsLayerValid(int layer)
        {
            if (AllGeAvatarCanUseLayers != null)
            {
                for(int i = 0; i < AllGeAvatarCanUseLayers.Length; i++)
                {
                    if(AllGeAvatarCanUseLayers[i] == layer)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsUsingLayer(int layer)
        {
            if (UsingLayers != null)
            {
                int index = UsingLayers.FindIndex(x => { return layer == x; });

                if (index >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddLayer(int layer)
        {
            if (UsingLayers != null)
            {
                UsingLayers.Add(layer);
            }
        }
        #endregion
    }
}
