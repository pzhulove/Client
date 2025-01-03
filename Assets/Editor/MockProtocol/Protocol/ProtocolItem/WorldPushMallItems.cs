using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 推送商城商品
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 推送商城商品", " world->client 推送商城商品")]
	public class WorldPushMallItems : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602826;
		public UInt32 Sequence;
		/// <summary>
		///  商城道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 商城道具", " 商城道具")]
		public MallItemInfo[] mallItems = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mallItems.Length);
			for(int i = 0; i < mallItems.Length; i++)
			{
				mallItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 mallItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref mallItemsCnt);
			mallItems = new MallItemInfo[mallItemsCnt];
			for(int i = 0; i < mallItems.Length; i++)
			{
				mallItems[i] = new MallItemInfo();
				mallItems[i].decode(buffer, ref pos_);
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
