using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 查询道具价格情况(最低出售价格列表)
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 查询道具价格情况(最低出售价格列表)", "client->world 查询道具价格情况(最低出售价格列表)")]
	public class WorldAuctionQueryItemPriceListReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603931;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 拍卖行类型", " 拍卖行类型")]
		public byte type;
		/// <summary>
		///  物品出售状态 [1]:公示 [2]:上架
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品出售状态 [1]:公示 [2]:上架", " 物品出售状态 [1]:公示 [2]:上架")]
		public byte auctionState;
		/// <summary>
		///  物品类型id
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品类型id", " 物品类型id")]
		public UInt32 itemTypeId;
		/// <summary>
		///  物品强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品强化等级", " 物品强化等级")]
		public UInt32 strengthen;
		/// <summary>
		/// 红字装备增幅类型:无效0/力量1/智力2/体力3/精神4
		/// </summary>
		[AdvancedInspector.Descriptor("红字装备增幅类型:无效0/力量1/智力2/体力3/精神4", "红字装备增幅类型:无效0/力量1/智力2/体力3/精神4")]
		public byte enhanceType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, auctionState);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref auctionState);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
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
