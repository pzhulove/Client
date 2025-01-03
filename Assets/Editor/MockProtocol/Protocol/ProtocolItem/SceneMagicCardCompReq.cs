using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 附魔的附魔卡等级
	/// </summary>
	/// <summary>
	/// 附魔卡合成
	/// </summary>
	[AdvancedInspector.Descriptor("附魔卡合成", "附魔卡合成")]
	public class SceneMagicCardCompReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500946;
		public UInt32 Sequence;

		public UInt64 cardA;
		/// <summary>
		/// 附魔卡A
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡A", "附魔卡A")]
		public UInt64 cardB;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, cardA);
			BaseDLL.encode_uint64(buffer, ref pos_, cardB);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref cardA);
			BaseDLL.decode_uint64(buffer, ref pos_, ref cardB);
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
