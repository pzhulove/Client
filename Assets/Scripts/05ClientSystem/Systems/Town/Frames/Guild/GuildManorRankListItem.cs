using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // 公会领地界面中排行榜的item
    internal class GuildManorRankListItem : ComUIListTemplateItem
    {
        [SerializeField]
        Image imgRank = null;

        [SerializeField]
        Text txtRank = null;

        [SerializeField]
        Text guildName = null;

        [SerializeField]
        Text score = null;

        [SerializeField]
        Image imgTitle = null;

        [SerializeField]
        Text txtTitle = null;

        [SerializeField]
        Color normal;

        [SerializeField]
        Color guildColor;

        class RankInfo
        {
            public int min = 0;
            public int max = 0;

            public override bool Equals(object obj)
            {
                var rankInfo = obj as RankInfo;
                if(rankInfo == null)
                {
                    return false;
                }

                return (min == rankInfo.min && max == rankInfo.max);
            }

            public override int GetHashCode()
            {
                return min.GetHashCode() + max.GetHashCode();
            }
        }

        static Dictionary<RankInfo, int> rank2TitleID = new Dictionary<RankInfo, int>();
        
        public static int GetTitleIDByRank(int rank)
        {
            int titleID = 0;
            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildBattleScoreRankRewardTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildBattleScoreRankRewardTable adt = iter.Current.Value as GuildBattleScoreRankRewardTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.ID == rank)
                    {
                        return adt.TitleId;
                    }

                    if (adt.ID > rank)
                    {
                        titleID = adt.TitleId;
                        break;
                    }                   
                }
            }

            return titleID;
        }

        private void SetTitleIcon(int titleID)
        {
            var table = TableManager.GetInstance().GetTableItem<NewTitleTable>(titleID);
            if(table == null)
            {
                return;
            }

            imgTitle.SafeSetImage(table.path);
//             if(imgTitle != null)
//             {
//                 imgTitle.SetNativeSize();
//             }
        }

        private void SetTitleInfo(int titleID)
        {
            var table = TableManager.GetInstance().GetTableItem<NewTitleTable>(titleID);
            if (table == null)
            {
                return;
            }

            if(table.Style == (int)TitleDataManager.eTitleStyle.Txt)
            {
                txtTitle.SafeSetText(table.name);

                txtTitle.CustomActive(true);
                imgTitle.CustomActive(false);
            }
            else if(table.Style == (int)TitleDataManager.eTitleStyle.Img)
            {
                imgTitle.SafeSetImage(table.path);
                if(imgTitle != null)
                {
                    imgTitle.SetNativeSize();
                }

                txtTitle.CustomActive(false);
                imgTitle.CustomActive(true);
            }
        }

        private void SetTitleText(int titleID)
        {
            var table = TableManager.GetInstance().GetTableItem<NewTitleTable>(titleID);
            if (table == null)
            {
                return;
            }

            txtTitle.SafeSetText(table.name);
        }



        public override void SetUp(object data)
        {
            GuildBattleTerrScoreRank guildBattleTerrScoreRank = data as GuildBattleTerrScoreRank;
            if(guildBattleTerrScoreRank == null)
            {
                return;
            }

            txtRank.SafeSetText(guildBattleTerrScoreRank.ranking.ToString());
            guildName.SafeSetText(guildBattleTerrScoreRank.name);
            score.SafeSetText(guildBattleTerrScoreRank.score.ToString());
            
            if(string.IsNullOrEmpty(guildBattleTerrScoreRank.name))
            {
                guildName.SafeSetText(TR.Value("guild_battle_title_empty"));
            }

            if(guildBattleTerrScoreRank.score == 0)
            {
                score.SafeSetText(TR.Value("guild_battle_title_empty"));
            }

            int titleID = GetTitleIDByRank(guildBattleTerrScoreRank.ranking);
            SetTitleInfo(titleID);

            // 根据rank获取的titleid为0表示没有头衔奖励
            if(titleID == 0)
            {
                txtTitle.SafeSetText(TR.Value("guild_battle_no_title"));

                txtTitle.CustomActive(true);
                imgTitle.CustomActive(false);
            }

            // 排行为0表示未上榜
            if (guildBattleTerrScoreRank.ranking == 0)
            {
                txtRank.SafeSetText(TR.Value("guild_battle_not_in_rank"));
                guildName.SafeSetText(GuildDataManager.GetInstance().GetMyGuildName());
                score.SafeSetText("0");

                txtTitle.SafeSetText(TR.Value("guild_battle_no_title"));

                txtTitle.CustomActive(true);
                imgTitle.CustomActive(false);
            }

            Color color = normal;
            if(GuildDataManager.GetInstance().HasSelfGuild() && guildBattleTerrScoreRank.id == GuildDataManager.GetInstance().myGuild.uGUID)
            {
                color = guildColor;
            }

            txtRank.SafeSetColor(color);
            guildName.SafeSetColor(color);
            score.SafeSetColor(color);
            txtTitle.SafeSetColor(color);
        }
    }
}
