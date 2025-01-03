using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    struct SecurityLockData
    {
        public SecurityLockState lockState;
        /// <summary>
        ///  是否常用设备
        /// </summary>
        public bool isCommonDev;
        /// <summary>
        ///  冻结时间
        /// </summary>
        public UInt32 freezeTime;
        /// <summary>
        ///  解冻时间
        /// </summary>
        public UInt32 unFreezeTime;

        public bool bBindDevice;

        public bool isUseLock;

        public uint verifyPwdFailedCount;
    }

    class SecurityLockDataManager : DataManager<SecurityLockDataManager>
    {
        #region variables
        private bool m_bNetBind = false;
        private string m_kDataFileName = "BindDevice.dat";
        private string m_kDeviceID = "";
        private uint m_nApplyStateBtnClickedCount = 0;

        private SecurityLockData m_kSecurityLockData;
        public static int nPwdMinLength = 4;
        public static int nPwdMaxLength = 8;
        #endregion

        #region overrides
        public override void Initialize()
        {
            m_kSecurityLockData.lockState = SecurityLockState.SECURITY_STATE_UNLOCK;
            m_kSecurityLockData.isCommonDev = false;
            m_kSecurityLockData.freezeTime = 0;
            m_kSecurityLockData.unFreezeTime = 0;
            m_kSecurityLockData.bBindDevice = false;
            m_kSecurityLockData.isUseLock = false;
            m_kSecurityLockData.verifyPwdFailedCount = 0;

            m_nApplyStateBtnClickedCount = 0;
      
            _BindNetMsg();
        }

        public override void Clear()
        {
            m_bNetBind = false;
            _UnBindNetMsg();           
        }

        public override void Update(float timeElapsed)
        {
            return;
        }

        public override void OnApplicationStart()
        {
            FileArchiveAccessor.LoadFileInPersistentFileArchive(m_kDataFileName, out m_kDeviceID);
            if (m_kDeviceID == null)
            {
                FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kDataFileName, "");
                m_kDeviceID = "";
                return;
            }

            return;
        }       

        #endregion

        #region methods
        private void _BindNetMsg()
        {
            if (m_bNetBind == false)
            {
                NetProcess.AddMsgHandler(WorldSecurityLockDataRes.MsgID, _OnWorldSecurityLockDataRes);
                NetProcess.AddMsgHandler(WorldSecurityLockOpRes.MsgID, _OnWorldSecurityLockOpRes);
                NetProcess.AddMsgHandler(WorldChangeSecurityPasswdRes.MsgID, _OnWorldChangeSecurityPasswdRes);
                NetProcess.AddMsgHandler(WorldBindDeviceRes.MsgID, _OnWorldBindDeviceRes);
                NetProcess.AddMsgHandler(WorldSecurityLockForbidNotify.MsgID, _OnWorldSecurityLockForbidNotifyRes);
                NetProcess.AddMsgHandler(GateSecurityLockRemoveRes.MsgID, _OnGateSecurityLockRemoveRes);
                NetProcess.AddMsgHandler(WorldSecurityLockPasswdErrorNum.MsgID, _OnWorldSecurityLockPasswdErrorNum);

                m_bNetBind = true;
            }
        }

        private void _UnBindNetMsg()
        {
            NetProcess.RemoveMsgHandler(WorldSecurityLockDataRes.MsgID, _OnWorldSecurityLockDataRes);
            NetProcess.RemoveMsgHandler(WorldSecurityLockOpRes.MsgID, _OnWorldSecurityLockOpRes);
            NetProcess.RemoveMsgHandler(WorldChangeSecurityPasswdRes.MsgID, _OnWorldChangeSecurityPasswdRes);
            NetProcess.RemoveMsgHandler(WorldBindDeviceRes.MsgID, _OnWorldBindDeviceRes);
            NetProcess.RemoveMsgHandler(WorldSecurityLockForbidNotify.MsgID, _OnWorldSecurityLockForbidNotifyRes);
            NetProcess.RemoveMsgHandler(GateSecurityLockRemoveRes.MsgID, _OnGateSecurityLockRemoveRes);
            NetProcess.RemoveMsgHandler(WorldSecurityLockPasswdErrorNum.MsgID, _OnWorldSecurityLockPasswdErrorNum);

            m_bNetBind = false;
        }

        public SecurityLockData GetSecurityLockData()
        {
            return m_kSecurityLockData;
        }

        public string GetDeviceID()
        {
            return m_kDeviceID;
        }

        public uint BtnClickedCount
        {
            get { return m_nApplyStateBtnClickedCount; }
            set { m_nApplyStateBtnClickedCount = value; }
        }

        public bool CheckSecurityLock(Func<bool> conditionFunc = null,Action UnLockAction = null)
        {
            if(ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_SECURITY_LOCK))
            {
                return false;
            }

            SecurityLockState lockState = m_kSecurityLockData.lockState;
            if(lockState == SecurityLockState.SECURITY_STATE_LOCK || lockState == SecurityLockState.SECURITY_STATE_APPLY)
            {
                bool bFlag = true;
                if(conditionFunc != null)
                {
                    bFlag = conditionFunc();
                }

                if(bFlag)
                {
                    ClientSystemManager.GetInstance().OpenFrame<AccountUnLock>(FrameLayer.Middle, UnLockAction);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        #endregion

        #region send msg
        public void SendWorldSecurityLockDataReq()
        {
            WorldSecurityLockDataReq req = new WorldSecurityLockDataReq();
            req.deviceID = m_kDeviceID;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendWorldSecurityLockOpReq(LockOpType opType,string passWord)
        {
            WorldSecurityLockOpReq req = new WorldSecurityLockOpReq();
            req.lockOpType = (uint)opType;
            req.passwd = passWord;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendWorldChangeSecurityPasswdReq(string oldPassword, string newPassword)
        {
            WorldChangeSecurityPasswdReq req = new WorldChangeSecurityPasswdReq();
            req.oldPasswd = oldPassword;
            req.newPasswd = newPassword;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendWorldBindDeviceReq(bool bBind)
        {
            WorldBindDeviceReq req = new WorldBindDeviceReq();
            req.bindType = (uint)(bBind ? 1 : 0);
            req.deviceID = m_kDeviceID; 

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        public void SendGateSecurityLockRemoveReq(string passWord)
        {
            GateSecurityLockRemoveReq req = new GateSecurityLockRemoveReq();
            req.passwd = passWord;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            return;
        }

        #endregion

        #region msg handle
        private void _OnWorldSecurityLockDataRes(MsgDATA msg)
        {
            WorldSecurityLockDataRes msgData = new WorldSecurityLockDataRes();
            msgData.decode(msg.bytes);

            m_kSecurityLockData.lockState = (SecurityLockState)msgData.lockState;
            m_kSecurityLockData.isCommonDev = msgData.isCommonDev > 0;
            m_kSecurityLockData.freezeTime = msgData.freezeTime;
            m_kSecurityLockData.unFreezeTime = msgData.unFreezeTime;
            m_kSecurityLockData.isUseLock = msgData.isUseLock > 0;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshSecurityLockDataUI);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SecurityLockApplyStateButton);

            //             if (m_kSecurityLockData.lockState == SecurityLockState.SECURITY_STATE_APPLY && AdsPush.LoginPushManager.GetInstance().IsFirstLogin())
            //             {
            //                 SystemNotifyManager.SystemNotifyOkCancel(9932, () => 
            //                 {
            //                     ClientSystemManager.GetInstance().OpenFrame<SettingFrame>(FrameLayer.Middle,SettingFrame.TabType.ACCOUNT_LOCK);
            //                 },null);
            //             }

            return;
        }

        private void _OnWorldSecurityLockOpRes(MsgDATA msg)
        {
            WorldSecurityLockOpRes msgData = new WorldSecurityLockOpRes();
            msgData.decode(msg.bytes);

            if (msgData.ret != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.ret);
                return;
            }

            m_kSecurityLockData.lockState = (SecurityLockState)msgData.lockState;
            m_kSecurityLockData.freezeTime = msgData.freezeTime;
            m_kSecurityLockData.unFreezeTime = msgData.unFreezeTime;

            if(msgData.lockOpType == (uint)LockOpType.LT_LOCK)
            {
                if (m_kSecurityLockData.isUseLock)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("安全锁上锁成功");
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("安全锁密码设置成功");
                }

                ClientSystemManager.GetInstance().CloseFrame<AccountLock>();
            }

            if(msgData.lockOpType == (uint)LockOpType.LT_UNLOCK)
            {
                m_kSecurityLockData.isUseLock = true;
                SystemNotifyManager.SysNotifyFloatingEffect("安全锁解锁成功");
                ClientSystemManager.GetInstance().CloseFrame<AccountUnLock>();
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshSecurityLockDataUI);
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SecurityLockApplyStateButton);

            return;
        }

        private void _OnWorldChangeSecurityPasswdRes(MsgDATA msg)
        {
            WorldChangeSecurityPasswdRes msgData = new WorldChangeSecurityPasswdRes();
            msgData.decode(msg.bytes);

            if (msgData.ret != (uint)ProtoErrorCode.SUCCESS)
            {
                if (msgData.ret == (uint)ProtoErrorCode.SECURITY_LOCK_CHANGE_PASSWD_ERROR)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(string.Format("原密码输入错误，本日错误次数：{0}/5", msgData.errNum));
                    return;
                }

                SystemNotifyManager.SystemNotify((int)msgData.ret);
                return;
            }

            m_kSecurityLockData.isCommonDev = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshSecurityLockDataUI);

            SystemNotifyManager.SysNotifyFloatingEffect("密码修改成功");
            ClientSystemManager.GetInstance().CloseFrame<AccountLockChangePwd>();

            return;
        }

        private void _OnWorldBindDeviceRes(MsgDATA msg)
        {
            WorldBindDeviceRes msgData = new WorldBindDeviceRes();
            msgData.decode(msg.bytes);

            if (msgData.ret != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.ret);
                return;
            }

            bool bBindOld = m_kSecurityLockData.bBindDevice;
            m_kSecurityLockData.bBindDevice = msgData.bindState > 0;

            if(m_kSecurityLockData.bBindDevice)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("绑定成功，下次登陆开始生效");
                ClientSystemManager.GetInstance().CloseFrame<AccountLockBindDevice>();
            }
            else if(!m_kSecurityLockData.bBindDevice)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("设备解绑成功");
            }

            if(m_kSecurityLockData.bBindDevice)
            {
                m_kDeviceID = msgData.deviceID;
                m_kSecurityLockData.isCommonDev = true;

                try
                {            
                    if (!string.IsNullOrEmpty(m_kDeviceID))
                    {
                        FileArchiveAccessor.SaveFileInPersistentFileArchive(m_kDataFileName, m_kDeviceID);
                    }
                }
                catch (System.Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }
            else
            {
                m_kSecurityLockData.isCommonDev = false;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshSecurityLockDataUI);

            return;
        }

        private void _OnWorldSecurityLockForbidNotifyRes(MsgDATA msg)
        {
            WorldSecurityLockForbidNotify msgData = new WorldSecurityLockForbidNotify();
            msgData.decode(msg.bytes);

            ClientSystemManager.GetInstance().OpenFrame<AccountUnLock>();

            return;
        }
        
        private void _OnGateSecurityLockRemoveRes(MsgDATA msg)
        {
            GateSecurityLockRemoveRes msgData = new GateSecurityLockRemoveRes();
            msgData.decode(msg.bytes);

            if (msgData.ret != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)msgData.ret);
                return;
            }

            SystemNotifyManager.SysNotifyFloatingEffect("安全锁解锁成功");
            ClientSystemManager.GetInstance().CloseFrame<AccountUnLock>();

            if (RoleObject.Selected != null && RoleObject.Selected.Value != null)
            {
                Protocol.GateDeleteRoleReq req = new Protocol.GateDeleteRoleReq();
                req.roldId = RoleObject.Selected.Value.roleInfo.roleId;
                int roleLevel = RoleObject.Selected.Value.roleInfo.level;
                req.deviceId = SecurityLockDataManager.GetInstance().GetDeviceID();

                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }

            return;
        }

        private void _OnWorldSecurityLockPasswdErrorNum(MsgDATA msg)
        {
            WorldSecurityLockPasswdErrorNum msgData = new WorldSecurityLockPasswdErrorNum();
            msgData.decode(msg.bytes);

            m_kSecurityLockData.verifyPwdFailedCount = msgData.error_num;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshVerifyPwdErrorCount);
            return;
        }

        #endregion

    }
}
