using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightCountDownView : MonoBehaviour
    {
        private int _countDownShowTime = 0;
        private int _countDownTotalTime = 0;
        private float _intervalTime = 0;

        [Space(25)] [HeaderAttribute("Root")] [Space(10)]
        [SerializeField] private GameObject countDownRoot;

        [SerializeField] private GameObject fightBeginRoot;


        [Space(25)] [HeaderAttribute("Image")] [Space(10)]
        [SerializeField] private Image countDownTimeThreeImage;
        [SerializeField] private Image countDownTimeSecondImage;
        [SerializeField] private Image countDownTimeFirstImage;
        
        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {
            _countDownShowTime = TeamDuplicationDataManager.GetInstance().FightCountDownShowTime;
            _countDownTotalTime = TeamDuplicationDataManager.GetInstance().FightCountDownTotalTime;

            if (_countDownShowTime < 3)
            {
                _countDownShowTime = 3;
            }

            if (_countDownTotalTime < 6)
                _countDownTotalTime = 6;

            _intervalTime = 0.0f;
        }

        private void InitView()
        {
        }

        private void Update()
        {
            _intervalTime += Time.deltaTime;
            if (_intervalTime >= 1.0f)
            {
                UpdateTime();
                _intervalTime = 0.0f;
            }
        }

        private void UpdateTime()
        {
            if (_countDownShowTime > 0)
            {
                _countDownShowTime -= 1;
            }

            //倒计时结束，关闭界面
            if (_countDownTotalTime <= 0)
            {
                OnCloseFrame();
            }
            else
            {
                _countDownTotalTime -= 1;
            }

        }

        private void OnCloseFrame()
        {
            //阶段开始
            TeamDuplicationUtility.OnOpenTeamDuplicationFightStageBeginDescriptionFrame(
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightStageId);

            TeamDuplicationUtility.OnCloseTeamDuplicationFightCountDownFrame();
        }

    }
}
