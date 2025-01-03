using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneNotifyTaskStatusRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501109;
		public UInt32 Sequence;

		public UInt32 taskID;

		public byte status;

		public UInt32 finTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskID);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, finTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskID);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref finTime);
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
