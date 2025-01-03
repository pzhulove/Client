using System;
using System.Text;

namespace Mock.Protocol
{

	public class AuctionQueryCondition : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  拍卖类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖类型", " 拍卖类型")]
		public byte type;
		/// <summary>
		///  商品状态(AuctionGoodState)
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品状态(AuctionGoodState)", " 商品状态(AuctionGoodState)")]
		public byte goodState;
		/// <summary>
		///  物品主类型(AuctionMainItemType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品主类型(AuctionMainItemType)", " 物品主类型(AuctionMainItemType)")]
		public byte itemMainType;
		/// <summary>
		///  物品子类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品子类型", " 物品子类型")]
		public UInt32[] itemSubTypes = new UInt32[0];
		/// <summary>
		///  排除物品子类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 排除物品子类型", " 排除物品子类型")]
		public UInt32[] excludeItemSubTypes = new UInt32[0];
		/// <summary>
		///  物品ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品ID", " 物品ID")]
		public UInt32 itemTypeID;
		/// <summary>
		///  品质
		/// </summary>
		[AdvancedInspector.Descriptor(" 品质", " 品质")]
		public byte quality;
		/// <summary>
		///  最低物品等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 最低物品等级", " 最低物品等级")]
		public byte minLevel;
		/// <summary>
		///  最高物品等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 最高物品等级", " 最高物品等级")]
		public byte maxLevel;
		/// <summary>
		///  最低强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 最低强化等级", " 最低强化等级")]
		public byte minStrength;
		/// <summary>
		///  最高强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 最高强化等级", " 最高强化等级")]
		public byte maxStrength;
		/// <summary>
		///  排序方式（对应枚举AuctionSortType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 排序方式（对应枚举AuctionSortType）", " 排序方式（对应枚举AuctionSortType）")]
		public byte sortType;
		/// <summary>
		///  页数
		/// </summary>
		[AdvancedInspector.Descriptor(" 页数", " 页数")]
		public UInt16 page;
		/// <summary>
		///  每页物品数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 每页物品数量", " 每页物品数量")]
		public byte itemNumPerPage;
		/// <summary>
		///  强化卷强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化卷强化等级", " 强化卷强化等级")]
		public UInt32 couponStrengthToLev;
		/// <summary>
		///  是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)", " 是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)")]
		public byte attent;
		/// <summary>
		///  职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业", " 职业")]
		public byte[] occus = new byte[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, goodState);
			BaseDLL.encode_int8(buffer, ref pos_, itemMainType);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemSubTypes.Length);
			for(int i = 0; i < itemSubTypes.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemSubTypes[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)excludeItemSubTypes.Length);
			for(int i = 0; i < excludeItemSubTypes.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, excludeItemSubTypes[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeID);
			BaseDLL.encode_int8(buffer, ref pos_, quality);
			BaseDLL.encode_int8(buffer, ref pos_, minLevel);
			BaseDLL.encode_int8(buffer, ref pos_, maxLevel);
			BaseDLL.encode_int8(buffer, ref pos_, minStrength);
			BaseDLL.encode_int8(buffer, ref pos_, maxStrength);
			BaseDLL.encode_int8(buffer, ref pos_, sortType);
			BaseDLL.encode_uint16(buffer, ref pos_, page);
			BaseDLL.encode_int8(buffer, ref pos_, itemNumPerPage);
			BaseDLL.encode_uint32(buffer, ref pos_, couponStrengthToLev);
			BaseDLL.encode_int8(buffer, ref pos_, attent);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occus.Length);
			for(int i = 0; i < occus.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occus[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
			BaseDLL.decode_int8(buffer, ref pos_, ref itemMainType);
			UInt16 itemSubTypesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemSubTypesCnt);
			itemSubTypes = new UInt32[itemSubTypesCnt];
			for(int i = 0; i < itemSubTypes.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemSubTypes[i]);
			}
			UInt16 excludeItemSubTypesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref excludeItemSubTypesCnt);
			excludeItemSubTypes = new UInt32[excludeItemSubTypesCnt];
			for(int i = 0; i < excludeItemSubTypes.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref excludeItemSubTypes[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeID);
			BaseDLL.decode_int8(buffer, ref pos_, ref quality);
			BaseDLL.decode_int8(buffer, ref pos_, ref minLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref maxLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref minStrength);
			BaseDLL.decode_int8(buffer, ref pos_, ref maxStrength);
			BaseDLL.decode_int8(buffer, ref pos_, ref sortType);
			BaseDLL.decode_uint16(buffer, ref pos_, ref page);
			BaseDLL.decode_int8(buffer, ref pos_, ref itemNumPerPage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref couponStrengthToLev);
			BaseDLL.decode_int8(buffer, ref pos_, ref attent);
			UInt16 occusCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref occusCnt);
			occus = new byte[occusCnt];
			for(int i = 0; i < occus.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occus[i]);
			}
		}


		#endregion

	}

}
