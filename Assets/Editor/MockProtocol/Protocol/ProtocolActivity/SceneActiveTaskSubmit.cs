using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 提交活动任务
	/// </summary>
	[AdvancedInspector.Descriptor("提交活动任务", "提交活动任务")]
	public class SceneActiveTaskSubmit : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501130;
		public UInt32 Sequence;

		public UInt32 taskId;

		public UInt32 param1;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			BaseDLL.encode_uint32(buffer, ref pos_, param1);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
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
