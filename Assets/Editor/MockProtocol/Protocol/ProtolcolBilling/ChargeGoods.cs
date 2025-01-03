using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  充值商品
	/// </summary>
	[AdvancedInspector.Descriptor(" 充值商品", " 充值商品")]
	public class ChargeGoods : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  商品ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品ID", " 商品ID")]
		public byte id;
		/// <summary>
		///  描述
		/// </summary>
		[AdvancedInspector.Descriptor(" 描述", " 描述")]
		public string desc;
		/// <summary>
		///  标签（位组合）
		/// </summary>
		[AdvancedInspector.Descriptor(" 标签（位组合）", " 标签（位组合）")]
		public UInt32 tags;
		/// <summary>
		///  充值金额
		/// </summary>
		[AdvancedInspector.Descriptor(" 充值金额", " 充值金额")]
		public UInt16 money;
		/// <summary>
		///  获得的vip积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得的vip积分", " 获得的vip积分")]
		public UInt16 vipScore;
		/// <summary>
		///  道具ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具ID", " 道具ID")]
		public UInt32 itemId;
		/// <summary>
		///  道具数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具数量", " 道具数量")]
		public UInt16 num;
		/// <summary>
		///  首冲数量补偿
		/// </summary>
		[AdvancedInspector.Descriptor(" 首冲数量补偿", " 首冲数量补偿")]
		public UInt16 firstAddNum;
		/// <summary>
		///  非首充数量补偿
		/// </summary>
		[AdvancedInspector.Descriptor(" 非首充数量补偿", " 非首充数量补偿")]
		public UInt16 unfirstAddNum;
		/// <summary>
		///  是否是首充
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否是首充", " 是否是首充")]
		public byte isFirstCharge;
		/// <summary>
		///  icon
		/// </summary>
		[AdvancedInspector.Descriptor(" icon", " icon")]
		public string icon;
		/// <summary>
		///  剩余天数
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余天数", " 剩余天数")]
		public UInt32 remainDays;
		/// <summary>
		///  剩余次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余次数", " 剩余次数")]
		public byte remainTimes;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, id);
			byte[] descBytes = StringHelper.StringToUTF8Bytes(desc);
			BaseDLL.encode_string(buffer, ref pos_, descBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, tags);
			BaseDLL.encode_uint16(buffer, ref pos_, money);
			BaseDLL.encode_uint16(buffer, ref pos_, vipScore);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint16(buffer, ref pos_, num);
			BaseDLL.encode_uint16(buffer, ref pos_, firstAddNum);
			BaseDLL.encode_uint16(buffer, ref pos_, unfirstAddNum);
			BaseDLL.encode_int8(buffer, ref pos_, isFirstCharge);
			byte[] iconBytes = StringHelper.StringToUTF8Bytes(icon);
			BaseDLL.encode_string(buffer, ref pos_, iconBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, remainDays);
			BaseDLL.encode_int8(buffer, ref pos_, remainTimes);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref id);
			UInt16 descLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref descLen);
			byte[] descBytes = new byte[descLen];
			for(int i = 0; i < descLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref descBytes[i]);
			}
			desc = StringHelper.BytesToString(descBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref tags);
			BaseDLL.decode_uint16(buffer, ref pos_, ref money);
			BaseDLL.decode_uint16(buffer, ref pos_, ref vipScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			BaseDLL.decode_uint16(buffer, ref pos_, ref firstAddNum);
			BaseDLL.decode_uint16(buffer, ref pos_, ref unfirstAddNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref isFirstCharge);
			UInt16 iconLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref iconLen);
			byte[] iconBytes = new byte[iconLen];
			for(int i = 0; i < iconLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref iconBytes[i]);
			}
			icon = StringHelper.BytesToString(iconBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainDays);
			BaseDLL.decode_int8(buffer, ref pos_, ref remainTimes);
		}


		#endregion

	}

}
