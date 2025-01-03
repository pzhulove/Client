using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Scripts.UI;
using System.Collections.Generic;
using System;

namespace GameClient
{
    public class DungeonTeamChatFrame : ClientFrame 
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Chat/DungeonTeamChatFrame";
        }

        #region ExtraUIBind
        private Toggle mEasyChat = null;
        private Toggle mChatRecord = null;
        private Button mSocialBtn = null;
        private Toggle mChangeChannel = null;
        private Text mChannel = null;
        private ComUIListScript mEasyChatTips = null;
        private ComUIListScript mChatRecordList = null;
        private OnFocusInputField mInputField = null;
        private Button mSendMsgBtn = null;

        protected override void _bindExUI()
        {
            mEasyChat = mBind.GetCom<Toggle>("EasyChatToggle");
            if(mEasyChat != null)
            {
                mEasyChat.onValueChanged.AddListener(_onEasyChatToggleClick);
            }
            mChatRecord = mBind.GetCom<Toggle>("ChatRecordToggle");
            if (mChatRecord != null) 
            {
                mChatRecord.onValueChanged.AddListener(_onChatRecordToggleClick);
            }

            mSocialBtn = mBind.GetCom<Button>("SocialBtn");
            if(mSocialBtn != null)
            {
                mSocialBtn.onClick.AddListener(_onSocialBtnClick);
            }

            mChangeChannel = mBind.GetCom<Toggle>("changeChannel");
            if(mChangeChannel != null)
            {
                mChangeChannel.onValueChanged.AddListener(_onChangeChannelToggleValueChange);
            }
            mChannel = mBind.GetCom<Text>("channelText");

            mEasyChatTips = mBind.GetCom<ComUIListScript>("easyChatTipsList");

            mChatRecordList = mBind.GetCom<ComUIListScript>("chatRecordList");

            mInputField = mBind.GetCom<OnFocusInputField>("ChatInputField");
            if (mInputField != null)
            {
                mInputField.onValueChanged.AddListener(OnInputValueChanged);
                mInputField.onClick += OnInputFieldClick;
                mInputField.onEndEdit.AddListener(OnInputFieldEndEdit); 
            }
            mSendMsgBtn = mBind.GetCom<Button>("SendMsgBtn");
            if(mSendMsgBtn != null)
            {
                mSendMsgBtn.onClick.AddListener(OnClickSendChatContent);
            }
        }

        protected override void _unbindExUI()
        {
            if (mEasyChat != null)
            {
                mEasyChat.onValueChanged.RemoveListener(_onEasyChatToggleClick);
            }
            mEasyChat = null;
            
            if (mChatRecord != null)
            {
                mChatRecord.onValueChanged.RemoveListener(_onChatRecordToggleClick);
            }
            mChatRecord = null;
            
            if (mSocialBtn != null)
            {
                mSocialBtn.onClick.RemoveListener(_onSocialBtnClick);
            }
            mSocialBtn = null;
            
            if (mChangeChannel != null)
            {
                mChangeChannel.onValueChanged.RemoveListener(_onChangeChannelToggleValueChange);
            }
            mChangeChannel = null;
            mChannel = null;

            mEasyChatTips = null;

            mChatRecordList = null;
            if(mInputField != null)
            {
                mInputField.onValueChanged.RemoveListener(OnInputValueChanged);
                mInputField.onClick -= OnInputFieldClick;
                mInputField.onEndEdit.RemoveListener(OnInputFieldEndEdit);
            }
            if(mSendMsgBtn != null)
            {
                mSendMsgBtn.onClick.RemoveListener(OnClickSendChatContent);
            }
            mSendMsgBtn = null;
        }
        #endregion

        #region UI_CALL_BACK
        private void _onEasyChatToggleClick(bool flag)
        {
            if (mEasyChatTips != null)
            {
                mEasyChatTips.gameObject.SetActive(flag);
            }
        }

        private void _onChatRecordToggleClick(bool flag)
        {
            if(mChatRecord != null)
            {
                mChatRecordList.gameObject.SetActive(flag);
                var count = BattleEasyChatDataManager.GetInstance().GetChatDataArrayLength();
                if (count > 0)
                {
                    mChatRecordList.SetElementAmount(count, BattleEasyChatDataManager.GetInstance().GetChatDataArraySize());
                    if (count > 0)
                    {
                        mChatRecordList.EnsureElementVisable(count - 1);
                    }
                }
            }
        }

        private void _onSocialBtnClick()
        {
            if (BeUtility.IsRaidBattle())
            {
                SystemNotifyManager.SysNotifyTextAnimation("团本地下城点击无效");
            }
            else
            {
                RelationFrameNew.CommandOpen();
            }
        }

        private void _onChangeChannelToggleValueChange(bool flag)
        {
            if (flag)
            {
                if (isRaidBattle)
                {
                    BattleEasyChatDataManager.GetInstance().DungeonChatType = ChatType.CT_TEAM_COPY_SQUAD;
                }
                else
                {
                    BattleEasyChatDataManager.GetInstance().DungeonChatType = ChatType.CT_TEAM;
                }
            }
            else
            {
                if (isRaidBattle)
                {
                    BattleEasyChatDataManager.GetInstance().DungeonChatType = ChatType.CT_TEAM_COPY_TEAM;
                }
            }
            if(mChannel != null)
            {
                mChannel.text = BattleEasyChatDataManager.GetInstance().DungeonChatType.GetDescription();
            }
        }

        void OnInputValueChanged(string value)
        {
            ms_holdText = value;
        }

        void OnInputFieldClick()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonChatInputFieldOpen);
        }

        void OnInputFieldEndEdit(string content)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonChatInputFieldClose);
        }

        void OnClickSendChatContent()
        {
            if(!string.IsNullOrEmpty(mInputField.text) && mInputField.text.Length < ChatData.CD_MAX_WORDS)
            {
                BattleEasyChatDataManager.GetInstance().SendBattleChatMsg(
                    ChatFrame.GetFliterSizeString(mInputField.text));
                mInputField.text = "";
                Close();
            }
            else
            {
                if (mInputField.text != null && mInputField.text.Length >= ChatData.CD_MAX_WORDS)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("chat_too_many_words"));
                }
            }
        }
        #endregion
        
        private bool isRaidBattle = false;

        private bool bInitialize = false;
        private List<ChatData> chatDatas = new List<ChatData>();

        static string ms_holdText = string.Empty;

        protected override void _OnOpenFrame()
        {
            var userArray = userData as object[];
            if(userArray == null)
            {
                return;
            }
            isRaidBattle = (bool)userArray[0];

            var setRedPoint = (bool)userArray[1];

            InitChatChannel();
            _InitEasyChatUIListScrollListBind();
            _InitChatRecordUIListScrollListBind();

            if (mEasyChat != null)
            {
                mEasyChat.isOn = true;
            }
            if (mChatRecord)
            {
                mChatRecord.isOn = false;
            }
            if (mEasyChatTips != null && BattleEasyChatDataManager.GetInstance().mEasyChatTips != null)
            {
                mEasyChatTips.SetElementAmount(BattleEasyChatDataManager.GetInstance().mEasyChatTips.Count);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonChatMsgDataUpdate, _onReceiveChatDataMsg);

            bInitialize = true;
        }

        protected override void _OnCloseFrame()
        {
            chatDatas.Clear();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonChatMsgDataUpdate, _onReceiveChatDataMsg);

            bInitialize = false;
        }

        private void InitChatChannel()
        {
            if (!isRaidBattle)
            {
                if (mChangeChannel != null)
                {
                    mChangeChannel.isOn = true;
                    mChangeChannel.enabled = false;
                }
            }
            else
            {
                if (BattleEasyChatDataManager.GetInstance().DungeonChatType == ChatType.CT_TEAM_COPY_SQUAD)
                {
                    mChangeChannel.isOn = true;
                    mChangeChannel.enabled = true;
                }
                else if (BattleEasyChatDataManager.GetInstance().DungeonChatType == ChatType.CT_TEAM_COPY_TEAM)
                {
                    mChangeChannel.isOn = false;
                    _onChangeChannelToggleValueChange(false);
                    mChangeChannel.enabled = true;
                }
                else
                {
                    BattleEasyChatDataManager.GetInstance().DungeonChatType = ChatType.CT_TEAM_COPY_SQUAD;
                    mChangeChannel.isOn = true;
                    mChangeChannel.enabled = true;
                }
            }
        }

        private void _InitEasyChatUIListScrollListBind()
        {
            mEasyChatTips.Initialize();

            mEasyChatTips.onItemVisiable = (item) => 
            {
                if(item!= null && item.m_index >= 0)
                {
                    var chatItem = item.GetComponent<EasyChatItem>();
                    if(chatItem != null)
                    {
                        chatItem.Init(item.m_index);
                    }
                }
            };

            mEasyChatTips.OnItemRecycle = (item) => 
            {
                if(item != null)
                {
                    var chatItem = item.GetComponent<EasyChatItem>();
                    if(chatItem != null)
                    {
                        chatItem.Recycle();
                    }
                }
            };
        }

        const string INDEX_PATH = "UI/Image/Packed/p_UI_Tuanben.png:UI_Tuanben_Shuzi_{0}";

        private void _InitChatRecordUIListScrollListBind()
        {
            mChatRecordList.Initialize();

            mChatRecordList.onItemVisiable = (item) =>
            {
                if (item != null && item.m_index >= 0)
                {
                    var mBind = item.GetComponent<ComCommonBind>();
                    if(mBind == null)
                    {
                        return;
                    }
                    var linkParse = mBind.GetCom<LinkParse>("linkParse");
                    var indexImage = mBind.GetCom<Image>("Index");
                    if(linkParse == null)
                    {
                        return;
                    }
                    var chatData = BattleEasyChatDataManager.GetInstance().GetChatBlockByIndex(item.m_index);
                    if(chatData == null)
                    {
                        return;
                    }
                    try
                    {
                        if (chatData.chatData != null)
                        {
                            if (BattleEasyChatDataManager.GetInstance().DungeonChatType == ChatType.CT_TEAM)
                            {
                                if(indexImage != null)
                                {
                                    indexImage.gameObject.SetActive(false);
                                }
                                linkParse.SetText(string.Format("{0}{1}:{2}", chatData.chatData.GetChannelString(), chatData.chatData.objname, chatData.chatData.GetWords()), true);
                                chatData = null;
                            }
                            else
                            {
                                var index = TeamDuplicationUtility.GetTeamDuplicationCaptainIdByPlayerGuid(chatData.chatData.objid);
                                if (indexImage != null)
                                {
                                    indexImage.gameObject.SetActive(true);
                                    ETCImageLoader.LoadSprite(ref indexImage, string.Format(INDEX_PATH, index));
                                }
                                linkParse.SetText(string.Format("<color=#00000000>XX</color>{0}{1}:{2}", chatData.chatData.GetChannelString(), chatData.chatData.objname, chatData.chatData.GetWords()), true);//有部分空白字符填充
                                chatData = null;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }
                }
            };
        }

        private void _onReceiveChatDataMsg(UIEvent ui)
        {
            if (mChatRecord.isOn)
            {
                var count = BattleEasyChatDataManager.GetInstance().GetChatDataArrayLength();
                if (count > 0)
                {
                    mChatRecordList.SetElementAmount(count, BattleEasyChatDataManager.GetInstance().GetChatDataArraySize());
                    if(count > 0)
                    {
                        mChatRecordList.EnsureElementVisable(count - 1);
                    }
                }
            }
            if (!mChatRecord.isOn)
            {
                mChatRecord.isOn = true;
                mEasyChat.isOn = false;
            }
        }
        

    }
}
