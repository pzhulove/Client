using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  头像框请求
	/// </summary>
	[Protocol]
	public class SceneHeadFrameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509101;
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

	public class HeadFrame : Protocol.IProtocolStream
	{
		/// <summary>
		///  头像框id
		/// </summary>
		public UInt32 headFrameId;
		/// <summary>
		///  过期时间
		/// </summary>
		public UInt32 expireTime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, headFrameId);
				BaseDLL.encode_uint32(buffer, ref pos_, expireTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrameId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref expireTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, headFrameId);
				BaseDLL.encode_uint32(buffer, ref pos_, expireTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrameId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref expireTime);
			}

			public int getLen()
			{
				int _len = 0;
				// headFrameId
				_len += 4;
				// expireTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  头像框返回
	/// </summary>
	[Protocol]
	public class SceneHeadFrameRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509102;
		public UInt32 Sequence;
		/// <summary>
		///  头像框列表
		/// </summary>
		public HeadFrame[] headFrameList = new HeadFrame[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)headFrameList.Length);
				for(int i = 0; i < headFrameList.Length; i++)
				{
					headFrameList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 headFrameListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref headFrameListCnt);
				headFrameList = new HeadFrame[headFrameListCnt];
				for(int i = 0; i < headFrameList.Length; i++)
				{
					headFrameList[i] = new HeadFrame();
					headFrameList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)headFrameList.Length);
				for(int i = 0; i < headFrameList.Length; i++)
				{
					headFrameList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 headFrameListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref headFrameListCnt);
				headFrameList = new HeadFrame[headFrameListCnt];
				for(int i = 0; i < headFrameList.Length; i++)
				{
					headFrameList[i] = new HeadFrame();
					headFrameList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// headFrameList
				_len += 2;
				for(int j = 0; j < headFrameList.Length; j++)
				{
					_len += headFrameList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  使用头像框请求
	/// </summary>
	[Protocol]
	public class SceneHeadFrameUseReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509103;
		public UInt32 Sequence;
		/// <summary>
		///  头像框id
		/// </summary>
		public UInt32 headFrameId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, headFrameId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrameId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, headFrameId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref headFrameId);
			}

			public int getLen()
			{
				int _len = 0;
				// headFrameId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  使用头像框返回
	/// </summary>
	[Protocol]
	public class SceneHeadFrameUseRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509104;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 retCode;

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
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  头像框通知
	/// </summary>
	[Protocol]
	public class SceneHeadFrameNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 509105;
		public UInt32 Sequence;
		/// <summary>
		///  1是获得，0删除
		/// </summary>
		public UInt32 isGet;
		/// <summary>
		///  头像框
		/// </summary>
		public HeadFrame headFrame = new HeadFrame();

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
				BaseDLL.encode_uint32(buffer, ref pos_, isGet);
				headFrame.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGet);
				headFrame.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, isGet);
				headFrame.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGet);
				headFrame.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// isGet
				_len += 4;
				// headFrame
				_len += headFrame.getLen();
				return _len;
			}
		#endregion

	}

}
