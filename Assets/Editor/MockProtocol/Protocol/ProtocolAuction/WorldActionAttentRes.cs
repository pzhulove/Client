using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 关注返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 关注返回", "world->client 关注返回")]
	public class WorldActionAttentRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603930;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public UInt32 code;
		/// <summary>
		/// 拍卖行物品信息
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行物品信息", "拍卖行物品信息")]
		public AuctionBaseInfo aution = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			aution.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			aution.decode(buffer, ref pos_);
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
