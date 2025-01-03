using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene->client 任务数据通知
	/// </summary>
	[AdvancedInspector.Descriptor(" scene->client 任务数据通知", " scene->client 任务数据通知")]
	public class SceneLostDungeonTaskDataNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510014;
		public UInt32 Sequence;

		public UInt32 taskId;

		public UInt32 status;

		public LostDungeonTaskVar[] ldTaskVar = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			BaseDLL.encode_uint32(buffer, ref pos_, status);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ldTaskVar.Length);
			for(int i = 0; i < ldTaskVar.Length; i++)
			{
				ldTaskVar[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref status);
			UInt16 ldTaskVarCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref ldTaskVarCnt);
			ldTaskVar = new LostDungeonTaskVar[ldTaskVarCnt];
			for(int i = 0; i < ldTaskVar.Length; i++)
			{
				ldTaskVar[i] = new LostDungeonTaskVar();
				ldTaskVar[i].decode(buffer, ref pos_);
			}
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
