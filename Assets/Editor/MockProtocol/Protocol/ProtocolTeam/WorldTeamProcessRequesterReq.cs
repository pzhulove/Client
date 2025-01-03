using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  处理申请者（同意、拒绝）
	/// </summary>
	[AdvancedInspector.Descriptor(" 处理申请者（同意、拒绝）", " 处理申请者（同意、拒绝）")]
	public class WorldTeamProcessRequesterReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601640;
		public UInt32 Sequence;
		/// <summary>
		///  目标ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标ID", " 目标ID")]
		public UInt64 targetId;
		/// <summary>
		///  是否同意
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否同意", " 是否同意")]
		public byte agree;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, targetId);
			BaseDLL.encode_int8(buffer, ref pos_, agree);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
			BaseDLL.decode_int8(buffer, ref pos_, ref agree);
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
