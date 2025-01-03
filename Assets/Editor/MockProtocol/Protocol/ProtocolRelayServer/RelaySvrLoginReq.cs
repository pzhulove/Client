using System;
using System.Text;

namespace Mock.Protocol
{

	public class RelaySvrLoginReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300001;
		public UInt32 Sequence;

		public byte seat;

		public UInt32 accid;

		public UInt64 roleid;

		public UInt64 session;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			BaseDLL.encode_uint64(buffer, ref pos_, roleid);
			BaseDLL.encode_uint64(buffer, ref pos_, session);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref session);
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
