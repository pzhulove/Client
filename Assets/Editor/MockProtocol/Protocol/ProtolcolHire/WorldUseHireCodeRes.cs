using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 
	/// </summary>
	[AdvancedInspector.Descriptor("", "")]
	public class WorldUseHireCodeRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601785;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public UInt32 errorCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
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
