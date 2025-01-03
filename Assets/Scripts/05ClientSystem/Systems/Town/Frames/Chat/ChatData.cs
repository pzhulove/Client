using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace GameClient
{
    public enum ChanelType
    {
        CHAT_CHANNEL_AROUND = 1,    //附近频道
        CHAT_CHANNEL_TEAM = 2,      //队伍频道
        CHAT_CHANNEL_WORLD = 3,     //世界
        CHAT_CHANNEL_PRIVATE = 4,   //私聊
        CHAT_CHANNEL_TRIBE = 5,     //部落
        CHAT_CHANNEL_HORN = 6,      //喇叭
        CHAT_CHANNEL_TEAM_SPECIAL = 7,      //队伍表情频道（特殊）
        CHAT_CHANNEL_SYSTEM = 8,
        CHAT_CHANNEL_ACCOMPANY = 9,
        CHAT_CHANNEL_PK3V3_ROOM = 11, // 这里和服务器进行匹配，服务器那边的枚举值为11

        CHAT_CHANNEL_TEAM_COPY_PREPARE = 12,    // 团本准备场景频道（附近频道）
        CHAT_CHANNEL_TEAM_COPY_TEAM = 13,       // 团本团队
        CHAT_CHANNEL_TEAM_COPY_SQUAD = 14,      // 团本小队

        CHAT_CHANNEL_MAX,
    };

    public class ChatData
    {
        public const UInt16 CD_MAX_WORDS = 100;

        public byte channel;        //频道
        public UInt64 objid;        //发送者uid
        public byte sex;            //发送者性别
        public byte occu;           //发送者职业
        public UInt16 level;        //发送者等级
        public byte viplvl;         //发送者vip等级
        public string objname;      //发送者名字
        public string word;         //发送文字
        public int guid;            //全局唯一ID
        public string shortTimeString;
        public ulong targetID;        //目标ID
        public byte bLink;          //是否链接
        public bool bGm;            //是否就GM
		public string voiceKey;     //语音消息key
		public byte voiceDuration;	//语音消息时长
		public bool bVoice;	        //是否语音
		public bool bVoicePlayFirst;//语音是否第一次播放
        public bool bHorn;          //是否是喇叭
        public bool bRedPacket;     //是否是红包
        public uint timeStamp;      //时间戳
        public bool bAddFriend;     //添加好友特殊标识

        public bool dirty;
        public ChatType eChatType;
        public int height = 0;
        public bool isShowTimeStamp; //是否显示过时间戳
        public UInt32 headFrame;//头像框
        public UInt32 zoneId;   //跨服，服务器Id

        static StringBuilder kBuilder = StringBuilderCache.Acquire();
        static string[] ms_channel_string = new string[(int)ChatType.CT_MAX_WORDS]
        {
            "",
            "chat_chanel_system",
            "chat_chanel_world",
            "chat_chanel_normal",
            "chat_chanel_guild",
            "chat_chanel_team",
            "",
            "chat_chanel_private",
            "chat_chanel_accompany",
            "",
            "chat_chanel_team_copy_prepare",
            "chat_chanel_team_copy_team",
            "chat_chanel_team_copy_squad",
        };
        static string ms_horn_string = "chat_channel_horn";

        public string GetWords()
        {
            if(!bGm)
            {
                return word;
            }
            else
            {
                return "<color=#FFFF00>" + word + "</color>";
            }
        }

        public bool IsWordsEmpty()
        {
            return string.IsNullOrEmpty(word);
        }

        public string GetNameLink()
        {
            if(string.IsNullOrEmpty(objname))
            {
                return "";
            }

            if(!bGm)
            {
                if(objid != 0)
                {
                    kBuilder.Clear();
                    kBuilder.Append("{P ");
                    kBuilder.AppendFormat("{0} {1} {2} {3}", objid, objname, occu, level);
                    kBuilder.Append("}");
                }
                else
                {
                    return "";
                }
            }
            else
            {
                kBuilder.Clear();
                kBuilder.AppendFormat(TR.Value("chat_gm_name"),objname);
            }
            return kBuilder.ToString();
        }

        public string GetChannelString()
        {
            if(bHorn)
            {
                return TR.Value("chat_channel_horn");
            }

            if(eChatType >= 0 && (int)eChatType < ms_channel_string.Length)
            {
                return TR.Value(ms_channel_string[(int)eChatType]);
            }

            return default(string);
        }

        public static string GetChannelString(ChatType eChatType)
        {
            if (eChatType >= 0 && (int)eChatType < ms_channel_string.Length)
            {
                return TR.Value(ms_channel_string[(int)eChatType]);
            }
            return default(string);
        }

        //public Sprite GetSystemIcon()
        //{
        //    return AssetLoader.instance.LoadRes("UI/Image/Packed/p_UI_Chat.png:UI_Liaotian_Tubiao_Xitong", typeof(Sprite)).obj as Sprite;
        //}

        public void GetSystemIcon(ref Image img)
        {
            ETCImageLoader.LoadSprite(ref img, "UI/Image/NewPacked/MainUI_Liaotian.png:Liaotian_Tx_Shezhi");
        }
    }

    /**
 *标签列表
 *文本		T	参数:颜色 + 字形
 *链接		L	参数:网址 + 颜色
 *道具		I	参数:唯一id(base64) + 类型id + 品质 + 是否绑定 + 宠物id
 *表情		F	参数:表情id
 *寻路		W	参数:地图id + npcid + x + y + 小飞鞋类型(0表示没有小飞鞋 1表示任务小飞鞋  2表示护送小飞鞋 ，默认为1)
 *换行		N	参数:无
 *副本招募	C	参数:队伍id(UInt32) + 是否需要密码(1需要 0不需要)
 *宠物		E	参数:唯一id(base64) + 星级
 *道具名称	M	参数:类型id + 品质
 *法宝		O	参数:唯一id(base64) + 类型id
 *玩家链接	P	参数:唯一id(base64) + 名字 + 性别 + 职业 + 等级|name
 *标题		B	参数:客户端资源id，不填默认为0
*/
    

    public class MsgTag
    {
        static readonly byte s_tagNum = 12;
        static readonly char[] s_MSG_TAGS = { 'T', 'L', 'I', 'F', 'W', 'N', 'C', 'E', 'M', 'O', 'P', 'B' };


        protected char _type;                                          //类型
        protected List<string> _parms = new List<string>();                                  //参数
	    protected string _content;                                     //内容

        public char Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public List<string> Parms
        {
            get { return _parms; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        static public char[] MsgTags
        {
            get { return s_MSG_TAGS; }
        }

        static public byte TagNum
        {
            get { return s_tagNum; }
        }
        
    }

    public class MsgParser
    {
        const char MSGTAG_LEFT_SEPARATOR = '{';
        const char MSGTAG_RIGHT_SEPARATOR = '}';
        const char MSGTAG_CONTENT_SEPARATOR = '|';
        const char MSGTAG_PARAM_SEPARATOR = ' ';

        const UInt16 MAX_TAGPARAM_NUM = 6;
        const UInt16 MAX_TAGPARAM_LENGTH = 128;
        const UInt16 MAX_TAG_NUM = 100;

        static protected List<MsgTag> _msgTags = new List<MsgTag>();

        static public bool Parse(string str)
        {
            _msgTags.Clear();
            while (str.Length > 0)
            {
                if (_msgTags.Count > MAX_TAG_NUM)
                {
                    return false;
                }

                if(str[0] == MSGTAG_LEFT_SEPARATOR)
                { 
                    str = str.Substring(1);
                    if(false == ParseTag(ref str))
                    {
                        return false;
                    }
                }
                else
                {
                    MsgTag tag = new MsgTag();
                    _msgTags.Add(tag);
                    tag.Type = (char)0;

                    int len = str.IndexOf(MSGTAG_LEFT_SEPARATOR);
                    if(len > 0)
                    {
                        tag.Content = str.Substring(0, len);
                        str = str.Substring(len);
                    }
                    else
                    {
                        tag.Content = str;
                        str = "";
                    }
                }
            }

            return true;
        }

        static bool ParseTag(ref string str)
        {
            if(false == IsValidTag(str[0]))
            {
                return false;
            }

            MsgTag tag = new MsgTag();
            _msgTags.Add(tag);
            tag.Type = str[0];
            str = str.Substring(1);
            if(false == ParseParams(tag, ref str))
            {
                return false;
            }

            switch(str[0])
            {
                case MSGTAG_CONTENT_SEPARATOR:
                    {
                        str = str.Substring(1);
                        while (str[0] == MSGTAG_PARAM_SEPARATOR) { str = str.Substring(1); }

                        int len = str.IndexOf(MSGTAG_RIGHT_SEPARATOR);
                        tag.Content = str.Substring(0, len);
                        if (len+1 == str.Length) { str = ""; }
                        else { str = str.Substring(len); }
                   
                    }
                    break;
                case MSGTAG_RIGHT_SEPARATOR:
                    {
                        if (1 == str.Length) { str = ""; }
                        else { str = str.Substring(1); }
                    }
                    break;
            }

            return true;
        }

        static bool ParseParams(MsgTag tag, ref string str)
        {
            while(tag.Parms.Count < MAX_TAGPARAM_NUM)
            {
                while (str[0] == MSGTAG_PARAM_SEPARATOR) { str = str.Substring(1); }

                if (str[0] == MSGTAG_CONTENT_SEPARATOR ||
                    str[0] == MSGTAG_RIGHT_SEPARATOR)
                {
                   
                    return true;
                }

                string param = "";
                while(str[0] != MSGTAG_PARAM_SEPARATOR &&
                      str[0] != MSGTAG_CONTENT_SEPARATOR &&
                      str[0] != MSGTAG_RIGHT_SEPARATOR)
                {
                    param += str[0];
                    if(param.Length > MAX_TAGPARAM_LENGTH) { return false; }
                    str = str.Substring(1);
                }
                tag.Parms.Add(param);
            }

            return false;
        }

        static bool IsValidTag(char tag)
        {
            char[] msgTags = MsgTag.MsgTags;

            for (int i = 0; i < MsgTag.TagNum; ++i)
                if (tag == msgTags[i]) return true;
            return false;
        }
    }
}
