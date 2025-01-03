using System;
using System.Text;

namespace Protocol
{
	public enum RedPacketType
	{
		/// <summary>
		///  ������
		/// </summary>
		GUILD = 1,
		/// <summary>
		///  ������
		/// </summary>
		NEW_YEAR = 2,
	}

	/// <summary>
	///  ���״̬
	/// </summary>
	public enum RedPacketStatus
	{
		/// <summary>
		///  ��ʼ״̬
		/// </summary>
		INIT = 0,
		/// <summary>
		///  δ���
		/// </summary>
		UNSATISFY = 1,
		/// <summary>
		///  �ȴ�������ȡ���
		/// </summary>
		WAIT_RECEIVE = 2,
		/// <summary>
		///  ����
		/// </summary>
		RECEIVED = 3,
		/// <summary>
		///  �ѱ�����
		/// </summary>
		EMPTY = 4,
		/// <summary>
		///  �ɴݻ�
		/// </summary>
		DESTORY = 5,
	}

	/// <summary>
	///  ���������Ϣ
	/// </summary>
	public class RedPacketBaseEntry : Protocol.IProtocolStream
	{
		/// <summary>
		///  ���ID
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  ����
		/// </summary>
		public string name;
		/// <summary>
		///  ������ID
		/// </summary>
		public UInt64 ownerId;
		/// <summary>
		///  ����������
		/// </summary>
		public string ownerName;
		/// <summary>
		///  ״̬����Ӧö��RedPacketStatus��
		/// </summary>
		public byte status;
		/// <summary>
		///  ��û�д򿪹�
		/// </summary>
		public byte opened;
		/// <summary>
		///  �������(��Ӧö��RedPacketType)
		/// </summary>
		public byte type;
		/// <summary>
		///  �����Դ
		/// </summary>
		public UInt16 reason;
		/// <summary>
		///  ������
		/// </summary>
		public UInt32 totalMoney;
		/// <summary>
		///  �������
		/// </summary>
		public byte totalNum;
		/// <summary>
		///  ���ʣ������
		/// </summary>
		public byte remainNum;
		/// <summary>
		///  ����ϵ��ս����ʱ���
		/// </summary>
		public UInt32 guildTimeStamp;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, ownerId);
				byte[] ownerNameBytes = StringHelper.StringToUTF8Bytes(ownerName);
				BaseDLL.encode_string(buffer, ref pos_, ownerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_int8(buffer, ref pos_, opened);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, reason);
				BaseDLL.encode_uint32(buffer, ref pos_, totalMoney);
				BaseDLL.encode_int8(buffer, ref pos_, totalNum);
				BaseDLL.encode_int8(buffer, ref pos_, remainNum);
				BaseDLL.encode_uint32(buffer, ref pos_, guildTimeStamp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownerId);
				UInt16 ownerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ownerNameLen);
				byte[] ownerNameBytes = new byte[ownerNameLen];
				for(int i = 0; i < ownerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ownerNameBytes[i]);
				}
				ownerName = StringHelper.BytesToString(ownerNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_int8(buffer, ref pos_, ref opened);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalMoney);
				BaseDLL.decode_int8(buffer, ref pos_, ref totalNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref remainNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildTimeStamp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, ownerId);
				byte[] ownerNameBytes = StringHelper.StringToUTF8Bytes(ownerName);
				BaseDLL.encode_string(buffer, ref pos_, ownerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_int8(buffer, ref pos_, opened);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, reason);
				BaseDLL.encode_uint32(buffer, ref pos_, totalMoney);
				BaseDLL.encode_int8(buffer, ref pos_, totalNum);
				BaseDLL.encode_int8(buffer, ref pos_, remainNum);
				BaseDLL.encode_uint32(buffer, ref pos_, guildTimeStamp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ownerId);
				UInt16 ownerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref ownerNameLen);
				byte[] ownerNameBytes = new byte[ownerNameLen];
				for(int i = 0; i < ownerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref ownerNameBytes[i]);
				}
				ownerName = StringHelper.BytesToString(ownerNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_int8(buffer, ref pos_, ref opened);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalMoney);
				BaseDLL.decode_int8(buffer, ref pos_, ref totalNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref remainNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref guildTimeStamp);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// ownerId
				_len += 8;
				// ownerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(ownerName);
					_len += 2 + _strBytes.Length;
				}
				// status
				_len += 1;
				// opened
				_len += 1;
				// type
				_len += 1;
				// reason
				_len += 2;
				// totalMoney
				_len += 4;
				// totalNum
				_len += 1;
				// remainNum
				_len += 1;
				// guildTimeStamp
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �����ȡ����Ϣ
	/// </summary>
	public class RedPacketReceiverEntry : Protocol.IProtocolStream
	{
		/// <summary>
		///  icon
		/// </summary>
		public PlayerIcon icon = new PlayerIcon();
		/// <summary>
		///  ��ý��
		/// </summary>
		public UInt32 money;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				icon.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, money);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				icon.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref money);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				icon.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, money);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				icon.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref money);
			}

			public int getLen()
			{
				int _len = 0;
				// icon
				_len += icon.getLen();
				// money
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �����ϸ��Ϣ
	/// </summary>
	public class RedPacketDetail : Protocol.IProtocolStream
	{
		/// <summary>
		///  ������Ϣ
		/// </summary>
		public RedPacketBaseEntry baseEntry = new RedPacketBaseEntry();
		/// <summary>
		///  ӵ����ͷ��
		/// </summary>
		public PlayerIcon ownerIcon = new PlayerIcon();
		/// <summary>
		///  ������ȡ�����
		/// </summary>
		public RedPacketReceiverEntry[] receivers = new RedPacketReceiverEntry[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				baseEntry.encode(buffer, ref pos_);
				ownerIcon.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)receivers.Length);
				for(int i = 0; i < receivers.Length; i++)
				{
					receivers[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				baseEntry.decode(buffer, ref pos_);
				ownerIcon.decode(buffer, ref pos_);
				UInt16 receiversCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref receiversCnt);
				receivers = new RedPacketReceiverEntry[receiversCnt];
				for(int i = 0; i < receivers.Length; i++)
				{
					receivers[i] = new RedPacketReceiverEntry();
					receivers[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				baseEntry.encode(buffer, ref pos_);
				ownerIcon.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)receivers.Length);
				for(int i = 0; i < receivers.Length; i++)
				{
					receivers[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				baseEntry.decode(buffer, ref pos_);
				ownerIcon.decode(buffer, ref pos_);
				UInt16 receiversCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref receiversCnt);
				receivers = new RedPacketReceiverEntry[receiversCnt];
				for(int i = 0; i < receivers.Length; i++)
				{
					receivers[i] = new RedPacketReceiverEntry();
					receivers[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// baseEntry
				_len += baseEntry.getLen();
				// ownerIcon
				_len += ownerIcon.getLen();
				// receivers
				_len += 2;
				for(int j = 0; j < receivers.Length; j++)
				{
					_len += receivers[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �����ȡ��¼
	/// </summary>
	public class RedPacketRecord : Protocol.IProtocolStream
	{
		/// <summary>
		///  Ψһid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  ʱ��
		/// </summary>
		public UInt32 time;
		/// <summary>
		///  �������������
		/// </summary>
		public string redPackOwnerName;
		/// <summary>
		///  ����id
		/// </summary>
		public UInt32 moneyId;
		/// <summary>
		///  ��������
		/// </summary>
		public UInt32 moneyNum;
		/// <summary>
		///  �Ƿ���Ѻ��
		/// </summary>
		public byte isBest;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
				byte[] redPackOwnerNameBytes = StringHelper.StringToUTF8Bytes(redPackOwnerName);
				BaseDLL.encode_string(buffer, ref pos_, redPackOwnerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, moneyId);
				BaseDLL.encode_uint32(buffer, ref pos_, moneyNum);
				BaseDLL.encode_int8(buffer, ref pos_, isBest);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
				UInt16 redPackOwnerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref redPackOwnerNameLen);
				byte[] redPackOwnerNameBytes = new byte[redPackOwnerNameLen];
				for(int i = 0; i < redPackOwnerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref redPackOwnerNameBytes[i]);
				}
				redPackOwnerName = StringHelper.BytesToString(redPackOwnerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref moneyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref moneyNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref isBest);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, time);
				byte[] redPackOwnerNameBytes = StringHelper.StringToUTF8Bytes(redPackOwnerName);
				BaseDLL.encode_string(buffer, ref pos_, redPackOwnerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, moneyId);
				BaseDLL.encode_uint32(buffer, ref pos_, moneyNum);
				BaseDLL.encode_int8(buffer, ref pos_, isBest);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref time);
				UInt16 redPackOwnerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref redPackOwnerNameLen);
				byte[] redPackOwnerNameBytes = new byte[redPackOwnerNameLen];
				for(int i = 0; i < redPackOwnerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref redPackOwnerNameBytes[i]);
				}
				redPackOwnerName = StringHelper.BytesToString(redPackOwnerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref moneyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref moneyNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref isBest);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// time
				_len += 4;
				// redPackOwnerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(redPackOwnerName);
					_len += 2 + _strBytes.Length;
				}
				// moneyId
				_len += 4;
				// moneyNum
				_len += 4;
				// isBest
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ��������Ϣ
	/// </summary>
	public class GuildRedPacketSpecInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  �������
		/// </summary>
		public byte type;
		/// <summary>
		///  ʱ��
		/// </summary>
		public UInt32 lastTime;
		/// <summary>
		///  ��������
		/// </summary>
		public UInt32 joinNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, lastTime);
				BaseDLL.encode_uint32(buffer, ref pos_, joinNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, lastTime);
				BaseDLL.encode_uint32(buffer, ref pos_, joinNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref joinNum);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// lastTime
				_len += 4;
				// joinNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ��¼ʱͬ�����������Ϣ
	/// </summary>
	[Protocol]
	public class WorldSyncRedPacket : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607301;
		public UInt32 Sequence;
		/// <summary>
		///  ���������Ϣ
		/// </summary>
		public RedPacketBaseEntry[] entrys = new RedPacketBaseEntry[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)entrys.Length);
				for(int i = 0; i < entrys.Length; i++)
				{
					entrys[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 entrysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref entrysCnt);
				entrys = new RedPacketBaseEntry[entrysCnt];
				for(int i = 0; i < entrys.Length; i++)
				{
					entrys[i] = new RedPacketBaseEntry();
					entrys[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)entrys.Length);
				for(int i = 0; i < entrys.Length; i++)
				{
					entrys[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 entrysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref entrysCnt);
				entrys = new RedPacketBaseEntry[entrysCnt];
				for(int i = 0; i < entrys.Length; i++)
				{
					entrys[i] = new RedPacketBaseEntry();
					entrys[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// entrys
				_len += 2;
				for(int j = 0; j < entrys.Length; j++)
				{
					_len += entrys[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ֪ͨ����º��
	/// </summary>
	[Protocol]
	public class WorldNotifyGotNewRedPacket : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607302;
		public UInt32 Sequence;
		/// <summary>
		///  ���������Ϣ
		/// </summary>
		public RedPacketBaseEntry entry = new RedPacketBaseEntry();

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
				entry.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				entry.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				entry.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				entry.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// entry
				_len += entry.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ֪ͨ���º������
	/// </summary>
	[Protocol]
	public class WorldNotifyNewRedPacket : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607303;
		public UInt32 Sequence;
		/// <summary>
		///  ���������Ϣ
		/// </summary>
		public RedPacketBaseEntry[] entry = new RedPacketBaseEntry[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)entry.Length);
				for(int i = 0; i < entry.Length; i++)
				{
					entry[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 entryCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref entryCnt);
				entry = new RedPacketBaseEntry[entryCnt];
				for(int i = 0; i < entry.Length; i++)
				{
					entry[i] = new RedPacketBaseEntry();
					entry[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)entry.Length);
				for(int i = 0; i < entry.Length; i++)
				{
					entry[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 entryCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref entryCnt);
				entry = new RedPacketBaseEntry[entryCnt];
				for(int i = 0; i < entry.Length; i++)
				{
					entry[i] = new RedPacketBaseEntry();
					entry[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// entry
				_len += 2;
				for(int j = 0; j < entry.Length; j++)
				{
					_len += entry[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ֪ͨɾ�����
	/// </summary>
	[Protocol]
	public class WorldNotifyDelRedPacket : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607304;
		public UInt32 Sequence;
		/// <summary>
		///  ��Ҫɾ���ĺ��ID
		/// </summary>
		public UInt64[] redPacketList = new UInt64[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)redPacketList.Length);
				for(int i = 0; i < redPacketList.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, redPacketList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 redPacketListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref redPacketListCnt);
				redPacketList = new UInt64[redPacketListCnt];
				for(int i = 0; i < redPacketList.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref redPacketList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)redPacketList.Length);
				for(int i = 0; i < redPacketList.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, redPacketList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 redPacketListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref redPacketListCnt);
				redPacketList = new UInt64[redPacketListCnt];
				for(int i = 0; i < redPacketList.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref redPacketList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// redPacketList
				_len += 2 + 8 * redPacketList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ֪ͨ�޸ĺ��״̬
	/// </summary>
	[Protocol]
	public class WorldNotifySyncRedPacketStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607305;
		public UInt32 Sequence;
		/// <summary>
		///  id
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  ״̬(��Ӧö��RedPacketStatus)
		/// </summary>
		public byte status;

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
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// status
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ���󷢺��
	/// </summary>
	[Protocol]
	public class WorldSendRedPacketReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607306;
		public UInt32 Sequence;
		/// <summary>
		///  id
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  �������
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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// num
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ���������
	/// </summary>
	[Protocol]
	public class WorldSendRedPacketRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607307;
		public UInt32 Sequence;
		/// <summary>
		///  ������
		/// </summary>
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
	///  ����򿪺��(����Ѿ��򿪹��ˣ��Ǿ��ǲ鿴)
	/// </summary>
	[Protocol]
	public class WorldOpenRedPacketReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607308;
		public UInt32 Sequence;
		/// <summary>
		///  id
		/// </summary>
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
	///  ���ش򿪺������
	/// </summary>
	[Protocol]
	public class WorldOpenRedPacketRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607309;
		public UInt32 Sequence;
		/// <summary>
		///  ������
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  �����ϸ��Ϣ
		/// </summary>
		public RedPacketDetail detail = new RedPacketDetail();

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
				detail.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				detail.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				detail.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				detail.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// detail
				_len += detail.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  �����ԷѺ��
	/// </summary>
	[Protocol]
	public class WorldSendCustomRedPacketReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607310;
		public UInt32 Sequence;
		/// <summary>
		///  reason
		/// </summary>
		public UInt16 reason;
		/// <summary>
		///  �������
		/// </summary>
		public string name;
		/// <summary>
		///  ����
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
				BaseDLL.encode_uint16(buffer, ref pos_, reason);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, reason);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// reason
				_len += 2;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// num
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ���ԷѺ������
	/// </summary>
	[Protocol]
	public class WorldSendCustomRedPacketRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607311;
		public UInt32 Sequence;
		/// <summary>
		///  result
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  ���ID
		/// </summary>
		public UInt64 redPacketId;

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
				BaseDLL.encode_uint64(buffer, ref pos_, redPacketId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref redPacketId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint64(buffer, ref pos_, redPacketId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint64(buffer, ref pos_, ref redPacketId);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// redPacketId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client ͬ�������¼
	/// </summary>
	[Protocol]
	public class WorldSyncRedPacketRecord : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607312;
		public UInt32 Sequence;
		public RedPacketRecord[] adds = new RedPacketRecord[0];
		public UInt64[] dels = new UInt64[0];
		public RedPacketRecord[] updates = new RedPacketRecord[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)updates.Length);
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 addsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref addsCnt);
				adds = new RedPacketRecord[addsCnt];
				for(int i = 0; i < adds.Length; i++)
				{
					adds[i] = new RedPacketRecord();
					adds[i].decode(buffer, ref pos_);
				}
				UInt16 delsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref delsCnt);
				dels = new UInt64[delsCnt];
				for(int i = 0; i < dels.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref dels[i]);
				}
				UInt16 updatesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref updatesCnt);
				updates = new RedPacketRecord[updatesCnt];
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i] = new RedPacketRecord();
					updates[i].decode(buffer, ref pos_);
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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)updates.Length);
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 addsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref addsCnt);
				adds = new RedPacketRecord[addsCnt];
				for(int i = 0; i < adds.Length; i++)
				{
					adds[i] = new RedPacketRecord();
					adds[i].decode(buffer, ref pos_);
				}
				UInt16 delsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref delsCnt);
				dels = new UInt64[delsCnt];
				for(int i = 0; i < dels.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref dels[i]);
				}
				UInt16 updatesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref updatesCnt);
				updates = new RedPacketRecord[updatesCnt];
				for(int i = 0; i < updates.Length; i++)
				{
					updates[i] = new RedPacketRecord();
					updates[i].decode(buffer, ref pos_);
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

	/// <summary>
	/// client->world ��ȡ��������Ϣ����
	/// </summary>
	[Protocol]
	public class WorldGetGuildRedPacketInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607313;
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
	/// world->client ��ȡ��������Ϣ����
	/// </summary>
	[Protocol]
	public class WorldGetGuildRedPacketInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 607314;
		public UInt32 Sequence;
		public UInt32 code;
		public GuildRedPacketSpecInfo[] infos = new GuildRedPacketSpecInfo[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new GuildRedPacketSpecInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new GuildRedPacketSpecInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new GuildRedPacketSpecInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new GuildRedPacketSpecInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
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
