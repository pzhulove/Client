using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会请求者信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会请求者信息", " 公会请求者信息")]
	public class GuildRequesterInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
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
		/// vip等级
		/// </summary>
		[AdvancedInspector.Descriptor("vip等级", "vip等级")]
		public byte vipLevel;
		/// <summary>
		/// 申请时间
		/// </summary>
		[AdvancedInspector.Descriptor("申请时间", "申请时间")]
		public UInt32 requestTime;
		/// <summary>
		///  玩家段位
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家段位", " 玩家段位")]
		public UInt32 seasonLevel;
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
			BaseDLL.encode_int8(buffer, ref pos_, vipLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, requestTime);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref vipLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref requestTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			playerLabelInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
