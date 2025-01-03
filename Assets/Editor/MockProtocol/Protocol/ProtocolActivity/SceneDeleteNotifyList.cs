using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求删除通知
	/// </summary>
	[AdvancedInspector.Descriptor("请求删除通知", "请求删除通知")]
	public class SceneDeleteNotifyList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501155;
		public UInt32 Sequence;

		public NotifyInfo notify = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			notify.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			notify.decode(buffer, ref pos_);
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
