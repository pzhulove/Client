using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团队面板属性
	/// </summary>
	[AdvancedInspector.Descriptor(" 团队面板属性", " 团队面板属性")]
	public class TeamCopyTeamProperty : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt32 teamId;
		/// <summary>
		///  模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 模式", " 模式")]
		public UInt32 teamModel;
		/// <summary>
		///  佣金
		/// </summary>
		[AdvancedInspector.Descriptor(" 佣金", " 佣金")]
		public UInt32 commission;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string teamName;
		/// <summary>
		///  难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度", " 难度")]
		public UInt32 teamGrade;
		/// <summary>
		///  人数
		/// </summary>
		[AdvancedInspector.Descriptor(" 人数", " 人数")]
		public UInt32 memberNum;
		/// <summary>
		///  装备评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备评分", " 装备评分")]
		public UInt32 equipScore;
		/// <summary>
		///  状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态", " 状态")]
		public UInt32 status;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
			BaseDLL.encode_uint32(buffer, ref pos_, commission);
			byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
			BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
			BaseDLL.encode_uint32(buffer, ref pos_, memberNum);
			BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			BaseDLL.encode_uint32(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref commission);
			UInt16 teamNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
			byte[] teamNameBytes = new byte[teamNameLen];
			for(int i = 0; i < teamNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
			}
			teamName = StringHelper.BytesToString(teamNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
			BaseDLL.decode_uint32(buffer, ref pos_, ref memberNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref status);
		}


		#endregion

	}

}
