using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server ", " client->server ")]
	public class CSOpenMagBoxReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500969;
		public UInt32 Sequence;

		public UInt64 itemUid;

		public byte isBatch;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			BaseDLL.encode_int8(buffer, ref pos_, isBatch);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			BaseDLL.decode_int8(buffer, ref pos_, ref isBatch);
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
