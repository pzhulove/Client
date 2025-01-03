using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 发布师门日常任务返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 发布师门日常任务返回", "world->client 发布师门日常任务返回")]
	public class WorldPublishMasterDialyTaskRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601753;
		public UInt32 Sequence;

		public UInt32 code;

		public UInt64 discipleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
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
