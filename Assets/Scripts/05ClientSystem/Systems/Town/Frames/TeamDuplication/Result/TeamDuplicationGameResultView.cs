using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationGameResultView : MonoBehaviour
    {
        [Space(25)]
        [HeaderAttribute("GameResult")]
        [Space(15)]
        [SerializeField] private GameObject gameSucceedRoot;
        [SerializeField] private CommonGameObjectOrderShowControl succeedOrderShowControl;
        [SerializeField] private GameObject gameFailRoot;
        [SerializeField] private CommonGameObjectOrderShowControl failedOrderShowControl;

        [Space(25)]
        [HeaderAttribute("CloseCountDownTimeController")]
        [Space(15)]
        [SerializeField] private CountDownTimeController closeCountDownTimeController;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ResetCloseFrameCountDownController();
        }

        private void BindUiEvents()
        {
            if (closeCountDownTimeController != null)
                closeCountDownTimeController.SetCountDownTimeCallback(OnCloseFrame);

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }
        }

        private void UnBindUiEvents()
        {
            if (closeCountDownTimeController != null)
                closeCountDownTimeController.SetCountDownTimeCallback(null);

            if(closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }


        public void Init(bool isSuccess = false)
        {
			//播放音效
            AudioManager.instance.PlaySound(TeamDuplicationDataManager.TeamDuplicationAudioGameResultId);

            if (isSuccess == true)
            {
                //成功

                CommonUtility.UpdateGameObjectVisible(gameFailRoot, false);

                CommonUtility.UpdateGameObjectVisible(gameSucceedRoot, true);
            }
            else
            {
                //失败

                CommonUtility.UpdateGameObjectVisible(gameSucceedRoot, false);

                CommonUtility.UpdateGameObjectVisible(gameFailRoot, true);
            }

            SetCloseFrameCountDownController();
        }

        private void SetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.EndTime =
                TimeManager.GetInstance().GetServerTime() +
                (uint) TeamDuplicationDataManager.GetInstance().GameResultShowTime;

            closeCountDownTimeController.InitCountDownTimeController();
        }

        private void ResetCloseFrameCountDownController()
        {
            if(closeCountDownTimeController != null)
                closeCountDownTimeController.ResetCountDownTimeController();
        }


        private void OnCloseFrame()
        {
            TeamDuplicationUtility.OnCloseTeamDuplicationGameResultFrame();
        }

        private void OrderShowFinish()
        {
            CommonUtility.UpdateButtonVisible(closeButton, true);
        }

    }
}
