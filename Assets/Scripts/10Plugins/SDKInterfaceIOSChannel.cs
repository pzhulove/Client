using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LitJson;

namespace SDKClient
{
#if UNITY_EDITOR
    public class SDKInterfaceIOSChannel : SDKInterfaceIOS
    {
    }
#elif UNITY_IPHONE || UNITY_IOS

    public class SDKInterfaceIOSChannel : SDKInterfaceIOS
    {
        public override void Init(bool debug)
        {
            base.Init(debug);

            string apkInfoJson = null;
            try
            {
                if (m_ApkInfo != null)
                {
                    apkInfoJson = JsonMapper.ToJson(m_ApkInfo);
                    _Init(apkInfoJson, debug);
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[Init] failed : {0}", e.ToString());
            }
        }


        public override void Login()
        {
            base.Login();
            if (!_IsLoginSmallGame())
            {
                _OpenLogin();
            }
        }

        public override void Pay(SDKPayInfo payInfo)
        {
            try
            {
                string payInfoString = null;
                string userInfoString = null;
                if (payInfo == null || m_SDKUserInfo == null)
                {
                    Logger.LogError("payInfo = Null or m_SDKUserInfo is Null");
                }
                else
                {
                    payInfoString = JsonMapper.ToJson(payInfo);
                    userInfoString = JsonMapper.ToJson(SDKUserInfo);
                    if (string.IsNullOrEmpty(payInfoString))
                    {
                        Logger.LogError("payInfoString is Null");
                        return;
                    }
                    if(string.IsNullOrEmpty(userInfoString))
                    {
                        Logger.LogError("userInfoString is Null !!!");
                        return;
                    }
                    _Pay(payInfoString, userInfoString);
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("Pay failed : {0}" , e.ToString());
            }
        }

        public override void Logout()
        {
            //_Logout();
        }

        public override void ReportRoleInfo(SDKReportRoleInfo updateInfo)
        {
            try
            {
                string updateInfoString = null;
                if (updateInfo == null)
                {
                    Logger.LogError("roleIofo = NULL");
                }
                else
                {
                    updateInfoString = JsonMapper.ToJson(updateInfo);
                    if (!string.IsNullOrEmpty(updateInfoString))
                    {
                        _ReportRoleInfo(updateInfoString, (int)updateInfo.sceneType);
                    }
                    else
                    {
                        Logger.LogError("roleInfoString = NULL");
                    }
                }
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("### ReportRoleInfo 上报角色信息 失败 : {0}", e.ToString());
            }
        }

        public override void GetNewVersionInAppstore()
        {
            _GetNewVersionInAppstore();
        }

        public override bool NeedSDKBindPhoneOpen()
        {
            return true;
        }

        public override void CheckIsPhoneBind()
        {
            _CheckIsBindPhoneNum();
        }

        public override void OpenBindPhone()
        {
            _OpenMobileBind();
        }

        public override bool IsIOSAppstoreLoadSmallGame()
        {
            return _IsLoginSmallGame();
        }

        // --- dllimport start ---
        [DllImport("__Internal")]
        private static extern void _Init(string apkInfoString, bool debug);

        [DllImport("__Internal")]
        private static extern void _OpenLogin();

        [DllImport("__Internal")]
        private static extern void _Pay(string payIofoString, string userInfoString);

        [DllImport("__Internal")]
        private static extern void _ReportRoleInfo(string updateInfoString ,int pram);

        [DllImport("__Internal")]
        private static extern void _GetNewVersionInAppstore();

        [DllImport("__Internal")]
        private static extern void _OpenMobileBind();

        [DllImport("__Internal")]
        private static extern void _CheckIsBindPhoneNum();

        [DllImport("__Internal")]
        private static extern bool _IsLoginSmallGame();

        [DllImport("__Internal")]
        private static extern bool _IsSDKInited();

    }

#endif

}
