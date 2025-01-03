using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using ProtoTable;
using System;
using ActivityLimitTime;
using Protocol;
using GameObject = UnityEngine.GameObject;

namespace GameClient
{
    public class TeamDuplicationTalkViewControl : MonoBehaviour
    {
        private float _upOffset = 125.0f + 25;
        private float _normalOffset = 64.0f + 25;

        [Space(10)]
        [HeaderAttribute("View")]
        [Space(10)]
        [SerializeField] private GameObject talkViewRoot;
        [SerializeField] private RectTransform talkViewRootRtf;
        [SerializeField] private ComTalkExtraParam talkExtraParam;

        [Space(10)] [HeaderAttribute("Button")] [Space(10)]
        [SerializeField] private Button upButton;
        [SerializeField] private GameObject upButtonRoot;
        [SerializeField] private Button downButton;
        [SerializeField] private GameObject downButtonRoot;
        [SerializeField] private Button chatButton;

        [Space(10)] [HeaderAttribute("ScrollView")] [Space(10)]
        [SerializeField] private GameObject chatItemParent;
        [SerializeField] private GameObject chatItemPrefab;
        [SerializeField] private FastVerticalLayout chatFastVerticalLayout;


        private void Awake()
        {
            BindUiEvents();

            ChatManager.GetInstance().onAddGlobalChatData += OnAddChat;
        }

        private void OnDestroy()
        {
            UnBindUiEvents();

            ChatManager.GetInstance().onAddGlobalChatData -= OnAddChat;
        }

        private void BindUiEvents()
        {
            if (upButton != null)
            {
                upButton.onClick.RemoveAllListeners();
                upButton.onClick.AddListener(OnUpButtonClick);
            }

            if (downButton != null)
            {
                downButton.onClick.RemoveAllListeners();
                downButton.onClick.AddListener(OnDownButtonClick);
            }

            if (chatButton != null)
            {
                chatButton.onClick.RemoveAllListeners();
                chatButton.onClick.AddListener(OnChatButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if(upButton != null)
                upButton.onClick.RemoveAllListeners();

            if(downButton != null)
                downButton.onClick.RemoveAllListeners();

            if(chatButton != null)
                chatButton.onClick.RemoveAllListeners();
        }

        public void Init()
        {
            InitData();
        }

        private void InitData()
        {
            if (talkExtraParam != null)
            {
                _upOffset = talkExtraParam.upOffsetHeight;
                _normalOffset = talkExtraParam.normalHeight;
            }
        }

        private void OnAddChat(ChatBlock chatBlock)
        {
            if (chatItemParent == null
                || chatItemPrefab == null)
                return;

            if (chatBlock == null)
                return;

            if (chatBlock.chatData == null)
                return;

            var chatData = chatBlock.chatData;

            if (chatData.eChatType != ChatType.CT_TEAM_COPY_PREPARE
                && chatData.eChatType != ChatType.CT_TEAM_COPY_TEAM
                && chatData.eChatType != ChatType.CT_TEAM_COPY_SQUAD)
                return;

            var chatItemGo = GameObject.Instantiate(chatItemPrefab) as GameObject;
            if (chatItemGo == null)
                return;

            //放置和显示
            Utility.AttachTo(chatItemGo, chatItemParent);
            chatItemGo.CustomActive(true);

            var teamDuplicationTalkItem = chatItemGo.GetComponent<TeamDuplicationTalkItem>();
            if (teamDuplicationTalkItem != null)
                teamDuplicationTalkItem.Init(chatBlock);

            if (chatFastVerticalLayout != null)
            {
                chatFastVerticalLayout.MarkDirty();
            }

        }

        #region Button
        private void OnUpButtonClick()
        {
            CommonUtility.UpdateGameObjectVisible(upButtonRoot, false);
            CommonUtility.UpdateGameObjectVisible(downButtonRoot, true);

            if (talkViewRootRtf == null)
                return;

            talkViewRootRtf.offsetMax = new Vector2(talkViewRootRtf.offsetMax.x, _upOffset);
            talkViewRootRtf.offsetMin = new Vector2(talkViewRootRtf.offsetMin.x, 0);
            talkViewRootRtf.anchoredPosition = new Vector2(talkViewRootRtf.anchoredPosition.x, 0);
            talkViewRootRtf.anchoredPosition3D = new Vector3(talkViewRootRtf.anchoredPosition3D.x,
                0, talkViewRootRtf.anchoredPosition3D.z);

        }

        private void OnDownButtonClick()
        {
            CommonUtility.UpdateGameObjectVisible(upButtonRoot, true);
            CommonUtility.UpdateGameObjectVisible(downButtonRoot, false);

            if (talkViewRootRtf == null)
                return;

            talkViewRootRtf.offsetMax = new Vector2(talkViewRootRtf.offsetMax.x, _normalOffset);
            talkViewRootRtf.offsetMin = new Vector2(talkViewRootRtf.offsetMin.x, 0);
            talkViewRootRtf.anchoredPosition = new Vector2(talkViewRootRtf.anchoredPosition.x, 0);
            talkViewRootRtf.anchoredPosition3D = new Vector3(talkViewRootRtf.anchoredPosition3D.x,
                0, talkViewRootRtf.anchoredPosition3D.z);
        }

        private void OnChatButtonClick()
        {
            TeamDuplicationUtility.OnOpenTeamDuplicationChatFrame();
        }
        #endregion

    }
}
