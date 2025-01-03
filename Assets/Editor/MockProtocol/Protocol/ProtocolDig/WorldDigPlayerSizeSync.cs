using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldDigPlayerSizeSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608203;
		public UInt32 Sequence;

		public UInt32 mapId;

		public UInt32 playerSize;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, mapId);
			BaseDLL.encode_uint32(buffer, ref pos_, playerSize);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref mapId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerSize);
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
