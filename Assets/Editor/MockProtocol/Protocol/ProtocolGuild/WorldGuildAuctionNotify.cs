using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拍卖行通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 拍卖行通知", " 拍卖行通知")]
	public class WorldGuildAuctionNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608519;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖类型", " 拍卖类型")]
		public UInt32 type;
		/// <summary>
		///  非0打开，0关闭
		/// </summary>
		[AdvancedInspector.Descriptor(" 非0打开，0关闭", " 非0打开，0关闭")]
		public UInt32 isOpen;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, isOpen);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isOpen);
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
