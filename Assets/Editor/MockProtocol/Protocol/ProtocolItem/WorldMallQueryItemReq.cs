using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询商城道具请求
	/// </summary>
	[AdvancedInspector.Descriptor("查询商城道具请求", "查询商城道具请求")]
	public class WorldMallQueryItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602803;
		public UInt32 Sequence;
		/// <summary>
		/// 商城热门类索引,1-热门
		/// </summary>
		[AdvancedInspector.Descriptor("商城热门类索引,1-热门", "商城热门类索引,1-热门")]
		public byte tagType;
		/// <summary>
		/// 商城主页签
		/// </summary>
		[AdvancedInspector.Descriptor("商城主页签", "商城主页签")]
		public byte type;
		/// <summary>
		/// 商城子页签
		/// </summary>
		[AdvancedInspector.Descriptor("商城子页签", "商城子页签")]
		public byte subType;
		/// <summary>
		/// 货币类别
		/// </summary>
		[AdvancedInspector.Descriptor("货币类别", "货币类别")]
		public byte moneyType;
		/// <summary>
		/// 职业
		/// </summary>
		[AdvancedInspector.Descriptor("职业", "职业")]
		public byte occu;
		/// <summary>
		/// 本地数据更新时间
		/// </summary>
		[AdvancedInspector.Descriptor("本地数据更新时间", "本地数据更新时间")]
		public UInt32 updateTime;
		/// <summary>
		/// 是否私人订制
		/// </summary>
		[AdvancedInspector.Descriptor("是否私人订制", "是否私人订制")]
		public byte isPersonalTailor;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, tagType);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, subType);
			BaseDLL.encode_int8(buffer, ref pos_, moneyType);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint32(buffer, ref pos_, updateTime);
			BaseDLL.encode_int8(buffer, ref pos_, isPersonalTailor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref tagType);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref subType);
			BaseDLL.decode_int8(buffer, ref pos_, ref moneyType);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref updateTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref isPersonalTailor);
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
