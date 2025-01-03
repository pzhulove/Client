using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
//using ProtoBuf;
using System;
using ProtoTable;
using System.ComponentModel;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class TableManager : Singleton<TableManager>
{
#if USE_FB_TABLE
    private const string kTablePath = "Data/table_fb/";
#else
    private const string kTablePath = "Data/table/";
#endif

    private bool bInit = false;
    static public bool bNeedUninit = false;

    private Type[] mTypeList =
    {
        typeof(GlobalSettingTable),
        typeof(MonsterAttributeTable),
        typeof(JobTable),
        typeof(EffectTable),
        typeof(FightPackageTable),
        typeof(SkillTable),
        typeof(SkillInputTable),
        typeof(MechanismTable),
        typeof(EqualItemTable),
        typeof(BuffInfoTable),
        typeof(BuffDrugConfigTable),
        typeof(BuffTable),
        typeof(BuffDrugConfigInfoTable),
        typeof(ObjectTable),
        typeof(ResTable),
        typeof(SceneRegionTable),
        typeof(ExpTable),
        typeof(MissionTable),
        typeof(DungeonTable),
        typeof(QuickBuyTable),
        typeof(DungeonUIConfigTable),
        typeof(ItemTable),
        typeof(ItemCollectionTable),
        typeof(TalkTable),
        typeof(NpcTable),
        typeof(DungeonTimesTable),
        typeof(PkLevelTable),
        typeof(EquipStrengthenTable),
        typeof(CitySceneTable),
        typeof(DestrucTable),
        typeof(UnitTable),
        //typeof(DungeonArgsTable),
        typeof(NameTable),
        typeof(MonsterSpeech),
        typeof(TipsTable),
        typeof(CommonTipsDesc),
        typeof(ShopItemTable),
        typeof(MonsterPrefixTable),
        typeof(AuctionClassify),
        typeof(ShopTable),
        typeof(TeamNameTable),
        typeof(SkillDescriptionTable),
        typeof(FaceTable),
        typeof(EquipSuitTable),
        typeof(EquipMasterTable),
        typeof(AnnounceTable),
        typeof(EquipAttrTable),
        typeof(FunctionUnLock),
        typeof(ActiveTable),
        typeof(MagicCardTable),
        typeof(ActivityDungeonTable),
        typeof(MallTypeTable),
        typeof(AreaTable),
        //typeof(MainlandTable),
        typeof(ActiveMainTable),
        typeof(ItemConfigTable),
        typeof(SystemValueTable),
        typeof(SeasonDailyTable),
        typeof(RewardAdapterTable),
        typeof(VipTable),
        typeof(DeathTowerAwardTable),
        typeof(AcquiredMethodTable),
        typeof(GuildTable),
        typeof(JarBonus),
        typeof(AIConfigTable),
        typeof(JarItemPool),
        typeof(GuildSkillTable),
        typeof(RobotConfigTable),
        //typeof(GuildStatueTable),
        typeof(GuildRoundtableTable),
        typeof(GuildBuildingTable),
        typeof(TestFunctionConfigTable),
        typeof(GuildTerritoryTable),
        typeof(PkHPLevelAdjustTable),
        typeof(PkHPProfessionAdjustTable),
        typeof(RedPacketTable),
        typeof(MissionScoreTable),
        typeof(GuildRewardTable),
        typeof(TeamDungeonTable),
        typeof(StrengthenTicketTable),
        typeof(GuildInspireTable),
        typeof(BudoAwardTable),
        typeof(DungeonDifficultyAdjustTable),
        typeof(NewbieGuideTable),
        typeof(VoiceTable),
        typeof(VipPrivilegeTable),
        typeof(FashionComposeSkyTable),
        typeof(SoundTable),
        typeof(EquipForgeTable),
        typeof(EquipStrModTable),
        typeof(LoadingResourcesTable),
        typeof(DefaultAdminServerTable),
        typeof(GuidanceMainTable),
        typeof(GuidanceMainItemTable),
        typeof(GuidanceEntranceTable),
        typeof(NotificationTable),
        typeof(SeasonLevelTable),
        //typeof(SeasonTable),
        typeof(SwitchClientFunctionTable),
        typeof(SeasonAttrTable),
        typeof(GuidanceLevelTable),
        typeof(AuctionBoothTable),
        typeof(CurrencyConfigureTable),
        //typeof(FashionAttributesTable),
        typeof(FashionAttributesConfigTable),
        typeof(EquipQLValueTable),
        typeof(ShopMainFrameTable),
        typeof(FashionComposeTable),
        typeof(ActivityJarTable),
        typeof(JarItemShowPool),
        typeof(ChargeMallTable),
        typeof(ChargeGearTable),
        typeof(GiftTable),
        //typeof(PushExhibitionTable),
        typeof(LiaoTianDynamicTextureTable),
        typeof(PetTable),
        typeof(GiftPackTable),
        typeof(PetFeedTable),
        typeof(PetDialogBaseTable),
        typeof(FatigueMakeUp),
        typeof(FatigueMakeUpPrice),
        typeof(PetLevelTable),
        typeof(LegendMainTable),
        typeof(LegendTraceTable),
        typeof(LinkTable),
        typeof(DungeonDailyDropTable),
        typeof(SuperLinkTable),
        typeof(MallGiftPackTable),
        typeof(MallItemTable),
        typeof(EquipScoreTable),
        typeof(MallLimitTimeActivity),
        typeof(PersonalTailorTriggerTable),
        typeof(BeadTable),
        typeof(PremiumLeagueTimeTable),
        typeof(DrawPrizeTable),
        typeof(RewardPoolTable),
        typeof(OpActivityTable),
        typeof(OpActivityTaskTable),
        typeof(JuedouchangItemPropellingTable),
        typeof(AchievementGroupMainItemTable),
        typeof(AchievementLevelInfoTable),
        typeof(AchievementGroupSubItemTable),
        typeof(AchievementGroupSecondMenuTable),
        typeof(ScreenShakeTable),
        typeof(EquipmentRelationTable),
        typeof(EquipHandbookAttachedTable),
        typeof(EquipHandbookCommentTable),
        typeof(EquipHandbookContentTable),
        typeof(EquipHandbookSourceTable),
        typeof(EquipHandbookCollectionTable),
        typeof(EquipTransMapTable),
        typeof(EquipRecoveryRewardTable),
        typeof(EquipRecoveryPriceTable),
        typeof(ChampionBattleTable),
        typeof(EquipRecoScUpConsRtiTable),
        typeof(ChapterTable),
        typeof(OneKeyWearTable),
        typeof(NewBieGuideJobData),
        typeof(InstituteTable),
        typeof(MoneyManageTable),
        typeof(LevelAdapterTable),
        typeof(ItemNotifyGetTable),
        typeof(ChargeDisplayTable),
        typeof(MysticalMerchantTable),
        typeof(AreaProvinceTable),
        typeof(TAPQuestionnaireTable),
        typeof(MasterSysGiftTable),
        typeof(ScoreWarRewardTable),
        typeof(DigMapTable),
        typeof(BetHorseShooter),
        typeof(BetHorseCfg),
        typeof(BetHorseMap),
        typeof(SkillPreTable),
        typeof(SkillPreInfoTable),
        typeof(FriendWelfareAddTable),
        typeof(BuffRandomTable),
        typeof(SDKClientResTable),
        typeof(RemovejewelsTable),
        typeof(StrengthenTicketSynthesisTable),
        typeof(StrenTicketFuseMaterialTable),
        typeof(StrengthenTicketFuseTable),
        typeof(ChangeFashionActiveMergeTable),
        typeof(BeadRandomBuff),
        typeof(GuildDungeonRewardTable),
        typeof(GuildDungeonLvlTable),
        typeof(AuctionNewFrameTable),
        typeof(AuctionNewFilterTable),
        typeof(EquipInitialAttribute),
        typeof(ItemBuffTable),
        typeof(GuildDungeonTypeTable),
        typeof(ReplacejewelsTable),
        typeof(ArtifactJarLotteryTable),
        typeof(ShopNewFilterTable),
        typeof(CommonHelpTable),
        typeof(BlackMarketTable),
        typeof(EquieUpdateTable),
        typeof(PveTrainingMonsterTable),
        typeof(ExpeditionMapTable),
        typeof(ExpeditionMapRewardTable),
        typeof(DungeonModelTable),
        typeof(ComboScoreTable),
        typeof(AdventureTeamTable),
        typeof(PictureFrameTable),
        typeof(WeekSignTable),
        typeof(WeekSignSumTable),
        typeof(GuildEmblemTable),
        typeof(NewTitleTable),
        typeof(GuildBuildInfoTable),
        typeof(ChiJiTimeTable),
        typeof(ChijiItemTable),
        typeof(ChijiBuffTable),
        typeof(ChiJiNpcRewardTable),//C-吃鸡Npc奖励表
        typeof(ChijiEffectMapTable),//吃鸡触发效果映射表
        typeof(ChijiSkillMapTable), //吃技能映射表加载
        typeof(MagicCardProbabilityTable),
        typeof(GuildActivityTable),
        typeof(AuctionMagiStrengAdditTable),
        typeof(EqualPvPEuqipTable),
        typeof(AssistEqStrengFouerDimAddTable),
        typeof(DailyTodoTable),//M-每日必做表.xls
        typeof(EquipEnhanceAttributeTable),//Z-装备增幅属性表.xls
        typeof(EquipEnhanceCostTable),//Z-装备增幅消耗表.xls
        typeof(MaterialsSynthesis),  //C-材料合成表.xls
		typeof(AdventureTeamTitleTypeTable),//Y-佣兵团头衔类型表.xls
        typeof(MonthSignAward),//Y-月签到奖励表.xls
        typeof(MonthSignCollectAward),//Y-月签到累计奖励表.xls
        typeof(TeamCopyFieldTable),             //团本据点
        typeof(TeamCopyFlopTable),              //团本小阶段翻牌
        typeof(TeamCopyTargetTable),            //团本目标
        typeof(TeamCopyStageTable),             //团本阶段
        typeof(TeamCopyValueTable),             //团本数值
        typeof(TeamDuplicationSetTable),        //团本设置
        typeof(AdventureTeamGradeTable),//Y-佣兵团评级表.xls
        typeof(UltimateChallengeBuffTable),//终极试炼buff表
        typeof(UltimateChallengeDungeonTable),//终极试炼地下城表
        typeof(MallShopMultiITable),           //S-商城多倍积分商品表.xls
        typeof(FashionDecomposeTable),           //M-时装分解铭文表.xls
        typeof(EquipInscriptionHoleTable),       //M-铭文孔对应装备表.xls
        typeof(InscriptionHoleSetTable),        //M-铭文孔镶嵌铭文表.xls
        typeof(InscriptionTable),               //M-铭文表.xls
        typeof(InscriptionExtractTable),        //M-铭文摘取表.xls
        typeof(InscriptionProbabilityTable),    //M-铭文概率对应表.xls
        typeof(CurrencyQuickTipsTable),         //H-货币快捷提示表.xls
        typeof(InscriptionSynthesisTable),      //M-铭文合成表.xls
        typeof(AdventurePassRewardTable),      //M-冒险通行证奖励表.xls
        typeof(AdventurePassActivityTable),      //M-冒险通行证活跃度经验表.xls
        typeof(AdventurePassBuyLevelTable),      //M-冒险通行证购买等级表.xls
        typeof(AdventurePassBuyRewardTable),      //M-冒险通行证购买额外奖励.xls
        typeof(EquipBaseScoreModTable),          //Z-装备基础评分系数表.xls
        typeof(EquipBaseScoreTable),             //Z-装备基础评分表.xls
        typeof(MagicCardUpdateTable),             //附魔卡升级表
        typeof(ChijiRewardTable), // 吃鸡奖励表
        typeof(RandomDungeonTable), //D-地宫活动地下城表
        typeof(ScoreWar2v2RewardTable), //J-积分赛奖励表2v2.xls
        typeof(BeadConvertCostTable), //宝珠转换消耗合成表
        typeof(EquipStrModIndAtkTable),//装备强化系数表_固定攻击
#if ROBOT_TEST
        typeof(AutoFightTest),
#endif
        typeof(HonorLevelTable),
        typeof(HonorPlayerTable),
        typeof(EquieChangeTable),
        typeof(EquChangeConsumeTable),
        typeof(ChiJiShopTable),
        typeof(ChiJiShopItemTable),
        typeof(WeekSignSpringTable),
        typeof(ChiJiPkSceneTable),
        typeof(HireTask),
        typeof(TeamRewardTable),        //组队奖励表
        typeof(HonorRewardTable),
        typeof(LimiteBargainShowTable),  //限时团购展示表
        typeof(WholeBargainDiscountTable), //全民砍价折扣表
		typeof(GuildBattleScoreRankRewardTable), //G-公会战积分排名奖励表.xls
        typeof(GuildBattleTimeTable), // 公会战时间表
        typeof(DungeonFunctionSwitchTable), //地下城功能开关表
        typeof(HelpFrameContentTable),// 帮助界面文本表
        typeof(ComItemConfigTable),// ComItem配置表
        typeof(ClientConstValueTable),// 客户端系统常量表
        typeof(EffectInfoTable),//T-特效信息表.xls
#if MG_TESTIN
        typeof(TestInDes),
#endif
#if !SERVER_LOGIC
        typeof(HitEffectInfoTable),
#endif
        typeof(AttrDescTable),//属性描述表
        typeof(DecomposeTable),//装备分解表
        typeof(PackageEnlargeTable),//背包扩展表
#if !SERVER_LOGIC
        typeof(RankWeaponTable),//排行榜武器表
        typeof(SevenDaysActiveTable),//排行榜武器表
        typeof(ChangeOccuBattleConfigTable),
#endif
        typeof(MonsterInstanceInfoTable),
        typeof(TalentTable),
        typeof(GoldRushTable),//失落的黄金之国表
        typeof(MagicCardUpgradeTable),//附魔卡升级表新
        typeof(SmithShopFilterTable),//缎冶筛选项表
        typeof(SummonInfoTable),//召唤信息表
    };


    private Dictionary<Type, Dictionary<int, object>> mTypeTableDict = new Dictionary<Type, Dictionary<int, object>>();

    private GlobalSettingTable mGst = null;
    public GlobalSettingTable gst
    {
        get 
        {
#if UNITY_EDITOR && !LOGIC_SERVER
            if (null == mGst)
            {
                if (!Application.isPlaying)
                {
                    UnityEngine.Debug.LogErrorFormat("在GlobalSettingInspector代码中，请使用 mGlobalSetting.GetValue<xxx>(xxx);");
                }

                return skEmpty;
            }
#endif
            return mGst;
        }
        set 
        {
            mGst = value;
        }
    }


#if UNITY_EDITOR
    private static GlobalSettingTable skEmpty = new GlobalSettingTable();
    public Dictionary<int, object> AddTableInEditorMode(Type type)
    {
        if (mTypeTableDict.ContainsKey(type))
        {
            return mTypeTableDict[type];
        }

        Dictionary<int, object> tableData = _loadTable(type);
        mTypeTableDict.Add(type, tableData);
        return tableData;
    }

    public void ReloadTable(Type t)
    {
        if (mTypeTableDict.ContainsKey(t))
        {
            mTypeTableDict.Remove(t);
        }

        Dictionary<int, object> tableData = _loadTable(t);
        mTypeTableDict.Add(t, tableData);
    }

    public Type[] GetAllTypeListInEditorMode()
    {
        return mTypeList;
    }
#endif

    /// <summary>
    /// 游戏表格初始化（异步）
    /// </summary>
    public IEnumerator _InitCoroutine(UnityAction<int, string> progress)
    {
        if (bInit)
        {
            Logger.LogErrorFormat("[INIT PROCESS]table _InitCoroutine binited!!!!!");

            yield break;
        }
        else
        {
            bInit = true;
            bNeedUninit = true;

            //Logger.LogErrorFormat("[INIT PROCESS]start read table");

            //加载游戏表格
            //---------------------------------------------------------------------
            for (int i = 0; i < mTypeList.Length; i++)
            {
                var curType = mTypeList[i];
                Dictionary<int, object> tableData = _loadTable(curType);
                if (typeof(ItemTable) == curType)
                {
                    _loadTable(curType, typeof(ChijiItemTable), tableData);
                }
                mTypeTableDict.Add(mTypeList[i], tableData);
            }
            //---------------------------------------------------------------------


            //Logger.LogErrorFormat("[INIT PROCESS]end read table");

            if (null != progress) progress(30, null);  //进度30
            yield return new WaitForEndOfFrame();

            //Logger.LogErrorFormat("[INIT PROCESS]start _loadSkillInfo");

            //---------------------------------------------------------------------

#if !SERVER_LOGIC

            _loadSkillInfo();                         //加载技能信息
            _loadExpInfo();                           //初始化升级经验表 
            _loadComboScore();
            _LoadJobInstituteInfo();
            _LoadGiftTable();           //加载礼品表
#endif
            //---------------------------------------------------------------------

            _LoadBornSkillInfo();

            if (null != progress) progress(40, null); //进度40
            yield return new WaitForEndOfFrame();

            //Logger.LogErrorFormat("[INIT PROCESS]start _LoadJobChangeMap");
            //---------------------------------------------------------------------
#if !SERVER_LOGIC

            _LoadJobChangeMap();                     //转职信息
            _LoadOpenSkillBarLevelInfo();            //

#endif

            _loadMonsterAttributeInfo();             // 
            _loadAuctionClassifyInfo();              //
            _loadTreasureDungeonInfo();
            //---------------------------------------------------------------------

            if (null != progress) progress(50, null);  //进度50
            yield return new WaitForEndOfFrame();

            //---------------------------------------------------------------------

#if !SERVER_LOGIC 

            ItemSearchEngine.CreateInstance();        //物品查找引擎初始化 ？？  

#endif

            //---------------------------------------------------------------------

            //Logger.LogErrorFormat("[INIT PROCESS]start _loadPKHPAdjustInfo");
            //---------------------------------------------------------------------
            _loadPKHPAdjustInfo();                   //
            _loadDungeonDifficultyAdjustInfo();      //

#if !SERVER_LOGIC 

            _LoadTeamDungeonInfo();                  //
            _LoadNewbieGuideOrderInfo();             // 

#endif

            //---------------------------------------------------------------------

            if (null != progress) progress(60, null); //进度60
            yield return new WaitForEndOfFrame();
        }
    }
    private void _loadTable(Type originalType, Type realType, Dictionary<int, object> table)
    {
        string filepath = _getTablePath(realType);
#if !USE_FB_TABLE
        byte[] data = null;
#if USE_FB
        filepath = Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(filepath, Utility.kRawDataExtension)).ToLower();
        if(File.Exists(filepath))
            data = System.IO.File.ReadAllBytes(filepath);
        Logger.LogProcessFormat(filepath + "\n");
#else
        AssetBinary textAsset = AssetLoader.instance.LoadRes(filepath, typeof(AssetBinary)).obj as AssetBinary;
        if (null == textAsset)
        {
            Logger.LogError("Load table has failed, table resource:" + filepath);
            return;
        }
        
        data = textAsset.m_DataBytes;
#endif
        if (null != data)
        {
            bool bCanParse = Serializer.CanParse(originalType);
            var IDMap = type.GetProperty("ID").GetGetMethod();
            for (int i = 0; i < data.Length;)
            {
                int len = 0;
                for (int j = i; j < i + 8; ++j)
                {
                    if (data[j] > 0)
                        len = len * 10 + (data[j] - '0');
                }

            i += 8;
            MemoryStream stream = new MemoryStream(data, i, len);
            try
            {
                object tableData = null;

                if(bCanParse)
                {
                    tableData = Serializer.ParseEx(originalType,stream);
                }
                else
                {
                    tableData = Serializer.DeserializeEx(originalType, stream);
                }
                
                if (tableData == null)
                {
                    Logger.LogErrorFormat("table data is nil {0}, {1}", filepath, i);
                }
                else
                {
                    var id = (int)IDMap.Invoke(tableData, null);
                    if (!table.ContainsKey(id))
                    {
                        table.Add(id, tableData);

                        if(type == typeof(GlobalSettingTable))
                        {
                            gst = tableData as GlobalSettingTable;
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("{0} : already contain the id : {1}", filepath, id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0} : *.cs don't match the *.xls, delete the *.proto, regenerate the *.cs", filepath);
                Logger.LogErrorFormat("error deserialize at line {0}, with error {1}", i + 1, e.ToString());

                string ErrorMsg = "表格："+filepath+" 加载失败，原因："+e.Message;

#if UNITY_EDITOR && !LOGIC_SERVER
                EditorUtility.DisplayDialog("【读表错误!】",ErrorMsg,"确定","");
#else
                Logger.LogErrorFormat("【读表错误!】 {0}",ErrorMsg);
#endif
                return;
            }

                i += len;
            }
        }
        else
            Logger.LogErrorFormat("表格：" + filepath + " 资源不存在!");





#else
        do
        {

            byte[] data = null;

#if USE_FB
            filepath = Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(filepath, Utility.kRawDataExtension)).ToLower();
            if(File.Exists(filepath))
                data = System.IO.File.ReadAllBytes(filepath);
            Logger.LogProcessFormat(filepath + "\n");
#else
            TextAsset textAssetFB = AssetLoader.instance.LoadRes(filepath, typeof(TextAsset)).obj as TextAsset;
            if (null == textAssetFB)
            {
                Logger.LogError("Load table has failed, table resource:" + filepath);
                return;
            }

            data = textAssetFB.bytes;
#endif

            FlatBuffers.Table ftable = new FlatBuffers.Table();
            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(data);

            ftable.bb_pos = 0;
            ftable.bb = buffer;

            int length = ftable.__vector_len(0);

            for (int index = 0; index < length; ++index)
            {
                ;
                int offset = ftable.__vector(index);

                //                        CreateInstance();

                var fobj = Activator.CreateInstance(originalType);

                MethodInfo __assign = originalType.GetMethod("__assign");
                var IDMap = originalType.GetProperty("ID").GetGetMethod();

                BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
                //GetValue方法的参数
                object[] parameters = new object[] { ftable.__indirect(ftable.__vector(0) + index * 4), ftable.bb };
                __assign.Invoke(fobj, flag, Type.DefaultBinder, parameters, null);

                int id = (int)IDMap.Invoke(fobj, null);

                if (!table.ContainsKey(id))
                {
                    table.Add(id, fobj);
                    if (originalType == typeof(GlobalSettingTable))
                    {
                        gst = fobj as GlobalSettingTable;
                    }
                }
                else
                {
                    Logger.LogErrorFormat("type:{0},拥有相同ID：{1},请及时修正", realType.Name, id);
                }
            }
        } while (false);

#endif
    }
    public override void Init()
    {
        if (bInit)
        {
            return;
        }

#if UNITY_EDITOR

#else
		string msg = "[错误]表格初始化流程异常，立马修改！！！！！！！";
		GameClient.SystemNotifyManager.BaseMsgBoxOK(msg, null);
		Logger.LogErrorFormat(msg);

		return;
#endif

        // 同步加载
        Utility.IterCoroutineImm(_InitCoroutine(null));
    }


    public void LogicServerInit()
    {
        if (bInit)
        {
            return;
        }
        // 同步加载
        Utility.IterCoroutineImm(_InitCoroutine(null));
    }

    public override void UnInit()
    {
        //加载游戏表格
        //---------------------------------------------------------------------
        mTypeTableDict.Clear();
        mSkillInfoDict.Clear();
        jobInstituteDic.Clear();
        //升级经验表
        mPerLevelNeedExp.Clear();
        mToLevelNeedExp.Clear();

        //刷怪技能信息
        mBornInfoDict.Clear();

        //转职信息
        JobChangeMap.Clear();

        OpenSkillBarLevelMap.Clear();
        comboScoreDic.Clear();
        monsterAttributeMap.Clear();
        comboDataList.Clear();
        AuctionGuiJianShiWeaponType.Clear();
        AuctionShenQiangShouWeaponType.Clear();
        AuctionMoFaShiWeaponType.Clear();
        AuctionGeDouJiaWeaponType.Clear();
        AuctionDefenceType.Clear();
        AuctionJewelryType.Clear();
        AuctionArmorType.Clear();
        AuctionQualityType.Clear();
        AuctionConsumablesType.Clear();
        AuctionMaterialsType.Clear();

        pkHPProfessionAdjustInfo = null;
        dungeonDifficultyAdjustInfo.Clear();

        dungeonDifficultyAdjustInfo.Clear();

        TeamDungeonFirstMenuList.Clear();
        TeamDungeonSecondMenuDict.Clear();
        treasureRoomRandomLibrary.Clear();
        NewbieGuideOrderList.Clear();

        giftInfoDict.Clear();

        bNeedUninit = false;
        bInit = false;
        base.UnInit();
    }

    public object GetTableItemForLua(Type curType, int id)
    {
        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return null;
        }

        var table = mTypeTableDict[curType];

        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return null;
        }

        if (!table.ContainsKey(id))
        {
            //Logger.LogWarningFormat("{0}表里没有id为{1}的项", curType.Name, id);
            return null;
        }

        return table[id];
    }

    public Dictionary<int, object> GetTableForLua(Type curType)
    {
        Dictionary<int, object> NullTable = new Dictionary<int, object>();
        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return null;
        }

        Dictionary<int, object> table = mTypeTableDict[curType];

        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return null;
        }

        return table;
    }

    public Dictionary<int, object> GetTable<T>()
    {
        return GetTable(typeof(T));
    }

    public Dictionary<int, object> GetTable(Type curType)
    {
        Dictionary<int, object> NullTable = new Dictionary<int, object>();

        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return NullTable;
        }

        Dictionary<int, object> table = mTypeTableDict[curType];
        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return NullTable;
        }

        return table;
    }

    public UltimateChallengeDungeonTable GetFinalTestTime(int dungeonID)
    {
        var data = mTypeTableDict[typeof(UltimateChallengeDungeonTable)];

        foreach (var item in data)
        {
            UltimateChallengeDungeonTable itemData = item.Value as UltimateChallengeDungeonTable;

            if (itemData.dungeonID == dungeonID)
                return itemData;
        }
        return null;
    }

    public bool IsLastFloor(int _id)
    {
        int id = 0;
        var data = mTypeTableDict[typeof(UltimateChallengeDungeonTable)];
        foreach (var item in data)
        {
            UltimateChallengeDungeonTable itemData = item.Value as UltimateChallengeDungeonTable;

            if (itemData.ID > id)
                id = itemData.ID;
        }
        return id == _id;
    }

    public T GetTableItem<T>(int id, string who = "", string dowhat = "")
    {
        var curType = typeof(T);

        Dictionary<int, object> curTblDict = null;
        if (mTypeTableDict.TryGetValue(curType, out curTblDict))
        {
            object curItem = null;
            if (curTblDict.TryGetValue(id, out curItem))
            {
                return (T)curItem;
            }
            else
            {
                return default(T);
            }
        }
        else
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return default(T);
        }


        /*
        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return default(T);
        }

        var table = mTypeTableDict[curType];

        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return default(T);
        }

        if (!table.ContainsKey(id))
        {
            //Logger.LogWarningFormat("{0}表里没有id为{1}的项", curType.Name, id);
            return default(T);
        }

        return (T)(table[id]);
        */
    }

    public object GetTableItem(Type curType, int id, string who = "", string dowhat = "")
    {
        Dictionary<int, object> curTblDict = null;
        if (mTypeTableDict.TryGetValue(curType, out curTblDict))
        {
            object curItem = null;
            if (curTblDict.TryGetValue(id, out curItem))
            {
                return curItem;
            }
            else
            {
                //Logger.LogWarningFormat("{0}表里没有id为{1}的项", curType.Name, id);
                return null;
            }
        }
        else
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return null;
        }

        /*
        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return null;
        }

        var table = mTypeTableDict[curType];

        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return null;
        }

        if (!table.ContainsKey(id))
        {
            //Logger.LogWarningFormat("{0}表里没有id为{1}的项", curType.Name, id);
            return null;
        }

        return table[id];
        */
    }

    public T GetTableItemByIndex<T>(int iIndex)
    {
        var curType = typeof(T);
        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return default(T);
        }

        var table = mTypeTableDict[curType];

        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return default(T);
        }

        int iCount = 0;
        foreach (var TableID in table.Keys)
        {
            if (iCount == iIndex)
            {
                return (T)(table[TableID]);
            }

            iCount++;
        }

        Logger.LogWarningFormat("{0}表里没有第{1}项索引", curType.Name, iIndex);
        return default(T);
    }

    public int GetTableItemCount<T>()
    {
        var curType = typeof(T);
        if (!mTypeTableDict.ContainsKey(curType))
        {
            Logger.LogErrorFormat("没有加载 {0}", curType.Name);
            return -1;
        }

        var table = mTypeTableDict[curType];

        if (table == null)
        {
            Logger.LogWarningFormat("{0}表里为null", curType.Name);
            return -1;
        }

        return table.Count;
    }

    public T GetTableItem<T>(string key)
    {
        var item = GetTableItem<T>(key.GetHashCode());

        if (item == null)
        {
            Logger.LogWarningFormat("表里没有key为{0}的项", key);
        }

        return item;
    }

    public NpcTable GetNpcItemByUnitTable(UnitTable unitItem)
    {
        if (unitItem != null)
        {
            Dictionary<int, object> npcTable;
            if (mTypeTableDict.TryGetValue(typeof(NpcTable), out npcTable))
            {
                if (npcTable != null)
                {
                    foreach (var npcItem in npcTable)
                    {
                        NpcTable objItem = npcItem.Value as NpcTable;
                        if (objItem.UnitTableID == unitItem.ID)
                        {
                            return objItem;
                        }
                    }
                }
            }
        }
        return null;
    }

    private string _getTablePath(Type type)
    {
        return kTablePath + type.Name;
    }

    public static string _getTablePathNew(Type type)
    {
        return string.Format("{0}.bytes", Utility.Combine(kTablePath, type.Name));
    }

    public void LoadTable(Type type)
    {
        
        var table = _loadTable(type);
        if (table == null)
            return;
        
        Debug.LogErrorFormat("重新加载 {0} 表成功", type.ToString());

        if (mTypeTableDict.ContainsKey(type))
            mTypeTableDict[type] = table;
        else
            mTypeTableDict.Add(type, table);
    }

    private Dictionary<int, object> _loadTable(Type type)
    {
        Dictionary<int, object> table = new Dictionary<int, object>();
        string filepath = _getTablePath(type);

#if !USE_FB_TABLE
        


        byte[] data = null;
#if USE_FB
        filepath = Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(filepath, Utility.kRawDataExtension)).ToLower();
        if(File.Exists(filepath))
            data = System.IO.File.ReadAllBytes(filepath);
        Logger.LogProcessFormat(filepath + "\n");
#else
        AssetBinary textAsset = AssetLoader.instance.LoadRes(filepath, typeof(AssetBinary)).obj as AssetBinary;
        if (null == textAsset)
        {
            Logger.LogError("Load table has failed, table resource:" + filepath);
            return table;
        }
        
        data = textAsset.m_DataBytes;
#endif
        if (null != data)
        {
            bool bCanParse = Serializer.CanParse(type);
            var IDMap = type.GetProperty("ID").GetGetMethod();
            for (int i = 0; i < data.Length;)
            {
                int len = 0;
                for (int j = i; j < i + 8; ++j)
                {
                    if (data[j] > 0)
                        len = len * 10 + (data[j] - '0');
                }

            i += 8;
            MemoryStream stream = new MemoryStream(data, i, len);
            try
            {
                object tableData = null;

                if(bCanParse)
                {
                    tableData = Serializer.ParseEx(type,stream);
                }
                else
                {
                    tableData = Serializer.DeserializeEx(type, stream);
                }
                
                if (tableData == null)
                {
                    Logger.LogErrorFormat("table data is nil {0}, {1}", filepath, i);
                }
                else
                {
                    var id = (int)IDMap.Invoke(tableData, null);
                    if (!table.ContainsKey(id))
                    {
                        table.Add(id, tableData);

                        if(type == typeof(GlobalSettingTable))
                        {
                            gst = tableData as GlobalSettingTable;
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("{0} : already contain the id : {1}", filepath, id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0} : *.cs don't match the *.xls, delete the *.proto, regenerate the *.cs", filepath);
                Logger.LogErrorFormat("error deserialize at line {0}, with error {1}", i + 1, e.ToString());

                string ErrorMsg = "表格："+filepath+" 加载失败，原因："+e.Message;

#if UNITY_EDITOR && !LOGIC_SERVER
                EditorUtility.DisplayDialog("【读表错误!】",ErrorMsg,"确定","");
#else
                Logger.LogErrorFormat("【读表错误!】 {0}",ErrorMsg);
#endif
                return table;
            }

                i += len;
            }
        }
        else
            Logger.LogErrorFormat("表格：" + filepath + " 资源不存在!");





#else
        do
        {

            byte[] data = null;

#if USE_FB
            filepath = Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(filepath, Utility.kRawDataExtension)).ToLower();
            if(File.Exists(filepath))
                data = System.IO.File.ReadAllBytes(filepath);
            Logger.LogProcessFormat(filepath + "\n");
#else
            TextAsset textAssetFB = AssetLoader.instance.LoadRes(filepath, typeof(TextAsset)).obj as TextAsset;
            if (null == textAssetFB)
            {
                Logger.LogError("Load table has failed, table resource:" + filepath);
                return table;
            }

            data = textAssetFB.bytes;
#endif

            FlatBuffers.Table ftable = new FlatBuffers.Table();
            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(data);

            ftable.bb_pos = 0;
            ftable.bb = buffer;

            int length = ftable.__vector_len(0);

            for (int index = 0; index < length; ++index)
            {
                ;
                int offset = ftable.__vector(index);

                //                        CreateInstance();

                var fobj = Activator.CreateInstance(type);

                MethodInfo __assign = type.GetMethod("__assign");
                var IDMap = type.GetProperty("ID").GetGetMethod();

                BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
                //GetValue方法的参数
                object[] parameters = new object[] { ftable.__indirect(ftable.__vector(0) + index * 4), ftable.bb };
                __assign.Invoke(fobj, flag, Type.DefaultBinder, parameters, null);

                int id = (int)IDMap.Invoke(fobj, null);

                if (!table.ContainsKey(id))
                {
                    table.Add(id, fobj);
                    if (type == typeof(GlobalSettingTable))
                    {
                        gst = fobj as GlobalSettingTable;
                    }
                }
                else
                {
                    Logger.LogErrorFormat("id {0} 相同,严重错误 {1}", id, type.ToString());
                }


            }

            return table;
        } while (false);

#endif


        return table;
    }

    #region dd: Skill Info Map
    private Dictionary<int, Dictionary<int, int>> mSkillInfoDict = new Dictionary<int, Dictionary<int, int>>(); // key:职业id，value：技能id，等级(初始值为1)
    private Dictionary<int, List<int>> mBornInfoDict = new Dictionary<int, List<int>>();
    private Dictionary<int, List<ComboScoreTable>> comboScoreDic = new Dictionary<int, List<ComboScoreTable>>();
    private List<UInt64> mPerLevelNeedExp = new List<UInt64>();

    /// <summary>
    /// 表格数据，复制一份，用于快速查询
    /// </summary>
    private List<UInt64> mToLevelNeedExp = new List<UInt64>();
    private Dictionary<int, IList<int>> JobChangeMap = new Dictionary<int, IList<int>>();
    private Dictionary<int, int> OpenSkillBarLevelMap = new Dictionary<int, int>();
    private List<int> NewbieGuideOrderList = new List<int>();
    private Dictionary<int, List<InstituteTable>> jobInstituteDic = new Dictionary<int, List<InstituteTable>>();
    private List<int> TeamDungeonFirstMenuList = new List<int>();
    private Dictionary<int, List<int>> TeamDungeonSecondMenuDict = new Dictionary<int, List<int>>();
    private List<int> AuctionGuiJianShiWeaponType = new List<int>();
    private List<int> AuctionShenQiangShouWeaponType = new List<int>();
    private List<int> AuctionMoFaShiWeaponType = new List<int>();
    private List<int> AuctionGeDouJiaWeaponType = new List<int>();
    private List<int> AuctionDefenceType = new List<int>();
    private List<int> AuctionJewelryType = new List<int>();
    private List<int> AuctionArmorType = new List<int>();
    private List<int> AuctionQualityType = new List<int>();
    private List<int> AuctionConsumablesType = new List<int>();
    private List<int> AuctionMaterialsType = new List<int>();

    private Dictionary<int, object> monsterAttributeMap = new Dictionary<int, object>();
    //宝藏关卡随机库
    private Dictionary<int, List<string>> treasureRoomRandomLibrary = new Dictionary<int, List<string>>();
    int[] pkHPProfessionAdjustInfo = null;

    Dictionary<int, object[]> dungeonDifficultyAdjustInfo = new Dictionary<int, object[]>();
    public List<ComboTeachData> comboDataList = new List<ComboTeachData>();

    public void ReloadRelatedLogic(Type tableType)
    {
        if (tableType == null) return;
        if (tableType == (typeof(ProtoTable.SkillTable)))
        {
            mSkillInfoDict.Clear();
            _loadSkillInfo();
            mBornInfoDict.Clear();
            _LoadBornSkillInfo();
        }
        else if (tableType ==(typeof(ProtoTable.MonsterAttributeTable)))
        {
            monsterAttributeMap.Clear();
            _loadMonsterAttributeInfo();
        }
        //
        else if (tableType ==typeof(ProtoTable.DungeonDifficultyAdjustTable))
        {
            dungeonDifficultyAdjustInfo.Clear();
            _loadDungeonDifficultyAdjustInfo();
        }
    }

    public void Reinit()
    {
        return;
        mTypeTableDict.Clear();
        comboDataList.Clear();
        mSkillInfoDict.Clear();//技能表
        mBornInfoDict.Clear();
        mPerLevelNeedExp.Clear();
        mToLevelNeedExp.Clear();
        comboScoreDic.Clear();
        JobChangeMap.Clear();
        OpenSkillBarLevelMap.Clear();
        NewbieGuideOrderList.Clear();

        TeamDungeonFirstMenuList.Clear();
        TeamDungeonSecondMenuDict.Clear();
        treasureRoomRandomLibrary.Clear();

        AuctionGuiJianShiWeaponType.Clear();
        AuctionShenQiangShouWeaponType.Clear();
        AuctionMoFaShiWeaponType.Clear();
        AuctionGeDouJiaWeaponType.Clear();
        AuctionDefenceType.Clear();
        AuctionJewelryType.Clear();
        AuctionArmorType.Clear();
        AuctionQualityType.Clear();
        AuctionConsumablesType.Clear();
        AuctionMaterialsType.Clear();

        monsterAttributeMap.Clear();
        bInit = false;

        Init();
    }

    public ComboTeachData GetComboData(int dungeonID)
    {
        for (int i = 0; i < comboDataList.Count; i++)
        {
            if (comboDataList[i] == null) continue;
            if (comboDataList[i].roomID == dungeonID)
                return comboDataList[i];
        }
        return null;
    }
    public void LoadData()
    {
        comboDataList.Clear();
        List<InstituteTable> list = TableManager.instance.GetJobInstituteData(GameClient.PlayerBaseData.GetInstance().JobTableID);
        for (int i = 0; i < list.Count; i++)
        {
            comboDataList.Add(LoadComboData(list[i]));
        }
    }
    private ComboTeachData LoadComboData(InstituteTable table)
    {
        string dif = table.DifficultyType == 1 ? "Easy" : "Advance";
        string key = table.Resource + "/" + dif + "/" + table.DungeonID;
        UnityEngine.Object data = AssetLoader.instance.LoadRes(key, typeof(ComboTeachData)).obj;
        if (data == null)
        {
            Logger.LogErrorFormat("combodata is null+ key{0}", key);
        }
        return data as ComboTeachData;
    }


    private void _loadExpInfo()
    {
        var expDatas = mTypeTableDict[typeof(ExpTable)];

        int count = expDatas.Count;
        mPerLevelNeedExp.Clear();
        mToLevelNeedExp.Clear();

        mPerLevelNeedExp.Add(0);
        mToLevelNeedExp.Add(0);

        for (int i = 1; i <= count; i++)
        {
            var expItem = expDatas[i] as ExpTable;
            if (expItem.TotalExp < 0)
            {
                mPerLevelNeedExp.Add(0);
                Logger.LogErrorFormat("expItem with negative value index : {0}", i);
            }
            else
            {
                mPerLevelNeedExp.Add((UInt64)expItem.TotalExp);
            }

            if (expItem.TotalExp <= 0)
            {
                mToLevelNeedExp.Add(0);
            }
            else
            {
                mToLevelNeedExp.Add((UInt64)expItem.TotalExp + mToLevelNeedExp[i - 1]);
            }
        }


        //for (int i = count - 1; i > 0; --i)
        //{
        //    mPerLevelNeedExp[i] = mPerLevelNeedExp[i] - mPerLevelNeedExp[i - 1];
        //}

        mPerLevelNeedExp[0] = 0;

        // Debug Code
        /*
        for (int i = 1; i <= count; i++)
        {
            Debug.LogErrorFormat("Don't match !!!!!!!!!! l {0} exp {1}", i, mPerLevelNeedExp[i]);
        }
        
        for (int i = 1; i <= count; ++i)
        {
            _testCode_GetLevelByExp(i, mToLevelNeedExp[i] - 10);
            _testCode_GetLevelByExp(i, mToLevelNeedExp[i] + 10);
            _testCode_GetLevelByExp(i, mToLevelNeedExp[i]);
        }
        */
    }

    /*
    private void _testCode_GetLevelByExp(int i, UInt64 exp)
    {
        Debug.LogWarningFormat("** start test {0}, {1}", i, exp);
        var idx = GetExpByCurExp(exp);

        for (int j = 1; j < mToLevelNeedExp.Count; ++j)
        {
            // 找到第一个比这个经验大的
            if (exp < mToLevelNeedExp[j])
            {
                if (idx != j)
                {
                    Debug.LogErrorFormat("wrong data with i : {0}, j {1}, exp {2}, big exp {3}", idx, j, exp, mToLevelNeedExp[j]);
                }
                break;
            }
        }
        Debug.LogWarningFormat("** end test {0}, {1}", i, exp);
    }
    */

    private void _loadSkillInfo()
    {
        var skillDatas = mTypeTableDict[typeof(SkillTable)];
        var data = skillDatas.GetEnumerator();

        while (data.MoveNext())
        {
            SkillTable skillItem = data.Current.Value as SkillTable;

            if (skillItem.SkillCategory >= 5)
            {
                continue;
            }

            for (int i = 0; i < skillItem.JobID.Count; i++)
            {
                int pid = skillItem.JobID[i];

                if (!mSkillInfoDict.ContainsKey(pid))
                {
                    mSkillInfoDict.Add(pid, new Dictionary<int, int>());
                }

                if (!mSkillInfoDict[pid].ContainsKey(skillItem.ID))
                {
                    mSkillInfoDict[pid].Add(skillItem.ID, 1);
                }
            }
        }
    }

    private void _loadComboScore()
    {
        var comboDatas = mTypeTableDict[typeof(ComboScoreTable)];
        var data = comboDatas.GetEnumerator();
        while (data.MoveNext())
        {
            ComboScoreTable comboScoreItem = data.Current.Value as ComboScoreTable;

            if (!comboScoreDic.ContainsKey(comboScoreItem.JobID))
            {
                List<ComboScoreTable> list = new List<ComboScoreTable>();
                list.Add(comboScoreItem);
                comboScoreDic[comboScoreItem.JobID] = list;
            }
            else
            {
                comboScoreDic[comboScoreItem.JobID].Add(comboScoreItem);
            }
        }

        foreach (var item in comboScoreDic)
        {
            item.Value.Sort((x, y) =>
            {
                if (x.Percent >= y.Percent)
                    return -1;
                if (x.Percent < y.Percent)
                    return 1;
                return 0;
            });
        }
    }

    public uint GetComboScore(int jobID, int percent, int minCombo)
    {
        List<ComboScoreTable> list;
        if (comboScoreDic.TryGetValue(jobID, out list))
        {
            percent = Mathf.Min(percent, list[0].Percent);
            ComboScoreTable item = FindItemByPercent(list, percent);
            if (item != null)
            {
                if (item.MinCombo <= minCombo)
                {
                    return (uint)item.BaseScore;
                }
                else if (item.MinCombo > minCombo)
                {
                    ComboScoreTable combo = FindItemByCombo(list, minCombo);
                    if (combo != null)
                        return (uint)combo.BaseScore;
                }
            }
        }
        return 0;
    }

    private ComboScoreTable FindItemByCombo(List<ComboScoreTable> list, int combo)
    {
        if (combo <= 0) return null;
        ComboScoreTable item = list.Find(x => x.MinCombo == combo);
        if (item == null)
        {
            combo--;
            return FindItemByCombo(list, combo);
        }
        else
        {
            return item;
        }
    }

    private ComboScoreTable FindItemByPercent(List<ComboScoreTable> list, int percent)
    {
        if (percent <= 0) return null;
        ComboScoreTable item = list.Find(x => x.Percent == percent);
        if (item == null)
        {
            percent--;
            return FindItemByPercent(list, percent);
        }
        else
        {
            return item;
        }
    }

    public Dictionary<int, int> GetSkillInfoByPid(int pid)
    {
        if (mSkillInfoDict.ContainsKey(pid))
        {
            return mSkillInfoDict[pid];
        }

        return new Dictionary<int, int>();
    }

    /// <summary>
    /// 获得当前等级对应一管经验条的大小
    /// O(1)
    /// </summary>
    public UInt64 GetExpByLevel(int level)
    {
        if (level >= mPerLevelNeedExp.Count || level <= 0)
        {
            Logger.LogErrorFormat("level out of range with level : {0}", level);
            return UInt64.MaxValue;
        }

        return mPerLevelNeedExp[level];
    }

    /// <summary>
    /// 根据当前总经验，获得当前等级
    /// 0 -> 为1级
    /// 大于等于59级的总经验的 -> 60级
    /// 所有等级分隔线俊测试通过，测试代码 _testCode_GetLevelByExp
    /// O(logn)
    /// </summary>
    public int GetLevelByExp(UInt64 totalExp)
    {
        int start = 1;
        // 60 -> max level
        int end = mToLevelNeedExp.Count - 1;
        int mid = -1;

        while (start < end)
        {
            mid = (start + end) / 2;

            if (mToLevelNeedExp[mid] <= totalExp)
            {
                start = mid + 1;
            }
            else
            {
                end = mid;
            }

            //Debug.LogFormat("start : {0}, mid {1}, end{2}", start, mid, end);
        }

        //Debug.LogFormat("** start : {0}, mid {1}, end{2}", start, mid, end);

        return start;
    }

    /// <summary>
    /// 获得当前等级，当前管的剩余血量,总血量
    /// </summary>
    public KeyValuePair<UInt64, UInt64> GetCurRoleExp(UInt64 totalExp)
    {
        var level = GetLevelByExp(totalExp);
        var val = GetExpByLevel(level);

        //Debug.LogErrorFormat("cur level {0}, val {1}", level, val);

        if (level == mToLevelNeedExp.Count - 1)
        {
            /// 因为 MaxLevel 填的是0
            /// <0,0> 表示满级
            return new KeyValuePair<UInt64, UInt64>(val, val);
        }

        return new KeyValuePair<UInt64, UInt64>(val - (mToLevelNeedExp[level] - totalExp), val);
    }

    public KeyValuePair<UInt64, UInt64> GetCurVipLevelExp(int VipLevel, UInt64 CurVipLvRmb)
    {
        // 算出来前面等级的总钱数
        ulong uMoney = 0;
        var VipTableData = GetTable<VipTable>();

        if (VipTableData != null)
        {
            var emu = VipTableData.GetEnumerator();
            while (emu.MoveNext())
            {
                var item = emu.Current.Value as VipTable;

                if (item.ID < VipLevel)
                {
                    uMoney += (ulong)item.TotalRmb;
                }
            }
        }

        // 满级(人为限制最高等级)
        var SystemValueTableData = GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);
        if (SystemValueTableData != null)
        {
            if (VipLevel >= SystemValueTableData.Value)
            {
                return new KeyValuePair<UInt64, UInt64>(CurVipLvRmb + uMoney, uMoney);
            }
        }

        UInt64 CurVipLvTotalRmb = 0;

        VipTable data = GetTableItem<VipTable>(VipLevel);
        if (data != null)
        {
            CurVipLvTotalRmb = (UInt64)data.TotalRmb;
        }

        // 满级
        if (CurVipLvTotalRmb == 0)
        {
            return new KeyValuePair<UInt64, UInt64>(CurVipLvRmb + uMoney, uMoney);
        }

        if (CurVipLvRmb >= CurVipLvTotalRmb)
        {
            CurVipLvRmb -= 1;
        }

        return new KeyValuePair<UInt64, UInt64>(CurVipLvRmb + uMoney, CurVipLvTotalRmb + uMoney);

        //         Dictionary<int, object> table = mTypeTableDict[typeof(VipTable)];
        // 
        //         int level = -2;
        //         UInt64 processExp = 0;
        // 
        //         foreach (var item in table.Values)
        //         {
        //             VipTable data = item as VipTable;
        // 
        //             if (totalExp >= (UInt64)data.TotalRmb)
        //             {
        //                 continue;
        //             }
        // 
        //             level = data.ID - 1;
        //             processExp = (UInt64)data.TotalRmb;
        // 
        //             break;
        //         }
        // 
        //         if (level == -2)
        //         {
        //             /// 因为 MaxLevel 填的是0
        //             /// <0,0> 表示满级
        //             return new KeyValuePair<UInt64, UInt64>(0, 0);
        //         }
        // 
        //         if (level == -1)
        //         {
        //             level = 0;
        //         }
        // 
        //         var VipLevelData = GetTableItem<VipTable>(level);
        //         if (VipLevelData == null)
        //         {
        //             return new KeyValuePair<UInt64, UInt64>(0, 0);
        //         }
        // 
        //         if (level == 0)
        //         {
        //             return new KeyValuePair<UInt64, UInt64>(totalExp, processExp);
        //         }
        //         else
        //         {
        //             return new KeyValuePair<UInt64, UInt64>(totalExp - (UInt64)VipLevelData.TotalRmb, processExp - (UInt64)VipLevelData.TotalRmb);
        //         }
    }

    public KeyValuePair<UInt64, UInt64> GetCurPetExpBar(int Level, UInt64 CurExp, PetTable.eQuality ePetQuality)
    {
        var TotalData = GetTable<PetLevelTable>();
        var enumer = TotalData.GetEnumerator();

        UInt64 CurLvTotalExp = 0;

        while (enumer.MoveNext())
        {
            PetLevelTable data = enumer.Current.Value as PetLevelTable;
            if (data == null)
            {
                continue;
            }

            if (data.Level != Level || data.Quality != (int)ePetQuality)
            {
                continue;
            }

            CurLvTotalExp = (UInt64)data.UplevelExp;

            break;
        }

        // 满级
        if (CurLvTotalExp == 0)
        {
            return new KeyValuePair<UInt64, UInt64>(0, 0);
        }

        return new KeyValuePair<UInt64, UInt64>(CurExp, CurLvTotalExp);
    }

    public InstituteTable GetDataByDungeonID(int jobID, int id)
    {
        List<InstituteTable> list = GetJobInstituteData(jobID);
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].DungeonID == id)
                    return list[i];
            }
        }
        return null;
    }

    public List<InstituteTable> GetJobInstituteData(int jobID)
    {
        List<InstituteTable> list;
        if (jobInstituteDic.TryGetValue(jobID, out list))
        {
            return list;
        }
        return new List<InstituteTable>();
    }

    public List<InstituteTable> GetJobAndTypeInstitue(int jobID, int type)
    {
        List<InstituteTable> list = GetJobInstituteData(jobID);
        List<InstituteTable> dataList = new List<InstituteTable>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].DifficultyType == type)
                dataList.Add(list[i]);
        }
        return dataList;
    }

    void _LoadJobInstituteInfo()
    {
        var instituteDatas = mTypeTableDict[typeof(InstituteTable)];

        foreach (var item in instituteDatas)
        {
            InstituteTable data = item.Value as InstituteTable;
            if (jobInstituteDic.ContainsKey(data.JobID))
            {
                jobInstituteDic[data.JobID].Add(data);
            }
            else
            {
                jobInstituteDic[data.JobID] = new List<InstituteTable>();
                jobInstituteDic[data.JobID].Add(data);
            }

        }
    }

    void _LoadBornSkillInfo()
    {
        var skillDatas = mTypeTableDict[typeof(SkillTable)];

        foreach (var item in skillDatas)
        {
            SkillTable skillItem = item.Value as SkillTable;

            if (skillItem.SkillCategory == 1 && skillItem.JobID[0] > 0)
            {
                foreach (var jobId in skillItem.JobID)
                {
                    if (!mBornInfoDict.ContainsKey(jobId))
                    {
                        mBornInfoDict.Add(jobId, new List<int>());
                    }

                    mBornInfoDict[jobId].Add(skillItem.ID);
                }
            }

            // 被动技能不只是辅助技能，辅助技能一定是被动技能。skillItem.PassiveSkillBtnIndex > 0这个判断条件保证了mPassiveSkillInfoDict数据集合都是辅助技能，by Wangbo 2020.08.13
            //             if (skillItem.SkillCategory == 3 && skillItem.JobID[0] > 0 && skillItem.PassiveSkillBtnIndex > 0)
            //             {
            //                 for (int i = 0; i < skillItem.JobID.Count; ++i)
            //                 {
            //                     int jobId = skillItem.JobID[i];
            //                     if (!mPassiveSkillInfoDict.ContainsKey(jobId))
            //                     {
            //                         mPassiveSkillInfoDict.Add(jobId, new List<int>());
            //                     }
            // 
            //                     mPassiveSkillInfoDict[jobId].Add(skillItem.ID);
            //                 }
            //             }
        }
    }

    public List<int> GetBornSkills(int jobId)
    {
        if (!mBornInfoDict.ContainsKey(jobId))
            return new List<int>();

        return mBornInfoDict[jobId];
    }
    #endregion

    #region GiftTable

    private Dictionary<int, List<GiftTable>> giftInfoDict = new Dictionary<int, List<GiftTable>>();
    private void _LoadGiftTable()
    {
        var giftDatas = mTypeTableDict[typeof(GiftTable)];
        var data = giftDatas.GetEnumerator();
        while (data.MoveNext())
        {
            var giftTable = data.Current.Value as GiftTable;
            if (giftTable == null)
                continue;

            if(!giftInfoDict.ContainsKey(giftTable.GiftPackID))
            {
                giftInfoDict.Add(giftTable.GiftPackID, new List<GiftTable>());
            }

            giftInfoDict[giftTable.GiftPackID].Add(giftTable);
        }
    }

    /// <summary>
    /// 得到礼包Id对应所有礼品
    /// </summary>
    /// <param name="giftPackID"></param>
    /// <returns></returns>
    public List<GiftTable> GetGiftTableData(int giftPackID)
    {
        List<GiftTable> datas;
        if(giftInfoDict != null)
        {
            if (giftInfoDict.TryGetValue(giftPackID, out datas))
            {
                return datas;
            }
        }

        return new List<GiftTable>();
    }

    #endregion

    #region ItemConfig
    /// <summary>
    /// 通过 道具ID 查找对应 类型（道具配置表ID）
    /// 单局内使用道具
    /// </summary>
    /// <param name="itemid"></param>
    /// <returns></returns>
    public int GetItemConfigID(int itemid)
    {
        var dict = mTypeTableDict[typeof(ItemConfigTable)];
        var iter = dict.GetEnumerator();

        ItemConfigTable item = null;

        while (iter.MoveNext())
        {
            item = iter.Current.Value as ItemConfigTable;
            if (null != item && item.ItemID == itemid)
            {
                return item.ID;
            }
        }

        return -1;
    }

    #endregion

    void _LoadJobChangeMap()
    {
        var JobDatas = mTypeTableDict[typeof(JobTable)];

        foreach (var item in JobDatas)
        {
            JobTable JobItem = item.Value as JobTable;

            if (JobItem.ID == 0)
            {
                continue;
            }

            if (JobItem.ToJob == null || JobItem.ToJob.Count <= 0 || (JobItem.ToJob.Count == 1 && JobItem.ToJob[0] == 0))
            {
                continue;
            }

            if (JobChangeMap.ContainsKey(JobItem.ID))
            {
                continue;
            }

            List<int> toJobList = new List<int>();
            foreach (var jobId in JobItem.ToJob)
            {
                var tb = GetTableItem<JobTable>(jobId);
                if (tb == null)
                {
                    continue;
                }

                if (tb.JobType <= 0)
                {
                    continue;
                }
                
                toJobList.Add(jobId);
            }

            JobChangeMap.Add(JobItem.ID, toJobList);
        }
    }

    public IList<int> GetNextJobsIDByCurJobID(int CurJobID)
    {
        IList<int> NextJobsID = null;

        if (!JobChangeMap.TryGetValue(CurJobID, out NextJobsID))
        {
            return null;
        }

        return NextJobsID;
    }

    void _LoadOpenSkillBarLevelInfo()
    {
        var OpenSkillLevelData = mTypeTableDict[typeof(ExpTable)];

        foreach (var item in OpenSkillLevelData)
        {
            ExpTable itemData = item.Value as ExpTable;

            if (OpenSkillBarLevelMap.ContainsKey(itemData.SkillNum))
            {
                continue;
            }

            OpenSkillBarLevelMap.Add(itemData.SkillNum, itemData.ID);
        }
    }

    public int GetLevelBySkillBarIndex(int skillbarIndex)
    {
        foreach (var index in OpenSkillBarLevelMap.Keys)
        {
            if (skillbarIndex <= index)
            {
                return OpenSkillBarLevelMap[index];
            }
        }

        return -1;
    }

    #region static methods

    public static int GetValueFromUnionCell(UnionCell ucell, int level, bool bNeedBaseLevel = true)
    {
        if (bNeedBaseLevel && level <= 0)
        {
            level = 1;
        }

        if (level > 0)
        {
            var valueType = ucell.valueType;

            if (valueType == UnionCellType.union_fix)
            {
                return ucell.fixValue;
            }

            if (valueType == UnionCellType.union_fixGrow)
            {
                return ucell.fixInitValue + (level - 1) * ucell.fixLevelGrow;
            }

            if (valueType == UnionCellType.union_everyvalue)
            {
                if (level - 1 < ucell.eValues.everyValues.Count)
                {
                    return ucell.eValues.everyValues[level - 1];
                }
                //超过就返回最后那个
                else {
                    return ucell.eValues.everyValues[ucell.eValues.everyValues.Count - 1];
                }
            }

            return 0;
        }

        return 0;
    }
    #endregion

    void MergeTable(Dictionary<int, object> t1, Dictionary<int, object> t2)
    {
        Dictionary<int, object>.Enumerator enumerator = t1.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (t2.ContainsKey(enumerator.Current.Key))
            {
                Logger.LogWarningFormat("MergeTable has same key:{0}", enumerator.Current.Key);
                continue;
            }

            t2.Add(enumerator.Current.Key, enumerator.Current.Value);
        }
    }

    void _loadMonsterAttributeInfo()
    {
        var table = mTypeTableDict[typeof(MonsterAttributeTable)];
        Dictionary<int, object>.Enumerator enumerator = table.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = (ProtoTable.MonsterAttributeTable)enumerator.Current.Value;
            int m = item.MonsterMode;
            int d = item.Difficulty;
            int t = item.MonsterType;
            int level = item.level;

            // marked by ckm
            // int key = d * GlobalLogic.VALUE_100000 + t * GlobalLogic.VALUE_10000 + m * GlobalLogic.VALUE_1000 + level;
            // int key = d * GlobalLogic.VALUE_100000 + t * GlobalLogic.VALUE_10000 + m * GlobalLogic.VALUE_1000 + level + m;
            string keystr = d + "" + t + "" + m + "" + level;
            int key = int.Parse(keystr);
            if (!monsterAttributeMap.ContainsKey(key))
                monsterAttributeMap.Add(key, enumerator.Current.Value);
        }
    }

    public object GetMonsterAttribute(int mode, int difficulty, int type, int level)
    {
        object obj = null;
        try
        {
            // int key = difficulty * GlobalLogic.VALUE_100000 + type * GlobalLogic.VALUE_10000 + mode * GlobalLogic.VALUE_1000 + level;
            // int key = difficulty * GlobalLogic.VALUE_100000 + type * GlobalLogic.VALUE_10000 + mode * GlobalLogic.VALUE_1000 + level + mode;
            string keystr = difficulty + "" + type + "" + mode + "" + level;
            int key = int.Parse(keystr);
            obj = monsterAttributeMap[key];
        }
        catch
        {
            Logger.LogErrorFormat("[获取怪物属] 出错 mode : {0}, diff {1}, type {2}, level {3}", mode, difficulty, type, level);
        }

        return obj;
    }

    bool _isProfessionValid(int pro)
    {
        return pro > 0 && pro <= Global.MAX_PROFESSION;
    }

    void _loadPKHPAdjustInfo()
    {
        // 		pkHPProfessionAdjustInfo = new int[Global.MAX_PROFESSION];
        // 
        // 		var table = mTypeTableDict[typeof(PkHPProfessionAdjustTable)];
        // 		Dictionary<int, object>.Enumerator enumerator = table.GetEnumerator();
        // 		while (enumerator.MoveNext())
        // 		{
        // 			var item = (ProtoTable.PkHPProfessionAdjustTable)enumerator.Current.Value;
        // 			if (_isProfessionValid(item.Profession1) && _isProfessionValid(item.Profession2))
        // 			{
        // 				pkHPProfessionAdjustInfo[item.Profession1,item.Profession2] = item.factor / (float)(GlobalLogic.VALUE_1000);
        // 			}
        // 			else
        // 				Logger.LogErrorFormat("决斗场血量职业调整表填写有问题，id={0} 职业1={1} 职业2={2}", enumerator.Current.Key, item.Profession1, item.Profession2);
        // 		}
    }

    public object GetDungeonDifficultyAdjustInfo(int dungeonID, int playerNum)
    {
        if (dungeonDifficultyAdjustInfo.ContainsKey(dungeonID) && playerNum > 0 && playerNum <= Global.MAX_TEAM_PLAYER_NUM)
        {
            return dungeonDifficultyAdjustInfo[dungeonID][playerNum];
        }

        return null;
    }

    void _loadDungeonDifficultyAdjustInfo()
    {
        var table = mTypeTableDict[typeof(DungeonDifficultyAdjustTable)];
        Dictionary<int, object>.Enumerator enumerator = table.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = (DungeonDifficultyAdjustTable)enumerator.Current.Value;
            int dungeonID = item.DungeonID;
            if (!dungeonDifficultyAdjustInfo.ContainsKey(dungeonID))
                dungeonDifficultyAdjustInfo.Add(dungeonID, new object[Global.MAX_TEAM_PLAYER_NUM + 1]);

            if (item.PlayerNum > 0 && item.PlayerNum <= Global.MAX_TEAM_PLAYER_NUM)
                dungeonDifficultyAdjustInfo[dungeonID][item.PlayerNum] = enumerator.Current.Value;
        }
    }
    void _loadTreasureDungeonInfo()
    {
        Dictionary<int, object> tables = TableManager.GetInstance().GetTable<ProtoTable.RandomDungeonTable>();
        var iter = tables.GetEnumerator();
        while (iter.MoveNext())
        {
            var curTable = iter.Current.Value as ProtoTable.RandomDungeonTable;
            if (curTable == null) continue;
            if (!treasureRoomRandomLibrary.ContainsKey(curTable.dungeonType))
            {
                treasureRoomRandomLibrary.Add(curTable.dungeonType, new List<string>());
            }
            var roomList = treasureRoomRandomLibrary[curTable.dungeonType];
            if (!roomList.Contains(curTable.DungeonRoom))
            {
                roomList.Add(curTable.DungeonRoom);
            }
        }
    }
    public List<string> GetTreasureDungeonRandomLibrary(int roomType)
    {
        if (treasureRoomRandomLibrary.ContainsKey(roomType))
        {
            return treasureRoomRandomLibrary[roomType];
        }
        return null;
    }
    void _LoadTeamDungeonInfo()
    {
        var TeamDungeonTableDatas = mTypeTableDict[typeof(TeamDungeonTable)];

        foreach (var item in TeamDungeonTableDatas)
        {
            TeamDungeonTable TeamDungeonTableItem = item.Value as TeamDungeonTable;

            if (TeamDungeonTableItem.ShowIndependent == 1)
            {
                TeamDungeonFirstMenuList.Add(TeamDungeonTableItem.ID);
            }
            else
            {
                if (TeamDungeonTableItem.Type == TeamDungeonTable.eType.MENU)
                {
                    TeamDungeonFirstMenuList.Add(TeamDungeonTableItem.ID);

                    List<int> SecondMenu = new List<int>();

                    if (!TeamDungeonSecondMenuDict.TryGetValue(TeamDungeonTableItem.ID, out SecondMenu))
                    {
                        SecondMenu = new List<int>();

                        TeamDungeonSecondMenuDict.Add(TeamDungeonTableItem.ID, SecondMenu);
                    }
                }
                else if (TeamDungeonTableItem.Type == TeamDungeonTable.eType.DUNGEON
                         || TeamDungeonTableItem.Type == TeamDungeonTable.eType.CityMonster)
                {
                    List<int> SecondMenu = new List<int>();

                    if (TeamDungeonSecondMenuDict.TryGetValue(TeamDungeonTableItem.MenuID, out SecondMenu))
                    {
                        SecondMenu.Add(TeamDungeonTableItem.ID);
                    }
                    else
                    {
                        SecondMenu = new List<int>();

                        SecondMenu.Add(TeamDungeonTableItem.ID);
                        TeamDungeonSecondMenuDict.Add(TeamDungeonTableItem.MenuID, SecondMenu);
                    }
                }
            }
        }
    }


    void _LoadNewbieGuideOrderInfo()
    {
        var NewbieGuideTableDatas = mTypeTableDict[typeof(NewbieGuideTable)];
        var datas = NewbieGuideTableDatas.GetEnumerator();

        while (datas.MoveNext())
        {
            NewbieGuideTable data = datas.Current.Value as NewbieGuideTable;

            if (data.ID == 1 || data.IsOpen == 0)
            {
                continue;
            }

            if (NewbieGuideOrderList.Count == 0)
            {
                NewbieGuideOrderList.Add(data.ID);
            }
            else
            {
                for (int i = 0; i < NewbieGuideOrderList.Count; i++)
                {
                    NewbieGuideTable NewbieGuideTabledata = GetTableItem<NewbieGuideTable>(NewbieGuideOrderList[i]);
                    if (NewbieGuideTabledata == null)
                    {
                        continue;
                    }

                    if (NewbieGuideTabledata.Order <= data.Order)
                    {
                        if (i == (NewbieGuideOrderList.Count - 1))
                        {
                            NewbieGuideOrderList.Add(data.ID);
                            break;
                        }

                        continue;
                    }

                    NewbieGuideOrderList.Insert(i, data.ID);
                    break;
                }
            }
        }
    }

    public float GetPKHPAdjustFactor(int ourPro, int raceType)
    {
        if (!_isProfessionValid(ourPro))
        {
            Logger.LogErrorFormat("GetPKHPAdjustFactor error ourPro:{0}", ourPro);
            return 1.0f;
        }

        //return pkHPProfessionAdjustInfo[ourPro, targetPro];

        var data = TableManager.GetInstance().GetTableItem<ProtoTable.PkHPProfessionAdjustTable>(ourPro);

        if (data != null)
        {
            if (raceType == (int)Protocol.RaceType.ScoreWar)
            {
                return data.factor_3v3 / (float)(GlobalLogic.VALUE_1000);
            }
            else if (raceType == (int)Protocol.RaceType.ChiJi)
            {
                return data.factor_chiji / (float)(GlobalLogic.VALUE_1000);
            }
            return data.factor / (float)(GlobalLogic.VALUE_1000);
        }

        return 1.0f;
    }

    void _loadAuctionClassifyInfo()
    {
        var AuctionClassifyDatas = mTypeTableDict[typeof(AuctionClassify)];

        foreach (var item in AuctionClassifyDatas)
        {
            AuctionClassify ClassifyItem = item.Value as AuctionClassify;

            if (ClassifyItem.IsFirstNode != 1)
            {
                continue;
            }

            if (ClassifyItem.Type == AuctionClassify.eType.AT_EQUIP)
            {
                if (ClassifyItem.Job == AuctionClassify.eJob.AC_JIANSHI)
                {
                    AuctionGuiJianShiWeaponType.Add(ClassifyItem.ID);
                }
                else if (ClassifyItem.Job == AuctionClassify.eJob.AC_QIANGSHOU)
                {
                    AuctionShenQiangShouWeaponType.Add(ClassifyItem.ID);
                }
                else if (ClassifyItem.Job == AuctionClassify.eJob.AC_FASHI)
                {
                    AuctionMoFaShiWeaponType.Add(ClassifyItem.ID);
                }
                else if (ClassifyItem.Job == AuctionClassify.eJob.AC_GEDOU)
                {
                    AuctionGeDouJiaWeaponType.Add(ClassifyItem.ID);
                }
            }
            else if (ClassifyItem.Type == AuctionClassify.eType.AT_DEFENCE)
            {
                AuctionDefenceType.Add(ClassifyItem.ID);
            }
            else if (ClassifyItem.Type == AuctionClassify.eType.AT_JEWELRY)
            {
                AuctionJewelryType.Add(ClassifyItem.ID);
            }
            else if (ClassifyItem.Type == AuctionClassify.eType.AT_ARMOR)
            {
                AuctionArmorType.Add(ClassifyItem.ID);
            }
            else if (ClassifyItem.Type == AuctionClassify.eType.AT_QUALITY)
            {
                AuctionQualityType.Add(ClassifyItem.ID);
            }
            else if (ClassifyItem.Type == AuctionClassify.eType.AT_EXPENDABLE)
            {
                AuctionConsumablesType.Add(ClassifyItem.ID);
            }
            else if (ClassifyItem.Type == AuctionClassify.eType.AT_MATERIAL)
            {
                AuctionMaterialsType.Add(ClassifyItem.ID);
            }
        }
    }

    public List<int> GetAuctionWeaponList(AuctionClassify.eJob eJobType)
    {
        if (eJobType == AuctionClassify.eJob.AC_JIANSHI)
        {
            return AuctionGuiJianShiWeaponType;
        }
        else if (eJobType == AuctionClassify.eJob.AC_QIANGSHOU)
        {
            return AuctionShenQiangShouWeaponType;
        }
        else if (eJobType == AuctionClassify.eJob.AC_FASHI)
        {
            return AuctionMoFaShiWeaponType;
        }
        else if (eJobType == AuctionClassify.eJob.AC_GEDOU)
        {
            return AuctionGeDouJiaWeaponType;
        }
        else
        {
            return null;
        }
    }

    public List<int> GetAuctionDefenceList()
    {
        return AuctionDefenceType;
    }

    public List<int> GetAuctionJewelryList()
    {
        return AuctionJewelryType;
    }

    public List<int> GetAuctionArmorList()
    {
        return AuctionArmorType;
    }

    public List<int> GetAuctionQualityList()
    {
        return AuctionQualityType;
    }

    public List<int> GetAuctionConsumablesList()
    {
        return AuctionConsumablesType;
    }

    public List<int> GetAuctionMaterialsList()
    {
        return AuctionMaterialsType;
    }

    public bool IsAuctionJianshiWeaponSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionGuiJianShiWeaponType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionGuiJianShiWeaponType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionQiangshouWeaponSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionShenQiangShouWeaponType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionShenQiangShouWeaponType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionFashiWeaponSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionMoFaShiWeaponType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionMoFaShiWeaponType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionGeDouJiaWeaponSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionGeDouJiaWeaponType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionGeDouJiaWeaponType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionDefenceSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionDefenceType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionDefenceType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionJewelrySum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionJewelryType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionJewelryType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionConsumablesSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionConsumablesType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionConsumablesType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionMaterialsSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionMaterialsType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionMaterialsType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionArmorSum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionArmorType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionArmorType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public bool IsAuctionQualitySum(int iIndex)
    {
        if (iIndex < 0 || iIndex >= AuctionQualityType.Count)
        {
            return false;
        }

        AuctionClassify data = GetTableItem<AuctionClassify>(AuctionQualityType[iIndex]);
        if (data == null)
        {
            return false;
        }

        if (data.Sum == AuctionClassify.eSum.SUM_ALL)
        {
            return true;
        }

        return false;
    }

    public int GetJobIDByFightID(int fightID)
    {
        var dic = mTypeTableDict[typeof(JobTable)];

        Dictionary<int, object>.Enumerator enumerator = dic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var data = enumerator.Current.Value as ProtoTable.JobTable;
            if (data != null && data.FightID == fightID)
            {
                return enumerator.Current.Key;
            }
        }

        return 0;
    }

    public List<int> GetTeamDungeonFirstMenuList()
    {
        return TeamDungeonFirstMenuList;
    }

    public Dictionary<int, List<int>> GetTeamDungeonSecondMenuDict()
    {
        return TeamDungeonSecondMenuDict;
    }

    public List<int> GetNewbieGuideOrderList()
    {
        return NewbieGuideOrderList;
    }

    #region SETGET
    public int GetMonsterTableProperty(AttributeType attributeType, ProtoTable.UnitTable data) {
        int value = 0;
        switch (attributeType)
        {
            case AttributeType.maxHp:
                value = data.maxHp;
                break;
            case AttributeType.maxMp:
                value = data.maxMp;
                break;
            case AttributeType.hpRecover:
                value = data.hpRecover;
                break;
            case AttributeType.mpRecover:
                value = data.mpRecover;
                break;
            case AttributeType.attack:
                value = data.attack;
                break;
            case AttributeType.magicAttack:
                value = data.magicAttack;
                break;
            case AttributeType.defence:
                value = data.defence;
                break;
            case AttributeType.magicDefence:
                value = data.magicDefence;
                break;
            case AttributeType.attackSpeed:
                value = data.attackSpeed;
                break;
            case AttributeType.spellSpeed:
                value = data.spellSpeed;
                break;
            case AttributeType.moveSpeed:
                value = data.moveSpeed;
                break;
            case AttributeType.ciriticalAttack:
                value = data.ciriticalAttack;
                break;
            case AttributeType.ciriticalMagicAttack:
                value = data.ciriticalMagicAttack;
                break;
            case AttributeType.dex:
                value = data.dex;
                break;
            case AttributeType.dodge:
                value = data.dodge;
                break;
            case AttributeType.frozen:
                value = data.frozen;
                break;
            case AttributeType.hard:
                value = data.hard;
                break;
            case AttributeType.cdReduceRate:
                value = data.cdReduceRate;
                break;
            /*
		case AttributeType.abnormalResist:
			value = data.abnormalResist;
			break;
		case AttributeType.criticalPercent:
			value = data.criticalPercent;
			break;
		
		case AttributeType.cdReduceRateMagic:
			value = data.cdReduceRateMagic;
			break;
		case AttributeType.mpCostReduceRate:
			value = data.mpCostReduceRate;
			break;
		case AttributeType.mpCostReduceRateMagic:
			value = data.mpCostReduceRateMagic;
			break;
		case AttributeType.attackAddRate:
			value = data.attackAddRate;
			break;
		case AttributeType.magicAttackAddRate:
			value = data.magicAttackAddRate;
			break;
        */
            case AttributeType.baseAtk:
                value = data.baseAtk;
                break;
            case AttributeType.baseInt:
                value = data.baseInt;
                break;
            case AttributeType.baseSpr:
                value = data.spr;
                break;
            case AttributeType.baseSta:
                value = data.sta;
                break;
            case AttributeType.ignoreDefAttackAdd:
                value = data.ignoreDefAttackAdd;
                break;
            case AttributeType.ignoreDefMagicAttackAdd:
                value = data.ignoreDefMagicAttackAdd;
                break;
                /*
                case AttributeType.attackReduceRate:
                    value = data.attackReduceRate;
                    break;
                case AttributeType.magicAttackReduceRate:
                    value = data.magicAttackReduceRate;
                    break;
                case AttributeType.attackReduceFix:
                    value = data.attackReduceFix;
                    break;
                case AttributeType.magicAttackReduceFix:
                    value = data.magicAttackReduceFix;
                    break;
                case AttributeType.defenceAddRate:
                    value = data.defenceAddRate;
                    break;
                case AttributeType.magicDefenceAddRate:
                    value = data.magicDefenceAddRate;
                    break;*/
        }
        return value;
    }

    public int GetMonsterAttributeTableProperty(AttributeType attributeType, ProtoTable.MonsterAttributeTable data) {
        int value = 0;
        switch (attributeType)
        {
            case AttributeType.maxHp:
                value = data.maxHp;
                break;
            case AttributeType.maxMp:
                value = data.maxMp;
                break;
            case AttributeType.hpRecover:
                value = data.hpRecover;
                break;
            case AttributeType.mpRecover:
                value = data.mpRecover;
                break;
            case AttributeType.attack:
                value = data.attack;
                break;
            case AttributeType.magicAttack:
                value = data.magicAttack;
                break;
            case AttributeType.defence:
                value = data.defence;
                break;
            case AttributeType.magicDefence:
                value = data.magicDefence;
                break;
            case AttributeType.attackSpeed:
                value = data.attackSpeed;
                break;
            case AttributeType.spellSpeed:
                value = data.spellSpeed;
                break;
            case AttributeType.moveSpeed:
                value = data.moveSpeed;
                break;
            case AttributeType.ciriticalAttack:
                value = data.ciriticalAttack;
                break;
            case AttributeType.ciriticalMagicAttack:
                value = data.ciriticalMagicAttack;
                break;
            case AttributeType.dex:
                value = data.dex;
                break;
            case AttributeType.dodge:
                value = data.dodge;
                break;
            case AttributeType.frozen:
                value = data.frozen;
                break;
            case AttributeType.hard:
                value = data.hard;
                break;
            case AttributeType.cdReduceRate:
                value = data.cdReduceRate;
                break;
            /*
        case AttributeType.abnormalResist:
            value = data.abnormalResist;
            break;
        case AttributeType.criticalPercent:
            value = data.criticalPercent;
            break;
        case AttributeType.cdReduceRateMagic:
            value = data.cdReduceRateMagic;
            break;
        case AttributeType.mpCostReduceRate:
            value = data.mpCostReduceRate;
            break;
        case AttributeType.mpCostReduceRateMagic:
            value = data.mpCostReduceRateMagic;
            break;
        case AttributeType.attackAddRate:
            value = data.attackAddRate;
            break;
        case AttributeType.magicAttackAddRate:
            value = data.magicAttackAddRate;
            break;
        */
            case AttributeType.baseAtk:
                value = data.baseAtk;
                break;
            case AttributeType.baseInt:
                value = data.baseInt;
                break;
            case AttributeType.baseSpr:
                value = data.spr;
                break;
            case AttributeType.baseSta:
                value = data.sta;
                break;
            case AttributeType.ignoreDefAttackAdd:
                value = data.ignoreDefAttackAdd;
                break;
            case AttributeType.ignoreDefMagicAttackAdd:
                value = data.ignoreDefMagicAttackAdd;
                break;
                /*
                case AttributeType.attackReduceRate:
                    value = data.attackReduceRate;
                    break;
                case AttributeType.magicAttackReduceRate:
                    value = data.magicAttackReduceRate;
                    break;
                case AttributeType.attackReduceFix:
                    value = data.attackReduceFix;
                    break;
                case AttributeType.magicAttackReduceFix:
                    value = data.magicAttackReduceFix;
                    break;
                case AttributeType.defenceAddRate:
                    value = data.defenceAddRate;
                    break;
                case AttributeType.magicDefenceAddRate:
                    value = data.magicDefenceAddRate;
                    break;*/
        }
        return value;
    }

    public ProtoTable.UnionCell GetBuffTableProperty(AttributeType attributeType, ProtoTable.BuffTable data) {
        ProtoTable.UnionCell value = null;
        switch (attributeType)
        {
            case AttributeType.baseAtk:
                value = data.baseAtk;
                break;
            case AttributeType.baseInt:
                value = data.baseInt;
                break;
            case AttributeType.baseIndependence:
                value = data.baseIndependent;
                break;
            case AttributeType.maxHp:
                value = data.maxHp;
                break;
            case AttributeType.maxMp:
                value = data.maxMp;
                break;
            case AttributeType.hpRecover:
                value = data.hpRecover;
                break;
            case AttributeType.mpRecover:
                value = data.mpRecover;
                break;
            case AttributeType.attack:
                value = data.attack;
                break;
            case AttributeType.magicAttack:
                value = data.magicAttack;
                break;
            case AttributeType.defence:
                value = data.defence;
                break;
            case AttributeType.magicDefence:
                value = data.magicDefence;
                break;
            case AttributeType.attackSpeed:
                value = data.attackSpeed;
                break;
            case AttributeType.spellSpeed:
                value = data.spellSpeed;
                break;
            case AttributeType.moveSpeed:
                value = data.moveSpeed;
                break;
            case AttributeType.ciriticalAttack:
                value = data.ciriticalAttack;
                break;
            case AttributeType.ciriticalMagicAttack:
                value = data.ciriticalMagicAttack;
                break;
            case AttributeType.dex:
                value = data.dex;
                break;
            case AttributeType.dodge:
                value = data.dodge;
                break;
            case AttributeType.frozen:
                value = data.frozen;
                break;
            case AttributeType.hard:
                value = data.hard;
                break;
            case AttributeType.abnormalResist:
                value = data.abnormalResist;
                break;
            case AttributeType.criticalPercent:
                value = data.criticalPercent;
                break;
            case AttributeType.cdReduceRate:
                value = data.cdReduceRate;
                break;
            case AttributeType.ingnoreDefRate:
                value = data.ingnoreDefRate;
                break;
            case AttributeType.ingnoreMagicDefRate:
                value = data.ingnoreMagicDefRate;
                break;
            /*
		case AttributeType.cdReduceRateMagic:
			value = data.cdReduceRateMagic;
			break;
		case AttributeType.mpCostReduceRate:
			value = data.mpCostReduceRate;
			break;
		case AttributeType.mpCostReduceRateMagic:
			value = data.mpCostReduceRateMagic;
			break;
			*/
            case AttributeType.attackAddRate:
                value = data.attackAddRate;
                break;
            case AttributeType.magicAttackAddRate:
                value = data.magicAttackAddRate;
                break;
            /*
		case AttributeType.ignoreDefAttackAdd:
			value = data.ignoreDefAttackAdd;
			break;
		case AttributeType.ignoreDefMagicAttackAdd:
			value = data.ignoreDefMagicAttackAdd;
			break;
		case AttributeType.attackReduceRate:
			value = data.attackReduceRate;
			break;
		case AttributeType.magicAttackReduceRate:
			value = data.magicAttackReduceRate;
			break;
		case AttributeType.attackReduceFix:
			value = data.attackReduceFix;
			break;

		case AttributeType.magicAttackReduceFix:
			value = data.magicAttackReduceFix;
			break;
			*/
            case AttributeType.defenceAddRate:
                value = data.defenceAddRate;
                break;
            case AttributeType.magicDefenceAddRate:
                value = data.magicDefenceAddRate;
                break;
        }
        return value;
    }

    public ProtoTable.UnionCell GetBuffTablePropertyByName(string itemName, ProtoTable.BuffTable data) {
        ProtoTable.UnionCell cell = null;
        switch (itemName)
        {
            case "atkAddRate":
                cell = data.atkAddRate;
                break;
            case "intAddRate":
                cell = data.intAddRate;
                break;
            case "staAddRate":
                cell = data.staAddRate;
                break;
            case "sprAddRate":
                cell = data.sprAddRate;
                break;
            case "maxHpAddRate":
                cell = data.maxHpAddRate;
                break;
            case "maxMpAddRate":
                cell = data.maxMpAddRate;
                break;
            case "ignoreDefAttackAddRate":
                cell = data.ignoreDefAttackAddRate;
                break;
            case "ignoreDefMagicAttackAddRate":
                cell = data.ignoreDefMagicAttackAddRate;
                break;
            case "independenceAddRate":
                cell = data.independentAddRate;
                break;
        }

        return cell;
    }

    #endregion

    public enum SkillType
    {
        None,
        BuffSkill,
        QteSkill,
        RunAttackSkill,
        AwakeSkill,
    }

    /// <summary>
    /// 指定的技能Id是否是Buff技能
    /// </summary>
    public SkillType GetSkillType(int skillId)
    {
        if (skillId < 0) return SkillType.None;
        var dic = mTypeTableDict[typeof(SkillTable)];
        if (dic == null) return SkillType.None;
        if (!dic.ContainsKey(skillId)) return SkillType.None;
        var skillData = dic[skillId] as SkillTable;
        if (skillData == null) return SkillType.None;

        if (skillData.IsBuff == 1)
            return SkillType.BuffSkill;
        else if (skillData.IsQTE == 1)
            return SkillType.QteSkill;
        else if (skillData.IsRunAttack == 1)
            return SkillType.RunAttackSkill;
        else if (skillData.SkillCategory == 4 && skillData.SkillType == SkillTable.eSkillType.ACTIVE)
            return SkillType.AwakeSkill;
        return SkillType.None;
    }
}
