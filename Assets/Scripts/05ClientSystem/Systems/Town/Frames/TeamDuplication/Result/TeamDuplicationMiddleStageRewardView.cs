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

    public class TeamDuplicationMiddleStageRewardView : MonoBehaviour
    {

        private int _stageId = 0;
        private float _countDownTime = 0;

        private bool _isShowCountDownTimeFinished = false;          //时间倒计时是否显示完成

        private int _rewardIndex = 0;           //翻牌牌面的索引
        private float _refreshInterval = 0.2f;          //翻牌特效播放的时间间隔

        private int _closeFrameIntervalTime = 7;            //倒计时结束7s后，强制关闭界面
        private float _rewardShowIntervalTime = 2.5f;       //奖励都展示完之后，额外展示的时间

        private List<TeamDuplicationFightStageRewardDataModel> _fightStageMiddleRewardDataModelList = null;

        [Space(15)]
        [HeaderAttribute("Text")]
        [Space(10)]
        [SerializeField] private Text fightStageLabel;

        [Space(15)]
        [HeaderAttribute("CountDownTimeImage")]
        [Space(5)]
        [SerializeField] private Image countDownTimeFiveImage;
        [SerializeField] private Image countDownTimeFourImage;
        [SerializeField] private Image countDownTimeThreeImage;
        [SerializeField] private Image countDownTimeSecondImage;
        [SerializeField] private Image countDownTimeFirstImage;
        [SerializeField] private Image countDownTimeZeroImage;

        [Space(15)]
        [HeaderAttribute("CloseCountDownTimeController")]
        [Space(5)]
        [SerializeField] private CountDownTimeController closeCountDownTimeController;

        [Space(15)]
        [HeaderAttribute("RewardItem")]
        [Space(10)]
        [SerializeField]
        private List<TeamDuplicationMiddleStageRewardItem> middleStageRewardItemList =
            new List<TeamDuplicationMiddleStageRewardItem>();

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
            StopAllCoroutines();
        }

        private void BindUiEvents()
        {
            if (closeCountDownTimeController != null)
                closeCountDownTimeController.SetCountDownTimeCallback(OnCloseFrame);
        }

        private void UnBindUiEvents()
        {
            if (closeCountDownTimeController != null)
                closeCountDownTimeController.SetCountDownTimeCallback(null);
        }

        private void ClearData()
        {
            _stageId = 0;
            _countDownTime = 0;

            _fightStageMiddleRewardDataModelList = null;

            _isShowCountDownTimeFinished = false;
        }

        private void OnEnable()
        {
            BindUiMessages();
        }

        private void OnDisable()
        {
            UnBindUiMessages();
        }

        private void BindUiMessages()
        {
            //奖励通知
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStageRewardNotify,
                OnReceiveTeamDuplicationFightStageRewardNotify);
        }

        private void UnBindUiMessages()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightStageRewardNotify,
                OnReceiveTeamDuplicationFightStageRewardNotify);
        }

        public void Init(int stageId)
        {
            _stageId = stageId;

            //发送协议
            TeamDuplicationDataManager.GetInstance().ResetFightStageRewardDataModelList();
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTargetFlopReq(_stageId);

            _countDownTime = TeamDuplicationDataManager.GetInstance().TeamDuplicationFightStageRewardTime;

            //默认第一阶段 3s;
            if (_stageId <= 0)
                _stageId = 1;
            if (_countDownTime <= 0)
                _countDownTime = 3;

            InitView();
        }

        private void InitView()
        {
            InitFightStageRewardCommonLabel();
            InitStageRewardItem();

            UpdateStageRewardCountDownTime(_countDownTime);

            SetCloseFrameCountDownController();
        }

        //用于关闭倒计时
        private void SetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.EndTime =
                TimeManager.GetInstance().GetServerTime() + (uint)(_countDownTime + _closeFrameIntervalTime);
            closeCountDownTimeController.InitCountDownTimeController();
        }

        private void ResetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.ResetCountDownTimeController();
        }

        private void InitFightStageRewardCommonLabel()
        {
            if (fightStageLabel != null)
            {
                var stageDescription = TeamDuplicationUtility.GetTeamDuplicationStageDescription(_stageId);
                if (string.IsNullOrEmpty(stageDescription) == false)
                    fightStageLabel.text = stageDescription;
            }
        }

        private void InitStageRewardItem()
        {
            if (middleStageRewardItemList == null)
                return;

            for (var i = 0; i < middleStageRewardItemList.Count; i++)
            {
                var curStageRewardItem = middleStageRewardItemList[i];
                if (curStageRewardItem != null)
                {
                    curStageRewardItem.Init(_stageId);
                }
            }
        }

        #region Update
        private void Update()
        {
            OnShowCountDownTimeUpdate();
        }

        //倒计时过程
        private void OnShowCountDownTimeUpdate()
        {
            if (_isShowCountDownTimeFinished == true)
                return;

            _countDownTime -= Time.deltaTime;
            if (_countDownTime <= -1)
            {
                //倒计时完成
                _isShowCountDownTimeFinished = true;
                //隐藏倒计时
                ResetCountDownTimeImage();
                //开始展示奖励
                StartMiddleStageRewardAction();
            }
            else
            {
                //倒计时：3,2,1,0
                UpdateStageRewardCountDownTime(_countDownTime + 1.0f);
            }
        }

        //更新倒计时的显示
        private void UpdateStageRewardCountDownTime(float countDownTime)
        {
            ResetCountDownTimeImage();

            var curCountDownTime = (int)countDownTime;
            if (curCountDownTime >= 5)
            {
                CommonUtility.UpdateImageVisible(countDownTimeFiveImage, true);
            }
            else if (curCountDownTime == 4)
            {
                CommonUtility.UpdateImageVisible(countDownTimeFourImage, true);
            }
            else if (curCountDownTime == 3)
            {
                CommonUtility.UpdateImageVisible(countDownTimeThreeImage, true);
            }
            else if (curCountDownTime == 2)
            {
                CommonUtility.UpdateImageVisible(countDownTimeSecondImage, true);
            }
            else if (curCountDownTime == 1)
            {
                CommonUtility.UpdateImageVisible(countDownTimeFirstImage, true);
            }
            else
            {
                CommonUtility.UpdateImageVisible(countDownTimeZeroImage, true);
            }
        }

        //开启小阶段翻牌奖励的流程
        private void StartMiddleStageRewardAction()
        {
            _rewardIndex = 0;
            InvokeRepeating("OnShowMiddleStageRewardAction", 0, _refreshInterval);
        }

        //逐个翻牌
        private void OnShowMiddleStageRewardAction()
        {
            if (_fightStageMiddleRewardDataModelList == null
                || _fightStageMiddleRewardDataModelList.Count <= 0
                || middleStageRewardItemList == null 
                || middleStageRewardItemList.Count <= 0)
            {
                CancelFinalStageRewardAction();
                return;
            }

            if (_rewardIndex < 0 
                || _rewardIndex >= _fightStageMiddleRewardDataModelList.Count
                || _rewardIndex >= middleStageRewardItemList.Count)
            {
                CancelFinalStageRewardAction();
                return;
            }

            //初始化卡牌奖励
            var fightStageRewardDataModel = _fightStageMiddleRewardDataModelList[_rewardIndex];
            if (fightStageRewardDataModel != null)
            {
                for (var i = 0; i < middleStageRewardItemList.Count; i++)
                {
                    var middleStageRewardItem = middleStageRewardItemList[i];
                    if(middleStageRewardItem == null)
                        continue;

                    if(middleStageRewardItem.GetRewardItemIndex() != fightStageRewardDataModel.RewardIndex)
                        continue;

                    if(middleStageRewardItem.GetRewardItemInitState() == true)
                        continue;

                    middleStageRewardItem.UpdateRewardItem(fightStageRewardDataModel);
                }
            }
            _rewardIndex += 1;
        }

        //取消翻牌
        private void CancelFinalStageRewardAction()
        {
            CancelInvoke("OnShowMiddleStageRewardAction");

            //隐藏未翻的牌面
            DisappearMiddleStageUnRewardCardList();

            //1s后自动关闭界面
            StartCoroutine(CloseFrameByIntervalTime());
        }

        //隐藏未翻的牌
        private void DisappearMiddleStageUnRewardCardList()
        {
            if (middleStageRewardItemList != null)
            {
                for (var i = 0; i < middleStageRewardItemList.Count; i++)
                {
                    var middleStageRewardItem = middleStageRewardItemList[i];
                    if(middleStageRewardItem == null)
                        continue;

                    //已经初始化Item不隐藏
                    if(middleStageRewardItem.GetRewardItemInitState() == true)
                        continue;

                    CommonUtility.UpdateGameObjectVisible(middleStageRewardItem.gameObject, false);
                }
            }
        }

        #endregion

        #region UIEvent

        private void OnReceiveTeamDuplicationFightStageRewardNotify(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var stageId = (int)uiEvent.Param1;
            if (stageId != _stageId)
                return;

            //获得奖励数据
            _fightStageMiddleRewardDataModelList =
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightStageRewardDataModelList;
        }

        //展示奖励
        private void UpdateFightStageRewardItemList()
        {

            //对应的奖励不存在，1s后关闭
            if (_fightStageMiddleRewardDataModelList == null
                || _fightStageMiddleRewardDataModelList.Count <= 0)
            {
                StartCoroutine(CloseFrameByIntervalTime());
                return;
            }

            //展示小队的奖励
            for (var i = 0; i < _fightStageMiddleRewardDataModelList.Count; i++)
            {
                var stageRewardDataModel = _fightStageMiddleRewardDataModelList[i];
                if (stageRewardDataModel == null)
                    continue;

                for (var j = 0; j < middleStageRewardItemList.Count; j++)
                {
                    var middleStageRewardItem = middleStageRewardItemList[j];
                    if (middleStageRewardItem == null)
                        continue;
                    //索引不同
                    if (middleStageRewardItem.GetRewardItemIndex() != stageRewardDataModel.RewardIndex)
                        continue;
                    //已经初始化
                    if (middleStageRewardItem.GetRewardItemInitState() == true)
                        continue;

                    middleStageRewardItem.UpdateRewardItem(stageRewardDataModel);

                }
            }

            //小队数量存在空缺，直接隐藏
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerNumberInCaptain
                != _fightStageMiddleRewardDataModelList.Count)
            {
                for (var i = 0; i < middleStageRewardItemList.Count; i++)
                {
                    var middleStageRewardItem = middleStageRewardItemList[i];
                    if(middleStageRewardItem == null)
                        continue;
                    if(middleStageRewardItem.GetRewardItemInitState() == true)
                        continue;
                    //没有翻牌的卡牌，直接隐藏掉
                    CommonUtility.UpdateGameObjectVisible(middleStageRewardItem.gameObject, false);
                }
            }

            //等待1s关闭界面
            StartCoroutine(CloseFrameByIntervalTime());
        }

        private IEnumerator CloseFrameByIntervalTime()
        {
            //等待1s
            yield return new WaitForSeconds(_rewardShowIntervalTime);
            OnCloseFrame();
        }

        #endregion

        private void ResetCountDownTimeImage()
        {
            CommonUtility.UpdateImageVisible(countDownTimeZeroImage, false);
            CommonUtility.UpdateImageVisible(countDownTimeFirstImage, false);
            CommonUtility.UpdateImageVisible(countDownTimeSecondImage, false);
            CommonUtility.UpdateImageVisible(countDownTimeThreeImage, false);
            CommonUtility.UpdateImageVisible(countDownTimeFourImage, false);
            CommonUtility.UpdateImageVisible(countDownTimeFiveImage, false);
        }

        private void OnCloseFrame()
        {

            ResetCloseFrameCountDownController();

            //小阶段翻牌结束
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationMiddleStageRewardCloseMessage,
                _stageId);

            TeamDuplicationDataManager.GetInstance().ResetFightStageRewardDataModelList();

            //道具的TipFrame界面
            ItemTipManager.GetInstance().CloseAll();

            TeamDuplicationUtility.OnCloseTeamDuplicationMiddleStageRewardFrame();
        }

    }
}
