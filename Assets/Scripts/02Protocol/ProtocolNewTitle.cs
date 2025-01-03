using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	/// 称谓名称类型
	/// </summary>
	public enum NewTitleType
	{
		TLETP_NONE = 0,
		/// <summary>
		/// 固定
		/// </summary>
		TLETP_FIXED = 1,
		/// <summary>
		/// 服务器拼接
		/// </summary>
		TLETP_JOINT = 2,
		TLETP_MAX = 3,
	}

	/// <summary>
	/// 称谓信息
	/// </summary>
	public class PlayerTitleInfo : Protocol.IProtocolStream
	{
		public UInt64 guid;
		/// <summary>
		/// 唯一id
		/// </summary>
		public UInt32 createTime;
		/// <summary>
		/// 创建时间(获取时间)
		/// </summary>
		public UInt32 titleId;
		/// <summary>
		/// 表id
		/// </summary>
		public byte type;
		/// <summary>
		/// 类型
		/// </summary>
		public UInt32 duetime;
		/// <summary>
		/// 到期时间,0永久
		/// </summary>
		public string name;
		/// <summary>
		/// 头衔名称
		/// </summary>
		public byte style;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, duetime);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, style);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duetime);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref style);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, duetime);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, style);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duetime);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref style);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// createTime
				_len += 4;
				// titleId
				_len += 4;
				// type
				_len += 1;
				// duetime
				_len += 4;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// style
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 头衔风格
	/// </summary>
	/// <summary>
	/// 穿戴的称谓信息
	/// </summary>
	public class PlayerWearedTitleInfo : Protocol.IProtocolStream
	{
		public UInt32 titleId;
		/// <summary>
		/// 表id
		/// </summary>
		public byte style;
		/// <summary>
		/// 风格
		/// </summary>
		public string name;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
				BaseDLL.encode_int8(buffer, ref pos_, style);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref style);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
				BaseDLL.encode_int8(buffer, ref pos_, style);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
				BaseDLL.decode_int8(buffer, ref pos_, ref style);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// titleId
				_len += 4;
				// style
				_len += 1;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 头衔名称
	/// </summary>
	/// <summary>
	/// world->client 同步头衔,上线
	/// </summary>
	[Protocol]
	public class WorldGetPlayerTitleSyncList : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609201;
		public UInt32 Sequence;
		public PlayerTitleInfo[] titles = new PlayerTitleInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)titles.Length);
				for(int i = 0; i < titles.Length; i++)
				{
					titles[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 titlesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref titlesCnt);
				titles = new PlayerTitleInfo[titlesCnt];
				for(int i = 0; i < titles.Length; i++)
				{
					titles[i] = new PlayerTitleInfo();
					titles[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)titles.Length);
				for(int i = 0; i < titles.Length; i++)
				{
					titles[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 titlesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref titlesCnt);
				titles = new PlayerTitleInfo[titlesCnt];
				for(int i = 0; i < titles.Length; i++)
				{
					titles[i] = new PlayerTitleInfo();
					titles[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// titles
				_len += 2;
				for(int j = 0; j < titles.Length; j++)
				{
					_len += titles[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 称谓数据
	/// </summary>
	/// <summary>
	/// client->world 头衔穿戴请求
	/// </summary>
	[Protocol]
	public class WorldNewTitleTakeUpReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609202;
		public UInt32 Sequence;
		public UInt64 titleGuid;
		/// <summary>
		/// 唯一id
		/// </summary>
		public UInt32 titleId;

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
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
			}

			public int getLen()
			{
				int _len = 0;
				// titleGuid
				_len += 8;
				// titleId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 表id
	/// </summary>
	/// <summary>
	/// world->client 头衔穿戴返回
	/// </summary>
	[Protocol]
	public class WorldNewTitleTakeUpRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609203;
		public UInt32 Sequence;
		public UInt32 res;
		/// <summary>
		/// 结果,0成功
		/// </summary>
		public UInt64 titleGuid;

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
				BaseDLL.encode_uint32(buffer, ref pos_, res);
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref res);
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, res);
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref res);
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// res
				_len += 4;
				// titleGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 唯一id
	/// </summary>
	/// <summary>
	/// client->world 头衔脱掉请求
	/// </summary>
	[Protocol]
	public class WorldNewTitleTakeOffReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609204;
		public UInt32 Sequence;
		public UInt64 titleGuid;
		/// <summary>
		/// 唯一id
		/// </summary>
		public UInt32 titleId;

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
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, titleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
			}

			public int getLen()
			{
				int _len = 0;
				// titleGuid
				_len += 8;
				// titleId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 表id
	/// </summary>
	/// <summary>
	/// world->client 头衔脱掉返回
	/// </summary>
	[Protocol]
	public class WorldNewTitleTakeOffRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609205;
		public UInt32 Sequence;
		public UInt32 res;
		/// <summary>
		/// 结果,0成功
		/// </summary>
		public UInt64 titleGuid;

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
				BaseDLL.encode_uint32(buffer, ref pos_, res);
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref res);
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, res);
				BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref res);
				BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// res
				_len += 4;
				// titleGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 唯一id
	/// </summary>
	/// <summary>
	/// world->client 同步新增或删除头衔
	/// </summary>
	[Protocol]
	public class WorldNewTitleSyncUpdate : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609206;
		public UInt32 Sequence;
		public PlayerTitleInfo[] adds = new PlayerTitleInfo[0];
		/// <summary>
		/// 新增
		/// </summary>
		public UInt64[] dels = new UInt64[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)adds.Length);
				for(int i = 0; i < adds.Length; i++)
				{
					adds[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dels.Length);
				for(int i = 0; i < dels.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, dels[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 addsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref addsCnt);
				adds = new PlayerTitleInfo[addsCnt];
				for(int i = 0; i < adds.Length; i++)
				{
					adds[i] = new PlayerTitleInfo();
					adds[i].decode(buffer, ref pos_);
				}
				UInt16 delsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref delsCnt);
				dels = new UInt64[delsCnt];
				for(int i = 0; i < dels.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref dels[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)adds.Length);
				for(int i = 0; i < adds.Length; i++)
				{
					adds[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dels.Length);
				for(int i = 0; i < dels.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, dels[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 addsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref addsCnt);
				adds = new PlayerTitleInfo[addsCnt];
				for(int i = 0; i < adds.Length; i++)
				{
					adds[i] = new PlayerTitleInfo();
					adds[i].decode(buffer, ref pos_);
				}
				UInt16 delsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref delsCnt);
				dels = new UInt64[delsCnt];
				for(int i = 0; i < dels.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref dels[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// adds
				_len += 2;
				for(int j = 0; j < adds.Length; j++)
				{
					_len += adds[j].getLen();
				}
				// dels
				_len += 2 + 8 * dels.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 删除
	/// </summary>
	/// <summary>
	/// world->client 更新头衔数据
	/// </summary>
	[Protocol]
	public class WorldNewTitleUpdateData : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609207;
		public UInt32 Sequence;
		public PlayerTitleInfo[] updates = new PlayerTitleInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)updates.Length);
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 updatesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref updatesCnt);
				updates = new PlayerTitleInfo[updatesCnt];
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i] = new PlayerTitleInfo();
					updates[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)updates.Length);
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 updatesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref updatesCnt);
				updates = new PlayerTitleInfo[updatesCnt];
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i] = new PlayerTitleInfo();
					updates[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// updates
				_len += 2;
				for(int j = 0; j < updates.Length; j++)
				{
					_len += updates[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
