using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  远征队员信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 远征队员信息", " 远征队员信息")]
	public class ExpeditionMemberInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  角色id
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色id", " 角色id")]
		public UInt64 roleid;
		/// <summary>
		///  角色名
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色名", " 角色名")]
		public string name;
		/// <summary>
		///  角色等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色等级", " 角色等级")]
		public UInt16 level;
		/// <summary>
		///  角色职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色职业", " 角色职业")]
		public byte occu;
		/// <summary>
		///  角色外观
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色外观", " 角色外观")]
		public PlayerAvatar avatar = null;
		/// <summary>
		///  远征地图id
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征地图id", " 远征地图id")]
		public byte expeditionMapId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleid);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, expeditionMapId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleid);
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
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref expeditionMapId);
		}


		#endregion

	}

}
