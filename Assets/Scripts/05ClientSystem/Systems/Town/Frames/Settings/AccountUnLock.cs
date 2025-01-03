using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    public class AccountUnLock : ClientFrame
    {
        #region val

        string strOk = "";
        int iCount = 0;
        const int maxCount = 3;
        const int maxErrorCount = 5;
        #endregion

        #region ui bind

        InputField edtPwd1;
        Button btnClose;
        Button btnOK;
        SetComButtonCD btnCD;
        Text txtOk;
        Text txtErrorCount;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SettingPanel/AccountUnLock";
        }

        protected override void _OnOpenFrame()
        {
            iCount = 0;
            BindUIEvent();

            _OnRefreshVerifyPwdErrorCount(null);
        }

        protected override void _OnCloseFrame()
        {
            InvokeMethod.RmoveInvokeIntervalCall(this);

            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            edtPwd1 = mBind.GetCom<InputField>("Pwd1");     
            btnClose = mBind.GetCom<Button>("BtnClose");
            btnCD = mBind.GetCom<SetComButtonCD>("btOKCD");
            if (btnClose != null)
            {
                btnClose.onClick.RemoveAllListeners();
                btnClose.onClick.AddListener(() => 
                {
                    frameMgr.CloseFrame(this);
                });
            }

            txtOk = mBind.GetCom<Text>("txtOk");
            if(txtOk != null)
            {
                strOk = txtOk.text;
            }

            txtErrorCount = mBind.GetCom<Text>("txtErrorCount");

            btnOK = mBind.GetCom<Button>("btOK");
            if(btnOK != null)
            {
                btnOK.onClick.RemoveAllListeners();
                btnOK.onClick.AddListener(() => 
                {
                    if(edtPwd1 != null)
                    {
                        if(edtPwd1.textComponent.text.Length < SecurityLockDataManager.nPwdMinLength || edtPwd1.textComponent.text.Length > SecurityLockDataManager.nPwdMaxLength)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect("密码长度错误(密码长度最小4位，最大8位)");
                            return;
                        }                       

                        ClientSystemLogin system = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemLogin;
                        if(system != null)
                        {
                            SecurityLockDataManager.GetInstance().SendGateSecurityLockRemoveReq(edtPwd1.textComponent.text);
                        }
                        else
                        {
                            SecurityLockDataManager.GetInstance().SendWorldSecurityLockOpReq(LockOpType.LT_UNLOCK, edtPwd1.textComponent.text);
                        }

                       

                        InvokeMethod.InvokeInterval(this,0.0f,1.0f,maxCount,
                            () => 
                            {
                                if (btnOK != null)
                                {
                                    UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>();
                                    if (gray != null)
                                    {
                                        gray.SetEnable(true);
                                    }

                                    btnOK.interactable = false;
                                    btnOK.image.raycastTarget = false;
                                }

                                if (txtOk != null)
                                {
                                    txtOk.text = string.Format("{0}({1}s)", strOk, maxCount - iCount);
                                    iCount++;
                                }

                            },
                            () => 
                            {
                                if(txtOk != null)
                                {
                                    txtOk.text = string.Format("{0}({1}s)",strOk, maxCount - iCount);
                                    iCount++;
                                }
                            },
                            () => 
                            {
                                if (btnOK != null)
                                {
                                    UIGray gray1 = btnOK.gameObject.GetComponent<UIGray>();
                                    if (gray1 != null)
                                    {
                                        gray1.SetEnable(false);
                                    }

                                    btnOK.interactable = true;
                                    btnOK.image.raycastTarget = true;
                                }

                                if (txtOk != null)
                                {
                                    txtOk.text = string.Format("{0}", strOk);
                                    iCount = 0;
                                }
                            });
                    }                   
                });
            }           
        }

        protected override void _unbindExUI()
        {
            edtPwd1 = null;      
            btnClose = null;
            btnOK = null;
            btnCD = null;
            txtOk = null;
            txtErrorCount = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        #endregion

        #region ui event

        void _OnRefreshVerifyPwdErrorCount(UIEvent uiEvent)
        {
            SecurityLockData data = SecurityLockDataManager.GetInstance().GetSecurityLockData();
            if(txtErrorCount != null)
            {
                txtErrorCount.text = TR.Value("verifyPwdFailedCount", maxErrorCount, data.verifyPwdFailedCount);
            }

            if(data.verifyPwdFailedCount >= maxErrorCount)
            {
                InvokeMethod.RmoveInvokeIntervalCall(this);

                if(btnOK != null)
                {
                    UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>();
                    if (gray != null)
                    {
                        gray.SetEnable(true);
                    }

                    btnOK.interactable = false;
                    btnOK.image.raycastTarget = false;
                }

                if (txtOk != null)
                {
                    txtOk.text = string.Format("{0}", strOk);
                    iCount = 0;
                }
            }
        }

        #endregion
    }
}
