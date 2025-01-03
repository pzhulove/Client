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

    //重生状态控制器
    public class TeamDuplicationFightPointRebornStatusControl : MonoBehaviour
    {

        private float _totalCountDownTime = 0;
        private float _curCountDownTime = 0;

        [Space(10)]
        [HeaderAttribute("Cover")]
        [Space(5)]
        [SerializeField] private Text countDownTimeLabel;
        [SerializeField] private Image sliderImageCover;


        //totalCountDownTime 必须大于0
        public void Init(float totalCountDownTime)
        {
            _totalCountDownTime = totalCountDownTime;
            _curCountDownTime = totalCountDownTime;

            if (_totalCountDownTime <= 0)
                return;

            UpdateCountDownView();
        }

        public void Init(int totalCountDownTime)
        {
            Init((float) totalCountDownTime);
        }

        private void Update()
        {
            if (_curCountDownTime <= 0)
                return;

            _curCountDownTime -= Time.deltaTime;

            UpdateCountDownView();
        }

        private void UpdateCountDownView()
        {
            float sliderValue = _curCountDownTime / _totalCountDownTime;
            if (sliderValue <= 0)
            {
                sliderValue = 0.0f;
            }else if (sliderValue >= 1)
            {
                sliderValue = 1.0f;
            }            
            if (sliderImageCover != null)
                sliderImageCover.fillAmount = sliderValue;

            //倒计时
            var countDownTimeStr = string.Format(TR.Value("team_duplication_reborn_status_count_down_time_format"),
                (int) (_curCountDownTime + 0.5f));

            countDownTimeLabel.text = countDownTimeStr;

        }
        



    }
}
