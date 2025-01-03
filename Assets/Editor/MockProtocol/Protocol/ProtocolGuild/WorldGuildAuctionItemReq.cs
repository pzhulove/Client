using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拍卖行信息请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行信息请求", " 拍卖行信息请求")]
	public class WorldGuildAuctionItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608513;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型(GuildAuctionType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖类型(GuildAuctionType)", " 拍卖类型(GuildAuctionType)")]
		public UInt32 type;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, type);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref type);
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
