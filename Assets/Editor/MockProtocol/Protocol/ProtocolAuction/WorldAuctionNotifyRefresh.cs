using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldAuctionNotifyRefresh : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603911;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖行类型", " 拍卖行类型")]
		public byte type;
		/// <summary>
		///  原因
		/// </summary>
		[AdvancedInspector.Descriptor(" 原因", " 原因")]
		public byte reason;
		/// <summary>
		///  拍卖行id
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖行id", " 拍卖行id")]
		public UInt64 auctGuid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, reason);
			BaseDLL.encode_uint64(buffer, ref pos_, auctGuid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref reason);
			BaseDLL.decode_uint64(buffer, ref pos_, ref auctGuid);
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
