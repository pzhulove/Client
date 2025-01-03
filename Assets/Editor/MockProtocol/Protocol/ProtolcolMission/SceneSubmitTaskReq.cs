using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneSubmitTaskReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501104;
		public UInt32 Sequence;

		public byte submitType;

		public UInt32 npcID;

		public UInt32 taskID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, submitType);
			BaseDLL.encode_uint32(buffer, ref pos_, npcID);
			BaseDLL.encode_uint32(buffer, ref pos_, taskID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref submitType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref npcID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
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
