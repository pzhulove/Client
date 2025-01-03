using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using Protocol;
using Network;
using System;

namespace _Settings
{   
    public class AccountLockSettings : SettingsBindUI
    {
        #region inner type

        public enum ShowType
        {
            Opened,             // 安全锁已开启
            NotOpen,            // 安全锁未开启
            ForceUnLock,        // 强制解锁
        }

        #endregion

        #region ui bind
        
        private Button btnLock;
        private Button btnBind;
        private Button btnUnBind;
        private Button btnUnLock;
        private Button btnForceUnLock;
        private Button btnChangePwd;
        private Button btnCancelForceUnLock;
        private Text txtApply;
        private Text txtApplyEnd;

        private GameObject goNotOpen;
        private GameObject goOpened;
        private GameObject goForceLock;

        #endregion

        #region vals

        ShowType eShowType = ShowType.NotOpen;

        #endregion

        public AccountLockSettings(GameObject root, ClientFrame frame)
            : base(root, frame)
        {

        }

        #region override

        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/accountLock";
        }

        protected override void InitBind()
        {
            InitButtonBind(ref btnLock, "btnLock", () =>
            {
                SecurityLockData data = SecurityLockDataManager.GetInstance().GetSecurityLockData();

                if(Input.GetKeyDown(KeyCode.LeftControl))
                {
                    ClientSystemManager.GetInstance().OpenFrame<AccountLock>();
                }
                else
                {
                    if (data.isUseLock)
                    {
                        SecurityLockDataManager.GetInstance().SendWorldSecurityLockOpReq(LockOpType.LT_LOCK, "");
                    }
                    else
                    {
                        ClientSystemManager.GetInstance().OpenFrame<AccountLock>();
                    }
                }               
            });

            InitButtonBind(ref btnUnLock, "btnUnLock", () =>
            {
                ClientSystemManager.GetInstance().OpenFrame<AccountUnLock>();
            });

            InitButtonBind(ref btnBind, "btnBind", () =>
            {
                //                 SystemNotifyManager.SysNotifyMsgBoxOkCancel("是否选择当前设备为常用设备？一旦选用，则以后使用该设备登录账号，安全锁自动解锁，无需进行密码验证(每个账号最多绑定10台设备)",
                //                     () => 
                //                     {
                //                         SecurityLockDataManager.GetInstance().SendWorldBindDeviceReq(true);
                //                     });

                ClientSystemManager.GetInstance().OpenFrame<AccountLockBindDevice>();
            });

            InitButtonBind(ref btnUnBind, "btnUnBind", () =>
            {
                SecurityLockDataManager.GetInstance().SendWorldBindDeviceReq(false);
            });

            InitButtonBind(ref btnForceUnLock, "btnForceUnLock", () =>
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel("确认强制解锁后，账号会进入7*24小时的冻结期，冻结期结束后，安全锁密码将被删除并变为“未开启”状态。是否确认强制解锁？",
                    () => 
                    {
                        SecurityLockDataManager.GetInstance().SendWorldSecurityLockOpReq(LockOpType.LT_FORCE_UNLOCK, "");
                    });

                
            });

            InitButtonBind(ref btnChangePwd, "btnChangePwd", () =>
            {
                ClientSystemManager.GetInstance().OpenFrame<AccountLockChangePwd>();
            });

            InitButtonBind(ref btnCancelForceUnLock, "btnCancelForceUnLock", () =>
            {
                SecurityLockDataManager.GetInstance().SendWorldSecurityLockOpReq(LockOpType.LT_CANCAL_APPLY, "");
            });

            goNotOpen = mBind.GetGameObject("NotOpen");
            goOpened = mBind.GetGameObject("Opened");
            goForceLock = mBind.GetGameObject("ForceUnLock");

            txtApply = mBind.GetCom<Text>("apply");
            txtApplyEnd = mBind.GetCom<Text>("applyEnd");

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshSecurityLockDataUI, _OnRefeshUI);
        }

        protected override void UnInitBind()
        {
            UnInitButtonBind(ref btnLock);
            UnInitButtonBind(ref btnUnLock);
            UnInitButtonBind(ref btnBind);
            UnInitButtonBind(ref btnUnBind);
            UnInitButtonBind(ref btnForceUnLock);
            UnInitButtonBind(ref btnCancelForceUnLock);
            UnInitButtonBind(ref btnChangePwd);

            goNotOpen = null;
            goOpened = null;
            goForceLock = null;

            txtApply = null;
            txtApplyEnd = null;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshSecurityLockDataUI, _OnRefeshUI);
        }

        protected override void OnShowOut()
        {
            RefeshUI();
        }

        protected override void OnHideIn()
        {
       
        }

        #endregion

        #region method

        private void InitButtonBind(ref Button btn, string name, UnityEngine.Events.UnityAction callback)
        {
            btn = mBind.GetCom<Button>(name);
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(callback);
            }
        }

        private void UnInitButtonBind(ref Button btn, UnityEngine.Events.UnityAction callback = null)
        {
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn = null;
            }
            return;
        }

        private void SetShowType(ShowType eType)
        {
            eShowType = eType;
        }

        private void RefeshUI()
        {
            SecurityLockData data = SecurityLockDataManager.GetInstance().GetSecurityLockData();
            Logger.LogWarningFormat("SecurityLockData info!!!isUseLock = {0},isCommonDev = {1},lockState = {2},freeTime = {3},unFreeTime = {4}",
                data.isUseLock,
                data.isCommonDev,
                data.lockState,
                data.freezeTime,
                data.unFreezeTime);

            if (data.lockState == SecurityLockState.SECURITY_STATE_UNLOCK)
            {
                SetShowType(AccountLockSettings.ShowType.NotOpen);
            }
            else if (data.lockState == SecurityLockState.SECURITY_STATE_LOCK)
            {
                SetShowType(AccountLockSettings.ShowType.Opened);
            }
            else if (data.lockState == SecurityLockState.SECURITY_STATE_APPLY)
            {
                SetShowType(AccountLockSettings.ShowType.ForceUnLock);
            }

            if (goNotOpen != null)
            {
                goNotOpen.CustomActive(false);
            }

            if (goOpened != null)
            {
                goOpened.CustomActive(false);
            }

            if (goForceLock != null)
            {
                goForceLock.CustomActive(false);
            }

            if (eShowType == ShowType.NotOpen)
            {
                if (goNotOpen != null)
                {
                    goNotOpen.CustomActive(true);
                }

                if (!data.isUseLock)
                {
                    StaticUtility.CustomActive(btnBind, false);
                    StaticUtility.CustomActive(btnUnBind, false);
                }
                else if (data.isUseLock && !data.isCommonDev)
                {
                    StaticUtility.CustomActive(btnBind, true);
                    StaticUtility.CustomActive(btnUnBind, false);
                }
                else if (data.isUseLock && data.isCommonDev)
                {
                    StaticUtility.CustomActive(btnBind, false);
                    StaticUtility.CustomActive(btnUnBind, true);
                }
            }
            else if (eShowType == ShowType.Opened)
            {
                if (goOpened != null)
                {
                    goOpened.CustomActive(true);
                }
            }
            else if (eShowType == ShowType.ForceUnLock)
            {
                if (goForceLock != null)
                {
                    goForceLock.CustomActive(true);
                }

                if(txtApply != null)
                {
                    DateTime dateTime = Function.ConvertIntDateTime((float)data.freezeTime);
                    txtApply.text = dateTime.ToString(TR.Value("tip_timestrmp2"), System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }

                if(txtApplyEnd != null)
                {
                    DateTime dateTime = Function.ConvertIntDateTime((float)data.unFreezeTime);
                    txtApplyEnd.text = dateTime.ToString(TR.Value("tip_timestrmp2"), System.Globalization.DateTimeFormatInfo.InvariantInfo);
                }
            }
            else
            {
                Logger.LogErrorFormat("show type error!!!,type = {0}", eShowType);
            }
        }

        #endregion

        #region ui event

        private void _OnRefeshUI(UIEvent a_event)
        {
            RefeshUI();
        }

        #endregion        
    } 
}
