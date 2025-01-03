using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    /// <summary>
    /// Pvp练习场相关UI
    /// </summary>
    public class BattleUITrainPvp : BattleUIBase
    {
        public BattleUITrainPvp() : base() { }

        #region ExtraUIBind
        private GameObject mGoTrain = null;
        private Button mBtnTrainReturn = null;
        private Button mBtnTrainReset = null;
        private UIGray mBtnTrainResetGray = null;
        private SimpleTimer mTimerController = null;

        protected override void _bindExUI()
        {
            mGoTrain = mBind.GetGameObject("GoTrain");
            mBtnTrainReturn = mBind.GetCom<Button>("BtnTrainReturn");
            mBtnTrainReturn.onClick.AddListener(_onBtnTrainReturnButtonClick);
            mBtnTrainReset = mBind.GetCom<Button>("BtnTrainReset");
            mBtnTrainReset.onClick.AddListener(_onBtnTrainResetButtonClick);
            mBtnTrainResetGray = mBind.GetCom<UIGray>("BtnTrainResetGray");
            mTimerController = mBind.GetCom<SimpleTimer>("TimerController");
        }

        protected override void _unbindExUI()
        {
            mGoTrain = null;
            mBtnTrainReturn.onClick.RemoveListener(_onBtnTrainReturnButtonClick);
            mBtnTrainReturn = null;
            mBtnTrainReset.onClick.RemoveListener(_onBtnTrainResetButtonClick);
            mBtnTrainReset = null;
            mBtnTrainResetGray = null;
            mTimerController = null;
        }
        #endregion

        #region Callback
        private void _onBtnTrainReturnButtonClick()
        {
            BeUtility.ResetCamera();
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }

        private void _onBtnTrainResetButtonClick()
        {
            var battle = BattleMain.instance.GetBattle() as TrainingBatte2;
            if (battle != null)
            {
                if (!_canTrainReset)
                    return;
                if (BattleMain.instance.GetPlayerManager() == null) return;
                var node = BattleMain.instance.GetPlayerManager().GetMainPlayer();
                if (node != null)
                {
                    BeActor actor = node.playerActor;
                    BeSkill currSkill = actor.GetCurrentSkill();
                    if (currSkill != null)
                    {
                        currSkill.joystickMode = SkillJoystickMode.FREE;
                        currSkill.RemoveJoystickEffect();
                        currSkill.EndJoystick();
                        currSkill.Cancel();
                    }
                }
                _canTrainReset = false;

#if UNITY_EDITOR &&!LOGIC_SERVER
                //编辑器下重新加载技能配置文件（方便策划测技能修改）
                BeActionFrameMgr.Clear(true);
#endif

                battle.RecreatePlayers();
                if (mTimerController != null)
                    mTimerController.Reset();
                mBtnTrainResetGray.enabled = true;
                ClientSystemManager.GetInstance().delayCaller.DelayCall(5000, () =>
                {
                    _canTrainReset = true;
                    if (mBtnTrainReset == null) return;
                    mBtnTrainReset.enabled = true;
                    mBtnTrainResetGray.enabled = false;
                });
                BeUtility.ResetCamera();
                var battleCommon = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
                if (battleCommon != null)
                    battleCommon.ResetCombo();
            }
            AudioManager.instance.StopAll(AudioType.AudioEffect);
        }
        #endregion

        private bool _canTrainReset = true;

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUITrainPvp";
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            InitForTrain();
        }

        public void InitForTrain()
        {
            _canTrainReset = true;
            if (mGoTrain != null)
                mGoTrain.CustomActive(true);

            StartTimer();
        }

        private void StartTimer(int countdown = 0)
        {
            if (mTimerController != null)
            {
                if (countdown > 0)
                    mTimerController.SetCountdown(countdown);
                mTimerController.StartTimer();
            }
        }
    }
}