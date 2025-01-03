using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 队员信息
	/// </summary>
	[AdvancedInspector.Descriptor("队员信息", "队员信息")]
	public class LostDungTeamMember : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 角色id
		/// </summary>
		[AdvancedInspector.Descriptor("角色id", "角色id")]
		public UInt64 roleId;
		/// <summary>
		/// 队伍id
		/// </summary>
		[AdvancedInspector.Descriptor("队伍id", "队伍id")]
		public UInt32 teamId;
		/// <summary>
		/// 队伍中位置
		/// </summary>
		[AdvancedInspector.Descriptor("队伍中位置", "队伍中位置")]
		public byte pos;
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

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_int8(buffer, ref pos_, pos);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
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
		}


		#endregion

	}

}
