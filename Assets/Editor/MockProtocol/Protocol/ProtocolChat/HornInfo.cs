using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 喇叭信息
	/// </summary>
	[AdvancedInspector.Descriptor("喇叭信息", "喇叭信息")]
	public class HornInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 角色id
		/// </summary>
		[AdvancedInspector.Descriptor("角色id", "角色id")]
		public UInt64 roldId;
		/// <summary>
		/// 名字
		/// </summary>
		[AdvancedInspector.Descriptor("名字", "名字")]
		public string name;
		/// <summary>
		/// 职业
		/// </summary>
		[AdvancedInspector.Descriptor("职业", "职业")]
		public byte occu;
		/// <summary>
		/// 等级
		/// </summary>
		[AdvancedInspector.Descriptor("等级", "等级")]
		public UInt16 level;
		/// <summary>
		/// vip等级
		/// </summary>
		[AdvancedInspector.Descriptor("vip等级", "vip等级")]
		public byte viplvl;
		/// <summary>
		///  内容
		/// </summary>
		[AdvancedInspector.Descriptor(" 内容", " 内容")]
		public string content;
		/// <summary>
		///  保护时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 保护时间", " 保护时间")]
		public byte minTime;
		/// <summary>
		///  持续时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 持续时间", " 持续时间")]
		public byte maxTime;
		/// <summary>
		///  combo数
		/// </summary>
		[AdvancedInspector.Descriptor(" combo数", " combo数")]
		public UInt16 combo;
		/// <summary>
		///  连发数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 连发数量", " 连发数量")]
		public byte num;
		/// <summary>
		///  头像框
		/// </summary>
		[AdvancedInspector.Descriptor(" 头像框", " 头像框")]
		public UInt32 headFrame;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roldId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, viplvl);
			byte[] contentBytes = StringHelper.StringToUTF8Bytes(content);
			BaseDLL.encode_string(buffer, ref pos_, contentBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, minTime);
			BaseDLL.encode_int8(buffer, ref pos_, maxTime);
			BaseDLL.encode_uint16(buffer, ref pos_, combo);
			BaseDLL.encode_int8(buffer, ref pos_, num);
			BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roldId);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref viplvl);
			UInt16 contentLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref contentLen);
			byte[] contentBytes = new byte[contentLen];
			for(int i = 0; i < contentLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref contentBytes[i]);
			}
			content = StringHelper.BytesToString(contentBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref minTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref maxTime);
			BaseDLL.decode_uint16(buffer, ref pos_, ref combo);
			BaseDLL.decode_int8(buffer, ref pos_, ref num);
			BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
		}


		#endregion

	}

}
