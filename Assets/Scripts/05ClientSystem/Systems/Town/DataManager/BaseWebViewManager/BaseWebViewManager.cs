using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Protocol;
using Network;

namespace GameClient
{
    public class BaseWebViewManager : DataManager<BaseWebViewManager>
    {

        private UnityEngine.Coroutine waitToShowScreenShotTips;
        private float mReportConsumeActivityValue = 0f;       

        public override void Initialize()
        {
            _InitReportLocalData();
        }

        public override void Clear()
        {
            if (waitToShowScreenShotTips != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToShowScreenShotTips);
                waitToShowScreenShotTips = null;
            }
        }

        private void _OpenBaseWebViewFrame(BaseWebViewParams param)
        {
            if (null == param)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<BaseWebViewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<BaseWebViewFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<BaseWebViewFrame>(FrameLayer.TopMoreMost, param);
        }

        private void _CloseBaseWebViewFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<BaseWebViewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<BaseWebViewFrame>();
            }
        }

        #region 问卷调查

        public bool CanUnlockQuestionnaire()
        {
            string url = ClientApplication.questionnaireUrl;
            if (string.IsNullOrEmpty(url) || (!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.Questionnaire)))
            {
                return false;
            }
            bool isSDKEnable = PluginManager.instance.IsSDKEnableSystemVersion(SDKInterface.FuncSDKType.FSDK_UniWebView);
            if (!isSDKEnable)
            {
                return false;
            }
            return true;
        }
        public bool CanOpenQuestionnaire()
        {
            string url = ClientApplication.questionnaireUrl;
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Questionnaire);

            if (string.IsNullOrEmpty(url) || (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("questionnaire_unlock_fail", functionData.FinishLevel));
                return false;
            }
            return true;
        }
        public string GetQuestionnaireUrl()
        {
            return ClientApplication.questionnaireUrl;
        }

        #endregion

        #region Report

        public bool IsReportFuncOpen()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(ClientApplication.reportPlayerUrl))
            {
                ClientApplication.reportPlayerUrl = "http://118.31.3.235/report.html";
            }
#endif
            if (string.IsNullOrEmpty(ClientApplication.reportPlayerUrl))
            {
                return false;
            }
            if (!Utility.IsFunctionCanUnlock(ProtoTable.FunctionUnLock.eFuncType.ReportingFunction))
            {
                return false;
            }
            bool isSDKEnable = PluginManager.instance.IsSDKEnableSystemVersion(SDKInterface.FuncSDKType.FSDK_UniWebView);
            if (!isSDKEnable)
            {
                return false;
            }
            return true;
        }

        public void TryOpenReportFrame(RelationData rData)
        {
            if (!_CheckEnableToReport())
            {
                return;
            }
            if (rData == null)
            {
                return;
            }
            InformantInfo info = GetInformantInfoByReleationData(rData);
            if (info != null)
            {
                TryOpenReportFrame(info);
            }
        }

        public void TryOpenReportFrame(InformantInfo info)
        {
            if (!_CheckEnableToReport())
            {
                return;
            }
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.ReportingFunction);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                return;
            }

            string fullUrl = _GetReportFullUrl(info);
            if (string.IsNullOrEmpty(fullUrl))
            {
                return;
            }
            BaseWebViewParams param = new BaseWebViewParams();
            param.type = BaseWebViewType.OnlineReport;
            param.fullUrl = fullUrl;
            param.uniWebViewMsgs = new List<BaseWebViewMsg>();
            param.uniWebViewMsgs.Add(
                     new BaseWebViewMsg()
                     {
                         scheme = "uniwebview",
                         path = "close",
                         onReceiveWebViewMsg = _OnWebViewMsgCloseFrame
                     }                    
                );
            param.uniWebViewMsgs.Add(
                    new BaseWebViewMsg()
                    {
                        scheme = "uniwebview",
                        path = "reportsuccess",
                        onReceiveWebViewMsg = _OnWebViewMsgReportSucc
                    }
                );
            _OpenBaseWebViewFrame(param);
        }

        public string GetJobNameByJobId(int jobId)
        {
            string jobName = "";
            ProtoTable.JobTable jobTableData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(jobId);
            if (jobTableData != null)
            {
                jobName = jobTableData.Name;
            }
            return jobName;
        }

        public InformantInfo GetInformantInfoByReleationData(RelationData rData)
        {
            if (rData == null)
            {
                return new InformantInfo();
            }
            InformantInfo info = new InformantInfo();
            info.roleId = rData.uid.ToString();
            info.roleName = rData.name;
            info.roleLevel = rData.level.ToString();
            info.vipLevel = rData.vipLv.ToString();
            info.jobId = rData.occu.ToString();
            info.jobName = GetJobNameByJobId((int)rData.occu);
            return info;
        }

        private ReporterInfo _GetReportPlayerInfo()
        {
            ReporterInfo localReprter =  new ReporterInfo();
            localReprter.roleId = PlayerBaseData.GetInstance().RoleID.ToString();
            localReprter.roleName = PlayerBaseData.GetInstance().Name;
            localReprter.roleLevel = PlayerBaseData.GetInstance().Level.ToString();
            localReprter.vipLevel = PlayerBaseData.GetInstance().VipLevel.ToString();
            int jobId = PlayerBaseData.GetInstance().JobTableID;
            localReprter.jobId = jobId.ToString();
            localReprter.jobName = GetJobNameByJobId(jobId);

            if (ClientApplication.playerinfo != null)
            {
                localReprter.accId = ClientApplication.playerinfo.accid.ToString();
                localReprter.serverId = ClientApplication.playerinfo.serverID.ToString();                
            }
            localReprter.serverName = ClientApplication.adminServer.name;
            localReprter.platformName = SDKInterface.Instance.GetPlatformNameByChannel();

            return localReprter;
        }

        private string _GetReportFullUrl(InformantInfo info)
        {
            string reportUrl = _FormatReportUrl(info);
            if (string.IsNullOrEmpty(ClientApplication.reportPlayerUrl) || 
                string.IsNullOrEmpty(reportUrl))
            {
                return "";
            }
            string fullUrl = string.Format("{0}?{1}", ClientApplication.reportPlayerUrl, reportUrl);
            return fullUrl;
        }

        private string _FormatReportUrl(InformantInfo info)
        {
            if (null == info)
            {
                return "";
            }
            InformantInfo informant = info;
            ReporterInfo localReprter = _GetReportPlayerInfo();

            StringBuilder urlSb = new StringBuilder();
            urlSb.AppendFormat("platform={0}&zoneid={1}&server={2}&", 
                localReprter.platformName, localReprter.serverId, localReprter.serverName);
            urlSb.AppendFormat("target_role_id={0}&target_role_name={1}&target_vip={2}&target_occu={3}&target_accid={4}&target_occu_name={5}&target_role_level={6}&", 
                informant.roleId, informant.roleName, informant.vipLevel, informant.jobId, "0", informant.jobName, informant.roleLevel);
            urlSb.AppendFormat("reporter_accid={0}&reporter_role_id={1}&reporter_role_name={2}&reporter_occu={3}&reporter_vip={4}",
                localReprter.accId, localReprter.roleId, localReprter.roleName, localReprter.jobId, localReprter.vipLevel);

            //string url = string.Format("platform={0}&zoneid={1}&server={2}&" +
            //    "target_role_id={3}&target_role_name={4}&target_vip={5}&target_occu={6}&" +
            //    "reporter_accid={7}&reporter_role_id={8}&reporter_role_name={9}&reporter_occu={10}&reporter_vip={11}",
            //    localReprter.platformName,localReprter.serverId,localReprter.serverName,
            //    informant.roleId,informant.roleName,informant.vipLevel, informant.jobId,
            //    localReprter.accId,localReprter.roleId,localReprter.roleName,localReprter.jobId,localReprter.vipLevel);

            return urlSb.ToString();
        }

        private void _NotifyReportSucc()
        {
            SceneReportNotify req = new SceneReportNotify();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        private void _OnWebViewMsgReportSucc(Dictionary<string, string> msgArgs, UniWebViewUtility uniWebViewUti)
        {
            _NotifyReportSucc();
        }

        private void _InitReportLocalData()
        {
            var systemValueTable = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType3.SVT_REPORT_CONSUME);
            if (systemValueTable != null)
            {
                mReportConsumeActivityValue = systemValueTable.Value;
            }
        }

        private bool _CheckEnableToReport()
        {
            if (PlayerBaseData.GetInstance().ActivityValue < mReportConsumeActivityValue)
            {
                SystemNotifyManager.SystemNotify(100016, mReportConsumeActivityValue);
                return false;
            }
            return true;
        }

        #endregion

        #region Convert Package

        public const string convertAccInfoFilePrefix = "convertInfo";
        public const string convertAccInfoFileExtension = "conf";

        public bool IsConvertAccountFuncOpen()
        {
#if UNITY_EDITOR
            return false;
#endif
            if (string.IsNullOrEmpty(ClientApplication.convertAccountInfoUrl))
            {
                return false;
            }
            bool isSDKEnable = PluginManager.instance.IsSDKEnableSystemVersion(SDKInterface.FuncSDKType.FSDK_UniWebView);
            if (!isSDKEnable)
            {
                return false;
            }
            return true;
        }

        public void ShowConvertAccountTips()
        {
            if (IsConvertAccountFuncOpen() == false)
            {
                return;
            }
            SystemNotifyManager.SysNotifyMsgBoxCancelOk(TR.Value("zymg_convert_account_tips"),TR.Value("zymg_convert_account_tips_cancel_desc"), "",
                null,
                () =>
                {
                    ReqGateConvertAccount();
                },
                5, 
                false,
                new CommonMsgBoxCancelOKParams()
                {
                    closeFrameOnCancelBtnCDEnd = false,
                    showCancelBtnGrayOnCDEnd = true
                }
                );
        }

        public void ReqGateConvertAccount()
        {
            if (IsConvertAccountFuncOpen() == false)
            {
                return;
            }
            GateConvertAccountReq req = new GateConvertAccountReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            WaitNetMessageManager.GetInstance().Wait<GateConvertAccountRes>(_WaitGateConvetAccountRes);
        }

        private void _WaitGateConvetAccountRes(GateConvertAccountRes msgRet)
        {
            if (null == msgRet)
            {
                return;
            }
            ConvertAccountInfo info = new ConvertAccountInfo();
            info.account = msgRet.account;
            info.password = msgRet.passwd;
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(info.account))
            {
                info.account = "testacc";
            }
            if (string.IsNullOrEmpty(info.password))
            {
                info.password = "testpwd";
            }
#endif
            info.channelName = SDKInterface.Instance.GetPlatformNameByChannel();
            if (ClientApplication.playerinfo != null)
            {
                if (string.IsNullOrEmpty(ClientApplication.playerinfo.openuid))
                {
                    info.originalOpenUid = ClientApplication.playerinfo.accid.ToString();
                }
                else
                {
                    info.originalOpenUid = ClientApplication.playerinfo.openuid;
                }
            }

            //bool isScreenShotOn = !ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(Protocol.ServiceType.SERVICE_CONVERT_ACC_SCREENSHOT);
            info.needScreenShot = msgRet.screenShot == 1 ? "0" : "1";
            if (msgRet.saveFile == 1)
            {
                _SaveConvertAccInfoToLocal(info);
            }     

            string fullUrl = _GetConvertAccountFullUrl(info);
            if (string.IsNullOrEmpty(fullUrl))
            {
                return;
            }
            BaseWebViewParams param = new BaseWebViewParams();
            param.type = BaseWebViewType.ConvertAccountInfo;
            param.fullUrl = fullUrl;
            param.uniWebViewMsgs = new List<BaseWebViewMsg>();
            param.uniWebViewMsgs.Add(
                 new BaseWebViewMsg()
                 {
                     scheme = "uniwebview",
                     path = "screenshot",
                     onReceiveWebViewMsg = _OnWebViewMsgScreenShot
                 }
                );

            param.uniWebViewMsgs.Add(
                 new BaseWebViewMsg()
                 {
                     scheme = "uniwebview",
                     path = "close",
                     onReceiveWebViewMsg = _OnWebViewMsgCloseFrame
                 }
                );
            _OpenBaseWebViewFrame(param);
        }


        private void _SaveConvertAccInfoToLocal(ConvertAccountInfo info)
        {
            if (info == null)
            {
                return;
            }
            string convertFileName = _GetConvertLocalFileName(info.channelName);
            string localAccInfoContent = "";

            try
            {
                localAccInfoContent = PluginManager.GetInstance().ReadDoc(TR.Value("zymg_convert_account_filerootpath"), convertFileName);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[BaseWebViewManager] -_SaveConvertAccInfoToLocal, ReadDoc failed : {0}", e.ToString());
            }

            LocalConvertAccInfos localAccInfos = null;
            bool isExistContent = false;

            try
            {
                if (!string.IsNullOrEmpty(localAccInfoContent))
                {
                    localAccInfos = LitJson.JsonMapper.ToObject<LocalConvertAccInfos>(localAccInfoContent);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[BaseWebViewManager] -_SaveConvertAccInfoToLocal, LitJson toObject failed : {0}", e.ToString());
            }

            if (localAccInfos != null && localAccInfos.convertAccountInfos != null)
            {
                for (int i = 0; i < localAccInfos.convertAccountInfos.Count; i++)
                {
                    var accinfo = localAccInfos.convertAccountInfos[i];
                    if (accinfo == null) continue;
                    if (accinfo.originalOpenUid == info.originalOpenUid &&
                        accinfo.channelName == info.channelName)
                    {
                        isExistContent = true;
                        break;
                    }
                }
            }
            if (isExistContent)
            {
                return;
            }
            if (localAccInfos == null)
            {
                localAccInfos = new LocalConvertAccInfos();
            }
            localAccInfos.convertAccountInfos.Add(info);

            string accInfoJsonText = "";
            try
            {
                accInfoJsonText = LitJson.JsonMapper.ToJson(localAccInfos);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[BaseWebViewManager] -_SaveConvertAccInfoToLocal, LitJson toJson failed : {0}", e.ToString());
            }

            if (!string.IsNullOrEmpty(accInfoJsonText))
            {
                try
                {
                    PluginManager.GetInstance().SaveDoc(accInfoJsonText, TR.Value("zymg_convert_account_filerootpath"), convertFileName, false);  //覆盖写入
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("[BaseWebViewManager] -_SaveConvertAccInfoToLocal, SaveDoc failed : {0}", e.ToString());
                }
            }
        }

        private string _GetConvertLocalFileName(string channelName)
        {
            return string.Format("{0}_{1}.{2}", convertAccInfoFilePrefix, channelName, convertAccInfoFileExtension);
        }

        private void _OnWebViewMsgScreenShot(Dictionary<string, string> msgArgs, UniWebViewUtility uniWebViewUti)
        {
            GameClient.TakePhotoModeFrame.MobileScreenShoot();          //TODO 注意 Unity ReadPixels 无法截取UniWebView内容 ！！！

            if (waitToShowScreenShotTips != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToShowScreenShotTips);
            }
            waitToShowScreenShotTips = GameFrameWork.instance.StartCoroutine(_WaitToShowScreenShotTips(uniWebViewUti));
        }

        private void _OnWebViewMsgCloseFrame(Dictionary<string, string> msgArgs, UniWebViewUtility uniWebViewUti)
        {
            _CloseBaseWebViewFrame();
        }

        private IEnumerator _WaitToShowScreenShotTips(UniWebViewUtility webViewUti)
        {
            //Test !!!!!!!
            yield return Yielders.GetWaitForSeconds(1f);

            if (webViewUti != null)
            {
                webViewUti.ExcuteJS("screen();");
            }
        }

        private string _GetConvertAccountFullUrl(ConvertAccountInfo info)
        {
            string convertAccUrl = _FormatConvertAccountUrl(info);
            if (string.IsNullOrEmpty(ClientApplication.convertAccountInfoUrl) || 
                string.IsNullOrEmpty(convertAccUrl))
            {
                return "";
            }
            string fullUrl = string.Format("{0}?{1}", ClientApplication.convertAccountInfoUrl, convertAccUrl);
            return fullUrl;
        }

        private string _FormatConvertAccountUrl(ConvertAccountInfo info)
        {
            if (null == info)
            {
                return "";
            }
            string url = string.Format("account={0}&password={1}&screen={2}&channel={3}",
                info.account, info.password, info.needScreenShot, info.channelName);
            return url;
        }

#endregion
    }
}