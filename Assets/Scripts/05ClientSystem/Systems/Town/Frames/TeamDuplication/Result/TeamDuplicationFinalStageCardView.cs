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

    public class TeamDuplicationFinalStageCardView : MonoBehaviour
    {

        private string middleEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_tuanben_zhankai_zhong";

        private int _stageId = 0;
        private uint _closeFrameIntervalTime;            //倒计时,强制关闭界面

        private float _refreshInterval = 0.2f;          //翻牌特效播放的时间间隔
        private int _rewardIndex = 0;

        private float _curShowCloseButtonLeftTime = 0.0f;           //关闭按钮展示剩余时间

        private bool _isReceiveRewardData = false;              //是否已经收到奖励数据
        private bool _isNeedShowRewardItem = false;             //是否需要展示奖励Item

        //奖励的数据
        private List<TeamDuplicationFightStageRewardDataModel> _finalStageRewardDataModelList = null;

        private const int RewardCoverEffectTimeInterval = 1; //背面特效，1帧加载3个，避免一次加载过多
        private int _rewardCoverEffectCurInterval = 0;              //临时变量
        private int _rewardCoverEffectRootIndex = 4;                    //从右到左边(4,3,2,1)
        private const int RewardCoverEffectNumberPerLine = 4;                       //每行存在4张卡牌

        [Space(15)] [HeaderAttribute("Common")] [Space(10)]
        //动画播放间隔
        [SerializeField] private float waitActionTime = 1.30f;
        //关闭按钮的间隔时间
        [SerializeField] private float showCloseButtonIntervalTime = 5.0f;
        [SerializeField] private CountDownTimeController closeCountDownTimeController;

        //奖励
        [Space(15)] [HeaderAttribute("StageCard")] [Space(10)]
        [SerializeField] private List<TeamDuplicationFinalStageCardItem> finalStageCardItemList =
            new List<TeamDuplicationFinalStageCardItem>();

        [Space(15)] [HeaderAttribute("StageCardMiddleEffect")] [Space(10)]
        [SerializeField] private GameObject middleEffectRoot;

        [Space(15)]
        [HeaderAttribute("CloseButton")]
        [Space(15)]
        [SerializeField]
        private Button closeButton;

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

            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _stageId = 0;
            _finalStageRewardDataModelList = null;
            _isReceiveRewardData = false;
            _isNeedShowRewardItem = false;

            _rewardCoverEffectRootIndex = 4;
            _rewardCoverEffectCurInterval = 0;
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
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStageRewardNotify,
                OnReceiveTeamDuplicationFightStageRewardNotify);
        }

        private void UnBindUiMessages()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStageRewardNotify,
                OnReceiveTeamDuplicationFightStageRewardNotify);
        }

        public void Init()
        {
            TeamDuplicationDataManager.GetInstance().IsAlreadyReceiveFinalReward = true;
            _stageId = (int)TeamCopyStage.TEAM_COPY_STAGE_FINAL;

            TeamDuplicationDataManager.GetInstance().ResetFightStageRewardDataModelList();
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTargetFlopReq(_stageId);

            _closeFrameIntervalTime = TeamDuplicationDataManager.GetInstance().TeamDuplicationFinalStageRewardTime;


            _rewardCoverEffectRootIndex = 4;
            _rewardCoverEffectCurInterval = 0;

            InitView();
        }

        private void InitView()
        {
            SetCloseFrameCountDownController();
            LoadMiddleEffect();
        }

        private void LoadMiddleEffect()
        {
            CommonUtility.LoadGameObjectWithPath(middleEffectRoot, middleEffectPath);
        }

        private void Update()
        {
            //牌扩展背面特效
            OnShowRewardItemCoverEffectUpdate();

            //翻牌特效
            OnShowFinalStageRewardActionUpdate();
            //关闭按钮
            OnShowCloseButtonUpdate();
        }

        //展示最终翻牌的特效
        private void OnShowFinalStageRewardActionUpdate()
        {
            if (waitActionTime <= 0)
                return;

            waitActionTime -= Time.deltaTime;
            //等待一定的时间,如果收到了奖励，则展示奖励的动画，如果没有展示，则设置标志位
            if (waitActionTime <= 0)
            {
                //收到奖励，展示动画
                if (_isReceiveRewardData == true)
                {
                    _isNeedShowRewardItem = false;
                    StartFinalStageRewardAction();
                }
                else
                {
                    //时间到了，没有收到奖励，设置标志
                    _isNeedShowRewardItem = true;
                }
            }
        }

        //展示关闭按钮的更新
        private void OnShowCloseButtonUpdate()
        {
            if (_curShowCloseButtonLeftTime <= 0)
                return;

            _curShowCloseButtonLeftTime -= Time.deltaTime;
            if (_curShowCloseButtonLeftTime <= 0)
            {
                ShowCloseButton();
            }
        }

        #region RewardItemCoverEffect
        //控制加载某一列的特效
        private void OnShowRewardItemCoverEffectUpdate()
        {
            //加载完成
            if (_rewardCoverEffectRootIndex <= 0)
                return;

            if (_rewardCoverEffectCurInterval < RewardCoverEffectTimeInterval)
            {
                _rewardCoverEffectCurInterval += 1;
            }
            else
            {
                //加载某一列的cover特效
                OnLoadRewardCoverEffectByIndex(_rewardCoverEffectRootIndex);
                //间隔重置
                _rewardCoverEffectCurInterval = 0;
                //特效索引减1
                _rewardCoverEffectRootIndex -= 1;
            }
        }

        //加载某一列的cover特效
        private void OnLoadRewardCoverEffectByIndex(int coverEffectRootIndex)
        {
            //第一排
            var firstEffectRootIndex = coverEffectRootIndex;
            OnLoadOneCoverEffect(firstEffectRootIndex);

            //第二排
            var secondEffectRootIndex = coverEffectRootIndex + RewardCoverEffectNumberPerLine;
            OnLoadOneCoverEffect(secondEffectRootIndex);

            //第三排
            var thirdEffectRootIndex = coverEffectRootIndex + RewardCoverEffectNumberPerLine * 2;
            OnLoadOneCoverEffect(thirdEffectRootIndex);
        }

        //加载具体的某一个cover特效
        private void OnLoadOneCoverEffect(int effectRootIndex)
        {
            if (finalStageCardItemList == null || finalStageCardItemList.Count <= 0)
                return;
            //是否处在List中
            if (effectRootIndex >= 1 && effectRootIndex <= finalStageCardItemList.Count)
            {
                var stageCardItem = finalStageCardItemList[effectRootIndex - 1];
                if(stageCardItem != null)
                    stageCardItem.LoadStageCardCoverEffect();
            }
        }
        #endregion 

        #region UIEvent

        //收到奖励的翻牌，则进行奖励翻牌的动画
        private void OnReceiveTeamDuplicationFightStageRewardNotify(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var stageId = (int)uiEvent.Param1;
            if (stageId != _stageId)
                return;

            _finalStageRewardDataModelList =
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightStageRewardDataModelList;

            //收到奖励数据
            _isReceiveRewardData = true;
            //动画播放完之后，才收到奖励，则展示动画
            if (_isNeedShowRewardItem == true)
            {
                _isNeedShowRewardItem = false;
                StartFinalStageRewardAction();
            }
        }

        #endregion

        #region StageRewardItemAction
        //展示奖励
        private void StartFinalStageRewardAction()
        {
            _rewardIndex = 0;
            InvokeRepeating("OnShowFinalStageRewardAction", 0, _refreshInterval);
        }

        //展示动画
        private void OnShowFinalStageRewardAction()
        {
            if (_finalStageRewardDataModelList == null || _finalStageRewardDataModelList.Count <= 0
                                                       || finalStageCardItemList == null ||
                                                       finalStageCardItemList.Count <= 0)
            {
                CancelFinalStageRewardAction();
                return;
            }

            if (_rewardIndex >= _finalStageRewardDataModelList.Count
                || _rewardIndex >= finalStageCardItemList.Count)
            {
                CancelFinalStageRewardAction();
                return;
            }

            var stageRewardDataModel = _finalStageRewardDataModelList[_rewardIndex];
            var stageRewardItem = finalStageCardItemList[_rewardIndex];
            if (stageRewardDataModel != null && stageRewardItem != null)
            {
                stageRewardItem.Init(stageRewardDataModel);
            }

            _rewardIndex += 1;
        }

        //翻牌结束
        private void CancelFinalStageRewardAction()
        {
            CancelInvoke("OnShowFinalStageRewardAction");

            //展示关闭按钮
            SetCloseButtonAction();
            //隐藏未翻的牌面
            DisappearFinalStageUnRewardCardList();
        }

        //没有展示的牌，直接消失
        private void DisappearFinalStageUnRewardCardList()
        {
            for (var i = 0; i < finalStageCardItemList.Count; i++)
            {
                var cardItem = finalStageCardItemList[i];
                if (cardItem != null)
                {
                    //没有设置数据，直接重置
                    var stageRewardDataModel = cardItem.GetFightStageRewardDataModel();
                    if (stageRewardDataModel == null)
                    {
                        CommonUtility.UpdateGameObjectVisible(cardItem.gameObject, false);
                    }
                }
            }
        }
        #endregion

        #region CloseFrame

        //用于关闭倒计时
        private void SetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.EndTime =
                TimeManager.GetInstance().GetServerTime() + _closeFrameIntervalTime;
            closeCountDownTimeController.InitCountDownTimeController();
        }

        private void ResetCloseFrameCountDownController()
        {
            if (closeCountDownTimeController == null)
                return;

            closeCountDownTimeController.ResetCountDownTimeController();
        }

        //一段时间之后，展示关闭按钮
        private void SetCloseButtonAction()
        {
            _curShowCloseButtonLeftTime = showCloseButtonIntervalTime;
        }

        //显示结束按钮
        private void ShowCloseButton()
        {
            CommonUtility.UpdateButtonVisible(closeButton, true);
        }

        //关闭
        private void OnCloseFrame()
        {

            ResetCloseFrameCountDownController();
            CancelFinalStageRewardAction();

            TeamDuplicationUtility.OnCloseTeamDuplicationFinalStageCardFrame();

            //第二阶段奖励流程结束
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationFightStageEndShowFinishMessage);
        }
        #endregion 

    }
}
