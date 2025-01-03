using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace GameClient
{
    [ExecuteAlways]
    public class LinkParse : MonoBehaviour
    {
        public enum RegexType
        {
            RT_EMOTION = 0,
            RT_NET_ITEM,
            RT_LOCAL,
            RT_PLAYER,
            RT_RETINUE,
            RT_ENERGY_STONE,
            RT_RED_PACKET,
            RT_JOIN_TABLE,
            RT_GUILD_SHOP,
            RT_TEAM_INVITE,
            RT_GUILD_MANOR,
            RT_DYNAMIC_IMAGE,
            RT_SUPER_GROUP,
            RT_WEB_URL,

            RT_COUNT,
        }

        public enum EmotionSizeType
        {
            EST_72 = 0,
            EST_36,
            EST_34,
            EST_COUNT,
        }

        public class LinkInfo
        {
            public RegexType eRegexType = RegexType.RT_COUNT;
            public int iStartIndex;
            public int iEndIndex;
            public SpriteAssetColor eColor;
            public Bounds bounds;
            public List<Bounds> allBounds = new List<Bounds>();
            public bool bNeedRemove;

            public int iParam0;
            public int iParam1;
            public int iParam2;

            public ulong guid0;
            public ulong guid1;

            public string str0;
            public string str1;
        }

        static readonly Regex[] ms_regexs = new Regex[(int)RegexType.RT_COUNT]
        {
        new Regex(@"{F (\d*\.?\d+%?)}", RegexOptions.Singleline),//RT_EMOTION
        new Regex(@"{I (\d*\.?\d+%?) (\d*\.?\d+%?)}", RegexOptions.Singleline),//RT_NET_ITEM
        new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline),//RT_LOCAL
        new Regex(@"{P (\d+) (\w+) (\d+) (\d+)}", RegexOptions.Singleline),//RT_PLAYER
        new Regex(@"{R (\d*\.?\d+%?) (\d*\.?\d+%?)}", RegexOptions.Singleline),//RT_RETINUE
        new Regex(@"{W (\d*\.?\d+%?)}", RegexOptions.Singleline),//RT_ENERGY_STONE
        new Regex(@"{O (\d+) (\w+)}", RegexOptions.Singleline),//RT_RED_PACKET
        new Regex(@"{H (\w+)}", RegexOptions.Singleline),//RT_JOIN_TABLE
        new Regex(@"{D (\w+)}", RegexOptions.Singleline),//RT_GUILD_SHOP
        new Regex(@"{T (\d+)}", RegexOptions.Singleline), //RT_TEAM_INVITE
        new Regex(@"{A (\w+)}"),
        new Regex(@"{M (\d+)}", RegexOptions.Singleline),
        new Regex(@"{X (\d+) (\d+) (\d+)}", RegexOptions.Singleline),
        new Regex(@"{U (\w+) ^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$} (\d+) ", RegexOptions.Singleline),
        };
        static readonly Regex ms_system_left_color = new Regex(@"<color=(.)+>", RegexOptions.Singleline);
        static readonly StringBuilder ms_TextBuilder = StringBuilderCache.Acquire(1024);
        static readonly string[] ms_replace_strings = new string[]
        {
            @"<quad size=72 width=1 height=1/>",
            @"<quad size=36 width=1 height=1/>",
            @"<quad size=34 width=1 height=1/>",
        };
        public EmotionSizeType eEmotionSizeType = EmotionSizeType.EST_72;

        public static string getReplaceString(EmotionSizeType eEmotionSizeType)
        {
            if(eEmotionSizeType >= 0 && eEmotionSizeType < EmotionSizeType.EST_COUNT)
            {
                return ms_replace_strings[(int)eEmotionSizeType];
            }
            return ms_replace_strings[0];
        }

        List<LinkInfo> m_akLinkInfos = new List<LinkInfo>();
        string m_kText = "";
        string m_kParsedValue = "";
        bool m_bNeedParse = true;

        public NewSuperLinkText m_kTarget;

        public float LinkTextWidth
        {
            get
            {
                if(m_kTarget == null)
                {
                    Logger.LogError("m_kTarget is null, 请检查预制体: [AnnouncementFrame], 超链接脚本missing");
                    return 0;
                }

                return m_kTarget.preferredWidth;
            }
        }
        public bool bMissionLink = true;

        string text
        {
            get
            {
                return m_kText;
            }
            set
            {
                //if (m_kText != value || string.IsNullOrEmpty(value))
                {
                    m_akLinkInfos.Clear();
                    m_kText = value;
                    m_kParsedValue = m_bNeedParse ? Parse() : value;
                    if (m_kTarget != null)
                    {
                        m_kTarget.SetLinkText(m_kParsedValue, m_akLinkInfos);
                    }
                }
            }
        }

        public static void _TryToken(StringBuilder ms_TextBuilder, string src, int iIndex, List<LinkInfo> akLinkInfo, EmotionSizeType eEmotionSizeType = EmotionSizeType.EST_72)
        {
            if (!(ms_TextBuilder != null && src != null && iIndex >= 0 && iIndex < src.Length))
            {
                return;
            }

            ms_TextBuilder.Clear();
            for (int i = iIndex; i < src.Length; ++i)
            {
                if (src[i] == '{' && i + 1 < src.Length)
                {
                    int j = i + 1;
                    while (j < src.Length && src[j] != '}')
                    {
                        ++j;
                    }

                    LinkInfo link = null;
                    bool bLinked = false;
                    var curSrc = src.Substring(i + 1, j - i - 1);
                    var tokens = curSrc.Split(' ');
                    switch (tokens[0])
                    {
                        case "F":
                            {
                                if (tokens.Length == 2)
                                {
                                    int iParam0 = 0;
                                    if (int.TryParse(tokens[1], out iParam0))
                                    {
                                        bLinked = true;
                                        if (null != akLinkInfo)
                                        {
                                            link = new LinkInfo();
                                            link.iParam0 = iParam0;
                                            link.iStartIndex = ms_TextBuilder.Length;
                                            ms_TextBuilder.Append(getReplaceString(eEmotionSizeType));
                                            link.iEndIndex = ms_TextBuilder.Length;
                                            link.eColor = SpriteAssetColor.SAC_COUNT;
                                            link.eRegexType = RegexType.RT_EMOTION;
                                            akLinkInfo.Add(link);
                                        }
                                        else
                                        {
                                            ms_TextBuilder.Append(getReplaceString(eEmotionSizeType));
                                        }
                                    }
                                }
                                break;
                            }
                        case "I":
                            {
                                if (tokens.Length == 4)
                                {
                                    ulong iParam0 = 0;
                                    int iParam1 = 0;
                                    int iParam2 = 0;
                                    if (ulong.TryParse(tokens[1], out iParam0) && int.TryParse(tokens[2], out iParam1) &&
                                        int.TryParse(tokens[3], out iParam2))
                                    {
                                        bLinked = true;
                                        if (null != akLinkInfo)
                                        {
                                            link = new LinkInfo();
                                            link.guid0 = iParam0;
                                            link.iParam0 = iParam1;
                                            link.iParam1 = iParam2;
                                            ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iParam1);
                                            ms_TextBuilder.AppendFormat("<color={0}>", Parser.ItemParser.GetItemColor(item));// link color
                                            link.iStartIndex = ms_TextBuilder.Length;
                                            ms_TextBuilder.Append("[");
                                            if (item == null)
                                            {
                                                ms_TextBuilder.Append("UnKnown Item");
                                            }
                                            else if (iParam2 <= 0)
                                            {
                                                ms_TextBuilder.Append(item.Name);
                                            }
                                            else
                                            {
                                                ms_TextBuilder.AppendFormat(TR.Value("super_link_item_name"), iParam2, item.Name);
                                            }
                                            ms_TextBuilder.Append("]");
                                            link.iEndIndex = ms_TextBuilder.Length;
                                            link.eColor = Parser.ItemParser.GetAssetColor(item);
                                            ms_TextBuilder.Append("</color>");
                                            link.eRegexType = RegexType.RT_NET_ITEM;
                                            akLinkInfo.Add(link);
                                        }
                                        else
                                        {
                                            ProtoTable.ItemTable item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iParam1);
                                            ms_TextBuilder.Append("[");
                                            if (item == null)
                                            {
                                                ms_TextBuilder.Append("UnKnown Item");
                                            }
                                            else if (iParam2 <= 0)
                                            {
                                                ms_TextBuilder.Append(item.Name);
                                            }
                                            else
                                            {
                                                ms_TextBuilder.AppendFormat("+{0} {1}", iParam2, item.Name);
                                            }
                                            ms_TextBuilder.Append("]");
                                        }
                                    }
                                }
                                break;
                            }
                        case "R":
                            {
                                if (tokens.Length == 3)
                                {
//                                     ulong iParam0 = 0;
//                                     int iParam1 = 0;
//                                     if (ulong.TryParse(tokens[1], out iParam0) && int.TryParse(tokens[2], out iParam1))
//                                     {
//                                         bLinked = true;
//                                         if (null != akLinkInfo)
//                                         {
//                                             link = new LinkInfo();
//                                             link.guid0 = iParam0;
//                                             link.iParam0 = iParam1;
//                                             ProtoTable.FollowPetTable item = TableManager.GetInstance().GetTableItem<ProtoTable.FollowPetTable>(iParam1);
//                                             ms_TextBuilder.AppendFormat("<color={0}>", Parser.ItemParser.GetRetinueColor(item));// link color
//                                             link.iStartIndex = ms_TextBuilder.Length;
//                                             ms_TextBuilder.Append("[");
//                                             ms_TextBuilder.Append(item != null ? item.Name : "UnKnown Retinue");
//                                             ms_TextBuilder.Append("]");
//                                             link.iEndIndex = ms_TextBuilder.Length;
//                                             link.eColor = Parser.ItemParser.GetAssetColor(item);
//                                             ms_TextBuilder.Append("</color>");
//                                             link.eRegexType = RegexType.RT_RETINUE;
//                                             akLinkInfo.Add(link);
//                                         }
//                                         else
//                                         {
//                                             ProtoTable.FollowPetTable item = TableManager.GetInstance().GetTableItem<ProtoTable.FollowPetTable>(iParam1);
//                                             ms_TextBuilder.Append("[");
//                                             ms_TextBuilder.Append(item != null ? item.Name : "UnKnown Retinue");
//                                             ms_TextBuilder.Append("]");
//                                         }
//                                     }
                                }
                                break;
                            }
                        case "P":
                            {
                                if (tokens.Length == 5)
                                {
                                    ulong guid = 0;
                                    int iParam0 = 0;
                                    int iParam1 = 0;
                                    if (ulong.TryParse(tokens[1], out guid) &&
                                        int.TryParse(tokens[3], out iParam0) &&
                                        int.TryParse(tokens[4], out iParam1))
                                    {
                                        bLinked = true;
                                        if (null != akLinkInfo)
                                        {
                                            link = new LinkInfo();
                                            if (guid == GameClient.PlayerBaseData.GetInstance().RoleID)
                                            {
                                                ms_TextBuilder.AppendFormat("<color={0}>", "#00c6ff");  //link color
                                                link.eColor = SpriteAssetColor.SAC_SELF_NAME;
                                            }
                                            else
                                            {
                                                ms_TextBuilder.AppendFormat("<color={0}>", "#ffd042");  //link color
                                                link.eColor = SpriteAssetColor.SAC_OTHER_NAME;
                                            }

                                            link.guid0 = guid;//guid
                                            link.str0 = tokens[2];//name
                                            link.iParam0 = iParam0;//occu
                                            link.iParam1 = iParam1;//level

                                            link.iStartIndex = ms_TextBuilder.Length;
                                            ms_TextBuilder.Append(tokens[2]);
                                            link.iEndIndex = ms_TextBuilder.Length;

                                            ms_TextBuilder.Append("</color>");  //link color
                                            link.eRegexType = RegexType.RT_PLAYER;
                                            akLinkInfo.Add(link);
                                        }
                                        else
                                        {
                                            ms_TextBuilder.Append(tokens[2]);
                                        }
                                    }
                                }
                                break;
                            }
                        case "W":
                            {
                                if (tokens.Length == 2)
                                {
//                                     int iItemID = 0;
//                                     if (int.TryParse(tokens[1], out iItemID))
//                                     {
//                                         bLinked = true;
//                                         if (null != akLinkInfo)
//                                         {
//                                             ProtoTable.WarpStone item = TableManager.GetInstance().GetTableItem<ProtoTable.WarpStone>(iItemID);
//                                             link = new LinkInfo();
//                                             link.iParam0 = iItemID;
//                                             ms_TextBuilder.AppendFormat("<color={0}>", Parser.ItemParser.GetWarpStoneColor(item));  // link color
//                                             link.iStartIndex = ms_TextBuilder.Length;
//                                             //name
//                                             ms_TextBuilder.Append("[");
//                                             ms_TextBuilder.Append(item != null ? item.Name : "Unknown Stone");
//                                             ms_TextBuilder.Append("]");
//                                             link.iEndIndex = ms_TextBuilder.Length;
//                                             link.eColor = Parser.ItemParser.GetAssetColor(item);
//                                             ms_TextBuilder.Append("</color>");
//                                             link.eRegexType = RegexType.RT_ENERGY_STONE;
//                                             akLinkInfo.Add(link);
//                                         }
//                                         else
//                                         {
//                                             ProtoTable.WarpStone item = TableManager.GetInstance().GetTableItem<ProtoTable.WarpStone>(iItemID);
//                                             //name
//                                             ms_TextBuilder.Append("[");
//                                             ms_TextBuilder.Append(item != null ? item.Name : "Unknown Stone");
//                                             ms_TextBuilder.Append("]");
//                                         }
//                                     }
                                }
                                break;
                            }
                        case "O"://RT_RED_PACKET 3
                            {
                                if (tokens.Length == 3)
                                {
                                    ulong guid = 0;
                                    if (ulong.TryParse(tokens[1], out guid))
                                    {
                                        bLinked = true;
                                        if (null != akLinkInfo)
                                        {
                                            link = new LinkInfo();
                                            link.guid0 = guid;
                                            link.eColor = SpriteAssetColor.SAC_SELF_NAME;
                                            ms_TextBuilder.Append(string.Format("<color={0}>", "#00c6ff"));  //link color
                                            link.iStartIndex = ms_TextBuilder.Length;
                                            ms_TextBuilder.Append(tokens[2]);
                                            link.iEndIndex = ms_TextBuilder.Length;
                                            ms_TextBuilder.Append("</color>");  //link color
                                            link.eRegexType = RegexType.RT_RED_PACKET;
                                            akLinkInfo.Add(link);
                                        }
                                        else
                                        {
                                            ms_TextBuilder.Append(tokens[2]);
                                        }
                                    }
                                }
                                break;
                            }
                        case "H"://RT_JOIN_TABLE 2
                            {
                                if (tokens.Length == 2)
                                {
                                    bLinked = true;
                                    if (null != akLinkInfo)
                                    {
                                        link = new LinkInfo();
                                        link.eColor = SpriteAssetColor.SAC_SELF_NAME;
                                        ms_TextBuilder.Append(string.Format("<color={0}>", "#00c6ff"));  //link color
                                        link.iStartIndex = ms_TextBuilder.Length;
                                        ms_TextBuilder.Append(tokens[1]);
                                        link.iEndIndex = ms_TextBuilder.Length;
                                        ms_TextBuilder.Append("</color>");  //link color
                                        link.eRegexType = RegexType.RT_JOIN_TABLE;
                                        akLinkInfo.Add(link);
                                    }
                                    else
                                    {
                                        ms_TextBuilder.Append(tokens[1]);
                                    }
                                }
                                break;
                            }
                        case "D"://RT_GUILD_SHOP 2
                            {
                                if (tokens.Length == 2)
                                {
                                    bLinked = true;
                                    if (null != akLinkInfo)
                                    {
                                        link = new LinkInfo();
                                        link.eColor = SpriteAssetColor.SAC_SELF_NAME;
                                        ms_TextBuilder.Append(string.Format("<color={0}>", "#00c6ff"));  //link color
                                        link.iStartIndex = ms_TextBuilder.Length;
                                        ms_TextBuilder.Append(tokens[1]);
                                        link.iEndIndex = ms_TextBuilder.Length;
                                        ms_TextBuilder.Append("</color>");  //link color
                                        link.eRegexType = RegexType.RT_GUILD_SHOP;
                                        akLinkInfo.Add(link);
                                    }
                                    else
                                    {
                                        ms_TextBuilder.Append(tokens[1]);
                                    }
                                }
                                break;
                            }
                        case "T": // RT_TEAM_INVITE 2
                            {
                                if (tokens.Length == 2)
                                {
                                    ulong guid = 0;
                                    if (ulong.TryParse(tokens[1], out guid))
                                    {
                                        bLinked = true;
                                        if (null != akLinkInfo)
                                        {
                                            link = new LinkInfo();
                                            link.guid0 = guid;

                                            link.eColor = SpriteAssetColor.SAC_BLUE;
                                            ms_TextBuilder.Append(string.Format("<color={0}>", "#00c6ff"));  //link color
                                            link.iStartIndex = ms_TextBuilder.Length;
                                            ms_TextBuilder.Append(TR.Value("JoinTeam"));
                                            link.iEndIndex = ms_TextBuilder.Length;
                                            ms_TextBuilder.Append("</color>");  //link color
                                            link.eRegexType = RegexType.RT_TEAM_INVITE;
                                            akLinkInfo.Add(link);
                                        }
                                        else
                                        {
                                            ms_TextBuilder.Append(TR.Value("JoinTeam"));
                                        }
                                    }
                                }
                                break;
                            }
                        case "A":
                            {
                                if (tokens.Length == 2)
                                {
                                    bLinked = true;
                                    if (null != akLinkInfo)
                                    {
                                        link = new LinkInfo();
                                        link.eColor = SpriteAssetColor.SAC_SELF_NAME;
                                        ms_TextBuilder.Append(string.Format("<color={0}>", "#00c6ff"));  //link color
                                        link.iStartIndex = ms_TextBuilder.Length;
                                        ms_TextBuilder.Append(tokens[1]);
                                        link.iEndIndex = ms_TextBuilder.Length;
                                        ms_TextBuilder.Append("</color>");  //link color
                                        link.eRegexType = RegexType.RT_GUILD_MANOR;
                                        akLinkInfo.Add(link);
                                    }
                                    else
                                    {
                                        ms_TextBuilder.Append(tokens[1]);
                                    }
                                }
                                break;
                            }
                        case "M":
                            {
                                if(tokens.Length == 2)
                                {
                                    int iImgLinkID = 0;
                                    if(int.TryParse(tokens[1],out iImgLinkID))
                                    {
                                        var chatItem = TableManager.GetInstance().GetTableItem<ProtoTable.LiaoTianDynamicTextureTable>(iImgLinkID);
                                        if(null != chatItem)
                                        {
                                            bLinked = true;

                                            if (null != akLinkInfo)
                                            {
                                                link = new LinkInfo();
                                                link.eColor = SpriteAssetColor.SAC_WHITE;
                                                link.iStartIndex = ms_TextBuilder.Length;
                                                ms_TextBuilder.AppendFormat("<quad size={0} width={1} height={2}/>",chatItem.Size, chatItem.Width, chatItem.Height);
                                                link.iEndIndex = ms_TextBuilder.Length;
                                                link.eRegexType = RegexType.RT_DYNAMIC_IMAGE;
                                                link.iParam0 = iImgLinkID;
                                                akLinkInfo.Add(link);
                                            }
                                            else
                                            {
                                                ms_TextBuilder.Append(curSrc);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case "X":
                            {
                                if (tokens.Length >= 3)
                                {
                                    int superLinkID = 0;
                                    ulong iParamID = 0;
                                    ulong iParam2 = 0;
                                    bool bEnable = false;
                                    if(tokens.Length == 3)
                                    {
                                        if(int.TryParse(tokens[1], out superLinkID) && ulong.TryParse(tokens[2], out iParamID))
                                        {
                                            bEnable = true;
                                        }
                                    }
                                    else if(tokens.Length == 4)
                                    {
                                        if (int.TryParse(tokens[1], out superLinkID) && ulong.TryParse(tokens[2], out iParamID) && ulong.TryParse(tokens[3], out iParam2))
                                        {
                                            bEnable = true;
                                        }
                                    }

                                    if(!bEnable)
                                    {
                                        break;
                                    }

                                    var superLinkItem = TableManager.GetInstance().GetTableItem<ProtoTable.SuperLinkTable>(superLinkID);
                                    if (null != superLinkItem)
                                    {
                                        switch (superLinkItem.LinkType)
                                        {
                                            case ProtoTable.SuperLinkTable.eLinkType.LT_DESC:
                                                {
                                                    if (superLinkItem.LocalParam.Count == 2)
                                                    {
                                                        bLinked = true;
                                                        if (null != akLinkInfo)
                                                        {
                                                            link = new LinkInfo();
                                                            link.eColor = SpriteAssetColor.SAC_WHITE;

                                                            SpriteAssetColor eColor = SpriteAssetColor.SAC_WHITE;
                                                            int iColorEnum = 0;
                                                            if (int.TryParse(superLinkItem.LocalParam[1], out iColorEnum) &&
                                                                iColorEnum >= 0 && iColorEnum < (int)SpriteAssetColor.SAC_COUNT)
                                                            {
                                                                eColor = (SpriteAssetColor)iColorEnum;
                                                            }

                                                            link.eColor = eColor;
                                                            ms_TextBuilder.Append("<color=#");
                                                            ms_TextBuilder.Append(NewSuperLinkText.GetColorString(eColor));
                                                            ms_TextBuilder.Append(">");
                                                            link.iStartIndex = ms_TextBuilder.Length;
                                                            ms_TextBuilder.Append(superLinkItem.LocalParam[0]);
                                                            link.iEndIndex = ms_TextBuilder.Length;
                                                            ms_TextBuilder.Append("</color>");
                                                            link.iParam0 = superLinkID;
                                                            link.guid0 = iParamID;
                                                            link.guid1 = iParam2;
                                                            link.eRegexType = RegexType.RT_SUPER_GROUP;
                                                            akLinkInfo.Add(link);
                                                        }
                                                        else
                                                        {
                                                            ms_TextBuilder.Append(superLinkItem.LocalParam[0]);
                                                        }
                                                    }
                                                }
                                                break;

                                            case ProtoTable.SuperLinkTable.eLinkType.LT_TABLE_NAME:
                                                {
                                                    if (superLinkItem.LocalParam.Count == 3)
                                                    {
                                                        System.Type type = System.Type.GetType(superLinkItem.LocalParam[0]);
                                                        if (null == type)
                                                        {
                                                            break;
                                                        }

                                                        var tableItem = TableManager.GetInstance().GetTableItem(type, (int)iParamID);
                                                        if (null == tableItem)
                                                        {
                                                            break;
                                                        }

                                                        var property = type.GetProperty(superLinkItem.LocalParam[1], BindingFlags.Instance | BindingFlags.Public);
                                                        if (null == property)
                                                        {
                                                            break;
                                                        }

                                                        string value = property.GetValue(tableItem, null) as string;
                                                        if (null == value)
                                                        {
                                                            break;
                                                        }

                                                        SpriteAssetColor eColor = SpriteAssetColor.SAC_WHITE;
                                                        int iColorEnum = 0;
                                                        if (int.TryParse(superLinkItem.LocalParam[2], out iColorEnum) &&
                                                            iColorEnum >= 0 && iColorEnum < (int)SpriteAssetColor.SAC_COUNT)
                                                        {
                                                            eColor = (SpriteAssetColor)iColorEnum;
                                                        }

                                                        bLinked = true;
                                                        if (null != akLinkInfo)
                                                        {
                                                            link = new LinkInfo();
                                                            link.eColor = eColor;
                                                            ms_TextBuilder.Append("<color=#");
                                                            ms_TextBuilder.Append(NewSuperLinkText.GetColorString(eColor));
                                                            ms_TextBuilder.Append(">");
                                                            link.iStartIndex = ms_TextBuilder.Length;
                                                            ms_TextBuilder.Append(value);
                                                            link.iEndIndex = ms_TextBuilder.Length;
                                                            ms_TextBuilder.Append("</color>");
                                                            link.iParam0 = superLinkID;
                                                            link.guid0 = iParamID;
                                                            link.guid1 = iParam2;
                                                            link.eRegexType = RegexType.RT_SUPER_GROUP;
                                                            akLinkInfo.Add(link);
                                                        }
                                                        else
                                                        {
                                                            ms_TextBuilder.Append(value);
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                        case "U":
                            {
                                if (tokens.Length == 4)
                                {
                                    bLinked = true;
                                    SpriteAssetColor eColorStr = SpriteAssetColor.SAC_ORANGE;  //link Str color
                                    int iColorStrEnum = 4;
                                    if (int.TryParse(tokens[3], out iColorStrEnum) &&
                                        iColorStrEnum >= 0 && iColorStrEnum < (int)SpriteAssetColor.SAC_COUNT)
                                    {
                                        eColorStr = (SpriteAssetColor)iColorStrEnum;
                                    }
                                    ms_TextBuilder.Append("<color=#");
                                    ms_TextBuilder.Append(NewSuperLinkText.GetColorString(eColorStr));
                                    ms_TextBuilder.Append(">");

                                    if (null != akLinkInfo)
                                    {
                                        link = new LinkInfo
                                        {
                                            eColor = eColorStr,
                                            iStartIndex = ms_TextBuilder.Length,
                                            eRegexType = RegexType.RT_WEB_URL,
                                            str0 = tokens[2]
                                        };
                                        ms_TextBuilder.Append(tokens[1]);
                                        link.iEndIndex = ms_TextBuilder.Length;

                                        akLinkInfo.Add(link);
                                    }
                                    else
                                    {
                                        ms_TextBuilder.Append(tokens[1]);
                                    }

                                    ms_TextBuilder.Append("</color>");  //link Str color
                                }                                
                            }
                            break;
                    }

                    if (bLinked == true)
                    {
                        i = j;
                        continue;
                    }
                }

                ms_TextBuilder.Append(src[i]);
            }
        }

        string Parse()
        {
            m_akLinkInfos.Clear();
            int iStartIndex = 0;
            ms_TextBuilder.Clear();

            if (string.IsNullOrEmpty(m_kText))
            {
                return "";
            }

            if (!bMissionLink)
            {
                _TryToken(ms_TextBuilder, m_kText, 0, m_akLinkInfos,eEmotionSizeType);
                return ms_TextBuilder.ToString();
            }

            for (int i = 0; i < ms_regexs.Length; ++i)
            {
                var matchCollections = ms_regexs[i].Matches(m_kText);
                if (matchCollections == null)
                {
                    continue;
                }

                for (int j = 0; j < matchCollections.Count; ++j)
                {
                    var curMatch = matchCollections[j];
                    if (curMatch == null || curMatch.Groups.Count <= 0 || string.IsNullOrEmpty(curMatch.Groups[0].Value))
                    {
                        continue;
                    }

                    LinkInfo link = new LinkInfo();
                    link.eRegexType = (RegexType)i;
                    link.iStartIndex = curMatch.Groups[0].Index;
                    link.iEndIndex = curMatch.Groups[0].Index + curMatch.Groups[0].Length;
                    m_akLinkInfos.Add(link);
                }
            }

            if (m_akLinkInfos.Count <= 0)
            {
                return m_kText;
            }

            m_akLinkInfos.Sort((x, y) =>
            {
                return x.iStartIndex - y.iStartIndex;
            });

            for (int i = 0; i < m_akLinkInfos.Count; ++i)
            {
                var current = m_akLinkInfos[i];
                if (current == null)
                {
                    Logger.LogErrorFormat("null reference! value = {0}", m_kText);
                    continue;
                }

                if (current.iStartIndex < 0 || current.iStartIndex >= current.iEndIndex || current.iEndIndex > m_kText.Length)
                {
                    Logger.LogErrorFormat("out of index! value = {0}", m_kText);
                    continue;
                }

                var match = ms_regexs[(int)current.eRegexType].Match(m_kText, current.iStartIndex, current.iEndIndex - current.iStartIndex);
                if (match == null || match.Groups.Count < 0 || string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    Logger.LogErrorFormat("parse error! value = {0}", m_kText);
                    m_akLinkInfos.RemoveAt(i--);
                    continue;
                }

                ms_TextBuilder.Append(m_kText.Substring(iStartIndex, match.Groups[0].Index - iStartIndex));

                switch (current.eRegexType)
                {
                    case RegexType.RT_LOCAL:
                        {
                            Parser.ParserReturn parseResult = OnParse(match.Groups[1].Value, match.Groups[2].Value);
                            ms_TextBuilder.Append(string.Format("<color={0}>", parseResult.color));  //link color

                            current.str0 = match.Groups[1].Value;//key
                            current.iParam0 = (int)parseResult.iId;//id(mission id,dungen id table id)

                            current.eColor = SpriteAssetColor.SAC_COUNT;

                            current.iStartIndex = ms_TextBuilder.Length;
                            ms_TextBuilder.Append(parseResult.content);
                            current.iEndIndex = ms_TextBuilder.Length;

                            ms_TextBuilder.Append("</color>");
                            break;
                        }
                }

                iStartIndex = match.Groups[0].Index + match.Groups[0].Length;
            }

            if (!string.IsNullOrEmpty(m_kText))
            {
                ms_TextBuilder.Append(m_kText.Substring(iStartIndex, m_kText.Length - iStartIndex));
            }

            return ms_TextBuilder.ToString();
        }

        protected Parser.ParserReturn OnParse(string key, string value)
        {
            Parser.ParserReturn ret;
            ret.color = "0xFFFFFF";
            ret.content = "";
            ret.iId = 0;

            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            string parserName = "";
            if (key.Length > 0)
            {
                parserName = key.Substring(0, 1);
                parserName = parserName.ToUpper();

                if (key.Length > 1)
                {
                    parserName += key.Substring(1, key.Length - 1);
                }
            }

            if (parserName.Length <= 0)
            {
                return ret;
            }

            string parserClassName = "Parser." + parserName + "Parser";

            object obj = assembly.CreateInstance(parserClassName); // 创建类的实例，返回为 object 类型，需要强制类型转换
            if (obj != null)
            {
                Parser.IParser parser = obj as Parser.IParser;
                if (parser != null)
                {
                    return parser.OnParse(value);
                }
            }

            return ret;
        }

        public void SetText(string value, bool bNeedParse = true)
        {
            m_bNeedParse = bNeedParse;
            text = value;
        }

        public void AddListener(NewSuperLinkText.OnClickItem listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickItem += listener;
        }

        public void RemoveListener(NewSuperLinkText.OnClickItem listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickItem -= listener;
        }

        public void AddListener(NewSuperLinkText.OnClickPlayerName listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickPlayerName += listener;
        }

        public void RemoveListener(NewSuperLinkText.OnClickPlayerName listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickPlayerName -= listener;
        }

        public void AddListener(NewSuperLinkText.OnClickLocalLink listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickLocalLink += listener;
        }

        public void RemoveListener(NewSuperLinkText.OnClickLocalLink listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickLocalLink -= listener;
        }

        public void AddListener(NewSuperLinkText.OnClickRetinueLink listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickRetinueLink += listener;
        }

        public void RemoveListener(NewSuperLinkText.OnClickRetinueLink listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickRetinueLink -= listener;
        }

        public void AddListener(NewSuperLinkText.OnClickWrapStoneLink listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickWrapStoneLink += listener;
        }

        public void RemoveListener(NewSuperLinkText.OnClickWrapStoneLink listener)
        {
            if (m_kTarget != null)
                m_kTarget.onClickWrapStoneLink -= listener;
        }

        public void AddOnFailedListener(UnityEngine.Events.UnityAction listener)
        {
            if (m_kTarget != null)
            {
                m_kTarget.AddFailedCallBack(listener);
            }
        }

        public void RemoveFailedListener(UnityEngine.Events.UnityAction listener)
        {
            if (m_kTarget != null)
            {
                m_kTarget.RemoveFailedCallBack(listener);
            }
        }

        public void RemoveAllDelegate()
        {
            if (m_kTarget != null)
            {
                m_kTarget.RemoveAllDelegate();
            }
        }

        public static bool IsLink(string value)
        {
            for (int i = 0; i < (int)RegexType.RT_COUNT; ++i)
            {
                var match = ms_regexs[i].Match(value);
                if (!string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}