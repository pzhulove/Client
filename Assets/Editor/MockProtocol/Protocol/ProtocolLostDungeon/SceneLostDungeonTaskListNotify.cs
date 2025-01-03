using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene->client 任务列表返回
	/// </summary>
	[AdvancedInspector.Descriptor(" scene->client 任务列表返回", " scene->client 任务列表返回")]
	public class SceneLostDungeonTaskListNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510005;
		public UInt32 Sequence;

		public UInt32[] taskList = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskList.Length);
			for(int i = 0; i < taskList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 taskListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref taskListCnt);
			taskList = new UInt32[taskListCnt];
			for(int i = 0; i < taskList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskList[i]);
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
