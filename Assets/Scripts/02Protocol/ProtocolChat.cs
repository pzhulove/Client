using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  聊天标记
	/// </summary>
	public enum ChatMask
	{
		/// <summary>
		///  红包信息
		/// </summary>
		CHAT_MASK_RED_PACKET = 1,
		/// <summary>
		///  添加好友
		/// </summary>
		CHAT_MASK_ADD_FRIEND = 2,
	}

	/// <summary>
	///  自定义日志上报类型
	/// </summary>
	public enum CustomLogReportType
	{
		/// <summary>
		///  非法
		/// </summary>
		CLRT_INVALID = 0,
		/// <summary>
		///  加入房间
		/// </summary>
		CLRT_JOIN_VOICE_ROOM = 1,
		/// <summary>
		///  退出房间
		/// </summary>
		CLRT_QUIT_VOICE_ROOM = 2,
		/// <summary>
		///  发送录音
		/// </summary>
		CLRT_SEND_RECORD_VOICE = 3,
		/// <summary>
		///  下载录音
		/// </summary>
		CLRT_LOAD_RECORD_VOICE = 4,
	}

	[Protocol]
	public class SceneChat : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500801;
		public UInt32 Sequence;
		public byte channel;
		public UInt64 targetId;
		public string word;
		public byte bLink;
		public string voiceKey;
		public byte voiceDuration;
		public byte isShare;

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
				BaseDLL.encode_int8(buffer, ref pos_, channel);
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, bLink);
				byte[] voiceKeyBytes = StringHelper.StringToUTF8Bytes(voiceKey);
				BaseDLL.encode_string(buffer, ref pos_, voiceKeyBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, voiceDuration);
				BaseDLL.encode_int8(buffer, ref pos_, isShare);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref channel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref bLink);
				UInt16 voiceKeyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref voiceKeyLen);
				byte[] voiceKeyBytes = new byte[voiceKeyLen];
				for(int i = 0; i < voiceKeyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref voiceKeyBytes[i]);
				}
				voiceKey = StringHelper.BytesToString(voiceKeyBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref voiceDuration);
				BaseDLL.decode_int8(buffer, ref pos_, ref isShare);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, channel);
				BaseDLL.encode_uint64(buffer, ref pos_, targetId);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, bLink);
				byte[] voiceKeyBytes = StringHelper.StringToUTF8Bytes(voiceKey);
				BaseDLL.encode_string(buffer, ref pos_, voiceKeyBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, voiceDuration);
				BaseDLL.encode_int8(buffer, ref pos_, isShare);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref channel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref bLink);
				UInt16 voiceKeyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref voiceKeyLen);
				byte[] voiceKeyBytes = new byte[voiceKeyLen];
				for(int i = 0; i < voiceKeyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref voiceKeyBytes[i]);
				}
				voiceKey = StringHelper.BytesToString(voiceKeyBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref voiceDuration);
				BaseDLL.decode_int8(buffer, ref pos_, ref isShare);
			}

			public int getLen()
			{
				int _len = 0;
				// channel
				_len += 1;
				// targetId
				_len += 8;
				// word
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(word);
					_len += 2 + _strBytes.Length;
				}
				// bLink
				_len += 1;
				// voiceKey
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(voiceKey);
					_len += 2 + _strBytes.Length;
				}
				// voiceDuration
				_len += 1;
				// isShare
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyExecGmcmd : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500802;
		public UInt32 Sequence;
		public byte suc;

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
				BaseDLL.encode_int8(buffer, ref pos_, suc);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref suc);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, suc);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref suc);
			}

			public int getLen()
			{
				int _len = 0;
				// suc
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  系统提示, 服务器主动发出
	/// </summary>
	[Protocol]
	public class SysNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 11;
		public UInt32 Sequence;
		public UInt16 type;
		public string word;

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
				BaseDLL.encode_uint16(buffer, ref pos_, type);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref type);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, type);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref type);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 2;
				// word
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(word);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  系统公告, 服务器主动发出
	/// </summary>
	[Protocol]
	public class SysAnnouncement : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 25;
		public UInt32 Sequence;
		public UInt32 id;
		public string word;

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
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// word
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(word);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步聊天
	/// </summary>
	[Protocol]
	public class SceneSyncChat : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500803;
		public UInt32 Sequence;
		public byte channel;
		public UInt64 objid;
		public byte sex;
		public byte occu;
		public UInt16 level;
		public byte viplvl;
		public string objname;
		public UInt64 receiverId;
		public string word;
		public byte bLink;
		public byte isGm;
		public string voiceKey;
		public byte voiceDuration;
		public UInt32 mask;
		public UInt32 headFrame;
		public UInt32 zoneId;

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
				BaseDLL.encode_int8(buffer, ref pos_, channel);
				BaseDLL.encode_uint64(buffer, ref pos_, objid);
				BaseDLL.encode_int8(buffer, ref pos_, sex);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, viplvl);
				byte[] objnameBytes = StringHelper.StringToUTF8Bytes(objname);
				BaseDLL.encode_string(buffer, ref pos_, objnameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, receiverId);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, bLink);
				BaseDLL.encode_int8(buffer, ref pos_, isGm);
				byte[] voiceKeyBytes = StringHelper.StringToUTF8Bytes(voiceKey);
				BaseDLL.encode_string(buffer, ref pos_, voiceKeyBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, voiceDuration);
				BaseDLL.encode_uint32(buffer, ref pos_, mask);
				BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref channel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref objid);
				BaseDLL.decode_int8(buffer, ref pos_, ref sex);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref viplvl);
				UInt16 objnameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref objnameLen);
				byte[] objnameBytes = new byte[objnameLen];
				for(int i = 0; i < objnameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref objnameBytes[i]);
				}
				objname = StringHelper.BytesToString(objnameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref receiverId);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref bLink);
				BaseDLL.decode_int8(buffer, ref pos_, ref isGm);
				UInt16 voiceKeyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref voiceKeyLen);
				byte[] voiceKeyBytes = new byte[voiceKeyLen];
				for(int i = 0; i < voiceKeyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref voiceKeyBytes[i]);
				}
				voiceKey = StringHelper.BytesToString(voiceKeyBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref voiceDuration);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mask);
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, channel);
				BaseDLL.encode_uint64(buffer, ref pos_, objid);
				BaseDLL.encode_int8(buffer, ref pos_, sex);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, viplvl);
				byte[] objnameBytes = StringHelper.StringToUTF8Bytes(objname);
				BaseDLL.encode_string(buffer, ref pos_, objnameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, receiverId);
				byte[] wordBytes = StringHelper.StringToUTF8Bytes(word);
				BaseDLL.encode_string(buffer, ref pos_, wordBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, bLink);
				BaseDLL.encode_int8(buffer, ref pos_, isGm);
				byte[] voiceKeyBytes = StringHelper.StringToUTF8Bytes(voiceKey);
				BaseDLL.encode_string(buffer, ref pos_, voiceKeyBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, voiceDuration);
				BaseDLL.encode_uint32(buffer, ref pos_, mask);
				BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref channel);
				BaseDLL.decode_uint64(buffer, ref pos_, ref objid);
				BaseDLL.decode_int8(buffer, ref pos_, ref sex);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref viplvl);
				UInt16 objnameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref objnameLen);
				byte[] objnameBytes = new byte[objnameLen];
				for(int i = 0; i < objnameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref objnameBytes[i]);
				}
				objname = StringHelper.BytesToString(objnameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref receiverId);
				UInt16 wordLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref wordLen);
				byte[] wordBytes = new byte[wordLen];
				for(int i = 0; i < wordLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref wordBytes[i]);
				}
				word = StringHelper.BytesToString(wordBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref bLink);
				BaseDLL.decode_int8(buffer, ref pos_, ref isGm);
				UInt16 voiceKeyLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref voiceKeyLen);
				byte[] voiceKeyBytes = new byte[voiceKeyLen];
				for(int i = 0; i < voiceKeyLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref voiceKeyBytes[i]);
				}
				voiceKey = StringHelper.BytesToString(voiceKeyBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref voiceDuration);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mask);
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			}

			public int getLen()
			{
				int _len = 0;
				// channel
				_len += 1;
				// objid
				_len += 8;
				// sex
				_len += 1;
				// occu
				_len += 1;
				// level
				_len += 2;
				// viplvl
				_len += 1;
				// objname
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(objname);
					_len += 2 + _strBytes.Length;
				}
				// receiverId
				_len += 8;
				// word
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(word);
					_len += 2 + _strBytes.Length;
				}
				// bLink
				_len += 1;
				// isGm
				_len += 1;
				// voiceKey
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(voiceKey);
					_len += 2 + _strBytes.Length;
				}
				// voiceDuration
				_len += 1;
				// mask
				_len += 4;
				// headFrame
				_len += 4;
				// zoneId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求聊天链接信息
	/// </summary>
	[Protocol]
	public class WorldChatLinkDataReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600802;
		public UInt32 Sequence;
		public byte type;
		public UInt64 uid;
		public UInt32 queryType;
		public UInt32 zoneId;

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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, queryType);
				BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// uid
				_len += 8;
				// queryType
				_len += 4;
				// zoneId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求聊天链接信息返回
	/// </summary>
	[Protocol]
	public class WorldChatLinkDataRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600803;
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
	///  请求发送喇叭
	/// </summary>
	[Protocol]
	public class SceneChatHornReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500808;
		public UInt32 Sequence;
		/// <summary>
		///  喇叭内容
		/// </summary>
		public string content;
		/// <summary>
		///  一次性发送的喇叭数量
		/// </summary>
		public byte num;

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
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// content
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(content);
					_len += 2 + _strBytes.Length;
				}
				// num
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回发送喇叭结果
	/// </summary>
	[Protocol]
	public class SceneChatHornRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500809;
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
	/// 喇叭信息
	/// </summary>
	public class HornInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// 角色id
		/// </summary>
		public UInt64 roldId;
		/// <summary>
		/// 名字
		/// </summary>
		public string name;
		/// <summary>
		/// 职业
		/// </summary>
		public byte occu;
		/// <summary>
		/// 等级
		/// </summary>
		public UInt16 level;
		/// <summary>
		/// vip等级
		/// </summary>
		public byte viplvl;
		/// <summary>
		///  内容
		/// </summary>
		public string content;
		/// <summary>
		///  保护时间
		/// </summary>
		public byte minTime;
		/// <summary>
		///  持续时间
		/// </summary>
		public byte maxTime;
		/// <summary>
		///  combo数
		/// </summary>
		public UInt16 combo;
		/// <summary>
		///  连发数量
		/// </summary>
		public byte num;
		/// <summary>
		///  头像框
		/// </summary>
		public UInt32 headFrame;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roldId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, viplvl);
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, minTime);
				BaseDLL.encode_int8(buffer, ref pos_, maxTime);
				BaseDLL.encode_uint16(buffer, ref pos_, combo);
				BaseDLL.encode_int8(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roldId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref viplvl);
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref minTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxTime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref combo);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roldId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, level);
				BaseDLL.encode_int8(buffer, ref pos_, viplvl);
				byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
				BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, minTime);
				BaseDLL.encode_int8(buffer, ref pos_, maxTime);
				BaseDLL.encode_uint16(buffer, ref pos_, combo);
				BaseDLL.encode_int8(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roldId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref level);
				BaseDLL.decode_int8(buffer, ref pos_, ref viplvl);
				UInt16 contentLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
				byte[] contentBytes = new byte[contentLen];
				for(int i = 0; i < contentLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
				}
				content = StringHelper.BytesToString(contentBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref minTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxTime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref combo);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
			}

			public int getLen()
			{
				int _len = 0;
				// roldId
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 1;
				// level
				_len += 2;
				// viplvl
				_len += 1;
				// content
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(content);
					_len += 2 + _strBytes.Length;
				}
				// minTime
				_len += 1;
				// maxTime
				_len += 1;
				// combo
				_len += 2;
				// num
				_len += 1;
				// headFrame
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  广播喇叭给客户端
	/// </summary>
	[Protocol]
	public class WorldChatHorn : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600815;
		public UInt32 Sequence;
		/// <summary>
		///  喇叭信息
		/// </summary>
		public HornInfo info = new HornInfo();

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
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求客服系统签名
	/// </summary>
	[Protocol]
	public class WorldCustomServiceSignReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600816;
		public UInt32 Sequence;
		/// <summary>
		///  加密信息
		/// </summary>
		public string info;

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
				byte[] infoBytes = StringHelper.StringToUTF8Bytes(info);
				BaseDLL.encode_string(buffer, ref pos_, infoBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 infoLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoLen);
				byte[] infoBytes = new byte[infoLen];
				for(int i = 0; i < infoLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref infoBytes[i]);
				}
				info = StringHelper.BytesToString(infoBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] infoBytes = StringHelper.StringToUTF8Bytes(info);
				BaseDLL.encode_string(buffer, ref pos_, infoBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 infoLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoLen);
				byte[] infoBytes = new byte[infoLen];
				for(int i = 0; i < infoLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref infoBytes[i]);
				}
				info = StringHelper.BytesToString(infoBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// info
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(info);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  客服系统签名返回
	/// </summary>
	[Protocol]
	public class WorldCustomServiceSignRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600817;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  签名
		/// </summary>
		public string sign;

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
				byte[] signBytes = StringHelper.StringToUTF8Bytes(sign);
				BaseDLL.encode_string(buffer, ref pos_, signBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 signLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref signLen);
				byte[] signBytes = new byte[signLen];
				for(int i = 0; i < signLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref signBytes[i]);
				}
				sign = StringHelper.BytesToString(signBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				byte[] signBytes = StringHelper.StringToUTF8Bytes(sign);
				BaseDLL.encode_string(buffer, ref pos_, signBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 signLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref signLen);
				byte[] signBytes = new byte[signLen];
				for(int i = 0; i < signLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref signBytes[i]);
				}
				sign = StringHelper.BytesToString(signBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// sign
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(sign);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  自定义日志上报
	/// </summary>
	[Protocol]
	public class SceneCustomLogReport : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503402;
		public UInt32 Sequence;
		/// <summary>
		///  对应枚举 CustomLogReportType
		/// </summary>
		public byte type;
		/// <summary>
		///  变量
		/// </summary>
		public string param;

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
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] paramBytes = StringHelper.StringToUTF8Bytes(param);
				BaseDLL.encode_string(buffer, ref pos_, paramBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 paramLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref paramLen);
				byte[] paramBytes = new byte[paramLen];
				for(int i = 0; i < paramLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref paramBytes[i]);
				}
				param = StringHelper.BytesToString(paramBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				byte[] paramBytes = StringHelper.StringToUTF8Bytes(param);
				BaseDLL.encode_string(buffer, ref pos_, paramBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 paramLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref paramLen);
				byte[] paramBytes = new byte[paramLen];
				for(int i = 0; i < paramLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref paramBytes[i]);
				}
				param = StringHelper.BytesToString(paramBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// param
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(param);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

}
