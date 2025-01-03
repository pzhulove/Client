using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 抽奖返回
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 抽奖返回", " server->client 抽奖返回")]
	public class SceneDrawPrizeRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501007;
		public UInt32 Sequence;
		/// <summary>
		///  抽奖表id
		/// </summary>
		[AdvancedInspector.Descriptor(" 抽奖表id", " 抽奖表id")]
		public UInt32 drawPrizeId;
		/// <summary>
		///  奖励id
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励id", " 奖励id")]
		public UInt32 rewardId;
		/// <summary>
		///  返回错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回错误码", " 返回错误码")]
		public UInt32 retCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, drawPrizeId);
			BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref drawPrizeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
