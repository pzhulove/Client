using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询冒险通行证情况返回
	/// </summary>
	[AdvancedInspector.Descriptor("查询冒险通行证情况返回", "查询冒险通行证情况返回")]
	public class WorldAventurePassStatusRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609502;
		public UInt32 Sequence;
		/// <summary>
		/// 冒险通行证等级
		/// </summary>
		[AdvancedInspector.Descriptor("冒险通行证等级", "冒险通行证等级")]
		public UInt32 lv;
		/// <summary>
		/// 开始时间
		/// </summary>
		[AdvancedInspector.Descriptor("开始时间", "开始时间")]
		public UInt32 startTime;
		/// <summary>
		/// 结束时间
		/// </summary>
		[AdvancedInspector.Descriptor("结束时间", "结束时间")]
		public UInt32 endTime;
		/// <summary>
		/// 当前赛季id
		/// </summary>
		[AdvancedInspector.Descriptor("当前赛季id", "当前赛季id")]
		public UInt32 seasonID;
		/// <summary>
		/// 经验
		/// </summary>
		[AdvancedInspector.Descriptor("经验", "经验")]
		public UInt32 exp;
		/// <summary>
		/// 通行证类型
		/// </summary>
		[AdvancedInspector.Descriptor("通行证类型", "通行证类型")]
		public byte type;
		/// <summary>
		/// 账号活跃度情况
		/// </summary>
		[AdvancedInspector.Descriptor("账号活跃度情况", "账号活跃度情况")]
		public UInt32 activity;
		/// <summary>
		/// 普通奖励领取情况
		/// </summary>
		[AdvancedInspector.Descriptor("普通奖励领取情况", "普通奖励领取情况")]
		public string normalReward;
		/// <summary>
		/// 高级奖励领取情况
		/// </summary>
		[AdvancedInspector.Descriptor("高级奖励领取情况", "高级奖励领取情况")]
		public string highReward;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, lv);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonID);
			BaseDLL.encode_uint32(buffer, ref pos_, exp);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, activity);
			byte[] normalRewardBytes = StringHelper.StringToUTF8Bytes(normalReward);
			BaseDLL.encode_string(buffer, ref pos_, normalRewardBytes, (UInt16)(buffer.Length - pos_));
			byte[] highRewardBytes = StringHelper.StringToUTF8Bytes(highReward);
			BaseDLL.encode_string(buffer, ref pos_, highRewardBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref exp);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref activity);
			UInt16 normalRewardLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref normalRewardLen);
			byte[] normalRewardBytes = new byte[normalRewardLen];
			for(int i = 0; i < normalRewardLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref normalRewardBytes[i]);
			}
			normalReward = StringHelper.BytesToString(normalRewardBytes);
			UInt16 highRewardLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref highRewardLen);
			byte[] highRewardBytes = new byte[highRewardLen];
			for(int i = 0; i < highRewardLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref highRewardBytes[i]);
			}
			highReward = StringHelper.BytesToString(highRewardBytes);
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
