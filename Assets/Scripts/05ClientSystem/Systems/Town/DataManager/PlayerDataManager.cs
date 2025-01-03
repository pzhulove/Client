using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Runtime.InteropServices;
using ActivityLimitTime;
using Protocol;
using Network;

namespace GameClient
{
    public class PlayerDataManager : Singleton<PlayerDataManager>, IDataManager
    {
        List<IDataManager> m_arrDataManagers = new List<IDataManager>();

        public override void Init()
        {
            base.Init();

            /*
            Assembly asmScripts = Assembly.GetExecutingAssembly();
            if (asmScripts != null)
            {
                System.Type[] types = asmScripts.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    if (types[i].IsAbstract)
                    {
                        continue;
                    }

                    if (types[i].GetInterface(typeof(IDataManager).Name) == null)
                    {
                        continue;
                    }

                    if (types[i] == typeof(PlayerDataManager))
                    {
                        continue;
                    }

                    var method = types[i].BaseType.GetMethod("GetInstance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (method != null)
                    {
                        IDataManager obj = method.Invoke(null, new object[0]) as IDataManager;
                        if (obj != null)
                        {
                            _AddDataManager(obj);
                        }
                    }
                }
            }*/

            _AddDataManager(GameClient.PlayerBaseData.GetInstance());
            _AddDataManager(GameClient.BattleDataManager.GetInstance());
            _AddDataManager(GameClient.AchievementGroupDataManager.GetInstance());
            _AddDataManager(GameClient.ActivityNoticeDataManager.GetInstance());
            _AddDataManager(GameClient.AnnouncementManager.GetInstance());
            _AddDataManager(GameClient.CostItemManager.GetInstance());
            _AddDataManager(GameClient.CountDataManager.GetInstance());
            _AddDataManager(GameClient.EquipSuitDataManager.GetInstance());
            _AddDataManager(GameClient.EquipMasterDataManager.GetInstance());
            _AddDataManager(GameClient.EquipForgeDataManager.GetInstance());
            _AddDataManager(GameClient.EquipRecoveryDataManager.GetInstance());
            _AddDataManager(GameClient.GuildDataManager.GetInstance());
            _AddDataManager(GameClient.ItemDataManager.GetInstance());
            _AddDataManager(GameClient.ItemSourceInfoTableManager.GetInstance());
            _AddDataManager(GameClient.ItemTipManager.GetInstance());
            _AddDataManager(GameClient.MagicBoxDataManager.GetInstance());
            _AddDataManager(GameClient.JarDataManager.GetInstance());
            _AddDataManager(GameClient.MallDataManager.GetInstance());
            _AddDataManager(GameClient.MoneyRewardsDataManager.GetInstance());
            _AddDataManager(GameClient.NewMessageNoticeManager.GetInstance());
            _AddDataManager(GameClient.SeasonDataManager.GetInstance());
            _AddDataManager(GameClient.PetDataManager.GetInstance());
            _AddDataManager(GameClient.Pk3v3DataManager.GetInstance());
            _AddDataManager(GameClient.RedPackDataManager.GetInstance());
            _AddDataManager(GameClient.RedPointDataManager.GetInstance());
            _AddDataManager(GameClient.SkillDataManager.GetInstance());
            _AddDataManager(GameClient.SwitchWeaponDataManager.GetInstance());
            _AddDataManager(GameClient.TeamDataManager.GetInstance());
            _AddDataManager(GameClient.WaitNetMessageManager.GetInstance());
            _AddDataManager(GameClient.ActivityDungeonDataManager.GetInstance());
            _AddDataManager(GameClient.OPPOPrivilegeDataManager.GetInstance());
            _AddDataManager(GameClient.ChapterBuffDrugManager.GetInstance());
            _AddDataManager(GameClient.EquipHandbookDataManager.GetInstance());
            _AddDataManager(FashionLimitTimeBuy.FashionLimitTimeBuyManager.GetInstance());
            _AddDataManager(ActivityLimitTimeCombineManager.GetInstance());
            _AddDataManager(MobileBind.MobileBindManager.GetInstance());
            _AddDataManager(OnlineServiceManager.GetInstance());
            _AddDataManager(ActivityTreasureLotteryDataManager.GetInstance());
            _AddDataManager(GameClient.RelationDataManager.GetInstance());
            _AddDataManager(_Settings.RoleInfoSettingManager.GetInstance());
            _AddDataManager(SevendaysDataManager.GetInstance());
            _AddDataManager(GameClient.ShopDataManager.GetInstance());
            _AddDataManager(GameClient.StrengthenDataManager.GetInstance());
            _AddDataManager(GameClient.ActiveManager.GetInstance());
            _AddDataManager(GameClient.BeadCardManager.GetInstance());
            _AddDataManager(GameClient.BudoManager.GetInstance());
            _AddDataManager(GameClient.ChatManager.GetInstance());
            _AddDataManager(GameClient.ChatRecordManager.GetInstance());
            _AddDataManager(GameClient.DevelopGuidanceManager.GetInstance());
            _AddDataManager(GameClient.EnchantmentsCardManager.GetInstance());
            _AddDataManager(GameClient.FashionAttributeSelectManager.GetInstance());
            _AddDataManager(GameClient.FashionMergeManager.GetInstance());
            _AddDataManager(GameClient.LinkManager.GetInstance());
            _AddDataManager(GameClient.MissionManager.GetInstance());
            _AddDataManager(GameClient.NpcRelationMissionManager.GetInstance());
            _AddDataManager(GameClient.OtherPlayerInfoManager.GetInstance());
            _AddDataManager(GameClient.SystemConfigManager.GetInstance());
            _AddDataManager(GameClient.TimeManager.GetInstance());
            _AddDataManager(TittleBookManager.GetInstance());
            _AddDataManager(FinancialPlanDataManager.GetInstance());
            _AddDataManager(PackageDataManager.GetInstance());
            _AddDataManager(GiftPackDataManager.GetInstance());
            _AddDataManager(GameClient.ServerSceneFuncSwitchManager.GetInstance());
            _AddDataManager(GameClient.ActivityDataManager.GetInstance());
            _AddDataManager(GameClient.ActivityManager.GetInstance());
            _AddDataManager(AttackCityMonsterDataManager.GetInstance());
            _AddDataManager(GameClient.Pk3v3CrossDataManager.GetInstance());
            _AddDataManager(GameClient.DailyChargeRaffleDataManager.GetInstance());
            _AddDataManager(GameClient.TAPNewDataManager.GetInstance());
			_AddDataManager(GameClient.RandomTreasureDataManager.GetInstance());
            _AddDataManager(MallNewDataManager.GetInstance());
            _AddDataManager(ShopNewDataManager.GetInstance());
			_AddDataManager(HorseGamblingDataManager.GetInstance());
            _AddDataManager(SecurityLockDataManager.GetInstance());
            _AddDataManager(StrengthenTicketMergeDataManager.GetInstance());
            _AddDataManager(AdventureTeamDataManager.GetInstance());
            _AddDataManager(AuctionNewDataManager.GetInstance());
            _AddDataManager(AccountShopDataManager.GetInstance());
            _AddDataManager(ArtifactDataManager.GetInstance());
            _AddDataManager(BlackMarketMerchantDataManager.GetInstance());
            _AddDataManager(EquipUpgradeDataManager.GetInstance());
            _AddDataManager(TopUpPushDataManager.GetInstance());
            _AddDataManager(ChijiDataManager.GetInstance());
            _AddDataManager(MailDataManager.GetInstance());
            _AddDataManager(GameClient.FollowingOpenQueueManager.GetInstance());
            _AddDataManager(WeekSignInDataManager.GetInstance());
            _AddDataManager(HeadPortraitFrameDataManager.GetInstance());
            _AddDataManager(TitleDataManager.GetInstance());
			_AddDataManager(MagicCardMergeDataManager.GetInstance());
			_AddDataManager(MonthCardRewardLockersDataManager.GetInstance());
			_AddDataManager(TeamDuplicationDataManager.GetInstance());
            _AddDataManager(FairDuelDataManager.GetInstance());
            _AddDataManager(EquipGrowthDataManager.GetInstance());
			_AddDataManager(DailyTodoDataManager.GetInstance());
            _AddDataManager(BaseWebViewManager.GetInstance());
            _AddDataManager(BattleEasyChatDataManager.GetInstance());
			_AddDataManager(DeadLineReminderDataManager.GetInstance());
            _AddDataManager(InscriptionMosaicDataManager.GetInstance());
            _AddDataManager(AdventurerPassCardDataManager.GetInstance());
            _AddDataManager(GameClient.Pk2v2CrossDataManager.GetInstance());
            _AddDataManager(EquipPlanDataManager.GetInstance());
            _AddDataManager(HonorSystemDataManager.GetInstance());
            _AddDataManager(EpicEquipmentTransformationDataManager.GetInstance());
            _AddDataManager(ChijiShopDataManager.GetInstance());
            _AddDataManager(WarriorRecruitDataManager.GetInstance());
            _AddDataManager(StorageDataManager.GetInstance());
            _AddDataManager(SceneSettingDataManager.GetInstance());
            _AddDataManager(SceneMapDataManager.GetInstance());
        }

        public EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public void BindEnterGameMsg(List<uint> a_msgEvent)
        {
            Logger.LogProcessFormat("绑定所有数据管理器进游戏的消息:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].BindEnterGameMsg(a_msgEvent);
                Logger.LogProcessFormat("{0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("完成！！！");
        }

        public void InitiallizeSystem()
        {
            Logger.LogProcessFormat("初始化所有数据管理器:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].InitiallizeSystem();
                Logger.LogProcessFormat("{0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("完成！！！");
        }

        public void ProcessInitNetMessage(WaitNetMessageManager.NetMessages a_msgEvent)
        {
            Logger.LogProcessFormat("初始化所有数据管理器:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].ProcessInitNetMessage(a_msgEvent);
                Logger.LogProcessFormat("{0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("完成！！！");
        }
        

        /*
        public void InitializeAll(WaitNetMessageManager.NetMessages a_msgEvent)
        {
            Logger.LogProcessFormat("初始化所有数据管理器:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].InitializeAll(a_msgEvent);
                Logger.LogProcessFormat("{0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("完成！！！");
        }
        */

        public void ClearAll()
        {
            Logger.LogProcessFormat("清空所有数据管理器:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].ClearAll();
                Logger.LogProcessFormat("{0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("完成！！！");

            LoginToggleMsgBoxOKCancelFrame.Reset();
        }

        public void Update(float a_fTime)
        {
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].Update(a_fTime);
            }
        }

        public void OnEnterSystem()
        {
            Logger.LogProcessFormat("EnterSystem:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].OnEnterSystem();
                Logger.LogProcessFormat("EnterSystem {0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("EnterSystem Finish!");
        }

        public void OnExitSystem()
        {
            Logger.LogProcessFormat("ExitSystem:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].OnExitSystem();
                Logger.LogProcessFormat("ExitSystem {0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("ExitSystem Finish!");
        }

        public void OnApplicationStart()
        {
            Logger.LogProcessFormat("OnApplicationStart:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].OnApplicationStart();
                Logger.LogProcessFormat("OnApplicationStart {0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("OnApplicationStart Finish!");
        }

        public void OnApplicationQuit()
        {
            Logger.LogProcessFormat("OnApplicationQuit:");
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                m_arrDataManagers[i].OnApplicationQuit();
                Logger.LogProcessFormat("OnApplicationQuit {0}", m_arrDataManagers[i].GetType().Name);
            }
            Logger.LogProcessFormat("OnApplicationQuit Finish!");
        }

        void _AddDataManager(IDataManager a_dataManager)
        {
            if (a_dataManager == null)
            {
                return;
            }

            bool bAdded = false;
            for (int i = 0; i < m_arrDataManagers.Count; ++i)
            {
                if (m_arrDataManagers[i] == a_dataManager)
                {
                    return;
                }
                if (a_dataManager.GetOrder() < m_arrDataManagers[i].GetOrder())
                {
                    m_arrDataManagers.Insert(i, a_dataManager);
                    bAdded = true;
                    break;
                }
            }
            if (bAdded == false)
            {
                m_arrDataManagers.Add(a_dataManager);
            }
        }
    }
}
