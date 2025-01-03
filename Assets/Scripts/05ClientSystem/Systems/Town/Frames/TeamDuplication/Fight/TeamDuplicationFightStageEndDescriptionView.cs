using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationFightStageEndDescriptionView : MonoBehaviour
    {


        private float _lastIntervalTime = 1.0f;
        private bool _isBegin = false;
        private int _stageId = 1;
        private float _curIntervalTime = 0.0f;
        //界面关闭的标志
        private bool _isCloseFrame = false;

        [Space(15)] [HeaderAttribute("Stage")] [Space(10)]
        [SerializeField] private GameObject firstStageRoot;

        [SerializeField] private GameObject secondStageRoot;
        

        public void Init(int stageId)
        {
            _stageId = stageId;
            _curIntervalTime = 0.0f;
            _lastIntervalTime = TeamDuplicationDataManager.GetInstance().TeamDuplicationStageEndIntervalTime;
            _isCloseFrame = false;

            InitView();
        }

        private void InitView()
        {
            ResetStageRoot();

            if (_stageId == 2)
            {
                CommonUtility.UpdateGameObjectVisible(secondStageRoot, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(firstStageRoot, true);
            }
        }

        private void ResetStageRoot()
        {
            CommonUtility.UpdateGameObjectVisible(firstStageRoot, false);
            CommonUtility.UpdateGameObjectVisible(secondStageRoot, false);
        }


        private void Update()
        {
            //已经关闭
            if (_isCloseFrame == true)
                return;

            if (_curIntervalTime >= _lastIntervalTime)
            {
                //关闭界面，设置标志位
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
            UIEventSystem.GetInstance().SendUIEvent(
                EUIEventID.OnReceiveTeamDuplicationStageEndDescriptionCloseMessage,
                _stageId);

            TeamDuplicationUtility.OnCloseTeamDuplicationFightStageEndDescriptionFrame();
        }

    }
}
