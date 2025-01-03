using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Network;
using Protocol;

namespace GameClient
{
    public class ComDungeonChat : MonoBehaviour 
    {
        public GameObject   redPoint;
        public Button       onTeamChatButton;

        private bool        mDirtyFlag = false;
        private bool        dirtyFlag
        {
            get 
            {
                return mDirtyFlag;
            }

            set 
            {
                if (mDirtyFlag != value)
                {
                    mDirtyFlag = value;

                    if (null != redPoint)
                    {
                        redPoint.SetActive(mDirtyFlag);
                    }
                }
            }
        }

        void Awake () 
        {
            _bindUIEvent();
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonChatMsgDataUpdate, _onUpdateRedPointState);
            BattleEasyChatDataManager.GetInstance().SetReceiveNetMsg(BattleMain.battleType == BattleType.RaidPVE);
            if (null != onTeamChatButton)
            {
                onTeamChatButton.onClick.AddListener(_onClickChat);
            }
        }

        void OnDestroy()
        {
            _unBindUIEvent();
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonChatMsgDataUpdate, _onUpdateRedPointState);
            BattleEasyChatDataManager.GetInstance().SetRejectNetMsg();

            if (null != onTeamChatButton)
            {
                onTeamChatButton.onClick.RemoveListener(_onClickChat);
            }
        }

        void _bindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonChatInputFieldOpen, _onInputFieldOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonChatInputFieldClose, _onCloseFieldClose);
        }
        void _unBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonChatInputFieldOpen, _onInputFieldOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonChatInputFieldClose, _onCloseFieldClose);
        }
        private void _onUpdateRedPointState(UIEvent ui)
        {
            if (!ClientSystemManager.instance.IsFrameOpen<DungeonTeamChatFrame>())
            {
                dirtyFlag = true;
                Logger.LogError("设置红点");
            }
        }

        bool isInputFieldOpen = false;
        private void _onInputFieldOpen(UIEvent ui)
        {
            isInputFieldOpen = true;
        }
        private void _onCloseFieldClose(UIEvent ui)
        {
            isInputFieldOpen = false;
        }
        private void _onClickChat()
        {
            if (isInputFieldOpen)
            {
                return;
            }
            if (!ClientSystemManager.instance.IsFrameOpen<DungeonTeamChatFrame>())
            {
                ClientSystemManager.instance.CloseFrame<DungeonChatRecordFrame>();
                ClientSystemManager.instance.OpenFrame<DungeonTeamChatFrame>(FrameLayer.Middle, new object[] { BattleMain.battleType == BattleType.RaidPVE, dirtyFlag });
                _clearDirtyFlag();
            }
            else

            {
                ClientSystemManager.instance.CloseFrame<DungeonTeamChatFrame>();
                    }
                }



        private void _clearDirtyFlag()
        {
            dirtyFlag = false;
        }

        //私聊等级限制
        //private bool IsPrivateChatLevelLimit(SceneSyncChat msgData)
        //{
        //    if (msgData == null)
        //    {
        //        return false;
        //    }
        //    ChatData chatdata = new ChatData();
        //    chatdata.objid = msgData.objid;
        //    chatdata.targetID = msgData.receiverId;
        //    chatdata.level = msgData.level;
        //    chatdata.channel = msgData.channel;

        //    if (ChatManager.GetInstance().IsPrivateChatStrangerLevelLimit(chatdata))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
