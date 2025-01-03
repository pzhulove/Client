using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 道具价格情况返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 道具价格情况返回", "world->client 道具价格情况返回")]
	public class WorldAuctionQueryItemPriceListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603932;
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
		///  目前在售的价格最低的同样道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 目前在售的价格最低的同样道具", " 目前在售的价格最低的同样道具")]
		public AuctionBaseInfo[] actionItems = null;

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
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref auctionState);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			UInt16 actionItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
			actionItems = new AuctionBaseInfo[actionItemsCnt];
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i] = new AuctionBaseInfo();
				actionItems[i].decode(buffer, ref pos_);
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
