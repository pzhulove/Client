using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  赏金联赛淘汰赛
	/// </summary>
	[AdvancedInspector.Descriptor(" 赏金联赛淘汰赛", " 赏金联赛淘汰赛")]
	public class CLPremiumLeagueBattle : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  比赛ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 比赛ID", " 比赛ID")]
		public UInt64 raceId;
		/// <summary>
		///  比赛类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 比赛类型", " 比赛类型")]
		public byte type;
		/// <summary>
		///  成员1
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员1", " 成员1")]
		public PremiumLeagueBattleFighter fighter1 = null;
		/// <summary>
		///  成员2
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员2", " 成员2")]
		public PremiumLeagueBattleFighter fighter2 = null;
		/// <summary>
		///  是否已经结束了
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否已经结束了", " 是否已经结束了")]
		public byte isEnd;
		/// <summary>
		///  胜者ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜者ID", " 胜者ID")]
		public UInt64 winnerId;
		/// <summary>
		///  relay地址
		/// </summary>
		[AdvancedInspector.Descriptor(" relay地址", " relay地址")]
		public SockAddr relayAddr = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, raceId);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			fighter1.encode(buffer, ref pos_);
			fighter2.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, isEnd);
			BaseDLL.encode_uint64(buffer, ref pos_, winnerId);
			relayAddr.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			fighter1.decode(buffer, ref pos_);
			fighter2.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref isEnd);
			BaseDLL.decode_uint64(buffer, ref pos_, ref winnerId);
			relayAddr.decode(buffer, ref pos_);
		}


		#endregion

	}

}
