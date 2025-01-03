using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会会长信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会会长信息", " 公会会长信息")]
	public class GuildLeaderInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ID", " ID")]
		public UInt64 id;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业", " 职业")]
		public byte occu;
		/// <summary>
		///  外观
		/// </summary>
		[AdvancedInspector.Descriptor(" 外观", " 外观")]
		public PlayerAvatar avatar = null;
		/// <summary>
		///  人气
		/// </summary>
		[AdvancedInspector.Descriptor(" 人气", " 人气")]
		public UInt32 popularoty;
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
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, popularoty);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref popularoty);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
