using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneUseItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500901;
		public UInt32 Sequence;

		public UInt64 uid;

		public byte useAll;

		public UInt32 param1;

		public UInt32 param2;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_int8(buffer, ref pos_, useAll);
			BaseDLL.encode_uint32(buffer, ref pos_, param1);
			BaseDLL.encode_uint32(buffer, ref pos_, param2);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_int8(buffer, ref pos_, ref useAll);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
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
