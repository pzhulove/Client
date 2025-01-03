using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Protocol;

namespace GameClient
{
    //该类用在战斗类快捷聊天数据存储，今后可能用在玩家快捷用语的配置存储等功能
    public class BattleEasyChatDataManager : DataManager<BattleEasyChatDataManager>
    {
        private ChatType dungeonChatType = ChatType.CT_TEAM; //发送消息类型 并非接受消息类型
        public ChatType DungeonChatType
        {
            get
            {
                return dungeonChatType;
            }
            set
            {
                dungeonChatType = value;
            }
        }

        public bool isRaidBattle = false;

        public override void Clear()
        {
            //UnInitTRValue();
            //UnRegisterNetHandler();
            m_uniqueid = 1200121234;
        }

        public override void Initialize()
        {
            //InitTRValue();
            //RegisterNetHandler();
            _LoadChatFrameFont();
        }

        public List<string> mEasyChatTips = new List<string>(); //战斗内快捷聊天用语显示，发送

        const string dungeonEasyChatTR = "dungeon_easy_chat_tip{0}";
        const string raidDungeonEasyChatTR = "raid_dungeon_easy_chat_tip{0}";

        void InitTRValue()
        {
            if(mEasyChatTips == null)
            {
                mEasyChatTips = new List<string>();
            }
            mEasyChatTips.Clear();
            string chatTR;
            if (isRaidBattle)
            {
                chatTR = raidDungeonEasyChatTR;
            }
            else
            {
                chatTR = dungeonEasyChatTR;
            }
            for (int i = 0; i < 6; ++i)
            {
                mEasyChatTips.Add(TR.Value(string.Format(chatTR, i)));
            }
        }

        void UnInitTRValue()
        {
            if(mEasyChatTips != null)
            {
                mEasyChatTips.Clear();
            }
        }

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneSyncChat.MsgID, _OnSyncChat);
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneSyncChat.MsgID, _OnSyncChat);
        }


        #region _Interface
        public void SendEasyChatTipsByIndex(int index)
        {
            if (mEasyChatTips == null || index > mEasyChatTips.Count - 1 && index < 0)
            {
                return;
            }
            ChatManager.GetInstance().SendChat(dungeonChatType, mEasyChatTips[index]);
        }

        public void SendBattleChatMsg(string content)
        {
            ChatManager.GetInstance().SendChat(dungeonChatType, content);
        }

        public string GetEasyChatStringByIndex(int index)
        {
            if (mEasyChatTips == null || index > mEasyChatTips.Count - 1 && index <0)
            {
                return string.Empty;
            }
            return mEasyChatTips[index];
        }

        public void SetReceiveNetMsg(bool raidFlag)
        {
            isRaidBattle = raidFlag;
            InitTRValue();
            if(m_akChatDataPool == null)
            {
                m_akChatDataPool = new Queue<ChatBlock>();
            }
            m_akChatDataPool.Clear();
            RegisterNetHandler();
            m_bDirty = false;
        }

        public void SetRejectNetMsg()
        {
            if (m_akChatDataPool == null)
            {
                m_akChatDataPool = new Queue<ChatBlock>();
            }
            UnInitTRValue();
            m_akChatDataPool.Clear();
            UnRegisterNetHandler();
            m_bDirty = false;
        }

        public int GetChatDataArrayLength()
        {
            if (m_bDirty)
            {
                mChatDataArray = m_akChatDataPool.ToArray();
                m_bDirty = false;
            }
            if(mChatDataArray == null)
            {
                return 0;
            }
            return mChatDataArray.Length;
        }

        public List<Vector2> GetChatDataArraySize()
        {
            if (m_bDirty)
            {
                mChatDataArray = m_akChatDataPool.ToArray();
                m_bDirty = false;
            }
            List<Vector2> list = new List<Vector2>();
            for(int i = 0; i < mChatDataArray.Length; ++i)
            {
                list.Add(new Vector2 { x = 258.8f, y = mChatDataArray[i].chatData.height + 58.8f });
            }
            return list;
        }

        public ChatBlock GetChatBlockByIndex(int index)
        {
            if (m_bDirty)
            {
                mChatDataArray = m_akChatDataPool.ToArray();
                m_bDirty = false;
            }
            if(mChatDataArray==null || index < 0 || index >= mChatDataArray.Length)
            {
                return null;
            }
            return mChatDataArray[index];
        }
        #endregion

        void _OnSyncChat(MsgDATA msg)
        {
            int pos = 0;
            SceneSyncChat msgRet = new SceneSyncChat();
            msgRet.decode(msg.bytes, ref pos);
            //过滤一下消息
            if (isRaidBattle)
            {
                if((ChanelType)msgRet.channel != ChanelType.CHAT_CHANNEL_TEAM_COPY_SQUAD 
                    && (ChanelType)msgRet.channel != ChanelType.CHAT_CHANNEL_TEAM_COPY_TEAM)
                {
                    return;
                }
            }
            else
            {
                if((ChanelType)msgRet.channel != ChanelType.CHAT_CHANNEL_TEAM)
                {
                    return;
                }
            }
            ChatBlock chatData = _NetData2LocalData(msgRet);
            if(chatData != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonChatMsgDataUpdate, chatData);
            }
            var currentSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemBattle;
            if (!ClientSystemManager.instance.IsFrameOpen<DungeonChatRecordFrame>() 
                && !ClientSystemManager.instance.IsFrameOpen<DungeonTeamChatFrame>() 
                && currentSystem != null) 
            {
                ClientSystemManager.instance.OpenFrame<DungeonChatRecordFrame>(FrameLayer.Middle, new object[] { chatData, BattleMain.battleType == BattleType.RaidPVE });
            }
        }

        private ChatBlock _NetData2LocalData(SceneSyncChat data)
        {
            ChatBlock chatBlock = _AllocChatData();

            ChatData chatData = chatBlock.chatData;
            chatData.channel = data.channel;
            chatData.objid = data.objid;
            chatData.sex = data.sex;
            chatData.occu = data.occu;
            chatData.level = data.level;
            chatData.viplvl = data.viplvl;
            chatData.objname = data.objname;
            RelationData relationData = null;
            RelationDataManager.GetInstance().FindPlayerIsRelation(chatData.objid, ref relationData);
            if (relationData != null)
            {
                if (relationData.remark != null && relationData.remark != "")
                {
                    chatData.objname = relationData.remark;
                }
            }
            chatData.word = data.word;
            chatData.guid = ++m_uniqueid;
            chatData.shortTimeString = "[" + TimeManager.GetInstance().GetTimeT() + "]";
            chatData.targetID = data.receiverId;
            chatData.eChatType = ChatManager.GetInstance()._TransChanelType((ChanelType)chatData.channel);
            chatData.dirty = true;
            chatData.bLink = data.bLink;
            chatData.bGm = data.isGm == 1;
            chatData.voiceKey = data.voiceKey;
            chatData.voiceDuration = data.voiceDuration;
            chatData.bVoice = !string.IsNullOrEmpty(data.voiceKey) && (data.voiceKey.Length > 1);
            chatData.bHorn = false;
            chatData.bRedPacket = (data.mask & (uint)Protocol.ChatMask.CHAT_MASK_RED_PACKET) == (uint)Protocol.ChatMask.CHAT_MASK_RED_PACKET;
            chatData.timeStamp = TimeManager.GetInstance().GetServerTime();
            chatData.isShowTimeStamp = false;
            chatData.bAddFriend = (data.mask & (uint)ChatMask.CHAT_MASK_ADD_FRIEND) == (uint)ChatMask.CHAT_MASK_ADD_FRIEND;
            chatData.headFrame = data.headFrame;
            chatData.zoneId = data.zoneId;

            var stringBuilder = StringBuilderCache.Acquire();
            LinkParse._TryToken(stringBuilder, chatData.word, 0, null);
            var temp = stringBuilder.ToString();
            chatData.height = (int)(GetContentHeightByGenerator(temp) + 0.50f);
            StringBuilderCache.Release(stringBuilder);

            return chatBlock;
        }

        private ChatBlock _AllocChatData()
        {
            m_bDirty = true;
            ChatBlock chatBlock = null;
            if (m_akChatDataPool.Count < CHAT_BLOCK_MAX_SIZE)
            {
                chatBlock = new ChatBlock
                {
                    chatData = new ChatData(),
                    eType = ChatBlockType.CBT_NEW,
                    iPreID = 0,
                    //iOrder = m_akChatDataPool.Count,
                };
                m_akChatDataPool.Enqueue(chatBlock);
                return chatBlock;
            }

            chatBlock = m_akChatDataPool.Dequeue();
            
            if(chatBlock.eType == ChatBlockType.CBT_KEEP)
            {
                chatBlock.iPreID = (ulong)chatBlock.chatData.guid;
                chatBlock.eType = ChatBlockType.CBT_REBUILD;
            }
            else if(chatBlock.eType == ChatBlockType.CBT_NEW)
            {
                chatBlock.eType = ChatBlockType.CBT_NEW;
            }
            else if(chatBlock.eType == ChatBlockType.CBT_REBUILD)
            {
                chatBlock.eType = ChatBlockType.CBT_REBUILD;
            }

            return chatBlock;
        }

        Font font = null;
        int m_fontsize = 27;
        FontStyle m_eFontStyle = FontStyle.Normal;
        float m_fChatFrameMaxWidth = 244.0f;
        void _LoadChatFrameFont()
        {
            string path = TR.Value("dungeon_battle_chat_frame_font");
            font = AssetLoader.instance.LoadRes(TR.Value("dungeon_battle_chat_frame_font"), typeof(Font)).obj as Font;
            if (null == font)
            {
                Logger.LogErrorFormat("font cant not be found {0}", TR.Value("ChatFrameFont"));
                return;
            }
            
            int.TryParse(TR.Value("dungeon_battle_chat_frame_font_size"), out m_fontsize);
            int fontStyle = (int)FontStyle.Normal;
            m_eFontStyle = FontStyle.Normal;
            if (int.TryParse(TR.Value("dungeon_battle_chat_frame_font_type"), out fontStyle) &&
                fontStyle >= (int)FontStyle.Normal && fontStyle <= (int)FontStyle.BoldAndItalic)
            {
                m_eFontStyle = (FontStyle)fontStyle;
            }
            float.TryParse(TR.Value("dungeon_battle_chat_frame_text_max_width"), out m_fChatFrameMaxWidth);
        }

        TextGenerator cachedTextGenerator = null;
        TextGenerationSettings textGeneratorSetting = new TextGenerationSettings();
        public float GetContentHeightByGenerator(string text)
        {
            if (null == cachedTextGenerator)
            {
                cachedTextGenerator = new TextGenerator(256);
                textGeneratorSetting.font = font;
                textGeneratorSetting.fontSize = m_fontsize;
                textGeneratorSetting.fontStyle = m_eFontStyle;
                textGeneratorSetting.lineSpacing = 1.0f;
                textGeneratorSetting.horizontalOverflow = HorizontalWrapMode.Wrap;
                textGeneratorSetting.verticalOverflow = VerticalWrapMode.Overflow;
                textGeneratorSetting.alignByGeometry = false;
                textGeneratorSetting.resizeTextForBestFit = false;
                textGeneratorSetting.richText = true;
                textGeneratorSetting.scaleFactor = 1.0f;
                textGeneratorSetting.updateBounds = false;
                textGeneratorSetting.generationExtents = new Vector2(244.0f, 0.0f);
            }
            
            float h = cachedTextGenerator.GetPreferredHeight(string.Format("XX",text), textGeneratorSetting);
            return h;
        }

        Queue<ChatBlock> m_akChatDataPool = new Queue<ChatBlock>();
        ChatBlock[] mChatDataArray;
        bool m_bDirty = false;
        const int CHAT_BLOCK_MAX_SIZE = 20;

        protected int m_uniqueid;
    }
}

