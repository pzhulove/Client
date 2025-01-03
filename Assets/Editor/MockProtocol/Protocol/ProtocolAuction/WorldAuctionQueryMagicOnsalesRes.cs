using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 
	/// </summary>
	[AdvancedInspector.Descriptor("world->client ", "world->client ")]
	public class WorldAuctionQueryMagicOnsalesRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603934;
		public UInt32 Sequence;

		public UInt32 code;

		public AuctionMagicOnsale[] magicOnsales = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)magicOnsales.Length);
			for(int i = 0; i < magicOnsales.Length; i++)
			{
				magicOnsales[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 magicOnsalesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref magicOnsalesCnt);
			magicOnsales = new AuctionMagicOnsale[magicOnsalesCnt];
			for(int i = 0; i < magicOnsales.Length; i++)
			{
				magicOnsales[i] = new AuctionMagicOnsale();
				magicOnsales[i].decode(buffer, ref pos_);
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
