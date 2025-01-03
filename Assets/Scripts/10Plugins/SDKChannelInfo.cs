
using System.ComponentModel;

namespace SDKClient
{
    public enum PlatformType
    {
        [Description("��׿")]
        Android = 0,
        [Description("ios")]
        IOS = 1,
        [Description("iosԽ��")]
        IOSOther = 2,
        [Description("����")]
        Other = 3,
    }

    //����SDK��Ϣ
    public class SDKChannelInfo
    {
        public SDKChannelInfo()
        {
            channelID = "none";
            channelName = "��������";
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
        /// ����ID   ������ʶ Сд  eg: mgsptiyan
        /// </summary>
        public string channelID;
        /// <summary>
        /// ������   �������� eg: mg�ְ������
        /// </summary>
        public string channelName;
        /// <summary>
        /// �������� ����������
        /// </summary>
        public string channelParam;                         
        /// <summary>
        /// �������� ���������ʶ ��д eg: MG
        /// </summary>
        public string channelType;                          
        /// <summary>
        /// ƽ̨���� eg: Android
        /// </summary>
        public PlatformType platformType;
        /// <summary>
        /// ֧���ص���ַ��ʽ
        /// </summary>
        public string payCallbackUrlFormat;
        /// <summary>
        /// ֧��ʱ��Ҫ��ǰ������������صĵ�½��Ϣ
        /// </summary>
        public bool needPayToken;
        /// <summary>
        /// �Ƿ���Ҫ��֧�����֪ͨ���ͻ���
        /// </summary>
        public bool needPayResultNotify;                 
        /// <summary>
        /// �������б����� �ļ��� eg: serverList.xml
        /// </summary>
        public string serverListName;
        /// <summary>
        /// �Ƿ���ҪEncodeUri Uid
        /// </summary>
        public bool needUriEncodeOpenUid;
        /// <summary>
        /// �Ƿ���Ҫ��ʾ�������а�ť
        /// </summary>
        public bool needShowChannelRankBtn;
        /// <summary>
        /// �Ƿ���Ҫ��������
        /// </summary>
        public bool needLocalNotification;
        /// <summary>
        /// �Ƿ���Ҫ�ֻ���
        /// </summary>
        public bool needBindMobilePhone;                 
    }
}