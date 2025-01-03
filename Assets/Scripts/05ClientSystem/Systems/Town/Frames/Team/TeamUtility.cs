using UnityEngine;
using System.Collections;
using ProtoTable;
using UnityEngine.UI;

namespace GameClient
{
    public class TeamUtility
    {
        public const uint kDefaultTeamDungeonID = 1;

        private static bool[, ,] kMissionTraceOpenFlag = new bool[2, 2, 2] {
            // 没有队伍
            { 
                // 没选中
                {
                    // mainFrame
                    false, 
                    // frame 
                    false
                },
                // 选中
                {
                    // mainFrame
                    false,
                    // frame 
                    true
                },
            },
            // 有队伍
            { 
                // 没选中
                {
                    // mainFrame
                    false,
                    // frame
                    false
                },

                // 选中
                {
                    // mainFrame
                    true,
                    // frame
                    false
                },
            },
        };


        private const int kTeamMainMenuIdx = 0;
        private const int kTeamMain      = 1;
        /// <summary>
        /// 任务追踪界面打开，关闭
        /// </summary>
        public static void OnMissionTraceSelectTeam(bool isSelect)
        {
            int idxHasTeam = TeamDataManager.GetInstance().HasTeam() ? 1 : 0;
            int idxHasSelect = isSelect ? 1 : 0;

            bool openTeamMainMenu = kMissionTraceOpenFlag[idxHasTeam, idxHasSelect, kTeamMainMenuIdx];
            bool openTeamMain = kMissionTraceOpenFlag[idxHasTeam, idxHasSelect, kTeamMain];

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                _opFrame<TeamMainFrame>(openTeamMainMenu);
                _opFrame<TeamMainMenuFrame>(openTeamMain);

                if(openTeamMain && !TeamDataManager.GetInstance().NotPopUpTeamList)
                {
                    TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                    //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>();
                }
                if(openTeamMain && TeamDataManager.GetInstance().NotPopUpTeamList)
                {
                    TeamDataManager.GetInstance().NotPopUpTeamList = false;
                }
            }
        }

        private static void _opFrame<T>(bool isOpen) where T : ClientFrame
        {
            if (isOpen)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen(typeof(T)))
                {
                    ClientSystemManager.GetInstance().CloseFrame<T>();
                }

                ClientSystemManager.GetInstance().OpenFrame<T>(FrameLayer.Bottom);
            }
            else
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen(typeof(T)))
                {
                    ClientSystemManager.GetInstance().CloseFrame<T>();
                }
            }
        }

		public static bool IsNormalTeamDungeonID(int teamDungeonID)
		{
			TeamDungeonTable curDungeonTab = TableManager.instance.GetTableItem<TeamDungeonTable> (teamDungeonID);
			if (null != curDungeonTab)
			{
				return curDungeonTab.ShowIndependent == 1 || curDungeonTab.Type == TeamDungeonTable.eType.DUNGEON;
			}

			return false;
		}

        // 是否是组队精英地下城
        public static bool IsEliteTeamDungeonID(int teamDungeonID)
        {
            TeamDungeonTable curDungeonTab = TableManager.instance.GetTableItem<TeamDungeonTable>(teamDungeonID);
            if (null == curDungeonTab)
            {
                return false;               
            }
            return IsEliteDungeonID(curDungeonTab.DungeonID);
        }
        
        // 是否是精英地下城
        public static bool IsEliteDungeonID(int dungeonID)
        {
            DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonID);
            if(dungeonTable == null)
            {
                return false;
            }
            return dungeonTable.ThreeType == DungeonTable.eThreeType.T_T_TEAM_ELITE;
        }
        public enum eType
        {
            NoTarget,
            Menu,
            Dungeon,
            AttackCityMonster,
        }

        public static eType GetTeamDungeonType(int teamDungeonID)
        {
            TeamDungeonTable curDungeonTab = TableManager.instance.GetTableItem<TeamDungeonTable> (teamDungeonID);
			if (null != curDungeonTab)
			{
                if (1 == curDungeonTab.ShowIndependent)
                {
                    return eType.NoTarget;
                }
                else
                {
                    if (TeamDungeonTable.eType.DUNGEON == curDungeonTab.Type)
                    {
                        return eType.Dungeon;
                    }
                    else if (curDungeonTab.Type == TeamDungeonTable.eType.CityMonster)
                    {
                        return eType.AttackCityMonster;
                    }
                    else
                    {
                        return eType.Menu;
                    }
                }
			}

            return eType.NoTarget;
        }


		public static int GetMenuTeamDungeonID(int teamDungeonID)
		{
            eType type = GetTeamDungeonType(teamDungeonID);
            switch (type)
            {
                case eType.Dungeon:
                    {
                        TeamDungeonTable curDungeonTab = TableManager.instance.GetTableItem<TeamDungeonTable>(teamDungeonID);
                        if (null != curDungeonTab)
                        {
                            return curDungeonTab.MenuID;
                        }
                    }
                    break;
                case eType.AttackCityMonster:
                {
                    TeamDungeonTable curDungeonTable =
                        TableManager.GetInstance().GetTableItem<TeamDungeonTable>(teamDungeonID);
                    if (null != curDungeonTable)
                    {
                        return curDungeonTable.MenuID;
                    }
                }
                    break;
                case eType.Menu:
                case eType.NoTarget:
                    return teamDungeonID;

            }

            return -1;
		}

        //public static Sprite GetSpriteByOccu(byte occu)
        //{
        //    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>((int)occu);
        //    if (jobData != null)
        //    {
        //        ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
        //        if (resData != null)
        //        {
        //            return AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
        //        }
        //    }

        //    return null;
        //}
        public static void GetSpriteByOccu(ref Image image, byte occu)
        {
            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>((int)occu);
            if (jobData != null)
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    // return AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref image, resData.IconPath);
                    return;
                }
            }
        }
    }
}
