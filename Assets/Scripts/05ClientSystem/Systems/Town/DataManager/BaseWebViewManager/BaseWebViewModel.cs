using System;
using System.Collections.Generic;

namespace GameClient
{
    public enum BaseWebViewType
    {
        None = 0,
        OnlineService,              //在线客服
        ChannelRanklist,            //分渠道排行榜浏览    
        OperateAdsBoard,            //运营方社区和官网浏览    

        OnlineReport,               //举报
        ConvertAccountInfo,         //转移包信息查询

        Questionnaire,        		//问卷调查

        Count
    }

    public class BaseWebViewParams
    {
        public BaseWebViewType type;
        public string fullUrl;
        public bool needFrameUpdate;
        public bool needGobackBtn;
        public bool needRefreshBtn;
        public List<BaseWebViewMsg> uniWebViewMsgs;

        public BaseWebViewParams()
        {
            Clear();
        }

        public void Clear()
        {
            type = BaseWebViewType.None;
            fullUrl = "";
            needFrameUpdate = false;
            needGobackBtn = false;
            needRefreshBtn = false;
            if (uniWebViewMsgs != null)
            {
                for (int i = 0; i < uniWebViewMsgs.Count; i++)
                {
                    var msg = uniWebViewMsgs[i];
                    msg.scheme = "";
                    msg.path = "";
                    msg.onReceiveWebViewMsg = null;
                }
                uniWebViewMsgs.Clear();
                uniWebViewMsgs = null;
            }
        }
    }

    public struct BaseWebViewMsg
    {
        public string scheme;
        public string path;
        public System.Action<Dictionary<string, string>, UniWebViewUtility> onReceiveWebViewMsg;
    }

    #region Report
    public class InformantInfo
    {
        public string roleId = "";              //被举报者id
        public string roleName = "";
        public string roleLevel = "";
        public string vipLevel = "";
        public string jobId = "";
        public string jobName = "";
    }

    class ReporterInfo
    {
        public string roleId = "";                   //举报者id
        public string roleName = "";
        public string roleLevel = "";
        public string accId = "";
        public string vipLevel = "";
        public string jobId = "";
        public string jobName = "";

        public string serverId = "";
        public string serverName = "";
        public string platformName = "";              //平台名
    }
    #endregion

    #region Convert Pack

    public class ConvertAccountInfo
    {
        public string account;
        public string password;
        public string needScreenShot;           // 1 : 不显示   |   0 ： 显示
        public string channelName;              // 服务器channel名
        public string originalOpenUid;          // 原先的账号ID
        public bool alreadyConverAccount;       //账号是否已经转移
    }

    public class LocalConvertAccInfos
    {
        public List<ConvertAccountInfo> convertAccountInfos = null;
        public LocalConvertAccInfos()
        {
            convertAccountInfos = new List<ConvertAccountInfo>();
        }
    }

    #endregion
}