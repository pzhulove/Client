using Network;
using Protocol;
using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    
    class RelationMenuData
    {
        public RelationData m_data;
        public CommonPlayerInfo.CommonPlayerType type;
    }

    class RelationMenuFram : ComMenuFrame
    {
        GameObject m_element;
        RelationMenuData m_data;

        protected override void _OnOpenFrame()
        {
            _InitData();
            _InitElement();
            base._OnOpenFrame();
            
        }

        protected override void _OnCloseFrame()
        {

        }

        protected void _InitData()
        {
            m_data = userData as RelationMenuData;
        }

        protected void _InitElement()
        {
            if (m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_COMMON)
            {
                _InitCommonMenu(false);
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_DEL_PRIVATE_CHAT)
            {
                _InitCommonMenu(true);
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_PRIVAT_CHAT)
            {
                _InitPrivateMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_RECOMEND)
            {
                _InitRecomendMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_BLACK)
            {
                _InitBlackMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_TEACHER)
            {
                _InitTeacherMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_PUPIL_REAL)
            {
                _InitPupilMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_PUPIL_APPLY)
            {
                _InitPupilApplyMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_TEACHER_REAL)
            {
                _InitMyTeacherMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_CLASSMATE)
            {
                _InitClassmateMenu();
            }
            else if(m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_TEAMDUPLICATION)
            {
                //团本
                _InitTeamDuplicationMenu();
            }
        }

        protected void _InitCommonMenu(bool bDeletePrivate = false)
        {
            if(!bDeletePrivate)
            {
                _AddElement("密聊", _OnChat);
            }
            else
            {
                _AddElement("删除密聊", _OnDelPriChat2);
            }
            _AddElement("查看信息", _OnCheckInfo);
            _AddElement("邀请入队", _OnInvitTeam);
            _AddElement("申请入队", _OnApplicationTeam);

            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Friend))
            {
                if (m_data.m_data.type == (byte)RelationType.RELATION_STRANGER)
                {
                    _AddElement("添加好友", _OnAddFriend);
                }
                else if (m_data.m_data.type == (byte)RelationType.RELATION_FRIEND)
                {
                    _AddElement("删除好友", _OnDelFriend);
                }
                _AddElement("加入黑名单", _OnAddBlack);
            }
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                _AddElement("邀请入会", _OnInviteMembership);
            }

            if(_CheckGetPupil(m_data.m_data))
            {
                _AddElement("收为弟子", ()=>
                {
                    _OnAskForPupil(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }

            if(_CheckGetTeacher(m_data.m_data))
            {
                _AddElement("拜师", ()=>
                {
                    if(_OnAskForTeacher(m_data.m_data))
                    {
                        frameMgr.CloseFrame(this);
                    }
                });
            }

            if (m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
                m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                _AddElement("解除师徒", ()=>
                {
                    _OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            if (BaseWebViewManager.GetInstance().IsReportFuncOpen())
            {
                _AddElement("举报违规", _OnReport);
            }
        }

        public static bool _CheckGetPupil(RelationData data)
        {
            if(null == data)
            {
                return false;
            }

            if (!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.TAPSystem))
            {
                return false;
            }

            if(!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.TAPSystem,data.level))
            {
                return false;
            }

            if (PlayerBaseData.GetInstance().Level > TAPNewDataManager.GetInstance().apprentLevelMax && 
                PlayerBaseData.GetInstance().Level < TAPNewDataManager.GetInstance().teacherMinLevel)
            {
                return false;
            }

            if (PlayerBaseData.GetInstance().Level <= data.level)
            {
                return false;
            }

            if(!TAPNewDataManager.GetInstance().canGetpupil)
            {
                return false;
            }

            if(TAPNewDataManager.GetInstance().isPupilFull)
            {
                return false;
            }

            if (data.type == (byte)RelationType.RELATION_MASTER ||
                data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                return false;
            }

            if(!TAPNewDataManager.GetInstance().CanApplyedPupil(data.uid))
            {
                return false;
            }

            return true;
        }

        public static bool _CheckGetTeacher(RelationData data)
        {
            if (null == data)
            {
                return false;
            }

            if(TAPNewDataManager.GetInstance().hasTeacher)
            {
                return false;
            }

            if (!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.TAPSystem))
            {
                return false;
            }

            if (!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.TAPSystem, data.level))
            {
                return false;
            }

            if (PlayerBaseData.GetInstance().Level > TAPNewDataManager.GetInstance().apprentLevelMax)
            {
                return false;
            }

            if (PlayerBaseData.GetInstance().Level >= data.level)
            {
                return false;
            }

            if (data.level < TAPNewDataManager.GetInstance().teacherMinLevel)
            {
                return false;
            }

            if (data.type == (byte)RelationType.RELATION_MASTER ||
                data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                return false;
            }

            if (!TAPNewDataManager.GetInstance().CanQuery(data.uid))
            {
                return false;
            }

            return true;
        }

        protected void _InitPrivateMenu()
        {
            _AddElement("删除密聊", _OnDelPriChat);
            _AddElement("查看信息", _OnCheckInfo);
            _AddElement("邀请入队", _OnInvitTeam);
            _AddElement("申请入队", _OnApplicationTeam);

            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Friend))
            {
                if (m_data.m_data.type == (byte)RelationType.RELATION_STRANGER)
                {
                    _AddElement("申请好友", _OnAddFriend);
                }
                else if (m_data.m_data.type == (byte)RelationType.RELATION_FRIEND)
                {
                    _AddElement("删除好友", _OnDelFriend);
                }
                _AddElement("加入黑名单", _OnAddBlack);
            }

            if (_CheckGetPupil(m_data.m_data))
            {
                _AddElement("收为弟子", () =>
                {
                    _OnAskForPupil(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }

            if (_CheckGetTeacher(m_data.m_data))
            {
                _AddElement("拜师", () =>
                {
                    if (_OnAskForTeacher(m_data.m_data))
                    {
                        frameMgr.CloseFrame(this);
                    }
                });
            }

            if (m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
                m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                _AddElement("解除师徒", () =>
                {
                    _OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
        }

        protected void _InitRecomendMenu()
        {
            
        }

        //团本的View
        protected void _InitTeamDuplicationMenu()
        {
            _AddElement("查看信息", OnCheckInfoInTeamDuplication);
        }

        protected void _InitBlackMenu()
        {
            _AddElement("查看信息", _OnCheckInfo);
            if(Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Friend))
            {
                _AddElement("申请好友", _OnAddFriend);
            }
        }

        protected void _InitTeacherMenu()
        {
            _AddElement("密聊", _OnChat);
            _AddElement("查看信息", _OnCheckInfo);
            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Friend))
            {
                _AddElement("申请好友", _OnAddFriend);
            }
            if (_CheckGetTeacher(m_data.m_data))
            {
                _AddElement("拜师", () =>
                {
                    if (_OnAskForTeacher(m_data.m_data))
                    {
                        frameMgr.CloseFrame(this);
                    }
                });
            }
            if (m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
                m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                _AddElement("解除师徒", () =>
                {
                    _OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            
        }

        protected void _InitPupilMenu()
        {
            _AddElement("密聊", _OnChat);
            _AddElement("查看信息", _OnCheckInfo);

            if(m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
                m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                _AddElement("解除师徒", () =>
                {
                    _OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            _AddElement("邀请入队", _OnInvitTeam);
            _AddElement("申请入队", _OnApplicationTeam);
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                _AddElement("邀请入会", _OnInviteMembership);
            }
        }

        protected void _InitClassmateMenu()
        {
            _AddElement("密聊", _OnChat);
            _AddElement("查看信息", _OnCheckInfo);
            _AddElement("邀请入队", _OnInvitTeam);
            _AddElement("申请入队", _OnApplicationTeam);
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                _AddElement("邀请入会", _OnInviteMembership);
            }
        }

        protected void _InitPupilApplyMenu()
        {
            _AddElement("密聊", _OnChat);
            _AddElement("查看信息", _OnCheckInfo);
            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Friend))
            {
                _AddElement("申请好友", _OnAddFriend);
            }
            if(_CheckGetPupil(m_data.m_data))
            {
                _AddElement("收为弟子", () =>
                {
                    if(_OnAskForPupil(m_data.m_data))
                    {
                        frameMgr.CloseFrame(this);
                    }
                });
            }
            if (m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
                m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                _AddElement("解除师徒", () =>
                {
                    _OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
        }

        protected void _InitMyTeacherMenu()
        {
            _AddElement("密聊", _OnChat);
            _AddElement("查看信息", _OnCheckInfo);
            if (m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
                m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE)
            {
                _AddElement("解除师徒", () =>
                {
                    _OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            _AddElement("邀请入队", _OnInvitTeam);
            _AddElement("申请入队", _OnApplicationTeam);
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                _AddElement("邀请入会", _OnInviteMembership);
            }
        }

        public static bool _OnAskForTeacher(RelationData data)
        {
            if (data.level < TAPNewDataManager.GetInstance().teacherMinLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("tap_other_get_pupil_need_lv", TAPNewDataManager.GetInstance().teacherMinLevel));
                return false;
            }

            TAPNewDataManager.GetInstance().SendApplyTeacher(data.uid);
            TAPNewDataManager.GetInstance().AddQueryInfo(data.uid);
            return true;
        }

        public static bool _OnAskForPupil(RelationData data)
        {
            if(!TAPNewDataManager.GetInstance().CheckApplyPupil(true))
            {
                return false;
            }

            var find = RelationDataManager.GetInstance().ApplyPupils.Find(x => { return x.uid == data.uid; });
            if(null != find)
            {
                RelationDataManager.GetInstance().AcceptApplyPupils(data.uid);
                RelationDataManager.GetInstance().RemoveApplyPupil(data.uid);
            }
            else
            {
                TAPNewDataManager.GetInstance().SendApplyPupil(data.uid);
                TAPNewDataManager.GetInstance().AddApplyedPupil(data.uid);
            }
            return true;
        }

        public static void _OnFireTeacher(RelationData data)
        {
            SystemNotifyManager.SystemNotifyOkCancel(7020,
                null,
                () =>
                {
                    if (data.type == (byte)RelationType.RELATION_MASTER)
                    {
                        RelationDataManager.GetInstance().DelRelation(data.uid, RelationType.RELATION_MASTER);
                    }
                    else if (data.type == (byte)RelationType.RELATION_DISCIPLE)
                    {
                        RelationDataManager.GetInstance().DelRelation(data.uid, RelationType.RELATION_DISCIPLE);
                    }
                });
        }

        protected void _AddElement(string name, UnityEngine.Events.UnityAction cb)
        {
            m_element = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Common/ComLayoutElement");

            Text title = Utility.GetComponetInChild<Text>(m_element, "Button/Text");
            title.text = name;
            Button btn = Utility.GetComponetInChild<Button>(m_element, "Button");
            btn.onClick.AddListener(cb);

            GameObject content = Utility.FindGameObject(frame, "Content");
            m_element.transform.SetParent(content.transform, false);
        }

        //正常查看好友信息
        protected void _OnCheckInfo()
        {
            OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(m_data.m_data.uid);
            frameMgr.CloseFrame(this);
            return;
        }

        //在团本中查看队员信息，需要服务器Id
        private void OnCheckInfoInTeamDuplication()
        {
            OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(m_data.m_data.uid,
                (uint) QueryPlayerType.QPT_TEAM_COPY,
                (uint) m_data.m_data.zoneId);
            frameMgr.CloseFrame(this);
        }


        protected void _OnChat()
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                return;
            }
            TAPNewDataManager.GetInstance()._TalkToPeople(m_data.m_data);
        }

        protected void _OnInvitePK()
        {
            SceneRequest msg = new SceneRequest();
            msg.type = (byte)RequestType.Request_Challenge_PK;  
            msg.target = m_data.m_data.uid; 
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            frameMgr.CloseFrame(this);
        }

        protected void _OnInvitTeam()
        {
            TeamDataManager.GetInstance().TeamInviteOtherPlayer(m_data.m_data.uid);

            frameMgr.CloseFrame(this);
        }

        protected void _OnApplicationTeam()
        {
            TeamDataManager.GetInstance().JoinTeam(m_data.m_data.uid);

            frameMgr.CloseFrame(this);
        }

        protected void _OnReport()
        {
            if (m_data != null)
            {
                BaseWebViewManager.GetInstance().TryOpenReportFrame(m_data.m_data);
            }
            frameMgr.CloseFrame(this);
        }

        protected void _OnAddFriend()
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if(null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                return;
            }

            SceneRequest req = new SceneRequest();
            req.type = (byte)RequestType.RequestFriend;
            req.target = m_data.m_data.uid;
            req.targetName = "";
            req.param = 0;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            frameMgr.CloseFrame(this);
        }

        protected void _OnDelFriend()
        {
            string msgCtx = String.Format("是否删除好友?");
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx, () =>
            {
                RelationDataManager.GetInstance().DelFriend(m_data.m_data.uid);
            }, () => { return; });

            frameMgr.CloseFrame(this);
        }

        protected void _OnDelPriChat()
        {
            RelationDataManager.GetInstance().DelPriChat(m_data.m_data.uid);

            frameMgr.CloseFrame(this);
        }

        protected void _OnDelPriChat2()
        {
            _OnDelPriChat();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SetChatTab, ChatType.CT_PRIVATE_LIST);
        }

        protected void _OnAddBlack()
        {
            string msgCtx = String.Format("是否加入黑名单?");
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx, () =>
            {
                RelationDataManager.GetInstance().AddBlackList(m_data.m_data.uid);

            }, () => { return; });
            
            frameMgr.CloseFrame(this);
        }
        protected void _OnInviteMembership()
        {
            GuildDataManager.GetInstance().InviteJoinGuild(m_data.m_data.uid);
            frameMgr.CloseFrame(this);
        }
    }
}
