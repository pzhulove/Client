using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  *****************************************************
	/// </summary>
	/// <summary>
	///  请求射手赔率
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求射手赔率", " 请求射手赔率")]
	public class ShooterOddsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708303;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
