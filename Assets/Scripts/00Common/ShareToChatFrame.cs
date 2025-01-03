using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public class ShareToChatFrame : ClientFrame
    {
        #region ExtraUIBind
        private GameObject mContent = null;
        private Button mNear = null;
        private Button mWorld = null;
        private Button mGuild = null;
        private Button mTeam = null;
        private Button mClose = null;

        protected override void _bindExUI()
        {
            mContent = mBind.GetGameObject("Content");
            mNear = mBind.GetCom<Button>("Near");
            if (null != mNear)
            {
                mNear.onClick.AddListener(_onNearButtonClick);
            }
            mWorld = mBind.GetCom<Button>("World");
            if (null != mWorld)
            {
                mWorld.onClick.AddListener(_onWorldButtonClick);
            }
            mGuild = mBind.GetCom<Button>("Guild");
            if (null != mGuild)
            {
                mGuild.onClick.AddListener(_onGuildButtonClick);
            }
            mTeam = mBind.GetCom<Button>("Team");
            if (null != mTeam)
            {
                mTeam.onClick.AddListener(_onTeamButtonClick);
            }
            mClose = mBind.GetCom<Button>("Close");
            if (null != mClose)
            {
                mClose.onClick.AddListener(_onCloseButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            mContent = null;
            if (null != mNear)
            {
                mNear.onClick.RemoveListener(_onNearButtonClick);
            }
            mNear = null;
            if (null != mWorld)
            {
                mWorld.onClick.RemoveListener(_onWorldButtonClick);
            }
            mWorld = null;
            if (null != mGuild)
            {
                mGuild.onClick.RemoveListener(_onGuildButtonClick);
            }
            mGuild = null;
            if (null != mTeam)
            {
                mTeam.onClick.RemoveListener(_onTeamButtonClick);
            }
            mTeam = null;
            if (null != mClose)
            {
                mClose.onClick.RemoveListener(_onCloseButtonClick);
            }
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onNearButtonClick()
        {
            SendMessage(ChatType.CT_NORMAL);
        }
        private void _onWorldButtonClick()
        {
            SendMessage(ChatType.CT_WORLD);
        }
        private void _onGuildButtonClick()
        {
            SendMessage(ChatType.CT_GUILD);
        }
        private void _onTeamButtonClick()
        {
            SendMessage(ChatType.CT_TEAM);
        }
        private void _onCloseButtonClick()
        {
            Close();
        }
        #endregion
        private string message = null;
        private ChatType chatType = ChatType.CT_NORMAL;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/ShareToChatFrame";
        }

        public void InitData(Vector3 pos,string msg)
        {
            if (mContent == null)
                return;
            mContent.transform.position = pos;
            message = msg;
            NetProcess.AddMsgHandler(SceneSyncChat.MsgID, _OnNetSyncChat);
        }

        private void SendMessage(ChatType type)
        {
            //如果不在队伍中 则不能发送消息到组队频道
            if(!TeamDataManager.GetInstance().HasTeam() && type == ChatType.CT_TEAM)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chat_failed_for_has_no_team"), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                Close();
                return;
            }
            chatType = type;
            ChatManager.GetInstance().SendChat(type, message);
            Close();
        }

        void _OnNetSyncChat(MsgDATA msg)
        {
            if (msg == null)
                return;
            SceneSyncChat msgData = new SceneSyncChat();
            msgData.decode(msg.bytes);

            if (null == msgData || msgData.objid != PlayerBaseData.GetInstance().RoleID)
                return;
            ChanelType eChanel = ChatManager.GetInstance()._TransChatType(chatType);
            if ((int)msgData.channel != (int)eChanel)
                return;

            SystemNotifyManager.SysNotifyTextAnimation("分享成功", CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
        }
    }
}