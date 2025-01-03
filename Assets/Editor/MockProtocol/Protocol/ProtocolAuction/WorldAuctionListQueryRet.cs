using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldAuctionListQueryRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603902;
		public UInt32 Sequence;

		public byte type;

		public AuctionBaseInfo[] data = null;

		public UInt32 curPage;

		public UInt32 maxPage;
		/// <summary>
		///  商品状态(AuctionGoodState)
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品状态(AuctionGoodState)", " 商品状态(AuctionGoodState)")]
		public byte goodState;
		/// <summary>
		///  是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)", " 是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)")]
		public byte attent;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
			for(int i = 0; i < data.Length; i++)
			{
				data[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, curPage);
			BaseDLL.encode_uint32(buffer, ref pos_, maxPage);
			BaseDLL.encode_int8(buffer, ref pos_, goodState);
			BaseDLL.encode_int8(buffer, ref pos_, attent);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			UInt16 dataCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
			data = new AuctionBaseInfo[dataCnt];
			for(int i = 0; i < data.Length; i++)
			{
				data[i] = new AuctionBaseInfo();
				data[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref curPage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxPage);
			BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
			BaseDLL.decode_int8(buffer, ref pos_, ref attent);
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
