using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  红包基础信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 红包基础信息", " 红包基础信息")]
	public class RedPacketBaseEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  红包ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包ID", " 红包ID")]
		public UInt64 id;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  发送者ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 发送者ID", " 发送者ID")]
		public UInt64 ownerId;
		/// <summary>
		///  发送者名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 发送者名字", " 发送者名字")]
		public string ownerName;
		/// <summary>
		///  状态（对应枚举RedPacketStatus）
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态（对应枚举RedPacketStatus）", " 状态（对应枚举RedPacketStatus）")]
		public byte status;
		/// <summary>
		///  有没有打开过
		/// </summary>
		[AdvancedInspector.Descriptor(" 有没有打开过", " 有没有打开过")]
		public byte opened;
		/// <summary>
		///  红包类型(对应枚举RedPacketType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包类型(对应枚举RedPacketType)", " 红包类型(对应枚举RedPacketType)")]
		public byte type;
		/// <summary>
		///  红包来源
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包来源", " 红包来源")]
		public UInt16 reason;
		/// <summary>
		///  红包金额
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包金额", " 红包金额")]
		public UInt32 totalMoney;
		/// <summary>
		///  红包数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包数量", " 红包数量")]
		public byte totalNum;
		/// <summary>
		///  红包剩余数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包剩余数量", " 红包剩余数量")]
		public byte remainNum;
		/// <summary>
		///  公会系列战场次时间戳
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会系列战场次时间戳", " 公会系列战场次时间戳")]
		public UInt32 guildTimeStamp;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, ownerId);
			byte[] ownerNameBytes = StringHelper.StringToUTF8Bytes(ownerName);
			BaseDLL.encode_string(buffer, ref pos_, ownerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_int8(buffer, ref pos_, opened);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, reason);
			BaseDLL.encode_uint32(buffer, ref pos_, totalMoney);
			BaseDLL.encode_int8(buffer, ref pos_, totalNum);
			BaseDLL.encode_int8(buffer, ref pos_, remainNum);
			BaseDLL.encode_uint32(buffer, ref pos_, guildTimeStamp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref ownerId);
			UInt16 ownerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref ownerNameLen);
			byte[] ownerNameBytes = new byte[ownerNameLen];
			for(int i = 0; i < ownerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref ownerNameBytes[i]);
			}
			ownerName = StringHelper.BytesToString(ownerNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_int8(buffer, ref pos_, ref opened);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint16(buffer, ref pos_, ref reason);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalMoney);
			BaseDLL.decode_int8(buffer, ref pos_, ref totalNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref remainNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref guildTimeStamp);
		}


		#endregion

	}

}
