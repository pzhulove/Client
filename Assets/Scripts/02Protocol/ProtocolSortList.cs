using System;
using System.Text;

namespace Protocol
{
	public class SortListType : Protocol.IProtocolStream
	{
		/// <summary>
		///  主类型
		/// </summary>
		public UInt32 mainType;
		/// <summary>
		///  子类型
		/// </summary>
		public UInt32 subType;
		/// <summary>
		///  数据ID
		/// </summary>
		public UInt64 dataId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mainType);
				BaseDLL.encode_uint32(buffer, ref pos_, subType);
				BaseDLL.encode_uint64(buffer, ref pos_, dataId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mainType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref subType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dataId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mainType);
				BaseDLL.encode_uint32(buffer, ref pos_, subType);
				BaseDLL.encode_uint64(buffer, ref pos_, dataId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mainType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref subType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref dataId);
			}

			public int getLen()
			{
				int _len = 0;
				// mainType
				_len += 4;
				// subType
				_len += 4;
				// dataId
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldSortListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602601;
		public UInt32 Sequence;
		public SortListType type = new SortListType();
		public byte occu;
		public UInt16 start;
		public UInt16 num;

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
				type.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				type.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				type.encode(buffer, ref pos_);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint16(buffer, ref pos_, start);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				type.decode(buffer, ref pos_);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint16(buffer, ref pos_, ref start);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += type.getLen();
				// occu
				_len += 1;
				// start
				_len += 2;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldSortListRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602602;
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
	public class WorldSortListSelfInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602610;
		public UInt32 Sequence;
		public SortListType type = new SortListType();

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
				type.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				type.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				type.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				type.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += type.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldSortListSelfInfoRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602611;
		public UInt32 Sequence;
		public UInt32 ranking;

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
				BaseDLL.encode_uint32(buffer, ref pos_, ranking);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ranking);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
			}

			public int getLen()
			{
				int _len = 0;
				// ranking
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldSortListWatchDataReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602603;
		public UInt32 Sequence;
		public SortListType type = new SortListType();
		public UInt64 id;

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
				type.encode(buffer, ref pos_);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				type.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				type.encode(buffer, ref pos_);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				type.decode(buffer, ref pos_);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += type.getLen();
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

}
