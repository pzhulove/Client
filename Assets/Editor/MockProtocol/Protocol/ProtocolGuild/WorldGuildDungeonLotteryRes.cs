using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城抽奖返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城抽奖返回", " 公会地下城抽奖返回")]
	public class WorldGuildDungeonLotteryRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608508;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		[AdvancedInspector.Descriptor(" 结果", " 结果")]
		public UInt32 result;
		/// <summary>
		///  奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励", " 奖励")]
		public GuildDungeonLotteryItem[] lotteryItemVec = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)lotteryItemVec.Length);
			for(int i = 0; i < lotteryItemVec.Length; i++)
			{
				lotteryItemVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 lotteryItemVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref lotteryItemVecCnt);
			lotteryItemVec = new GuildDungeonLotteryItem[lotteryItemVecCnt];
			for(int i = 0; i < lotteryItemVec.Length; i++)
			{
				lotteryItemVec[i] = new GuildDungeonLotteryItem();
				lotteryItemVec[i].decode(buffer, ref pos_);
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
