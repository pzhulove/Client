using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    //气泡管理
    public class TeamDuplicationTalkItem : MonoBehaviour
    {
        private ChatBlock _chatBlock;
        private ChatData _chatData;

        [SerializeField] private LinkParse content;
        [SerializeField] private LayoutSortOrder layoutSortOrder;

        private void Awake()
        {
            if (content != null)
            {
                content.RemoveFailedListener(OnTalkItemFailedClick);
                content.AddOnFailedListener(OnTalkItemFailedClick);
            }
        }

        private void OnDestroy()
        {
            if (content != null)
            {
                content.RemoveFailedListener(OnTalkItemFailedClick);
            }

            _chatBlock = null;
        }

        public void Init(ChatBlock chatBlock)
        {
            _chatBlock = chatBlock;
            if (_chatBlock == null)
                return;

            _chatData = _chatBlock.chatData;
            if (_chatData == null)
                return;

            InitItem();
        }

        private void InitItem()
        {
            var nameLink = _chatData.GetNameLink();
            if (string.IsNullOrEmpty(nameLink) == false)
            {
                content.SetText(_chatData.GetChannelString() + nameLink + ":" + _chatData.GetWords(), true);
            }
            else
            {
                content.SetText(_chatData.GetChannelString() + _chatData.GetWords(), true);
            }

            if (layoutSortOrder != null)
                layoutSortOrder.SortID = _chatBlock.iOrder;
        }

        private void OnTalkItemFailedClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationChatFrame();
        }

    }
}
