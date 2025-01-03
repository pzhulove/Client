using UnityEngine;
using UnityEngine.UI;
using Network;

namespace GameClient
{
    /// <summary>
    /// 播放录像相关UI
    /// </summary>
    public class BattleUIReplay : BattleUIBase
    {
        public BattleUIReplay() : base() { }

        #region ExtraUIBind
        private GameObject mGoReplay = null;
        private Button mBtnReplayReturn = null;
        private Button mBtnReplaySpeed = null;
        private Text mTxtSpeedDesc = null;
        private Button mBtnReplayResume = null;
        private Button mBtnReplayPause = null;

        protected override void _bindExUI()
        {
            mGoReplay = mBind.GetGameObject("GoReplay");
            mBtnReplayReturn = mBind.GetCom<Button>("BtnReplayReturn");
            mBtnReplayReturn.onClick.AddListener(_onBtnReplayReturnButtonClick);
            mBtnReplaySpeed = mBind.GetCom<Button>("BtnReplaySpeed");
            mBtnReplaySpeed.onClick.AddListener(_onBtnReplaySpeedButtonClick);
            mTxtSpeedDesc = mBind.GetCom<Text>("TxtSpeedDesc");
            mBtnReplayResume = mBind.GetCom<Button>("BtnReplayResume");
            mBtnReplayResume.onClick.AddListener(_onBtnReplayResumeButtonClick);
            mBtnReplayPause = mBind.GetCom<Button>("BtnReplayPause");
            mBtnReplayPause.onClick.AddListener(_onBtnReplayPauseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mGoReplay = null;
            mBtnReplayReturn.onClick.RemoveListener(_onBtnReplayReturnButtonClick);
            mBtnReplayReturn = null;
            mBtnReplaySpeed.onClick.RemoveListener(_onBtnReplaySpeedButtonClick);
            mBtnReplaySpeed = null;
            mTxtSpeedDesc = null;
            mBtnReplayResume.onClick.RemoveListener(_onBtnReplayResumeButtonClick);
            mBtnReplayResume = null;
            mBtnReplayPause.onClick.RemoveListener(_onBtnReplayPauseButtonClick);
            mBtnReplayPause = null;
        }
        #endregion


        #region Callback
        private void _onBtnReplayReturnButtonClick()
        {
            if (ReplayServer.GetInstance().IsLiveShow())
            {
                if (Network.NetManager.instance != null)
                    Network.NetManager.instance.Disconnect(ServerType.RELAY_SERVER);
            }
            ReplayServer.GetInstance().Stop(false, "InitForReplay");
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }
        private void _onBtnReplaySpeedButtonClick()
        {
            if (ReplayServer.GetInstance().IsLiveShow())
            {
                ReplayServer.GetInstance().StartPersue();
                mBtnReplaySpeed.gameObject.SetActive(false);
            }
            else
            {
                ReplayServer.GetInstance().ScaleTime();
                mTxtSpeedDesc.text = string.Format("速度×{0}", ReplayServer.GetInstance().timeScaler);
            }
        }
        private void _onBtnReplayResumeButtonClick()
        {
            mBtnReplayResume.gameObject.CustomActive(false);
            mBtnReplayPause.gameObject.CustomActive(true);

            ReplayServer.GetInstance().Resume();
        }
        private void _onBtnReplayPauseButtonClick()
        {
            mBtnReplayResume.gameObject.CustomActive(true);
            mBtnReplayPause.gameObject.CustomActive(false);

            ReplayServer.GetInstance().Pause();
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIReplay";
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            InitForReplay();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.onLiveShowPursueModeChange, _onLiveShowPursueModeChange);
        }

        protected override void OnExit()
        {
            base.OnExit();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.onLiveShowPursueModeChange, _onLiveShowPursueModeChange);
        }


        public void InitForReplay()
        {
            if (!ReplayServer.GetInstance().IsReplay())
                return;

            if (mGoReplay != null)
                mGoReplay.CustomActive(true);
            
            if (mBtnReplaySpeed != null)
            {
                if (ReplayServer.GetInstance().IsLiveShow())
                {
                    mTxtSpeedDesc.text = "同步进度";
                }
                else
                {
                    mTxtSpeedDesc.text = string.Format("速度×{0}", ReplayServer.GetInstance().timeScaler);
                }
            }

            if (mBtnReplayResume != null && mBtnReplayPause != null)
            {
                if (ReplayServer.GetInstance().IsLiveShow())
                {
                    mBtnReplayResume.gameObject.CustomActive(false);
                    mBtnReplayPause.gameObject.CustomActive(false);
                }
                else
                {
                    mBtnReplayPause.gameObject.CustomActive(true);
                }

                mBtnReplayResume.gameObject.CustomActive(false);
            }
        }
        public void SetReplayVisible(bool flag)
        {
            if (mGoReplay != null)
                mGoReplay.CustomActive(flag);
        }

        private void _onLiveShowPursueModeChange(UIEvent ui)
        {
            if (ReplayServer.GetInstance().IsLiveShow())
            {
                if (!ReplayServer.GetInstance().isInPersueMode)
                {
                    mBtnReplaySpeed.gameObject.SetActive(true);
                }
            }

        }
    }
}