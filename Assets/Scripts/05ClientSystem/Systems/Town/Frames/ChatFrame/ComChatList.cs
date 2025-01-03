using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    class ComChatList
    {
        bool bInitialize = false;
        public bool Initilized
        {
            get
            {
                return bInitialize;
            }
        }
        ClientFrame clientFrame = null;
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        List<ChatData> chatDatas = new List<ChatData>();
        List<Vector2> size = new List<Vector2>();
        ChatType chatType = ChatType.CT_WORLD;
        ChatFrameData chatFrameData = null;

        ComChatElementConvert _OnBindItemDelegate(GameObject itemObject)
        {
            ComChatElementConvert comChatElement = itemObject.GetComponent<ComChatElementConvert>();
            if(comChatElement != null)
            {
                comChatElement.OnCreate(clientFrame);
            }
            return comChatElement;
        }

        public void Initialize(ClientFrame clientFrame,
            GameObject gameObject,
            ChatFrameData chatFrameData)
        {
            if(bInitialize)
            {
                return;
            }
            bInitialize = true;
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            this.chatFrameData = chatFrameData;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();
            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;

            _LoadAllChatdatas();
        }

        public void RefreshChatData(ChatFrameData data)
        {
            this.chatFrameData = data;
            _LoadAllChatdatas();
        }

        bool _NormalChatFilter(ChatBlock chatBlock)
        {
            if(chatBlock.chatData.eChatType == ChatType.CT_ALL)
            {
                return false;
            }

            if(chatBlock.chatData.eChatType != chatFrameData.eChatType)
            {
                return true;
            }

            return false;
        }

        bool _PrivateChatFilter(ChatBlock chatBlock)
        {
            if (chatBlock.chatData.eChatType != chatFrameData.eChatType)
            {
                return true;
            }

            if(chatFrameData.curPrivate == null)
            {
                return true;
            }

            if (chatBlock.chatData.objid == PlayerBaseData.GetInstance().RoleID && chatBlock.chatData.targetID == chatFrameData.curPrivate.uid ||
    chatBlock.chatData.objid == chatFrameData.curPrivate.uid && PlayerBaseData.GetInstance().RoleID == chatBlock.chatData.targetID)
            {
                return false;
            }

            return true;
        }

        void _LoadAllChatdatas()
        {
            chatDatas.Clear();
            size.Clear();

            if (chatFrameData.eChatType != ChatType.CT_PRIVATE && chatFrameData.eChatType != ChatType.CT_PRIVATE_LIST)
            {
                var channelType = ChatManager.GetInstance()._TransChatType(chatFrameData.eChatType);
                var chatBlocks = ChatManager.GetInstance().GetChatDataByChanelType(channelType);
                if (chatBlocks != null)
                {
                    chatBlocks.RemoveAll(_NormalChatFilter);

                    for (int i = 0; i < chatBlocks.Count; ++i)
                    {
                        chatDatas.Add(chatBlocks[i].chatData);
                        float fVoiceH = chatBlocks[i].chatData.bVoice ? 75.0f : 0.0f;
                        float fBGH = 6.0f + 6.0f;
                        float fInterval = 0.0f;
                        if(chatBlocks[i].chatData.bVoice && !string.IsNullOrEmpty(chatBlocks[i].chatData.word))
                        {
                            fInterval = 12.0f;
                        }
                        float contentHeight = chatBlocks[i].chatData.height + 0 + 37.72f + fVoiceH + fBGH + fInterval;
                        contentHeight = Mathf.Max(contentHeight, 109.0f);
                        if (chatBlocks[i].chatData.bHorn)
                        {
                            contentHeight = 24.0f + 106.0f + 45.0f + 37.72f - 99.0f;
                        }
                        var curSize = new Vector2 { x = 850.0f, y = contentHeight + 24.0f };
                        size.Add(curSize);
                        //Logger.LogErrorFormat("size = {{{0},{1}}}", curSize.x, curSize.y);
                    }
                }
            }
            //else if(chatFrameData.eChatType == ChatType.CT_PRIVATE)
            //{
            //    if(null != chatFrameData && null != chatFrameData.curPrivate)
            //    {
            //        var chatBlocks = ChatManager.GetInstance().GetPrivateChat(chatFrameData.curPrivate.uid);
            //        if (chatBlocks != null)
            //        {
            //            for (int i = 0; i < chatBlocks.Count; ++i)
            //            {
            //                chatDatas.Add(chatBlocks[i].chatData);
            //                float fVoiceH = chatBlocks[i].chatData.bVoice ? 75.0f : 0.0f;
            //                float fBGH = 6.0f + 6.0f;
            //                float fInterval = 0.0f;
            //                if (chatBlocks[i].chatData.bVoice && !string.IsNullOrEmpty(chatBlocks[i].chatData.word))
            //                {
            //                    fInterval = 12.0f;
            //                }
            //                float contentHeight = chatBlocks[i].chatData.height + 24.0f + 37.72f + fVoiceH + fBGH + fInterval;
            //                contentHeight = Mathf.Max(contentHeight, 109.0f);
            //                if (chatBlocks[i].chatData.bHorn)
            //                {
            //                    contentHeight = 24.0f + 106.0f + 45.0f + 37.72f;
            //                }
            //                var curSize = new Vector2 { x = 850.0f, y = contentHeight + 24.0f };
            //                size.Add(curSize);
            //                //Logger.LogErrorFormat("size = {{{0},{1}}}", curSize.x, curSize.y);
            //            }
            //        }
            //    }
            //}

            comUIListScript.SetElementAmount(chatDatas.Count,size);
            if (chatDatas.Count > 0)
            {
                comUIListScript.EnsureElementVisable(chatDatas.Count - 1);
            }
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComChatElementConvert;
            if (current != null && item.m_index >= 0 && item.m_index < chatDatas.Count)
            {
                current.OnItemVisible(chatDatas[item.m_index]);
            }
        }

        public void UnInitialize()
        {
            if(comUIListScript != null)
            {
                comUIListScript.onBindItem -= _OnBindItemDelegate;
                comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                comUIListScript = null;
            }

            this.chatFrameData = null;
            this.chatType = ChatType.CT_WORLD;
            this.gameObject = null;
            this.clientFrame = null;
            this.chatDatas.Clear();
            bInitialize = false;
        }
    }
}