using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    class GuildBeInvitedListFrame : ClientFrame
    {
        string BeInvitedListElePath = "UIFlatten/Prefabs/Team/TeamBeInvitedEle";

        List<GameObject> EleObjList = new List<GameObject>();

        private InvitedTabType invitedTabType = InvitedTabType.ITT_None;

        private List<InvitedTabData> mMainTabDataList = new List<InvitedTabData>();
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamBeInvitedListFrame";
        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamNewInviteNoticeUpdate, OnNewInviteNoticeUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildInviteNoticeUpdate, OnNewInviteNoticeUpdate);
            InitMainTabUIList();
            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamNewInviteNoticeUpdate, OnNewInviteNoticeUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildInviteNoticeUpdate, OnNewInviteNoticeUpdate);
            ClearData();
            UnInitMainTabUIList();
        }

        #region MainTab

        private void InitMainTabUIList()
        {
            if (mMainTabs != null)
            {
                mMainTabs.Initialize();
                mMainTabs.onBindItem += OnBindItemDelegate;
                mMainTabs.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitMainTabUIList()
        {
            if (mMainTabs != null)
            {
                mMainTabs.onBindItem -= OnBindItemDelegate;
                mMainTabs.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private TeamInvitedTabItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<TeamInvitedTabItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var teamInvitedTabItem = item.gameObjectBindScript as TeamInvitedTabItem;
            if (teamInvitedTabItem != null && item.m_index >= 0 && item.m_index < mMainTabDataList.Count)
            {
                var mainTabData = mMainTabDataList[item.m_index];
                if (invitedTabType != InvitedTabType.ITT_None)
                {
                    teamInvitedTabItem.InitTab(mainTabData, OnInvitedTabClick, mainTabData.mInvitedTabType == invitedTabType);
                }
                else
                {
                    teamInvitedTabItem.InitTab(mainTabData, OnInvitedTabClick, item.m_index == 0);
                }
            }
        }

        private void OnInvitedTabClick(InvitedTabData invitedTabData)
        {
            if (invitedTabData == null)
            {
                return;
            }

            invitedTabType = invitedTabData.mInvitedTabType;

            UpdateEleObjList();
        }

        #endregion
        void OnNewInviteNoticeUpdate(UIEvent iEvent)
        {
            List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            if (InviteTeamList.Count <= 0)
            {
                invitedTabType = InvitedTabType.ITT_None;
            }

            UpdateMainTabList();
            UpdateEleObjList();
        }
     
        void ClearData()
        {
            for(int i = 0; i < EleObjList.Count; i++)
            {
                if(EleObjList[i] == null)
                {
                    continue;
                }

                ComCommonBind bind = EleObjList[i].GetComponent<ComCommonBind>();
                if(bind == null)
                {
                    continue;
                }

                GameObject BtReject = bind.GetGameObject("BtReject");
                BtReject.GetComponent<Button>().onClick.RemoveAllListeners();

                GameObject BtAgree = bind.GetGameObject("BtAgree");
                BtAgree.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            EleObjList.Clear();

            invitedTabType = InvitedTabType.ITT_None;

            if (mMainTabDataList != null)
            {
                mMainTabDataList.Clear();
            }
        }

        void OnReject(int index)
        {
            if (invitedTabType == InvitedTabType.ITT_Team)
            {
                NetManager netMgr = NetManager.Instance();

                List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();

                if (index < 0 || index > InviteTeamList.Count)
                {
                    return;
                }

                SceneReply msg = new SceneReply();

                msg.result = 0;
                msg.type = (byte)RequestType.InviteTeam;
                msg.requester = InviteTeamList[index].baseinfo.teamId;

                netMgr.SendCommand(ServerType.GATE_SERVER, msg);

                InviteTeamList.RemoveAt(index);

                if (InviteTeamList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }
            else
            {
                List<WorldGuildInviteNotify> GuildInviteList = GuildDataManager.GetInstance().GuildInviteList;

                if (index < 0 || index > GuildInviteList.Count)
                {
                    return;
                }

                GuildInviteList.RemoveAt(index);
                
                if (GuildInviteList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }
            
            UpdateEleObjList();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInviteNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);

            if (TeamDataManager.GetInstance().GetInviteTeamList().Count <= 0 &&
                GuildDataManager.GetInstance().GuildInviteList.Count <= 0)
            {
                frameMgr.CloseFrame(this);
            }
        }

        void OnAgree(int index)
        {
            if (invitedTabType == InvitedTabType.ITT_Team)
            {
                if (PkWaitingRoom.bBeginSeekPlayer)
                {
                    SystemNotifyManager.SystemNotify(4004);
                    return;
                }

                List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();

                if (index < 0 || index > InviteTeamList.Count)
                {
                    return;
                }

                SceneReply msg = new SceneReply();

                msg.result = 1;
                msg.type = (byte)RequestType.InviteTeam;
                msg.requester = InviteTeamList[index].baseinfo.teamId;

                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);

                InviteTeamList.RemoveAt(index);

                if (InviteTeamList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }
            else
            {
                List<WorldGuildInviteNotify> GuildInviteList = GuildDataManager.GetInstance().GuildInviteList;

                if (index < 0 || index > GuildInviteList.Count)
                {
                    return;
                }

                GuildDataManager.GetInstance().RequestJoinGuild(GuildInviteList[index].guildId);

                GuildInviteList.RemoveAt(index);

                if (GuildInviteList.Count <= 0)
                {
                    invitedTabType = InvitedTabType.ITT_None;
                }
            }
            
            UpdateEleObjList();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInviteNoticeUpdate);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);

            if (invitedTabType == InvitedTabType.ITT_Team || GuildDataManager.GetInstance().GuildInviteList.Count <= 0 && TeamDataManager.GetInstance().GetInviteTeamList().Count <= 0)
            {
                frameMgr.CloseFrame(this);
            }
        }

        void InitInterface()
        {
            //UpdateEleObjList();

            if (mTitle != null)
            {
                mTitle.text = "邀请列表";
            }

            UpdateMainTabList();
        }

        private void UpdateMainTabList()
        {
            if (mMainTabDataList != null)
            {
                mMainTabDataList.Clear();
            }

            for (int i = 0; i < (int)InvitedTabType.ITT_Count; i++)
            {
                int RealNum = 0;
                if (i == (int)InvitedTabType.ITT_Team)
                {
                    RealNum = TeamDataManager.GetInstance().GetInviteTeamList().Count;
                    if (RealNum > 0)
                    {
                        InvitedTabData tabData = new InvitedTabData();
                        tabData.mInvitedTabType = InvitedTabType.ITT_Team;
                        tabData.mTabName = "组队";

                        mMainTabDataList.Add(tabData);
                    }
                }

                if (i == (int)InvitedTabType.ITT_Guild)
                {
                    RealNum = GuildDataManager.GetInstance().GuildInviteList.Count;
                    if (RealNum > 0)
                    {
                        InvitedTabData tabData = new InvitedTabData();
                        tabData.mInvitedTabType = InvitedTabType.ITT_Guild;
                        tabData.mTabName = "公会";

                        mMainTabDataList.Add(tabData);
                    }
                }
            }

            mMainTabs.SetElementAmount(mMainTabDataList.Count);
        }

        void UpdateEleObjList()
        {
            int RealNum = 0;
            if (invitedTabType == InvitedTabType.ITT_Team)
            {
                RealNum = TeamDataManager.GetInstance().GetInviteTeamList().Count;
            }
            else
            {
                RealNum = GuildDataManager.GetInstance().GuildInviteList.Count;
            }
            
            if (RealNum > EleObjList.Count)
            {
                int iDiff = RealNum - EleObjList.Count;

                for (int i = 0; i < iDiff; i++)
                {
                    GameObject EleObj = AssetLoader.instance.LoadResAsGameObject(BeInvitedListElePath);
                    if (EleObj == null)
                    {
                        continue;
                    }

                    Utility.AttachTo(EleObj, mEleRoot);

                    EleObjList.Add(EleObj);
                }
            }

            for(int i = 0; i < EleObjList.Count; i++)
            {
                if(i < RealNum)
                {
                    ComCommonBind commonbind = EleObjList[i].GetComponent<ComCommonBind>();
                    if(commonbind == null)
                    {
                        EleObjList[i].SetActive(false);
                        continue;
                    }
                    GameObject icon = commonbind.GetGameObject("Icon");
                    Image iconImage = icon.GetComponent<Image>();
                    GameObject LevelBack = commonbind.GetGameObject("LevelBack");
                    GameObject Name = commonbind.GetGameObject("Name");
                    GameObject level = commonbind.GetGameObject("Level");
                    GameObject target = commonbind.GetGameObject("Target");
                    GameObject reject = commonbind.GetGameObject("BtReject");
                    GameObject agree = commonbind.GetGameObject("BtAgree");
                    Text name = Name.GetComponent<Text>();
                   
                    if (invitedTabType == InvitedTabType.ITT_Team)
                    {
                        int JobID = 0;
                        List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
                        name.text = string.Format("{0}  ({1}/{2})", InviteTeamList[i].baseinfo.masterInfo.name, InviteTeamList[i].baseinfo.memberNum, InviteTeamList[i].baseinfo.maxMemberNum);
                        RelationData relationData = null;
                        RelationDataManager.GetInstance().FindPlayerIsRelation(InviteTeamList[i].baseinfo.masterInfo.id, ref relationData);
                        if (relationData != null)
                        {
                            if (relationData.remark != null && relationData.remark != "")
                            {
                                name.text = string.Format("{0}  ({1}/{2})", relationData.remark, InviteTeamList[i].baseinfo.memberNum, InviteTeamList[i].baseinfo.maxMemberNum);
                            }
                        }

                        level.GetComponent<Text>().text = string.Format("Lv.{0}", InviteTeamList[i].baseinfo.masterInfo.level);
                        JobID = InviteTeamList[i].baseinfo.masterInfo.occu;

                        TeamDungeonTable table = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)InviteTeamList[i].baseinfo.target);
                        if (table != null)
                        {
                            target.GetComponent<Text>().text = string.Format("目标:{0}", table.Name);
                        }

                        JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(JobID);
                        if (jobData != null)
                        {
                            ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                            if (resData != null)
                            {
                                ETCImageLoader.LoadSprite(ref iconImage, resData.IconPath);
                            }
                        }

                        LevelBack.gameObject.SetActive(true);
                    }
                    else
                    {
                        List<WorldGuildInviteNotify> GuildInviteList = GuildDataManager.GetInstance().GuildInviteList;

                        ETCImageLoader.LoadSprite(ref iconImage, "UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Gonghui");

                        name.text = string.Format("{0}邀请你加入", GuildInviteList[i].inviterName);

                        target.GetComponent<Text>().text = string.Format("[{0}]公会", GuildInviteList[i].guildName);

                        LevelBack.gameObject.SetActive(false);
                    }
                    

                    Button btReject = reject.GetComponent<Button>();
                    btReject.onClick.RemoveAllListeners();

                    int index = i;
                    btReject.onClick.AddListener(() => { OnReject(index); });
                    
                    Button btAgree = agree.GetComponent<Button>();
                    btAgree.onClick.RemoveAllListeners();

                    int iIndex = i;
                    btAgree.onClick.AddListener(() => { OnAgree(iIndex); });

                    EleObjList[i].SetActive(true);
                }
                else
                {
                    EleObjList[i].SetActive(false);
                }
            }
        }

        #region ExtraUIBind
        private Button mBtRejectAll = null;
        private GameObject mEleRoot = null;
        private Button mBtClose = null;
        public Text mTitle = null;
        private ComUIListScript mMainTabs = null;

        protected override void _bindExUI()
        {
            mBtRejectAll = mBind.GetCom<Button>("BtRejectAll");
            mBtRejectAll.onClick.AddListener(_onBtRejectAllButtonClick);
            mEleRoot = mBind.GetGameObject("EleRoot");
            mBtClose = mBind.GetCom<Button>("BtClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mTitle = mBind.GetCom<Text>("Title");
            mMainTabs = mBind.GetCom<ComUIListScript>("MainTabs");
        }

        protected override void _unbindExUI()
        {
            mBtRejectAll.onClick.RemoveListener(_onBtRejectAllButtonClick);
            mBtRejectAll = null;
            mEleRoot = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mTitle = null;
            mMainTabs = null;
        }
        #endregion

        #region Callback
        private void _onBtRejectAllButtonClick()
        {
            if (invitedTabType == InvitedTabType.ITT_Team)
            {
                NetManager netMgr = NetManager.Instance();
                List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();

                for (int i = 0; i < InviteTeamList.Count; i++)
                {
                    SceneReply msg = new SceneReply();
                    msg.type = (byte)RequestType.InviteTeam;
                    msg.requester = InviteTeamList[i].baseinfo.teamId;
                    msg.result = 0;
                    
                    netMgr.SendCommand(ServerType.GATE_SERVER, msg);
                }

                InviteTeamList.Clear();
            }
            else
            {
                List<WorldGuildInviteNotify> GuildInviteList = GuildDataManager.GetInstance().GuildInviteList;

                GuildInviteList.Clear();
            }

            invitedTabType = InvitedTabType.ITT_None;

            UpdateEleObjList();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildInviteNoticeUpdate);

            List<NewTeamInviteList> mInviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            List<WorldGuildInviteNotify> mGuildInviteList = GuildDataManager.GetInstance().GuildInviteList;
            if (mInviteTeamList.Count <= 0 && mGuildInviteList.Count <= 0)
            {
                frameMgr.CloseFrame(this);
            }
        }

        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
