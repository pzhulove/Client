using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Scripts.UI;
using System.Collections.Generic;
using System;

namespace GameClient
{
    public class DungeonChatRecordFrame : ClientFrame 
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Chat/DungeonChatRecordFrame";
        }

        #region ExtraUIBind
        private ComUIListScript mChatRecordList = null;

        protected override void _bindExUI()
        {
            mChatRecordList = mBind.GetCom<ComUIListScript>("chatRecordList");
            if(mChatRecordList != null)
            {
                _InitChatRecordUIListScrollListBind();
            }
            _bindUIEvent();
        }

        protected override void _unbindExUI()
        {
            mChatRecordList = null;
            _unBindUIEvent();
        }
        #endregion

        #region UI_CALL_BACK
        void _bindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonChatMsgDataUpdate, _onReceiveChatDataMsg);
        }

        void _unBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonChatMsgDataUpdate, _onReceiveChatDataMsg);
        }
        #endregion

        private List<ChatBlock> mChatDataList;

        protected override void _OnOpenFrame()
        {
            var userArray = userData as object[];
            if (userArray == null)
            {
                return;
            }
            var chatData = userArray[0] as ChatBlock;
            if(chatData == null)
            {
                return;
            }
            bool isRaidBattle = (bool)userArray[1];
            if(isRaidBattle && BattleEasyChatDataManager.GetInstance().DungeonChatType == ChatType.CT_TEAM)
            {
                BattleEasyChatDataManager.GetInstance().DungeonChatType = ChatType.CT_TEAM_COPY_TEAM;
            }
            mChatDataList = new List<ChatBlock>();
            mChatDataList.Add(chatData);
            mChatRecordList.SetElementAmount(mChatDataList.Count, GetChatDataArraySize());
            if (mChatDataList.Count > 0)
            {
                mChatRecordList.EnsureElementVisable(mChatDataList.Count - 1);
            }
        }

        protected override void _OnCloseFrame()
        {
            if(mChatDataList != null)
            {
                mChatDataList.Clear();
            }
            mChatDataList = null;
            timer = 0;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        const float frameCloseSec = 3;
        private float timer;
        protected override void _OnUpdate(float timeElapsed)
        {
            timer += timeElapsed;
            if (timer > frameCloseSec)
            {
                timer = 0;
                Close();
            }
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
                    if (linkParse == null)
                    {
                        return;
                    }
                    var chatData = GetChatBlockByIndex(item.m_index);
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
                                if (indexImage != null)
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
            var chatData = ui.Param1 as ChatBlock;
            if(chatData == null || mChatDataList == null)
            {
                return;
            }
            mChatDataList.Add(chatData);
            mChatRecordList.SetElementAmount(mChatDataList.Count, GetChatDataArraySize());
            if(mChatDataList.Count > 0)
            {
                mChatRecordList.EnsureElementVisable(mChatDataList.Count - 1);
            }
            timer = 0;
        }

        private ChatBlock GetChatBlockByIndex(int index)
        {
            if (mChatDataList == null || index < 0 || index >= mChatDataList.Count)
            {
                return null;
            }
            return mChatDataList[index];
        }

        private List<Vector2> GetChatDataArraySize()
        {
            List<Vector2> list = new List<Vector2>();
            for (int i = 0; i < mChatDataList.Count; ++i)
            {
                if(mChatDataList[i] == null)
                {
                    list.Add(new Vector2 { x = 0, y = 0 });
                }
                list.Add(new Vector2 { x = 268, y = mChatDataList[i].chatData.height + 58.8f });
            }
            return list;
        }
    }
}
