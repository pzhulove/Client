
using System.ComponentModel;

namespace SDKClient
{
    public enum PlatformType
    {
        [Description("安卓")]
        Android = 0,
        [Description("ios")]
        IOS = 1,
        [Description("ios越狱")]
        IOSOther = 2,
        [Description("其他")]
        Other = 3,
    }

    //渠道SDK信息
    public class SDKChannelInfo
    {
        public SDKChannelInfo()
        {
            channelID = "none";
            channelName = "内网测试";
            channelParam = "none";
            channelType = "NONE";
            platformType = PlatformType.Android;

            payCallbackUrlFormat = "http://{0}/charge";

            needPayToken = false;
            needPayResultNotify = false;
            serverListName = "serverList.xml";
            needUriEncodeOpenUid = false;
            needShowChannelRankBtn = false;
            needLocalNotification = false;
            needBindMobilePhone = false;
        }
        /// <summary>
        /// 渠道ID   渠道标识 小写  eg: mgsptiyan
        /// </summary>
        public string channelID;
        /// <summary>
        /// 渠道名   中文描述 eg: mg分包体验服
        /// </summary>
        public string channelName;
        /// <summary>
        /// 渠道参数 服务器传参
        /// </summary>
        public string channelParam;                         
        /// <summary>
        /// 渠道类型 渠道打包标识 大写 eg: MG
        /// </summary>
        public string channelType;                          
        /// <summary>
        /// 平台类型 eg: Android
        /// </summary>
        public PlatformType platformType;
        /// <summary>
        /// 支付回调地址格式
        /// </summary>
        public string payCallbackUrlFormat;
        /// <summary>
        /// 支付时需要提前传入服务器返回的登陆信息
        /// </summary>
        public bool needPayToken;
        /// <summary>
        /// 是否需要把支付结果通知给客户端
        /// </summary>
        public bool needPayResultNotify;                 
        /// <summary>
        /// 服务器列表名称 文件名 eg: serverList.xml
        /// </summary>
        public string serverListName;
        /// <summary>
        /// 是否需要EncodeUri Uid
        /// </summary>
        public bool needUriEncodeOpenUid;
        /// <summary>
        /// 是否需要显示渠道排行榜按钮
        /// </summary>
        public bool needShowChannelRankBtn;
        /// <summary>
        /// 是否需要本地推送
        /// </summary>
        public bool needLocalNotification;
        /// <summary>
        /// 是否需要手机绑定
        /// </summary>
        public bool needBindMobilePhone;                 
    }
}