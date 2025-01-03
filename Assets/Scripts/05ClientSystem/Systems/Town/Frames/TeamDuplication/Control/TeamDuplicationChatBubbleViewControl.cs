using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    //聊天气泡管理
    public class TeamDuplicationChatBubbleViewControl : MonoBehaviour
    {
        private enum ChatMessageState
        {
            ChatShow,
            ChatHidden,
        }

        private ulong _playerGuid;            //对应角色的GUID，用于控制View是否显示
        private float _interval = 0.0f;
        private ChatMessageState _chatMessageState = ChatMessageState.ChatHidden;

        [SerializeField] private RectTransform chatBgRtf;
        public float showDelay = 5.0f;
        public GameObject chatRoot;
        public Text chatContent;
        public GameObject linkRoot;
        public LinkParse linkParse;

        // Use this for initialization
        public void SetMessage(string msg)
        {
            ShowRoot(true);
            SetChatMessage(msg);
            _interval = showDelay;
        }

        public void SetChatBgRotate()
        {
            if (chatBgRtf == null)
                return;

            chatBgRtf.localRotation = new Quaternion(0.0f, 180.0f, 0.0f, 1f);
        }

        public void SetChatPlayerGuid(ulong guid)
        {
            _playerGuid = guid;
        }

        public ulong GetChatPlayerGuid()
        {
            return _playerGuid;
        }

        public void ShowRoot(bool isShow)
        {
            _chatMessageState = isShow ? ChatMessageState.ChatShow : ChatMessageState.ChatHidden;

            if (null != chatRoot)
            {
                chatRoot.SetActive(isShow);
            }
        }

        private void SetChatMessage(string msg)
        {
            if (null != chatContent)
            {
                chatContent.text = msg;
            }

            if (null != linkParse)
            {
                linkParse.SetText(msg);
            }
        }

        void Update()
        {
            if (_chatMessageState == ChatMessageState.ChatShow)
            {
                if (_interval > 0)
                {
                    _interval -= Time.deltaTime;
                }
                else
                {
                    ShowRoot(false);
                }
            }
        }
    }
}
