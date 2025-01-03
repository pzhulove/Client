using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  圆桌会议成员信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 圆桌会议成员信息", " 圆桌会议成员信息")]
	public class GuildTableMember : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色ID", " 角色ID")]
		public UInt64 id;
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
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public byte seat;
		/// <summary>
		///  参与类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与类型", " 参与类型")]
		public byte type;
		/// <summary>
		///  玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家标签信息", " 玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			playerLabelInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
