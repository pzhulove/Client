using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  冒险队(佣兵团)扩展信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 冒险队(佣兵团)扩展信息", " 冒险队(佣兵团)扩展信息")]
	public class AdventureTeamExtraInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
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
		/// <summary>
		///  冒险队评级
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队评级", " 冒险队评级")]
		public string adventureTeamGrade;
		/// <summary>
		///  冒险队排名
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队排名", " 冒险队排名")]
		public UInt32 adventureTeamRanking;
		/// <summary>
		///  拥有角色栏位数
		/// </summary>
		[AdvancedInspector.Descriptor(" 拥有角色栏位数", " 拥有角色栏位数")]
		public UInt32 adventureTeamOwnRoleFileds;
		/// <summary>
		///  冒险团角色总评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险团角色总评分", " 冒险团角色总评分")]
		public UInt32 adventureTeamRoleTotalScore;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] adventureTeamNameBytes = StringHelper.StringToUTF8Bytes(adventureTeamName);
			BaseDLL.encode_string(buffer, ref pos_, adventureTeamNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, adventureTeamLevel);
			BaseDLL.encode_uint64(buffer, ref pos_, adventureTeamExp);
			byte[] adventureTeamGradeBytes = StringHelper.StringToUTF8Bytes(adventureTeamGrade);
			BaseDLL.encode_string(buffer, ref pos_, adventureTeamGradeBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRanking);
			BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamOwnRoleFileds);
			BaseDLL.encode_uint32(buffer, ref pos_, adventureTeamRoleTotalScore);
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
			UInt16 adventureTeamGradeLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref adventureTeamGradeLen);
			byte[] adventureTeamGradeBytes = new byte[adventureTeamGradeLen];
			for(int i = 0; i < adventureTeamGradeLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref adventureTeamGradeBytes[i]);
			}
			adventureTeamGrade = StringHelper.BytesToString(adventureTeamGradeBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRanking);
			BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamOwnRoleFileds);
			BaseDLL.decode_uint32(buffer, ref pos_, ref adventureTeamRoleTotalScore);
		}


		#endregion

	}

}
