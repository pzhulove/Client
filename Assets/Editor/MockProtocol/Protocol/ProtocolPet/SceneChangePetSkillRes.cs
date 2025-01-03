using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneChangePetSkillRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 502212;
		public UInt32 Sequence;

		public UInt32 result;

		public UInt64 petId;

		public byte skillIndex;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint64(buffer, ref pos_, petId);
			BaseDLL.encode_int8(buffer, ref pos_, skillIndex);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint64(buffer, ref pos_, ref petId);
			BaseDLL.decode_int8(buffer, ref pos_, ref skillIndex);
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
