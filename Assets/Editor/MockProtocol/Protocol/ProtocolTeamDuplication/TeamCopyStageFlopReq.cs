using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求翻牌
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求翻牌", " 请求翻牌")]
	public class TeamCopyStageFlopReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100035;
		public UInt32 Sequence;
		/// <summary>
		///  阶段id
		/// </summary>
		[AdvancedInspector.Descriptor(" 阶段id", " 阶段id")]
		public UInt32 stageId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, stageId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
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
