using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class InstituteBattleFrame : ClientFrame
    {

        [UIObject("settingPanel")]
        GameObject settingFrame;

        [UIObject("controlContainer")]
        GameObject controlContainer;

        [UIObject("controlContainer/playbtn")]
        GameObject playbtn;

        [UIObject("controlContainer/pauseBtn")]
        GameObject pauseBtn;

        [UIObject("controlContainer/rate")]
        GameObject rate;

        [UIObject("GameObject/resetBtn/EffUI_lianzhaoxitong_anniu")]
        GameObject effect;

        [UIObject("GameObject/custom_Image/phase")]
        GameObject phase;

        [UIObject("GameObject/resetBtn")]
        GameObject resetBtn;

        [UIObject("GameObject/returnBtn")]
        GameObject returnBtn;

        [UIObject("GameObject/returnTeachBtn")]
        GameObject returnTeachBtn;

        public bool hasPassed = false;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/InstituteBattleFrame";
        }

        protected override void _bindExUI()
        {
            base._bindExUI();
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            List<InstituteTable> list = TableManager.instance.GetJobInstituteData(BattleMain.instance.GetLocalPlayer().playerInfo.occupation);
            InstituteTable curData = list.Find(x => { return x.ID == InstituteFrame.id; });
            hasPassed = MissionManager.GetInstance().GetState(curData)==1;
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
        }

        public void SetBtnState(bool flag)
        {
            resetBtn.CustomActive(flag);
            returnTeachBtn.CustomActive(flag);
        }

        public void SetBtnEnable(bool flag)
        {
            if (!IsOpen()) return;
            returnBtn.GetComponent<Button>().interactable = flag;
            resetBtn.GetComponent<Button>().interactable = flag;
            returnTeachBtn.GetComponent<Button>().interactable = flag;
        }

        public void SetTitle(int phase)
        {
            if (phase == 0)
            {
                this.phase.GetComponent<Text>().text = "自动演示";
            }
            else
            {
                this.phase.GetComponent<Text>().text = "自由挑战";
            }
        }

        [UIEventHandle("GameObject/returnBtn")]
        void OpenSettingFrame()
        {
            settingFrame.CustomActive(true);
        }

        public void SetEffectState(bool flag)
        {
            effect.CustomActive(flag);
        }


        bool canClick = true;
        [UIEventHandle("GameObject/returnTeachBtn")]
        void ReturnTeach()
        {
            ClientSystemManager.instance.delayCaller.DelayCall(1000, () =>
            {
                canClick = true;
            });
            if (canClick)
            {
                canClick = false;
                SkillComboControl.instance.RestartTeachFight();
            }
        }

        [UIEventHandle("GameObject/resetBtn")]
        void ResetTrain()
        {
            ClientSystemManager.instance.delayCaller.DelayCall(1000, () =>
            {
                canClick = true;
            });

            if (canClick)
            {
                canClick = false;
                SkillComboControl.instance.RestartPracticeFight();
            }
        }

        [UIEventHandle("settingPanel/cancel")]
        void ReturnTrain()
        {
            settingFrame.CustomActive(false);
        }


        [UIEventHandle("settingPanel/ok")]
        void ReturnCaterlog()
        {
            BeUtility.ResetCamera();
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }

        private float timeScale = 1.0f;
        [UIEventHandle("controlContainer/playbtn")]
        void Play()
        {
            Time.timeScale = timeScale;
            pauseBtn.CustomActive(true);
            playbtn.CustomActive(false);
        }

        [UIEventHandle("controlContainer/accBtn")]
        void Acc()
        {
            timeScale += 0.2f;
            timeScale = Mathf.Clamp(timeScale, 0, 1);
            SetTimeScale();
        }

        [UIEventHandle("controlContainer/pauseBtn")]
        void Pause()
        {
            Time.timeScale = 0;
            pauseBtn.CustomActive(false);
            playbtn.CustomActive(true);
        }

        [UIEventHandle("controlContainer/backBtn")]
        void Back()
        {
            timeScale -= 0.2f;
            timeScale = Mathf.Clamp(timeScale, 0, 1);
            SetTimeScale();
        }

        private void SetTimeScale()
        {
            Time.timeScale = timeScale;
            rate.GetComponent<Text>().text = string.Format("{0}x", string.Format("{0:N1}", Time.timeScale));
        }

        public void SetControlContainer(bool flag)
        {
            controlContainer.CustomActive(flag);
        }

        public void ResetTimeScale()
        {
            timeScale = 1;
            SetTimeScale();
        }

    }
}
