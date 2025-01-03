using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    /// <summary>
    /// 语音相关UI
    /// </summary>
    public class BattleUIVoice : BattleUIBase
    {
        public BattleUIVoice() : base() { }

        #region ExtraUIBind
        private GameObject mTalkVoiceRoot = null;
        private GameObject mTeamVoiceBtnGo = null;
        private VoiceInputBtn mTeamVoiceBtn = null;

        private ComVoiceChat mComVoiceChat = null;

        protected override void _bindExUI()
        {
            mTalkVoiceRoot = mBind.GetGameObject("TalkVoiceRoot");
            mTeamVoiceBtnGo = mBind.GetGameObject("TeamVoiceBtnGo");
            mTeamVoiceBtn = mBind.GetCom<VoiceInputBtn>("TeamVoiceBtn");

            mComVoiceChat = mBind.GetCom<ComVoiceChat>("ComVoiceChat");
        }

        protected override void _unbindExUI()
        {
            mTalkVoiceRoot = null;
            mTeamVoiceBtnGo = null;
            mTeamVoiceBtn = null;

            mComVoiceChat = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIVoice";
        }

        private ComVoiceTalk mComVoiceTalk = null;

        protected override void OnEnter()
        {
            base.OnEnter();

            InitTeamChatVoice();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartVoteForFight, TryInitVoiceChat);
        }

        protected override void OnExit()
        {
            base.OnExit();

            _UnInitVoiceModule();
            UnInitTeamChatVoice();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartVoteForFight, TryInitVoiceChat);
        }

        private void TryInitVoiceChat(UIEvent uiEvent)
        {
            _InitVoiceModule();
        }

        private void _UnInitVoiceModule()
        {
            if(mComVoiceTalk != null)
            {
                ComVoiceTalk.ForceDestroy();
            }
        }

        private void _InitVoiceModule()
        {
            ComVoiceTalk.LocalCacheData localData = new ComVoiceTalk.LocalCacheData();            
            ComVoiceTalk.ComVoiceTalkType talkType = ComVoiceTalk.ComVoiceTalkType.None;
            bool isPk3v3 = _isVoiceOpenIn3v3();
            bool isTeam = _isVoiceOpenInTeam(); 
            bool otherSwitch = isPk3v3 || isTeam;
            if(isPk3v3)
            {
               talkType = ComVoiceTalk.ComVoiceTalkType.Pk3v3Battle;
            }  
            else if(isTeam)
            {
                talkType = ComVoiceTalk.ComVoiceTalkType.TeamDungeon;
            }
            if(mComVoiceTalk == null)
            {
                mComVoiceTalk = ComVoiceTalk.Create(mTalkVoiceRoot, localData, talkType, otherSwitch);                
            }
            if(mComVoiceTalk != null)
            {
                string pk3v3VoiceChannelId = TryGetVoiceSDKChannalId();
                mComVoiceTalk.AddGameSceneId(pk3v3VoiceChannelId);
            }
        }

        private bool _isTeamAllPlayerMoreThanOne()
        {
            if(null == BattleMain.instance)
                return false;
            if(BattleMain.instance.GetPlayerManager() == null || BattleMain.instance.GetPlayerManager().GetAllPlayers() == null)
                return false;
            bool bMoreThanOnePlayer = BattleMain.instance.GetPlayerManager ().GetAllPlayers ().Count > 1;
            return bMoreThanOnePlayer;
        }

        private bool _isVoiceOpenInTeam()
        {
            if (null == BattleMain.instance)
                return false;
            bool bMoreThanOnePlayer = _isTeamAllPlayerMoreThanOne();
            bool bModeInTeam = BattleMain.IsTeamMode(BattleMain.battleType, BattleMain.mode);
            return bModeInTeam && bMoreThanOnePlayer;
        }

        private bool _isVoiceOpenIn3v3()
        {
            if (null == BattleMain.instance)
                return false;
            if (ReplayServer.GetInstance().IsReplay())
                return false;
            if (BattleMain.instance.GetPlayerManager() == null)
                return false;
            bool bMoreThanOnePlayer = _isTeamAllPlayerMoreThanOne();
            bool bMode3v3 = BattleMain.IsModePVP3V3(BattleMain.battleType);
            return bMoreThanOnePlayer && bMode3v3;
        }

        private string TryGetVoiceSDKChannalId()
        {
            uint groupType = 0;
            if (BattleMain.IsModePVP3V3(BattleMain.battleType))
            {
                if (null != BattleMain.instance)
                    groupType = (uint)BattleMain.instance.GetPlayerManager().GetMainPlayer().teamType;
                else
                    Logger.LogError("null == BattleMain.instance!!!!");
            }
            ulong channelId = ClientApplication.playerinfo.session * 10 + groupType;
            return channelId + "";
        }

        void InitTeamChatVoice()
        {
            if(mComVoiceChat != null)
            {
                mComVoiceChat.Init(ComVoiceChat.ComVoiceChatType.Global, IsInTeamMode());
            }
        }

        bool IsInTeamMode()
        {
            if (null == BattleMain.instance)
                return false;

            if (BattleMain.instance.GetPlayerManager() == null || BattleMain.instance.GetPlayerManager().GetAllPlayers() == null)
                return false;
            bool bMoreThanOnePlayer = BattleMain.instance.GetPlayerManager().GetAllPlayers().Count > 1;
            bool bModeInTeam = BattleMain.IsTeamMode(BattleMain.battleType, BattleMain.mode);
            bool bMode3v3 = BattleMain.IsModePVP3V3(BattleMain.battleType);

            if (bMode3v3)
                return false;

            return bModeInTeam && bMoreThanOnePlayer;
        }

        void UnInitTeamChatVoice()
        {
            if(mComVoiceChat != null)
            {
                mComVoiceChat.UnInit();                
            }
        }

    }
}