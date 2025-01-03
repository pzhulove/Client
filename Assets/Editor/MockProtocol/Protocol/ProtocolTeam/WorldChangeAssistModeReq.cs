using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步队员属性
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步队员属性", " 同步队员属性")]
	public class WorldChangeAssistModeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601655;
		public UInt32 Sequence;
		/// <summary>
		///  是否助战
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否助战", " 是否助战")]
		public byte isAssist;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isAssist);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isAssist);
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
