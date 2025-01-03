using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 发布师门日常任务请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 发布师门日常任务请求", "client->world 发布师门日常任务请求")]
	public class WorldPublishMasterDialyTaskReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601752;
		public UInt32 Sequence;

		public UInt64 discipleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
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
