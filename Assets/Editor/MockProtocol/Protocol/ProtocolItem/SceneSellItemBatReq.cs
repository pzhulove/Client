using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 批量出售道具请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 批量出售道具请求", " client->server 批量出售道具请求")]
	public class SceneSellItemBatReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500973;
		public UInt32 Sequence;

		public UInt64[] itemUids = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
			for(int i = 0; i < itemUids.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 itemUidsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
			itemUids = new UInt64[itemUidsCnt];
			for(int i = 0; i < itemUids.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
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
