using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拍卖行一口价返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行一口价返回", " 拍卖行一口价返回")]
	public class WorldGuildAuctionFixRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608518;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回码", " 返回码")]
		public UInt32 retCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
