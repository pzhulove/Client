using System.Collections.Generic;
using Protocol;
using ProtoTable;
using System.Reflection;
using System;
using Network;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameClient
{
    public enum SkillLevelAddType
    {
        EQUIP = 0,          //装备
        SKILL               //其他技能
    }

    public enum SkillSystemSourceType
    {
        None = 0,           
        FairDuel,           //公平竞技场
        Chiji               //吃鸡
    }

    public enum ESkillPage
    {
        Page1 = 0,
        Page2 = 1,
    }

    public class SkillLevelAddInfo
    {
        public List<SkillLevelAddItem> items = new List<SkillLevelAddItem>();
        public int totalAddLevel = 0;

        #region -
        public int GetCurrentTotalAddNum()
        {
            int sum = 0;
            for (int i = 0; i < items.Count; ++i)
                sum += items[i].addLevel;

            return sum;
        }

        public void DebugPrint(int skillID)
        {
            Logger.LogErrorFormat("技能:{0} 等级总加成:{1}\n", skillID, totalAddLevel);
            for (int i = 0; i < items.Count; ++i)
            {
                Logger.LogErrorFormat("{0}:{1}", items[i].type, items[i].addLevel);
            }
        }
        #endregion
    }

    public class SkillLevelAddItem
    {
        public SkillLevelAddType type;
        public int addLevel;
        public SkillLevelAddItem(SkillLevelAddType t, int n)
        {
            type = t;
            addLevel = n;
        }
    }

    class SkillDataManager : DataManager<SkillDataManager>
    {
        // 服务器数据
        private List<Skill> skillList = new List<Skill>();             // PVE玩家已经学习的技能列表(不包含后来新加的被动技能页签下的技能)
        private List<Skill> skillList2 = new List<Skill>();            //PVE玩家学习的技能列表2

        private List<Skill> pvpSkillList = new List<Skill>();          // 同上，用于pvp的技能树
        private List<Skill> pvpSkillList2 = new List<Skill>();         // 同上，用于pvp的技能树2

        private List<SkillBarGrid> skillBar = new List<SkillBarGrid>();        // 玩家当前使用的技能栏
        private List<SkillBarGrid> skillBar2 = new List<SkillBarGrid>();       // 玩家当前使用的技能栏2

        private List<SkillBarGrid> pvpSkillBar = new List<SkillBarGrid>();     // 同上用于pvp玩家当前使用的技能栏
        private List<SkillBarGrid> pvpSkillBar2 = new List<SkillBarGrid>();    // 同上用于pvp玩家当前使用的技能栏2
        private Dictionary<int, List<TalentTable>> mSkillTalentData = new Dictionary<int, List<TalentTable>>(); //技能对应天赋数据

        public bool PVESkillPage2IsUnlock = false;      //pve的技能页2是否已经解锁
        public bool PVPSkillPage2IsUnlock = false;      //Pvp的技能页2是否已经解锁

        public List<Skill> ChijiSkillList = new List<Skill>();                   // 吃鸡模式专用技能树
        public List<SkillBarGrid> ChijiSkillBar = new List<SkillBarGrid>();      // 吃鸡专用技能栏

        public List<Skill> FairDuelSkillList = new List<Skill>();                //公平竞技场的技能树
        public List<SkillBarGrid> FairDuelSkillBar = new List<SkillBarGrid>();   //公平竞技场的技能栏
        public bool IsHaveSetFairDueSkillBar = false;                            //是否已经设置了公平竞技场的技能栏

  
        // 客户端数据
        public List<int> InitSkills = new List<int>();
        public List<int> LockSkillList = new List<int>();
        public List<int> NewOpenSkillList = new List<int>();
        public List<int> NewOpenSkillAllForBattle = new List<int>();
        public int LastSeeSkillLv = 1;
        public bool bNoticeSkillLvUp = true;
        public bool IsNotShowSkillConfigTip = false;
        private const int ShowSkillButtonLevel = 15;   //显示技能按钮的等级

        public bool IsFinishUnlockTask = false; //在60级的时候，是否完成了解锁任务
        public int UnLockTaskLvl = 60;  //解锁任务的等级

        public ESkillPage CurPVESKillPage = ESkillPage.Page1;//当前PVE的技能配置方案
        public ESkillPage CurPVPSKillPage = ESkillPage.Page1;//当前PVP的技能配置方案

        public Dictionary<int, SkillLevelAddInfo> AddSkillInfo = new Dictionary<int, SkillLevelAddInfo>();
        public Dictionary<int, SkillLevelAddInfo> pvpAddSkillInfo = new Dictionary<int, SkillLevelAddInfo>();

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.SkillDataManager;
        }

        public override void Initialize()
        {
            Clear();

            BindEvents();
            _InitSkillTalentData();
        }

        //加载技能天赋表数据
        private void _InitSkillTalentData()
        {
            if (null == mSkillTalentData)
                mSkillTalentData = new Dictionary<int, List<TalentTable>>();
            if (mSkillTalentData.Count > 0)
                return;
            var tables = TableManager.GetInstance().GetTable<TalentTable>();
            foreach (TalentTable table in tables.Values)
            {
                if (!mSkillTalentData.ContainsKey(table.SkillID))
                {
                    mSkillTalentData.Add(table.SkillID, new List<TalentTable>());
                }
                mSkillTalentData[table.SkillID].Add(table);
            }
        }

        //判断技能是否有天赋
        public bool IsSkillHaveTalent(int skillId)
        {
            return mSkillTalentData.ContainsKey(skillId);
        }
        //获取技能的所有天赋
        public List<TalentTable> GetSkillTalentData(int skillId)
        {
            if (mSkillTalentData.ContainsKey(skillId))
            {
                return mSkillTalentData[skillId];
            }
            return null;
        }

        public override void Clear()
        {
            skillList.Clear();
            pvpSkillList.Clear();

            skillList2.Clear();
            pvpSkillList2.Clear();

            skillBar.Clear();
            pvpSkillBar.Clear();

            skillBar2.Clear();
            pvpSkillBar2.Clear();

            ClearChijiSkill();

            InitSkills.Clear();
            LockSkillList.Clear();
            NewOpenSkillList.Clear();
            NewOpenSkillAllForBattle.Clear();
            LastSeeSkillLv = 1;
            bNoticeSkillLvUp = true;
            IsNotShowSkillConfigTip = false;
            IsFinishUnlockTask = false;

            ClearFairDuelSkill();

            CurPVESKillPage = ESkillPage.Page1;
            CurPVPSKillPage = ESkillPage.Page1;

            PVESkillPage2IsUnlock = false;
            PVPSkillPage2IsUnlock = false;

            if(AddSkillInfo != null)
            {
                AddSkillInfo.Clear();
            }

            if(pvpAddSkillInfo != null)
            {
                pvpAddSkillInfo.Clear();
            }

            UnBindEvents();
        }

        public void ClearChijiSkill()
        {
            ChijiSkillList.Clear();
            ChijiSkillBar.Clear();
        }
        /// <summary>
        /// 清除公平竞技场技能数据
        /// </summary>
        private void ClearFairDuelSkill()
        {
            FairDuelSkillList.Clear();
            FairDuelSkillBar.Clear();
        }
         /// <summary>
         /// 设置PVE第一个技能页的skillbar
         /// </summary>
         /// <param name="list"></param>
        public void SetPvePage1SkillBar(List<SkillBarGrid> list)
        {
            skillBar = list;
        }

        public void UpdateSkillData(SkillMgr skillMgr, SkillConfigType skillType)
        {
            if(skillMgr.skillPages.Length<2)
            {
                Logger.LogError("服务器下发的技能页数量小于2");
                return;
            }
            for (int i = 0; i < skillMgr.skillPages.Length; i++)//技能页
            {
                for (int j = 0; j < skillMgr.skillPages[i].skillList.Length; j++)//技能
                {
                    Skill skill = skillMgr.skillPages[i].skillList[j];
                    SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skill.id);

                    if (skillData == null)
                    {
                        continue;
                    }
                
                    switch (skillType)
                    {
                        case SkillConfigType.SKILL_CONFIG_PVE:
                            if(i==0)//第一个pve技能页
                            {
                                UpdateSkillList(skill, skillList);
                            }
                            else if (i == 1)//第二个pve技能页
                            {
                                UpdateSkillList(skill, skillList2);
                            }
                           
                            break;
                        case SkillConfigType.SKILL_CONFIG_PVP:
                            if (i == 0)//第一个pvp技能页
                            {
                                UpdateSkillList(skill, pvpSkillList);
                            }
                            else if (i == 1)//第二个pvp技能页
                            {
                                UpdateSkillList(skill, pvpSkillList2);
                            }
                            break;
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillLearnedLevelChanged, skill); 
                }
               
            }
        }
        /// <summary>
        /// 更新PVP或者PVESkillList
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillList"></param>
        private void UpdateSkillList(Skill skill,List<Skill> skillList)
        {
            bool bFind = false;
            for (int j = 0; j < skillList.Count; j++)
            {
                if (skill.id == skillList[j].id)
                {
                    if (skill.level == 0)
                    {
                        skillList.RemoveAt(j);
                    }
                    else
                    {
                        skillList[j].level = skill.level;
                        skillList[j].talentId = skill.talentId;
                    }

                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                skillList.Add(skill);
            }
        }

        public void UpdateChijiSkillData(SkillMgr skillMgr)
        {
            if(skillMgr==null||skillMgr.skillPages.Length<=0)
            {
                Logger.LogError("skillMgr is null or skillMgr.skillPages.Length<=0 ");
                return;
            }
           Skill[] tmpSkillList = skillMgr.skillPages[0].skillList;//取第一个技能页
           
            for (int i = 0; i < tmpSkillList.Length; i++)
            {
                if (tmpSkillList[i].id == 0)
                {
                    continue;
                }

                bool bFind = false;

                for (int j = 0; j < ChijiSkillList.Count; j++)
                {
                    if (tmpSkillList[i].id == ChijiSkillList[j].id)
                    {
                        if (tmpSkillList[i].level == 0)
                        {
                            Logger.LogErrorFormat("吃鸡捡到的技能等级为0,技能id = {0}", tmpSkillList[i].id);
                        }
                        else
                        {
                            ChijiSkillList[j].level = tmpSkillList[i].level;
                            ChijiSkillList[j].talentId = tmpSkillList[i].talentId;
                        }

                        bFind = true;
                        break;
                    }
                  
                }

                if (!bFind)
                {
                    SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(tmpSkillList[i].id);
                    if (skillData == null)
                    {
                        return;
                    }

                    ChijiSkillList.Add(tmpSkillList[i]);

                    if(ChijiDataManager.GetInstance().CurBattleStage >= ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_1)
                    {
                        //SystemNotifyManager.SysNotifyFloatingEffect(string.Format("捡取技能 [{0}] Lv.{1}", skillData.Name, tmpSkillList[i].level));

                        // 捡到新技能自动配上去
                        //_AutoSendSaveChijiSkillPlan(skillData);
                    }
                }
            }
        }
        /// <summary>
        /// 更新公平竞技场的技能
        /// </summary>
        public void UpdateFairDuelSkillData(SkillMgr skillMgr)
        {
            if (skillMgr == null || skillMgr.skillPages.Length <= 0)
            {
                Logger.LogError("skillMgr is null or skillMgr.skillPages.Length<=0 ");
                return;
            }
            Skill[] tmpSkillList = skillMgr.skillPages[0].skillList;//取第一个技能页
            
            for (int i = 0; i < tmpSkillList.Length; i++)
            {
                if (tmpSkillList[i].id == 0)
                {
                    continue;
                }

                bool bFind = false;

                for (int j = 0; j < FairDuelSkillList.Count; j++)
                {
                    if (tmpSkillList[i].id == FairDuelSkillList[j].id)
                    {
                        if (tmpSkillList[i].level == 0)
                        {
                            FairDuelSkillList.RemoveAt(j);
                        }
                        else
                        {
                            FairDuelSkillList[j].level = tmpSkillList[i].level;
                            FairDuelSkillList[j].talentId = tmpSkillList[i].talentId;
                        }

                        bFind = true;
                        break;
                    }
                }

                if (!bFind)
                {
                    FairDuelSkillList.Add(tmpSkillList[i]);
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillLearnedLevelChanged, tmpSkillList[i]);
            }
        }

        void _AutoSendSaveChijiSkillPlan(SkillTable skillData)
        {
            if (ChijiSkillBar == null || ChijiSkillBar.Count >= 11)
            {
                return;
            }

            if (skillData.SkillType == SkillTable.eSkillType.PASSIVE || skillData.IsQTE != 0)
            {
                return;
            }

            if (skillData.IsBuff == 1)
            {
                return;
            }

            bool bAdaptJob = IsSkillJobAdaptToTargetJob(skillData, PlayerBaseData.GetInstance().JobTableID);

            if(!bAdaptJob)
            {
                return;
            }

            NetManager netMgr = NetManager.Instance();

            SceneExchangeSkillBarReq req = new SceneExchangeSkillBarReq();

            req.skillBars.index = 1;

            req.skillBars.bar = new SkillBar[1];
            req.skillBars.bar[0] = new SkillBar();
            req.skillBars.bar[0].grids = new SkillBarGrid[1];

            req.skillBars.bar[0].index = 1;
            req.skillBars.bar[0].grids[0] = new SkillBarGrid();
            req.skillBars.bar[0].grids[0].id = (UInt16)skillData.ID;
            req.skillBars.bar[0].grids[0].slot = (byte)(ChijiSkillBar.Count + 1);

            req.configType = (byte)SkillConfigType.SKILL_CONFIG_PVP;

            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 更新技能Bar
        /// </summary>
        /// <param name="skillBars"></param>
        /// <param name="skillType"></param>
        public void UpdateSkillBar(SkillBars skillBars, SkillConfigType skillType)
        {
            switch (skillType)
            {
                case SkillConfigType.SKILL_CONFIG_PVE:
                    if(skillBars.bar.Length>=2)
                    {
                        for (int i = 0; i < skillBars.bar.Length; i++)
                        {
                            if(i==0)
                            {
                                UpdatePvpOrPveSkillBar(skillBar, skillBars.bar[i]);
                            }
                            else if(i==1)
                            {
                                UpdatePvpOrPveSkillBar(skillBar2, skillBars.bar[i]);
                            }
                        }
                    }
                    break;
                case SkillConfigType.SKILL_CONFIG_PVP:
                    if (skillBars.bar.Length >= 2)
                    {
                        for (int i = 0; i < skillBars.bar.Length; i++)
                        {
                            if (i == 0)
                            {
                                UpdatePvpOrPveSkillBar(pvpSkillBar, skillBars.bar[i]);
                            }
                            else if (i == 1)
                            {
                                UpdatePvpOrPveSkillBar(pvpSkillBar2, skillBars.bar[i]);
                            }
                        }
                    }
                    break;
            }
            
        }
        /// <summary>
        /// 更新skillbar
        /// </summary>
        /// <param name="skillSolution"></param>
        /// <param name="skillBarGird"></param>
        /// <param name="skillbar"></param>
        private void UpdatePvpOrPveSkillBar(List<SkillBarGrid> skillBarGird, SkillBar skillbar)
        {
            skillBarGird.Clear();
            for (int j = 0; j < skillbar.grids.Length; j++)
            {
                skillBarGird.Add(skillbar.grids[j]);
            }
        }

        public void UpdateFairDuelSkillBar(SkillBars skillBars)
        {
            if (skillBars.bar.Length >= 1)
            {
                SkillBarGrid[] skillBarGrid = skillBars.bar[0].grids;
                if(skillBarGrid!=null)
                {
                    FairDuelSkillBar.Clear();
                    for (int i = 0; i < skillBarGrid.Length; i++)
                    {
                        FairDuelSkillBar.Add(skillBarGrid[i]);
                    }
                }
            }
           
        }

        public void UpdateChijiSkillBar(SkillBars skillBars)
        {
            for (int i = 0; i < skillBars.bar.Length; i++)
            {
                if (skillBars.bar[i].index != skillBars.index)
                {
                    continue;
                }

                ChijiSkillBar.Clear();

                for (int j = 0; j < skillBars.bar[i].grids.Length; j++)
                {
                    ChijiSkillBar.Add(skillBars.bar[i].grids[j]);
                }
            }
        }

        public void UpdateNewSkill()
        {
            _UpdateNewSkillList();

            if (NewOpenSkillList.Count > 0)
            {
                //Utility.NewNotifyAnimation("NewSkill");             
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SkillLvUpNoticeUpdate);

            ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (clientSystem != null)
            {
                RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.Skill);
            }
        }

        #region new skill data 纯数据接口,内部参数与UI无关,放心使用
        /// <summary>
        /// 获取PVE SkillList
        /// </summary>
        /// <returns></returns>
        public List<Skill> GetPveSkillList()
        {
            List<Skill> tmpSkill = null;
            if (CurPVESKillPage == ESkillPage.Page1)
            {
                tmpSkill = skillList;
            }
            else if (CurPVESKillPage == ESkillPage.Page2)
            {
                tmpSkill = skillList2;
            }

            return tmpSkill;
        }

        /// <summary>
        /// 获取PVP SkillList
        /// </summary>
        /// <returns></returns>
        public List<Skill> GetPvpSkillList()
        {
            List<Skill> tmpSkill = null;
            if (CurPVPSKillPage == ESkillPage.Page1)
            {
                tmpSkill = pvpSkillList;
            }
            else if (CurPVPSKillPage == ESkillPage.Page2)
            {
                tmpSkill = pvpSkillList2;
            }

            return tmpSkill;
        }

        /// <summary>
        /// 获取公平竞技场技能List
        /// </summary>
        /// <returns></returns>
        public List<Skill> GetFairDuelSkillList()
        {
            return FairDuelSkillList;
        }

        public List<Skill> GetChijiSkillList()
        {
            return ChijiSkillList;
        }

        /// <summary>
        /// 获取PVE技能配置
        /// </summary>
        /// <returns></returns>
        public List<SkillBarGrid> GetPveSkillBar()
        {
            List<SkillBarGrid> tmpSkillBar = null;
            if (CurPVESKillPage == ESkillPage.Page1)
            {
                tmpSkillBar = skillBar;
            }
            else if (CurPVESKillPage == ESkillPage.Page2)
            {
                tmpSkillBar = skillBar2;
            }

            return tmpSkillBar;
        }

        /// <summary>
        /// 获取PVP技能配置
        /// </summary>
        /// <returns></returns>
        public List<SkillBarGrid> GetPvpSkillBar()
        {
            List<SkillBarGrid> tmpSkillBar = null;
            if (CurPVPSKillPage == ESkillPage.Page1)
            {
                tmpSkillBar = pvpSkillBar;
            }
            else if (CurPVPSKillPage == ESkillPage.Page2)
            {
                tmpSkillBar = pvpSkillBar2;
            }

            return tmpSkillBar;
        }

        /// <summary>
        /// 获取公平竞技场的技能栏
        /// </summary>
        /// <returns></returns>
        public List<SkillBarGrid> GetFairDuelSkillBar()
        {
            return FairDuelSkillBar;
        }

        public List<SkillBarGrid> GetChijiSkillBar()
        {
            return ChijiSkillBar;
        }

        public Skill GetSkillInfoById(int SkillId, bool isPvp = false, SkillSystemSourceType skillSourceType = SkillSystemSourceType.None)
        {
            if ((BattleMain.instance != null && BattleMain.IsChiji()) || skillSourceType == SkillSystemSourceType.Chiji)
            {
                return ChijiSkillList.Find(s =>{ return s.id == SkillId; });
            }
            else if ((BattleMain.instance != null && BattleMain.IsFairDuel()) || skillSourceType == SkillSystemSourceType.FairDuel)
            {
                return FairDuelSkillList.Find(s =>{ return s.id == SkillId; });
            }
            else
            {
                if (isPvp)
                {
                    List<Skill> curSkillList = GetPvpSkillList();
                    return curSkillList.Find(s =>{ return s.id == SkillId; });
                }
                else
                {
                    List<Skill> curSkillList = GetPveSkillList();
                    return curSkillList.Find(s =>{ return s.id == SkillId; });
                }
            }
        }

        /// <summary>
        /// 给单机模式下使用（自由练习场和修炼场模式下，进入战斗时服务器不会下发数据，所以要客户端自己组织数据），。
        /// 正常的pve，pvp模式下，服务器给客户端下发的skilllist的数据集合也是上述范围
        /// 第三个参数表示怎么去过滤技能
        /// Battle: 战斗内获取角色已学习并且配置了的主动技能以及其他类型（被动，buff,QTE）的技能，已学习但是未配置的主动技能不包括在内
        /// SkillAddInfo :已经学习的所有的技能
        /// </summary>
        /// <param name="isPvp"></param>
        /// <returns></returns>
        public Dictionary<int, int> GetSkillInfo(bool isPvp, SkillSystemSourceType skillSourceType = SkillSystemSourceType.None)
        {
            Dictionary<int, int> skillInfo = new Dictionary<int, int>();

            // 获取skillbar
            List<SkillBarGrid> curSkillBar = GetSkillConfiguration(isPvp, skillSourceType);
            // 获取skilllist
            List<Skill> curSkillList = GetSkillList(isPvp, skillSourceType);

            //装备的技能
            foreach (var equipedSkill in curSkillBar)
            {
                Skill skill = GetSkillInfoById(equipedSkill.id, isPvp, skillSourceType);
                if (skill != null && !skillInfo.ContainsKey(skill.id))
                {
                    skillInfo.Add(skill.id, skill.level);
                }
            }

            //被动技能
            for (int i = 0; i < curSkillList.Count; ++i)
            {
                int sid = curSkillList[i].id;
                int level = curSkillList[i].level;
                if (!skillInfo.ContainsKey(sid))
                {
                    skillInfo.Add(sid, level);
                }
            }

            // 这里对公会战领地占领者的技能特殊处理一下，客户端自己加一下领地会长对应的技能id，进而添加对应buff的属性计算。
            // 但是原先天空之城（3级领地）城主的属性加成机制是由服务器下发的，现在的1，2级领地的占领加成，他们不想算了让客户端自己算. by 王博 2020.04.09
            if (GuildDataManager.GetInstance().HasSelfGuild() && skillSourceType == SkillSystemSourceType.None)
            {
                int manorid = GuildDataManager.GetInstance().myGuild.nSelfManorID;
                GuildTerritoryTable Territorytable = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(manorid);

                if (Territorytable != null && PlayerBaseData.GetInstance().eGuildDuty == EGuildDuty.Leader)
                {
                    if (isPvp)
                    {
                        if (Territorytable.LeaderPvPAddSkill != 0)
                        {
                            if (!skillInfo.ContainsKey(Territorytable.LeaderPvPAddSkill))
                            {
                                skillInfo.Add(Territorytable.LeaderPvPAddSkill, 1);
                            }
                        }
                    }
                    else
                    {
                        if (Territorytable.LeaderPveAddSkill != 0)
                        {
                            if (!skillInfo.ContainsKey(Territorytable.LeaderPveAddSkill))
                            {
                                skillInfo.Add(Territorytable.LeaderPveAddSkill, 1);
                            }
                        }
                    }
                }
            }

            return skillInfo;
        }

        public uint GetSp(bool isPvp, SkillSystemSourceType skillSourceType = SkillSystemSourceType.None)
        {
            if (skillSourceType == SkillSystemSourceType.Chiji)
            {
                return 0;
            }
            else if (skillSourceType == SkillSystemSourceType.FairDuel)
            {
                return PlayerBaseData.GetInstance().FairDuelSp;
            }
            else
            {
                if (!isPvp)
                {
                    if (CurPVESKillPage == ESkillPage.Page1)
                    {
                        return PlayerBaseData.GetInstance().SP;
                    }
                    else if (CurPVESKillPage == ESkillPage.Page2)
                    {
                        return PlayerBaseData.GetInstance().SP2;
                    }
                }
                else
                {
                    if (CurPVPSKillPage == ESkillPage.Page1)
                    {
                        return PlayerBaseData.GetInstance().pvpSP;
                    }
                    else if (CurPVPSKillPage == ESkillPage.Page2)
                    {
                        return PlayerBaseData.GetInstance().pvpSP2;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 得到pvp pve 公平竞技场 吃鸡的skillList
        /// </summary>
        /// <param name="isPvp"></param>
        /// <param name="skillSourceType"></param>
        /// <returns></returns>
        public List<Skill> GetSkillList(bool isPvp, SkillSystemSourceType skillSourceType = SkillSystemSourceType.None)
        {
            List<Skill> curSkillList = null;

            if ((BattleMain.instance != null && BattleMain.IsChiji()) || skillSourceType == SkillSystemSourceType.Chiji)
            {
                curSkillList = GetChijiSkillList();
            }
            else if ((BattleMain.instance != null && BattleMain.IsFairDuel()) || skillSourceType == SkillSystemSourceType.FairDuel)
            {
                curSkillList = GetFairDuelSkillList();
            }
            else
            {
                curSkillList = isPvp ? GetPvpSkillList() : GetPveSkillList();
            }

            return curSkillList;
        }

        /// <summary>
        /// 得到pvp pve 公平竞技场 吃鸡的skillBar
        /// </summary>
        /// <param name="isPvp"></param>
        /// <param name="skillSourceType"></param>
        /// <returns></returns>
        public List<SkillBarGrid> GetSkillConfiguration(bool isPvp, SkillSystemSourceType skillSourceType = SkillSystemSourceType.None)
        {
            List<SkillBarGrid> curSkillBar = null;

            if ((BattleMain.instance != null && BattleMain.IsChiji())/*战斗内有效*/ || skillSourceType == SkillSystemSourceType.Chiji/*战斗外有效*/)
            {
                curSkillBar = GetChijiSkillBar();
            }
            else if ((BattleMain.instance != null && BattleMain.IsFairDuel())/*战斗内有效*/ || skillSourceType == SkillSystemSourceType.FairDuel/*战斗外有效*/)
            {
                curSkillBar = GetFairDuelSkillBar();
            }
            else
            {
                curSkillBar = isPvp ? GetPvpSkillBar() : GetPveSkillBar();
            }

#if MG_TEST
            if (!ClientSystemManager.GetInstance().IsFrameOpen<SkillTreeFrame>())
            {
                string ss = string.Format("体验服城镇技能数据带入战斗,isPvp={0},BattleMain.IsChiji={1},BattleMain.IsFairDuel={2},skillSourceType={3}", isPvp, BattleMain.IsChiji(), BattleMain.IsFairDuel(), skillSourceType);

                ExceptionManager.GetInstance().RecordLog(ss);
                GameStatisticManager.GetInstance().UploadLocalLogToServer(ss);
            }
#endif
            return curSkillBar;
        }
        #endregion

        #region new skill frame data 这些接口内部参数与UI有关
        public SkillConfigType GetCurType()
        {
            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
            {
                if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                {
                    return SkillConfigType.SKILL_CONFIG_PVE;
                }
                else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
                {
                    return SkillConfigType.SKILL_CONFIG_PVP;
                }
            }
            else if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                return SkillConfigType.SKILL_CONFIG_EQUAL_PVP;
            }
            return SkillConfigType.SKILL_CONFIG_INVALID;
        }

        public uint GetCurSp()
        {
            uint CurSp = 0;

            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
            {
                if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                {
                    CurSp = GetSp(false);
                }
                else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
                {
                    CurSp = GetSp(true);
                }
            }
            else if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                CurSp = GetSp(true, SkillSystemSourceType.FairDuel);
            }

            return CurSp;
        }

        public Skill GetCurSkillInfoById(int SkillId)
        {
            Skill skillInfo = null;

            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
            {
                if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                {
                    skillInfo = GetSkillInfoById(SkillId, false);
                }
                else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
                {
                    skillInfo = GetSkillInfoById(SkillId, true);
                }
            }
            else if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                skillInfo = GetSkillInfoById(SkillId, true, SkillSystemSourceType.FairDuel);
            }

            return skillInfo;
        }

        public SkillBarGrid GetCurSlotInfoBySkillId(UInt16 SkillId)
        {
            List<SkillBarGrid> SkillConfiguration = null;

            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
            {
                if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                {
                    SkillConfiguration = GetSkillConfiguration(false);

                    if(SkillConfiguration != null)
                    {
                        return SkillConfiguration.Find(s => { return s.id == SkillId; });
                    }
                }
                else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
                {
                    SkillConfiguration = GetSkillConfiguration(true);

                    if(SkillConfiguration != null)
                    {
                        return SkillConfiguration.Find(s => { return s.id == SkillId; });
                    }
                }
            }
            else if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                SkillConfiguration = GetSkillConfiguration(true, SkillSystemSourceType.FairDuel);

                if(SkillConfiguration != null)
                {
                    return SkillConfiguration.Find(s => { return s.id == SkillId; });
                }
            }

            return null;
        }

        public SkillBarGrid GetCurSlotInfoBySlot(int slot)
        {
            List<SkillBarGrid> SkillConfiguration = null;

            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
            {
                if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                {
                    SkillConfiguration = GetSkillConfiguration(false);

                    if(SkillConfiguration != null)
                    {
                        return SkillConfiguration.Find(s => { return s.slot == slot; });
                    }
                }
                else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
                {
                    SkillConfiguration = GetSkillConfiguration(true);

                    if(SkillConfiguration != null )
                    {
                        return SkillConfiguration.Find(s => { return s.slot == slot; });
                    }
                }
            }
            else if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                SkillConfiguration = GetSkillConfiguration(true, SkillSystemSourceType.FairDuel);

                if(SkillConfiguration != null)
                {
                    return SkillConfiguration.Find(s => { return s.slot == slot; });
                }
            }

            return null;
        }

        public int GetLearnedSkillLv(SkillTable tableData)
        {
            int level = 0;

            if (tableData == null)
            {
                return level;
            }
            //公平竞技场不考虑等级
            if (PlayerBaseData.GetInstance().Level < tableData.LevelLimit && tableData.IsPreJob == 0 && SkillFrame.frameParam.frameType != SkillFrameType.FairDuel)
            {
                return level;
            }

            Skill skillinfo = GetCurSkillInfoById(tableData.ID);

            if (skillinfo == null)
            {
                return level;
            }

            level = skillinfo.level;

            return level;
        }

        public bool CheckDrag(int iSkillID)
        {
            SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(iSkillID);
            if (skillData == null)
            {
                Logger.LogError(string.Format("技能表没有ID为 {0} 的条目", iSkillID));
                return false;
            }

            Skill skillInfo = GetCurSkillInfoById(iSkillID);

            if (skillInfo == null || (skillInfo != null && skillInfo.level <= 0))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("未学习技能无法拖拽");
                return false;
            }

            if (skillData.SkillType == SkillTable.eSkillType.PASSIVE || skillData.IsQTE != 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("被动技能无法配置到技能栏");
                return false;
            }

            if (skillData.IsBuff == 1)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("Buff技能无法配置到技能栏");
                return false;
            }

            if (skillData.SkillCategory == 4)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("觉醒技能有固定栏位，不需要配置到技能栏");
                return false;
            }

            if (skillData.IsPreJob == 1)
            {
                JobTable JobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
                if (JobData != null && JobData.JobType == 0)
                {
                    SystemNotifyManager.SystemNotify(800006);
                    return false;
                }
            }

            if (PlayerBaseData.GetInstance().Level < skillData.LevelLimit && skillData.IsPreJob == 0 && SkillFrame.frameParam.frameType != SkillFrameType.FairDuel)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("角色等级不足");
                return false;
            }

            // 添加判断该技能是否在pvp中被禁用
            if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP || SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                if (skillData.CanUseInPVP == 3)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("skill_pvpforbid"));
                    return false;
                }
            }

            return true;
        }

        // 第3个参数是可以为null的，前两个参数一定是不为null的，在调用接口之前就判断好
        public bool CheckDrop(int iDragSkillID, Drag_Me dragImg, Drop_Me receiveImg, Drag_Me receiveImgDragMe)
        {
            SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(iDragSkillID);
            if (skillData == null)
            {
                return false;
            }

            // 丢弃技能
            if (dragImg.DragGroup == EDragGroup.SkillConfigGroup && receiveImg.bIsDropDelete)
            {
                return true;
            }
            else
            {
                if (receiveImgDragMe != null && receiveImgDragMe.DragGroup == EDragGroup.SkillConfigGroup)
                {
                    if (receiveImgDragMe.GroupIndex > SkillConfigurationFrame.UnLockSkillSlotNum)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("该技能槽位尚未解锁");
                        return false;
                    }
                }
            }

            return true;
        }

        public bool DealDeleteDrop(PointerEventData DragData, GameObject ReceiveImgObj)
        {
            GameObject DragObj = DragData.pointerPress;

            if(DragObj == null || ReceiveImgObj == null)
            {
                return false;
            }

            Drag_Me mDrag = DragObj.GetComponent<Drag_Me>();
            if (mDrag == null)
            {
                return false;
            }

            Drop_Me mReceive = ReceiveImgObj.GetComponent<Drop_Me>();
            if(mReceive == null)
            {
                return false; 
            }

            if (mDrag.DragGroup != EDragGroup.SkillConfigGroup)
            {
                return false;
            }

            SkillBarGrid slotinfo = GetCurSlotInfoBySlot(mDrag.GroupIndex);

            if (slotinfo == null || (slotinfo != null && slotinfo.id <= 0))
            {
                return false;
            }

            if (!CheckDrop(slotinfo.id, mDrag, mReceive, null))
            {
                return false;
            }

            // 发送协议
            SkillBarGrid DeleteSkill = new SkillBarGrid();

            DeleteSkill.slot = slotinfo.slot;
            DeleteSkill.id = 0;

            return SendChangeSlotSkillReq(DeleteSkill);
        }

        public bool CheckLvUp(int LearnedLevel, SkillTable tableData, bool ShowNotify = true)
        {
            if (tableData == null)
            {
                return false;
            }

            // 角色等级是否足够初始升级 公平竞技场无视等级
            if (PlayerBaseData.GetInstance().Level < tableData.LevelLimit && SkillFrame.frameParam.frameType != SkillFrameType.FairDuel)
            {
                if (ShowNotify)
                {
                    SystemNotifyManager.SystemNotify(1008);
                }
                return false;
            }

            // 添加判断该技能是否在pvp中被禁用
            if ((SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP || SkillFrame.frameParam.frameType == SkillFrameType.FairDuel) && tableData.CanUseInPVP == 3)
            {
                if (ShowNotify)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("skill_pvpforbid"));
                }
                return false;
            }

            // 当前角色等级下是否可以升级
            if (!CanLvUpByCurRoleLv(tableData, (byte)LearnedLevel) && SkillFrame.frameParam.frameType != SkillFrameType.FairDuel)
            {
                if (ShowNotify)
                {
                    SystemNotifyManager.SystemNotify(1006);
                }

                return false;
            }

            // 技能等级是否达到上限
            if (LearnedLevel >= (byte)tableData.TopLevelLimit)
            {
                if (ShowNotify)
                {
                    SystemNotifyManager.SystemNotify(1016);
                }

                return false;
            }

            // 判断是否需要前置技能及其等级是否足够
            if (LearnedLevel == 0 && tableData.PreSkills.Count > 0 && tableData.PreSkills[0] > 0)
            {
                if (!_CheckPreSkills(tableData, ShowNotify))
                {
                    return false;
                }
            }

            // sp是否足够
            uint CurSp = GetCurSp();

            if (CurSp < tableData.LearnSPCost)
            {
                if (ShowNotify)
                {
                    SystemNotifyManager.SystemNotify(1005);
                }

                return false;
            }

            if (tableData.SkillCategory == 4)
            {
                if (PlayerBaseData.GetInstance().AwakeState <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckLvDown(int LearnedLevel, SkillTable tableData, bool ShowNotify = true)
        {
            if (tableData == null)
            {
                return false;
            }

            // 是否已经降到初始等级
            int MinLv = 0;

            bool bIsInitSkill = CheckInitSkills(tableData.ID);
            if (bIsInitSkill || tableData.IsPreJob == 1)
            {
                MinLv = 1;
            }

            if (LearnedLevel <= MinLv)
            {
                if (ShowNotify)
                {
                    SystemNotifyManager.SystemNotify(1017);
                }

                return false;
            }

            // 如果该技能已经在技能槽里配置了，那么禁止该技能降到0级
            SkillBarGrid slotinfo = GetCurSlotInfoBySkillId((UInt16)tableData.ID);
            if (slotinfo != null && LearnedLevel <= 1)
            {
                if (ShowNotify)
                {
                    SystemNotifyManager.SystemNotify(1018);
                }

                return false;
            }

            // 后置技能是否满足
            if (tableData.PostSkills.Count > 0 && tableData.PostSkills[0] > 0)
            {
                if (!_CheckPostSkills((byte)LearnedLevel, tableData, ShowNotify))
                {
                    return false;
                }
            }

            return true;
        }

        private bool _CheckPreSkills(SkillTable skillData, bool ShowNotify)
        {
            if (skillData.PreSkills.Count != skillData.PreSkillsLevel.Count)
            {
                Logger.LogError(string.Format("技能表 {0} 的前置技能与等级数组长度不等，请检查表格", skillData.ID));
                return false;
            }

            for (int i = 0; i < skillData.PreSkills.Count; i++)
            {
                var preskill = TableManager.GetInstance().GetTableItem<SkillTable>(skillData.PreSkills[i]);
                if (preskill == null)
                {
                    Logger.LogError(string.Format("技能表没有ID为 {0} 的条目", skillData.PreSkills[i]));
                    return false;
                }

                if (GetLearnedSkillLv(preskill) < (byte)skillData.PreSkillsLevel[i])
                {
                    object[] args = new object[2];

                    args[0] = preskill.Name;
                    args[1] = skillData.PreSkillsLevel[i];

                    if (ShowNotify)
                    {
                        SystemNotifyManager.SystemNotify(1019, args);
                    }

                    return false;
                }
            }

            return true;
        }

        private bool _CheckPostSkills(byte CurSelSkillLv, SkillTable skillData, bool ShowNotify)
        {
            if (skillData.PostSkills.Count != skillData.NeedLevel.Count)
            {
                Logger.LogError(string.Format("技能表 {0} 的后置技能与所需等级数组长度不等，请检查表格", skillData.ID));
                return false;
            }

            for (int i = 0; i < skillData.NeedLevel.Count; i++)
            {
                if (CurSelSkillLv > skillData.NeedLevel[i])
                {
                    continue;
                }

                var postskill = TableManager.GetInstance().GetTableItem<SkillTable>(skillData.PostSkills[i]);
                if (postskill == null)
                {
                    Logger.LogError(string.Format("技能表没有ID为 {0} 的条目", skillData.PostSkills[i]));
                    return false;
                }

                int MinLv = 0;

                bool bIsInitSkill = CheckInitSkills(postskill.ID);
                if (bIsInitSkill)
                {
                    MinLv = 1;
                }

                if (GetLearnedSkillLv(postskill) > MinLv)
                {
                    object[] args = new object[1];
                    args[0] = postskill.Name;

                    if (ShowNotify)
                    {
                        SystemNotifyManager.SystemNotify(1020, args);
                    }

                    return false;
                }
            }

            return true;
        }

        public bool SendChangeSlotSkillReq(SkillBarGrid targetSlotNewSkill)
        {
            List<SkillBarGrid> skillExchangedList = _GetChangedSlotList(targetSlotNewSkill);

            if (skillExchangedList == null || skillExchangedList.Count <= 0)
            {
                return false;
            }

            SceneExchangeSkillBarReq req = new SceneExchangeSkillBarReq();
            req.skillBars.bar = new SkillBar[1];
            req.skillBars.bar[0] = new SkillBar();

            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
            {
                if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                {
                    req.configType = (byte)SkillConfigType.SKILL_CONFIG_PVE;
                    req.skillBars.index = (byte)(CurPVESKillPage + 1);
                    req.skillBars.bar[0].index = req.skillBars.index;
                }
                else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
                {
                    req.configType = (byte)SkillConfigType.SKILL_CONFIG_PVP;
                    req.skillBars.index = (byte)(CurPVPSKillPage + 1);
                    req.skillBars.bar[0].index = req.skillBars.index;
                }
            }
            else if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
            {
                req.configType = (byte)SkillConfigType.SKILL_CONFIG_EQUAL_PVP;
                req.skillBars.index = 1;
                req.skillBars.bar[0].index = 1;
            }

            req.skillBars.bar[0].grids = new SkillBarGrid[skillExchangedList.Count];

            for (int i = 0; i < skillExchangedList.Count; i++)
            {
                req.skillBars.bar[0].grids[i] = new SkillBarGrid();

                req.skillBars.bar[0].grids[i].slot = skillExchangedList[i].slot;
                req.skillBars.bar[0].grids[i].id = skillExchangedList[i].id;
            }

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

            return true;
        }

        private List<SkillBarGrid> _GetChangedSlotList(SkillBarGrid targetSlotNewSkill)
        {
            List<SkillBarGrid> ExchangedList = new List<SkillBarGrid>();

            SkillBarGrid targetSlotPreSkill = null;
            SkillBarGrid originalSkillSlotInfo = null;

            // 获取到该slot位置上原有的技能数据（有可能有技能，也有可能没有技能）
            targetSlotPreSkill = GetCurSlotInfoBySlot(targetSlotNewSkill.slot);
            originalSkillSlotInfo = GetCurSlotInfoBySkillId(targetSlotNewSkill.id);

            // 自己替换自己不做处理
            if (originalSkillSlotInfo != null && targetSlotPreSkill != null && originalSkillSlotInfo.id == targetSlotPreSkill.id)
            {
                return ExchangedList;
            }

            if (originalSkillSlotInfo != null)
            {
                ExchangedList.Add(targetSlotNewSkill);

                SkillBarGrid ExchangedSkill = new SkillBarGrid();

                if (targetSlotPreSkill != null)
                {
                    if (originalSkillSlotInfo.id != targetSlotPreSkill.id)
                    {
                        ExchangedSkill.slot = originalSkillSlotInfo.slot;
                        ExchangedSkill.id = targetSlotPreSkill.id;
                    }
                }
                else
                {
                    ExchangedSkill.slot = originalSkillSlotInfo.slot;
                    ExchangedSkill.id = 0;
                }

                ExchangedList.Add(ExchangedSkill);
            }
            else
            {
                ExchangedList.Add(targetSlotNewSkill);
            }

            return ExchangedList;
        }
        #endregion

        public bool IsSkillJobAdaptToTargetJob(SkillTable skillData, int TargetJobId)
        {
            if (skillData == null || skillData.JobID == null || skillData.JobID.Count < 1)
            {
                return false;
            }

            // 技能的职业适配规则与装备的职业适配规则不一样，基础职业的装备，进阶职业也可以用.但是技能表里填了什么职业就只给什么职业用
            // 进阶职业也是可以使用基础职业的技能，机制本身是没有限制的,需要把基础职业的id也配到该技能表的适配职业列表里.填表不能只填一个基础职业的id
            if (skillData.JobID[0] == -1) // 适配所有职业
            {
                return true;
            }
            else if(skillData.JobID[0] == 0) // 非角色人物技能，比如宠物技能，怪物技能
            {
                return false;
            }
            else
            {
                if(BattleMain.battleType != BattleType.NewbieGuide)
                {
                    // 基础职业的技能不向下传递给进阶职业，所以只填基础职业id就是只给基础职业用的
                    bool bfind = false;
                    for (int i = 0; i < skillData.JobID.Count; i++)
                    {
                        if (skillData.JobID[i] == TargetJobId && TargetJobId != 0)
                        {
                            bfind = true;
                            break;
                        }
                    }

                    if(bfind)
                    {
                        return true;
                    }
                    else
                    {
                        Logger.LogErrorFormat(string.Format("技能与目标职业不匹配，技能id={0},技能名称={1}，职业id={2},", skillData.ID,skillData.Name, TargetJobId));
                    }
                }
                else
                {
                    // 第一场战斗需求比较特殊，需要把转职后职业的大招加进去,
                    // 所以需要判断该技能所属职业是否是传进来职业的进阶职业，这话有点拗口，但就是这样
                    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(TargetJobId);
                    if (jobData != null && jobData.JobType == 0)
                    {
                        for (int i = 0; i < jobData.ToJob.Count; i++)
                        {
                            bool bfind = false;

                            for (int j = 0; j < skillData.JobID.Count; j++)
                            {
                                if (jobData.ToJob[i] == skillData.JobID[j])
                                {
                                    bfind = true;
                                    break;
                                }
                            }

                            if (bfind)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        public List<int> GetBuffSkillID(bool isPvp)
        {
            List<int> mBuffList = new List<int>();

            List<Skill> curSkillList = GetSkillList(isPvp);
            for (int i = 0; i < curSkillList.Count; ++i)
            {
                int sid = curSkillList[i].id;
                var data = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(sid);
                if (data != null && data.IsBuff > 0 && sid > 0)
                {
                    mBuffList.Add(sid);
                }
            }
            return mBuffList;
        }

        public int GetQTESkillID(bool isPvp)
        {
            List<Skill> curSkillList = GetSkillList(isPvp);
            for (int i = 0; i < curSkillList.Count; ++i)
            {
                int sid = curSkillList[i].id;
                var data = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(sid);
                if (data != null && data.IsQTE > 0 && sid > 0)
                {
                    return sid;
                }
            }

            return 0;
        }

        public int GetRunAttackSkillID(bool isPvp)
        {
            List<Skill> curSkillList = GetSkillList(isPvp);
           
            for (int i = 0; i < curSkillList.Count; ++i)
            {
                int sid = curSkillList[i].id;
                var data = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(sid);
                if (data != null && data.IsRunAttack > 0 && sid > 0)
                {
                    return sid;
                }
            }

            return 0;
        }

        public bool IsSkillBarFull(SkillConfigType skillType)
        {
            List<SkillBarGrid> tempSkillBar = new List<SkillBarGrid>();

            switch (skillType)
            {
                case SkillConfigType.SKILL_CONFIG_PVE:
                    tempSkillBar = GetPveSkillBar();                 
                    break;
                case SkillConfigType.SKILL_CONFIG_PVP:
                    tempSkillBar = GetPvpSkillBar();      
                    break;
            }

            int level = PlayerBaseData.GetInstance().Level;
            ExpTable expdata = null;

            if (level != UnLockTaskLvl)
            {
                expdata = TableManager.GetInstance().GetTableItem<ExpTable>(level);
            }
            else
            {
                if(!IsFinishUnlockTask)
                {
                    level--;
                }

                expdata = TableManager.GetInstance().GetTableItem<ExpTable>(level);
            }
          
            if (expdata == null)
            {
                return true;
            }

            bool isFull = true;

            int UnLockNum = expdata.SkillNum;
            for(int i = 1; i <= UnLockNum; i++)
            {
                SkillBarGrid grid = tempSkillBar.Find(s => { return s.slot == i; });
                if(grid != null && grid.id > 0)
                {
                    continue;
                }

                isFull = false;
            }

            return isFull;
        }

        public void InitSkillData(int iLevel)
        {
            // 获取当前职业可学技能列表
            Dictionary<int, int> skilllist = TableManager.GetInstance().GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);

            var skilllistemu = skilllist.GetEnumerator();
            while (skilllistemu.MoveNext())
            {
                int skillID = skilllistemu.Current.Key;

                var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillID);

                if (skillData == null)
                {
                    Logger.LogError(string.Format("技能表没有ID为 {0} 的条目", skillID));
                    return;
                }

                // 过滤掉普攻
                if (skillData.SkillCategory == 1)
                {
                    continue;
                }

                // 未解锁技能
                if (iLevel < skillData.LevelLimit)
                {
                    LockSkillList.Add(skillID);
                }
            }

            // 初始解锁技能
            var JobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (JobData != null && JobData.InitSkills.Count > 0 && JobData.InitSkills[0] != 0)
            {
                InitSkills = new List<int>(JobData.InitSkills);
            }

            _UpdateNewSkillList();
        }

        void _UpdateNewSkillList()
        {
            NewOpenSkillList.Clear();

            for (int i = 0; i < LockSkillList.Count; i++)
            {
                var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(LockSkillList[i]);

                if (skillData == null)
                {
                    continue;
                }

                if (skillData.LevelLimit > PlayerBaseData.GetInstance().Level)
                {
                    continue;
                }

                if(skillData.LevelLimit <= LastSeeSkillLv)
                {
                    continue;
                }

                if(skillData.SkillCategory >= 2 && skillData.SkillCategory <= 3)
                {
                    NewOpenSkillList.Add(LockSkillList[i]);
                    NewOpenSkillAllForBattle.Add(LockSkillList[i]);
                }
            }
        }

        public bool IsSkillNewForBattle(int id)
        {
            if(NewOpenSkillAllForBattle.Contains(id))
            {
                NewOpenSkillAllForBattle.Remove(id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckInitSkills(int SkillID)
        {
            for (int i = 0; i < InitSkills.Count; i++)
            {
                if (InitSkills[i] == SkillID)
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateSkillLevelAddInfo(bool bUpdateAll = true, bool bIsPve = true)
        {
            var equips = PlayerBaseData.GetInstance().GetEquipedEquipments();

            if(bUpdateAll)
            {
                AddSkillInfo = GetSkillLevelAddInfo(true, equips);
                pvpAddSkillInfo = GetSkillLevelAddInfo(false, equips);
            }
            else
            {
                if(bIsPve)
                {
                    AddSkillInfo = GetSkillLevelAddInfo(true, equips);
                }
                else
                {
                    pvpAddSkillInfo = GetSkillLevelAddInfo(false, equips);
                }
            }
        }

        public Dictionary<int, SkillLevelAddInfo> GetSkillLevelAddInfo(bool isPVE, List<ItemProperty> equips = null, SkillSystemSourceType skillSourceType = SkillSystemSourceType.None)
        {
            Dictionary<int, SkillLevelAddInfo> info = new Dictionary<int, SkillLevelAddInfo>();

            //装备
            //var equips = PlayerBaseData.GetInstance().GetEquipedEquipments();
            if (equips == null)
                equips = PlayerBaseData.GetInstance().GetEquipedEquipments();

            // SkillSystemSourceType仅用于战斗外给技能等级加成的计算，不用于战斗内，战斗内都是用单局类型判断
            // 宝珠对PVE，PVP都有效，所以技能等级加成的显示没有问题
            // 附魔卡对PVE，PVP都有效，但是目前没有对技能等级加成的附魔卡，所以技能等级加成的显示也没有问题
            // 铭文只对PVE 有效，并且还有对技能等级加成，所以PVP的技能等级加成不能算进去
            // 公平竞技场所有的加成规则与PVP保持一致，但加成等级的来源是以给公平竞技场所配置的装备（但是装备数据只有进入战斗的时候，服务器才会给到）来计算的
            for (int i = 0; i < equips.Count; ++i)
            {
                if(isPVE)
                {
                    ExtractSkillLevelAddInfo(equips[i].attachBuffIDs, info, SkillLevelAddType.EQUIP);
                }
                else
                {
                    ExtractSkillLevelAddInfo(equips[i].attachPVPBuffIDs, info, SkillLevelAddType.EQUIP);
                }
            }

            bool isPvp = !isPVE;

            //其他技能
            BeActor actor = BeUtility.GetMainPlayerActor(isPvp, equips, skillSourceType);
            //Dictionary<int, CrypticInt32> skillLevelInfo = BeEntityData.GetSkillLevelInfo(PlayerBaseData.GetInstance().Level, GetSkillInfo(isPvp), PlayerBaseData.GetInstance().JobTableID);

            Dictionary<int, CrypticInt32> skillLevelInfo = actor.GetEntityData().skillLevelInfo;
            var dict = skillLevelInfo.GetEnumerator();

            while (dict.MoveNext())
            {
                int skillID = dict.Current.Key;

                Skill skill = new Skill();

                if (isPVE)
                {
                    if (CurPVESKillPage == ESkillPage.Page1)//区分是pve哪个技能页
                    {
                        skill = skillList.Find(x => { return x.id == skillID; });
                    }
                    else
                    {
                        skill = skillList2.Find(x => { return x.id == skillID; });
                    }
                }
                else
                {
                    // 公平竞技场要各自用对应的数据
                    if (skillSourceType == SkillSystemSourceType.FairDuel)
                    {
                        skill = FairDuelSkillList.Find(x =>{ return x.id == skillID; });
                    }
                    else
                    {
                        if (CurPVPSKillPage == ESkillPage.Page1)//区分是pvp哪个技能页
                        {
                            skill = pvpSkillList.Find(x =>{ return x.id == skillID; });
                        }
                        else
                        {
                            skill = pvpSkillList2.Find(x =>{ return x.id == skillID; });
                        }
                    }
                }

                if (skill != null)
                {
                    int totalAddLevel = dict.Current.Value - skill.level;

                    if (!info.ContainsKey(skillID))
                    {
                        info.Add(skillID, new SkillLevelAddInfo());
                    }

                    var slAddinfo = info[skillID];
                    slAddinfo.totalAddLevel = totalAddLevel;
                    var currentTotalNum = slAddinfo.GetCurrentTotalAddNum();
                    if (currentTotalNum < totalAddLevel)
                    {
                        slAddinfo.items.Add(new SkillLevelAddItem(SkillLevelAddType.SKILL, totalAddLevel - currentTotalNum));
                    }
                }
            }

            return info;
        }
      
        public int GetAddedSkillLevel(int iSkillID, bool ispve)
        {
            SkillLevelAddInfo kAddSkillLevelInfo = null;

            if (ispve)
            {
                if (AddSkillInfo != null && AddSkillInfo.TryGetValue(iSkillID, out kAddSkillLevelInfo))
                {
                    return kAddSkillLevelInfo.totalAddLevel;
                }
            }
            else
            {
                if (pvpAddSkillInfo != null && pvpAddSkillInfo.TryGetValue(iSkillID, out kAddSkillLevelInfo))
                {
                    return kAddSkillLevelInfo.totalAddLevel;
                }
            }

            return 0;
        }

        public SkillLevelAddInfo GetAddedSkillInfo(int iSkillID, Dictionary<int, SkillLevelAddInfo> AddSkillLevelInfo)
        {
            if (AddSkillLevelInfo == null)
            {
                return null;
            }

            SkillLevelAddInfo kAddSkillLevelInfo = null;
            if (!AddSkillLevelInfo.TryGetValue(iSkillID, out kAddSkillLevelInfo))
            {
                return null;
            }

            return kAddSkillLevelInfo;
        }

        private void ExtractSkillLevelAddInfo(List<BuffInfoData> buffInfos, Dictionary<int, SkillLevelAddInfo> info, SkillLevelAddType type)
        {
            for (int i = 0; i < buffInfos.Count; ++i)
            {
                var buffInfo = buffInfos[i];
                if (buffInfo != null && buffInfo.skillIDs != null && buffInfo.skillIDs.Count > 0 && buffInfo.skillIDs[0] > 0)
                {
                    for (int j = 0; j < buffInfo.skillIDs.Count; ++j)
                    {
                        int skillID = buffInfo.skillIDs[j];
                        int addLevel = 0;
                        var buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffInfo.buffID);
                        if (buffData != null)
                        {
                            addLevel = TableManager.GetValueFromUnionCell(buffData.level, buffInfo.level);
                        }
                        if (addLevel > 0)
                            FillSkill(skillID, addLevel, info, type);
                    }
                }
            }
        }

        private void ExtractSkillLevelAddInfo(IList<int> buffInfoIDs, Dictionary<int, SkillLevelAddInfo> info, SkillLevelAddType type)
        {
            List<BuffInfoData> buffInfos = new List<BuffInfoData>();
            for (int i = 0; i < buffInfoIDs.Count; ++i)
            {
                BuffInfoData buffInfo = new BuffInfoData(buffInfoIDs[i], 1);
                buffInfos.Add(buffInfo);
                if (buffInfo.data != null)
                {
                    if (buffInfo.data.RelatedSkillLV.Length == 1)
                    {
                        int lv = buffInfo.data.RelatedSkillLV[0];
                        if (lv != 0)
                        {
                           // var iter = skillList.GetEnumerator();
                            var iter = GetPveSkillList().GetEnumerator();
                            while (iter.MoveNext())
                            {
                                var skill = iter.Current;
                                if (skill != null)
                                {
                                    var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skill.id);
                                    if (skillData != null && skillData.LevelLimit == lv && !buffInfo.skillIDs.Contains(iter.Current.id))
                                    {
                                        buffInfo.skillIDs.Add(iter.Current.id);
                                    }
                                }
                            }
                        }
                    }
                    else if (buffInfo.data.RelatedSkillLV.Length == 2)
                    {
                        int lowLv = buffInfo.data.RelatedSkillLV[0];
                        int highLv = buffInfo.data.RelatedSkillLV[1];
                        if (lowLv > 0 && lowLv < highLv)
                        {
                           // var iter = skillList.GetEnumerator();
                            var iter = GetPveSkillList().GetEnumerator();
                            while (iter.MoveNext())
                            {
                                var skill = iter.Current;
                                if (skill != null)
                                {
                                    var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(skill.id);
                                    if (skillData != null && skillData.LevelLimit >= lowLv && skillData.LevelLimit <= highLv && !buffInfo.skillIDs.Contains(iter.Current.id))
                                    {
                                        buffInfo.skillIDs.Add(iter.Current.id);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ExtractSkillLevelAddInfo(buffInfos, info, type);
        }

        private void FillSkill(int skillID, int addLevel, Dictionary<int, SkillLevelAddInfo> info, SkillLevelAddType type, bool merge = true)
        {
            List<Skill> tmpSkill = GetPveSkillList();
            var skill = tmpSkill.Find(x => {
                return x.id == skillID;
            });

            if (skill != null)
            {
                if (!info.ContainsKey(skillID))
                {
                    info.Add(skillID, new SkillLevelAddInfo());
                }

                if (!merge)
                {
                    var slAddInfo = info[skillID];
                    slAddInfo.items.Add(new SkillLevelAddItem(type, addLevel));
                }
                else {
                    var slAddInfo = info[skillID];
                    var item = slAddInfo.items.Find(x => {
                        return x.type == type;
                    });

                    if (item != null)
                    {
                        item.addLevel += addLevel;
                    }
                    else {
                        info[skillID].items.Add(new SkillLevelAddItem(type, addLevel));
                    }
                }

            }
        }

        public string UpdateSkillDescription(int skillID, byte AlreadyLearnLv, byte NeedShowLv, SkillFrameTabType TypeLabel = SkillFrameTabType.PVE)
        {
            string description = "";

            if (NeedShowLv <= 0)
            {
                return TR.Value("NotLearnSkillDes");
            }

            var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillID);
            if (skillData == null)
            {
                return description;
            }

            if (NeedShowLv > (byte)skillData.TopLevel)
            {
                return TR.Value("MaxLvSkillDes");
            }

            List<string> desList = GetSkillDesList(skillID, NeedShowLv, TypeLabel);

            for (int i = 0; i < desList.Count; i++)
            {
                string[] StrArray = desList[i].Split(new char[] { ':' });

                if (StrArray.Length < 2)
                {
                    continue;
                }

                string RichTextstr = StrArray[0] + "+<color=";

                if (AlreadyLearnLv < NeedShowLv)
                {
                    RichTextstr += "#686868ff>";
                }
                else if (AlreadyLearnLv == NeedShowLv)
                {
                    RichTextstr += "#C6C1B3FF>";
                }
                else
                {
                    RichTextstr += "#CF3838FF>";
                }

                if (i == desList.Count - 1)
                {
                    RichTextstr += StrArray[1] + "</color>";
                }
                else
                {
                    RichTextstr += StrArray[1] + "</color>\n";
                }

                description += RichTextstr;
            }  

            return description;
        }

        public string UpdatePetSkillDescription(int skillID, byte AlreadyLearnLv, byte NeedShowLv, SkillFrameTabType TypeLabel = SkillFrameTabType.PVE)
        {
            string description = "";

            if (NeedShowLv <= 0)
            {
                return TR.Value("NotLearnSkillDes");
            }

            var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillID);
            if (skillData == null)
            {
                return description;
            }

            if (NeedShowLv > (byte)skillData.TopLevel)
            {
                return TR.Value("MaxLvSkillDes");
            }

            List<string> desList = GetPetSkillDesList(skillID, NeedShowLv, TypeLabel);

            for (int i = 0; i < desList.Count; i++)
            {
                string[] StrArray = desList[i].Split(new char[] { ':' });

                if (StrArray.Length < 2)
                {
                    //string Des = StrArray[0];
                    //description += Des + "\n";
                    continue;
                }
                else
                {
                    string RichTextstr = StrArray[0] + ":<color=";

                    if (AlreadyLearnLv < NeedShowLv)
                    {
                        RichTextstr += "#1BE224FF>";
                    }
                    else if (AlreadyLearnLv == NeedShowLv)
                    {
                        RichTextstr += "#CFCFCFFF>";
                    }
                    else
                    {
                        RichTextstr += "#CFCFCFFF>";
                    }

                    if (i == desList.Count - 1)
                    {
                        RichTextstr += StrArray[1] + "</color>";
                    }
                    else
                    {
                        RichTextstr += StrArray[1] + "</color>\n";
                    }

                    description += RichTextstr;
                }
            }


            return description;
        }

        public List<string> GetPetSkillDesList(int skillid, byte skillShowLv, SkillFrameTabType TypeLabel = SkillFrameTabType.PVE)
        {
            List<string> SkillDesList = new List<string>();

            var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillid);
            if (skillData == null)
            {
                return SkillDesList;
            }

            var SkillDescription = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(skillid);
            if (SkillDescription == null)
            {
                return SkillDesList;
            }

            PropertyInfo[] fieldInfos = SkillDescription.GetType().GetProperties((BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty));

            string StrValue = "";
            int index = 1;
            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                string DescriptionName = string.Format("DataText{0}", index);
                string DataName1 = string.Format("DataNumber{0}", index);
                string Description = string.Format("Description");

                if (TypeLabel == SkillFrameTabType.PVP)
                {
                    DataName1 = string.Format("PVPDataNum{0}", index);
                }

                if (Description == fieldInfos[i].Name)
                {
                    if (fieldInfos[i].GetValue(SkillDescription, null) as string == "0")
                    {
                        break;
                    }
                    StrValue = fieldInfos[i].GetValue(SkillDescription, null) as string;
                    SkillDesList.Add(StrValue);
                }

                if (DescriptionName == fieldInfos[i].Name)
                {
                    if (fieldInfos[i].GetValue(SkillDescription, null) as string == "0")
                    {
                        break;
                    }

                    StrValue = fieldInfos[i].GetValue(SkillDescription, null) as string;
                }

                if (DataName1 == fieldInfos[i].Name)
                {
                    string Data = fieldInfos[i].GetValue(SkillDescription, null) as string;

                    string[] FirstParamArray = Data.Split(new char[] { ';' });

                    if (FirstParamArray.Length > 0 && (FirstParamArray[0] == "-" || FirstParamArray[0] == ""))
                    {
                        continue;
                    }

                    object[] args = new object[FirstParamArray.Length];
                    for (int j = 0; j < FirstParamArray.Length; j++)
                    {
                        string[] SecParamArray = FirstParamArray[j].Split(new char[] { ',' });

                        if (SecParamArray.Length == 1)
                        {
                            args[j] = SecParamArray[0];
                        }
                        else if (SecParamArray.Length == 2)
                        {
                            float fParam = float.Parse(SecParamArray[0]) + float.Parse(SecParamArray[1]) * (skillShowLv - 1);

                            args[j] = Utility.GetStringByFloat(fParam);
                        }
                        else
                        {
                            if (skillShowLv > SecParamArray.Length)
                            {
                                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("技能描述表技能id = {0} 所填数据长度与技能极限等级长度不匹配，打策划", skillid));
                                break;
                            }

                            for (int k = 0; k < SecParamArray.Length; k++)
                            {
                                if (k == (skillShowLv - 1))
                                {
                                    args[j] = SecParamArray[k];
                                    break;
                                }
                            }
                        }
                    }

                    try
                    {
                        StrValue = string.Format(StrValue, args);
                    }
                    catch (Exception e)
                    {
                        Logger.LogErrorFormat("[技能描述表string.Format参数长度不一致,让策划检查表格], skillid = {0}, skillShowLv = {1}, e = {2}", skillid, skillShowLv, e.ToString());
                    }

                    SkillDesList.Add(StrValue);

                    index++;
                }
            }

            return SkillDesList;
        }

        public List<string> GetSkillDesList(int skillid, byte skillShowLv, SkillFrameTabType TypeLabel = SkillFrameTabType.PVE)
        {
            List<string> SkillDesList = new List<string>();

            var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillid);
            if (skillData == null)
            {
                return SkillDesList;
            }

            var SkillDescription = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(skillid);
            if (SkillDescription == null)
            {
                return SkillDesList;
            }

            PropertyInfo[] fieldInfos = SkillDescription.GetType().GetProperties((BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty));

            string StrValue = "";
            int index = 1;
            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                string DescriptionName = string.Format("DataText{0}", index);
                string DataName1 = string.Format("DataNumber{0}", index);

                if (TypeLabel == SkillFrameTabType.PVP)
                {
                    DataName1 = string.Format("PVPDataNum{0}", index);
                }

                if (DescriptionName == fieldInfos[i].Name)
                {
                    if (fieldInfos[i].GetValue(SkillDescription, null) as string == "0")
                    {
                        break;
                    }

                    StrValue = fieldInfos[i].GetValue(SkillDescription, null) as string;
                }

                if (DataName1 == fieldInfos[i].Name)
                {
                    string Data = fieldInfos[i].GetValue(SkillDescription, null) as string;

                    string[] FirstParamArray = Data.Split(new char[] { ';' });

                    if(FirstParamArray.Length > 0 && (FirstParamArray[0] == "-"  || FirstParamArray[0] == ""))
                    {
                        continue;
                    }

                    object[] args = new object[FirstParamArray.Length];
                    for (int j = 0; j < FirstParamArray.Length; j++)
                    {
                        string[] SecParamArray = FirstParamArray[j].Split(new char[] { ',' });

                        if (SecParamArray.Length == 1)
                        {
                            args[j] = SecParamArray[0];
                        }
                        else if (SecParamArray.Length == 2)
                        {
                            float fParam = float.Parse(SecParamArray[0]) + float.Parse(SecParamArray[1]) * (skillShowLv - 1);

                            args[j] = Utility.GetStringByFloat(fParam);
                        }
                        else
                        {
                            if (skillShowLv > SecParamArray.Length)
                            {
                                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("技能描述表技能id = {0} 所填数据长度与技能极限等级长度不匹配，打策划", skillid));
                                break;
                            }

                            for (int k = 0; k < SecParamArray.Length; k++)
                            {
                                if (k == (skillShowLv - 1))
                                {
                                    args[j] = SecParamArray[k];
                                    break;
                                }
                            }
                        }
                    }

                    try
                    {
                        StrValue = string.Format(StrValue, args);
                    }
                    catch (Exception e)
                    {
                        Logger.LogErrorFormat("[技能描述表string.Format参数长度不一致,让策划检查表格], skillid = {0}, StrValue = {1}, args.Length = {2}, skillShowLv = {3}, skillData.TopLevel = {4}, e = {5}", skillid, StrValue, args.Length, skillShowLv, skillData.TopLevel, e.ToString());
                    }

                    SkillDesList.Add(StrValue);

                    index++;
                }
            }

            return SkillDesList;
        }
        public List<float> GetSkillDataList(int skillid, byte skillShowLv, SkillFrameTabType TypeLabel = SkillFrameTabType.PVE)
        {
            List<float> SkillDataList = new List<float>();

            var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(skillid);
            if (skillData == null)
            {
                return SkillDataList;
            }

            var SkillDescription = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(skillid);
            if (SkillDescription == null)
            {
                return SkillDataList;
            }

            PropertyInfo[] fieldInfos = SkillDescription.GetType().GetProperties((BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty));

            int index = 1;
            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                string DataName1 = string.Format("DataNumber{0}", index);

                if (TypeLabel == SkillFrameTabType.PVP)
                {
                    DataName1 = string.Format("PVPDataNum{0}", index);
                }

                if (DataName1 == fieldInfos[i].Name)
                {
                    string Data = fieldInfos[i].GetValue(SkillDescription, null) as string;

                    if (Data == "0")
                    {
                        continue;
                    }

                    string[] FirstParamArray = Data.Split(new char[] { ';' });
                    string[] args = new string[FirstParamArray.Length];

                    if (FirstParamArray.Length > 0 && (FirstParamArray[0] == "-" || FirstParamArray[0] == ""))
                    {
                        continue;
                    }

                    for (int j = 0; j < FirstParamArray.Length; j++)
                    {
                        string[] SecParamArray = FirstParamArray[j].Split(new char[] { ',' });

                        if (SecParamArray.Length == 1)
                        {
                            args[j] = SecParamArray[0];
                        }
                        else if (SecParamArray.Length == 2)
                        {
                            float fParam = float.Parse(SecParamArray[0]) + float.Parse(SecParamArray[1]) * (skillShowLv - 1);

                            args[j] = Utility.GetStringByFloat(fParam);
                        }
                        else
                        {
                            if (skillShowLv > SecParamArray.Length)
                            {
                                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format("技能描述表技能id = {0} 所填数据长度与技能极限等级长度不匹配，打策划", skillid));
                                break;
                            }

                            for (int k = 0; k < SecParamArray.Length; k++)
                            {
                                if (k == (skillShowLv - 1))
                                {
                                    args[j] = SecParamArray[k];
                                    break;
                                }
                            }
                        }
                    }

                    SkillDataList.Add(float.Parse(args[0]));

                    index++;
                }
            }

            return SkillDataList;
        }

        public string GetSkillDes(int skillid, byte skillShowLv, SkillFrameTabType TypeLabel = SkillFrameTabType.PVE)
        {
            string str = "";

            var SkillDescription = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(skillid);
            if (SkillDescription == null)
            {
                return str;
            }

            List<float> dataList = GetSkillDataList(skillid, skillShowLv, TypeLabel);
            if (dataList.Count > 0)
            {
                object[] arrParams = new object[dataList.Count];
                for (int i = 0; i < dataList.Count; ++i)
                {
                    arrParams[i] = dataList[i];
                }
                str = string.Format(SkillDescription.Description, arrParams);
            }

            return str;
        }

        public bool CheckSkillLvUp(int SkillID)
        {
            var skillData = TableManager.GetInstance().GetTableItem<SkillTable>(SkillID);
            if (skillData == null)
            {
                return false;
            }

            // 筛选掉普攻
            if (skillData.SkillCategory == 1)
            {
                return false;
            }

            // 角色等级是否足够初始升级
            if (PlayerBaseData.GetInstance().Level < skillData.LevelLimit)
            {
                return false;
            }

            int iSkillLv = CalSkillLvFromOutside(skillData);

            // 当前角色等级下是否可以升级
            if (!CanLvUpByCurRoleLv(skillData, (byte)iSkillLv))
            {
                return false;
            }

            // 技能等级是否达到上限
            if (iSkillLv >= (byte)skillData.TopLevelLimit)
            {
                return false;
            }

            // 判断是否需要前置技能及其等级是否足够
            if (iSkillLv == 0 && skillData.PreSkills.Count > 0 && skillData.PreSkills[0] > 0)
            {
                if (!CheckPreSkillsFromOutside(skillData))
                {
                    return false;
                }
            }
            // sp是否足够
            uint sp = 0;
            if (CurPVESKillPage == ESkillPage.Page1)
            {
                sp = PlayerBaseData.GetInstance().SP;
            }
            else if (CurPVESKillPage == ESkillPage.Page2)
            {
                sp = PlayerBaseData.GetInstance().SP2;
            }
          
            if (sp < skillData.LearnSPCost)
            {
                return false;
            }

            return true;
        }

        public bool CheckPreSkillsFromOutside(SkillTable skillData)
        {
            if (skillData.PreSkills.Count != skillData.PreSkillsLevel.Count)
            {
                Logger.LogError(string.Format("技能表 {0} 的前置技能与等级数组长度不等，请检查表格", skillData.ID));
                return false;
            }

            for (int i = 0; i < skillData.PreSkills.Count; i++)
            {
                var preskill = TableManager.GetInstance().GetTableItem<SkillTable>(skillData.PreSkills[i]);
                if (preskill == null)
                {
                    Logger.LogError(string.Format("技能表没有ID为 {0} 的条目", skillData.PreSkills[i]));
                    return false;
                }

                if (CalSkillLvFromOutside(skillData) < (byte)skillData.PreSkillsLevel[i])
                {
                    return false;
                }
            }

            return true;
        }

        public int CalSkillLvFromOutside(SkillTable skillData)
        {
            int SkillLv = 0;
            List<Skill> tmpList = GetPveSkillList();
           
            for (int i = 0; i < tmpList.Count; i++)
            {
                if (skillData.ID == tmpList[i].id)
                {
                    return tmpList[i].level;
                }
            }

            return SkillLv;
        }

        public bool CanLvUpByCurRoleLv(SkillTable skilldata, byte SkillCurLv)
        {
            bool bFlag = false;

            if(skilldata.TopLevelLimit == 1 && SkillCurLv >= 1)
            {
                return false;
            }

            if(skilldata.TopLevelLimit > 1 && skilldata.LevelLimitAmend <= 0)
            {
                return false;
            }

            // 10000以下是人物技能，不然技能类型配置都一样（宠物技能，城主雕像buff技能等都应该被过滤掉），无法区分某些非人物技能，具体含义看技能表
            if (skilldata.ID >= 10000)
            {
                return false;
            }

            UInt16 CurRoleLv = PlayerBaseData.GetInstance().Level;

            if (skilldata.LevelLimit + skilldata.LevelLimitAmend * SkillCurLv <= CurRoleLv)
            {
                bFlag = true;
            }

            return bFlag;
        }

        public bool HasSkillLvCanUp()
        {
            if (!Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Skill))
                return false;
            Dictionary<int, int> skilllist = TableManager.GetInstance().GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);

            var emu = skilllist.GetEnumerator();

            while (emu.MoveNext())
            {
                int iSkillID = emu.Current.Key;

                if (CheckSkillLvUp(iSkillID))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasNewSkillorSkillCanUp()
        {
            Dictionary<int, int> skilllist = TableManager.GetInstance().GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);
            var emu = skilllist.GetEnumerator();

            while (emu.MoveNext())
            {
                var skillTable = TableManager.GetInstance().GetTableItem<SkillTable>(emu.Current.Key);

                if (skillTable != null)
                {
                    // 筛选掉普攻
                    if (skillTable.SkillCategory == 1)
                    {
                        continue;
                    }

                    if (skillTable.LevelLimit == PlayerBaseData.GetInstance().Level)
                    {
                        return true;
                    }
                    else if (skillTable.LevelLimit < PlayerBaseData.GetInstance().Level)
                    {
                        if (CheckSkillLvUp(skillTable.ID))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public int GetSkillNextOpenNeedRoleLv(SkillTable skilldata, int iCurSkillLv)
        {
            return (skilldata.LevelLimit + skilldata.LevelLimitAmend * iCurSkillLv);
        }

        public string GetSkillDescription(SkillTable skillData)
        {
            var skillDesData = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(skillData.ID);
            if (skillDesData == null)
            {
                Logger.LogError(string.Format("技能描述表没有ID为 {0} 的条目", skillData.ID));
                return "";
            }

            return skillDesData.Description;
        }

        public string GetSkillType(SkillTable skillData)
        {
            var skillDesData = TableManager.GetInstance().GetTableItem<SkillDescriptionTable>(skillData.ID);
            if (skillDesData == null)
            {
                Logger.LogError(string.Format("技能描述表没有ID为 {0} 的条目", skillData.ID));
                return "";
            }

            if (skillData.IsBuff == 1)
            {
                return "[Buff技能]";
            }
            else if (skillData.IsQTE != 0)
            {
                return "[被动技能]";
            }
            else if (skillData.SkillType == ProtoTable.SkillTable.eSkillType.ACTIVE)
            {
                return "[主动技能]";
            }
            else if (skillData.SkillType == ProtoTable.SkillTable.eSkillType.PASSIVE)
            {
                return "[被动技能]";
            }
            else
            {
                return "";
            }
        }

#region SkillInitConfig

        //主界面上是否显示技能按钮
        public bool IsShowSkillButton()
        {
            if (PlayerBaseData.GetInstance().Level >= ShowSkillButtonLevel)
                return true;

            return false;
        }

        private void BindEvents()
        {
            NetProcess.AddMsgHandler(SceneInitSkillsRes.MsgID, OnReceiveSceneInitSkillsRes);
            NetProcess.AddMsgHandler(SceneRecommendSkillsRes.MsgID, OnReceiveSceneRecommendSkillsRes);
            NetProcess.AddMsgHandler(SceneBattleChangeSkillsRes.MsgID, OnChijiLearnSkillRes); // 吃鸡相关
            NetProcess.AddMsgHandler(SceneSetEqualPvpSkillConfigRes.MsgID, _OnReciveSetEqualPvpSkillConfigRes);
            NetProcess.AddMsgHandler(SceneSkillSlotUnlockNotify.MsgID, _OnReceiveSkillSlotUnLockNotify);

            NetProcess.AddMsgHandler(SceneSetSkillPageRes.MsgID, _OnReceiveSetSkillPageRes);
            NetProcess.AddMsgHandler(SceneBuySkillPageRes.MsgID, _OnReceiveBuySkillPageRes);
            NetProcess.AddMsgHandler(SceneSetTalentRes.MsgID, _OnSceneSetTalentRes);
            
        }

        private void UnBindEvents()
        {
            NetProcess.RemoveMsgHandler(SceneInitSkillsRes.MsgID, OnReceiveSceneInitSkillsRes);
            NetProcess.RemoveMsgHandler(SceneRecommendSkillsRes.MsgID, OnReceiveSceneRecommendSkillsRes);
            NetProcess.RemoveMsgHandler(SceneBattleChangeSkillsRes.MsgID, OnChijiLearnSkillRes);
            NetProcess.RemoveMsgHandler(SceneSetEqualPvpSkillConfigRes.MsgID, _OnReciveSetEqualPvpSkillConfigRes);
            NetProcess.RemoveMsgHandler(SceneSkillSlotUnlockNotify.MsgID, _OnReceiveSkillSlotUnLockNotify);

            NetProcess.RemoveMsgHandler(SceneSetSkillPageRes.MsgID, _OnReceiveSetSkillPageRes);
            NetProcess.RemoveMsgHandler(SceneBuySkillPageRes.MsgID, _OnReceiveBuySkillPageRes);
            NetProcess.RemoveMsgHandler(SceneSetTalentRes.MsgID, _OnSceneSetTalentRes);
        }

#region 公平竞技场相关
        /// <summary>
        /// 查询是否设置了公平竞技场的技能栏
        /// </summary>
        /// <param name="isSetedEqualPvPConfig">0是查询 1是第一次设置</param>
        public void SendSetSkillConfigReq(byte isSetedEqualPvPConfig)
        {
            SceneSetEqualPvpSkillConfigReq req = new SceneSetEqualPvpSkillConfigReq();
            req.isSetedEqualPvPConfig = isSetedEqualPvPConfig;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }
        private void _OnReciveSetEqualPvpSkillConfigRes(MsgDATA data)
        {
            SceneSetEqualPvpSkillConfigRes ret = new SceneSetEqualPvpSkillConfigRes();
            ret.decode(data.bytes);
            if (ret.result == 0)//0代表没有设置
            {
                IsHaveSetFairDueSkillBar = false;
                //弹出是否设置技能栏的提示
                var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
                {
                    ContentLabel = TR.Value("fairduel_setskillBar_content"),
                    IsShowNotify = false,
                    LeftButtonText = TR.Value("fairduel_setskillBar_cancel"),
                    RightButtonText = TR.Value("fairduel_setskillBar_ok"),
                    OnRightButtonClickCallBack = OnOpenFairSkillFrame,
                };
                SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
            }
            else//其他代表已经设置
            {
                IsHaveSetFairDueSkillBar = true;
            }
        }

        private void OnOpenFairSkillFrame()
        {
            SendSetSkillConfigReq(1);
            SkillFrameParam frameParam = new SkillFrameParam();
            frameParam.frameType = SkillFrameType.FairDuel;

            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle, frameParam);
        }
#endregion
        public void OnSendSceneInitSkillsReq(SkillConfigType configType)
        {
            var sceneInitSkillsReq = new SceneInitSkillsReq();
            sceneInitSkillsReq.configType = (UInt32)configType;
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneInitSkillsReq);
        }
        //根据当前页面初始化技能配置
        public void OnSendSceneInitSkillsReqByCurType()
        {
            if (GetCurType() == SkillConfigType.SKILL_CONFIG_INVALID)
            {
                Logger.LogError("初始化技能失败 当前类型为空");
                return;
            }
            var sceneInitSkillsReq = new SceneInitSkillsReq();
            sceneInitSkillsReq.configType = (uint)GetCurType();
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneInitSkillsReq);
        }

        private void OnReceiveSceneInitSkillsRes(MsgDATA msgData)
        {
            SceneInitSkillsRes sceneInitSkillsRes = new SceneInitSkillsRes();
            sceneInitSkillsRes.decode(msgData.bytes);

            if ((ProtoErrorCode) sceneInitSkillsRes.result == ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("skill_new_initialize_succeed"));
            }
            else
            {
                SystemNotifyManager.SystemNotify((int) sceneInitSkillsRes.result);
            }
        }

        public void OnSendSceneRecommendSkillsReq(SkillConfigType configType)
        {
            var sceneRecommendSkillsReq = new SceneRecommendSkillsReq();
            sceneRecommendSkillsReq.configType = (UInt32)configType;
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneRecommendSkillsReq);
        }

        //根据当前的页面设置推荐配置界面
        public void OnSendRecommendSkillsReqByCurType()
        {
            if (GetCurType() == SkillConfigType.SKILL_CONFIG_INVALID)
            {
                Logger.LogError("设置推荐配置 当前类型为空");
                return;
            }
            var sceneRecommendSkillsReq = new SceneRecommendSkillsReq();
            sceneRecommendSkillsReq.configType = (uint)GetCurType();
            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, sceneRecommendSkillsReq);
        }

        private void OnReceiveSceneRecommendSkillsRes(MsgDATA msgData)
        {
            SceneRecommendSkillsRes sceneRecommendSkillsRes = new SceneRecommendSkillsRes();
            sceneRecommendSkillsRes.decode(msgData.bytes);

            if ((ProtoErrorCode) sceneRecommendSkillsRes.result == ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("skill_new_recommend_config_succeed"));
            }
            else
            {
                SystemNotifyManager.SystemNotify((int) sceneRecommendSkillsRes.result);
            }
        }

        private void OnChijiLearnSkillRes(MsgDATA msgData)
        {
            SceneBattleChangeSkillsRes res = new SceneBattleChangeSkillsRes();
            res.decode(msgData.bytes);

            if ((ProtoErrorCode)res.result != ProtoErrorCode.SUCCESS)
            {
                Logger.LogErrorFormat("[SceneBattleChangeSkillsRes] error code = {0}", res.result);
                return;
            }
        }

        //设置技能天赋
        public void OnSendSetCurskillTalentReq(int skillId, int talentId)
        {
            SceneSetTalentReq req = new SceneSetTalentReq();
            req.configType = (byte)GetCurType();
            req.skillId = (ushort)skillId;
            req.talentId = (ushort)talentId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        //设置天赋返回
        private void _OnSceneSetTalentRes(MsgDATA msg)
        {
            SceneSetTalentRes res = new SceneSetTalentRes();
            res.decode(msg.bytes);
            if (0 != res.result)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
            }
        }

        public bool IsShowSkillTreeFrameTipBySkillConfig(Action onEnterGame)
        {
            //不再显示配置
            if (IsNotShowSkillConfigTip == true)
                return false;

            //不配置
            if (SkillDataManager.GetInstance().IsExistSkillCanConfig() == false)
                return false;

            //显示提示
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("skill_new_skill_point_enough"),
                IsShowNotify = true,
                OnCommonMsgBoxToggleClick = OnUpdateSkillConfigTip,
                LeftButtonText = TR.Value("skill_new_enter_dungeon"),
                OnLeftButtonClickCallBack = onEnterGame,
                RightButtonText = TR.Value("skill_new_enter_skill_config"),
                OnRightButtonClickCallBack = OnShowTreeFrame,
            };

            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);

            return true;
        }

        //是否显示技能配置：在进入地下城时判断
        public bool IsExistSkillCanConfig()
        {
            //技能点 小于 系数 * 等级，不显示
            var systemValueTable = TableManager.GetInstance()
                .GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DUNGEONS_SKILLPOINTLIMIT_NUM);
            if (systemValueTable != null)
            {
                var value = systemValueTable.Value * PlayerBaseData.GetInstance().Level;
                uint sp = 0;
                if(CurPVESKillPage==ESkillPage.Page1)
                {
                    sp = PlayerBaseData.GetInstance().SP;
                }else if(CurPVESKillPage==ESkillPage.Page2)
                {
                    sp = PlayerBaseData.GetInstance().SP2;
                }
                if (sp < value)
                    return false;
            }

            //不存在可以升级的技能：不显示
            if (HasSkillLvCanUp() == false)
                return false;

            return true;
        }

        private void OnShowTreeFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<SkillFrame>())
                ClientSystemManager.GetInstance().CloseFrame<SkillFrame>();
            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle);
        }

        private void OnUpdateSkillConfigTip(bool value)
        {
            IsNotShowSkillConfigTip = value;
        }

        /// <summary>
        /// 得到该职业的主动的觉醒技能
        /// </summary>
        /// <returns></returns>
        public SkillTable GetActiveAwakeSkillData()
        {
            SkillTable data = null;
            Dictionary<int, int> skillDic = TableManager.GetInstance().GetSkillInfoByPid(PlayerBaseData.GetInstance().JobTableID);

            if (skillDic != null)
            {
                var iter = skillDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    SkillTable skillData = TableManager.GetInstance().GetTableItem<SkillTable>(iter.Current.Key);

                    if (skillData == null) 
                    {
                        continue;
                    }

                    if(skillData.SkillType == SkillTable.eSkillType.ACTIVE && skillData.SkillCategory == 4)//是主动的觉醒技能
                    {
                        for (int i = 0; i < skillData.JobID.Length; i++)
                        {
                            if(skillData.JobID[i]== PlayerBaseData.GetInstance().JobTableID)
                            {
                                data = skillData;
                            }
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// 通知是否完成60级的技能解锁任务
        /// </summary>
        private void _OnReceiveSkillSlotUnLockNotify(MsgDATA data)
        {
            IsFinishUnlockTask = true;
        }

#endregion

        /// <summary>
        /// 选择技能页
        /// </summary>
        public void SendChooseSkillPage(SkillConfigType config,byte page)
        {
            SceneSetSkillPageReq sceneSetSkillPageReq = new SceneSetSkillPageReq();
            sceneSetSkillPageReq.configType = (byte)config;
            sceneSetSkillPageReq.page = page;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, sceneSetSkillPageReq);
        }

        /// <summary>
        /// 收到选择技能页的回复
        /// </summary>
        /// <param name="obj"></param>
        private void _OnReceiveSetSkillPageRes(MsgDATA obj)
        {
            SceneSetSkillPageRes sceneSetSkillPageRes = new SceneSetSkillPageRes();
            sceneSetSkillPageRes.decode(obj.bytes);
            if ((ProtoErrorCode)sceneSetSkillPageRes.result == ProtoErrorCode.SUCCESS)
            {
                if (sceneSetSkillPageRes.configType == (byte)SkillConfigType.SKILL_CONFIG_PVE)
                {
                    if (sceneSetSkillPageRes.page == (byte)ESkillPage.Page1)
                    {
                        CurPVESKillPage = ESkillPage.Page1;
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Change_SKillConfig1"));
                    }
                    else
                    {
                        CurPVESKillPage = ESkillPage.Page2;
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Change_SKillConfig2"));
                    }
                }
                else
                {
                    if (sceneSetSkillPageRes.page == (byte)ESkillPage.Page1)
                    {
                        CurPVPSKillPage = ESkillPage.Page1;
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Change_SKillConfig1"));
                    }
                    else
                    {
                        CurPVPSKillPage = ESkillPage.Page2;
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Change_SKillConfig2"));
                    }
                }
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSelectSkillPage);
            }
        }


        /// <summary>
        /// 购买解锁技能页
        /// </summary>
        /// <param name="page"></param>
        public void SendBuySkillPageReq(SkillConfigType configType , byte page)
        {
            SceneBuySkillPageReq sceneBuySkillPageReq = new SceneBuySkillPageReq();
            sceneBuySkillPageReq.page = page;
            sceneBuySkillPageReq.configType =(byte)configType;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, sceneBuySkillPageReq);
        }
        public void SendBuyCurSkillPageReq(byte page)
        {
            if (GetCurType() == SkillConfigType.SKILL_CONFIG_INVALID)
            {
                Logger.LogError("解锁技能页 当前类型为空");
                return;
            }
            SceneBuySkillPageReq sceneBuySkillPageReq = new SceneBuySkillPageReq();
            sceneBuySkillPageReq.page = page;
            sceneBuySkillPageReq.configType = (byte)GetCurType();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, sceneBuySkillPageReq);
        }

        /// <summary>
        /// 购买技能页的返回
        /// </summary>
        /// <param name="obj"></param>
        private void _OnReceiveBuySkillPageRes(MsgDATA obj)
        {
            SceneBuySkillPageRes sceneBuySkillPageRes = new SceneBuySkillPageRes();
            sceneBuySkillPageRes.decode(obj.bytes);
            if((ProtoErrorCode)sceneBuySkillPageRes.result==ProtoErrorCode.SUCCESS)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BuySkillPage2Sucess);
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("CnaNotBugSkillPage_Tip"));
            }
        }
    }
}
