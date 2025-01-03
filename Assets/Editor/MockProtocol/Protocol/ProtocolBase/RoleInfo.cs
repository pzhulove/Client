using System;
using System.Text;

namespace Mock.Protocol
{

	public class RoleInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleId;

		public string strRoleId;

		public string name;

		public byte sex;

		public byte occupation;

		public UInt16 level;

		public UInt32 offlineTime;

		public UInt32 deleteTime;

		public PlayerAvatar avatar = null;

		public UInt32 newboot;

		public byte preOccu;
		/// <summary>
		///  是否是预约角色
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否是预约角色", " 是否是预约角色")]
		public byte isAppointmentOccu;
		/// <summary>
		/// 是否老兵回归
		/// </summary>
		[AdvancedInspector.Descriptor("是否老兵回归", "是否老兵回归")]
		public byte isVeteranReturn;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			byte[] strRoleIdBytes = StringHelper.StringToUTF8Bytes(strRoleId);
			BaseDLL.encode_string(buffer, ref pos_, strRoleIdBytes, (UInt16)(buffer.Length - pos_));
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, sex);
			BaseDLL.encode_int8(buffer, ref pos_, occupation);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_uint32(buffer, ref pos_, offlineTime);
			BaseDLL.encode_uint32(buffer, ref pos_, deleteTime);
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, newboot);
			BaseDLL.encode_int8(buffer, ref pos_, preOccu);
			BaseDLL.encode_int8(buffer, ref pos_, isAppointmentOccu);
			BaseDLL.encode_int8(buffer, ref pos_, isVeteranReturn);
			playerLabelInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			UInt16 strRoleIdLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref strRoleIdLen);
			byte[] strRoleIdBytes = new byte[strRoleIdLen];
			for(int i = 0; i < strRoleIdLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref strRoleIdBytes[i]);
			}
			strRoleId = StringHelper.BytesToString(strRoleIdBytes);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref sex);
			BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_uint32(buffer, ref pos_, ref offlineTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref deleteTime);
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newboot);
			BaseDLL.decode_int8(buffer, ref pos_, ref preOccu);
			BaseDLL.decode_int8(buffer, ref pos_, ref isAppointmentOccu);
			BaseDLL.decode_int8(buffer, ref pos_, ref isVeteranReturn);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
