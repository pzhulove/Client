using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  夺宝商品信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 夺宝商品信息", " 夺宝商品信息")]
	public class GambingItemInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  商品id
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品id", " 商品id")]
		public UInt32 gambingItemId;
		/// <summary>
		///  商品数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品数量", " 商品数量")]
		public UInt32 gambingItemNum;
		/// <summary>
		///  排序
		/// </summary>
		[AdvancedInspector.Descriptor(" 排序", " 排序")]
		public UInt16 sortId;
		/// <summary>
		///  道具表id
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具表id", " 道具表id")]
		public UInt32 itemDataId;
		/// <summary>
		///  花费货币id
		/// </summary>
		[AdvancedInspector.Descriptor(" 花费货币id", " 花费货币id")]
		public UInt32 costMoneyId;
		/// <summary>
		///  单价(一份)
		/// </summary>
		[AdvancedInspector.Descriptor(" 单价(一份)", " 单价(一份)")]
		public UInt32 unitPrice;
		/// <summary>
		///  剩余组数
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余组数", " 剩余组数")]
		public UInt16 restGroups;
		/// <summary>
		///  总组数
		/// </summary>
		[AdvancedInspector.Descriptor(" 总组数", " 总组数")]
		public UInt16 totalGroups;
		/// <summary>
		///  每份奖励
		/// </summary>
		[AdvancedInspector.Descriptor(" 每份奖励", " 每份奖励")]
		public ItemReward[] rewardsPerCopy = null;
		/// <summary>
		///  当前组id
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前组id", " 当前组id")]
		public UInt16 curGroupId;
		/// <summary>
		///  当前组状态(对应枚举 GambingItemStatus)
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前组状态(对应枚举 GambingItemStatus)", " 当前组状态(对应枚举 GambingItemStatus)")]
		public byte statusOfCurGroup;
		/// <summary>
		///  当前组已售份数
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前组已售份数", " 当前组已售份数")]
		public UInt32 soldCopiesInCurGroup;
		/// <summary>
		///  当前组总份数
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前组总份数", " 当前组总份数")]
		public UInt32 totalCopiesOfCurGroup;
		/// <summary>
		///  当前组开售时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前组开售时间", " 当前组开售时间")]
		public UInt32 sellBeginTime;
		/// <summary>
		///  我的夺宝数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 我的夺宝数据", " 我的夺宝数据")]
		public GambingParticipantInfo mineGambingInfo = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
			BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
			BaseDLL.encode_uint16(buffer, ref pos_, sortId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
			BaseDLL.encode_uint32(buffer, ref pos_, costMoneyId);
			BaseDLL.encode_uint32(buffer, ref pos_, unitPrice);
			BaseDLL.encode_uint16(buffer, ref pos_, restGroups);
			BaseDLL.encode_uint16(buffer, ref pos_, totalGroups);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewardsPerCopy.Length);
			for(int i = 0; i < rewardsPerCopy.Length; i++)
			{
				rewardsPerCopy[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, curGroupId);
			BaseDLL.encode_int8(buffer, ref pos_, statusOfCurGroup);
			BaseDLL.encode_uint32(buffer, ref pos_, soldCopiesInCurGroup);
			BaseDLL.encode_uint32(buffer, ref pos_, totalCopiesOfCurGroup);
			BaseDLL.encode_uint32(buffer, ref pos_, sellBeginTime);
			mineGambingInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
			BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref costMoneyId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref unitPrice);
			BaseDLL.decode_uint16(buffer, ref pos_, ref restGroups);
			BaseDLL.decode_uint16(buffer, ref pos_, ref totalGroups);
			UInt16 rewardsPerCopyCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsPerCopyCnt);
			rewardsPerCopy = new ItemReward[rewardsPerCopyCnt];
			for(int i = 0; i < rewardsPerCopy.Length; i++)
			{
				rewardsPerCopy[i] = new ItemReward();
				rewardsPerCopy[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint16(buffer, ref pos_, ref curGroupId);
			BaseDLL.decode_int8(buffer, ref pos_, ref statusOfCurGroup);
			BaseDLL.decode_uint32(buffer, ref pos_, ref soldCopiesInCurGroup);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalCopiesOfCurGroup);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sellBeginTime);
			mineGambingInfo.decode(buffer, ref pos_);
		}


		#endregion

	}

}
