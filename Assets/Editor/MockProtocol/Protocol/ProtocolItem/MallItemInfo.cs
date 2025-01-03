using System;
using System.Text;

namespace Mock.Protocol
{

	public class MallItemInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public byte type;

		public byte subtype;

		public byte jobtype;

		public UInt32 itemid;

		public UInt32 itemnum;

		public UInt32 price;

		public UInt32 discountprice;

		public byte moneytype;

		public byte limit;

		public UInt16 limitnum;

		public byte gift;

		public UInt16 vipscore;

		public string icon;

		public UInt32 starttime;

		public UInt32 endtime;

		public UInt16 limittotalnum;

		public ItemReward[] giftItems = null;

		public string giftName;

		public byte tagType;

		public UInt32 sortIdx;

		public UInt32 hotSortIdx;

		public UInt16 goodsSubType;

		public byte isRecommend;

		public byte isPersonalTailor;

		public UInt32 discountRate;

		public byte loginPushId;

		public string fashionImagePath;

		public string giftDesc;
		/// <summary>
		///  购买获得物信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买获得物信息", " 购买获得物信息")]
		public MallBuyGotInfo[] buyGotInfos = null;
		/// <summary>
		///  扩展参数
		/// </summary>
		[AdvancedInspector.Descriptor(" 扩展参数", " 扩展参数")]
		public UInt32[] extParams = new UInt32[0];
		/// <summary>
		///  账号刷新类型 RefreshType
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号刷新类型 RefreshType", " 账号刷新类型 RefreshType")]
		public byte accountRefreshType;
		/// <summary>
		///  账号限购次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号限购次数", " 账号限购次数")]
		public UInt32 accountLimitBuyNum;
		/// <summary>
		///  账号剩余购买次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号剩余购买次数", " 账号剩余购买次数")]
		public UInt32 accountRestBuyNum;
		/// <summary>
		///  折扣券id
		/// </summary>
		[AdvancedInspector.Descriptor(" 折扣券id", " 折扣券id")]
		public UInt32 discountCouponId;
		/// <summary>
		///  多倍积分倍率
		/// </summary>
		[AdvancedInspector.Descriptor(" 多倍积分倍率", " 多倍积分倍率")]
		public byte multiple;
		/// <summary>
		///  多倍结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 多倍结束时间", " 多倍结束时间")]
		public UInt32 multipleEndTime;
		/// <summary>
		///  抵扣券
		/// </summary>
		[AdvancedInspector.Descriptor(" 抵扣券", " 抵扣券")]
		public UInt32 deductionCouponId;
		/// <summary>
		///  是否使用信用点券
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否使用信用点券", " 是否使用信用点券")]
		public UInt32 creditPointDeduction;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, subtype);
			BaseDLL.encode_int8(buffer, ref pos_, jobtype);
			BaseDLL.encode_uint32(buffer, ref pos_, itemid);
			BaseDLL.encode_uint32(buffer, ref pos_, itemnum);
			BaseDLL.encode_uint32(buffer, ref pos_, price);
			BaseDLL.encode_uint32(buffer, ref pos_, discountprice);
			BaseDLL.encode_int8(buffer, ref pos_, moneytype);
			BaseDLL.encode_int8(buffer, ref pos_, limit);
			BaseDLL.encode_uint16(buffer, ref pos_, limitnum);
			BaseDLL.encode_int8(buffer, ref pos_, gift);
			BaseDLL.encode_uint16(buffer, ref pos_, vipscore);
			byte[] iconBytes = StringHelper.StringToUTF8Bytes(icon);
			BaseDLL.encode_string(buffer, ref pos_, iconBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, starttime);
			BaseDLL.encode_uint32(buffer, ref pos_, endtime);
			BaseDLL.encode_uint16(buffer, ref pos_, limittotalnum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftItems.Length);
			for(int i = 0; i < giftItems.Length; i++)
			{
				giftItems[i].encode(buffer, ref pos_);
			}
			byte[] giftNameBytes = StringHelper.StringToUTF8Bytes(giftName);
			BaseDLL.encode_string(buffer, ref pos_, giftNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, tagType);
			BaseDLL.encode_uint32(buffer, ref pos_, sortIdx);
			BaseDLL.encode_uint32(buffer, ref pos_, hotSortIdx);
			BaseDLL.encode_uint16(buffer, ref pos_, goodsSubType);
			BaseDLL.encode_int8(buffer, ref pos_, isRecommend);
			BaseDLL.encode_int8(buffer, ref pos_, isPersonalTailor);
			BaseDLL.encode_uint32(buffer, ref pos_, discountRate);
			BaseDLL.encode_int8(buffer, ref pos_, loginPushId);
			byte[] fashionImagePathBytes = StringHelper.StringToUTF8Bytes(fashionImagePath);
			BaseDLL.encode_string(buffer, ref pos_, fashionImagePathBytes, (UInt16)(buffer.Length - pos_));
			byte[] giftDescBytes = StringHelper.StringToUTF8Bytes(giftDesc);
			BaseDLL.encode_string(buffer, ref pos_, giftDescBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buyGotInfos.Length);
			for(int i = 0; i < buyGotInfos.Length; i++)
			{
				buyGotInfos[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)extParams.Length);
			for(int i = 0; i < extParams.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, extParams[i]);
			}
			BaseDLL.encode_int8(buffer, ref pos_, accountRefreshType);
			BaseDLL.encode_uint32(buffer, ref pos_, accountLimitBuyNum);
			BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
			BaseDLL.encode_uint32(buffer, ref pos_, discountCouponId);
			BaseDLL.encode_int8(buffer, ref pos_, multiple);
			BaseDLL.encode_uint32(buffer, ref pos_, multipleEndTime);
			BaseDLL.encode_uint32(buffer, ref pos_, deductionCouponId);
			BaseDLL.encode_uint32(buffer, ref pos_, creditPointDeduction);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref subtype);
			BaseDLL.decode_int8(buffer, ref pos_, ref jobtype);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemnum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discountprice);
			BaseDLL.decode_int8(buffer, ref pos_, ref moneytype);
			BaseDLL.decode_int8(buffer, ref pos_, ref limit);
			BaseDLL.decode_uint16(buffer, ref pos_, ref limitnum);
			BaseDLL.decode_int8(buffer, ref pos_, ref gift);
			BaseDLL.decode_uint16(buffer, ref pos_, ref vipscore);
			UInt16 iconLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref iconLen);
			byte[] iconBytes = new byte[iconLen];
			for(int i = 0; i < iconLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref iconBytes[i]);
			}
			icon = StringHelper.BytesToString(iconBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref starttime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endtime);
			BaseDLL.decode_uint16(buffer, ref pos_, ref limittotalnum);
			UInt16 giftItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref giftItemsCnt);
			giftItems = new ItemReward[giftItemsCnt];
			for(int i = 0; i < giftItems.Length; i++)
			{
				giftItems[i] = new ItemReward();
				giftItems[i].decode(buffer, ref pos_);
			}
			UInt16 giftNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref giftNameLen);
			byte[] giftNameBytes = new byte[giftNameLen];
			for(int i = 0; i < giftNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref giftNameBytes[i]);
			}
			giftName = StringHelper.BytesToString(giftNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref tagType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sortIdx);
			BaseDLL.decode_uint32(buffer, ref pos_, ref hotSortIdx);
			BaseDLL.decode_uint16(buffer, ref pos_, ref goodsSubType);
			BaseDLL.decode_int8(buffer, ref pos_, ref isRecommend);
			BaseDLL.decode_int8(buffer, ref pos_, ref isPersonalTailor);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discountRate);
			BaseDLL.decode_int8(buffer, ref pos_, ref loginPushId);
			UInt16 fashionImagePathLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref fashionImagePathLen);
			byte[] fashionImagePathBytes = new byte[fashionImagePathLen];
			for(int i = 0; i < fashionImagePathLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref fashionImagePathBytes[i]);
			}
			fashionImagePath = StringHelper.BytesToString(fashionImagePathBytes);
			UInt16 giftDescLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref giftDescLen);
			byte[] giftDescBytes = new byte[giftDescLen];
			for(int i = 0; i < giftDescLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref giftDescBytes[i]);
			}
			giftDesc = StringHelper.BytesToString(giftDescBytes);
			UInt16 buyGotInfosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buyGotInfosCnt);
			buyGotInfos = new MallBuyGotInfo[buyGotInfosCnt];
			for(int i = 0; i < buyGotInfos.Length; i++)
			{
				buyGotInfos[i] = new MallBuyGotInfo();
				buyGotInfos[i].decode(buffer, ref pos_);
			}
			UInt16 extParamsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref extParamsCnt);
			extParams = new UInt32[extParamsCnt];
			for(int i = 0; i < extParams.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref extParams[i]);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountLimitBuyNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discountCouponId);
			BaseDLL.decode_int8(buffer, ref pos_, ref multiple);
			BaseDLL.decode_uint32(buffer, ref pos_, ref multipleEndTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref deductionCouponId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref creditPointDeduction);
		}


		#endregion

	}

}
