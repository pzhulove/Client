using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	/// 查询招募信息
	/// </summary>
	[Protocol]
	public class WorldQueryHireInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601782;
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
	/// 
	/// </summary>
	[Protocol]
	public class WorldQueryHireInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601783;
		public UInt32 Sequence;
		/// <summary>
		/// 身份
		/// </summary>
		public byte identity;
		/// <summary>
		/// 邀请码
		/// </summary>
		public string code;
		/// <summary>
		/// 是否已绑定
		/// </summary>
		public byte isBind;
		/// <summary>
		/// 是否有别人绑定我
		/// </summary>
		public byte isOtherBindMe;

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
				BaseDLL.encode_int8(buffer, ref pos_, identity);
				byte[] codeBytes = StringHelper.StringToUTF8Bytes(code);
				BaseDLL.encode_string(buffer, ref pos_, codeBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isBind);
				BaseDLL.encode_int8(buffer, ref pos_, isOtherBindMe);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref identity);
				UInt16 codeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref codeLen);
				byte[] codeBytes = new byte[codeLen];
				for(int i = 0; i < codeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref codeBytes[i]);
				}
				code = StringHelper.BytesToString(codeBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isBind);
				BaseDLL.decode_int8(buffer, ref pos_, ref isOtherBindMe);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, identity);
				byte[] codeBytes = StringHelper.StringToUTF8Bytes(code);
				BaseDLL.encode_string(buffer, ref pos_, codeBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isBind);
				BaseDLL.encode_int8(buffer, ref pos_, isOtherBindMe);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref identity);
				UInt16 codeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref codeLen);
				byte[] codeBytes = new byte[codeLen];
				for(int i = 0; i < codeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref codeBytes[i]);
				}
				code = StringHelper.BytesToString(codeBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isBind);
				BaseDLL.decode_int8(buffer, ref pos_, ref isOtherBindMe);
			}

			public int getLen()
			{
				int _len = 0;
				// identity
				_len += 1;
				// code
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(code);
					_len += 2 + _strBytes.Length;
				}
				// isBind
				_len += 1;
				// isOtherBindMe
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 使用邀请码
	/// </summary>
	[Protocol]
	public class WorldUseHireCodeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601784;
		public UInt32 Sequence;
		/// <summary>
		/// 邀请码
		/// </summary>
		public string code;

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
				byte[] codeBytes = StringHelper.StringToUTF8Bytes(code);
				BaseDLL.encode_string(buffer, ref pos_, codeBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 codeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref codeLen);
				byte[] codeBytes = new byte[codeLen];
				for(int i = 0; i < codeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref codeBytes[i]);
				}
				code = StringHelper.BytesToString(codeBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] codeBytes = StringHelper.StringToUTF8Bytes(code);
				BaseDLL.encode_string(buffer, ref pos_, codeBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 codeLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref codeLen);
				byte[] codeBytes = new byte[codeLen];
				for(int i = 0; i < codeLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref codeBytes[i]);
				}
				code = StringHelper.BytesToString(codeBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(code);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 
	/// </summary>
	[Protocol]
	public class WorldUseHireCodeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601785;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 errorCode;

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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public int getLen()
			{
				int _len = 0;
				// errorCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 招募任务完成情况
	/// </summary>
	[Protocol]
	public class WorldQueryTaskStatusReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601786;
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

	public class HireInfoData : Protocol.IProtocolStream
	{
		public UInt32 taskID;
		public UInt32 cnt;
		public byte status;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_uint32(buffer, ref pos_, cnt);
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cnt);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskID);
				BaseDLL.encode_uint32(buffer, ref pos_, cnt);
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cnt);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public int getLen()
			{
				int _len = 0;
				// taskID
				_len += 4;
				// cnt
				_len += 4;
				// status
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 
	/// </summary>
	[Protocol]
	public class WorldQueryTaskStatusRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601787;
		public UInt32 Sequence;
		/// <summary>
		/// 招募任务情况
		/// </summary>
		public HireInfoData[] hireTaskInfoList = new HireInfoData[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)hireTaskInfoList.Length);
				for(int i = 0; i < hireTaskInfoList.Length; i++)
				{
					hireTaskInfoList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 hireTaskInfoListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref hireTaskInfoListCnt);
				hireTaskInfoList = new HireInfoData[hireTaskInfoListCnt];
				for(int i = 0; i < hireTaskInfoList.Length; i++)
				{
					hireTaskInfoList[i] = new HireInfoData();
					hireTaskInfoList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)hireTaskInfoList.Length);
				for(int i = 0; i < hireTaskInfoList.Length; i++)
				{
					hireTaskInfoList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 hireTaskInfoListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref hireTaskInfoListCnt);
				hireTaskInfoList = new HireInfoData[hireTaskInfoListCnt];
				for(int i = 0; i < hireTaskInfoList.Length; i++)
				{
					hireTaskInfoList[i] = new HireInfoData();
					hireTaskInfoList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// hireTaskInfoList
				_len += 2;
				for(int j = 0; j < hireTaskInfoList.Length; j++)
				{
					_len += hireTaskInfoList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 指定id的任务完成名单
	/// </summary>
	[Protocol]
	public class WorldQueryHireTaskAccidListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601788;
		public UInt32 Sequence;
		public UInt32 taskId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 
	/// </summary>
	[Protocol]
	public class WorldQueryHireTaskAccidListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601789;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public string[] nameList = new string[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)nameList.Length);
				for(int i = 0; i < nameList.Length; i++)
				{
					byte[] nameListBytes = StringHelper.StringToUTF8Bytes(nameList[i]);
					BaseDLL.encode_string(buffer, ref pos_, nameListBytes, (UInt16)(buffer.Length - pos_));
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 nameListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameListCnt);
				nameList = new string[nameListCnt];
				for(int i = 0; i < nameList.Length; i++)
				{
					UInt16 nameListLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref nameListLen);
					byte[] nameListBytes = new byte[nameListLen];
					for(int j = 0; j < nameListLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref nameListBytes[j]);
					}
					nameList[i] = StringHelper.BytesToString(nameListBytes);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)nameList.Length);
				for(int i = 0; i < nameList.Length; i++)
				{
					byte[] nameListBytes = StringHelper.StringToUTF8Bytes(nameList[i]);
					BaseDLL.encode_string(buffer, ref pos_, nameListBytes, (UInt16)(buffer.Length - pos_));
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 nameListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameListCnt);
				nameList = new string[nameListCnt];
				for(int i = 0; i < nameList.Length; i++)
				{
					UInt16 nameListLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos_, ref nameListLen);
					byte[] nameListBytes = new byte[nameListLen];
					for(int j = 0; j < nameListLen; j++)
					{
						BaseDLL.decode_int8(buffer, ref pos_, ref nameListBytes[j]);
					}
					nameList[i] = StringHelper.BytesToString(nameListBytes);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// nameList
				_len += 2;
				for(int j = 0; j < nameList.Length; j++)
				{
					{
						byte[] _strBytes = StringHelper.StringToUTF8Bytes(nameList[j]);
						_len += 2 + _strBytes.Length;
					}
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  客户端通信的招募玩家信息
	/// </summary>
	public class HirePlayerData : Protocol.IProtocolStream
	{
		/// <summary>
		/// 角色id
		/// </summary>
		public UInt64 userId;
		/// <summary>
		/// 姓名
		/// </summary>
		public string name;
		/// <summary>
		/// 职业
		/// </summary>
		public byte occu;
		/// <summary>
		/// 在线状态
		/// </summary>
		public byte online;
		/// <summary>
		///  等级
		/// </summary>
		public UInt32 lv;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, userId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, online);
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref userId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref online);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, userId);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_int8(buffer, ref pos_, online);
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref userId);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_int8(buffer, ref pos_, ref online);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public int getLen()
			{
				int _len = 0;
				// userId
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// occu
				_len += 1;
				// online
				_len += 1;
				// lv
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询招募列表
	/// </summary>
	[Protocol]
	public class WorldQueryHireListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601790;
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
	/// 
	/// </summary>
	[Protocol]
	public class WorldQueryHireListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601791;
		public UInt32 Sequence;
		/// <summary>
		/// 招募列表玩家信息
		/// </summary>
		public HirePlayerData[] hireList = new HirePlayerData[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)hireList.Length);
				for(int i = 0; i < hireList.Length; i++)
				{
					hireList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 hireListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref hireListCnt);
				hireList = new HirePlayerData[hireListCnt];
				for(int i = 0; i < hireList.Length; i++)
				{
					hireList[i] = new HirePlayerData();
					hireList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)hireList.Length);
				for(int i = 0; i < hireList.Length; i++)
				{
					hireList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 hireListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref hireListCnt);
				hireList = new HirePlayerData[hireListCnt];
				for(int i = 0; i < hireList.Length; i++)
				{
					hireList[i] = new HirePlayerData();
					hireList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// hireList
				_len += 2;
				for(int j = 0; j < hireList.Length; j++)
				{
					_len += hireList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 提交招募任务
	/// </summary>
	[Protocol]
	public class WorldSubmitHireTaskReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601792;
		public UInt32 Sequence;
		public UInt32 taskId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 
	/// </summary>
	[Protocol]
	public class WorldSubmitHireTaskRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601793;
		public UInt32 Sequence;
		public UInt32 taskId;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 errorCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId);
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public int getLen()
			{
				int _len = 0;
				// taskId
				_len += 4;
				// errorCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询招募硬币
	/// </summary>
	[Protocol]
	public class WorldQueryHireCoinReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601795;
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
	/// 查询或设置招募是否已推送
	/// </summary>
	[Protocol]
	public class WorldQueryHirePushReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601796;
		public UInt32 Sequence;
		/// <summary>
		/// 0 是查询 1是设置
		/// </summary>
		public byte type;

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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 设置招募红点
	/// </summary>
	[Protocol]
	public class SceneQueryHireRedPointReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501729;
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
	/// 查询有没有在别的服绑定
	/// </summary>
	[Protocol]
	public class WorldQueryHireAlreadyBindReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601797;
		public UInt32 Sequence;
		public string platform;
		public UInt32 accid;
		public UInt32 zone;

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
				byte[] platformBytes = StringHelper.StringToUTF8Bytes(platform);
				BaseDLL.encode_string(buffer, ref pos_, platformBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				BaseDLL.encode_uint32(buffer, ref pos_, zone);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 platformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref platformLen);
				byte[] platformBytes = new byte[platformLen];
				for(int i = 0; i < platformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref platformBytes[i]);
				}
				platform = StringHelper.BytesToString(platformBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zone);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] platformBytes = StringHelper.StringToUTF8Bytes(platform);
				BaseDLL.encode_string(buffer, ref pos_, platformBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				BaseDLL.encode_uint32(buffer, ref pos_, zone);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 platformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref platformLen);
				byte[] platformBytes = new byte[platformLen];
				for(int i = 0; i < platformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref platformBytes[i]);
				}
				platform = StringHelper.BytesToString(platformBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zone);
			}

			public int getLen()
			{
				int _len = 0;
				// platform
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(platform);
					_len += 2 + _strBytes.Length;
				}
				// accid
				_len += 4;
				// zone
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询有没有在别的服绑定返回
	/// </summary>
	[Protocol]
	public class WorldQueryHireAlreadyBindRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601798;
		public UInt32 Sequence;
		public UInt32 errorCode;
		public UInt32 accid;
		public UInt32 zone;

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
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				BaseDLL.encode_uint32(buffer, ref pos_, zone);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zone);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
				BaseDLL.encode_uint32(buffer, ref pos_, accid);
				BaseDLL.encode_uint32(buffer, ref pos_, zone);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref zone);
			}

			public int getLen()
			{
				int _len = 0;
				// errorCode
				_len += 4;
				// accid
				_len += 4;
				// zone
				_len += 4;
				return _len;
			}
		#endregion

	}

}
