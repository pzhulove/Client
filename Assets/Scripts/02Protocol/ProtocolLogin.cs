using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  玩家登陆状态
	/// </summary>
	public enum PlayerLoginStatus
	{
		PLS_NONE = 0,
		/// <summary>
		///  每天第一次登录
		/// </summary>
		PLS_FIRST_LOGIN_DAILY = 1,
	}

	public enum AuthIDType
	{
		/// <summary>
		///   未实名
		/// </summary>
		AUTH_NO_ID = 0,
		/// <summary>
		///   未成年
		/// </summary>
		AUTH_NO_ADULT = 1,
		/// <summary>
		///   成年
		/// </summary>
		AUTH_ADULT = 2,
	}

	public enum SysSwitchType
	{
		SST_NONE = 0,
	}

	/// <summary>
	///  登录推送信息
	/// </summary>
	public class LoginPushInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		public byte id;
		/// <summary>
		///  名称
		/// </summary>
		public string name;
		/// <summary>
		///  解锁等级
		/// </summary>
		public UInt16 unlockLevel;
		/// <summary>
		///  图标路径
		/// </summary>
		public string iconPath;
		/// <summary>
		///  链接位置
		/// </summary>
		public string linkInfo;
		/// <summary>
		///  开始时间
		/// </summary>
		public UInt32 startTime;
		/// <summary>
		///  结束时间
		/// </summary>
		public UInt32 endTime;
		/// <summary>
		///  loading预制体路径
		/// </summary>
		public string loadingIconPath;
		/// <summary>
		///  排序序号
		/// </summary>
		public byte sortNum;
		/// <summary>
		///  开启间隔
		/// </summary>
		public string openInterval;
		/// <summary>
		///  关闭间隔
		/// </summary>
		public string closeInterval;
		/// <summary>
		///  是否显示时间
		/// </summary>
		public byte isShowTime;
		/// <summary>
		///  是否设置背景图片原比例大小
		/// </summary>
		public byte isSetNative;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, unlockLevel);
				byte[] iconPathBytes = StringHelper.StringToUTF8Bytes(iconPath);
				BaseDLL.encode_string(buffer, ref pos_, iconPathBytes, (UInt16)(buffer.Length - pos_));
				byte[] linkInfoBytes = StringHelper.StringToUTF8Bytes(linkInfo);
				BaseDLL.encode_string(buffer, ref pos_, linkInfoBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				byte[] loadingIconPathBytes = StringHelper.StringToUTF8Bytes(loadingIconPath);
				BaseDLL.encode_string(buffer, ref pos_, loadingIconPathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, sortNum);
				byte[] openIntervalBytes = StringHelper.StringToUTF8Bytes(openInterval);
				BaseDLL.encode_string(buffer, ref pos_, openIntervalBytes, (UInt16)(buffer.Length - pos_));
				byte[] closeIntervalBytes = StringHelper.StringToUTF8Bytes(closeInterval);
				BaseDLL.encode_string(buffer, ref pos_, closeIntervalBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isShowTime);
				BaseDLL.encode_int8(buffer, ref pos_, isSetNative);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref unlockLevel);
				UInt16 iconPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref iconPathLen);
				byte[] iconPathBytes = new byte[iconPathLen];
				for(int i = 0; i < iconPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref iconPathBytes[i]);
				}
				iconPath = StringHelper.BytesToString(iconPathBytes);
				UInt16 linkInfoLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref linkInfoLen);
				byte[] linkInfoBytes = new byte[linkInfoLen];
				for(int i = 0; i < linkInfoLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref linkInfoBytes[i]);
				}
				linkInfo = StringHelper.BytesToString(linkInfoBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				UInt16 loadingIconPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref loadingIconPathLen);
				byte[] loadingIconPathBytes = new byte[loadingIconPathLen];
				for(int i = 0; i < loadingIconPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref loadingIconPathBytes[i]);
				}
				loadingIconPath = StringHelper.BytesToString(loadingIconPathBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref sortNum);
				UInt16 openIntervalLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref openIntervalLen);
				byte[] openIntervalBytes = new byte[openIntervalLen];
				for(int i = 0; i < openIntervalLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref openIntervalBytes[i]);
				}
				openInterval = StringHelper.BytesToString(openIntervalBytes);
				UInt16 closeIntervalLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref closeIntervalLen);
				byte[] closeIntervalBytes = new byte[closeIntervalLen];
				for(int i = 0; i < closeIntervalLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref closeIntervalBytes[i]);
				}
				closeInterval = StringHelper.BytesToString(closeIntervalBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isShowTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref isSetNative);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, unlockLevel);
				byte[] iconPathBytes = StringHelper.StringToUTF8Bytes(iconPath);
				BaseDLL.encode_string(buffer, ref pos_, iconPathBytes, (UInt16)(buffer.Length - pos_));
				byte[] linkInfoBytes = StringHelper.StringToUTF8Bytes(linkInfo);
				BaseDLL.encode_string(buffer, ref pos_, linkInfoBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				byte[] loadingIconPathBytes = StringHelper.StringToUTF8Bytes(loadingIconPath);
				BaseDLL.encode_string(buffer, ref pos_, loadingIconPathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, sortNum);
				byte[] openIntervalBytes = StringHelper.StringToUTF8Bytes(openInterval);
				BaseDLL.encode_string(buffer, ref pos_, openIntervalBytes, (UInt16)(buffer.Length - pos_));
				byte[] closeIntervalBytes = StringHelper.StringToUTF8Bytes(closeInterval);
				BaseDLL.encode_string(buffer, ref pos_, closeIntervalBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isShowTime);
				BaseDLL.encode_int8(buffer, ref pos_, isSetNative);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint16(buffer, ref pos_, ref unlockLevel);
				UInt16 iconPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref iconPathLen);
				byte[] iconPathBytes = new byte[iconPathLen];
				for(int i = 0; i < iconPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref iconPathBytes[i]);
				}
				iconPath = StringHelper.BytesToString(iconPathBytes);
				UInt16 linkInfoLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref linkInfoLen);
				byte[] linkInfoBytes = new byte[linkInfoLen];
				for(int i = 0; i < linkInfoLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref linkInfoBytes[i]);
				}
				linkInfo = StringHelper.BytesToString(linkInfoBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				UInt16 loadingIconPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref loadingIconPathLen);
				byte[] loadingIconPathBytes = new byte[loadingIconPathLen];
				for(int i = 0; i < loadingIconPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref loadingIconPathBytes[i]);
				}
				loadingIconPath = StringHelper.BytesToString(loadingIconPathBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref sortNum);
				UInt16 openIntervalLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref openIntervalLen);
				byte[] openIntervalBytes = new byte[openIntervalLen];
				for(int i = 0; i < openIntervalLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref openIntervalBytes[i]);
				}
				openInterval = StringHelper.BytesToString(openIntervalBytes);
				UInt16 closeIntervalLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref closeIntervalLen);
				byte[] closeIntervalBytes = new byte[closeIntervalLen];
				for(int i = 0; i < closeIntervalLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref closeIntervalBytes[i]);
				}
				closeInterval = StringHelper.BytesToString(closeIntervalBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isShowTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref isSetNative);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 1;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// unlockLevel
				_len += 2;
				// iconPath
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(iconPath);
					_len += 2 + _strBytes.Length;
				}
				// linkInfo
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(linkInfo);
					_len += 2 + _strBytes.Length;
				}
				// startTime
				_len += 4;
				// endTime
				_len += 4;
				// loadingIconPath
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(loadingIconPath);
					_len += 2 + _strBytes.Length;
				}
				// sortNum
				_len += 1;
				// openInterval
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(openInterval);
					_len += 2 + _strBytes.Length;
				}
				// closeInterval
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(closeInterval);
					_len += 2 + _strBytes.Length;
				}
				// isShowTime
				_len += 1;
				// isSetNative
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class AdminLoginVerifyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 200201;
		public UInt32 Sequence;
		public UInt32 version;
		public string source1;
		public UInt32 append1;
		public string source2;
		public byte append2;
		public byte[] tableMd5 = new byte[16];
		public UInt32 svnVersion;
		public byte[] append3 = new byte[12];
		public string param;
		public byte[] hashValue = new byte[20];
		public UInt32 version1;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, version);
				byte[] source1Bytes = StringHelper.StringToUTF8Bytes(source1);
				BaseDLL.encode_string(buffer, ref pos_, source1Bytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, append1);
				byte[] source2Bytes = StringHelper.StringToUTF8Bytes(source2);
				BaseDLL.encode_string(buffer, ref pos_, source2Bytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, append2);
				for(int i = 0; i < tableMd5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, tableMd5[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, svnVersion);
				for(int i = 0; i < append3.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, append3[i]);
				}
				byte[] paramBytes = StringHelper.StringToUTF8Bytes(param);
				BaseDLL.encode_string(buffer, ref pos_, paramBytes, (UInt16)(buffer.Length - pos_));
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, hashValue[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, version1);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref version);
				UInt16 source1Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref source1Len);
				byte[] source1Bytes = new byte[source1Len];
				for(int i = 0; i < source1Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref source1Bytes[i]);
				}
				source1 = StringHelper.BytesToString(source1Bytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref append1);
				UInt16 source2Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref source2Len);
				byte[] source2Bytes = new byte[source2Len];
				for(int i = 0; i < source2Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref source2Bytes[i]);
				}
				source2 = StringHelper.BytesToString(source2Bytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref append2);
				for(int i = 0; i < tableMd5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref tableMd5[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref svnVersion);
				for(int i = 0; i < append3.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref append3[i]);
				}
				UInt16 paramLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref paramLen);
				byte[] paramBytes = new byte[paramLen];
				for(int i = 0; i < paramLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref paramBytes[i]);
				}
				param = StringHelper.BytesToString(paramBytes);
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref hashValue[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref version1);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, version);
				byte[] source1Bytes = StringHelper.StringToUTF8Bytes(source1);
				BaseDLL.encode_string(buffer, ref pos_, source1Bytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, append1);
				byte[] source2Bytes = StringHelper.StringToUTF8Bytes(source2);
				BaseDLL.encode_string(buffer, ref pos_, source2Bytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, append2);
				for(int i = 0; i < tableMd5.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, tableMd5[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, svnVersion);
				for(int i = 0; i < append3.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, append3[i]);
				}
				byte[] paramBytes = StringHelper.StringToUTF8Bytes(param);
				BaseDLL.encode_string(buffer, ref pos_, paramBytes, (UInt16)(buffer.Length - pos_));
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, hashValue[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, version1);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref version);
				UInt16 source1Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref source1Len);
				byte[] source1Bytes = new byte[source1Len];
				for(int i = 0; i < source1Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref source1Bytes[i]);
				}
				source1 = StringHelper.BytesToString(source1Bytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref append1);
				UInt16 source2Len = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref source2Len);
				byte[] source2Bytes = new byte[source2Len];
				for(int i = 0; i < source2Len; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref source2Bytes[i]);
				}
				source2 = StringHelper.BytesToString(source2Bytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref append2);
				for(int i = 0; i < tableMd5.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref tableMd5[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref svnVersion);
				for(int i = 0; i < append3.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref append3[i]);
				}
				UInt16 paramLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref paramLen);
				byte[] paramBytes = new byte[paramLen];
				for(int i = 0; i < paramLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref paramBytes[i]);
				}
				param = StringHelper.BytesToString(paramBytes);
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref hashValue[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref version1);
			}

			public int getLen()
			{
				int _len = 0;
				// version
				_len += 4;
				// source1
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(source1);
					_len += 2 + _strBytes.Length;
				}
				// append1
				_len += 4;
				// source2
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(source2);
					_len += 2 + _strBytes.Length;
				}
				// append2
				_len += 1;
				// tableMd5
				_len += 1 * tableMd5.Length;
				// svnVersion
				_len += 4;
				// append3
				_len += 1 * append3.Length;
				// param
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(param);
					_len += 2 + _strBytes.Length;
				}
				// hashValue
				_len += 1 * hashValue.Length;
				// version1
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class AdminLoginVerifyRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 200202;
		public UInt32 Sequence;
		/// <summary>
		///  手机绑定的角色ID
		/// </summary>
		public UInt64 phoneBindRoleId;
		public string errMsg;
		public UInt32 accid;
		/// <summary>
		///  目录服务器校验签名
		/// </summary>
		public string dirSig;
		public SockAddr addr = new SockAddr();
		public UInt32 result;
		/// <summary>
		///  录像服务器地址
		/// </summary>
		public string replayAgentAddr;
		/// <summary>
		///  是否协议加密
		/// </summary>
		public byte isEncryptProtocol;
		/// <summary>
		///  是否开启bugly
		/// </summary>
		public byte openBugly;
		/// <summary>
		///  语音标识
		/// </summary>
		public UInt32 voiceFlag;
		/// <summary>
		///  活动排行榜URL
		/// </summary>
		public string activityYearSortListUrl;
		/// <summary>
		///  是否开启新的重连机制
		/// </summary>
		public byte openNewReconnectAlgo;
		/// <summary>
		///  是否开启新的发送帧算法
		/// </summary>
		public byte openNewReportFrameAlgo;
		/// <summary>
		///  网页活动链接
		/// </summary>
		public string webActivityUrl;
		/// <summary>
		///  装备图鉴评论服地址
		/// </summary>
		public string commentServerAddr;
		/// <summary>
		///  服务器id
		/// </summary>
		public UInt32 serverId;
		/// <summary>
		///  绾㈠寘鎺掕閾炬帴
		/// </summary>
		public string redPacketRankUrl;
		/// <summary>
		///  账号转移链接地址
		/// </summary>
		public string convertUrl;
		/// <summary>
		///  举报服务器地址
		/// </summary>
		public string reportUrl;
		/// <summary>
		///  是否使用tcp连接战斗服
		/// </summary>
		public byte battleUseTcp;
		/// <summary>
		///  填写问卷地址
		/// </summary>
		public string writeQuestionnaireUrl;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
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

			public void encode(MapViewStream buffer, ref int pos_)
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

			public void decode(MapViewStream buffer, ref int pos_)
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

			public int getLen()
			{
				int _len = 0;
				// phoneBindRoleId
				_len += 8;
				// errMsg
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(errMsg);
					_len += 2 + _strBytes.Length;
				}
				// accid
				_len += 4;
				// dirSig
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(dirSig);
					_len += 2 + _strBytes.Length;
				}
				// addr
				_len += addr.getLen();
				// result
				_len += 4;
				// replayAgentAddr
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(replayAgentAddr);
					_len += 2 + _strBytes.Length;
				}
				// isEncryptProtocol
				_len += 1;
				// openBugly
				_len += 1;
				// voiceFlag
				_len += 4;
				// activityYearSortListUrl
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(activityYearSortListUrl);
					_len += 2 + _strBytes.Length;
				}
				// openNewReconnectAlgo
				_len += 1;
				// openNewReportFrameAlgo
				_len += 1;
				// webActivityUrl
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(webActivityUrl);
					_len += 2 + _strBytes.Length;
				}
				// commentServerAddr
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(commentServerAddr);
					_len += 2 + _strBytes.Length;
				}
				// serverId
				_len += 4;
				// redPacketRankUrl
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(redPacketRankUrl);
					_len += 2 + _strBytes.Length;
				}
				// convertUrl
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(convertUrl);
					_len += 2 + _strBytes.Length;
				}
				// reportUrl
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(reportUrl);
					_len += 2 + _strBytes.Length;
				}
				// battleUseTcp
				_len += 1;
				// writeQuestionnaireUrl
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(writeQuestionnaireUrl);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateClientLoginReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300203;
		public UInt32 Sequence;
		public UInt32 accid;
		public byte[] hashValue = new byte[20];
		public UInt32 accid1;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, hashValue[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, accid1);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref hashValue[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid1);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, hashValue[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, accid1);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				for(int i = 0; i < hashValue.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref hashValue[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid1);
			}

			public int getLen()
			{
				int _len = 0;
				// accid
				_len += 4;
				// hashValue
				_len += 1 * hashValue.Length;
				// accid1
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateClientLoginRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300204;
		public UInt32 Sequence;
		public UInt32 result;
		public byte hasrole;
		/// <summary>
		///  需要等待的玩家数
		/// </summary>
		public UInt32 waitPlayerNum;
		/// <summary>
		///  服务器开服时间
		/// </summary>
		public UInt32 serverStartTime;
		/// <summary>
		/// 通知老兵回归
		/// </summary>
		public byte notifyVeteranReturn;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, hasrole);
				BaseDLL.encode_uint32(buffer, ref pos_, waitPlayerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, serverStartTime);
				BaseDLL.encode_int8(buffer, ref pos_, notifyVeteranReturn);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasrole);
				BaseDLL.decode_uint32(buffer, ref pos_, ref waitPlayerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref serverStartTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref notifyVeteranReturn);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, hasrole);
				BaseDLL.encode_uint32(buffer, ref pos_, waitPlayerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, serverStartTime);
				BaseDLL.encode_int8(buffer, ref pos_, notifyVeteranReturn);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasrole);
				BaseDLL.decode_uint32(buffer, ref pos_, ref waitPlayerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref serverStartTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref notifyVeteranReturn);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// hasrole
				_len += 1;
				// waitPlayerNum
				_len += 4;
				// serverStartTime
				_len += 4;
				// notifyVeteranReturn
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateSendRoleInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300301;
		public UInt32 Sequence;
		public RoleInfo[] roles = new RoleInfo[0];
		/// <summary>
		///  可预约职业
		/// </summary>
		public byte[] appointmentOccus = new byte[0];
		/// <summary>
		///  已经创建的预约角色数量
		/// </summary>
		public UInt32 appointmentRoleNum;
		/// <summary>
		/// 角色基础栏位
		/// </summary>
		public UInt32 baseRoleField;
		/// <summary>
		/// 可扩展角色栏位
		/// </summary>
		public UInt32 extensibleRoleField;
		/// <summary>
		/// 可扩展角色解锁栏位
		/// </summary>
		public UInt32 unlockedExtensibleRoleField;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roles.Length);
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)appointmentOccus.Length);
				for(int i = 0; i < appointmentOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, appointmentOccus[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, appointmentRoleNum);
				BaseDLL.encode_uint32(buffer, ref pos_, baseRoleField);
				BaseDLL.encode_uint32(buffer, ref pos_, extensibleRoleField);
				BaseDLL.encode_uint32(buffer, ref pos_, unlockedExtensibleRoleField);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 rolesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rolesCnt);
				roles = new RoleInfo[rolesCnt];
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i] = new RoleInfo();
					roles[i].decode(buffer, ref pos_);
				}
				UInt16 appointmentOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref appointmentOccusCnt);
				appointmentOccus = new byte[appointmentOccusCnt];
				for(int i = 0; i < appointmentOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref appointmentOccus[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref appointmentRoleNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref baseRoleField);
				BaseDLL.decode_uint32(buffer, ref pos_, ref extensibleRoleField);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unlockedExtensibleRoleField);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roles.Length);
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)appointmentOccus.Length);
				for(int i = 0; i < appointmentOccus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, appointmentOccus[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, appointmentRoleNum);
				BaseDLL.encode_uint32(buffer, ref pos_, baseRoleField);
				BaseDLL.encode_uint32(buffer, ref pos_, extensibleRoleField);
				BaseDLL.encode_uint32(buffer, ref pos_, unlockedExtensibleRoleField);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 rolesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rolesCnt);
				roles = new RoleInfo[rolesCnt];
				for(int i = 0; i < roles.Length; i++)
				{
					roles[i] = new RoleInfo();
					roles[i].decode(buffer, ref pos_);
				}
				UInt16 appointmentOccusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref appointmentOccusCnt);
				appointmentOccus = new byte[appointmentOccusCnt];
				for(int i = 0; i < appointmentOccus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref appointmentOccus[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref appointmentRoleNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref baseRoleField);
				BaseDLL.decode_uint32(buffer, ref pos_, ref extensibleRoleField);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unlockedExtensibleRoleField);
			}

			public int getLen()
			{
				int _len = 0;
				// roles
				_len += 2;
				for(int j = 0; j < roles.Length; j++)
				{
					_len += roles[j].getLen();
				}
				// appointmentOccus
				_len += 2 + 1 * appointmentOccus.Length;
				// appointmentRoleNum
				_len += 4;
				// baseRoleField
				_len += 4;
				// extensibleRoleField
				_len += 4;
				// unlockedExtensibleRoleField
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateCreateRoleReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300302;
		public UInt32 Sequence;
		public string name;
		public byte sex;
		public byte occupation;
		public byte isnewer;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, sex);
				BaseDLL.encode_int8(buffer, ref pos_, occupation);
				BaseDLL.encode_int8(buffer, ref pos_, isnewer);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref sex);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
				BaseDLL.decode_int8(buffer, ref pos_, ref isnewer);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, sex);
				BaseDLL.encode_int8(buffer, ref pos_, occupation);
				BaseDLL.encode_int8(buffer, ref pos_, isnewer);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref sex);
				BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
				BaseDLL.decode_int8(buffer, ref pos_, ref isnewer);
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// sex
				_len += 1;
				// occupation
				_len += 1;
				// isnewer
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateCreateRoleRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300303;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateDeleteRoleReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300304;
		public UInt32 Sequence;
		public UInt64 roldId;
		public string deviceId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roldId);
				byte[] deviceIdBytes = StringHelper.StringToUTF8Bytes(deviceId);
				BaseDLL.encode_string(buffer, ref pos_, deviceIdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roldId);
				UInt16 deviceIdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIdLen);
				byte[] deviceIdBytes = new byte[deviceIdLen];
				for(int i = 0; i < deviceIdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIdBytes[i]);
				}
				deviceId = StringHelper.BytesToString(deviceIdBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roldId);
				byte[] deviceIdBytes = StringHelper.StringToUTF8Bytes(deviceId);
				BaseDLL.encode_string(buffer, ref pos_, deviceIdBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roldId);
				UInt16 deviceIdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIdLen);
				byte[] deviceIdBytes = new byte[deviceIdLen];
				for(int i = 0; i < deviceIdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref deviceIdBytes[i]);
				}
				deviceId = StringHelper.BytesToString(deviceIdBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// roldId
				_len += 8;
				// deviceId
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(deviceId);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateEnterGameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300306;
		public UInt32 Sequence;
		public UInt64 roleId;
		public byte option;
		public string city;
		public UInt32 inviter;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, option);
				byte[] cityBytes = StringHelper.StringToUTF8Bytes(city);
				BaseDLL.encode_string(buffer, ref pos_, cityBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, inviter);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref option);
				UInt16 cityLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref cityLen);
				byte[] cityBytes = new byte[cityLen];
				for(int i = 0; i < cityLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref cityBytes[i]);
				}
				city = StringHelper.BytesToString(cityBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inviter);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, option);
				byte[] cityBytes = StringHelper.StringToUTF8Bytes(city);
				BaseDLL.encode_string(buffer, ref pos_, cityBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, inviter);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref option);
				UInt16 cityLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref cityLen);
				byte[] cityBytes = new byte[cityLen];
				for(int i = 0; i < cityLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref cityBytes[i]);
				}
				city = StringHelper.BytesToString(cityBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inviter);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// option
				_len += 1;
				// city
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(city);
					_len += 2 + _strBytes.Length;
				}
				// inviter
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateEnterGameRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300307;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  离开游戏
	/// </summary>
	[Protocol]
	public class GateLeaveGameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300401;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateReconnectGameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300311;
		public UInt32 Sequence;
		public UInt32 accid;
		public UInt64 roleId;
		public UInt32 sequence;
		public byte[] session = new byte[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, sequence);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)session.Length);
				for(int i = 0; i < session.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, session[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sequence);
				UInt16 sessionCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref sessionCnt);
				session = new byte[sessionCnt];
				for(int i = 0; i < session.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref session[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, sequence);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)session.Length);
				for(int i = 0; i < session.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, session[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sequence);
				UInt16 sessionCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref sessionCnt);
				session = new byte[sessionCnt];
				for(int i = 0; i < session.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref session[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// accid
				_len += 4;
				// roleId
				_len += 8;
				// sequence
				_len += 4;
				// session
				_len += 2 + 1 * session.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateReconnectGameRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300312;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 lastRecvSequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, lastRecvSequence);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastRecvSequence);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, lastRecvSequence);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastRecvSequence);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// lastRecvSequence
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateRecoverRoleReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300305;
		public UInt32 Sequence;
		public UInt64 roleId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  恢复角色返回
	/// </summary>
	[Protocol]
	public class GateRecoverRoleRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300314;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt32 result;
		public string roleUpdateLimit;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				byte[] roleUpdateLimitBytes = StringHelper.StringToUTF8Bytes(roleUpdateLimit);
				BaseDLL.encode_string(buffer, ref pos_, roleUpdateLimitBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 roleUpdateLimitLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleUpdateLimitLen);
				byte[] roleUpdateLimitBytes = new byte[roleUpdateLimitLen];
				for(int i = 0; i < roleUpdateLimitLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleUpdateLimitBytes[i]);
				}
				roleUpdateLimit = StringHelper.BytesToString(roleUpdateLimitBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				byte[] roleUpdateLimitBytes = StringHelper.StringToUTF8Bytes(roleUpdateLimit);
				BaseDLL.encode_string(buffer, ref pos_, roleUpdateLimitBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 roleUpdateLimitLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleUpdateLimitLen);
				byte[] roleUpdateLimitBytes = new byte[roleUpdateLimitLen];
				for(int i = 0; i < roleUpdateLimitLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleUpdateLimitBytes[i]);
				}
				roleUpdateLimit = StringHelper.BytesToString(roleUpdateLimitBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// result
				_len += 4;
				// roleUpdateLimit
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(roleUpdateLimit);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  删除角色返回
	/// </summary>
	[Protocol]
	public class GateDeleteRoleRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300315;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt32 result;
		public string roleUpdateLimit;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				byte[] roleUpdateLimitBytes = StringHelper.StringToUTF8Bytes(roleUpdateLimit);
				BaseDLL.encode_string(buffer, ref pos_, roleUpdateLimitBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 roleUpdateLimitLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleUpdateLimitLen);
				byte[] roleUpdateLimitBytes = new byte[roleUpdateLimitLen];
				for(int i = 0; i < roleUpdateLimitLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleUpdateLimitBytes[i]);
				}
				roleUpdateLimit = StringHelper.BytesToString(roleUpdateLimitBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				byte[] roleUpdateLimitBytes = StringHelper.StringToUTF8Bytes(roleUpdateLimit);
				BaseDLL.encode_string(buffer, ref pos_, roleUpdateLimitBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 roleUpdateLimitLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleUpdateLimitLen);
				byte[] roleUpdateLimitBytes = new byte[roleUpdateLimitLen];
				for(int i = 0; i < roleUpdateLimitLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleUpdateLimitBytes[i]);
				}
				roleUpdateLimit = StringHelper.BytesToString(roleUpdateLimitBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// result
				_len += 4;
				// roleUpdateLimit
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(roleUpdateLimit);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  更新排队信息
	/// </summary>
	[Protocol]
	public class GateNotifyLoginWaitInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300316;
		public UInt32 Sequence;
		public UInt32 waitPlayerNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, waitPlayerNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref waitPlayerNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, waitPlayerNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref waitPlayerNum);
			}

			public int getLen()
			{
				int _len = 0;
				// waitPlayerNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知玩家可以登录了
	/// </summary>
	[Protocol]
	public class GateNotifyAllowLogin : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300317;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  退出排队
	/// </summary>
	[Protocol]
	public class GateLeaveLoginQueue : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300318;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  角色切换请求
	/// </summary>
	[Protocol]
	public class RoleSwitchReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300319;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  角色切换返回
	/// </summary>
	[Protocol]
	public class RoleSwitchRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300320;
		public UInt32 Sequence;
		public UInt32 result;
		public byte hasrole;
		/// <summary>
		///  需要等待的玩家数
		/// </summary>
		public UInt32 waitPlayerNum;
		/// <summary>
		///  服务器开服时间
		/// </summary>
		public UInt32 serverStartTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, hasrole);
				BaseDLL.encode_uint32(buffer, ref pos_, waitPlayerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, serverStartTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasrole);
				BaseDLL.decode_uint32(buffer, ref pos_, ref waitPlayerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref serverStartTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, hasrole);
				BaseDLL.encode_uint32(buffer, ref pos_, waitPlayerNum);
				BaseDLL.encode_uint32(buffer, ref pos_, serverStartTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasrole);
				BaseDLL.decode_uint32(buffer, ref pos_, ref waitPlayerNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref serverStartTime);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// hasrole
				_len += 1;
				// waitPlayerNum
				_len += 4;
				// serverStartTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  发送登录推送信息
	/// </summary>
	[Protocol]
	public class GateSendLoginPushInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300321;
		public UInt32 Sequence;
		/// <summary>
		///  登录推送信息
		/// </summary>
		public LoginPushInfo[] infos = new LoginPushInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new LoginPushInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new LoginPushInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new LoginPushInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new LoginPushInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// infos
				_len += 2;
				for(int j = 0; j < infos.Length; j++)
				{
					_len += infos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求转移账号信息
	/// </summary>
	[Protocol]
	public class GateConvertAccountReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300322;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回转移账号信息
	/// </summary>
	[Protocol]
	public class GateConvertAccountRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300323;
		public UInt32 Sequence;
		/// <summary>
		///  账号
		/// </summary>
		public string account;
		/// <summary>
		///  密码
		/// </summary>
		public string passwd;
		/// <summary>
		///  是否保存文件
		/// </summary>
		public byte saveFile;
		/// <summary>
		///  是否截图
		/// </summary>
		public byte screenShot;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] accountBytes = StringHelper.StringToUTF8Bytes(account);
				BaseDLL.encode_string(buffer, ref pos_, accountBytes, (UInt16)(buffer.Length - pos_));
				byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
				BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, saveFile);
				BaseDLL.encode_int8(buffer, ref pos_, screenShot);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 accountLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref accountLen);
				byte[] accountBytes = new byte[accountLen];
				for(int i = 0; i < accountLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref accountBytes[i]);
				}
				account = StringHelper.BytesToString(accountBytes);
				UInt16 passwdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
				byte[] passwdBytes = new byte[passwdLen];
				for(int i = 0; i < passwdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
				}
				passwd = StringHelper.BytesToString(passwdBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref saveFile);
				BaseDLL.decode_int8(buffer, ref pos_, ref screenShot);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] accountBytes = StringHelper.StringToUTF8Bytes(account);
				BaseDLL.encode_string(buffer, ref pos_, accountBytes, (UInt16)(buffer.Length - pos_));
				byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
				BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, saveFile);
				BaseDLL.encode_int8(buffer, ref pos_, screenShot);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 accountLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref accountLen);
				byte[] accountBytes = new byte[accountLen];
				for(int i = 0; i < accountLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref accountBytes[i]);
				}
				account = StringHelper.BytesToString(accountBytes);
				UInt16 passwdLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
				byte[] passwdBytes = new byte[passwdLen];
				for(int i = 0; i < passwdLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
				}
				passwd = StringHelper.BytesToString(passwdBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref saveFile);
				BaseDLL.decode_int8(buffer, ref pos_, ref screenShot);
			}

			public int getLen()
			{
				int _len = 0;
				// account
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(account);
					_len += 2 + _strBytes.Length;
				}
				// passwd
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(passwd);
					_len += 2 + _strBytes.Length;
				}
				// saveFile
				_len += 1;
				// screenShot
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步玩家登陆状态
	/// </summary>
	[Protocol]
	public class SyncPlayerLoginStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600308;
		public UInt32 Sequence;
		/// <summary>
		///  对应枚举PlayerLoginStatus
		/// </summary>
		public byte playerLoginStatus;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, playerLoginStatus);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerLoginStatus);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, playerLoginStatus);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerLoginStatus);
			}

			public int getLen()
			{
				int _len = 0;
				// playerLoginStatus
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class GateFinishNewbeeGuide : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300313;
		public UInt32 Sequence;
		public UInt64 roleId;
		public UInt32 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// id
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知客户端被踢
	/// </summary>
	[Protocol]
	public class GateNotifyKickoff : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300404;
		public UInt32 Sequence;
		public UInt32 errorCode;
		public string msg;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
				byte[] msgBytes = StringHelper.StringToUTF8Bytes(msg);
				BaseDLL.encode_string(buffer, ref pos_, msgBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				UInt16 msgLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref msgLen);
				byte[] msgBytes = new byte[msgLen];
				for(int i = 0; i < msgLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref msgBytes[i]);
				}
				msg = StringHelper.BytesToString(msgBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
				byte[] msgBytes = StringHelper.StringToUTF8Bytes(msg);
				BaseDLL.encode_string(buffer, ref pos_, msgBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				UInt16 msgLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref msgLen);
				byte[] msgBytes = new byte[msgLen];
				for(int i = 0; i < msgLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref msgBytes[i]);
				}
				msg = StringHelper.BytesToString(msgBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// errorCode
				_len += 4;
				// msg
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(msg);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步客户端开服时间
	/// </summary>
	[Protocol]
	public class WorldNotifyGameStartTime : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 604401;
		public UInt32 Sequence;
		public UInt32 time;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, time);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, time);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			}

			public int getLen()
			{
				int _len = 0;
				// time
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  新手引导选择请求
	/// </summary>
	[Protocol]
	public class SceneNoviceGuideChooseReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300205;
		public UInt32 Sequence;
		/// <summary>
		///  角色id
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		///  选择标志(对应枚举NoviceGuideChooseFlag)
		/// </summary>
		public byte chooseFlag;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, chooseFlag);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref chooseFlag);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, chooseFlag);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref chooseFlag);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				// chooseFlag
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  开关结构
	/// </summary>
	public class SysSwitchCfg : Protocol.IProtocolStream
	{
		public UInt32 sysType;
		public byte switchValue;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sysType);
				BaseDLL.encode_int8(buffer, ref pos_, switchValue);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sysType);
				BaseDLL.decode_int8(buffer, ref pos_, ref switchValue);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sysType);
				BaseDLL.encode_int8(buffer, ref pos_, switchValue);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sysType);
				BaseDLL.decode_int8(buffer, ref pos_, ref switchValue);
			}

			public int getLen()
			{
				int _len = 0;
				// sysType
				_len += 4;
				// switchValue
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  通知客户端系统开关
	/// </summary>
	[Protocol]
	public class GateNotifySysSwitch : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300324;
		public UInt32 Sequence;
		public SysSwitchCfg[] cfg = new SysSwitchCfg[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)cfg.Length);
				for(int i = 0; i < cfg.Length; i++)
				{
					cfg[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 cfgCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref cfgCnt);
				cfg = new SysSwitchCfg[cfgCnt];
				for(int i = 0; i < cfg.Length; i++)
				{
					cfg[i] = new SysSwitchCfg();
					cfg[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)cfg.Length);
				for(int i = 0; i < cfg.Length; i++)
				{
					cfg[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 cfgCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref cfgCnt);
				cfg = new SysSwitchCfg[cfgCnt];
				for(int i = 0; i < cfg.Length; i++)
				{
					cfg[i] = new SysSwitchCfg();
					cfg[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// cfg
				_len += 2;
				for(int j = 0; j < cfg.Length; j++)
				{
					_len += cfg[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->gate 角色收藏请求
	/// </summary>
	[Protocol]
	public class GateRoleCollectionReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300325;
		public UInt32 Sequence;
		/// <summary>
		/// 角色id
		/// </summary>
		public UInt64 roleId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			}

			public int getLen()
			{
				int _len = 0;
				// roleId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  gate->client 角色收藏返回
	/// </summary>
	[Protocol]
	public class GateRoleCollectionRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 300326;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 result;
		/// <summary>
		/// 角色id
		/// </summary>
		public UInt64 roleId;
		/// <summary>
		/// 收藏状态
		/// </summary>
		public byte isCollection;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, isCollection);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCollection);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, roleId);
				BaseDLL.encode_int8(buffer, ref pos_, isCollection);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCollection);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// roleId
				_len += 8;
				// isCollection
				_len += 1;
				return _len;
			}
		#endregion

	}

}
