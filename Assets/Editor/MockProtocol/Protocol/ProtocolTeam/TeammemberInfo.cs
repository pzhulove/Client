using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  队伍成员信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 队伍成员信息", " 队伍成员信息")]
	public class TeammemberInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// id
		/// </summary>
		[AdvancedInspector.Descriptor("id", "id")]
		public UInt64 id;
		/// <summary>
		/// 名字
		/// </summary>
		[AdvancedInspector.Descriptor("名字", "名字")]
		public string name;
		/// <summary>
		/// 等级
		/// </summary>
		[AdvancedInspector.Descriptor("等级", "等级")]
		public UInt16 level;
		/// <summary>
		/// 职业
		/// </summary>
		[AdvancedInspector.Descriptor("职业", "职业")]
		public byte occu;
		/// <summary>
		///  状态掩码（对应枚举TeamMemberStatusMask）
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态掩码（对应枚举TeamMemberStatusMask）", " 状态掩码（对应枚举TeamMemberStatusMask）")]
		public byte statusMask;
		/// <summary>
		///  外观
		/// </summary>
		[AdvancedInspector.Descriptor(" 外观", " 外观")]
		public PlayerAvatar avatar = null;
		/// <summary>
		///  剩余次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余次数", " 剩余次数")]
		public UInt32 remainTimes;
		/// <summary>
		///  公会ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会ID", " 公会ID")]
		public UInt64 guildId;
		/// <summary>
		///  vip等级
		/// </summary>
		[AdvancedInspector.Descriptor(" vip等级", " vip等级")]
		public byte vipLevel;
		/// <summary>
		///  抗魔值
		/// </summary>
		[AdvancedInspector.Descriptor(" 抗魔值", " 抗魔值")]
		public UInt32 resistMagic;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_int8(buffer, ref pos_, statusMask);
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, remainTimes);
			BaseDLL.encode_uint64(buffer, ref pos_, guildId);
			BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, resistMagic);
			playerLabelInfo.encode(buffer, ref pos_);
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
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_int8(buffer, ref pos_, ref statusMask);
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainTimes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
			BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref resistMagic);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
