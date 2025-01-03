using System;
using System.Text;

namespace Mock.Protocol
{

	public class MasterTaskShareData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 academicTotalGrowth;

		public UInt32 masterDailyTaskGrowth;

		public UInt32 masterAcademicTaskGrowth;

		public UInt32 masterUplevelGrowth;

		public UInt32 masterGiveEquipGrowth;

		public UInt32 masterGiveGiftGrowth;

		public UInt32 masterTeamClearDungeonGrowth;

		public UInt32 goodTeachValue;

		public MissionInfo[] dailyTasks = null;

		public MissionInfo[] academicTasks = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, academicTotalGrowth);
			BaseDLL.encode_uint32(buffer, ref pos_, masterDailyTaskGrowth);
			BaseDLL.encode_uint32(buffer, ref pos_, masterAcademicTaskGrowth);
			BaseDLL.encode_uint32(buffer, ref pos_, masterUplevelGrowth);
			BaseDLL.encode_uint32(buffer, ref pos_, masterGiveEquipGrowth);
			BaseDLL.encode_uint32(buffer, ref pos_, masterGiveGiftGrowth);
			BaseDLL.encode_uint32(buffer, ref pos_, masterTeamClearDungeonGrowth);
			BaseDLL.encode_uint32(buffer, ref pos_, goodTeachValue);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dailyTasks.Length);
			for(int i = 0; i < dailyTasks.Length; i++)
			{
				dailyTasks[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)academicTasks.Length);
			for(int i = 0; i < academicTasks.Length; i++)
			{
				academicTasks[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref academicTotalGrowth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref masterDailyTaskGrowth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref masterAcademicTaskGrowth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref masterUplevelGrowth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref masterGiveEquipGrowth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref masterGiveGiftGrowth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref masterTeamClearDungeonGrowth);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goodTeachValue);
			UInt16 dailyTasksCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dailyTasksCnt);
			dailyTasks = new MissionInfo[dailyTasksCnt];
			for(int i = 0; i < dailyTasks.Length; i++)
			{
				dailyTasks[i] = new MissionInfo();
				dailyTasks[i].decode(buffer, ref pos_);
			}
			UInt16 academicTasksCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref academicTasksCnt);
			academicTasks = new MissionInfo[academicTasksCnt];
			for(int i = 0; i < academicTasks.Length; i++)
			{
				academicTasks[i] = new MissionInfo();
				academicTasks[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
