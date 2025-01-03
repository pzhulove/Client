using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  客户端通信的招募玩家信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 客户端通信的招募玩家信息", " 客户端通信的招募玩家信息")]
	public class HirePlayerData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 角色id
		/// </summary>
		[AdvancedInspector.Descriptor("角色id", "角色id")]
		public UInt64 userId;
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
		/// 在线状态
		/// </summary>
		[AdvancedInspector.Descriptor("在线状态", "在线状态")]
		public byte online;
		/// <summary>
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
		public UInt32 lv;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, userId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_int8(buffer, ref pos_, online);
			BaseDLL.encode_uint32(buffer, ref pos_, lv);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref userId);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_int8(buffer, ref pos_, ref online);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
		}


		#endregion

	}

}
