using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  赏金联赛结算
	/// </summary>
	[AdvancedInspector.Descriptor(" 赏金联赛结算", " 赏金联赛结算")]
	public class WorldPremiumLeagueRaceEnd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607712;
		public UInt32 Sequence;
		/// <summary>
		///  是不是预选赛
		/// </summary>
		[AdvancedInspector.Descriptor(" 是不是预选赛", " 是不是预选赛")]
		public byte isPreliminay;
		/// <summary>
		///  战斗结果
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗结果", " 战斗结果")]
		public byte result;
		/// <summary>
		///  原有积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 原有积分", " 原有积分")]
		public UInt32 oldScore;
		/// <summary>
		///  新积分
		/// </summary>
		[AdvancedInspector.Descriptor(" 新积分", " 新积分")]
		public UInt32 newScore;
		/// <summary>
		///  奖励数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励数量", " 奖励数量")]
		public UInt32 preliminayRewardNum;
		/// <summary>
		///  获得荣誉
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得荣誉", " 获得荣誉")]
		public UInt32 getHonor;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isPreliminay);
			BaseDLL.encode_int8(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, oldScore);
			BaseDLL.encode_uint32(buffer, ref pos_, newScore);
			BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
			BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isPreliminay);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
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
