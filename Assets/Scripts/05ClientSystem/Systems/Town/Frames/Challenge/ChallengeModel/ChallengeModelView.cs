using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class ChallengeModelView : MonoBehaviour
    {

        private List<ChatBlock> _chatBlockList = null;

        [Space(20)]
        [HeaderAttribute("Deep")]
        [Space(10)]
        [SerializeField] private Button deepButton;
        [SerializeField] private ChallengeModelDropControl deepModelDropControl;

        [Space(20)]
        [HeaderAttribute("Ancient")]
        [Space(10)]
        [SerializeField] private Button ancientButton;
        [SerializeField] private ChallengeModelDropControl ancientModelDropControl;

        [Space(20)] [HeaderAttribute("TeamRoot")] [Space(10)] [SerializeField]
        private Button teamButton;
        [SerializeField] private Text teamTips;
        [SerializeField] private ComUIListScript teamChatList;

        [Space(10)]
        [HeaderAttribute("Close")]
        [Space(10)]
        [SerializeField]
        private Button closeButton;

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

            if (deepButton != null)
            {
                deepButton.onClick.RemoveAllListeners();
                deepButton.onClick.AddListener(OnDeepButtonClick);
            }

            if (ancientButton != null)
            {
                ancientButton.onClick.RemoveAllListeners();
                ancientButton.onClick.AddListener(OnAncientButtonClick);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (teamButton != null)
            {
                teamButton.onClick.RemoveAllListeners();
                teamButton.onClick.AddListener(OnTeamButtonClick);
            }

            if (teamChatList != null)
            {
                teamChatList.Initialize();
                teamChatList.onItemVisiable += OnTeamChatVisible;
                teamChatList.OnItemRecycle += OnTeamChatRecycle;
            }

            UIEventSystem.GetInstance()
                .RegisterEventHandler(EUIEventID.OnChallengeTeamChatDataUpdate, OnChallengeTeamChatDataUpdate);
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if(deepButton != null)
                deepButton.onClick.RemoveAllListeners();

            if(ancientButton != null)
                ancientButton.onClick.RemoveAllListeners();

            if(teamButton != null)
                teamButton.onClick.RemoveAllListeners();

            if (teamChatList != null)
            {
                teamChatList.onItemVisiable -= OnTeamChatVisible;
                teamChatList.OnItemRecycle -= OnTeamChatRecycle;
            }

            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnChallengeTeamChatDataUpdate, OnChallengeTeamChatDataUpdate);
        }

        private void ClearData()
        {
            if (_chatBlockList != null)
            {
                _chatBlockList.Clear();
                _chatBlockList = null;
            }
        }
        
        public void InitView()
        {
            InitDeepModel();
            InitAncientModel();
            InitOtherModel();
            InitChallengeTeamContent();
        }

        private void InitDeepModel()
        {
            if (deepModelDropControl == null)
                return;

            
            var dungeonModelTable =
                ChallengeUtility.GetChallengeDungeonModelTableByModelType(DungeonModelTable.eType.DeepModel);
            deepModelDropControl.InitModelControl(dungeonModelTable);
        }

        private void InitAncientModel()
        {
            if (ancientModelDropControl == null)
                return;


            var dungeonModelTable =
                ChallengeUtility.GetChallengeDungeonModelTableByModelType(DungeonModelTable.eType.AncientModel);
            deepModelDropControl.InitModelControl(dungeonModelTable);
        }

        private void InitOtherModel()
        {

        }

        private void InitChallengeTeamContent()
        {
            if (teamTips != null)
                teamTips.text = TR.Value("challenge_team_model_tips");

            UpdateChallengeTeamChatList();
        }

        private void OnDeepButtonClick()
        {
            //ChallengeUtility.OnOpenChallengeMapFrame(DungeonModelTable.eType.DeepModel, 0);
        }

        private void OnAncientButtonClick()
        {
            //ChallengeUtility.OnOpenChallengeMapFrame(DungeonModelTable.eType.AncientModel, 0);
        }
        
        private void OnCloseFrame()
        {
            ChallengeUtility.OnCloseChallengeModelFrame();
        }

        #region TeamBottom

        private void OnTeamButtonClick()
        {
            Logger.LogErrorFormat("OnTeamButtonClick");

            ChallengeUtility.OnOpenTeamListFrame();
        }

        private void UpdateChallengeTeamChatList()
        {
            SetChatBlockList();
            _chatBlockList.Clear();

            for (var i = 0; i < ChatManager.GetInstance().GlobalChatBlock.Count; i++)
            {
                var chatBlock = ChatManager.GetInstance().GlobalChatBlock[i];
                if (chatBlock != null
                    && chatBlock.chatData != null
                    && chatBlock.chatData.eChatType == ChatType.CT_ACOMMPANY)
                    _chatBlockList.Add(chatBlock);
            }

            SetTeamChatListAmount();
        }

        private void SetChatBlockList()
        {
            if (_chatBlockList == null)
                _chatBlockList = new List<ChatBlock>();
        }

        private void SetTeamChatListAmount()
        {
            var itemNumber = 0;
            if (_chatBlockList != null)
                itemNumber = _chatBlockList.Count;

            if (teamChatList != null)
                teamChatList.SetElementAmount(itemNumber);
        }

        private void OnChallengeTeamChatDataUpdate(UIEvent uiEvent)
        {
            UpdateChallengeTeamChatList();

        }


        private void OnTeamChatVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_chatBlockList == null)
                return;

            if (teamChatList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _chatBlockList.Count)
                return;

            var chatBlock = _chatBlockList[item.m_index];
            var chatItem = item.GetComponent<ChallengeModelTeamChatItem>();

            if (chatItem != null && chatBlock != null)
                chatItem.InitItem(chatBlock);

        }

        private void OnTeamChatRecycle(ComUIListElementScript item)
        {

        }

        #endregion
    }
}
