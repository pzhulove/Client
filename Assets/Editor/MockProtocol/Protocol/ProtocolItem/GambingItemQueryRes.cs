using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 夺宝商品数据查询返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 夺宝商品数据查询返回", "world->client 夺宝商品数据查询返回")]
	public class GambingItemQueryRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707904;
		public UInt32 Sequence;

		public GambingItemInfo[] gambingItems = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gambingItems.Length);
			for(int i = 0; i < gambingItems.Length; i++)
			{
				gambingItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 gambingItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gambingItemsCnt);
			gambingItems = new GambingItemInfo[gambingItemsCnt];
			for(int i = 0; i < gambingItems.Length; i++)
			{
				gambingItems[i] = new GambingItemInfo();
				gambingItems[i].decode(buffer, ref pos_);
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
