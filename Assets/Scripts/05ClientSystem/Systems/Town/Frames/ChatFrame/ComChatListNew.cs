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
    class ComChatListNew
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
        
        RelationData relationData = null;

        ComChatElementConvertNew _OnBindItemDelegate(GameObject itemObject)
        {
            ComChatElementConvertNew comChatElement = itemObject.GetComponent<ComChatElementConvertNew>();
            if(comChatElement != null)
            {
                comChatElement.OnCreate(clientFrame);
            }
            return comChatElement;
        }

        public void Initialize(ClientFrame clientFrame,
            GameObject gameObject,
            RelationData relationData)
        {
            if(bInitialize)
            {
                return;
            }
            bInitialize = true;
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            this.relationData = relationData;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();
            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;

            //_LoadAllChatdatas();
        }

        public void RefreshChatData(RelationData data)
        {
            this.relationData = data;
            _LoadAllChatdatas();
        }
        
        void _LoadAllChatdatas()
        {
            chatDatas.Clear();
            size.Clear();

            if (null != relationData)
            {
                var chatBlocks = ChatManager.GetInstance().GetPrivateChat(relationData.uid);
                if (chatBlocks != null)
                {
                    for (int i = 0; i < chatBlocks.Count; ++i)
                    {
                        var chatData = chatBlocks[i].chatData;
                        chatDatas.Add(chatData);
                        var stringBuilder = StringBuilderCache.Acquire();
                        LinkParse._TryToken(stringBuilder, chatData.word, 0, null);
                        var temp = stringBuilder.ToString();
                        chatData.height = (int)(ChatManager.GetInstance().GetContentHeightByGeneratorNew(temp) + 0.50f);
                        StringBuilderCache.Release(stringBuilder);
                        float fVoiceH = chatData.bVoice ? 60.0f : 0.0f;
                        //float fBGH = 6.0f + 6.0f;
                        float fInterval = 0.0f;
                        if (chatData.bVoice && !string.IsNullOrEmpty(chatData.word))
                        {
                            fInterval = 0.0f;
                        }
                        float contentHeight = chatData.height + 32.0f ;
                        contentHeight = Mathf.Max(contentHeight, 90f);
                        contentHeight = contentHeight + 40 + fVoiceH + fInterval;
                        var curSize = new Vector2 { x = 984f, y = contentHeight + 32f};
                        size.Add(curSize);
                        //Logger.LogErrorFormat("size = {{{0},{1}}}", curSize.x, curSize.y);
                    }
                }
            }

            comUIListScript.SetElementAmount(chatDatas.Count, size);
            if (chatDatas.Count > 0)
            {
                comUIListScript.EnsureElementVisable(chatDatas.Count - 1);
            }
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComChatElementConvertNew;
            if (current != null && item.m_index >= 0 && item.m_index < chatDatas.Count)
            {
                current.OnItemVisible(chatDatas[item.m_index],item.m_index, chatDatas);
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

            this.relationData = null;
            this.gameObject = null;
            this.clientFrame = null;
            this.chatDatas.Clear();
            bInitialize = false;
        }
    }
}