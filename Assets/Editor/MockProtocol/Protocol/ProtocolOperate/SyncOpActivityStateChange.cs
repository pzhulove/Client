using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步运营活动状态变化
	/// </summary>
	[AdvancedInspector.Descriptor("同步运营活动状态变化", "同步运营活动状态变化")]
	public class SyncOpActivityStateChange : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501149;
		public UInt32 Sequence;

		public OpActivityData data = null;

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
