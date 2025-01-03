using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world ÇÀ¹º¹ºÂòÅÄÂôµÀ¾ß
	/// </summary>
	[AdvancedInspector.Descriptor("client->world ÇÀ¹º¹ºÂòÅÄÂôµÀ¾ß", "client->world ÇÀ¹º¹ºÂòÅÄÂôµÀ¾ß")]
	public class WorldAuctionRusyBuy : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603926;
		public UInt32 Sequence;

		public UInt64 id;

		public UInt32 num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
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
