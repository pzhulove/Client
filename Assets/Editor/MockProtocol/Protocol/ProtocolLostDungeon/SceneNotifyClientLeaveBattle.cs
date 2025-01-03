using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// battleScene->client  通知客户端离开战场
	/// </summary>
	[AdvancedInspector.Descriptor("battleScene->client  通知客户端离开战场", "battleScene->client  通知客户端离开战场")]
	public class SceneNotifyClientLeaveBattle : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2210011;
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
