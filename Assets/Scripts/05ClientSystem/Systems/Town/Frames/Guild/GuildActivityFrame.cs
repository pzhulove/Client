using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using System.Reflection;

namespace GameClient
{   
    public class GuildActivityFrame : ClientFrame
    {
        public static void OpenGuildManorFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildManorFrame>();
        }

        public static void OpenGuildCrossManorFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildCrossManorFrame>();
        }

        public static void OpenGuildDungeonHelpFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildDungeonHelpFrame>();
        }

        public static bool IsGuildDungeonUnLocked()
        {
            int iLimitGuildLv = GuildDataManager.GetGuildDungeonActivityGuildLvLimit();
            int iLimitPlayerLevel = GuildDataManager.GetGuildDungeonActivityPlayerLvLimit();

            return GuildDataManager.GetInstance().GetGuildLv() >= iLimitGuildLv 
                && PlayerBaseData.GetInstance().Level >= iLimitPlayerLevel;
        }

        public static string GetGuildManorStateText()
        {
            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL)
            {
                EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();
                switch(state)
                {
                    case EGuildBattleState.Signup:
                        return TR.Value("guild_activity_signup_state");
                    case EGuildBattleState.Preparing:
                        return TR.Value("guild_activity_prepare_state");
                    case EGuildBattleState.Firing:
                        return TR.Value("guild_activity_firing_state");
                    case EGuildBattleState.LuckyDraw:
                        return TR.Value("guild_activity_lucydraw_state");

                    default:
                        return "";
                }
            }

            return "";
        }

        public static string GetCrossGuildManorStateText()
        {
            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS)
            {
                EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();
                switch (state)
                {
                    case EGuildBattleState.Signup:
                        return TR.Value("guild_activity_signup_state");
                    case EGuildBattleState.Preparing:
                        return TR.Value("guild_activity_prepare_state");
                    case EGuildBattleState.Firing:
                        return TR.Value("guild_activity_firing_state");
                    case EGuildBattleState.LuckyDraw:
                        return TR.Value("guild_activity_lucydraw_state");

                    default:
                        return "";
                }
            }

            return "";
        }

        public static string GetGuildDungeonStateText()
        {
            var state = GuildDataManager.GetInstance().GetGuildDungeonActivityStatus();
            return "";
        }

        public static bool IsShowGuildManorRedPoint()
        {
            ERedPoint redPointType = ERedPoint.GuildTerrDayReward;
            bool show = RedPointDataManager.GetInstance().HasRedPoint(redPointType);
            return show;
        }

        #region inner def

        public class GuildActivityData
        {
            public int guildActivityTableID = 0;
        }
        #endregion

        #region val
        List<GuildActivityData> guildActivityDatas = null;
        #endregion

        #region ui bind
        private ComUIListScript activitys = null;

        private Button mCloseBtn;
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildActivity";
        }

        protected override void _OnOpenFrame()
        {
            guildActivityDatas = new List<GuildActivityData>();
           
            UpdateGuildActivityList();
            BindUIEvent(); 
        }

        protected override void _OnCloseFrame()
        {         
            guildActivityDatas = null;
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            activitys = mBind.GetCom<ComUIListScript>("activitys");
            mCloseBtn = mBind.GetCom<Button>("CloseBtn");
            mCloseBtn.SafeAddOnClickListener(_OnCloseBtnClick);
        }
        
        protected override void _unbindExUI()
        {

            activitys = null;
            mCloseBtn.SafeRemoveOnClickListener(_OnCloseBtnClick);
            mCloseBtn = null;
        }
      

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
           
        }
        private void _OnCloseBtnClick()
        {
            frameMgr.CloseFrame<GuildActivityFrame>();
        }

        void CalcGuildActivityDatas()
        {
            guildActivityDatas = new List<GuildActivityData>();
            if (guildActivityDatas == null)
            {
                return;
            }

            {       
                Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildActivityTable>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        GuildActivityTable adt = iter.Current.Value as GuildActivityTable;
                        if (adt == null)
                        {
                            continue;
                        }

                        GuildActivityData guildActivityData = new GuildActivityData() { guildActivityTableID = adt.ID};
                        if(guildActivityData == null)
                        {
                            continue;
                        }

                        guildActivityDatas.Add(guildActivityData);
                    }
                }
            }

            return;
        }

        void UpdateGuildActivityItem(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (guildActivityDatas == null)
            {
                return;
            }

            if (item.m_index >= guildActivityDatas.Count)
            {
                return;
            }

            GuildActivityItem guildActivityItem = item.gameObjectBindScript as GuildActivityItem;
            if (guildActivityItem != null && guildActivityDatas[item.m_index] != null)
            {
                guildActivityItem.SetUp(guildActivityDatas[item.m_index]);
            }
        }

        void UpdateGuildActivityList()
        {
            if (activitys == null)
            {
                return;
            }

            CalcGuildActivityDatas();
            if (guildActivityDatas == null)
            {
                return;
            }

            activitys.Initialize();
            activitys.onBindItem = (item) =>
            {
                if (item != null)
                {
                    return item.GetComponent<GuildActivityItem>();
                }
                return null;
            };

            activitys.onItemVisiable = (item) =>
            {
                UpdateGuildActivityItem(item);
            };

            activitys.OnItemUpdate = (item) =>
            {
                UpdateGuildActivityItem(item);
            };

            activitys.UpdateElementAmount(guildActivityDatas.Count);
        }

        #endregion

        #region ui event       

        #endregion
    }
}
