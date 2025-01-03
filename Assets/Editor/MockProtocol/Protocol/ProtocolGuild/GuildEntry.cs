using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会信息", " 公会信息")]
	public class GuildEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
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
		///  公会等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会等级", " 公会等级")]
		public byte level;
		/// <summary>
		///  公会人数
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会人数", " 公会人数")]
		public byte memberNum;
		/// <summary>
		///  会长名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 会长名字", " 会长名字")]
		public string leaderName;
		/// <summary>
		///  宣言
		/// </summary>
		[AdvancedInspector.Descriptor(" 宣言", " 宣言")]
		public string declaration;
		/// <summary>
		///  是否已经申请
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否已经申请", " 是否已经申请")]
		public byte isRequested;
		/// <summary>
		///  跨服领地
		/// </summary>
		[AdvancedInspector.Descriptor(" 跨服领地", " 跨服领地")]
		public byte occupyCrossTerrId;
		/// <summary>
		///  本服领地
		/// </summary>
		[AdvancedInspector.Descriptor(" 本服领地", " 本服领地")]
		public byte occupyTerrId;
		/// <summary>
		///  入会等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 入会等级", " 入会等级")]
		public UInt32 joinLevel;
		/// <summary>
		///  会长id
		/// </summary>
		[AdvancedInspector.Descriptor(" 会长id", " 会长id")]
		public UInt64 leaderId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, level);
			BaseDLL.encode_int8(buffer, ref pos_, memberNum);
			byte[] leaderNameBytes = StringHelper.StringToUTF8Bytes(leaderName);
			BaseDLL.encode_string(buffer, ref pos_, leaderNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] declarationBytes = StringHelper.StringToUTF8Bytes(declaration);
			BaseDLL.encode_string(buffer, ref pos_, declarationBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, isRequested);
			BaseDLL.encode_int8(buffer, ref pos_, occupyCrossTerrId);
			BaseDLL.encode_int8(buffer, ref pos_, occupyTerrId);
			BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
			BaseDLL.encode_uint64(buffer, ref pos_, leaderId);
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
			BaseDLL.decode_int8(buffer, ref pos_, ref level);
			BaseDLL.decode_int8(buffer, ref pos_, ref memberNum);
			UInt16 leaderNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref leaderNameLen);
			byte[] leaderNameBytes = new byte[leaderNameLen];
			for(int i = 0; i < leaderNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref leaderNameBytes[i]);
			}
			leaderName = StringHelper.BytesToString(leaderNameBytes);
			UInt16 declarationLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref declarationLen);
			byte[] declarationBytes = new byte[declarationLen];
			for(int i = 0; i < declarationLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref declarationBytes[i]);
			}
			declaration = StringHelper.BytesToString(declarationBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref isRequested);
			BaseDLL.decode_int8(buffer, ref pos_, ref occupyCrossTerrId);
			BaseDLL.decode_int8(buffer, ref pos_, ref occupyTerrId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
			BaseDLL.decode_uint64(buffer, ref pos_, ref leaderId);
		}


		#endregion

	}

}
