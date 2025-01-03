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
    class GuildManorInfoFrame : ClientFrame
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

        [UIObject("DayReward/ScrollView/Viewport/Content")]
        GameObject m_objDayRewardRoot;
        [UIObject("DayReward/ScrollView/Viewport/Content/Template")]
        GameObject m_objDayRewardTemplate;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildManorInfo";
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
            m_objDayRewardTemplate.SetActive(false);

            GuildTerritoryTable tableData = _GetTerritoryTableData(m_guildTerritoryBaseInfo.terrId);
            Assert.IsNotNull(tableData);

            m_labTitle.text = tableData.Name;
            m_labSignupManorName.text = tableData.Name;
            m_labSignupSignUpCount.text = m_guildTerritoryBaseInfo.enrollSize.ToString();    

            GuildTerritoryTable NeedOccupyTerritory = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(tableData.NeedTerritoryId);

            mLevelRoot.CustomActive(NeedOccupyTerritory == null);
            mSignUpConditionRoot.CustomActive(NeedOccupyTerritory != null);

            mLevel.text = string.Format("{0}级", tableData.Level);

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
                Utility.GetComponetInChild<Text>(obj, "Name").text = itemData.Name;
            }

            for (int i = 0; i < tableData.LeaderDayReward.Count; ++i)
            {
                string[] values = tableData.LeaderDayReward[i].Split('_');
                if (values.Length < 2)
                {
                    continue;
                }
                GameObject obj = GameObject.Instantiate(m_objDayRewardTemplate);
                obj.transform.SetParent(m_objDayRewardRoot.transform, false);
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
                Utility.GetComponetInChild<Text>(obj, "Name").text = itemData.Name;
            }
            for (int i = 0; i < tableData.DayReward.Count; ++i)
            {
                string[] values = tableData.DayReward[i].Split('_');

                if (values.Length < 2)
                {
                    continue;
                }

                GameObject obj = GameObject.Instantiate(m_objDayRewardTemplate);
                obj.transform.SetParent(m_objDayRewardRoot.transform, false);
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
                Utility.GetComponetInChild<Text>(obj, "Name").text = itemData.Name;
            }

            m_btnSignup.onClick.RemoveAllListeners();
            m_btnSignup.onClick.AddListener(() => { OnClickSignUp(m_guildTerritoryBaseInfo, tableData); });

            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL)
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
            var table = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(m_guildTerritoryBaseInfo.terrId);
            if(table != null)
            {
                icon.SafeSetImage(table.iconPath, true);
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

                if (string.IsNullOrEmpty(str))
                {
                    GuildDataManager.GetInstance().BattleSignup(data.terrId);
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("guild_manor_signup_cost", tableData.Level, str), () =>
                    {
                        GuildDataManager.GetInstance().BattleSignup(data.terrId);
                    });
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
        private GameObject mLevelRoot = null;
        private GameObject mSignUpConditionRoot = null;
        private Text mLevel = null;
        private Text mCondition = null;
        private Image icon = null;

        protected override void _bindExUI()
        {
            mLevelRoot = mBind.GetGameObject("LevelRoot");
            mSignUpConditionRoot = mBind.GetGameObject("SignUpConditionRoot");
            mLevel = mBind.GetCom<Text>("Level");
            mCondition = mBind.GetCom<Text>("Condition");
            icon = mBind.GetCom<Image>("icon");
        }

        protected override void _unbindExUI()
        {
            mLevelRoot = null;
            mSignUpConditionRoot = null;
            mLevel = null;
            mCondition = null;
            icon = null;
        }
        #endregion
    }
}
