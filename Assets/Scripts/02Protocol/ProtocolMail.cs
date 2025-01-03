using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  邮件类型
	/// </summary>
	public enum MailType
	{
		/// <summary>
		///  系统邮件
		/// </summary>
		MAIL_TYPE_SYSTEM = 0,
		/// <summary>
		///  普通邮件
		/// </summary>
		MAIL_TYPE_NORMAL = 1,
		/// <summary>
		///  GM邮件
		/// </summary>
		MAIL_TYPE_GM = 2,
		/// <summary>
		///  公会邮件
		/// </summary>
		MAIL_TYPE_GUILD = 3,
	}

	/// <summary>
	/// 邮件标题信息
	/// </summary>
	public class MailTitleInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// 邮件ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		/// 发件人
		/// </summary>
		public string sender;
		/// <summary>
		/// 发送日期
		/// </summary>
		public UInt32 date;
		/// <summary>
		/// 截至日期
		/// </summary>
		public UInt32 deadline;
		/// <summary>
		/// 邮件类型
		/// </summary>
		public byte type;
		/// <summary>
		/// 状态	0未读 1已读
		/// </summary>
		public byte status;
		/// <summary>
		/// 是否有附件 0没有 1有
		/// </summary>
		public byte hasItem;
		/// <summary>
		/// 标题
		/// </summary>
		public string title;
		/// <summary>
		/// 物品数据ID,用于显示图标
		/// </summary>
		public UInt32 itemId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] senderBytes = StringHelper.StringToUTF8Bytes(sender);
				BaseDLL.encode_string(buffer, ref pos_, senderBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, date);
				BaseDLL.encode_uint32(buffer, ref pos_, deadline);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_int8(buffer, ref pos_, hasItem);
				byte[] titleBytes = StringHelper.StringToUTF8Bytes(title);
				BaseDLL.encode_string(buffer, ref pos_, titleBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 senderLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref senderLen);
				byte[] senderBytes = new byte[senderLen];
				for(int i = 0; i < senderLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref senderBytes[i]);
				}
				sender = StringHelper.BytesToString(senderBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref date);
				BaseDLL.decode_uint32(buffer, ref pos_, ref deadline);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasItem);
				UInt16 titleLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref titleLen);
				byte[] titleBytes = new byte[titleLen];
				for(int i = 0; i < titleLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref titleBytes[i]);
				}
				title = StringHelper.BytesToString(titleBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] senderBytes = StringHelper.StringToUTF8Bytes(sender);
				BaseDLL.encode_string(buffer, ref pos_, senderBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, date);
				BaseDLL.encode_uint32(buffer, ref pos_, deadline);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_int8(buffer, ref pos_, hasItem);
				byte[] titleBytes = StringHelper.StringToUTF8Bytes(title);
				BaseDLL.encode_string(buffer, ref pos_, titleBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 senderLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref senderLen);
				byte[] senderBytes = new byte[senderLen];
				for(int i = 0; i < senderLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref senderBytes[i]);
				}
				sender = StringHelper.BytesToString(senderBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref date);
				BaseDLL.decode_uint32(buffer, ref pos_, ref deadline);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasItem);
				UInt16 titleLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref titleLen);
				byte[] titleBytes = new byte[titleLen];
				for(int i = 0; i < titleLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref titleBytes[i]);
				}
				title = StringHelper.BytesToString(titleBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// sender
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(sender);
					_len += 2 + _strBytes.Length;
				}
				// date
				_len += 4;
				// deadline
				_len += 4;
				// type
				_len += 1;
				// status
				_len += 1;
				// hasItem
				_len += 1;
				// title
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(title);
					_len += 2 + _strBytes.Length;
				}
				// itemId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 请求邮件列表
	/// </summary>
	[Protocol]
	public class WorldMailListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601502;
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
	///  server->client 返回邮件列表
	/// </summary>
	[Protocol]
	public class WorldMailListRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601503;
		public UInt32 Sequence;
		public MailTitleInfo[] mails = new MailTitleInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mails.Length);
				for(int i = 0; i < mails.Length; i++)
				{
					mails[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 mailsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mailsCnt);
				mails = new MailTitleInfo[mailsCnt];
				for(int i = 0; i < mails.Length; i++)
				{
					mails[i] = new MailTitleInfo();
					mails[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mails.Length);
				for(int i = 0; i < mails.Length; i++)
				{
					mails[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 mailsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mailsCnt);
				mails = new MailTitleInfo[mailsCnt];
				for(int i = 0; i < mails.Length; i++)
				{
					mails[i] = new MailTitleInfo();
					mails[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mails
				_len += 2;
				for(int j = 0; j < mails.Length; j++)
				{
					_len += mails[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 阅读邮件
	/// </summary>
	[Protocol]
	public class WorldReadMailReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601504;
		public UInt32 Sequence;
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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 收取邮件附件
	/// </summary>
	[Protocol]
	public class WorldGetMailItems : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601506;
		public UInt32 Sequence;
		/// <summary>
		/// 类型 0为收取单个 1为全部收取
		/// </summary>
		public byte type;
		public UInt64 id;
		public UInt32 mailType;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, mailType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mailType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, mailType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mailType);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 8;
				// mailType
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 同步邮件状态
	/// </summary>
	[Protocol]
	public class WorldSyncMailStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601507;
		public UInt32 Sequence;
		public UInt64 id;
		/// <summary>
		/// 状态	0未读 1已读
		/// </summary>
		public byte status;
		/// <summary>
		/// 是否有附件 0没有 1有
		/// </summary>
		public byte hasItem;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_int8(buffer, ref pos_, hasItem);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasItem);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_int8(buffer, ref pos_, hasItem);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_int8(buffer, ref pos_, ref hasItem);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// status
				_len += 1;
				// hasItem
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 通知新邮件
	/// </summary>
	[Protocol]
	public class WorldNotifyNewMail : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601509;
		public UInt32 Sequence;
		/// <summary>
		/// 邮件标题信息
		/// </summary>
		public MailTitleInfo info = new MailTitleInfo();

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
	///  client->server 删除邮件
	/// </summary>
	[Protocol]
	public class WorldDeleteMail : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601510;
		public UInt32 Sequence;
		/// <summary>
		/// 邮件ID列表
		/// </summary>
		public UInt64[] ids = new UInt64[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, ids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 idsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
				ids = new UInt64[idsCnt];
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref ids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, ids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 idsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
				ids = new UInt64[idsCnt];
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref ids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// ids
				_len += 2 + 8 * ids.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 通知删除邮件
	/// </summary>
	[Protocol]
	public class WorldNotifyDeleteMail : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 601511;
		public UInt32 Sequence;
		/// <summary>
		/// 邮件ID列表
		/// </summary>
		public UInt64[] ids = new UInt64[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, ids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 idsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
				ids = new UInt64[idsCnt];
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref ids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, ids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 idsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
				ids = new UInt64[idsCnt];
				for(int i = 0; i < ids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref ids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// ids
				_len += 2 + 8 * ids.Length;
				return _len;
			}
		#endregion

	}

}
