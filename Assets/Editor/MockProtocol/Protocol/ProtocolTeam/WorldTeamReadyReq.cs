using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  准备
	/// </summary>
	[AdvancedInspector.Descriptor(" 准备", " 准备")]
	public class WorldTeamReadyReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601632;
		public UInt32 Sequence;
		/// <summary>
		///  是否准备好(0:取消 1:准备)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否准备好(0:取消 1:准备)", " 是否准备好(0:取消 1:准备)")]
		public byte ready;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, ready);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref ready);
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
