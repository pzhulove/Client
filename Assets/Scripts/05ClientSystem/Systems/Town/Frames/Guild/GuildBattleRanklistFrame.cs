using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Scripts.UI;

namespace GameClient
{
    class GuildBattleRanklistFrame : ClientFrame
    {
        [UIControl("Content/Tabs/Self", typeof(Toggle))]
        Toggle m_toggleSelf;

        [UIControl("Content/ScrollGroup/ScrollView42", typeof(ComUIListScript))]
        ComUIListScript m_comScrollRank42;

        [UIObject("Content/SelfGroup/Self42")]
        GameObject m_objSelfRank42;


        [UIControl("Content/Tabs/Guild", typeof(Toggle))]
        Toggle m_toggleGuild;

        [UIControl("Content/ScrollGroup/ScrollView41", typeof(ComUIListScript))]
        ComUIListScript m_comScrollRank41;

        [UIObject("Content/SelfGroup/Self41")]
        GameObject m_objSelfRank41;


        [UIControl("Content/Tabs/Detail", typeof(Toggle))]
        Toggle m_toggleDetail;

        [UIControl("Content/ScrollGroup/ScrollView43", typeof(ComUIListScript))]
        ComUIListScript m_comScrollRank43;

        [UIObject("Content/SelfGroup/Self43")]
        GameObject m_objSelfRank43;

        [UIControl("Content/TitleGroup/Title41/Server", typeof(Text))]
        Text m_Server;

        SortListType m_eCurrentRanklist = SortListType.SORTLIST_GUILD_BATTLE_MEMBER;
        BaseSortList m_ranklistData;
        DelayCallUnitHandle m_repeatCall;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBattleRanklist";
        }

        protected override void _OnOpenFrame()
        {
            _InitUI();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleRanklistChanged, _OnGuildBattleRanklistChanged);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleRanklistChanged, _OnGuildBattleRanklistChanged);
        }

        void _InitUI()
        {
            _InitTabs();
            _InitRanklists();
            _SetupCurrentRanklist();
        }

        void _ClearUI()
        {
            _ClearTabs();
            _ClearRankLists();
        }

        void _InitTabs()
        {
            m_toggleSelf.onValueChanged.AddListener((bool a_bChecked) =>
            {
                m_comScrollRank42.SetElementAmount(0);
                m_objSelfRank42.SetActive(false);

                if (a_bChecked)
                {
                    m_eCurrentRanklist = SortListType.SORTLIST_GUILD_BATTLE_MEMBER;
                    GuildDataManager.GetInstance().RequestRanklist(m_eCurrentRanklist);
                }
            });

            m_toggleGuild.onValueChanged.AddListener((bool a_bChecked) =>
            {
                m_comScrollRank41.SetElementAmount(0);
                m_objSelfRank41.SetActive(false);

                if (a_bChecked)
                {
                    m_eCurrentRanklist = SortListType.SORTLIST_GUILD_BATTLE_SCORE;
                    GuildDataManager.GetInstance().RequestRanklist(m_eCurrentRanklist);
                }
            });

            m_toggleDetail.onValueChanged.AddListener((bool a_bChecked) =>
            {
                m_comScrollRank43.SetElementAmount(0);
                m_objSelfRank43.SetActive(false);

                if (a_bChecked)
                {
                    m_eCurrentRanklist = SortListType.SORTLIST_GUILD_MEMBER_SCORE;
                    GuildDataManager.GetInstance().RequestRanklist(m_eCurrentRanklist);
                }
            });

            m_Server.gameObject.CustomActive(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS);
        }

        void _ClearTabs()
        {

        }

        void _InitRanklists()
        {
            m_comScrollRank42.Initialize();
            m_comScrollRank42.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0 && item.m_index < m_ranklistData.entries.Count)
                {
                    GuildBattleMemberScore data = m_ranklistData.entries[item.m_index] as GuildBattleMemberScore;
                    Utility.GetComponetInChild<Text>(item.gameObject, "Rank").text = data.ranking.ToString();
                    Utility.GetComponetInChild<Text>(item.gameObject, "Name").text = data.name;
                    Utility.GetComponetInChild<Text>(item.gameObject, "Score").text = data.score.ToString();
                    Utility.GetComponetInChild<Text>(item.gameObject, "GuildName").text = data.guildName;
                }
            };

            m_comScrollRank41.Initialize();
            m_comScrollRank41.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0 && item.m_index < m_ranklistData.entries.Count)
                {
                    GuildBattleScore data = m_ranklistData.entries[item.m_index] as GuildBattleScore;
                    Utility.GetComponetInChild<Text>(item.gameObject, "Rank").text = data.ranking.ToString();
                    Utility.GetComponetInChild<Text>(item.gameObject, "Name").text = data.name;
                    Utility.GetComponetInChild<Text>(item.gameObject, "Score").text = data.score.ToString();

                    Text[] texts = item.gameObject.GetComponentsInChildren<Text>(true);

                    for(int i = 0; i < texts.Length; i++)
                    {
                        if(texts[i].name == "Server")
                        {
                            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                            {
                                texts[i].text = data.serverName;
                            }

                            texts[i].gameObject.CustomActive(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS);

                            break;
                        }
                    }
                }
            };

            m_comScrollRank43.Initialize();
            m_comScrollRank43.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0 && item.m_index < m_ranklistData.entries.Count)
                {
                    GuildBattleMemberScore data = m_ranklistData.entries[item.m_index] as GuildBattleMemberScore;
                    Utility.GetComponetInChild<Text>(item.gameObject, "Rank").text = data.ranking.ToString();
                    Utility.GetComponetInChild<Text>(item.gameObject, "Name").text = data.name;
                    Utility.GetComponetInChild<Text>(item.gameObject, "Score").text = data.score.ToString();
                }
            };

            m_repeatCall = ClientSystemManager.GetInstance().delayCaller.RepeatCall(10000, () =>
            {
                GuildDataManager.GetInstance().RequestRanklist(m_eCurrentRanklist);
            });
        }

        void _UpdateRankLists(BaseSortList a_ranklist)
        {
            if ((SortListType)a_ranklist.type.mainType != m_eCurrentRanklist)
            {
                return;
            }

            m_ranklistData = a_ranklist;

            switch (m_ranklistData.type.mainType)
            {
                case (uint)SortListType.SORTLIST_GUILD_BATTLE_MEMBER:
                    {
                        m_comScrollRank42.SetElementAmount(m_ranklistData.entries.Count);

                        m_objSelfRank42.SetActive(true);
                        GuildBattleMemberScore selfData = m_ranklistData.selfEntry as GuildBattleMemberScore;
                        Utility.GetComponetInChild<Text>(m_objSelfRank42, "Rank").text = selfData.ranking <= 0 ? TR.Value("guild_battle_no_rank") : selfData.ranking.ToString();
                        Utility.GetComponetInChild<Text>(m_objSelfRank42, "Name").text = PlayerBaseData.GetInstance().Name;
                        Utility.GetComponetInChild<Text>(m_objSelfRank42, "Score").text = selfData.score.ToString();
                        Utility.GetComponetInChild<Text>(m_objSelfRank42, "GuildName").text = GuildDataManager.GetInstance().GetMyGuildName();
                        break;
                    }
                case (uint)SortListType.SORTLIST_GUILD_BATTLE_SCORE:
                    {
                        m_comScrollRank41.SetElementAmount(m_ranklistData.entries.Count);

                        m_objSelfRank41.SetActive(true);
                        GuildBattleScore selfData = m_ranklistData.selfEntry as GuildBattleScore;
                        Utility.GetComponetInChild<Text>(m_objSelfRank41, "Rank").text = selfData.ranking <= 0 ? TR.Value("guild_battle_no_rank") : selfData.ranking.ToString();
                        Utility.GetComponetInChild<Text>(m_objSelfRank41, "Name").text = GuildDataManager.GetInstance().GetMyGuildName();
                        Utility.GetComponetInChild<Text>(m_objSelfRank41, "Score").text = selfData.score.ToString();

                        Text[] texts = m_objSelfRank41.GetComponentsInChildren<Text>(true);

                        for (int i = 0; i < texts.Length; i++)
                        {
                            if (texts[i].name == "Server")
                            {
                                if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
                                {
                                    texts[i].text = selfData.serverName;
                                }

                                texts[i].gameObject.CustomActive(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS);

                                break;
                            }
                        }

                        break;
                    }
                case (uint)SortListType.SORTLIST_GUILD_MEMBER_SCORE:
                    {
                        m_comScrollRank43.SetElementAmount(m_ranklistData.entries.Count);

                        m_objSelfRank43.SetActive(true);
                        GuildBattleMemberScore selfData = m_ranklistData.selfEntry as GuildBattleMemberScore;
                        Utility.GetComponetInChild<Text>(m_objSelfRank43, "Rank").text = selfData.ranking <= 0 ? TR.Value("guild_battle_no_rank") : selfData.ranking.ToString();
                        Utility.GetComponetInChild<Text>(m_objSelfRank43, "Name").text = PlayerBaseData.GetInstance().Name;
                        Utility.GetComponetInChild<Text>(m_objSelfRank43, "Score").text = selfData.score.ToString();
                        break;
                    }
            }
        }

        void _ClearRankLists()
        {
            m_ranklistData = null;

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCall);
        }

        void _SetupCurrentRanklist()
        {
            m_toggleSelf.isOn = true;
        }

        void _OnGuildBattleRanklistChanged(UIEvent a_event)
        {
            _UpdateRankLists(a_event.Param1 as BaseSortList);
        }

        [UIEventHandle("Content/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
