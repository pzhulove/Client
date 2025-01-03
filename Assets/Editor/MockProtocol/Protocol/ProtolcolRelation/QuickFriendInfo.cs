using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  查找的名字
	/// </summary>
	/// <summary>
	/// 推荐关系结构
	/// </summary>
	[AdvancedInspector.Descriptor("推荐关系结构", "推荐关系结构")]
	public class QuickFriendInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 玩家id
		/// </summary>
		[AdvancedInspector.Descriptor("玩家id", "玩家id")]
		public UInt64 playerId;
		/// <summary>
		/// 姓名
		/// </summary>
		[AdvancedInspector.Descriptor("姓名", "姓名")]
		public string name;
		/// <summary>
		/// 职业
		/// </summary>
		[AdvancedInspector.Descriptor("职业", "职业")]
		public byte occu;
		/// <summary>
		/// 性别
		/// </summary>
		[AdvancedInspector.Descriptor("性别", "性别")]
		public UInt32 seasonLv;
		/// <summary>
		/// 等级
		/// </summary>
		[AdvancedInspector.Descriptor("等级", "等级")]
		public UInt16 level;
		/// <summary>
		/// vip等级
		/// </summary>
		[AdvancedInspector.Descriptor("vip等级", "vip等级")]
		public byte vipLv;
		/// <summary>
		/// 师傅公告
		/// </summary>
		[AdvancedInspector.Descriptor("师傅公告", "师傅公告")]
		public string masterNote;
		/// <summary>
		/// 外观信息
		/// </summary>
		[AdvancedInspector.Descriptor("外观信息", "外观信息")]
		public PlayerAvatar avatar = null;
		/// <summary>
		/// 在线时间类型
		/// </summary>
		[AdvancedInspector.Descriptor("在线时间类型", "在线时间类型")]
		public byte activeTimeType;
		/// <summary>
		/// 师傅类型
		/// </summary>
		[AdvancedInspector.Descriptor("师傅类型", "师傅类型")]
		public byte masterType;
		/// <summary>
		/// 地区id
		/// </summary>
		[AdvancedInspector.Descriptor("地区id", "地区id")]
		public byte regionId;
		/// <summary>
		/// 宣言
		/// </summary>
		[AdvancedInspector.Descriptor("宣言", "宣言")]
		public string declaration;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLv);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, vipLv);
			byte[] masterNoteBytes = StringHelper.StringToUTF8Bytes(masterNote);
			BaseDLL.encode_string(buffer, ref pos_, masterNoteBytes, (UInt16)(buffer.Length - pos_));
			avatar.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, activeTimeType);
			BaseDLL.encode_int8(buffer, ref pos_, masterType);
			BaseDLL.encode_int8(buffer, ref pos_, regionId);
			byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
			BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			playerLabelInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLv);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref vipLv);
			UInt16 masterNoteLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref masterNoteLen);
			byte[] masterNoteBytes = new byte[masterNoteLen];
			for(int i = 0; i < masterNoteLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref masterNoteBytes[i]);
			}
			masterNote = StringHelper.BytesToString(masterNoteBytes);
			avatar.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref activeTimeType);
			BaseDLL.decode_int8(buffer, ref pos_, ref masterType);
			BaseDLL.decode_int8(buffer, ref pos_, ref regionId);
			UInt16 declarationLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
			byte[] declarationBytes = new byte[declarationLen];
			for(int i = 0; i < declarationLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
			}
			declaration = StringHelper.BytesToString(declarationBytes);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
