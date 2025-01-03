using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
using GameClient;
using ProtoTable;
using Protocol;
using Network;

/// <summary>
/// 地下城数据
/// 只提供数据
/// </summary>
public class DungeonDataManager : IDungeonPreloadAssets
{
    #region 预加载的数据接口
    public string PreloadPath()
    {
        return mTable.DungeonConfig;
    }

    public void Preload(string path)
    {
        var dungeonData = mAsset;

        //!!
        // 	    if (GameClient.SwitchFunctionUtility.IsOpen(11))
        // 		{
        // 			//加载怪物技能配置文件
        // 			for(int i=0; i<dungeonData.GetAreaConnectListLength(); ++i)
        // 			{
        // 				ISceneData sceneData = dungeonData.GetAreaConnectList(i).GetSceneData();
        //                 if (null == sceneData)
        //                 {
        //                     continue;
        //                 }
        // 
        // 				for(int j=0; j<sceneData.GetMonsterInfoLength(); ++j)
        // 				{
        // 					//string modelPath = item.scenedata._monsterinfo[j].GetModelPathByResID();
        // 
        // 					MonsterIDData mdata = new MonsterIDData(sceneData.GetMonsterInfo(j).GetEntityInfo().GetResid());
        // 
        // 					var unit = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(mdata.mid);
        // 					if (unit != null)
        // 					{
        // 						var resID = unit.Mode;
        // 						PreloadManager.PreloadSkillFile(resID);
        // 					}
        // 				}
        // 			}
        // 		}
        if (battleType == BattleType.TreasureMap)
            return;
        for (int i = 0; i < dungeonData.GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData conData = dungeonData.GetAreaConnectList(i);
            if (null == conData)
            {
                continue;
            }

            ISceneData sceneData = conData.GetSceneData();
            if (null == sceneData)
            {
                conData.SetSceneData(DungeonUtility.LoadSceneData(conData.GetSceneAreaPath()));
                sceneData = conData.GetSceneData();
            }

            if (null == sceneData)
            {
                continue;
            }

            for (int j = 0; j < sceneData.GetDecoratorInfoLenth(); ++j)
            {
                string modelPath = sceneData.GetDecoratorInfo(j).GetEntityInfo().GetModelPathByResID();
                CResPreloader.instance.AddRes(modelPath, false);
            }

/*            for (int j = 0; j < item.scenedata._monsterinfo.Length; ++j)
            {
                string modelPath = item.scenedata._monsterinfo[j].GetModelPathByResID();
                CResPreloader.instance.AddRes(modelPath, false);
            }*/

            for (int j = 0; j < sceneData.GetDestructibleInfoLength(); ++j)
            {
                string modelPath = sceneData.GetDestructibleInfo(j).GetEntityInfo().GetModelPathByResID();
                CResPreloader.instance.AddRes(modelPath, false);
            }

            for (int j = 0; j < sceneData.GetTransportDoorLength(); ++j)
            {
                string modelPath = sceneData.GetTransportDoor(j).GetRegionInfo().GetEntityInfo().GetModelPathByResID();
                CResPreloader.instance.AddRes(modelPath, false);
            }

            for (int j = 0; j < sceneData.GetRegionInfoLength(); ++j)
            {
                string modelPath = sceneData.GetRegionInfo(j).GetEntityInfo().GetModelPathByResID();
                CResPreloader.instance.AddRes(modelPath, false);
            }
        }

        PreloadData[] preloadData = new PreloadData[]{
			new PreloadData("Actor/Other_ShadowPlane/p_ShadowPlane", 1),//纸片影子

            new PreloadData(ETCButton.skEffectOnCDFinishBuff, 1),

			//全局配置
			new PreloadData(Global.Settings.defaultHitEffect, 1),
			new PreloadData(Global.Settings.defaultProjectileHitEffect, 1),
			new PreloadData(Global.Settings.critialDeadEffect, 1),
			new PreloadData(Global.Settings.immediateDeadEffect, 1),
			new PreloadData(Global.Settings.normalDeadEffect, 1),

			new PreloadData("Effects/Scene_effects/BOSS/Prefab/Eff_BOSS_GH", 1),//待定
			new PreloadData("Effects/Scene_effects/EffectUI/EffUI_KillMark", 1),//第一场战斗
			new PreloadData("_NEW_RESOURCES/Effects/Common/Prefab/Skill_Common_Bosssiwang_suduxian", 1),
			new PreloadData("Effects/Hero_Zhaohuanshi/Bingnaisi/Prefab/Eff_Zhaohuanbingnaisi_zhaohuan_02", 1),
			new PreloadData("Effects/Common/Sfx/Jiaodi/Prefab/Eff_common_jiaodi", 1),
			//buff内
			new PreloadData("Effects/Hero_Manyou/Siwangzuolun/Prefab/Eff_Siwangzuolun_fire", 1),
            new PreloadData("Effects/Hero_Zhaohuanshi/Kaxiliyasi/Prefab/Eff_kaxiliyasi_xiaoshi", 1),
            new PreloadData("Effects/Hero_Mage/Huzhao/Prefab/Eff_Huzhao_beiji", 1),
            new PreloadData("Effects/Common/Sfx/Huifu/Prefab/Eff_Common_HP", 1),
            new PreloadData("Effects/Hero_Qiangpao/Liangzi/Prefab/Eff_Liangzi_kuang_jiguang_guo", 1),
            new PreloadData("Effects/Scene_effects/EffectUI/Eff_Jinengfanwei_guo", 1),
            new PreloadData("Effects/Hero_Mage/Caoren/Prefab/Eff_Jiekebaodan_yan", 1),

			//被击特效
			new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_guo", 10),
            new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_jin_newguo", 10),
            new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_gun_newguo", 10),
			//死亡特效
			new PreloadData("_NEW_RESOURCES/Effects/Common/Prefab/Skill_Common_hit", 10),
            new PreloadData("Effects/Common/Sfx/Siwang/Eff_die_yiji", 10),
            
            //属性攻击特效
            new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_guang",1),
            new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_huo",1),
            new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_bing",1),
            new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_an",1),
			//施法条,特效
			new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonBarRoot", 1),
			new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorHeadSn", 1),
			new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorHeadPn", 1),
			new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorBar", 1),
			new PreloadData("Effects/Hero_Jixieshi/EZ-8Zibaozhe/Perfab/Eff_Ez-8Zibaozhe_guang", 1),
			//战斗中
			new PreloadData("Effects/Common/Sfx/Hit/Prefab/Eff_hit_miss_newguo", 1),
			new PreloadData("Effects/Common/Sfx/Yaogan/Prefab/Eff_common_yaogan", 1),
			new PreloadData("Effects/Scene_effects/EffectUI/EffUI_Hexue_guo", 1),//喝药特效
			//技能按钮特效
			new PreloadData(ETCButton.skEffectOnClick, 1),
            new PreloadData(ETCButton.skEffectOnCDFinish, 1),
            new PreloadData("Effects/Scene_effects/EffectUI/EffUI_Autoskill_hong_guo", 1),
            new PreloadData("Effects/Scene_effects/EffectUI/EffUI_Autoskill_chixu_guo", 1),
            new PreloadData("Effects/Scene_effects/EffectUI/EffUI_Autoskill_chixu", 1),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonButtonStateCharge", 1),
            new PreloadData("Effects/Scene_effects/EffectUI/EffUI_Autoskill_lan_guo", 1),

			//head text
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/PlayerInfo_SpecialAttack", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/PlayerInfo_BuffName", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/PlayerInfo_GetExp", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/PlayerInfo_GetGold", 1),

			//怪物召唤出特效
			new PreloadData("Effects/Hero_Zhaohuanshi/Aosuo/Prefab/Eff_Zhaohuanaosuo_dimian_guo", 1),
			//门头顶的箭头
			new PreloadData("Effects/Common/Sfx/Jiaodi/Prefab/Eff_toudingjiantou_guo", 1),
			
			//双摇杆
			new PreloadData("UIFlatten/Prefabs/ETCInput/HGJoystick", 1),
			
			//UI
			new PreloadData("UIFlatten/Prefabs/CommonSystemNotify/TipGWQCAnimation", 1),

            //伤害数字预制体加载
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/NormalHurtTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/NormalHurtWhiteTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/CriticleHurtTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/CriticleHurtWhiteTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/HurtOwnTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/HurtOwnWhiteTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/BuffHurtTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/BuffHurtWhiteTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/HurtFriendTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/HPTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/MPTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/NormalAttachTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/CriticleAttachTextRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/CriticleHurtImageRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/MissImageRenderer", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/New/BuffNameTextRenderer", 1),
        };

        //关卡才要预加载
        PreloadData[] preloadData2 = new PreloadData[]{

            new PreloadData("UIFlatten/Prefabs/Common/FadingFrame", 1),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonMonsterHpBar_Green",1),

            //head text
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/PlayerInfo_GetExp", 1),
            new PreloadData("UIFlatten/Prefabs/Battle_Digit/PlayerInfo_GetGold", 1),
            
            //自动战斗
			//new PreloadData("Effects/Scene_effects/EffectUI/EffUI_Autofight", 1),
            new PreloadData("Effects/Scene_effects/EffectUI/EffUI_huoqu_guo", 1),

            //怪物头顶对话框
			new PreloadData("UIFlatten/Prefabs/DialogParent/DialogParent_battle", 1),
            new PreloadData("UIFlatten/Prefabs/DialogParent/DialogParent_battle_skill", 1),

            //boss血条怪物死亡特效
			//new PreloadData("Effects/Scene_effects/EffectUI/EffUI_guaiwusiwang", 1),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonBar/HPBar_White", 1),
            //升级特效
			new PreloadData("Effects/Common/Sfx/Levelup/Prefab/Eff_Common_levelup", 1),
			//复活特效
			new PreloadData("Effects/Common/Sfx/level_up/Prefab/Eff_fuhuo_guo", 1),

            //掉落(pve)
			new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonBox", 1),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DropItem/DropItemIconRoot", 3),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DropItem/DropItemNameRoot", 3),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DropItem/DropItemSpecailDescRoot", 3),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DropItem/DropItemTextBgRoot", 3),
            new PreloadData("UIFlatten/Prefabs/BattleUI/DropItem/DropItemEffectRoot", 3),

            new PreloadData("UI/Image/Icon/Icon_Item/Drop_Normal", 1),
            new PreloadData("UI/Image/Icon/Icon_Item/Drop_Normal_Material", 1),
            new PreloadData("UI/Image/Icon/Icon_Item/Drop_Gold", 1),
            new PreloadData("UI/Image/Icon/Icon_Item/Drop_Gold_Material", 1),

            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_jinbi_tuowei", 1),
            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_jinbi_shiqu_guo", 1),
            new PreloadData("Effects/Scene_effects/Eff_jipinzhuangbei_dimian_guo",1),
            new PreloadData("Effects/Scene_effects/Eff_jipinzhuangbei_dimian_guo02",1),
            new PreloadData("Effects/Scene_effects/Eff_fensezhuangbei_guo",1),

            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_jinbi_yuandi", 1),
            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_putong_yuandi", 1),
            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_jinse_yuandi", 1),
            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_fense_yuandi", 1),
            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_putong_tuowei",1),
            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_jinse_tuowei",1),
            new PreloadData("Effects/Common/Sfx/DiaoLuo/Eff_fense_tuowei",1), 

   //         // 结算界面 DungeonFinish
			//new PreloadData("UIFlatten/Prefabs/Battle/Finish/DungeonNormalFinish", 1),

   //         // 菜单界面 DungeonMenu
			//new PreloadData("UIFlatten/Prefabs/BattleUI/DungeonMenu", 1),

   //         // 翻牌界面 DungeonReward
			//new PreloadData("UIFlatten/Prefabs/Battle/Reward/DungeonReward", 1),

   //         // 复活界面 DungeonReborn
			//new PreloadData("UIFlatten/Prefabs/Battle/Reborn/DungeonReborn", 1),

   //         // 暂停界面 DungeonPuase
			//new PreloadData("UIFlatten/Prefabs/Battle/Pause/DungeonPause", 1),
            
        };


        //通用，所有战斗都要加载
        for (int i = 0; i < preloadData.Length; ++i)
        {
            CResPreloader.instance.AddRes(preloadData[i].resPath, false, preloadData[i].num);
        }

        //只是PVE关卡,并且不是新手引导关卡和修炼场
        if (!BattleMain.IsModePvP(mBattleType) && mBattleType != BattleType.NewbieGuide && mBattleType != BattleType.TrainingSkillCombo)
        {
            for (int i = 0; i < preloadData2.Length; ++i)
            {
                CResPreloader.instance.AddRes(preloadData2[i].resPath, false, preloadData2[i].num);
            }
        }

		//预加载一些通用音效（被击等)
		var soundTable = TableManager.GetInstance().GetTable<ProtoTable.SoundTable>();
		var iter = soundTable.GetEnumerator();
		ProtoTable.SoundTable soundItem = null;
		while(iter.MoveNext())
		{
			soundItem = iter.Current.Value as SoundTable;
			if (soundItem.Type == 1)
				AudioManager.instance.PreloadSound(soundItem.ID);
		}

        CResPreloader.instance.AddRes("UI/Image/Icon/Icon_Item/consumable_83", false, 1, null, 0, null, CResPreloader.ResType.RES, typeof(Sprite));
        CResPreloader.instance.AddRes("Animat/MaterialAnimatConfig", false);
        CResPreloader.instance.AddRes("UI/Image/NewPacked/NewBattle/Battle_BuffIcon", false, 1, null, 0, null, CResPreloader.ResType.RES, typeof(Sprite));
        CResPreloader.instance.AddRes("UI/Font/new_font/pic_break_action", false, 1, null, 0, null, CResPreloader.ResType.RES, typeof(Sprite));
		CResPreloader.instance.AddRes("UI/Font/new_font/pic_back_hit", false, 1, null, 0, null, CResPreloader.ResType.RES, typeof(Sprite));
    }
    #endregion

    public struct PreloadData
    {
        public string resPath;
        public int num;
        public PreloadData(string path, int num)
        {
            resPath = path;
            this.num = num;
        }
    }

    #region 不可被修改的数据
    /// <summary>
    /// 地下城ID
    /// </summary>
    protected DungeonID mID;

    /// <summary>
    /// 地下城模式
    /// </summary>
    protected eDungeonMode mDungeonMode;

    /// <summary>
    /// 战斗类型
    /// </summary>
    protected BattleType mBattleType = BattleType.None;

    /// <summary>
    /// 表格数据
    /// </summary>
    protected DungeonTable mTable;

    /// <summary>
    /// UI配置数据
    /// </summary>
    protected DungeonUIConfigTable mConfigTable;

    /// <summary>
    /// Asset数据
    /// 地下城链接关系
    /// </summary>
    protected IDungeonData mAsset;

    /// <summary>
    /// 服务端下发数据
    /// </summary>
    protected Battle.BattleInfo mBattleInfo;

    #endregion

    #region 缓存的数据
    /// <summary>
    /// 缓存的数据
    ///
    /// 所有对象的添加删除操作
    /// </summary>
    protected class CacheArea
    {
        public bool visited;

        /// <summary>
        /// 如果要去boss房间，下一个要去的房间索引
        /// </summary>
        public List<int> toboss;

		public List<int> toZhuzi;

        /// <summary>
        /// 正常房间的数据
        /// </summary>
        public Battle.DungeonArea area;

        /// <summary>
        /// 深渊房间的数据
        /// </summary>
        public Battle.DungeonHellInfo hell;

        /// <summary>
        /// 房间数据统计
        /// </summary>
        public DungeonStatistics statistics;

        /// <summary>
        /// 杀死的怪物ID
        /// </summary>
        public List<uint> killdMonster;

        /// <summary>
        /// 捡起的掉落物品ID
        /// </summary>
        public List<uint> pickedItems;
        public List<CustomSceneRegionInfo> dynamicRegionInfoes;
        public List<int> dynamicRegions; ///游戏中自动生成的不是根据场景编辑器生成
    }

    protected List<CacheArea> mCacheAreaList = new List<CacheArea>();

    #endregion

    #region 私有数据
    /// <summary>
    /// 上一个访问的地图数据索引
    /// </summary>
    private int mLastIndex = -1;

    /// <summary>
    /// 当前地图数据索引
    /// </summary>
    private int mCurrentIndex = -1;

    private int lastIndex
    {
        get
        {
            return mLastIndex;
        }
    }

    private int currentIndex
    {
        get
        {
            return mCurrentIndex;
        }

        set
        {
            mLastIndex = mCurrentIndex;
            mCurrentIndex = value;
        }
    }

    /// <summary>
    /// 上一个地图门的类型
    /// </summary>
    private TransportDoorType mLastDoorType;
    #endregion

    #region 直接对外的数据接口
    public DungeonID id
    {
        get
        {
            return mID;
        }
    }

    /// <summary>
    /// DungeonTable
    /// </summary>
    public DungeonTable table
    {
        get
        {
            return mTable;
        }
    }

    /// <summary>
    /// DungeonUIConfigTable
    /// </summary>
    public DungeonUIConfigTable configTable
    {
        get
        {
            return mConfigTable;
        }
    }

    /// <summary>
    /// DDungeonData
    /// </summary>
    public IDungeonData asset
    {
        get
        {
            return mAsset;
        }
    }

    /// <summary>
    /// BattleInfo
    /// </summary>
    public Battle.BattleInfo battleInfo
    {
        get
        {
            return mBattleInfo;
        }
    }

    public eDungeonMode dungeonMode
    {
        get 
        {
            return mDungeonMode;
        }
    }

    public BattleType battleType
    {
        get
        {
            return mBattleType;
        }
    }
    public bool IsHardRaid
    {
        get; private set;
    }
    #endregion

    /// <summary>
    /// AI找门专用
    /// </summary>
    public ISceneTransportDoorData door;

    public DungeonDataManager(BattleType type, eDungeonMode mode, int dungeonID)
    {
        mID = new DungeonID(dungeonID);

        mBattleType = type;

        mDungeonMode = mode;

        mTable = TableManager.instance.GetTableItem<DungeonTable>(mID.dungeonID);
        if (null == mTable)
        {
            Logger.LogErrorFormat("Dungeon Table is nil with {0}", mID.dungeonID);
            return;
        }
        if (mTable != null && mTable.SubType == ProtoTable.DungeonTable.eSubType.S_RAID_DUNGEON)
        {
            int leftValue = mID.dungeonID / 1000;
            int realDiff = leftValue % 10;
            if (realDiff == 1)
            {
                IsHardRaid = true;
            }
        }
        else
        {
            IsHardRaid = false;
        }
        mConfigTable = TableManager.instance.GetTableItem<DungeonUIConfigTable>(mID.dungeonID);
        if (mTable.SubType == DungeonTable.eSubType.S_TREASUREMAP)
        {
            return;
        }
        else
        {
            mAsset = DungeonUtility.LoadDungeonData(mTable.DungeonConfig);
        }
       
        _initInternalData();
    }
    private void _initInternalData()
    {
        if (null == mAsset)
        {
            Logger.LogErrorFormat("Dungeon Asset is nil with {0}", mTable.DungeonConfig);
            return;
        }

        mAsset.SetName(mTable.Name);

        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData data = mAsset.GetAreaConnectList(i);
            data.SetSceneData(DungeonUtility.LoadSceneData(data.GetSceneAreaPath()));
        }

        _prepareDebugData(mDungeonMode);

        // 这个时候BattleDataManager.GetInstance().BattleInfo已经有数据
        mBattleInfo = BattleDataManager.GetInstance().BattleInfo;

        _initData();
        _initPath2BossRoom();
        _initRandData(mDungeonMode);

        _bindNetMessage();

        Logger.LogProcessFormat("[战斗数据] 初始化完成");
    }
    public void OnInitDungeonData(FrameRandomImp rand)
    {
        if (mTable != null && mTable.SubType == DungeonTable.eSubType.S_TREASUREMAP)
        {
            TreasureMapGenerator.BuildTreasureDungeonData(rand, out mAsset);
            _initInternalData();
            _initTreasureMapBattleInfo();
            return;
        }
    }

    private void _initTreasureMapBattleInfo()
    {
        mBattleInfo.areas.Clear();

        var playerInfo = BattleDataManager.GetInstance().PlayerInfo;
        if (playerInfo.Length <= 0) return;
        int dungeonLevel = playerInfo[0].level;
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData item = mAsset.GetAreaConnectList(i);
            var area = new Battle.DungeonArea();
            area.id = _getDataNodeAreaID(item);

            if (item.IsStart())
            {
                BattleDataManager.GetInstance().BattleInfo.startAreaId = area.id;
            }

            ISceneData sceneData = item.GetSceneData();

            if (null == sceneData)
            {
                continue;
            }
            MonsterIDData m_MonsterID = new MonsterIDData(0);
            for (int j = 0; j < sceneData.GetMonsterInfoLength(); ++j)
            {
                m_MonsterID.SetID(sceneData.GetMonsterInfo(j).GetEntityInfo().GetResid());
                area.AddMonster(new Battle.DungeonMonster()
                {
                    id = j,
                    pointId = j,
                    typeId = m_MonsterID.GenFullMonsterID(dungeonLevel),
                });
            }

            for (int j = 0; j < sceneData.GetDestructibleInfoLength(); ++j)
            {
                if (sceneData.GetDestructibleInfo(j).GetFlushGroupID() > 0)
                {
                    area.destructs.Add(new Battle.DungeonMonster()
                    {
                        id = j + sceneData.GetMonsterInfoLength(),
                        pointId = j + sceneData.GetMonsterInfoLength(),
                        typeId = sceneData.GetMonsterInfo(j).GetEntityInfo().GetResid(),
                    });
                }
            }
            area.regions.Clear();
            mBattleInfo.areas.Add(area);
        }
        _initData();
    }
    
    public void OnInitClientDungeonData()
    {
        mBattleInfo.areas.Clear();

        var playerInfo = BattleDataManager.GetInstance().PlayerInfo;
        if (playerInfo.Length <= 0) return;
        int dungeonLevel = playerInfo[0].level;
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData item = mAsset.GetAreaConnectList(i);
            var area = new Battle.DungeonArea();
            area.id = _getDataNodeAreaID(item);

            if (item.IsStart())
            {
                BattleDataManager.GetInstance().BattleInfo.startAreaId = area.id;
            }

            ISceneData sceneData = item.GetSceneData();

            if (null == sceneData)
            {
                continue;
            }
            MonsterIDData m_MonsterID = new MonsterIDData(0);
            for (int j = 0; j < sceneData.GetMonsterInfoLength(); ++j)
            {
                m_MonsterID.SetID(sceneData.GetMonsterInfo(j).GetEntityInfo().GetResid());
                var level = sceneData.GetMonsterInfo(j).GetMonsterLevel();
                if (level == 0)
                {
                    level = dungeonLevel;
                }
                area.AddMonster(new Battle.DungeonMonster()
                {
                    id = j,
                    pointId = j,
                    typeId = m_MonsterID.GenFullMonsterID(level),
                });
            }

            for (int j = 0; j < sceneData.GetDestructibleInfoLength(); ++j)
            {
                if (sceneData.GetDestructibleInfo(j).GetFlushGroupID() > 0)
                {
                    area.destructs.Add(new Battle.DungeonMonster()
                    {
                        id = j + sceneData.GetMonsterInfoLength(),
                        pointId = j + sceneData.GetMonsterInfoLength(),
                        typeId = sceneData.GetMonsterInfo(j).GetEntityInfo().GetResid(),
                    });
                }
            }
            area.regions.Clear();
            mBattleInfo.areas.Add(area);
        }
        _initData();
    }
    
    private void GetRoomPosByType(int roomType, ref int x, ref int y, ref int roomIndex)
    {
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            var data = mAsset.GetAreaConnectList(i);
            if (data != null)
            {
                if (data.GetTreasureType() == (byte)roomType)
                {
                    x = data.GetPositionX();
                    y = data.GetPositionY();
                    roomIndex = i;
                    return;
                }
            }
        }
    }
    //获得大魔王房间坐标
    public void GetBossRoom(ref int x, ref int y, ref int roomIndex)
    {
        GetRoomPosByType((int)TreasureMapGenerator.ROOM_TYPE.BOSS_ROOM, ref x, ref y, ref roomIndex);
    }
    //获得宝藏关卡的坐标
    public void GetEndRoom(ref int x, ref int y)
    {
        int roomIndex = -1;
        GetRoomPosByType((int)TreasureMapGenerator.ROOM_TYPE.END_ROOM, ref x, ref y, ref roomIndex);
    }
    public bool IsRoomDefined(int x, int y)
    {
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            var data = mAsset.GetAreaConnectList(i);
            if (data != null)
            {
                if (data.GetPositionX() == x && data.GetPositionY() == y)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int GetRoomCountByType(byte type)
    {
        int roomCount = 0;
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            var data = mAsset.GetAreaConnectList(i);
            if (data != null)
            {
                if (data.GetTreasureType() == type)
                {
                    roomCount++;
                }
            }
        }
        return roomCount;
    }
    public bool CanWalkToRoom(int srcX, int srcY, int tgtX, int tgtY, ref int nextRoomIndex)
    {
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            var data = mAsset.GetAreaConnectList(i);
            if (data == null) continue;
            if (data.GetPositionX() != srcX || data.GetPositionY() != srcY) continue;
            for (int j = 0; j < 4; ++j)
            {
                if (!data.GetIsConnect(j)) continue;
                var conData = mAsset.GetSideByType(data, (TransportDoorType)j);
                if (conData == null) continue;
                if (conData.GetPositionX() != tgtX || conData.GetPositionY() != tgtY) continue;
                for (int k = 0; k < mAsset.GetAreaConnectListLength(); k++)
                {
                    var curdata = mAsset.GetAreaConnectList(k);
                    if (curdata.GetPositionX() != tgtX || curdata.GetPositionY() != tgtY) continue;
                    nextRoomIndex = k;
                    break;
                }
                return true;
            }
        }
        return false;
    }
    public bool GenBossNextRoom(int bossIndex, FrameRandomImp rand, ref int bossNextX, ref int bossNextY, ref int nextIndex)
    {
        if (rand == null || bossIndex < 0 || bossIndex >= mAsset.GetAreaConnectListLength()) return false;
        var data = mAsset.GetAreaConnectList(bossIndex);
        if (data == null) return false;
        int connectCount = 0;
        IDungeonConnectData nextData = null;
        for (int i = 0; i < 4; ++i)
        {
            if (!data.GetIsConnect(i)) continue;
            nextData = mAsset.GetSideByType(data, (TransportDoorType)i);
            if (nextData != null && nextData.GetTreasureType() != (byte)TreasureMapGenerator.ROOM_TYPE.END_ROOM)
            {
                connectCount++;
            }
        }
        if (connectCount == 0) return false;
        int randIndex = rand.InRange(0, connectCount);
        int validConnectCount = 0;
        int doorTypeIndex = -1;
        nextData = null;
        for (int i = 0; i < 4; ++i)
        {
            if (!data.GetIsConnect(i)) continue;
            nextData = mAsset.GetSideByType(data, (TransportDoorType)i);
            if (nextData != null && nextData.GetTreasureType() != (byte)TreasureMapGenerator.ROOM_TYPE.END_ROOM)
            {
                if (randIndex == validConnectCount)
                {
                    doorTypeIndex = i;
                    break;
                }
                validConnectCount++;
            }
        }
        if (doorTypeIndex < 0) return false;
        if (nextData == null) return false;
        bossNextX = nextData.GetPositionX();
        bossNextY = nextData.GetPositionY();
        mAsset.GetSideByType(data, (TransportDoorType)doorTypeIndex, out nextIndex);
        return true;

    }

    public void Clear()
    {
        _unbindNetMessage();
        _checkNoVertifyDrops();
        SendNoVertifyDrops();

        mID = null;
        mAsset = null;
        mTable = null;
        mConfigTable = null;

        if (null != mCacheAreaList)
        {
            mCacheAreaList.Clear();
        }
        mCacheAreaList = null;

        Logger.LogProcessFormat("[战斗数据] 反初始化完成");
    }
    public bool isDeViILDdOM()
    {
        if(mTable == null)return false;
        return mTable.SubType == ProtoTable.DungeonTable.eSubType.S_DEVILDDOM;
    }
    //[Conditional("UNITY_EDITOR")]
    private void _prepareDebugData(eDungeonMode mode)
    {
        if (eDungeonMode.Test == mode)
        {
            for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
            {
                IDungeonConnectData item = mAsset.GetAreaConnectList(i);
                var area = new Battle.DungeonArea();
                area.id = _getDataNodeAreaID(item);

                if (item.IsStart())
                {
                    BattleDataManager.GetInstance().BattleInfo.startAreaId = area.id;
                }

                {
                    ISceneData sceneData = item.GetSceneData();

                    if (null == sceneData)
                    {
                        continue;
                    }

                    for (int j = 0; j < sceneData.GetMonsterInfoLength(); ++j)
                    {
                        area.AddMonster(new Battle.DungeonMonster()
                        {
                            id = j,
                            pointId = j,
                            typeId = sceneData.GetMonsterInfo(j).GetEntityInfo().GetResid(),
                        });
                    }

                    for (int j = 0; j < sceneData.GetDestructibleInfoLength(); ++j)
                    {
                        if (sceneData.GetDestructibleInfo(j).GetFlushGroupID() > 0)
                        {
                            var summons = new List<Battle.DungeonMonster>();
                            area.destructs.Add(new Battle.DungeonMonster()
                            {
                                id = j + sceneData.GetMonsterInfoLength(),
                                pointId = j + sceneData.GetMonsterInfoLength(),
                                typeId = sceneData.GetDestructibleInfo(j).GetEntityInfo().GetResid(),
                                summonerId = 1,
                                summonerMonsters = summons,
                            });

                            if (area.MonsterCount >= 2)
                            {
                                summons.Add(area.GetMonsterAt(0));
                                summons.Add(area.GetMonsterAt(1));
                            }
                        }
                    }
                }

                BattleDataManager.GetInstance().BattleInfo.areas.Add(area);

                if (mTable.SubType == ProtoTable.DungeonTable.eSubType.S_HELL)
                {
                    int areaId = -1;
                    List<int> allRandList = new List<int>();
                    for (int ii = 0; ii < mAsset.GetAreaConnectListLength(); ++ii)
                    {
                        IDungeonConnectData con = mAsset.GetAreaConnectList(ii);

                        if (null != con && !con.IsStart() && !con.IsBoss())
                        {
                            allRandList.Add(_getDataNodeAreaID(con));
                        }
                    }

                    areaId = allRandList[UnityEngine.Random.Range(0, allRandList.Count - 1)];

                    BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.areaId = areaId;
                    BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.mode = DungeonHellMode.Hard;

                    for (int ii = 0; ii < 3; ii++)
                    {
                        var monsters = new List<Battle.DungeonMonster>();
                        BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.waveInfos.Add(new Battle.DungeonHellWaveInfo()
                        {
                            wave = ii,
                            monsters = monsters,
                        });

                        if (area.MonsterCount >= 2)
                        {
                            monsters.Add(area.GetMonsterAt(0));
                            monsters.Add(area.GetMonsterAt(1));
                        }
                    }

                    BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.dropItems = new List<Battle.DungeonDropItem>();
                    for (int ii = 0; ii < 3; ++ii)
                    {
                        Battle.DungeonDropItem ditem = new Battle.DungeonDropItem();

                        ditem.id     = ii;
                        ditem.typeId = 600000007;
                        ditem.num    = 100;

                        BattleDataManager.GetInstance().BattleInfo.dungeonHealInfo.dropItems.Add(ditem);
                        BattleDataManager.GetInstance().BattleInfo.bossDropItems.Add(ditem);
                    }
                }
            }
        }
    }

    #region public

    #region ChangeArea -- 场景切换
    public bool FirstArea()
    {
        if (battleType == BattleType.ChampionMatch)
        {
            return NextAreaByIndexBaseOnServerData(0);
        }
        else
        {
            int startAreaId = mBattleInfo.startAreaId;
            return _changeIndexByAreaID(startAreaId) >= 0;
        }
    }

    /// <summary>
    /// 根据上一个场景的传送门来切换区域
    /// </summary>
    /// <param name="type"></param>
    public bool NextArea(TransportDoorType type)
    {
        if (battleType == BattleType.ChampionMatch)
        {
            Logger.LogErrorFormat("[战斗] [数据] 不支持当前模式 {0}", type);
            return false;
        }

        var nextAreaNode = mAsset.GetSideByType(currentIndex, type);
        if (null == nextAreaNode)
        {
            Logger.LogErrorFormat("item is nil with curIdx {0}, with type {1}", currentIndex, type);
            return false;
        }

        if (null == nextAreaNode.GetSceneData())
        {
            Logger.LogErrorFormat("item.scenedata is nil : {0} {1}", currentIndex, type);
        }

        mLastDoorType = type;

        int areaId = _getDataNodeAreaID(nextAreaNode);
        return _changeIndexByAreaID(areaId) >= 0;
    }

    public bool IsNextAreaVisited(TransportDoorType type)
    {
        if (battleType == BattleType.ChampionMatch)
        {
            Logger.LogErrorFormat("[战斗] [数据] 不支持当前模式 {0}", type);
            return false;
        }

        var nextAreaNode = mAsset.GetSideByType(currentIndex, type);
        if (nextAreaNode != null)
        {
            CacheArea cacheArea = _getCacheArea(nextAreaNode);
            if (cacheArea != null)
                return cacheArea.visited;
        }

        return false;
    }


    /// <summary>
    /// 根据index切换数据
    /// </summary>
    /// <param name="index"></param>
    public bool NextArea(int index)
    {
        if (battleType == BattleType.ChampionMatch)
        {
            Logger.LogErrorFormat("[战斗] [数据] 不支持当前模式 {0}", battleType);
            return false;
        }

        return _changeIndexByAreaID(index) >= 0;
    }

    public bool NextAreaByIndex(int index, bool dungeon = false)
    {
        if (battleType == BattleType.ChampionMatch)
        {
            Logger.LogErrorFormat("[战斗] [数据] 不支持当前模式 {0}", battleType);
            return false;
        }

        if (dungeon)
        {
            CacheArea area = _currentCacheNode();
            if (battleType == BattleType.Hell && 0 == IsHellAreaVisited())
            {
                if (area != null && area.toZhuzi != null && area.toZhuzi.Count > 0)
                {
                    index = -1;
                    for (int i = 0; i < area.toZhuzi.Count; i++)
                    {
                        index = area.toZhuzi[i];
                        IDungeonConnectData _toArea = mAsset.GetAreaConnectList(index);
                        if (_toArea != null && !_toArea.IsBoss()) break;
                    }
                }
            }
            else
            {
                if (area != null && area.toboss != null && area.toboss.Count > 0)
                {
                    index = area.toboss[0];
                }
            }
        }

        return _changeIndex(index) >= 0;
    }
    
    public int GetNextAreaIndex()
    {
        int index = -1;
        CacheArea area = _currentCacheNode();
        if (battleType == BattleType.Hell && 0 == IsHellAreaVisited())
        {
            if (area != null && area.toZhuzi != null && area.toZhuzi.Count > 0)
            {
                index = -1;
                for (int i = 0; i < area.toZhuzi.Count; i++)
                {
                    index = area.toZhuzi[i];
                    IDungeonConnectData _toArea = mAsset.GetAreaConnectList(index);
                    if (_toArea != null && !_toArea.IsBoss()) break;
                }
            }
        }
        else
        {
            if (area != null && area.toboss != null && area.toboss.Count > 0)
            {
                index = area.toboss[0];
            }
        }

        return index;
    }
    

    /// <summary>
    /// 根据服务器的列表顺序来切换数据
    /// </summary>
    /// <param name="cacheAreaIndex"></param>
    /// <returns></returns>
    public bool NextAreaByIndexBaseOnServerData(int cacheAreaIndex)
    {
        if (battleType != BattleType.ChampionMatch)
        {
            Logger.LogErrorFormat("[战斗] [数据] 不支持当前模式 {0}", battleType);

            return false;
        }

        if (0 > cacheAreaIndex || cacheAreaIndex >= mCacheAreaList.Count)
        {
            return false;
        }

        return _changeIndexByAreaID(mCacheAreaList[cacheAreaIndex].area.id) >= 0;
    }
    #endregion

    #region 获取场景数据

    public bool IsNextAreaBoss(TransportDoorType type)
    {
        List<Battle.DungeonDoor> doors = CurrentDoors();

        for (int i = 0; i < doors.Count; ++i)
        {
            if (doors[i].doorType == type)
            {
                return doors[i].isconnectwithboss;
            }
        }

        return false;
    }

    public bool IsBossArea()
    {
        if (battleType == BattleType.ChampionMatch)
        {
            if (mCacheAreaList.Count > 0)
            {
                return mCacheAreaList[mCacheAreaList.Count - 1].area.id == mBattleInfo.areaId;
            }

            return false;
        }
        else
        {
            IDungeonConnectData node = CurrentDataConnect();
            return null != node && node.IsBoss();
        }
    }

    public bool IsHellArea()
    {
        var node = CurrentDataConnect();
        return null != node && _getDataNodeAreaID(node) == mBattleInfo.dungeonHealInfo.areaId;
    }

    /// <summary>
    /// 是否在boss房间邻近的房间
    /// </summary>
    public bool IsBossAreaNearby()
    {
        List<Battle.DungeonDoor> doors = CurrentDoors();
        for (int i = 0; i < doors.Count; ++i)
        {
            if (doors[i].isconnectwithboss)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///
    /// 是否深渊房间打过
    ///
    /// <return>
    /// 0: 没打过
    /// 1: 打过
    /// -1: 没有深渊房间
    /// </return>
    /// </summary>
    public int IsHellAreaVisited()
    {
        if (null != mBattleInfo.dungeonHealInfo)
        {
            int hellid = mBattleInfo.dungeonHealInfo.areaId;
            CacheArea area = _getCacheAreaByAreaID(hellid);

            if (null != area)
            {
                return area.visited ? 1 : 0;
            }
        }

        return -1;
    }

    /// <summary>
    /// 获得当前场景出生点位置
    /// </summary>
    /// <returns>当前场景出生点位置</returns>
    private VInt3 GetBirthPosition(IDungeonConnectData node)
    {
        if (null == node)
        {
            return VInt3.zero;
        }

        ISceneData sceneData = node.GetSceneData();
        if (null == sceneData)
        {
            return VInt3.zero;
        }

        if (null != sceneData.GetBirthPosition())
        {
            return new VInt3(sceneData.GetBirthPosition().GetPosition());
        }
        else
        {
            var x = new VInt2(sceneData.GetLogicXSize());
            var z = new VInt2(sceneData.GetLogicZSize());
            return new VInt3((x.x + x.y) / 2, 0, (z.x + z.y) / 2);
        }

        return VInt3.zero;
    }

    private VInt3 _getDoorPosition(IDungeonConnectData node, TransportDoorType fromDoorType)
    {
        VInt3 doorBirthPosition = VInt3.zero;

        ISceneData sceneData = node.GetSceneData();
        if (null == sceneData)
        {
            return VInt3.zero;
        }

        ISceneEntityInfoData item = null;

        for (int i = 0; i < sceneData.GetTransportDoorLength(); ++i)
        {
            if (sceneData.GetTransportDoor(i).GetDoortype() == fromDoorType)
            {
                item = sceneData.GetTransportDoor(i).GetRegionInfo().GetEntityInfo();
                break;
            }
        }

        if (item != null)
        {
            return new VInt3(item.GetPosition());
        }

        return doorBirthPosition;
    }

    private VInt3 _getDoorBirthPosition(IDungeonConnectData node, TransportDoorType fromDoorType)
    {
        VInt3 doorBirthPosition = VInt3.zero;

        if (null == node)
        {
            return doorBirthPosition;
        }

        ISceneData sceneData = node.GetSceneData();
        if (null == sceneData)
        {
            return doorBirthPosition;
        }
       

        //var doorList = node.scenedata._transportdoor;
        //var item = Array.Find<DTransportDoor>(doorList, x => { return x.doortype == fromDoorType; });

        ISceneTransportDoorData item = null;
        for (int i = 0; i < sceneData.GetTransportDoorLength(); ++i)
        {
            if (sceneData.GetTransportDoor(i).GetDoortype() == fromDoorType)
            {
                item       = sceneData.GetTransportDoor(i);
                break;
            }
        }

        if (null == item)
        {
            return doorBirthPosition;
        }
        door = item;
        var radius = VInt.Float2VIntValue(item.GetRegionInfo().GetRadius() + 0.4f);
        var regionPosition = new VInt3(item.GetRegionInfo().GetEntityInfo().GetPosition());
        var dis = item.GetBirthposition() - regionPosition;

        if (dis.magnitude > radius * 3)
        {
            Logger.LogWarning("transport door birth position is so long, use the defaulte birth position by door type, doorpostion -> doorbirth");

            switch (mLastDoorType.OppositeType())
            {
                case TransportDoorType.Buttom:
                    doorBirthPosition = regionPosition + new VInt3(0, 0, radius);
                    break;
                case TransportDoorType.Top:
                    doorBirthPosition = regionPosition - new VInt3(0, 0, radius);
                    break;
                case TransportDoorType.Left:
                    doorBirthPosition = regionPosition + new VInt3(radius, 0, 0);
                    break;
                case TransportDoorType.Right:
                    doorBirthPosition = regionPosition - new VInt3(radius, 0, 0);
                    break;
            }
        }
        else
        {
            doorBirthPosition = item.GetBirthposition();
        }

        return doorBirthPosition;
    }


    /// <summary>
    /// 获得通过传送门的出生点
    /// </summary>
    /// <param name="fromDoorType">上一个场景的门的类型</param>
    /// <returns>当前场景对应的门的出生位置</returns>
    private VInt3 GetDoorBirthPosition(IDungeonConnectData node)
    {
        return _getDoorBirthPosition(node, mLastDoorType.OppositeType());
    }

    public VInt3 CurrentBirthPosition()
    {
        var node = CurrentDataConnect();
        return GetBirthPosition(node);
    }

    public VInt3 CurrentDoorBirthPosition(TransportDoorType type)
    {
        var node = CurrentDataConnect();
        return GetDoorBirthPosition(node);
    }

	public VInt3 CurrentGuidePosition(ref TransportDoorType doorType)
    {
        CacheArea area = _currentCacheNode();

        if (area != null && area.toboss != null && area.toboss.Count > 0)
        {
            int idx = area.toboss[0];
            IDungeonConnectData toArea = mAsset.GetAreaConnectList(idx);
            IDungeonConnectData fromArea = CurrentDataConnect();

            int door = mAsset.GetConnectStatus(fromArea, toArea);
            if (door >= 0)
            {
				doorType = (TransportDoorType)door;
				return _getDoorBirthPosition(fromArea, doorType);
            }
        }

        return VInt3.zero;
    }


	public VInt3 CurrentGuideHellPosition(ref TransportDoorType doorType)
	{
		CacheArea area = _currentCacheNode();

		if (area != null && area.toZhuzi != null && area.toZhuzi.Count > 0)
		{
            int idx = -1;
            for (int i = 0; i < area.toZhuzi.Count; i++)
            {
                idx = area.toZhuzi[i];
                IDungeonConnectData _toArea = mAsset.GetAreaConnectList(idx);
                if (_toArea != null && !_toArea.IsBoss()) break;
            }
            IDungeonConnectData toArea = mAsset.GetAreaConnectList(idx);
			IDungeonConnectData fromArea = CurrentDataConnect();

			int door = mAsset.GetConnectStatus(fromArea, toArea);
			if (door >= 0)
			{
				doorType = (TransportDoorType)door;
				return _getDoorBirthPosition(fromArea, doorType);
			}
		}

		return VInt3.zero;
	}

    public VInt3 CurrentGuideDoorPosition()
    {
        CacheArea area = _currentCacheNode();

        if (area != null && area.toboss != null && area.toboss.Count > 0)
        {
            int idx = area.toboss[0];
            IDungeonConnectData toArea = mAsset.GetAreaConnectList(idx);
            IDungeonConnectData fromArea = CurrentDataConnect();

            int door = mAsset.GetConnectStatus(fromArea, toArea);
            if (door >= 0)
            {
                return _getDoorPosition(fromArea, (TransportDoorType)door);
            }
        }

        return VInt3.zero;
    }

    public VInt3 GetBirthPosition()
    {
        var node = CurrentDataConnect();
        if (lastIndex < 0 && _getDataNodeAreaID(node) == mBattleInfo.startAreaId)
        {
            return GetBirthPosition(node);
        }
        else
        {
            return GetDoorBirthPosition(node);
        }
    }

    public VInt3 GetAirBattleBornPos(int i)
    {
        var node = CurrentDataConnect();
        if (null == node)
        {
            return VInt3.zero;
        }

        ISceneData sceneData = node.GetSceneData();
        if (null == sceneData)
        {
            return VInt3.zero;
        }
        var bornPos = sceneData.GetAirBattleBornPos(i);
        if(bornPos != null)
        {
            return new VInt3(bornPos.GetPosition());

        }
        Logger.LogErrorFormat("GetAirBattleBornPos is null index {0} id {1}",i,id.dungeonID);
        return VInt3.zero;
    }

    public int CurrentIndex()
    {
        if (battleType == BattleType.ChampionMatch)
        {
            for (int i = 0; i < mCacheAreaList.Count; i++)
            {
                if (mCacheAreaList[i].area.id == mBattleInfo.areaId)
                {
                    return i;
                }
            }

            return -1;
        }
        else
        {
            return currentIndex;
        }
    }

    public int CurrentAreaID()
    {
        if (battleType == BattleType.ChampionMatch)
        {
            return mBattleInfo.areaId;
        }
        else
        {
            var node = CurrentDataConnect();
            return _getDataNodeAreaID(node);
        }
    }

    public int CurrentAreaIDWithoutDiff()
    {
        return CurrentAreaID() / 10;
    }

    private Battle.DungeonAddMonsterDropItem mSearchOp = new Battle.DungeonAddMonsterDropItem();
    private void _appendDropItem(IList<Battle.DungeonMonster> list)
    {
        var appendList = mBattleInfo.addMonsterDropItem;

        for (int i = 0; i < list.Count; ++i)
        {
            var monster = list[i];

            mSearchOp.monsterId = monster.id;

            var idx = appendList.BinarySearch(mSearchOp);
            if (idx < 0)
            {
                Logger.LogProcessFormat("can't find the monster's drop item with id {0}", monster.id);
            }
            else
            {
                var appendItem = appendList[idx];
                for (int k = 0; k < appendItem.dropItems.Count; k++)
                {
                    Logger.LogProcessFormat("add drop item with id {0}, at monster with id {1}", appendItem.dropItems[k], monster.id);
                    monster.dropItems.Add(appendItem.dropItems[k]);
                }

                appendList.Remove(appendItem);
            }
        }
    }

    private void _makeupHell()
    {
        if (IsHellArea())
        {
            var cacheNode = _currentCacheNode();
            var hellInfo = mBattleInfo.dungeonHealInfo;
            if (null != cacheNode)
            {
                cacheNode.hell = hellInfo.Duplicate();
            }
        }
    }
    
    private void _makeupMonsterDestruct()
    {
        if (battleType == BattleType.TreasureMap)
        {
            //技能实现的怪物无须创建了，因为已经放入待创建怪物列表中去了
            return;
        }
        IDungeonConnectData node = CurrentDataConnect();
        var cacheNode = _currentCacheNode();

        var remote = cacheNode.area.FirstMonsterList;

        if (null == node.GetSceneData())
        {
            return ;
        }

        ISceneData sceneData = node.GetSceneData();

        if (null == sceneData)
        {
            return;
        }

        for (int i = 0; i < sceneData.GetMonsterInfoLength(); ++i)
        {
            ISceneMonsterInfoData monsterData = sceneData.GetMonsterInfo(i);
            if (null == monsterData)
            {
                continue;
            }

            var unititem = TableManager.instance.GetTableItem<ProtoTable.UnitTable>(monsterData.GetEntityInfo().GetResid());

            if (null != unititem && unititem.Type == UnitTable.eType.SKILL_MONSTER)
            {

                Battle.DungeonMonster findItem = null;
                for (int j = 0; j < remote.Count; ++j)
                {
                    var x = remote[j];
                    if ( (x.pointId % 100) == i && (x.typeId / 10) == (monsterData.GetEntityInfo().GetResid() / 10) )
                    {
                        findItem = x;
                        break;
                    }
                }

                //var findItem = remote.Find(x =>
                //{
                //    return (x.pointId % 100) == i && (x.typeId / 10) == (monsterData.GetEntityInfo().GetResid() / 10);
                //});

                if (null == findItem)
                {
                    remote.Add(new Battle.DungeonMonster()
                    {
                        pointId = i,
                        typeId = monsterData.GetEntityInfo().GetResid() / 10 * 10 + mID.diffID + 1,
                        removed = false,
                    });
                }
            }
        }
    }

    private void _makeupDestructList()
    {
        var node = CurrentDataConnect();
        var cacheNode = _currentCacheNode();
        if (node == null || cacheNode == null || cacheNode.area == null || node.GetSceneData() == null) return;
        //var local = node.GetSceneData.GetD;
        var remote = cacheNode.area.destructs;

        int destructLen = node.GetSceneData().GetDestructibleInfoLength();
        int offset = node.GetSceneData().GetMonsterInfoLength();

        for (int i = 0; i < destructLen; ++i)
        {
            var findItem = remote.Find(x =>
            {
                return (x.pointId % 100) == (offset + i) && x.typeId == node.GetSceneData().GetDestructibleInfo(i).GetEntityInfo().GetResid();
            });

            if (null == findItem)
            {
                remote.Add(new Battle.DungeonMonster()
                {
                    id = int.MaxValue,
                    pointId = offset + i,
                    typeId = node.GetSceneData().GetDestructibleInfo(i).GetEntityInfo().GetResid(),
                    removed = false,
                });
            }
        }
    }

    /// <summary>
    /// 获得当前场景的怪物列表
    /// </summary>
    public IList<Battle.DungeonMonster> CurrentMonsters(int idx = 0)
    {
        var cacheNode = _currentCacheNode();
        if (cacheNode == null || cacheNode.area == null)
            return null;
        return cacheNode.area.GetMonsterListAt(idx);
    }

    /// <summary>
    /// 获取当前怪物分组数量
    /// </summary>
    /// <returns></returns>
    public int CurrentMonsterGroupCount()
    {
        var cacheNode = _currentCacheNode();
        if (cacheNode == null || cacheNode.area == null)
            return 0;
        return cacheNode.area.MonsterListCount;
    }

    /// <summary>
    /// 获得当前场景的可破坏物列表
    /// </summary>
    /// <returns></returns>
    public List<Battle.DungeonMonster> CurrentDestructs()
    {
        var cacheNode = _currentCacheNode();
        if (cacheNode == null || cacheNode.area == null)
            return null;
        return cacheNode.area.destructs;
    }
    List<int> sDefautlRegions = new List<int>();
    public List<int> CurrentRegions()
    {
        var cacheNode = _currentCacheNode();
        if(cacheNode == null)
        {
            Logger.LogErrorFormat("cacheNode is Invalid {0} dungeonId {1}", currentIndex,this.mID.dungeonID);
            return sDefautlRegions;
        }
        if(cacheNode.area == null)
        {
            Logger.LogErrorFormat("cacheNode.area is Invalid {0} dungeonId {1}", currentIndex, this.mID.dungeonID);
            return sDefautlRegions;
        }
        return cacheNode.area.regions;
    }

    public void AddDynamicRegion(CustomSceneRegionInfo regionInfo)
    {
        var cacheNode = _currentCacheNode();
        if (cacheNode == null)
        {
            Logger.LogErrorFormat("cacheNode is Invalid {0} dungeonId {1}", currentIndex, this.mID.dungeonID);
            return;
        }
        if (cacheNode.area == null)
        {
            Logger.LogErrorFormat("cacheNode.area is Invalid {0} dungeonId {1}", currentIndex, this.mID.dungeonID);
            return;
        }
        cacheNode.dynamicRegionInfoes.Add(regionInfo);
    }
    public List<int> CurrentDynamicRegions()
    {
        var cacheNode = _currentCacheNode();
        if (cacheNode == null)
        {
            Logger.LogErrorFormat("cacheNode is Invalid {0} dungeonId {1}", currentIndex, this.mID.dungeonID);
            return sDefautlRegions;
        }
        if (cacheNode.area == null)
        {
            Logger.LogErrorFormat("cacheNode.area is Invalid {0} dungeonId {1}", currentIndex, this.mID.dungeonID);
            return sDefautlRegions;
        }
        return cacheNode.dynamicRegions;
    }
    public List<CustomSceneRegionInfo> CurrentDynamicRegionInfoes()
    {
        var cacheNode = _currentCacheNode();
        if (cacheNode == null)
        {
            Logger.LogErrorFormat("cacheNode is Invalid {0} dungeonId {1}", currentIndex, this.mID.dungeonID);
            return null;
        }
        if (cacheNode.area == null)
        {
            Logger.LogErrorFormat("cacheNode.area is Invalid {0} dungeonId {1}", currentIndex, this.mID.dungeonID);
            return null;
        }
        return cacheNode.dynamicRegionInfoes;
    }


    List<Battle.DungeonDoor> mDoors = new List<Battle.DungeonDoor>();
    public List<Battle.DungeonDoor> CurrentDoors(bool isIgnoreConnect = false)
    {
        mDoors.Clear();

        IDungeonConnectData node = CurrentDataConnect();
        if (null == node)
        {
            return mDoors;
        }

        ISceneData sceneData = node.GetSceneData();
        if (null == sceneData)
        {
            return mDoors;
        }


        for (int i = 0; i < sceneData.GetTransportDoorLength(); i++)
        {
            ISceneTransportDoorData doorData = sceneData.GetTransportDoor(i);

            //var door = node.scenedata._transportdoor[i];
            Battle.DungeonDoor dungeonDoor = new Battle.DungeonDoor();

            if (
                #if UNITY_EDITOR
                isIgnoreConnect ||
                #endif
                node.GetIsConnect((int)doorData.GetDoortype()))
            {
                dungeonDoor.isconnectwithboss = _isNearBossRoom(node, doorData.GetDoortype());
                dungeonDoor.isvisited         = _isVisitedRoom(node, doorData.GetDoortype());
                dungeonDoor.door       		  = doorData;
                dungeonDoor.doorType   		  = doorData.GetDoortype();
                dungeonDoor.isEggDoor = doorData.IsEggDoor();
                dungeonDoor.materialPath = doorData.GetMaterialPath();
            }
            mDoors.Add(dungeonDoor);
        }

        return mDoors;
    }

    public Battle.DungeonHellInfo CurrentHellDestructs()
    {
        var cacheNode = _currentCacheNode();
        if (null != cacheNode && null != cacheNode.hell)
        {
            return cacheNode.hell;
        }
        return null;
    }

    public string CurrentScenePath()
    {
        var node = CurrentDataConnect();
        if (null != node)
        {
            return node.GetSceneAreaPath();
        }

        return "";
    }

    public ISceneData CurrentSceneData()
    {
        IDungeonConnectData node = CurrentDataConnect();

        if (null != node)
        {
            return node.GetSceneData();
        }

        return null;
    }
    #endregion

    #region 删除数据接口

    public void DeleteMonster(int id)
    { }

    public void DeleteDropItem(int id)
    { }

    public void DeleteDestruct(int id)
    { }

    #endregion

    #endregion

    #region 私有处理函数

    /// <summary>
    /// 这个房间的传送门是否和boss房间相邻
    /// </summary>
    /// <param name="node">房间节点</param>
    /// <param name="type">门的类型</param>
    /// <returns></returns>
    private bool _isNearBossRoom(IDungeonConnectData node, TransportDoorType type)
    {
        var opp = mAsset.GetSideByType(node, type);
        return null != opp && opp.IsBoss();
    }

    /// <summary>
    /// 这个房间的传送门对应的房间是否已经打过
    /// </summary>
    /// <param name="node"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool _isVisitedRoom(IDungeonConnectData node, TransportDoorType type)
    {
        var opp = mAsset.GetSideByType(node, type);
        var fItem = _getCacheArea(opp);
        return null != fItem && fItem.visited;
    }

    /// <summary>
    /// 初始化数据，并对数据做一些处理
    /// </summary>
    private void _initData()
    {
        mCacheAreaList = new List<CacheArea>();
        Battle.DungeonArea area = null;

        for (int i = 0; i < mBattleInfo.areas.Count; ++i)
        {
            area = mBattleInfo.areas[i].Duplicate();

            CacheArea cache = new CacheArea();

            cache.area = area;
            cache.visited = false;
            cache.statistics = new DungeonStatistics();
            cache.toboss = new List<int>();
			cache.toZhuzi = new List<int>();
            cache.killdMonster = new List<uint>();
            cache.pickedItems = new List<uint>();
            cache.dynamicRegions = new List<int>();
            cache.dynamicRegionInfoes = new List<CustomSceneRegionInfo>();
            mCacheAreaList.Add(cache);
        }
    }

    /// <summary>
    /// 初始化随机数据，并对数据做一些处理
    /// </summary>
    private void _initRandData(eDungeonMode mode)
    {
        //if (mBattleInfo.dungeonHealInfo != null)
        //{
        //    FrameRandom.ResetSeed(mBattleInfo.randomSeed);

        //    List<int> randList = new List<int>();

        //    for (int i = 0; i < mAsset.areaconnectlist.Length; ++i)
        //    {
        //        DSceneDataConnect con = mAsset.areaconnectlist[i];

        //        int areaId = _getDataNodeAreaID(con);
        //        if (!con.isboss && mBattleInfo.startAreaId != areaId)
        //        {
        //            randList.Add(areaId);
        //        }
        //    }

        //    //int hellAreaID = randList[FrameRandom.];
        //}
    }

    /// <summary>
    /// 初始化小地图的地图数据
    /// </summary>
    private void _initPath2BossRoom()
    {
        Queue<int> q = new Queue<int>();
        bool[] flag = new bool[mAsset.GetAreaConnectListLength()];

        /// find boss area enqueue
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData data = mAsset.GetAreaConnectList(i);
            if (data.IsBoss())
            {
                // TODO use the areaid?
                q.Enqueue(i);
                break;
            }
        }

        while (q.Count > 0)
        {
            int idx = q.Dequeue();
            IDungeonConnectData data = mAsset.GetAreaConnectList(idx);
            for (int i = 0; i < data.GetIsConnectLength(); ++i)
            {
                if (data.GetIsConnect(i))
                {
                    IDungeonConnectData fData = mAsset.GetSideByType(data, (TransportDoorType)i);

                    if (!flag[fData.GetAreaIndex()])
                    {
                        q.Enqueue(fData.GetAreaIndex());
                        CacheArea fItem = _getCacheArea(fData);
                        if (null != fItem)
                        {
                            // TODO use the areaid?
                            fItem.toboss.Add(data.GetAreaIndex());
                        }
                    }
                }
            }
            flag[idx] = true;
        }


		if (mBattleInfo.dungeonHealInfo.areaId <= 0)
			return;

		q.Clear();
		bool hasZhuzi = false;
		for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
		{
			IDungeonConnectData data = mAsset.GetAreaConnectList(i);
			flag[i] = false;
            // marked by ckm
            CacheArea tmpItem = _getCacheArea(data);
            if(tmpItem != null)
            {
                if (tmpItem.area.id == mBattleInfo.dungeonHealInfo.areaId)
                {
                    hasZhuzi = true;
                    q.Enqueue(i);
                }
            }
		}

		if (hasZhuzi)
		{
			while (q.Count > 0)
			{
				int idx = q.Dequeue();
				IDungeonConnectData data = mAsset.GetAreaConnectList(idx);
				for (int i = 0; i < data.GetIsConnectLength(); ++i)
				{
					if (data.GetIsConnect(i))
					{
						IDungeonConnectData fData = mAsset.GetSideByType(data, (TransportDoorType)i);

						if (!flag[fData.GetAreaIndex()])
						{
							q.Enqueue(fData.GetAreaIndex());
							CacheArea fItem = _getCacheArea(fData);
							if (null != fItem)
							{
								// TODO use the areaid?
								fItem.toZhuzi.Add(data.GetAreaIndex());
							}
						}
					}
				}
				flag[idx] = true;
			}
		}


    }

    /// <summary>
    /// 根据区域ID获取房间所在的序号ID
    /// </summary>
    /// <param name="areaId"></param>
    /// <returns></returns>
    public int GetIndexByAreaId()
    {
        return _changeIndexByAreaID(CurrentAreaID());
    }

    /// <summary> 
    /// 切换数据到对应AreaID的区域中
    /// </summary> 
    private int _changeIndexByAreaID(int areaId)
    {
        IDungeonConnectData node = null;
        //Pvp模式直接取第一个房间数据
        if (mAsset.GetAreaConnectListLength() == 1)
        {
            node = mAsset.GetAreaConnectList(0);
        }
        else
        {
            for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
            {
                IDungeonConnectData findNode = mAsset.GetAreaConnectList(i);

                if (_getDataNodeAreaID(findNode) == areaId)
                {
                    node = findNode;
                    break;
                }

            }
        }

        if (null != node)
        {
            currentIndex = node.GetAreaIndex();
            mBattleInfo.areaId = areaId;

            _makeupAreaDatas();

            _doStatEnterRoom(areaId);

            return currentIndex;
        }
        else
        {
            //Logger.LogErrorFormat("场景数据找不到AreaID {0}", areaId);
            return -1;
        }
    }

    private void _doStatEnterRoom(int areaId)
    {
#if !LOGIC_SERVER
        try
        {
            bool needSend = true;
            BaseBattle battle= null;
            if (BattleMain.instance != null && BattleMain.instance.GetBattle() != null)
            {
                battle = BattleMain.instance.GetBattle() as BaseBattle;
                if (battle != null)
                    needSend = battle.NeedSendMsg;
            }

            //GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.PASS_DOOR, id.dungeonID, mBattleInfo.areaId, "");
            if (needSend)
                GameStatisticManager.instance.DoStatEnterPVERoom(id.dungeonID, areaId);
        }
        catch (Exception e)
        {
            Logger.LogErrorFormat("[战斗数据] 发送统计数据出错 {0}", e.ToString());
        }
#endif
    }

    /// <summary>
    /// 根据index切换数据
    /// 此函数比较危险，切记不要谁便使用
    /// </summary>
    private int _changeIndex(int index)
    {
        if (index >= 0 && index < mAsset.GetAreaConnectListLength())
        {
            currentIndex = index;

            mBattleInfo.areaId = _getDataNodeAreaID(mAsset.GetAreaConnectList(index));

            _makeupAreaDatas();

            _doStatEnterRoom(mBattleInfo.areaId);

            return currentIndex;
        }
        else
        {
            Logger.LogErrorFormat("index {0} is out of range.", index);
            return -1;
        }
    }

    /// <summary> 
    /// 计算出所有的
    /// 可破坏物列表
    /// 怪物列表，
    /// 掉落
    /// 结算的掉落
    /// </summary> 
    private void _makeupAreaDatas()
    {
        var cnode = _currentCacheNode();
        if (null != cnode && !cnode.visited)
        {
            cnode.visited = true;

            // 先分组
            if (null != CurrentDataConnect())
            {
                _makeupMonsterSubGroup(cnode.area, CurrentDataConnect().GetSceneData());
            }

            for (int i = 0; i < cnode.area.MonsterListCount; ++i)
            {
                // make monster list, append monster drop item list
                _appendDropItem(cnode.area.GetMonsterListAt(i));
            }

            // make destruct list, append destruct drop monster list, append drop item list
            _appendDropItem(cnode.area.destructs);
                                                     
            // make resulte drop item list

            // 组合可破坏物列表
            _makeupDestructList();

            _makeupMonsterDestruct();

            _makeupHell();
        }

        //_makeupPath(cnode);
    }

    private void _makeupMonsterSubGroup(Battle.DungeonArea area, ISceneData sceneData)
    {
        if (null == sceneData || null == area)
        {
            return;
        }

        int miniGroupID = _GetAreaMonsterMinialSubGroupID(sceneData);

        int cnt = area.MonsterCount;
        for (int i = 0; i < cnt; ++i)
        {
            var curRemoteMonster = area.GetMonsterAt(i);
            if (null == curRemoteMonster)
            {
                continue;
            }

            var localData = _FindAreaMonsterByPointID(sceneData, curRemoteMonster.pointId);
            if (null != localData)
            {
                area.AddSubGroupMonster(localData.GetSubGroupID(), curRemoteMonster);
            }
            else
            {
                area.AddSubGroupMonster(miniGroupID, curRemoteMonster);
            }
        }

        area.SortMonsterSubGroup();
    }

    /// <summary>
    /// 获得最小的分组ID
    /// </summary>
    /// <param name="sceneData"></param>
    /// <returns></returns>
    private int _GetAreaMonsterMinialSubGroupID(ISceneData sceneData)
    {
        int minGroupID = int.MaxValue;
        if (null == sceneData)
        {
            return minGroupID;
        }

        int cnt = sceneData.GetMonsterInfoLength();

        for (int i = 0; i < cnt; ++i)
        {
            var data = sceneData.GetMonsterInfo(i);

            if (data.GetSubGroupID() < minGroupID)
            {
                minGroupID = data.GetSubGroupID();
            }
        }

        return minGroupID;
    }

    private ISceneMonsterInfoData _FindAreaMonsterByPointID(ISceneData sceneData, int pointID)
    {
        if (null == sceneData)
        {
            return null;
        }

        int cnt = sceneData.GetMonsterInfoLength();

        ISceneMonsterInfoData localData = null;

        for (int i = 0; i < cnt; ++i)
        {
            if (i == (pointID % GlobalLogic.VALUE_100))
            {
                localData = sceneData.GetMonsterInfo(i);
                break;
            }
        }

        return localData;
    }

    /// <summary>
    /// 获得一个从当前位置到boss房间的路径
    /// </summary>
    private int[] _makeupPath(CacheArea from)
    {
        CacheArea iterArea = from;

        List<int> path = new List<int>();

        if (iterArea == null)
        {
            return null;
        }

        while (iterArea.toboss != null && iterArea.toboss.Count > 0)
        {
            path.Add(mCacheAreaList.IndexOf(iterArea));
            iterArea = mCacheAreaList[iterArea.toboss[0]];
        }

        //string ans = "";
        //if (path.Count > 0)
        //{
        //    ans = path[0].ToString();
        //    for (int i = 1; i < path.Count; ++i)
        //    {
        //        ans += string.Format("->{0}", path[i]);
        //    }
        //}
        //Logger.LogError("final path = " + ans);

        return path.ToArray();
    }

    private CacheArea _getCacheArea(IDungeonConnectData node)
    {
        int areaId = _getDataNodeAreaID(node);
        return _getCacheAreaByAreaID(areaId);
    }

    private CacheArea _getCacheAreaByAreaID(int areaId)
    {
        for (int i = 0; i < mCacheAreaList.Count; ++i)
        {
            if (mCacheAreaList[i].area.id == areaId)
            {
                return mCacheAreaList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 获得本地数据的在现在关卡的AreaID
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private int _getDataNodeAreaID(IDungeonConnectData node)
    {
        if (null != node)
        {
            // 这里的AreaID难度从1开始
            return node.GetId() * 10 + mID.diffID + 1;
        }

        return -1;
    }

    public int FindDataConnectIDByAreaID(int areaID)
    {
        for (int i = 0; i < mAsset.GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData con = mAsset.GetAreaConnectList(i);

            if (null != con && _getDataNodeAreaID(con) == areaID)
            {
                return con.GetId();
            }
        }

        return -1;
    }

    /// <summary>
    /// 获取当前DSceneData节点
    /// </summary>
    /// <returns></returns>
    public IDungeonConnectData CurrentDataConnect()
    {
        if (battleType == BattleType.ChampionMatch)
        {
            return _currentDataConnectBaseOnServerData();
        }
        else
        {
            return _currentDataConnectBaseOnAssetData();
        }
    }

    private IDungeonConnectData _currentDataConnectBaseOnServerData()
    {
        CacheArea cacheArea = _getCacheAreaByAreaID(mBattleInfo.areaId);
        if (null == cacheArea)
        {
            Logger.LogErrorFormat("_currentDataConnectBaseOnServerData areaid {0} can't be found", mBattleInfo.areaId);
            return null;
        }

        int areaId = _getAreaIdWithNoDiff(cacheArea.area.id);

        IDungeonConnectData conData = _currentDataConnectBaseOnAssetData();
        if (conData == null) return null;
        if (conData.GetId() == areaId)
        {
            return conData;
        }

        for (int i = 0; i < conData.GetLinkAreaIndexesLength(); i++)
        {
            int conLinkIdx = conData.GetLinkAreaIndex(i);

            IDungeonConnectData linkConData = mAsset.GetAreaConnectList(conLinkIdx);
            if (null != linkConData && linkConData.GetId() == areaId)
            {
                return linkConData;
            }
        }

        return null;
    }

    private int _getAreaIdWithNoDiff(int areaId)
    {
        return areaId / 10;
    }

    private IDungeonConnectData _currentDataConnectBaseOnAssetData()
    {
        if (currentIndex < 0 || currentIndex >= mAsset.GetAreaConnectListLength())
        {
            Logger.LogErrorFormat("_currentDataConnectBaseOnAssetData index {0} can't be found {1}", currentIndex, mAsset.GetAreaConnectListLength());
            return null;
        }

        return mAsset.GetAreaConnectList(currentIndex);
    }

    private List<DungeonStatistics> mAllDungeonStatistics = new List<DungeonStatistics>();

    public List<DungeonStatistics> AllDungeonStatistics()
    {
        mAllDungeonStatistics.Clear();

        for (int i = 0; i < mCacheAreaList.Count; ++i)
        {
            mAllDungeonStatistics.Add(mCacheAreaList[i].statistics);
        }

        return mAllDungeonStatistics;
    }

    public DungeonStatistics CurrentDungeonStatistics()
    {
        var cacheNode = _currentCacheNode();
        if (null == cacheNode)
        {
            return null;
        }

        return cacheNode.statistics;
    }

    public int CurrentFightTime(bool withClear)
    {
        var node = CurrentDungeonStatistics();
        if (node == null)
        {
            return 0;
        }
        int sum = node.areaFightTime;
        if (withClear) sum += node.areaClearTime;
        return sum;
    }

    public int AllFightTime(bool withClear)
    {
        int sum = 0;

        for (int i = 0; i < mCacheAreaList.Count; ++i)
        {
            sum += mCacheAreaList[i].statistics.areaFightTime;
            if (withClear)
            {
                sum += mCacheAreaList[i].statistics.areaClearTime;
            }
        }

        return sum;
    }

    private List<DungeonStatistics> mStatistics = new List<DungeonStatistics>();

    public int LastFightTimeWithCount(bool withClear, int count)
    {
        mStatistics.Clear();

        for (int i = 0; i < mCacheAreaList.Count; ++i)
        {
            if (mCacheAreaList[i].statistics.lastVisitFrame != uint.MaxValue)
            {
                mStatistics.Add(mCacheAreaList[i].statistics);
            }
        }

        mStatistics.Sort((a, b)=>{
            if (b.lastVisitFrame > a.lastVisitFrame) { return 1; }
            else if (b.lastVisitFrame == a.lastVisitFrame) { return 0; }
            else { return -1; }
        });

        int sum = 0;

        for (int i = 0; i < mStatistics.Count && i < count; i++)
        {
            sum += mStatistics[i].areaFightTime;

            if(withClear) { sum += mStatistics[i].areaClearTime; }
        }

        return sum;
    }


    /// <summary>
    /// 获取当前CacheNode节点
    /// </summary>
    /// <returns></returns>
    private CacheArea _currentCacheNode()
    {
        IDungeonConnectData node = CurrentDataConnect();
        return _getCacheArea(node);
    }
    #endregion

    #region 网络请求数据

    #region Common

    private void _bindNetMessage()
    {
 #if !SERVER_LOGIC 

       NetProcess.AddMsgHandler(SceneDungeonRewardRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.AddMsgHandler(SceneDungeonKillMonsterRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.AddMsgHandler(SceneDungeonEnterNextAreaRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.AddMsgHandler(SceneDungeonEndDropRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.AddMsgHandler(WorldDungeonEnterRaceRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.AddMsgHandler(WorldDungeonReportFrameRes.MsgID,   _dungeonDataMsgBinder);
 #endif

    }

    private void _unbindNetMessage()
    {
#if !SERVER_LOGIC 

     NetProcess.RemoveMsgHandler(SceneDungeonRewardRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.RemoveMsgHandler(SceneDungeonKillMonsterRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.RemoveMsgHandler(SceneDungeonEnterNextAreaRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.RemoveMsgHandler(SceneDungeonEndDropRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.RemoveMsgHandler(WorldDungeonEnterRaceRes.MsgID, _dungeonDataMsgBinder);
        NetProcess.RemoveMsgHandler(WorldDungeonReportFrameRes.MsgID,   _dungeonDataMsgBinder);
 #endif

    }

    private void _dungeonDataMsgBinder(MsgDATA data)
    {
        if (SceneDungeonRewardRes.MsgID == data.id)
        {
            SceneDungeonRewardRes res = new SceneDungeonRewardRes();
            res.decode(data.bytes);
            _onSceneDungeonRewardRes(res);
        }
        else if (SceneDungeonKillMonsterRes.MsgID == data.id)
        {
            SceneDungeonKillMonsterRes res = new SceneDungeonKillMonsterRes();
            res.decode(data.bytes);
            _onSceneDungeonKillMonsterRes(res);

        }
        else if (SceneDungeonEnterNextAreaRes.MsgID == data.id)
        {
            SceneDungeonEnterNextAreaRes res = new SceneDungeonEnterNextAreaRes();
            res.decode(data.bytes);
            _onSceneDungeonEnterNextAreaRes(res);
        }
        else if (SceneDungeonEndDropRes.MsgID == data.id)
        {
            SceneDungeonEndDropRes res = new SceneDungeonEndDropRes();
            res.decode(data.bytes);
            _onSceneDungeonEndDropRes(res);
        }
        else if (SceneDungeonRaceEndRes.MsgID == data.id)
        {
            SceneDungeonRaceEndRes res = new SceneDungeonRaceEndRes();
            res.decode(data.bytes);
            _onSceneDungeonRaceEndRes(res);
        }
        else if (WorldDungeonEnterRaceRes.MsgID == data.id)
        {
            WorldDungeonEnterRaceRes res = new WorldDungeonEnterRaceRes();
            res.decode(data.bytes);
            _onWorldDungeonEnterRaceRes(res);
        }
        else if (WorldDungeonReportFrameRes.MsgID == data.id)
        {
            WorldDungeonReportFrameRes res = new WorldDungeonReportFrameRes();
            res.decode(data.bytes);
            _onWorldDungeonReportFrameRes(res);
        }
        else
        {
            Logger.LogErrorFormat("[战斗数据] 没处理的消息 {0}", data.id);
        }
    }

    /// <summary>
    /// 往列表中添加一个元素
    ///
    /// <return>
    /// 如果元素存在返回false, 不添加
    /// 如果元素不存在返回true, 添加成功
    /// </return>
    /// </summary>
    private bool _addUniqueItem<T>(List<T> list, T item)
    {
        if (null != list)
        {
            int idx = list.BinarySearch(item);

            if (idx < 0)
            {
                list.Insert(~idx, item);

                return true;
            }
        }

        return false;
    }

    private bool _isAllSendVertify(List<uint> sendList, List<uint> vertifyList)
    {
        sendList.Sort();
        vertifyList.Sort();

        int j = 0;
        int i = 0;

        while (i < sendList.Count && j < vertifyList.Count)
        {
            if (sendList[i] == vertifyList[j])
            {
                i++;
                j++;
            }
            else
            {
                i++;
                break;
            }
        }

        return i == j;
    }

    /// <summary>
    /// 这里需要保证vertifyList中含有的元素sendList必有
    /// </summary>
    private uint[] _getAllNoVertifyItems(List<uint> sendList, List<uint> vertifyList)
    {
        List<uint> diff = new List<uint>();

        sendList.Sort();
        vertifyList.Sort();

        int i = 0;
        int j = 0;

        while (i < sendList.Count && j < vertifyList.Count)
        {
            if (sendList[i] == vertifyList[j])
            {
                i++;
                j++;
            }
            else
            {
                diff.Add(sendList[i]);
                i++;
            }
        }

        while (i < sendList.Count) 
        {
            diff.Add(sendList[i]);
            i++;
        }

        return diff.ToArray();
    }

    private bool _sendNetMsg2Gate<T>(T req, string tag) where T : Protocol.IProtocolStream, Protocol.IGetMsgID
    {
        if (null != req)
        {
            ResendDungeonEnterRace();

            if (SceneDungeonKillMonsterReq.MsgID == req.GetMsgID())
            {
                Logger.LogProcessFormat("[战斗数据] {0} 杀死怪物 {1}", tag, ObjectDumper.Dump(req));
            }
            else if (SceneDungeonEnterNextAreaReq.MsgID == req.GetMsgID())
            {
                Logger.LogProcessFormat("[战斗数据] {0} 请求进入区域 {1}", tag, ObjectDumper.Dump(req));
            }
            else if (SceneDungeonRewardReq.MsgID == req.GetMsgID())
            {
                Logger.LogProcessFormat("[战斗数据] {0} 发送已经捡取的掉落 {1}", tag, ObjectDumper.Dump(req));

                SceneDungeonRewardReq msg = req as SceneDungeonRewardReq;

                if (null != msg)
                {
                    for (int i = 0; i < msg.dropItemIds.Length; ++i)
                    {
                        _printDropInfo("发送捡", msg.dropItemIds[i], false);
                    }
                }
            }
            else if (SceneDungeonEndDropReq.MsgID == req.GetMsgID())
            {
                Logger.LogProcessFormat("[战斗数据] {0} 请求结算掉落 {1}", tag, ObjectDumper.Dump(req));
            }
            else if (SceneDungeonEnterNextAreaReq.MsgID == req.GetMsgID())
            {
                SceneDungeonEnterNextAreaReq msg = req as SceneDungeonEnterNextAreaReq;

                if (null != msg)
                {
                    Logger.LogProcessFormat("[战斗数据] {0} 尝试进入区域 {1}", tag, msg.areaId);
                }
            }
            else if (WorldDungeonReportFrameReq.MsgID == req.GetMsgID())
            {
                Logger.LogProcessFormat("[战斗数据] {0} 发送验证数据 {1}", tag, ObjectDumper.Dump(req));

                WorldDungeonReportFrameReq msg = req as WorldDungeonReportFrameReq;

                if (null != msg && null != msg.checksums && null != msg.frames)
                {
                    Logger.LogProcessFormat("[战斗数据] {0} 发送验证数据 操作帧数 {1}, 随机数数目 {2}", tag, msg.frames.Length, msg.checksums.Length);
                }
            }
            else
            {
                Logger.LogProcessFormat("[战斗数据] {0} 发送数据 {1}", tag, ObjectDumper.Dump(req));
            }

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
            return true;
        }

        return false;
    }
#endregion

    #region 杀死怪物上报

    protected List<uint> mAllKillMonster        = new List<uint>();
    protected List<uint> mAllKillMonsterVertify = new List<uint>();

    /// <summary>
    /// 发送已经杀死的怪物
    /// </summary>
    public void SendKillMonsters()
    {
        SceneDungeonKillMonsterReq req = _getDungenKillMonsterReq();
        _sendNetMsg2Gate<SceneDungeonKillMonsterReq>(req, "KillMonster");
    }

    /// <summary>
    /// 是否有未验证的杀死的怪物
    /// </summary>
    public bool HasNoVertifyKilledMonsters()
    {
        return !_isAllSendVertify(mAllKillMonster, mAllKillMonsterVertify);
    }

    /// <summary>
    /// 发送未验证的杀死的怪物
    /// </summary>
    public void SendNoVertifyKilledMonsters()
    {
        if (HasNoVertifyKilledMonsters())
        {
            _sendNetMsg2Gate<SceneDungeonKillMonsterReq>(_getDungenNoVertifyKillMonsterReq(), "NoVertify");
        }
    }

    private SceneDungeonKillMonsterReq _getDungenNoVertifyKillMonsterReq()
    {
        uint[] monsters = _getAllNoVertifyItems(mAllKillMonster, mAllKillMonsterVertify);

        if (null != monsters && monsters.Length > 0)
        {
            SceneDungeonKillMonsterReq req = new SceneDungeonKillMonsterReq();
            req.monsterIds = monsters;
            return req;
        }

        return null;
    }

    private SceneDungeonKillMonsterReq _getDungenKillMonsterReq()
    {
        List<int> killedList = mBattleInfo.killedMonsters;


        List<uint> list = new List<uint>();

        for (int i = 0; i < killedList.Count; ++i)
        {
            _addUniqueItem<uint>(mAllKillMonster, (uint)killedList[i]);

            if (!_addUniqueItem<uint>(list, (uint)killedList[i]))
            {
                Logger.LogErrorFormat("[战斗数据] 已经杀死 {0}", killedList[i]);
            }
        }

        killedList.Clear();

        if (list.Count > 0)
        {
            SceneDungeonKillMonsterReq req = new SceneDungeonKillMonsterReq();
            req.monsterIds = list.ToArray();
            return req;
        }
        else
        {
            return null;
        }
    }

    private void _onSceneDungeonKillMonsterRes(SceneDungeonKillMonsterRes res)
    {
        // 处理怪物杀死之后的数据处理
        if (null != res && null != res.monsterIds)
        {
            Logger.LogProcessFormat("[战斗数据] 杀死怪返回 {0}", ObjectDumper.Dump(res));

            for (int i = 0; i < res.monsterIds.Length; ++i)
            {
                _addUniqueItem<uint>(mAllKillMonsterVertify, res.monsterIds[i]);
            }
        }
    }
    #endregion

    #region 结算

    #endregion

    #region 切换区域
    protected Queue<uint> mAreaEnterIdSendQueue   = new Queue<uint>();
    protected List<uint> mAreaEnterIdRecivedStack = new List<uint>();

    protected class CacheEnterNextArea
    {
        public uint areaID;
        public uint lastFrame;
    }

    protected List<CacheEnterNextArea> mCacheEnterNextArea = new List<CacheEnterNextArea>();

    public void SendSceneDungeonAreaChange(int nextAreaID)
    {
        _pushAreaEnterIdSceneQueue((uint)nextAreaID);
        _processAreaEnterIdSendQueue();
    }

    protected void _pushAreaEnterIdSceneQueue(uint areaID)
    {
        if (battleType == BattleType.TreasureMap) return;
        mAreaEnterIdSendQueue.Enqueue(areaID);
    }

    protected void _processAreaEnterIdSendQueue(bool needReport = true)
    {
        if (battleType == BattleType.TreasureMap) return;
        //  if (ClientApplication.isOpenNewReconnectAlgo)
        {
            uint areaID = _popAreaEnterIdSendQueue();
            uint recivedId = _getFrontRecivedAreaEnterId();
            _popRecivedAreaEnterId();
            if (areaID > 0)
            {
                if(needReport)
                    _processSendWorldDungeonReportFrame();

                SceneDungeonEnterNextAreaReq msg = new SceneDungeonEnterNextAreaReq()
                {
                    areaId = areaID,
                    lastFrame = _getFrameIndexByEnterNextAreaAreaId(areaID)
                };
                msg.staticVal.values = _getDungeonEnterNextAreaKeyValueArray();
                if (null != msg)
                {
                    Logger.LogProcessFormat("[战斗数据] Area 尝试进入区域 {0}", msg.areaId);
                }
                NetManager.instance.SendCommand(ServerType.GATE_SERVER, msg);
            }
        }
        //else
        //{
        //    uint areaID = _getFrontAreaEnterIdSendQueue();
        //    uint recivedId = _getFrontRecivedAreaEnterId();

        //    while (areaID > 0 && areaID == recivedId)
        //    {
        //        _popAreaEnterIdSendQueue();
        //        _popRecivedAreaEnterId();

        //        Logger.LogProcessFormat("[战斗数据] 进入区域验证了 {0}", areaID);

        //        areaID = _getFrontAreaEnterIdSendQueue();
        //        recivedId = _getFrontRecivedAreaEnterId();
        //    }

        //    if (areaID > 0)
        //    {
        //        Logger.LogProcessFormat("[战斗数据] 重发进入场景消息 {0}", areaID);

        //        _processSendWorldDungeonReportFrame();

        //        SceneDungeonEnterNextAreaReq msg = new SceneDungeonEnterNextAreaReq();

        //        msg.areaId = areaID;
        //        msg.staticVal.values = _getDungeonEnterNextAreaKeyValueArray();
        //        msg.lastFrame = _getFrameIndexByEnterNextAreaAreaId(areaID);

        //        _sendNetMsg2Gate<SceneDungeonEnterNextAreaReq>(msg, "Area");
        //    }
        //}
    }

    private uint _getFrameIndexByEnterNextAreaAreaId(uint areaID)
    {
        for (int i = 0; i < mCacheEnterNextArea.Count; ++i)
        {
            if (areaID == mCacheEnterNextArea[i].areaID)
            {
                return mCacheEnterNextArea[i].lastFrame;
            }
        }

        CacheEnterNextArea nextArea = new CacheEnterNextArea();
        nextArea.areaID             = areaID;
        nextArea.lastFrame          = mLastFrameIndexClientSended;

        mCacheEnterNextArea.Add(nextArea);

        Logger.LogProcessFormat("[战斗数据] 找不到 AreaID {0} 对应的 FrameIndex, 新建一个 {1}", nextArea.areaID, nextArea.lastFrame);

        return nextArea.lastFrame;
    }

    private int[] _getDungeonEnterNextAreaKeyValueArray()
    {
        List<int> array = new List<int>();

        array.Add(GlobalLogic.VALUE_1    );
        array.Add(GlobalLogic.VALUE_2    );
        array.Add(GlobalLogic.VALUE_5    );
        array.Add(GlobalLogic.VALUE_10   );
        array.Add(GlobalLogic.VALUE_50   );
        array.Add(GlobalLogic.VALUE_60   );
        array.Add(GlobalLogic.VALUE_100  );
        array.Add(GlobalLogic.VALUE_150  );
        array.Add(GlobalLogic.VALUE_200  );
        array.Add(GlobalLogic.VALUE_250  );
        array.Add(GlobalLogic.VALUE_500  );
        array.Add(GlobalLogic.VALUE_700  );
        array.Add(GlobalLogic.VALUE_1000 );
        array.Add(GlobalLogic.VALUE_1500 );
        array.Add(GlobalLogic.VALUE_2000 );
        array.Add(GlobalLogic.VALUE_3600 );
        array.Add(GlobalLogic.VALUE_5000 );
        array.Add(GlobalLogic.VALUE_10000);


		array.Add(GlobalLogic.VALUE_100000);    
		array.Add(GlobalLogic.VALUE_99999);
		array.Add(GlobalLogic.VALUE_20000);
		array.Add(GlobalLogic.VALUE_4000); 
		array.Add(GlobalLogic.VALUE_3000); 
		array.Add(GlobalLogic.VALUE_400);  
		array.Add(GlobalLogic.VALUE_360);  
		array.Add(GlobalLogic.VALUE_300);  
		array.Add(GlobalLogic.VALUE_180);  
		array.Add(GlobalLogic.VALUE_30);   
		array.Add(GlobalLogic.VALUE_3);    

        return array.ToArray();
    }

    protected uint _getFrontAreaEnterIdSendQueue()
    {
        if (mAreaEnterIdSendQueue.Count > 0)
        {
            return mAreaEnterIdSendQueue.Peek();
        }

        return 0;
    }

    protected uint _getFrontRecivedAreaEnterId()
    {
        if (mAreaEnterIdRecivedStack.Count > 0)
        {
            return mAreaEnterIdRecivedStack[0];
        }

        return 0;
    }

    protected uint _popAreaEnterIdSendQueue()
    {
        if (mAreaEnterIdSendQueue.Count > 0)
        {
            return mAreaEnterIdSendQueue.Dequeue();
        }

        return 0;
    }

    protected void _popRecivedAreaEnterId()
    {
        if (mAreaEnterIdRecivedStack.Count > 0)
        {
            mAreaEnterIdRecivedStack.RemoveAt(0);
        }
    }

    public SceneDungeonEnterNextAreaReq _getDungeonEnterNextAreaReq()
    {
        uint      areaID    = (uint)CurrentAreaID();
        CacheArea cacheNode = _currentCacheNode();

        if (areaID == cacheNode.area.id)
        {
            var msg              = new SceneDungeonEnterNextAreaReq();

            msg.areaId           = areaID;
            msg.staticVal.values = _getDungeonEnterNextAreaKeyValueArray();
            msg.lastFrame        = _getFrameIndexByEnterNextAreaAreaId(areaID);

            return msg;
        }
        else
        {
            Logger.LogErrorFormat("[战斗数据] 请求进入区域错误  服务端ID {0} 本地ID {1}", cacheNode.area.id, areaID);
        }

        return null;
    }

    private void _onSceneDungeonEnterNextAreaRes(SceneDungeonEnterNextAreaRes res)
    {
        if (res.result == 0)
        {
            Logger.LogProcessFormat("[战斗数据] 成功进入区域 {0}", res.areaId);
            _pushRecivedAreaEnterId(res.areaId);
        }
        else
        {
            Logger.LogErrorFormat("[战斗数据] 无法进入区域 {0}", res.result);
        }
    }

    protected void _pushRecivedAreaEnterId(uint areaID)
    {
        uint lastAreaID = _getLastedRecivedAreaEnterId();
        if (lastAreaID != areaID)
        {
            mAreaEnterIdRecivedStack.Add(areaID);
        }
    }

    protected uint _getLastedRecivedAreaEnterId()
    {
        if (mAreaEnterIdRecivedStack.Count > 0)
        {
            return mAreaEnterIdRecivedStack[mAreaEnterIdRecivedStack.Count - 1];
        }

        return 0;
    }
    #endregion

    #region Update
    protected const int kTickTimeGap = 5000;

    protected int mTickTime = 0;
    public void Update(int delta)
    {
        mTickTime += delta;
        if (mTickTime > kTickTimeGap)
        {
            mTickTime -= kTickTimeGap;
            _processSendWorldDungeonReportFrame();
            _processAreaEnterIdSendQueue(false);
        }
    }
    #endregion


    #region 掉落

    protected List<uint> mAllPickedDrops        = new List<uint>();
    protected List<uint> mAllPickedVertifyDrops = new List<uint>();

    /// <summary>
    /// 发送已经捡起来的掉落物品
    /// </summary>
    public void SendPickedDrops()
    {
        SceneDungeonRewardReq req = _getDungeonRewardReqData();
        _sendNetMsg2Gate<SceneDungeonRewardReq>(req, "Picked");
    }

    /// <summary>
    /// 发送BOSS掉落
    /// </summary>
    public void SendBossDrops()
    {
        SceneDungeonRewardReq req = _getDungeonBossRewardReqData();
        _sendNetMsg2Gate<SceneDungeonRewardReq>(req, "BOSS");
    }

    /// <summary>
    /// 重新发送未捡起的掉落
    /// </summary>
    public void SendNoVertifyDrops()
    {
        if (HasNoVertifyDrops())
        {
            SceneDungeonRewardReq req = _getDungeonNoVertifyRewardReqData();
            _sendNetMsg2Gate<SceneDungeonRewardReq>(req, "NoVertify");
        }
    }

    /// <summary>
    /// 是否还有未验证的掉落
    /// </summary>
    public bool HasNoVertifyDrops()
    {
        return !_isAllSendVertify(mAllPickedDrops, mAllPickedVertifyDrops);
    }

    /// <summary>
    /// 获取未验证的掉落物品请求
    /// </summary>
    private SceneDungeonRewardReq _getDungeonNoVertifyRewardReqData()
    {
        SceneDungeonRewardReq req = new SceneDungeonRewardReq();
        req.dropItemIds           = _getAllNoVertifyItems(mAllPickedDrops, mAllPickedVertifyDrops);
        if (SwitchFunctionUtility.IsOpen(55))
        {
            if (!mBattleEndLock)
            {
                req.lastFrame = mLastFrameIndexClientSended;
            }
            else
            {
                req.lastFrame = mFinalFrameIndex;
            }
        }
        else
        {
            req.lastFrame = mLastFrameIndexClientSended;
        }
        req.md5                   = _getRewardRequestMd5Array(req.dropItemIds, req.lastFrame);
        return req;
    }

    /// <summary>
    /// 获取BOSS掉落请求
    /// </summary>
    private SceneDungeonRewardReq _getDungeonBossRewardReqData()
    {
        SceneDungeonRewardReq req = new SceneDungeonRewardReq();

        List<uint> dropItems      = new List<uint>();

        for (int i = 0; i < mBattleInfo.bossDropItems.Count; ++i)
        {
            uint id = (uint)mBattleInfo.bossDropItems[i].id;

            dropItems.Add(id);

            _addUniqueItem<uint>(mAllPickedDrops, id);
        }

        if (dropItems.Count > 0)
        {
            req.dropItemIds = dropItems.ToArray();
            if (SwitchFunctionUtility.IsOpen(55))
            {
                if (!mBattleEndLock)
                {
                    req.lastFrame = mLastFrameIndexClientSended;
                }
                else
                {
                    req.lastFrame = mFinalFrameIndex;
                }
            }
            else
            {
                req.lastFrame = mLastFrameIndexClientSended;
            }
            req.md5         = _getRewardRequestMd5Array(req.dropItemIds, req.lastFrame);
            return req;
        }

        return null;
    }

    /// <summary>
    /// 获取捡起的掉落请求
    /// </summary>
    private SceneDungeonRewardReq _getDungeonRewardReqData()
    {
        List<int> dropList    = mBattleInfo.dropItemIds;

        if (dropList.Count > 0)
        {
            SceneDungeonRewardReq req = new SceneDungeonRewardReq();

            req.dropItemIds = new uint[dropList.Count];

            for (int i = 0; i < dropList.Count; ++i)
            {
                uint id = (uint)(dropList[i]);

                req.dropItemIds[i] = id;

                if (!_addUniqueItem<uint>(mAllPickedDrops, id))
                {
                    Logger.LogProcessFormat("[战斗数据] 已经捡取了掉落物品 {0}", id);
                   // _printDropInfo("已经捡取", id, false);
                }
            }
            if (SwitchFunctionUtility.IsOpen(55))
            {
                if (!mBattleEndLock)
                {
                    req.lastFrame = mLastFrameIndexClientSended;
                }
                else
                {
                    req.lastFrame = mFinalFrameIndex;
                }
            }
            else
            {
                req.lastFrame = mLastFrameIndexClientSended;
            }
            req.md5       = _getRewardRequestMd5Array(req.dropItemIds, req.lastFrame);

            dropList.Clear();

            return req;
        }

        return null;
    }

    private System.Text.StringBuilder mRequestRewardBuilder = new System.Text.StringBuilder();

    private byte[] _getRewardRequestMd5Array(uint[] dropItemIds, uint lastFrame)
    {
        mRequestRewardBuilder.Clear();

        mRequestRewardBuilder.Append(lastFrame);

        for (int i = 0; i < dropItemIds.Length; ++i)
        {
            mRequestRewardBuilder.Append(dropItemIds[i]);
        }

        return DungeonUtility.GetBattleEncryptMD5(mRequestRewardBuilder.ToString());
    }

    /// <summary>
    /// 收到回来的奖励消息
    /// <summary>
    private void _convertDungeonRewardResData(SceneDungeonRewardRes res)
    {
        uint[]    resPickList = res.pickedItems;

        for (int i = 0; i < resPickList.Length; ++i)
        {
            uint pickItem = resPickList[i];

            if (_addUniqueItem<uint>(mAllPickedVertifyDrops, pickItem))
            {
                Logger.LogProcessFormat("[战斗数据] 捡起掉落 {0}", pickItem);
                _printDropInfo("验证成功", pickItem, false);
            }
            else
            {
                Logger.LogErrorFormat("[战斗数据] 验证捡起掉落 {0}", pickItem);
                _printDropInfo("验证重复", pickItem, true);
            }
        }
    }

    private void _onSceneDungeonRewardRes(SceneDungeonRewardRes res)
    {
        _convertDungeonRewardResData(res);
    }

    private void _checkNoVertifyDrops()
    {
        if (HasNoVertifyDrops())
        {
            uint[] novertify = _getAllNoVertifyItems(mAllPickedDrops, mAllPickedVertifyDrops);

            for (int i = 0; i < novertify.Length; ++i)
            {
                _printDropInfo("!!没有捡到!!", novertify[i], true);
            }
        }
    }

    #endregion

    #region 结算掉落

    private List<Battle.DungeonDropItem> mCachedRaceEndDrops = new List<Battle.DungeonDropItem>();

    public void SendEndDropsRequest(byte multi)
    {
        SceneDungeonEndDropReq req = new SceneDungeonEndDropReq();
        req.multi = multi;
        _sendNetMsg2Gate<SceneDungeonEndDropReq>(req, "结算掉落");
    }

    public List<Battle.DungeonDropItem> GetRaceEndDrops()
    {
        return mCachedRaceEndDrops;
    }

    public bool HasRaceDropItem()
    {
        return null != mBattleInfo && 0 != mBattleInfo.endRaceDropMulti;
    }

    private void _onSceneDungeonEndDropRes(SceneDungeonEndDropRes res)
    {
        mBattleInfo.endRaceDropMulti = res.multi;

        mCachedRaceEndDrops.Clear();

        if (0 != res.multi)
        {
            Logger.LogProcessFormat("[战斗数据] 结算掉落倍数 {0}", res.multi);

            for (int i = 0; i < res.items.Length; ++i)
            {
                SceneDungeonDropItem drop = res.items[i];

                //_printDropInfoByItemID("结算掉落", drop.id, drop.typeId, drop.num, false);

                Battle.DungeonDropItem bdrop = new Battle.DungeonDropItem();

                bdrop.id       = (int)drop.id;
                bdrop.num      = (int)drop.num;
                bdrop.typeId   = (int)drop.typeId;
                bdrop.isDouble = drop.isDouble == 1;
                bdrop.strenthLevel = drop.strenth;
                bdrop.equipType = (EEquipType)drop.equipType;

                mCachedRaceEndDrops.Add(bdrop);
            }
        }
        else
        {
            Logger.LogErrorFormat("[战斗数据] 没有结算掉落");
        }
    }
    #endregion

    #region 结算消息
    private void _onSceneDungeonRaceEndRes(SceneDungeonRaceEndRes res)
    {
        Logger.LogProcessFormat("[战斗数据] 没有结算掉落");
    }
    #endregion

    #region 进入游戏的请求
    private bool mHasRecivedEnterRaceRes = false;

    public void SendDungeonEnterRace()
    {
		if (Global.Settings.startSystem == EClientSystem.Battle)
			return;

        WorldDungeonEnterRaceReq req = new WorldDungeonEnterRaceReq();
        Logger.LogProcessFormat("[战斗数据] 通知服务器进入游戏 {0}", ObjectDumper.Dump(req));
        NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
    }

    public bool HasRecivedDungeonEnterRace()
    {
        return mHasRecivedEnterRaceRes;
    }

    public void ResendDungeonEnterRace()
    {
        if (!HasRecivedDungeonEnterRace())
        {
            SendDungeonEnterRace();
        }
    }

    private void _onWorldDungeonEnterRaceRes(WorldDungeonEnterRaceRes res)
    {
        Logger.LogProcessFormat("[战斗数据] 服务器知道我进游戏了");
        mHasRecivedEnterRaceRes = true;
    }
    #endregion

#region 操作帧上报
    private class FrameNode
    {
        public Frame  frame;
        public UInt32 randomNum;

        public List<_fighterInput> cacheInputData = new List<_fighterInput>();
    }

    private class FrameRandomNum
    {
        public UInt32 frameIndex;
        public UInt32 randomNum;
    }

    private const UInt32 kInvalidFrameRandomNum      = UInt32.MinValue;
    private const int    kMaxFrameSize               = 64 * 2;
    private const int    kCheckSumSplit              = 50;

    private UInt32 mLastFrameIndexServerRecived      = 0;
    private UInt32 mLastFrameIndexClientSended       = 0;
    private bool   mHasGetFrameIndexServerRecived    = false;
    private bool   mIsUpdateCheck                    = true;
    private bool   mIsLockUpdateCheck                = false;
    private UInt32 mLastFrameReportRand              = 0;
    private UInt32 mFinalFrameIndex                  = 0;

    private List<FrameNode>      mLocalFrameQueue    = new List<FrameNode>();
    //  private List<FrameRandomNum> mLocalFrameNumCache = new List<FrameRandomNum>();
    private Dictionary<UInt32, FrameRandomNum> mLocalFrameNumCache = new Dictionary<UInt32, FrameRandomNum>();
    private UInt32 mLastFrameNumIndex = 0;
    private List<Frame>          mCacheFrameList     = new List<Frame>();
    private List<FrameChecksum>  mCacheFrameCheckSum = new List<FrameChecksum>();
    private bool mBattleEndLock = false;
    public UInt32 GetFinalFrameDataIndex()
    {
        return mFinalFrameIndex;
    }
    public void LockBattleEnd()
    {
        mBattleEndLock = true;
    }

    public void FinishUpdateFrameData()
    {
        mIsUpdateCheck = false;
    }

    public void PushFinalFrameData()
    {
        if (!_canProcessDungeonReportFrame())
        {
            return ;
        }

        UInt32 frameIndex        = _pushLastFrameNum2Queue();
        if (0 == frameIndex)
        {
            return ;
        }
   
        mIsLockUpdateCheck       = true;
        mFinalFrameIndex         = frameIndex;
    }

    private UInt32 _pushLastFrameNum2Queue()
    {
        //int finalIndex = mLocalFrameNumCache.Count - 1;
        //if (finalIndex < 0)
        //{
        //    Logger.LogProcessFormat("[战斗数据] 随机数为空");
        //    return 0;
        //}
        if (mLastFrameNumIndex <= 0)
        {
            Logger.LogProcessFormat("[战斗数据] 随机数为空");
            return 0;
        }

        FrameRandomNum randomNum = null;
        if (!mLocalFrameNumCache.TryGetValue(mLastFrameNumIndex, out randomNum))
        {
            Logger.LogErrorFormat("[战斗数据] 找不到随机数 {0}", mLastFrameNumIndex);
            return 0;
        }

        FrameNode frameNode      = _findLocalFrameByIndex(randomNum.frameIndex);

        if (null != frameNode)
        {
            if (frameNode.randomNum != randomNum.randomNum)
            {
                Logger.LogErrorFormat("[战斗数据] 随机不不同 {0}->{1}", frameNode.randomNum, randomNum.randomNum);
            }
        }
        else
        {
            Frame frameData          = new Frame();
            frameData.sequence       = randomNum.frameIndex;

            frameNode                = new FrameNode();
            frameNode.frame          = frameData;
            frameNode.randomNum      = randomNum.randomNum;

            _enqueuLocalFrameQueue(frameNode);
            Logger.LogProcessFormat("[战斗数据] 空帧数据 帧序号{0}, 随机数 {1}", randomNum.frameIndex, randomNum.randomNum);
        }

        return randomNum.frameIndex;
    }

    public void UnlockUpdateCheck()
    {
        mIsLockUpdateCheck = false;
    }

    public bool IsAllReportDataServerRecived()
    {
        return true;

        //kreturn mLocalFrameQueue.Count <= 0;

        //if (mLastFrameIndexServerRecived <= 0)
        //{
        //    return false;
        //}

        //return mFinalFrameIndex == mLastFrameIndexServerRecived;
    }

    public void PushLocalFrameRandNum(UInt32 frameIndex, UInt32 num)
    {
        if (!_canProcessDungeonReportFrame())
        {
            return ;
        }
        if (mLocalFrameNumCache.ContainsKey(frameIndex))
        {
            if (mLocalFrameNumCache[frameIndex].randomNum != num)
            {
                Logger.LogErrorFormat("[战斗数据] 已有 帧{0} 随机数 {1}<->{2} 不匹配",
                           mLocalFrameNumCache[frameIndex].frameIndex,
                           mLocalFrameNumCache[frameIndex].randomNum,
                           num);
            }
        }
        else
        {
            mLastFrameNumIndex = frameIndex;
            mLocalFrameNumCache.Add(frameIndex, new FrameRandomNum()
            {
                frameIndex = frameIndex,
                randomNum = num
            });
        }
        //bool find = false;
        //for (int i = 0; i < mLocalFrameNumCache.Count; ++i)
        //{
        //    if (mLocalFrameNumCache[i].frameIndex == frameIndex)
        //    {
        //        find = true;

        //        if (mLocalFrameNumCache[i].randomNum != num)
        //        {
        //            Logger.LogErrorFormat("[战斗数据] 已有 帧{0} 随机数 {1}<->{2} 不匹配", 
        //                    mLocalFrameNumCache[i].frameIndex,
        //                    mLocalFrameNumCache[i].randomNum,
        //                    num);
        //        }
        //        break;
        //    }
        //}

        //if (!find)
        //{
        //    mLocalFrameNumCache.Add(new FrameRandomNum()
        //    {
        //        frameIndex = frameIndex,
        //        randomNum  = num
        //    });
        //}
    }

    public void PushLocalFrameData(IFrameCommand frame)
    {
        if (!_canProcessDungeonReportFrame())
        {
            return ;
        }

        if (null == frame)
        {
            Logger.LogProcessFormat("[战斗数据] 压入帧数据为空");
            return ;
        }


        _fighterInput inputData  = new _fighterInput();
        inputData.input          = frame.GetInputData();
        inputData.seat           = frame.GetSeat();

        UInt32 randomNum         = _findFrameRandnumByFrameIndex(frame.GetFrame());

        FrameNode frameNode      = _findLocalFrameByIndex(frame.GetFrame());

        if (null != frameNode)
        {
            frameNode.cacheInputData.Add(inputData);
            Logger.LogProcessFormat("[战斗数据] 更新帧数据 帧序号{0}, 随机数 {1}", frame.GetFrame(), randomNum);
        }
        else
        {
            Frame frameData     = new Frame();
            frameData.data      = null;
            frameData.sequence  = frame.GetFrame();
            
            frameNode           = new FrameNode();
            frameNode.frame     = frameData;
            frameNode.randomNum = randomNum;
            frameNode.cacheInputData.Add(inputData);

            Logger.LogProcessFormat("[战斗数据] 压入帧数据 帧序号{0}, 随机数 {1}", frame.GetFrame(), randomNum);
            _enqueuLocalFrameQueue(frameNode);
        }
    }

    public void SendWorldDungeonReportFrame()
    {
        if (!_canProcessDungeonReportFrame())
        {
            Logger.LogProcessFormat("[战斗数据] 不上报帧数据了");
            return ;
        }
     //   if (ClientApplication.isOpenNewReconnectAlgo)
        {
            WorldDungeonReportFrameReq req = _getWorldDungeonReportFrameReq();
            if (req == null)
            {
                _pushLastFrameNum2Queue();
            }
            else
            {
                Logger.LogProcessFormat("[战斗数据] FrameReport 发送验证数据 {0}", ObjectDumper.Dump(req));
                NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
            }
        }
        //else
        //{
        //    WorldDungeonReportFrameReq req = _getWorldDungeonReportFrameReq();
        //    if (!_sendNetMsg2Gate<WorldDungeonReportFrameReq>(req, "FrameReport"))
        //    {
        //        _pushLastFrameNum2Queue();
        //    }
        //}
    }

    private bool _canProcessDungeonReportFrame()
    {
        if (eDungeonMode.LocalFrame != dungeonMode)
        {
            return false;
        }

        //修炼场不要发送结算消息
        if(mBattleType == BattleType.TrainingPVE)
        {
            return false;
        }

        return true;
    }

    private void _processSendWorldDungeonReportFrame()
    {
        if (!mIsUpdateCheck)
        {
            Logger.LogProcessFormat("[战斗数据] 比赛已经结束，不另外上报了哟哟哟哟哟哟");
            return;
        }

        if(mIsLockUpdateCheck)
        {
            Logger.LogProcessFormat("[战斗数据] 正在上报数据，不另外上报数据哟哟哟哟哟");
            return;
        }

        SendWorldDungeonReportFrame();
    }

    private void _removeUnUsedFrameRandeNum(UInt32 endIndex)
    {
        //if (!mHasGetFrameIndexServerRecived)
        //{
        //    Logger.LogProcessFormat("[战斗数据] 还未收到服务器返回, 不清楚缓存随机数");
        //    return ;
        //}

        //mLastFrameReportRand = mLastFrameIndexServerRecived / kCheckSumSplit * kCheckSumSplit;

        ////mLocalFrameNumCache.RemoveAll((x)=> 
        ////{
        ////    return x.frameIndex <= mLastFrameIndexServerRecived;
        ////});
        mLastFrameReportRand = endIndex / kCheckSumSplit * kCheckSumSplit;
        List<UInt32> removeList = new List<UInt32>();
        var iter = mLocalFrameNumCache.GetEnumerator();
        UInt32 lastFrameIndex = 0;
        while (iter.MoveNext())
        {
            if (iter.Current.Key <= mLastFrameReportRand)
            {
                removeList.Add(iter.Current.Key);
            }
            else if (lastFrameIndex < iter.Current.Key)
            {
                lastFrameIndex = iter.Current.Key;
            }
        }
        for (int i = 0; i < removeList.Count; i++)
        {
            mLocalFrameNumCache.Remove(removeList[i]);
        }
    }

    private UInt32 _findFrameRandnumByFrameIndex(UInt32 frameIndex)
    {
        if (mLocalFrameNumCache.ContainsKey(frameIndex))
        {
            return mLocalFrameNumCache[frameIndex].randomNum;
        }
        //for (int i = 0; i < mLocalFrameNumCache.Count; ++i)
        //{
        //    if (frameIndex == mLocalFrameNumCache[i].frameIndex)
        //    {
        //        return mLocalFrameNumCache[i].randomNum;
        //    }
        //}

        Logger.LogProcessFormat("[战斗数据] 无法找到 {0} 的随机数", frameIndex);

        return kInvalidFrameRandomNum;
    }
    private bool _removeFrameRandumByFrameIndex(UInt32 frameIndex)
    {
        if (mLocalFrameNumCache.Count > 1 && mLocalFrameNumCache.ContainsKey(frameIndex))
        {
            mLocalFrameNumCache.Remove(frameIndex);
            return true;
        }
        return false;
    }


    public void ResetLastFrameIndexClientSendedToServerRecived()
    {
        Logger.LogProcessFormat("[战斗数据] 重置客户端发送的帧序号到 {0} -> {1}", mLastFrameIndexClientSended, mLastFrameIndexServerRecived);
        mLastFrameIndexClientSended = mLastFrameIndexServerRecived;
    }

    private WorldDungeonReportFrameReq _getWorldDungeonReportFrameReq()
    {
        //_removeServerRecivedFrame();

        mCacheFrameList.Clear();
        mCacheFrameCheckSum.Clear();

        UInt32 startFrameIndex  = 0;
        UInt32 endFrameIndex    = 0;

        bool   hasGetStartFrame = false;
        List<int> removeLocalFrameQueue = new List<int>();
        for (int i = 0; i < mLocalFrameQueue.Count && i < kMaxFrameSize; ++i)
        {
            FrameNode frame = mLocalFrameQueue[i];

            if (mLastFrameIndexClientSended < frame.frame.sequence)
            {
                if (null == frame.frame.data)
                {
                    frame.frame.data = frame.cacheInputData.ToArray();
                }

                if (!hasGetStartFrame)
                {
                    hasGetStartFrame = true;
                    startFrameIndex  = frame.frame.sequence;
                }

                endFrameIndex = frame.frame.sequence;
                removeLocalFrameQueue.Add(i);
                mCacheFrameList.Add(frame.frame);
            }
        }

        _tryAppendCheckSum(endFrameIndex);

        if (mCacheFrameList.Count <= 0 && mCacheFrameCheckSum.Count <= 0)
        {
            return null;
        }
        //  _tryRemoveCheckSum(endFrameIndex);    
        _removeUnUsedFrameRandeNum(endFrameIndex);
        for (int i = removeLocalFrameQueue.Count - 1; i >= 0; i--)
        {
            mLocalFrameQueue.RemoveAt(removeLocalFrameQueue[i]);
        }
        WorldDungeonReportFrameReq req = new WorldDungeonReportFrameReq();

        req.frames                     = mCacheFrameList.ToArray();
        req.checksums                  = mCacheFrameCheckSum.ToArray();

        mLastFrameIndexClientSended    = endFrameIndex;

        Logger.LogProcessFormat("[战斗数据] *发送{0}-{1}帧", startFrameIndex, endFrameIndex);

#if UNITY_EDITOR
        string ans = string.Empty;

        for (int i = 0; i < req.frames.Length; ++i)
        {
            ans += string.Format("{0},", req.frames[i].sequence);
        }

        for (int i = 0; i < req.checksums.Length; ++i)
        {
            ans += string.Format("[-{0},{1}],", req.checksums[i].frame, req.checksums[i].checksum);
        }

        Logger.LogProcessFormat("[战斗数据] *发送{0}帧", ans);
#endif

        return req;
    }

    private void _removeServerRecivedFrame()
    {
        UInt32 startFrameIndex = 0;
        UInt32 endFrameIndex   = 0;

        bool hasGetStartFrame = false;

        while (!_isLocalFrameQueueEmpty() && _isLocalFrameQueueContainInvalidFrame())
        {
            FrameNode node = _dequeuLocalFrameQueue();

            if (null == node)
            {
                break;
            }

            if (!hasGetStartFrame)
            {
                startFrameIndex = node.frame.sequence;
            }

            endFrameIndex    = node.frame.sequence;

            hasGetStartFrame = true;
        }

        if (hasGetStartFrame)
        {
            Logger.LogProcessFormat("[战斗数据] 服务器已经收{0}-{1}帧 直接从队列中丢弃", startFrameIndex, endFrameIndex);
        }
        else
        {
            Logger.LogProcessFormat("[战斗数据] 没有需要丢弃的帧数据");
        }
    }

    private bool _isLocalFrameQueueContainInvalidFrame()
    {
        if (!mHasGetFrameIndexServerRecived)
        {
            Logger.LogProcessFormat("[战斗数据] 还未收到服务器返回, 不清楚队列数据");
            return false;
        }

        FrameNode frame = _getFrontLocalFrameQueue();

        if (null == frame.frame)
        {
            return true;
        }

        return frame.frame.sequence <= mLastFrameIndexServerRecived;
    }

    private bool _isLocalFrameQueueEmpty()
    {
        return mLocalFrameQueue.Count <= 0;
    }

    private FrameNode _findLocalFrameByIndex(UInt32 frameIndex)
    {
        for (int i = 0; i < mLocalFrameQueue.Count; ++i)
        {
            if (frameIndex == mLocalFrameQueue[i].frame.sequence)
            {
                return mLocalFrameQueue[i];
            }
        }

        return null; 
    }

    private FrameNode _getFrontLocalFrameQueue()
    {
        int count = mLocalFrameQueue.Count;
        if (count <= 0)
        {
            Logger.LogErrorFormat("[战斗数据] 队列为空");
            return null;
        }

        return mLocalFrameQueue[0];
    }

    private void _enqueuLocalFrameQueue(FrameNode node)
    {
        mLocalFrameQueue.Add(node);
    }

    private FrameNode _dequeuLocalFrameQueue()
    {
        int count = mLocalFrameQueue.Count;
        if (count <= 0)
        {
            return null;
        }

        FrameNode frame = mLocalFrameQueue[0];
        mLocalFrameQueue.RemoveAt(0);
        return frame;
    }


    private void _tryAppendCheckSum(UInt32 end)
    {
        UInt32 endFrameIndex = end / kCheckSumSplit * kCheckSumSplit;

        if (endFrameIndex <= 0)
        {
            return ;
        }

        UInt32 startIndex = mLastFrameIndexClientSended / kCheckSumSplit * kCheckSumSplit + kCheckSumSplit;// mLastFrameReportRand + kCheckSumSplit; //start / kCheckSumSplit * kCheckSumSplit;

        bool flag = false;

        while (startIndex <= end)
        {
            flag = true;

            _addFrameCheckSum(startIndex);

            startIndex += kCheckSumSplit;
        }

        if (!flag)
        {
            _addFrameCheckSum(endFrameIndex);
        }
    }

    public void _tryRemoveCheckSum(UInt32 end)
    {
        UInt32 endFrameIndex = end / kCheckSumSplit * kCheckSumSplit;

        if (endFrameIndex <= 0)
        {
            return;
        }

        UInt32 startIndex = mLastFrameReportRand + kCheckSumSplit; //start / kCheckSumSplit * kCheckSumSplit;
        bool flag = false;
        //   Logger.LogErrorFormat("_tryRemoveCheckSum end {0} startIndex {1} count {2}", end, startIndex, end - startIndex);
        while (startIndex <= end)
        {
            flag = true;
            _removeFrameRandumByFrameIndex(startIndex);
            startIndex += kCheckSumSplit;
        }
        if (!flag)
        {
            _removeFrameRandumByFrameIndex(endFrameIndex);
        }
    }

    private void _addFrameCheckSum(UInt32 frameIndex)
    {
        UInt32 randomNum = _findFrameRandnumByFrameIndex(frameIndex);

        //Logger.LogProcessFormat("[战斗数据] 上报随机数 帧数{0} 随机数{1}", frameIndex, randomNum);

        mCacheFrameCheckSum.Add(new FrameChecksum() 
        { 
            frame    = frameIndex,
            checksum = randomNum 
        });
    }
        
    private void _onWorldDungeonReportFrameRes(WorldDungeonReportFrameRes res)
    {
        if (res.lastFrame != mLastFrameIndexClientSended)
        {
            Logger.LogErrorFormat("[战斗数据] 上报数据回复错误 {0}-{1}", res.lastFrame, mLastFrameIndexClientSended);
        }
        Logger.LogProcessFormat("[战斗数据] 上报数据回复 {0}", res.lastFrame);

        //mHasGetFrameIndexServerRecived = true;

        //if (mLastFrameIndexServerRecived < res.lastFrame)
        //{
        //    Logger.LogProcessFormat("[战斗数据] 更新服务器接受最大帧 {0} -> {1}", mLastFrameIndexServerRecived, res.lastFrame);
        //    mLastFrameIndexServerRecived = res.lastFrame;
        //}

        //_removeServerRecivedFrame();
        //_removeUnUsedFrameRandeNum();
    }

#endregion
//#IsAllSceneDungeonAreaChangedMsgIsRecived

    #region 调试信息
    private Battle.DungeonDropItem _getDungeonDropItemByID(uint dropId)
    {
        List<Battle.DungeonDropItem> alldrops = mBattleInfo.dropItems;

        for (int i = 0; i < alldrops.Count; ++i)
        {
            if (alldrops[i].id == dropId)
            {
                return alldrops[i];
            }
        }

        return null;
    }

    private void _printDropInfoByItemID(string tag, uint id, uint itemID, uint count, bool isError)
    {
        ItemTable table = TableManager.instance.GetTableItem<ItemTable>((int)itemID);
        if (null != table)
        {
            if (isError)
            {
                Logger.LogErrorFormat("[战斗数据] {0} 道具 ID: {1}({2}), 名字 {3}, 数量 {4}", tag, id, itemID, table.Name, count);
            }
            else
            {
                Logger.LogProcessFormat("[战斗数据] {0} 道具 ID: {1}({2}), 名字 {3}, 数量 {4}", tag, id, itemID, table.Name, count);
            }
        }
    }

    private void _printDropInfo(string tag, uint id, bool isError = false)
    {
        Battle.DungeonDropItem drop = _getDungeonDropItemByID(id);
        if (null != drop)
        {
            _printDropInfoByItemID(tag, (uint)drop.id, (uint)drop.typeId, (uint)drop.num, isError);
        }
    }

    private void _printMonsterInfo(uint id)
    {

    }

    /// <summary>
    /// 判断当前关卡是否是异界关卡
    /// </summary>
    /// <returns></returns>
    public bool IsYiJieCheckPoint()
    {
        if (mTable == null) return false;
        return mTable.SubType == DungeonTable.eSubType.S_DEVILDDOM;
    }

    /// <summary>
    /// 判断当前是否是混沌关卡
    /// </summary>
    /// <returns></returns>
    public bool IsHunDunCheckPoint()
    {
        if (mTable == null) return false;
        return mTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER|| mTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY|| mTable.SubType == DungeonTable.eSubType.S_WEEK_HELL;
    }
    #endregion

    #endregion
}
