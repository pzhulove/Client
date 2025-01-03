using System;
using System.Text;

namespace Protocol
{
	public enum AccountCounterType
	{
		ACC_COUNTER_INVALID = 0,
		/// <summary>
		///  赐福水晶
		/// </summary>
		ACC_COUNTER_BLESS_CRYSTAL = 1,
		/// <summary>
		///  赐福经验
		/// </summary>
		ACC_COUNTER_BLESS_CRYSTAL_EXP = 2,
		/// <summary>
		///  传承祝福
		/// </summary>
		ACC_COUNTER_INHERIT_BLESS = 3,
		/// <summary>
		///  传承经验
		/// </summary>
		ACC_COUNTER_INHERIT_BLESS_EXP = 4,
		/// <summary>
		///  精力货币
		/// </summary>
		ACC_COUNTER_ENERGY_COIN = 5,
		/// <summary>
		///  赏金
		/// </summary>
		ACC_COUNTER_BOUNTY = 6,
		/// <summary>
		///  公会红包日领取上限
		/// </summary>
		ACC_GUILD_REDPACKET_DAILY_MAX = 7,
		/// <summary>
		///  冒险通信证邮件发送
		/// </summary>
		ACC_ADVENTURE_PASS_MAIL_SEND = 8,
		/// <summary>
		///  新服礼包打折标记
		/// </summary>
		ACC_NEW_SERVER_GIFT_DISCOUNT = 9,
		/// <summary>
		///  招募硬币
		/// </summary>
		ACC_COUNTER_HIRE_COIN = 10,
		/// <summary>
		///  招募推送
		/// </summary>
		ACC_COUNTER_HIRE_PUS = 11,
	}

	/// <summary>
	///  client->world 角色扩展栏位解锁请求
	/// </summary>
	[Protocol]
	public class WorldExtensibleRoleFieldUnlockReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608701;
		public UInt32 Sequence;
		/// <summary>
		///  需要消耗的道具
		/// </summary>
		public UInt64 costItemUId;
		public UInt32 costItemDataId;

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
				BaseDLL.encode_uint64(buffer, ref pos_, costItemUId);
				BaseDLL.encode_uint32(buffer, ref pos_, costItemDataId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref costItemUId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costItemDataId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, costItemUId);
				BaseDLL.encode_uint32(buffer, ref pos_, costItemDataId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref costItemUId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costItemDataId);
			}

			public int getLen()
			{
				int _len = 0;
				// costItemUId
				_len += 8;
				// costItemDataId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 角色扩展栏位解锁返回
	/// </summary>
	[Protocol]
	public class WorldExtensibleRoleFieldUnlockRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608702;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class AccountCounter : Protocol.IProtocolStream
	{
		/// <summary>
		///  计数类型
		/// </summary>
		public UInt32 type;
		/// <summary>
		///  数量
		/// </summary>
		public UInt64 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 4;
				// num
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  账号计数
	/// </summary>
	[Protocol]
	public class WorldAccountCounterNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600606;
		public UInt32 Sequence;
		/// <summary>
		///  账号计数列表
		/// </summary>
		public AccountCounter[] accCounterList = new AccountCounter[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)accCounterList.Length);
				for(int i = 0; i < accCounterList.Length; i++)
				{
					accCounterList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 accCounterListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref accCounterListCnt);
				accCounterList = new AccountCounter[accCounterListCnt];
				for(int i = 0; i < accCounterList.Length; i++)
				{
					accCounterList[i] = new AccountCounter();
					accCounterList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)accCounterList.Length);
				for(int i = 0; i < accCounterList.Length; i++)
				{
					accCounterList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 accCounterListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref accCounterListCnt);
				accCounterList = new AccountCounter[accCounterListCnt];
				for(int i = 0; i < accCounterList.Length; i++)
				{
					accCounterList[i] = new AccountCounter();
					accCounterList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// accCounterList
				_len += 2;
				for(int j = 0; j < accCounterList.Length; j++)
				{
					_len += accCounterList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
