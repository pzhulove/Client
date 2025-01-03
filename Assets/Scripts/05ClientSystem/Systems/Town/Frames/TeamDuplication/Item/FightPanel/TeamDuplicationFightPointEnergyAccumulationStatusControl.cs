using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{

    //能量蓄积状态控制器
    public class TeamDuplicationFightPointEnergyAccumulationStatusControl : MonoBehaviour
    {

        private int _startTime = 0;

        [Space(10)]
        [HeaderAttribute("Cover")]
        [Space(5)]
        [SerializeField] private Text countDownTimeLabel;
        [SerializeField] private Image sliderImageCover;
        [SerializeField] private Text energyAccumulationFinishedLabel;
        [SerializeField] private Text energyAccumulationProcessingLabel;


        
        public void Init(int startTime)
        {
            _startTime = startTime;

            UpdateStatusView();
        }

        public void UpdateStatusView()
        {
            var intervalTime = (int)TimeManager.GetInstance().GetServerTime() - _startTime;
            if (intervalTime <= 0)
                intervalTime = 0;

            //进度条
            float sliderValue = 0.0f;
            if (intervalTime >= 1000)
            {
                sliderValue = 1.0f;
            }
            else
            {
                sliderValue = (float) intervalTime / 1000.0f;
            }            
            if (sliderImageCover != null)
                sliderImageCover.fillAmount = sliderValue;

            //进度百分比
            int firstPart = 0;
            int secondPart = 0;
            if (intervalTime >= 1000)
            {
                firstPart = 100;
            }
            else
            {
                firstPart = intervalTime / 10;
                secondPart = intervalTime % 10;
            }

            if (countDownTimeLabel != null)
            {
                //100%
                if (firstPart >= 100)
                {
                    //显示100%
                    countDownTimeLabel.text = string.Format(
                        TR.Value("team_duplication_fight_point_energy_accumulation_percent_format_second"),
                        100);
                }
                else
                {
                    //小数部分为0，不显示小数部分
                    if (secondPart == 0)
                    {
                        countDownTimeLabel.text = string.Format(
                            TR.Value("team_duplication_fight_point_energy_accumulation_percent_format_second"),
                            firstPart);
                    }
                    else
                    {
                        //显示小数部分
                        countDownTimeLabel.text = string.Format(
                            TR.Value("team_duplication_fight_point_energy_accumulation_percent_format_one"),
                            firstPart, secondPart);
                    }
                }
            }

            if (firstPart < 100)
            {
                //能量积蓄中...  小于100%
                CommonUtility.UpdateTextVisible(energyAccumulationProcessingLabel, true);
                CommonUtility.UpdateTextVisible(energyAccumulationFinishedLabel, false);
            }
            else
            {
                //boss已变强        100%
                CommonUtility.UpdateTextVisible(energyAccumulationProcessingLabel, false);
                CommonUtility.UpdateTextVisible(energyAccumulationFinishedLabel, true);
            }

        }
        
    }
}
