using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneFeedPetRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 502208;
		public UInt32 Sequence;

		public UInt32 result;

		public byte feedType;

		public byte isCritical;

		public UInt32 value;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, feedType);
			BaseDLL.encode_int8(buffer, ref pos_, isCritical);
			BaseDLL.encode_uint32(buffer, ref pos_, value);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref feedType);
			BaseDLL.decode_int8(buffer, ref pos_, ref isCritical);
			BaseDLL.decode_uint32(buffer, ref pos_, ref value);
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
