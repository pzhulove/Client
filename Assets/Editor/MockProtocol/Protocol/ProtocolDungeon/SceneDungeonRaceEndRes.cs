using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonRaceEndRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506812;
		public UInt32 Sequence;

		public UInt32 result;

		public byte score;

		public UInt32 usedTime;

		public UInt32 killMonsterTotalExp;

		public UInt32 raceEndExp;

		public byte hasRaceEndDrop;

		public byte raceEndDropBaseMulti;

		public DungeonAdditionInfo addition = null;

		public ItemReward teamReward = null;
		/// <summary>
		///  有没有结算翻牌
		/// </summary>
		[AdvancedInspector.Descriptor(" 有没有结算翻牌", " 有没有结算翻牌")]
		public byte hasRaceEndChest;
		/// <summary>
		///  月卡结算金币奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 月卡结算金币奖励", " 月卡结算金币奖励")]
		public UInt32 monthcartGoldReward;
		/// <summary>
		///  金币称号金币奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 金币称号金币奖励", " 金币称号金币奖励")]
		public UInt32 goldTitleGoldReward;
		/// <summary>
		///  疲劳燃烧类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 疲劳燃烧类型", " 疲劳燃烧类型")]
		public byte fatigueBurnType;
		/// <summary>
		///  翻牌翻倍标志
		/// </summary>
		[AdvancedInspector.Descriptor(" 翻牌翻倍标志", " 翻牌翻倍标志")]
		public byte chestDoubleFlag;
		/// <summary>
		///  回归掉落奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 回归掉落奖励", " 回归掉落奖励")]
		public ItemReward veteranReturnReward = null;
		/// <summary>
		///  roLl掉落
		/// </summary>
		[AdvancedInspector.Descriptor(" roLl掉落", " roLl掉落")]
		public RollItemInfo[] rollReward = null;
		/// <summary>
		///  roll单人获取掉落情况
		/// </summary>
		[AdvancedInspector.Descriptor(" roll单人获取掉落情况", " roll单人获取掉落情况")]
		public ItemReward[] rollSingleReward = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, score);
			BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
			BaseDLL.encode_uint32(buffer, ref pos_, killMonsterTotalExp);
			BaseDLL.encode_uint32(buffer, ref pos_, raceEndExp);
			BaseDLL.encode_int8(buffer, ref pos_, hasRaceEndDrop);
			BaseDLL.encode_int8(buffer, ref pos_, raceEndDropBaseMulti);
			addition.encode(buffer, ref pos_);
			teamReward.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, hasRaceEndChest);
			BaseDLL.encode_uint32(buffer, ref pos_, monthcartGoldReward);
			BaseDLL.encode_uint32(buffer, ref pos_, goldTitleGoldReward);
			BaseDLL.encode_int8(buffer, ref pos_, fatigueBurnType);
			BaseDLL.encode_int8(buffer, ref pos_, chestDoubleFlag);
			veteranReturnReward.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rollReward.Length);
			for(int i = 0; i < rollReward.Length; i++)
			{
				rollReward[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rollSingleReward.Length);
			for(int i = 0; i < rollSingleReward.Length; i++)
			{
				rollSingleReward[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref score);
			BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref killMonsterTotalExp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref raceEndExp);
			BaseDLL.decode_int8(buffer, ref pos_, ref hasRaceEndDrop);
			BaseDLL.decode_int8(buffer, ref pos_, ref raceEndDropBaseMulti);
			addition.decode(buffer, ref pos_);
			teamReward.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref hasRaceEndChest);
			BaseDLL.decode_uint32(buffer, ref pos_, ref monthcartGoldReward);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goldTitleGoldReward);
			BaseDLL.decode_int8(buffer, ref pos_, ref fatigueBurnType);
			BaseDLL.decode_int8(buffer, ref pos_, ref chestDoubleFlag);
			veteranReturnReward.decode(buffer, ref pos_);
			UInt16 rollRewardCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rollRewardCnt);
			rollReward = new RollItemInfo[rollRewardCnt];
			for(int i = 0; i < rollReward.Length; i++)
			{
				rollReward[i] = new RollItemInfo();
				rollReward[i].decode(buffer, ref pos_);
			}
			UInt16 rollSingleRewardCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rollSingleRewardCnt);
			rollSingleReward = new ItemReward[rollSingleRewardCnt];
			for(int i = 0; i < rollSingleReward.Length; i++)
			{
				rollSingleReward[i] = new ItemReward();
				rollSingleReward[i].decode(buffer, ref pos_);
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
