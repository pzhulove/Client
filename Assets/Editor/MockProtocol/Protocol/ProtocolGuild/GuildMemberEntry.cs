using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会成员
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会成员", " 公会成员")]
	public class GuildMemberEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt64 id;
		/// <summary>
		///  name
		/// </summary>
		[AdvancedInspector.Descriptor(" name", " name")]
		public string name;
		/// <summary>
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
		public UInt16 level;
		/// <summary>
		///  职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业", " 职业")]
		public byte occu;
		/// <summary>
		///  职务(对应枚举GuildPost)
		/// </summary>
		[AdvancedInspector.Descriptor(" 职务(对应枚举GuildPost)", " 职务(对应枚举GuildPost)")]
		public byte post;
		/// <summary>
		///  历史贡献
		/// </summary>
		[AdvancedInspector.Descriptor(" 历史贡献", " 历史贡献")]
		public UInt32 contribution;
		/// <summary>
		///  离线时间(0代表在线)
		/// </summary>
		[AdvancedInspector.Descriptor(" 离线时间(0代表在线)", " 离线时间(0代表在线)")]
		public UInt32 logoutTime;
		/// <summary>
		///  活跃度
		/// </summary>
		[AdvancedInspector.Descriptor(" 活跃度", " 活跃度")]
		public UInt32 activeDegree;
		/// <summary>
		/// vip等级
		/// </summary>
		[AdvancedInspector.Descriptor("vip等级", "vip等级")]
		public byte vipLevel;
		/// <summary>
		///  玩家段位
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家段位", " 玩家段位")]
		public UInt32 seasonLevel;
		/// <summary>
		///  玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家标签信息", " 玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_int8(buffer, ref pos_, post);
			BaseDLL.encode_uint32(buffer, ref pos_, contribution);
			BaseDLL.encode_uint32(buffer, ref pos_, logoutTime);
			BaseDLL.encode_uint32(buffer, ref pos_, activeDegree);
			BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref post);
			BaseDLL.decode_uint32(buffer, ref pos_, ref contribution);
			BaseDLL.decode_uint32(buffer, ref pos_, ref logoutTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref activeDegree);
			BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
