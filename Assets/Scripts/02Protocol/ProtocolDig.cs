using System;
using System.Text;

namespace Protocol
{
	public enum DigType
	{
		/// <summary>
		/// 无效
		/// </summary>
		DIG_INVALID = 0,
		/// <summary>
		/// 金挖宝点
		/// </summary>
		DIG_GLOD = 1,
		/// <summary>
		/// 银挖宝点
		/// </summary>
		DIG_SILVER = 2,
	}

	public enum DigStatus
	{
		/// <summary>
		/// 无效
		/// </summary>
		DIG_STATUS_INVALID = 0,
		/// <summary>
		/// 初始状态
		/// </summary>
		DIG_STATUS_INIT = 1,
		/// <summary>
		/// 打开状态
		/// </summary>
		DIG_STATUS_OPEN = 2,
	}

	public class DigInfo : Protocol.IProtocolStream
	{
		public UInt32 index;
		public byte type;
		public byte status;
		public UInt32 refreshTime;
		public UInt32 changeStatusTime;
		public UInt32 openItemId;
		public UInt32 openItemNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, refreshTime);
				BaseDLL.encode_uint32(buffer, ref pos_, changeStatusTime);
				BaseDLL.encode_uint32(buffer, ref pos_, openItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, openItemNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref refreshTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref changeStatusTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref openItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref openItemNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, refreshTime);
				BaseDLL.encode_uint32(buffer, ref pos_, changeStatusTime);
				BaseDLL.encode_uint32(buffer, ref pos_, openItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, openItemNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref refreshTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref changeStatusTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref openItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref openItemNum);
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 4;
				// type
				_len += 1;
				// status
				_len += 1;
				// refreshTime
				_len += 4;
				// changeStatusTime
				_len += 4;
				// openItemId
				_len += 4;
				// openItemNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class DigItemInfo : Protocol.IProtocolStream
	{
		public UInt32 itemId;
		public UInt32 itemNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class DigDetailInfo : Protocol.IProtocolStream
	{
		public DigInfo simpleInfo = new DigInfo();
		public DigItemInfo[] digItems = new DigItemInfo[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				simpleInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)digItems.Length);
				for(int i = 0; i < digItems.Length; i++)
				{
					digItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				simpleInfo.decode(buffer, ref pos_);
				UInt16 digItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref digItemsCnt);
				digItems = new DigItemInfo[digItemsCnt];
				for(int i = 0; i < digItems.Length; i++)
				{
					digItems[i] = new DigItemInfo();
					digItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				simpleInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)digItems.Length);
				for(int i = 0; i < digItems.Length; i++)
				{
					digItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				simpleInfo.decode(buffer, ref pos_);
				UInt16 digItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref digItemsCnt);
				digItems = new DigItemInfo[digItemsCnt];
				for(int i = 0; i < digItems.Length; i++)
				{
					digItems[i] = new DigItemInfo();
					digItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// simpleInfo
				_len += simpleInfo.getLen();
				// digItems
				_len += 2;
				for(int j = 0; j < digItems.Length; j++)
				{
					_len += digItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class DigMapInfo : Protocol.IProtocolStream
	{
		public UInt32 mapId;
		public UInt32 goldDigSize;
		public UInt32 silverDigSize;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, goldDigSize);
				BaseDLL.encode_uint32(buffer, ref pos_, silverDigSize);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldDigSize);
				BaseDLL.decode_uint32(buffer, ref pos_, ref silverDigSize);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, goldDigSize);
				BaseDLL.encode_uint32(buffer, ref pos_, silverDigSize);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldDigSize);
				BaseDLL.decode_uint32(buffer, ref pos_, ref silverDigSize);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				// goldDigSize
				_len += 4;
				// silverDigSize
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class DigRecordInfo : Protocol.IProtocolStream
	{
		public UInt32 mapId;
		public UInt32 digIndex;
		public byte type;
		public UInt64 playerId;
		public string playerName;
		public UInt32 itemId;
		public UInt32 itemNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, digIndex);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref digIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, digIndex);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, playerId);
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref digIndex);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				// digIndex
				_len += 4;
				// type
				_len += 1;
				// playerId
				_len += 8;
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigInfoSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608201;
		public UInt32 Sequence;
		public UInt32 mapId;
		public DigInfo info = new DigInfo();

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigRefreshSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608202;
		public UInt32 Sequence;
		public UInt32 mapId;
		public DigInfo[] infos = new DigInfo[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new DigInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new DigInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new DigInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new DigInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
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

	[Protocol]
	public class WorldDigPlayerSizeSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608203;
		public UInt32 Sequence;
		public UInt32 mapId;
		public UInt32 playerSize;

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, playerSize);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerSize);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, playerSize);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref playerSize);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				// playerSize
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigRecordInfoSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608204;
		public UInt32 Sequence;
		public DigRecordInfo info = new DigRecordInfo();

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

	[Protocol]
	public class WorldDigMapOpenReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608205;
		public UInt32 Sequence;
		public UInt32 mapId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigMapOpenRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608206;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 mapId;
		public DigInfo[] infos = new DigInfo[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new DigInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new DigInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new DigInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new DigInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// mapId
				_len += 4;
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

	[Protocol]
	public class WorldDigMapCloseReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608207;
		public UInt32 Sequence;
		public UInt32 mapId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigMapCloseRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608208;
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
	public class WorldDigWatchReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608209;
		public UInt32 Sequence;
		public UInt32 mapId;
		public UInt32 index;

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				// index
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigWatchRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608210;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 mapId;
		public UInt32 index;
		public DigDetailInfo info = new DigDetailInfo();

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				info.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				info.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				info.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				info.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// mapId
				_len += 4;
				// index
				_len += 4;
				// info
				_len += info.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigOpenReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608211;
		public UInt32 Sequence;
		public UInt32 mapId;
		public UInt32 index;

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
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mapId);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public int getLen()
			{
				int _len = 0;
				// mapId
				_len += 4;
				// index
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigOpenRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608212;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt32 itemIndex;
		public UInt32 itemId;
		public UInt32 itemNum;

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, itemIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// itemIndex
				_len += 4;
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigMapInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608213;
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
	public class WorldDigMapInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608214;
		public UInt32 Sequence;
		public UInt32 result;
		public DigMapInfo[] digMapInfos = new DigMapInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)digMapInfos.Length);
				for(int i = 0; i < digMapInfos.Length; i++)
				{
					digMapInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 digMapInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref digMapInfosCnt);
				digMapInfos = new DigMapInfo[digMapInfosCnt];
				for(int i = 0; i < digMapInfos.Length; i++)
				{
					digMapInfos[i] = new DigMapInfo();
					digMapInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)digMapInfos.Length);
				for(int i = 0; i < digMapInfos.Length; i++)
				{
					digMapInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 digMapInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref digMapInfosCnt);
				digMapInfos = new DigMapInfo[digMapInfosCnt];
				for(int i = 0; i < digMapInfos.Length; i++)
				{
					digMapInfos[i] = new DigMapInfo();
					digMapInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// digMapInfos
				_len += 2;
				for(int j = 0; j < digMapInfos.Length; j++)
				{
					_len += digMapInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldDigRecordsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608215;
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
	public class WorldDigRecordsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608216;
		public UInt32 Sequence;
		public UInt32 result;
		public DigRecordInfo[] infos = new DigRecordInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new DigRecordInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new DigRecordInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new DigRecordInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new DigRecordInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
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

}
