using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 选择任务请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 选择任务请求", " client->scene 选择任务请求")]
	public class SceneLostDungeonChoiceTaskReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510006;
		public UInt32 Sequence;

		public UInt32 taskId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
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