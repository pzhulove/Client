using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  冒险队(佣兵团)信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 冒险队(佣兵团)信息", " 冒险队(佣兵团)信息")]
	public class AdventureTeamInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  冒险队队名
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队队名", " 冒险队队名")]
		public string adventureTeamName;
		/// <summary>
		///  冒险队等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队等级", " 冒险队等级")]
		public UInt16 adventureTeamLevel;
		/// <summary>
		///  冒险队经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队经验", " 冒险队经验")]
		public UInt64 adventureTeamExp;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
			BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, adventureTeamLevel);
			BaseDLL.encode_uint64(buffer, ref pos_, adventureTeamExp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 adventureTeamNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamNameLen);
			byte[] adventureTeamNameBytes = new byte[adventureTeamNameLen];
			for(int i = 0; i < adventureTeamNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamNameBytes[i]);
			}
			adventureTeamName = StringHelper.BytesToString(adventureTeamNameBytes);
			BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamLevel);
			BaseDLL.decode_uint64(buffer, ref pos_, ref adventureTeamExp);
		}


		#endregion

	}

}
