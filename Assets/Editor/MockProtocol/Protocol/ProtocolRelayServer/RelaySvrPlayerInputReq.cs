using System;
using System.Text;

namespace Mock.Protocol
{

	public class RelaySvrPlayerInputReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300005;
		public UInt32 Sequence;

		public UInt64 session;

		public byte seat;

		public UInt64 roleid;

		public _inputData input = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, session);
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			BaseDLL.encode_uint64(buffer, ref pos_, roleid);
			input.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref session);
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleid);
			input.decode(buffer, ref pos_);
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
