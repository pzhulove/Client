using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拍卖行一口价请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行一口价请求", " 拍卖行一口价请求")]
	public class WorldGuildAuctionFixReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608517;
		public UInt32 Sequence;
		/// <summary>
		///  guid
		/// </summary>
		[AdvancedInspector.Descriptor(" guid", " guid")]
		public UInt64 guid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
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
