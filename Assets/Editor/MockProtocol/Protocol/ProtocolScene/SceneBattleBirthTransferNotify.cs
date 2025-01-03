using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneBattleBirthTransferNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508917;
		public UInt32 Sequence;

		public UInt32 battleID;

		public UInt64 playerID;

		public UInt32 birthPosX;

		public UInt32 birthPosY;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint64(buffer, ref pos_, playerID);
			BaseDLL.encode_uint32(buffer, ref pos_, birthPosX);
			BaseDLL.encode_uint32(buffer, ref pos_, birthPosY);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref birthPosX);
			BaseDLL.decode_uint32(buffer, ref pos_, ref birthPosY);
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
