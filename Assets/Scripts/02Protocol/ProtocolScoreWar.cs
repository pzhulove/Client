using System;
using System.Text;

namespace Protocol
{
	public enum ScoreWarStatus
	{
		/// <summary>
		/// 未开始
		/// </summary>
		SWS_INVALID = 0,
		/// <summary>
		/// 准备状态
		/// </summary>
		SWS_PREPARE = 1,
		/// <summary>
		/// 战斗状态
		/// </summary>
		SWS_BATTLE = 2,
		/// <summary>
		/// 等待结束
		/// </summary>
		SWS_WAIT_END = 3,
		/// <summary>
		/// 最大
		/// </summary>
		ROOM_TYPE_MAX = 4,
	}

	/// <summary>
	/// client->scene 请求积分赛奖励
	/// </summary>
	[Protocol]
	public class SceneScoreWarRewardReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508103;
		public UInt32 Sequence;
		/// <summary>
		/// 奖励ID
		/// </summary>
		public byte rewardId;

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
				BaseDLL.encode_int8(buffer, ref pos_, rewardId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rewardId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, rewardId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref rewardId);
			}

			public int getLen()
			{
				int _len = 0;
				// rewardId
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene 请求积分赛奖励响应
	/// </summary>
	[Protocol]
	public class SceneScoreWarRewardRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508104;
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

	/// <summary>
	///  server->client 同步积分赛信息
	/// </summary>
	[Protocol]
	public class SceneSyncScoreWarInfo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 508101;
		public UInt32 Sequence;
		public byte status;
		public UInt32 statusEndTime;

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
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, statusEndTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref statusEndTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, status);
				BaseDLL.encode_uint32(buffer, ref pos_, statusEndTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
				BaseDLL.decode_uint32(buffer, ref pos_, ref statusEndTime);
			}

			public int getLen()
			{
				int _len = 0;
				// status
				_len += 1;
				// statusEndTime
				_len += 4;
				return _len;
			}
		#endregion

	}

}
