using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using System;

namespace GameClient
{
  
    /// <summary>
    /// 打开公会成员列表界面的参数
    /// </summary>
    public class OpenGuildMemberFrameData
    {
        /// <summary>
        /// 是否是自己的公会
        /// </summary>
        public bool IsSelfGuild;
        
        /// <summary>
        /// 公会的GUID 只有打开的其他公会的成员列表的时候才填写
        /// </summary>
        public ulong GuildId;
       
        public OpenGuildMemberFrameData()
        {
            IsSelfGuild = true;
        }

        public OpenGuildMemberFrameData(ulong guildId)
        {
            IsSelfGuild = false;
            GuildId = guildId;
        }
    }
    class GuildMemberFrame : ClientFrame
    {
        [UIObject("Menu")]
        GameObject m_objMenu;

        [UIObject("ScrollView/Viewport/Content")]
        GameObject m_objMemberRoot;

        [UIObject("ScrollView/Viewport/Content/Template")]
        GameObject m_objMemberTemplate;

        [UIObject("Menu/Func")]
        GameObject m_objMenuFuncTempLate;

        [UIControl("CloseMenu", typeof(ComButtonEx))]
        ComButtonEx m_comBtnCloseMenu;

        [UIObject("BottomFuncs/RequesterList/RedPoint")]
        GameObject m_objRedPoint;

        [UIObject("BottomFuncs/RequesterList")]
        GameObject m_objRequesterList;

        [UIControl("onLineCount")]
        Text onLineCountTexte;

        ulong m_uCurrMemberID = 0;

        private Button mCloseBtn;
        private Button mDissolveGuildBtn;
        private Text mDissolveGuildTxt;
        List<GameObject> m_arrMenuFuncs = new List<GameObject>();

        private OpenGuildMemberFrameData mData = null;


        enum EColType
        {
            Job = 0,
            Name,
            Level,
            Duty,
            Contribution,
            OfflineTime,
            ActiveDegree,
            SeasonLv,

            Count,
        }

        delegate int CompareFunc(GuildMemberData a_left, GuildMemberData a_right);
        class SortInfo
        {
            public bool bAscending = true;
            public Image imgAscending = null;
            public CompareFunc delCompare = null;
        }
        List<SortInfo> m_arrSortInfos = new List<SortInfo>();
        EColType[] m_arrSortPriority =
        {
            EColType.OfflineTime,
            EColType.Duty,
            EColType.Contribution,
            EColType.Level,
            EColType.Name,
            EColType.Job,
            EColType.ActiveDegree,
            EColType.SeasonLv,
        };

        class GuildMemberInfo
        {
            public GuildMemberData data;
            public GameObject objRoot;
            public GameObject objSelect;
            public Text labJob;
            public Text labName;
            public Text labLevel;
            public Text labDuty;
            public Text labContribution;
            public Text labOffline;
            public Text labActiveDegree;
        }
        //List<GuildMemberInfo> m_arrGuildMemberInfos = new List<GuildMemberInfo>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMemberFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (userData == null)
            {
                Logger.LogError("打开公会成员列表的时候必须要传递参数");
                return;
            }
            mData = (OpenGuildMemberFrameData)userData;
            if (mData == null) return;
        
            if (mData.IsSelfGuild && GuildDataManager.GetInstance().HasSelfGuild() == false)
            {
                return;
            }

            m_objMemberTemplate.SetActive(false);
            m_objMenu.SetActive(false);
            m_objMenuFuncTempLate.SetActive(false);

            m_comBtnCloseMenu.gameObject.SetActive(false);
            m_comBtnCloseMenu.onMouseDown.RemoveAllListeners();
            m_comBtnCloseMenu.onMouseDown.AddListener((var) =>
            {
                _CloseMenu();
            });
            m_comBtnCloseMenu.onClick.RemoveAllListeners();
            m_comBtnCloseMenu.onClick.AddListener(() =>
            {
                m_comBtnCloseMenu.gameObject.SetActive(false);
            });

            _RegisterUIEvent();

            for (int i = 0; i < (int)EColType.Count; ++i)
            {
                m_arrSortInfos.Add(new SortInfo());
            }
            m_arrSortInfos[(int)EColType.Level].imgAscending = Utility.GetComponetInChild<Image>(frame, "ScrollView/Title/Level/Sort");
            m_arrSortInfos[(int)EColType.Duty].imgAscending = Utility.GetComponetInChild<Image>(frame, "ScrollView/Title/Duty/Sort");
            m_arrSortInfos[(int)EColType.Contribution].imgAscending = Utility.GetComponetInChild<Image>(frame, "ScrollView/Title/Contribution/Sort");
            m_arrSortInfos[(int)EColType.OfflineTime].imgAscending = Utility.GetComponetInChild<Image>(frame, "ScrollView/Title/OffLineTime/Sort");
            m_arrSortInfos[(int)EColType.ActiveDegree].imgAscending = Utility.GetComponetInChild<Image>(frame, "ScrollView/Title/Active/Sort");
            m_arrSortInfos[(int)EColType.SeasonLv].imgAscending = mBind.GetCom<Image>("SortSeasonLv");
            for (int i = 0; i < m_arrSortInfos.Count; ++i)
            {
                m_arrSortInfos[i].imgAscending.CustomActive(false);
            }

            #region init sort func
            m_arrSortInfos[(int)EColType.Job].delCompare = (a_left, a_right) =>
            {
                return a_left.nJobID - a_right.nJobID;
            };
            m_arrSortInfos[(int)EColType.Name].delCompare = (a_left, a_right) =>
            {
                return string.Compare(a_left.strName, a_right.strName);
            };
            m_arrSortInfos[(int)EColType.Level].delCompare = (a_left, a_right) =>
            {
                return a_left.nLevel - a_right.nLevel;
            };
            m_arrSortInfos[(int)EColType.Duty].delCompare = (a_left, a_right) =>
            {
                return a_left.eGuildDuty - a_right.eGuildDuty;
            };
            m_arrSortInfos[(int)EColType.Contribution].delCompare = (a_left, a_right) =>
            {
                return a_left.nContribution - a_right.nContribution;
            };           
            m_arrSortInfos[(int)EColType.OfflineTime].delCompare = (a_left, a_right) =>
            {
                int nResult = 0;
                if (a_left.uOffLineTime == 0)
                {
                    if (a_right.uOffLineTime > 0)
                    {
                        nResult = 1;
                    }
                    else
                    {
                        nResult = 0;
                    }
                }
                else
                {
                    if (a_right.uOffLineTime > 0)
                    {
                        nResult = (int)a_left.uOffLineTime - (int)a_right.uOffLineTime;
                    }
                    else
                    {
                        nResult = -1;
                    }
                }
                return nResult;
            };
            m_arrSortInfos[(int)EColType.ActiveDegree].delCompare = (a_left, a_right) =>
            {
                return (int)a_left.uActiveDegree - (int)a_right.uActiveDegree;
            };
			
            m_arrSortInfos[(int)EColType.SeasonLv].delCompare = (a_left, a_right) =>
            {
                return ((int)a_left.seasonLevel - (int)a_right.seasonLevel);
            };
            #endregion

            _UpdateRequesterList();
            _UpdateRedPoint();
            _UpdatePermission();
            if(mData.IsSelfGuild)
            {
                GuildDataManager.GetInstance().RequestGuildMembers();
            }
            else
            {
                GuildDataManager.GetInstance().RequestCanMergerdGuildMembers(mData.GuildId);
            }
           
        }

        protected override void _OnCloseFrame()
        {
            m_uCurrMemberID = 0;
            //m_arrGuildMemberInfos.Clear();
            m_arrSortInfos.Clear();
            _CloseMenu();
            _UnRegisterUIEvent();
            mData = null;
        }

        void _RegisterUIEvent()
        {
            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildMembersUpdated, _OnGuildMembersUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildKickMemberSuccess, _OnKickMemberSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildProcessRequesterSuccess, _OnProcessRequesterSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildChangeDutySuccess, _OnChangeDutySuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataGuildUpdated, _OnGuildDataChanged);


            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestDismissSuccess, _OnRequestDismissGuildSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestCancelDismissSuccess, _OnRequestCancelDismissGuildSuccess);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CanMergerdGuildMemberUpdate, _OnGuildMembersUpdate);
        }

        

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildMembersUpdated, _OnGuildMembersUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildKickMemberSuccess, _OnKickMemberSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildProcessRequesterSuccess, _OnProcessRequesterSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildChangeDutySuccess, _OnChangeDutySuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataGuildUpdated, _OnGuildDataChanged);


            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestDismissSuccess, _OnRequestDismissGuildSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestCancelDismissSuccess, _OnRequestCancelDismissGuildSuccess);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CanMergerdGuildMemberUpdate, _OnGuildMembersUpdate);
        }

        void _UpdateRequesterList()
        {
            if(mData.IsSelfGuild)
            {
                m_objRequesterList.CustomActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.ProcessRequester));
            }
            else
            {
                m_objRequesterList.CustomActive(false);
            }
           
        }

        void _UpdateRedPoint()
        {
            if (mData.IsSelfGuild)
            {
                m_objRedPoint.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMember));
            }
            else
            {
                m_objRedPoint.CustomActive(false);
            }
                 
        }
        void _DestroyGuildMemberInfo(ulong a_uGUID)
        {
            List<GuildMemberData> arrMembers = null;
            if (mData.IsSelfGuild)
            {
                arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
            }
            else
            {
                arrMembers = GuildDataManager.GetInstance().CanMergerdGuildMembers;
            }

            int nIndex = arrMembers.FindIndex(value => { return value.uGUID == a_uGUID; });

            if (nIndex >= 0 && nIndex < arrMembers.Count)
            {
                arrMembers.RemoveAt(nIndex);
            }
            RefreshMemberListCount();
        }

        string _GetOfflineDesc(int a_nOffline)
        {
            if (a_nOffline <= 0)
            {
                return TR.Value("guild_online");
            }
            else
            {
                int nTime = (int)TimeManager.GetInstance().GetServerTime() - a_nOffline;
                if (nTime < 1)
                {
                    nTime = 1;
                }
                int nDay = nTime / (3600 * 24);
                if (nDay > 0)
                {
                    return TR.Value("guild_offline_day", nDay);
                }
                else
                {
                    int nHour = nTime / 3600;
                    if (nHour > 0)
                    {
                        return TR.Value("guild_offline_hour", nHour);
                    }
                    else
                    {
                        int nMinute = nTime / 60;
                        if (nMinute > 0)
                        {
                            return TR.Value("guild_offline_minute", nMinute);
                        }
                        else
                        {
                            return TR.Value("guild_offline_second", nTime);
                        }
                    }
                }
            }
        }


        void _OpenMenu(GuildMemberData MemData)
        {
            if (MemData.uGUID == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }
            
            m_objMenu.SetActive(true);
            m_comBtnCloseMenu.gameObject.SetActive(true);

            {
                GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                obj.transform.SetParent(m_objMenu.transform, false);
                obj.SetActive(true);
                obj.GetComponent<Button>().onClick.AddListener(() => { _OnMenuFuncChat(MemData); });
                Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_chat");

                m_arrMenuFuncs.Add(obj);
            }

            {
                GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                obj.transform.SetParent(m_objMenu.transform, false);
                obj.SetActive(true);
                obj.GetComponent<Button>().onClick.AddListener(() => { _OnMenuFuncWatch(MemData.uGUID); });
                Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_watch");
                m_arrMenuFuncs.Add(obj);
            }

            {
                GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                obj.transform.SetParent(m_objMenu.transform, false);
                obj.SetActive(true);
                obj.GetComponent<Button>().onClick.AddListener(() => { _OnMenuFuncAddFriend(MemData.uGUID); });
                Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_add_friend");
                m_arrMenuFuncs.Add(obj);
            }

            if(mData.IsSelfGuild)
            {
                if (GuildDataManager.GetInstance().HasPermission(EGuildPermission.KickMember, MemData.eGuildDuty))
                {
                    GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                    obj.transform.SetParent(m_objMenu.transform, false);
                    obj.SetActive(true);
                    obj.GetComponent<Button>().onClick.AddListener(() => { _OnMenuFuncKickMember(MemData.uGUID); });
                    Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_kick_member");
                    m_arrMenuFuncs.Add(obj);
                }

                if (
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.SetDutyNormal, MemData.eGuildDuty) &&
                    MemData.eGuildDuty != EGuildDuty.Normal && MemData.eGuildDuty != EGuildDuty.Elite
                    )
                {
                    GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                    obj.transform.SetParent(m_objMenu.transform, false);
                    obj.SetActive(true);
                    obj.GetComponent<Button>().onClick.AddListener(() => { _OnMenuSetDuty(MemData.uGUID, EGuildDuty.Normal); });
                    Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_set_duty_normal");
                    m_arrMenuFuncs.Add(obj);
                }

                if (
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.SetDutyElder, MemData.eGuildDuty) &&
                    MemData.eGuildDuty != EGuildDuty.Elder
                    )
                {
                    GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                    obj.transform.SetParent(m_objMenu.transform, false);
                    obj.SetActive(true);
                    obj.GetComponent<Button>().onClick.AddListener(() => { _OnMenuSetDuty(MemData.uGUID, EGuildDuty.Elder); });
                    Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_set_duty_elder");
                    m_arrMenuFuncs.Add(obj);
                }

                if (
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.SetDutyAssistant, MemData.eGuildDuty) &&
                    MemData.eGuildDuty != EGuildDuty.Assistant
                    )
                {
                    GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                    obj.transform.SetParent(m_objMenu.transform, false);
                    obj.SetActive(true);
                    obj.GetComponent<Button>().onClick.AddListener(() => { _OnMenuSetDuty(MemData.uGUID, EGuildDuty.Assistant); });
                    Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_set_duty_assistant");
                    m_arrMenuFuncs.Add(obj);
                }

                if (
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.SetDutyLeader, MemData.eGuildDuty) &&
                    MemData.eGuildDuty != EGuildDuty.Leader
                    )
                {
                    GameObject obj = GameObject.Instantiate(m_objMenuFuncTempLate);
                    obj.transform.SetParent(m_objMenu.transform, false);
                    obj.SetActive(true);
                    obj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        _CloseMenu();
                        SystemNotifyManager.SystemNotify(3003, () =>
                        {
                            _OnMenuSetDuty(MemData.uGUID, EGuildDuty.Leader);
                        });
                    });
                    Utility.GetComponetInChild<Text>(obj, "Text").text = TR.Value("guild_menu_set_duty_leader");
                    m_arrMenuFuncs.Add(obj);
                }
            }
           
        }

        void _CloseMenu()
        {
            m_objMenu.SetActive(false);

            for (int i = 0; i < m_arrMenuFuncs.Count; ++i)
            {
                GameObject.Destroy(m_arrMenuFuncs[i]);
            }
            m_arrMenuFuncs.Clear();
        }

        void _OnMenuFuncChat(GuildMemberData a_memberData)
        {
            RelationData data = new RelationData();
            data.type = (byte)RelationType.RELATION_NONE;
            data.uid = a_memberData.uGUID;
            data.name = a_memberData.strName;
            data.level = (ushort)a_memberData.nLevel;
            data.isOnline = a_memberData.uOffLineTime <= 0 ? (byte)1 : (byte)0;
            data.occu = (byte)a_memberData.nJobID;
            ChatManager.GetInstance().OpenPrivateChatFrame(data);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCloseMainFrame);
        }

        void _OnMenuFuncWatch(ulong a_uGUID)
        {
            OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(a_uGUID);
            _CloseMenu();
        }

        void _OnMenuFuncAddFriend(ulong a_uGUID)
        {
            RelationDataManager.GetInstance().AddFriendByID(a_uGUID);
            _CloseMenu();
        }

        void _OnMenuFuncKickMember(ulong a_uGUID)
        {
            SystemNotifyManager.SystemNotify(3002, () =>
            {
                GuildDataManager.GetInstance().KickMember(a_uGUID);
            });
            _CloseMenu();
        }

        void _OnMenuSetDuty(ulong a_uMemberGUID, EGuildDuty a_eDuty)
        {
            if (GuildDataManager.GetInstance().IsDutyFull(a_eDuty))
            {
                GuildChangeDutyData data = new GuildChangeDutyData();
                data.uMemberGUID = a_uMemberGUID;
                data.eDuty = a_eDuty;
                frameMgr.OpenFrame<GuildChangeDutyFrame>(FrameLayer.Middle, data);
            }
            else
            {
                GuildDataManager.GetInstance().ChangeMemberDuty(a_uMemberGUID, a_eDuty, 0);
            }
            _CloseMenu();
        }

        int _CompareByColType(GuildMemberData a_left, GuildMemberData a_right, EColType a_colType, int a_nSign)
        {
            if (a_left.uGUID == PlayerBaseData.GetInstance().RoleID)
            {
                return -1;
            }
            else if (a_right.uGUID == PlayerBaseData.GetInstance().RoleID)
            {
                return 1;
            }

            int nResult = 0;
            int nIndex = (int)a_colType;
            if (nIndex >= 0 && nIndex < m_arrSortInfos.Count)
            {
                nResult = m_arrSortInfos[nIndex].delCompare(a_left, a_right);
                if (nResult == 0)
                {
                    for (int i = 0; i < m_arrSortPriority.Length; ++i)
                    {
                        int nTempIndex = (int)m_arrSortPriority[i];
                        if (nTempIndex != nIndex)
                        {
                            nResult = m_arrSortInfos[nTempIndex].delCompare(a_left, a_right);
                            if (nResult != 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return nResult * a_nSign;
        }

        void _SortMembers(EColType a_colType)
        {
            List<GuildMemberData> arrMembers = null;
            if (mData.IsSelfGuild)
            {
                arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
            }
            else
            {
                arrMembers = GuildDataManager.GetInstance().CanMergerdGuildMembers;
            }

            int nIndex = (int)a_colType;

            if (nIndex >= 0 && nIndex < m_arrSortInfos.Count)
            {
                SortInfo sortInfo = m_arrSortInfos[nIndex];
                sortInfo.bAscending = !sortInfo.bAscending;
                int nSign = sortInfo.bAscending ? 1 : -1;

                arrMembers.Sort((a_left, a_right) =>
                {
                    if (a_left.uGUID == PlayerBaseData.GetInstance().RoleID)
                    {
                        return -1;
                    }
                    else if (a_right.uGUID == PlayerBaseData.GetInstance().RoleID)
                    {
                        return 1;
                    }

                    int nResult = 0;

                    nResult = sortInfo.delCompare(a_left, a_right);
                    if (nResult == 0)
                    {
                        for (int i = 0; i < m_arrSortPriority.Length; ++i)
                        {
                            int nTempIndex = (int)m_arrSortPriority[i];
                            if (nTempIndex != nIndex)
                            {
                                nResult = m_arrSortInfos[nTempIndex].delCompare(a_left, a_right);
                                if (nResult != 0)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    return nResult * nSign;
                });
                
                for (int i = 0; i < m_arrSortInfos.Count; ++i)
                {
                    m_arrSortInfos[i].imgAscending.CustomActive(false);
                }

                m_arrSortInfos[nIndex].imgAscending.CustomActive(true);
                m_arrSortInfos[nIndex].imgAscending.transform.localRotation = m_arrSortInfos[nIndex].bAscending ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
            }
        }

        void _OnGuildMembersUpdate(UIEvent a_event)
        {
            InitMemberScrollListBind();
            List<GuildMemberData> arrMembers = null;
            if (mData.IsSelfGuild)
            {
                arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
            }
            else
            {
                arrMembers = GuildDataManager.GetInstance().CanMergerdGuildMembers;
            }
            _SortMembers(EColType.OfflineTime);
            RefreshMemberListCount();
        }

        void RefreshOnLineMembers()
        {
            List<GuildMemberData> arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
            int onLineTimeCount = 0;
            for (int i = 0; i < arrMembers.Count; ++i)
            {
                string str = _GetOfflineDesc((int)arrMembers[i].uOffLineTime);
                if (str != TR.Value("guild_online"))
                {
                    continue;
                }

                onLineTimeCount++;
            }

            onLineCountTexte.text = "在线成员：" + onLineTimeCount + "/" + arrMembers.Count;
        }

        void _OnKickMemberSuccess(UIEvent a_event)
        {
            ulong uGUID = (ulong)a_event.Param1;
            _DestroyGuildMemberInfo(uGUID);
        }

        void _OnProcessRequesterSuccess(UIEvent a_event)
        {
            ulong uGUID = (ulong)a_event.Param1;
            GuildMemberData data = GuildDataManager.GetInstance().myGuild.arrMembers.Find(value => { return value.uGUID == uGUID; });

            if(data != null)
            {
                RefreshMemberListCount();
            }
        }

        void _OnChangeDutySuccess(UIEvent a_event)
        {
  
            RefreshMemberListCount();
        }

        void _OnRedPointChanged(UIEvent a_event)
        {
            _UpdateRedPoint();
        }

        void _OnGuildDataChanged(UIEvent a_event)
        {
            Protocol.SceneObjectAttr attr = (Protocol.SceneObjectAttr)(a_event.Param1);
            if (attr == Protocol.SceneObjectAttr.SOA_GUILD_POST)
            {
                _UpdateRequesterList();
            }
        }

        private void _OnRequestDismissGuildSuccess(UIEvent uiEvent)
        {
            _UpdatePermission();
        }

        private void _OnRequestCancelDismissGuildSuccess(UIEvent uiEvent)
        {
            _UpdatePermission();
        }

        [UIEventHandle("BottomFuncs/RequesterList")]
        void _OnRequesterListClicked()
        {
            frameMgr.OpenFrame<GuildRequesterListFrame>(FrameLayer.Middle);
            RedPointDataManager.GetInstance().ClearRedPoint(ERedPoint.GuildRequester);
        }

        [UIEventHandle("ScrollView/Title/Level")]
        void _OnTitleLevelClicked()
        {
            _SortMembers(EColType.Level);
            RefreshMemberListCount();
        }


        [UIEventHandle("ScrollView/Title/Duty")]
        void _OnTitleDutyClicked()
        {
            _SortMembers(EColType.Duty);
            RefreshMemberListCount();
        }

        [UIEventHandle("ScrollView/Title/Contribution")]
        void _OnTitleContributionClicked()
        {
            _SortMembers(EColType.Contribution);
            RefreshMemberListCount();
        }

        [UIEventHandle("ScrollView/Title/OffLineTime")]
        void _OnTitleOffLineTimeClicked()
        {
            _SortMembers(EColType.OfflineTime);
            RefreshMemberListCount();
        }       
        
        [UIEventHandle("ScrollView/Title/Active")]
        void _OnTitleActiveClicked()
        {
            _SortMembers(EColType.ActiveDegree);
            RefreshMemberListCount();
        }

        #region ExtraUIBind
        private ComUIListScript mMemberList = null;
        private Button btnSortSeasonLv = null;

        protected override void _bindExUI()
        {
            mMemberList = mBind.GetCom<ComUIListScript>("MemberList");

            btnSortSeasonLv = mBind.GetCom<Button>("btnSortSeasonLv");
            btnSortSeasonLv.SafeAddOnClickListener(_OnSortSeasonLvBtnClick);

            mCloseBtn = mBind.GetCom<Button>("CloseBtn");
            mCloseBtn.SafeAddOnClickListener(_OnCloseBtnClick);

            mDissolveGuildBtn = mBind.GetCom<Button>("DissolveGuildBtn");
            mDissolveGuildBtn.SafeAddOnClickListener(_OnDissolveGuideBtnClick);

            mDissolveGuildTxt = mBind.GetCom<Text>("DissolveGuildTxt");
        }

        protected override void _unbindExUI()
        {
            mMemberList = null;

            btnSortSeasonLv.SafeRemoveOnClickListener(_OnSortSeasonLvBtnClick);
            btnSortSeasonLv = null;

            mCloseBtn.SafeRemoveOnClickListener(_OnCloseBtnClick);
            mCloseBtn = null;

            mDissolveGuildBtn.SafeRemoveOnClickListener(_OnDissolveGuideBtnClick);
            mDissolveGuildBtn = null;

            mDissolveGuildTxt = null;
        }

        private void _OnSortSeasonLvBtnClick()
        {
            _SortMembers(EColType.SeasonLv);
            RefreshMemberListCount();
        }

        private void _OnDissolveGuideBtnClick()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                if (GuildDataManager.GetInstance().HasPermission(EGuildPermission.Dismiss))
                {
                    if (GuildDataManager.GetInstance().myGuild.nDismissTime > 0)
                    {
                        GuildDataManager.GetInstance().CancelDismissGuild();
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(3004, () =>
                        {
                            GuildDataManager.GetInstance().DismissGuild();
                        });
                    }
                }
                else
                {
                    SystemNotifyManager.SystemNotify(3001, () =>
                    {
                        GuildDataManager.GetInstance().LeaveGuild();
                    });
                }
            }
        }

        private void _OnCloseBtnClick()
        {
            frameMgr.CloseFrame<GuildMemberFrame>();
        }

      
        #endregion

        void InitMemberScrollListBind()
        {
            mMemberList.Initialize();

            mMemberList.onItemChageDisplay = (ComUIListElementScript item, bool bSelected) =>
            {
                List<GuildMemberData> arrMembers = null;
                if (mData.IsSelfGuild)
                {
                    arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
                }
                else
                {
                    arrMembers = GuildDataManager.GetInstance().CanMergerdGuildMembers;
                }

                if (item.m_index >= 0 && item.m_index < arrMembers.Count)
                {
                    ComCommonBind combind = item.GetComponent<ComCommonBind>();
                    if (combind == null)
                    {
                        return;
                    }

                    GameObject mSelect = combind.GetGameObject("Select");

                    mSelect.CustomActive(false);
                }
            };

            mMemberList.onItemSelected = (ComUIListElementScript item) =>
            {
                List<GuildMemberData> arrMembers = null;
                if (mData.IsSelfGuild)
                {
                    arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
                }
                else
                {
                    arrMembers = GuildDataManager.GetInstance().CanMergerdGuildMembers;
                }

                if (item.m_index >= 0 && item.m_index < arrMembers.Count)
                {
                    ComCommonBind combind = item.GetComponent<ComCommonBind>();
                    if (combind == null)
                    {
                        return;
                    }

                    GameObject mSelect = combind.GetGameObject("Select");

                    mSelect.CustomActive(true);
                }
            };

            mMemberList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    UpdateMemberScrollListBind(item);
                }
            };

            mMemberList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Button mMask = combind.GetCom<Button>("Mask");
                mMask.onClick.RemoveAllListeners();
            };
        }

        void UpdateMemberScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            List<GuildMemberData> arrMembers = null;
            if (mData.IsSelfGuild)
            {
                arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
            }
            else
            {
                arrMembers = GuildDataManager.GetInstance().CanMergerdGuildMembers;
            }

            if (item.m_index < 0 || item.m_index >= arrMembers.Count)
            {
                return;
            }

            GuildMemberData MemData = arrMembers[item.m_index];
            if (MemData == null)
            {
                return;
            }

            GameObject mSelect = combind.GetGameObject("Select");
            Text mJobName = combind.GetCom<Text>("JobName");
            Text mName = combind.GetCom<Text>("Name");
            Text mLevel = combind.GetCom<Text>("Level");
            Text mDuty = combind.GetCom<Text>("Duty");
            Text mContribution = combind.GetCom<Text>("Contribution");
            Text mOffLineTime = combind.GetCom<Text>("OffLineTime");
            Text mActive = combind.GetCom<Text>("Active");
            Text seasonLv = combind.GetCom<Text>("seasonLv");
            Image mFace = combind.GetCom<Image>("Face");
            Button mMask = combind.GetCom<Button>("Mask");
            UIGray mFaceGray = combind.GetCom<UIGray>("FaceGray");

            ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(MemData.nJobID);

            mSelect.SetActive(false);
            if(mData.IsSelfGuild)
            {
                mJobName.text = Utility.GetJobName(MemData.nJobID);
            }
            else
            {
                mJobName.text = Utility.GetJobName(MemData.nJobID, MemData.playerLabelInfo.awakenStatus);
            }
           
            mName.text = MemData.strName;
            mLevel.text = string.Format("{0}级", MemData.nLevel);
            mDuty.text = TR.Value(MemData.eGuildDuty.GetDescription());
            mContribution.text = MemData.nContribution.ToString();
            mOffLineTime.text = _GetOfflineDesc((int)MemData.uOffLineTime);
            mActive.text = MemData.uActiveDegree.ToString();
            seasonLv.SafeSetText(SeasonDataManager.GetInstance().GetRankName((int)MemData.seasonLevel));  
            if(MemData.seasonLevel == 0)
            {
                seasonLv.SafeSetText(TR.Value("no_seasonlv_data"));
            }
            
            string path = "";
            ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobTable.Mode);
            if (resData != null)
            {
                path = resData.IconPath;
            }

            ETCImageLoader.LoadSprite(ref mFace, path);
            mFaceGray.SetEnable(MemData.uOffLineTime > 0);

            mMask.onClick.RemoveAllListeners();
            mMask.onClick.AddListener(() =>
            {
                if (MemData != null)
                {
                    mSelect.SetActive(false);
                }

                mSelect.SetActive(true);

                m_uCurrMemberID = MemData.uGUID;

                _OpenMenu(MemData);
            });
        }

        void RefreshMemberListCount()
        {
            List<GuildMemberData> arrMembers = null;
            if (mData.IsSelfGuild)
            {
                arrMembers = GuildDataManager.GetInstance().myGuild.arrMembers;
            }
            else
            {
                arrMembers = GuildDataManager.GetInstance().CanMergerdGuildMembers;
            }
            mMemberList.SetElementAmount(arrMembers.Count);

            RefreshOnLineMembers();
        }

        void _UpdatePermission()
        {
            if(mData.IsSelfGuild)
            {
                mDissolveGuildBtn.CustomActive(true);
                if (GuildDataManager.GetInstance().HasPermission(EGuildPermission.Dismiss))
                {
                    if (GuildDataManager.GetInstance().myGuild.nDismissTime > 0)
                    {
                        mDissolveGuildTxt.SafeSetText(TR.Value("guild_cancel_dismiss"));

                    }
                    else
                    {
                        mDissolveGuildTxt.SafeSetText(TR.Value("guild_dissmiss"));
                    }
                }
                else
                {
                    mDissolveGuildTxt.SafeSetText(TR.Value("guild_quit"));
                }
            }
            else
            {
                mDissolveGuildBtn.CustomActive(false);
            }
          
        }
    }
}
