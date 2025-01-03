using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;
using Protocol;
using Network;

namespace GameClient
{
    class DeleteRoleConfirmFrame : ClientFrame
    {
		private Text mTxtVerify = null;

		protected override void _bindExUI()
		{
			mTxtVerify = mBind.GetCom<Text>("txtVerify");
		}

		protected override void _unbindExUI()
		{
			mTxtVerify = null;
		}

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SelecteRoleNew/DeleteRoleFrame";
        }

        [UIControl("InputField",typeof(InputField))]
        InputField m_kInputField;

        string verifyString = null;

        protected override void _OnOpenFrame()
        {
            //verifyString = TR.Value("delete_role_verify");
			verifyString = UnityEngine.Random.Range(1000, 9999).ToString();
			if (mTxtVerify  != null)
				mTxtVerify.text = verifyString;
			if (m_kInputField != null)
				m_kInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
        }

        protected override void _OnCloseFrame()
        {

        }

        bool CheckVerify()
        {
            if(!string.Equals(m_kInputField.text, verifyString))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("delete_verify_content_error"));
                return false;
            }

            return true;
        }

        [UIEventHandle("Ok")]
        void OnClickOk()
        {
            if(!CheckVerify())
            {
                return;
            }

            if(RoleObject.Selected != null &&
                RoleObject.Selected.Value != null)
            {
                Protocol.GateDeleteRoleReq req = new Protocol.GateDeleteRoleReq();
                req.roldId = RoleObject.Selected.Value.roleInfo.roleId;
				int roleLevel = RoleObject.Selected.Value.roleInfo.level;
                req.deviceId = SecurityLockDataManager.GetInstance().GetDeviceID();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

                WaitNetMessageManager.GetInstance().Wait<GateDeleteRoleRes>(msgRet =>
                {
                    if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                    {
                        if (msgRet.result == (int)Protocol.ProtoErrorCode.ENTERGAME_DELETE_ROLE_LIMIT)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(msgRet.roleUpdateLimit);
                        }
                        else if (msgRet.result == (int)ProtoErrorCode.SECURITY_LOCK_DEL_ROLE)
                        {
                            ClientSystemManager.GetInstance().OpenFrame<AccountUnLock>();
                        }
                        else
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.result);
                        }
                    }
                    else
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RoleDeleteOk, msgRet.roleId);

							if (roleLevel <= 10)
								SystemNotifyManager.SysNotifyTextAnimation(TR.Value("delete_hint1"));
							else 
								SystemNotifyManager.SysNotifyTextAnimation(TR.Value("delete_hint2"));

                        //Logger.LogErrorFormat("roleid = {0} delete succeed!", msgRet.roleId);
                    }
                }, true, 15.0f);
            }

            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Cancel")]
        void OnClickCancel()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
