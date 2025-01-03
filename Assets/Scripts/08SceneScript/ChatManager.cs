using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.ComponentModel;
using ProtoTable;

namespace GameClient
{
    public enum ChatType
    {
        [DescriptionAttribute("全部")]
        [EnumCommonAttribute("<color=#0391FF>[全部]</color>")]
        CT_ALL = 0,
        [DescriptionAttribute("系统")]
        [EnumCommonAttribute("<color=#FF00FF>[系统]</color>")]
        CT_SYSTEM = 1,
        [DescriptionAttribute("世界")]
        [EnumCommonAttribute("<color=#00FF00>[世界]</color>")]
        CT_WORLD = 2,
        [DescriptionAttribute("附近")]
        [EnumCommonAttribute("<color=#FFFF00>[附近]</color>")]
        CT_NORMAL = 3,
        [DescriptionAttribute("公会")]
        [EnumCommonAttribute("<color=#0391FF>[公会]</color>")]
        CT_GUILD = 4,
        [DescriptionAttribute("队伍")]
        [EnumCommonAttribute("<color=#0391FF>[队伍]</color>")]
        CT_TEAM = 5,
        [DescriptionAttribute("密聊")]
        [EnumCommonAttribute("<color=#0391FF>[密聊]</color>")]
        CT_PRIVATE_LIST = 6,
        CT_PRIVATE = 7,
        [DescriptionAttribute("组队")]
        [EnumCommonAttribute("<color=#0391FF>[组队]</color>")]
        CT_ACOMMPANY = 8,
        CT_PK3V3_ROOM = 9,

        [DescriptionAttribute("跨服")]
        [EnumCommonAttribute("<color=#FFFF00>[跨服]</color>")]
        CT_TEAM_COPY_PREPARE = 10,    // 团本准备场景频道（附近频道）

        [DescriptionAttribute("团队")]
        [EnumCommonAttribute("<color=#0391FF>[团队]</color>")]
        CT_TEAM_COPY_TEAM = 11,       // 团本团队

        [DescriptionAttribute("小队")]
        [EnumCommonAttribute("<color=#0391FF>[小队]</color>")]
        CT_TEAM_COPY_SQUAD = 12,      // 团本小队

        CT_MAX_WORDS,
    }
    public enum ChatBlockType
    {
        CBT_NEW = 0,//需要新建
        CBT_REBUILD = 1,//需要重建
        CBT_KEEP,//需要放在正确位置
        CBT_COUNT,
        CBT_SIZE = 30,
    }

    public enum PrivateChatLevelLimit
    {
        LessLevelTen,
        LessLevelTwenty,
        LessLevelThirty,
        LessLevelForty,
    }

    public class ChatBlock
    {
        public ChatData chatData;
        public ChatBlockType eType;
        public ulong iPreID;
        public int iOrder;
    }

    public class ChatManager : DataManager<ChatManager>
    {
        public static int[] ms_sort_orders = new int[(int)ChatType.CT_MAX_WORDS]
        {
            8,0,1,2,3,4,5,6,7,9,10,11,12
        };
        public static int GetMapIndex(int iIndex)
        {
            return ms_sort_orders[iIndex];
        }

        private static bool isAcceptStrangerInfo = true;
        /// <summary>
        /// 是否接受陌生人消息 等于false不接受  等于true接受
        /// </summary>
        public static bool IsAcceptStrangerInfo
        {
            get { return isAcceptStrangerInfo; }
            set { isAcceptStrangerInfo = value; }
        }

        public static int ms_accompany_cool_down
        {
            get
            {
                var functionValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TEAM_CHAT_COOL_TIME);
                if (functionValue != null)
                {
                    return functionValue.Value;
                }
                return 15;
            }
        }

        static int ms_accompany_cool_time = 0;
        public static int accompany_cool_time
        {
            get
            {
                return ms_accompany_cool_time;
            }
        }

        public static bool accompany_chat_try_enter_cool_down()
        {
            if (ms_accompany_cool_time <= 0)
            {
                ms_accompany_cool_time = ms_accompany_cool_down;
                InvokeMethod.RmoveInvokeIntervalCall(ms_accompany_cool_time);
                InvokeMethod.InvokeInterval(ms_accompany_cool_time,
                    0.0f,
                    1.0f,
                    ms_accompany_cool_down,
                    () => { ms_accompany_cool_time = ms_accompany_cool_down; },
                    () => { ms_accompany_cool_time -= 1; if (ms_accompany_cool_time < 0) ms_accompany_cool_time = 0; },
                    () => { ms_accompany_cool_time = 0; });
                return true;
            }
            return false;
        }

        public static int ms_arround_cool_down
        {
            get
            {
                var functionValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_ARROUND_CHAT_COOL_TIME);
                if (functionValue != null)
                {
                    return functionValue.Value;
                }
                return 15;
            }
        }

        public static int ms_teamcopy_prepare_cool_down
        {
            get
            {                
                return Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_TEAMCOPY_PREPARE_CHANNEL_CD,30);
            }
        }

        public static int ms_teamcopy_team_cool_down
        {
            get
            {
                return Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_TEAMCOPY_TEAM_CHANNEL_CD,5);
            }
        }

        public static int ms_teamcopy_squad_cool_down
        {
            get
            {
                return Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_TEAMCOPY_SQUAD_CHANNEL_CD,5);
            }
        }

        static int ms_arround_cool_time = 0;
        public static int arround_cool_time
        {
            get
            {
                return ms_arround_cool_time;
            }
        }

        static int ms_teamcopy_prepare_cool_time = 0;
        public static int teamcopy_prepare_cool_time
        {
            get
            {
                return ms_teamcopy_prepare_cool_time;
            }
        }

        static int ms_teamcopy_team_cool_time = 0;
        public static int teamcopy_team_cool_time
        {
            get
            {
                return ms_teamcopy_team_cool_time;
            }
        }

        static int ms_teamcopy_squad_cool_time = 0;
        public static int teamcopy_squad_cool_time
        {
            get
            {
                return ms_teamcopy_squad_cool_time;
            }
        }

        public static bool arround_chat_try_enter_cool_down()
        {
            if (ms_arround_cool_time <= 0)
            {
                ms_arround_cool_time = ms_arround_cool_down;
                InvokeMethod.RmoveInvokeIntervalCall(ms_arround_cool_time);
                InvokeMethod.InvokeInterval(ms_arround_cool_time,
                    0.0f,
                    1.0f,
                    ms_arround_cool_down,
                    () => { ms_arround_cool_time = ms_arround_cool_down; },
                    () => { ms_arround_cool_time -= 1; if (ms_arround_cool_time < 0) ms_arround_cool_time = 0; },
                    () => { ms_arround_cool_time = 0; });
                return true;
            }
            return false;
        }

        public static bool teamcopy_prepare_chat_try_enter_cool_down()
        {
            if (ms_teamcopy_prepare_cool_time <= 0)
            {
                ms_teamcopy_prepare_cool_time = ms_teamcopy_prepare_cool_down;
                InvokeMethod.RmoveInvokeIntervalCall(ms_teamcopy_prepare_cool_time);
                InvokeMethod.InvokeInterval(ms_teamcopy_prepare_cool_time,
                    0.0f,
                    1.0f,
                    ms_teamcopy_prepare_cool_down,
                    () => { ms_teamcopy_prepare_cool_time = ms_teamcopy_prepare_cool_down; },
                    () => { ms_teamcopy_prepare_cool_time -= 1; if (ms_teamcopy_prepare_cool_time < 0) ms_teamcopy_prepare_cool_time = 0; },
                    () => { ms_teamcopy_prepare_cool_time = 0; });
                return true;
            }
            return false;
        }

        public static bool teamcopy_team_chat_try_enter_cool_down()
        {
            if (ms_teamcopy_team_cool_time <= 0)
            {
                ms_teamcopy_team_cool_time = ms_teamcopy_team_cool_down;
                InvokeMethod.RmoveInvokeIntervalCall(ms_teamcopy_team_cool_time);
                InvokeMethod.InvokeInterval(ms_teamcopy_team_cool_time,
                    0.0f,
                    1.0f,
                    ms_teamcopy_team_cool_down,
                    () => { ms_teamcopy_team_cool_time = ms_teamcopy_team_cool_down; },
                    () => { ms_teamcopy_team_cool_time -= 1; if (ms_teamcopy_team_cool_time < 0) ms_teamcopy_team_cool_time = 0; },
                    () => { ms_teamcopy_team_cool_time = 0; });
                return true;
            }
            return false;
        }

        public static bool teamcopy_squad_chat_try_enter_cool_down()
        {
            if (ms_teamcopy_squad_cool_time <= 0)
            {
                ms_teamcopy_squad_cool_time = ms_teamcopy_squad_cool_down;
                InvokeMethod.RmoveInvokeIntervalCall(ms_teamcopy_squad_cool_time);
                InvokeMethod.InvokeInterval(ms_teamcopy_squad_cool_time,
                    0.0f,
                    1.0f,
                    ms_teamcopy_squad_cool_down,
                    () => { ms_teamcopy_squad_cool_time = ms_teamcopy_squad_cool_down; },
                    () => { ms_teamcopy_squad_cool_time -= 1; if (ms_teamcopy_squad_cool_time < 0) ms_teamcopy_squad_cool_time = 0; },
                    () => { ms_teamcopy_squad_cool_time = 0; });
                return true;
            }
            return false;
        }

        public static int ms_world_cool_down
        {
            get
            {
                var functionValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLD_CHAT_COOL_TIME);
                if(functionValue != null)
                {
                    return functionValue.Value;
                }
                return 15;
            }
        }

        static int ms_world_cool_time = 0;
        public static int world_cool_time
        {
            get
            {
                return ms_world_cool_time;
            }
        }

        public static bool world_chat_try_enter_cool_down()
        {
            if(ms_world_cool_time <= 0)
            {
                ms_world_cool_time = ms_world_cool_down;
                InvokeMethod.RmoveInvokeIntervalCall(ms_world_cool_time);
                InvokeMethod.InvokeInterval(ms_world_cool_time,
                    0.0f,
                    1.0f, 
                    ms_world_cool_down,
                    () => { ms_world_cool_time = ms_world_cool_down; },
                    () => { ms_world_cool_time -= 1; if (ms_world_cool_time < 0) ms_world_cool_time = 0; },
                    () => { ms_world_cool_time = 0; });
                return true;
            }
            return false;
        }

        public static int WorldChatCostActivityValue
        {
            get
            {
                var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLDCHAT_ACT_NEED);
                if(functionData != null)
                {
                    return functionData.Value;
                }
                return 0;
            }
        }

        public int FreeWorldChatLeftTimes
        {
            get
            {
                int iRet = 0;
                int iUsedTimes = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_WORLD_FREE_CHAT_TIMES);
                int iMax = (int)Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.WORLD_CHAT_FREE_TIMES);
                iRet = iMax - iUsedTimes;
                iRet = (int)IntMath.Clamp(iRet, 0, iMax);
                return iRet;
            }
        }

        public bool CheckWorldActivityValueEnough()
        {
            if(WorldChatCostActivityValue > PlayerBaseData.GetInstance().ActivityValue)
            {
                return false;
            }
            return true;
        }
        #region delegate
        public delegate void OnAddChatData(ChatBlock data);
        public delegate void OnRebuildChatData(ulong pre, ChatBlock current);
        public delegate void OnAddGlobalChatData(ChatBlock data);
        public delegate void OnRebuildGlobalChatData(ulong pre, ChatBlock current);

        public OnAddChatData onAddChatdata;
        public OnRebuildChatData onRebuildChatData;
        public OnAddGlobalChatData onAddGlobalChatData;
        public OnRebuildGlobalChatData onRebuildGlobalChatData;

        public Dictionary<ulong, int> _recvPrivateMsgNum = new Dictionary<ulong, int>();
        UIEventRecvPrivateChat _eventChat = new UIEventRecvPrivateChat(true);
        #endregion

        #region process

        void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(SceneSyncChat.MsgID, _OnSyncChat);
            NetProcess.AddMsgHandler(WorldChatHorn.MsgID, _WorldChatHorn);
            NetProcess.AddMsgHandler(TeamCopyTeamQuitRes.MsgID, _TeamCopyTeamQuitRes);
         
        }

        void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(SceneSyncChat.MsgID, _OnSyncChat);
            NetProcess.RemoveMsgHandler(WorldChatHorn.MsgID, _WorldChatHorn);
            NetProcess.RemoveMsgHandler(TeamCopyTeamQuitRes.MsgID, _TeamCopyTeamQuitRes);
        }

        public override void Initialize()
        {
            RegisterNetHandler();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleIdChanged, _OnRoleIdChanged);
            ChatRecordManager.GetInstance().onLoadPrivateChatDataOK += _OnLoadPrivateChatDataOK;
            _LoadChatFrameFont();
            privateChatLimitLevel = GetPrivateChatLimitLevel();
        }

        #region font
        Font font = null;
        int m_fontsize = 28;
        FontStyle m_eFontStyle = FontStyle.Normal;
        float m_fChatFrameMaxWidth = 646.0f;

        int m_chatNewFontSize = 28;
        void _LoadChatFrameFont()
        {
            string path = TR.Value("chat_frame_font");
            font = AssetLoader.instance.LoadRes(TR.Value("chat_frame_font"),typeof(Font)).obj as Font;
            if(null == font)
            {
                Logger.LogErrorFormat("font cant not be found {0}", TR.Value("ChatFrameFont"));
                return;
            }

            m_fontsize = 28;
            int.TryParse(TR.Value("chat_frame_font_size"), out m_fontsize);
            int fontStyle = (int)FontStyle.Normal;
            m_eFontStyle = FontStyle.Normal;
            if (int.TryParse(TR.Value("chat_frame_font_type"),out fontStyle) &&
                fontStyle >= (int)FontStyle.Normal && fontStyle <= (int)FontStyle.BoldAndItalic)
            {
                m_eFontStyle = (FontStyle)fontStyle;
            }
            float.TryParse(TR.Value("chat_frame_text_max_width"), out m_fChatFrameMaxWidth);
        }

        TextGenerator cachedTextGenerator = null;
        TextGenerationSettings textGeneratorSetting = new TextGenerationSettings();
        public float GetContentHeightByGenerator(string text)
        {
            if(null == cachedTextGenerator)
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
                textGeneratorSetting.generationExtents = new Vector2(650.0f, 0.0f);
            }

            
            float h = cachedTextGenerator.GetPreferredHeight(text, textGeneratorSetting);
// #if UNITY_EDITOR
//             float w = cachedTextGenerator.GetPreferredWidth(text, textGeneratorSetting);
//             Logger.LogErrorFormat("width0 = {0} heigth0={1}", w,h);
// #endif

            return h;
        }
        //新聊天界面用
        TextGenerator cachedTextGeneratorNew = null;
        TextGenerationSettings textGeneratorSettingNew = new TextGenerationSettings();
        public float GetContentHeightByGeneratorNew(string text)
        {
            if (null == cachedTextGeneratorNew)
            {
                cachedTextGeneratorNew = new TextGenerator(256);
                textGeneratorSettingNew.font = font;
                textGeneratorSettingNew.fontSize = m_chatNewFontSize;
                textGeneratorSettingNew.fontStyle = m_eFontStyle;
                textGeneratorSettingNew.lineSpacing = 1.0f;
                textGeneratorSettingNew.horizontalOverflow = HorizontalWrapMode.Wrap;
                textGeneratorSettingNew.verticalOverflow = VerticalWrapMode.Overflow;
                textGeneratorSettingNew.alignByGeometry = false;
                textGeneratorSettingNew.resizeTextForBestFit = false;
                textGeneratorSettingNew.richText = true;
                textGeneratorSettingNew.scaleFactor = 1.0f;
                textGeneratorSettingNew.updateBounds = false;
                textGeneratorSettingNew.generationExtents = new Vector2(650.0f, 0.0f);
            }


            float h = cachedTextGeneratorNew.GetPreferredHeight(text, textGeneratorSettingNew);
            // #if UNITY_EDITOR
            //             float w = cachedTextGenerator.GetPreferredWidth(text, textGeneratorSetting);
            //             Logger.LogErrorFormat("width0 = {0} heigth0={1}", w,h);
            // #endif

            return h;
        }
        #endregion

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.ChatDataManager;
        }

        void _UpdateGlobalData()
        {
            if (!m_bDirty)
            {
                return;
            }

            for(int i = 0; i < m_akChatDataPool.Count; ++i)
            {
                var chatBlock = m_akChatDataPool[i];
                if (chatBlock.eType == ChatBlockType.CBT_NEW)
                {
                    if (onAddGlobalChatData != null)
                    {
                        onAddGlobalChatData(chatBlock);
                    }
                }
                else if (chatBlock.eType == ChatBlockType.CBT_REBUILD)
                {
                    if (onRebuildGlobalChatData != null)
                    {
                        onRebuildGlobalChatData(chatBlock.iPreID, chatBlock);
                    }
                }
            }

            m_bDirty = false;
        }
        void _UpdateLocalData()
        {
            for(int i = 0; i < (int)ChanelType.CHAT_CHANNEL_MAX; ++i)
            {
                var current = m_chanelChatData[i];
                if(current != null && current.bDirty)
                {
                    for(int j = 0; j < current.cacheBlocks.Count; ++j)
                    {
                        var chatBlock = current.cacheBlocks[j];
                        if (chatBlock.eType == ChatBlockType.CBT_NEW)
                        {
                            if (onAddChatdata != null)
                            {
                                onAddChatdata(chatBlock);
                            }
                        }
                        else if (chatBlock.eType == ChatBlockType.CBT_REBUILD)
                        {
                            if (onRebuildChatData != null)
                            {
                                onRebuildChatData(chatBlock.iPreID, chatBlock);
                            }
                        }
                    }
                    current.bDirty = false;
                }
            }
        }

        public void Update()
        {
            ++m_iUpdateFrame;
            if (m_iUpdateFrame == 1)
            {
                _UpdateGlobalData();
            }
            else if (m_iUpdateFrame == 2)
            {
                _UpdateLocalData();
            }

            if(m_iUpdateFrame >= 2)
            {
                m_iUpdateFrame = 0;
            }
        }

        public override void Clear()
        {
            UnRegisterNetHandler();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleIdChanged, _OnRoleIdChanged);
            ChatRecordManager.GetInstance().onLoadPrivateChatDataOK -= _OnLoadPrivateChatDataOK;
            _recvPrivateMsgNum.Clear();
            for(int i = 0; i < (int)ChanelType.CHAT_CHANNEL_MAX; ++i)
            {
                m_chanelChatData[i].Clear();
            }
            m_akObjid2Data.Clear();
            m_dataCacheMax = 20;
            m_uniqueid = 1200121234;
            m_akGlobalChatDatas.Clear();
            m_bShareEquipmentLocked = false;
            m_akCachedDatas.Clear();
            m_akCachedChatDatas.Clear();

            m_bDirty = false;
            m_akChatDataPool.Clear();
            m_iDealPos = 0;
            m_iRollEnd = 0;
            m_iUpdateFrame = 0;
            font = null;
            isAcceptStrangerInfo = true;

            RemoveAllSyncVoiceChatListener();
        }
        #endregion

        #region netMsg
        //[MessageHandle(SceneSyncChat.MsgID)]
        void _OnSyncChat(MsgDATA msg)
        {
			#if DEBUG_REPORT_ROOT
			if (DebugSettings.instance.DisableChatDisplay)
				return;
			#endif

            int pos = 0;
            SceneSyncChat msgRet = new SceneSyncChat();
            msgRet.decode(msg.bytes, ref pos);
  
            // 不在团本场景来了团本聊天消息，过滤之
            if (IsTeamCopyChannel((ChanelType)msgRet.channel) && !TeamDuplicationUtility.IsInTeamDuplicationScene() && !BeUtility.IsRaidBattle()) 
            {
                return;
            }

            // 在团本场景来的不是团本聊天，也过滤之
            if (!IsTeamCopyChannel((ChanelType)msgRet.channel) && TeamDuplicationUtility.IsInTeamDuplicationScene() && !BeUtility.IsRaidBattle()) 
            {
                return;
            }

            ChatBlock chatData = _NetData2LocalData(msgRet);

// #if UNITY_EDITOR
//             Logger.LogErrorFormat("<color=#00ff00>chat vip level = [{0}]</color>", msgRet.viplvl);
// #endif

            _OnDealChatData(chatData);

            if (msgRet.channel == (byte)ChanelType.CHAT_CHANNEL_TEAM_COPY_TEAM || msgRet.channel == (byte)ChanelType.CHAT_CHANNEL_TEAM_COPY_SQUAD)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationChatContentMessage, msgRet.objid, msgRet.word);
            }

            //团本场景中团本相关频道收到数据，进行刷新
            if (TeamDuplicationUtility.IsInTeamDuplicationScene() == true)
            {
                if (IsTeamCopyChannel((ChanelType) msgRet.channel) == true)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshChatData);
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChallengeTeamChatDataUpdate);

        }

        //[MessageHandle(WorldChatHorn.MsgID)]
        void _WorldChatHorn(MsgDATA msg)
        {
			#if DEBUG_REPORT_ROOT
			if (DebugSettings.instance.DisableChatDisplay)
				return;
			#endif
			
            int pos = 0;
            WorldChatHorn msgRet = new WorldChatHorn();
            msgRet.decode(msg.bytes, ref pos);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.WordChatHorn, msgRet.info);

            ChatBlock chatData = _WorldHorn2LocalData(msgRet.info);

            _OnDealChatData(chatData);
        }

        public void ClearChannelChatData(ChanelType chanelType)
        {
            if (m_chanelChatData == null)
            {
                return;
            }

            int channel = (int)chanelType;
            if(channel >= m_chanelChatData.Length)
            {
                return;
            }

            ChatBlockConfig chatBlockConfig = m_chanelChatData[channel];
            if(chatBlockConfig == null)
            {
                return;
            }

            chatBlockConfig.Clear();           
            chatBlockConfig.bDirty = true;
        }

        void _TeamCopyTeamQuitRes(MsgDATA msgData)
        {
            if (msgData == null)
            {
                return;
            }

            TeamCopyTeamQuitRes teamQuitRes = new TeamCopyTeamQuitRes();
            teamQuitRes.decode(msgData.bytes);

            if (teamQuitRes.retCode != (uint)ProtoErrorCode.SUCCESS)
            {                
                return;
            }

            // 退出团本后清空团队 小队频道的数据
            ClearChannelChatData(ChanelType.CHAT_CHANNEL_TEAM_COPY_TEAM);
            ClearChannelChatData(ChanelType.CHAT_CHANNEL_TEAM_COPY_SQUAD);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshChatData);
        }

        public static bool IsTeamCopyChannel(ChanelType chanelType)
        {
            if(chanelType == ChanelType.CHAT_CHANNEL_TEAM_COPY_PREPARE 
                || chanelType == ChanelType.CHAT_CHANNEL_TEAM_COPY_TEAM
                || chanelType == ChanelType.CHAT_CHANNEL_TEAM_COPY_SQUAD)
            {
                return true;
            }

            return false;
        }

        void _OnDealChatData(ChatBlock chatData)
        {
            if (chatData == null)
            {
                return;
            }

            _AddChatData(chatData);

            OnVoiceChatCome(chatData.chatData);
        }
        #endregion

        #region var
        public class ChatBlockConfig
        {
            public List<ChatBlock> cacheBlocks = new List<ChatBlock>();
            public int iDealPos = 0;
            public int iRollEnd = 0;
            public bool bDirty = false;
            public void Clear()
            {
                iDealPos = 0;
                iRollEnd = 0;
                bDirty = false;
                cacheBlocks.Clear();
            }
        }
        protected ChatBlockConfig[] m_chanelChatData = new ChatBlockConfig[(int)ChanelType.CHAT_CHANNEL_MAX]
        {
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),
            new ChatBlockConfig(),  
        };
        protected int m_dataCacheMax;
        protected int m_uniqueid;
        protected Dictionary<ulong, List<ChatBlock>> m_akObjid2Data = new Dictionary<ulong, List<ChatBlock>>();
        public Dictionary<ulong, List<ChatBlock>> PrivateChatObjects
        {
            get
            {
                return m_akObjid2Data;
            }
        }
        protected List<ChatData> m_akGlobalChatDatas = new List<ChatData>();
        public List<ChatData> GlobalChatDatas
        {
            get { return m_akGlobalChatDatas; }
        }
        List<ChatData> m_akCachedDatas = new List<ChatData>();

        public List<ChatBlock> GlobalChatBlock
        {
            get
            {
                return m_akChatDataPool;
            }
        }

        public int recvPrivateMsgNum { get; private set; }
        #endregion

        public List<ChatBlock> GetPrivateChat(ulong uid)
        {
            List<ChatBlock> chatDatas = null;
            if (uid != 0)
            {
                m_akObjid2Data.TryGetValue(uid, out chatDatas);
            }
            else if(m_akObjid2Data.Count > 0)
            {
                chatDatas = m_akObjid2Data[0];
            }

            return chatDatas;
        }

        public void RemovePrivateChatData(ulong uid)
        {
            if(m_akObjid2Data.ContainsKey(uid))
            {
                m_akObjid2Data.Remove(uid);
            }
        }

        public List<ChatBlock> GetChatDataByChanelType(ChanelType eChanelType)
        {
            ChatBlockConfig chatBlockConfig = null;
            if(eChanelType == ChanelType.CHAT_CHANNEL_PRIVATE)
            {
                return null;
            }
            else if(eChanelType >= 0 && eChanelType < ChanelType.CHAT_CHANNEL_MAX)
            {
                chatBlockConfig = m_chanelChatData[(int)eChanelType];
            }

            if (chatBlockConfig != null)
            {
                return chatBlockConfig.cacheBlocks;
            }
            return null;
        }

        List<ChatData> m_akCachedChatDatas = new List<ChatData>();

        private ChatBlock _AllocChatData()
        {
            m_bDirty = true;
            ChatBlock chatBlock = null;
            if (m_akChatDataPool.Count < (int)ChatBlockType.CBT_SIZE)
            {
                chatBlock = new ChatBlock
                {
                    chatData = new ChatData(),
                    eType = ChatBlockType.CBT_NEW,
                    iPreID = 0,
                    iOrder = m_akChatDataPool.Count,
                };
                m_akChatDataPool.Add(chatBlock);
                m_iRollEnd = m_akChatDataPool.Count - 1;
                return chatBlock;
            }

            chatBlock = m_akChatDataPool[0];
            m_akChatDataPool.RemoveAt(0);
            chatBlock.iOrder = m_akChatDataPool[m_akChatDataPool.Count - 1].iOrder + 1;
            m_akChatDataPool.Add(chatBlock);

            if (chatBlock.eType == ChatBlockType.CBT_KEEP)
            {
                chatBlock.iPreID = (ulong)chatBlock.chatData.guid;
                chatBlock.eType = ChatBlockType.CBT_REBUILD;
            }
            else if(chatBlock.eType == ChatBlockType.CBT_NEW)
            {
                chatBlock.eType = ChatBlockType.CBT_NEW;
            }
            else if (chatBlock.eType == ChatBlockType.CBT_REBUILD)
            {
                chatBlock.eType = ChatBlockType.CBT_REBUILD;
            }

            return chatBlock;
        }
        List<ChatBlock> m_akChatDataPool = new List<ChatBlock>();
        int m_iRollEnd = 0;
        int m_iDealPos = 0;
        bool m_bDirty = false;
        int m_iUpdateFrame = 0;

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

            if (chatData.channel == (byte)ChanelType.CHAT_CHANNEL_PRIVATE)
            {
                chatData.shortTimeString = Function.GetDateTime((int)TimeManager.GetInstance().GetServerTime());
            }
            else
            {
                chatData.shortTimeString ="[" + TimeManager.GetInstance().GetTimeT() + "]";
            }
            chatData.targetID = data.receiverId;
            chatData.eChatType = _TransChanelType((ChanelType)chatData.channel);
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
            LinkParse._TryToken(stringBuilder, chatData.word, 0,null);
            var temp = stringBuilder.ToString();
            chatData.height = (int)(GetContentHeightByGenerator(temp) + 0.50f);
            StringBuilderCache.Release(stringBuilder);

            if (IsPrivateChatStrangerLevelLimit(chatData) || IsAcceptStrangerInfo == false)
            {
                if (m_akChatDataPool != null && m_akChatDataPool.Contains(chatBlock))
                {
                    m_akChatDataPool.Remove(chatBlock);
                }
                return null;
            }

            return chatBlock;
        }

        private ChatBlock _WorldHorn2LocalData(HornInfo data)
        {
            ChatBlock chatBlock = _AllocChatData();

            ChatData chatData = chatBlock.chatData;
            chatData.channel = (int)ChanelType.CHAT_CHANNEL_WORLD;
            chatData.objid = data.roldId;
            chatData.sex = 0;
            chatData.occu = data.occu;
            chatData.level = data.level;
            chatData.viplvl = data.viplvl;
            chatData.objname = data.name;
            chatData.word = data.content;
            chatData.guid = ++m_uniqueid;
            chatData.shortTimeString = "[" + TimeManager.GetInstance().GetTimeT() + "]";
            chatData.targetID = 0;
            chatData.eChatType = _TransChanelType((ChanelType)chatData.channel);
            chatData.dirty = true;
            chatData.bLink = 1;
            chatData.bGm = false;
            chatData.voiceKey = string.Empty;
            chatData.voiceDuration = 0;
            chatData.bVoice = false;
            chatData.bHorn = true;
            chatData.bRedPacket = false;
            chatData.headFrame = data.headFrame;

            var stringBuilder = StringBuilderCache.Acquire();
            LinkParse._TryToken(stringBuilder, chatData.word, 0, null);
            var temp = stringBuilder.ToString();
            chatData.height = (int)(GetContentHeightByGenerator(temp) + 0.50f);
            StringBuilderCache.Release(stringBuilder);

            return chatBlock;
        }

        private ChatBlock _CopyBlock(ChatBlock src,ChatBlock data)
        {
            if(src != null)
            {
                src.chatData.channel = data.chatData.channel;
                src.chatData.objid = data.chatData.objid;
                src.chatData.sex = data.chatData.sex;
                src.chatData.occu = data.chatData.occu;
                src.chatData.level = data.chatData.level;
                src.chatData.viplvl = data.chatData.viplvl;
                src.chatData.objname = data.chatData.objname;
                src.chatData.word = data.chatData.word;
                src.chatData.guid = ++m_uniqueid;
                src.chatData.shortTimeString = data.chatData.shortTimeString;
                src.chatData.targetID = data.chatData.targetID;
                src.chatData.bLink = data.chatData.bLink;
                src.chatData.dirty = data.chatData.dirty;
                src.chatData.eChatType = data.chatData.eChatType;
                src.chatData.bGm = data.chatData.bGm;
                //不要覆盖PRE Type;
                //src.eType = data.eType;
                src.iOrder = data.iOrder;
                //不要覆盖PREID
                //src.iPreID = data.iPreID;
				//需要覆盖语音相关 voiceKey voiceDuration bVoice
				src.chatData.bVoice = data.chatData.bVoice;
				src.chatData.voiceKey = data.chatData.voiceKey;
				src.chatData.voiceDuration = data.chatData.voiceDuration;
                src.chatData.bHorn = data.chatData.bHorn;
                src.chatData.height = data.chatData.height;
                src.chatData.bRedPacket = data.chatData.bRedPacket;
                src.chatData.timeStamp = data.chatData.timeStamp;
                src.chatData.isShowTimeStamp = data.chatData.isShowTimeStamp;
                src.chatData.bAddFriend = data.chatData.bAddFriend;
                src.chatData.headFrame = data.chatData.headFrame;
                src.chatData.zoneId = data.chatData.zoneId;
                return src;
            }
            return new ChatBlock
            {
                chatData = new ChatData
                {
                    channel = data.chatData.channel,
                    objid = data.chatData.objid,
                    sex = data.chatData.sex,
                    occu = data.chatData.occu,
                    level = data.chatData.level,
                    viplvl = data.chatData.viplvl,
                    objname = data.chatData.objname,
                    word = data.chatData.word,
                    guid = ++m_uniqueid,
                    shortTimeString = data.chatData.shortTimeString,
                    targetID = data.chatData.targetID,
                    bLink = data.chatData.bLink,
                    dirty = data.chatData.dirty,
                    eChatType = data.chatData.eChatType,
                    bGm = data.chatData.bGm,
					//voice
					bVoice = data.chatData.bVoice,
					voiceKey = data.chatData.voiceKey,
					voiceDuration = data.chatData.voiceDuration,
                    bHorn = data.chatData.bHorn,
                    height = data.chatData.height,
                    bRedPacket = data.chatData.bRedPacket,
                    timeStamp = data.chatData.timeStamp,
                    isShowTimeStamp = data.chatData.isShowTimeStamp,
                    bAddFriend = data.chatData.bAddFriend,
                    headFrame = data.chatData.headFrame,
                    zoneId = data.chatData.zoneId,
                },
                eType = ChatBlockType.CBT_NEW,
                iOrder = data.iOrder,
                iPreID = 0,
            };
        }

        const int teamCopyChannelMaxMsgCount = 50;

        int GetChannelMaxMsgCount(ChanelType chanelType)
        {
            if(chanelType == ChanelType.CHAT_CHANNEL_TEAM_COPY_PREPARE)
            {                
                return Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_TEAMCOPY_PREPARE_CHANNEL_MAX_MSG_COUNT,teamCopyChannelMaxMsgCount);
            }

            if (chanelType == ChanelType.CHAT_CHANNEL_TEAM_COPY_TEAM)
            {
                return Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_TEAMCOPY_TEAM_CHANNEL_MAX_MSG_COUNT, teamCopyChannelMaxMsgCount);
            }

            if (chanelType == ChanelType.CHAT_CHANNEL_TEAM_COPY_SQUAD)
            {
                return Utility.GetSystemValueFromTable(SystemValueTable.eType3.SVT_TEAMCOPY_SQUAD_CHANNEL_MAX_MSG_COUNT, teamCopyChannelMaxMsgCount);
            }

            return m_dataCacheMax;
        }

        private void _AddChatData(ChatBlock data)
        {
            if (data == null)
            {
                return;
            }
            if(data.chatData.channel < 0 || data.chatData.channel >= (int)ChanelType.CHAT_CHANNEL_MAX)
            {
                return;
            }

            if(data.chatData.channel == (int)ChanelType.CHAT_CHANNEL_AROUND)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PopChatMsg, data);
            }

            //私聊
            if (data.chatData.channel == (int)ChanelType.CHAT_CHANNEL_PRIVATE)
            {
                ChatBlock copyData = _CopyBlock(null, data);
                AddPrivateChatData(copyData);
                ChatRecordManager.GetInstance().AddPrivateChatData(copyData.chatData);

                bool self = false;
                ulong tarUid = 0;
                if (copyData.chatData.objid == PlayerBaseData.GetInstance().RoleID)
                {
                    self = true;
                    tarUid = copyData.chatData.targetID;
                }
                else if (copyData.chatData.targetID == PlayerBaseData.GetInstance().RoleID)
                {
                    tarUid = copyData.chatData.objid;
                }
                RelationData rd = RelationDataManager.GetInstance().GetRelationByRoleID(tarUid);
                if (rd == null && !self)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RoleChatDirtyChanged,tarUid, true);
                }
                return;
            }

            ChatBlockConfig chatBlockConfig = m_chanelChatData[data.chatData.channel];
            chatBlockConfig.bDirty = true;
            ChatBlock chatBlock = null;            

            int msgMaxCount = GetChannelMaxMsgCount((ChanelType)data.chatData.channel);     

            if (chatBlockConfig.cacheBlocks.Count < msgMaxCount)
            {
                chatBlock = _CopyBlock(null,data);
                chatBlock.chatData.guid = ++m_uniqueid;
                chatBlockConfig.cacheBlocks.Add(chatBlock);
                chatBlockConfig.iRollEnd = chatBlockConfig.cacheBlocks.Count - 1;
                chatBlock.iOrder = chatBlockConfig.cacheBlocks.Count;
                chatBlock.eType = ChatBlockType.CBT_NEW;
            }
            else
            {
                chatBlock = chatBlockConfig.cacheBlocks[0];
                chatBlockConfig.cacheBlocks.RemoveAt(0);
                var endBlock = chatBlockConfig.cacheBlocks[chatBlockConfig.cacheBlocks.Count - 1];
                _CopyBlock(chatBlock, data);
                chatBlock.iOrder = endBlock.iOrder + 1;
                chatBlockConfig.cacheBlocks.Add(chatBlock);

                if (chatBlock.eType == ChatBlockType.CBT_KEEP)
                {
                    chatBlock.eType = ChatBlockType.CBT_REBUILD;
                }
                else if (chatBlock.eType == ChatBlockType.CBT_NEW)
                {
                    chatBlock.eType = ChatBlockType.CBT_NEW;
                }
                else if (chatBlock.eType == ChatBlockType.CBT_REBUILD)
                {
                    chatBlock.eType = ChatBlockType.CBT_REBUILD;
                }
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.LockStatuNewMessageNumber, data);
        }

        public bool IsDirtyFriendChat(ulong uid)
        {
//             List<ChatData> chatDatas = null;
//             if (m_akObjid2Data.TryGetValue(uid, out chatDatas))
//             {
//                 for(int i = 0; i < chatDatas.Count; ++i)
//                 {
//                     if(chatDatas[i].dirty == true)
//                     {
//                         return true;
//                     }
//                 }
//             }

            return false;
        }

        public bool IsDirtyFriendChat()
        {
            if (_recvPrivateMsgNum.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ReduceRecvPrivateMsgNum(ulong uid)
        {
            if(true == _recvPrivateMsgNum.ContainsKey(uid))
            {
                _recvPrivateMsgNum.Remove(uid);
            }
            
        }

        public void SetClean(ulong uid)
        {
//             List<ChatData> chatDatas = null;
//             if (m_akObjid2Data.TryGetValue(uid, out chatDatas))
//             {
//                 for (int i = 0; i < chatDatas.Count; ++i)
//                 {
//                     chatDatas[i].dirty = false;
//                 }
//             }
        }

        bool m_bShareEquipmentLocked = false;
        public void ShareEquipment(ItemData data, ChatType type = ChatType.CT_WORLD)
        {
            if(data == null || m_bShareEquipmentLocked)
            {
                if(m_bShareEquipmentLocked)
                {
#if APPLE_STORE
                    if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SHARE_TEXT_CHANGE))
                    {
                        SystemNotifyManager.SysNotifyTextAnimation("展示太频繁了休息一会儿吧！");
                    }
                    else
                    {
#endif
                        SystemNotifyManager.SystemNotify(5012);
#if APPLE_STORE
                    }
#endif
                }
                return;
            }
            m_bShareEquipmentLocked = true;

            string content = "{" + string.Format("I {0} {1} {2}",data.GUID,data.TableID,data.StrengthenLevel) + "}";
            SendChat(type, content,0,1);

            ChatFrame cframe = ClientSystemManager.GetInstance().GetFrame(typeof(ChatFrame)) as ChatFrame;
            if(cframe != null)
            {
                cframe.world_chat_try_enter_cool_down();
            }

#if APPLE_STORE			
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SHARE_TEXT_CHANGE))
            {
                SystemNotifyManager.SysNotifyTextAnimation("展示成功！请前往世界频道查看！");
            }
            else
            {
                SystemNotifyManager.SystemNotify(5011);
            }
#endif  

            InvokeMethod.Invoke(this, 15.0f, () =>
              {
                  m_bShareEquipmentLocked = false;
              });
        }

        public void SendChat(ChatType type, string content, UInt64 tarId = 0,byte share = 0)
        {
            ChanelType eChanel = _TransChatType(type);
            if (eChanel >= ChanelType.CHAT_CHANNEL_AROUND && eChanel < ChanelType.CHAT_CHANNEL_MAX)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    //添加客户端CM指令
                    int findIndex = content.IndexOf("-cm ");
                    if (findIndex != -1)
                    {
                        string strCmd = content.Substring(findIndex + 4, content.Length - 4);
                        int cmdIndex = strCmd.IndexOf("suitwear");
                        if (cmdIndex != -1)
                        {
                            string strIntParam = strCmd.Substring(cmdIndex + 9, strCmd.Length - 9);
                            string[] IntParamArray = strIntParam.Split(' ');
                            if (IntParamArray.Length == 4)
                            {
                                int suitId = 0;
                                int.TryParse(IntParamArray[0].Substring(3), out suitId);
                                int strengTh = 0;
                                int.TryParse(IntParamArray[1].Substring(4), out strengTh);
                                int typ = 0;
                                int.TryParse(IntParamArray[2].Substring(4), out typ);
                                int attr = 0;
                                int.TryParse(IntParamArray[3].Substring(5), out attr);
                                Utility.UseOneKeySuitWear(suitId, strengTh, typ, attr);
                            }
                        }
                        cmdIndex = strCmd.IndexOf("ui");
                        if (cmdIndex != -1)
                        {
                            string strIntParam = strCmd.Substring(cmdIndex + 3, strCmd.Length - 3);
                            string[] IntParamArray = strIntParam.Split(' ');
                            if (IntParamArray.Length == 1)
                            {
                                int param1 = 0;
                                if (int.TryParse(IntParamArray[0], out param1))
                                {
                                    var battle = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemBattle;
                                    if(battle != null)
                                    {
                                        battle.BattleUIComponentManager.EnableUI(param1 != 0);
                                    }
                                }
                            }
                        }
                        cmdIndex = strCmd.IndexOf("chasing ");
                        if (cmdIndex != -1)
                        {
                            string strIntParam = strCmd.Substring(cmdIndex + 8, strCmd.Length - 8);
                            string[] IntParamArray = strIntParam.Split(' ');
                            if (IntParamArray.Length == 4)
                            {
                                int param1 = 0;
                                int param2 = 0;
                                int param3 = 0;
                                int param4 = 0;
                                if (int.TryParse(IntParamArray[0], out param1) && int.TryParse(IntParamArray[1], out param2) &&
                                    int.TryParse(IntParamArray[2], out param3) && int.TryParse(IntParamArray[3], out param4))
                                {
                                    FrameSync.instance.SetChasingMoniterParam(param1, param2, param3, param4);
                                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("设置 SetChasingMoniterParam {0} - {1} - {2} - {3}", param1, param2, param3, param4));
                                }
                            }
                        }
                        return;
                    }
                    SceneChat req = new SceneChat();
                    req.channel = (byte)eChanel;
                    req.targetId = tarId;
                    req.word = content;
                    req.bLink = LinkParse.IsLink(content) ? (byte)1 : (byte)0;
                    req.voiceKey = string.Empty;
                    req.voiceDuration = 0;
					req.isShare = share;

                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
                }
            }
        }

        public void SendVoiceChat(ChatType type,string voiceKey,string content,byte voiceTimeLength, UInt64 tarId = 0)
        {
            ChanelType eChanel = _TransChatType(type);
            if (eChanel >= ChanelType.CHAT_CHANNEL_AROUND && eChanel < ChanelType.CHAT_CHANNEL_MAX)
            {
				if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(voiceKey))
                {
                    SceneChat req = new SceneChat();
                    req.channel = (byte)eChanel;
                    req.targetId = tarId;
                    req.word = content;
                    req.bLink = 0;
					req.voiceKey = voiceKey;
					req.voiceDuration = voiceTimeLength;

                    NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
                }
            }
        }

        public void SendPrivateChat(string content,ulong targetID)
        {
            if (!string.IsNullOrEmpty(content))
            {
                SceneChat req = new SceneChat();
                req.channel = (byte)ChanelType.CHAT_CHANNEL_PRIVATE;
                req.targetId = targetID;
                req.word = content;
                req.voiceKey = string.Empty;
                req.voiceDuration = 0;
                req.bLink = LinkParse.IsLink(content) ? (byte)1 : (byte)0;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

                ChatBlock chatBlock = _AllocChatData();
                ChatData chatData = chatBlock.chatData;
                chatData.channel = (byte)ChanelType.CHAT_CHANNEL_PRIVATE;
                chatData.objid = PlayerBaseData.GetInstance().RoleID;
                chatData.sex = 0;
                chatData.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                chatData.level = PlayerBaseData.GetInstance().Level;
                chatData.objname = PlayerBaseData.GetInstance().Name;
                chatData.word = content;
                chatData.guid = ++m_uniqueid;
                chatData.shortTimeString = "[" + TimeManager.GetInstance().GetTimeT() + "]";
                chatData.targetID = targetID;
                chatData.eChatType = _TransChanelType((ChanelType)chatData.channel);
                chatData.bLink = req.bLink;
                chatData.viplvl = (byte)PlayerBaseData.GetInstance().VipLevel;
                //Logger.LogErrorFormat("guid = {0}", chatData.guid);

                _AddChatData(chatBlock);
            }
        }

        public void AddLocalMsg(string content, ChanelType eChanel = ChanelType.CHAT_CHANNEL_TRIBE)
        {
            if (!string.IsNullOrEmpty(content))
            {
                ChatBlock chatBlock = _AllocChatData();
                ChatData chatData = chatBlock.chatData;
                chatData.channel = (byte)eChanel;
                chatData.objid = PlayerBaseData.GetInstance().RoleID;
                chatData.sex = 0;
                chatData.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                chatData.level = PlayerBaseData.GetInstance().Level;
                chatData.objname = PlayerBaseData.GetInstance().Name;
                chatData.word = content;
                chatData.guid = ++m_uniqueid;
                chatData.shortTimeString = "[" + TimeManager.GetInstance().GetTimeT() + "]";
                chatData.targetID = 0;
                chatData.eChatType = _TransChanelType((ChanelType)chatData.channel);
                chatData.bLink = LinkParse.IsLink(content) ? (byte)1 : (byte)0;
                chatData.viplvl = (byte)PlayerBaseData.GetInstance().VipLevel;
                _AddChatData(chatBlock);
                //Logger.LogErrorFormat("guid = {0}", chatData.guid);
            }
        }

        public void AddAskForPupilInvite(RelationData rd,string content)
        {
            if(null != rd && !string.IsNullOrEmpty(content))
            {
                ChatBlock chatBlock = _AllocChatData();
                ChatData chatData = chatBlock.chatData;
                chatData.channel = (byte)ChanelType.CHAT_CHANNEL_PRIVATE;
                chatData.objid = rd.uid;
                chatData.sex = 0;
                chatData.occu = rd.occu;
                chatData.level = rd.level;
                chatData.objname = rd.name;
                chatData.word = content;
                chatData.guid = ++m_uniqueid;
                chatData.shortTimeString = Function.GetDateTime((int)TimeManager.GetInstance().GetServerTime());
                chatData.targetID = PlayerBaseData.GetInstance().RoleID;
                chatData.eChatType = _TransChanelType((ChanelType)chatData.channel);
                chatData.bLink = LinkParse.IsLink(content) ? (byte)1 : (byte)0;
                chatData.viplvl = rd.vipLv;
                chatData.timeStamp = TimeManager.GetInstance().GetServerTime();
                chatData.isShowTimeStamp = false;
                _AddChatData(chatBlock);
            }
        }

        public ChatType _TransChanelType(ChanelType type)
        {
            switch(type)
            {
                case ChanelType.CHAT_CHANNEL_AROUND:   return ChatType.CT_NORMAL;
                case ChanelType.CHAT_CHANNEL_TEAM: return ChatType.CT_TEAM;
                case ChanelType.CHAT_CHANNEL_WORLD: return ChatType.CT_WORLD;
                case ChanelType.CHAT_CHANNEL_PRIVATE: return ChatType.CT_PRIVATE;
                case ChanelType.CHAT_CHANNEL_TRIBE: return ChatType.CT_GUILD;
                case ChanelType.CHAT_CHANNEL_SYSTEM: return ChatType.CT_SYSTEM;
                case ChanelType.CHAT_CHANNEL_ACCOMPANY: return ChatType.CT_ACOMMPANY;
                case ChanelType.CHAT_CHANNEL_TEAM_COPY_PREPARE: return ChatType.CT_TEAM_COPY_PREPARE;
                case ChanelType.CHAT_CHANNEL_TEAM_COPY_TEAM: return ChatType.CT_TEAM_COPY_TEAM;
                case ChanelType.CHAT_CHANNEL_TEAM_COPY_SQUAD: return ChatType.CT_TEAM_COPY_SQUAD;
                default: return ChatType.CT_MAX_WORDS;
            }
        }

        public ChanelType _TransChatType(ChatType type)
        {
            switch (type)
            {
                case ChatType.CT_NORMAL: return ChanelType.CHAT_CHANNEL_AROUND;
                case ChatType.CT_TEAM: return ChanelType.CHAT_CHANNEL_TEAM;
                case ChatType.CT_WORLD: return ChanelType.CHAT_CHANNEL_WORLD;
                case ChatType.CT_PRIVATE: return ChanelType.CHAT_CHANNEL_PRIVATE;
                case ChatType.CT_GUILD: return ChanelType.CHAT_CHANNEL_TRIBE;
                case ChatType.CT_SYSTEM: return ChanelType.CHAT_CHANNEL_SYSTEM;
                case ChatType.CT_ACOMMPANY: return ChanelType.CHAT_CHANNEL_ACCOMPANY;
                case ChatType.CT_PK3V3_ROOM: return ChanelType.CHAT_CHANNEL_PK3V3_ROOM;
                case ChatType.CT_TEAM_COPY_PREPARE: return ChanelType.CHAT_CHANNEL_TEAM_COPY_PREPARE;
                case ChatType.CT_TEAM_COPY_TEAM: return ChanelType.CHAT_CHANNEL_TEAM_COPY_TEAM;
                case ChatType.CT_TEAM_COPY_SQUAD: return ChanelType.CHAT_CHANNEL_TEAM_COPY_SQUAD;
                default: return ChanelType.CHAT_CHANNEL_MAX;
            }
        }


        public void AddPrivateChatData(ChatBlock data)
        {
            bool self = false;
            List<ChatBlock> chatDatas = null;
            ulong tarUid = 0;
            if (data.chatData.objid == PlayerBaseData.GetInstance().RoleID)
            {
                self = true;
                tarUid = data.chatData.targetID;
            }
            else if (data.chatData.targetID == PlayerBaseData.GetInstance().RoleID)
            {
                tarUid = data.chatData.objid;
            }

            if (!m_akObjid2Data.TryGetValue(tarUid, out chatDatas))
            {
                chatDatas = new List<ChatBlock>();
                data.iOrder = chatDatas.Count;
                chatDatas.Add(data);
                m_akObjid2Data.Add(tarUid, chatDatas);
            }
            else
            {
                if (chatDatas.Count >= m_dataCacheMax)
                {
                    chatDatas.RemoveAt(0);
                    data.iOrder = chatDatas[chatDatas.Count - 1].iOrder + 1;
                }
                else
                {
                    data.iOrder = chatDatas.Count;
                }
                chatDatas.Add(data);
            }

            if (onAddChatdata != null)
            {
                onAddChatdata(data);
            }

            if (!self)
            {
                RelationData rd = RelationDataManager.GetInstance().GetRelationByRoleID(tarUid);
                if (rd == null)
                {
                    rd = new RelationData();
                    rd.uid = tarUid;
                    rd.type = (byte)RelationType.RELATION_STRANGER;
                    rd.name = data.chatData.objname;
                    rd.seasonLv = 0;
                    rd.level = data.chatData.level;
                    rd.occu = data.chatData.occu;
                    rd.isOnline = 1;
                    rd.vipLv = data.chatData.viplvl;
                }

                RelationDataManager.GetInstance().OnAddPriChatList(rd, true);
            }
        }

        public bool OpenPrivateChatFrame(RelationData data)
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_friend_chat_need_lv", functionData.FinishLevel));
                return false;
            }

            if (data == null)
            {
                return false;
            }
            RelationDataManager.GetInstance().OnAddPriChatList(data, false);

            //if (ClientSystemManager.instance.IsFrameOpen<ChatFrame>())
            //{
            //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnPrivateRdChanged, new ChatFrameData { eChatType = ChatType.CT_PRIVATE, curPrivate = data });
            //}
            //else
            //{
            //    ClientSystemManager.instance.OpenFrame<ChatFrame>(FrameLayer.Middle, new ChatFrameData {  eChatType = ChatType.CT_PRIVATE, curPrivate = data });
            //}
            RelationFrameData relationFrmaeData = new RelationFrameData();
            relationFrmaeData.eCurrentRelationData = data;
            RelationFrameNew.CommandOpen(relationFrmaeData);
            return true;
        }

        public bool OpenTeamChatFrame()
        {
            if (!BeUtility.IsRaidBattle()) 
            {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();

            if (myTeam == null)
            {
                return false;
            }

            ProtoTable.TeamDungeonTable data = TableManager.GetInstance().GetTableItem<ProtoTable.TeamDungeonTable>((int)TeamDataManager.GetInstance().TeamDungeonID);
            if (data == null)
            {
                return false;
                }
            }

            ChatFrame cframe = ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle) as ChatFrame;
            if (!BeUtility.IsRaidBattle())
            {
            cframe.SetTab(ChatType.CT_TEAM);
            }
            else
            {
                if (BattleEasyChatDataManager.GetInstance() != null)
                {
                    cframe.SetTab(BattleEasyChatDataManager.GetInstance().DungeonChatType);
                }
            }
            return true;
        }
        
        //添加本地工会聊天数据 added by Jermaine 2017-3-7
        public bool AddLocalGuildChatData(string content)
        {
            ChatBlock chatBlock = _AllocChatData();

            ChatData chatData = chatBlock.chatData;
            chatData.channel = (byte)ChanelType.CHAT_CHANNEL_TRIBE;
            chatData.objid = 0;
            chatData.sex = 0;
            chatData.occu = 0;
            chatData.level = 0;
            chatData.viplvl = 0;
            chatData.objname = "系统提示";
            chatData.word = content;
            chatData.guid = ++m_uniqueid;
            chatData.shortTimeString = "[" + TimeManager.GetInstance().GetTimeT() + "]";
            chatData.targetID = PlayerBaseData.GetInstance().RoleID;
            chatData.eChatType = _TransChanelType(ChanelType.CHAT_CHANNEL_TRIBE);
            chatData.dirty = true;
            chatData.bLink = 1;
            
            _AddChatData(chatBlock);

            return true;
        }

        void _OnRoleIdChanged(UIEvent uiEvent)
        {
            Logger.LogProcessFormat("[ChatRecord] ChatManager EUIEventID.RoleIdChanged");
            _recvPrivateMsgNum.Clear();
            m_akObjid2Data.Clear();
            ulong curRoleId = (ulong)uiEvent.Param2;
            if(curRoleId > 0)
            {
                LoadPrivateDataFromRecords(curRoleId);
            }
        }

        void _OnLoadPrivateChatDataOK(ulong roleId)
        {
            Logger.LogProcessFormat("[ChatRecord] ChatManager onLoadPrivateChatDataOK roleId = {0}",roleId);
            if (roleId > 0)
            {
                LoadPrivateDataFromRecords(roleId);
            }
        }

        void LoadPrivateDataFromRecords(ulong RoleId)
        {
            if(!SwitchFunctionUtility.IsOpen(ChatRecordManager.functionId))
            {
                return;
            }
            _recvPrivateMsgNum.Clear();
            m_akObjid2Data.Clear();

            RoleInfo roleInfo = null;
            for(int i = 0; i < ClientApplication.playerinfo.roleinfo.Length; ++i)
            {
                if(RoleId == ClientApplication.playerinfo.roleinfo[i].roleId)
                {
                    roleInfo = ClientApplication.playerinfo.roleinfo[i];
                    break;
                }
            }
            if(roleInfo == null)
            {
                Logger.LogErrorFormat("LoadPrivateDataFromRecords error roleInfo is null!");
                return;
            }

            Logger.LogProcessFormat("[ChatRecord] ChatManager LoadPrivateDataFromRecords RoleId = {0}", RoleId);
            var chatRecords = ChatRecordManager.GetInstance().GetChatRecords(RoleId);
            if(chatRecords != null)
            {
                for(int i = 0; i < chatRecords.RoleChats.Count; ++i)
                {
                    var curChatRecord = chatRecords.RoleChats[i];
                    bool bDirty = curChatRecord.Dirty;
                    var rd = curChatRecord.RelationDataRecords.ConvertTo(new RelationData());
                    RelationDataManager.GetInstance().OnAddPriChatList(rd, bDirty);
                    if(curChatRecord != null)
                    {
                        for(int j = 0; j < curChatRecord.TargetChats.Count; ++j)
                        {
                            ChatBlock chatBlock = new ChatBlock();
                            chatBlock.chatData = GameClient.ChatRecordManager.PrivateChatRecords.ConvertFrom(curChatRecord.TargetChats[j],rd, roleInfo);
                            chatBlock.chatData.guid = ++m_uniqueid;
                            chatBlock.eType = ChatBlockType.CBT_NEW;
                            chatBlock.iOrder = j;
                            chatBlock.iPreID = 0;
                            AddPrivateChatData(chatBlock);
                        }
                        if(!bDirty)
                        {
                            RelationDataManager.GetInstance().ClearPriChatDirty((ulong)curChatRecord.friendId);
                        }
                    }
                }
                chatRecords.TryUpdateRelation();
            }
        }

        public void TrySendChatContent(ChatType eChatType,UnityEngine.Events.UnityAction cb)
        {
            switch (eChatType)
            {
                case ChatType.CT_TEAM:
                    {
                        var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_TEAM_CHAT_LV_LIMIT);
                        if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_team_talk_need_level"), systemValue.Value), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        //if (!ChatManager.accompany_chat_try_enter_cool_down())
                        //{
                        //    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_accompany_talk_need_interval"), ChatManager.accompany_cool_time), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                        //    break;
                        //}

                        if(null != cb)
                        {
                            cb.Invoke();
                        }
                    }
                    break;
                case ChatType.CT_WORLD:
                    {
                        var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_WORLD_CHAT_LV_LIMIT);
                        if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_level"), systemValue.Value), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        int iFreeTimes = ChatManager.GetInstance().FreeWorldChatLeftTimes;
                        if (iFreeTimes <= 0)
                        {
                            if (!ChatManager.GetInstance().CheckWorldActivityValueEnough())
                            {
                                SystemNotifyManager.SystemNotify(7006, () =>
                                {
                                    ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.VIP);
                                });
                                break;
                            }
                        }
                        if (!ChatManager.world_chat_try_enter_cool_down())
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_world_talk_need_interval"), ChatManager.world_cool_time), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }

                        if (null != cb)
                        {
                            cb.Invoke();
                        }
                    }
                    break;
                case ChatType.CT_GUILD:
                    {
                        if (PlayerBaseData.GetInstance().eGuildDuty == EGuildDuty.Invalid)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chat_guild_talk_need_guild"), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }

                        if (null != cb)
                        {
                            cb.Invoke();
                        }
                    }
                    break;
                case ChatType.CT_NORMAL:
                    {
                        if (!EnableNormalChat())
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("chat_normal_talk_scene_not_allow"),
                                CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }

                        var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_AROUND_CHAT_LV_LIMIT);
                        if (systemValue != null && systemValue.Value > PlayerBaseData.GetInstance().Level)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_normal_talk_need_level"), systemValue.Value), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }
                        if (!ChatManager.arround_chat_try_enter_cool_down())
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("chat_normal_talk_need_interval"), ChatManager.arround_cool_time), CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                            break;
                        }

                        if (null != cb)
                        {
                            cb.Invoke();
                        }
                    }
                    break;
            }
        }
        
        public bool EnableNormalChat()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown != null)
            {
                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null && scenedata.SceneType == CitySceneTable.eSceneType.SINGLE)
                {
                    return false;
                }
            }
            return true;
        }

        #region Private chat level limit

        public const string PRIVATE_CHAT_LIMIT_LEVEL = "PrivateChatLimitLevel";
        private int privateChatLimitLevel = 10;

        private int GetPrivateChatLimitLevel()
        {
            if (PlayerPrefs.HasKey(PRIVATE_CHAT_LIMIT_LEVEL))
                return PlayerPrefs.GetInt(PRIVATE_CHAT_LIMIT_LEVEL);
            else
                return privateChatLimitLevel;
        }

        private void SavePrivateLimitLevelPref(int level)
        {
            PlayerPrefs.SetInt(PRIVATE_CHAT_LIMIT_LEVEL, level);
            PlayerPrefs.Save();
        }

        public bool IsPrivateChatStrangerLevelLimit(ChatData chatData)
        {
            if (chatData == null)
            {
                return false;
            }
            if (chatData.channel != (int)ChanelType.CHAT_CHANNEL_PRIVATE)
            {
                return false;
            }
            bool self = false;
            ulong tarUid = 0;
            int tarLevel = 0;
            tarLevel = chatData.level;
            if (chatData.objid == PlayerBaseData.GetInstance().RoleID)
            {
                self = true;
                tarUid = chatData.targetID;
            }
            else if (chatData.targetID == PlayerBaseData.GetInstance().RoleID)
            {
                tarUid = chatData.objid;
            }
            RelationData rd = RelationDataManager.GetInstance().GetRelationByRoleID(tarUid);
            if (!self && rd == null && tarLevel <= privateChatLimitLevel)
            {
                return true;
            }
            return false;
        }

        public void SetPrivateChatLevelLimit(PrivateChatLevelLimit levelLimit)
        {
            string keyStr = "";
            switch (levelLimit)
            {
                case PrivateChatLevelLimit.LessLevelTen:
                    keyStr = "voice_private_chat_level_limit_ten";
                    break;
                case PrivateChatLevelLimit.LessLevelTwenty:
                    keyStr = "voice_private_chat_level_limit_twenty";
                    break;
                case PrivateChatLevelLimit.LessLevelThirty:
                    keyStr = "voice_private_chat_level_limit_thirty";
                    break;
                case PrivateChatLevelLimit.LessLevelForty:
                    keyStr = "voice_private_chat_level_limit_forty";
                    break;
            }
            if (int.TryParse(TR.Value(keyStr), out privateChatLimitLevel))
            {
                SavePrivateLimitLevelPref(privateChatLimitLevel);
            }
        }
        public bool GetIsPrivateChatLevelLimit(PrivateChatLevelLimit levelLimit)
        {
            int limitLevel = GetPrivateChatLimitLevel();
            int pcLimitLevel = 10;
            string keyStr = "";
            switch (levelLimit)
            {
                case PrivateChatLevelLimit.LessLevelTen:
                    keyStr = "voice_private_chat_level_limit_ten";
                    break;
                case PrivateChatLevelLimit.LessLevelTwenty:
                    keyStr = "voice_private_chat_level_limit_twenty";
                    break;
                case PrivateChatLevelLimit.LessLevelThirty:
                    keyStr = "voice_private_chat_level_limit_thirty";
                    break;
                case PrivateChatLevelLimit.LessLevelForty:
                    keyStr = "voice_private_chat_level_limit_forty";
                    break;
            }
            if (int.TryParse(TR.Value(keyStr), out pcLimitLevel))
            {
                if (pcLimitLevel == limitLevel)
                {
                    return true;
                }
            }
            else if (levelLimit == PrivateChatLevelLimit.LessLevelTen)
            {
                //默认值
                return true;
            }
            return false;
        }
        #endregion

        #region Voice Chat Msg Listener

        private event System.Action<ChatData> OnVoiceChatComeHandler;

        void OnVoiceChatCome(ChatData chatData)
        {
            if (chatData.bVoice)
            {
                if (OnVoiceChatComeHandler != null)
                    OnVoiceChatComeHandler(chatData);
            }
        }

        public void AddSyncVoiceChatListener(System.Action<ChatData> handler)
        {
            RemoveAllSyncVoiceChatListener();
            if (OnVoiceChatComeHandler == null)
                OnVoiceChatComeHandler += handler;
        }

        public void RemoveAllSyncVoiceChatListener()
        {
            if (OnVoiceChatComeHandler != null)
            {
                foreach (Delegate d in OnVoiceChatComeHandler.GetInvocationList())
                {
                    OnVoiceChatComeHandler -= d as Action<ChatData>;
                }
                OnVoiceChatComeHandler = null;
            }
        }

        #endregion
    }
}
