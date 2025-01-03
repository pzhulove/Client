using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步活动任务状态
	/// </summary>
	[AdvancedInspector.Descriptor("同步活动任务状态", "同步活动任务状态")]
	public class SceneNotifyActiveTaskStatus : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501127;
		public UInt32 Sequence;

		public UInt32 taskId;

		public byte status;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			BaseDLL.encode_int8(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
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
