using System;
using System.Text;

namespace Protocol
{
	public class WarpStoneInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public byte level;
		public UInt32 energy;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, energy);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref energy);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
				BaseDLL.encode_uint32(buffer, ref pos_, energy);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
				BaseDLL.decode_uint32(buffer, ref pos_, ref energy);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// level
				_len += 1;
				// energy
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ChargeInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneWarpStoneUnlockReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506901;
		public UInt32 Sequence;
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
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneWarpStoneUnlockRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506902;
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
	public class SceneWarpStoneChargeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506903;
		public UInt32 Sequence;
		public UInt32 id;
		public ChargeInfo[] info = new ChargeInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)info.Length);
				for(int i = 0; i < info.Length; i++)
				{
					info[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 infoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoCnt);
				info = new ChargeInfo[infoCnt];
				for(int i = 0; i < info.Length; i++)
				{
					info[i] = new ChargeInfo();
					info[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)info.Length);
				for(int i = 0; i < info.Length; i++)
				{
					info[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				UInt16 infoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infoCnt);
				info = new ChargeInfo[infoCnt];
				for(int i = 0; i < info.Length; i++)
				{
					info[i] = new ChargeInfo();
					info[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// info
				_len += 2;
				for(int j = 0; j < info.Length; j++)
				{
					_len += info[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneWarpStoneChargeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 506904;
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

}
