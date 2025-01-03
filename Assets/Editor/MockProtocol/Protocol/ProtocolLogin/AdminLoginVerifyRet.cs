using System;
using System.Text;

namespace Mock.Protocol
{

	public class AdminLoginVerifyRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 200202;
		public UInt32 Sequence;
		/// <summary>
		///  手机绑定的角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 手机绑定的角色ID", " 手机绑定的角色ID")]
		public UInt64 phoneBindRoleId;

		public string errMsg;

		public UInt32 accid;
		/// <summary>
		///  目录服务器校验签名
		/// </summary>
		[AdvancedInspector.Descriptor(" 目录服务器校验签名", " 目录服务器校验签名")]
		public string dirSig;

		public SockAddr addr = null;

		public UInt32 result;
		/// <summary>
		///  录像服务器地址
		/// </summary>
		[AdvancedInspector.Descriptor(" 录像服务器地址", " 录像服务器地址")]
		public string replayAgentAddr;
		/// <summary>
		///  是否协议加密
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否协议加密", " 是否协议加密")]
		public byte isEncryptProtocol;
		/// <summary>
		///  是否开启bugly
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否开启bugly", " 是否开启bugly")]
		public byte openBugly;
		/// <summary>
		///  语音标识
		/// </summary>
		[AdvancedInspector.Descriptor(" 语音标识", " 语音标识")]
		public UInt32 voiceFlag;
		/// <summary>
		///  活动排行榜URL
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动排行榜URL", " 活动排行榜URL")]
		public string activityYearSortListUrl;
		/// <summary>
		///  是否开启新的重连机制
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否开启新的重连机制", " 是否开启新的重连机制")]
		public byte openNewReconnectAlgo;
		/// <summary>
		///  是否开启新的发送帧算法
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否开启新的发送帧算法", " 是否开启新的发送帧算法")]
		public byte openNewReportFrameAlgo;
		/// <summary>
		///  网页活动链接
		/// </summary>
		[AdvancedInspector.Descriptor(" 网页活动链接", " 网页活动链接")]
		public string webActivityUrl;
		/// <summary>
		///  装备图鉴评论服地址
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备图鉴评论服地址", " 装备图鉴评论服地址")]
		public string commentServerAddr;
		/// <summary>
		///  服务器id
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器id", " 服务器id")]
		public UInt32 serverId;
		/// <summary>
		///  绾㈠寘鎺掕閾炬帴
		/// </summary>
		[AdvancedInspector.Descriptor(" 绾㈠寘鎺掕閾炬帴", " 绾㈠寘鎺掕閾炬帴")]
		public string redPacketRankUrl;
		/// <summary>
		///  账号转移链接地址
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号转移链接地址", " 账号转移链接地址")]
		public string convertUrl;
		/// <summary>
		///  举报服务器地址
		/// </summary>
		[AdvancedInspector.Descriptor(" 举报服务器地址", " 举报服务器地址")]
		public string reportUrl;
		/// <summary>
		///  是否使用tcp连接战斗服
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否使用tcp连接战斗服", " 是否使用tcp连接战斗服")]
		public byte battleUseTcp;
		/// <summary>
		///  填写问卷地址
		/// </summary>
		[AdvancedInspector.Descriptor(" 填写问卷地址", " 填写问卷地址")]
		public string writeQuestionnaireUrl;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, phoneBindRoleId);
			byte[] errMsgBytes = StringHelper.StringToUTF8Bytes(errMsg);
			BaseDLL.encode_string(buffer, ref pos_, errMsgBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			byte[] dirSigBytes = StringHelper.StringToUTF8Bytes(dirSig);
			BaseDLL.encode_string(buffer, ref pos_, dirSigBytes, (UInt16)(buffer.Length - pos_));
			addr.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			byte[] replayAgentAddrBytes = StringHelper.StringToUTF8Bytes(replayAgentAddr);
			BaseDLL.encode_string(buffer, ref pos_, replayAgentAddrBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, isEncryptProtocol);
			BaseDLL.encode_int8(buffer, ref pos_, openBugly);
			BaseDLL.encode_uint32(buffer, ref pos_, voiceFlag);
			byte[] activityYearSortListUrlBytes = StringHelper.StringToUTF8Bytes(activityYearSortListUrl);
			BaseDLL.encode_string(buffer, ref pos_, activityYearSortListUrlBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, openNewReconnectAlgo);
			BaseDLL.encode_int8(buffer, ref pos_, openNewReportFrameAlgo);
			byte[] webActivityUrlBytes = StringHelper.StringToUTF8Bytes(webActivityUrl);
			BaseDLL.encode_string(buffer, ref pos_, webActivityUrlBytes, (UInt16)(buffer.Length - pos_));
			byte[] commentServerAddrBytes = StringHelper.StringToUTF8Bytes(commentServerAddr);
			BaseDLL.encode_string(buffer, ref pos_, commentServerAddrBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, serverId);
			byte[] redPacketRankUrlBytes = StringHelper.StringToUTF8Bytes(redPacketRankUrl);
			BaseDLL.encode_string(buffer, ref pos_, redPacketRankUrlBytes, (UInt16)(buffer.Length - pos_));
			byte[] convertUrlBytes = StringHelper.StringToUTF8Bytes(convertUrl);
			BaseDLL.encode_string(buffer, ref pos_, convertUrlBytes, (UInt16)(buffer.Length - pos_));
			byte[] reportUrlBytes = StringHelper.StringToUTF8Bytes(reportUrl);
			BaseDLL.encode_string(buffer, ref pos_, reportUrlBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, battleUseTcp);
			byte[] writeQuestionnaireUrlBytes = StringHelper.StringToUTF8Bytes(writeQuestionnaireUrl);
			BaseDLL.encode_string(buffer, ref pos_, writeQuestionnaireUrlBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref phoneBindRoleId);
			UInt16 errMsgLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref errMsgLen);
			byte[] errMsgBytes = new byte[errMsgLen];
			for(int i = 0; i < errMsgLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref errMsgBytes[i]);
			}
			errMsg = StringHelper.BytesToString(errMsgBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			UInt16 dirSigLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dirSigLen);
			byte[] dirSigBytes = new byte[dirSigLen];
			for(int i = 0; i < dirSigLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref dirSigBytes[i]);
			}
			dirSig = StringHelper.BytesToString(dirSigBytes);
			addr.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 replayAgentAddrLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref replayAgentAddrLen);
			byte[] replayAgentAddrBytes = new byte[replayAgentAddrLen];
			for(int i = 0; i < replayAgentAddrLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref replayAgentAddrBytes[i]);
			}
			replayAgentAddr = StringHelper.BytesToString(replayAgentAddrBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref isEncryptProtocol);
			BaseDLL.decode_int8(buffer, ref pos_, ref openBugly);
			BaseDLL.decode_uint32(buffer, ref pos_, ref voiceFlag);
			UInt16 activityYearSortListUrlLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref activityYearSortListUrlLen);
			byte[] activityYearSortListUrlBytes = new byte[activityYearSortListUrlLen];
			for(int i = 0; i < activityYearSortListUrlLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref activityYearSortListUrlBytes[i]);
			}
			activityYearSortListUrl = StringHelper.BytesToString(activityYearSortListUrlBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref openNewReconnectAlgo);
			BaseDLL.decode_int8(buffer, ref pos_, ref openNewReportFrameAlgo);
			UInt16 webActivityUrlLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref webActivityUrlLen);
			byte[] webActivityUrlBytes = new byte[webActivityUrlLen];
			for(int i = 0; i < webActivityUrlLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref webActivityUrlBytes[i]);
			}
			webActivityUrl = StringHelper.BytesToString(webActivityUrlBytes);
			UInt16 commentServerAddrLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref commentServerAddrLen);
			byte[] commentServerAddrBytes = new byte[commentServerAddrLen];
			for(int i = 0; i < commentServerAddrLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref commentServerAddrBytes[i]);
			}
			commentServerAddr = StringHelper.BytesToString(commentServerAddrBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref serverId);
			UInt16 redPacketRankUrlLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref redPacketRankUrlLen);
			byte[] redPacketRankUrlBytes = new byte[redPacketRankUrlLen];
			for(int i = 0; i < redPacketRankUrlLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref redPacketRankUrlBytes[i]);
			}
			redPacketRankUrl = StringHelper.BytesToString(redPacketRankUrlBytes);
			UInt16 convertUrlLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref convertUrlLen);
			byte[] convertUrlBytes = new byte[convertUrlLen];
			for(int i = 0; i < convertUrlLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref convertUrlBytes[i]);
			}
			convertUrl = StringHelper.BytesToString(convertUrlBytes);
			UInt16 reportUrlLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref reportUrlLen);
			byte[] reportUrlBytes = new byte[reportUrlLen];
			for(int i = 0; i < reportUrlLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref reportUrlBytes[i]);
			}
			reportUrl = StringHelper.BytesToString(reportUrlBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref battleUseTcp);
			UInt16 writeQuestionnaireUrlLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref writeQuestionnaireUrlLen);
			byte[] writeQuestionnaireUrlBytes = new byte[writeQuestionnaireUrlLen];
			for(int i = 0; i < writeQuestionnaireUrlLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref writeQuestionnaireUrlBytes[i]);
			}
			writeQuestionnaireUrl = StringHelper.BytesToString(writeQuestionnaireUrlBytes);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
