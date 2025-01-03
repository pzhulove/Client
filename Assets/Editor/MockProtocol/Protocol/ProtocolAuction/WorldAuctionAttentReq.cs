using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 关注
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 关注", "client->world 关注")]
	public class WorldAuctionAttentReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603929;
		public UInt32 Sequence;
		/// <summary>
		/// 拍卖行商品id
		/// </summary>
		[AdvancedInspector.Descriptor("拍卖行商品id", "拍卖行商品id")]
		public UInt64 autionGuid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, autionGuid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref autionGuid);
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
