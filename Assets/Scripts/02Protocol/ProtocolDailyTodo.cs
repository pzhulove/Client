using System;
using System.Text;

namespace Protocol
{
	public class DailyTodoInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// 唯一id
		/// </summary>
		public UInt64 id;
		/// <summary>
		/// 表id
		/// </summary>
		public UInt32 dataId;
		/// <summary>
		/// 每日进度
		/// </summary>
		public UInt32 dayProgress;
		/// <summary>
		/// 每日进度更新时间戳
		/// </summary>
		public UInt32 dayProgTime;
		/// <summary>
		/// 每日进度最大值
		/// </summary>
		public UInt32 dayProgMax;
		/// <summary>
		/// 每周进度
		/// </summary>
		public UInt32 weekProgress;
		/// <summary>
		/// 每周进度更新时间戳
		/// </summary>
		public UInt32 weekProgTime;
		/// <summary>
		/// 每周进度最大值
		/// </summary>
		public UInt32 weekProgMax;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint32(buffer, ref pos_, dayProgress);
				BaseDLL.encode_uint32(buffer, ref pos_, dayProgTime);
				BaseDLL.encode_uint32(buffer, ref pos_, dayProgMax);
				BaseDLL.encode_uint32(buffer, ref pos_, weekProgress);
				BaseDLL.encode_uint32(buffer, ref pos_, weekProgTime);
				BaseDLL.encode_uint32(buffer, ref pos_, weekProgMax);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgress);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgMax);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgress);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgMax);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint32(buffer, ref pos_, dayProgress);
				BaseDLL.encode_uint32(buffer, ref pos_, dayProgTime);
				BaseDLL.encode_uint32(buffer, ref pos_, dayProgMax);
				BaseDLL.encode_uint32(buffer, ref pos_, weekProgress);
				BaseDLL.encode_uint32(buffer, ref pos_, weekProgTime);
				BaseDLL.encode_uint32(buffer, ref pos_, weekProgMax);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgress);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgMax);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgress);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgMax);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// dataId
				_len += 4;
				// dayProgress
				_len += 4;
				// dayProgTime
				_len += 4;
				// dayProgMax
				_len += 4;
				// weekProgress
				_len += 4;
				// weekProgTime
				_len += 4;
				// weekProgMax
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 每日必做请求
	/// </summary>
	[Protocol]
	public class WorldGetPlayerDailyTodosReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609301;
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
	/// world->client 每日必做返回
	/// </summary>
	[Protocol]
	public class WorldGetPlayerDailyTodosRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609302;
		public UInt32 Sequence;
		public DailyTodoInfo[] dailyTodos = new DailyTodoInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dailyTodos.Length);
				for(int i = 0; i < dailyTodos.Length; i++)
				{
					dailyTodos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 dailyTodosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dailyTodosCnt);
				dailyTodos = new DailyTodoInfo[dailyTodosCnt];
				for(int i = 0; i < dailyTodos.Length; i++)
				{
					dailyTodos[i] = new DailyTodoInfo();
					dailyTodos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dailyTodos.Length);
				for(int i = 0; i < dailyTodos.Length; i++)
				{
					dailyTodos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 dailyTodosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dailyTodosCnt);
				dailyTodos = new DailyTodoInfo[dailyTodosCnt];
				for(int i = 0; i < dailyTodos.Length; i++)
				{
					dailyTodos[i] = new DailyTodoInfo();
					dailyTodos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// dailyTodos
				_len += 2;
				for(int j = 0; j < dailyTodos.Length; j++)
				{
					_len += dailyTodos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
