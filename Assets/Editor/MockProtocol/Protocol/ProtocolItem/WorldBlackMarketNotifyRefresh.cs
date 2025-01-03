using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 通知客户端重新拉去列表
	/// </summary>
	[AdvancedInspector.Descriptor("通知客户端重新拉去列表", "通知客户端重新拉去列表")]
	public class WorldBlackMarketNotifyRefresh : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609008;
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
