using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 显示的公会类型
    /// </summary>
    public enum EShowGuildType
    {
        All,//所有的公会
        CanMerged//可以兼并的公会
    }
    class GuildListFrame : ClientFrame
    {
        [UIObject("List/Content")]
        GameObject m_objGuildListRoot;

        [UIObject("List/Template")]
        GameObject m_objGuildTemplate;

        [UIControl("List/Page/LeftPage", typeof(ComButtonEnbale))]
        ComButtonEnbale m_comLeftPageBtnEnable;

        [UIControl("List/Page/RightPage", typeof(ComButtonEnbale))]
        ComButtonEnbale m_comRightPageBtnEnable;

        [UIControl("List/Text")]
        Text m_labPage;

        [UIControl("JoinGuild/Content/Text")]
        Text m_labGuildDeclaration;

        [UIControl("JoinGuild/Funcs/Create", typeof(ComButtonEnbale))]
        ComButtonEnbale m_comCreateBtnEnable;

        [UIControl("JoinGuild/Funcs/Join", typeof(UIGray))]
        UIGray m_comJoinBtnEnable;

        [UIControl("JoinGuild/Funcs/JoinAll")]
        ComButtonEnbale m_comJoinAllBtnEnable;
       

        #region ui bind

        private Button JumpTo = null;
        private InputField InputPage = null;
        private Text guildLv = null;
        private Text crossTerrName = null;
        private Text terrName = null;
        private Text joinLv = null;
        private Text mGuildLvTxt;
        private Text mGuildNameTxt;
        private GameObject goGuildInfo = null;
        private Button ContactGuildLeader = null;

        private Button mMergeBtn = null;//申请兼并
        private UIGray mMergeBtnGray = null;
        private Button mCancelMergeBtn = null;//取消兼并
        private Button mMembersBtn = null;//成员信息
        private UIGray mMembersBtnGray = null;

        private Text mTitleTxt = null;//工会标题
        
        #endregion


        int m_nCurrentStart = -1;
        int m_nTotalCount = 0;
        const int m_nPerPageCount = 5;
        ulong m_uCurrGuildID = 0;


        private const string mTitleDes1 = "公会信息";

        private const string mTitleDes2 = "公会兼并";
      
        class GuildInfo
        {
            public GuildData data;
            public GameObject objApplied;
            public GameObject objAgree;
            public Text labName;
            public Text labLevel;
            public Text labMemberCount;
            public Text labLeader;
        }
        List<GuildInfo> m_arrGuildInfos = new List<GuildInfo>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildList";
        }

        public static void OpenLinkFrame(string strParam)
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildListFrame>(FrameLayer.Middle);
        }

        #region override

 

        private EShowGuildType eShowGuildType = EShowGuildType.All;
        protected override void _OnOpenFrame()
        {
            if(userData!=null)
            {
                eShowGuildType = (EShowGuildType)userData;
            }
            else
            {
                eShowGuildType = EShowGuildType.All;//默认是显示所有的公会
            }
            _InitUI();
            _RequestGuildList(0, m_nPerPageCount);
            
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            m_nCurrentStart = -1;
            m_nTotalCount = 0;
            m_uCurrGuildID = 0;
            m_arrGuildInfos.Clear();
            eShowGuildType = EShowGuildType.All;

            _UnRegisterUIEvent();
        }

        protected override void _bindExUI()
        {
            JumpTo = mBind.GetCom<Button>("JumpTo");
            JumpTo.SafeRemoveAllListener();
            JumpTo.SafeAddOnClickListener(() => 
            {
                RequestGuildListByInput();
            });

            InputPage = mBind.GetCom<InputField>("InputPage");

            guildLv = mBind.GetCom<Text>("guildLv");
            crossTerrName = mBind.GetCom<Text>("crossTerrName");
            terrName = mBind.GetCom<Text>("terrName");
            joinLv = mBind.GetCom<Text>("joinLv");
            goGuildInfo = mBind.GetGameObject("guildInfo");
            mGuildLvTxt = mBind.GetCom<Text>("GuildLvTxt");
            mGuildNameTxt = mBind.GetCom<Text>("GuildNameTxt");

            // 联系会长按钮
            ContactGuildLeader = mBind.GetCom<Button>("ContactGuildLeader");
            ContactGuildLeader.SafeAddOnClickListener(_OnContactGuildeLeaderBtnClick);

            mMergeBtn = mBind.GetCom<Button>("Merger");
            mMergeBtnGray = mBind.GetCom<UIGray>("Merger");
            mMergeBtn.SafeAddOnClickListener(OnMergeBtnClick);
            mCancelMergeBtn = mBind.GetCom<Button>("CancelMerger");
            mCancelMergeBtn.SafeAddOnClickListener(OnCancelMergerClick);
            mMembersBtn = mBind.GetCom<Button>("Members");
            mMembersBtnGray = mBind.GetCom<UIGray>("Members");
            mMembersBtn.SafeAddOnClickListener(OnMemberstbClick);
            mTitleTxt = mBind.GetCom<Text>("TitleTxt");
        }

        

        protected override void _unbindExUI()
        {
            JumpTo = null;
            InputPage = null;
            guildLv = null;
            crossTerrName = null;
            terrName = null;
            joinLv = null;
            mGuildLvTxt = null;
            mGuildNameTxt = null;
            goGuildInfo = null;
            ContactGuildLeader.SafeRemoveOnClickListener(_OnContactGuildeLeaderBtnClick);
            ContactGuildLeader = null;
            mMergeBtn.SafeRemoveOnClickListener(OnMergeBtnClick);
            mMergeBtn = null;
            mMergeBtnGray = null;
            mMembersBtn.SafeRemoveOnClickListener(OnMemberstbClick);
            mMembersBtn = null;
            mMembersBtnGray = null;
            mTitleTxt = null;
            mCancelMergeBtn.SafeRemoveOnClickListener(OnCancelMergerClick);
            mCancelMergeBtn = null;
            
        }

        #endregion

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildListUpdated, _OnUpdateGuildList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestJoinSuccess, _OnRequestJoinSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestJoinAllSuccess, _OnRequestJoinAllSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VirtualInputNumberChange, _OnInputPageNumChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.VirtualInputEnsure, _OnVirtualInputEnsure);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RequestGuildMergerSucess, _OnRequestGuildMergerSucess);
          
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildListUpdated, _OnUpdateGuildList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestJoinSuccess, _OnRequestJoinSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestJoinAllSuccess, _OnRequestJoinAllSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VirtualInputNumberChange, _OnInputPageNumChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.VirtualInputEnsure, _OnVirtualInputEnsure);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RequestGuildMergerSucess, _OnRequestGuildMergerSucess);
           
        }

        void _InitUI()
        {
            if(eShowGuildType==EShowGuildType.CanMerged)
            {
                mTitleTxt.SafeSetText(mTitleDes2);
            }
            else if(eShowGuildType==EShowGuildType.All)
            {
                mTitleTxt.SafeSetText(mTitleDes1);
            }
            m_objGuildTemplate.SetActive(false);
            _UpdatePage();
            _UpdateJoinGuild();
            _UpdateJoinAll();
            _UpdateSelectGuildUI();
        }

        void _RequestGuildList(int a_nStartIndex, int a_nCount)
        {
          
            if (eShowGuildType==EShowGuildType.All)
            {
                GuildDataManager.GetInstance().RequestGuildList(a_nStartIndex, a_nCount);
            }
            else if(eShowGuildType==EShowGuildType.CanMerged)
            {
                GuildDataManager.GetInstance().RequestCanMergerdGuildList(a_nStartIndex, a_nCount);
            }
          
        }

        void _UpdatePage()
        {
            int nIndex = m_nCurrentStart + 1;
            int nCurrPage = nIndex / m_nPerPageCount;
            if (nIndex % m_nPerPageCount > 0)
            {
                nCurrPage++;
            }

            int nTotalPage = m_nTotalCount / m_nPerPageCount;
            if (m_nTotalCount % m_nPerPageCount > 0)
            {
                nTotalPage++;
            }

            if (nCurrPage < 0)
            {
                nCurrPage = 0;
            }

            if (nCurrPage > nTotalPage)
            {
                nCurrPage = nTotalPage;
            }

            m_comLeftPageBtnEnable.SetEnable(true);
            m_comRightPageBtnEnable.SetEnable(true);
            if (nCurrPage <= 1)
            {
                m_comLeftPageBtnEnable.SetEnable(false);
            }
            if (nCurrPage >= nTotalPage)
            {
                m_comRightPageBtnEnable.SetEnable(false);
            }
            m_labPage.text = string.Format("{0} / {1}", nCurrPage, nTotalPage);

            //if(InputPage != null)
            //{
            //    InputPage.textComponent.text = nCurrPage.ToString();
            //}
        }

        void _UpdateJoinGuild()
        {
            GuildInfo guild = _GetGuildInfo(m_uCurrGuildID);
            if (guild != null)
            {
                m_labGuildDeclaration.text = guild.data.strDeclaration;
                goGuildInfo.CustomActive(true);
                guildLv.SafeSetText(guild.data.nLevel.ToString());
                crossTerrName.SafeSetText(GuildDataManager.GetTerrName(guild.data.occupyCrossTerrId));
                terrName.SafeSetText(GuildDataManager.GetTerrName(guild.data.occupyTerrId));
                joinLv.SafeSetText(guild.data.joinLevel.ToString());
                mGuildLvTxt.SafeSetText(guild.data.nLevel.ToString());
                mGuildNameTxt.SafeSetText(guild.data.strName);
            }
            else
            {
                m_labGuildDeclaration.text = string.Empty;

                guildLv.SafeSetText("");
                crossTerrName.SafeSetText("");
                terrName.SafeSetText("");
                joinLv.SafeSetText("");
                mGuildLvTxt.SafeSetText("");
                mGuildNameTxt.SafeSetText("");
                goGuildInfo.CustomActive(false);
            }

            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                m_comCreateBtnEnable.gameObject.SetActive(false);
                m_comJoinBtnEnable.gameObject.SetActive(false);
                m_comJoinAllBtnEnable.gameObject.CustomActive(false);
                ContactGuildLeader.CustomActive(eShowGuildType==EShowGuildType.CanMerged);
               
            }
            else
            {
                m_comCreateBtnEnable.gameObject.SetActive(true);
                m_comJoinBtnEnable.gameObject.SetActive(true);
                m_comJoinAllBtnEnable.gameObject.CustomActive(true);
                ContactGuildLeader.CustomActive(true);
            }


        }

        #region ui event

        void _OnUpdateGuildList(UIEvent a_event)
        {
         
           if(eShowGuildType==EShowGuildType.All)
            {
                WorldGuildListRes msg1 = a_event.Param1 as WorldGuildListRes;
                if(msg1!=null)
                {
                    _UpdateGuildList(msg1.start, msg1.totalnum, msg1.guilds);
                }
            }
            else if(eShowGuildType==EShowGuildType.CanMerged)
            {
                WorldGuildWatchCanMergerRet msg2 = a_event.Param1 as WorldGuildWatchCanMergerRet;
                if (msg2 != null)
                {
                    _UpdateGuildList(msg2.start, msg2.totalNum, msg2.guilds);
                }
            }
         
        }
        /// <summary>
        /// 刷新公会列表 1 所有的公会 2可以兼并的公会
        /// </summary>
        private void _UpdateGuildList(int start,int totalNum, GuildEntry[] guilds)
        {
            m_arrGuildInfos.Clear();

            for (int i = 0; i < m_objGuildListRoot.transform.childCount; ++i)
            {
                GameObject.Destroy(m_objGuildListRoot.transform.GetChild(i).gameObject);
            }

            m_nCurrentStart = start;
            m_nTotalCount = totalNum;
            int nCount = guilds.Length;
            if (nCount > m_nPerPageCount)
            {
                nCount = m_nPerPageCount;
            }
            Toggle toggleTemp = null;
            for (int i = 0; i < nCount; ++i)
            {
                GuildEntry source = guilds[i];
                GuildData data = new GuildData();
                data.uGUID = source.id;
                data.nLevel = source.level;
                data.nMemberCount = source.memberNum;
                data.nMemberMaxCount = TableManager.GetInstance().GetTableItem<ProtoTable.GuildTable>(data.nLevel).MemberNum;
                data.strDeclaration = source.declaration;
                data.strLeaderName = source.leaderName;
                data.strName = source.name;
                data.bHasApplied = source.isRequested != 0;
                data.leaderID = source.leaderId;
                data.occupyCrossTerrId = source.occupyCrossTerrId;
                data.occupyTerrId = source.occupyTerrId;
                data.joinLevel = source.joinLevel;

                RelationData relationData = null;
                RelationDataManager.GetInstance().FindPlayerIsRelation(source.id, ref relationData);
                if (relationData != null)
                {
                    if (relationData.remark != null && relationData.remark != "")
                    {
                        data.remark = relationData.remark;
                    }
                }

                GameObject obj = GameObject.Instantiate(m_objGuildTemplate);
                obj.transform.SetParent(m_objGuildListRoot.transform, false);
                obj.SetActive(true);

                GuildInfo guildInfo = new GuildInfo();
                guildInfo.data = data;
                guildInfo.objApplied = Utility.FindGameObject(obj, "Tag/Image");
                guildInfo.labName = Utility.GetComponetInChild<Text>(obj, "Name/Text");
                guildInfo.labLevel = Utility.GetComponetInChild<Text>(obj, "Level/Text");
                guildInfo.labMemberCount = Utility.GetComponetInChild<Text>(obj, "Count/Text");
                guildInfo.labLeader = Utility.GetComponetInChild<Text>(obj, "Leader/Text");
                guildInfo.objAgree= Utility.FindGameObject(obj, "Agree");
                m_arrGuildInfos.Add(guildInfo);
                GuildMyData guildMyData = GuildDataManager.GetInstance().myGuild;
                if(guildMyData!=null)
                {
                    if(guildMyData.mergerRequestType==(uint)EMergerRequestType.HaveSend)
                    {
                        guildInfo.objApplied.SetActive(data.bHasApplied);
                        guildInfo.objAgree.SetActive(false);
                    }
                    else if(guildMyData.mergerRequestType == (uint)EMergerRequestType.HaveAccept)
                    {
                        guildInfo.objAgree.SetActive(data.bHasApplied);
                        guildInfo.objApplied.SetActive(false);
                    }
                    else
                    {
                        guildInfo.objApplied.SetActive(data.bHasApplied);
                        guildInfo.objAgree.SetActive(false);
                    }
                   
                }
                else
                {
                    if (eShowGuildType == EShowGuildType.CanMerged)
                    {
                        guildInfo.objApplied.SetActive(false);
                        guildInfo.objAgree.SetActive(false);
                    }
                    else
                    {
                        guildInfo.objApplied.SetActive(data.bHasApplied);
                        guildInfo.objAgree.SetActive(false);
                    }
                }
               
                guildInfo.labName.text = data.strName;
                guildInfo.labLevel.text = string.Format("Lv{0}", data.nLevel);
                guildInfo.labMemberCount.text = string.Format("{0}/{1}", data.nMemberCount, data.nMemberMaxCount);
                if (data.remark != null && relationData.remark != "")
                {
                    guildInfo.labLeader.text = data.remark;
                }
                else
                {
                    guildInfo.labLeader.text = data.strLeaderName;
                }
                

                m_uCurrGuildID = 0;
              

                Toggle togSelect = obj.GetComponent<Toggle>();
                togSelect.onValueChanged.RemoveAllListeners();
                togSelect.onValueChanged.AddListener(var =>
                {
                    if (var == true)
                    {
                        m_labGuildDeclaration.text = guildInfo.data.strDeclaration;
                        m_uCurrGuildID = guildInfo.data.uGUID;

                        guildLv.SafeSetText(guildInfo.data.nLevel.ToString());
                        crossTerrName.SafeSetText(GuildDataManager.GetTerrName(guildInfo.data.occupyCrossTerrId));
                        terrName.SafeSetText(GuildDataManager.GetTerrName(guildInfo.data.occupyTerrId));
                        joinLv.SafeSetText(guildInfo.data.joinLevel.ToString());
                        mGuildLvTxt.SafeSetText(guildInfo.data.nLevel.ToString());
                        mGuildNameTxt.SafeSetText(guildInfo.data.strName);
                        goGuildInfo.CustomActive(true);
                        _UpdateSelectGuildUI();
                    }
                });

                if (i == 0 && togSelect != null)
                {
                    toggleTemp = togSelect;
                }
            }

            if (toggleTemp != null)
            {
                toggleTemp.isOn = true;
            }

            _UpdatePage();
            _UpdateSelectGuildUI();
        }

        void _OnRequestJoinAllSuccess(UIEvent a_event)
        {
            //             for (int i = 0; i < m_arrGuildInfos.Count; ++i)
            //             {
            //                 m_arrGuildInfos[i].data.bHasApplied = true;
            //                 m_arrGuildInfos[i].objApplied.SetActive(true);
            //             }

            _RequestGuildList(m_nCurrentStart, m_nPerPageCount);

            _UpdateJoinAll();
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_request_join_all_success"));
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        void _OnRequestJoinSuccess(UIEvent a_event)
        {
            ulong uGuildGUID = (ulong)a_event.Param1;
            GuildInfo info = _GetGuildInfo(uGuildGUID);
            if (info != null)
            {
                info.data.bHasApplied = true;
                info.objApplied.SetActive(true);
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_request_join_success", info.data.strName));
            }
        }

        void _OnInputPageNumChange(UIEvent a_event)
        {
            if(InputPage != null)
            {
                int nTotalPage = m_nTotalCount / m_nPerPageCount;
                if (m_nTotalCount % m_nPerPageCount > 0)
                {
                    nTotalPage++;
                }

                int iPage = Utility.ToInt(InputPage.textComponent.text);
                if (iPage > nTotalPage)
                {
                    InputPage.textComponent.text = nTotalPage.ToString();             
                }               
            }
        }

        void _OnVirtualInputEnsure(UIEvent a_event)
        {
            RequestGuildListByInput();
        }
        /// <summary>
        /// 成功申请兼并
        /// </summary>
        /// <param name="uiEvent"></param>
        private void _OnRequestGuildMergerSucess(UIEvent uiEvent)
        {
            GuildInfo info = _GetGuildInfo(m_uCurrGuildID);
            byte opType = (byte)uiEvent.Param1;
            if (info != null)
            {
                bool op = (opType == (byte)EMergerOpType.Apply);//申请兼并成功
                info.data.bHasApplied = op;
                info.objApplied.CustomActive(op);
                info.objAgree.CustomActive(false);
                mMergeBtn.CustomActive(!op);
                if (mMergeBtnGray != null)
                {
                    mMergeBtnGray.enabled = false;
                    mMergeBtnGray.SetEnable(false);
                }
                mCancelMergeBtn.CustomActive(op);
                if(op)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guildmerge_applySuceess", info.data.strName));
                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guildmerge_cancelapplySuceess", info.data.strName));
                }
               
            }
        }
 
    
        #endregion

        void _UpdateJoinAll()
        {
            m_comJoinAllBtnEnable.SetEnable(GuildDataManager.GetInstance().canJoinAllGuild);
        }

        void _UpdateSelectGuildUI()
        {
            GuildInfo guild = _GetGuildInfo(m_uCurrGuildID);

            UIGray buttonEnable = ContactGuildLeader.gameObject.SafeAddComponent<UIGray>(false);
            if(buttonEnable != null)
            {
                buttonEnable.SetEnable(false);
                buttonEnable.SetEnable(guild == null);
            }

            if(ContactGuildLeader != null)
            {
                ContactGuildLeader.interactable = (guild != null);
            }
            goGuildInfo.CustomActive(guild != null);
            m_labGuildDeclaration.CustomActive(guild != null);
            if (eShowGuildType==EShowGuildType.All)
            {
                if (m_comJoinBtnEnable != null)
                {
                    m_comJoinBtnEnable.SetEnable(false);
                    m_comJoinBtnEnable.SetEnable(guild == null);
                }
                ContactGuildLeader.CustomActive(false);
                Button btnJoin = m_comJoinBtnEnable.gameObject.SafeAddComponent<Button>(false);
                if (btnJoin != null)
                {
                    btnJoin.interactable = (guild != null);
                }
                mMembersBtn.CustomActive(false);
                mMergeBtn.CustomActive(false);
                mCancelMergeBtn.CustomActive(false);
            }
            else if(eShowGuildType==EShowGuildType.CanMerged)
            {
                m_comJoinBtnEnable.CustomActive(false);
                m_comJoinBtnEnable.CustomActive(false);
                ContactGuildLeader.CustomActive(true);
                mMembersBtn.CustomActive(true);
               
                if(guild!=null&&guild.data!=null)
                {

                    mMergeBtn.CustomActive(!guild.data.bHasApplied);
                    mCancelMergeBtn.CustomActive(guild.data.bHasApplied);
                }
                else
                {
                    mMergeBtn.CustomActive(true);
                    mCancelMergeBtn.CustomActive(false);
                }
               
                if(mMembersBtnGray!=null)
                {
                    mMembersBtnGray.enabled = (guild == null);
                    mMembersBtnGray.SetEnable(guild == null);
                }
                if (mMembersBtn != null)
                {
                    mMembersBtn.interactable = (guild != null);
                }

              
                if (mMergeBtnGray!= null)
                {
                    mMergeBtnGray.enabled = (guild == null);
                    mMergeBtnGray.SetEnable(guild == null);
                }
                if (mMergeBtn != null)
                {
                    mMergeBtn.interactable = (guild != null);
                }


            }

        }

        GuildInfo _GetGuildInfo(ulong a_uGUID)
        {
            return m_arrGuildInfos.Find(value => { return value.data.uGUID == a_uGUID; });
        }

        [UIEventHandle("List/Page/LeftPage")]
        void _OnLeftPageClicked()
        {
            int nIndex = m_nCurrentStart + 1;
            int nCurrPage = nIndex / m_nPerPageCount;
            if (nIndex % m_nPerPageCount > 0)
            {
                nCurrPage++;
            }

            nCurrPage--;

            if (nCurrPage < 1)
            {
                return;
            }

            _RequestGuildList((nCurrPage - 1) * m_nPerPageCount, m_nPerPageCount);
        }

        [UIEventHandle("List/Page/RightPage")]
        void _OnRightPageClicked()
        {
            int nIndex = m_nCurrentStart + 1;
            int nCurrPage = nIndex / m_nPerPageCount;
            if (nIndex % m_nPerPageCount > 0)
            {
                nCurrPage++;
            }

            nCurrPage++;

            int nTotalPage = m_nTotalCount / m_nPerPageCount;
            if (m_nTotalCount % m_nPerPageCount > 0)
            {
                nTotalPage++;
            }

            if (nCurrPage > nTotalPage)
            {
                return;
            }

            _RequestGuildList((nCurrPage - 1) * m_nPerPageCount, m_nPerPageCount);
        }

        [UIEventHandle("JoinGuild/Funcs/Create")]
        void _OnCreateGuildClicked()
        {
            frameMgr.OpenFrame<GuildCreateFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("JoinGuild/Funcs/Join")]
        void _OnJoinGuildClicked()
        {
            GuildInfo info = _GetGuildInfo(m_uCurrGuildID);
            if (info != null)
            {
                GuildDataManager.GetInstance().RequestJoinGuild(info.data.uGUID);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_please_select_one_guild"));
            }
        }

        [UIEventHandle("JoinGuild/Funcs/JoinAll")]
        void _OnJoinAllGuildClicked()
        {
            GuildDataManager.GetInstance().RequestJoinAllGuild();
        }

        [UIEventHandle("Title/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }

        void RequestGuildListByInput()
        {
            if (InputPage != null)
            {
                if (string.IsNullOrEmpty(InputPage.textComponent.text))
                {
                    SystemNotifyManager.SystemNotify(9980);
                    return;
                }

                int iPage = 0;
                int.TryParse(InputPage.textComponent.text, out iPage);

                if(iPage == 0)
                {
                    _UpdatePage();
                }
                else
                {
                    _RequestGuildList((iPage - 1) * m_nPerPageCount, m_nPerPageCount);
                }                
            }

            return;
        }

        private void _OnContactGuildeLeaderBtnClick()
        {
            GuildInfo guild = _GetGuildInfo(m_uCurrGuildID);
            if (guild != null)
            {
                var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(guild.data.leaderID);
                //相关数据存在，好友或者陌生人
                if (relationData != null)
                {
                    //直接密聊
                    AuctionNewUtility.OpenChatFrame(relationData);
                    return;
                }

                //相关数据不存在的时候，向服务器请求相关数据，获得数据之后再打开密聊界面
                OtherPlayerInfoManager.GetInstance().SendWatchOnShelfItemOwnerInfo(guild.data.leaderID);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_please_select_one_guild"));
            }
        }

        /// <summary>
        ///显示改工会成员信息
        /// </summary>
        private void OnMemberstbClick()
        {
            OpenGuildMemberFrameData data = new OpenGuildMemberFrameData(m_uCurrGuildID);
            ClientSystemManager.GetInstance().OpenFrame<GuildMemberFrame>(FrameLayer.Middle, data);
        }
        /// <summary>
        ///申请兼并
        /// </summary>
        private void OnMergeBtnClick()
        {
            if(GuildDataManager.GetInstance().GuildMemberIsEnoughMegrge())
            {
                GuildDataManager.GetInstance().RequestGuildMergeOp(m_uCurrGuildID, 0);
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildmerge_menbersOver"));
            }
       
        }
        /// <summary>
        /// 取消申请兼并
        /// </summary>
        private void OnCancelMergerClick()
        {

           GuildMyData guildMyData= GuildDataManager.GetInstance().myGuild;
            if(guildMyData!=null)
            {
                if(guildMyData.mergerRequestType==(uint)EMergerRequestType.HaveAccept)
                {
                    var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
                    {
                        ContentLabel = TR.Value("guildmerge_cancelmergerContent"),
                        IsShowNotify = false,
                        LeftButtonText = TR.Value("guildmerge_cancelmergerContent_Cancel"),
                        RightButtonText = TR.Value("guildmerge_cancelmergerContent_OK"),
                        OnRightButtonClickCallBack = OnAgree,
                    };
                    SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
                }
                else
                {
                    GuildDataManager.GetInstance().RequestGuildMergeOp(m_uCurrGuildID, 1);
                }
               
            }

        }

        private void OnAgree()
        {
            GuildDataManager.GetInstance().RequestGuildMergeOp(m_uCurrGuildID, 1);
        }
    }
}
