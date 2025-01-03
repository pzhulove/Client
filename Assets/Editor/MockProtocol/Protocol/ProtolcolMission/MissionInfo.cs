using System;
using System.Text;

namespace Mock.Protocol
{

	public class MissionInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 taskID;

		public byte status;

		public MissionPair[] akMissionPairs = null;

		public UInt32 finTime;

		public byte submitCount;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)akMissionPairs.Length);
			for(int i = 0; i < akMissionPairs.Length; i++)
			{
				akMissionPairs[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, finTime);
			BaseDLL.encode_int8(buffer, ref pos_, submitCount);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			UInt16 akMissionPairsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref akMissionPairsCnt);
			akMissionPairs = new MissionPair[akMissionPairsCnt];
			for(int i = 0; i < akMissionPairs.Length; i++)
			{
				akMissionPairs[i] = new MissionPair();
				akMissionPairs[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref finTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref submitCount);
		}


		#endregion

	}

}
