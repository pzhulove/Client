using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回赏金联赛奖金
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回赏金联赛奖金", " 返回赏金联赛奖金")]
	public class WorldPremiumLeagueRewardPoolRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607703;
		public UInt32 Sequence;
		/// <summary>
		///  报名人数
		/// </summary>
		[AdvancedInspector.Descriptor(" 报名人数", " 报名人数")]
		public UInt32 enrollPlayerNum;
		/// <summary>
		///  奖金数
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖金数", " 奖金数")]
		public UInt32 money;
		/// <summary>
		///  各排名奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 各排名奖励", " 各排名奖励")]
		public UInt32[] rewards = new UInt32[5];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, enrollPlayerNum);
			BaseDLL.encode_uint32(buffer, ref pos_, money);
			for(int i = 0; i < rewards.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, rewards[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref enrollPlayerNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref money);
			for(int i = 0; i < rewards.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewards[i]);
			}
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
