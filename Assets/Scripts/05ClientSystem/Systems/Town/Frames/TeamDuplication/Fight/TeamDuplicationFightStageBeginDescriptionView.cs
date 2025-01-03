using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightStageBeginDescriptionView : MonoBehaviour
    {

        private float _lastIntervalTime = 1.0f;
        private bool _isBegin = false;
        private int _stageId = 1;
        private float _curIntervalTime = 0.0f;
        //界面关闭的标志
        private bool _isCloseFrame = false;

        [Space(15)]
        [HeaderAttribute("Stage")]
        [Space(10)]
        [SerializeField] private GameObject firstStageRoot;

        [SerializeField] private GameObject secondStageRoot;


        public void Init(int stageId, bool isBegin = true)
        {
            _stageId = stageId;
            _isBegin = isBegin;
            _curIntervalTime = 0.0f;
            _lastIntervalTime = TeamDuplicationDataManager.GetInstance().TeamDuplicationStageBeginIntervalTime;
            _isCloseFrame = false;

            InitView();
        }

        private void InitView()
        {
            ResetStageRoot();

            if (_stageId == 1)
            {
                CommonUtility.UpdateGameObjectVisible(firstStageRoot, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(secondStageRoot, true);
            }            
        }


        private void ResetStageRoot()
        {
            CommonUtility.UpdateGameObjectVisible(firstStageRoot, false);
            CommonUtility.UpdateGameObjectVisible(secondStageRoot, false);
        }

        private void Update()
        {
            if (_isCloseFrame == true)
                return;

            if (_curIntervalTime >= _lastIntervalTime)
            {
                OnCloseFrame();
                _isCloseFrame = true;
                return;
            }
            else
            {
                _curIntervalTime += Time.deltaTime;
            }
        }

        private void OnCloseFrame()
        {
            //阶段开始
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTeamDuplicationFightStageBeginMessage,
                _stageId);

            //第一阶段奖励流程结束
            if (_stageId == 2)
            {
                UIEventSystem.GetInstance()
                    .SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightStageEndShowFinishMessage);
            }

            TeamDuplicationUtility.OnCloseTeamDuplicationFightStageBeginDescriptionFrame();
        }

    }
}
