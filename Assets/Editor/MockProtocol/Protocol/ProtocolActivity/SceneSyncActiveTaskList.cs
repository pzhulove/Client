using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步活动任务列表
	/// </summary>
	[AdvancedInspector.Descriptor("同步活动任务列表", "同步活动任务列表")]
	public class SceneSyncActiveTaskList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501129;
		public UInt32 Sequence;

		public MissionInfo[] tasks = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
			for(int i = 0; i < tasks.Length; i++)
			{
				tasks[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 tasksCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
			tasks = new MissionInfo[tasksCnt];
			for(int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = new MissionInfo();
				tasks[i].decode(buffer, ref pos_);
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
