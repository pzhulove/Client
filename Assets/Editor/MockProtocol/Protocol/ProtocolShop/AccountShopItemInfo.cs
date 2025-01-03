using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  账号商店商品信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 账号商店商品信息", " 账号商店商品信息")]
	public class AccountShopItemInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  商品id
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品id", " 商品id")]
		public UInt32 shopItemId;
		/// <summary>
		///  商品名称
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品名称", " 商品名称")]
		public string shopItemName;
		/// <summary>
		///  页签类别
		/// </summary>
		[AdvancedInspector.Descriptor(" 页签类别", " 页签类别")]
		public byte tabType;
		/// <summary>
		///  职业类别
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业类别", " 职业类别")]
		public byte jobType;
		/// <summary>
		///  上架道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 上架道具", " 上架道具")]
		public UInt32 itemDataId;
		/// <summary>
		///  上架数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 上架数量", " 上架数量")]
		public UInt32 itemNum;
		/// <summary>
		///  购买消耗道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买消耗道具", " 购买消耗道具")]
		public ItemReward[] costItems = null;
		/// <summary>
		///  账号刷新类型 RefreshType
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号刷新类型 RefreshType", " 账号刷新类型 RefreshType")]
		public byte accountRefreshType;
		/// <summary>
		///  账号刷新时间点
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号刷新时间点", " 账号刷新时间点")]
		public string accountRefreshTimePoint;
		/// <summary>
		///  账号限购数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号限购数量", " 账号限购数量")]
		public UInt32 accountLimitBuyNum;
		/// <summary>
		///  账号剩余购买数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号剩余购买数量", " 账号剩余购买数量")]
		public UInt32 accountRestBuyNum;
		/// <summary>
		///  账号购买记录下次刷新时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号购买记录下次刷新时间", " 账号购买记录下次刷新时间")]
		public UInt32 accountBuyRecordNextRefreshTimestamp;
		/// <summary>
		///  角色刷新类型 RefreshType
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色刷新类型 RefreshType", " 角色刷新类型 RefreshType")]
		public byte roleRefreshType;
		/// <summary>
		///  角色刷新时间点
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色刷新时间点", " 角色刷新时间点")]
		public string roleRefreshTimePoint;
		/// <summary>
		///  角色购买记录下次刷新时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色购买记录下次刷新时间", " 角色购买记录下次刷新时间")]
		public UInt32 roleBuyRecordNextRefreshTimestamp;
		/// <summary>
		///  角色限购数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色限购数量", " 角色限购数量")]
		public UInt32 roleLimitBuyNum;
		/// <summary>
		///  角色剩余购买数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色剩余购买数量", " 角色剩余购买数量")]
		public UInt32 roleRestBuyNum;
		/// <summary>
		///  扩展条件
		/// </summary>
		[AdvancedInspector.Descriptor(" 扩展条件", " 扩展条件")]
		public UInt32 extensibleCondition;
		/// <summary>
		///  是否需要预览按钮
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否需要预览按钮", " 是否需要预览按钮")]
		public byte needPreviewFunc;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
			byte[] shopItemNameBytes = StringHelper.StringToUTF8Bytes(shopItemName);
			BaseDLL.encode_string(buffer, ref pos_, shopItemNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, tabType);
			BaseDLL.encode_int8(buffer, ref pos_, jobType);
			BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)costItems.Length);
			for(int i = 0; i < costItems.Length; i++)
			{
				costItems[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_int8(buffer, ref pos_, accountRefreshType);
			byte[] accountRefreshTimePointBytes = StringHelper.StringToUTF8Bytes(accountRefreshTimePoint);
			BaseDLL.encode_string(buffer, ref pos_, accountRefreshTimePointBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, accountLimitBuyNum);
			BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
			BaseDLL.encode_uint32(buffer, ref pos_, accountBuyRecordNextRefreshTimestamp);
			BaseDLL.encode_int8(buffer, ref pos_, roleRefreshType);
			byte[] roleRefreshTimePointBytes = StringHelper.StringToUTF8Bytes(roleRefreshTimePoint);
			BaseDLL.encode_string(buffer, ref pos_, roleRefreshTimePointBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, roleBuyRecordNextRefreshTimestamp);
			BaseDLL.encode_uint32(buffer, ref pos_, roleLimitBuyNum);
			BaseDLL.encode_uint32(buffer, ref pos_, roleRestBuyNum);
			BaseDLL.encode_uint32(buffer, ref pos_, extensibleCondition);
			BaseDLL.encode_int8(buffer, ref pos_, needPreviewFunc);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
			UInt16 shopItemNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemNameLen);
			byte[] shopItemNameBytes = new byte[shopItemNameLen];
			for(int i = 0; i < shopItemNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopItemNameBytes[i]);
			}
			shopItemName = StringHelper.BytesToString(shopItemNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref tabType);
			BaseDLL.decode_int8(buffer, ref pos_, ref jobType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			UInt16 costItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref costItemsCnt);
			costItems = new ItemReward[costItemsCnt];
			for(int i = 0; i < costItems.Length; i++)
			{
				costItems[i] = new ItemReward();
				costItems[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshType);
			UInt16 accountRefreshTimePointLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref accountRefreshTimePointLen);
			byte[] accountRefreshTimePointBytes = new byte[accountRefreshTimePointLen];
			for(int i = 0; i < accountRefreshTimePointLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshTimePointBytes[i]);
			}
			accountRefreshTimePoint = StringHelper.BytesToString(accountRefreshTimePointBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountLimitBuyNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountBuyRecordNextRefreshTimestamp);
			BaseDLL.decode_int8(buffer, ref pos_, ref roleRefreshType);
			UInt16 roleRefreshTimePointLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref roleRefreshTimePointLen);
			byte[] roleRefreshTimePointBytes = new byte[roleRefreshTimePointLen];
			for(int i = 0; i < roleRefreshTimePointLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref roleRefreshTimePointBytes[i]);
			}
			roleRefreshTimePoint = StringHelper.BytesToString(roleRefreshTimePointBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref roleBuyRecordNextRefreshTimestamp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref roleLimitBuyNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref roleRestBuyNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref extensibleCondition);
			BaseDLL.decode_int8(buffer, ref pos_, ref needPreviewFunc);
		}


		#endregion

	}

}
