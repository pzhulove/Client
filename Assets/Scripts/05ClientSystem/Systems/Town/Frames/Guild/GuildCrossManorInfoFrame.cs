using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using Protocol;

namespace GameClient
{
    class GuildCrossManorInfoFrame : ClientFrame
    {
        [UIControl("Title/Text")]
        Text m_labTitle;

        [UIControl("Name/Text")]
        Text m_labSignupManorName;

        [UIControl("SignupCount/Text")]
        Text m_labSignupSignUpCount;

        [UIObject("Reward/ScrollView/Viewport/Content")]
        GameObject m_objRewardRoot;

        [UIObject("Reward/ScrollView/Viewport/Content/Template")]
        GameObject m_objRewardTemplate;

        [UIControl("SignUp", typeof(Button))]
        Button m_btnSignup;

        [UIControl("SignUp", typeof(ComButtonEnbale))]
        ComButtonEnbale m_comBtnEnableSignup;

        [UIControl("SignUp/Text")]
        Text m_labSignup;

        GuildTerritoryBaseInfo m_guildTerritoryBaseInfo = new GuildTerritoryBaseInfo();

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildCrossManorInfo";
        }

        protected sealed override void _OnOpenFrame()
        {
            m_guildTerritoryBaseInfo = userData as GuildTerritoryBaseInfo;
            if (m_guildTerritoryBaseInfo == null)
            {
                Logger.LogErrorFormat("打开领地详情界面错误，缺少参数");
                return;
            }

            m_objRewardTemplate.SetActive(false);
            mReward2Template.CustomActive(false);

            GuildTerritoryTable tableData = _GetTerritoryTableData(m_guildTerritoryBaseInfo.terrId);
            Assert.IsNotNull(tableData);

            m_labTitle.text = tableData.Name;
            m_labSignupManorName.text = string.Format("{0}级", tableData.Level);     
            m_labSignupSignUpCount.text = m_guildTerritoryBaseInfo.enrollSize.ToString();    

            GuildTerritoryTable NeedOccupyTerritory = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(tableData.NeedTerritoryId);

            if (NeedOccupyTerritory != null)
            {
                mCondition.text = TR.Value("guild_manor_signup_requirement_1");
            }
            else
            {
                mCondition.text = TR.Value("guild_manor_signup_requirement_2");
            }

            if(tableData.ChestDoubleDungeons != null)
            {
                string[] ChestDoubleDungeons = tableData.ChestDoubleDungeons.Split(new char[] { '|' });

                if (ChestDoubleDungeons != null && ChestDoubleDungeons.Length > 0)
                {
                    int DungeonID = 0;
                    if (int.TryParse(ChestDoubleDungeons[0], out DungeonID))
                    {
                        if (DungeonID != 0)
                        {
                            if (tableData.ID == 8)
                            {
                                mDes2.text = string.Format("每天前{0}次<color=#00BAFFFF>通关远古地下城</color>翻牌装备数量*2", tableData.DailyChestDoubleTimes);
                                mDungeon.text = "远古地下城";
                            }
                            else
                            {
                                DungeonTable dungeondata = TableManager.GetInstance().GetTableItem<DungeonTable>(DungeonID);
                                if (dungeondata != null)
                                {
                                    mDes2.text = string.Format("每天前{0}次通关<color=#00BAFFFF>{1}</color>翻牌装备数量*2", tableData.DailyChestDoubleTimes, dungeondata.Name);
                                    mDungeon.text = dungeondata.Name;
                                }
                            }

                            mGoto.gameObject.CustomActive(true);
                        }
                        else
                        {
                            mDungeon.text = "无";
                            mGoto.gameObject.CustomActive(false);
                        }
                    }
                }
                else
                {
                    mDungeon.text = "无";
                    mGoto.gameObject.CustomActive(false);
                }
            }
            else
            {
                mDungeon.text = "无";
                mGoto.gameObject.CustomActive(false);
            }

            for (int i = 0; i < tableData.LeaderReward.Count; ++i)
            {
                if (tableData.LeaderReward[i] == "-")
                {
                    continue;
                }

                string[] values = tableData.LeaderReward[i].Split('_');
                Assert.IsTrue(values.Length == 2);

                GameObject obj = GameObject.Instantiate(m_objRewardTemplate);
                obj.transform.SetParent(m_objRewardRoot.transform, false);
                obj.SetActive(true);
                GameObject leaderExclusive = Utility.FindGameObject(obj, "Leader");
                GameObject memberExclusive = Utility.FindGameObject(obj, "Member");
                if (leaderExclusive != null)
                {
                    leaderExclusive.CustomActive(true);
                }

                if (memberExclusive != null)
                {
                    memberExclusive.CustomActive(false);
                }

                ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                itemData.Count = int.Parse(values[1]);
                ComItem comItem = CreateComItem(Utility.FindGameObject(obj, "Icon"));
                comItem.Setup(itemData, (var1, var2) =>
                {
                    ItemTipManager.GetInstance().ShowTip(var2);
                });
                comItem.labCount.fontSize = 24;
                comItem.labLevel.fontSize = 24;
                Utility.GetComponetInChild<Text>(obj, "Name").text = itemData.Name;
            }

            for (int i = 0; i < tableData.MemberReward.Count; ++i)
            {
                if (tableData.MemberReward[i] == "-")
                {
                    continue;
                }

                string[] values = tableData.MemberReward[i].Split('_');
                Assert.IsTrue(values.Length == 2);

                GameObject obj = GameObject.Instantiate(m_objRewardTemplate);
                obj.transform.SetParent(m_objRewardRoot.transform, false);
                obj.SetActive(true);
                GameObject leaderExclusive = Utility.FindGameObject(obj, "Leader");
                GameObject memberExclusive = Utility.FindGameObject(obj, "Member");
                if (leaderExclusive != null)
                {
                    leaderExclusive.CustomActive(false);
                }

                if (memberExclusive != null)
                {
                    memberExclusive.CustomActive(true);
                }
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                itemData.Count = int.Parse(values[1]);
                ComItem comItem = CreateComItem(Utility.FindGameObject(obj, "Icon"));
                comItem.Setup(itemData, (var1, var2) =>
                {
                    ItemTipManager.GetInstance().ShowTip(var2);
                });
                comItem.labCount.fontSize = 24;
                comItem.labLevel.fontSize = 24;
                Utility.GetComponetInChild<Text>(obj, "Name").text = itemData.Name;
            }

            for (int i = 0; i < tableData.DayReward.Count; ++i)
            {
                string[] values = tableData.DayReward[i].Split('_');

                if (values.Length < 2)
                {
                    continue;
                }

                GameObject obj = GameObject.Instantiate(m_objRewardTemplate);
                obj.transform.SetParent(m_objRewardRoot.transform, false);
                obj.SetActive(true);
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                itemData.Count = int.Parse(values[1]);
                ComItem comItem = CreateComItem(Utility.FindGameObject(obj, "Icon"));
                comItem.Setup(itemData, (var1, var2) =>
                {
                    ItemTipManager.GetInstance().ShowTip(var2);
                });
                comItem.labCount.fontSize = 24;
                comItem.labLevel.fontSize = 24;
                Utility.GetComponetInChild<Text>(obj, "Name").text = itemData.Name;
            }

            for (int i = 0; i < tableData.PropRewards.Count; ++i)
            {
                string[] values = tableData.PropRewards[i].Split('_');

                if (values.Length < 2)
                {
                    continue;
                }

                GameObject obj = GameObject.Instantiate(mReward2Template);
                obj.transform.SetParent(mReward2Content.transform, false);
                obj.SetActive(true);
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                itemData.Count = int.Parse(values[1]);
                ComItem comItem = CreateComItem(Utility.FindGameObject(obj, "Icon"));
                comItem.Setup(itemData, (var1, var2) =>
                {
                    ItemTipManager.GetInstance().ShowTip(var2);
                });
                comItem.labCount.fontSize = 24;
                comItem.labLevel.fontSize = 24;
                Utility.GetComponetInChild<Text>(obj, "Name").text = itemData.Name;
            }

            m_btnSignup.onClick.RemoveAllListeners();
            m_btnSignup.onClick.AddListener(() => { OnClickSignUp(m_guildTerritoryBaseInfo, tableData); });

            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
            {
                if (GuildDataManager.GetInstance().HasTargetManor())
                {
                    m_comBtnEnableSignup.SetEnable(false);
                    m_labSignup.text = TR.Value("guild_manor_has_signup");
                }
                else
                {
                    if (GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Signup)
                    {
                        m_comBtnEnableSignup.SetEnable(true);
                        m_labSignup.text = TR.Value("guild_manor_signup");
                    }
                    else
                    {
                        m_comBtnEnableSignup.SetEnable(false);
                        m_labSignup.text = TR.Value("guild_manor_signup_not_start");
                    }
                }
            }
            else
            {
                m_comBtnEnableSignup.SetEnable(false);
                m_labSignup.text = TR.Value("guild_manor_signup_not_start");
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            m_guildTerritoryBaseInfo = null;

            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {

        }

        void _UnRegisterUIEvent()
        {

        }

        void OnClickSignUp(GuildTerritoryBaseInfo data, GuildTerritoryTable tableData)
        {
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_GUILD_CROSS_BATTLE))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("跨服公会战系统目前已关闭");
                return;
            }

            if (GuildDataManager.GetInstance().HasTargetManor())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_manor_signup_cannot_repeat"));
            }
            else
            {
                string str = string.Empty;
                for (int i = 0; i < tableData.ConsumeItem.Count; ++i)
                {
                    if (string.IsNullOrEmpty(tableData.ConsumeItem[i]))
                    {
                        continue;
                    }
                    string[] values = tableData.ConsumeItem[i].Split('_');
                    if (values.Length == 2)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(int.Parse(values[0]));
                        str += string.Format("{0}{1}", values[1], itemData.Name);
                    }
                }

                bool bCanSignUp = false;

                if (tableData.NeedTerritoryId != 0)
                {
                    if (tableData.NeedTerritoryId == GuildDataManager.GetInstance().myGuild.nSelfHistoryManorID)
                    {
                        bCanSignUp = true;
                    }
                }
                else
                {
                    if (GuildDataManager.GetInstance().myGuild.nSelfHistoryManorID != 0)
                    {
                        bCanSignUp = true;
                    }
                }

                if (bCanSignUp)
                {
                    GuildDataManager.GetInstance().BattleSignup(data.terrId);
                }
                else
                {
                    if(tableData.ID!=8)//远古王座的时候屏蔽该功能
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("guild_crossmanor_signup_cost", str), () =>
                        {
                            GuildDataManager.GetInstance().BattleSignup(data.terrId);
                        });
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(2314005);
                    }
                  
                }
            }
        }

        GuildTerritoryTable _GetTerritoryTableData(int a_nID, bool a_bShowError = true)
        {
            GuildTerritoryTable tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(a_nID);
            if (tableData == null)
            {
                Logger.LogErrorFormat("加载公会领地表错误，id{0}不存在", a_nID);
            }
            return tableData;
        }

        [UIEventHandle("Title/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }

        #region ExtraUIBind
        private GameObject mSignUpConditionRoot = null;
        private Text mCondition = null;
        private Text mDes2 = null;
        private GameObject mReward2 = null;
        private GameObject mReward2Content = null;
        private GameObject mReward2Template = null;
        private Text mDungeon = null;
        private Button mGoto = null;

        protected override void _bindExUI()
        {
            mSignUpConditionRoot = mBind.GetGameObject("SignUpConditionRoot");
            mCondition = mBind.GetCom<Text>("Condition");
            mDes2 = mBind.GetCom<Text>("Des2");
            mReward2 = mBind.GetGameObject("Reward2");
            mReward2Content = mBind.GetGameObject("Reward2Content");
            mReward2Template = mBind.GetGameObject("Reward2Template");
            mDungeon = mBind.GetCom<Text>("Dungeon");
            mGoto = mBind.GetCom<Button>("Goto");
            mGoto.onClick.AddListener(_onGotoButtonClick);
        }

        protected override void _unbindExUI()
        {
            mSignUpConditionRoot = null;
            mCondition = null;
            mDes2 = null;
            mReward2 = null;
            mReward2Content = null;
            mReward2Template = null;
            mDungeon = null;
            mGoto.onClick.RemoveListener(_onGotoButtonClick);
            mGoto = null;
        }
        #endregion

        #region Callback
        private void _onGotoButtonClick()
        {
            GuildTerritoryTable tableData = _GetTerritoryTableData(m_guildTerritoryBaseInfo.terrId);
            if(tableData == null)
            {
                //Logger.LogErrorFormat("领地id错误 : m_guildTerritoryBaseInfo.terrId = {0}", m_guildTerritoryBaseInfo.terrId);
                return;
            }

            AcquiredMethodTable linkTableData = TableManager.GetInstance().GetTableItem<AcquiredMethodTable>(tableData.LinkID);
            if(linkTableData == null)
            {
                //Logger.LogErrorFormat("链接id错误 : tableData.LinkID = {0}", tableData.LinkID);
                return;
            }

            if(m_guildTerritoryBaseInfo.terrId != 8)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCloseMainFrame);
                //ClientSystemManager.GetInstance().CloseFrame<GuildMyMainFrame>();
                frameMgr.CloseFrame(this);
            }

            ActiveManager.GetInstance().OnClickLinkInfo(linkTableData.LinkInfo);
        }
        #endregion
    }
}
