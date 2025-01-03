using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  查询玩家详细信息（可根据角色ID和名字查询，优先使用角色ID）
	/// </summary>
	[AdvancedInspector.Descriptor(" 查询玩家详细信息（可根据角色ID和名字查询，优先使用角色ID）", " 查询玩家详细信息（可根据角色ID和名字查询，优先使用角色ID）")]
	public class WorldQueryPlayerDetailsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601726;
		public UInt32 Sequence;
		/// <summary>
		///  角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色ID", " 角色ID")]
		public UInt64 roleId;
		/// <summary>
		///  查询类型(QueryPlayerType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询类型(QueryPlayerType)", " 查询类型(QueryPlayerType)")]
		public UInt32 queryType;
		/// <summary>
		/// zone(跨服查询需填)
		/// </summary>
		[AdvancedInspector.Descriptor("zone(跨服查询需填)", "zone(跨服查询需填)")]
		public UInt32 zoneId;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, queryType);
			BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
