using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步飞升状态
	/// </summary>
	[AdvancedInspector.Descriptor("同步飞升状态", "同步飞升状态")]
	public class SceneSyncFlyUpStatus : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501070;
		public UInt32 Sequence;
		/// <summary>
		/// FlyUpStatus
		/// </summary>
		[AdvancedInspector.Descriptor("FlyUpStatus", "FlyUpStatus")]
		public byte status;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
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
