using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  赏金联赛淘汰赛成员信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 赏金联赛淘汰赛成员信息", " 赏金联赛淘汰赛成员信息")]
	public class PremiumLeagueBattleFighter : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色ID", " 角色ID")]
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

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
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
		}


		#endregion

	}

}
