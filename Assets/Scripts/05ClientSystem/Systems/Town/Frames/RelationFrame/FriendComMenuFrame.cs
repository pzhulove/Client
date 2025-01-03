using Network;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FriendComMenuFrame : ClientFrame
    {
        GameObject m_element;
        RelationMenuData m_data;

        #region ExtraUIBind
        private Text mName = null;
        private GameObject mContent = null;
        private Button mClose = null;

        protected sealed override void _bindExUI()
        {
            mName = mBind.GetCom<Text>("Name");
            mContent = mBind.GetGameObject("Content");
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected sealed override void _unbindExUI()
        {
            mName = null;
            mContent = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCloseMenu);
        }
        #endregion
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RelationFrame/FriendComMenuFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _InitData();
            _InitElement();
            _Initialize();
        }

        protected sealed override void _OnCloseFrame()
        {
           
        }

        void _Initialize()
        {
            _SetupFramePosition(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
        void _SetupFramePosition(Vector2 pos)
        {
            RectTransform rectContent = mContent.GetComponent<RectTransform>();
            RectTransform rectParent = rectContent.transform.parent as RectTransform;
            Vector2 localPos;
            bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, pos, ClientSystemManager.GetInstance().UICamera, out localPos);
            if (!success)
            {
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectContent);

            Vector2 bounder = new Vector2(10.0f, 10.0f);
            float xMin = bounder.x;
            float xMax = rectParent.rect.size.x - bounder.x - rectContent.rect.size.x;
            float yMax = bounder.y;
            float yMin = -(rectParent.rect.size.y - bounder.y - rectContent.rect.size.y);

            localPos.x = Mathf.Clamp(localPos.x, xMin, xMax);
            localPos.y = Mathf.Clamp(localPos.y, yMin, yMax);

            rectContent.anchoredPosition = localPos;
        }

        protected void _InitData()
        {
            m_data = userData as RelationMenuData;
        }
        
        protected void _InitElement()
        {
            if (m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_COMMON)
            {
                _InitCommonMenu();
            }
            else if (m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_PRIVAT_CHAT)
            {
                _InitPrivateMenu();
            }
            else if (m_data.type == CommonPlayerInfo.CommonPlayerType.CPT_BLACK)
            {
                _InitBlackMenu();
            }
        }

        protected void _InitCommonMenu()
        {
            _AddElement("查看信息", _OnCheckInfo);
            _AddElement("邀请入队", _OnInvitTeam);
            _AddElement("申请入队", _OnApplicationTeam);
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                _AddElement("邀请入会", _OnInviteMembership);
            }
            if (m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
             m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE ||
             m_data.m_data.type == (byte)RelationType.RELATION_FRIEND)
            {
                _AddElement("备注名称", () =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<SettingNoteNameFrame>(FrameLayer.Middle, m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Friend))
            {
                if (m_data.m_data.type == (byte)RelationType.RELATION_FRIEND)
                {
                    _AddElement("删除好友", _OnDelFriend);
                }
                _AddElement("加入黑名单", _OnAddBlack);
            }
            
            if (RelationMenuFram._CheckGetPupil(m_data.m_data))
            {
                _AddElement("收为弟子", () =>
                {
                    RelationMenuFram._OnAskForPupil(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }

            if (RelationMenuFram._CheckGetTeacher(m_data.m_data))
            {
                _AddElement("拜师", () =>
                {
                    if (RelationMenuFram._OnAskForTeacher(m_data.m_data))
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
                    RelationMenuFram._OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            if (BaseWebViewManager.GetInstance().IsReportFuncOpen())
            {
                _AddElement("举报违规", _OnReport);
            }
        }

        protected void _InitPrivateMenu()
        {
            _AddElement("查看信息", _OnCheckInfo);
            if (m_data.m_data.type == (byte)RelationType.RELATION_MASTER ||
             m_data.m_data.type == (byte)RelationType.RELATION_DISCIPLE ||
             m_data.m_data.type == (byte)RelationType.RELATION_FRIEND)
            {
                _AddElement("备注名称", () =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<SettingNoteNameFrame>(FrameLayer.Middle, m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            _AddElement("邀请入队", _OnInvitTeam);
            _AddElement("申请入队", _OnApplicationTeam);
            if (GuildDataManager.GetInstance().myGuild != null)
            {
                _AddElement("邀请入会", _OnInviteMembership);
            }
            _AddElement("移除列表", _OnPrivateMenuRemoveList);
            if (Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Friend))
            {
                if (m_data.m_data.type == (byte)RelationType.RELATION_STRANGER)
                {
                    _AddElement("加为好友", _OnAddFriend);
                }
                else if (m_data.m_data.type == (byte)RelationType.RELATION_FRIEND)
                {
                    _AddElement("删除好友", _OnDelFriend);
                }
                _AddElement("加入黑名单", _OnAddBlack);
            }
            
            if (RelationMenuFram._CheckGetPupil(m_data.m_data))
            {
                _AddElement("收为弟子", () =>
                {
                    RelationMenuFram._OnAskForPupil(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }

            if (RelationMenuFram._CheckGetTeacher(m_data.m_data))
            {
                _AddElement("拜师", () =>
                {
                    if (RelationMenuFram._OnAskForTeacher(m_data.m_data))
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
                    RelationMenuFram._OnFireTeacher(m_data.m_data);
                    frameMgr.CloseFrame(this);
                });
            }
            if (BaseWebViewManager.GetInstance().IsReportFuncOpen())
            {
                _AddElement("举报违规", _OnReport);
            }
        }

        protected void _InitBlackMenu()
        {
            _AddElement("移除列表", _OnBlackMenuRemoveList);
            if (BaseWebViewManager.GetInstance().IsReportFuncOpen())
            {
                _AddElement("举报违规", _OnReport);
            }
        }

        protected void _OnReport()
        {
            if (m_data != null)
            {
                BaseWebViewManager.GetInstance().TryOpenReportFrame(m_data.m_data);
            }
            frameMgr.CloseFrame(this);
        }

        protected void _OnCheckInfo()
        {
            OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(m_data.m_data.uid);
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
        protected void _OnAddFriend()
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
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
            string msgCtx = String.Format("您确定删除好友{0}吗？删除后友好度清零！",m_data.m_data.name);
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgCtx, () =>
            {
                RelationDataManager.GetInstance().DelFriend(m_data.m_data.uid);
            }, () => { return; });

            frameMgr.CloseFrame(this);
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

        protected void _OnPrivateMenuRemoveList()
        {
            List<PrivateChatPlayerData> m_priChatList = RelationDataManager.GetInstance().GetPriChatList();
            if (m_priChatList != null)
            {
                var find = m_priChatList.Find(x => { return x.relationData.uid == m_data.m_data.uid; });
                if (find != null)
                {
                    m_priChatList.Remove(find);
                    ChatRecordManager.GetInstance().RemoveChatRecords(PlayerBaseData.GetInstance().RoleID, m_data.m_data.uid);
                    ChatManager.GetInstance().RemovePrivateChatData(m_data.m_data.uid);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.FriendComMenuRemoveList);
                    frameMgr.CloseFrame(this);
                }
            }
        }
        protected void _OnBlackMenuRemoveList()
        {
            if (null != m_data.m_data)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("relation_confirm_to_del_black"), () =>
                {
                    RelationDataManager.GetInstance().DelBlack(m_data.m_data.uid);
                    frameMgr.CloseFrame(this);
                });
            }
        }

        protected void _AddElement(string name, UnityEngine.Events.UnityAction cb)
        {
            m_element = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/RelationFrame/FriendComMenuLayoutElement");

            Text title = Utility.GetComponetInChild<Text>(m_element, "Button/Text");
            title.text = name;
            Button btn = Utility.GetComponetInChild<Button>(m_element, "Button");
            btn.onClick.AddListener(cb);

            GameObject content = Utility.FindGameObject(frame, "Content");
            m_element.transform.SetParent(content.transform, false);
        }
    }
}


