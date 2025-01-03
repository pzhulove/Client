using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    class ComChatElementNew : MonoBehaviour
    {
        public StateController mState;
        public Image kIcon;
        public LinkParse kName;
        public Text kTime;
        public Text kNormalChat;
        public LinkParse kContent;
        public Button headIconBtn;

        public GameObject goHornParent;
        LinkParse kHornContent;
       
        public GameObject goChatWordBG;

        public GameObject goLvRoot;
        public Text lv;

        //add voice play button
        public Button voicePlayBtn;
        public Text voiceDuration;
        public AudioVoiceElement auvElement;

        public GameObject goHuaWenLeft;
        public GameObject goHuawenRight;

        public ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame;

        protected ChatData chatData;
        protected RelationFriendFrame THIS;
        public ChatType ChatType
        {
            get
            {
                return chatData.eChatType;
            }
        }

        public void OnCreate(ClientFrame frame)
        {
            THIS = frame as RelationFriendFrame;
            
            //new add for voice
            if (voicePlayBtn != null)
            {
                voicePlayBtn.onClick.RemoveAllListeners ();
                voicePlayBtn.onClick.AddListener(OnVoicePlayButtonClick);
               
            }
            if (headIconBtn != null)
            {
                headIconBtn.onClick.RemoveAllListeners();
                headIconBtn.onClick.AddListener(OnHeadClick);
            }
        }

      
        void OnDestroy()
        {
            //new add for voice
            if (voicePlayBtn != null)
            {
                voicePlayBtn.onClick.RemoveAllListeners ();
                voicePlayBtn = null;
            }
            if (headIconBtn != null)
            {
                headIconBtn.onClick.RemoveAllListeners();
                headIconBtn = null;
            }

            goHornParent = null;
            kHornContent = null;

            auvElement = null;
        }

        public void OnRecycle()
        {
            chatData = null;
            THIS = null;
            //new add for voice
            if (auvElement != null)
            {
                auvElement.RecyclePlayVoiceTex();
            }
        }
        
        public void OnItemVisible(ChatData chatData,int index, List<ChatData> chatDatas)
        {
            this.chatData = chatData;
            try
            {
                if (chatData != null)
                {
                    ShowVoiceContent();//是否是语音，显示隐藏语音内容

                    kName.SetText(chatData.GetNameLink(), true);

                   
                    if (!chatData.bHorn)
                    {
                        kContent.SetText(chatData.GetWords(), chatData.bLink == 1);
                        goChatWordBG.CustomActive(!chatData.IsWordsEmpty());
                        kNormalChat.CustomActive(true);
                    }
                    else
                    {
                        goChatWordBG.CustomActive(false);
                        if (null != kHornContent)
                        {
                            kHornContent.SetText(chatData.GetWords(), chatData.bLink == 1);
                        }
                        kNormalChat.CustomActive(false);
                    }

                    if (ChatType != ChatType.CT_SYSTEM)
                    {
                        string path = "";
                        ProtoTable.JobTable jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(chatData.occu);
                        if (jobData != null)
                        {
                            ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobData.Mode);
                            if (resData != null)
                            {
                                path = resData.IconPath;
                            }
                        }
                        if (chatData.objid == 0)
                        {
                            // kIcon.sprite = chatData.GetSystemIcon();
                            chatData.GetSystemIcon(ref kIcon);
                        }
                        else if (chatData.bAddFriend)
                        {
                            chatData.GetSystemIcon(ref kIcon);
                            if (mReplaceHeadPortraitFrame != null)
                            {
                                mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                            }
                        }
                        else
                        {
                            // kIcon.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref kIcon, path);

                            if (mReplaceHeadPortraitFrame != null)
                            {
                                if (chatData.headFrame != 0)
                                {
                                    mReplaceHeadPortraitFrame.ReplacePhotoFrame((int)chatData.headFrame);
                                }
                                else
                                {
                                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                                }
                            }
                        }
                    }
                    else
                    {
                        // kIcon.sprite = chatData.GetSystemIcon();
                        chatData.GetSystemIcon(ref kIcon);
                    }

                    if(null != mState)
                    {
                        if(chatData.bRedPacket)
                        {
                            mState.Key = "redpacket";
                        }
                        else
                        {
                            mState.Key = "normal";
                        }
                    }

                    kTime.text = "";
                    if (index == chatDatas.Count - 1)
                    {
                        ChatData lastChatData = chatDatas[index];//最后一条数据
                        if ((index - 1) >= 0)
                        {
                            ChatData lastbutOneChatData = chatDatas[index - 1];//倒数第二条数据
                            if (lastChatData.timeStamp - lastbutOneChatData.timeStamp >= 60)
                            {
                                kTime.text = chatData.shortTimeString;
                                chatData.isShowTimeStamp = true;
                            }
                        }
                        else
                        {
                            kTime.text = chatData.shortTimeString;
                            chatData.isShowTimeStamp = true;
                        }
                    }

                    if (chatData.isShowTimeStamp)
                    {
                        kTime.text = chatData.shortTimeString;
                    }
                    
                    if (kTime.text == "")
                    {
                        goHuaWenLeft.CustomActive(false);
                        goHuawenRight.CustomActive(false);
                    }
                    else
                    {
                        goHuaWenLeft.CustomActive(true);
                        goHuawenRight.CustomActive(true);
                    }

                    chatData = null;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private void OnHeadClick()
        {
            if (chatData != null)
            {
                if (chatData.bAddFriend)
                {
                    return;
                }
            }
            
            if (THIS!=null)
            {
                THIS.OnPopupMenu();
            }
        }

        #region  Play Voice Chat

        void OnVoicePlayButtonClick()
        {
           if (chatData == null)
            {
                return;
            }
            if (chatData.bVoice == false)
            {
                return;
            }
            if (THIS == null)
            {
                return;
            }
            if (auvElement != null)
            {
                auvElement.ResetPlayVoiceTex();
            }
            THIS.PlayVoice(chatData.voiceKey);
        }

        void ShowVoiceContent()
        {
            if (chatData != null)
            {
                if (chatData.eChatType == ChatType.CT_SYSTEM )
                {
                    voiceDuration.transform.parent.gameObject.SetActive (false);
                    return;
                }
                voiceDuration.text = chatData.bVoice ? (int)chatData.voiceDuration + "\"" : "";
                voiceDuration.transform.parent.gameObject.SetActive (chatData.bVoice);
            }
        }
        #endregion
    }
}