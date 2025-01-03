using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{

    public enum SortListType
    {
        SORTLIST_INVALID = 0,
        SORTLIST_POWER = 1,          //战斗力
        SORTLIST_LEVEL = 2,          //等级排行榜  等级总榜，不分职业
        SORTLIST_WEAPON = 3,        //武器排行榜  武器总榜，不分子类别
        SORTLIST_PET = 4,          //宠物评分榜
		SORTLIST_SCORE_WAR = 6,				//33积分赛排行榜

		SORTLIST_NEW_YEAR_RED_PACKET = 8,   //新年红包排行榜
        SORTLIST_ACHIEVEMENT_SCORE = 16,
		SORTLIST_ADVENTURE_TEAM_GRADE = 17, //冒险队(佣兵团)评级排行榜

		SORTLIST_TOWER = 28,        //爬塔

        SORTLIST_DEATH_TOWER = 30,      //死亡之塔

        SORTLIST_PREMIUM_LEAGUE = 35,   // 赏金联赛

        // 赛季匹配
        SORTLIST_1V1_SEASON = 40,           // 1V1赛季

        //公会战排行
        SORTLIST_GUILD_BATTLE_SCORE = 41,   //公会的公会战总积分排名
        SORTLIST_GUILD_BATTLE_MEMBER = 42,  //个人在整个领地中的公会战积分的排名
        SORTLIST_GUILD_MEMBER_SCORE = 43,   //个人在所属公会中公会战积分的排名

        SORTLIST_GUILD_LEVEL = 44,           //公会等级排行
		SORTLIST_GUILD_BATTLE_WEEK_SCORE = 45,	//公会战周积分排名

        SORTLIST_CHIJI_SCORE = 250,						//吃鸡积分

        SORTLIST_2V2_SCORE_WAR = 251,				//积分赛排行榜


		SORTLIST_MAX,
    }

    public class BaseSortList
    {
        public Protocol.SortListType type;
        public UInt16 start;
        public UInt16 totalNum;
        public UInt16 num;
        public List<BaseSortListEntry> entries = new List<BaseSortListEntry>();
        public BaseSortListEntry selfEntry = null;

        public bool Init(Protocol.SortListType type1)
        {
			type = type1;
            return true;
        }
    }

    public class BaseSortListEntry
    {
        public UInt16 ranking;
        public UInt64 id;
        public string name;

        public virtual bool Decode(byte[] buffer, ref int pos)
        {
            BaseDLL.decode_uint16(buffer, ref pos, ref ranking);
            if (ranking == 0) return false;
            BaseDLL.decode_uint64(buffer, ref pos, ref id);

            UInt16 wordLen = 0;
            BaseDLL.decode_uint16(buffer, ref pos, ref wordLen);
            byte[] wordBytes = new byte[wordLen];
            for (int i = 0; i < wordLen; i++)
            {
                BaseDLL.decode_int8(buffer, ref pos, ref wordBytes[i]);
            }
            name = StringHelper.BytesToString(wordBytes);
            return true;
        }

    }

    public class LevelSortListEntry : BaseSortListEntry
    {
        public byte occu;
        public UInt16 level;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if(!base.Decode(buffer, ref pos))
            {
                return false;
            }
            BaseDLL.decode_int8(buffer, ref pos, ref occu);
            BaseDLL.decode_uint16(buffer, ref pos, ref level);
            return true;
        }
    }

    public class DeathTowerSortListEntry : LevelSortListEntry
    {
        public UInt32 layer;
        public UInt32 usedTime;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
            BaseDLL.decode_uint32(buffer, ref pos, ref layer);
            BaseDLL.decode_uint32(buffer, ref pos, ref usedTime);
            return true;
        }
    }

    public class SeasonSortListEntry : LevelSortListEntry
    {
        public UInt32 seasonLevel;
        public UInt32 seasonStar;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
            BaseDLL.decode_uint32(buffer, ref pos, ref seasonLevel);
            BaseDLL.decode_uint32(buffer, ref pos, ref seasonStar);
            return true;
        }
    }

    public class GuildBattleScore : BaseSortListEntry
    {
		public string serverName;
        public UInt32 score;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }

			UInt16 wordLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos, ref wordLen);
			byte[] wordBytes = new byte[wordLen];
			for (int i = 0; i < wordLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos, ref wordBytes[i]);
			}
			serverName = StringHelper.BytesToString(wordBytes);

			BaseDLL.decode_uint32(buffer, ref pos, ref score);
            return true;
        }
    }

    public class GuildLevelSortListEntry : BaseSortListEntry
    {
        public string leader;
        public UInt32 memberCount;
        public UInt32 level;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }

            UInt16 wordLen = 0;
            BaseDLL.decode_uint16(buffer, ref pos, ref wordLen);
            byte[] wordBytes = new byte[wordLen];
            for (int i = 0; i < wordLen; i++)
            {
                BaseDLL.decode_int8(buffer, ref pos, ref wordBytes[i]);
            }
            leader = StringHelper.BytesToString(wordBytes);
            BaseDLL.decode_uint32(buffer, ref pos, ref memberCount);
            BaseDLL.decode_uint32(buffer, ref pos, ref level);
            return true;
        }
    }

    public class GuildBattleMemberScore : GuildBattleScore
    {
        public string guildName;
        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
            UInt16 wordLen = 0;
            BaseDLL.decode_uint16(buffer, ref pos, ref wordLen);
            byte[] wordBytes = new byte[wordLen];
            for (int i = 0; i < wordLen; i++)
            {
                BaseDLL.decode_int8(buffer, ref pos, ref wordBytes[i]);
            }
            guildName = StringHelper.BytesToString(wordBytes);
            return true;
        }
    }
	
	public class GuildBattleTerrScoreRank : BaseSortListEntry
    {
        public UInt32 score;
        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
			
			BaseDLL.decode_uint32(buffer, ref pos, ref score);
          
            return true;
        }
    }

    public class ItemSortListEntry : BaseSortListEntry
    {
        public UInt64 ownerId;
        public string ownerName;
        public UInt16 level;
        public UInt32 strengthen;
        public UInt32 score;
        public UInt32 itemId;
        public byte equipType;//装备类型
        public byte growthAttr;//增幅属性

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }

            UInt16 wordLen = 0;
            BaseDLL.decode_uint64(buffer, ref pos, ref ownerId);
            BaseDLL.decode_uint16(buffer, ref pos, ref wordLen);
            byte[] wordBytes = new byte[wordLen];
            for (int i = 0; i < wordLen; i++)
            {
                BaseDLL.decode_int8(buffer, ref pos, ref wordBytes[i]);
            }
            ownerName = StringHelper.BytesToString(wordBytes);
            BaseDLL.decode_uint16(buffer, ref pos, ref level);
            BaseDLL.decode_uint32(buffer, ref pos, ref strengthen);
            BaseDLL.decode_uint32(buffer, ref pos, ref score);
            BaseDLL.decode_uint32(buffer, ref pos, ref itemId);
            BaseDLL.decode_int8(buffer, ref pos, ref equipType);
            BaseDLL.decode_int8(buffer, ref pos, ref growthAttr);
            return true;
        }

    }

    public class MoneyRewardsSortListEntry : BaseSortListEntry
    {
        public byte occu;
        public UInt16 level;
        public UInt32 maxScore;
        public UInt32 time;
        public UInt32 score;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
            BaseDLL.decode_int8(buffer, ref pos, ref occu);
            BaseDLL.decode_uint16(buffer, ref pos, ref level);
            BaseDLL.decode_uint32(buffer, ref pos, ref maxScore);
            BaseDLL.decode_uint32(buffer, ref pos, ref time);
            BaseDLL.decode_uint32(buffer, ref pos, ref score);
            return true;
        }
    }

	public class NewYearRedPacketSortListEntry : BaseSortListEntry
	{
		public byte occu;
		public UInt32 redPacketNum;

		public override bool Decode(byte[] buffer, ref int pos)
		{
			if (!base.Decode(buffer, ref pos))
			{
				return false;
			}
			BaseDLL.decode_int8(buffer, ref pos, ref occu);
			BaseDLL.decode_uint32(buffer, ref pos, ref redPacketNum);
			return true;
		}
	}

    public class AchievementScoreSortListEntry : BaseSortListEntry
    {
        public byte occu;
        public UInt32 score;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
            BaseDLL.decode_int8(buffer, ref pos, ref occu);
            BaseDLL.decode_uint32(buffer, ref pos, ref score);
            return true;
        }
    }

	public class ScoreWarSortListEntry : GuildBattleScore
	{

	}

    public class AdventureTeamGradeSortListEntry : BaseSortListEntry
    {
        public UInt16 adventureTeamLevel;
        public string adventureTeamName;
		public UInt32 adventureTeamScore;
        public UInt32 adventureTeamGrade;
		
        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
            BaseDLL.decode_uint16(buffer, ref pos, ref adventureTeamLevel);
			UInt16 wordLen = 0;
            BaseDLL.decode_uint16(buffer, ref pos, ref wordLen);
            byte[] wordBytes = new byte[wordLen];
            for (int i = 0; i < wordLen; i++)
            {
                BaseDLL.decode_int8(buffer, ref pos, ref wordBytes[i]);
            }
            adventureTeamName = StringHelper.BytesToString(wordBytes);
			BaseDLL.decode_uint32(buffer, ref pos, ref adventureTeamScore);
            BaseDLL.decode_uint32(buffer, ref pos, ref adventureTeamGrade);
			
            return true;
        }
    }

    /// <summary>
    /// 吃鸡积分
    /// </summary>
    public class ChijiScoreSortListEntry : BaseSortListEntry
    {
        public UInt32 occu;
        public UInt32 score;

        public override bool Decode(byte[] buffer, ref int pos)
        {
            if (!base.Decode(buffer, ref pos))
            {
                return false;
            }
            BaseDLL.decode_uint32(buffer, ref pos, ref occu);
            BaseDLL.decode_uint32(buffer, ref pos, ref score);
            return true;
        }
    }

    public class SortListDecoder
    {
        public static Protocol.SortListType MakeSortListType(GameClient.SortListType type,uint secondType = 0,ulong dataID = 0)
        {
            return new Protocol.SortListType() { mainType = (uint)type,subType = secondType,dataId = dataID};
        }

        public static BaseSortListEntry GetEntry(Protocol.SortListType type)
        {
            switch ((GameClient.SortListType)(type.mainType))
            {
                case GameClient.SortListType.SORTLIST_LEVEL:
                    return new LevelSortListEntry();
                case GameClient.SortListType.SORTLIST_1V1_SEASON:
                    return new SeasonSortListEntry();
                case GameClient.SortListType.SORTLIST_TOWER:
                case GameClient.SortListType.SORTLIST_DEATH_TOWER:
                    return new DeathTowerSortListEntry();
                case GameClient.SortListType.SORTLIST_WEAPON:
                    return new ItemSortListEntry();
                case GameClient.SortListType.SORTLIST_GUILD_BATTLE_SCORE:
                    return new GuildBattleScore();
                case GameClient.SortListType.SORTLIST_GUILD_BATTLE_MEMBER:
                    return new GuildBattleMemberScore();
                case GameClient.SortListType.SORTLIST_GUILD_MEMBER_SCORE:
                    return new GuildBattleMemberScore();
                case GameClient.SortListType.SORTLIST_GUILD_LEVEL:
                    return new GuildLevelSortListEntry();
				case GameClient.SortListType.SORTLIST_GUILD_BATTLE_WEEK_SCORE:
                    return new GuildBattleTerrScoreRank();
                case GameClient.SortListType.SORTLIST_PREMIUM_LEAGUE:
                    return new MoneyRewardsSortListEntry();
				case GameClient.SortListType.SORTLIST_NEW_YEAR_RED_PACKET:
					return new NewYearRedPacketSortListEntry();
                case GameClient.SortListType.SORTLIST_ACHIEVEMENT_SCORE:
                    return new AchievementScoreSortListEntry();
				case GameClient.SortListType.SORTLIST_SCORE_WAR:
					return new ScoreWarSortListEntry();
				case GameClient.SortListType.SORTLIST_ADVENTURE_TEAM_GRADE:
					return new AdventureTeamGradeSortListEntry();
                case GameClient.SortListType.SORTLIST_CHIJI_SCORE:
                    return new ChijiScoreSortListEntry();
				case GameClient.SortListType.SORTLIST_2V2_SCORE_WAR:
					return new ScoreWarSortListEntry();

                default:
                    return null;
            }
        }

        public static BaseSortList Decode(byte[] buffer, ref int pos, int length, bool isUpdate = false)
        {
            var temp = new Protocol.SortListType();
            temp.decode(buffer, ref pos);
            if(GameClient.SortListType.SORTLIST_INVALID > (GameClient.SortListType)temp.mainType || GameClient.SortListType.SORTLIST_MAX < (GameClient.SortListType)temp.mainType)
            {
                return null;
            }
            BaseSortList sortList = new BaseSortList();
            sortList.type = temp;

            BaseDLL.decode_uint16(buffer, ref pos, ref sortList.start);
            BaseDLL.decode_uint16(buffer, ref pos, ref sortList.totalNum);
            BaseDLL.decode_uint16(buffer, ref pos, ref sortList.num);
			if(sortList.totalNum == 0)
			{
				return sortList;
			}

            for (int i = 0; i < sortList.num; ++i)
            {
                BaseSortListEntry entry = GetEntry(sortList.type);
                if (entry == null) return null;
                entry.Decode(buffer, ref pos);
                sortList.entries.Add(entry);
            }

            sortList.selfEntry = GetEntry(sortList.type);
            sortList.selfEntry.Decode(buffer, ref pos);

            return sortList;
        }
    }
}

