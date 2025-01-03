using System;
using System.Text;

namespace Mock.Protocol
{

	public class GateClientLoginReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300203;
		public UInt32 Sequence;

		public UInt32 accid;

		public byte[] hashValue = new byte[20];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			for(int i = 0; i < hashValue.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, hashValue[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			for(int i = 0; i < hashValue.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref hashValue[i]);
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
