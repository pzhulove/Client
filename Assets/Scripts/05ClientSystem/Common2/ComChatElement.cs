using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class ComChatElement : MonoBehaviour
    {
        public StateController mState;
        public Image kIcon;
        public LinkParse kName;
        public Text kTime;
        public Text kNormalChat;
        public LinkParse kContent;
        public Text kChannel;
        public GameObject goVip;
        public UINumber kVipLevel;
        public Button headIconBtn;

        public GameObject goHornParent;
        LinkParse kHornContent;
        GameObject goHornObject;
        public GameObject goChatWordBG;


        //add voice play button
        public Button voicePlayBtn;
        public Text voiceDuration;
        public AudioVoiceElement auvElement;

        public ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame;

        protected ChatData chatData;
        protected ChatFrame THIS;
        public ChatType ChatType
        {
            get
            {
                return chatData.eChatType;
            }
        }

        public void OnCreate(ClientFrame frame)
        {
            THIS = frame as ChatFrame;
            if(headIconBtn != null)
            {
                headIconBtn.onClick.RemoveAllListeners();
                headIconBtn.onClick.AddListener(_OnClickHeadIcon);
            }
            //new add for voice
            if (voicePlayBtn != null)
            {
                voicePlayBtn.onClick.RemoveAllListeners ();
                voicePlayBtn.onClick.AddListener (OnVoicePlayButtonClick);
            }
        }

        void OnDestroy()
        {
            if (headIconBtn != null)
            {
                headIconBtn.onClick.RemoveAllListeners();
                headIconBtn = null;
            }
            //new add for voice
            if (voicePlayBtn != null)
            {
                voicePlayBtn.onClick.RemoveAllListeners ();
                voicePlayBtn = null;
            }

            goHornParent = null;
            kHornContent = null;
            goHornObject = null;

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

        private void _OnClickHeadIcon()
        {
            if(chatData == null)
            {
                return;
            }

            if (chatData.eChatType == ChatType.CT_SYSTEM || chatData.objid == 0)
            {
                return;
            }

            if (chatData.objid == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            if (ClientSystemManager.instance.IsFrameOpen<RelationMenuFram>())
            {
                ClientSystemManager.instance.CloseFrame<RelationMenuFram>();
            }

            RelationData rd = RelationDataManager.GetInstance().GetRelationByRoleID(chatData.objid);
            if (null == rd || rd.type == (byte)RelationType.RELATION_STRANGER)
            {
                rd = new RelationData();
                rd.uid = chatData.objid;
                rd.name = chatData.objname;
                rd.occu = chatData.occu;
                rd.level = chatData.level;
                rd.isOnline = 1;
                rd.type = (byte)RelationType.RELATION_STRANGER;
            }
            
            //添加跨服的服务器Id
            rd.zoneId = chatData.zoneId;

            RelationMenuData menuData = new RelationMenuData();
            menuData.m_data = rd;
            if (chatData.channel == (int)ChanelType.CHAT_CHANNEL_PRIVATE)
            {
                menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_DEL_PRIVATE_CHAT;
            }
            else
            {
                menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_COMMON;
            }

            //团本场景中，聊天类型为团本
            if (TeamDuplicationUtility.IsInTeamDuplicationScene() == true)
            {
                menuData.type = CommonPlayerInfo.CommonPlayerType.CPT_TEAMDUPLICATION;
            }

            UIEventSystem.GetInstance().SendUIEvent(new UIEventShowFriendSecMenu(menuData));
        }

        static string ms_horn_res_path = "UIFlatten/Prefabs/Chat/LaBa";
        void _TryLoadHornResources()
        {
            if(null == goHornObject)
            {
                goHornObject = AssetLoader.instance.LoadRes(ms_horn_res_path, typeof(GameObject)).obj as GameObject;
                if(null != goHornParent)
                {
                    kHornContent = goHornObject.GetComponent<LinkParse>();
                    Utility.AttachTo(goHornObject, goHornParent);
                    goHornObject.transform.SetSiblingIndex(0);
                }
            }
        }

        public void OnItemVisible(ChatData chatData)
        {
            this.chatData = chatData;
            try
            {
                if (chatData != null)
                {
                    ShowVoiceContent();//是否是语音，显示隐藏语音内容

                    kName.SetText(chatData.GetNameLink(), true);
                  
                    if(chatData.bHorn)
                    {
                        _TryLoadHornResources();
                    }

                    if (!chatData.bHorn)
                    {
                        kContent.SetText(chatData.GetWords(), chatData.bLink == 1);
                        goChatWordBG.CustomActive(!chatData.IsWordsEmpty());
                        kNormalChat.CustomActive(true);
                        goHornObject.CustomActive(false);
                    }
                    else
                    {
                        goChatWordBG.CustomActive(false);
                        if (null != kHornContent)
                        {
                            kHornContent.SetText(chatData.GetWords(), chatData.bLink == 1);
                        }
                        kNormalChat.CustomActive(false);
                        goHornObject.CustomActive(true);
                        ReplaceHeadPortraitFrame(chatData);
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
                        else
                        {
                            // kIcon.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref kIcon, path);
                          

                            ReplaceHeadPortraitFrame(chatData);
                        }
                    }
                    else
                    {
                        // kIcon.sprite = chatData.GetSystemIcon();
                        chatData.GetSystemIcon(ref kIcon);

                        if (mReplaceHeadPortraitFrame != null)
                        {
                            mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                        }
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

                    kTime.text = chatData.shortTimeString;
                    kChannel.text = chatData.GetChannelString();

                    goVip.CustomActive(false);
                    //goVip.CustomActive(PlayerBaseData.GetInstance().VipLevel > 0 && ChatType != ChatType.CT_SYSTEM && chatData.objid != 0);
                    //kVipLevel.Value = PlayerBaseData.GetInstance().VipLevel;

                    chatData = null;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        #region ReplaceHeadPortraitFrame

        private void ReplaceHeadPortraitFrame(ChatData chatData)
        {
            if (chatData == null)
            {
                return;
            }

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

        #endregion

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