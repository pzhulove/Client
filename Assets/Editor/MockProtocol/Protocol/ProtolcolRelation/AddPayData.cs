using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 增加代付
	/// </summary>
	[AdvancedInspector.Descriptor("增加代付", "增加代付")]
	public class AddPayData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601723;
		public UInt32 Sequence;

		public AddonPayData data = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			data.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			data.decode(buffer, ref pos_);
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
