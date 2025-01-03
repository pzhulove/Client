using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene -> client 礼包信息返回
	/// </summary>
	[AdvancedInspector.Descriptor(" scene -> client 礼包信息返回", " scene -> client 礼包信息返回")]
	public class SceneGiftPackInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503212;
		public UInt32 Sequence;

		public GiftPackSyncInfo[] giftPacks = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftPacks.Length);
			for(int i = 0; i < giftPacks.Length; i++)
			{
				giftPacks[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 giftPacksCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref giftPacksCnt);
			giftPacks = new GiftPackSyncInfo[giftPacksCnt];
			for(int i = 0; i < giftPacks.Length; i++)
			{
				giftPacks[i] = new GiftPackSyncInfo();
				giftPacks[i].decode(buffer, ref pos_);
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
