using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ************************************************************
	/// </summary>
	/// <summary>
	///  射手的历史战绩
	/// </summary>
	[AdvancedInspector.Descriptor(" 射手的历史战绩", " 射手的历史战绩")]
	public class ShooterHistoryRecordReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708305;
		public UInt32 Sequence;
		/// <summary>
		///  射手id
		/// </summary>
		[AdvancedInspector.Descriptor(" 射手id", " 射手id")]
		public UInt32 id;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
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
