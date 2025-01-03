using System;
using System.Text;

namespace Mock.Protocol
{

	public class OpActivityData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 dataId;
		/// <summary>
		/// id
		/// </summary>
		[AdvancedInspector.Descriptor("id", "id")]
		public byte state;
		/// <summary>
		/// 状态OpActivityState
		/// </summary>
		[AdvancedInspector.Descriptor("状态OpActivityState", "状态OpActivityState")]
		public UInt32 tmpType;
		/// <summary>
		/// 模板类型OpActivityTmpType
		/// </summary>
		[AdvancedInspector.Descriptor("模板类型OpActivityTmpType", "模板类型OpActivityTmpType")]
		public string name;
		/// <summary>
		/// 活动名
		/// </summary>
		[AdvancedInspector.Descriptor("活动名", "活动名")]
		public byte tag;
		/// <summary>
		/// 活动标签OpActivityTag
		/// </summary>
		[AdvancedInspector.Descriptor("活动标签OpActivityTag", "活动标签OpActivityTag")]
		public UInt32 prepareTime;
		/// <summary>
		/// 准备时间
		/// </summary>
		[AdvancedInspector.Descriptor("准备时间", "准备时间")]
		public UInt32 startTime;
		/// <summary>
		/// 开始时间
		/// </summary>
		[AdvancedInspector.Descriptor("开始时间", "开始时间")]
		public UInt32 endTime;
		/// <summary>
		/// 结束时间
		/// </summary>
		[AdvancedInspector.Descriptor("结束时间", "结束时间")]
		public string desc;
		/// <summary>
		/// 描述
		/// </summary>
		[AdvancedInspector.Descriptor("描述", "描述")]
		public string ruleDesc;
		/// <summary>
		/// 规则描述OpActivityCircleType
		/// </summary>
		[AdvancedInspector.Descriptor("规则描述OpActivityCircleType", "规则描述OpActivityCircleType")]
		public byte circleType;
		/// <summary>
		/// 循环类型
		/// </summary>
		[AdvancedInspector.Descriptor("循环类型", "循环类型")]
		public OpActTaskData[] tasks = null;
		/// <summary>
		/// 任务信息
		/// </summary>
		[AdvancedInspector.Descriptor("任务信息", "任务信息")]
		public string taskDesc;
		/// <summary>
		/// 任务描述
		/// </summary>
		/// <summary>
		/// 扩展参数
		/// </summary>
		[AdvancedInspector.Descriptor("扩展参数", "扩展参数")]
		public UInt32 parm;
		/// <summary>
		/// 扩展参数2
		/// </summary>
		[AdvancedInspector.Descriptor("扩展参数2", "扩展参数2")]
		public UInt32[] parm2 = new UInt32[0];
		/// <summary>
		/// 开启等级限制(玩家等级)
		/// </summary>
		[AdvancedInspector.Descriptor("开启等级限制(玩家等级)", "开启等级限制(玩家等级)")]
		public UInt16 playerLevelLimit;
		/// <summary>
		/// logo描述
		/// </summary>
		[AdvancedInspector.Descriptor("logo描述", "logo描述")]
		public string logoDesc;
		/// <summary>
		/// 活动相关count参数
		/// </summary>
		[AdvancedInspector.Descriptor("活动相关count参数", "活动相关count参数")]
		public string countParam;
		/// <summary>
		/// 扩展参数3
		/// </summary>
		[AdvancedInspector.Descriptor("扩展参数3", "扩展参数3")]
		public UInt32[] parm3 = new UInt32[0];
		/// <summary>
		/// 活动预制体路径
		/// </summary>
		[AdvancedInspector.Descriptor("活动预制体路径", "活动预制体路径")]
		public string prefabPath;
		/// <summary>
		/// 宣传图路径
		/// </summary>
		[AdvancedInspector.Descriptor("宣传图路径", "宣传图路径")]
		public string logoPath;
		/// <summary>
		/// 字符串参数
		/// </summary>
		[AdvancedInspector.Descriptor("字符串参数", "字符串参数")]
		public string[] strParams = new string[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_int8(buffer, ref pos_, state);
			BaseDLL.encode_uint32(buffer, ref pos_, tmpType);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, tag);
			BaseDLL.encode_uint32(buffer, ref pos_, prepareTime);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			byte[] descBytes = StringHelper.StringToUTF8Bytes(desc);
			BaseDLL.encode_string(buffer, ref pos_, descBytes, (UInt16)(buffer.Length - pos_));
			byte[] ruleDescBytes = StringHelper.StringToUTF8Bytes(ruleDesc);
			BaseDLL.encode_string(buffer, ref pos_, ruleDescBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, circleType);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tasks.Length);
			for(int i = 0; i < tasks.Length; i++)
			{
				tasks[i].encode(buffer, ref pos_);
			}
			byte[] taskDescBytes = StringHelper.StringToUTF8Bytes(taskDesc);
			BaseDLL.encode_string(buffer, ref pos_, taskDescBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, parm);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parm2.Length);
			for(int i = 0; i < parm2.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, parm2[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, playerLevelLimit);
			byte[] logoDescBytes = StringHelper.StringToUTF8Bytes(logoDesc);
			BaseDLL.encode_string(buffer, ref pos_, logoDescBytes, (UInt16)(buffer.Length - pos_));
			byte[] countParamBytes = StringHelper.StringToUTF8Bytes(countParam);
			BaseDLL.encode_string(buffer, ref pos_, countParamBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parm3.Length);
			for(int i = 0; i < parm3.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, parm3[i]);
			}
			byte[] prefabPathBytes = StringHelper.StringToUTF8Bytes(prefabPath);
			BaseDLL.encode_string(buffer, ref pos_, prefabPathBytes, (UInt16)(buffer.Length - pos_));
			byte[] logoPathBytes = StringHelper.StringToUTF8Bytes(logoPath);
			BaseDLL.encode_string(buffer, ref pos_, logoPathBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)strParams.Length);
			for(int i = 0; i < strParams.Length; i++)
			{
				byte[] strParamsBytes = StringHelper.StringToUTF8Bytes(strParams[i]);
				BaseDLL.encode_string(buffer, ref pos_, strParamsBytes, (UInt16)(buffer.Length - pos_));
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
			BaseDLL.decode_uint32(buffer, ref pos_, ref tmpType);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref tag);
			BaseDLL.decode_uint32(buffer, ref pos_, ref prepareTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			UInt16 descLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref descLen);
			byte[] descBytes = new byte[descLen];
			for(int i = 0; i < descLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref descBytes[i]);
			}
			desc = StringHelper.BytesToString(descBytes);
			UInt16 ruleDescLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref ruleDescLen);
			byte[] ruleDescBytes = new byte[ruleDescLen];
			for(int i = 0; i < ruleDescLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref ruleDescBytes[i]);
			}
			ruleDesc = StringHelper.BytesToString(ruleDescBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref circleType);
			UInt16 tasksCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref tasksCnt);
			tasks = new OpActTaskData[tasksCnt];
			for(int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = new OpActTaskData();
				tasks[i].decode(buffer, ref pos_);
			}
			UInt16 taskDescLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref taskDescLen);
			byte[] taskDescBytes = new byte[taskDescLen];
			for(int i = 0; i < taskDescLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref taskDescBytes[i]);
			}
			taskDesc = StringHelper.BytesToString(taskDescBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref parm);
			UInt16 parm2Cnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref parm2Cnt);
			parm2 = new UInt32[parm2Cnt];
			for(int i = 0; i < parm2.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref parm2[i]);
			}
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerLevelLimit);
			UInt16 logoDescLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref logoDescLen);
			byte[] logoDescBytes = new byte[logoDescLen];
			for(int i = 0; i < logoDescLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref logoDescBytes[i]);
			}
			logoDesc = StringHelper.BytesToString(logoDescBytes);
			UInt16 countParamLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref countParamLen);
			byte[] countParamBytes = new byte[countParamLen];
			for(int i = 0; i < countParamLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref countParamBytes[i]);
			}
			countParam = StringHelper.BytesToString(countParamBytes);
			UInt16 parm3Cnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref parm3Cnt);
			parm3 = new UInt32[parm3Cnt];
			for(int i = 0; i < parm3.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref parm3[i]);
			}
			UInt16 prefabPathLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref prefabPathLen);
			byte[] prefabPathBytes = new byte[prefabPathLen];
			for(int i = 0; i < prefabPathLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref prefabPathBytes[i]);
			}
			prefabPath = StringHelper.BytesToString(prefabPathBytes);
			UInt16 logoPathLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref logoPathLen);
			byte[] logoPathBytes = new byte[logoPathLen];
			for(int i = 0; i < logoPathLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref logoPathBytes[i]);
			}
			logoPath = StringHelper.BytesToString(logoPathBytes);
			UInt16 strParamsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref strParamsCnt);
			strParams = new string[strParamsCnt];
			for(int i = 0; i < strParams.Length; i++)
			{
				UInt16 strParamsLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref strParamsLen);
				byte[] strParamsBytes = new byte[strParamsLen];
				for(int j = 0; j < strParamsLen; j++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref strParamsBytes[j]);
				}
				strParams[i] = StringHelper.BytesToString(strParamsBytes);
			}
		}


		#endregion

	}

}
