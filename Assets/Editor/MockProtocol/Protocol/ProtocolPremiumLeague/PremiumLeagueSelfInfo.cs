using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  自己的赏金联赛信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 自己的赏金联赛信息", " 自己的赏金联赛信息")]
	public class PremiumLeagueSelfInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  胜场数
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜场数", " 胜场数")]
		public UInt32 winNum;
		/// <summary>
		///  积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 积分", " 积分")]
		public UInt32 score;
		/// <summary>
		///  排名
		/// </summary>
		[AdvancedInspector.Descriptor(" 排名", " 排名")]
		public UInt32 ranking;
		/// <summary>
		///  报名次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 报名次数", " 报名次数")]
		public UInt32 enrollCount;
		/// <summary>
		///  负场数
		/// </summary>
		[AdvancedInspector.Descriptor(" 负场数", " 负场数")]
		public UInt32 loseNum;
		/// <summary>
		///  预选赛获得的奖金
		/// </summary>
		[AdvancedInspector.Descriptor(" 预选赛获得的奖金", " 预选赛获得的奖金")]
		public UInt32 preliminayRewardNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, winNum);
			BaseDLL.encode_uint32(buffer, ref pos_, score);
			BaseDLL.encode_uint32(buffer, ref pos_, ranking);
			BaseDLL.encode_uint32(buffer, ref pos_, enrollCount);
			BaseDLL.encode_uint32(buffer, ref pos_, loseNum);
			BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
			BaseDLL.decode_uint32(buffer, ref pos_, ref enrollCount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref loseNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
		}


		#endregion

	}

}
