using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class ChallengeModelTeamChatItem : MonoBehaviour
    {

        private ChatBlock _chatBlock;

        [SerializeField] private LinkParse chatContent;
        
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
        }

        private void UnBindEvents()
        {
        }

        private void ClearData()
        {
            _chatBlock = null;
        }

        public void InitItem(ChatBlock chatBlock)
        {
            _chatBlock = chatBlock;

            if (_chatBlock == null || _chatBlock.chatData == null)
            {
                Logger.LogErrorFormat("ChallengeModelTeamChatItem ChatData is null");
                return;
            }

            InitContent();
        }

        private void InitContent()
        {
            if (chatContent != null)
            {
                chatContent.RemoveFailedListener(OnClickFailed);
                chatContent.AddOnFailedListener(OnClickFailed);

                var nameLink = _chatBlock.chatData.GetNameLink();

                if (!string.IsNullOrEmpty(nameLink))
                {
                    chatContent.SetText(_chatBlock.chatData.GetChannelString() + nameLink + ":" + _chatBlock.chatData.GetWords(), true);
                }
                else
                {
                    chatContent.SetText(_chatBlock.chatData.GetChannelString() + _chatBlock.chatData.GetWords(), true);
                }
            }
        }
        
        private void OnClickFailed()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ChatFrame>();

            ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle);
        }

    }
}
