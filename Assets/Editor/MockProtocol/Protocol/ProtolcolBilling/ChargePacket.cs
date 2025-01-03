using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  充值礼包
	/// </summary>
	[AdvancedInspector.Descriptor(" 充值礼包", " 充值礼包")]
	public class ChargePacket : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  商品ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品ID", " 商品ID")]
		public byte id;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  原价
		/// </summary>
		[AdvancedInspector.Descriptor(" 原价", " 原价")]
		public UInt16 oldPrice;
		/// <summary>
		///  价格
		/// </summary>
		[AdvancedInspector.Descriptor(" 价格", " 价格")]
		public UInt16 money;
		/// <summary>
		///  获得的vip积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得的vip积分", " 获得的vip积分")]
		public UInt16 vipScore;
		/// <summary>
		///  icon
		/// </summary>
		[AdvancedInspector.Descriptor(" icon", " icon")]
		public string icon;
		/// <summary>
		///  开始时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始时间", " 开始时间")]
		public UInt32 startTime;
		/// <summary>
		///  结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 结束时间", " 结束时间")]
		public UInt32 endTime;
		/// <summary>
		///  当天剩余次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 当天剩余次数", " 当天剩余次数")]
		public UInt16 limitDailyNum;
		/// <summary>
		///  当天剩余次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 当天剩余次数", " 当天剩余次数")]
		public UInt16 limitTotalNum;
		/// <summary>
		///  礼包内容
		/// </summary>
		[AdvancedInspector.Descriptor(" 礼包内容", " 礼包内容")]
		public ItemReward[] rewards = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, oldPrice);
			BaseDLL.encode_uint16(buffer, ref pos_, money);
			BaseDLL.encode_uint16(buffer, ref pos_, vipScore);
			byte[] iconBytes = StringHelper.StringToUTF8Bytes(icon);
			BaseDLL.encode_string(buffer, ref pos_, iconBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			BaseDLL.encode_uint16(buffer, ref pos_, limitDailyNum);
			BaseDLL.encode_uint16(buffer, ref pos_, limitTotalNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewards.Length);
			for(int i = 0; i < rewards.Length; i++)
			{
				rewards[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_uint16(buffer, ref pos_, ref oldPrice);
			BaseDLL.decode_uint16(buffer, ref pos_, ref money);
			BaseDLL.decode_uint16(buffer, ref pos_, ref vipScore);
			UInt16 iconLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref iconLen);
			byte[] iconBytes = new byte[iconLen];
			for(int i = 0; i < iconLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref iconBytes[i]);
			}
			icon = StringHelper.BytesToString(iconBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			BaseDLL.decode_uint16(buffer, ref pos_, ref limitDailyNum);
			BaseDLL.decode_uint16(buffer, ref pos_, ref limitTotalNum);
			UInt16 rewardsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsCnt);
			rewards = new ItemReward[rewardsCnt];
			for(int i = 0; i < rewards.Length; i++)
			{
				rewards[i] = new ItemReward();
				rewards[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
