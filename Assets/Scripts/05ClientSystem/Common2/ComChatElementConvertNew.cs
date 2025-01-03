using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    class ComChatElementConvertNew : MonoBehaviour
    {
        string ms_chat_prefab_self = "UIFlatten/Prefabs/Chat/ChatPrefabSelf";
        string ms_chat_prefab_other = "UIFlatten/Prefabs/Chat/ChatPrefabOther";

        CanvasGroup canvasSelf;
        CanvasGroup canvasOther;
        ComChatElementNew self;
        ComChatElementNew other;

        CanvasGroup canvas = null;
        ComChatElementNew chatElement = null;
        ChatData chatData;
        RelationFriendFrame relationFriendFrame;

        void _TryLoadChatResource()
        {
            if (chatData.objid == PlayerBaseData.GetInstance().RoleID)
            {
                if (null == self)
                {
                    GameObject goCurrent = AssetLoader.instance.LoadRes(ms_chat_prefab_self, typeof(GameObject)).obj as GameObject;
                    if (null != goCurrent)
                    {
                        Utility.AttachTo(goCurrent, gameObject);
                        self = goCurrent.GetComponent<ComChatElementNew>();
                        canvasSelf = goCurrent.GetComponent<CanvasGroup>();
                        if (null != self)
                        {
                            self.OnCreate(relationFriendFrame);
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("can not find gameobject with path = {0}", ms_chat_prefab_self);
                    }
                }
            }
            else
            {
                if (null == other)
                {
                    GameObject goCurrent = AssetLoader.instance.LoadRes(ms_chat_prefab_other, typeof(GameObject)).obj as GameObject;
                    if (null != goCurrent)
                    {
                        Utility.AttachTo(goCurrent, gameObject);
                        other = goCurrent.GetComponent<ComChatElementNew>();
                        canvasOther = goCurrent.GetComponent<CanvasGroup>();
                        if (null != other)
                        {
                            other.OnCreate(relationFriendFrame);
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("can not find gameobject with path = {0}", ms_chat_prefab_other);
                    }
                }
            }
        }

        public void OnCreate(ClientFrame chatFrame)
        {
            this.relationFriendFrame = chatFrame as RelationFriendFrame;
        }

        public void OnRecycle()
        {
            chatData = null;
            relationFriendFrame = null;
            if(null != canvasOther)
            {
                canvasOther.alpha = 0.0f;
                canvasOther.interactable = false;
            }
            if(null != canvasSelf)
            {
                canvasSelf.alpha = 0.0f;
                canvasSelf.interactable = false;
            }
            canvas = null;
            chatElement = null;
            if(null != self)
            {
                self.OnRecycle();
            }
            if(null != other)
            {
                other.OnRecycle();
            }
        }

        public void OnItemVisible(ChatData chatData,int index, List<ChatData> chatDatas)
        {
            this.chatData = chatData;
            _TryLoadChatResource();
            if (chatData.objid == PlayerBaseData.GetInstance().RoleID)
            {
                canvas = canvasSelf;
                chatElement = self;
                if(null != canvasOther)
                {
                    canvasOther.alpha = 0.0f;
                    canvasOther.interactable = false;
                    canvasOther.blocksRaycasts = false;//new add for chat voice
                }
            }
            else
            {
                canvas = canvasOther;
                chatElement = other;
                if(null != canvasSelf)
                {
                    canvasSelf.alpha = 0.0f;
                    canvasSelf.interactable = false;
                    canvasSelf.blocksRaycasts = false;//new add for chat voice
                }
            }

            if(null != canvas)
            {
                canvas.blocksRaycasts = true;//new add for chat voice
                canvas.alpha = 1.0f;
                canvas.interactable = true;
            }
            else
            {
                Logger.LogErrorFormat("canvas is null for chat frame,how could this happened!!!");
            }

            if(null != chatElement)
            {
                chatElement.OnItemVisible(chatData,index,chatDatas);
            }
            else
            {
                Logger.LogErrorFormat("chatElement is null for chat frame,how could this happened!!!");
            }
        }
    }
}