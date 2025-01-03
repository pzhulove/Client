using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildMyBaseFrame : ClientFrame
    {
        [UIControl("Left/Level")]
        Text m_labLevel;

        [UIControl("Left/Name")]
        Text m_labName;

        [UIControl("Left/Fund")]
        Text m_labFund;

        [UIControl("Left/MemberCount")]
        Text m_labMemberCount;

        [UIControl("Left/Leader")]
        Text m_labLeader;

        [UIControl("Right/Notice/Text")]
        Text m_labNotice;

        [UIControl("Right/Notice/Title/CountLimit")]
        Text m_labNoticeWordsCount;

        [UIControl("Right/Declaration/Text")]
        Text m_labDeclaration;

        [UIControl("Right/Declaration/Title/CountLimit")]
        Text m_labDeclarationWordsCount;

        [UIControl("Left/Quit/Text")]
        Text m_labQuitOrDismiss;

        [UIObject("Bottom/Func/Mail")]
        GameObject m_objMail;

        [UIObject("Right/Notice/Title/Change")]
        GameObject m_objChangeNotice;

        [UIObject("Right/Declaration/Title/Change")]
        GameObject m_objChangeDeclaration;

        [UIObject("Left/Name/Change")]
        GameObject m_objChangeName;

        [UIObject("Left/time")]
        GameObject m_objTime;

        [UIControl("Left/time")]
        Text m_labTime;

        float m_fUpdateTime = 0.0f;
        bool isUpdate = false;

        #region ExtraUIBind
        private Text joinLv = null;
        private Button setJoinLv = null;
        private Button guildTips = null;
        private Button mGuildMergeBtn = null;//工会兼并按钮

        private UIGray mGuildMergeGray = null;

        private GameObject mGuildMergeRedPointGo;//公会兼并的红点

        private Text mLastWeekCashTxt;//上周资金

        private Text mThisWeekCashTxt;//本周资金
        #endregion
        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMyBase";
        }

        protected override void _OnOpenFrame()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild() == false)
            {
                return;
            }

            _UpdateLeftBaseInfo();
            _UpdateNotice();
            _UpdateDeclaration();
            _UpdatePermission();
            _UpdateSetJoinLvInfo();           
            _UpdateGuildTips();
            mGuildMergeRedPointGo.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMerger));
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
        }

        public override bool IsNeedUpdate()
        {
            return isUpdate;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            //if (m_fUpdateTime <= 0)
            //{
            //    return;
            //}

            m_fUpdateTime -= timeElapsed;

            if (m_fUpdateTime <= 0)
            {
                if (m_objTime != null)
                {
                    m_labTime.text = _GetFreeTimeCDDesc((int)GuildDataManager.GetInstance().myGuild.nDismissTime);
                }
            }
        }

        protected override void _bindExUI()
        {
            joinLv = mBind.GetCom<Text>("joinLv");
            setJoinLv = mBind.GetCom<Button>("setLv");
            setJoinLv.SafeAddOnClickListener(_onSetLvButtonClick);
            guildTips = mBind.GetCom<Button>("guildTips");
            guildTips.SafeRemoveAllListener();
            guildTips.SafeAddOnClickListener(() => 
            {
                guildTips.CustomActive(false);
            });

            mGuildMergeBtn = mBind.GetCom<Button>("GuildMerge");
            mGuildMergeBtn.SafeAddOnClickListener(OnGuildMergeBtnClick);
            if(mGuildMergeBtn!=null)
            {
                mGuildMergeGray = mGuildMergeBtn.GetComponent<UIGray>();
            }
            mGuildMergeRedPointGo = mBind.GetGameObject("GuildMergerRedPoint");

            mLastWeekCashTxt = mBind.GetCom<Text>("LastWeekCashTxt");
            mThisWeekCashTxt = mBind.GetCom<Text>("thisWeekCashTxt");
        }
        protected override void _unbindExUI()
        {
            joinLv = null;         
            setJoinLv = null;
            guildTips = null;
            mGuildMergeBtn.SafeRemoveOnClickListener(OnGuildMergeBtnClick);
            mGuildMergeBtn = null;
            mGuildMergeGray = null;
            mLastWeekCashTxt = null;
            mThisWeekCashTxt = null;
        }
        #endregion
        #region Callback
        private void _onSetLvButtonClick()
        {
            if(!GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeJoinLv))
            {
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<GuildSetJoinLvFrame>();
            return; 
        }
        /// <summary>
        /// 工会兼并
        /// </summary>
        private void OnGuildMergeBtnClick()
        {
           
            GuildMyData guildMyDat=  GuildDataManager.GetInstance().myGuild;
            if(guildMyDat!=null)
            {
                if (guildMyDat.prosperity == (uint)EGuildProsperityState.Mid)
                {
                    RedPointDataManager.GetInstance().ClearRedPoint(ERedPoint.GuildMerger);
                    ClientSystemManager.GetInstance().OpenFrame<GuildListFrame>(FrameLayer.Middle, EShowGuildType.CanMerged);
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildmerge_lackselect"));
                }
              
            }
           
        }
        #endregion
        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildLeaveGuildSuccess, _OnLeaveGuildSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestDismissSuccess, _OnRequestDismissGuildSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestCancelDismissSuccess, _OnRequestCancelDismissGuildSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildChangeNoticeSuccess, _OnChangeNoticeSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildChangeDeclarationSuccess, _OnChangeDeclarationSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildChangeNameSuccess, _OnChangeNameSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildSendMailSuccess, _OnSendMailSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildJoinLvUpdate, _OnGuildJoinLvUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildChangeDutySuccess, _OnChangeDutySuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBaseInfoUpdated, _OnGuildBaseInfoUpdated);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildLeaveGuildSuccess, _OnLeaveGuildSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestDismissSuccess, _OnRequestDismissGuildSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestCancelDismissSuccess, _OnRequestCancelDismissGuildSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildChangeNoticeSuccess, _OnChangeNoticeSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildChangeDeclarationSuccess, _OnChangeDeclarationSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildChangeNameSuccess, _OnChangeNameSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildSendMailSuccess, _OnSendMailSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildJoinLvUpdate, _OnGuildJoinLvUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildChangeDutySuccess, _OnChangeDutySuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBaseInfoUpdated, _OnGuildBaseInfoUpdated);
        }

        void _UpdateLeftBaseInfo()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                GuildMyData guildMy = GuildDataManager.GetInstance().myGuild;
                m_labLevel.text = guildMy.nLevel.ToString();
                m_labFund.text = guildMy.nFund.ToString();
                m_labLeader.text = TR.Value("guild_leader", guildMy.leaderData.strName);
                m_labMemberCount.text = string.Format("{0}/{1}", guildMy.nMemberCount, guildMy.nMemberMaxCount);
                m_labName.text = guildMy.strName;
                mLastWeekCashTxt.SafeSetText(guildMy.lastWeekFunValue.ToString());
                mThisWeekCashTxt.SafeSetText(guildMy.thisWeekFunValue.ToString());
            }
        }

        void _UpdateNotice()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                m_labNotice.text = GuildDataManager.GetInstance().myGuild.strNotice;
                m_labNoticeWordsCount.text = TR.Value("guild_worlds_count", 
                    GuildDataManager.GetInstance().myGuild.strNotice.Length, 
                    TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_NOTICE_MAX_WORDS).Value
                    );
            }
        }

        void _UpdateDeclaration()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                m_labDeclaration.text = GuildDataManager.GetInstance().myGuild.strDeclaration;
                m_labDeclarationWordsCount.text = TR.Value("guild_worlds_count",
                    GuildDataManager.GetInstance().myGuild.strDeclaration.Length,
                    TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_DECLARATION_MAX_WORDS).Value
                    );
            }
        }

        string _GetFreeTimeCDDesc(int a_timeStamp)
        {
            int nTimeLeft = a_timeStamp - (int)TimeManager.GetInstance().GetServerTime();
            if (nTimeLeft < 0)
            {
                nTimeLeft = 0;
            }

            int second = 0;
            int minute = 0;
            int hour = 0;
            second = nTimeLeft % 60;
            int temp = nTimeLeft / 60;
            if (temp > 0)
            {
                minute = temp % 60;
                hour = temp / 60;
            }

            return TR.Value("guild_dissolutionguild_success", hour, minute, second);
        }

        void _UpdatePermission()
        {
            if (GuildDataManager.GetInstance().HasPermission(EGuildPermission.Dismiss))
            {
                if (GuildDataManager.GetInstance().myGuild.nDismissTime > 0)
                {
                    m_labQuitOrDismiss.text = TR.Value("guild_cancel_dismiss");
                    m_objTime.CustomActive(true);
                    m_labTime.text = _GetFreeTimeCDDesc((int)GuildDataManager.GetInstance().myGuild.nDismissTime);
                    m_fUpdateTime = 1.0f;
                    isUpdate = true;
                }
                else
                {
                    m_labQuitOrDismiss.text = TR.Value("guild_dissmiss");
                    m_objTime.CustomActive(false);
                    isUpdate = false;
                }
            }
            else
            {
                m_labQuitOrDismiss.text = TR.Value("guild_quit");
                m_objTime.CustomActive(false);
                isUpdate = false;
            }

            m_objMail.SetActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.SendMail));
            m_objChangeNotice.SetActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeNotice));
            m_objChangeDeclaration.SetActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeDeclaration));
            m_objChangeName.SetActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeName));
            if(ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICR_GUILDMERGER))
            {
                if (GuildDataManager.GetInstance().IsCanGuildMerger())
                {
                    mGuildMergeBtn.CustomActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.GuildMeger));
                }
                else
                {
                    mGuildMergeBtn.CustomActive(false);
                }
            }
            else
            {
                mGuildMergeBtn.CustomActive(false);
            }
           
          
            if(mGuildMergeGray!=null)
            {
                GuildMyData guildMyData = GuildDataManager.GetInstance().myGuild;
                if (guildMyData != null)
                {
                    if (guildMyData.prosperity != (uint)EGuildProsperityState.Mid)
                    {
                        mGuildMergeGray.enabled = true;
                    }
                    else
                    {
                        mGuildMergeGray.enabled = false;
                    }
                }
            }
        }

        void _UpdateSetJoinLvInfo()
        {
            setJoinLv.CustomActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeJoinLv));
            _OnGuildJoinLvUpdate(null);
        }
        void _OnLeaveGuildSuccess(UIEvent a_event)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCloseMainFrame);
            frameMgr.OpenFrame<GuildListFrame>(FrameLayer.Middle);
        }

        void _OnRequestDismissGuildSuccess(UIEvent a_event)
        {
            _UpdatePermission();
        }

        void _OnRequestCancelDismissGuildSuccess(UIEvent a_event)
        {
            _UpdatePermission();
        }

        void _OnChangeNoticeSuccess(UIEvent a_event)
        {
            _UpdateNotice();
            frameMgr.CloseFrame<GuildCommonModifyFrame>();
        }

        void _OnChangeDeclarationSuccess(UIEvent a_event)
        {
            _UpdateDeclaration();
            frameMgr.CloseFrame<GuildCommonModifyFrame>();
        }

        void _OnChangeNameSuccess(UIEvent a_event)
        {
            _UpdateLeftBaseInfo();
            frameMgr.CloseFrame<GuildCommonModifyFrame>();
        }

        void _OnSendMailSuccess(UIEvent a_event)
        {
            frameMgr.CloseFrame<GuildCommonModifyFrame>();
        }
        void _OnGuildJoinLvUpdate(UIEvent a_event)
        {
            if(GuildDataManager.GetInstance().HasSelfGuild())
            {
                joinLv.SafeSetText(GuildDataManager.GetInstance().myGuild.nJoinLevel.ToString());
            }            
        }
        void _OnChangeDutySuccess(UIEvent a_event)
        {
            setJoinLv.CustomActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeJoinLv));
        }
        void _OnSyncActivityState(UIEvent uiEvent)
        {
            _UpdateGuildTips();
        }
        void _UpdateGuildTips()
        {
            guildTips.CustomActive(GuildDataManager.GetInstance().IsGuildDungeonActivityOpen());
        }
        private void _OnRedPointChanged(UIEvent uiEvent)
        {
            mGuildMergeRedPointGo.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMerger));
        }

        private void _OnGuildBaseInfoUpdated(UIEvent uiEvent)
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                GuildMyData guildMy = GuildDataManager.GetInstance().myGuild;
                mLastWeekCashTxt.SafeSetText(guildMy.lastWeekFunValue.ToString());
                mThisWeekCashTxt.SafeSetText(guildMy.thisWeekFunValue.ToString());
            }
           
        }
        [UIEventHandle("Left/Quit")]
        void _OnQuitClicked()
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
                    SystemNotifyManager.SystemNotify(3001, () => {
                        GuildDataManager.GetInstance().LeaveGuild();
                    });
                }
            }
        }

        [UIEventHandle("Bottom/Func/GuildList")]
        void _OnGuildListClicked()
        {
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCloseMainFrame);
            frameMgr.OpenFrame<GuildListFrame>(FrameLayer.Middle);
        }

        [UIEventHandle("Right/Notice/Title/Change")]
        void _OnChangeNoticeClicked()
        {
            GuildCommonModifyData data = new GuildCommonModifyData();
            data.bHasCost = false;
            data.nMaxWords = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_NOTICE_MAX_WORDS).Value;
            data.onOkClicked = (string a_strValue) =>
            {
                GuildDataManager.GetInstance().ChangeNotice(a_strValue);
            };
            data.strTitle = TR.Value("guild_change_notice");
            data.strEmptyDesc = TR.Value("guild_change_notice_desc");
            data.strDefultContent = GuildDataManager.GetInstance().myGuild.strNotice;
            data.eMode = EGuildCommonModifyMode.Long;
            frameMgr.OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
        }


        [UIEventHandle("Right/Declaration/Title/Change")]
        void _OnChangeDeclarationClicked()
        {
            GuildCommonModifyData data = new GuildCommonModifyData();
            if(data == null)
            {
                return;
            }
            if(GuildDataManager.GetInstance().myGuild == null)
            {
                return;
            }
            data.bHasCost = false;
            data.nMaxWords = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_DECLARATION_MAX_WORDS).Value;
            data.onOkClicked = (string a_strValue) =>
            {
                GuildDataManager.GetInstance().ChangeDeclaration(a_strValue);
            };
            data.strTitle = TR.Value("guild_change_declaration");
            data.strEmptyDesc = TR.Value("guild_change_declaration_desc");
            data.strDefultContent = GuildDataManager.GetInstance().myGuild.strDeclaration;
            data.eMode = EGuildCommonModifyMode.Long;
            frameMgr.OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
        }

        [UIEventHandle("Left/Name/Change")]
        void _OnChangeNameClicked()
        {
            ItemData changeCard = null;
            List<ulong> arrGUIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
            for (int i = 0; i < arrGUIDs.Count; ++i)
            {
                ItemData temp = ItemDataManager.GetInstance().GetItem(arrGUIDs[i]);
                if (temp != null)
                {
                    if (temp.SubType == (int)ProtoTable.ItemTable.eSubType.ChangeName && temp.ThirdType == ProtoTable.ItemTable.eThirdType.ChangeGuildName)
                    {
                        changeCard = temp;
                        break;
                    }
                }
            }

            GuildCommonModifyData data = new GuildCommonModifyData();
            data.bHasCost = false;
            //data.nCostCount = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_CHANGE_NAME_COST).Value;
            //data.nCostID = 600000002;
            data.nMaxWords = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_NAME_MAX_WORDS).Value;
            data.onOkClicked = (string a_strValue) =>
            {
                if (changeCard != null)
                {
                    GuildDataManager.GetInstance().ChangeName((uint)changeCard.TableID, changeCard.GUID, a_strValue);
                }
                else
                {
                    ProtoTable.QuickBuyTable costTableData = TableManager.GetInstance().GetTableItem<ProtoTable.QuickBuyTable>(230000303);
                    ItemData costItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(costTableData.CostItemID);

                    string mContent = TR.Value("guild_change_name_ask", costItem.GetColorName(), costTableData.CostNum);

                    int multiple = costTableData.multiple;
                    int endTime = 0;
                    bool isTimer = false;
                    MallItemMultipleIntegralData multipleData = MallNewDataManager.GetInstance().CheckMallItemMultipleIntegral(costTableData.ID);
                    if (multipleData != null)
                    {
                        multiple += multipleData.multiple;
                        endTime = multipleData.endTime;
                    }

                    if (endTime > 0)
                    {
                        isTimer = (endTime - TimeManager.GetInstance().GetServerTime()) > 0;
                    }

                    if (multiple > 0)
                    {
                        int price = MallNewUtility.GetTicketConvertIntergalNumnber(costTableData.CostNum) * multiple;
                        string str = string.Empty;
                        if (multiple <= 1)
                        {
                            str = TR.Value("mall_fast_buy_intergral_single_multiple_desc", price);
                        }
                        else
                        {
                            str = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple,string.Empty);
                        }

                        if (isTimer == true)
                        {
                            str = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple, TR.Value("mall_fast_buy_intergral_many_multiple_remain_time_desc", Function.SetShowTimeDay(endTime)));
                        }

                        mContent += str;
                    }

                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(
                        mContent,
                        () => 
                        {
                            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                            {
                                return;
                            }

                            if (multiple > 0)
                            {
                                string content = string.Empty;
                                //积分商城积分等于上限值
                                if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper() &&
                                     MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                                {
                                    content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");

                                    MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, () => { GuildDataManager.GetInstance().ChangeName(0, 0, a_strValue); });
                                }
                                else
                                {
                                    int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(costTableData.CostNum) * multiple;

                                    int allIntergralScore = (int)PlayerBaseData.GetInstance().IntergralMallTicket + ticketConvertScoreNumber;

                                    //购买道具后商城积分超出上限值
                                    if (allIntergralScore > MallNewUtility.GetIntergralMallTicketUpper() &&
                                        (int)PlayerBaseData.GetInstance().IntergralMallTicket != MallNewUtility.GetIntergralMallTicketUpper() &&
                                        MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed == false)
                                    {
                                        content = TR.Value("mall_buy_intergral_mall_score_exceed_upper_value_desc",
                                                           (int)PlayerBaseData.GetInstance().IntergralMallTicket,
                                                           MallNewUtility.GetIntergralMallTicketUpper(),
                                                           MallNewUtility.GetIntergralMallTicketUpper() - (int)PlayerBaseData.GetInstance().IntergralMallTicket);

                                        MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, () => { GuildDataManager.GetInstance().ChangeName(0, 0, a_strValue); });
                                    }
                                    else
                                    {//未超出
                                        GuildDataManager.GetInstance().ChangeName(0, 0, a_strValue);
                                    }
                                }
                            }
                            else
                            {
                                GuildDataManager.GetInstance().ChangeName(0, 0, a_strValue);
                            }
                        }
                        );
                }
            };
            data.strTitle = TR.Value("guild_change_name");
            data.strEmptyDesc = TR.Value("guild_change_name_desc");
            data.strDefultContent = GuildDataManager.GetInstance().myGuild.strName;
            data.eMode = EGuildCommonModifyMode.Short;
            frameMgr.OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
        }

        [UIEventHandle("Bottom/Func/Mail")]
        void _OnMailClicked()
        {
            GuildCommonModifyData data = new GuildCommonModifyData();
            data.bHasCost = false;
            data.nMaxWords = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_MAIL_MAX_WORLDS).Value;
            data.onOkClicked = (string a_strValue) =>
            {
                GuildDataManager.GetInstance().SendMail(a_strValue);
            };
            data.strTitle = TR.Value("guild_send_mail");
            data.strEmptyDesc = TR.Value("guild_send_mail_desc");
            data.eMode = EGuildCommonModifyMode.Long;
            frameMgr.OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
        }

        [UIEventHandle("Bottom/Func/GuildScene")]
        void _OnSwitchToGuildScene()
        {
//             if(TeamDataManager.GetInstance().HasTeam())
//             {
//                 SystemNotifyManager.SystemNotify(1104);
//                 return;
//             }

            GuildDataManager.GetInstance().SwitchToGuildScene();
        }
    }
}
