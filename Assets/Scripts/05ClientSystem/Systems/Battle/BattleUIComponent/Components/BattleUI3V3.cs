using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    /// <summary>
    /// 3V3相关UI
    /// </summary>
    public class BattleUI3V3 : BattleUIBase
    {
        #region ExtraUIBind
        private GameObject mPvp3v3LeftPendingRoot = null;
        private GameObject mPvp3v3RightPendingRoot = null;
        private ComTimeLimitButton mPvp3v3TimeLimitButton = null;
        private Button mPvp3v3Button = null;
        private GameObject mPvp3v3FightOn = null;
        private GameObject mPvp3v3FightOff = null;
        private GameObject mPvp3v3TipsRoot = null;
        private Image mOpenCloseImage = null;
        private Image mPvp3v3PlayerBtnClose = null;
        private Button mPvp3v3PlayerBtn = null;
        private Image mPvp3v3PlayerBtnBg = null;
        private Image mOnOffImage = null;
        private Button mPvp3v3MicBtn = null;
        private Image mPvp3v3MicBtnClose = null;
        private Image mPvp3v3MicBtnBg = null;

        protected override void _bindExUI()
        {
            mPvp3v3LeftPendingRoot = mBind.GetGameObject("Pvp3v3LeftPendingRoot");
            mPvp3v3RightPendingRoot = mBind.GetGameObject("Pvp3v3RightPendingRoot");
            mPvp3v3TimeLimitButton = mBind.GetCom<ComTimeLimitButton>("Pvp3v3TimeLimitButton");
            mPvp3v3Button = mBind.GetCom<Button>("Pvp3v3Button");
            mPvp3v3Button.onClick.AddListener(_onPvp3v3ButtonButtonClick);
            mPvp3v3FightOn = mBind.GetGameObject("Pvp3v3FightOn");
            mPvp3v3FightOff = mBind.GetGameObject("Pvp3v3FightOff");
            mPvp3v3TipsRoot = mBind.GetGameObject("Pvp3v3TipsRoot");
            mOpenCloseImage = mBind.GetCom<Image>("OpenCloseImage");
            mPvp3v3PlayerBtnClose = mBind.GetCom<Image>("Pvp3v3PlayerBtnClose");
            mPvp3v3PlayerBtn = mBind.GetCom<Button>("Pvp3v3PlayerBtn");
            mPvp3v3PlayerBtn.onClick.AddListener(_onPvp3v3PlayerBtnButtonClick);
            mPvp3v3PlayerBtnBg = mBind.GetCom<Image>("Pvp3v3PlayerBtnBg");
            mOnOffImage = mBind.GetCom<Image>("OnOffImage");
            mPvp3v3MicBtn = mBind.GetCom<Button>("Pvp3v3MicBtn");
            mPvp3v3MicBtn.onClick.AddListener(_onPvp3v3MicBtnButtonClick);
            mPvp3v3MicBtnClose = mBind.GetCom<Image>("Pvp3v3MicBtnClose");
            mPvp3v3MicBtnBg = mBind.GetCom<Image>("Pvp3v3MicBtnBg");
        }

        protected override void _unbindExUI()
        {
            mPvp3v3LeftPendingRoot = null;
            mPvp3v3RightPendingRoot = null;
            mPvp3v3TimeLimitButton = null;
            mPvp3v3Button.onClick.RemoveListener(_onPvp3v3ButtonButtonClick);
            mPvp3v3Button = null;
            mPvp3v3FightOn = null;
            mPvp3v3FightOff = null;
            mPvp3v3TipsRoot = null;
            mOpenCloseImage = null;
            mPvp3v3PlayerBtnClose = null;
            mPvp3v3PlayerBtn.onClick.RemoveListener(_onPvp3v3PlayerBtnButtonClick);
            mPvp3v3PlayerBtn = null;
            mPvp3v3PlayerBtnBg = null;
            mOnOffImage = null;
            mPvp3v3MicBtn.onClick.RemoveListener(_onPvp3v3MicBtnButtonClick);
            mPvp3v3MicBtn = null;
            mPvp3v3MicBtnClose = null;
            mPvp3v3MicBtnBg = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUI3V3";
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
        }

        protected override void OnExit()
        {
            base.OnExit();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3StartRedyFightCount, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3VoteForFightStatusChanged, _onPK3V3StartRedyFightCount);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK3V3GetRoundEndResult, _onPK3V3GetRoundEndResult);
        }

        private void _onPvp3v3ButtonButtonClick()
        {
            /* put your code in here */
            if (null == BattleMain.instance)
                return;
            if (ReplayServer.GetInstance().IsReplay())
                return;
            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            if (null == mainPlayer)
            {
                return;
            }

            MatchRoundVote cmd = new MatchRoundVote
            {
                isVote = !mainPlayer.isVote
            };
            FrameSync.instance.FireFrameCommand(cmd);

            Logger.LogProcessFormat("[战斗] 战斗中投票 是否要出战呢 : {0}", cmd.isVote);

            _update3v3ApplyFightStatus();
        }

        private void _onPvp3v3MicBtnButtonClick()
        {
            if (ReplayServer.GetInstance().IsReplay())
                return;
            OnVoiceSDKMicClick();
        }
        private void _onPvp3v3PlayerBtnButtonClick()
        {
            if (ReplayServer.GetInstance().IsReplay())
                return;
            OnVoiceSDKPlayerClick();
        }

        void OnVoiceSDKMicClick()
        {
            if (VoiceSDK.SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
                return;
            }
            VoiceSDK.SDKVoiceManager.GetInstance().ControlRealVoiceMic();
        }


        void OnVoiceSDKPlayerClick()
        {
            if (VoiceSDK.SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_play_not_enabled"));
                return;
            }
            VoiceSDK.SDKVoiceManager.GetInstance().ControlRealVociePlayer();
        }


        private class PVP3V3Unit
        {
            public byte seat;
            public ComPVP3V3PendingCharactor com;
        }

        List<PVP3V3Unit> mPVP3V3Units = new List<PVP3V3Unit>();

        private PVP3V3Unit _getPVP3V3Unit(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return null;
            }

            for (int i = 0; i < mPVP3V3Units.Count; ++i)
            {
                if (mPVP3V3Units[i].seat == player.GetPlayerSeat())
                {
                    return mPVP3V3Units[i];
                }
            }

            return null;
        }

        private PVP3V3Unit _addPVP3V3Unit(BattlePlayer player)
        {
            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return null;
            }

            GameObject charactor = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle/Bars/Charactor/PVP3V3/3v3PendingCharactor");

            if (null == charactor)
            {
                return null;
            }

            if (player.IsTeamRed())
            {
                Utility.AttachTo(charactor, mPvp3v3LeftPendingRoot);
            }
            else
            {
                Utility.AttachTo(charactor, mPvp3v3RightPendingRoot);
            }

            PVP3V3Unit unit = new PVP3V3Unit
            {
                seat = player.GetPlayerSeat(),
                com = charactor.GetComponent<ComPVP3V3PendingCharactor>()
            };
            unit.com.InitWithSeat(unit.seat);

            mPVP3V3Units.Add(unit);

            return unit;
        }

        private void _clearPVP3V3Units()
        {
            for (int i = 0; i < mPVP3V3Units.Count; ++i)
            {
                mPVP3V3Units[i].seat = byte.MaxValue;
                mPVP3V3Units[i].com = null;
            }

            mPVP3V3Units.Clear();
        }

        private void _onPK3V3StartRedyFightCount(UIEvent ui)
        {
            _update3v3AllPendingCharactor();
            _update3v3UIVisible();
            _update3v3ApplyFightStatus();
        }

        private void _onPK3V3GetRoundEndResult(UIEvent ui)
        {
            if (null != mPvp3v3TimeLimitButton)
            {
                mPvp3v3TimeLimitButton.ResetCount();
            }

            if (null == mPvp3v3Button)
            {
                return;
            }

            mPvp3v3Button.gameObject.CustomActive(false);
        }

        private bool mIsInit3v3AllPendingCharactor = false;


        private void _init3v3AllPendingCharactor()
        {
            if (mIsInit3v3AllPendingCharactor)
            {
                return;
            }

            mIsInit3v3AllPendingCharactor = true;

            List<BattlePlayer> allPlayers = BattleMain.instance.GetPlayerManager().GetAllPlayers();

            for (int i = 0; i < allPlayers.Count; ++i)
            {
                BattlePlayer player = allPlayers[i];
                _addPVP3V3Unit(player);
            }
        }

        private void _update3v3AllPendingCharactor()
        {
            _init3v3AllPendingCharactor();

            for (int i = 0; i < mPVP3V3Units.Count; ++i)
            {
                PVP3V3Unit unit = mPVP3V3Units[i];
                if (null != unit && null != unit.com)
                {
                    Logger.LogProcessFormat("[战斗] 更新HUD信息 玩家 {0} ", unit.seat);

                    unit.com.UpdateInfo();

                    BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(unit.seat);


                    if (BattlePlayer.IsDataValidBattlePlayer(player))
                    {
                        Logger.LogProcessFormat("[战斗] 更新HUD显示隐藏 玩家 {0} {1}  是否战斗 {2} ", player.GetPlayerName(), player.GetPlayerSeat(), player.isFighting);
                        unit.com.gameObject.CustomActive(!player.isFighting);
                    }
                }
            }
        }

        private void _update3v3ApplyFightStatus()
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return;
            }

            if (!player.hasFighted)
            {
                if (player.isVote)
                {
                    mPvp3v3FightOn.CustomActive(true);
                    mPvp3v3FightOff.CustomActive(false);
                }
                else
                {
                    mPvp3v3FightOn.CustomActive(false);
                    mPvp3v3FightOff.CustomActive(true);
                }
            }
            else
            {
                _update3v3UIVisible();
            }
        }

        private void _update3v3UIVisible()
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            if (!BattlePlayer.IsDataValidBattlePlayer(player))
            {
                return;
            }

            if (null == mPvp3v3Button)
            {
                return;
            }

            bool isShow = true;

            if (player.isFighting)
            {
                isShow = false;
            }

            if (player.isPassedInRound)
            {
                isShow = false;
            }
            var battleUISSwitchWeaponAndEquip = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
            if(battleUISSwitchWeaponAndEquip!=null)
                battleUISSwitchWeaponAndEquip.InitWeaponChange();
            mPvp3v3Button.gameObject.CustomActive(isShow && !ReplayServer.instance.IsReplay());
        }

        public void ShowPVP3V3Tips()
        {
            if (null == mPvp3v3TipsRoot)
            {
                return;
            }

            mPvp3v3TipsRoot.CustomActive(true);
        }

        public void HiddenPVP3V3Tips()
        {
            if (null == mPvp3v3TipsRoot)
            {
                return;
            }

            mPvp3v3TipsRoot.CustomActive(false);
        }

        public void ChangeMicBtnStatus(bool isMicOn)
        {
            if (mOnOffImage != null)
                mOnOffImage.gameObject.CustomActive(!isMicOn);


            if (BattleMain.IsModePVP3V3(BattleMain.battleType))
            {
                if (mPvp3v3MicBtnClose != null)
                {
                    mPvp3v3MicBtnClose.gameObject.CustomActive(!isMicOn);
                }
                if (mPvp3v3MicBtnBg != null)
                {
                    mPvp3v3MicBtnBg.enabled = isMicOn;
                }
            }
        }

        public void ChangePlayerBtnStatus(bool isPlayerOpen)
        {
            if (mOpenCloseImage != null)
                mOpenCloseImage.gameObject.CustomActive(!isPlayerOpen);


            if (BattleMain.IsModePVP3V3(BattleMain.battleType))
            {
                if (mPvp3v3PlayerBtnClose != null)
                {
                    mPvp3v3PlayerBtnClose.gameObject.CustomActive(!isPlayerOpen);
                }
                if (mPvp3v3PlayerBtnBg != null)
                {
                    mPvp3v3PlayerBtnBg.enabled = isPlayerOpen;
                }
            }
        }
    }
}