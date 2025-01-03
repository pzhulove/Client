using System;
using System.Text;

namespace Mock.Protocol
{
	public class TaskBriefInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		public UInt32 taskID;
		public byte status;
		public TaskPair[] taskPairs = null;
		public UInt32 finTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskPairs.Length);
			for(int i = 0; i < taskPairs.Length; i++)
			{
				taskPairs[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, finTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			UInt16 taskPairsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref taskPairsCnt);
			taskPairs = new TaskPair[taskPairsCnt];
			for(int i = 0; i < taskPairs.Length; i++)
			{
				taskPairs[i] = new TaskPair();
				taskPairs[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref finTime);
		}

		public uint GetSequence()
		{
			return 0;
		}

		public void SetSequence(uint seq)
		{
			return ;
		}
		#endregion

	}

}
