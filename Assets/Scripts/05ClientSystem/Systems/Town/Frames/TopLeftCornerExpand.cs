using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    // 左上角扩展
    internal class TopLeftCornerExpand : MonoBehaviour
    {
        [SerializeField]
        Button DailyTodo = null;

        [SerializeField]
        Button oppo = null;

        [SerializeField]
        Button oppo2 = null;

        [SerializeField]
        Button Questionnaire = null;

        [SerializeField]
        Button OnlineService = null;

        [SerializeField]
        GameObject mDailyTodoRoot = null;

        [SerializeField]
        GameObject mOppoRoot = null;

        [SerializeField]
        GameObject mOppo2Root = null;

        [SerializeField]
        Image mOppoImg = null;

        [SerializeField]
        Text mOppoText = null;

        [SerializeField]
        Image mVivoImg = null;

        [SerializeField]
        Text mVivoText = null;

        [SerializeField]
        GameObject mOPPORepoint = null;

        [SerializeField]
        GameObject onlineServiceNote = null;

        [SerializeField]
        GameObject mOnLineTips = null;

        [SerializeField]
        ComSdkChannelIcon mOppo2sdk = null;

        [SerializeField]
        private Button mOperateAdsButton = null;

        [SerializeField]
        private Text mOperateAdsText = null;

        [SerializeField]
        private GameObject mOperateAdsRoot = null;

        public const int oppoPrivilegeID = 12000;
        public const int vivoPrivilegeID = 23000;

        private void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecOnlineServiceNewNote, OnRecOnlineServiceNewNote);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;
        }

        private void Start()
        {
            DailyTodo.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<DailyTodoFrame>(FrameLayer.Middle);
            });

            oppo.SafeSetOnClickListener(() => 
            {
                if (SDKInterface.Instance.IsOppoPlatform())
                {
                    ClientSystemManager.instance.OpenFrame<OPPOPrivilegeFrame>(FrameLayer.Middle);
                    GameStatisticManager.GetInstance().DoStartUIButton("OPPOPrivilege");
                }
                else if (SDKInterface.Instance.IsVivoPlatForm())
                {
                    ClientSystemManager.instance.OpenFrame<VIVOPrivilegeFrame>(FrameLayer.Middle);
                    GameStatisticManager.GetInstance().DoStartUIButton("VIVOPrivilege");
                }
            });

            oppo2.SafeSetOnClickListener(() => 
            {
                SDKInterface.Instance.GotoSDKChannelCommunity();
            });

            Questionnaire.SafeSetOnClickListener(() => 
            {
                if (BaseWebViewManager.GetInstance().CanOpenQuestionnaire())
                {
                    BaseWebViewParams param = new BaseWebViewParams();
                    param.fullUrl = BaseWebViewManager.GetInstance().GetQuestionnaireUrl();
                    param.type = BaseWebViewType.Questionnaire;
                    ClientSystemManager.GetInstance().OpenFrame<BaseWebViewFrame>(FrameLayer.TopMoreMost, param);
                }
            });

            OnlineService.SafeSetOnClickListener(() => 
            {
                OnlineServiceManager.GetInstance().TryReqOnlineServiceSign();
                GameStatisticManager.GetInstance().DoStartUIButton("OnLineService");
            });

            mOperateAdsButton.SafeSetOnClickListener(() => 
            {
                string operateFrameName = "";
                if (mOperateAdsText)
                {
                    operateFrameName = mOperateAdsText.text;
                }
                ClientSystemManager.instance.OpenFrame<OperateAdsBoardFrame>(FrameLayer.TopMoreMost, ClientApplication.operateAdsServer, operateFrameName);
                GameStatisticManager.GetInstance().DoStartUIButton("OperateAds");
            });

            _InitDailyTodoRoot();
            UpdateShowOppoLoginButton();
            InitQuestionnaireBtn();
            InitOnlineServiceBtn();
            _InitSDKIcon();
            InitShowOperateAdsButton();
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecOnlineServiceNewNote, OnRecOnlineServiceNewNote);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged, OnLevelChanged);
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {         
            UpdateShowOppoLoginButton();          
        }

        void InitShowOperateAdsButton()
        {
            if (mOperateAdsText)
            {
                mOperateAdsText.text = PluginManager.GetInstance().IsMGSDKChannel() ? TR.Value("operateAds_website") : TR.Value("operateAds_community");
            }
            UpdateShowOperateAdsButton();
        }

        void UpdateShowOperateAdsButton()
        {
            if (mOperateAdsRoot)
            {
                // mOperateAdsRoot.CustomActive(IsOperateAdsBtnShow());
                mOperateAdsRoot.CustomActive(false);
            }
        }

        bool IsOperateAdsBtnShow()
        {
#if APPLE_STORE
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.GAME_SERVICE))
            {
                return false;
            }
#endif

            bool isSDKEnable = PluginManager.instance.IsSDKEnableSystemVersion(SDKInterface.FuncSDKType.FSDK_UniWebView);
            if (!isSDKEnable)
            {
                return false;
            }
            if (string.IsNullOrEmpty(ClientApplication.operateAdsServer))
            {
                return false;
            }
            string unlockLevel = TR.Value("operateAds_unlock_level");
            int tarLevel = 10;
            if (int.TryParse(unlockLevel, out tarLevel))
            {
                if (tarLevel > PlayerBaseData.GetInstance().Level)
                {
                    return false;
                }
            }
            return true;
        }

        void _InitSDKIcon()
        {
            if (mOppo2sdk != null)
            {
                mOppo2sdk.UpdateShow();
            }
        }

        void _InitDailyTodoRoot()
        {
            if (mDailyTodoRoot)
            {
                mDailyTodoRoot.CustomActive(DailyTodoDataManager.GetInstance().BFuncOpen);
            }
        }

        bool _IsOppoLogin()
        {
            if (!SDKInterface.Instance.IsOppoPlatform() && !SDKInterface.Instance.IsVivoPlatForm())
            {
                return false;
            }

            if (!OPPOPrivilegeDataManager.GetInstance()._ActiveIsOpen())
            {
                return false;
            }

            if (SDKInterface.Instance.IsOppoPlatform())
            {
                GameStatisticManager.GetInstance().DoStartOPPO(StartOPPOType.OPPOICON);
            }
            else if (SDKInterface.Instance.IsVivoPlatForm())
            {
                GameStatisticManager.GetInstance().DoStartVIVO(StartVIVOType.VIVOICON);
            }


            return true;
        }

        void UpdateShowOppoLoginButton()
        {
            bool bIsFlag = _IsOppoLogin();

            mOppoRoot.CustomActive(bIsFlag);
            mOppo2Root.CustomActive(false);

            if (SDKInterface.Instance.IsOppoPlatform())
            {
                mOppoImg.CustomActive(true);
                mOppoText.CustomActive(true);
                mVivoImg.CustomActive(false);
                mVivoText.CustomActive(false);
                if (mOPPORepoint != null)
                {
                    bool bIsShow = (OPPOPrivilegeDataManager.GetInstance()._CheckDail() ||
                        OPPOPrivilegeDataManager.GetInstance()._CheckPrivilrge(oppoPrivilegeID) ||
                        OPPOPrivilegeDataManager.GetInstance()._CheckLuckyGuy() ||
                        OPPOPrivilegeDataManager.GetInstance()._CheckAmberGiftBag() ||
                        OPPOPrivilegeDataManager.GetInstance()._CheckAmberPrivilege() ||
                        OPPOPrivilegeDataManager.GetInstance()._CheckOPPOGrowthHaoLi());
                    mOPPORepoint.CustomActive(bIsShow);
                }

                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_OPPO_COMMUNITY) == false)
                {
                    mOppo2Root.CustomActive(true);
                }
            }
            else if (SDKInterface.Instance.IsVivoPlatForm())
            {
                mOppoImg.CustomActive(false);
                mOppoText.CustomActive(false);
                mVivoImg.CustomActive(true);
                mVivoText.CustomActive(true);
                if (mOPPORepoint != null)
                {
                    mOPPORepoint.CustomActive(OPPOPrivilegeDataManager.GetInstance()._CheckPrivilrge(vivoPrivilegeID));
                }

                if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_VIVO_COMMUNITY) == false)
                {
                    mOppo2Root.CustomActive(true);
                }
            }

        }

        private void InitQuestionnaireBtn()
        {
            bool isshow = false;
            if (BaseWebViewManager.GetInstance().CanUnlockQuestionnaire())
            {
                isshow = true;
            }
            else
            {
                isshow = false;
            }
            Questionnaire.gameObject.CustomActive(isshow);
        }

        void InitOnlineServiceBtn()
        {

#if APPLE_STORE
            //IOS屏蔽功能 
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_ACTIVITY))
            {
                onlineServiceBtn.gameObject.CustomActive(false);
                return;
            }
#endif

            bool isShow = false;
            //add api check
            if (!PluginManager.instance.IsSDKEnableSystemVersion(SDKInterface.FuncSDKType.FSDK_UniWebView))
            {
                isShow = false;
                if (onlineServiceNote)
                {
                    onlineServiceNote.CustomActive(false);
                }

                if (mOnLineTips)
                    mOnLineTips.CustomActive(false);
            }
            else
            {
                //OnlineServiceManager.GetInstance().ReqOfflineInfoSign();
                isShow = OnlineServiceManager.GetInstance().TryReqOnlineServiceOpen();
            }
            if (OnlineService)
            {
                OnlineService.gameObject.CustomActive(isShow);
            }
        }

        void MakeShowOnlineService(UIEvent mEvent)
        {
            if (OnlineService)
            {
                bool isBtnShow = OnlineServiceManager.GetInstance().IsOnlineServiceOpen;

                OnlineService.gameObject.CustomActive(isBtnShow);
                if (onlineServiceNote)
                    onlineServiceNote.CustomActive(false);

                if (mOnLineTips)
                    mOnLineTips.CustomActive(false);

                if (isBtnShow == false)
                {
                    if (ClientSystemManager.GetInstance().IsFrameOpen<OnlineServiceFrame>())
                    {
                        ClientSystemManager.GetInstance().CloseFrame<OnlineServiceFrame>();
                    }
                }
            }
        }

        void OnRecOnlineServiceNewNote(UIEvent mEvent)
        {
            bool isShow = (bool)mEvent.Param1;
            if (onlineServiceNote)
                onlineServiceNote.CustomActive(isShow);

            if (mOnLineTips)
                mOnLineTips.CustomActive(isShow);
        }

        void OnLevelChanged(UIEvent uiEvent)
        {
            UpdateShowOperateAdsButton();
        }
    }
}
