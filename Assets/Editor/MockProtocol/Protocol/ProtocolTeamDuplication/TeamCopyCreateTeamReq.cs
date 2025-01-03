using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  创建团队请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 创建团队请求", " 创建团队请求")]
	public class TeamCopyCreateTeamReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100003;
		public UInt32 Sequence;
		/// <summary>
		///  难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度", " 难度")]
		public UInt32 teamGrade;
		/// <summary>
		///  模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 模式", " 模式")]
		public UInt32 teamModel;
		/// <summary>
		///  装备评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备评分", " 装备评分")]
		public UInt32 equipScore;
		/// <summary>
		///  param(佣金)
		/// </summary>
		[AdvancedInspector.Descriptor(" param(佣金)", " param(佣金)")]
		public UInt32 param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
			BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
			BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			BaseDLL.encode_uint32(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
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
