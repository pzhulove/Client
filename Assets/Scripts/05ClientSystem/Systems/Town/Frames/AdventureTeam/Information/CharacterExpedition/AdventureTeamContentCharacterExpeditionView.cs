using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Scripts.UI;
using Protocol;
using System;

namespace GameClient
{
    public class AdventureTeamContentCharacterExpeditionView : AdventureTeamContentBaseView
    {
        [SerializeField] private Button mExpeditionButton;
        [SerializeField] private ComUIListScript mTimeToggleRoot;
        [SerializeField] private Image mMapBackground;
        [SerializeField] private Text mMapName;
        [SerializeField] private Text mMapLevelLimit;
        [SerializeField] private ComUIListScript mMiniMapToggleRoot;
        [SerializeField] private ComUIListScript mRewardScrollList;
        //[SerializeField] private Slider mTimeSlider;
        [SerializeField] private GameObject mRoleUnitRoot;
        [SerializeField] private Text mExpeditionButtonText;
        [SerializeField] private ComUIListScript mCharacterSelectScrollList;
        [SerializeField] private GameObject mExpeditionTimer;
        [SerializeField] private ToggleGroup mTimeToggleGroup;
        [SerializeField] private ToggleGroup mMiniMapToggleGroup;

        
        [SerializeField] private CommonFrameButtonBuryPoint mBuryPoint;


        [SerializeField] private string normalBtnImgResPath = "UI/Image/NewPacked/Common.png:Common_Btn_03";
        [SerializeField] private string enableBtnImgResPath = "UI/Image/NewPacked/Common.png:Common_Btn_10";

        private string thisButtonTypeName = "";

        private string tr_expedition_btn_dispatch = "";
        private string tr_expedition_btn_cancel = "";
        private string tr_expedition_btn_get_reward = "";
        private string tr_expedition_map_role_level_limit = "";
        private string tr_expedition_timer_finish = "";
        private string tr_expedition_timer_tips = "";
        private string tr_expedition_dispatch_tips = "";
        private string tr_expeidtion_cancel_tips = "";
        private string tr_expedition_no_roles_tips = "";

        private Protocol.ExpeditionMapBaseInfo[] mExpeditionMiniMaps;

        private List<byte> mExpeditionTimeList = new List<byte>();
        private bool isChangeMapid = false;
        private int rewardCount = 0;

        private ExpeditionMemberInfo tempMemBer;

        //远征结果界面是否打开过
        private bool hasTriedShowExpeditionResultFrame;
        //远征一键界面是否打开过
        private bool hasTriedShowExpeditionOnekeyFrame;
        private byte lastRewardReqExpeditionMapId;

        private void Awake()
        {
            _InitTR();
            BindEvents();
            _bindEvents();
            _InitTimeToggleScrollListBind();
            _InitMiniMapToggleScrollListBind();
            _InitRewardScrollListBind();
            _InitCharacterSelectScrollListBind();
        }

        private void OnDestroy()
        {
            _ClearTR();
            _unBindEvents();
            UnBindEvents();
            mExpeditionMiniMaps = null;
            mExpeditionTimeList = null;
            mMiniMapToggleRoot.UnInitialize();
            mTimeToggleRoot.UnInitialize();
            mCharacterSelectScrollList.UnInitialize();
            mRewardScrollList.UnInitialize();
            rewardCount = 0;
            ResetMapId();
            thisButtonTypeName = "";

            tempMemBer = null;

            hasTriedShowExpeditionResultFrame = false;
            hasTriedShowExpeditionOnekeyFrame = false;
            lastRewardReqExpeditionMapId = 0;
        }

        private void BindEvents()
        {

        }

        private void UnBindEvents()
        {
            mExpeditionButton.onClick.RemoveAllListeners();
        }

        private void _InitTR()
        {
            tr_expedition_btn_dispatch = TR.Value("adventure_team_expedition_dispatch");
            tr_expedition_btn_cancel = TR.Value("adventure_team_expedition_cancel");
            tr_expedition_btn_get_reward = TR.Value("adventure_team_expedition_get_reward");
            tr_expedition_map_role_level_limit = TR.Value("adventure_team_expedition_map_role_level_limit");
            tr_expedition_timer_finish = TR.Value("adventure_team_expedition_timer_finish");
            tr_expedition_timer_tips = TR.Value("adventure_team_expedition_timer_tips");
            tr_expedition_dispatch_tips = TR.Value("adventure_team_expeidtion_dispatch_tips");
            tr_expeidtion_cancel_tips = TR.Value("adventure_team_expeidtion_cancel_tips");
            tr_expedition_no_roles_tips = TR.Value("adventure_team_expedition_no_roles_tips");
        }

        private void _ClearTR()
        {
            tr_expedition_btn_dispatch = "";
            tr_expedition_btn_cancel = "";
            tr_expedition_btn_get_reward = "";
            tr_expedition_map_role_level_limit = "";
            tr_expedition_timer_finish = "";
            tr_expedition_timer_tips = "";
            tr_expedition_dispatch_tips = "";
            tr_expeidtion_cancel_tips = "";
            tr_expedition_no_roles_tips = "";
        }

        private void _bindEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionMapInfoChanged, _MapNetInfoChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionRolesChanged, _OnCharacterChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionMiniMapChanged, _ExpeditionMiniMapInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionRolesSelected, _OnExpeditionCharacterSelectFrameChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionDispatch, _OnExpeditionDispatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeddtionCancel, _OnExpeditionCancel);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionGetReward, _OnExpeditionGetReward);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionTimeChanged, _OnExpeditionTimeChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionIDChanged, _OnExpeditionMapIdChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionTimerFinish, _OnGetExpeditionFinishMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionResultFrameClose, _OnAdventureTeamExpeditionResultFrameClose);
        }

        private void _unBindEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionMapInfoChanged, _MapNetInfoChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionRolesChanged, _OnCharacterChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionMiniMapChanged, _ExpeditionMiniMapInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionRolesSelected, _OnExpeditionCharacterSelectFrameChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionDispatch, _OnExpeditionDispatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeddtionCancel, _OnExpeditionCancel);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionGetReward, _OnExpeditionGetReward);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionTimeChanged, _OnExpeditionTimeChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionIDChanged, _OnExpeditionMapIdChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionTimerFinish, _OnGetExpeditionFinishMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamExpeditionResultFrameClose, _OnAdventureTeamExpeditionResultFrameClose);
        }

        public override void InitData()
        {
            hasTriedShowExpeditionResultFrame = false;
            hasTriedShowExpeditionOnekeyFrame = false;
            _UpdateExpeditionAllMapsInfo();
        }

        public override void OnEnableView()
        {
            hasTriedShowExpeditionResultFrame = false;
            hasTriedShowExpeditionOnekeyFrame = false;
            _UpdateExpeditionAllMapsInfo();
        }

        #region UIEvent
        //void _UpdateMiniMapInfo(byte mapId, ExpeditionStatus mapState,ComCommonBind mBind)
        //{
        //    var description = mBind.GetCom<Text>("Desc");
        //    var background = mBind.GetCom<Image>("mMiniMapImage");
        //    //读表刷新数据，确定状态及解锁情况
        //}

        void ResetMapId()
        {
            byte[] totalMapsId = AdventureTeamDataManager.GetInstance().GetAllExpeditionMapsId();
            if (totalMapsId.Length > 0)
            {
                AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId = totalMapsId[0];
            }
        }

        void _UpdateTimerFinish()
        {
            Text timerText = mExpeditionTimer.GetComponent<Text>();
            if (timerText)
            {
                timerText.text = tr_expedition_timer_finish;
            }
        }

        void _UpdateMapInfo(byte index)
        {
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey(index))
            {
                ExpeditionMapModel tempModel = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic[index];
                if(null != mMapName)
                {
                    mMapName.text = tempModel.mapName;
                }
                if (null != mMapLevelLimit)
                {
                    mMapLevelLimit.text = string.Format(tr_expedition_map_role_level_limit, tempModel.playerLevelLimit);
                }
                if (mMapBackground && !string.IsNullOrEmpty(tempModel.mapImagePath)) 
                {
                    ETCImageLoader.LoadSprite(ref mMapBackground, tempModel.mapImagePath);
                }
            }
        }

        void _UpdateExpeditionBtnEvent(ExpeditionStatus tempStatus)
        {
            if (mExpeditionButton == null) return;
            mExpeditionButton.enabled = true;
            Image buttonColor = mExpeditionButton.GetComponent<Image>();
            if (buttonColor == null) return; 
            mExpeditionButton.onClick.RemoveAllListeners();
            switch (tempStatus)
            {
                case ExpeditionStatus.EXPEDITION_STATUS_PREPARE:
                    mExpeditionButton.onClick.AddListener(_TryReqExpeditionStart);
                    mExpeditionButtonText.text = tr_expedition_btn_dispatch;
                    ETCImageLoader.LoadSprite(ref buttonColor, normalBtnImgResPath);
                    thisButtonTypeName = "BtnGoExpedition";
                    break;
                case ExpeditionStatus.EXPEDITION_STATUS_IN:
                    mExpeditionButton.onClick.AddListener(_TryReqExpeditionCancel);
                    mExpeditionButtonText.text = tr_expedition_btn_cancel;
                    ETCImageLoader.LoadSprite(ref buttonColor, normalBtnImgResPath);
                    thisButtonTypeName = "BtnCancelExpedtion";
                    break;
                case ExpeditionStatus.EXPEDITION_STATUS_OVER:
                    mExpeditionButton.onClick.AddListener(_TryReqExpeditionFinish);
                    mExpeditionButtonText.text = tr_expedition_btn_get_reward;
                    ETCImageLoader.LoadSprite(ref buttonColor, enableBtnImgResPath);
                    thisButtonTypeName = "BtnGetRewardExpedition";
                    break;
                default:
                    break;
            }
        }
        void _StartButtonCD()
        {
            if (mExpeditionButton)
            {
                SetComButtonCD cd = mExpeditionButton.gameObject.GetComponent<SetComButtonCD>();
                if (cd)
                {
                    cd.StartBtCD();
                }
            }
        }

        void _UpdateExpeditionAllMapsInfo()
        {                        
            _TryReqAllExpeditionMapInfo();
            _TryReqGetAllExpeditionMaps();
        }

        void _UpdateExpeditionCurrentMapInfo()
        {            
            _TryReqExpeditionMapInfo(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId);
            _TryReqGetAllExpeditionMaps();
        }

        void _UpdateExpeditionCurrentMapInfo(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
            {
                return;
            }
            var netMapModel = uiEvent.Param1 as ExpeditionMapNetInfo;
            if (netMapModel != null &&
                AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo != null &&
                netMapModel.mapId == AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId)
            {
                _TryReqExpeditionMapInfo(netMapModel.mapId);
            }
            _TryReqGetAllExpeditionMaps();            
        }


        //void _InitTimeSlideUIEvent()
        //{
        //    ExpeditionMapModel tempMapBaseDate = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic[AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
        //    mTimeSlider.wholeNumbers = true;
        //    mTimeSlider.minValue = 0;
        //    mTimeSlider.maxValue = tempMapBaseDate.expeditionTime.Split('|').Length - 1;
        //    mTimeSlider.onValueChanged.AddListener(_OnTimeSliderValueChange);
        //}

        //void _OnTimeSliderValueChange(float value)
        //{
        //    ExpeditionMapModel tempMapBaseDate = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic[AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
        //    int time = 1;
        //    int.TryParse(tempMapBaseDate.expeditionTime.Split('|')[(int)value], out time);
        //    //更改显示的时间
        //    AdventureTeamDataManager.GetInstance().SetEpxeditionTime(time);

        //    mRewardScrollList.SetElementAmount(tempMapBaseDate.rewardList.Count);
        //    AdventureTeamDataManager.GetInstance().IsChangeExpeditionTime = false;
        //}
        #endregion

        #region UIListElement
        void _InitTimeToggleScrollListBind()
        {
            mTimeToggleRoot.Initialize();

            mTimeToggleRoot.onItemVisiable = (item) =>
            {
                if (mTimeToggleGroup != null && item != null && item.m_index >= 0) 
                {
                    mTimeToggleGroup.allowSwitchOff = true;
                    _UpdateTimeToggleScrollListBind(item);
                    mTimeToggleGroup.allowSwitchOff = false;
                }

            };
            mTimeToggleRoot.OnItemRecycle = (item) =>
            {
                if (mTimeToggleGroup != null && item != null)
                {
                    mTimeToggleGroup.allowSwitchOff = true;
                    AdventureTeamExpeditionTimeToggle mItem = item.GetComponent<AdventureTeamExpeditionTimeToggle>();
                    if (null == mItem)
                    {
                        return;
                    }
                    mItem.OnItemRecycle();
                    mTimeToggleGroup.allowSwitchOff = false;
                }
            };
        }

        void _UpdateTimeToggleScrollListBind(ComUIListElementScript item)
        {
            if (item == null) return;
            if (!AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey((int)AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId)) return;
            ExpeditionMapModel tempMap = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic
                [(int)AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
            AdventureTeamExpeditionTimeToggle mItem = item.GetComponent<AdventureTeamExpeditionTimeToggle>();
            if (null == mItem)
            {
                return;
            }
            if (item.m_index < 0 || item.m_index >= mExpeditionTimeList.Count)
            {
                return;
            }
            if(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapStatus == ExpeditionStatus.EXPEDITION_STATUS_PREPARE)
            {
                mItem.InitItemView(mExpeditionTimeList[item.m_index], true);
            }
            else
            {
                mItem.InitItemView(mExpeditionTimeList[item.m_index], false);
            }

            if (mExpeditionTimeList[item.m_index] == AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.durationOfExpedition)  
            {
                mItem.ChangeToggleState(true);
            }
            else
            {
                mItem.ChangeToggleState(false);
            }
            mItem.UpdateItemInfo();
        }

        void _InitMiniMapToggleScrollListBind()
        {
            mMiniMapToggleRoot.Initialize();

            mMiniMapToggleRoot.onItemVisiable = (item) =>
            {
                if (mMiniMapToggleGroup != null && item != null & item.m_index >= 0) 
                {
                    mMiniMapToggleGroup.allowSwitchOff = true;
                    _UpdateMiniMapToggleScrollListBind(item);
                    mMiniMapToggleGroup.allowSwitchOff = false;
                }

            };
            mMiniMapToggleRoot.OnItemRecycle = (item) =>
            {
                if (mMiniMapToggleGroup != null && item != null)
                {
                    mMiniMapToggleGroup.allowSwitchOff = true;
                    AdventureTeamExpeditionMinimapToggle mItem = item.GetComponent<AdventureTeamExpeditionMinimapToggle>();
                    if (null == mItem)
                    {
                        return;
                    }
                    mItem.OnItemRecycle();
                    mMiniMapToggleGroup.allowSwitchOff = false;
                }
            };
        }

        void _UpdateMiniMapToggleScrollListBind(ComUIListElementScript item)
        {
            AdventureTeamExpeditionMinimapToggle mItem = item.GetComponent<AdventureTeamExpeditionMinimapToggle>();
            if (null == mItem)
            {
                return;
            }
            byte[] totalMapsId = AdventureTeamDataManager.GetInstance().GetAllExpeditionMapsId();
            if (item.m_index < 0 || item.m_index >= totalMapsId.Length)
            {
                return;
            }

            ExpeditionMapModel tempModel = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic[totalMapsId[item.m_index]];
            byte mapId = totalMapsId[item.m_index];
            ExpeditionStatus mapState = (ExpeditionStatus)mExpeditionMiniMaps[item.m_index].expeditionStatus;

            mItem.InitItemView(mExpeditionMiniMaps[item.m_index], mapId);

            if (mapId == AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId) 
            {
                mItem.ChangeToggleState(true);
            }
            mItem.UpdateItemInfo();
        }

        /// <summary>
        /// 奖励界面刷新
        /// </summary>
        void _InitRewardScrollListBind()
        {
            mRewardScrollList.Initialize();

            mRewardScrollList.onItemVisiable = (item) =>
            {
                if (item != null && item.m_index >= 0) 
                {
                    _UpdateRewardScrollListBind(item);
                }
            };
            mRewardScrollList.OnItemRecycle = (item) =>
            {
                if (item == null) return;
                AdventureTeamExpeditionRewardItem mItem = item.GetComponent<AdventureTeamExpeditionRewardItem>();
                if (mItem == null) return;
                mItem.OnItemRecycle();
            };
        }

        void _UpdateRewardScrollListBind(ComUIListElementScript item)
        {
            if (item == null) return;
            AdventureTeamExpeditionRewardItem mItem = item.GetComponent<AdventureTeamExpeditionRewardItem>();
            if (mItem == null) return;
            if (!AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.
                ContainsKey(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId)) return;

            ExpeditionMapModel tempInfo = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic
                 [AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
            if (item.m_index < 0 || item.m_index >= tempInfo.rewardList.Count)
            {
                return;
            }
            ExpeditionRewardCondition tempCondition = tempInfo.rewardList[item.m_index].rewardCondition;
            mItem.InitItemView(item.m_index, tempInfo, tempCondition);
            if (isChangeMapid)
            {
                mItem.UpdateExpeditionMapBaseDate();
            }
            if (AdventureTeamDataManager.GetInstance().IsChangeExpeditionTime 
                && AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.durationOfExpedition != 0
                || AdventureTeamDataManager.GetInstance().IsChangedExpeditionMap)
            {
                mItem.OnExpeditionTimeChanged();
            }
            if (AdventureTeamDataManager.GetInstance().IsChangedExpedtionRoles ) 
            {
                mItem.OnExpeditionRolesChanged();
            }
            if (mItem.IsReach() && (AdventureTeamDataManager.GetInstance().IsChangedExpedtionRoles || isChangeMapid || AdventureTeamDataManager.GetInstance().IsChangeExpeditionTime))  
            {
                rewardCount++;
            }
        }

        void _InitCharacterSelectScrollListBind()
        {
            mCharacterSelectScrollList.Initialize();

            mCharacterSelectScrollList.onItemVisiable = (item) =>
            {
                if (item != null && item.m_index >= 0) 
                {
                    _UpdateCharacterSelectScrollListBind(item);
                }

            };
            mCharacterSelectScrollList.OnItemRecycle = (item) =>
            {
                if(item == null)
                {
                    return;
                }
                AdventureTeamExpeditionRoleSelectItem mItemScript = item.GetComponent<AdventureTeamExpeditionRoleSelectItem>();
                if (null == mItemScript)
                {
                    return;
                }
                mItemScript.OnItemRecycle();
            };
        }

        void _UpdateCharacterSelectScrollListBind(ComUIListElementScript item)
        {
            if (item == null) return;
            AdventureTeamExpeditionRoleSelectItem mItemScript = item.GetComponent<AdventureTeamExpeditionRoleSelectItem>();
            if (null == mItemScript)
            {
                return;
            }
            if (!AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.
                ContainsKey(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId)) return;

            ExpeditionMapModel tempInfo = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic
                 [AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
            if (item.m_index < 0 || item.m_index >= tempInfo.rolesCapacity) 
            {
                return;
            }
            
            if (item.m_index< AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.roles.Count)
            {
                tempMemBer = AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.roles[item.m_index];
            }
            else
            {
                tempMemBer = null;
            }
            mItemScript.InitItemView(item.m_index, tempMemBer);
        }

        #endregion

        #region Protocol Request
        /// <summary>
        /// TODO 远征界面地图信息 远征状态 (派遣远征队信息) 远征截止信息
        ///// </summary>
        private void _TryReqExpeditionMapInfo(byte id)
        {
            AdventureTeamDataManager.GetInstance().ReqExpeditionMapInfo(id);
        }

        private void _TryReqAllExpeditionMapInfo()
        {
            AdventureTeamDataManager.GetInstance().ReqExpeditionAllMapInfo();
        }

        /// <summary>
        /// TODO 远征界面角色信息 精力 派遣状态 
        /// </summary>
        private void _TryReqExpeditionTeamCharacterInfo()
        {
            AdventureTeamDataManager.GetInstance().ReqExpeditionRolesInfo();
        }

        /// <summary>
        /// TODO 派遣 角色 地图 开始远征
        /// </summary>
        private void _TryReqExpeditionStart()
        {
            int count = AdventureTeamDataManager.GetInstance().ExpeditionRoleListCount();
            if(count != 0)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(
                string.Format(tr_expedition_dispatch_tips, rewardCount, AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.durationOfExpedition)
                , ()=>{
                    AdventureTeamDataManager.GetInstance().ReqDispatchExpeditionTeam();
                    _SetBuryPoint();
                });
            }
            else
            {
                //SystemNotifyManager.SysNotifyTextAnimation(tr_expedition_no_roles_tips);
                AdventureTeamDataManager.GetInstance().TryOpenExpeditionRoleSelectFrame(tempMemBer);
            }
            _StartButtonCD();
        }

        /// <summary>
        /// TODO 取消 地图 远征
        /// </summary>
        private void _TryReqExpeditionCancel()
        {
            Text timeText = mExpeditionTimer.GetComponent<Text>();
            string time = "";
            if (timeText)
            {
                time = timeText.text.Replace(tr_expedition_timer_tips + "\n", "");
            }
            string notify = string.Format(tr_expeidtion_cancel_tips, time);
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(notify,
             ()=>{
                AdventureTeamDataManager.GetInstance().ReqCancelExpeditionTeam();
                _SetBuryPoint();
            });
            _StartButtonCD();
        }

        /// <summary>
        /// TODO 完成 地图 远征
        /// </summary>
        private void _TryReqExpeditionFinish()
        {
            AdventureTeamDataManager.GetInstance().ReqGetExpeditionRewards();
            _StartButtonCD();

            _SetBuryPoint();
        }

        private void _TryReqGetAllExpeditionMaps()
        {
            AdventureTeamDataManager.GetInstance().ReqGetAllExpeditionMaps();
        }
        #endregion

        private void _MapNetInfoChanged(UIEvent ui)
        {
            if(AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey((int)AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId))
            {
                ExpeditionMapModel tempMap = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic
                [AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
                if(mRewardScrollList != null)
                {
                    rewardCount = 0;
                    mRewardScrollList.SetElementAmount(tempMap.rewardList.Count);
                }
                if (mCharacterSelectScrollList != null) 
                {
                    mCharacterSelectScrollList.SetElementAmount(tempMap.rolesCapacity);
                }
                //_InitTimeSlideUIEvent();

                AdventureTeamDataManager.GetInstance().IsChangedExpedtionRoles = false;

                if (mExpeditionTimeList != null)
                {
                    mExpeditionTimeList.Clear();
                    string[] times = tempMap.expeditionTime.Split('|');
                    for (int i = 0; i < times.Length; i++)
                    {
                        int temp = 1;
                        if (int.TryParse(times[i], out temp))
                        {
                        mExpeditionTimeList.Add(BitConverter.GetBytes(temp)[0]);
                        }
                    }
                    if(mTimeToggleRoot != null)
                    {
                        mTimeToggleRoot.SetElementAmount(mExpeditionTimeList.Count);
                    }
                }
                if(mExpeditionTimer != null)
                {
                    AdventureTeamExpeditionTimer timer = mExpeditionTimer.GetComponent<AdventureTeamExpeditionTimer>();
                    switch (AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapStatus)
                    {
                        case ExpeditionStatus.EXPEDITION_STATUS_PREPARE:
                            mExpeditionTimer.SetActive(false);
                            break;
                        case ExpeditionStatus.EXPEDITION_STATUS_IN:
                            mExpeditionTimer.SetActive(true);
                            if (timer)
                            {
                                uint now = TimeManager.GetInstance().GetServerTime();
                                double second = AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.endTimeOfExpedition - now;

                                timer.SetCountdown((int)second);
                                timer.StartTimer();
                            }
                            break;
                        case ExpeditionStatus.EXPEDITION_STATUS_OVER:
                            mExpeditionTimer.SetActive(true);
                            if(timer)
                            {
                                timer.StopTimer();
                            }
                            _UpdateTimerFinish();
                            break;
                        default:
                            break;
                    }
                }
            }

            //刷新奖励状态


            ////if (AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapStatus != ExpeditionStatus.EXPEDITION_STATUS_PREPARE)
            ////{
            ////    mTimeToggleRoot.SetElementAmount(1);
            ////}
            ////else
            ////{
            ////    string[] expeditionTime = tempMap.expeditionTime.Split('|');
            ////    mTimeToggleRoot.SetElementAmount(expeditionTime.Length);
            ////}
            //_InitTimeSlideUIEvent();
        }

        void _OnCharacterChanged(UIEvent ui)
        {
            if (!hasTriedShowExpeditionOnekeyFrame)
            {
                var readyMapModels = AdventureTeamDataManager.GetInstance().GetReadyExpeditionMapModels();
                if (readyMapModels != null && readyMapModels.Count > 0)
                {
                    if (ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamExpeditionCharacterSelectFrame>())
                    {
                        ClientSystemManager.GetInstance().CloseFrame<AdventureTeamExpeditionCharacterSelectFrame>();
                    }
                    if (!ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamExpeditionOnekeyFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<AdventureTeamExpeditionOnekeyFrame>(FrameLayer.Middle, readyMapModels);
                    }
                }
                hasTriedShowExpeditionOnekeyFrame = true;
            }
            else
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamExpeditionCharacterSelectFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<AdventureTeamExpeditionCharacterSelectFrame>();
                }
            }
        }

        private void _ExpeditionMiniMapInfo(UIEvent ui)
        {
            mExpeditionMiniMaps = ui.Param1 as Protocol.ExpeditionMapBaseInfo[];
            if (mExpeditionMiniMaps != null)
            {
                if(mMiniMapToggleRoot != null)
                {
                    mMiniMapToggleRoot.SetElementAmount(AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.Count);
                }
                if (mExpeditionMiniMaps != null)
                {
                    for (int i = 0; i < mExpeditionMiniMaps.Length; i++)
                    {
                        if (mExpeditionMiniMaps[i].mapId == AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId)
                        {
                            _UpdateExpeditionBtnEvent((ExpeditionStatus)mExpeditionMiniMaps[i].expeditionStatus);
                        }

                        if (mExpeditionMiniMaps[i].mapId == lastRewardReqExpeditionMapId && !hasTriedShowExpeditionOnekeyFrame)
                        {
                            _TryReqExpeditionTeamCharacterInfo();
                            lastRewardReqExpeditionMapId = 0;
                        }
                    }
                }
            }

            if (!hasTriedShowExpeditionResultFrame)
            {
                var finishMapModels = AdventureTeamDataManager.GetInstance().GetFinishedExpeditionMapModels();
                if (finishMapModels != null && finishMapModels.Count > 0)
                {
                    if (!ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamExpeditionResultFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<AdventureTeamExpeditionResultFrame>(FrameLayer.Middle, finishMapModels);
                    }
                }
                else
                {
                    _TryReqExpeditionTeamCharacterInfo();
                }
                hasTriedShowExpeditionResultFrame = true;
            }
        }

        private void _OnExpeditionCharacterSelectFrameChanged(UIEvent ui)
        {
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId))
            {
                ExpeditionMapModel tempInfo = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic
                   [AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
                if (mRewardScrollList != null)
                {
                    rewardCount = 0;
                    mRewardScrollList.SetElementAmount(tempInfo.rewardList.Count);
                }
                
                if (mCharacterSelectScrollList != null)
                {
                    mCharacterSelectScrollList.SetElementAmount(tempInfo.rolesCapacity);
                }
                AdventureTeamDataManager.GetInstance().IsChangedExpedtionRoles = false;
            }
        }

        private void _OnExpeditionDispatch(UIEvent ui)
        {
            _UpdateExpeditionCurrentMapInfo(ui);
        }

        private void _OnExpeditionCancel(UIEvent ui)
        {
            _UpdateExpeditionCurrentMapInfo(ui);
        }

        private void _OnExpeditionGetReward(UIEvent ui)
        {
            _UpdateExpeditionCurrentMapInfo(ui);
        }

        private void _OnExpeditionTimeChanged(UIEvent ui)
        {
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId))
            {
                ExpeditionMapModel tempInfo = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic
                 [AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
                if (mRewardScrollList != null)
                {
                    rewardCount = 0;
                    mRewardScrollList.SetElementAmount(tempInfo.rewardList.Count);
                }
                AdventureTeamDataManager.GetInstance().IsChangeExpeditionTime = false;
            }
        }

        private void _OnExpeditionMapIdChange(UIEvent ui)
        {
            _UpdateMapInfo(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId);
            if (AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic.ContainsKey(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId))
            {
                ExpeditionMapModel tempInfo = AdventureTeamDataManager.GetInstance().ExpeditionMapBaseInfo.expeditionMapDic
                   [AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId];
                isChangeMapid = true;
                if (mRewardScrollList != null)
                {
                    rewardCount = 0;
                    mRewardScrollList.SetElementAmount(tempInfo.rewardList.Count);
                }
                isChangeMapid = false;
            }
            if (mExpeditionMiniMaps != null)
            {
                for (int i = 0; i < mExpeditionMiniMaps.Length; i++)
                {
                    if (mExpeditionMiniMaps[i].mapId == AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.mapId)
                    {
                        _UpdateExpeditionBtnEvent((ExpeditionStatus)mExpeditionMiniMaps[i].expeditionStatus);
                    }
                }
            }
        }

        private void _OnGetExpeditionFinishMessage(UIEvent ui)
        {
            _UpdateExpeditionCurrentMapInfo();
        }

        private void _OnAdventureTeamExpeditionResultFrameClose(UIEvent uiEvent)
        {
            if (uiEvent != null && uiEvent.Param1 != null)
            {
                //领取结算界面奖励时，如果有奖励可领取，则需要等待领取最后一张地图的奖励后才打开 一键远征界面！
                var netMapInfo = uiEvent.Param1 as ExpeditionMapNetInfo;
                if (netMapInfo != null)
                {
                    lastRewardReqExpeditionMapId = netMapInfo.mapId;
                }
            }
            else
            {
                _TryReqExpeditionTeamCharacterInfo();
            }
        }

        private void _SetBuryPoint()
        {
            if(mBuryPoint)
            {
                mBuryPoint.ButtonName = thisButtonTypeName;
                mBuryPoint.OnSendBuryingPoint();
            }
        }
    }
}