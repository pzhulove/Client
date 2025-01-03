using System;
using System.Text;

namespace Mock.Protocol
{

	public class GateReconnectGameReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300311;
		public UInt32 Sequence;

		public UInt32 accid;

		public UInt64 roleId;

		public UInt32 sequence;

		public byte[] session = new byte[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, sequence);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)session.Length);
			for(int i = 0; i < session.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, session[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sequence);
			UInt16 sessionCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref sessionCnt);
			session = new byte[sessionCnt];
			for(int i = 0; i < session.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref session[i]);
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
