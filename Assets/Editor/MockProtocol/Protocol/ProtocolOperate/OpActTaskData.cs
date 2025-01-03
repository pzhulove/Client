using System;
using System.Text;

namespace Mock.Protocol
{

	public class OpActTaskData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 dataid;
		/// <summary>
		/// dataId
		/// </summary>
		[AdvancedInspector.Descriptor("dataId", "dataId")]
		public UInt32 completeNum;
		/// <summary>
		/// 完成数量
		/// </summary>
		[AdvancedInspector.Descriptor("完成数量", "完成数量")]
		public OpTaskReward[] rewards = null;
		/// <summary>
		/// 奖励组
		/// </summary>
		[AdvancedInspector.Descriptor("奖励组", "奖励组")]
		public UInt32[] variables = new UInt32[0];
		/// <summary>
		/// 变量组
		/// </summary>
		[AdvancedInspector.Descriptor("变量组", "变量组")]
		public UInt32[] variables2 = new UInt32[0];
		/// <summary>
		/// 变量组2
		/// </summary>
		[AdvancedInspector.Descriptor("变量组2", "变量组2")]
		public CounterItem[] counters = null;
		/// <summary>
		/// counter组
		/// </summary>
		/// <summary>
		///  任务名
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务名", " 任务名")]
		public string taskName;

		public string[] varProgressName = new string[0];
		/// <summary>
		/// 任务变量进度名
		/// </summary>
		/// <summary>
		/// 开启等级限制(玩家等级)
		/// </summary>
		[AdvancedInspector.Descriptor("开启等级限制(玩家等级)", "开启等级限制(玩家等级)")]
		public UInt16 playerLevelLimit;
		/// <summary>
		///  账户每日领奖限制次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 账户每日领奖限制次数", " 账户每日领奖限制次数")]
		public UInt32 accountDailySubmitLimit;
		/// <summary>
		///  账户总领奖限制次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 账户总领奖限制次数", " 账户总领奖限制次数")]
		public UInt32 accountTotalSubmitLimit;
		/// <summary>
		///  重置类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 重置类型", " 重置类型")]
		public UInt32 resetType;
		/// <summary>
		///  新增3项
		/// </summary>
		[AdvancedInspector.Descriptor(" 新增3项", " 新增3项")]
		public UInt32 cantAccept;

		public UInt32 eventType;

		public UInt32 subType;
		/// <summary>
		///  账户每周领奖限制次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 账户每周领奖限制次数", " 账户每周领奖限制次数")]
		public UInt32 accountWeeklySubmitLimit;
		/// <summary>
		///  账号任务
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号任务", " 账号任务")]
		public UInt32 accountTask;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dataid);
			BaseDLL.encode_uint32(buffer, ref pos_, completeNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewards.Length);
			for(int i = 0; i < rewards.Length; i++)
			{
				rewards[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)variables.Length);
			for(int i = 0; i < variables.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, variables[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)variables2.Length);
			for(int i = 0; i < variables2.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, variables2[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)counters.Length);
			for(int i = 0; i < counters.Length; i++)
			{
				counters[i].encode(buffer, ref pos_);
			}
			byte[] taskNameBytes = StringHelper.StringToUTF8Bytes(taskName);
			BaseDLL.encode_string(buffer, ref pos_, taskNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)varProgressName.Length);
			for(int i = 0; i < varProgressName.Length; i++)
			{
				byte[] varProgressNameBytes = StringHelper.StringToUTF8Bytes(varProgressName[i]);
				BaseDLL.encode_string(buffer, ref pos_, varProgressNameBytes, (UInt16)(buffer.Length - pos_));
			}
			BaseDLL.encode_uint16(buffer, ref pos_, playerLevelLimit);
			BaseDLL.encode_uint32(buffer, ref pos_, accountDailySubmitLimit);
			BaseDLL.encode_uint32(buffer, ref pos_, accountTotalSubmitLimit);
			BaseDLL.encode_uint32(buffer, ref pos_, resetType);
			BaseDLL.encode_uint32(buffer, ref pos_, cantAccept);
			BaseDLL.encode_uint32(buffer, ref pos_, eventType);
			BaseDLL.encode_uint32(buffer, ref pos_, subType);
			BaseDLL.encode_uint32(buffer, ref pos_, accountWeeklySubmitLimit);
			BaseDLL.encode_uint32(buffer, ref pos_, accountTask);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref completeNum);
			UInt16 rewardsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsCnt);
			rewards = new OpTaskReward[rewardsCnt];
			for(int i = 0; i < rewards.Length; i++)
			{
				rewards[i] = new OpTaskReward();
				rewards[i].decode(buffer, ref pos_);
			}
			UInt16 variablesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref variablesCnt);
			variables = new UInt32[variablesCnt];
			for(int i = 0; i < variables.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref variables[i]);
			}
			UInt16 variables2Cnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref variables2Cnt);
			variables2 = new UInt32[variables2Cnt];
			for(int i = 0; i < variables2.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref variables2[i]);
			}
			UInt16 countersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref countersCnt);
			counters = new CounterItem[countersCnt];
			for(int i = 0; i < counters.Length; i++)
			{
				counters[i] = new CounterItem();
				counters[i].decode(buffer, ref pos_);
			}
			UInt16 taskNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref taskNameLen);
			byte[] taskNameBytes = new byte[taskNameLen];
			for(int i = 0; i < taskNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref taskNameBytes[i]);
			}
			taskName = StringHelper.BytesToString(taskNameBytes);
			UInt16 varProgressNameCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref varProgressNameCnt);
			varProgressName = new string[varProgressNameCnt];
			for(int i = 0; i < varProgressName.Length; i++)
			{
				UInt16 varProgressNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref varProgressNameLen);
				byte[] varProgressNameBytes = new byte[varProgressNameLen];
				for(int j = 0; j < varProgressNameLen; j++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref varProgressNameBytes[j]);
				}
				varProgressName[i] = StringHelper.BytesToString(varProgressNameBytes);
			}
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerLevelLimit);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountDailySubmitLimit);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountTotalSubmitLimit);
			BaseDLL.decode_uint32(buffer, ref pos_, ref resetType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref cantAccept);
			BaseDLL.decode_uint32(buffer, ref pos_, ref eventType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref subType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountWeeklySubmitLimit);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountTask);
		}


		#endregion

	}

}
