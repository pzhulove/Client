using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步黑市商人类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步黑市商人类型", " 同步黑市商人类型")]
	public class WorldBlackMarketSyncType : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609001;
		public UInt32 Sequence;
		/// <summary>
		///  商人类型(BlackMarketType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 商人类型(BlackMarketType)", " 商人类型(BlackMarketType)")]
		public byte type;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
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
