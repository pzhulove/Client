using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	/// 查询冒险通行证情况
	/// </summary>
	[Protocol]
	public class WorldAventurePassStatusReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609501;
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
	/// 查询冒险通行证情况返回
	/// </summary>
	[Protocol]
	public class WorldAventurePassStatusRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609502;
		public UInt32 Sequence;
		/// <summary>
		/// 冒险通行证等级
		/// </summary>
		public UInt32 lv;
		/// <summary>
		/// 开始时间
		/// </summary>
		public UInt32 startTime;
		/// <summary>
		/// 结束时间
		/// </summary>
		public UInt32 endTime;
		/// <summary>
		/// 当前赛季id
		/// </summary>
		public UInt32 seasonID;
		/// <summary>
		/// 经验
		/// </summary>
		public UInt32 exp;
		/// <summary>
		/// 通行证类型
		/// </summary>
		public byte type;
		/// <summary>
		/// 账号活跃度情况
		/// </summary>
		public UInt32 activity;
		/// <summary>
		/// 普通奖励领取情况
		/// </summary>
		public string normalReward;
		/// <summary>
		/// 高级奖励领取情况
		/// </summary>
		public string highReward;

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
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonID);
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, activity);
				byte[] normalRewardBytes = StringHelper.StringToUTF8Bytes(normalReward);
				BaseDLL.encode_string(buffer, ref pos_, normalRewardBytes, (UInt16)(buffer.Length - pos_));
				byte[] highRewardBytes = StringHelper.StringToUTF8Bytes(highReward);
				BaseDLL.encode_string(buffer, ref pos_, highRewardBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref activity);
				UInt16 normalRewardLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref normalRewardLen);
				byte[] normalRewardBytes = new byte[normalRewardLen];
				for(int i = 0; i < normalRewardLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref normalRewardBytes[i]);
				}
				normalReward = StringHelper.BytesToString(normalRewardBytes);
				UInt16 highRewardLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref highRewardLen);
				byte[] highRewardBytes = new byte[highRewardLen];
				for(int i = 0; i < highRewardLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref highRewardBytes[i]);
				}
				highReward = StringHelper.BytesToString(highRewardBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
				BaseDLL.encode_uint32(buffer, ref pos_, startTime);
				BaseDLL.encode_uint32(buffer, ref pos_, endTime);
				BaseDLL.encode_uint32(buffer, ref pos_, seasonID);
				BaseDLL.encode_uint32(buffer, ref pos_, exp);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, activity);
				byte[] normalRewardBytes = StringHelper.StringToUTF8Bytes(normalReward);
				BaseDLL.encode_string(buffer, ref pos_, normalRewardBytes, (UInt16)(buffer.Length - pos_));
				byte[] highRewardBytes = StringHelper.StringToUTF8Bytes(highReward);
				BaseDLL.encode_string(buffer, ref pos_, highRewardBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
				BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref seasonID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref activity);
				UInt16 normalRewardLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref normalRewardLen);
				byte[] normalRewardBytes = new byte[normalRewardLen];
				for(int i = 0; i < normalRewardLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref normalRewardBytes[i]);
				}
				normalReward = StringHelper.BytesToString(normalRewardBytes);
				UInt16 highRewardLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref highRewardLen);
				byte[] highRewardBytes = new byte[highRewardLen];
				for(int i = 0; i < highRewardLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref highRewardBytes[i]);
				}
				highReward = StringHelper.BytesToString(highRewardBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// lv
				_len += 4;
				// startTime
				_len += 4;
				// endTime
				_len += 4;
				// seasonID
				_len += 4;
				// exp
				_len += 4;
				// type
				_len += 1;
				// activity
				_len += 4;
				// normalReward
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(normalReward);
					_len += 2 + _strBytes.Length;
				}
				// highReward
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(highReward);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 购买冒险通行证
	/// </summary>
	[Protocol]
	public class WorldAventurePassBuyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609503;
		public UInt32 Sequence;
		/// <summary>
		/// 通行证购买类型
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
	/// world -> client 购买冒险通行证返回
	/// </summary>
	[Protocol]
	public class WorldAventurePassBuyRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609504;
		public UInt32 Sequence;
		/// <summary>
		/// 冒险通行证类型
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
	/// client->world 购买通行证等级
	/// </summary>
	[Protocol]
	public class WorldAventurePassBuyLvReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609505;
		public UInt32 Sequence;
		/// <summary>
		/// 购买的等级档次
		/// </summary>
		public UInt32 lv;

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
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public int getLen()
			{
				int _len = 0;
				// lv
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world -> client 购买冒险通行证等级返回
	/// </summary>
	[Protocol]
	public class WorldAventurePassBuyLvRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609506;
		public UInt32 Sequence;
		/// <summary>
		/// 购买的等级档次 0为失败
		/// </summary>
		public UInt32 lv;

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
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public int getLen()
			{
				int _len = 0;
				// lv
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 领取查询经验包
	/// </summary>
	[Protocol]
	public class WorldAventurePassExpPackReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609507;
		public UInt32 Sequence;
		/// <summary>
		/// 0 是查询 1是领取
		/// </summary>
		public byte op;

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
				BaseDLL.encode_int8(buffer, ref pos_, op);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref op);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, op);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref op);
			}

			public int getLen()
			{
				int _len = 0;
				// op
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world -> client 领取查询经验包返回
	/// </summary>
	[Protocol]
	public class WorldAventurePassExpPackRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609508;
		public UInt32 Sequence;
		/// <summary>
		/// 0 未解锁 1已解锁未领取 2已领取
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
	/// client->world 领取通行证等级奖励
	/// </summary>
	[Protocol]
	public class WorldAventurePassRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609509;
		public UInt32 Sequence;
		public UInt32 lv;

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
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			}

			public int getLen()
			{
				int _len = 0;
				// lv
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world -> client 领取通行证等级奖励返回
	/// </summary>
	[Protocol]
	public class WorldAventurePassRewardRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609510;
		public UInt32 Sequence;
		/// <summary>
		/// 请求领取的等级
		/// </summary>
		public UInt32 lv;
		/// <summary>
		/// 领取结果
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
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, lv);
				BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
				BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			}

			public int getLen()
			{
				int _len = 0;
				// lv
				_len += 4;
				// errorCode
				_len += 4;
				return _len;
			}
		#endregion

	}

}
