using UnityEngine;
using System.Collections;
using System;
using System.Text;
using LitJson;

namespace SDKClient
{
    public class SDKInterfaceAndroidChannel : SDKInterfaceAndroid
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
                    GetActivity().Call("Init", debug, apkInfoJson);
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("[Init] failed : {0}", e.ToString());
            }
        }

        public override void Login()
        {
            GetActivity().Call("Login");
        }

        public override void Logout()
        {
            GetActivity().Call("Logout");
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
                    if (string.IsNullOrEmpty(userInfoString))
                    {
                        Logger.LogError("userInfoString is Null !!!");
                        return;
                    }
                    GetActivity().Call("Pay", payInfoString, userInfoString);
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("payinfoJson编译传入失败： {0}", e.ToString());
            }
        }

        //信息上报
        public override void ReportRoleInfo(SDKReportRoleInfo roleInfo)
        {
            string roleInfoString = null;
            try
            {
                if (roleInfo == null)
                {
                    Logger.LogError("updateInfo = NULL");
                }
                else
                {
                    roleInfoString = JsonMapper.ToJson(roleInfo);
                    if (!string.IsNullOrEmpty(roleInfoString))
                    {
                        GetActivity().Call("UpdateRoleInfo", roleInfoString, (int)roleInfo.sceneType);
                    }
                    else
                    {
                        Logger.LogError("roleInfoString = NULL");
                    }
                }

            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("### ReportRoleInfo 上报角色信息 failed : {0}", e.ToString());
            }
        }

        public override void OpenBindPhone()
        {
            try
            {
                GetActivity().Call("OpenBindPhone");
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("OpenBindPhone invoke failed : {0}", e.ToString());
            }
        }

        public override void CheckIsPhoneBind()
        {
            try
            {
                GetActivity().Call("CheckBindPhoneSucc");
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("CheckBindPhoneSucc invoke failed : {0}", e.ToString());
            }
        }


        public override void SetSpecialPayUserInfo(string openUid, string token)
        {
            base.SetSpecialPayUserInfo(openUid, token);
            try
            {
                GetActivity().Call("SetUserInfo", openUid, token);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("SetSpecialPayUserInfo invoke failed: {0}", e.ToString());
            }
        }
    }
}