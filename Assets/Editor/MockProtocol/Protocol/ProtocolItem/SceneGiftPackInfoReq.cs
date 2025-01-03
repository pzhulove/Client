using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client -> scene 礼包信息请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client -> scene 礼包信息请求", " client -> scene 礼包信息请求")]
	public class SceneGiftPackInfoReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503211;
		public UInt32 Sequence;

		public UInt32[] giftPackIds = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftPackIds.Length);
			for(int i = 0; i < giftPackIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, giftPackIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 giftPackIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref giftPackIdsCnt);
			giftPackIds = new UInt32[giftPackIdsCnt];
			for(int i = 0; i < giftPackIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref giftPackIds[i]);
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
