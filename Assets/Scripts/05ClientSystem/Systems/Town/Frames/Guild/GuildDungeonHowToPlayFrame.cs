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

namespace GameClient
{
    public class GuildDungeonHowToPlayFrame : ClientFrame
    {
        #region val

       
        #endregion

        #region ui bind

        private Button btnClose = null;
        private Text playing = null;
        private Text playingDesc = null;
        private Text weaknesses = null;
        private Text weaknessesDesc = null;
        private Text recommend = null;
        private Text recommendDesc = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonHowToPlay";
        }

        protected override void _OnOpenFrame()
        {           
            BindUIEvent();
            
            if(userData != null)
            {
                UpdateContent((uint)(ulong)userData);
            }
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {         
            btnClose = mBind.GetCom<Button>("btnClose");
            btnClose.SafeRemoveAllListener();
            btnClose.SafeAddOnClickListener(() =>
            {
                frameMgr.CloseFrame(this);
            });

            playing = mBind.GetCom<Text>("playing");
            playingDesc = mBind.GetCom<Text>("playingDesc");
            weaknesses = mBind.GetCom<Text>("weaknesses");
            weaknessesDesc = mBind.GetCom<Text>("weaknessesDesc");
            recommend = mBind.GetCom<Text>("recommend");
            recommendDesc = mBind.GetCom<Text>("recommendDesc");
        }

        protected override void _unbindExUI()
        {          
            btnClose = null;
            playing = null;
            playingDesc = null;
            weaknesses = null;
            weaknessesDesc = null;
            recommend = null;
            recommendDesc = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
            
        }

        void UpdateContent(uint iGuildDungeonID)
        {
            playing.SafeSetText(TR.Value("guild_dungeon_playing", GuildDataManager.GetInstance().GetGuildDungeonName(iGuildDungeonID)));
            playingDesc.SafeSetText(GuildDataManager.GetInstance().GetGuildDungeonPlayingDesc(iGuildDungeonID));

            weaknesses.SafeSetText(TR.Value("guild_dungeon_weakness", GuildDataManager.GetInstance().GetGuildDungeonName(iGuildDungeonID)));
            weaknessesDesc.SafeSetText(GuildDataManager.GetInstance().GetGuildDungeonWeaknessDesc(iGuildDungeonID));

            recommend.SafeSetText(TR.Value("guild_dungeon_recommend_job"));
            recommendDesc.SafeSetText(GuildDataManager.GetInstance().GetGuildDungeonRecommendDesc(iGuildDungeonID));
        }

        #endregion

        #region ui event     

        #endregion
    }
}
